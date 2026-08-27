# Authentication, Key Generation, and Key Storage

## Password authentication

Handled entirely by [`UserAuthController`](../SecureNotesWebApi/Controllers/UserAuthController.cs) using
[Isopoh.Cryptography.Argon2](https://www.nuget.org/packages/Isopoh.Cryptography.Argon2):

**Register** (`POST /api/userauth/register`):
1. Reject if `Username` already exists (`409 Conflict`).
2. Generate a random 16-byte salt (`RandomNumberGenerator.GetBytes(16)`).
3. `Argon2.Hash(new Argon2Config { Salt = salt, Password = UTF8(password) })` — hashed with the library's default
   Argon2 parameters (type/memory/iterations/parallelism are not overridden in `Argon2Config`).
4. Store the base64 salt and the resulting Argon2 hash string (which itself embeds the algorithm parameters, in
   standard `$argon2...$` encoded form) in the `UserAuths` row, alongside the RSA public key the client sent.

**Login** (`POST /api/userauth/login`):
1. Look up the user by username.
2. Re-run Argon2 with the stored salt against the submitted password and `Argon2.Verify(storedHash, config)`.
3. `200 OK` on match, `401 Unauthorized` for both "no such user" and "wrong password" (same message either way, which
   is good practice — it avoids revealing whether a username exists via this endpoint specifically, though
   `is_registered` and `get_public_key` both leak that same information anyway).

There's no session, cookie, or token issued afterward (`// TODO: Implement JWT` in the controller) — see
[security-weaknesses.md](security-weaknesses.md).

## RSA key generation

Two distinct code paths exist in [`AuthViewModel`](../SecureNotes/ViewModels/AuthViewModel.cs); **only `RegisterCng`
is wired to the Register button.**

### Active path: `RegisterCng()` — CNG + Windows key storage provider

```csharp
CngKeyCreationParameters keyParams = new CngKeyCreationParameters
{
    ExportPolicy = CngExportPolicies.None,                                  // private key cannot be exported
    KeyCreationOptions = CngKeyCreationOptions.OverwriteExistingKey,        // TODO: for testing only
    KeyUsage = CngKeyUsages.Signing | CngKeyUsages.Decryption
};
keyParams.Parameters.Add(new CngProperty("Length", BitConverter.GetBytes(3072), CngPropertyOptions.None));
CngKey cng = CngKey.Create(CngAlgorithm.Rsa, $"SecureNotes-PK-{UsernameText}", keyParams);
using RSA rsa = new RSACng(cng);

X509Store store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
store.Open(OpenFlags.ReadWrite);
CertificateRequest certRequest = new CertificateRequest(
    $"CN=SecureNotes-{UsernameText}", rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pss);
X509Certificate2 cert = certRequest.CreateSelfSigned(DateTimeOffset.UtcNow, DateTimeOffset.UtcNow.AddYears(10));
store.Add(cert);
store.Close();

UserAuth user = new UserAuth(UsernameText, PasswordText, publicKey: rsa.ExportSubjectPublicKeyInfoPem());
await _http.Register(user);
```

Step by step:
1. **`CngKey.Create`** asks the OS's default Key Storage Provider (KSP) — Microsoft Software KSP on a machine without
   a TPM/smartcard configured, or a TPM-backed provider if one is set up — to generate a 3072-bit RSA key pair named
   `SecureNotes-PK-{username}`, with `ExportPolicy = None`. That flag is what makes the private key **non-exportable**:
   the KSP will hand out signatures/decryption results computed with the key, but will refuse to ever release the raw
   private key material, even to the SecureNotes process itself.
2. **`RSACng(cng)`** wraps that CNG key handle as a `.NET` `RSA` object so the rest of the code (`CertificateRequest`,
   later `cert.GetRSAPrivateKey()`) can use it through the normal `System.Security.Cryptography.RSA` API without ever
   touching raw key bytes.
3. **`CertificateRequest.CreateSelfSigned`** builds a self-signed X.509 certificate binding the subject name
   `CN=SecureNotes-{username}` to that key, valid for 10 years. There is no certificate authority — every user's
   certificate is self-signed, so the certificate itself doesn't prove identity to anyone; it's really just a
   convenient, OS-managed *container* that lets the app find "the RSA key for user X" later via `X509Store` lookups
   without having to manage raw key files. (A `// TODO: later implement certificate authority to sign certificates`
   comment in the code acknowledges this.)
4. **`store.Add(cert)`** persists the certificate (with its association to the KSP-backed private key) into the
   current user's personal certificate store (`Cert:\CurrentUser\My`, a.k.a. `StoreName.My` /
   `StoreLocation.CurrentUser`).
5. The RSA **public** key (`rsa.ExportSubjectPublicKeyInfoPem()`) is sent to the API and stored in `UserAuth.PublicKey`
   — this is the only copy of key material that ever leaves the machine.

`KeyCreationOptions.OverwriteExistingKey` means re-registering the same username on the same machine silently
replaces the previous key pair — flagged in the code itself as a testing convenience, not something safe for a real
multi-run deployment (a user who re-registers loses access to anything encrypted under their old key).

### Legacy path: `Register()` — file-exported PEM key (unused)

Present but not called by any command. Generates an in-memory `RSA.Create(2048)` key pair, uploads the public key the
same way, then writes the **exportable** PKCS8 private key PEM to a `.txt` file the user picks via a save dialog. This
predates the CNG-based flow and represents "manage your own private key file" key custody, which the CNG approach
replaced.

## Where keys actually live

| Key material | Where it lives | How it's reached |
|---|---|---|
| RSA private key (messages/secrets) | Windows CNG Key Storage Provider, non-exportable, named `SecureNotes-PK-{username}` | Never accessed directly; always through the certificate that references it |
| Self-signed certificate wrapping that key | Windows certificate store: `Cert:\CurrentUser\My`, subject `CN=SecureNotes-{username}` | `X509Store(StoreName.My, StoreLocation.CurrentUser)` → `.Certificates.Find(X509FindType.FindBySubjectName, "SecureNotes-{username}", ...)` → `cert.GetRSAPrivateKey()` / `cert.GetRSAPublicKey()` |
| RSA public key (for others to encrypt/verify to you) | PostgreSQL, `UserAuths.PublicKey`, as SubjectPublicKeyInfo PEM | `GET /api/userauth/get_public_key/{username}` |
| Account password | PostgreSQL, `UserAuths.Password`, as an Argon2 hash string (parameters embedded in the hash) + separate base64 `Salt` column | Never leaves the server in hashed form except as an unintended side effect of the register response (see weaknesses doc) |
| Local-file-encryption AES key/IV (Flow 1) | Process memory only (`EncryptDecryptService.AesAlg`, static field), optionally exported to a plaintext `key,iv` text file by the user | `HomeViewModel.ExportKey`/`ChangeKey` commands |
| Per-message / per-secret AES-GCM key (Flows 2 & 3) | Never persisted on its own — generated fresh, immediately wrapped with RSA, and only the RSA-encrypted form (`Payload.Key` / `Secret.CiphertextKey`) is stored/sent | N/A |

Both certificate lookups used across the app (`SecretsViewModel`, `MessageViewModel`) key off the exact same naming
convention — `SecureNotes-{username}` for the certificate subject name — which is duplicated as a literal string in
three places (`AuthViewModel`, `SecretsViewModel.KEY_NAME_PREFIX`, `MessageViewModel`,
`EncryptDecryptService.SignatureCng`) rather than defined once and shared.

Because the private key is generated and stored per-Windows-account on a specific machine (non-exportable by design),
a user's identity is effectively tied to that one Windows profile: there is no way to move to another machine and
recover the same private key, and reinstalling Windows or wiping the certificate store permanently strands anything
encrypted to that user (their own secrets, and any messages sent to them that they haven't already decrypted).

# Security Weaknesses and Suggested Improvements

This is a training project, and it reads like one — the cryptographic core (AES-GCM + RSA-OAEP + RSA-PSS,
non-exportable CNG keys) is solid and well thought out, but the pieces around it (API auth, transport of secrets,
error handling) are not. Findings below are grouped by severity/theme, each with a concrete fix.

## No API authentication or authorization

Every route in [`UserAuthController`](../SecureNotesWebApi/Controllers/UserAuthController.cs) and
[`PayloadController`](../SecureNotesWebApi/Controllers/PayloadController.cs) is anonymous — `Login` succeeding
doesn't issue any credential the client presents on later calls. Concretely:

- **Anyone can list every registered user, including password hashes and salts** (`GET /userauth/get_users`) — the
  full `UserAuth` row is returned, not a DTO limited to `Username`/`PublicKey`.
- **Anyone can read anyone's inbox** (`GET /payload/received_messages/{username}`) just by knowing (or guessing) a
  username — this doesn't leak plaintext (payloads are still encrypted), but it does leak *who is messaging whom,
  when, and file names* (`Format`), and lets an outsider harvest ciphertext for later replay (see below).
- **Anyone can POST a payload claiming to be any sender to any recipient** (`POST /payload/send`) — `Sender` is a
  free-text field the caller sets. This is caught for genuine content-tampering by the signature check on download,
  but nothing stops spamming a recipient's inbox with garbage `Payload` rows.

**Fix:** issue a token (the code's own `// TODO: Implement JWT` comment) on login, require it via
`[Authorize]`/ASP.NET Core authentication middleware on every route except `register`/`login`/`is_registered`, and
derive `Sender` from the authenticated identity server-side instead of trusting the request body.

## Signature doesn't cover `Sender`, `Recipient`, `UUID`, or `Timestamp`

`SignatureCng`/`Verify` (see [encryption-and-data-flow.md](encryption-and-data-flow.md)) sign only
`Ciphertext ‖ Key ‖ IV ‖ Tag`. Combined with the lack of API auth above, this means a previously observed `Payload`
(visible to anyone via the unauthenticated `received_messages` endpoint) can be re-POSTed with a **different**
`UUID`, `Sender`, `Recipient`, or `Timestamp` and will still pass signature verification on the receiving end,
because none of those fields are part of what was signed. An attacker can't decrypt the replayed content (the AES key
is still RSA-wrapped to the *original* recipient), but they can make a stale or out-of-context payload appear to be a
fresh, legitimately signed message from a given sender.

**Fix:** include `UUID`, `Sender`, `Recipient`, and `Timestamp` in the signed byte string (with explicit length
prefixes or a fixed delimiter, not naive concatenation — see next item), and have the server reject payloads whose
`Timestamp` is outside a small tolerance window.

## Naive byte concatenation for signing has no length framing

`Ciphertext ‖ Key ‖ IV ‖ Tag` are concatenated with plain `Array.Copy` and no delimiters or length prefixes. It
happens to be unambiguous today only because `IV` (12 bytes) and `Tag` (16 bytes) are fixed-length and `Key`'s length
is implicitly fixed by the RSA modulus size. This is fragile: if a future change makes any field variable-length
without also changing the framing, two different `(ciphertext, key, iv, tag)` tuples could concatenate to the same
byte string and validate against the same signature.

**Fix:** use a canonical, length-prefixed encoding (e.g. 4-byte big-endian length + bytes, per field) before signing,
or sign a hash of a well-defined structure (e.g. concatenate `SHA-256(field)` for each field, or just sign a
canonical JSON/CBOR encoding with strict, versioned rules — the project already tried and abandoned a JSON approach
once, see the `PayloadJcs` history in [encryption-and-data-flow.md](encryption-and-data-flow.md), which is worth
revisiting with a *stricter* canonicalization this time rather than relying on default serializer output).

## Password hash leaked back in the registration response

```csharp
return Ok($"Successfully registered user. Hashed password {userAuth.Password}");
```
The Argon2 hash (with its embedded parameters and salt-derived encoding) is echoed straight back in the HTTP
response body. It doesn't defeat Argon2 by itself, but there's no reason to send it back at all, and it risks being
logged (browser history, proxy logs, API gateway logs) somewhere the raw hash shouldn't need to travel.

**Fix:** return a plain success message with no hash, or a minimal `{ "username": "..." }` acknowledgment.

## Static, reused AES key/IV for local file encryption (Flow 1)

`EncryptDecryptService.AesAlg` is a single static `Aes` instance created once per process and reused for **every**
`AesEncryptBytes` call in `EncryptViewModel`, in CBC mode. Reusing the same key+IV pair to encrypt more than one
message under CBC is a well-known integrity/confidentiality weakener: two plaintexts sharing a prefix produce
identical leading ciphertext blocks, and the scheme provides no authentication (a corrupted/tampered ciphertext file
just silently decrypts to garbage or throws a padding-related `CryptographicException`, rather than being detected as
tampered).

**Fix:** generate a fresh random IV per encryption (standard practice even when the key is reused), prepend it to the
ciphertext so decryption doesn't need an out-of-band IV, and prefer AES-GCM (already used elsewhere in the codebase)
for its built-in authentication tag instead of unauthenticated CBC.

## Plaintext password kept in memory for the whole session

`AuthViewModel.Login()` builds `new UserAuth(UsernameText, PasswordText)` and passes that same object all the way
down through every subsequent view model (`HomeViewModel`, `MessageViewModel`, `SecretsViewModel`, ...). The
plaintext password the user typed lives in a regular (non-pinned, GC-managed, swappable-to-disk) `string` for the
entire session, even though nothing past the initial login call actually needs it again.

**Fix:** drop the password from the `UserAuth` instance immediately after a successful login (or use a `SecureString`
/ clear the backing char array), and stop threading it through view models that don't use it.

## Connection string (with password) committed to source control

[`appsettings.json`](../SecureNotesWebApi/appsettings.json) hardcodes `Username=postgres; Password=1234`. Low risk as
committed (points at `localhost`), but it's a bad habit that becomes a real leak the moment this ever points at a
shared or cloud database.

**Fix:** move secrets to user secrets (`dotnet user-secrets`) for local dev and environment variables / a secret
manager (Azure Key Vault, AWS Secrets Manager, etc.) for any real deployment; keep only non-secret defaults in
`appsettings.json`.

## No transport-level protections beyond the SDK defaults

- The client hardcodes `https://localhost:7042` with no TLS certificate pinning/validation beyond the OS trust
  store — fine for local dev against a self-signed dev cert, but would need real cert validation before pointing at
  any non-localhost host.
- `HttpService.client` is a single static `HttpClient` — correct .NET practice (avoids socket exhaustion), but it
  never sets timeouts, retry policies, or `Accept`/content-type negotiation headers explicitly, which is why
  `get_public_key` relies on ASP.NET Core's default string-vs-JSON formatter behavior rather than an explicit
  content type both sides agree on.

## Self-signed certificates provide no identity guarantee

Every user's X.509 certificate (`CN=SecureNotes-{username}`) is self-signed
(`CertificateRequest.CreateSelfSigned`), with no certificate authority in the loop (acknowledged in-code:
`// TODO: later implement certificate authority to sign certificates`). The certificate is really just a convenient
local *handle* to a CNG key, not proof of identity — the actual trust anchor in this system is "the public key stored
under this username in the database," which anyone who can call the unauthenticated `register` endpoint can set.
Combined with the missing-auth issue above, nothing currently stops an attacker from registering a *second* account
using a victim's chosen username variant, or (if `register` isn't locked down) overwriting expectations about who
owns a given public key.

**Fix:** once basic authentication/authorization is in place, this is less urgent, but longer-term a lightweight
internal CA (even just the server signing user certificates on registration) would let a client verify a
certificate's provenance instead of trusting "whatever the API currently says this username's key is."

## `KeyCreationOptions.OverwriteExistingKey` is a footgun outside testing

`RegisterCng()` always overwrites any existing CNG key named `SecureNotes-PK-{username}` on the local machine (flagged
in-code as `// TODO: Overwrite key for testing`). Re-running registration for the same username — accidentally or
via the unauthenticated `register` endpoint from another session — silently destroys the old, non-exportable private
key, permanently stranding anything encrypted to the old public key (the user's own secrets file, and any messages
sent to them that hadn't yet been decrypted).

**Fix:** default to failing if a key with that name already exists; only overwrite behind an explicit, user-confirmed
"reset my keys" action, and warn about the consequences (already-encrypted data becomes unrecoverable).

## Minor / hygiene

- `is_registered/{username}` returns `200 OK` for "available" and `409 Conflict` for "taken" — functionally fine, but
  the naming (`CanRegister` returning success when the user does *not* exist) reads backwards and is easy to
  misuse when this API grows.
- `FileService`'s catch blocks re-throw `new FileNotFoundException()`/`new IOException()`/etc. instead of rethrowing
  or wrapping the original exception, discarding the original stack trace and inner exception — makes production
  debugging harder for no benefit.
- No automated tests exist for any cryptographic routine (`EncryptDecryptService`) or controller — given how easy
  it is to subtly break a signing/verification scheme (as the `PayloadJcs` → concatenated-bytes migration already
  demonstrates), unit tests around "does `Verify` correctly reject a tampered field" for each field of `Payload`
  would catch regressions like the one described above before they ship.

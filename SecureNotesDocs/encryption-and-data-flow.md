# Hybrid Encryption, Signing, and Data Flow

All cryptographic primitives live in
[`EncryptDecryptService`](../SecureNotes/Services/EncryptDecryptService.cs) on the client. The API never sees
plaintext, an AES key, or a private key — it only ever stores/relays ciphertext, wrapped keys, and public data.

The app actually implements **three independent crypto flows** that share the same primitives but differ in what gets
encrypted, whose keys are used, and where the result ends up. They're easy to conflate, so this doc treats them
separately, then covers the primitives each one relies on.

## The three flows

### 1. Local file encryption (`EncryptViewModel` / `DecryptViewModel`) — symmetric only

The simplest and least secure flow. No RSA, no per-operation key, no signing.

- `EncryptDecryptService` holds a single **static** `Aes` instance (`AesAlg`), created once with `Aes.Create()`
  (random key + IV at process start, CBC mode, PKCS7 padding — all .NET defaults).
- `EncryptViewModel.Encrypt()`: read file bytes → `AesEncryptBytes` (encrypt under the static key/IV) → write ciphertext
  to a new file.
- `DecryptViewModel.Decrypt()`: read ciphertext file → `AesDecryptBytes` (decrypt under the *same* static key/IV) →
  write plaintext to a new file.
- `HomeViewModel.ExportUserKey()` / `ImportUserKey()` let the key+IV be saved to / loaded from a `key,iv` (base64,
  comma-separated) text file, via `EncryptDecryptService.GetKeyIV()` / `ChangeAesKey()`, so a decrypting party needs a
  copy of that file.

This flow never touches the network, the database, or the Windows certificate store — it's plain shared-secret AES.

### 2. Personal encrypted note (`SecretsViewModel`) — hybrid, self-addressed

One note, one file on disk, encrypted to the current user's *own* key so only they (on a machine with their
certificate) can read it back.

**Save:**
1. UTF-8 encode the note text.
2. `AesGcmEncrypt(plaintext)` → fresh random 256-bit key + 96-bit IV, AES-GCM encrypt → `(ciphertext, key, iv, tag)`.
3. Look up the current user's own certificate in the Windows cert store (`CN=SecureNotes-{username}`), pull its RSA
   **public** key, and `RsaEncryptBytes(key, publicKeyPem)` (RSA-OAEP/SHA-256) to wrap the AES key.
4. Base64-encode `ciphertextKey`, `iv`, `tag`, `ciphertext` into a `Secret` object, JSON-serialize it, write to a
   user-chosen file.

**Read:** reverse — parse the JSON, base64-decode each field, pull the matching certificate's **private** key from
the cert store, `RsaDecryptBytes` to recover the AES key, then `AesGcmDecrypt` to recover the note text.

There is no signature here — a note only needs confidentiality and integrity (AES-GCM's tag), not authenticity,
since sender and verifier are the same person.

### 3. Sending a file to another user (`MessageViewModel`) — hybrid + signed

The full scheme: confidentiality via AES-GCM, key exchange via RSA-OAEP, authenticity via RSA-PSS signatures, key
custody via the Windows certificate store. This is the flow the rest of this document focuses on.

## Data flow: conceived → sent → received → decrypted

```
 SENDER (WPF)                                    API / DB                        RECIPIENT (WPF)
 ─────────────                                   ─────────                       ────────────────
 1. Pick file + recipient
 2. AES-GCM encrypt file
      → ciphertext, key, iv, tag
 3. Fetch recipient's public key  ───GET /userauth/get_public_key/{recipient}──▶
                                  ◀──────────────── PEM public key ─────────────
 4. RSA-OAEP encrypt the AES key
      with recipient's public key → ciphertextKey
 5. Concatenate ciphertext‖ciphertextKey‖iv‖tag
 6. Sign the concatenation with
      sender's CNG private key    → signature
 7. Build Payload{uuid, sender,
      recipient, ciphertext,
      ciphertextKey, iv, tag,
      format, timestamp, signature}
 8. POST /payload/send  ───────────────────────▶  INSERT INTO "Messages"
                                                          │
                                                          │  (recipient later polls)
                                                          ▼
                                  ◀────GET /payload/received_messages/{me}─────  9. Load inbox
                                        [Payload, Payload, ...]
                                                                                 10. Pick a message
                                                                                 11. Fetch sender's public key
                                  ◀───GET /userauth/get_public_key/{sender}────
                                        PEM public key
                                                                                 12. Re-concatenate ciphertext‖
                                                                                     ciphertextKey‖iv‖tag and
                                                                                     RSA-PSS verify against
                                                                                     Signature, using sender's
                                                                                     public key
                                                                                 13. If valid: look up own
                                                                                     certificate in cert store,
                                                                                     get RSA private key,
                                                                                     RSA-OAEP decrypt
                                                                                     ciphertextKey → AES key
                                                                                 14. AES-GCM decrypt ciphertext
                                                                                     with (key, iv, tag)
                                                                                 15. Save plaintext to disk
                                                                                     under original filename
```

Concretely, in code:

**Conceive & send** — [`MessageViewModel.CreateSendPayload()`](../SecureNotes/ViewModels/MessageViewModel.cs):
```csharp
(byte[] ciphertext, byte[] key, byte[] iv, byte[] tag) = _encryptDecryptService.AesGcmEncrypt(data);
byte[] ciphertextKey = _encryptDecryptService.RsaEncryptBytes(key, Recipient.PublicKey);
// concatenate ciphertext | ciphertextKey | iv | tag into `toSign`
byte[] signature = _encryptDecryptService.SignatureCng(toSign, _currentUser.Username);
// build Payload, JSON-serialize, POST to HttpService.API_SEND_PAYLOAD
```

**Receive & decrypt** — [`MessageViewModel._DownloadPayload()`](../SecureNotes/ViewModels/MessageViewModel.cs):
```csharp
string publicKeyPem = await _http.GetPublicKey(SelectedMessage.Sender);
if (!_encryptDecryptService.Verify(SelectedMessage, publicKeyPem)) { /* refuse to decrypt */ }
// pull own cert from X509Store, get RSA private key
byte[] aesGcmKey = _encryptDecryptService.RsaDecryptBytes(ciphertextKey, rsa);
byte[] plaintext = _encryptDecryptService.AesGcmDecrypt(ciphertext, aesGcmKey, iv, tag);
```

**Storage in transit / at rest**: the API layer (`PayloadController.Send` / `GetAllMessages`) does no crypto at all —
it just persists and returns the `Payload` row as-is (see [database.md](database.md)). All confidentiality,
integrity, and authenticity guarantees come entirely from the client-side steps above; the server is trusted only to
store and forward bytes.

## The primitives (`EncryptDecryptService`)

| Method | Algorithm | Used by |
|---|---|---|
| `AesEncryptBytes` / `AesDecryptBytes` | AES (CBC, PKCS7, static shared `Aes` instance) | Flow 1 (local file encrypt/decrypt) |
| `AesGcmEncrypt(plaintext)` → `(ciphertext, key, iv, tag)` | AES-GCM, **fresh** random 256-bit key + 96-bit IV per call, 128-bit tag | Flows 2 and 3 |
| `AesGcmDecrypt(ciphertext, key, iv, tag)` | AES-GCM decrypt+verify | Flows 2 and 3 |
| `RsaEncryptBytes(bytes, publicKeyPem)` | RSA-OAEP, SHA-256 | Wrapping the AES-GCM key to a recipient's (or your own) public key |
| `RsaDecryptBytes(bytes, privateKeyPem)` / `RsaDecryptBytes(bytes, RSA key)` | RSA-OAEP, SHA-256 | Unwrapping the AES-GCM key with a PEM key or an `RSA` handle pulled from a certificate |
| `Signature(PayloadJcs, privateKeyPem)` | RSA-PSS, SHA-256, over the UTF-8 JSON serialization of a `PayloadJcs` | **Legacy/unused.** Superseded by `SignatureCng` + raw byte concatenation (see below). |
| `SignatureCng(bytes, username)` | RSA-PSS, SHA-256, over caller-supplied raw bytes, using the private key from the `SecureNotes-{username}` certificate in the Windows cert store | Flow 3, signing |
| `Verify(Payload, publicKeyPem)` | RSA-PSS, SHA-256 | Flow 3, verification |

### Why signing changed from JSON to concatenated bytes

An earlier version (`PayloadJcs` + `Signature(PayloadJcs, ...)`) signed the UTF-8 bytes of
`JsonSerializer.Serialize(payloadJcs)` — i.e., a JSON Canonicalization Scheme (JCS)-flavored approach. That's fragile:
`System.Text.Json`'s default serialization isn't guaranteed to be byte-stable across property reordering, .NET
versions, or culture settings, so a signature produced on one machine could fail to verify on another even though the
*meaning* of the payload hadn't changed.

The current scheme (`SignatureCng` / `Verify`) instead concatenates the raw bytes of exactly four fields —
`Ciphertext ‖ Key ‖ IV ‖ Tag` — in a fixed order, and signs/verifies that byte string directly. This removes the
serialization instability, at the cost of **not** covering `UUID`, `Sender`, `Recipient`, `Format`, or `Timestamp` —
see [security-weaknesses.md](security-weaknesses.md) for the implications.

### Verify — exact byte layout

```csharp
byte[] toVerify = ciphertext ‖ ciphertextKey ‖ iv ‖ tag;   // Array.Copy, in this order, no separators/lengths
rsa.VerifyData(toVerify, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pss);
```

Because there are no length-prefixes or delimiters between the four fields, this concatenation is only unambiguous
because AES-GCM's `iv` (12 bytes) and `tag` (16 bytes) are fixed-length; `ciphertext` and `ciphertextKey` can vary in
length but their boundary is implicitly fixed by RSA's fixed output size (`ciphertextKey` is always exactly the RSA
modulus size, 256 or 384 bytes depending on key size) and by AES-GCM producing ciphertext the same length as the
plaintext.

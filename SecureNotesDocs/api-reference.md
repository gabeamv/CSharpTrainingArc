# SecureNotesWebApi Reference

Base URL (Development, per `SecureNotes/Services/HttpService.cs`): `https://localhost:7042/api`

The API has two controllers: `UserAuthController` (`/api/userauth`) and `PayloadController` (`/api/payload`). Swagger
UI is enabled in Development (`app.UseSwagger()/UseSwaggerUI()` in [Program.cs](../SecureNotesWebApi/Program.cs)).

There is **no authentication middleware** — every route below is callable by anyone who can reach the host. See
[security-weaknesses.md](security-weaknesses.md).

## Models / resources

### `UserAuth`

Table `UserAuths`, keyed on `Username`. Represents a registered account and doubles as the request/response DTO for
auth endpoints (see [SecureNotesWebApi/Models/UserAuth.cs](../SecureNotesWebApi/Models/UserAuth.cs)).

| Field | Type | Notes |
|---|---|---|
| `Username` | `string` (required) | Primary key. |
| `Password` | `string` (required) | Plaintext on the wire for register/login; stored as an Argon2 hash. |
| `PublicKey` | `string?` | SubjectPublicKeyInfo PEM of the user's RSA public key. |
| `Salt` | `string?` | Base64 16-byte salt used for the Argon2 hash. |

There's a separate, near-identical client-side model at
[SecureNotes/Models/UserAuth.cs](../SecureNotes/Models/UserAuth.cs) — same shape, but fields aren't `required` and it
has a constructor for convenience.

### `Payload`

Table `Messages`, keyed on `UUID`. Represents one encrypted file sent from one user to another (see
[SecureNotesWebApi/Models/Payload.cs](../SecureNotesWebApi/Models/Payload.cs)).

| Field | Type | Notes |
|---|---|---|
| `UUID` | `string` (required) | Primary key, client-generated `Guid.NewGuid()`. |
| `Sender` | `string` (required) | Sender's username (client-asserted, not verified against a session). |
| `Recipient` | `string` (required) | Recipient's username; used to filter `received_messages`. |
| `Ciphertext` | `string` (required) | Base64 AES-GCM ciphertext of the file contents. |
| `Key` | `string` (required) | Base64 RSA-OAEP(SHA-256)-encrypted AES-GCM key, encrypted to the recipient's public key. |
| `IV` | `string` (required) | Base64 12-byte AES-GCM nonce. |
| `Tag` | `string` (required) | Base64 16-byte AES-GCM authentication tag. |
| `Format` | `string` (required) | Original file name (used to name the file on decrypt). |
| `Timestamp` | `DateTime` (required) | UTC send time. |
| `Signature` | `string` (required) | Base64 RSASSA-PSS(SHA-256) signature over `Ciphertext‖Key‖IV‖Tag` (raw bytes, concatenated — see [encryption-and-data-flow.md](encryption-and-data-flow.md)). |

The client also has a `PayloadJcs` DTO ([SecureNotes/Dtos/PayloadJcs.cs](../SecureNotes/Dtos/PayloadJcs.cs)) from an
earlier design that canonicalized and signed the whole JSON object. It's no longer used by any view model — the
signature scheme now signs only the four concatenated byte fields above (see the commit that changed this).

## Endpoints

### `GET /api/userauth/get`

Health check.

**Response `200 OK`**
```
"API is working"
```

### `GET /api/userauth/get_users`

Returns every registered user (including password hash and salt — see [security-weaknesses.md](security-weaknesses.md)).
Used by `MessageViewModel` to populate the recipient picker.

**Response `200 OK`**
```json
[
  {
    "username": "gabeamv",
    "password": "$argon2i$v=19$m=65536,t=3,p=1$...",
    "publicKey": "-----BEGIN PUBLIC KEY-----\nMIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8A...\n-----END PUBLIC KEY-----\n",
    "salt": "z1c8m3+3s7...=="
  }
]
```

### `POST /api/userauth/register`

Creates a new user. The client sends the plaintext password and the PEM public key it just generated; the server
salts and Argon2-hashes the password before persisting.

**Request body**
```json
{
  "username": "gabeamv",
  "password": "Test123",
  "publicKey": "-----BEGIN PUBLIC KEY-----\nMIIBoDANBgkqhkiG9w0BAQEFAAOCAY0A...\n-----END PUBLIC KEY-----\n"
}
```

**Response `200 OK`** (note: echoes the hash back — see weaknesses doc)
```
"Successfully registered user. Hashed password $argon2i$v=19$m=65536,t=3,p=1$..."
```

**Response `409 Conflict`** — username already exists
```
"There is already a user."
```

### `POST /api/userauth/login`

Verifies a password against the stored Argon2 hash for that username.

**Request body**
```json
{
  "username": "gabeamv",
  "password": "Test123"
}
```

**Response `200 OK`**
```
"Successfully logged in user."
```

**Response `401 Unauthorized`** — unknown username, or a real user but the request skips straight past because the
password check failed (both cases fall through to the same response)
```
"There is no such user."
```

There's a `// TODO: Implement JWT` in the controller — no token, cookie, or session is issued. The client just treats
the `200` as "logged in" and continues to use unauthenticated requests for everything else.

### `GET /api/userauth/get_public_key/{username}`

Returns the requested user's PEM public key as the raw response body (the action returns a bare `string`, so ASP.NET
Core's plain-text formatter is used rather than a JSON string). Used both to encrypt a message *to* someone and to
verify a signature claimed to be *from* someone.

**Response `200 OK`**
```
-----BEGIN PUBLIC KEY-----
MIIBoDANBgkqhkiG9w0BAQEFAAOCAY0A...
-----END PUBLIC KEY-----
```

**Response `404 Not Found`**
```
"No user found."
```

### `GET /api/userauth/is_registered/{username}`

Availability check used before generating a new key pair during registration. Despite the name, `200 OK` means the
username is **free** and `409 Conflict` means it's **taken**.

**Response `200 OK`** — no body, username is available
**Response `409 Conflict`** — no body, username is taken

### `POST /api/payload/send`

Stores an encrypted payload for later retrieval by its recipient. No validation beyond EF's `[Key]` constraint on
`UUID` (a duplicate UUID would surface as an unhandled `DbUpdateException` → `500`).

**Request body**
```json
{
  "uuid": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "sender": "gabeamv",
  "recipient": "alice",
  "ciphertext": "8gk3n2Q0m1c...==",
  "key": "Zt2f9Kx0...==",
  "iv": "u8y3kd92md1x==",
  "tag": "aXVxYnZjeHo9==",
  "format": "notes.txt",
  "timestamp": "2026-08-26T18:04:12.0000000Z",
  "signature": "MEUCIQD7Xy...=="
}
```

**Response `200 OK`**
```
"Payload '3fa85f64-5717-4562-b3fc-2c963f66afa6' has been sent."
```

### `GET /api/payload/received_messages/{username}`

Returns every payload whose `Recipient` matches `username`.

**Response `200 OK`**
```json
[
  {
    "uuid": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "sender": "gabeamv",
    "recipient": "alice",
    "ciphertext": "8gk3n2Q0m1c...==",
    "key": "Zt2f9Kx0...==",
    "iv": "u8y3kd92md1x==",
    "tag": "aXVxYnZjeHo9==",
    "format": "notes.txt",
    "timestamp": "2026-08-26T18:04:12Z",
    "signature": "MEUCIQD7Xy...=="
  }
]
```

# Pro C# 10 w/ .NET 6 Training Arc 

Reading the book "Pro C# 10 with .NET 6" by Andrew Troelsen and Philip Japikse. 
This repository contains projects that aim to apply what I learn from the book as well as what
I learn from online sources. The main goal is to implement a big project that combines a lot 
of these concepts I have learned. The main big project is called Encrypted File Drop (later changed to SecureNotes), which 
you can click the link below to know what it is. If I get stuck implementing something for the
main project, or I feel that I don't have a good enough foundation to build a feature for the
main project, I will implement a smaller scale project that addresses the concepts and issues.
I will also implement mini projects, kind of like small exercises, that reinforce what I learn.
These projects may be scaled later in the future the more I learn.

## Mini Projects That Apply What I Read
* [EncryptOrDie (Console Application)](./EncryptOrDie/Documentation/EncryptOrDie.md)
* [CardGames (Console Application)](./CardGames/Documentation/CardGames.md)

# SecureNotes

SecureNotes is a two-part learning project consisting of:

- **[SecureNotes](./SecureNotes)** — a WPF desktop client (MVVM), the user-facing application.
- **[SecureNotesWebApi](./SecureNotesWebApi)** — an ASP.NET Core Web API backend, backed by PostgreSQL via EF Core.

They are deployed and versioned as one project: the WPF app is the only client of the API, and the API exists solely to
support the WPF app's "send an encrypted file to another user" feature. Together they form a small end-to-end
encrypted note/file-sharing system, plus a purely local encryption tool that doesn't touch the network at all.

## Goal and problem being solved

The project is a hands-on exploration of applied cryptography and full desktop-app architecture in C#. It solves (or
attempts to solve) three related problems:

1. **Encrypt arbitrary files locally**, so they're unreadable without a key (`EncryptViewModel` / `DecryptViewModel`).
2. **Store a personal, encrypted note on disk** that only the owning user can decrypt, using key material anchored to
   the OS (`SecretsViewModel`).
3. **Send an encrypted file to another registered user** over HTTP such that the server can relay it without ever
   seeing the plaintext, the recipient can decrypt it with a private key the server never has, and the recipient can
   cryptographically verify who really sent it (`MessageViewModel`, backed by `SecureNotesWebApi`).

In other words: the API is a dumb, semi-trusted relay/directory (usernames, password hashes, public keys, and
ciphertext blobs), and all the interesting cryptography — key generation, encryption, decryption, signing,
verification — happens on the WPF client.

## Documentation map

| Doc | Contents |
|---|---|
| [api-reference.md](./SecureNotesDocs/api-reference.md) | Web API models, routes, request/response examples |
| [frontend-architecture.md](./SecureNotesDocs/frontend-architecture.md) | WPF MVVM structure, views, view models, services, navigation |
| [encryption-and-data-flow.md](./SecureNotesDocs/encryption-and-data-flow.md) | Hybrid RSA+AES-GCM scheme, signing/verification, end-to-end data flow for each feature |
| [authentication-and-keys.md](./SecureNotesDocs/authentication-and-keys.md) | Registration/login, RSA key generation, Windows CNG key storage & certificate store |
| [database.md](./SecureNotesDocs/database.md) | PostgreSQL/EF Core integration, schema, migrations |
| [security-weaknesses.md](./SecureNotesDocs/security-weaknesses.md) | Known weaknesses and suggested improvements |

## Features at a glance

- **Register / Login** — Argon2-hashed passwords in PostgreSQL; registration also generates a non-exportable RSA
  key pair in the Windows key store and wraps it in a self-signed certificate.
- **Encrypt File / Decrypt File** — symmetric AES encryption of an arbitrary file using a single shared, in-memory
  AES key/IV that can be exported to or imported from a text file.
- **Secrets** — a single encrypted note, AES-GCM encrypted with a fresh random key, which is itself RSA-encrypted to
  the current user's own public key (from the certificate store) and saved to disk as JSON.
- **Messages** — pick another registered user, AES-GCM encrypt a file, RSA-encrypt the AES key to *their* public key,
  sign the payload with your own certificate-backed private key, and POST it to the API. The recipient lists their
  messages, verifies the sender's signature using the sender's public key (fetched from the API), then decrypts.

## Solution layout

```
SecureNotes/            WPF client (net8.0-windows)
SecureNotesWebApi/      ASP.NET Core Web API (net8.0)
SecureNotesDocs/        This documentation
```




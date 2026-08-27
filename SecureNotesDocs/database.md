# PostgreSQL Integration

`SecureNotesWebApi` persists all server-side state in PostgreSQL via EF Core (`Npgsql.EntityFrameworkCore.PostgreSQL`
9.0.1). This was added in a later commit ("implementing local postgresql connection to persist data across all
executions of the web api") — earlier iterations presumably ran against `Microsoft.EntityFrameworkCore.InMemory`
(still referenced in [SecureNotesWebApi.csproj](../SecureNotesWebApi/SecureNotesWebApi.csproj), though `Program.cs`
now wires up Npgsql, not the in-memory provider).

## Wiring

[`Program.cs`](../SecureNotesWebApi/Program.cs):
```csharp
builder.Services.AddDbContext<SecureNotesContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("WebApiDatabase"))
);
```

[`appsettings.json`](../SecureNotesWebApi/appsettings.json):
```json
"ConnectionStrings": {
  "WebApiDatabase": "Host=localhost; Port=25565; Database=secure_notes; Username=postgres; Password=1234"
}
```

The connection string — including a plaintext password — is committed directly to source control (there's even a
`// TODO: enter connection string to database.` comment above it, suggesting this was meant to be replaced). It's
scoped to `localhost` on a non-default port (`25565`, the Minecraft default — presumably a local dev container/instance),
so the practical exposure is limited to a developer's own machine, but this pattern (secrets in `appsettings.json`)
doesn't belong in a real deployment — see [security-weaknesses.md](security-weaknesses.md).

## `SecureNotesContext`

[`SecureNotesContext.cs`](../SecureNotesWebApi/Context/SecureNotesContext.cs) is a minimal `DbContext` with two
`DbSet`s and no fluent configuration — the schema is derived entirely from data annotations on the model classes:

```csharp
public DbSet<UserAuth> UserAuths { get; set; }
public DbSet<Payload> Messages { get; set; }
```

## Schema

From the single migration, [`20251124063115_init.cs`](../SecureNotesWebApi/Migrations/20251124063115_init.cs):

### `UserAuths`

| Column | Type | Nullable | Notes |
|---|---|---|---|
| `Username` | `text` | No | Primary key (`[Key]` on the model) |
| `Password` | `text` | No | Argon2 hash |
| `PublicKey` | `text` | Yes | RSA SubjectPublicKeyInfo PEM |
| `Salt` | `text` | Yes | Base64 16-byte salt |

### `Messages` (backs the `Payload` model)

| Column | Type | Nullable | Notes |
|---|---|---|---|
| `UUID` | `text` | No | Primary key |
| `Sender` | `text` | No | |
| `Recipient` | `text` | No | Queried via `WHERE Recipient = @username` in `GetAllMessages` |
| `Ciphertext` | `text` | No | Base64 |
| `Key` | `text` | No | Base64, RSA-wrapped AES key |
| `IV` | `text` | No | Base64 |
| `Tag` | `text` | No | Base64 |
| `Format` | `text` | No | Original filename |
| `Timestamp` | `timestamp with time zone` | No | |
| `Signature` | `text` | No | Base64 |

Both tables use string primary keys (`Username`, `UUID`) rather than surrogate integer/GUID keys with a separate
unique index — reasonable here since both are already naturally unique and are also the lookup key for every query in
the controllers.

## Migrations

Managed with `Microsoft.EntityFrameworkCore.Tools`. There is one migration (`init`), plus the generated
[`SecureNotesContextModelSnapshot.cs`](../SecureNotesWebApi/Migrations/SecureNotesContextModelSnapshot.cs). To apply
it against a fresh database, from `SecureNotesWebApi/`:

```bash
dotnet ef database update
```

(requires the `dotnet-ef` global tool, and a reachable PostgreSQL instance matching the `WebApiDatabase` connection
string).

## Query patterns

Both controllers use simple, parameterized LINQ-to-EF queries (`FirstOrDefaultAsync(u => u.Username == ...)`,
`Where(m => m.Recipient == username).ToListAsync()`) — EF Core parameterizes these automatically, so there's no SQL
injection risk from user-supplied usernames despite them flowing directly from route/body values into the query.

There is no pagination anywhere (`get_users`, `received_messages/{username}` both return the full unfiltered result
set), no indexes beyond the primary keys, and no cascading delete/relationship between `UserAuths` and `Messages` —
`Sender`/`Recipient` are plain string columns, not foreign keys, so deleting a user does not affect their messages
and a `Payload` can reference usernames that don't exist.

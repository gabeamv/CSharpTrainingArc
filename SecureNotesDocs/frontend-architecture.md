# Frontend Architecture (WPF / MVVM)

`SecureNotes` is a WPF app on `.NET 8` using a hand-rolled MVVM setup (no framework like Prism or CommunityToolkit.Mvvm)
and a single-window, "swap the content" navigation model.

## Composition root and navigation

- [`App.xaml`](../SecureNotes/App.xaml) declares `DataTemplate`s that map each view model type to its view:

  ```xml
  <DataTemplate DataType="{x:Type viewModels:AuthViewModel}"><views:AuthView/></DataTemplate>
  <DataTemplate DataType="{x:Type viewModels:HomeViewModel}"><views:HomeView/></DataTemplate>
  <DataTemplate DataType="{x:Type viewModels:EncryptViewModel}"><views:EncryptView/></DataTemplate>
  <DataTemplate DataType="{x:Type viewModels:DecryptViewModel}"><views:DecryptView/></DataTemplate>
  <DataTemplate DataType="{x:Type viewModels:MessageViewModel}"><views:MessageView/></DataTemplate>
  <DataTemplate DataType="{x:Type viewModels:SecretsViewModel}"><views:SecretsView/></DataTemplate>
  ```

  WPF's implicit `DataTemplate` resolution is what actually performs "view lookup" — there's no view-model-first
  navigation framework involved.

- [`MainWindow.xaml`](../SecureNotes/MainWindow.xaml) is just:
  ```xml
  <ContentControl Content="{Binding CurrentViewModel}"/>
  ```
  Whatever object `MainViewModel.CurrentViewModel` holds gets rendered via the matching `DataTemplate` above.

- [`MainViewModel`](../SecureNotes/ViewModels/MainViewModel.cs) is the composition root: it constructs the two
  process-lifetime services (`NavigationService`, `HttpService`) and sets the initial screen to `AuthViewModel`.
  ```csharp
  var navService = new NavigationService(viewModel => CurrentViewModel = viewModel);
  var httpService = new HttpService();
  CurrentViewModel = new AuthViewModel(navService, httpService);
  ```
  There is no DI container — every view model manually `new`s up whichever services it needs beyond what's passed in
  (e.g. every view model does `new FileService()` and `new EncryptDecryptService()` itself).

- [`NavigationService`](../SecureNotes/Services/NavigationService.cs) implements
  [`INavigationService`](../SecureNotes/IServices/INavigationService.cs) as a thin wrapper around an
  `Action<object>` callback supplied by `MainViewModel`. Calling `NavigateTo(viewModel)` just reassigns
  `CurrentViewModel`, so navigation is one-directional with no back stack — "Go Home" buttons work by constructing a
  brand new `HomeViewModel(nav, http, user)` rather than popping to a previous instance.

## View model graph

```
MainViewModel
 └─ AuthViewModel  (Login / Register)
     └─ HomeViewModel (per logged-in user)
         ├─ EncryptViewModel   (local file encryption)
         ├─ DecryptViewModel   (local file decryption)
         ├─ SecretsViewModel   (personal encrypted note)
         └─ MessageViewModel   (send/receive encrypted files via the API)
```

Every screen past `AuthViewModel` is constructed with the same three arguments — `NavigationService`, `HttpService`,
and the current `UserAuth`(username/password/public key) — which is how the "current session" is threaded through the
app instead of a dedicated session/auth service. `HomeViewModel` is the hub all four feature screens navigate back to.

### `AuthViewModel`

Login and registration. Two RSA registration paths exist in the code, but only one is wired up:

- `RegisterCommand` → `RegisterCng()` — the **active** path. Generates a non-exportable RSA key in the Windows key
  storage provider via CNG and wraps it in a self-signed certificate (see
  [authentication-and-keys.md](authentication-and-keys.md)).
- `Register()` (private, unused by any command) — an older flow that generates an in-memory `RSA` key pair and writes
  the PKCS8 private key PEM to a user-chosen `.txt` file via a save dialog. Left in the codebase but dead code.

`LoginCommand` → `Login()` posts credentials to `/api/userauth/login`; on success it navigates to
`new HomeViewModel(_nav, _http, user)`, carrying the `UserAuth` (including the plaintext password the user typed) in
memory for the rest of the session.

### `EncryptViewModel` / `DecryptViewModel`

The simplest feature: pick a file via `OpenFileDialog`, run it through `EncryptDecryptService.AesEncryptBytes` /
`AesDecryptBytes`, and save the result. These use the service's **static, shared, plain-AES** key/IV (see
[encryption-and-data-flow.md](encryption-and-data-flow.md)) rather than anything tied to the logged-in user. Errors
(`FormatException`, `CryptographicException`) are caught and surfaced through `FeedbackMessage`.

### `SecretsViewModel`

A single encrypted note. `Save()` AES-GCM encrypts the note text with a fresh random key, RSA-encrypts that key to the
*current user's own* public key (read straight from the certificate in the Windows cert store, not from the API),
bundles everything into a `Secret` model, and writes it as JSON to a user-chosen file. `Read()` reverses the process,
pulling the matching private key from the certificate store to unwrap the AES key.

### `MessageViewModel`

The networked feature. Backed by `HttpService` for all API calls:

- `LoadUsers` → `HttpService.GetUsers()` — populates the recipient `ComboBox`.
- `LoadMessages` → `HttpService.GetAllMessages(username)` — populates the inbox `ComboBox` for the current user.
- `SendFiles` → `CreateSendPayload()` — encrypts, signs, and POSTs a file to the selected recipient.
- `DownloadPayload` → `_DownloadPayload()` — fetches the sender's public key, verifies the signature, then decrypts
  and saves the file.

See [encryption-and-data-flow.md](encryption-and-data-flow.md) for the full step-by-step flow of both operations.

### `HomeViewModel`

Hub screen; also owns `ExportKey`/`ChangeKey` commands that export/import the shared static AES key+IV used by
`EncryptViewModel`/`DecryptViewModel` as a `base64key,base64iv` line in a text file. (These buttons are commented out
in `HomeView.xaml`, so they're unreachable from the UI in the current build, though the code paths still exist.)

## Services

| Service | Interface | Responsibility |
|---|---|---|
| [`HttpService`](../SecureNotes/Services/HttpService.cs) | *(none implemented — `IHttpService` is an empty marker)* | Owns a single static `HttpClient`; wraps every API call (`GetUsers`, `Register`, `CanRegister`, `GetAllMessages`, `GetPublicKey`) and JSON (de)serialization. |
| [`EncryptDecryptService`](../SecureNotes/Services/EncryptDecryptService.cs) | [`IEncryptDecryptService`](../SecureNotes/IServices/IEncryptDecryptService.cs) *(interface only declares the plain-AES methods; the class exposes many more — see below)* | All cryptography: plain AES, RSA encrypt/decrypt, AES-GCM encrypt/decrypt, signing (PEM-based and CNG-based), and signature verification. |
| [`FileService`](../SecureNotes/Services/FileService.cs) | [`IFileService`](../SecureNotes/IServices/IFileService.cs) | Thin wrapper over `System.IO.File` — read/write bytes, read/write text — with exceptions normalized to fresh instances (stack traces of the originals are discarded). |
| [`NavigationService`](../SecureNotes/Services/NavigationService.cs) | [`INavigationService`](../SecureNotes/IServices/INavigationService.cs) | Swaps `MainViewModel.CurrentViewModel`, described above. |

Two things worth calling out about how these are used:

- **No dependency injection.** View models depend on `EncryptDecryptService`/`FileService` concretely and construct
  their own instances (`new EncryptDecryptService()`), so the `I*Service` interfaces exist but aren't leveraged for
  substitutability anywhere — there's also no unit test project exercising them.
- **`HttpService` and `NavigationService` are the only services actually threaded through constructors** from
  `MainViewModel` down; everything else is a fresh `new` per view model instance.

## Commands

[`RelayCommand`](../SecureNotes/Commands/RelayCommand.cs) is the sole `ICommand` implementation, wrapping an
`Action` (+ optional `Func<bool>` for `CanExecute`, unused everywhere it's constructed). All buttons in every view
bind to a `RelayCommand` property exposed by the view model — this is the standard MVVM command pattern, just without
a third-party library.

## Views

Views are plain `UserControl`s (not `Window`s) so they can be swapped into `MainWindow`'s single `ContentControl`.
Each binds its controls directly to the matching view model's properties/commands (e.g. `AuthView.xaml` binds
`TextBox.Text` to `UsernameText`/`PasswordText` and buttons to `LoginCommand`/`RegisterCommand`). There's an unused
`ClearableTextBox` user control (`Views/UserControls/ClearableTextBox.xaml`) referenced only from commented-out XAML,
suggesting an earlier iteration of the UI that was replaced by plain `TextBox`es.

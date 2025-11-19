using Microsoft.Win32;
using SecureNotes.Commands;
using SecureNotes.Models;
using SecureNotes.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Printing.IndexedProperties;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SecureNotes.ViewModels
{
    public class SecretsViewModel : INotifyPropertyChanged
    {
        public const string KEY_NAME_PREFIX = "SecureNotes-";

        private EncryptDecryptService _encryptDecryptService = new EncryptDecryptService();
        private FileService _fileService = new FileService();
        private JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        private string _secretMessage;
        private UserAuth _user;

        public ICommand SaveCommand { get; }
        public ICommand ReadCommand { get; }
        public ICommand NavigateHome { get; }

        public string SecretMessage
        {
            get { return _secretMessage; }
            set
            {
                _secretMessage = value;
                OnPropertyChanged();
            }
        }

        public SecretsViewModel(NavigationService nav, HttpService http, UserAuth user)
        {
            _user = user;
            NavigateHome = new RelayCommand(() => nav.NavigateTo(new HomeViewModel(nav, http, user)));
            SaveCommand = new RelayCommand(() => Save());
            ReadCommand = new RelayCommand(() => Read());
        }

        public void Save() 
        {
            // Convert message into bytes.
            byte[] plaintext = Encoding.UTF8.GetBytes(SecretMessage);
            // Generate random AES-GCM key and encrypt the bytes of the message.
            (byte[] ciphertextMessage, byte[] key, byte[] iv, byte[] tag) = _encryptDecryptService.AesGcmEncrypt(plaintext);
            // Get public key using windows cert store and encrypt aes key with it.
            byte[] ciphertextKey;
            using (X509Store store = new X509Store(StoreName.My, StoreLocation.CurrentUser))
            {
                store.Open(OpenFlags.ReadOnly);
                var cert = store.Certificates
                    .Find(X509FindType.FindBySubjectName, KEY_NAME_PREFIX + _user.Username, false)
                    .FirstOrDefault() ?? throw new CryptographicException();

                RSA rsaKey = cert.GetRSAPublicKey() ?? throw new CryptographicException();
                ciphertextKey = _encryptDecryptService.RsaEncryptBytes(key, rsaKey.ExportSubjectPublicKeyInfoPem());
                store.Close();
            }
            // Base64 aes-gcm ciphertext, iv, tag, and encrypted text. Encapsulated all the data into an object, then serialize the object.
            Secret userSecret = new Secret(
                Convert.ToBase64String(ciphertextKey),
                Convert.ToBase64String(iv),
                Convert.ToBase64String(tag),
                Convert.ToBase64String(ciphertextMessage)
                );
            String json = JsonSerializer.Serialize<Secret>(userSecret, _jsonOptions);
            // Store the string data into a txt file.
            OpenFileDialog path = new OpenFileDialog
            {
                CheckFileExists = false,
                ValidateNames = false,
                Multiselect = false,
            };
            bool? success = path.ShowDialog();
            if (success == true)
            {
                _fileService.WriteStringTxtFile(path.FileName, json);
            }
        }

        public void Read() 
        {
            // Openfile dialog to select file
            OpenFileDialog path = new OpenFileDialog
            {
                Multiselect = false
            };
            bool? success = path.ShowDialog();
            if (success == true) 
            {
                try
                {
                    // Read the string form the file
                    string json = _fileService.ReadTxtFileAsString(path.FileName);
                    SecretMessage = json;
                    // Deserialize the string into an object
                    Secret secret = JsonSerializer.Deserialize<Secret>(json, _jsonOptions) ?? throw new JsonException();
                    // Convert base64 fields into byte[]
                    byte[] ciphertextKey = Convert.FromBase64String(secret.CiphertextKey);
                    byte[] iv = Convert.FromBase64String(secret.IV);
                    byte[] tag = Convert.FromBase64String(secret.Tag);
                    byte[] ciphertextMessage = Convert.FromBase64String(secret.CiphertextMessage);
                    // Decrypt aes-gcm key using private RSA key and store in an instance
                    byte[] key;
                    using (X509Store store = new X509Store(StoreName.My, StoreLocation.CurrentUser))
                    {
                        store.Open(OpenFlags.ReadOnly);
                        var cert = store.Certificates
                            .Find(X509FindType.FindBySubjectName, KEY_NAME_PREFIX + _user.Username, false)
                            .FirstOrDefault() ?? throw new CryptographicException();
                        key = _encryptDecryptService.RsaDecryptBytes(ciphertextKey, cert.GetRSAPrivateKey() ?? throw new CryptographicException());
                    }
                    // Use aes-gcm key, iv, and tag to decrypt encrypted message
                    byte[] plaintext = _encryptDecryptService.AesGcmDecrypt(ciphertextMessage, key, iv, tag);
                    // Assign field that displays messages with the message plaintext
                    SecretMessage = Encoding.UTF8.GetString(plaintext);
                }
                catch (JsonException e)
                {
                    SecretMessage = "Json exception.";
                }
                catch (CryptographicException e)
                {
                    SecretMessage = "Cryptographic Exception";
                }
                catch (InvalidOperationException e)
                {
                    SecretMessage = "Invalid Operation Exception";
                }
            }
                
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

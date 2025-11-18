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

        EncryptDecryptService _encryptDecryptService = new EncryptDecryptService();
        FileService _fileService = new FileService();

        private string _secret;
        private UserAuth _user;

        public ICommand SaveCommand { get; }
        public ICommand ReadCommand { get; }
        public ICommand NavigateHome { get; }

        public string Secret
        {
            get { return _secret; }
            set
            {
                _secret = value;
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
            byte[] plaintext = Encoding.UTF8.GetBytes(Secret);
            // Generate random AES-GCM key and encrypt the bytes of the message.
            (byte[] ciphertextMessage, byte[] key, byte[] iv, byte[] tag) = _encryptDecryptService.AesGcmEncrypt(plaintext);
            // Get public key using windows cert store and encrypt aes key with it.
            byte[] ciphertextKey;
            using (X509Store store = new X509Store(StoreName.My, StoreLocation.CurrentUser))
            {
                store.Open(OpenFlags.ReadOnly);
                var cert = store.Certificates
                    .Find(X509FindType.FindBySubjectName, $"SecureNotes-{_user.Username}", false)
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
            String json = JsonSerializer.Serialize<Secret>(userSecret);
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
            // Read the string form the file
            // Deserialize the string into an object
            // Convert base64 fields into byte[]
            // Decrypt aes-gcm key and store in an instance
            // Use aes-gcm key, iv, and tag to decrypt encrypted message
            // Assign field that displays messages with the message plaintext
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

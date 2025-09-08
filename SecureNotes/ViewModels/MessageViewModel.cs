using Microsoft.Win32;
using SecureNotes.Commands;
using SecureNotes.Models;
using SecureNotes.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Printing;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SecureNotes.ViewModels
{
    internal class MessageViewModel : INotifyPropertyChanged
    {

        private HttpService _http;
        private FileService _fileService = new FileService();
        private EncryptDecryptService _encryptDecryptService = new EncryptDecryptService();
        private List<UserAuth> _users = new List<UserAuth>();
        private List<Payload> _userMessages = new List<Payload>();
        private UserAuth _currentUser;

        private bool _sendFilesVisible = false;
        private string _feedbackMessage = "Feedback Message";
        private UserAuth _recipient;
        private Payload _selectedMessage;

        public ICommand NavigateHome { get; }
        public ICommand LoadUsers { get; }
        public ICommand SendFiles { get; }
        public ICommand LoadMessages { get; }
        public ICommand DownloadPayload { get; }

        public string FeedbackMessage 
        { 
            get { return _feedbackMessage; } 
            set 
            { 
                _feedbackMessage = value;
                OnPropertyChanged();
            }
        }
        public List<UserAuth> Users 
        { 
            get { return _users; } 
            set
            {
                _users = value;
                OnPropertyChanged();
            }

        }

        public List<Payload> UserMessages
        {
            get { return _userMessages; }
            set
            {
                _userMessages = value;
                OnPropertyChanged();
            }
        }

        public UserAuth Recipient
        {
            get { return _recipient; }
            set
            {
                _recipient = value;
                OnPropertyChanged();
            }
        }

        public UserAuth CurrentUser
        {
            get { return _currentUser; }
            set
            {
                _currentUser = value;
                OnPropertyChanged();
            }
        }

        public Payload SelectedMessage
        {
            get { return _selectedMessage; }
            set
            {
                _selectedMessage = value;
                OnPropertyChanged();
            }
        }

        public bool SendFilesVisible
        {
            get { return _sendFilesVisible; }
            set
            {
                _sendFilesVisible = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public MessageViewModel(NavigationService nav, HttpService http, UserAuth user)
        {
            _http = http;
            _currentUser = user;
            NavigateHome = new RelayCommand(() => nav.NavigateTo(new HomeViewModel(nav, http, user)));
            LoadUsers = new RelayCommand(async () => { await _LoadUsers(); });
            LoadMessages = new RelayCommand(async () => { await _LoadMessages(); });
            SendFiles = new RelayCommand(async () => { await CreateSendPayload(); });
            DownloadPayload = new RelayCommand(() => { _DownloadPayload(); });
        }


        public async Task _LoadUsers()
        {
            Users = await _http.GetUsers();
        }

        public async Task _LoadMessages()
        {
            UserMessages = await _http.GetAllMessages(CurrentUser.Username);
        }

        // Method to read files, aes encrypt files, rsa encrypt the aes key, convert aes key into base64,
        // and store into payload to be sent through http request later on.
        // TODO: implement multiple files to be selected.
        public async Task CreateSendPayload()
        {
            try
            {
                if (Recipient == null) throw new InvalidOperationException();

                // Open and read file.
                OpenFileDialog fileSelection = new OpenFileDialog();
                fileSelection.Filter = "All Files (*.*)|*.*";
                bool? success = fileSelection.ShowDialog();
                if (success == true)
                {
                    string filePath = fileSelection.FileName;
                    byte[] data = _fileService.ReadAllBytes(filePath);
                    // TODO: Implement AesGcm
                    EncryptDecryptService.GenerateAes(); // generate a random new aes key
                    string cipherText = Convert.ToBase64String(_encryptDecryptService.AesEncryptBytes(data));
                    string uuid = Guid.NewGuid().ToString();
                    string encryptedEncodedAesKey = Convert.ToBase64String(_encryptDecryptService.RsaEncryptBytes(EncryptDecryptService.AesAlg.Key, Recipient.PublicKey));
                    string encodedIV = Convert.ToBase64String(EncryptDecryptService.AesAlg.IV);
                    DateTime univDateTime = DateTime.Now.ToUniversalTime();
                    // Construct the payload
                    Payload payload = new Payload
                    {
                        UUID = uuid,
                        Sender = CurrentUser.Username,
                        Recipient = Recipient.Username,
                        Ciphertext = cipherText,
                        Key = encryptedEncodedAesKey,
                        IV = encodedIV,
                        Format = fileSelection.SafeFileName,
                        Timestamp = univDateTime
                    };
                    // Serialize payload and prepare it to be sent.
                    string jsonPayload = JsonSerializer.Serialize(payload);
                    StringContent payloadContent = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
                    // Make an api request to send the payload to the user.
                    try
                    {
                        using HttpResponseMessage response = await HttpService.client.PostAsync(HttpService.API_SEND_PAYLOAD, payloadContent);
                        response.EnsureSuccessStatusCode();
                        string responseBody = await response.Content.ReadAsStringAsync();
                        FeedbackMessage = responseBody;
                    }
                    catch (HttpRequestException e)
                    {
                        FeedbackMessage = e.Message;
                    }
                    
                }
                else
                {
                    FeedbackMessage = "No File Path";
                }
            }
            catch (InvalidOperationException e)
            {
                FeedbackMessage = "Recipient is null";
            }
        }

        public void _DownloadPayload()
        {
            // 1. Open file dialog, user selects private key.
            OpenFileDialog privateKeySelection = new OpenFileDialog
            {
                Filter = "Select Private Key (*.txt) | *.txt"    
            };

            bool? success = privateKeySelection.ShowDialog();

            if (success == true)
            {
                // 2. Convert encoded base64 aes key to byte[].
                byte[] cipherTextAesKey = Convert.FromBase64String(SelectedMessage.Key);
                // 3. Decrypt byte[] aes key using user rsa private key
                string privateKeyPem = _fileService.ReadTxtFileAsString(privateKeySelection.FileName);
                byte[] aesKey = _encryptDecryptService.RsaDecryptBytes(cipherTextAesKey, privateKeyPem);
                // 4. Convert encoded base64 ciphertext to byte[].
                byte[] cipherText = Convert.FromBase64String(SelectedMessage.Ciphertext);
                // 5. Convert encoded base64 IV to byte[].
                byte[] iv = Convert.FromBase64String(SelectedMessage.IV);
                // 6. Decrypt byte[] ciphertext using aes key and IV.
                EncryptDecryptService.ChangeAesKey(aesKey, iv);
                byte[] plaintextBytes = _encryptDecryptService.AesDecryptBytes(cipherText);
                // 7. Open file dialog, user selects and stores directory where data will be saved.
                OpenFileDialog fileDestination = new OpenFileDialog
                {
                    CheckFileExists = false,
                    ValidateNames = false,
                    Multiselect = false,
                    // 8. Append the directory string with the file name, as well as the format of the file.
                    FileName = SelectedMessage.Format
                };
                success = fileDestination.ShowDialog();
                if (success == true)
                {
                    // 9. Write bytes to path. Generalize it for all formats instead of just .txt files.
                    _fileService.WriteAllBytes(fileDestination.FileName, plaintextBytes);
                }
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string stringProperty = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(stringProperty));
        }
    }
}

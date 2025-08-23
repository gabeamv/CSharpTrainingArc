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

        private string _feedbackMessage = "Feedback Message";
        private UserAuth _recipient;

        public ICommand NavigateHome { get; }
        public ICommand LoadUsers { get; }

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

        public UserAuth Recipient
        {
            get { return _recipient; }
            set
            {
                _recipient = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public MessageViewModel(NavigationService nav, HttpService http)
        {
            _http = http;
            NavigateHome = new RelayCommand(() => nav.NavigateTo(new HomeViewModel(nav, http)));
            LoadUsers = new RelayCommand(async () => { await _LoadUsers(); });
        }


        public async Task _LoadUsers()
        {
            Users = await _http.GetUsers();
        }

        // Method to read files, aes encrypt files, rsa encrypt the aes key, convert aes key into base64,
        // and store into payload to be sent through http request later on.
        // TODO: implement multiple files.
        public void CreatePayload()
        {
            try
            {
                if (Recipient == null) throw new InvalidOperationException();

                // Open and read file.
                OpenFileDialog fileSelection = new OpenFileDialog();
                fileSelection.Filter = "Select Files |All files (*.*)";
                bool? success = fileSelection.ShowDialog();
                if (success == true)
                {
                    string filePath = fileSelection.FileName;
                    byte[] data = _fileService.ReadTxtFileAsBytes(filePath);
                    // Encrypt Data
                    // TODO: Implement AesGcm
                    EncryptDecryptService.GenerateAes(); // generate new aes key
                    // Data that will be stored in the payload.
                    byte[] cipherText = _encryptDecryptService.AesEncryptBytes(data);
                    string uuid = Guid.NewGuid().ToString(); // ID
                    // Key and IV
                    string encryptedEncodedAesKey = Convert.ToBase64String(_encryptDecryptService.RsaEncryptBytes(EncryptDecryptService.AesAlg.Key, Recipient.PublicKey));
                    string encodedIV = Convert.ToBase64String(EncryptDecryptService.AesAlg.IV);
                    DateTime univDateTime = DateTime.Now.ToUniversalTime(); // Timestamp
                    // Construct the payload
                    Payload payload = new Payload
                    {
                        ID = uuid,
                        Sender = "TODO",
                        Recipient = Recipient.Username,
                        Key = encryptedEncodedAesKey,
                        IV = encodedIV,
                        Format = "TODO",
                        Timestamp = univDateTime
                    };
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

        

        protected void OnPropertyChanged([CallerMemberName] string stringProperty = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(stringProperty));
        }
    }
}

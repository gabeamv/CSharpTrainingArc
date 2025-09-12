using Microsoft.Win32;
using SecureNotes.Commands;
using SecureNotes.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using System.Security.Cryptography;
using SecureNotes.Models;

namespace SecureNotes.ViewModels
{
    public class DecryptViewModel : INotifyPropertyChanged
    {
        private FileService _fileService = new FileService();
        private EncryptDecryptService _encryptDecryptService = new EncryptDecryptService();
        private HttpService _http;

        private string _feedbackMessage;
        public string FeedbackMessage
        {
            get { return _feedbackMessage; }
            set { 
                _feedbackMessage = value;
                OnPropertyChanged();
            }
        }
        public DecryptViewModel(NavigationService navService, HttpService http, UserAuth user)
        {
            NavigateHome = new RelayCommand(() => navService.NavigateTo(new HomeViewModel(navService, http, user)));
            DecryptFile = new RelayCommand(() => this.Decrypt());
        }


        public ICommand NavigateHome { get; }
        public ICommand DecryptFile { get; }

        public event PropertyChangedEventHandler? PropertyChanged;

        public void Decrypt()
        {
            OpenFileDialog fileSelection = new OpenFileDialog();
            fileSelection.Filter = "Select File (*.*)|*.*";
            bool? success = fileSelection.ShowDialog();
            if (success == true)
            {
                try 
                {
                    byte[] cipherText = _fileService.ReadAllBytes(fileSelection.FileName);                                                          
                    byte[] plainText = _encryptDecryptService.AesDecryptBytes(cipherText);
                    OpenFileDialog dirSelection = new OpenFileDialog();
                    dirSelection.Filter = "The Directory (*.*)|*.*";
                    dirSelection.CheckFileExists = false;
                    dirSelection.ValidateNames = false;
                    dirSelection.Multiselect = false;
                    dirSelection.FileName = $"decrypted_{fileSelection.SafeFileName}";
                    success = dirSelection.ShowDialog();
                    if (success == true)
                    {
                        _fileService.WriteAllBytes(dirSelection.FileName, plainText);
                        // Write the AES key to txt using the file service and the key.
                        FeedbackMessage = "Decryption Success!";
                    }
                }
                catch (FormatException e)
                {
                    FeedbackMessage = "Invalid File. Base64 format required.";
                }
                catch (CryptographicException e)
                {
                    FeedbackMessage = "Decryption Failed.";
                }

            }
            else
            {
                FeedbackMessage = "No File Path";
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string stringProperty = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(stringProperty));
        }
    }
}


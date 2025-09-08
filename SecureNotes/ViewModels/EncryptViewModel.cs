using Microsoft.Win32;
using SecureNotes.Commands;
using SecureNotes.Models;
using SecureNotes.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SecureNotes.ViewModels
{
    public class EncryptViewModel : INotifyPropertyChanged
    {
        private FileService _fileService = new FileService();
        private EncryptDecryptService _encryptDecryptService = new EncryptDecryptService();
        public EncryptViewModel(NavigationService navService, HttpService http, UserAuth user) 
        {
            NavigateHome = new RelayCommand(() => navService.NavigateTo(new HomeViewModel(navService, http, user)));
            EncryptFile = new RelayCommand(() => Encrypt());
        }

        private string _feedbackMessage;
        public string FeedbackMessage
        {
            get { return _feedbackMessage; }
            set
            {
                _feedbackMessage = value;
                OnPropertyChanged();
            }
        }

        public ICommand NavigateHome { get; }
        public ICommand EncryptFile { get; }


        public event PropertyChangedEventHandler? PropertyChanged;

        private void Encrypt()
        {
            OpenFileDialog fileSelection = new OpenFileDialog();
            fileSelection.Filter = "Select A File (*.*)|*.*";
            bool? success = fileSelection.ShowDialog();
            if (success == true)
            {
                string filePath = fileSelection.FileName;
                byte[] txtData = _fileService.ReadAllBytes(filePath);
                byte[] cipherTextByte = _encryptDecryptService.AesEncryptBytes(txtData);
                string strCipher = Convert.ToBase64String(cipherTextByte);

                OpenFileDialog dirSelection = new OpenFileDialog();
                dirSelection.Filter = "Select Directory (*.*)|*.*";
                dirSelection.CheckFileExists = false;
                dirSelection.ValidateNames = false;
                dirSelection.Multiselect = false;
                dirSelection.FileName = $"encrypted_{fileSelection.SafeFileName}";
                success = dirSelection.ShowDialog();
                if (success == true)
                {
                    _fileService.WriteAllBytes(dirSelection.FileName, cipherTextByte);
                    FeedbackMessage = "Encryption Success!";
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

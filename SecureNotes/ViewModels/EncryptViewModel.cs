using Microsoft.Win32;
using SecureNotes.Commands;
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
        public EncryptViewModel(NavigationService navService, HttpService http) 
        {
            NavigateHome = new RelayCommand(() => navService.NavigateTo(new HomeViewModel(navService, http)));
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
            OpenFileDialog txtSelection = new OpenFileDialog();
            txtSelection.Filter = "Please Select A .txt File (*.txt) | *.txt";
            bool? success = txtSelection.ShowDialog();
            if (success == true)
            {
                string filePath = txtSelection.FileName;
                byte[] txtData = _fileService.ReadTxtFileAsBytes(filePath);
                byte[] cipherTextByte = _encryptDecryptService.AesEncryptBytes(txtData);
                string strCipher = Convert.ToBase64String(cipherTextByte);

                OpenFileDialog dirSelection = new OpenFileDialog();
                dirSelection.Filter = "Please Select The Directory For The .txt File | *.txt*";
                dirSelection.CheckFileExists = false;
                dirSelection.ValidateNames = false;
                dirSelection.Multiselect = false;
                dirSelection.FileName = "encrypted_file.txt";
                success = dirSelection.ShowDialog();
                if (success == true)
                {
                    _fileService.WriteStringTxtFile(dirSelection.FileName, strCipher);
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

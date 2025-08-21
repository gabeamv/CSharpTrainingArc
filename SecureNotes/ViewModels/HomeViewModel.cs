using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Win32;
using SecureNotes.Commands;
using SecureNotes.Services;



namespace SecureNotes.ViewModels
{
    public class HomeViewModel : INotifyPropertyChanged
    {
        private EncryptDecryptService _encryptDecryptService = new EncryptDecryptService();
        private FileService _fileService = new FileService();

        public event PropertyChangedEventHandler? PropertyChanged;

        private string _feedbackMessage;
        public string FeedbackMessage
        {
            get { return _feedbackMessage; }
            set { 
                _feedbackMessage = value;
                OnPropertyChanged();
            }
        }

        public ICommand ExportKey { get; }
        public ICommand ChangeKey { get; }
        public ICommand NavigateEncryptFile { get; }
        public ICommand NavigateDecryptFile { get; }
        public ICommand NavigateMessage { get; }
        public HomeViewModel(NavigationService nav) {
            NavigateEncryptFile = new RelayCommand(() => { nav.NavigateTo(new EncryptViewModel(nav)); });
            NavigateDecryptFile = new RelayCommand(() => { nav.NavigateTo(new DecryptViewModel(nav)); });
            NavigateMessage = new RelayCommand(() => { nav.NavigateTo(new MessageViewModel(nav)); });
            ExportKey = new RelayCommand(() => { ExportUserKey(); });
            ChangeKey = new RelayCommand(() => { ImportUserKey(); });
        }

        private void ExportUserKey()
        {
            OpenFileDialog dirSelection = new OpenFileDialog();
            dirSelection.CheckFileExists = false;
            dirSelection.ValidateNames = false;
            dirSelection.Multiselect = false;
            dirSelection.FileName = "key_iv.txt";
            dirSelection.Filter = "Output Directory | *.txt";
            bool? success = dirSelection.ShowDialog();
            if (success == true)
            {
                _fileService.WriteStringTxtFile(dirSelection.FileName, EncryptDecryptService.GetKeyIV());
            }

        }

        private void ImportUserKey()
        {
            OpenFileDialog keySelection = new OpenFileDialog();
            keySelection.Filter = "Select Key File | *.txt";
            bool? success = keySelection.ShowDialog();
            if (success == true)
            {
                try
                {
                    string base64KeyIV = _fileService.ReadTxtFileAsString(keySelection.FileName);
                    string[] base64KeyAndIVArr = base64KeyIV.Split(",");
                    byte[] key = Convert.FromBase64String(base64KeyAndIVArr[0]);
                    byte[] iv = Convert.FromBase64String(base64KeyAndIVArr[1]);
                    EncryptDecryptService.ChangeAesKey(key, iv);
                    FeedbackMessage = "Import Success!";
                }
                catch (FormatException e)
                {
                    // Format for the key should be ("{base64EncodedKey},{base64EncodedIV}")
                    FeedbackMessage = "Invalid File. Base64 format required.";
                }
                catch (IndexOutOfRangeException e)
                {
                    // If index out of bounds, user inputted an invalid file.
                    FeedbackMessage = "Import Failed";
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

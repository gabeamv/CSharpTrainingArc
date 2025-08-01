using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Win32;
using SecureNotes.Commands;
using SecureNotes.Services;



namespace SecureNotes.ViewModels
{
    public class HomeViewModel
    {
        private EncryptDecryptService _encryptDecryptService = new EncryptDecryptService();
        private FileService _fileService = new FileService();
        public ICommand ExportKey { get; }
        public ICommand ChangeKey { get; }
        public ICommand NavigateEncryptFile { get; }
        public ICommand NavigateDecryptFile { get; }
        public HomeViewModel(NavigationService nav) {
            NavigateEncryptFile = new RelayCommand(() => { nav.NavigateTo(new EncryptViewModel(nav)); });
            NavigateDecryptFile = new RelayCommand(() => { nav.NavigateTo(new DecryptViewModel(nav)); });
            ExportKey = new RelayCommand(() => { ExportUserKey(); });
            ChangeKey = new RelayCommand(() => { });
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
    }
}

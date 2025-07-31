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
        public EncryptViewModel(NavigationService navService) 
        {
            NavigateHome = new RelayCommand(() => navService.NavigateTo(new HomeViewModel(navService)));
            EncryptFile = new RelayCommand(() => Encrypt());
            TestOutput = "Testing";
        }

        private string _testOutput;
        public string TestOutput
        {
            get { return _testOutput; }
            set { 
                _testOutput = value;
                OnPropertyChanged();
            }
        }

        private string _fileDialogSuccess;

        public ICommand NavigateHome { get; }
        public ICommand EncryptFile { get; }
        public string FileDialogSuccess 
        { 
            get { return _fileDialogSuccess; }
            set
            {
                {  
                    _fileDialogSuccess = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public void Encrypt()
        {
            OpenFileDialog txtSelection = new OpenFileDialog();
            txtSelection.Filter = "Please Select A .txt File (*.txt) | *.txt";
            bool? success = txtSelection.ShowDialog();
            if (success == true)
            {
                
                string filePath = txtSelection.FileName;
                TestOutput = filePath; // test
                byte[] txtData = _fileService.ReadTxtFileAsBytes(filePath);
                //string strTxtData = Convert.ToBase64String(txtData); // test
                //TestOutput = strTxtData;
                byte[] cipherTextByte = _encryptDecryptService.AesEncryptBytes(txtData);
                // Convert binary encryption into readable text so that it can be readable 
                // in a text file. (ChatGPT)
                string strCipher = Convert.ToBase64String(cipherTextByte);
                TestOutput = strCipher; // test
                OpenFileDialog dirSelection = new OpenFileDialog();
                dirSelection.Filter = "Please Select The Directory For The .txt File | *.txt*";
                // Folder selection trick
                dirSelection.CheckFileExists = false;
                dirSelection.ValidateNames = false;
                dirSelection.Multiselect = false;
                dirSelection.FileName = "encrypted_file.txt";
                success = dirSelection.ShowDialog();
                if (success == true)
                {
                    // Output/write the encrypted data to txt using the file service and the encrypted bytes
                    //_fileService.WriteBytesTxtFile(dirSelection.FileName, cipherTextByte);
                    _fileService.WriteStringTxtFile(dirSelection.FileName, strCipher);
                    // Write the AES key to txt using the file service and the key.
                    
                }


            }
            else
            {
                TestOutput = "No File Path";
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string stringProperty = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(stringProperty));
        }
    }
}

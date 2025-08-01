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

namespace SecureNotes.ViewModels
{
    public class DecryptViewModel : INotifyPropertyChanged
    {
        private FileService _fileService = new FileService();
        private EncryptDecryptService _encryptDecryptService = new EncryptDecryptService();

        private string _feedbackMessage;
        public string FeedbackMessage
        {
            get { return _feedbackMessage; }
            set { 
                _feedbackMessage = value;
                OnPropertyChanged();
            }
        }
        public DecryptViewModel(NavigationService navService)
        {
            NavigateHome = new RelayCommand(() => navService.NavigateTo(new HomeViewModel(navService)));
            DecryptFile = new RelayCommand(() => this.Decrypt());
        }


        public ICommand NavigateHome { get; }
        public ICommand DecryptFile { get; }

        public event PropertyChangedEventHandler? PropertyChanged;

        public void Decrypt()
        {
            OpenFileDialog txtSelection = new OpenFileDialog();
            txtSelection.Filter = "Please Select A .txt File (*.txt) | *.txt";
            bool? success = txtSelection.ShowDialog();
            if (success == true)
            {
                try 
                {
                    
                    string filePath = txtSelection.FileName;

                    string base64 = _fileService.ReadTxtFileAsString(filePath);
                    byte[] cipherText = Convert.FromBase64String(base64); 
                                                                                   
                    string plainTextStr = _encryptDecryptService.AesDecryptBytes(cipherText);
                    OpenFileDialog dirSelection = new OpenFileDialog();
                    dirSelection.Filter = "Please Select The Directory | *.txt";
                    dirSelection.CheckFileExists = false;
                    dirSelection.ValidateNames = false;
                    dirSelection.Multiselect = false;
                    dirSelection.FileName = "decrypted_file.txt";
                    success = dirSelection.ShowDialog();
                    if (success == true)
                    {
                        _fileService.WriteStringTxtFile(dirSelection.FileName, plainTextStr);
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


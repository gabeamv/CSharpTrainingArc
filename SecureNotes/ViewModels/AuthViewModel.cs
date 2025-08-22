using Microsoft.Win32;
using SecureNotes.Commands;
using SecureNotes.Models;
using SecureNotes.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SecureNotes.ViewModels
{
    public class AuthViewModel : INotifyPropertyChanged
    {
        private string _usernameText;
        private string _passwordText;
        private string _usernameLabel;
        private string _passwordLabel;
        private string _feedbackMessage;
        private NavigationService _nav;
        private FileService _fileService = new FileService();
        private HttpService _http;

        public event PropertyChangedEventHandler? PropertyChanged;

        public string UsernameText {
            get { return _usernameText; }
            set 
            { 
                _usernameText = value;
                OnPropertyChanged();
            }
        }

        public string UsernameLabel
        {
            get { return _usernameLabel; }
            set
            {
                _usernameLabel = value;
                OnPropertyChanged();
            }
        }

        public string PasswordText { 
            get { return _passwordText; } 
            set
            {
                _passwordText = value;
                OnPropertyChanged();
            } 
        }
        public string PasswordLabel
        {
            get { return _passwordLabel; }
            set
            {
                _passwordLabel = value;
                OnPropertyChanged();
            }
        }
        public string FeedbackMessage
        {
            get { return _feedbackMessage; }
            set
            {
                _feedbackMessage = value;
                OnPropertyChanged();
            }
        }

        public ICommand LoginCommand { get; }
        public ICommand RegisterCommand { get; }
        public AuthViewModel(NavigationService nav, HttpService http) {
            _nav = nav;
            _http = http;
            LoginCommand = new RelayCommand(() => { 
                try
                {
                    _ = Login();
                }
                catch (HttpRequestException e)
                {
                    FeedbackMessage = e.Message;
                }
            });
            RegisterCommand = new RelayCommand(() => { 
                try
                {
                    _ = Register();
                }
                catch (HttpRequestException e)
                {
                    FeedbackMessage = e.Message;
                }
            });
            FeedbackMessage = "This is for feedback.";
            UsernameText = "gabeamv";
            PasswordText = "Test123";
        }

        private void NavigateTo(object o, RoutedEventArgs e)
        {
            _nav.NavigateTo(new HomeViewModel(_nav, _http));
        }

        protected void OnPropertyChanged([CallerMemberName] string stringProperty = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(stringProperty));
        }

        private async Task HttpTest()
        {
            using HttpResponseMessage response = await HttpService.client.GetAsync("https://localhost:7042/api/userauth/get");
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            FeedbackMessage = responseBody;
        }

        private async Task Register()
        {
            // magic no.
            using (RSA rsa = RSA.Create(2048))
            {
                UserAuth user = new UserAuth(UsernameText, PasswordText, rsa.ExportSubjectPublicKeyInfoPem());
                string userJson = JsonSerializer.Serialize(user);
                StringContent userJsonContent = new StringContent(userJson, Encoding.UTF8, "application/json");
                try
                {
                    // Store private RSA key that corresponds to the now generated public key.
                    OpenFileDialog dirSelection = new OpenFileDialog();
                    dirSelection.CheckFileExists = false;
                    dirSelection.ValidateNames = false;
                    dirSelection.Multiselect = false;
                    dirSelection.FileName = $"{UsernameText}_private_key.txt";
                    dirSelection.Filter = "Output Directory | *.txt";
                    bool? success = dirSelection.ShowDialog();
                    if (success == true)
                    {
                        // Upload public key and other user info 
                        using HttpResponseMessage response = await HttpService.client.PostAsync("https://localhost:7042/api/userauth/register", userJsonContent);
                        response.EnsureSuccessStatusCode();
                        string responseBody = await response.Content.ReadAsStringAsync();
                        FeedbackMessage = responseBody;
                        // Write private key to file.
                        _fileService.WriteStringTxtFile(dirSelection.FileName, rsa.ExportPkcs8PrivateKeyPem());
                    }
                    else
                    {
                        throw new DirectoryNotFoundException();
                    }
                    
                }
                catch (HttpRequestException e)
                {
                    FeedbackMessage = e.Message;
                }
                catch (DirectoryNotFoundException e)
                {
                    FeedbackMessage = e.Message;
                }
            }
            
        }

        private async Task Login()
        {
            UserAuth user = new UserAuth(UsernameText, PasswordText);
            string userJson = JsonSerializer.Serialize(user);
            StringContent userJsonContent = new StringContent(userJson, Encoding.UTF8, "application/json");
            try
            {
                using HttpResponseMessage response = await HttpService.client.PostAsync("https://localhost:7042/api/userauth/login", userJsonContent);
                response.EnsureSuccessStatusCode();
                if (response.IsSuccessStatusCode)
                {
                    _nav.NavigateTo(new HomeViewModel(_nav, _http));
                }
            }
            catch (HttpRequestException e)
            {
                FeedbackMessage = e.Message;
            }
            
        }
    }
}

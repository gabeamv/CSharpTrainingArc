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
using System.Security.Cryptography.X509Certificates;
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
                    _ = RegisterCng();
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
        // Register using Microsoft Software Key Storage Provider with Cryptography API: Next Generation (CNG)
        private async Task RegisterCng()
        {
            if (!await _http.CanRegister(UsernameText))
            {
                FeedbackMessage = "User already exists.";
                return;
            }
            CngKeyCreationParameters keyParams = new CngKeyCreationParameters
            {
                ExportPolicy = CngExportPolicies.None,
                KeyCreationOptions = CngKeyCreationOptions.OverwriteExistingKey, // for testing
                KeyUsage = CngKeyUsages.Signing | CngKeyUsages.Decryption
            };

            keyParams.Parameters.Add(new CngProperty("Length", BitConverter.GetBytes(3072), CngPropertyOptions.None));
            CngKey cng = CngKey.Create(CngAlgorithm.Rsa, $"SecureNotes-PK-{UsernameText}", keyParams);
            
            using (RSA rsa = new RSACng(cng))
            { 
                X509Store store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
                store.Open(OpenFlags.ReadWrite);
                // TODO: later implement certificate authority to sign certificates
                CertificateRequest certRequest = new CertificateRequest($"CN=SecureNotes-{UsernameText}", rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pss);
                X509Certificate2 cert = certRequest.CreateSelfSigned(DateTimeOffset.UtcNow, DateTimeOffset.UtcNow.AddYears(10));
                store.Add(cert);
                store.Close();
                // TODO: store certificates in database instead of public keys
                UserAuth user = new UserAuth(UsernameText, PasswordText, publicKey: rsa.ExportSubjectPublicKeyInfoPem());
                using (HttpResponseMessage response = await _http.Register(user))
                {
                    response.EnsureSuccessStatusCode();
                    string responseBody = await response.Content.ReadAsStringAsync();
                    FeedbackMessage = responseBody;
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
                    _nav.NavigateTo(new HomeViewModel(_nav, _http, user));
                }
            }
            catch (HttpRequestException e)
            {
                FeedbackMessage = e.Message;
            }
            
        }
    }
}

using SecureNotes.Commands;
using SecureNotes.Models;
using SecureNotes.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
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
        public AuthViewModel(NavigationService nav) {
            _nav = nav;
            LoginCommand = new RelayCommand(() => { 

                // TODO: Make an http request to log in. If log in is successful, navigate to the home page.
                _nav.NavigateTo(new HomeViewModel(_nav)); 

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
            UsernameLabel = "Username";
            PasswordLabel = "Password";
            FeedbackMessage = "This is for feedback.";
            UsernameText = "gabeamv";
            PasswordText = "Test123";
        }

        private void NavigateTo(object o, RoutedEventArgs e)
        {
            _nav.NavigateTo(new HomeViewModel(_nav));
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
            UserAuth user = new UserAuth(UsernameText, PasswordText);
            string userJson = JsonSerializer.Serialize(user);
            StringContent userJsonContent = new StringContent(userJson, Encoding.UTF8, "application/json");
            try
            {
                using HttpResponseMessage response = await HttpService.client.PostAsync("https://localhost:7042/api/userauth/register", userJsonContent);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                FeedbackMessage = responseBody;
            }
            catch (HttpRequestException e)
            {
                FeedbackMessage = e.Message;
            }
        }
    }
}

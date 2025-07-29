using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using SecureNotes.Services;
using SecureNotes.Commands;
using System.ComponentModel;

namespace SecureNotes.ViewModels
{
    public class AuthViewModel : INotifyPropertyChanged
    {
        private string _userNameText;
        private NavigationService _nav;

        public event PropertyChangedEventHandler? PropertyChanged;

        // ViewModel can expose command to UI .
        public string UsernameText {
            get { return _userNameText; }
            set 
            { 
                _userNameText = value;
                OnPropertyChanged(_userNameText);
            }
        }
        public string PasswordText { get; set; }

        public ICommand LoginCommand { get; }
        public AuthViewModel(NavigationService nav) {
            _nav = nav;
            LoginCommand = new RelayCommand(() => _nav.NavigateTo(new HomeViewModel(_nav)));
        }

        private void NavigateTo(object o, RoutedEventArgs e)
        {
            _nav.NavigateTo(new HomeViewModel(_nav));
        }

        protected void OnPropertyChanged([CallerMemberName] string stringProperty = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(stringProperty));
        }
    }
}

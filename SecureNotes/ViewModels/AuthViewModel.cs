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

namespace SecureNotes.ViewModels
{
    public class AuthViewModel
    {
        private NavigationService _nav;
        // ViewModel can expose command to UI .
        public ICommand NavigateCommand { get; }
        public AuthViewModel(NavigationService nav) {
            _nav = nav;
            NavigateCommand = new RelayCommand(() => _nav.NavigateTo(new HomeViewModel(_nav)));
        }

        private void NavigateTo(object o, RoutedEventArgs e)
        {
            _nav.NavigateTo(new HomeViewModel(_nav));
        }
    }
}

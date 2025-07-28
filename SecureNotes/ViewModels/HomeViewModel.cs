using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using SecureNotes.Commands;
using SecureNotes.Services;


namespace SecureNotes.ViewModels
{
    public class HomeViewModel
    {

        public ICommand NavigateEncryptFile { get; }
        public ICommand NavigateDecryptFile { get; }
        public HomeViewModel(NavigationService nav) {
            // Navigate to encryption feature
            NavigateEncryptFile = new RelayCommand(() => { nav.NavigateTo(new EncryptViewModel(nav)); });
            // Navigate to decryption feature
            NavigateDecryptFile = new RelayCommand(() => { nav.NavigateTo(new DecryptViewModel(nav)); });
        }
    }
}

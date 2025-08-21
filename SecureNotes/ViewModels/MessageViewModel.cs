using SecureNotes.Commands;
using SecureNotes.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SecureNotes.ViewModels
{
    internal class MessageViewModel : INotifyPropertyChanged
    {
        public ICommand NavigateHome { get; }
        public MessageViewModel(NavigationService nav)
        {
            NavigateHome = new RelayCommand(() => nav.NavigateTo(new HomeViewModel(nav)));
        }
        public event PropertyChangedEventHandler? PropertyChanged;
    }
}

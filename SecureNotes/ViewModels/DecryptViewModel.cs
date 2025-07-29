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
    public class DecryptViewModel
    {
        public ICommand NavigateHome { get; }
        public DecryptViewModel(NavigationService nav) 
        {
            NavigateHome = new RelayCommand(() => nav.NavigateTo(new HomeViewModel(nav)));
        }
    }
}

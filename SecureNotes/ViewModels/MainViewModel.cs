using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecureNotes.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        // We declare a public property of the Current viewmodel which will be exposed to the UI
        public object CurrentViewModel { get; set; }

        public MainViewModel()
        {
            // Initial page will be the Authentication Page
            CurrentViewModel = new AuthViewModel();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}

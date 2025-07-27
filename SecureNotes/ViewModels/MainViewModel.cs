using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using SecureNotes.Services;


namespace SecureNotes.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {

        private object _currentViewModel;

        // We declare a public property of the Current viewmodel which will be exposed to the UI
        public object CurrentViewModel {

            get => _currentViewModel;
            set
            {
                _currentViewModel = value;
                OnPropertyChanged();
            }
        }

        public MainViewModel()
        {
            // Instantiate a new nav service that will pass in a lambda function to change the current viewmodel
            // from the scope of feature viewmodels.
            var navService = new NavigationService(viewModel => CurrentViewModel = viewModel);
            // Initial page will be the Authentication Page
            CurrentViewModel = new AuthViewModel(navService);
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

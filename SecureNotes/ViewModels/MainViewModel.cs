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
            // Serivces
            var navService = new NavigationService(viewModel => CurrentViewModel = viewModel);
            var httpService = new HttpService();
            
            CurrentViewModel = new AuthViewModel(navService);
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

using SecureNotes.Commands;
using SecureNotes.Models;
using SecureNotes.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Printing;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SecureNotes.ViewModels
{
    internal class MessageViewModel : INotifyPropertyChanged
    {
        private HttpService _http;
        private List<UserAuth> _users = new List<UserAuth>();

        private string _feedbackMessage = "Feedback Message";
        private UserAuth? _recipient = null;

        public ICommand NavigateHome { get; }
        public ICommand LoadUsers { get; }

        public string FeedbackMessage 
        { 
            get { return _feedbackMessage; } 
            set 
            { 
                _feedbackMessage = value;
                OnPropertyChanged();
            }
        }
        public List<UserAuth> Users 
        { 
            get { return _users; } 
            set
            {
                _users = value;
                OnPropertyChanged();
            }

        }

        public UserAuth? Recipient
        {
            get { return _recipient; }
            set
            {
                _recipient = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public MessageViewModel(NavigationService nav, HttpService http)
        {
            _http = http;
            NavigateHome = new RelayCommand(() => nav.NavigateTo(new HomeViewModel(nav, http)));
            LoadUsers = new RelayCommand(async () => { await _LoadUsers(); });
        }


        public async Task _LoadUsers()
        {
            Users = await _http.GetUsers();
            FeedbackMessage = "";
            foreach (UserAuth user in Users)
            {
                FeedbackMessage += $"{user.Username}, ";
            }

        }

        protected void OnPropertyChanged([CallerMemberName] string stringProperty = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(stringProperty));
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SecureNotes.IServices;

namespace SecureNotes.Services
{
    // Implement the contract that viewmodels use.
    public class NavigationService : INavigationService
    {
        
        private readonly Action<object> _setViewModel;

        public NavigationService(Action<object> setViewModel)
        {
            _setViewModel = setViewModel;
        }

        public void NavigateTo(object viewModel)
        {
            _setViewModel(viewModel);
        }
    }
}

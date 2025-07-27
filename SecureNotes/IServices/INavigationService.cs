using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecureNotes.IServices
{
    // Contract that viewmodels use to handle navigation
    public class INavigationService
    {
        void NavigateTo(object viewModel) { }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SecureNotes.View.UserControls
{

    public partial class ClearableTextBox : UserControl
    {
        private string _Placeholder;
        // Dangerous
        public string Placeholder 
        {
            get { return _Placeholder; } 

            set {
                _Placeholder = value;
                TextInitial.Text = _Placeholder;
            } 
        }
        public ClearableTextBox()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            TextInput.Text = string.Empty;
            TextInput.Focus();
        }

        private void TextInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TextInput.Text != string.Empty)
            {
                TextInitial.Visibility = Visibility.Hidden;
            }
            else { TextInitial.Visibility = Visibility.Visible; }
        }
    }
}

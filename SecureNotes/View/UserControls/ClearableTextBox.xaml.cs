using System;
using System.Collections.Generic;
using System.ComponentModel;
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

    public partial class ClearableTextBox : UserControl, INotifyPropertyChanged
    {
        private string _BoundText;

        public event PropertyChangedEventHandler? PropertyChanged;

        public string BoundText
        {
            get { return _BoundText; } 

            set {
                _BoundText= value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("BoundText"));
            } 
        }
        public ClearableTextBox()
        {
            DataContext = this;
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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
        private string _TextLabel;
        private string _UserText;

        public event PropertyChangedEventHandler? PropertyChanged;

        public string TextLabel
        {
            get { return _TextLabel; } 

            set {
                _TextLabel = value;
                OnPropertyChanged();
            } 
        }

        public string UserText 
        { 
            get { return _UserText; }
            set
            {
                _UserText = value;
                OnPropertyChanged();
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

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

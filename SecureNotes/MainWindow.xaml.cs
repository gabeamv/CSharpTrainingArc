using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SecureNotes
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private string boundText;

        public event PropertyChangedEventHandler? PropertyChanged;


        public string BoundText
        {
            get { return boundText; }
            set 
            {
                boundText = value;
                OnPropertyChanged();
            }
        }
        public MainWindow()
        {
            DataContext = this;
            InitializeComponent();

            // BtnRun.Content = "Button";
        }

        private void btnSet_Click(object sender, RoutedEventArgs e) 
        {
            BoundText = "set from code";
        }
        
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
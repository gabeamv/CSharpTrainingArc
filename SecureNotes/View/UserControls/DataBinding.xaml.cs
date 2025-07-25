﻿using System;
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
    /// <summary>
    /// Interaction logic for DataBinding.xaml
    /// </summary>
    public partial class DataBinding : UserControl, INotifyPropertyChanged
    {
        public DataBinding()
        {
            DataContext = this;
            InitializeComponent();
        }

        private int _BoundText;

        public event PropertyChangedEventHandler? PropertyChanged;

        public int BoundText
        {
            get { return _BoundText; }
            set { 
                _BoundText = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(BoundText)));
            }
        }

    }
}

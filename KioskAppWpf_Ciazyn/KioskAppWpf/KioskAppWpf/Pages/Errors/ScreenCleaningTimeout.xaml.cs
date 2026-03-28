using System.ComponentModel;
using System.Windows.Controls;
using System.Windows;
using System.Threading;
using System.Configuration;
using System;
using OpcCommunication.Devices;
using DbCommunication.Entities;

namespace KioskAppWpf.Pages.Errors
{
    /// <summary>
    /// Interaction logic for ErrorPage.xaml
    /// </summary>
    public partial class ScreenCleaningTimeout : Page
    {
        public Transaction Transaction
        {
            get
            {
                return ((MainWindow)Application.Current.MainWindow).Transaction;
            }
        }

        public ScreenCleaningTimeout()
        {
            InitializeComponent();
            pfCustomer.Transaction = Transaction;
        }

        private void PageLoaded(object sender, RoutedEventArgs e)
        {
            pfCustomer.Transaction = Transaction;
        }
    }
}

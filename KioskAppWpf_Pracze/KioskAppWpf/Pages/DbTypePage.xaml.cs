using DbCommunication.Entities;
using DbCommunication.Enums;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace KioskAppWpf.Pages
{
    /// <summary>
    /// Interaction logic for SummaryPage.xaml
    /// </summary>
    public partial class DbTypePage : Page
    {
        public DbTypePage()
        {
            InitializeComponent();
            BindCustomer();
            Application.Current.MainWindow.KeyUp -= new KeyEventHandler(DbTypePage_KeyUp);
            Application.Current.MainWindow.KeyUp += new KeyEventHandler(DbTypePage_KeyUp);
        }

        private void BindCustomer()
        {
            var transaction = ((MainWindow)Application.Current.MainWindow).Transaction;
            pfCustomer.Transaction = transaction;
        }

        private void DbTypePage_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.D1)
            {
                ((MainWindow)Application.Current.MainWindow).UpdateTransactionDbType(AddressDbType.RegularCustomers);
            }
            if (e.Key == Key.D2)
            {
                ((MainWindow)Application.Current.MainWindow).UpdateTransactionDbType(AddressDbType.Teryt);
            }
        }

        private void PageLoad(object sender, RoutedEventArgs e)
        {
            BindCustomer();
            Application.Current.MainWindow.KeyUp -= new KeyEventHandler(DbTypePage_KeyUp);
            Application.Current.MainWindow.KeyUp += new KeyEventHandler(DbTypePage_KeyUp);
        }
        private void PageUnload(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.KeyUp -= new KeyEventHandler(DbTypePage_KeyUp);
        }
    }
}

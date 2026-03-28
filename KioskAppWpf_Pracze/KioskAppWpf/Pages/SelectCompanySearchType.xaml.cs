using DbCommunication.Entities;
using DbCommunication.Enums;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace KioskAppWpf.Pages
{
    public partial class SelectCompanySearchType : Page
    {
        public Transaction Transaction
        {
            get
            {
                return ((MainWindow)Application.Current.MainWindow).Transaction;
            }
        }
        public CustomerAddress CurrentAddress
        {
            get
            {
                return ((MainWindow)Application.Current.MainWindow).CurrentTransactionAddress;
            }
        }

        public SelectCompanySearchType()
        {
            InitializeComponent();
        }

        private void BindPage()
        {
            phHead.DbType = CurrentAddress.DbType;
            phHead.Transaction = Transaction;
            BindCustomer();
        }

        private void BindCustomer()
        {
            var transaction = ((MainWindow)Application.Current.MainWindow).Transaction;
            pfCustomer.Transaction = transaction;
        }

        private void SelectCompanySearchType_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.D1)
            {
                ((MainWindow)Application.Current.MainWindow).UpdateTransactionCompanySearchType(CompanySearchType.SearchByName);
            }
            if (e.Key == Key.D2)
            {
                ((MainWindow)Application.Current.MainWindow).UpdateTransactionCompanySearchType(CompanySearchType.SearchByType);
            }
        }

        private void PageLoaded(object sender, RoutedEventArgs e)
        {
            BindPage();
            Application.Current.MainWindow.KeyUp -= new KeyEventHandler(SelectCompanySearchType_KeyDown);
            Application.Current.MainWindow.KeyUp += new KeyEventHandler(SelectCompanySearchType_KeyDown);
        }

        private void PageUnload(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.KeyUp -= new KeyEventHandler(SelectCompanySearchType_KeyDown);
        }
    }
}

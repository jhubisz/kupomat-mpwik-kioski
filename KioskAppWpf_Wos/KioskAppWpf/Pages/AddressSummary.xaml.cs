using DbCommunication.Entities;
using DbCommunication.Enums;
using KioskAppWpf.Controls;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace KioskAppWpf.Pages
{
    /// <summary>
    /// Interaction logic for SummaryPage.xaml
    /// </summary>
    public partial class AddressSummary : Page
    {
        public Transaction Transaction
        {
            get
            {
                return ((MainWindow)Application.Current.MainWindow).Transaction;
            }
        }
        public Company CurrentCompany
        {
            get
            {
                return ((MainWindow)Application.Current.MainWindow).CurrentTransactionCompany;
            }
        }
        public CustomerAddress CurrentAddress
        {
            get
            {
                return ((MainWindow)Application.Current.MainWindow).CurrentTransactionAddress;
            }
        }
        public int NoOfAddresses
        {
            get
            {
                return Transaction.CustomerAddresses.Count + Transaction.EditedAddress;
            }
        }

        public AddressSummary()
        {
            InitializeComponent();
            DataContext = this;
            //BindPage();
            //BindAddress();
            //Application.Current.MainWindow.KeyUp += new KeyEventHandler(CargoDirectionPage_KeyUp);
        }

        private void BindAddress()
        {
            var address = ((MainWindow)Application.Current.MainWindow).CurrentTransactionAddress;

            spGmina.Visibility = Visibility.Visible;
            spMiejscowosc.Visibility = Visibility.Visible;
            spUlica.Visibility = Visibility.Visible;
            spNrPosesji.Visibility = Visibility.Visible;

            if (CurrentCompany != null)
            {
                lNazwa.Text = "Nazwa: ";
                tbNazwa.Text = CurrentCompany.Name;
                spCompanyName.Visibility = Visibility.Visible;
                lNumer.Text = "Numer posesji: ";
            }
            else if (Transaction.Cargo.Id == 3) // ROD
            {
                lNazwa.Text = "Nazwa ROD: ";
                tbNazwa.Text = address.RodName;
                spCompanyName.Visibility = Visibility.Visible;
                lNumer.Text = "Numer działki: ";
            }
            else
            {
                spCompanyName.Visibility = Visibility.Collapsed;
                lNumer.Text = "Numer posesji: ";
            }

            tbGmina.Text = CurrentAddress.GminaName;
            tbMiejscowosc.Text = CurrentAddress.MiejscowoscName;
            tbUlica.Text = CurrentAddress.UlicaName;
            tbNumer.Text = CurrentAddress.AddressNumber;
            tbNrUmowy.Text = CurrentAddress.ContractNo;
            tbIlosc.Text = CurrentAddress.DeclaredAmount.ToString();
        }
        private void ClearAddress()
        {
            lNazwa.Text = "";
            tbNazwa.Text = "";
            spCompanyName.Visibility = Visibility.Hidden;
            lNumer.Text = "";
            tbGmina.Text = "";
            tbMiejscowosc.Text = "";
            tbUlica.Text = "";
            tbNumer.Text = "";
            tbNrUmowy.Text = "";
            tbIlosc.Text = "";
        }

        private void BindPage()
        {
            pfCustomer.Transaction = Transaction;
            phHead.DbType = CurrentAddress.DbType;
            phHead.Transaction = Transaction;

            if (NoOfAddresses >= 15 || Transaction.Cargo.Id == 2) // Przekroczona liczba adresów lub ścieki przemysłowe
            {
                gNextAddress.Visibility = Visibility.Hidden;
            }
            else
            {
                gNextAddress.Visibility = Visibility.Visible;
            }
        }
        
        private void CargoDirectionPage_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.N && NoOfAddresses < 15 && Transaction.Cargo.Id != 2)
            {
                ((MainWindow)Application.Current.MainWindow).UpdateTransactionNewAddress();
            }
            if (e.Key == Key.Enter)
            {
                ((MainWindow)Application.Current.MainWindow).UpdateTransactionStartDump();
            }
            if (e.Key == Key.Delete)
            {
                ((MainWindow)Application.Current.MainWindow).UpdateTransactionRestart();
            }
        }

        private void PageLoaded(object sender, RoutedEventArgs e)
        {
            BindPage();

            BindAddress();

            Application.Current.MainWindow.KeyUp -= new KeyEventHandler(CargoDirectionPage_KeyUp);
            Application.Current.MainWindow.KeyUp += new KeyEventHandler(CargoDirectionPage_KeyUp);
        }

        private void PageUnload(object sender, RoutedEventArgs e)
        {
            ClearAddress();
            Application.Current.MainWindow.KeyUp -= new KeyEventHandler(CargoDirectionPage_KeyUp);
        }
    }
}

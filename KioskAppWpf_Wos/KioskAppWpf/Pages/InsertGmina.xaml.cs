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
    public partial class InsertGmina : Page
    {
        const int MIN_STRING_LENGTH_ALLOWED = 3;
        public string InsertedGmina { get; set; }
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

        public InsertGmina()
        {
            InitializeComponent();
            DataContext = this;
            //BindPage();
            //Application.Current.MainWindow.KeyUp += new KeyEventHandler(CargoDirectionPage_KeyUp);
        }

        private void BindPage()
        {
            pfCustomer.Transaction = Transaction;
            phHead.DbType = CurrentAddress.DbType;
            phHead.Transaction = Transaction;
        }

        private void InsertGminaPage_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab)
            {
                UpdatePage();
            }

            if (!(Keyboard.FocusedElement is TextBox))
            {
                if (e.Key == Key.Enter)
                {
                    if (!String.IsNullOrEmpty(tbGmina.Text)
                        && tbGmina.Text.Trim().Length >= MIN_STRING_LENGTH_ALLOWED)
                    {
                        ((MainWindow)Application.Current.MainWindow).UpdateTransactionAddressGmina(tbGmina.Text);
                    }
                }
                if (e.Key == Key.D1)
                {
                    ((MainWindow)Application.Current.MainWindow).UpdateTransactionDbType(AddressDbType.RegularCustomers);
                    tbGmina.Text = "";
                    SetFocusOnAutoCompleteBox();
                    BindPage();
                    UpdatePage();
                }
                if (e.Key == Key.D2)
                {
                    ((MainWindow)Application.Current.MainWindow).UpdateTransactionDbType(AddressDbType.Teryt);
                    tbGmina.Text = "";
                    SetFocusOnAutoCompleteBox();
                    BindPage();
                    UpdatePage();
                }

                if (!e.KeyboardDevice.IsKeyDown(Key.RightAlt) && e.Key == Key.Delete)
                {
                    ((MainWindow)Application.Current.MainWindow).UpdateTransactionRestart();
                }

                if (e.KeyboardDevice.IsKeyDown(Key.RightAlt) && e.Key == Key.Delete
                    && Transaction.TransactionType != TransactionType.ToiToi)
                {
                    ((MainWindow)Application.Current.MainWindow).UpdateTransactionRestartFull();
                }
            }
        }

        private void UpdatePage()
        {
            if (Keyboard.FocusedElement is TextBox)
            {
                gEnter.Visibility = Visibility.Hidden;
                gTab.Visibility = Visibility.Visible;
                gTabToEdit.Visibility = Visibility.Hidden;
            }
            else if (String.IsNullOrEmpty(tbGmina.Text) 
                || tbGmina.Text.Trim().Length < MIN_STRING_LENGTH_ALLOWED)
            {
                gEnter.Visibility = Visibility.Hidden;
                gTab.Visibility = Visibility.Hidden;
                gTabToEdit.Visibility = Visibility.Visible;
            }
            else
            {
                gEnter.Visibility = Visibility.Visible;
                gTab.Visibility = Visibility.Hidden;
                gTabToEdit.Visibility = Visibility.Hidden;
            }

            gRestartTransaction.Visibility = Transaction.TransactionType == TransactionType.ToiToi ? Visibility.Hidden : Visibility.Visible;

            if (!(Keyboard.FocusedElement is TextBox))
            {
                gShortcuts.Visibility = Visibility.Visible;
            }
            else
            {
                gShortcuts.Visibility = Visibility.Hidden;
            }
        }

        private void SetFocusOnAutoCompleteBox()
        {
            Keyboard.Focus(tbGmina);
        }

        private void PageLoaded(object sender, RoutedEventArgs e)
        {
            BindPage();

            SetFocusOnAutoCompleteBox();
            UpdatePage();

            tbGmina.Text = "";

            Application.Current.MainWindow.KeyUp -= new KeyEventHandler(InsertGminaPage_KeyUp);
            Application.Current.MainWindow.KeyUp += new KeyEventHandler(InsertGminaPage_KeyUp);
        }

        private void PageUnload(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.KeyUp -= new KeyEventHandler(InsertGminaPage_KeyUp);
        }
    }
}

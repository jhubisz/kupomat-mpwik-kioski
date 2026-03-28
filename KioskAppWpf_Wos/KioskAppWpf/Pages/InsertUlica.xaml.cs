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
    public partial class InsertUlica : Page
    {
        const int MIN_STRING_LENGTH_ALLOWED = 3;
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

        public InsertUlica()
        {
            InitializeComponent();
            DataContext = this;
            //BindPage();
            //Application.Current.MainWindow.KeyUp += new KeyEventHandler(InsertUlicaPage_KeyUp);
        }

        private void BindPage()
        {
            pfCustomer.Transaction = Transaction;
            phHead.DbType = CurrentAddress.DbType;
            phHead.Transaction = Transaction;
        }

        private void InsertUlicaPage_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab)
            {
                UpdatePage();
            }

            if (!(Keyboard.FocusedElement is TextBox))
            {
                if (e.Key == Key.Enter)
                {
                    if (!String.IsNullOrEmpty(tbUlica.Text)
                        && tbUlica.Text.Trim().Length >= MIN_STRING_LENGTH_ALLOWED)
                    {
                        ((MainWindow)Application.Current.MainWindow).UpdateTransactionAddressUlica(tbUlica.Text);
                    }
                }

                if (e.Key == Key.B)
                {
                    if (String.IsNullOrEmpty(tbUlica.Text))
                    {
                        ((MainWindow)Application.Current.MainWindow).UpdateTransactionAddressUlica("");
                    }
                }

                if (e.Key == Key.D1)
                {
                    ((MainWindow)Application.Current.MainWindow).UpdateTransactionDbType(AddressDbType.RegularCustomers);
                    tbUlica.Text = "";
                    SetFocusOnAutoCompleteBox();
                    BindPage();
                    UpdatePage();
                }
                if (e.Key == Key.D2)
                {
                    ((MainWindow)Application.Current.MainWindow).UpdateTransactionDbType(AddressDbType.Teryt);
                    tbUlica.Text = "";
                    SetFocusOnAutoCompleteBox();
                    BindPage();
                    UpdatePage();
                }

                if (!e.KeyboardDevice.IsKeyDown(Key.RightAlt) && e.Key == Key.Delete)
                {
                    ((MainWindow)Application.Current.MainWindow).UpdateTransactionRestart();
                }

                if (e.KeyboardDevice.IsKeyDown(Key.RightAlt) && e.Key == Key.Delete)
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
            else if (String.IsNullOrEmpty(tbUlica.Text)
                || tbUlica.Text.Trim().Length < MIN_STRING_LENGTH_ALLOWED)
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
            Keyboard.Focus(tbUlica);
        }

        private void PageLoaded(object sender, RoutedEventArgs e)
        {
            BindPage();

            SetFocusOnAutoCompleteBox();
            UpdatePage();

            tbUlica.Text = "";

            Application.Current.MainWindow.KeyUp -= new KeyEventHandler(InsertUlicaPage_KeyUp);
            Application.Current.MainWindow.KeyUp += new KeyEventHandler(InsertUlicaPage_KeyUp);
        }

        private void PageUnload(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.KeyUp -= new KeyEventHandler(InsertUlicaPage_KeyUp);
        }
    }
}

using DbCommunication.Entities;
using DbCommunication.Enums;
using KioskAppWpf.Controls;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace KioskAppWpf.Pages
{
    /// <summary>
    /// Interaction logic for SummaryPage.xaml
    /// </summary>
    public partial class SelectAmount : Page
    {
        public string SelectedAmount { get; set; }
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

        public SelectAmount()
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
            
            BindNavigation();
        }

        private void BindNavigation()
        {
            var dbType = CurrentAddress.DbType;
            gGoToRegularCustomers.Visibility = Visibility.Hidden;
            gGoToManualEntry.Visibility = Visibility.Hidden;
        }

        private void CargoDirectionPage_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab)
            {
                UpdatePage();
            }

            if (!(Keyboard.FocusedElement is TextBox))
            {
                if (e.Key == Key.Enter)
                {
                    if (!String.IsNullOrEmpty(SelectedAmount))
                    {
                        if (Decimal.TryParse(SelectedAmount, out decimal amount))
                        {
                            amount = amount > 111 ? 111 : amount;
                            ((MainWindow)Application.Current.MainWindow).UpdateTransactionAddressAmount(amount);
                        }
                    }
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
            else if (String.IsNullOrEmpty(tbNumer.Text)
                || !Decimal.TryParse(tbNumer.Text, out decimal result))
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
            Keyboard.Focus(tbNumer);
        }

        private void PageLoaded(object sender, RoutedEventArgs e)
        {
            BindPage();

            SetFocusOnAutoCompleteBox();
            UpdatePage();

            tbNumer.Text = "";

            if (Transaction.Card.Type == RfidCardType.ToiToi)
            {
                ((MainWindow)Application.Current.MainWindow).UpdateTransactionAddressAmount(Transaction.Card.Pojemnosc);
            }

            Application.Current.MainWindow.KeyUp -= new KeyEventHandler(CargoDirectionPage_KeyUp);
            Application.Current.MainWindow.KeyUp += new KeyEventHandler(CargoDirectionPage_KeyUp);
        }

        private void PageUnload(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.KeyUp -= new KeyEventHandler(CargoDirectionPage_KeyUp);
        }

        private void tbNumer_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }
        private static bool IsTextAllowed(string text)
        {
            Regex regex = new Regex("[^0-9,]+"); 
            return !regex.IsMatch(text);
        }
    }
}

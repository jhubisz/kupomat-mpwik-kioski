using DbCommunication.Entities;
using DbCommunication.Enums;
using KioskAppWpf.Controls;
using KioskAppWpf.Enums;
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
    public partial class InsertCompanyName : Page
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

        public InsertCompanyName()
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
            if (dbType == AddressDbType.RegularCustomers)
            {
                tbGoToRegularCustomers.Text = "- Przejdź do bazy Teryt";
                gGoToManualEntry.Visibility = Visibility.Hidden;
            }
            if (dbType == AddressDbType.Teryt)
            {
                tbGoToRegularCustomers.Text = "- Przejdź do stałych klientów";
                gGoToManualEntry.Visibility = Visibility.Visible;
            }
            if (dbType == AddressDbType.ManualEntry)
            {
                tbGoToRegularCustomers.Text = "- Przejdź do stałych klientów";
                tbGoToManualEntry.Text = "- Przejdź do bazy Teryt";
                gGoToManualEntry.Visibility = Visibility.Visible;
            }
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
                    if (!string.IsNullOrEmpty(tbCompany.Text))
                    {
                        ((MainWindow)Application.Current.MainWindow).UpdateTransactionCompany(tbCompany.Text);
                    }
                }

                if (e.Key == Key.D1)
                {
                    var dbType = CurrentAddress.DbType;
                    if (dbType == AddressDbType.RegularCustomers)
                    {
                        ((MainWindow)Application.Current.MainWindow).UpdateTransactionDbType(AddressDbType.Teryt);
                    }
                    if (dbType == AddressDbType.Teryt || dbType == AddressDbType.ManualEntry)
                    {
                        ((MainWindow)Application.Current.MainWindow).UpdateTransactionDbType(AddressDbType.RegularCustomers);
                    }
                    tbCompany.Text = "";
                    SetFocusOnAutoCompleteBox();
                    BindPage();
                    UpdatePage();
                }

                if (e.Key == Key.D2)
                {
                    var dbType = CurrentAddress.DbType;
                    if (dbType == AddressDbType.RegularCustomers)
                    {
                        return;
                    }
                    if (dbType == AddressDbType.Teryt)
                    {
                        ((MainWindow)Application.Current.MainWindow).UpdateTransactionDbTypeManualEntry(DisplayPage.InsertCompanyName);
                    }
                    if (dbType == AddressDbType.ManualEntry)
                    {
                        ((MainWindow)Application.Current.MainWindow).UpdateTransactionDbType(AddressDbType.Teryt);
                    }
                    tbCompany.Text = "";
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
            else if (string.IsNullOrEmpty(tbCompany.Text))
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
            Keyboard.Focus(tbCompany);
        }

        private void PageLoaded(object sender, RoutedEventArgs e)
        {
            BindPage();

            SetFocusOnAutoCompleteBox();
            UpdatePage();

            tbCompany.Text = "";

            Application.Current.MainWindow.KeyUp -= new KeyEventHandler(CargoDirectionPage_KeyUp);
            Application.Current.MainWindow.KeyUp += new KeyEventHandler(CargoDirectionPage_KeyUp);
        }

        private void PageUnload(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.KeyUp -= new KeyEventHandler(CargoDirectionPage_KeyUp);
        }
    }
}

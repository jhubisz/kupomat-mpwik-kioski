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
    public partial class SelectCompanyName : Page
    {
        public IEnumerable<Company> Companies { get; set; }
        public Company SelectedCompany { get; set; }
        public AutoCompleteFilterPredicate<object> CompanyFilter
        {
            get
            {
                return (searchText, obj) =>
                    (obj as Company).Name.ToLower().Contains(searchText.ToLower());
            }
        }
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

        public SelectCompanyName()
        {
            InitializeComponent();
            //BindCustomers();
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

        private void BindCustomers()
        {
            if (CurrentAddress.DbType == AddressDbType.RegularCustomers)
            {
                Companies = ((MainWindow)Application.Current.MainWindow).DbRead.GetAllRegularCompanies(CurrentAddress, Transaction.Card.Customer.Id);
                acbCompany.ItemsSource = Companies;
            }
            else
            {
                Companies = ((MainWindow)Application.Current.MainWindow).DbRead.GetAllCompanies(CurrentAddress);
                acbCompany.ItemsSource = Companies;
            }
        }
        
        private void SelectCompanyNamePage_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab)
            {
                UpdatePage();
            }

            if (!(Keyboard.FocusedElement is TextBox))
            {
                if (e.Key == Key.Enter)
                {
                    if (SelectedCompany != null)
                    {
                        ((MainWindow)Application.Current.MainWindow).UpdateTransactionCompany(SelectedCompany);
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
                    acbCompany.SelectedItem = null;
                    acbCompany.Text = "";
                    BindCustomers();
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
                    acbCompany.SelectedItem = null;
                    acbCompany.Text = "";
                    BindCustomers();
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

        private void acbCompany_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AutoCompleteBox box = (AutoCompleteBox)sender;
            ListBox innerListBox = (ListBox)box.Template.FindName("Selector", box);
            if (innerListBox != null)
            {
                innerListBox.ScrollIntoView(innerListBox.SelectedItem);
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
            else if (SelectedCompany == null)
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
            Keyboard.Focus(acbCompany);
            acbCompany.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));

            if (Keyboard.FocusedElement is TextBox)
            {
                ((TextBox)Keyboard.FocusedElement).SelectAll();
            }
        }

        private void PageLoaded(object sender, RoutedEventArgs e)
        {
            BindCustomers();
            BindPage();

            SetFocusOnAutoCompleteBox();
            UpdatePage();

            acbCompany.SelectedItem = null;
            acbCompany.Text = "";

            Application.Current.MainWindow.KeyUp -= new KeyEventHandler(SelectCompanyNamePage_KeyUp);
            Application.Current.MainWindow.KeyUp += new KeyEventHandler(SelectCompanyNamePage_KeyUp);
        }

        private void PageUnload(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.KeyUp -= new KeyEventHandler(SelectCompanyNamePage_KeyUp);
        }
    }
}

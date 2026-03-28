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
    public partial class SelectUlica : Page
    {
        public List<Ulica> Ulice { get; set; }
        public Ulica SelectedUlica { get; set; }
        public AutoCompleteFilterPredicate<object> UlicaFilter
        {
            get
            {
                return (searchText, obj) =>
                    (obj as Ulica).Name.ToLower().Contains(searchText.ToLower())
                    || (obj as Ulica).NameWithoutPl.ToLower().Contains(searchText.ToLower());
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
        public CustomerAddress PrevAddress
        {
            get
            {
                return ((MainWindow)Application.Current.MainWindow).PrevTransactionAddress;
            }
        }

        public SelectUlica()
        {
            InitializeComponent();
            //BindUlice();
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

        private void CheckUlice()
        {
            if (Ulice.Count == 0)
            {
                ((MainWindow)Application.Current.MainWindow).UpdateTransactionAddressUlica("");
                return;
            }
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

        private void BindUlice()
        {
            if (CurrentAddress.DbType == AddressDbType.RegularCustomers)
            {
                Ulice = ((MainWindow)Application.Current.MainWindow).DbRead.GetAllRegularUlice(CurrentAddress.MiejscowoscId, Transaction.Card.Customer.Id);
                acbUlica.ItemsSource = Ulice;
                PreselectAutoCompleteBox();
            }
            else
            {
                Ulice = ((MainWindow)Application.Current.MainWindow).DbRead.GetAllUlice(CurrentAddress.MiejscowoscId);
                acbUlica.ItemsSource = Ulice;
                PreselectAutoCompleteBox();
            }
        }

        private void PreselectAutoCompleteBox()
        {
            if (PrevAddress == null || acbUlica.SelectedItem != null)
                return;

            if (PrevAddress.MiejscowoscId == CurrentAddress.MiejscowoscId)
            {
                var selected = Ulice.Find(item => item.Id == PrevAddress.UlicaId);
                acbUlica.SelectedItem = selected;
            }

            if (Keyboard.FocusedElement is TextBox)
            {
                ((TextBox)Keyboard.FocusedElement).SelectAll();
            }
        }

        private void SelectUlicaPage_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab)
            {
                UpdatePage();
            }

            if (!(Keyboard.FocusedElement is TextBox))
            {
                if (e.Key == Key.Enter)
                {
                    if (SelectedUlica != null)
                    {
                        ((MainWindow)Application.Current.MainWindow).UpdateTransactionAddressUlica(SelectedUlica);
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
                    acbUlica.SelectedItem = null;
                    acbUlica.Text = "";
                    BindUlice();
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
                        ((MainWindow)Application.Current.MainWindow).UpdateTransactionDbTypeManualEntry(DisplayPage.InsertUlica);
                    }
                    if (dbType == AddressDbType.ManualEntry)
                    {
                        ((MainWindow)Application.Current.MainWindow).UpdateTransactionDbType(AddressDbType.Teryt);
                    }
                    acbUlica.SelectedItem = null;
                    acbUlica.Text = "";
                    BindUlice();
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
            else
            {
                if (e.Key == Key.Enter)
                {
                    PreselectAutoCompleteBox();
                }
            }
        }

        private void acbUlica_SelectionChanged(object sender, SelectionChangedEventArgs e)
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
            else if (SelectedUlica == null)
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
            Keyboard.Focus(acbUlica);
            acbUlica.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));

            if (Keyboard.FocusedElement is TextBox)
            {
                ((TextBox)Keyboard.FocusedElement).SelectAll();
            }
        }

        private void PageLoaded(object sender, RoutedEventArgs e)
        {
            BindUlice();
            BindPage();

            SetFocusOnAutoCompleteBox();
            UpdatePage();

            CheckUlice();

            acbUlica.SelectedItem = null;
            acbUlica.Text = "";
            PreselectAutoCompleteBox();

            Application.Current.MainWindow.KeyUp -= new KeyEventHandler(SelectUlicaPage_KeyUp);
            Application.Current.MainWindow.KeyUp += new KeyEventHandler(SelectUlicaPage_KeyUp);
        }

        private void PageUnload(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.KeyUp -= new KeyEventHandler(SelectUlicaPage_KeyUp);
        }
    }
}

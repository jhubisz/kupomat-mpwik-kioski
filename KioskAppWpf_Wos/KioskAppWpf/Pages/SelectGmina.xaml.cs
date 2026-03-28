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
    public partial class SelectGmina : Page
    {
        public List<Gmina> Gminy { get; set; }
        public Gmina SelectedGmina { get; set; }
        public AutoCompleteFilterPredicate<object> GminaFilter
        {
            get
            {
                return (searchText, obj) =>
                    (obj as Gmina).Name.ToLower().Contains(searchText.ToLower())
                    || (obj as Gmina).NameWithoutPl.ToLower().Contains(searchText.ToLower());
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

        public SelectGmina()
        {
            InitializeComponent();
            //BindGminy();
            DataContext = this;
            //BindPage();
            //Application.Current.MainWindow.KeyUp += new KeyEventHandler(SelectGminaDirectionPage_KeyDown);
            //Application.Current.MainWindow.KeyUp += new KeyEventHandler(SelectGminaDirectionPage_KeyUp);
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
                tbGoToManualEntry.Text = "- Brak adresu, wprowadź ręcznie";
                gGoToManualEntry.Visibility = Visibility.Visible;
            }
            if (dbType == AddressDbType.ManualEntry)
            {
                tbGoToRegularCustomers.Text = "- Przejdź do stałych klientów";
                tbGoToManualEntry.Text = "- Przejdź do bazy Teryt";
                gGoToManualEntry.Visibility = Visibility.Visible;
            }
        }

        private void BindGminy()
        {
            if (CurrentAddress.DbType == AddressDbType.RegularCustomers)
            {
                Gminy = ((MainWindow)Application.Current.MainWindow).DbRead.GetAllRegularGmina(Transaction.Card.Customer.Id);
                acbGmina.ItemsSource = Gminy;
                PreselectAutoCompleteBox();
            }
            else
            {
                Gminy = ((MainWindow)Application.Current.MainWindow).DbRead.GetAllGmina();
                acbGmina.ItemsSource = Gminy;
                PreselectAutoCompleteBox();
            }
        }

        private void PreselectAutoCompleteBox()
        {
            if (PrevAddress == null || acbGmina.SelectedItem != null)
                return;
            
            var selected = Gminy.Find(item => item.Id == PrevAddress.GminaId);
            acbGmina.SelectedItem = selected;

            if (Keyboard.FocusedElement is TextBox)
            {
                ((TextBox)Keyboard.FocusedElement).SelectAll();
            }
        }

        private void SelectGminaDirectionPage_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab)
            {
                UpdatePage();
            }

            if (!(Keyboard.FocusedElement is TextBox))
            {
                if (e.Key == Key.Enter)
                {
                    if (SelectedGmina != null)
                    {
                        ((MainWindow)Application.Current.MainWindow).UpdateTransactionAddressGmina(SelectedGmina);
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
                    acbGmina.SelectedItem = null;
                    acbGmina.Text = "";
                    BindGminy();
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
                        ((MainWindow)Application.Current.MainWindow).UpdateTransactionDbTypeManualEntry(DisplayPage.InsertGmina);
                    }
                    if (dbType == AddressDbType.ManualEntry)
                    {
                        ((MainWindow)Application.Current.MainWindow).UpdateTransactionDbType(AddressDbType.Teryt);
                    }
                    acbGmina.SelectedItem = null;
                    acbGmina.Text = "";
                    BindGminy();
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
            else
            {
                if (e.Key == Key.Enter)
                {
                    PreselectAutoCompleteBox();
                }
            }
        }

        private void acbGmina_SelectionChanged(object sender, SelectionChangedEventArgs e)
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
            else if (SelectedGmina == null)
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
            Keyboard.Focus(acbGmina);
            //acbGmina.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            acbGmina.Focus();

            if (Keyboard.FocusedElement is TextBox)
            {
                ((TextBox)Keyboard.FocusedElement).SelectAll();
            }
        }

        private void PageLoaded(object sender, RoutedEventArgs e)
        {
            BindGminy();
            BindPage();

            SetFocusOnAutoCompleteBox();
            UpdatePage();

            acbGmina.SelectedItem = null;
            acbGmina.Text = "";
            PreselectAutoCompleteBox();

            Application.Current.MainWindow.KeyUp -= new KeyEventHandler(SelectGminaDirectionPage_KeyUp);
            Application.Current.MainWindow.KeyUp += new KeyEventHandler(SelectGminaDirectionPage_KeyUp);
        }

        private void PageUnload(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.KeyUp -= new KeyEventHandler(SelectGminaDirectionPage_KeyUp);
        }

        private void pfCustomer_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}

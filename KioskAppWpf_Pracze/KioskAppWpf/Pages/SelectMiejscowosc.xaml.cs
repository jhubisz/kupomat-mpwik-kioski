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
    public partial class SelectMiejscowosc : Page
    {
        public List<Miejscowosc> Miejscowosci { get; set; }
        public Miejscowosc SelectedMiejscowosc { get; set; }
        public AutoCompleteFilterPredicate<object> MiejscowoscFilter
        {
            get
            {
                return (searchText, obj) =>
                    (obj as Miejscowosc).Name.ToLower().Contains(searchText.ToLower())
                    || (obj as Miejscowosc).NameWithoutPl.ToLower().Contains(searchText.ToLower());
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

        public SelectMiejscowosc()
        {
            InitializeComponent();
            //BindMiasta();
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
                gGoToManualEntry.Visibility = Visibility.Hidden;
            }
            if (dbType == AddressDbType.ManualEntry)
            {
                tbGoToRegularCustomers.Text = "- Przejdź do stałych klientów";
                tbGoToManualEntry.Text = "- Przejdź do bazy Teryt";
                gGoToManualEntry.Visibility = Visibility.Hidden;
            }
        }

        private void BindMiasta()
        {
            if (CurrentAddress.DbType == AddressDbType.RegularCustomers)
            {
                Miejscowosci = ((MainWindow)Application.Current.MainWindow).DbRead.GetAllRegularMiasta(CurrentAddress.GminaId, Transaction.Card.Customer.Id);
                acbMiejscowosc.ItemsSource = Miejscowosci;
                PreselectAutoCompleteBox();
            }
            else
            {
                Miejscowosci = ((MainWindow)Application.Current.MainWindow).DbRead.GetAllMiasta(CurrentAddress.GminaId);
                acbMiejscowosc.ItemsSource = Miejscowosci;
                PreselectAutoCompleteBox();
            }
        }

        private void PreselectAutoCompleteBox()
        {
            if (PrevAddress == null || acbMiejscowosc.SelectedItem != null)
                return;

            if (PrevAddress.GminaId == CurrentAddress.GminaId)
            {
                var selected = Miejscowosci.Find(item => item.Id == PrevAddress.MiejscowoscId);
                acbMiejscowosc.SelectedItem = selected;
            }

            if (Keyboard.FocusedElement is TextBox)
            {
                ((TextBox)Keyboard.FocusedElement).SelectAll();
            }
        }

        private void SelectMiejscowoscPage_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab)
            {
                UpdatePage();
            }

            if (!(Keyboard.FocusedElement is TextBox))
            {
                if (e.Key == Key.Enter)
                {
                    if (SelectedMiejscowosc != null)
                    {
                        ((MainWindow)Application.Current.MainWindow).UpdateTransactionAddressMiejscowosc(SelectedMiejscowosc);
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
                    acbMiejscowosc.SelectedItem = null;
                    acbMiejscowosc.Text = "";
                    BindMiasta();
                    SetFocusOnAutoCompleteBox();
                    BindPage();
                    UpdatePage();
                }

                if (e.Key == Key.D2 && false)
                {
                    var dbType = CurrentAddress.DbType;
                    if (dbType == AddressDbType.RegularCustomers)
                    {
                        return;
                    }
                    if (dbType == AddressDbType.Teryt)
                    {
                        ((MainWindow)Application.Current.MainWindow).UpdateTransactionDbTypeManualEntry(DisplayPage.InsertMiejscowosc);
                    }
                    if (dbType == AddressDbType.ManualEntry)
                    {
                        ((MainWindow)Application.Current.MainWindow).UpdateTransactionDbType(AddressDbType.Teryt);
                    }
                    acbMiejscowosc.SelectedItem = null;
                    acbMiejscowosc.Text = "";
                    BindMiasta();
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

        private void acbMiejscowosc_SelectionChanged(object sender, SelectionChangedEventArgs e)
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
            else if (SelectedMiejscowosc == null)
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
            Keyboard.Focus(acbMiejscowosc);
            acbMiejscowosc.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));

            if (Keyboard.FocusedElement is TextBox)
            {
                ((TextBox)Keyboard.FocusedElement).SelectAll();
            }
        }

        private void PageLoaded(object sender, RoutedEventArgs e)
        {
            BindMiasta();
            BindPage();

            SetFocusOnAutoCompleteBox();
            UpdatePage();

            acbMiejscowosc.SelectedItem = null;
            acbMiejscowosc.Text = "";
            PreselectAutoCompleteBox();

            Application.Current.MainWindow.KeyUp -= new KeyEventHandler(SelectMiejscowoscPage_KeyUp);
            Application.Current.MainWindow.KeyUp += new KeyEventHandler(SelectMiejscowoscPage_KeyUp);
        }

        private void PageUnload(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.KeyUp -= new KeyEventHandler(SelectMiejscowoscPage_KeyUp);
        }
    }
}

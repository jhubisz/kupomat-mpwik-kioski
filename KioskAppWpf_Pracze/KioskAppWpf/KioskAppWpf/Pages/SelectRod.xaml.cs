using DbCommunication.Entities;
using DbCommunication.Enums;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace KioskAppWpf.Pages
{
    /// <summary>
    /// Interaction logic for SummaryPage.xaml
    /// </summary>
    public partial class SelectRod : Page
    {
        public List<Rod> Rody { get; set; }
        public Rod SelectedRod { get; set; }
        public AutoCompleteFilterPredicate<object> RodFilter
        {
            get
            {
                return (searchText, obj) =>
                    (obj as Rod).Name.ToLower().Contains(searchText.ToLower());
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

        public SelectRod()
        {
            InitializeComponent();
            //BindRody();
            DataContext = this;
            //BindPage();
            //Application.Current.MainWindow.KeyUp -= new KeyEventHandler(SelectRodPage_KeyUp);
            //Application.Current.MainWindow.KeyUp += new KeyEventHandler(SelectRodPage_KeyUp);
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
            if (dbType == AddressDbType.Rod)
            {
                tbGoToRegularCustomers.Text = "- Brak ROD w bazie";
                gGoToManualEntry.Visibility = Visibility.Hidden;
            }
        }

        private void BindRody()
        {
            Rody = ((MainWindow)Application.Current.MainWindow).DbRead.GetAllRod();
            acbRod.ItemsSource = Rody;
        }

        private void PreselectAutoCompleteBox()
        {
            if (PrevAddress == null)
                return;

            var selected = Rody.Find(item => item.Id == PrevAddress.RodId);
            acbRod.SelectedItem = selected;

            if (Keyboard.FocusedElement is TextBox)
            {
                ((TextBox)Keyboard.FocusedElement).SelectAll();
            }
        }

        private void SelectRodPage_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab)
            {
                UpdatePage();
            }

            if (!(Keyboard.FocusedElement is TextBox))
            {
                if (e.Key == Key.Enter)
                {
                    if (SelectedRod != null)
                    {
                        ((MainWindow)Application.Current.MainWindow).UpdateTransactionAddressRod(SelectedRod);
                    }
                }

                if (e.Key == Key.R)
                {
                    if (SelectedRod == null)
                    {
                        ((MainWindow)Application.Current.MainWindow).UpdateTransactionNoRod();
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

        private void acbRod_SelectionChanged(object sender, SelectionChangedEventArgs e)
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
            else if (SelectedRod == null)
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
            Keyboard.Focus(acbRod);
            acbRod.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));

            if (Keyboard.FocusedElement is TextBox)
            {
                ((TextBox)Keyboard.FocusedElement).SelectAll();
            }
        }

        private void PageLoaded(object sender, RoutedEventArgs e)
        {
            BindRody();
            BindPage();
                
            SetFocusOnAutoCompleteBox();
            UpdatePage();

            acbRod.SelectedItem = null;
            acbRod.Text = "";
            PreselectAutoCompleteBox();

            Application.Current.MainWindow.KeyUp -= new KeyEventHandler(SelectRodPage_KeyUp);
            Application.Current.MainWindow.KeyUp += new KeyEventHandler(SelectRodPage_KeyUp);
        }

        private void PageUnload(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.KeyUp -= new KeyEventHandler(SelectRodPage_KeyUp);
        }
    }
}

using DbCommunication;
using DbCommunication.Entities;
using KioskAppWpf.Controls;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace KioskAppWpf.Pages
{
    /// <summary>
    /// Interaction logic for SummaryPage.xaml
    /// </summary>
    public partial class CargoTypePage : Page
    {
        private const int maxItems = 8;
        private int pageNo = 1;
        private List<CargoType> cargoTypes;

        private List<CargoType> CargoTypes
        {
            get
            {
                if (cargoTypes == null)
                    cargoTypes = GetCargoTypes();

                return cargoTypes;
            }
        }
        private Dictionary<int, CargoType> BindedCargoTypes;

        private bool NextPageAvailable
        {
            get
            {
                return CargoTypes.Count > pageNo * maxItems;
            }
        }
        private bool PrevPageAvailable
        {
            get
            {
                return pageNo > 1;
            }
        }

        public CargoTypePage()
        {
            InitializeComponent();
            Application.Current.MainWindow.KeyUp -= new KeyEventHandler(CargoTypePage_KeyUp);
            Application.Current.MainWindow.KeyUp += new KeyEventHandler(CargoTypePage_KeyUp);

            BindCargoTypes();
            BindCustomer();
        }

        private void BindCustomer()
        {
            var transaction = ((MainWindow)Application.Current.MainWindow).Transaction;

            pfCustomer.Transaction = transaction;
        }

        private void BindCargoTypes()
        {
            BindedCargoTypes = new Dictionary<int, CargoType>();
            spCargoTypesItems.Children.Clear();
            var index = 0;

            foreach (var type in CargoTypes)
            {
                index++;

                if (index < (pageNo - 1) * maxItems) continue;
                if (index > maxItems * pageNo) break;

                var keyIndexForItem = index - ((pageNo - 1) * (maxItems - 1));

                BindedCargoTypes.Add(keyIndexForItem, type);

                CargoTypeItem item = new CargoTypeItem(type, keyIndexForItem.ToString());
                item.Margin = new Thickness(0, 0, 0, 24);
                spCargoTypesItems.Children.Add(item);
            }
        }

        private List<CargoType> GetCargoTypes()
        {
            var dbRead = ((MainWindow)Application.Current.MainWindow).DbRead;
            return dbRead.GetCargoTypes();
        }

        private void CargoTypePage_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key >= Key.D1 && e.Key <= IntToKey(2))
            {
                ((MainWindow)Application.Current.MainWindow).UpdateTransactionCargoType(BindedCargoTypes[KeyToInt(e.Key)]);
            }
        }

        private Key IntToKey(int number)
        {
            switch (number)
            {
                case 1: return Key.D1;
                case 2: return Key.D2;
                case 3: return Key.D3;
                case 4: return Key.D4;
                case 5: return Key.D5;
                case 6: return Key.D6;
                case 7: return Key.D7;
                case 8: return Key.D8;
                case 9: return Key.D9;
            }
            return Key.None;
        }
        private int KeyToInt(Key key)
        {
            switch (key)
            {
                case Key.D1: return 1;
                case Key.D2: return 2;
                case Key.D3: return 3;
                case Key.D4: return 4;
                case Key.D5: return 5;
                case Key.D6: return 6;
                case Key.D7: return 7;
                case Key.D8: return 8;
                case Key.D9: return 9;
            }
            return 0;
        }

        private void PageLoad(object sender, RoutedEventArgs e)
        {
            BindCustomer();
            Application.Current.MainWindow.KeyUp -= new KeyEventHandler(CargoTypePage_KeyUp);
            Application.Current.MainWindow.KeyUp += new KeyEventHandler(CargoTypePage_KeyUp);
        }
        private void PageUnload(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.KeyUp -= new KeyEventHandler(CargoTypePage_KeyUp);
        }
    }
}

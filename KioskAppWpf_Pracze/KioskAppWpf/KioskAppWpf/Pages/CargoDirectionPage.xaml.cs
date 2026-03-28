using DbCommunication.Entities;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace KioskAppWpf.Pages
{
    /// <summary>
    /// Interaction logic for SummaryPage.xaml
    /// </summary>
    public partial class CargoDirectionPage : Page
    {
        public CargoDirectionPage()
        {
            InitializeComponent();
            Application.Current.MainWindow.KeyDown += new KeyEventHandler(CargoDirectionPage_KeyDown);
        }

        private void CargoDirectionPage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.D1)
            {
                ((MainWindow)Application.Current.MainWindow).UpdateTransactionCargoDirection(CargoDirection.In);
            }
            if (e.Key == Key.D2)
            {
                ((MainWindow)Application.Current.MainWindow).UpdateTransactionCargoDirection(CargoDirection.Out);
            }
        }

        private void PageUnload(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.KeyDown -= new KeyEventHandler(CargoDirectionPage_KeyDown);
        }
    }
}

using System.ComponentModel;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace KioskAppWpf.Pages
{
    /// <summary>
    /// Interaction logic for SummaryPage.xaml
    /// </summary>
    public partial class WeightPage : Page
    {
        private static BackgroundWorker workerWeightStatus = new BackgroundWorker();
        private decimal Weight { get; set; }

        public WeightPage()
        {
            InitializeComponent();
            RunWeightWorker();
            Application.Current.MainWindow.KeyDown += new KeyEventHandler(WeightPage_KeyDown);

            CheckWeight();
        }

        private void RunWeightWorker()
        {
            workerWeightStatus.DoWork += WorkerWeightWork;
            workerWeightStatus.ProgressChanged += WorkerWeightProgressChanged;
            workerWeightStatus.WorkerReportsProgress = true;

            if (!workerWeightStatus.IsBusy)
                workerWeightStatus.RunWorkerAsync();
        }

        private void CheckWeight()
        {
            Weight = ((MainWindow)Application.Current.MainWindow).DevicesState.Weight.WeightInMg;
            tbWeight.Text = Weight.ToString("C3", CultureInfo.CreateSpecificCulture("pl-PL")).Replace("zł", "Mg");
        }

        private void WorkerWeightWork(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                workerWeightStatus.ReportProgress(0);
                Thread.Sleep(1000);
            }
        }

        private void WorkerWeightProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            CheckWeight();
        }

        private void WeightPage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ((MainWindow)Application.Current.MainWindow).UpdateTransactionCargoWeight(Weight);
            }
        }

        private void PageUnload(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.KeyDown -= new KeyEventHandler(WeightPage_KeyDown);
        }
    }
}

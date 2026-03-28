using DbCommunication.Entities;
using DbCommunication.Enums;
using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace KioskAppWpf.Pages
{
    /// <summary>
    /// Interaction logic for SummaryPage.xaml
    /// </summary>
    public partial class RfidCardPage : Page
    {
        private static BackgroundWorker workerUpdater = new BackgroundWorker();

        public RfidCardPage()
        {
            InitializeComponent();

            RunWorkerUpdater();
        }

        #region Update worker
        private void RunWorkerUpdater()
        {
            workerUpdater.DoWork -= WorkerUpdaterWork;
            workerUpdater.DoWork += WorkerUpdaterWork;
            workerUpdater.ProgressChanged -= WorkerUpdaterChanged;
            workerUpdater.ProgressChanged += WorkerUpdaterChanged;
            workerUpdater.WorkerReportsProgress = true;

            if (!workerUpdater.IsBusy)
                workerUpdater.RunWorkerAsync();
        }
        private void WorkerUpdaterWork(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                var kioskConfig = Dispatcher.Invoke((Func<KioskConfiguration>)(() => ((MainWindow)Application.Current.MainWindow).KioskConf));

                if (kioskConfig.Open == KioskOpenState.Open && gPageGrid.Visibility == Visibility.Hidden)
                {
                    workerUpdater.ReportProgress(0, true);
                }
                if (kioskConfig.Open == KioskOpenState.Closed && gPageGrid.Visibility == Visibility.Visible)
                {
                    workerUpdater.ReportProgress(0, false);
                }

                Thread.Sleep(1000);
            }
        }

        private void WorkerUpdaterChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.UserState is bool)
            {
                if ((bool)e.UserState)
                {
                    gPageGrid.Visibility = Visibility.Visible;
                }
                else
                {
                    gPageGrid.Visibility = Visibility.Hidden;
                }
            }
        }
        #endregion
    }
}

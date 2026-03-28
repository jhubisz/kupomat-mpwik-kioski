using DbCommunication.Entities;
using DbCommunication.Enums;
using KioskAppWpf.Enums;
using OpcCommunication;
using OpcCommunication.Devices;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace KioskAppWpf.Pages.Errors
{
    /// <summary>
    /// Interaction logic for ErrorPage.xaml
    /// </summary>
    public partial class GateOpenPage : Page
    {
        private int _SequenceErrorPageDisplayTime
        {
            get
            {
                return Int32.Parse(ConfigurationManager.AppSettings["_SequenceErrorPageDisplayTime"]);
            }
        }
        private static BackgroundWorker workerUpdater = new BackgroundWorker();
        public bool TransactionSaving { get; set; }
        private DateTime ErrorDisplayTimer { get; set; }

        public GateOpenPage()
        {
            InitializeComponent();

            tbTakeRecipt.Visibility = Visibility.Hidden;
            tbPrintingBlocked.Visibility = Visibility.Hidden;

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
                var transaction = Dispatcher.Invoke((Func<Transaction>)(() => ((MainWindow)Application.Current.MainWindow).Transaction));
                var opcManager = Dispatcher.Invoke((Func<OpcManager>)(() => ((MainWindow)Application.Current.MainWindow).OpcManager));
                var opcDevicesState = Dispatcher.Invoke((Func<DevicesState>)(() => ((MainWindow)Application.Current.MainWindow).DevicesState));
                var kioskConfig = Dispatcher.Invoke((Func<KioskConfiguration>)(() => ((MainWindow)Application.Current.MainWindow).KioskConf));

                if (opcDevicesState != null
                    && opcManager != null && opcManager.Client != null && opcManager.Client.IsConnected)
                {
                    var printerError = Dispatcher.Invoke((Func<bool>)(() => ((MainWindow)Application.Current.MainWindow).PrinterErrorState));
                    var printeroutOfPaperError = Dispatcher.Invoke((Func<bool>)(() => ((MainWindow)Application.Current.MainWindow).PrinterOutOfPaperErrorState));

                    if ((opcDevicesState.Flow.SeqErrDev || (transaction != null && transaction.ActualAmount > 0)) 
                        && tbTakeRecipt.Visibility == Visibility.Hidden && !TransactionSaving
                        && !printerError && !printeroutOfPaperError && !kioskConfig.KioskBlockages[KioskBlockageType.Printer])
                    {
                        TransactionSaving = true;
                        ErrorDisplayTimer = DateTime.Now;
                        workerUpdater.ReportProgress(0, PrinterVisibility.TakeReciptVisible);
                    }
                    if ((opcDevicesState.Flow.SeqErrDev || (transaction != null && transaction.ActualAmount > 0))
                        && tbTakeRecipt.Visibility == Visibility.Hidden && !TransactionSaving
                        && kioskConfig.KioskBlockages[KioskBlockageType.Printer])
                    {
                        TransactionSaving = true;
                        ErrorDisplayTimer = DateTime.Now;
                        workerUpdater.ReportProgress(0, PrinterVisibility.PrinterBlockedVisible);
                    }
                    if (!(opcDevicesState.Flow.SeqErrDev || (transaction != null && transaction.ActualAmount > 0))
                        && (tbTakeRecipt.Visibility == Visibility.Visible || tbPrintingBlocked.Visibility == Visibility.Visible)
                        && TransactionSaving && ErrorDisplayTimer.AddSeconds(_SequenceErrorPageDisplayTime) < DateTime.Now)
                    {
                        TransactionSaving = false;
                        workerUpdater.ReportProgress(0, PrinterVisibility.ReciptInfoHidden);
                    }
                }

                Thread.Sleep(1000);
            }
        }

        private void WorkerUpdaterChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.UserState is PrinterVisibility)
            {
                if ((PrinterVisibility)e.UserState == PrinterVisibility.TakeReciptVisible)
                {
                    tbTakeRecipt.Visibility = Visibility.Visible;
                    tbPrintingBlocked.Visibility = Visibility.Hidden;
                }
                if ((PrinterVisibility)e.UserState == PrinterVisibility.PrinterBlockedVisible)
                {
                    tbTakeRecipt.Visibility = Visibility.Hidden;
                    tbPrintingBlocked.Visibility = Visibility.Visible;
                }
                if ((PrinterVisibility)e.UserState == PrinterVisibility.ReciptInfoHidden)
                {
                    tbTakeRecipt.Visibility = Visibility.Hidden;
                    tbPrintingBlocked.Visibility = Visibility.Hidden;
                }
            }
        }
        #endregion
    }
}

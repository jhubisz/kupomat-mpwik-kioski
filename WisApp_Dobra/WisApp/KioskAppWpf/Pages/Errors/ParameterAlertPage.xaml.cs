using System.ComponentModel;
using System.Windows.Controls;
using System.Windows;
using System.Threading;
using System.Configuration;
using System;
using OpcCommunication.Devices;
using KioskAppWpf.Enums;
using DbCommunication.Entities;
using OpcCommunication;
using DbCommunication.Enums;

namespace KioskAppWpf.Pages.Errors
{
    /// <summary>
    /// Interaction logic for ErrorPage.xaml
    /// </summary>
    public partial class ParameterAlertPage : Page
    {
        private int _SequenceErrorPageDisplayTime
        {
            get
            {
                return Int32.Parse(ConfigurationManager.AppSettings["_SequenceErrorPageDisplayTime"]);
            }
        }
        private DateTime TimerStartTime
        {
            get; set;
        }
        public bool TransactionSaving { get; set; }
        private DateTime ErrorDisplayTimer { get; set; }
        public bool PageActive { get; set; }

        private static BackgroundWorker workerWaitStatus = new BackgroundWorker();

        public ParameterAlertPage()
        {
            InitializeComponent();
        }

        private void RunWaitWorker()
        {
            TimerStartTime = DateTime.Now;
            workerWaitStatus.DoWork -= WorkerWaitWork;
            workerWaitStatus.DoWork += WorkerWaitWork;
            workerWaitStatus.ProgressChanged -= WorkerWaitProgressChanged;
            workerWaitStatus.ProgressChanged += WorkerWaitProgressChanged;
            workerWaitStatus.WorkerReportsProgress = true;

            if (!workerWaitStatus.IsBusy)
                workerWaitStatus.RunWorkerAsync();
        }

        private void WorkerWaitWork(object sender, DoWorkEventArgs e)
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
                        workerWaitStatus.ReportProgress(0, PrinterVisibility.TakeReciptVisible);
                    }
                    if ((opcDevicesState.Flow.SeqErrDev || (transaction != null && transaction.ActualAmount > 0))
                        && tbTakeRecipt.Visibility == Visibility.Hidden && !TransactionSaving
                        && kioskConfig.KioskBlockages[KioskBlockageType.Printer])
                    {
                        TransactionSaving = true;
                        ErrorDisplayTimer = DateTime.Now;
                        workerWaitStatus.ReportProgress(0, PrinterVisibility.PrinterBlockedVisible);
                    }
                    if (!(opcDevicesState.Flow.SeqErrDev || (transaction != null && transaction.ActualAmount > 0))
                        && (tbTakeRecipt.Visibility == Visibility.Visible || tbPrintingBlocked.Visibility == Visibility.Visible)
                        && TransactionSaving && ErrorDisplayTimer.AddSeconds(_SequenceErrorPageDisplayTime) < DateTime.Now)
                    {
                        TransactionSaving = false;
                        workerWaitStatus.ReportProgress(0, PrinterVisibility.ReciptInfoHidden);
                    }
                }

                if (TimerStartTime.AddSeconds(_SequenceErrorPageDisplayTime) < DateTime.Now
                    && opcDevicesState.Flow.DumpStartTime == null && opcDevicesState.Flow.SeqAckTime == null
                    && PageActive)
                {
                    workerWaitStatus.ReportProgress(0, true);
                    break;
                }

                Thread.Sleep(1000);
            }
        }
        private void WorkerWaitProgressChanged(object sender, ProgressChangedEventArgs e)
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

            if (e.UserState is bool && (bool)e.UserState)
            {
                ((MainWindow)Application.Current.MainWindow).UpdateThankYou();
            }
        }

        private void PageLoad(object sender, RoutedEventArgs e)
        {
            DataContext = this;
            RunWaitWorker();

            tbTakeRecipt.Visibility = Visibility.Hidden;
            tbPrintingBlocked.Visibility = Visibility.Hidden;

            TimerStartTime = DateTime.Now;
            PageActive = true;
        }
        private void PageUnload(object sender, RoutedEventArgs e)
        {
            PageActive = false;

            tbTakeRecipt.Visibility = Visibility.Hidden;
            tbPrintingBlocked.Visibility = Visibility.Hidden;
        }
    }
}

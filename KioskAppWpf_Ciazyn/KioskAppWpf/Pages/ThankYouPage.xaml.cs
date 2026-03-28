using DbCommunication.Entities;
using DbCommunication.Enums;
using KioskAppWpf.Classes;
using KioskAppWpf.Enums;
using OpcCommunication;
using OpcCommunication.Devices;
using System;
using System.ComponentModel;
using System.Configuration;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace KioskAppWpf.Pages
{
    /// <summary>
    /// Interaction logic for SummaryPage.xaml
    /// </summary>
    public partial class ThankYouPage : Page
    {
        private int _SequenceThankYouPageDisplayTime
        {
            get
            {
                return Int32.Parse(ConfigurationManager.AppSettings["_SequenceThankYouPageDisplayTime"]);
            }
        }
        private DateTime TimerStartTime
        {
            get; set;
        }

        private static AbortableBackgroundWorker workerWaitStatus = new AbortableBackgroundWorker();

        public ThankYouPage()
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
                var transactionExists = transaction != null;
                var opcManager = Dispatcher.Invoke((Func<OpcManager>)(() => ((MainWindow)Application.Current.MainWindow).OpcManager));
                var opcDevicesState = Dispatcher.Invoke((Func<DevicesState>)(() => ((MainWindow)Application.Current.MainWindow).DevicesState));
                var kioskConfig = Dispatcher.Invoke((Func<KioskConfiguration>)(() => ((MainWindow)Application.Current.MainWindow).KioskConf));

                if (opcDevicesState != null
                    && opcManager != null && opcManager.Client != null && opcManager.Client.IsConnected)
                {
                    var printerError = Dispatcher.Invoke((Func<bool>)(() => ((MainWindow)Application.Current.MainWindow).PrinterErrorState));
                    var printeroutOfPaperError = Dispatcher.Invoke((Func<bool>)(() => ((MainWindow)Application.Current.MainWindow).PrinterOutOfPaperErrorState));

                    if ((opcDevicesState.Flow.SeqErrDev || transaction != null)
                        && tbTakeRecipt.Visibility == Visibility.Hidden 
                        && !printerError && !printeroutOfPaperError && !kioskConfig.KioskBlockages[KioskBlockageType.Printer])
                    {
                        workerWaitStatus.ReportProgress(0, PrinterVisibility.TakeReciptVisible);
                    }
                    if ((opcDevicesState.Flow.SeqErrDev || transaction != null)
                        && tbTakeRecipt.Visibility == Visibility.Hidden
                        && kioskConfig.KioskBlockages[KioskBlockageType.Printer])
                    {
                        workerWaitStatus.ReportProgress(0, PrinterVisibility.PrinterBlockedVisible);
                    }
                }

                if (TimerStartTime.AddSeconds(_SequenceThankYouPageDisplayTime) < DateTime.Now)
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
                    tbPrinterIcon.Visibility = Visibility.Visible;
                    tbPrintingBlocked.Visibility = Visibility.Hidden;
                }
                if ((PrinterVisibility)e.UserState == PrinterVisibility.PrinterBlockedVisible)
                {
                    tbTakeRecipt.Visibility = Visibility.Hidden;
                    tbPrinterIcon.Visibility = Visibility.Hidden;
                    tbPrintingBlocked.Visibility = Visibility.Visible;
                }
            }

            if (e.UserState is bool && (bool)e.UserState)
            {
                ((MainWindow)Application.Current.MainWindow).UpdateThankYou();
            }
        }

        private void PageLoad(object sender, RoutedEventArgs e)
        {
            RunWaitWorker();
            
            tbTakeRecipt.Visibility = Visibility.Hidden;
            tbPrintingBlocked.Visibility = Visibility.Hidden;
        }
        private void PageUnload(object sender, RoutedEventArgs e)
        {
            tbTakeRecipt.Visibility = Visibility.Hidden;
            tbPrintingBlocked.Visibility = Visibility.Hidden;
        }
    }
}

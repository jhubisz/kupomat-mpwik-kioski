using System.ComponentModel;
using System.Windows.Controls;
using System.Windows;
using System.Threading;
using System.Configuration;
using System;
using OpcCommunication.Devices;
using KioskAppWpf.Classes;

namespace KioskAppWpf.Pages.Errors
{
    /// <summary>
    /// Interaction logic for ErrorPage.xaml
    /// </summary>
    public partial class NoFlowPage : Page
    {
        private int _SequenceNoFlowPageDisplayTime
        {
            get
            {
                return Int32.Parse(ConfigurationManager.AppSettings["_SequenceNoFlowPageDisplayTime"]);
            }
        }
        private DateTime TimerStartTime
        {
            get; set;
        }
        private static AbortableBackgroundWorker workerWaitStatus = new AbortableBackgroundWorker();

        public NoFlowPage()
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

        private void WorkerWaitProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ((MainWindow)Application.Current.MainWindow).UpdateThankYou();
        }

        private void WorkerWaitWork(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                var opcDevicesState = Dispatcher.Invoke((Func<DevicesState>)(() => ((MainWindow)Application.Current.MainWindow).DevicesState));
                if (TimerStartTime.AddSeconds(_SequenceNoFlowPageDisplayTime) < DateTime.Now
                    && opcDevicesState.Flow.DumpStartTime == null && opcDevicesState.Flow.SeqAckTime == null)
                {
                    workerWaitStatus.ReportProgress(0);
                    break;
                }
                Thread.Sleep(1000);
            }
        }

        private void PageLoad(object sender, RoutedEventArgs e)
        {
            RunWaitWorker();
        }
        private void PageUnload(object sender, RoutedEventArgs e)
        {
            //workerWaitStatus.Abort();
            //workerWaitStatus.Dispose();
        }
    }
}

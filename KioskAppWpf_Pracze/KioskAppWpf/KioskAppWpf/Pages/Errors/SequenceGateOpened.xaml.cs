using System.ComponentModel;
using System.Windows.Controls;
using System.Windows;
using System.Threading;
using System.Configuration;
using System;
using OpcCommunication.Devices;

namespace KioskAppWpf.Pages.Errors
{
    /// <summary>
    /// Interaction logic for ErrorPage.xaml
    /// </summary>
    public partial class SequenceGateOpened : Page
    {
        private int _SequenceNoFlowPageDisplayTime
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
        private static BackgroundWorker workerWaitStatus = new BackgroundWorker();

        public SequenceGateOpened()
        {
            InitializeComponent();
            RunWaitWorker();
        }

        private void RunWaitWorker()
        {
            TimerStartTime = DateTime.Now;
            workerWaitStatus.DoWork += WorkerWaitWork;
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
    }
}

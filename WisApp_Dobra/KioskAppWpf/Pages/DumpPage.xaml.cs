using DbCommunication.Entities;
using DbCommunication.Enums;
using OpcCommunication.Devices;
using System;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace KioskAppWpf.Pages
{
    /// <summary>
    /// Interaction logic for SummaryPage.xaml
    /// </summary>
    public partial class DumpPage : Page
    {
        private bool _chztOn
        {
            get
            {
                return ConfigurationManager.AppSettings["_chztOn"] == "1";
            }
        }

        public Transaction Transaction
        {
            get
            {
                return ((MainWindow)Application.Current.MainWindow).Transaction;
            }
        }
        public Company CurrentCompany
        {
            get
            {
                return ((MainWindow)Application.Current.MainWindow).CurrentTransactionCompany;
            }
        }
        public CustomerAddress CurrentAddress
        {
            get
            {
                return ((MainWindow)Application.Current.MainWindow).CurrentTransactionAddress;
            }
        }

        private static BackgroundWorker workerFlowStatus = new BackgroundWorker();

        public DumpPage()
        {
            InitializeComponent();
            DataContext = this;
            //BindPage();
            //Application.Current.MainWindow.KeyUp += new KeyEventHandler(DumpPage_KeyUp);
        }

        private void BindPage()
        {
            spChzt.Visibility = _chztOn ? Visibility.Visible : Visibility.Collapsed;
            ClearFlowData();
            pfCustomer.Transaction = Transaction;
            RunFlowWorker();
        }

        #region Flow worker
        private void RunFlowWorker()
        {
            workerFlowStatus.DoWork -= WorkerFlowWork;
            workerFlowStatus.DoWork += WorkerFlowWork;
            workerFlowStatus.ProgressChanged -= WorkerFlowProgressChanged;
            workerFlowStatus.ProgressChanged += WorkerFlowProgressChanged;
            workerFlowStatus.WorkerReportsProgress = true;

            if (!workerFlowStatus.IsBusy)
                workerFlowStatus.RunWorkerAsync();
        }

        private void WorkerFlowWork(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                var opcDevicesState = Dispatcher.Invoke((Func<DevicesState>)(() => ((MainWindow)Application.Current.MainWindow).DevicesState));
                var currentFlowData = opcDevicesState.Flow;
                workerFlowStatus.ReportProgress(0, currentFlowData);
                Thread.Sleep(1000);
            }
        }

        private void WorkerFlowProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.UserState is Flow)
            {
                BindFlowData((Flow)e.UserState);
                BindWaterForDriver((Flow)e.UserState);
            }
        }

        private void ClearFlowData()
        {
            tbSewageType.Text = "";
            tbDeclaredAmount.Text = "";

            tbFlow.Text = "";
            tbCurrentAmount.Text = "";

            tbPhMeter.Text = "";
            tbTempMeter.Text = "";
        }
        private void BindFlowData(Flow flow)
        {
            if (Transaction == null || Transaction.Cargo == null) return;

            tbSewageType.Text = Transaction.Cargo.Name;
            tbDeclaredAmount.Text = Transaction.CustomerAddresses.Sum(item => item.DeclaredAmount).ToString("0.00") + " m3";

            //Transaction.ActualAmount = flow.FlowFlowCnt;
            //Transaction.ScheduledSample = flow.CheckScheduledSampleTaken();
            //Transaction.AlarmSample = flow.CheckAlarmSampleTaken();
            //Transaction.Parameters = flow.CheckTransactionParameters();
            tbFlow.Text = flow.FlowFlowCurr.ToString("0.00") + " m3/h";
            tbCurrentAmount.Text = flow.FlowFlowCnt.ToString("0.00") + " m3";

            var kioskConfig = Dispatcher.Invoke((Func<KioskConfiguration>)(() => ((MainWindow)Application.Current.MainWindow).KioskConf));

            if (!kioskConfig.KioskBlockages[KioskBlockageType.MeasuringPh])
            {
                tbPhMeter.Text = flow.FlowpHCurr.ToString("0.0") + " pH";
            }
            if (!kioskConfig.KioskBlockages[KioskBlockageType.MeasugingTemperature])
            {
                tbTempMeter.Text = flow.FlowTempCurr.ToString("0.0") + " °C";
            }
            if (!kioskConfig.KioskBlockages[KioskBlockageType.MeasuringChzt])
            {
                tbChztMeter.Text = flow.FlowChztCurr.ToString("0.0") + " mg/l";
            }
        }
        private void BindWaterForDriver(Flow flow)
        {
            var kioskConfig = Dispatcher.Invoke((Func<KioskConfiguration>)(() => ((MainWindow)Application.Current.MainWindow).KioskConf));
        }
        #endregion

        private void DumpPage_KeyUp(object sender, KeyEventArgs e)
        {
        }

        private void PageLoaded(object sender, RoutedEventArgs e)
        {
            BindPage();
            
            Application.Current.MainWindow.KeyUp -= new KeyEventHandler(DumpPage_KeyUp);
            Application.Current.MainWindow.KeyUp += new KeyEventHandler(DumpPage_KeyUp);
        }

        private void PageUnload(object sender, RoutedEventArgs e)
        {
            ClearFlowData();

            Application.Current.MainWindow.KeyUp -= new KeyEventHandler(DumpPage_KeyUp);
        }
    }
}

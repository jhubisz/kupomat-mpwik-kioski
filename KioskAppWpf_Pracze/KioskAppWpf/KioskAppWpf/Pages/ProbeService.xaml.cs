using DbCommunication.Entities;
using DbCommunication.Enums;
using KioskAppWpf.Controls;
using KioskAppWpf.Enums;
using OpcCommunication.Devices;
using PrinterCommunication;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace KioskAppWpf.Pages
{
    enum PageCloseTimeout
    {
        ClosePage
    }

    /// <summary>
    /// Interaction logic for SummaryPage.xaml
    /// </summary>
    public partial class ProbeService : Page
    {
        private int _ProbeResetPageDisplayTimeout
        {
            get
            {
                return Int32.Parse(ConfigurationManager.AppSettings["_ProbeResetPageDisplayTimeout"]);
            }
        }
        public SampleTrayInfo TrayInfo { get; set; }
        public bool ResetButtonActive { get; set; }
        private static BackgroundWorker workerProbeStatus = new BackgroundWorker();

        public bool PageActive { get; set; }
        public DateTime PageDisplayTimeout { get; set; }

        public ProbeService()
        {
            InitializeComponent();
            RunFlowWorker();
        }
        
        #region Flow worker
        private void RunFlowWorker()
        {
            workerProbeStatus.DoWork -= WorkerProbeWork;
            workerProbeStatus.DoWork += WorkerProbeWork;
            workerProbeStatus.ProgressChanged -= WorkerProbeProgressChanged;
            workerProbeStatus.ProgressChanged += WorkerProbeProgressChanged;
            workerProbeStatus.WorkerReportsProgress = true;

            if (!workerProbeStatus.IsBusy)
                workerProbeStatus.RunWorkerAsync();
        }

        private void WorkerProbeWork(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                var opcDevicesState = Dispatcher.Invoke((Func<DevicesState>)(() => ((MainWindow)Application.Current.MainWindow).DevicesState));
                var probeResetting = opcDevicesState.ProbeResetting;
                workerProbeStatus.ReportProgress(0, probeResetting);

                if (PageActive && PageDisplayTimeout.AddSeconds(_ProbeResetPageDisplayTimeout) < DateTime.Now)
                {
                    workerProbeStatus.ReportProgress(0, PageCloseTimeout.ClosePage);
                }

                Thread.Sleep(500);
            }
        }

        private void WorkerProbeProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.UserState is PageCloseTimeout && ((PageCloseTimeout)e.UserState) == PageCloseTimeout.ClosePage)
            {
                ((MainWindow)Application.Current.MainWindow).BindPage(DisplayPage.RfidCardPage);
            }
            if (e.UserState is bool)
            {
                if ((bool)e.UserState)
                {
                    gShortcuts.Visibility = Visibility.Hidden;
                    ResetButtonActive = false;
                }
                else
                {
                    gShortcuts.Visibility = Visibility.Visible;
                    ResetButtonActive = true;
                }
            }
        }
        #endregion

        private void ProbeService_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.R && ResetButtonActive)
            {
                PageDisplayTimeout = DateTime.Now;

                PrinterManager printerManager = new PrinterManager();
                var trayId = ((MainWindow)Application.Current.MainWindow).DbRead.ResetSampleTray(TrayInfo);
                ((MainWindow)Application.Current.MainWindow).OpcManager.SendResetSampler();
                var sampleTray = ((MainWindow)Application.Current.MainWindow).DbRead.GetSampleTray(trayId);
                var printedDoc = printerManager.PrintSampleTray(sampleTray);
                ((MainWindow)Application.Current.MainWindow).DbWrite.UpdatePrintedPaperLenght(printedDoc.TotalHeight);
                ((MainWindow)Application.Current.MainWindow).printerPaperStatus.ReciptPrintedLenghtInMilimeters += printedDoc.TotalHeight;
                ((MainWindow)Application.Current.MainWindow).DbRead.MarkTrayReciptAsPrinted(trayId);
                ((MainWindow)Application.Current.MainWindow).DbWrite.UpdatePrintedPaperLenght(printedDoc.TotalHeight);
                BindSampleTrayInfo();
            }
        }
        
        private void BindSampleTrayInfo()
        {
            TrayInfo = ((MainWindow)Application.Current.MainWindow).DbRead.GetStampleTrayInfo();
            tbBottleNo.Text = "Aktualny numer butelki: " + ((MainWindow)Application.Current.MainWindow).DevicesState.Flow.SamplerBottleNo.ToString();
            tbTrayNo.Text = "Aktualny numer tacki: " + TrayInfo.TrayNo.ToString();
        }
        private void PageLoad(object sender, RoutedEventArgs e)
        {
            DataContext = this;
            BindSampleTrayInfo();

            Application.Current.MainWindow.KeyUp -= new KeyEventHandler(ProbeService_KeyUp);
            Application.Current.MainWindow.KeyUp += new KeyEventHandler(ProbeService_KeyUp);

            PageDisplayTimeout = DateTime.Now;
            PageActive = true;
        }
        private void PageUnload(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.KeyUp -= new KeyEventHandler(ProbeService_KeyUp);
            PageActive = false;
        }
    }
}

using DbCommunication.Entities;
using DbCommunication.Enums;
using KioskAppWpf.Classes;
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
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace KioskAppWpf.Pages.Errors
{
    public partial class ErrorPage : Page
    {
        private int _SequenceNoFlowPageDisplayTime
        {
            get
            {
                return Int32.Parse(ConfigurationManager.AppSettings["_SequenceDevErrorPageDisplayTime"]);
            }
        }
        private int _lastBottleNo
        {
            get
            {
                return Int32.Parse(ConfigurationManager.AppSettings["_lastBottleNo"]);
            }
        }

        private static BackgroundWorker workerUpdater = new BackgroundWorker();
        public bool TransactionSaving { get; set; }
        private DateTime ErrorDisplayTimer { get; set; }

        public ErrorPage()
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
                var plcLifeBitError = Dispatcher.Invoke((Func<bool>)(() => ((MainWindow)Application.Current.MainWindow).PlcLifeBitError));
                var sqlError = Dispatcher.Invoke((Func<bool>)(() => ((MainWindow)Application.Current.MainWindow).DbErrorState));

                if (opcDevicesState != null  
                    && opcManager != null && opcManager.Client != null && opcManager.Client.IsConnected)
                {
                    var tbCurrentErrorCode = Dispatcher.Invoke((Func<string>)(() => ((string)tbErrorCode.Text)));
                    var errorCode = GetErrorCode(plcLifeBitError, sqlError, opcDevicesState, kioskConfig);
                    if (tbCurrentErrorCode != errorCode)
                    {
                        workerUpdater.ReportProgress(0, errorCode);
                    }
                    
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
                        && TransactionSaving && ErrorDisplayTimer.AddSeconds(_SequenceNoFlowPageDisplayTime) < DateTime.Now)
                    {
                        TransactionSaving = false;
                        workerUpdater.ReportProgress(0, PrinterVisibility.ReciptInfoHidden);
                    }
                }
                else
                {
                    if (sqlError)
                    {
                        workerUpdater.ReportProgress(0, ErrorCodes._itemSqlErrorLabel);
                    }
                    else
                    {
                        workerUpdater.ReportProgress(0, ErrorCodes._itemOpcErrorLabel);
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
            if (e.UserState is string)
            {
                tbErrorCode.Text = (string)e.UserState;
            }
        }
        #endregion

        public string GetErrorCode(bool plcLifeBitError, bool sqlServerError, DevicesState opcDevicesState, KioskConfiguration kioskConfig)
        {
            if (sqlServerError) return ErrorCodes._itemSqlErrorLabel;
            if (plcLifeBitError) return ErrorCodes._itemLifeBitLabel;
            if (!opcDevicesState.Power230Ok) return ErrorCodes._item230PowerLabel;
            if (!opcDevicesState.Power24Ok) return ErrorCodes._item24PowerLabel;
            if (!opcDevicesState.FlowOk) return ErrorCodes._itemFlowMeterLabel;
            if (!opcDevicesState.DumpPathOk) return ErrorCodes._itemDumpPathLabel;
            if (!opcDevicesState.ScreenOk) return ErrorCodes._itemScreenLabel;
            if (opcDevicesState.ScreenFull) return ErrorCodes._itemScreenFullLabel;

            if (!opcDevicesState.ProbeOk && (kioskConfig == null || !kioskConfig.KioskBlockages[KioskBlockageType.TakingSamples])) return ErrorCodes._itemProbeLabel;
            if (!opcDevicesState.ProbeReadyOk && (kioskConfig == null || !kioskConfig.KioskBlockages[KioskBlockageType.TakingSamples])) return ErrorCodes._itemProbeReadyLabel;
            if (!opcDevicesState.PhMeterOk && (kioskConfig == null || !kioskConfig.KioskBlockages[KioskBlockageType.MeasuringPh])) return ErrorCodes._itemPhMeterLabel;
            if (!opcDevicesState.TempMeterOk && (kioskConfig == null || !kioskConfig.KioskBlockages[KioskBlockageType.MeasugingTemperature])) return ErrorCodes._itemTempMeterLabel;
            if (!opcDevicesState.PressureMeterOk && (kioskConfig == null || !kioskConfig.KioskBlockages[KioskBlockageType.MeasuringPressure])) return ErrorCodes._itemPressureMeterLabel;
            if (opcDevicesState.KioskDoorOpen) return ErrorCodes._itemKioskDoorOpenLabel;
            if (opcDevicesState.Flow.SamplerBottleNo >= _lastBottleNo && !kioskConfig.KioskBlockages[KioskBlockageType.TakingSamples]) return ErrorCodes._itemProbeOutOfBottlesLabel;
            if (!opcDevicesState.ValveMeterOk && (kioskConfig == null || !kioskConfig.KioskBlockages[KioskBlockageType.WaterForMeasuringDevices])) return ErrorCodes._itemValveMeterOkLabel;

            if (opcDevicesState.ProbeDoorOpen) return ErrorCodes._itemProbeDoorOpenLabel;
            if (!opcDevicesState.AirOk) return ErrorCodes._itemAirOkLabel;
            if (!opcDevicesState.Distribution230VACOk) return ErrorCodes._itemDistribution230VACOkLabel;
            if (!opcDevicesState.Rack230VACOk) return ErrorCodes._itemRack230VACOkLabel;
            if (!opcDevicesState.Rack24VDCOk) return ErrorCodes._itemRack24VDCOkLabel;
            if (!opcDevicesState.Rack24VDCUPSOk) return ErrorCodes._itemRack24VDCUPSOkLabel;
            if (!opcDevicesState.Rack12VDCOk) return ErrorCodes._itemRack12VDCOkLabel;

            if (!opcDevicesState.PumpRoomOk) return ErrorCodes._itemPumpRoomOkLabel;
            if (!opcDevicesState.PumpRoomWorking) return ErrorCodes._itemPumpRoomWorkOkLabel;
            if (opcDevicesState.ScreenRoomLeak) return ErrorCodes._itemScreenRoomLeakLabel;
            if (!opcDevicesState.AugerOk) return ErrorCodes._itemAugerOkLabel;
            if (!opcDevicesState.PreScreenOk) return ErrorCodes._itemPreScreenOkLabel;

            var printerError = Dispatcher.Invoke((Func<bool>)(() => ((MainWindow)Application.Current.MainWindow).PrinterErrorState));
            if (printerError) return ErrorCodes._itemPrinterErrorLabel;
            var printeroutOfPaperError = Dispatcher.Invoke((Func<bool>)(() => ((MainWindow)Application.Current.MainWindow).PrinterOutOfPaperErrorState));
            if (printeroutOfPaperError) return ErrorCodes._itemPrinterOutOfPaperErrorLabel;
            var keyboardError = Dispatcher.Invoke((Func<bool>)(() => ((MainWindow)Application.Current.MainWindow).KeyboardErrorState));
            if (keyboardError) return ErrorCodes._itemKayboardErrorLabel;
            var rfidError = Dispatcher.Invoke((Func<bool>)(() => ((MainWindow)Application.Current.MainWindow).RfidErrorState));
            if (rfidError) return ErrorCodes._itemRfidErrorLabel;

            return "";
        }
    }
}

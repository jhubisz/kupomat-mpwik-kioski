using DbCommunication;
using DbCommunication.Entities;
using KioskAppWpf.Classes;
using KioskAppWpf.Controls;
using KioskAppWpf.Enums;
using OpcCommunication.Devices;
using RfidCommunication.Enums;
using System;
using System.ComponentModel;
using System.Configuration;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Utilities.Classes;

namespace KioskAppWpf.Pages
{
    public partial class ServicePage : Page
    {
        private bool passwordCorrect;
        private string _servicePass
        {
            get
            {
                return ConfigurationManager.AppSettings["_servicePass"];
            }
        }

        public Transaction Transaction
        {
            get
            {
                return ((MainWindow)Application.Current.MainWindow).Transaction;
            }
        }

        public bool WaterRunning { get; set; }
        public DateTime WaterRunningStart { get; set; }

        #region left panel indexes
        private static int _itemPlcIndex = 0;
        private static int _item230PowerIndex = 1;
        private static int _item24PowerIndex = 2;
        private static int _itemKioskDoorIndex = 3;
        private static int _itemAirIndex = 4;
        private static int _itemFlowMeterIndex = 5;
        private static int _itemDumpPathIndex = 6;
        private static int _itemPhMeterIndex = 7;
        private static int _itemTempMeterIndex = 8;
        private static int _itemPressureMeterIndex = 9;
        private static int _itemProbeIndex = 10;
        private static int _itemProbeReadyIndex = 11;
        private static int _itemDistribution230VACIndex = 12;
        private static int _itemRack230VACIndex = 13;
        private static int _itemRack24VDCIndex = 14;
        private static int _itemRack24VDCUPSIndex = 15;
        private static int _itemRack12VDCIndex = 16;
        private static int _itemValveIndex = 17;
        private static int _itemScreenIndex = 18;
        private static int _itemScreenFullIndex = 19;
        private static int _itemGateIndex = 20;
        #endregion
        #region left panel labels
        private static string _itemPlcLabel = "Komunikacja ze sterownikiem PLC";
        private static string _item230PowerLabel = "Obecność zasilania 230VAC";
        private static string _item24PowerLabel = "Obecność zasilania 24VDC";
        private static string _itemKioskDoorLabel = "Drzwiczki kiosku";
        private static string _itemAirLabel = "Powietrze";
        private static string _itemFlowMeterLabel = "Przepływomierz ścieków";
        private static string _itemDumpPathLabel = "Droga zrzutu";
        private static string _itemPhMeterLabel = "Pomiar pH";
        private static string _itemTempMeterLabel = "Pomiar temperatury";
        private static string _itemPressureMeterLabel = "Pomiar ciśnienia";

        private static string _itemProbeLabel = "Pobierak";
        private static string _itemProbeReadyLabel = "Pobierak - gotowość";

        private static string _itemDistribution230VACLabel = "Rozdzielnia 230VAC";
        private static string _itemRack230VACLabel = "Szafa SA 230VAC";
        private static string _itemRack24VDCLabel = "Szafa SA 24VDC";
        private static string _itemRack24VDCUPSLabel = "Szafa SA 24VDC UPS";
        private static string _itemRack12VDCLabel = "Szafa SA 12VDC";

        private static string _itemValveLabel = "Zasuwa ścieków";
        private static string _itemScreenLabel = "Sitopiaskownik";
        private static string _itemScreenFullLabel = "Sitopiaskownik pełen";
        #endregion

        #region right panel indexes
        private static int _itemPumpRoomIndex = 0;
        private static int _itemPumpRoomWorkIndex = 1;
        private static int _itemScreenRoomLeakIndex = 2;
        private static int _itemAugerIndex = 3;
        private static int _itemPreScreenIndex = 4;

        private static int _itemRfidIndex = 5;
        private static int _itemOpcIndex = 6;
        private static int _itemSqlIndex = 7;
        private static int _itemPrinterIndex = 8;
        private static int _itemKeyboardIndex = 9;
        private static int _itemRemoteSqlIndex = 10;
        #endregion
        #region right panel labels 
        private static string _itemPumpRoomLabel = "Hydrofornia";
        private static string _itemPumpRoomWorkLabel = "Hydrofornia - praca";
        private static string _itemScreenRoomLeakLabel = "Wyciek w pomieszczeniu sita";
        private static string _itemAugerLabel = "Przenośnik ślimakowy";
        private static string _itemPreScreenLabel = "Urządzenie grzewcze";

        private static string _itemRfidLabel = "Komunikacja z czytnikiem RFID";
        private static string _itemOpcLabel = "Uruchomiony serwer OPC";
        private static string _itemSqlLabel = "Uruchomiony serwer SQL";
        private static string _itemPrinterLabel = "Obecność drukarki w systemie";
        private static string _itemKeyboardLabel = "Obecność klawiatury w systemie";
        private static string _itemRemoteSqlLabel = "Komunikacja z serwerem zdalnym";
        #endregion

        private static BackgroundWorker workerWeightStatus = new BackgroundWorker();
        private static BackgroundWorker workerUpdater = new BackgroundWorker();
        private decimal Weight { get; set; }

        private PrinterPaperStatus printerPaperStatus;
        private bool printerResetActive;
        private bool printerResetConfirmation;

        public ServicePage()
        {
            InitializeComponent();
            Application.Current.MainWindow.KeyDown -= new KeyEventHandler(ServicePage_KeyDown);
            Application.Current.MainWindow.KeyDown += new KeyEventHandler(ServicePage_KeyDown);

            BindServiceItems();
            RunWorkerUpdater();
        }

        private void BindServiceItems()
        {
            spServiceItems.Children.Clear();

            var opcDevicesState = ((MainWindow)Application.Current.MainWindow).DevicesState;
            var remoteConnection = ((MainWindow)Application.Current.MainWindow).ServerConnection;

            #region left panel
            if (opcDevicesState != null)
            {
                ServicePageItem itemPlc = new ServicePageItem(new ServicePageItemState() { Label = _itemPlcLabel, Error = ((MainWindow)Application.Current.MainWindow).PlcLifeBitError, Index = _itemPlcIndex });
                spServiceItems.Children.Add(itemPlc);

                ServicePageItem item230Power = new ServicePageItem(new ServicePageItemState() { Label = _item230PowerLabel, Error = !opcDevicesState.Power230Ok, Index = _item230PowerIndex });
                spServiceItems.Children.Add(item230Power);
                ServicePageItem item24Power = new ServicePageItem(new ServicePageItemState() { Label = _item24PowerLabel, Error = !opcDevicesState.Power24Ok, Index = _item24PowerIndex });
                spServiceItems.Children.Add(item24Power);
                ServicePageItem itemKioskDoor = new ServicePageItem(new ServicePageItemState() { Label = _itemKioskDoorLabel, Error = opcDevicesState.KioskDoorOpen, Index = _itemKioskDoorIndex });
                spServiceItems.Children.Add(itemKioskDoor);
                ServicePageItem itemAir = new ServicePageItem(new ServicePageItemState() { Label = _itemAirLabel, Error = !opcDevicesState.AirOk, Index = _itemAirIndex });
                spServiceItems.Children.Add(itemAir);
                ServicePageItem itemFlowMeter = new ServicePageItem(new ServicePageItemState() { Label = _itemFlowMeterLabel, Error = !opcDevicesState.FlowOk, Index = _itemFlowMeterIndex });
                spServiceItems.Children.Add(itemFlowMeter);
                ServicePageItem itemDumpPath = new ServicePageItem(new ServicePageItemState() { Label = _itemDumpPathLabel, Error = !opcDevicesState.DumpPathOk, Index = _itemDumpPathIndex });
                spServiceItems.Children.Add(itemDumpPath);
                ServicePageItem itemPhMeter = new ServicePageItem(new ServicePageItemState() { Label = _itemPhMeterLabel, Error = !opcDevicesState.PhMeterOk, Index = _itemPhMeterIndex });
                spServiceItems.Children.Add(itemPhMeter);
                ServicePageItem itemTempMeter = new ServicePageItem(new ServicePageItemState() { Label = _itemTempMeterLabel, Error = !opcDevicesState.TempMeterOk, Index = _itemTempMeterIndex });
                spServiceItems.Children.Add(itemTempMeter);
                ServicePageItem itemPressureMeter = new ServicePageItem(new ServicePageItemState() { Label = _itemPressureMeterLabel, Error = !opcDevicesState.PressureMeterOk, Index = _itemPressureMeterIndex });
                spServiceItems.Children.Add(itemPressureMeter);

                ServicePageItem probeItem = new ServicePageItem(new ServicePageItemState() { Label = _itemProbeLabel, Error = !opcDevicesState.ProbeOk, Index = _itemProbeIndex });
                spServiceItems.Children.Add(probeItem);
                ServicePageItem probeReadyItem = new ServicePageItem(new ServicePageItemState() { Label = _itemProbeReadyLabel, Error = !opcDevicesState.ProbeReadyOk, Index = _itemProbeReadyIndex });
                spServiceItems.Children.Add(probeReadyItem);

                ServicePageItem distribution230VACItem = new ServicePageItem(new ServicePageItemState() { Label = _itemDistribution230VACLabel, Error = !opcDevicesState.Distribution230VACOk, Index = _itemDistribution230VACIndex });
                spServiceItems.Children.Add(distribution230VACItem);
                ServicePageItem rack230VACItem = new ServicePageItem(new ServicePageItemState() { Label = _itemRack230VACLabel, Error = !opcDevicesState.Rack230VACOk, Index = _itemRack230VACIndex });
                spServiceItems.Children.Add(rack230VACItem);
                ServicePageItem rack24VDCItem = new ServicePageItem(new ServicePageItemState() { Label = _itemRack24VDCLabel, Error = !opcDevicesState.Rack24VDCOk, Index = _itemRack24VDCIndex });
                spServiceItems.Children.Add(rack24VDCItem);
                ServicePageItem rack24VDCUPSItem = new ServicePageItem(new ServicePageItemState() { Label = _itemRack24VDCUPSLabel, Error = !opcDevicesState.Rack24VDCUPSOk, Index = _itemRack24VDCUPSIndex });
                spServiceItems.Children.Add(rack24VDCUPSItem);
                ServicePageItem rack12VDCItem = new ServicePageItem(new ServicePageItemState() { Label = _itemRack12VDCLabel, Error = !opcDevicesState.Rack12VDCOk, Index = _itemRack12VDCIndex });
                spServiceItems.Children.Add(rack12VDCItem);

                ServicePageItem itemValve = new ServicePageItem(new ServicePageItemState() { Label = _itemValveLabel, Error = false, Index = _itemValveIndex });
                spServiceItems.Children.Add(itemValve);
                ServicePageItem itemScreen = new ServicePageItem(new ServicePageItemState() { Label = _itemScreenLabel, Error = !opcDevicesState.ScreenOk, Index = _itemScreenIndex });
                spServiceItems.Children.Add(itemScreen);
                ServicePageItem itemScreenFull = new ServicePageItem(new ServicePageItemState() { Label = _itemScreenFullLabel, Error = opcDevicesState.ScreenFull, Index = _itemScreenFullIndex });
                spServiceItems.Children.Add(itemScreenFull);
            }
            else
            {
                ServicePageItem itemPlc = new ServicePageItem(new ServicePageItemState() { Label = _itemPlcLabel, Error = true, Index = _itemPlcIndex });
                spServiceItems.Children.Add(itemPlc);

                ServicePageItem item230Power = new ServicePageItem(new ServicePageItemState() { Label = _item230PowerLabel, Error = true, Index = _item230PowerIndex });
                spServiceItems.Children.Add(item230Power);
                ServicePageItem item24Power = new ServicePageItem(new ServicePageItemState() { Label = _item24PowerLabel, Error = true, Index = _item24PowerIndex });
                spServiceItems.Children.Add(item24Power);
                ServicePageItem itemKioskDoor = new ServicePageItem(new ServicePageItemState() { Label = _itemKioskDoorLabel, Error = true, Index = _itemKioskDoorIndex });
                spServiceItems.Children.Add(itemKioskDoor);
                ServicePageItem itemAir = new ServicePageItem(new ServicePageItemState() { Label = _itemAirLabel, Error = true, Index = _itemAirIndex });
                spServiceItems.Children.Add(itemAir);
                ServicePageItem itemFlowMeter = new ServicePageItem(new ServicePageItemState() { Label = _itemFlowMeterLabel, Error = true, Index = _itemFlowMeterIndex });
                spServiceItems.Children.Add(itemFlowMeter);
                ServicePageItem itemDumpPath = new ServicePageItem(new ServicePageItemState() { Label = _itemDumpPathLabel, Error = true, Index = _itemDumpPathIndex });
                spServiceItems.Children.Add(itemDumpPath);
                ServicePageItem itemPhMeter = new ServicePageItem(new ServicePageItemState() { Label = _itemPhMeterLabel, Error = true, Index = _itemPhMeterIndex });
                spServiceItems.Children.Add(itemPhMeter);
                ServicePageItem itemTempMeter = new ServicePageItem(new ServicePageItemState() { Label = _itemTempMeterLabel, Error = true, Index = _itemTempMeterIndex });
                spServiceItems.Children.Add(itemTempMeter);
                ServicePageItem itemPressureMeter = new ServicePageItem(new ServicePageItemState() { Label = _itemPressureMeterLabel, Error = true, Index = _itemPressureMeterIndex });
                spServiceItems.Children.Add(itemPressureMeter);

                ServicePageItem probeItem = new ServicePageItem(new ServicePageItemState() { Label = _itemProbeLabel, Error = true, Index = _itemProbeIndex });
                spServiceItems.Children.Add(probeItem);
                ServicePageItem probeReadyItem = new ServicePageItem(new ServicePageItemState() { Label = _itemProbeReadyLabel, Error = true, Index = _itemProbeReadyIndex });
                spServiceItems.Children.Add(probeReadyItem);

                ServicePageItem distribution230VACItem = new ServicePageItem(new ServicePageItemState() { Label = _itemDistribution230VACLabel, Error = true, Index = _itemDistribution230VACIndex });
                spServiceItems.Children.Add(distribution230VACItem);
                ServicePageItem rack230VACItem = new ServicePageItem(new ServicePageItemState() { Label = _itemRack230VACLabel, Error = true, Index = _itemRack230VACIndex });
                spServiceItems.Children.Add(rack230VACItem);
                ServicePageItem rack24VDCItem = new ServicePageItem(new ServicePageItemState() { Label = _itemRack24VDCLabel, Error = true, Index = _itemRack24VDCIndex });
                spServiceItems.Children.Add(rack24VDCItem);
                ServicePageItem rack24VDCUPSItem = new ServicePageItem(new ServicePageItemState() { Label = _itemRack24VDCUPSLabel, Error = true, Index = _itemRack24VDCUPSIndex });
                spServiceItems.Children.Add(rack24VDCUPSItem);
                ServicePageItem rack12VDCItem = new ServicePageItem(new ServicePageItemState() { Label = _itemRack12VDCLabel, Error = true, Index = _itemRack12VDCIndex });
                spServiceItems.Children.Add(rack12VDCItem);

                ServicePageItem itemValve = new ServicePageItem(new ServicePageItemState() { Label = _itemValveLabel, Error = true, Index = _itemValveIndex });
                spServiceItems.Children.Add(itemValve);
                ServicePageItem itemScreen = new ServicePageItem(new ServicePageItemState() { Label = _itemScreenLabel, Error = true, Index = _itemScreenIndex });
                spServiceItems.Children.Add(itemScreen);
                ServicePageItem itemScreenFull = new ServicePageItem(new ServicePageItemState() { Label = _itemScreenFullLabel, Error = true, Index = _itemScreenFullIndex });
                spServiceItems.Children.Add(itemScreenFull);
            }
            #endregion

            #region right panel
            if (opcDevicesState != null)
            {
                ServicePageItem itemPumpRoom = new ServicePageItem(new ServicePageItemState() { Label = _itemPumpRoomLabel, Error = !opcDevicesState.PumpRoomOk, Index = _itemPumpRoomIndex, Postion = ServiceScreenPostion.RightBox });
                spServiceItemsRight.Children.Add(itemPumpRoom);
                ServicePageItem itemPumpRoomWork = new ServicePageItem(new ServicePageItemState() { Label = _itemPumpRoomWorkLabel, Error = !opcDevicesState.PumpRoomWorking, Index = _itemPumpRoomWorkIndex, Postion = ServiceScreenPostion.RightBox });
                spServiceItemsRight.Children.Add(itemPumpRoomWork);
                ServicePageItem itemScreenRoomLeak = new ServicePageItem(new ServicePageItemState() { Label = _itemScreenRoomLeakLabel, Error = opcDevicesState.ScreenRoomLeak, Index = _itemScreenRoomLeakIndex, Postion = ServiceScreenPostion.RightBox });
                spServiceItemsRight.Children.Add(itemScreenRoomLeak);
                ServicePageItem itemAuger = new ServicePageItem(new ServicePageItemState() { Label = _itemAugerLabel, Error = !opcDevicesState.AugerOk, Index = _itemAugerIndex, Postion = ServiceScreenPostion.RightBox });
                spServiceItemsRight.Children.Add(itemAuger);
                ServicePageItem itemPreScreen = new ServicePageItem(new ServicePageItemState() { Label = _itemPreScreenLabel, Error = !opcDevicesState.PreScreenOk, Index = _itemPreScreenIndex, Postion = ServiceScreenPostion.RightBox });
                spServiceItemsRight.Children.Add(itemPreScreen);
            }
            else
            {
                ServicePageItem itemPumpRoom = new ServicePageItem(new ServicePageItemState() { Label = _itemPumpRoomLabel, Error = true, Index = _itemPumpRoomIndex, Postion = ServiceScreenPostion.RightBox });
                spServiceItemsRight.Children.Add(itemPumpRoom);
                ServicePageItem itemPumpRoomWork = new ServicePageItem(new ServicePageItemState() { Label = _itemPumpRoomWorkLabel, Error = true, Index = _itemPumpRoomWorkIndex, Postion = ServiceScreenPostion.RightBox });
                spServiceItemsRight.Children.Add(itemPumpRoomWork);
                ServicePageItem itemScreenRoomLeak = new ServicePageItem(new ServicePageItemState() { Label = _itemScreenRoomLeakLabel, Error = true, Index = _itemScreenRoomLeakIndex, Postion = ServiceScreenPostion.RightBox });
                spServiceItemsRight.Children.Add(itemScreenRoomLeak);
                ServicePageItem itemAuger = new ServicePageItem(new ServicePageItemState() { Label = _itemAugerLabel, Error = true, Index = _itemAugerIndex, Postion = ServiceScreenPostion.RightBox });
                spServiceItemsRight.Children.Add(itemAuger);
                ServicePageItem itemPreScreen = new ServicePageItem(new ServicePageItemState() { Label = _itemPreScreenLabel, Error = true, Index = _itemPreScreenIndex, Postion = ServiceScreenPostion.RightBox });
                spServiceItemsRight.Children.Add(itemPreScreen);
            }

            ServicePageItem itemRfid = new ServicePageItem(new ServicePageItemState() { Label = _itemRfidLabel, Error = ((MainWindow)Application.Current.MainWindow).RfidErrorState, Index = _itemRfidIndex, Postion = ServiceScreenPostion.RightBox });
            spServiceItemsRight.Children.Add(itemRfid);

            ServicePageItem itemOpc = new ServicePageItem(new ServicePageItemState() { Label = _itemOpcLabel, Error = ((MainWindow)Application.Current.MainWindow).OpcErrorState, Index = _itemOpcIndex, Postion = ServiceScreenPostion.RightBox });
            spServiceItemsRight.Children.Add(itemOpc);

            ServicePageItem itemSql = new ServicePageItem(new ServicePageItemState() { Label = _itemSqlLabel, Error = ((MainWindow)Application.Current.MainWindow).DbErrorState, Index = _itemSqlIndex, Postion = ServiceScreenPostion.RightBox });
            spServiceItemsRight.Children.Add(itemSql);

            ServicePageItem itemPrinter = new ServicePageItem(new ServicePageItemState() { Label = _itemPrinterLabel, Error = ((MainWindow)Application.Current.MainWindow).PrinterErrorState, Index = _itemPrinterIndex, Postion = ServiceScreenPostion.RightBox });
            spServiceItemsRight.Children.Add(itemPrinter);

            ServicePageItem itemKeyboard = new ServicePageItem(new ServicePageItemState() { Label = _itemKeyboardLabel, Error = ((MainWindow)Application.Current.MainWindow).KeyboardErrorState, Index = _itemKeyboardIndex, Postion = ServiceScreenPostion.RightBox });
            spServiceItemsRight.Children.Add(itemKeyboard);

            var remoteServerError = remoteConnection == null;
            ServicePageItem itemRemoteSql = new ServicePageItem(new ServicePageItemState() { Label = _itemRemoteSqlLabel, Error = remoteServerError, Index = _itemRemoteSqlIndex });
            spServiceItemsRight.Children.Add(itemRemoteSql);
            #endregion

            if (((MainWindow)Application.Current.MainWindow).printerPaperStatus != null)
            {
                BindPrinterPaperStatus(((MainWindow)Application.Current.MainWindow).printerPaperStatus);
            }
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
                var opcDevicesState = Dispatcher.Invoke((Func<DevicesState>)(() => ((MainWindow)Application.Current.MainWindow).DevicesState));

                #region left panel
                if (opcDevicesState != null)
                {
                    var plcLifeBitError = Dispatcher.Invoke((Func<bool>)(() => ((MainWindow)Application.Current.MainWindow).PlcLifeBitError));
                    var plcItem = Dispatcher.Invoke((Func<ServicePageItem>)(() => ((ServicePageItem)spServiceItems.Children[_itemPlcIndex])));
                    if (plcItem.State.Error != plcLifeBitError)
                    {
                        workerUpdater.ReportProgress(0, new ServicePageItemState() { Label = _itemPlcLabel, Error = plcLifeBitError, Index = _itemPlcIndex });
                    }

                    var power230Item = Dispatcher.Invoke((Func<ServicePageItem>)(() => ((ServicePageItem)spServiceItems.Children[_item230PowerIndex])));
                    if (power230Item.State.Error != !opcDevicesState.Power230Ok)
                    {
                        workerUpdater.ReportProgress(0, new ServicePageItemState() { Label = _item230PowerLabel, Error = !opcDevicesState.Power230Ok, Index = _item230PowerIndex });
                    }
                    var power24Item = Dispatcher.Invoke((Func<ServicePageItem>)(() => ((ServicePageItem)spServiceItems.Children[_item24PowerIndex])));
                    if (power24Item.State.Error != !opcDevicesState.Power24Ok)
                    {
                        workerUpdater.ReportProgress(0, new ServicePageItemState() { Label = _item24PowerLabel, Error = !opcDevicesState.Power24Ok, Index = _item24PowerIndex });
                    }
                    var kioskDoorItem = Dispatcher.Invoke((Func<ServicePageItem>)(() => ((ServicePageItem)spServiceItems.Children[_itemKioskDoorIndex])));
                    if (kioskDoorItem.State.Error != opcDevicesState.KioskDoorOpen)
                    {
                        workerUpdater.ReportProgress(0, new ServicePageItemState() { Label = _itemKioskDoorLabel, Error = opcDevicesState.KioskDoorOpen, Index = _itemKioskDoorIndex });
                    }
                    var airItem = Dispatcher.Invoke((Func<ServicePageItem>)(() => ((ServicePageItem)spServiceItems.Children[_itemAirIndex])));
                    if (airItem.State.Error != !opcDevicesState.AirOk)
                    {
                        workerUpdater.ReportProgress(0, new ServicePageItemState() { Label = _itemAirLabel, Error = !opcDevicesState.AirOk, Index = _itemAirIndex });
                    }
                    var flowMeterItem = Dispatcher.Invoke((Func<ServicePageItem>)(() => ((ServicePageItem)spServiceItems.Children[_itemFlowMeterIndex])));
                    if (flowMeterItem.State.Error != false)
                    {
                        workerUpdater.ReportProgress(0, new ServicePageItemState() { Label = _itemFlowMeterLabel, Error = false, Index = _itemFlowMeterIndex });
                    }
                    var dumpPathItem = Dispatcher.Invoke((Func<ServicePageItem>)(() => ((ServicePageItem)spServiceItems.Children[_itemDumpPathIndex])));
                    if (dumpPathItem.State.Error != !opcDevicesState.DumpPathOk)
                    {
                        workerUpdater.ReportProgress(0, new ServicePageItemState() { Label = _itemDumpPathLabel, Error = !opcDevicesState.DumpPathOk, Index = _itemDumpPathIndex });
                    }
                    var phMeterItem = Dispatcher.Invoke((Func<ServicePageItem>)(() => ((ServicePageItem)spServiceItems.Children[_itemPhMeterIndex])));
                    if (phMeterItem.State.Error != !opcDevicesState.PhMeterOk)
                    {
                        workerUpdater.ReportProgress(0, new ServicePageItemState() { Label = _itemPhMeterLabel, Error = !opcDevicesState.PhMeterOk, Index = _itemPhMeterIndex });
                    }
                    var tempMeterItem = Dispatcher.Invoke((Func<ServicePageItem>)(() => ((ServicePageItem)spServiceItems.Children[_itemTempMeterIndex])));
                    if (tempMeterItem.State.Error != !opcDevicesState.TempMeterOk)
                    {
                        workerUpdater.ReportProgress(0, new ServicePageItemState() { Label = _itemTempMeterLabel, Error = !opcDevicesState.TempMeterOk, Index = _itemTempMeterIndex });
                    }
                    var pressureMeterItem = Dispatcher.Invoke((Func<ServicePageItem>)(() => ((ServicePageItem)spServiceItems.Children[_itemPressureMeterIndex])));
                    if (pressureMeterItem.State.Error != !opcDevicesState.PressureMeterOk)
                    {
                        workerUpdater.ReportProgress(0, new ServicePageItemState() { Label = _itemPressureMeterLabel, Error = !opcDevicesState.PressureMeterOk, Index = _itemPressureMeterIndex });
                    }

                    var probeItem = Dispatcher.Invoke((Func<ServicePageItem>)(() => ((ServicePageItem)spServiceItems.Children[_itemProbeIndex])));
                    if (probeItem.State.Error != !opcDevicesState.ProbeOk)
                    {
                        workerUpdater.ReportProgress(0, new ServicePageItemState() { Label = _itemProbeLabel, Error = !opcDevicesState.ProbeOk, Index = _itemProbeIndex });
                    }
                    var probeReadyItem = Dispatcher.Invoke((Func<ServicePageItem>)(() => ((ServicePageItem)spServiceItems.Children[_itemProbeReadyIndex])));
                    if (probeReadyItem.State.Error != !opcDevicesState.ProbeReadyOk)
                    {
                        workerUpdater.ReportProgress(0, new ServicePageItemState() { Label = _itemProbeReadyLabel, Error = !opcDevicesState.ProbeReadyOk, Index = _itemProbeReadyIndex });
                    }

                    var distribution230VACItem = Dispatcher.Invoke((Func<ServicePageItem>)(() => ((ServicePageItem)spServiceItems.Children[_itemDistribution230VACIndex])));
                    if (distribution230VACItem.State.Error != !opcDevicesState.Distribution230VACOk)
                    {
                        workerUpdater.ReportProgress(0, new ServicePageItemState() { Label = _itemDistribution230VACLabel, Error = !opcDevicesState.Distribution230VACOk, Index = _itemDistribution230VACIndex });
                    }
                    var rack230VACItem = Dispatcher.Invoke((Func<ServicePageItem>)(() => ((ServicePageItem)spServiceItems.Children[_itemRack230VACIndex])));
                    if (rack230VACItem.State.Error != !opcDevicesState.Rack230VACOk)
                    {
                        workerUpdater.ReportProgress(0, new ServicePageItemState() { Label = _itemRack230VACLabel, Error = !opcDevicesState.Rack230VACOk, Index = _itemRack230VACIndex });
                    }
                    var rack24VDCItem = Dispatcher.Invoke((Func<ServicePageItem>)(() => ((ServicePageItem)spServiceItems.Children[_itemRack24VDCIndex])));
                    if (rack24VDCItem.State.Error != !opcDevicesState.Rack24VDCOk)
                    {
                        workerUpdater.ReportProgress(0, new ServicePageItemState() { Label = _itemRack24VDCLabel, Error = !opcDevicesState.Rack24VDCOk, Index = _itemRack24VDCIndex });
                    }
                    var rack24VDCUPSItem = Dispatcher.Invoke((Func<ServicePageItem>)(() => ((ServicePageItem)spServiceItems.Children[_itemRack24VDCUPSIndex])));
                    if (rack24VDCUPSItem.State.Error != !opcDevicesState.Rack24VDCUPSOk)
                    {
                        workerUpdater.ReportProgress(0, new ServicePageItemState() { Label = _itemRack24VDCUPSLabel, Error = !opcDevicesState.Rack24VDCUPSOk, Index = _itemRack24VDCUPSIndex });
                    }
                    var rack12VDCItem = Dispatcher.Invoke((Func<ServicePageItem>)(() => ((ServicePageItem)spServiceItems.Children[_itemRack12VDCIndex])));
                    if (rack12VDCItem.State.Error != !opcDevicesState.Rack12VDCOk)
                    {
                        workerUpdater.ReportProgress(0, new ServicePageItemState() { Label = _itemRack12VDCLabel, Error = !opcDevicesState.Rack12VDCOk, Index = _itemRack12VDCIndex });
                    }

                    var valveItem = Dispatcher.Invoke((Func<ServicePageItem>)(() => ((ServicePageItem)spServiceItems.Children[_itemValveIndex])));
                    if (valveItem.State.Error != false)
                    {
                        workerUpdater.ReportProgress(0, new ServicePageItemState() { Label = _itemValveLabel, Error = false, Index = _itemValveIndex });
                    }
                    var screenItem = Dispatcher.Invoke((Func<ServicePageItem>)(() => ((ServicePageItem)spServiceItems.Children[_itemScreenIndex])));
                    if (screenItem.State.Error != !opcDevicesState.ScreenOk)
                    {
                        workerUpdater.ReportProgress(0, new ServicePageItemState() { Label = _itemScreenLabel, Error = !opcDevicesState.ScreenOk, Index = _itemScreenIndex });
                    }
                    var screenFullItem = Dispatcher.Invoke((Func<ServicePageItem>)(() => ((ServicePageItem)spServiceItems.Children[_itemScreenFullIndex])));
                    if (screenFullItem.State.Error != opcDevicesState.ScreenFull)
                    {
                        workerUpdater.ReportProgress(0, new ServicePageItemState() { Label = _itemScreenFullLabel, Error = opcDevicesState.ScreenFull, Index = _itemScreenFullIndex });
                    }
                }
                #endregion

                #region right panel
                if (opcDevicesState != null)
                {
                    var pumpRoomItem = Dispatcher.Invoke((Func<ServicePageItem>)(() => ((ServicePageItem)spServiceItemsRight.Children[_itemPumpRoomIndex])));
                    if (pumpRoomItem.State.Error != !opcDevicesState.PumpRoomOk)
                    {
                        workerUpdater.ReportProgress(0, new ServicePageItemState() { Label = _itemPumpRoomLabel, Error = !opcDevicesState.PumpRoomOk, Index = _itemPumpRoomIndex, Postion = ServiceScreenPostion.RightBox });
                    }
                    var pumpRoomWorkItem = Dispatcher.Invoke((Func<ServicePageItem>)(() => ((ServicePageItem)spServiceItemsRight.Children[_itemPumpRoomWorkIndex])));
                    if (pumpRoomWorkItem.State.Error != !opcDevicesState.PumpRoomWorking)
                    {
                        workerUpdater.ReportProgress(0, new ServicePageItemState() { Label = _itemPumpRoomWorkLabel, Error = !opcDevicesState.PumpRoomWorking, Index = _itemPumpRoomWorkIndex, Postion = ServiceScreenPostion.RightBox });
                    }
                    var screenRoomLeakItem = Dispatcher.Invoke((Func<ServicePageItem>)(() => ((ServicePageItem)spServiceItemsRight.Children[_itemScreenRoomLeakIndex])));
                    if (screenRoomLeakItem.State.Error != opcDevicesState.ScreenRoomLeak)
                    {
                        workerUpdater.ReportProgress(0, new ServicePageItemState() { Label = _itemScreenRoomLeakLabel, Error = opcDevicesState.ScreenRoomLeak, Index = _itemScreenRoomLeakIndex, Postion = ServiceScreenPostion.RightBox });
                    }
                    var augerItem = Dispatcher.Invoke((Func<ServicePageItem>)(() => ((ServicePageItem)spServiceItemsRight.Children[_itemAugerIndex])));
                    if (augerItem.State.Error != !opcDevicesState.AugerOk)
                    {
                        workerUpdater.ReportProgress(0, new ServicePageItemState() { Label = _itemAugerLabel, Error = !opcDevicesState.AugerOk, Index = _itemAugerIndex, Postion = ServiceScreenPostion.RightBox });
                    }
                    var preScreenItem = Dispatcher.Invoke((Func<ServicePageItem>)(() => ((ServicePageItem)spServiceItemsRight.Children[_itemPreScreenIndex])));
                    if (preScreenItem.State.Error != !opcDevicesState.PreScreenOk)
                    {
                        workerUpdater.ReportProgress(0, new ServicePageItemState() { Label = _itemPreScreenLabel, Error = !opcDevicesState.PreScreenOk, Index = _itemPreScreenIndex, Postion = ServiceScreenPostion.RightBox });
                    }
                }
                var rfidError = Dispatcher.Invoke((Func<bool>)(() => ((MainWindow)Application.Current.MainWindow).RfidErrorState));
                var rfidItem = Dispatcher.Invoke((Func<ServicePageItem>)(() => ((ServicePageItem)spServiceItemsRight.Children[_itemRfidIndex])));
                if (rfidItem.State.Error != rfidError)
                {
                    workerUpdater.ReportProgress(0, new ServicePageItemState() { Label = _itemRfidLabel, Error = rfidError, Index = _itemRfidIndex, Postion = ServiceScreenPostion.RightBox });
                }

                var opcError = Dispatcher.Invoke((Func<bool>)(() => ((MainWindow)Application.Current.MainWindow).OpcErrorState));
                var opcItem = Dispatcher.Invoke((Func<ServicePageItem>)(() => ((ServicePageItem)spServiceItemsRight.Children[_itemOpcIndex])));
                if (opcItem.State.Error != opcError)
                {
                    workerUpdater.ReportProgress(0, new ServicePageItemState() { Label = _itemOpcLabel, Error = opcError, Index = _itemOpcIndex, Postion = ServiceScreenPostion.RightBox });
                }

                var sqlError = Dispatcher.Invoke((Func<bool>)(() => ((MainWindow)Application.Current.MainWindow).DbErrorState));
                var sqlItem = Dispatcher.Invoke((Func<ServicePageItem>)(() => ((ServicePageItem)spServiceItemsRight.Children[_itemSqlIndex])));
                if (sqlItem.State.Error != sqlError)
                {
                    workerUpdater.ReportProgress(0, new ServicePageItemState() { Label = _itemSqlLabel, Error = sqlError, Index = _itemSqlIndex, Postion = ServiceScreenPostion.RightBox });
                }

                var printerError = Dispatcher.Invoke((Func<bool>)(() => ((MainWindow)Application.Current.MainWindow).PrinterErrorState));
                var printerItem = Dispatcher.Invoke((Func<ServicePageItem>)(() => ((ServicePageItem)spServiceItemsRight.Children[_itemPrinterIndex])));
                if (printerItem.State.Error != printerError)
                {
                    workerUpdater.ReportProgress(0, new ServicePageItemState() { Label = _itemPrinterLabel, Error = printerError, Index = _itemPrinterIndex, Postion = ServiceScreenPostion.RightBox });
                }

                var keyboardError = Dispatcher.Invoke((Func<bool>)(() => ((MainWindow)Application.Current.MainWindow).KeyboardErrorState));
                var keyboardItem = Dispatcher.Invoke((Func<ServicePageItem>)(() => ((ServicePageItem)spServiceItemsRight.Children[_itemKeyboardIndex])));
                if (keyboardItem.State.Error != keyboardError)
                {
                    workerUpdater.ReportProgress(0, new ServicePageItemState() { Label = _itemKeyboardLabel, Error = keyboardError, Index = _itemKeyboardIndex, Postion = ServiceScreenPostion.RightBox });
                }

                var remoteConnection = Dispatcher.Invoke((Func<DbConnection>)(() => ((MainWindow)Application.Current.MainWindow).ServerConnection));
                var remoteServerItem = Dispatcher.Invoke((Func<ServicePageItem>)(() => ((ServicePageItem)spServiceItemsRight.Children[_itemRemoteSqlIndex])));
                var remoteConnectionError = remoteConnection == null;
                if (remoteServerItem.State.Error != remoteConnectionError)
                {
                    workerUpdater.ReportProgress(0, new ServicePageItemState() { Label = _itemRemoteSqlLabel, Error = remoteConnectionError, Index = _itemRemoteSqlIndex, Postion = ServiceScreenPostion.RightBox });
                }
                #endregion

                var paperState = Dispatcher.Invoke((Func<PrinterPaperStatus>)(() => ((MainWindow)Application.Current.MainWindow).printerPaperStatus));
                if (printerPaperStatus == null || printerPaperStatus != paperState)
                {
                    workerUpdater.ReportProgress(0, paperState);
                }

                var dbWrite = Dispatcher.Invoke((Func<DbWrite>)(() => ((MainWindow)Application.Current.MainWindow).DbWrite));
                if (dbWrite == null && printerResetActive)
                {
                    workerUpdater.ReportProgress(0, false);
                }
                if (dbWrite != null && !printerResetActive)
                {
                    workerUpdater.ReportProgress(0, true);
                }

                var tbTransactionText = Dispatcher.Invoke((Func<String>)(() => (tbTransaction.Text)));
                var transaction = Dispatcher.Invoke((Func<Transaction>)(() => (Transaction)));
                var cardId = transaction == null ? "brak" : transaction.Card.CardId;
                if (!tbTransactionText.StartsWith(cardId))
                {
                    workerUpdater.ReportProgress(0, transaction);
                }

                Thread.Sleep(1000);
            }
        }

        private void WorkerUpdaterChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.UserState is ServicePageItemState)
            {
                var state = (ServicePageItemState)e.UserState;
                if (state.Postion == ServiceScreenPostion.RightBox)
                {
                    ((ServicePageItem)spServiceItemsRight.Children[state.Index]).State = state;
                }
                else
                {
                    ((ServicePageItem)spServiceItems.Children[state.Index]).State = state;
                }
            }
            if (e.UserState is PrinterPaperStatus)
            {
                BindPrinterPaperStatus((PrinterPaperStatus)e.UserState);
            }
            if (e.UserState is bool)
            {
                MakePaperResetActive((bool)e.UserState);
            }
            if (e.UserState is Transaction)
            {
                BindTransaction();
            }
        }

        private void MakePaperResetActive(bool paperResetActive)
        {
            if (paperResetActive)
            {
                printerResetActive = true;
                btnReset.Visibility = Visibility.Visible;
            }
            else
            {
                printerResetActive = false;
                btnReset.Visibility = Visibility.Hidden;
            }
        }

        private void BindPrinterPaperStatus(PrinterPaperStatus printerState)
        {
            printerPaperStatus = printerState;
            tbPrintedNumber.Text = printerPaperStatus.ReciptPrintedLenghtInMilimeters.ToString() + " mm";
            tbPrintLeft.Text = printerPaperStatus.ReciptPaperLeftInMilimeters.ToString() + " mm";
        }

        private void BindTransaction()
        {
            if (Transaction == null)
            {
                tbTransaction.Text = "brak";
            }
            else
            {
                tbTransaction.Text = String.Format("{0} [{1}]", Transaction.Card.CardId, Transaction.Address.Customer.Name);
            }
        }
        #endregion
        
        private void ShowPrinterResetConfirmation()
        {
            printerResetConfirmation = true;
            btnCancel.Visibility = Visibility.Visible;
            btnConfirm.Visibility = Visibility.Visible;
            btnReset.Visibility = Visibility.Hidden;
        }
        private void HidePrinterResetConfirmation()
        {
            printerResetConfirmation = false;
            btnCancel.Visibility = Visibility.Hidden;
            btnConfirm.Visibility = Visibility.Hidden;
            btnReset.Visibility = Visibility.Visible;
        }
        private void ResetPrinterPaper()
        {
            var dbWrite = ((MainWindow)Application.Current.MainWindow).DbWrite;
            if (dbWrite != null)
            {
                var resetOk = dbWrite.ResetPrintedPaperLenght();
                if (resetOk)
                {
                    ((MainWindow)Application.Current.MainWindow).printerPaperStatus.ReciptPrintedLenghtInMilimeters = 0;
                    BindPrinterPaperStatus(((MainWindow)Application.Current.MainWindow).printerPaperStatus);
                }
            }
        }

        public void RfidPing(string cardId, ReaderLocation location, RfidCard card)
        {
            bCardNumber.Text = cardId;
            bReaderLocation.Text = location.ToString();
            if (card != null)
            {
                if (card.Type == RfidCardType.Customer)
                {
                    bCardType.Text = card.Type.ToString();
                    bCustomerName.Text = card.Customer.Name;
                    bCustomerAddr1.Text = card.Address.AddressLine1;
                    bCustomerAddr2.Text = card.Address.AddressLine2;
                    bCustomerCity.Text = card.Address.PostCode + " " + card.Address.City;
                }
                if (card.Type == RfidCardType.Superuser)
                {
                    bCardType.Text = card.Type.ToString();
                    bCustomerName.Text = "";
                    bCustomerAddr1.Text = "";
                    bCustomerAddr2.Text = "";
                    bCustomerCity.Text = "";
                }
            }
            else
            {
                bCardType.Text = RfidCardType.Unknown.ToString();
                bCustomerName.Text = "";
                bCustomerAddr1.Text = "";
                bCustomerAddr2.Text = "";
                bCustomerCity.Text = "";
            }
        }

        private void ServicePage_KeyDown(object sender, KeyEventArgs e)
        {
            if (!passwordCorrect)
            {
                if (e.Key == Key.Enter) CheckPassword();
                return;
            }

            if (e.Key == Key.R)
            {
                ShowPrinterResetConfirmation();
            }

            if (e.Key == Key.T && printerResetConfirmation)
            {
                ResetPrinterPaper();
                HidePrinterResetConfirmation();
            }
            if (e.Key == Key.N && printerResetConfirmation)
            {
                HidePrinterResetConfirmation();
            }
        }

        private void CheckPassword()
        {
            var pass = pbPassword.Password;
            var kioskConfig = Dispatcher.Invoke((Func<KioskConfiguration>)(() => ((MainWindow)Application.Current.MainWindow).KioskConf));
            
            if (pass == _servicePass)
            {
                passwordCorrect = true;
                gPassword.Visibility = Visibility.Hidden;
            }
            else
            {
                tbBadPassword.Visibility = Visibility.Visible;
            }
        }

        private void PageUnload(object sender, RoutedEventArgs e)
        {
            passwordCorrect = false;
            tbBadPassword.Visibility = Visibility.Hidden;
            Application.Current.MainWindow.KeyDown -= new KeyEventHandler(ServicePage_KeyDown);
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.KeyDown -= new KeyEventHandler(ServicePage_KeyDown);
            Application.Current.MainWindow.KeyDown += new KeyEventHandler(ServicePage_KeyDown);

            BindTransaction();

            if (!passwordCorrect)
            {
                tbBadPassword.Visibility = Visibility.Hidden;
                gPassword.Visibility = Visibility.Visible;
                pbPassword.Focus();
            }

            if (((MainWindow)Application.Current.MainWindow).printerPaperStatus != null)
            {
                BindPrinterPaperStatus(((MainWindow)Application.Current.MainWindow).printerPaperStatus);
            }
        }
    }
}

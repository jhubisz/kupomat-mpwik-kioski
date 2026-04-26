using DbCommunication;
using DbCommunication.Entities;
using DbCommunication.Enums;
using KioskAppWpf.Classes;
using KioskAppWpf.Enums;
using KioskAppWpf.Pages;
using KioskAppWpf.Pages.Errors;
using KioskAppWpf.WebApi;
using Microsoft.Owin.Hosting;
using OpcCommunication;
using OpcCommunication.Devices;
using OpcComunication.Devices;
using PrinterCommunication;
using RfidCommunication;
using RfidCommunication.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Utilities.Classes;

namespace KioskAppWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int _opoznienieStartuAplikacji
        {
            get
            {
                return Int32.Parse(ConfigurationManager.AppSettings["_opoznienieStartuAplikacji"]);
            }
        }
        private string _wersja
        {
            get
            {
                //return "v.1.13.1";
                return "v.1.15";
            }
        }

        private int _kioskId
        {
            get
            {
                return Int32.Parse(ConfigurationManager.AppSettings["_kioskId"]);
            }
        }
        private bool _debugMode
        {
            get
            {
                return ConfigurationManager.AppSettings["_debugMode"] == "1";
            }
        }

        private int _supervisorCardCustomerId
        {
            get
            {
                return Int32.Parse(ConfigurationManager.AppSettings["_supervisorCardCustomerId"]);
            }
        }

        private int _SequenceConfirmationTimeout
        {
            get
            {
                return Int32.Parse(ConfigurationManager.AppSettings["_SequenceConfirmationTimeout"]);
            }
        }

        private int _errorPageDurationInSeconds
        {
            get
            {
                return Int32.Parse(ConfigurationManager.AppSettings["_errorPageDurationInSeconds"]);
            }
        }

        private int _screenCleaningTimeout
        {
            get
            {
                return Int32.Parse(ConfigurationManager.AppSettings["_screenCleaningTimeout"]);
            }
        }
        private int _pathChangingTimeout
        {
            get
            {
                return Int32.Parse(ConfigurationManager.AppSettings["_pathChangingTimeout"]);
            }
        }

        private int _lastBottleNo
        {
            get
            {
                return Int32.Parse(ConfigurationManager.AppSettings["_lastBottleNo"]);
            }
        }

        private bool _printerOn
        {
            get
            {
                return ConfigurationManager.AppSettings["_printerOn"] == "1";
            }
        }
        private string _printerName
        {
            get
            {
                return ConfigurationManager.AppSettings["_printerName"];
            }
        }

        private string _mainPageHead
        {
            get
            {
                return ConfigurationManager.AppSettings["_mainPageHead"];
            }
        }
        private string _mainPageAddr1
        {
            get
            {
                return ConfigurationManager.AppSettings["_mainPageAddr1"];
            }
        }
        private string _mainPageAddr2
        {
            get
            {
                return ConfigurationManager.AppSettings["_mainPageAddr2"];
            }
        }
        private string _mainPageAddr3
        {
            get
            {
                return ConfigurationManager.AppSettings["_mainPageAddr3"];
            }
        }

        private bool _opcOn
        {
            get
            {
                return ConfigurationManager.AppSettings["_opcOn"] == "1";
            }
        }

        private int _OpenGeteTimeInSeconds
        {
            get
            {
                return Int32.Parse(ConfigurationManager.AppSettings["_OpenGeteTimeInSeconds"]);
            }
        }
        private int _OpenDistributionTimeInSeconds
        {
            get
            {
                return Int32.Parse(ConfigurationManager.AppSettings["_OpenDistributionTimeInSeconds"]);
            }
        }
        private int _OpenScreenRoomTimeInSeconds
        {
            get
            {
                return Int32.Parse(ConfigurationManager.AppSettings["_OpenScreenRoomTimeInSeconds"]);
            }
        }
        private int _OpenToiletTimeInSeconds
        {
            get
            {
                return Int32.Parse(ConfigurationManager.AppSettings["_OpenToiletTimeInSeconds"]);
            }
        }

        private int _StartupScreenTimeoutInSeconds
        {
            get
            {
                return Int32.Parse(ConfigurationManager.AppSettings["_StartupScreenTimeoutInSeconds"]);
            }
        }

        private int _UnknownRegistrationNumberInSeconds
        {
            get
            {
                return Int32.Parse(ConfigurationManager.AppSettings["_UnknownRegistrationNumberInSeconds"]);
            }
        }

        public int _reciptLenghtInMilimeters
        {
            get
            {
                return Int32.Parse(ConfigurationManager.AppSettings["_reciptLenghtInMilimeters"]);
            }
        }
        public int _reciptPaperLenghtInMilimeters
        {
            get
            {
                return Int32.Parse(ConfigurationManager.AppSettings["_reciptPaperLenghtInMilimeters"]);
            }
        }
        public PrinterPaperStatus printerPaperStatus;

        private bool servicePageOn;

        private KioskConfiguration kioskConf;
        public KioskConfiguration KioskConf
        {
            get
            {
                return kioskConf;
            }
            set
            {
                kioskConf = value;

                tbOpenHours.Text = kioskConf.OpenFromString + "-" + kioskConf.OpenToString;
                if (kioskConf.Open == KioskOpenState.Open)
                {
                    tbOpenState.Text = "OTWARTE";
                    tbOpenState.Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 182, 0));
                    tbOpenState.FontFamily = new FontFamily("Lucida Console");
                    tbOpenHours.Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 182, 0));
                    tbOpenHours.FontFamily = new FontFamily("Lucida Console");
                }
                else
                {
                    tbOpenState.Text = "ZAMKNIĘTE";
                    tbOpenState.Foreground = Brushes.DarkRed;
                    tbOpenHours.Foreground = Brushes.DarkRed;
                }
            }
        }

        public DisplayPage CurrentPage { get; set; }
        public DisplayPage ShowingPage { get; set; }
        private Transaction transaction;
        public Transaction Transaction
        {
            get
            {
                return transaction;
            }
            set
            {
                transaction = value;
            }
        }
        public CustomerAddress CurrentTransactionAddress { get; set; }
        public CustomerAddress PrevTransactionAddress { get; set; }
        public Company CurrentTransactionCompany { get; set; }
        public CompanySearchType CompanySearchType { get; set; }
        public TransactionStep Step { get; set; }
        public string CardAllowedToOpen { get; set; }
        public bool SupervisorIn { get; set; }

        public OpcManager OpcManager { get; set; }
        public DevicesState DevicesState { get; set; }

        public DbLocalConfiguration DbLocalConfiguration { get; set; }
        public DbSyncConfiguration DbSyncConfiguration { get; set; }
        public DbServerConfiguration DbServerConfiguration { get; set; }
        public DbSavingServiceConfiguration DbSavingServiceConfiguration { get; set; }

        public DbConnection Connection { get; set; }
        public DbRead DbRead { get; set; }
        public DbWrite DbWrite { get; set; }

        public DbConnection SyncConnection { get; set; }
        public DbConnection ServerConnection { get; set; }
        public DbServerRead DbServerRead { get; set; }
        public DbServerWrite DbServerWrite { get; set; }

        public ExportManager ExportManager { get; set; }

        public bool UnknownCameraSet { get; set; }
        public DateTime UnknownCameraSetTimer { get; set; }
        public string LastRegistrationFromCamera { get; set; }
        public string LastRegistrationFromCamera2 { get; set; }
        public int LastDumpPathId { get; set; }

        #region Life Bit
        public short PlcLifeBit { get; set; }
        public DateTime PlcLifeBitLastStateChange { get; set; }
        public short KioskLifeBit { get; set; }

        public bool SequenceRunning { get; set; }
        public short TransactionLifeBit { get; set; }
        public DateTime TransactionLifeBitLastStateChange { get; set; }
        public short TransactionKioskLifeBit { get; set; }
        #endregion

        #region Errors
        public bool RfidErrorState { get; set; }
        public bool DbErrorState { get; set; }
        public bool OpcErrorState { get; set; }

        public bool PlcLifeBitError { get; set; }
        public bool TransactionLifeBitError { get; set; }

        public bool Power230LostError { get; set; }
        public bool Power24LostError { get; set; }
        public bool FlowMeterError { get; set; }
        public bool KioskDoorError { get; set; }
        public bool ProbeDoorOpenError { get; set; }

        public bool AirError { get; set; }
        public bool PhMeterError { get; set; }
        public bool CondMeterError { get; set; }
        public bool TempMeterError { get; set; }
        public bool ChztMeterError { get; set; }
        public bool PressureMeterError { get; set; }
        public bool ProbeError { get; set; }
        public bool ProbeReadyError { get; set; }

        public bool Distribution230VACError { get; set; }
        public bool Distribution230VACUpsError { get; set; }
        public bool Rack230VACError { get; set; }
        public bool Rack24VDCError { get; set; }
        public bool Rack24VDCUpsError { get; set; }
        public bool Rack12VDCError { get; set; }

        public bool PumpRoomError { get; set; }
        public bool PumpRoomWorkError { get; set; }
        public bool ScreenRoomLeakError { get; set; }
        public bool ScreenError { get; set; }
        public bool ScreenFullError { get; set; }

        public bool ValveMeterError { get; set; }
        public bool ZasuwaError { get; set; }

        public bool AugerError { get; set; }
        public bool PreScreenError { get; set; }
        public bool DumpPathError { get; set; }

        public bool ProbeFullError { get; set; }

        public bool KeyboardErrorState { get; set; }
        public bool PrinterErrorState { get; set; }
        public bool PrinterOutOfPaperState { get; set; }
        public bool PrinterOutOfPaperErrorState
        {
            get
            {
                var printerOutOfPaper = false;
                if (printerPaperStatus == null)
                {
                    printerOutOfPaper = true;
                }
                else
                {
                    printerOutOfPaper = printerPaperStatus.OutOfPaper;
                }
                return printerOutOfPaper;
            }
        }

        public bool ErrorState
        {
            get
            {
                if (DevicesState == null || KioskConf == null) return true;

                var errorState = RfidErrorState || DbErrorState || OpcErrorState || PlcLifeBitError
                    || KeyboardErrorState
                    || Power230LostError || Power24LostError || FlowMeterError || DumpPathError
                    || ScreenError || ScreenFullError
                    || KioskDoorError || ProbeDoorOpenError || AirError
                    || Distribution230VACError || Rack230VACError || Rack24VDCError || Rack24VDCUpsError || Rack12VDCError
                    || PumpRoomError || PumpRoomWorkError || ScreenRoomLeakError || AugerError || PreScreenError;

                if (PrinterErrorState && _printerOn && !KioskConf.KioskBlockages[KioskBlockageType.Printer]) errorState = true;

                if (ProbeError && !KioskConf.KioskBlockages[KioskBlockageType.TakingSamples]) errorState = true;
                if (ProbeReadyError && !KioskConf.KioskBlockages[KioskBlockageType.TakingSamples]) errorState = true;
                if (ProbeFullError && !KioskConf.KioskBlockages[KioskBlockageType.TakingSamples]) errorState = true;
                if (PhMeterError && !KioskConf.KioskBlockages[KioskBlockageType.MeasuringPh]) errorState = true;
                if (CondMeterError && !KioskConf.KioskBlockages[KioskBlockageType.MeasuringConduction]) errorState = true;
                if (TempMeterError && !KioskConf.KioskBlockages[KioskBlockageType.MeasuringTemperature]) errorState = true;
                if (ChztMeterError && !KioskConf.KioskBlockages[KioskBlockageType.MeasuringChzt]) errorState = true;
                if (PressureMeterError && !KioskConf.KioskBlockages[KioskBlockageType.MeasuringPressure]) errorState = true;
                if (ValveMeterError && !KioskConf.KioskBlockages[KioskBlockageType.WaterForMeasuringDevices]) errorState = true;
                if (ZasuwaError && !KioskConf.KioskBlockages[KioskBlockageType.Zasuwa]) errorState = true;

                if (DevicesState.Flow.SamplerBottleNo >= _lastBottleNo && !KioskConf.KioskBlockages[KioskBlockageType.TakingSamples] && !KioskConf.KioskBlockages[KioskBlockageType.ProbeBottles]) return true;

                if (PrinterOutOfPaperErrorState && Transaction == null && !KioskConf.KioskBlockages[KioskBlockageType.Printer])
                    errorState = true;
                return errorState;
            }
        }

        public bool FocusLost { get; set; }

        public DateTime? ErrorStartDateTime { get; set; }
        #endregion

        #region Workers
        private static BackgroundWorker workerRfidStatus = new BackgroundWorker();
        private static BackgroundWorker workerLifebitStatus = new BackgroundWorker();
        private static BackgroundWorker workerOpcStatus = new BackgroundWorker();
        private static BackgroundWorker workerDb = new BackgroundWorker();
        private static BackgroundWorker workerStateMonitoring = new BackgroundWorker();
        private static BackgroundWorker workerLightsSteering = new BackgroundWorker();
        private static BackgroundWorker workerKioskMonitoring = new BackgroundWorker();
        #endregion

        #region Timers
        private DateTime? GateTimer { get; set; }
        private DateTime? GateOutTimer { get; set; }
        private DateTime? ScreenRoomTimer { get; set; }
        private DateTime? DistributionTimer { get; set; }
        private DateTime? ToiletTimer { get; set; }

        private DateTime? ScreenCleaningTimer { get; set; }
        private DateTime? PathChangingTimer { get; set; }
        #endregion

        #region display pages
        private RfidCardPage rfidCardPage;
        public RfidCardPage RfidCardPage
        {
            get
            {
                if (rfidCardPage == null)
                    rfidCardPage = new RfidCardPage();
                return rfidCardPage;
            }
        }
        private DbTypePage dbTypePage;
        public DbTypePage DbTypePage
        {
            get
            {
                if (dbTypePage == null)
                    dbTypePage = new DbTypePage();
                return dbTypePage;
            }
        }
        private CargoTypePage cargoTypePage;
        public CargoTypePage CargoTypePage
        {
            get
            {
                if (cargoTypePage == null)
                    cargoTypePage = new CargoTypePage();
                return cargoTypePage;
            }
        }
        private SelectGmina selectGmina;
        public SelectGmina SelectGmina
        {
            get
            {
                if (selectGmina == null)
                    selectGmina = new SelectGmina();
                return selectGmina;
            }
        }
        private SelectMiejscowosc selectMiejscowosc;
        public SelectMiejscowosc SelectMiejscowosc
        {
            get
            {
                if (selectMiejscowosc == null)
                    selectMiejscowosc = new SelectMiejscowosc();
                return selectMiejscowosc;
            }
        }
        private SelectUlica selectUlica;
        public SelectUlica SelectUlica
        {
            get
            {
                if (selectUlica == null)
                    selectUlica = new SelectUlica();
                return selectUlica;
            }
        }
        private SelectRod selectRod;
        public SelectRod SelectRod
        {
            get
            {
                if (selectRod == null)
                    selectRod = new SelectRod();
                return selectRod;
            }
        }
        private SelectCompanySearchType selectCompanySearchtype;
        public SelectCompanySearchType SelectCompanySearchType
        {
            get
            {
                if (selectCompanySearchtype == null)
                    selectCompanySearchtype = new SelectCompanySearchType();
                return selectCompanySearchtype;
            }
        }
        private SelectCompanyByName selectCompanyByName;
        public SelectCompanyByName SelectCompanyByName
        {
            get
            {
                if (selectCompanyByName == null)
                    selectCompanyByName = new SelectCompanyByName();
                return selectCompanyByName;
            }
        }
        private SelectCompanyName selectCompanyName;
        public SelectCompanyName SelectCompanyName
        {
            get
            {
                if (selectCompanyName == null)
                    selectCompanyName = new SelectCompanyName();
                return selectCompanyName;
            }
        }
        private SelectNumer selectNumer;
        public SelectNumer SelectNumer
        {
            get
            {
                if (selectNumer == null)
                    selectNumer = new SelectNumer();
                return selectNumer;
            }
        }
        private SelectContractNo selectContractNo;
        public SelectContractNo SelectContractNo
        {
            get
            {
                if (selectContractNo == null)
                    selectContractNo = new SelectContractNo();
                return selectContractNo;
            }
        }
        private SelectAmount selectAmount;
        public SelectAmount SelectAmount
        {
            get
            {
                if (selectAmount == null)
                    selectAmount = new SelectAmount();
                return selectAmount;
            }
        }
        private InsertGmina insertgmina;
        public InsertGmina InsertGmina
        {
            get
            {
                if (insertgmina == null)
                    insertgmina = new InsertGmina();
                return insertgmina;
            }
        }
        private InsertMiejscowosc insertMiejscowosc;
        public InsertMiejscowosc InsertMiejscowosc
        {
            get
            {
                if (insertMiejscowosc == null)
                    insertMiejscowosc = new InsertMiejscowosc();
                return insertMiejscowosc;
            }
        }
        private InsertUlica insertUlica;
        public InsertUlica InsertUlica
        {
            get
            {
                if (insertUlica == null)
                    insertUlica = new InsertUlica();
                return insertUlica;
            }
        }
        private InsertCompanyName insertCompanyName;
        public InsertCompanyName InsertCompanyName
        {
            get
            {
                if (insertCompanyName == null)
                    insertCompanyName = new InsertCompanyName();
                return insertCompanyName;
            }
        }
        private InsertPlotNumber insertPlotNumber;
        public InsertPlotNumber InsertPlotNumber
        {
            get
            {
                if (insertPlotNumber == null)
                    insertPlotNumber = new InsertPlotNumber();
                return insertPlotNumber;
            }
        }
        private AddressSummary addressSummary;
        public AddressSummary AddressSummary
        {
            get
            {
                if (addressSummary == null)
                    addressSummary = new AddressSummary();
                return addressSummary;
            }
        }
        private DumpPage dumpPage;
        public DumpPage DumpPage
        {
            get
            {
                if (dumpPage == null)
                    dumpPage = new DumpPage();
                return dumpPage;
            }
        }
        private ScreenCleaningPage screenCleaningPage;
        public ScreenCleaningPage ScreenCleaningPage
        {
            get
            {
                if (screenCleaningPage == null)
                    screenCleaningPage = new ScreenCleaningPage();
                return screenCleaningPage;
            }
        }
        private ScreenCleaningTimeout screenCleaningTimeout;
        public ScreenCleaningTimeout ScreenCleaningTimeout
        {
            get
            {
                if (screenCleaningTimeout == null)
                    screenCleaningTimeout = new ScreenCleaningTimeout();
                return screenCleaningTimeout;
            }
        }
        private PathChangingPage pathChangingPage;
        public PathChangingPage PathChangingPage
        {
            get
            {
                if (pathChangingPage == null)
                    pathChangingPage = new PathChangingPage();
                return pathChangingPage;
            }
        }
        private PathChangingTimeout pathChangingTimeout;
        public PathChangingTimeout PathChangingTimeout
        {
            get
            {
                if (pathChangingTimeout == null)
                    pathChangingTimeout = new PathChangingTimeout();
                return pathChangingTimeout;
            }
        }
        private KioskBlocked kioskBlockedPage;
        public KioskBlocked KioskBlockedPage
        {
            get
            {
                if (kioskBlockedPage == null)
                    kioskBlockedPage = new KioskBlocked();
                return kioskBlockedPage;
            }
        }
        private UnknownCardPage unknownCardPage;
        public UnknownCardPage UnknownCardPage
        {
            get
            {
                if (unknownCardPage == null)
                    unknownCardPage = new UnknownCardPage();
                return unknownCardPage;
            }
        }
        private NoLicensePage noLicensePage;
        public NoLicensePage NoLicensePage
        {
            get
            {
                if (noLicensePage == null)
                    noLicensePage = new NoLicensePage();
                return noLicensePage;
            }
        }
        private CardBlockedPage cardBlockedPage;
        public CardBlockedPage CardBlockedPage
        {
            get
            {
                if (cardBlockedPage == null)
                    cardBlockedPage = new CardBlockedPage();
                return cardBlockedPage;
            }
        }
        private NoRodPage noRodPage;
        public NoRodPage NoRodPage
        {
            get
            {
                if (noRodPage == null)
                    noRodPage = new NoRodPage();
                return noRodPage;
            }
        }
        private GateOpenPage gateOpenPage;
        public GateOpenPage GateOpenPage
        {
            get
            {
                if (gateOpenPage == null)
                    gateOpenPage = new GateOpenPage();
                return gateOpenPage;
            }
        }
        private ServicePage servicePage;
        public ServicePage ServicePage
        {
            get
            {
                if (servicePage == null)
                    servicePage = new ServicePage();
                return servicePage;
            }
        }
        private ThankYouPage thankYouPage;
        public ThankYouPage ThankYouPage
        {
            get
            {
                if (thankYouPage == null)
                    thankYouPage = new ThankYouPage();
                return thankYouPage;
            }
        }
        private NoFlowPage noFlowPage;
        public NoFlowPage NoFlowPage
        {
            get
            {
                if (noFlowPage == null)
                    noFlowPage = new NoFlowPage();
                return noFlowPage;
            }
        }
        private StationNotReadyPage stationNotReadyPage;
        public StationNotReadyPage StationNotReadyPage
        {
            get
            {
                if (stationNotReadyPage == null)
                    stationNotReadyPage = new StationNotReadyPage();
                return stationNotReadyPage;
            }
        }
        private ProbeService probeServicePage;
        public ProbeService ProbeServicePage
        {
            get
            {
                if (probeServicePage == null)
                    probeServicePage = new ProbeService();
                return probeServicePage;
            }
        }
        private ParameterAlertPage parameterAlertPage;
        public ParameterAlertPage ParameterAlertPage
        {
            get
            {
                if (parameterAlertPage == null)
                    parameterAlertPage = new ParameterAlertPage();
                return parameterAlertPage;
            }
        }
        private StartupPage startupPage;
        public StartupPage StartupPage
        {
            get
            {
                if (startupPage == null)
                    startupPage = new StartupPage();
                return startupPage;
            }
        }
        #endregion

        private bool KioskBlocked { get; set; }

        private DispatcherTimer ErrorPageTimer { get; set; }

        private ErrorPage ErrorPage { get; set; }

        private TransactionSavingService TransactionSavingService { get; set; }

        public DateTime StartupTime { get; set; }
        private bool startup;
        public bool Startup
        {
            get
            {
                return startup;
            }
            set
            {
                if (value)
                    StartupTime = DateTime.Now;

                startup = value;
            }
        }

        private IDisposable webApp;

        public MainWindow()
        {
            Thread.Sleep(_opoznienieStartuAplikacji);

            if (!ApplicationIsActivated())
            {
                ShowApp();
            }

            //StartApiServer();

            CurrentPage = DisplayPage.RfidCardPage;
            Step = TransactionStep.NoTransaction;
            InitializeComponent();

            Startup = true;
            ShowPage(DisplayPage.StartupPage);

            tbVersion.Text = _wersja;

            DbLocalConfiguration = new DbLocalConfiguration();
            DbSyncConfiguration = new DbSyncConfiguration();
            DbServerConfiguration = new DbServerConfiguration();
            DbSavingServiceConfiguration = new DbSavingServiceConfiguration();

            BindMainPageLabels();

            this.KeyDown -= new KeyEventHandler(MainWindow_KeyDown);
            this.KeyDown += new KeyEventHandler(MainWindow_KeyDown);

            RunClock();
            BindPage();

            RunRfidWorker();
            RunLifebitWorker();
            RunOpcWorker();
            RunDbMonitoring();
            RunKioskMonitoring();
        }

        private void StartApiServer()
        {
            var baseAddress = "http://localhost:5000";

            try
            {
                webApp = WebApp.Start<Startup>(url: baseAddress);
                MessageBox.Show($"API running at {baseAddress}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error starting API: {ex.Message}");
            }
        }

        private void BindMainPageLabels()
        {
            tbMainHead.Text = _mainPageHead;
            tbMainAddr1.Text = _mainPageAddr1;
            tbMainAddr2.Text = _mainPageAddr2;
            tbMainAddr3.Text = _mainPageAddr3;
        }

        #region Rfid Worker
        private void RunRfidWorker()
        {
            workerRfidStatus.DoWork -= WorkerRfidWork;
            workerRfidStatus.DoWork += WorkerRfidWork;
            workerRfidStatus.ProgressChanged -= WorkerRfidProgressChanged;
            workerRfidStatus.ProgressChanged += WorkerRfidProgressChanged;
            workerRfidStatus.WorkerReportsProgress = true;

            if (!workerRfidStatus.IsBusy)
                workerRfidStatus.RunWorkerAsync();
        }

        private void WorkerRfidWork(object sender, DoWorkEventArgs e)
        {
            var rfid = new RfidManager(new RfidConfig());

            while (true)
            {
                var eventList = rfid.CheckCardStatus();
                if (eventList.Count > 0 || RfidErrorState)
                {
                    workerRfidStatus.ReportProgress(0, eventList);
                }
                Thread.Sleep(1000);
            }
        }

        private void WorkerRfidProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            var readerEvents = (List<TouchedCard>)e.UserState;

            var rfidError = CheckRfidCardError(readerEvents);

            if (!rfidError && readerEvents.Count > 0 && DbRead != null)
            {
                var readerEvent = readerEvents[0];
                if (!String.IsNullOrEmpty(readerEvent.LicensePlate))
                {
                    LogDebug("Odczyt rejestracji " + readerEvent.LicensePlate);
                    var licensePlate = readerEvent.LicensePlate;
                    var card = DbRead.GetRfidCardByLicensePlate(licensePlate);

                    RfidCardAction(card != null ? card.CardId : "", card, readerEvents[0].CardLocation);
                }
                else
                {
                    var rfidCardId = readerEvent.CardId;
                    var card = DbRead.GetRfidCard(rfidCardId);
                    RfidCardAction(rfidCardId, card, readerEvents[0].CardLocation);
                }
            }
        }

        private bool CheckRfidCardError(List<TouchedCard> eventList)
        {
            foreach (var evnt in eventList)
            {
                if (evnt.Error)
                {
                    EnterRfidErrorState(evnt.ErrorMsg);
                    return true;
                }
            }
            if (RfidErrorState)
                EndRfidErrorState();
            return false;
        }

        private void RfidCardAction(string cardId, RfidCard card, ReaderLocation location)
        {
            LogDebug("RFiD - przyłożenie karty " + cardId + ": " + location);

            if (fMain.Content is ServicePage)
            {
                ((ServicePage)fMain.Content).RfidPing(cardId, location, card);
            }

            if (KioskConf.Open == KioskOpenState.Closed && (card == null || card.Type != RfidCardType.Superuser))
            {
                return;
            }

            if (KioskBlocked)
            {
                return;
            }

            if (card == null)
            {
                LogDebug("RFiD - nieznana karta: " + cardId);

                if (location != ReaderLocation.Camera)
                    ShowErrorPage(DisplayPage.UnknownCardPage);

                return;
            }

            if (card.Blocked)
            {
                LogDebug("RFiD - karta zablokowana: " + cardId);

                if (location != ReaderLocation.Camera)
                    ShowErrorPage(DisplayPage.CardBlocked);

                return;
            }

            var cardCapturedInSequence = false;
            if (DevicesState != null || !_opcOn)
            {
                switch (card.Type)
                {
                    case RfidCardType.Customer:
                        cardCapturedInSequence = CheckNormalCardSequence(location, card);
                        break;
                    case RfidCardType.Superuser:
                        cardCapturedInSequence = CheckTrayCardSequence(location, card);
                        break;
                }
            }
        }

        private bool CheckNormalCardSequence(ReaderLocation location, RfidCard card)
        {
            if (location == ReaderLocation.Kiosk && Transaction == null)
            {
                var license = DbRead.GetLicense(card.Customer.Id);
                if (license != null)
                    LogDebug("RFiD - odczytanie licencji: " + license.Number);
                else
                    LogDebug("RFiD - odczytanie licencji: brak licencji");

                if (license == null)
                {
                    ShowErrorPage(DisplayPage.NoLicensePage);
                    return true;
                }

                if (DevicesState != null)
                {
                    var success = StartNewTransaction(card, license);
                    if (success)
                    {
                        if (Transaction.DumpStarted) return true;

                        BindPage(DisplayPage.CargoTypePage);
                        return true;
                    }
                }
            }

            if (location == ReaderLocation.Camera)
            {
                SendOpenGateSignal();
            }
            if (location == ReaderLocation.GateIn
                && !KioskConf.KioskBlockages[KioskBlockageType.GatesSteering])
            {
                SendOpenGateSignal();
            }
            if (location == ReaderLocation.GateOut
                && !KioskConf.KioskBlockages[KioskBlockageType.GatesSteering])
            {
                SendOpenGateOutSignal();
            }
            if (location == ReaderLocation.Huber && card.Customer.Id == _supervisorCardCustomerId)
            {
                SendOpenScreenRoomSignal();
            }
            if (location == ReaderLocation.Rozdzielnia && card.Customer.Id == _supervisorCardCustomerId)
            {
                SendOpenDistributionSignal();
            }
            if (location == ReaderLocation.Toaleta
                && !KioskConf.KioskBlockages[KioskBlockageType.ToiletSteering])
            {
                SendOpenToiletSignal();
            }

            return false;
        }
        private void SendOpenDistributionSignal()
        {
            LogDebug("RFiD - Sygnał otwarcia dystrybucji START");
            DistributionTimer = DateTime.Now;
            if (OpcManager != null && OpcManager.Client.IsConnected)
            {
                LogDebug("RFiD - Sygnał otwarcia dystrybucji START - send OPC");
                OpcManager.SetDistributionOpen(true);
            }
        }
        private void RemoveOpenDistributionSignal()
        {
            LogDebug("RFiD - Sygnał otwarcia dystrybucji KONIEC");
            DistributionTimer = null;
            if (OpcManager != null && OpcManager.Client.IsConnected)
            {
                LogDebug("RFiD - Sygnał otwarcia dystrybucji KONIEC - send OPC");
                OpcManager.SetDistributionOpen(false);
            }
        }
        private void SendOpenScreenRoomSignal()
        {
            LogDebug("RFiD - Sygnał otwarcia pomieszczenia sita START");
            ScreenRoomTimer = DateTime.Now;
            if (OpcManager != null && OpcManager.Client.IsConnected)
            {
                LogDebug("RFiD - Sygnał otwarcia pomieszczenia sita START - send OPC");
                OpcManager.SetScreenRoomOpen(true);
            }
        }
        private void RemoveOpenScreenRoomSignal()
        {
            LogDebug("RFiD - Sygnał otwarcia pomieszczenia sita KONIEC");
            ScreenRoomTimer = null;
            if (OpcManager != null && OpcManager.Client.IsConnected)
            {
                LogDebug("RFiD - Sygnał otwarcia pomieszczenia sita KONIEC - send OPC");
                OpcManager.SetScreenRoomOpen(false);
            }
        }
        private void SendOpenToiletSignal()
        {
            LogDebug("RFiD - Sygnał otwarcia toalety START");
            ToiletTimer = DateTime.Now;
            if (OpcManager != null && OpcManager.Client.IsConnected)
            {
                LogDebug("RFiD - Sygnał otwarcia toalety START - send OPC");
                OpcManager.SetToiletOpen(true);
            }
        }
        private void RemoveOpenToiletSignal()
        {
            LogDebug("RFiD - Sygnał otwarcia toalety KONIEC");
            ToiletTimer = null;
            if (OpcManager != null && OpcManager.Client.IsConnected)
            {
                LogDebug("RFiD - Sygnał otwarcia toalety KONIEC - send OPC");
                OpcManager.SetToiletOpen(false);
            }
        }
        private void SendOpenGateSignal()
        {
            LogDebug("RFiD - Sygnał otwarcia bramy START");
            GateTimer = DateTime.Now;
            if (OpcManager != null && OpcManager.Client.IsConnected)
            {
                LogDebug("RFiD - Sygnał otwarcia bramy START - send OPC");
                OpcManager.SetGateOpen(true);
            }
        }
        private void RemoveOpenGateSignal()
        {
            LogDebug("RFiD - Sygnał otwarcia bramy KONIEC");
            GateTimer = null;
            if (OpcManager != null && OpcManager.Client.IsConnected)
            {
                LogDebug("RFiD - Sygnał otwarcia bramy KONIEC - send OPC");
                OpcManager.SetGateOpen(false);
            }
        }
        private void SendOpenGateOutSignal()
        {
            LogDebug("RFiD - Sygnał otwarcia bramy wyjazdowej START");
            GateOutTimer = DateTime.Now;
            if (OpcManager != null && OpcManager.Client.IsConnected)
            {
                LogDebug("RFiD - Sygnał otwarcia bramy wyjazdowej START - send OPC");
                OpcManager.SetGateOutOpen(true);
            }
        }
        private void RemoveOpenGateOutSignal()
        {
            LogDebug("RFiD - Sygnał otwarcia bramy wyjazdowej KONIEC");
            GateOutTimer = null;
            if (OpcManager != null && OpcManager.Client.IsConnected)
            {
                LogDebug("RFiD - Sygnał otwarcia bramy wyjazdowej KONIEC - send OPC");
                OpcManager.SetGateOutOpen(false);
            }
        }

        private bool CheckTrayCardSequence(ReaderLocation location, RfidCard card)
        {
            if (Transaction != null || fMain.Content is ServicePage)
            {
                return true;
            }

            if (CurrentPage != DisplayPage.ProbeService)
            {
                BindPage(DisplayPage.ProbeService);
            }
            else
            {
                BindPage(DisplayPage.RfidCardPage);
            }
            return true;
        }
        #endregion

        #region lifebit Worker
        private void RunLifebitWorker()
        {
            workerLifebitStatus.DoWork -= WorkerLifebitWork;
            workerLifebitStatus.DoWork += WorkerLifebitWork;
            workerLifebitStatus.ProgressChanged -= WorkerLifebitProgressChanged;
            workerLifebitStatus.ProgressChanged += WorkerLifebitProgressChanged;
            workerLifebitStatus.WorkerReportsProgress = true;

            if (!workerLifebitStatus.IsBusy)
                workerLifebitStatus.RunWorkerAsync();
        }

        private void WorkerLifebitWork(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                try
                {
                    if (_opcOn)
                    {
                        var opcLive = CheckOpcIsLive();

                        if (opcLive)
                        {
                            LifeBitWork();
                            SequenceLifeBitWork();

                            if (OpcErrorState)
                            {
                                workerLifebitStatus.ReportProgress(0, OpcState.OpcLiveOk);
                            }
                        }
                    }

                    Thread.Sleep(500);
                }
                catch (Exception ex)
                {
                    Application.Current.Dispatcher.Invoke(new Action(() => { LogError(ex); }));
                }
            }
        }

        private void LifeBitWork()
        {
            if (!_opcOn) return;

            var lifeBitRead = OpcManager.CheckPlcLiveBit();
            if (PlcLifeBit != lifeBitRead)
            {
                PlcLifeBit = lifeBitRead;
                PlcLifeBitLastStateChange = DateTime.Now;
                if (PlcLifeBitError)
                {
                    workerLifebitStatus.ReportProgress(0, OpcState.PlcLifeBitOk);
                }
            }
            else if (PlcLifeBitLastStateChange.AddSeconds(20) < DateTime.Now)
            {
                workerLifebitStatus.ReportProgress(0, OpcState.PlcLifeBitError);
            }
            KioskLifeBit++;
            if (KioskLifeBit > 1000) KioskLifeBit = 0;
            OpcManager.SendKioskLiveBit(KioskLifeBit);
        }
        private void SequenceLifeBitWork()
        {
            if (TransactionLifeBitError)
            {
                workerLifebitStatus.ReportProgress(0, OpcState.TransactionLifeBitOk);
                return;
            }

            if (Transaction == null)
            {
                return;
            }

            if (SequenceRunning)
            {
                var lifeBitRead = OpcManager.CheckTransactionLiveBit();
                if (TransactionLifeBit != lifeBitRead)
                {
                    TransactionLifeBit = lifeBitRead;
                    TransactionLifeBitLastStateChange = DateTime.Now;
                }
                else if (TransactionLifeBitLastStateChange.AddSeconds(20) < DateTime.Now)
                {
                    workerLifebitStatus.ReportProgress(0, OpcState.TransactionLifeBitError);
                }
            }

            TransactionKioskLifeBit++;
            if (TransactionKioskLifeBit > 1000) TransactionKioskLifeBit = 0;
            OpcManager.SendTransactionKioskLiveBit(TransactionKioskLifeBit);
        }

        private void WorkerLifebitProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.UserState is OpcState)
            {
                switch ((OpcState)e.UserState)
                {
                    case OpcState.OpcLiveError: EnterOpcErrorState(); break;
                    case OpcState.OpcLiveOk: EndOpcErrorState(); break;
                    case OpcState.PlcLifeBitError: EnterPlcLifeBitErrorState(); break;
                    case OpcState.PlcLifeBitOk: EndPlcLifeBitErrorState(); break;

                    case OpcState.TransactionLifeBitError: EnterTransactionLifeBitErrorState(); break;
                    case OpcState.TransactionLifeBitOk: EndTransactionLifeBitErrorState(); break;
                }
            }
        }
        #endregion

        #region Opc Worker
        private void RunOpcWorker()
        {
            workerOpcStatus.DoWork -= WorkerOpcWork;
            workerOpcStatus.DoWork += WorkerOpcWork;
            workerOpcStatus.ProgressChanged -= WorkerOpcProgressChanged;
            workerOpcStatus.ProgressChanged += WorkerOpcProgressChanged;
            workerOpcStatus.WorkerReportsProgress = true;

            if (!workerOpcStatus.IsBusy)
                workerOpcStatus.RunWorkerAsync();
        }

        private void WorkerOpcWork(object sender, DoWorkEventArgs e)
        {
            var counter = 0;
            while (true)
            {
                try
                {
                    if (KioskConf == null)
                    {
                        Thread.Sleep(250);
                        continue;
                    }

                    counter++;

                    var opcLive = CheckOpcIsLive();

                    if (counter >= 2)
                    {
                        if (opcLive)
                        {
                            var opcState = OpcManager.CheckDeviceState(KioskConf);
                            OpcManager.CheckCodeState();

                            UpdateOpcParameters();

                            if (LastDumpPathId != opcState.DumpPath)
                            {
                                LastDumpPathId = opcState.DumpPath;
                                workerOpcStatus.ReportProgress(0, FlowState.DumpPathChanged);
                            }

                            if (!opcState.Power230Ok && !Power230LostError) workerOpcStatus.ReportProgress(0, OpcState.Power230Error);
                            if (opcState.Power230Ok && Power230LostError) workerOpcStatus.ReportProgress(0, OpcState.Power230Ok);

                            if (!opcState.Power24Ok && !Power24LostError) workerOpcStatus.ReportProgress(0, OpcState.Power24Error);
                            if (opcState.Power24Ok && Power24LostError) workerOpcStatus.ReportProgress(0, OpcState.Power24Ok);

                            if (!opcState.FlowOk && !FlowMeterError) workerOpcStatus.ReportProgress(0, OpcState.FlowMeterError);
                            if (opcState.FlowOk && FlowMeterError) workerOpcStatus.ReportProgress(0, OpcState.FlowMeterOk);
                            if (!opcState.DumpPathOk && !DumpPathError) workerOpcStatus.ReportProgress(0, OpcState.DumpPathError);
                            if (opcState.DumpPathOk && DumpPathError) workerOpcStatus.ReportProgress(0, OpcState.DumpPathOk);

                            if (!opcState.ScreenOk && !ScreenError) workerOpcStatus.ReportProgress(0, OpcState.ScreenError);
                            if (opcState.ScreenOk && ScreenError) workerOpcStatus.ReportProgress(0, OpcState.ScreenOk);
                            if (opcState.ScreenFull && !ScreenFullError && !opcState.Flow.DumpStartTime.HasValue) workerOpcStatus.ReportProgress(0, OpcState.ScreenFullError);
                            if (!opcState.ScreenFull && ScreenFullError && !opcState.Flow.DumpStartTime.HasValue) workerOpcStatus.ReportProgress(0, OpcState.ScreenFullOk);

                            if (!opcState.ProbeOk && !KioskConf.KioskBlockages[KioskBlockageType.TakingSamples] && !KioskConf.KioskBlockages[KioskBlockageType.ProbeError] && !ProbeError) workerOpcStatus.ReportProgress(0, OpcState.ProbeError);
                            if ((opcState.ProbeOk || KioskConf.KioskBlockages[KioskBlockageType.TakingSamples] || KioskConf.KioskBlockages[KioskBlockageType.ProbeError]) && ProbeError) workerOpcStatus.ReportProgress(0, OpcState.ProbeOk);
                            if (!opcState.ProbeReadyOk && !KioskConf.KioskBlockages[KioskBlockageType.TakingSamples] && !KioskConf.KioskBlockages[KioskBlockageType.ProbeError] && !ProbeReadyError && Transaction == null) workerOpcStatus.ReportProgress(0, OpcState.ProbeReadyError);
                            if ((opcState.ProbeReadyOk || KioskConf.KioskBlockages[KioskBlockageType.TakingSamples] || KioskConf.KioskBlockages[KioskBlockageType.ProbeError]) && ProbeReadyError) workerOpcStatus.ReportProgress(0, OpcState.ProbeReadyOk);
                            if (opcState.Flow.SamplerBottleNo >= _lastBottleNo && !KioskConf.KioskBlockages[KioskBlockageType.TakingSamples] && !KioskConf.KioskBlockages[KioskBlockageType.ProbeBottles] && !ProbeFullError && Transaction == null) workerOpcStatus.ReportProgress(0, OpcState.ProbeFullError);
                            if ((opcState.Flow.SamplerBottleNo < _lastBottleNo || KioskConf.KioskBlockages[KioskBlockageType.TakingSamples] || KioskConf.KioskBlockages[KioskBlockageType.ProbeBottles]) && ProbeFullError) workerOpcStatus.ReportProgress(0, OpcState.ProbeFullOk);

                            if (!opcState.PhMeterOk && !KioskConf.KioskBlockages[KioskBlockageType.MeasuringPh] && !PhMeterError) workerOpcStatus.ReportProgress(0, OpcState.PhMeterError);
                            if ((opcState.PhMeterOk || KioskConf.KioskBlockages[KioskBlockageType.MeasuringPh]) && PhMeterError) workerOpcStatus.ReportProgress(0, OpcState.PhMeterOk);
                            if (!opcState.TempMeterOk && !KioskConf.KioskBlockages[KioskBlockageType.MeasuringTemperature] && !TempMeterError) workerOpcStatus.ReportProgress(0, OpcState.TempMeterError);
                            if ((opcState.TempMeterOk || KioskConf.KioskBlockages[KioskBlockageType.MeasuringTemperature]) && TempMeterError) workerOpcStatus.ReportProgress(0, OpcState.TempMeterOk);
                            if (!opcState.ChztOk && !KioskConf.KioskBlockages[KioskBlockageType.MeasuringChzt] && !ChztMeterError) workerOpcStatus.ReportProgress(0, OpcState.ChztMeterError);
                            if ((opcState.ChztOk || KioskConf.KioskBlockages[KioskBlockageType.MeasuringChzt]) && ChztMeterError) workerOpcStatus.ReportProgress(0, OpcState.ChztMeterOk);
                            if (!opcState.PressureMeterOk && !KioskConf.KioskBlockages[KioskBlockageType.MeasuringPressure] && !PressureMeterError) workerOpcStatus.ReportProgress(0, OpcState.PressureMeterError);
                            if ((opcState.PressureMeterOk || KioskConf.KioskBlockages[KioskBlockageType.MeasuringPressure]) && PressureMeterError) workerOpcStatus.ReportProgress(0, OpcState.PressureMeterOk);

                            if (!opcState.ValveMeterOk && !KioskConf.KioskBlockages[KioskBlockageType.WaterForMeasuringDevices] && !ValveMeterError) workerOpcStatus.ReportProgress(0, OpcState.ValveMeterError);
                            if ((opcState.ValveMeterOk || KioskConf.KioskBlockages[KioskBlockageType.WaterForMeasuringDevices]) && ValveMeterError) workerOpcStatus.ReportProgress(0, OpcState.ValveMeterOk);
                            if (!opcState.ZasuwaOk && !KioskConf.KioskBlockages[KioskBlockageType.Zasuwa] && !ZasuwaError) workerOpcStatus.ReportProgress(0, OpcState.ZasuwaError);
                            if ((opcState.ZasuwaOk || KioskConf.KioskBlockages[KioskBlockageType.Zasuwa]) && ZasuwaError) workerOpcStatus.ReportProgress(0, OpcState.ZasuwaOk);

                            if (opcState.ProbeDoorOpen && !ProbeDoorOpenError) workerOpcStatus.ReportProgress(0, OpcState.ProbeDoorOpenError);
                            if (!opcState.ProbeDoorOpen && ProbeDoorOpenError) workerOpcStatus.ReportProgress(0, OpcState.ProbeDoorOpenOk);
                            if (!opcState.AirOk && !AirError) workerOpcStatus.ReportProgress(0, OpcState.AirError);
                            if (opcState.AirOk && AirError) workerOpcStatus.ReportProgress(0, OpcState.AirOk);
                            if (!opcState.Distribution230VACOk && !Distribution230VACError) workerOpcStatus.ReportProgress(0, OpcState.Distribution230VACError);
                            if (opcState.Distribution230VACOk && Distribution230VACError) workerOpcStatus.ReportProgress(0, OpcState.Distribution230VACOk);
                            if (!opcState.Rack230VACOk && !Rack230VACError) workerOpcStatus.ReportProgress(0, OpcState.Rack230VACError);
                            if (opcState.Rack230VACOk && Rack230VACError) workerOpcStatus.ReportProgress(0, OpcState.Rack230VACOk);
                            if (!opcState.Rack24VDCOk && !Rack24VDCError) workerOpcStatus.ReportProgress(0, OpcState.Rack24VDCError);
                            if (opcState.Rack24VDCOk && Rack24VDCError) workerOpcStatus.ReportProgress(0, OpcState.Rack24VDCOk);
                            if (!opcState.Rack24VDCUPSOk && !Rack24VDCUpsError) workerOpcStatus.ReportProgress(0, OpcState.Rack24VDCUPSError);
                            if (opcState.Rack24VDCUPSOk && Rack24VDCUpsError) workerOpcStatus.ReportProgress(0, OpcState.Rack24VDCUPSOk);
                            if (!opcState.Rack12VDCOk && !Rack12VDCError) workerOpcStatus.ReportProgress(0, OpcState.Rack12VDCError);
                            if (opcState.Rack12VDCOk && Rack12VDCError) workerOpcStatus.ReportProgress(0, OpcState.Rack12VDCOk);

                            if (!opcState.PumpRoomOk && !PumpRoomError) workerOpcStatus.ReportProgress(0, OpcState.PumpRoomError);
                            if (opcState.PumpRoomOk && PumpRoomError) workerOpcStatus.ReportProgress(0, OpcState.PumpRoomOk);
                            if (!opcState.PumpRoomWorking && !PumpRoomWorkError) workerOpcStatus.ReportProgress(0, OpcState.PumpRoomWorkError);
                            if (opcState.PumpRoomWorking && PumpRoomWorkError) workerOpcStatus.ReportProgress(0, OpcState.PumpRoomWorkOk);
                            if (opcState.ScreenRoomLeak && !ScreenRoomLeakError) workerOpcStatus.ReportProgress(0, OpcState.ScreenRoomLeakError);
                            if (!opcState.ScreenRoomLeak && ScreenRoomLeakError) workerOpcStatus.ReportProgress(0, OpcState.ScreenRoomLeakOk);
                            if (!opcState.AugerOk && !AugerError) workerOpcStatus.ReportProgress(0, OpcState.AugerError);
                            if (opcState.AugerOk && AugerError) workerOpcStatus.ReportProgress(0, OpcState.AugerOk);
                            if (!opcState.PreScreenOk && !PreScreenError) workerOpcStatus.ReportProgress(0, OpcState.PreScreenError);
                            if (opcState.PreScreenOk && PreScreenError) workerOpcStatus.ReportProgress(0, OpcState.PreScreenOk);
                            if (opcState.KioskDoorOpen && !KioskDoorError) workerOpcStatus.ReportProgress(0, OpcState.KioskDoorError);
                            if (!opcState.KioskDoorOpen && KioskDoorError) workerOpcStatus.ReportProgress(0, OpcState.KioskDoorOk);

                            // Starting screen cleaning
                            if (opcState.Flow.DumpStartTime.HasValue && opcState.Flow.SeqHeld
                                && opcState.ScreenFull && !ScreenCleaningTimer.HasValue)
                            {
                                workerOpcStatus.ReportProgress(0, FlowState.SequenceScreenCleaningStart);
                            }
                            // Ending screen cleaning
                            if (opcState.Flow.DumpStartTime.HasValue && ScreenCleaningTimer.HasValue && !opcState.Flow.SeqHeld)
                            {
                                workerOpcStatus.ReportProgress(0, FlowState.SequenceScreenCleaningEnd);
                            }
                            // Screen cleaning timeout
                            if (opcState.Flow.DumpStartTime.HasValue && opcState.Flow.SeqHeld && opcState.ScreenFull
                                && ScreenCleaningTimer.HasValue && ScreenCleaningTimer.Value.AddSeconds(_screenCleaningTimeout) < DateTime.Now)
                            {
                                workerOpcStatus.ReportProgress(0, FlowState.SequenceScreenCleaningTimeout);
                            }

                            //// Starting path changing cleaning
                            //if (opcState.Flow.DumpStartTime.HasValue && opcState.Flow.SeqHeld
                            //    && !opcState.ZspOk && !PathChangingTimer.HasValue)
                            //{
                            //    workerOpcStatus.ReportProgress(0, FlowState.SequencePathChangingStart);
                            //}
                            //// Ending path changing cleaning
                            //if (opcState.Flow.DumpStartTime.HasValue && PathChangingTimer.HasValue && !opcState.Flow.SeqHeld)
                            //{
                            //    workerOpcStatus.ReportProgress(0, FlowState.SequencePathChangingEnd);
                            //}
                            //// Screen path changing timeout
                            //if (opcState.Flow.DumpStartTime.HasValue && opcState.Flow.SeqHeld && !opcState.ZspOk
                            //    && PathChangingTimer.HasValue && PathChangingTimer.Value.AddSeconds(_pathChangingTimeout) < DateTime.Now)
                            //{
                            //    workerOpcStatus.ReportProgress(0, FlowState.SequencePathChangingTimeout);
                            //}

                            if (opcState.Flow.DumpStartTime.HasValue && opcState.Flow.SeqErrScreenFull)
                            {
                                workerOpcStatus.ReportProgress(0, FlowState.SequenceScreenCleaningTimeout);
                            }

                            if (opcState.Flow.DumpStartTime.HasValue && opcState.Flow.SeqErrAlarm)
                            {
                                workerOpcStatus.ReportProgress(0, FlowState.SequenceErrorAlarm);
                            }

                            if (opcState.Flow.DumpStartTime.HasValue && opcState.Flow.SeqErrDev)
                            {
                                workerOpcStatus.ReportProgress(0, FlowState.SequenceErrorDevice);
                            }

                            if (opcState.Flow.DumpStartTime.HasValue && !opcState.Flow.SeqAckTime.HasValue && opcState.Flow.SeqAck)
                            {
                                if (!opcState.Flow.SeqErrAlarm && !opcState.Flow.SeqErrDev && !opcState.Flow.SeqErrNoFlow && !opcState.Flow.SeqErrScreenFull && !ErrorState)
                                {
                                    workerOpcStatus.ReportProgress(0, FlowState.SequenceThankYouPage);
                                }
                                workerOpcStatus.ReportProgress(0, FlowState.SequenceFinished);
                            }

                            if (opcState.Flow.SeqAckTime.HasValue && !opcState.Flow.SeqAck
                                && DevicesState.Flow.SeqAckTime.Value.AddSeconds(_SequenceConfirmationTimeout) < DateTime.Now) //Timeout
                            {
                                workerOpcStatus.ReportProgress(0, FlowState.SequenceFinishedTimeout);
                            }

                            if (opcState.Flow.SeqAckTime.HasValue && !opcState.Flow.SeqAck)
                            {
                                workerOpcStatus.ReportProgress(0, FlowState.SequenceResetConfirmation);
                            }

                            if (opcState.Flow.DumpStartTime.HasValue && !opcState.Flow.SeqAckTime.HasValue && opcState.Flow.SeqNoFlowAck)
                            {
                                workerOpcStatus.ReportProgress(0, FlowState.SequenceNoFlow);
                            }

                            if (opcState.Flow.SeqAckTime.HasValue && !opcState.Flow.SeqNoFlowAck
                                && DevicesState.Flow.SeqAckTime.Value.AddSeconds(_SequenceConfirmationTimeout) < DateTime.Now) //Timeout
                            {
                                workerOpcStatus.ReportProgress(0, FlowState.SequenceNoFlowTimeout);
                            }

                            if (opcState.Flow.SeqAckTime.HasValue && !opcState.Flow.SeqAck)
                            {
                                workerOpcStatus.ReportProgress(0, FlowState.SequenceNoFlowResetConfirmation);
                            }

                            if (opcState != null && opcState != DevicesState)
                            {
                                workerOpcStatus.ReportProgress(0, opcState);
                            }

                            #region Check devices open/close state
                            if (OpcManager.DevicesSteering.GateOpen
                                && (!GateTimer.HasValue || GateTimer.Value.AddSeconds(_OpenGeteTimeInSeconds) < DateTime.Now))
                            {
                                workerOpcStatus.ReportProgress(0, DeviceSteeringCommands.CloseGate);
                            }
                            if (OpcManager.DevicesSteering.GateOutOpen
                                && (!GateOutTimer.HasValue || GateOutTimer.Value.AddSeconds(_OpenGeteTimeInSeconds) < DateTime.Now))
                            {
                                workerOpcStatus.ReportProgress(0, DeviceSteeringCommands.CloseGateOut);
                            }
                            if (OpcManager.DevicesSteering.DistributionOpen
                                && (!DistributionTimer.HasValue || DistributionTimer.Value.AddSeconds(_OpenDistributionTimeInSeconds) < DateTime.Now))
                            {
                                workerOpcStatus.ReportProgress(0, DeviceSteeringCommands.CloseDistribution);
                            }
                            if (OpcManager.DevicesSteering.ScreenRoomOpen
                                && (!ScreenRoomTimer.HasValue || ScreenRoomTimer.Value.AddSeconds(_OpenScreenRoomTimeInSeconds) < DateTime.Now))
                            {
                                workerOpcStatus.ReportProgress(0, DeviceSteeringCommands.CloseScreenRoom);
                            }
                            if (OpcManager.DevicesSteering.ToiletOpen
                                && (!ToiletTimer.HasValue || ToiletTimer.Value.AddSeconds(_OpenToiletTimeInSeconds) < DateTime.Now))
                            {
                                workerOpcStatus.ReportProgress(0, DeviceSteeringCommands.CloseToilet);
                            }
                            #endregion

                            #region Check registration number from Camera
                            if (!KioskConf.KioskBlockages[KioskBlockageType.CameraOpcRegistration]
                                && DevicesState != null && !string.IsNullOrEmpty(DevicesState.RegistrationNumberFromCamera)
                                && DevicesState.RegistrationNumberFromCamera != LastRegistrationFromCamera)
                            {
                                workerOpcStatus.ReportProgress(0, new CameraReading { RegistrationNumber = DevicesState.RegistrationNumberFromCamera, CameraNumber = 1 });
                            }
                            if (!KioskConf.KioskBlockages[KioskBlockageType.CameraOpcRegistration]
                                && DevicesState != null && !string.IsNullOrEmpty(DevicesState.RegistrationNumberFromCamera)
                                && DevicesState.RegistrationNumberFromCamera2 != LastRegistrationFromCamera2)
                            {
                                workerOpcStatus.ReportProgress(0, new CameraReading { RegistrationNumber = DevicesState.RegistrationNumberFromCamera2, CameraNumber = 2 });
                            }
                            if (UnknownCameraSet && UnknownCameraSetTimer.AddSeconds(_UnknownRegistrationNumberInSeconds) < DateTime.Now)
                            {
                                workerOpcStatus.ReportProgress(0, OpcState.UnsetCameraUnknownReading);
                            }
                            #endregion

                            if (OpcErrorState)
                            {
                                workerOpcStatus.ReportProgress(0, OpcState.OpcLiveOk);
                            }
                        }
                        else
                        {
                            workerOpcStatus.ReportProgress(0, OpcState.OpcLiveError);
                        }

                        if ((ShowingPage == DisplayPage.ErrorPage || ShowingPage == DisplayPage.StartupPage) && !ErrorState)
                        {
                            workerOpcStatus.ReportProgress(0, OpcState.AllOk);
                        }

                        if (ShowingPage == DisplayPage.StartupPage && ErrorState && StartupTime.AddSeconds(_StartupScreenTimeoutInSeconds) < DateTime.Now)
                        {
                            workerOpcStatus.ReportProgress(0, OpcState.OpcLiveError);
                        }

                        // zabezpieczenie na wypadek błędów występujących zbyt krótko żeby przechwycić je z opc
                        if (ShowingPage == DisplayPage.DumpPage && Transaction == null)
                        {
                            // jeżeli wykryjemy brak transakcji, a stron zrzutu jest dalej aktywna zgłaszamy bład
                            // rodzaj błędu jest nieistotny, ponieważ nie zostanie wykryty na stronie błedu - wyświetlony zostanie błąd generyczny
                            workerOpcStatus.ReportProgress(0, OpcState.Power230Error);
                        }

                        counter = 0;
                    }
                    Thread.Sleep(250);
                }
                catch (Exception ex)
                {
                    Application.Current.Dispatcher.Invoke(new Action(() => { LogError(ex); }));
                }
            }
        }

        private void WorkerOpcProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.UserState is OpcState)
            {
                switch ((OpcState)e.UserState)
                {
                    case OpcState.OpcLiveError: EnterOpcErrorState(); break;
                    case OpcState.OpcLiveOk: EndOpcErrorState(); break;
                    case OpcState.PlcLifeBitError: EnterPlcLifeBitErrorState(); break;
                    case OpcState.PlcLifeBitOk: EndPlcLifeBitErrorState(); break;

                    case OpcState.Power230Error: EnterPower230LostErrorState(); break;
                    case OpcState.Power230Ok: EndPower230LostErrorState(); break;
                    case OpcState.Power24Error: EnterPower24LostErrorState(); break;
                    case OpcState.Power24Ok: EndPower24LostErrorState(); break;
                    case OpcState.FlowMeterError: EnterFlowMeterErrorState(); break;
                    case OpcState.FlowMeterOk: EndFlowMeterErrorState(); break;
                    case OpcState.DumpPathError: EnterDumpPathErrorState(); break;
                    case OpcState.DumpPathOk: EndDumpPathErrorState(); break;
                    case OpcState.ScreenError: EnterScreenErrorState(); break;
                    case OpcState.ScreenOk: EndScreenErrorState(); break;
                    case OpcState.ScreenFullError: EnterScreenFullErrorState(); break;
                    case OpcState.ScreenFullOk: EndScreenFullErrorState(); break;
                    case OpcState.ProbeError: EnterProbeErrorState(); break;
                    case OpcState.ProbeOk: EndProbeErrorState(); break;
                    case OpcState.ProbeReadyError: EnterProbeReadyErrorState(); break;
                    case OpcState.ProbeReadyOk: EndProbeReadyErrorState(); break;
                    case OpcState.ProbeFullError: EnterProbeFullErrorState(); break;
                    case OpcState.ProbeFullOk: EndProbeFullErrorState(); break;
                    case OpcState.PhMeterError: EnterPhMeterErrorState(); break;
                    case OpcState.PhMeterOk: EndPhMeterErrorState(); break;
                    case OpcState.TempMeterError: EnterTempMeterErrorState(); break;
                    case OpcState.TempMeterOk: EndTempMeterErrorState(); break;
                    case OpcState.PressureMeterError: EnterPressureMeterErrorState(); break;
                    case OpcState.PressureMeterOk: EndPressureMeterErrorState(); break;
                    case OpcState.ChztMeterError: EnterChztMeterErrorState(); break;
                    case OpcState.ChztMeterOk: EndChztMeterErrorState(); break;
                    case OpcState.KioskDoorError: EnterKioskDoorErrorState(); break;
                    case OpcState.KioskDoorOk: EndKioskDoorErrorState(); break;
                    case OpcState.ValveMeterError: EnterValveMeterErrorState(); break;
                    case OpcState.ValveMeterOk: EndValveMeterErrorState(); break;
                    case OpcState.ZasuwaError: EnterZasuwaErrorState(); break;
                    case OpcState.ZasuwaOk: EndZasuwaErrorState(); break;

                    case OpcState.ProbeDoorOpenError: EnterErrorState(ErrorType.ProbeDoorOpen); break;
                    case OpcState.ProbeDoorOpenOk: EndErrorState(ErrorType.ProbeDoorOpen); break;
                    case OpcState.AirError: EnterErrorState(ErrorType.Air); break;
                    case OpcState.AirOk: EndErrorState(ErrorType.Air); break;
                    case OpcState.Distribution230VACError: EnterErrorState(ErrorType.Distribution230VAC); break;
                    case OpcState.Distribution230VACOk: EndErrorState(ErrorType.Distribution230VAC); break;
                    case OpcState.Rack230VACError: EnterErrorState(ErrorType.Rack230VAC); break;
                    case OpcState.Rack230VACOk: EndErrorState(ErrorType.Rack230VAC); break;
                    case OpcState.Rack24VDCError: EnterErrorState(ErrorType.Rack24VDC); break;
                    case OpcState.Rack24VDCOk: EndErrorState(ErrorType.Rack24VDC); break;
                    case OpcState.Rack24VDCUPSError: EnterErrorState(ErrorType.Rack24VDCUPS); break;
                    case OpcState.Rack24VDCUPSOk: EndErrorState(ErrorType.Rack24VDCUPS); break;
                    case OpcState.Rack12VDCError: EnterErrorState(ErrorType.Rack12VDC); break;
                    case OpcState.Rack12VDCOk: EndErrorState(ErrorType.Rack12VDC); break;

                    case OpcState.PumpRoomError: EnterErrorState(ErrorType.PumpRoom); break;
                    case OpcState.PumpRoomOk: EndErrorState(ErrorType.PumpRoom); break;
                    case OpcState.PumpRoomWorkError: EnterErrorState(ErrorType.PumpRoomWork); break;
                    case OpcState.PumpRoomWorkOk: EndErrorState(ErrorType.PumpRoomWork); break;
                    case OpcState.ScreenRoomLeakError: EnterErrorState(ErrorType.ScreenRoomLeak); break;
                    case OpcState.ScreenRoomLeakOk: EndErrorState(ErrorType.ScreenRoomLeak); break;
                    case OpcState.AugerError: EnterErrorState(ErrorType.Auger); break;
                    case OpcState.AugerOk: EndErrorState(ErrorType.Auger); break;
                    case OpcState.PreScreenError: EnterErrorState(ErrorType.PreScreen); break;
                    case OpcState.PreScreenOk: EndErrorState(ErrorType.PreScreen); break;

                    case OpcState.UnsetCameraUnknownReading: UnsetCameraUnknownReading(); break;

                    case OpcState.AllOk: EndErrorState(); break;
                }
            }
            if (e.UserState is FlowState)
            {
                switch ((FlowState)e.UserState)
                {
                    case FlowState.SequenceFinished: SequenceFinished(); break;
                    case FlowState.SequenceFinishedTimeout: SequenceFinishedTimeout(); break;
                    case FlowState.SequenceResetConfirmation: SequenceFinishedResetConfirmation(); break;
                    case FlowState.SequenceNoFlow: SequenceNoFlowFinished(); break;
                    case FlowState.SequenceNoFlowTimeout: SequenceNoFlowTimeout(); break;
                    case FlowState.SequenceNoFlowResetConfirmation: SequenceNoFlowFinishedResetConfirmation(); break;
                    case FlowState.SequenceThankYouPage: SequenceThankYouPage(); break;
                    case FlowState.SequenceErrorAlarm: SequenceErrorAlarm(); break;
                    case FlowState.SequenceErrorDevice: SequenceErrorDevice(); break;

                    case FlowState.SequenceScreenCleaningStart: SequenceScreenCleaningStart(); break;
                    case FlowState.SequenceScreenCleaningEnd: SequenceScreenCleaningEnd(); break;
                    case FlowState.SequenceScreenCleaningTimeout: SequenceScreenCleaningTimeout(); break;

                    case FlowState.SequencePathChangingStart: SequenceScreenCleaningStart(); break;
                    case FlowState.SequencePathChangingEnd: SequenceScreenCleaningEnd(); break;
                    case FlowState.SequencePathChangingTimeout: SequenceScreenCleaningTimeout(); break;

                    case FlowState.GateOpened: SequenceDoorOpened(); break;

                    case FlowState.DumpPathChanged: SetDumpPathMessage(); break;
                }
            }
            if (e.UserState is DeviceSteeringCommands)
            {
                switch ((DeviceSteeringCommands)e.UserState)
                {
                    case DeviceSteeringCommands.CloseGate: RemoveOpenGateSignal(); break;
                    case DeviceSteeringCommands.CloseGateOut: RemoveOpenGateOutSignal(); break;
                    case DeviceSteeringCommands.CloseDistribution: RemoveOpenDistributionSignal(); break;
                    case DeviceSteeringCommands.CloseScreenRoom: RemoveOpenScreenRoomSignal(); break;
                    case DeviceSteeringCommands.CloseToilet: RemoveOpenToiletSignal(); break;
                }
            }
            if (e.UserState is DevicesState)
            {
                var bindPage = false;
                if (DevicesState == null)
                {
                    bindPage = true;
                }
                DevicesState = (DevicesState)e.UserState;
                if (bindPage && !ErrorState && !DevicesState.DeviceError)
                {
                    BindPage();
                }

                SetDumpPathMessage();
            }
            if (e.UserState is CameraReading reading)
            {
                if (reading.RegistrationNumber != "")
                {
                    var regNumber = reading.RegistrationNumber.Replace("\r", "").Replace("\n", "");
                    LogDebug("KAMERA - odczyt rejestracji " + regNumber);
                    var card = DbRead.GetRfidCardByLicensePlate(regNumber);
                    if (card != null)
                    {
                        LogDebug("KAMERA - odczyt rejestracji " + card.CardId);
                    }
                    else
                    {
                        LogDebug("KAMERA - odczyt rejestracji - brak rejestracji w bazie");
                        OpcManager.SetLicensePlateUnknownVariable();
                        UnknownCameraSet = true;
                        UnknownCameraSetTimer = DateTime.Now;
                    }
                }

                if (reading.CameraNumber == 1)
                    LastRegistrationFromCamera = reading.RegistrationNumber;
                if (reading.CameraNumber == 2)
                    LastRegistrationFromCamera2 = reading.RegistrationNumber;
            }
        }
        private void UnsetCameraUnknownReading()
        {
            LogDebug("KAMERA - zdjęcie informacji o braku rejestracji");
            OpcManager.UnSetLicensePlateUnknownVariable();
            UnknownCameraSet = false;
        }
        private void SequenceErrorAlarm()
        {
            LogDebug("Sekwencja - przekroczenie parametrów ścieku");

            Transaction.FinishedCorrectly = false;
            Transaction.FinishReason = "Przekroczenie parametrów";

            var pressureError = DevicesState.PressureDumpError;
            if (pressureError)
            {
                Transaction.FinishReason = "Przekroczenie parametru - Ciśnienie";
            }
            else
            {
                var alarmReasons = OpcManager.MeasCodeState.GetErrorMessages();

                if (alarmReasons.Count > 0)
                {
                    Transaction.FinishReason = alarmReasons.FirstOrDefault();
                }

                foreach (var msg in alarmReasons)
                {
                    LogDebug("Błąd urządzenia:" + msg);
                }
            }

            BindPage(DisplayPage.SequenceErrorAlertPage);
        }
        private void SequenceErrorDevice()
        {
            if (Transaction != null && Transaction.DumpStarted && !Transaction.Saved)
            {
                Transaction.FinishedCorrectly = false;
                var finishReasons = OpcManager.StopCodeState.GetErrorMessages();
                Transaction.FinishReason = finishReasons.FirstOrDefault();
                LogDebug("Sekwencja - błąd urządzenia");

                foreach (var msg in finishReasons)
                {
                    LogDebug("Błąd urządzenia:" + msg);
                }
            }
        }
        private void SequenceThankYouPage()
        {
            if (Transaction != null)
            {
                Transaction.FinishedCorrectly = true;
                Transaction.FinishReason = "Zakończono poprawnie";
                BindPage(DisplayPage.ThankYouPage);
            }
            LogDebug("Wyświetlenie strony z podziękowaniem");
        }
        private void SequenceFinished()
        {
            if (Transaction == null)
            {
                return;
            }
            Transaction.DumpEnd = DateTime.Now;
            DevicesState.Flow.CheckStatus();
            Transaction.ActualAmount = DevicesState.Flow.FlowFlowCnt;
            Transaction.ScheduledSample = DevicesState.Flow.CheckScheduledSampleTaken();
            Transaction.AlarmSample = DevicesState.Flow.CheckAlarmSampleTaken();
            Transaction.Parameters = DevicesState.Flow.CheckTransactionParameters();
            Transaction.PressurePumpOn = DevicesState.PressurePumpOn;
            CleanParametersAccordingToBlockages(Transaction);
            Transaction.TransactionEnd = DateTime.Now;
            if (!Transaction.Saved)
            {
                LogDebug("Sekwencja - transakcja, zapis do bazy");
                //Transaction.Saved = DbWrite.SaveTransaction(Transaction);
                Transaction.Saved = TransactionSavingService.Enqueue(Transaction);
                LogDebug("Sekwencja - transakcja, zapisano");
                PrintRecipt();
            }
            //CurrentPage = DisplayPage.RfidCardPage;

            LogDebug("Sekwencja - SeqAck, ustawianie na true");
            OpcManager.SetKioskSeqAckToTrue();
            LogDebug("Sekwencja - SeqAck, ustawione na true");
        }
        private void SequenceFinishedTimeout()
        {
            DbWrite.SaveTransaction(Transaction);
            PrintRecipt();
            BindPage(DisplayPage.SquenceFinishError);
            Transaction = null;
            LogDebug("Sekwencja - przekroczenie czasu na zakończenie transakcji");
        }
        private void SequenceFinishedResetConfirmation()
        {
            LogDebug("Sekwencja - SeqAck, ustawianie na false");
            OpcManager.SetKioskSeqAckToFalse();
            LogDebug("Sekwencja - SeqAck, ustawione na false");
            SequenceRunning = false;
            Transaction = null;
        }
        private void SequenceNoFlowFinished()
        {
            BindPage(DisplayPage.NoFlowPage);
            LogDebug("Sekwencja - SeqNoFlowAck, ustawianie na true");
            OpcManager.SetKioskSeqNoFlowAckToTrue();
            LogDebug("Sekwencja - SeqNoFlowAck, ustawione na true");
        }
        private void SequenceNoFlowTimeout()
        {
            BindPage(DisplayPage.SquenceFinishError);
            Transaction = null;
            LogDebug("Sekwencja - przekroczenie czasu na zakończenie transakcji z braku przepływu");
        }
        private void SequenceNoFlowFinishedResetConfirmation()
        {
            LogDebug("Sekwencja - SeqNoFlowAck, ustawianie na true");
            OpcManager.SetKioskSeqNoFlowAckToFalse();
            LogDebug("Sekwencja - SeqNoFlowAck, ustawione na true");
            SequenceRunning = false;
            Transaction = null;
        }

        private void SequenceScreenCleaningStart()
        {
            ScreenCleaningTimer = DateTime.Now;
            BindPage(DisplayPage.ScreenCelaningPage);
            LogDebug("Sekwencja - Rozpoczęcie czyszczenia sita");
        }
        private void SequenceScreenCleaningEnd()
        {
            ScreenCleaningTimer = null;
            BindPage(DisplayPage.DumpPage);
            LogDebug("Sekwencja - Zakończenie czyszczenia sita");
        }
        private void SequenceScreenCleaningTimeout()
        {
            ScreenCleaningTimer = null;
            Transaction.FinishedCorrectly = false;
            Transaction.FinishReason = "Przekroczono czas oczekiwania na opróżnienie zbiornika sita";
            CurrentPage = DisplayPage.RfidCardPage;
            ShowErrorPage(DisplayPage.SequenceErrorScreenCleaningTimeout);
            LogDebug("Sekwencja - Przekroczenie czasu czyszczenia sita");
        }

        private void SequencePathChangingStart()
        {
            PathChangingTimer = DateTime.Now;
            BindPage(DisplayPage.PathChangingPage);
            LogDebug("Sekwencja - Rozpoczęcie zmiany drogi");
        }
        private void SequencePathChangingEnd()
        {
            PathChangingTimer = null;
            BindPage(DisplayPage.DumpPage);
            LogDebug("Sekwencja - Zakończenie zmiany drogi");
        }
        private void SequencePathChangingTimeout()
        {
            PathChangingTimer = null;
            Transaction.FinishedCorrectly = false;
            Transaction.FinishReason = "Przekroczono czas oczekiwania na zmianę drogi zrzutu";
            CurrentPage = DisplayPage.RfidCardPage;
            ShowErrorPage(DisplayPage.SequenceErrorScreenCleaningTimeout);
            LogDebug("Sekwencja - Przekroczenie czasu zmiany drogi");
        }

        private void SequenceDoorOpened()
        {
            CurrentPage = DisplayPage.RfidCardPage;
            ShowErrorPage(DisplayPage.SequenceErrorDoorOpened);
            LogDebug("Sekwencja - Drzwi otwarte, wysyłanie SequenceStop");
            OpcManager.SendSequenceStop();
            LogDebug("Sekwencja - Drzwi otwarte, wysłane SequenceStop");
        }

        private bool CheckOpcIsLive()
        {
            var opcLive = false;
            try
            {
                if (OpcManager == null || OpcManager.Client == null || (_opcOn && !OpcManager.Client.IsConnected))
                {
                    OpcManager = new OpcManager(_opcOn);
                }

                if (OpcManager != null && OpcManager.Client != null && (!_opcOn || OpcManager.Client.IsConnected))
                {
                    opcLive = true;
                }
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(new Action(() => { LogError(ex); }));
            }

            return opcLive;
        }
        private void UpdateOpcParameters()
        {
            if (KioskConf == null || OpcManager == null)
            {
                return;
            }

            if (OpcManager.OpcOn && OpcManager.OpcParameters == null)
            {
                OpcManager.BuildOpcParameters(KioskConf, PrinterOutOfPaperErrorState);
            }
            if (OpcManager.OpcOn && OpcManager.OpcParameters != null)
            {
                if (Transaction != null)
                {
                    OpcManager.OpcParameters.ProbeScheduled = Transaction.HarmonogramedSample.Count > 0;
                    OpcManager.OpcParameters.ProbePermit = !KioskConf.KioskBlockages[KioskBlockageType.TakingSamples] && Transaction.Card.Customer.TakingSamples;

                    OpcManager.OpcParameters.DeclaredSewageAmount = (short)Transaction.CustomerAddresses.Sum(item => item.DeclaredAmount);
                }
                else
                {
                    OpcManager.OpcParameters.ProbePermit = !KioskConf.KioskBlockages[KioskBlockageType.TakingSamples];
                }
                OpcManager.OpcParameters.WaterForMeasuringDevices = !KioskConf.KioskBlockages[KioskBlockageType.WaterForMeasuringDevices];

                OpcManager.OpcParameters.MonitoringPhOn = !KioskConf.KioskBlockages[KioskBlockageType.MeasuringPh];
                OpcManager.OpcParameters.MonitoringTempOn = !KioskConf.KioskBlockages[KioskBlockageType.MeasuringTemperature];
                OpcManager.OpcParameters.MonitoringChztOn = !KioskConf.KioskBlockages[KioskBlockageType.MeasuringChzt];
                OpcManager.OpcParameters.MonitoringPressureOn = !KioskConf.KioskBlockages[KioskBlockageType.MeasuringPressure];
                OpcManager.OpcParameters.PressureBlockageOn = !KioskConf.KioskBlockages[KioskBlockageType.PressureBlockageOn];

                OpcManager.OpcParameters.PaperEnd = PrinterOutOfPaperErrorState;
                OpcManager.OpcParameters.PrinterOn = !KioskConf.KioskBlockages[KioskBlockageType.Printer];

                OpcManager.OpcParameters.ProbeStart = KioskConf.ConfigurationSettings[KioskConfigurationType.ProbeStartAfterPercent];
                OpcManager.OpcParameters.ProbeMin = KioskConf.ConfigurationSettings[KioskConfigurationType.ProbeMinimalAfterStartL];
                OpcManager.OpcParameters.Probe1 = KioskConf.ConfigurationSettings[KioskConfigurationType.Probe1AfterStartPercent];
                OpcManager.OpcParameters.Probe2 = KioskConf.ConfigurationSettings[KioskConfigurationType.Probe2AfterStartPercent];
                OpcManager.OpcParameters.Probe3 = KioskConf.ConfigurationSettings[KioskConfigurationType.Probe3AfterStartPercent];
                OpcManager.OpcParameters.SeqHoldTime = KioskConf.ConfigurationSettings[KioskConfigurationType.WaitTimeForEmptyScreen];
                OpcManager.OpcParameters.SeqNoFlowTime = KioskConf.ConfigurationSettings[KioskConfigurationType.WaitTimeForInitialFlow];
                OpcManager.OpcParameters.SeqNormalFlowEnd = KioskConf.ConfigurationSettings[KioskConfigurationType.WaitTimeForEndOfFlow];
                OpcManager.OpcParameters.WaterForDriversTime = KioskConf.ConfigurationSettings[KioskConfigurationType.TimeForWaterForDrivers];
                OpcManager.OpcParameters.WaterForMeasuringDevicesTime = KioskConf.ConfigurationSettings[KioskConfigurationType.TimeForWaterForMeasuringDevices];

                OpcManager.OpcParameters.PressureFactor = KioskConf.ConfigurationSettings[KioskConfigurationType.PressureFactor];
                OpcManager.OpcParameters.PressureMaxTime = KioskConf.ConfigurationSettings[KioskConfigurationType.PressureMaxTime];

                OpcManager.OpcParameters.KioskOpenState = KioskConf.Open == KioskOpenState.Open;
                OpcManager.OpcParameters.ProbeMoveOn = !KioskConf.KioskBlockages[KioskBlockageType.ProbeMove];

                OpcManager.OpcParameters.UpdateOpcParameters();

                OpcManager.OpcParameters.ProbeBottlesErrorBlockage = KioskConf.KioskBlockages[KioskBlockageType.ProbeBottles];
                OpcManager.OpcParameters.ProbeErrorBlockage = KioskConf.KioskBlockages[KioskBlockageType.ProbeError];
            }
        }

        private void CleanParametersAccordingToBlockages(Transaction transaction)
        {
            if (KioskConf.KioskBlockages[KioskBlockageType.MeasuringPh])
            {
                transaction.Parameters.FlowphMax = 0;
                transaction.Parameters.FlowphMed = 0;
                transaction.Parameters.FlowphMin = 0;
            }
            if (KioskConf.KioskBlockages[KioskBlockageType.MeasuringConduction])
            {
                transaction.Parameters.FlowCondMax = 0;
                transaction.Parameters.FlowCondMed = 0;
                transaction.Parameters.FlowCondMin = 0;
                transaction.Parameters.FlowCondCurr = 0;
            }
            if (KioskConf.KioskBlockages[KioskBlockageType.MeasuringTemperature])
            {
                transaction.Parameters.FlowTempMax = 0;
                transaction.Parameters.FlowTempMed = 0;
                transaction.Parameters.FlowTempMin = 0;
                transaction.Parameters.FlowTempCurr = 0;
            }
            if (KioskConf.KioskBlockages[KioskBlockageType.MeasuringChzt])
            {
                transaction.Parameters.FlowChztMax = 0;
                transaction.Parameters.FlowChztMed = 0;
                transaction.Parameters.FlowChztMin = 0;
                transaction.Parameters.FlowChztCurr = 0;
            }
        }
        #endregion

        #region db monitoring and synch
        private void RunDbMonitoring()
        {
            workerDb.DoWork -= WorkerDbWork;
            workerDb.DoWork += WorkerDbWork;
            workerDb.ProgressChanged -= WorkerDbProgressChanged;
            workerDb.ProgressChanged += WorkerDbProgressChanged;
            workerDb.WorkerReportsProgress = true;

            if (!workerDb.IsBusy)
                workerDb.RunWorkerAsync();
        }
        private void WorkerDbWork(object sender, DoWorkEventArgs e)
        {
            var counter = 0;
            while (true)
            {
                var dbLive = CheckDbIsLive();
                if (dbLive)
                {
                    if (DbErrorState) workerDb.ReportProgress(0, true);

                    var newKioskConf = DbRead.GetKioskConfiguration();
                    if (KioskConf != newKioskConf)
                    {
                        workerDb.ReportProgress(0, newKioskConf);
                    }

                    if (printerPaperStatus == null)
                    {
                        var printedReciptsLenghtFromDb = DbRead.GetPrintedReciptsLenght();
                        printerPaperStatus = new PrinterPaperStatus() { ReciptLenghtInMilimeters = _reciptLenghtInMilimeters, ReciptPaperLenghtInMilimeters = _reciptPaperLenghtInMilimeters, ReciptPrintedLenghtInMilimeters = printedReciptsLenghtFromDb };
                    }

                    var kioskBlocked = DbRead.CheckIfKioskIsBlocked(_kioskId);
                    if (KioskBlocked != kioskBlocked)
                    {
                        workerDb.ReportProgress(0, kioskBlocked ? KioskOpenState.Blocked : KioskOpenState.Unblocked);
                    }

                    if (counter >= 6)
                    {
                        if (ServerConnection != null && DbServerRead != null && DbServerWrite != null)
                        {
                            try
                            {
                                var probeProgramOn = !OpcManager.ReadyCodeState.State[OpcCommunication.Enums.ReadyCode.ProbeNoProg].Item1;
                                var probeStatus = new ProbeStatus
                                {
                                    Error = !DevicesState.ProbeOk,
                                    ProgramOn = probeProgramOn,
                                    BottleNo = DevicesState.Flow.SamplerBottleNo,
                                };
                                ExportManager.ServerSync(KioskConf, printerPaperStatus, probeStatus);

                                ResetApp? reset = ExportManager.CheckResetApp();
                                if (reset.HasValue)
                                {
                                    workerDb.ReportProgress(0, reset);
                                }
                            }
                            catch (Exception ex)
                            {
                                Application.Current.Dispatcher.Invoke(new Action(() => { LogError(ex); }));
                                DbConnection.CloseInstance(DbServerConfiguration);
                                ServerConnection = null;
                                DbServerRead = null;
                                DbServerWrite = null;
                                ExportManager = null;
                            }
                        }

                        counter = 0;
                    }
                }
                else
                {
                    Connection = null;
                    DbRead = null;
                    DbWrite = null;
                    SyncConnection = null;
                    workerDb.ReportProgress(0, false);
                }

                Thread.Sleep(5000);
                counter++;
            }
        }

        private void WorkerDbProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.UserState is bool)
            {
                if ((bool)e.UserState)
                {
                    EndDbErrorState();
                }
                else
                {
                    EnterDbErrorState();
                }
            }
            if (e.UserState is KioskConfiguration)
            {
                KioskConf = (KioskConfiguration)e.UserState;
            }
            if (e.UserState is ResetApp)
            {
                if ((ResetApp)e.UserState == DbCommunication.Enums.ResetApp.ResetApp)
                {
                    ResetApp(false);
                }

                if ((ResetApp)e.UserState == DbCommunication.Enums.ResetApp.ResetSequence)
                {
                    ResetApp(true);
                }
            }
            if (e.UserState is KioskOpenState)
            {
                if ((KioskOpenState)e.UserState == KioskOpenState.Blocked)
                {
                    BlockKiosk();
                }
                else
                {
                    UnBlockKiosk();
                }
            }
        }
        private bool CheckDbIsLive()
        {
            var dbLive = false;
            var dbConnected = false;
            if (Connection == null || DbRead == null || DbWrite == null || SyncConnection == null)
            {
                dbConnected = OpenDbConnection();
            }
            else
            {
                dbConnected = true;
            }

            if (ServerConnection == null || DbServerRead == null || DbServerWrite == null)
            {
                OpenServerConnection();
            }

            if (dbConnected)
            {
                dbLive = DbRead.SqlServerLiveBeat();
            }

            return dbLive;
        }
        private bool OpenDbConnection()
        {
            try
            {
                Connection = DbConnection.GetInstance(DbLocalConfiguration, _kioskId);
                SyncConnection = DbConnection.GetInstance(DbSyncConfiguration, _kioskId);
                DbRead = new DbRead(Connection);
                DbWrite = new DbWrite(Connection);
                TransactionSavingService = new TransactionSavingService(DbSavingServiceConfiguration, _kioskId);
                var kioskConfThreadSafe = Dispatcher.Invoke((Func<KioskConfiguration>)(() => ((MainWindow)Application.Current.MainWindow).KioskConf));
                kioskConfThreadSafe = DbRead.GetKioskConfiguration();
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(new Action(() => { LogError(ex); }));
                return false;
            }
            return true;
        }
        private bool OpenServerConnection()
        {
            try
            {
                ServerConnection = DbConnection.GetInstance(new DbServerConfiguration(), _kioskId);
                DbServerRead = new DbServerRead(ServerConnection);
                DbServerWrite = new DbServerWrite(ServerConnection, SyncConnection);
                ExportManager = new ExportManager(Connection, DbRead, DbWrite, ServerConnection, DbServerRead, DbServerWrite, KioskConf);
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(new Action(() => { LogError(ex); }));
                return false;
            }
            return true;
        }
        #endregion

        #region state monitoring
        private void RunKioskMonitoring()
        {
            workerKioskMonitoring.DoWork -= WorkerKioskMonitoringWork;
            workerKioskMonitoring.DoWork += WorkerKioskMonitoringWork;
            workerKioskMonitoring.ProgressChanged -= WorkerKioskMonitoringProgressChanged;
            workerKioskMonitoring.ProgressChanged += WorkerKioskMonitoringProgressChanged;
            workerKioskMonitoring.WorkerReportsProgress = true;

            if (!workerKioskMonitoring.IsBusy)
                workerKioskMonitoring.RunWorkerAsync();
        }
        private void WorkerKioskMonitoringWork(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                var focusLost = !ApplicationIsActivated();
                if (FocusLost != focusLost)
                {
                    if (focusLost)
                    {
                        workerKioskMonitoring.ReportProgress(0, KioskPeripheralState.FocusLost);
                    }
                    else
                    {
                        workerKioskMonitoring.ReportProgress(0, KioskPeripheralState.FocusActive);
                    }
                }
                var keyboardError = CheckKeyboardError();
                if (KeyboardErrorState != keyboardError)
                {
                    if (keyboardError)
                    {
                        workerKioskMonitoring.ReportProgress(0, KioskPeripheralState.KeyboardError);
                    }
                    else
                    {
                        workerKioskMonitoring.ReportProgress(0, KioskPeripheralState.KeyboardOk);
                    }
                }
                var printerError = CheckPrinterError();
                if (PrinterErrorState != printerError)
                {
                    if (printerError)
                    {
                        workerKioskMonitoring.ReportProgress(0, KioskPeripheralState.PrinterError);
                    }
                    else
                    {
                        workerKioskMonitoring.ReportProgress(0, KioskPeripheralState.PrinterOk);
                    }
                }
                var printerOutOfPaperState = PrinterOutOfPaperErrorState;
                if (PrinterOutOfPaperState != printerOutOfPaperState)
                {
                    if (printerOutOfPaperState)
                    {
                        workerKioskMonitoring.ReportProgress(0, KioskPeripheralState.PrinterPaperError);
                    }
                    else
                    {
                        workerKioskMonitoring.ReportProgress(0, KioskPeripheralState.PrinterPaperOk);
                    }
                }

                var openText = Dispatcher.Invoke((Func<string>)(() => ((MainWindow)Application.Current.MainWindow).tbOpenState.Text));
                var kioskConfig = Dispatcher.Invoke((Func<KioskConfiguration>)(() => ((MainWindow)Application.Current.MainWindow).KioskConf));
                if (kioskConfig != null)
                {
                    if (kioskConfig.Open == KioskOpenState.Open && openText == "ZAMKNIĘTE")
                    {
                        workerKioskMonitoring.ReportProgress(0, true);
                    }
                    if (kioskConfig.Open == KioskOpenState.Closed && openText == "OTWARTE")
                    {
                        workerKioskMonitoring.ReportProgress(0, false);
                    }
                }

                Thread.Sleep(1000);
            }
        }

        private void WorkerKioskMonitoringProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.UserState is KioskPeripheralState)
            {
                if ((KioskPeripheralState)e.UserState == KioskPeripheralState.KeyboardError)
                {
                    EnterKeyboardErrorState();
                }
                if ((KioskPeripheralState)e.UserState == KioskPeripheralState.KeyboardOk)
                {
                    EndKeyboardErrorState();
                }
                if ((KioskPeripheralState)e.UserState == KioskPeripheralState.PrinterError)
                {
                    EnterPrinterErrorState();
                }
                if ((KioskPeripheralState)e.UserState == KioskPeripheralState.PrinterOk)
                {
                    EndPrinterErrorState();
                }
                if ((KioskPeripheralState)e.UserState == KioskPeripheralState.PrinterPaperError)
                {
                    EnterPrinterPaperErrorState();
                }
                if ((KioskPeripheralState)e.UserState == KioskPeripheralState.PrinterPaperOk)
                {
                    EndPrinterPaperErrorState();
                }
                if ((KioskPeripheralState)e.UserState == KioskPeripheralState.FocusActive)
                {
                    FocusLost = false;
                    SetFocusedProcessText();
                    tbFocusLost.Visibility = Visibility.Visible;
                }
                if ((KioskPeripheralState)e.UserState == KioskPeripheralState.FocusLost)
                {
                    FocusLost = true;
                    SetFocusedProcessText();
                    ShowApp();
                    tbFocusLost.Visibility = Visibility.Visible;
                }
            }

            if (e.UserState is bool)
            {
                if ((bool)e.UserState)
                {
                    tbOpenState.Text = "OTWARTE";
                    tbOpenState.Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 182, 0));
                    tbOpenState.FontFamily = new FontFamily("Lucida Console");
                    tbOpenHours.Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 182, 0));
                    tbOpenHours.FontFamily = new FontFamily("Lucida Console");
                }
                else
                {
                    tbOpenState.Text = "ZAMKNIĘTE";
                    tbOpenState.Foreground = Brushes.DarkRed;
                    tbOpenHours.Foreground = Brushes.DarkRed;
                }
            }
        }

        public bool ApplicationIsActivated()
        {
            var activatedHandle = GetForegroundWindow();
            if (activatedHandle == IntPtr.Zero)
            {
                return false;       // No window is currently activated
            }

            var procId = Process.GetCurrentProcess().Id;
            int activeProcId;
            GetWindowThreadProcessId(activatedHandle, out activeProcId);

            return activeProcId == procId;
        }
        private void SetFocusedProcessText()
        {
            var activatedHandle = GetForegroundWindow();

            var procId = Process.GetCurrentProcess().Id;
            int activeProcId;
            GetWindowThreadProcessId(activatedHandle, out activeProcId);

            tbFocusLost.Text = activeProcId + " / " + procId;
        }
        public static void ShowApp()
        {
            Process[] processList = Process.GetProcesses();

            foreach (Process theProcess in processList)
            {
                string processName = theProcess.ProcessName;
                string mainWindowTitle = theProcess.MainWindowTitle;
                if (processName == "KioskAppWpf")
                {
                    SetForegroundWindow(theProcess.MainWindowHandle);
                    SetActiveWindow(theProcess.MainWindowHandle);
                    SetFocus(new HandleRef(null, theProcess.MainWindowHandle));
                }
            }
        }
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        private static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetWindowThreadProcessId(IntPtr handle, out int processId);
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr SetFocus(HandleRef hWnd);
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SetActiveWindow(IntPtr hWnd);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool SetForegroundWindow(IntPtr hWnd);
        #endregion

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftShift) && Keyboard.IsKeyDown(Key.RightAlt) && Keyboard.IsKeyDown(Key.S))
            {
                if (servicePageOn)
                {
                    servicePageOn = false;
                    BindPage();
                }
                else
                {
                    ShowPage(DisplayPage.Service);
                    servicePageOn = true;
                }
            }
            RfidCardSim(sender, e);
        }

        #region displaying pages
        public void BindPage(DisplayPage page)
        {
            CurrentPage = page;
            BindPage();
        }
        private void BindPage()
        {
            ShowPage(CurrentPage);
        }
        private void ShowPage(DisplayPage page)
        {
            if (Startup && ErrorState && page != DisplayPage.StartupPage && StartupTime.AddSeconds(_StartupScreenTimeoutInSeconds) > DateTime.Now)
            {
                return;
            }
            else if (Startup && (!ErrorState || StartupTime.AddSeconds(_StartupScreenTimeoutInSeconds) < DateTime.Now))
            {
                Startup = false;
            }

            if (page == DisplayPage.ErrorPage)
            {
                ErrorStartDateTime = DateTime.Now;
            }

            if (servicePageOn)
            {
                return;
            }
            if (KioskBlocked && page != DisplayPage.Service)
            {
                page = DisplayPage.KioskBlocked;
            }
            if ((ErrorState && !KioskBlocked && !Startup) && page != DisplayPage.Service && page != DisplayPage.ProbeService)
            {
                page = DisplayPage.ErrorPage;
                if (ErrorPage == null)
                {
                    ErrorPage = new ErrorPage();
                }
            }

            ShowingPage = page;
            switch (page)
            {
                case DisplayPage.RfidCardPage: fMain.Content = RfidCardPage; break;
                case DisplayPage.CargoTypePage: fMain.Content = CargoTypePage; break;
                case DisplayPage.DbTypePage: fMain.Content = DbTypePage; break;
                case DisplayPage.SelectCompanySearchType: fMain.Content = SelectCompanySearchType; break;
                case DisplayPage.SelectCompanyByName: fMain.Content = SelectCompanyByName; break;
                case DisplayPage.SelectGmina: fMain.Content = SelectGmina; break;
                case DisplayPage.InsertGmina: fMain.Content = InsertGmina; break;
                case DisplayPage.SelectMiejscowosc: fMain.Content = SelectMiejscowosc; break;
                case DisplayPage.InsertMiejscowosc: fMain.Content = InsertMiejscowosc; break;
                case DisplayPage.SelectUlica: fMain.Content = SelectUlica; break;
                case DisplayPage.InsertUlica: fMain.Content = InsertUlica; break;
                case DisplayPage.SelectRod: fMain.Content = SelectRod; break;
                case DisplayPage.InsertPlotNumber: fMain.Content = InsertPlotNumber; break;
                case DisplayPage.SelectNumer: fMain.Content = SelectNumer; break;
                case DisplayPage.SelectCompanyName: fMain.Content = SelectCompanyName; break;
                case DisplayPage.InsertCompanyName: fMain.Content = InsertCompanyName; break;
                case DisplayPage.SelectContractNo: fMain.Content = SelectContractNo; break;
                case DisplayPage.SelectAmount: fMain.Content = SelectAmount; break;
                case DisplayPage.AddressSummary: fMain.Content = AddressSummary; break;
                case DisplayPage.DumpPage: fMain.Content = DumpPage; break;
                case DisplayPage.ScreenCelaningPage: fMain.Content = ScreenCleaningPage; break;
                case DisplayPage.ThankYouPage: fMain.Content = ThankYouPage; break;
                case DisplayPage.Service: fMain.Content = ServicePage; break;
                case DisplayPage.ProbeService: fMain.Content = ProbeServicePage; break;

                case DisplayPage.KioskBlocked: fMain.Content = KioskBlockedPage; break;
                case DisplayPage.ErrorPage: fMain.Content = ErrorPage; break;
                case DisplayPage.UnknownCardPage: fMain.Content = UnknownCardPage; break;
                case DisplayPage.NoLicensePage: fMain.Content = NoLicensePage; break;
                case DisplayPage.CardBlocked: fMain.Content = CardBlockedPage; break;
                case DisplayPage.NoRodInDb: fMain.Content = NoRodPage; break;
                case DisplayPage.GateOpened: fMain.Content = GateOpenPage; break;
                case DisplayPage.NoFlowPage: fMain.Content = NoFlowPage; break;
                case DisplayPage.SequenceErrorAlertPage: fMain.Content = ParameterAlertPage; break;
                case DisplayPage.SequenceErrorScreenCleaningTimeout: fMain.Content = ScreenCleaningTimeout; break;
                case DisplayPage.SequenceErrorPathChangingTimeout: fMain.Content = PathChangingTimeout; break;
                case DisplayPage.SequenceErrorDoorOpened: fMain.Content = GateOpenPage; break;
                case DisplayPage.StationNotReadyPage: fMain.Content = StationNotReadyPage; break;

                case DisplayPage.StartupPage: fMain.Content = StartupPage; break;
            }
        }
        #endregion

        #region Error pages
        private void BlockKiosk()
        {
            if (Transaction == null)
            {
                KioskBlocked = true;
                Transaction = null;
                BindPage(DisplayPage.KioskBlocked);
            }
        }
        private void UnBlockKiosk()
        {
            KioskBlocked = false;

            if (Transaction == null)
            {
                BindPage(DisplayPage.RfidCardPage);
            }
        }
        private void ShowErrorPage(DisplayPage errorPage)
        {
            ShowPage(errorPage);

            ErrorPageTimer = new DispatcherTimer();
            ErrorPageTimer.Interval = TimeSpan.FromSeconds(_errorPageDurationInSeconds);
            ErrorPageTimer.Tick += new EventHandler(ErrorPageTimerElapsed);
            ErrorPageTimer.Start();
        }
        private void ErrorPageTimerElapsed(object sender, EventArgs e)
        {
            ErrorPageTimer.Tick -= new EventHandler(ErrorPageTimerElapsed);
            BindPage();
        }
        #endregion

        #region OPC Steering
        #endregion

        #region Error States
        public void EnterRfidErrorState(string errorMsg)
        {
            if (!RfidErrorState)
            {
                LogDebug("Awaria start - RFID");
                LogDebug(errorMsg);
            }

            RfidErrorState = true;
            TransactionErrorSequenceStop();

            if (!(fMain.Content is ServicePage))
            {
                ShowPage(DisplayPage.ErrorPage);
            }
            CurrentPage = DisplayPage.RfidCardPage;
        }
        public void EndRfidErrorState()
        {
            if (ErrorPage != null && ErrorPage.TransactionSaving) return;
            if (ErrorStartDateTime.HasValue && ErrorStartDateTime.Value.AddSeconds(_errorPageDurationInSeconds) >= DateTime.Now) return;

            RfidErrorState = false;
            Transaction = null;
            CurrentTransactionAddress = null;
            CurrentTransactionCompany = null;
            PrevTransactionAddress = null;
            if (!ErrorState)
                BindPage();

            LogDebug("Awaria koniec - RFID");
        }
        public void EnterDbErrorState()
        {
            if (!DbErrorState)
                LogDebug("Awaria start - DB");

            DbErrorState = true;
            TransactionErrorSequenceStop();

            if (!(fMain.Content is ServicePage))
            {
                ShowPage(DisplayPage.ErrorPage);
            }

            CurrentPage = DisplayPage.RfidCardPage;
        }
        public void EndDbErrorState()
        {
            if (ErrorPage != null && ErrorPage.TransactionSaving) return;
            if (ErrorStartDateTime.HasValue && ErrorStartDateTime.Value.AddSeconds(_errorPageDurationInSeconds) >= DateTime.Now) return;

            DbErrorState = false;
            Transaction = null;
            CurrentTransactionAddress = null;
            CurrentTransactionCompany = null;
            PrevTransactionAddress = null;
            if (!ErrorState)
                BindPage();

            LogDebug("Awaria koniec - DB");
        }
        public void EnterOpcErrorState()
        {
            if (_opcOn)
            {
                if (!OpcErrorState)
                    LogDebug("Awaria start - OPC");

                OpcErrorState = true;

                if (Transaction != null && !Transaction.Saved)
                {
                    Transaction.DumpEnd = DateTime.Now;
                    Transaction.ActualAmount = 0;
                    Transaction.TransactionEnd = DateTime.Now;
                    Transaction.FinishReason = "Brak komunikacji";
                    Transaction.Saved = DbWrite.SaveTransaction(Transaction);
                    PrintRecipt();
                    Transaction = null;
                }

                if (!(fMain.Content is ServicePage))
                {
                    ShowPage(DisplayPage.ErrorPage);
                }

                CurrentPage = DisplayPage.RfidCardPage;
            }
        }
        public void EndOpcErrorState()
        {
            if (ErrorPage != null && ErrorPage.TransactionSaving) return;
            if (ErrorStartDateTime.HasValue && ErrorStartDateTime.Value.AddSeconds(_errorPageDurationInSeconds) >= DateTime.Now) return;

            if (_opcOn)
            {
                OpcErrorState = false;
                Transaction = null;
                CurrentTransactionAddress = null;
                CurrentTransactionCompany = null;
                PrevTransactionAddress = null;
            }
            if (!ErrorState)
                BindPage();

            LogDebug("Awaria koniec - OPC");
        }

        public void EnterPlcLifeBitErrorState()
        {
            if (_opcOn)
            {
                if (!PlcLifeBitError)
                    LogDebug("Awaria start - Plc Life Bit");

                PlcLifeBitError = true;

                if (Transaction != null && Transaction.DumpStarted)
                {
                    Transaction.FinishReason = "Brak komunikacji";
                    Transaction.DumpEnd = DateTime.Now;
                    Transaction.TransactionEnd = DateTime.Now;
                    if (!Transaction.Saved)
                    {
                        Transaction.Saved = DbWrite.SaveTransaction(Transaction);
                        PrintRecipt();
                    }
                }

                if (!(fMain.Content is ServicePage))
                {
                    ShowPage(DisplayPage.ErrorPage);
                }

                CurrentPage = DisplayPage.RfidCardPage;
            }
        }
        public void EndPlcLifeBitErrorState()
        {
            if (ErrorPage != null && ErrorPage.TransactionSaving) return;
            if (ErrorStartDateTime.HasValue && ErrorStartDateTime.Value.AddSeconds(_errorPageDurationInSeconds) >= DateTime.Now) return;

            if (_opcOn)
            {
                PlcLifeBitError = false;
                Transaction = null;
                CurrentTransactionAddress = null;
                CurrentTransactionCompany = null;
                PrevTransactionAddress = null;
                if (!ErrorState)
                    BindPage();

                LogDebug("Awaria koniec - Plc Life Bit");
            }
        }
        public void EnterTransactionLifeBitErrorState()
        {
            if (_opcOn)
            {
                if (!TransactionLifeBitError)
                    LogDebug("Awaria start - Transakcja Life Bit");

                TransactionLifeBitError = true;
                ErrorStartDateTime = DateTime.Now;

                if (Transaction != null && Transaction.DumpStarted)
                {
                    Transaction.FinishReason = "Brak komunikacji";
                    Transaction.DumpEnd = DateTime.Now;
                    Transaction.TransactionEnd = DateTime.Now;
                    if (!Transaction.Saved)
                    {
                        //Transaction.Saved = DbWrite.SaveTransaction(Transaction);
                        Transaction.Saved = TransactionSavingService.Enqueue(Transaction);
                        PrintRecipt();
                    }
                }

                if (!(fMain.Content is ServicePage))
                {
                    ShowPage(DisplayPage.ErrorPage);
                }

                CurrentPage = DisplayPage.RfidCardPage;
            }
        }
        public void EndTransactionLifeBitErrorState()
        {
            if (ErrorPage != null && ErrorPage.TransactionSaving) return;
            if (ErrorStartDateTime.HasValue && ErrorStartDateTime.Value.AddSeconds(_errorPageDurationInSeconds) >= DateTime.Now) return;

            if (_opcOn)
            {
                TransactionLifeBitError = false;
                Transaction = null;
                CurrentTransactionAddress = null;
                CurrentTransactionCompany = null;
                PrevTransactionAddress = null;
                if (!ErrorState)
                    BindPage();
            }

            LogDebug("Awaria end - Transakcja Life Bit");
        }

        public void EnterKeyboardErrorState()
        {
            if (KeyboardErrorState)
                LogDebug("Awaria start - Klawiatura");

            KeyboardErrorState = true;
            TransactionErrorSequenceStop();

            if (!(fMain.Content is ServicePage))
            {
                ShowPage(DisplayPage.ErrorPage);
            }

            CurrentPage = DisplayPage.RfidCardPage;
        }
        public void EndKeyboardErrorState()
        {
            if (ErrorPage != null && ErrorPage.TransactionSaving) return;
            if (ErrorStartDateTime.HasValue && ErrorStartDateTime.Value.AddSeconds(_errorPageDurationInSeconds) >= DateTime.Now) return;

            KeyboardErrorState = false;
            Transaction = null;
            CurrentTransactionAddress = null;
            CurrentTransactionCompany = null;
            PrevTransactionAddress = null;
            if (!ErrorState)
                BindPage();

            LogDebug("Awaria koniec - Klawiatura");
        }
        public void EnterPrinterErrorState()
        {
            if (!PrinterErrorState)
                LogDebug("Awaria start - Drukarka");

            PrinterErrorState = true;
            TransactionErrorSequenceStop();

            if (_printerOn)
            {
                if (!(fMain.Content is ServicePage))
                {
                    ShowPage(DisplayPage.ErrorPage);
                }
            }

            CurrentPage = DisplayPage.RfidCardPage;
        }
        public void EndPrinterErrorState()
        {
            if (ErrorPage != null && ErrorPage.TransactionSaving) return;
            if (ErrorStartDateTime.HasValue && ErrorStartDateTime.Value.AddSeconds(_errorPageDurationInSeconds) >= DateTime.Now) return;

            PrinterErrorState = false;
            Transaction = null;
            CurrentTransactionAddress = null;
            CurrentTransactionCompany = null;
            PrevTransactionAddress = null;
            if (!ErrorState)
                BindPage();

            LogDebug("Awaria koniec - Drukarka");
        }
        public void EnterPrinterPaperErrorState()
        {
            if (PrinterOutOfPaperErrorState)
                LogDebug("Awaria start - Drukarka, brak papieru");

            PrinterOutOfPaperState = true;
            if (_printerOn)
            {
                if (!(fMain.Content is ServicePage))
                {
                    ShowPage(DisplayPage.ErrorPage);
                }

                CurrentPage = DisplayPage.RfidCardPage;
            }
        }
        public void EndPrinterPaperErrorState()
        {
            if (ErrorPage != null && ErrorPage.TransactionSaving) return;
            if (ErrorStartDateTime.HasValue && ErrorStartDateTime.Value.AddSeconds(_errorPageDurationInSeconds) >= DateTime.Now) return;

            PrinterOutOfPaperState = false;
            //Transaction = null;
            //CurrentTransactionAddress = null;
            //CurrentTransactionCompany = null;
            //PrevTransactionAddress = null;
            if (!ErrorState)
                BindPage();

            LogDebug("Awaria koniec - Drukarka, brak papieru");
        }

        public void EnterPower230LostErrorState()
        {
            if (Power230LostError)
                LogDebug("Awaria start - Zasilanie 230V");

            Power230LostError = true;
            if (!(fMain.Content is ServicePage))
            {
                ShowPage(DisplayPage.ErrorPage);
            }

            CurrentPage = DisplayPage.RfidCardPage;
        }
        public void EndPower230LostErrorState()
        {
            if (ErrorPage != null && ErrorPage.TransactionSaving) return;

            Power230LostError = false;
            Transaction = null;
            CurrentTransactionAddress = null;
            CurrentTransactionCompany = null;
            PrevTransactionAddress = null;
            if (!ErrorState)
                BindPage();

            LogDebug("Awaria koniec - Zasilanie 230V");
        }
        public void EnterPower24LostErrorState()
        {
            if (Power24LostError)
                LogDebug("Awaria start - Zasilanie 24V");

            Power24LostError = true;
            if (!(fMain.Content is ServicePage))
            {
                ShowPage(DisplayPage.ErrorPage);
            }

            CurrentPage = DisplayPage.RfidCardPage;
        }
        public void EndPower24LostErrorState()
        {
            if (ErrorPage != null && ErrorPage.TransactionSaving) return;

            Power24LostError = false;
            Transaction = null;
            CurrentTransactionAddress = null;
            CurrentTransactionCompany = null;
            PrevTransactionAddress = null;
            if (!ErrorState)
                BindPage();

            LogDebug("Awaria koniec - Zasilanie 24V");
        }

        public void EnterFlowMeterErrorState()
        {
            if (!FlowMeterError)
                LogDebug("Awaria start - Przepływomierz");

            FlowMeterError = true;
            if (!(fMain.Content is ServicePage))
            {
                ShowPage(DisplayPage.ErrorPage);
            }

            CurrentPage = DisplayPage.RfidCardPage;
        }
        public void EndFlowMeterErrorState()
        {
            if (ErrorPage != null && ErrorPage.TransactionSaving) return;

            FlowMeterError = false;
            Transaction = null;
            CurrentTransactionAddress = null;
            CurrentTransactionCompany = null;
            PrevTransactionAddress = null;
            if (!ErrorState)
                BindPage();

            LogDebug("Awaria koniec - Przepływomierz");
        }
        public void EnterDumpPathErrorState()
        {
            if (!FlowMeterError)
                LogDebug("Awaria start - Droga zrzutu");

            DumpPathError = true;
            if (!(fMain.Content is ServicePage))
            {
                ShowPage(DisplayPage.ErrorPage);
            }

            CurrentPage = DisplayPage.RfidCardPage;
        }
        public void EndDumpPathErrorState()
        {
            if (ErrorPage != null && ErrorPage.TransactionSaving) return;

            DumpPathError = false;
            Transaction = null;
            CurrentTransactionAddress = null;
            CurrentTransactionCompany = null;
            PrevTransactionAddress = null;
            if (!ErrorState)
                BindPage();

            LogDebug("Awaria koniec - Droga zrzutu");
        }
        public void EnterScreenErrorState()
        {
            if (!FlowMeterError)
                LogDebug("Awaria start - Sito");

            ScreenError = true;
            if (!(fMain.Content is ServicePage))
            {
                ShowPage(DisplayPage.ErrorPage);
            }

            CurrentPage = DisplayPage.RfidCardPage;
        }
        public void EndScreenErrorState()
        {
            if (ErrorPage != null && ErrorPage.TransactionSaving) return;

            ScreenError = false;
            Transaction = null;
            CurrentTransactionAddress = null;
            CurrentTransactionCompany = null;
            PrevTransactionAddress = null;
            if (!ErrorState)
                BindPage();

            LogDebug("Awaria koniec - Sito");
        }
        public void EnterScreenFullErrorState()
        {
            if (!FlowMeterError)
                LogDebug("Awaria start - Sito pełne");

            ScreenFullError = true;
            if (!(fMain.Content is ServicePage))
            {
                ShowPage(DisplayPage.ErrorPage);
            }

            CurrentPage = DisplayPage.RfidCardPage;
        }
        public void EndScreenFullErrorState()
        {
            if (ErrorPage != null && ErrorPage.TransactionSaving) return;

            ScreenFullError = false;
            Transaction = null;
            CurrentTransactionAddress = null;
            CurrentTransactionCompany = null;
            PrevTransactionAddress = null;
            if (!ErrorState)
                BindPage();

            LogDebug("Awaria koniec - Sito pełne");
        }

        public void EnterProbeErrorState()
        {
            if (!ProbeError)
                LogDebug("Awaria start - Pobierak");

            ProbeError = true;
            if (!(fMain.Content is ServicePage))
            {
                ShowPage(DisplayPage.ErrorPage);
            }

            CurrentPage = DisplayPage.RfidCardPage;
        }
        public void EndProbeErrorState()
        {
            if (ErrorPage != null && ErrorPage.TransactionSaving) return;

            ProbeError = false;
            Transaction = null;
            CurrentTransactionAddress = null;
            CurrentTransactionCompany = null;
            PrevTransactionAddress = null;
            if (!ErrorState)
                BindPage();

            LogDebug("Awaria koniec - Pobierak");
        }
        public void EnterProbeReadyErrorState()
        {
            if (!ProbeReadyError)
                LogDebug("Awaria start - Pobierak, gotowość");

            ProbeReadyError = true;
            if (!(fMain.Content is ServicePage))
            {
                ShowPage(DisplayPage.ErrorPage);
            }

            CurrentPage = DisplayPage.RfidCardPage;
        }
        public void EndProbeReadyErrorState()
        {
            if (ErrorPage != null && ErrorPage.TransactionSaving) return;

            ProbeReadyError = false;
            if (!ErrorState)
                BindPage();

            LogDebug("Awaria start - Pobierak, gotowość");
        }
        public void EnterProbeFullErrorState()
        {
            if (!ProbeFullError)
                LogDebug("Awaria start - Pobierak, brak butelek");

            ProbeFullError = true;
            if (!(fMain.Content is ServicePage))
            {
                ShowPage(DisplayPage.ErrorPage);
            }

            CurrentPage = DisplayPage.RfidCardPage;
        }
        public void EndProbeFullErrorState()
        {
            if (ErrorPage != null && ErrorPage.TransactionSaving) return;

            ProbeFullError = false;
            Transaction = null;
            CurrentTransactionAddress = null;
            CurrentTransactionCompany = null;
            PrevTransactionAddress = null;
            if (!ErrorState)
                BindPage();

            LogDebug("Awaria koniec - Pobierak, brak butelek");
        }
        public void EnterPhMeterErrorState()
        {
            if (!PhMeterError)
                LogDebug("Awaria start - Pomiar pH");

            PhMeterError = true;
            if (!(fMain.Content is ServicePage))
            {
                ShowPage(DisplayPage.ErrorPage);
            }

            CurrentPage = DisplayPage.RfidCardPage;
        }
        public void EndPhMeterErrorState()
        {
            if (ErrorPage != null && ErrorPage.TransactionSaving) return;

            PhMeterError = false;
            Transaction = null;
            CurrentTransactionAddress = null;
            CurrentTransactionCompany = null;
            PrevTransactionAddress = null;
            if (!ErrorState)
                BindPage();

            LogDebug("Awaria koniec - Pomiar pH");
        }
        public void EnterCondMeterErrorState()
        {
            if (!CondMeterError)
                LogDebug("Awaria start - Pomiar przewodności");

            CondMeterError = true;
            if (!(fMain.Content is ServicePage))
            {
                ShowPage(DisplayPage.ErrorPage);
            }

            CurrentPage = DisplayPage.RfidCardPage;
        }
        public void EndCondMeterErrorState()
        {
            if (ErrorPage != null && ErrorPage.TransactionSaving) return;

            CondMeterError = false;
            Transaction = null;
            CurrentTransactionAddress = null;
            CurrentTransactionCompany = null;
            PrevTransactionAddress = null;
            if (!ErrorState)
                BindPage();

            LogDebug("Awaria koniec - Pomiar przewodności");
        }
        public void EnterTempMeterErrorState()
        {
            if (!TempMeterError)
                LogDebug("Awaria start - Pomiar temperatury");

            TempMeterError = true;
            if (!(fMain.Content is ServicePage))
            {
                ShowPage(DisplayPage.ErrorPage);
            }

            CurrentPage = DisplayPage.RfidCardPage;
        }
        public void EndTempMeterErrorState()
        {
            if (ErrorPage != null && ErrorPage.TransactionSaving) return;

            TempMeterError = false;
            Transaction = null;
            CurrentTransactionAddress = null;
            CurrentTransactionCompany = null;
            PrevTransactionAddress = null;
            if (!ErrorState)
                BindPage();

            LogDebug("Awaria koniec - Pomiar temperatury");
        }
        public void EnterPressureMeterErrorState()
        {
            if (!PressureMeterError)
                LogDebug("Awaria start - Pomiar ciśnienia");

            PressureMeterError = true;
            if (!(fMain.Content is ServicePage))
            {
                ShowPage(DisplayPage.ErrorPage);
            }

            CurrentPage = DisplayPage.RfidCardPage;
        }
        public void EndPressureMeterErrorState()
        {
            if (ErrorPage != null && ErrorPage.TransactionSaving) return;

            PressureMeterError = false;
            Transaction = null;
            CurrentTransactionAddress = null;
            CurrentTransactionCompany = null;
            PrevTransactionAddress = null;
            if (!ErrorState)
                BindPage();

            LogDebug("Awaria koniec - Pomiar ciśnienia");
        }
        public void EnterChztMeterErrorState()
        {
            if (!ChztMeterError)
                LogDebug("Awaria start - Pomiar CHZT");

            ChztMeterError = true;
            if (!(fMain.Content is ServicePage))
            {
                ShowPage(DisplayPage.ErrorPage);
            }

            CurrentPage = DisplayPage.RfidCardPage;
        }
        public void EndChztMeterErrorState()
        {
            if (ErrorPage != null && ErrorPage.TransactionSaving) return;

            ChztMeterError = false;
            Transaction = null;
            CurrentTransactionAddress = null;
            CurrentTransactionCompany = null;
            PrevTransactionAddress = null;
            if (!ErrorState)
                BindPage();

            LogDebug("Awaria koniec - Pomiar CHZT");
        }
        public void EnterKioskDoorErrorState()
        {
            if (!KioskDoorError)
                LogDebug("Awaria start - Drzwi kiosku otwarte");

            KioskDoorError = true;
            if (!(fMain.Content is ServicePage))
            {
                ShowPage(DisplayPage.ErrorPage);
            }

            CurrentPage = DisplayPage.RfidCardPage;
        }
        public void EndKioskDoorErrorState()
        {
            if (ErrorPage != null && ErrorPage.TransactionSaving) return;

            KioskDoorError = false;
            Transaction = null;
            CurrentTransactionAddress = null;
            CurrentTransactionCompany = null;
            PrevTransactionAddress = null;
            if (!ErrorState)
                BindPage();

            LogDebug("Awaria koniec - Drzwi kiosku otwarte");
        }
        public void EnterValveMeterErrorState()
        {
            if (!ValveMeterError)
                LogDebug("Awaria start - Woda dla urządzeń pomiarowych");

            ValveMeterError = true;
            if (!(fMain.Content is ServicePage))
            {
                ShowPage(DisplayPage.ErrorPage);
            }

            CurrentPage = DisplayPage.RfidCardPage;
        }
        public void EndValveMeterErrorState()
        {
            if (ErrorPage != null && ErrorPage.TransactionSaving) return;

            ValveMeterError = false;
            Transaction = null;
            CurrentTransactionAddress = null;
            CurrentTransactionCompany = null;
            PrevTransactionAddress = null;
            if (!ErrorState)
                BindPage();

            LogDebug("Awaria koniec - Woda dla urządzeń pomiarowych");
        }
        public void EnterZasuwaErrorState()
        {
            if (!ZasuwaError)
                LogDebug("Awaria start - Zasuwa");

            ZasuwaError = true;
            if (!(fMain.Content is ServicePage))
            {
                ShowPage(DisplayPage.ErrorPage);
            }

            CurrentPage = DisplayPage.RfidCardPage;
        }
        public void EndZasuwaErrorState()
        {
            if (ErrorPage != null && ErrorPage.TransactionSaving) return;

            ZasuwaError = false;
            Transaction = null;
            CurrentTransactionAddress = null;
            CurrentTransactionCompany = null;
            PrevTransactionAddress = null;
            if (!ErrorState)
                BindPage();

            LogDebug("Awaria koniec - Zasuwa");
        }

        public void EnterErrorState(ErrorType error)
        {
            SetErrorValue(error, true);

            if (!(fMain.Content is ServicePage))
            {
                ShowPage(DisplayPage.ErrorPage);
            }

            CurrentPage = DisplayPage.RfidCardPage;
        }
        public void EndErrorState(ErrorType error)
        {
            if (ErrorPage != null && ErrorPage.TransactionSaving) return;

            SetErrorValue(error, false);
            Transaction = null;
            CurrentTransactionAddress = null;
            CurrentTransactionCompany = null;
            PrevTransactionAddress = null;
            if (!ErrorState)
                BindPage();
        }
        public void SetErrorValue(ErrorType error, bool value)
        {
            if (value)
                LogErrorStart(error);

            switch (error)
            {
                case ErrorType.ProbeDoorOpen:
                    ProbeDoorOpenError = value; break;
                case ErrorType.Air:
                    AirError = value; break;
                case ErrorType.Distribution230VAC:
                    Distribution230VACError = value; break;
                case ErrorType.Rack230VAC:
                    Rack230VACError = value; break;
                case ErrorType.Rack24VDC:
                    Rack24VDCError = value; break;
                case ErrorType.Rack24VDCUPS:
                    Rack24VDCUpsError = value; break;
                case ErrorType.Rack12VDC:
                    Rack12VDCError = value; break;

                case ErrorType.PumpRoom:
                    PumpRoomError = value; break;
                case ErrorType.PumpRoomWork:
                    PumpRoomWorkError = value; break;
                case ErrorType.ScreenRoomLeak:
                    ScreenRoomLeakError = value; break;
                case ErrorType.Auger:
                    AugerError = value; break;
                case ErrorType.PreScreen:
                    PreScreenError = value; break;
            }

            if (!value)
                LogErrorEnd(error);
        }
        public void LogErrorStart(ErrorType error)
        {
            switch (error)
            {
                case ErrorType.ProbeDoorOpen:
                    if (!ProbeDoorOpenError) LogDebug("Awaria start - " + error); break;
                case ErrorType.Air:
                    if (!AirError) LogDebug("Awaria start - " + error); break;
                case ErrorType.Distribution230VAC:
                    if (!Distribution230VACError) LogDebug("Awaria start - " + error); break;
                case ErrorType.Rack230VAC:
                    if (!Rack230VACError) LogDebug("Awaria start - " + error); break;
                case ErrorType.Rack24VDC:
                    if (!Rack24VDCError) LogDebug("Awaria start - " + error); break;
                case ErrorType.Rack24VDCUPS:
                    if (!Rack24VDCUpsError) LogDebug("Awaria start - " + error); break;
                case ErrorType.Rack12VDC:
                    if (!Rack12VDCError) LogDebug("Awaria start - " + error); break;

                case ErrorType.PumpRoom:
                    if (!PumpRoomError) LogDebug("Awaria start - " + error); break;
                case ErrorType.PumpRoomWork:
                    if (!PumpRoomWorkError) LogDebug("Awaria start - " + error); break;
                case ErrorType.ScreenRoomLeak:
                    if (!ScreenRoomLeakError) LogDebug("Awaria start - " + error); break;
                case ErrorType.Auger:
                    if (!AugerError) LogDebug("Awaria start - " + error); break;
                case ErrorType.PreScreen:
                    if (!PreScreenError) LogDebug("Awaria start - " + error); break;
            }
        }
        public void LogErrorEnd(ErrorType error)
        {
            LogDebug("Awaria koniec - " + error);
        }

        public void EnterGateOpenState()
        {
            if (!(fMain.Content is ServicePage))
            {
                CurrentPage = DisplayPage.RfidCardPage;
                ShowErrorPage(DisplayPage.GateOpened);
            }

            Transaction = null;
            LogDebug("Awaria start - Brama otwarta");
        }
        public void EnterSequenceGateOpenState()
        {
            if (!(fMain.Content is ServicePage))
            {
                CurrentPage = DisplayPage.RfidCardPage;
                ShowErrorPage(DisplayPage.GateOpened);
            }

            Transaction = null;

            LogDebug("Awaria koniec - Brama otwarta");
        }

        public void EndErrorState()
        {
            if (!ErrorState)
                BindPage();
            LogDebug("Awaria koniec");
        }

        private void TransactionErrorSequenceStop()
        {
            if (Transaction != null && Transaction.DumpStarted && !Transaction.Saved)
            {
                Transaction.FinishReason = "Błąd urządzenia";

                var errors = OpcManager.StopCodeState.GetErrorMessages();
                if (errors.Count > 0)
                {
                    Transaction.FinishReason = errors.FirstOrDefault();
                }

                LogDebug("Sekwencja - wysyłanie zatrzymania z powodu błędu urządzenia");
                foreach (var msg in errors)
                {
                    LogDebug("Błąd urządzenia:" + msg);
                }
                OpcManager.SendSequenceStop();
                LogDebug("Sekwencja - wysyłane zatrzymanie z powodu błędu urządzenia");
            }
        }
        #endregion

        private void RunClock()
        {
            DispatcherTimer timer = new DispatcherTimer(new TimeSpan(0, 0, 1), DispatcherPriority.Normal, delegate
            {
                tbClock.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            }, Dispatcher);
        }

        private bool CheckPrinterError()
        {
            if (!_printerOn) return false;
            if (KioskConf != null && KioskConf.KioskBlockages[KioskBlockageType.Printer]) return false;

            // Set management scope
            ManagementScope scope = new ManagementScope(@"\root\cimv2");
            scope.Connect();

            // Select Printers from WMI Object Collections
            ManagementObjectSearcher searcher = new
            ManagementObjectSearcher("SELECT * FROM Win32_Printer");

            string printerName = "";
            foreach (ManagementObject printer in searcher.Get())
            {
                printerName = printer["Name"].ToString();
                if (printerName.Equals(_printerName))
                {
                    if (printer["WorkOffline"].ToString().ToLower().Equals("true"))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        private bool CheckKeyboardError()
        {
            if (KioskConf != null && KioskConf.KioskBlockages[KioskBlockageType.Keyboard]) return false;

            ManagementObjectSearcher searcher = new ManagementObjectSearcher("Select Name from Win32_Keyboard");
            int keyboardCount = 0;
            foreach (ManagementObject keyboard in searcher.Get())
            {
                if (!keyboard.GetPropertyValue("Name").Equals(""))
                {
                    keyboardCount++;
                }
            }
            return keyboardCount == 0;
        }

        #region updating transaction state
        public bool StartNewTransaction(RfidCard card, StationLicense license)
        {
            if (!DevicesState.Flow.SeqReady)
            {
                ShowErrorPage(DisplayPage.StationNotReadyPage);
                return false;
            }

            if (fMain.Content == ServicePage)
            {
                return false;
            }

            if (ErrorState)
            {
                return false;
            }

            SequenceRunning = false;
            TransactionLifeBit = -1;
            var transaction = new Transaction() { Card = card, Address = card.Address, TransactionStart = DateTime.Now, License = license };
            TransactionLifeBitLastStateChange = DateTime.Now;
            Transaction = transaction;

            if (KioskConf.KioskBlockages[KioskBlockageType.Keyboard])
            {
                // blokada klawiatury, przechodzim od razu do zrzutu
                LogDebug("Klawiatura zablokowana");
                Transaction.Cargo = new CargoType() { Id = 5, Name = ".............." };
                CurrentTransactionAddress = new CustomerAddress() { DbType = AddressDbType.Teryt };
                CurrentTransactionCompany = null;
                PrevTransactionAddress = null;

                Transaction.DumpStart = DateTime.Now;
                Transaction.DumpStarted = true;

                BindPage(DisplayPage.DumpPage);
                OpcManager.StartSequence();
                SequenceRunning = true;
                LogDebug("Start sekwencji zrzutu");
            }
            else
            {
                BindPage(DisplayPage.CargoTypePage);
            }

            LogDebug("Transakcja started");

            return true;
        }
        public void UpdateTransactionRestart()
        {
            CurrentTransactionAddress = null;
            CurrentTransactionCompany = null;
            BindPage(DisplayPage.CargoTypePage);
            LogDebug("Transakcja restart");
        }
        public void UpdateTransactionRestartFull()
        {
            var card = Transaction.Card;
            PrevTransactionAddress = null;
            CurrentTransactionAddress = null;
            CurrentTransactionCompany = null;
            var transaction = new Transaction() { Card = card, Address = card.Address, TransactionStart = DateTime.Now };
            transaction.License = DbRead.GetLicense(transaction.Card.Customer.Id);
            Transaction = transaction;
            BindPage(DisplayPage.CargoTypePage);
            LogDebug("Transakcja restart full");
        }
        public void UpdateTransactionCargoType(CargoType cargo)
        {
            Transaction.Cargo = cargo;
            if (Transaction.Cargo.Id == 3) //ROD
            {
                CurrentTransactionAddress = new CustomerAddress() { DbType = AddressDbType.Rod };
                Transaction.EditedAddress = 1;
                BindPage(DisplayPage.SelectRod);
            }
            else if (Transaction.Cargo.Id == 4) //ToiToi
            {
                CurrentTransactionAddress = new CustomerAddress() { DbType = AddressDbType.ToiToi };
                Transaction.EditedAddress = 1;
                BindPage(DisplayPage.SelectContractNo);
            }
            else
            {
                BindPage(DisplayPage.DbTypePage);
            }
        }
        public void UpdateTransactionDbType(AddressDbType type)
        {
            CurrentTransactionAddress = new CustomerAddress() { DbType = type };
            Transaction.EditedAddress = 1;
            if (CurrentTransactionAddress.DbType == AddressDbType.ManualEntry)
            {
                BindPage(DisplayPage.InsertGmina);
            }
            else
            {
                if (Transaction.Cargo.Id == 1)
                {
                    BindPage(DisplayPage.SelectGmina);
                }
                if (Transaction.Cargo.Id == 2)
                {
                    if (CurrentTransactionAddress.DbType == AddressDbType.RegularCustomers)
                    {
                        BindPage(DisplayPage.SelectCompanySearchType);
                    }
                    if (CurrentTransactionAddress.DbType == AddressDbType.Teryt)
                    {
                        BindPage(DisplayPage.SelectGmina);
                    }
                }
            }
        }
        public void UpdateTransactionDbTypeManualEntry(DisplayPage page)
        {
            CurrentTransactionAddress.DbType = AddressDbType.ManualEntry;
            if (CurrentTransactionAddress.DbType == AddressDbType.ManualEntry)
            {
                BindPage(page);
            }
        }
        public void UpdateTransactionCompanySearchType(CompanySearchType type)
        {
            CompanySearchType = type;
            Transaction.EditedAddress = 1;
            if (CurrentTransactionAddress.DbType == AddressDbType.ManualEntry)
            {
                BindPage(DisplayPage.InsertGmina);
            }
            else
            {
                if (type == CompanySearchType.SearchByName)
                {
                    BindPage(DisplayPage.SelectCompanyByName);
                }
                else
                {
                    BindPage(DisplayPage.SelectGmina);
                }
            }
        }
        public void UpdateTransactionCompany(Company company)
        {
            company.Address.DbType = CurrentTransactionAddress.DbType;
            CurrentTransactionAddress = company.Address;
            CurrentTransactionAddress.Company = company;
            CurrentTransactionCompany = company;
            BindPage(DisplayPage.SelectContractNo);
        }
        public void UpdateTransactionCompany(string companyName)
        {
            Company company = new Company() { Name = companyName, Address = CurrentTransactionAddress };
            CurrentTransactionAddress.Company = company;
            CurrentTransactionCompany = company;
            BindPage(DisplayPage.SelectContractNo);
        }
        public void UpdateTransactionAddressGmina(Gmina gmina)
        {
            CurrentTransactionAddress.GminaId = gmina.Id;
            CurrentTransactionAddress.GminaName = gmina.Name;
            if (CurrentTransactionAddress.DbType == AddressDbType.ManualEntry)
            {
                BindPage(DisplayPage.InsertMiejscowosc);
            }
            else
            {
                BindPage(DisplayPage.SelectMiejscowosc);
            }
        }
        public void UpdateTransactionAddressGmina(string gmina)
        {
            CurrentTransactionAddress.GminaName = gmina;
            if (CurrentTransactionAddress.DbType == AddressDbType.ManualEntry)
            {
                BindPage(DisplayPage.InsertMiejscowosc);
            }
            else
            {
                BindPage(DisplayPage.SelectMiejscowosc);
            }
        }
        public void UpdateTransactionAddressMiejscowosc(string miejscowosc)
        {
            CurrentTransactionAddress.MiejscowoscName = miejscowosc;
            if (CurrentTransactionAddress.DbType == AddressDbType.ManualEntry)
            {
                BindPage(DisplayPage.InsertUlica);
            }
            else
            {
                BindPage(DisplayPage.SelectUlica);
            }
        }
        public void UpdateTransactionAddressMiejscowosc(Miejscowosc miejscowosc)
        {
            CurrentTransactionAddress.MiejscowoscId = miejscowosc.Id;
            CurrentTransactionAddress.MiejscowoscName = miejscowosc.Name;
            if (CurrentTransactionAddress.DbType == AddressDbType.ManualEntry)
            {
                BindPage(DisplayPage.InsertUlica);
            }
            else
            {
                BindPage(DisplayPage.SelectUlica);
            }
        }
        public void UpdateTransactionAddressUlica(string ulica)
        {
            CurrentTransactionAddress.UlicaName = ulica;
            BindPage(DisplayPage.SelectNumer);
        }
        public void UpdateTransactionAddressUlica(Ulica ulica)
        {
            CurrentTransactionAddress.UlicaId = ulica.Id;
            CurrentTransactionAddress.UlicaName = ulica.Name;
            BindPage(DisplayPage.SelectNumer);
        }
        public void UpdateTransactionAddressRod(Rod rod)
        {
            if (rod != null)
            {
                CurrentTransactionAddress.RodId = rod.Id;
                CurrentTransactionAddress.RodName = rod.Name;
            }
            BindPage(DisplayPage.InsertPlotNumber);
        }
        public void UpdateTransactionNoRod()
        {
            Transaction = null;
            CurrentPage = DisplayPage.RfidCardPage;
            ShowErrorPage(DisplayPage.NoRodInDb);
        }
        public void UpdateTransactionAddressNumer(string numer)
        {
            CurrentTransactionAddress.AddressNumber = numer;
            if (Transaction.Cargo.Id == 1 || Transaction.Cargo.Id == 3 || Transaction.Cargo.Id == 4)
            {
                BindPage(DisplayPage.SelectContractNo);
            }
            else if (Transaction.Cargo.Id == 2)
            {
                if (CurrentTransactionAddress.DbType == AddressDbType.ManualEntry || CurrentTransactionAddress.DbType == AddressDbType.Teryt)
                {
                    BindPage(DisplayPage.InsertCompanyName);
                }
                else
                {
                    BindPage(DisplayPage.SelectCompanyName);
                }
            }
        }
        public void UpdateTransactionAddressContractNo(string contractNo)
        {
            CurrentTransactionAddress.ContractNo = contractNo;

            BindPage(DisplayPage.SelectAmount);
        }
        public void UpdateTransactionAddressAmount(decimal amount)
        {
            CurrentTransactionAddress.DeclaredAmount = amount;
            Transaction.EditedSewageAmount = amount;
            BindPage(DisplayPage.AddressSummary);
        }
        public void UpdateTransactionNewAddress()
        {
            Transaction.CustomerAddresses.Add(CurrentTransactionAddress);
            CheckHarmonogramedSample(CurrentTransactionAddress);
            PrevTransactionAddress = CurrentTransactionAddress;
            CurrentTransactionAddress = null;
            CurrentTransactionCompany = null;
            Transaction.EditedAddress = 0;
            Transaction.EditedSewageAmount = 0;
            if (Transaction.Cargo.Id == 3) //ROD
            {
                CurrentTransactionAddress = new CustomerAddress() { DbType = AddressDbType.Rod };
                Transaction.EditedAddress = 1;
                BindPage(DisplayPage.SelectRod);
            }
            else if (Transaction.Cargo.Id == 4) //ToiToi
            {
                CurrentTransactionAddress = new CustomerAddress() { DbType = AddressDbType.ToiToi };
                Transaction.EditedAddress = 1;
                BindPage(DisplayPage.SelectContractNo);
            }
            else
            {
                BindPage(DisplayPage.DbTypePage);
            }
        }
        public void UpdateTransactionStartDump()
        {
            Transaction.CustomerAddresses.Add(CurrentTransactionAddress);
            CheckHarmonogramedSample(CurrentTransactionAddress);
            PrevTransactionAddress = CurrentTransactionAddress;
            CurrentTransactionAddress = null;
            CurrentTransactionCompany = null;
            PrevTransactionAddress = null;
            Transaction.EditedAddress = 0;
            Transaction.EditedSewageAmount = 0;
            Transaction.DumpStart = DateTime.Now;
            Transaction.DumpStarted = true;

            BindPage(DisplayPage.DumpPage);
            OpcManager.StartSequence();
            SequenceRunning = true;
            LogDebug("Start sekwencji zrzutu");
        }
        public void UpdateSummary()
        {
            //Transaction.TransactionEnd = DateTime.Now;
            //Transaction.Finished = true;
            //DbWrite.SaveTransaction(Transaction);

            //PrinterManager printerManager = new PrinterManager();
            //if (_printerOn)
            //{
            //    printerManager.PrintRecipt(Transaction);
            //}
            //DbWrite.MarkReciptAsPrinted(Transaction.ReciptId.Value);
            //DbWrite.UpdatePrintedPaperLenght(_reciptLenghtInMilimeters);
            //printerPaperStatus.ReciptPrintedLenghtInMilimeters += _reciptLenghtInMilimeters;
            //BindPage(DisplayPage.ThankYouPage);
            //Step = TransactionStep.ReciptPrint;
        }
        public void UpdateThankYou()
        {
            if (CurrentPage == DisplayPage.ThankYouPage || CurrentPage == DisplayPage.NoFlowPage || CurrentPage == DisplayPage.SequenceErrorAlertPage)
                BindPage(DisplayPage.RfidCardPage);
        }
        public void UpdateTransactionKillToiToi()
        {
            OpcManager.SendSequenceStop();
        }
        #endregion

        private void CheckHarmonogramedSample(CustomerAddress address)
        {
            if (Transaction.HarmonogramedSample.Count == 0 && Transaction.Card.Customer.TakingSamples)
            {
                var sample = DbRead.CheckHarmonogramedSample(Transaction.Card, Transaction.Cargo, address);
                if (sample != null)
                {
                    Transaction.HarmonogramedSample.Add(sample);
                    UpdateOpcParameters();
                }
            }
        }

        private void SetDumpPathMessage()
        {
            var deviceStateId = DevicesState != null ? DevicesState.DumpPath : 0;
            var dumpPathId = deviceStateId == 2 || deviceStateId == 4 ? DumpPath.Backup : DumpPath.Regular;
            switch (dumpPathId)
            {
                case (DumpPath.Regular):
                    tbConnectionType.Text = ConfigurationManager.AppSettings["_dumpPathMessageRegular"];
                    break;
                case (DumpPath.Backup):
                    tbConnectionType.Text = ConfigurationManager.AppSettings["_dumpPathMessageBackup"];
                    break;
            }
        }

        private void ResetApp(bool resetSequence)
        {
            OpcManager = new OpcManager(_opcOn);

            RfidErrorState = DbErrorState = OpcErrorState = KeyboardErrorState = PrinterErrorState = false;

            Connection = null;
            DbRead = null;
            DbWrite = null;
            ServerConnection = null;
            DbServerRead = null;
            DbServerWrite = null;

            if (resetSequence)
            {
                CurrentPage = DisplayPage.RfidCardPage;
                Step = TransactionStep.NoTransaction;
                Transaction = null;

                RunClock();
                BindPage();
            }

            RunRfidWorker();
            RunOpcWorker();
            RunDbMonitoring();
            RunKioskMonitoring();
        }

        private void RfidCardTouchedSim(string rfidCardId)
        {
            if (DbRead == null) return;

            var card = DbRead.GetRfidCard(rfidCardId);
            RfidCardAction(rfidCardId, card, ReaderLocation.Kiosk);
        }

        private void PrintRecipt()
        {
            LogDebug("Sekwencja - paragon, drukowanie");
            if (KioskConf.KioskBlockages[KioskBlockageType.Printer] || PrinterErrorState || PrinterOutOfPaperErrorState) return;

            var printerManager = new PrinterManager();
            var printedDoc = printerManager.PrintRecipt(Transaction, KioskConf);
            DbWrite.UpdatePrintedPaperLenght(printedDoc.TotalHeight);
            printerPaperStatus.ReciptPrintedLenghtInMilimeters += printedDoc.TotalHeight;
            LogDebug("Sekwencja - paragon, wydrukowany");
        }

        private void RfidCardSim(object sender, KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) && e.Key == Key.F1)
            {
                var rfidCardId = "0500460BC8";
                var card = DbRead.GetRfidCard(rfidCardId);
                RfidCardAction(rfidCardId, card, ReaderLocation.GateIn);
            }
            if (Keyboard.IsKeyDown(Key.LeftCtrl) && e.Key == Key.F2)
            {
                var rfidCardId = "1000000001";
                var card = DbRead.GetRfidCard(rfidCardId);
                RfidCardAction(rfidCardId, card, ReaderLocation.GateOut);
            }
            if (Keyboard.IsKeyDown(Key.LeftCtrl) && e.Key == Key.F3)
            {
                var rfidCardId = "0500460BC8";
                var card = DbRead.GetRfidCard(rfidCardId);
                RfidCardAction(rfidCardId, card, ReaderLocation.Kiosk);
            }
            if (Keyboard.IsKeyDown(Key.LeftCtrl) && e.Key == Key.F4)
            {
                var rfidCardId = "1000000001";
                var card = DbRead.GetRfidCard(rfidCardId);
                RfidCardAction(rfidCardId, card, ReaderLocation.Huber);
            }
            if (Keyboard.IsKeyDown(Key.LeftCtrl) && e.Key == Key.F5)
            {
                var rfidCardId = "1000000001";
                var card = DbRead.GetRfidCard(rfidCardId);
                RfidCardAction(rfidCardId, card, ReaderLocation.Rozdzielnia);
            }

            if (Keyboard.IsKeyDown(Key.LeftCtrl) && e.Key == Key.F6)
            {
                var rfidCardId = "";
                //var card = DbRead.GetRfidCard(rfidCardId);
                RfidCardAction(rfidCardId, null, ReaderLocation.Camera);
            }

            if (Keyboard.IsKeyDown(Key.LeftShift) && e.Key == Key.F1)
            {
                var rfidCardId = "0500460BC8";
                var card = DbRead.GetRfidCard(rfidCardId);
                RfidCardAction(rfidCardId, card, ReaderLocation.GateIn);
            }
            if (Keyboard.IsKeyDown(Key.LeftShift) && e.Key == Key.F2)
            {
                var rfidCardId = "2000000002";
                var card = DbRead.GetRfidCard(rfidCardId);
                RfidCardAction(rfidCardId, card, ReaderLocation.GateOut);
            }
            if (Keyboard.IsKeyDown(Key.LeftShift) && e.Key == Key.F3)
            {
                var rfidCardId = "0500460BC8";
                var card = DbRead.GetRfidCard(rfidCardId);
                RfidCardAction(rfidCardId, card, ReaderLocation.Kiosk);
            }
            if (Keyboard.IsKeyDown(Key.LeftShift) && e.Key == Key.F4)
            {
                var rfidCardId = "2000000002";
                var card = DbRead.GetRfidCard(rfidCardId);
                RfidCardAction(rfidCardId, card, ReaderLocation.Huber);
            }
            if (Keyboard.IsKeyDown(Key.LeftShift) && e.Key == Key.F5)
            {
                var rfidCardId = "2000000002";
                var card = DbRead.GetRfidCard(rfidCardId);
                RfidCardAction(rfidCardId, card, ReaderLocation.Rozdzielnia);
            }

            if (Keyboard.IsKeyDown(Key.RightCtrl) && e.Key == Key.F1)
            {
                var rfidCardId = "4000000004";
                var card = DbRead.GetRfidCard(rfidCardId);
                RfidCardAction(rfidCardId, card, ReaderLocation.GateIn);
            }
            if (Keyboard.IsKeyDown(Key.RightCtrl) && e.Key == Key.F2)
            {
                var rfidCardId = "4000000004";
                var card = DbRead.GetRfidCard(rfidCardId);
                RfidCardAction(rfidCardId, card, ReaderLocation.GateOut);
            }
            if (Keyboard.IsKeyDown(Key.RightCtrl) && e.Key == Key.F3)
            {
                var rfidCardId = "4000000004";
                var card = DbRead.GetRfidCard(rfidCardId);
                RfidCardAction(rfidCardId, card, ReaderLocation.Kiosk);
            }
            if (Keyboard.IsKeyDown(Key.RightCtrl) && e.Key == Key.F4)
            {
                var rfidCardId = "4000000004";
                var card = DbRead.GetRfidCard(rfidCardId);
                RfidCardAction(rfidCardId, card, ReaderLocation.Huber);
            }
            if (Keyboard.IsKeyDown(Key.RightCtrl) && e.Key == Key.F5)
            {
                var rfidCardId = "4000000004";
                var card = DbRead.GetRfidCard(rfidCardId);
                RfidCardAction(rfidCardId, card, ReaderLocation.Rozdzielnia);
            }
        }

        private void LogError(Exception ex)
        {
            SimpleLogger sl = new SimpleLogger();
            sl.Error(ex.Message);
            sl.Error(ex.StackTrace);
        }
        private void LogDebug(string msg)
        {
            if (!_debugMode) return;

            SimpleLogger sl = new SimpleLogger();

            if (Transaction != null)
            {
                sl.Debug("Transakcja " + Transaction.TimeStamp + " | " + msg);
            }
            sl.Debug(msg);
        }
    }
}

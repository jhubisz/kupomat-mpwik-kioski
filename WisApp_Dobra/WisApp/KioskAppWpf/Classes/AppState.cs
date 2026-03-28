namespace KioskAppWpf.Classes
{
    class AppState
    {
        public bool TransactionInProgress { get; set; }

        public bool RfidGateInPresent { get; set; }
        public bool RfidGateOutPresent { get; set; }

        public bool RfidBarrierOneInOk { get; set; }
        public bool RfidBarrierOneOutOk { get; set; }
        public bool RfidBarrierTwoInOk { get; set; }
        public bool RfidBarrierTwoOutOk { get; set; }
        public bool RfidKioskOk { get; set; }

        public bool DbLocalOk { get; set; }
        public bool DbRemoteOk { get; set; }

        public bool OpcServerOk { get; set; }

        public bool PrinterOk { get; set; }
        public bool KeyboardOk { get; set; }
    }
}

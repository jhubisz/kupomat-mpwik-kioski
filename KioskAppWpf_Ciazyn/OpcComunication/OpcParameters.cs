using OpcCommunication;
using System.Configuration;

namespace OpcComunication
{
    public class OpcParameters
    {
        private bool _chztOn
        {
            get
            {
                return ConfigurationManager.AppSettings["_chztOn"] == "1";
            }
        }

        private bool OpcOn { get; set; }
        private OpcClient OpcClient;

        public bool KioskLifeBit { get; set; }

        #region Kiosk_to_PLC_1
        public bool ProbeScheduled { get; set; }
        public bool ProbePermit { get; set; }
        public bool WaterForMeasuringDevices { get; set; }
        public bool MonitoringPhOn { get; set; }
        public bool MonitoringTempOn { get; set; }

        public bool PaperEnd { get; set; }
        public bool PrinterOn { get; set; }

        public bool KioskOn { get; set; }

        public bool SeqNoFlowAck { get; set; }
        public bool SeqAck { get; set; }

        public bool MonitoringChztOn { get; set; }

        public bool ProbeBottlesErrorBlockage { get; set; }
        public bool ProbeErrorBlockage { get; set; }

        public bool MonitoringPressureOn { get; set; }
        public bool PressureBlockageOn { get; set; }

        public bool CarRegistrationNumberUnknown { get; set; }
        #endregion

        public short ProbeStart { get; set; }
        public short ProbeMin { get; set; }
        public short Probe1 { get; set; }
        public short Probe2 { get; set; }
        public short Probe3 { get; set; }
        public short DeclaredSewageAmount { get; set; }
        public short SeqHoldTime { get; set; }
        public short SeqNoFlowTime { get; set; }
        public short SeqNormalFlowEnd { get; set; }
        public short WaterForDriversTime { get; set; }
        public short WaterForMeasuringDevicesTime { get; set; }

        public short PressureFactor { get; set; }
        public short PressureMaxTime { get; set; }

        public bool KioskOpenState { get; set; }
        public bool ProbeMoveOn { get; set; }

        public OpcParameters(OpcClient client, bool opcOn)
        {
            OpcClient = client;
            OpcOn = opcOn;
        }

        public void UpdateOpcParameters()
        {
            if (!OpcOn) return;

            uint parameters = 0;
            if (ProbeScheduled) parameters = parameters | OpcClient.Configuration.MaskProbeSched;
            if (ProbePermit) parameters = parameters | OpcClient.Configuration.MaskProbePermit;
            if (WaterForMeasuringDevices) parameters = parameters | OpcClient.Configuration.MaskH2OMeter;
            if (MonitoringPhOn) parameters = parameters | OpcClient.Configuration.MaskpHOn;
            if (MonitoringTempOn) parameters = parameters | OpcClient.Configuration.MaskTempOn;

            if (PaperEnd) parameters = parameters | OpcClient.Configuration.MaskPaperEnd;
            if (PrinterOn) parameters = parameters | OpcClient.Configuration.MaskPrinterOn;

            if (KioskOn) parameters = parameters | OpcClient.Configuration.MaskKioskOn;

            if (SeqNoFlowAck) parameters = parameters | OpcClient.Configuration.MaskKioskSeqNoFlowAck;
            if (SeqAck) parameters = parameters | OpcClient.Configuration.MaskKioskSeqAck;

            if (_chztOn)
                if (MonitoringChztOn) parameters = parameters | OpcClient.Configuration.MaskChztOn;

            if (ProbeBottlesErrorBlockage) parameters = parameters | OpcClient.Configuration.MaskBottlesBlockage;
            if (ProbeErrorBlockage) parameters = parameters | OpcClient.Configuration.MaskProbeErrorBlockage;

            if (MonitoringPressureOn) parameters = parameters | OpcClient.Configuration.MaskPressureOn;
            if (PressureBlockageOn) parameters = parameters | OpcClient.Configuration.MaskPressureBlockageOn;

            if (CarRegistrationNumberUnknown) parameters = parameters | OpcClient.Configuration.MaskRfidRegNoRecUnknown;

            if (KioskOpenState) parameters = parameters | OpcClient.Configuration.MaskKioskOpenState;
            if (ProbeMoveOn) parameters = parameters | OpcClient.Configuration.MaskProbeMoveOn;

            OpcClient.WriteNewBlockValue(OpcClient.Configuration.VarAddressKioskToPlc1, parameters);

            OpcClient.WriteNewBlockValue(OpcClient.Configuration.VarAddressProbeStart, ProbeStart);
            OpcClient.WriteNewBlockValue(OpcClient.Configuration.VarAddressProbeMin, ProbeMin);
            OpcClient.WriteNewBlockValue(OpcClient.Configuration.VarAddressProbe1, Probe1);
            OpcClient.WriteNewBlockValue(OpcClient.Configuration.VarAddressProbe2, Probe2);
            OpcClient.WriteNewBlockValue(OpcClient.Configuration.VarAddressProbe3, Probe3);
            OpcClient.WriteNewBlockValue(OpcClient.Configuration.VarAddressDeclaredSewageAmount, DeclaredSewageAmount);
            OpcClient.WriteNewBlockValue(OpcClient.Configuration.VarAddressSeqHoldTime, SeqHoldTime);
            OpcClient.WriteNewBlockValue(OpcClient.Configuration.VarAddressSeqNoFlowTime, SeqNoFlowTime);
            OpcClient.WriteNewBlockValue(OpcClient.Configuration.VarAddressSeqNormalFlowEnd, SeqNormalFlowEnd);
            OpcClient.WriteNewBlockValue(OpcClient.Configuration.VarAddressWaterForMeasuringDevicesTime, WaterForMeasuringDevicesTime);

            OpcClient.WriteNewBlockValue(OpcClient.Configuration.VarAddressPressureFactor, PressureFactor);
            OpcClient.WriteNewBlockValue(OpcClient.Configuration.VarAddressPressureMaxTime, PressureMaxTime);
        }
    }
}

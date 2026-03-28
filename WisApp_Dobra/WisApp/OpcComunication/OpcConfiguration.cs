using System;
using System.Configuration;

namespace OpcCommunication
{
    public class OpcConfiguration : IOpcConfiguration
    {
        private bool _chztOn
        {
            get
            {
                return ConfigurationManager.AppSettings["_chztOn"] == "1";
            }
        }

        public string ServerAddress { get; private set; }
        public string NamespaceUri { get; private set; }

        public string VarAddressLifeBit { get; private set; }
        public string VarAddressKioskLifeBit { get; private set; }

        public string VarAddressTransactionLifeBit { get; private set; }
        public string VarAddressTransactionKioskLifeBit { get; private set; }

        public string VarAddressFlowCnt { get; private set; }

        public string VarAddressProbeStart { get; private set; }
        public string VarAddressProbeMin { get; private set; }
        public string VarAddressDeclaredSewageAmount { get; private set; }
        public string VarAddressProbe1 { get; private set; }
        public string VarAddressProbe2 { get; private set; }
        public string VarAddressProbe3 { get; private set; }
        public string VarAddressSeqHoldTime { get; private set; }
        public string VarAddressSeqNoFlowTime { get; private set; }
        public string VarAddressSeqNormalFlowEnd { get; private set; }
        public string VarAddressWaterForMeasuringDevicesTime { get; private set; }

        public string VarAddressPressureFactor { get; private set; }
        public string VarAddressPressureMaxTime { get; private set; }

        public string VarAddressProbeScheduledBottleNo { get; private set; }
        public string VarAddressProbeAlarmBottleNo { get; private set; }

        public string VarAddressDumpPath { get; private set; }
        public string VarAddressCameraRegistration { get; private set; }
        public string VarAddressParameterOverrun { get; private set; }

        public string VarAddressPlcToKiosk1 { get; private set; }
        public string VarAddressPlcToKiosk2 { get; private set; }
        public string VarAddressPlcToKiosk3 { get; private set; }

        public string VarAddressKioskToPlc1 { get; private set; }
        public string VarAddressKioskToPlc2 { get; private set; }

        #region PLC_To_Kiosk_1
        public uint Mask230VAC { get; private set; }
        public uint Mask24VDC { get; private set; }

        public uint MaskKioskDoorOpen { get; private set; }
        public uint MaskProbeDoorOpen { get; private set; }

        public uint MaskAirOk { get; private set; }
        public uint MaskFlowOk { get; private set; }
        public uint MaskPhMeterOk { get; private set; }
        public uint MaskTempMeterOk { get; private set; }
        public uint MaskProbeOk { get; private set; }
        public uint MaskProbeReady { get; private set; }

        public uint MaskDistribution230VACOk { get; private set; }
        public uint MaskDistribution230VACUpsOk { get; private set; }
        public uint MaskRack230VACOk { get; private set; }
        public uint MaskRack24VDCOk { get; private set; }
        public uint MaskRack24VDCUPSOk { get; private set; }
        public uint MaskRack12VDCOk { get; private set; }

        public uint MaskPumpRoomOk { get; private set; }
        public uint MaskPumpRoomWorking { get; private set; }
        public uint MaskScreenRoomLeak { get; private set; }
        public uint MaskScreenOk { get; private set; }
        public uint MaskScreenAuto { get; private set; }
        public uint MaskScreenFull { get; private set; }

        public uint MaskAugerOk { get; private set; }
        public uint MaskAugerAuto { get; private set; }

        public uint MaskPreScreenOk { get; private set; }
        public uint MaskPreScreenAuto { get; private set; }

        public uint MaskDumpPathOk { get; private set; }
        public uint MaskDumpPathAuto { get; private set; }

        public uint MaskGateOk { get; private set; }
        public uint MaskGateOpen { get; private set; }

        public uint MaskValveMeterOk { get; private set; }
        public uint MaskProbeResetting { get; private set; }
        #endregion

        #region PLC_To_Kiosk_2
        public uint MaskSeqRun { get; private set; }
        public uint MaskSeqIdle { get; private set; }
        public uint MaskSeqHeld { get; private set; }
        public uint MaskSeqFinish { get; private set; }
        
        public uint MaskSeqErrNoFlow { get; private set; }
        public uint MaskSeqErrDev { get; private set; }
        public uint MaskSeqErrAlarm { get; private set; }
        public uint MaskSeqErrAlarmProbe { get; private set; }
        public uint MaskSeqErrScreenFull { get; private set; }
        public uint MaskSeqErrKiosk { get; private set; }
        public uint MaskSeqNoFlowAck { get; private set; }
        public uint MaskSeqAck { get; private set; }

        public uint MaskProbeScheduledTaken { get; private set; }
        public uint MaskProbeAlarmTaken { get; private set; }

        public uint MaskKioskLightOn { get; private set; }

        public uint MaskWaterForMeasuringDevicesOn { get; private set; }

        public uint MaskSeqReady { get; private set; }
        public uint MaskSeqPermit { get; private set; }

        public uint MaskGate2Ok { get; private set; }
        public uint MaskGate2Open { get; private set; }

        public uint MaskPressureDumpError { get; private set; }
        public uint MaskPressurePumpOn { get; private set; }

        public uint MaskChztOk { get; private set; }

        public uint MaskGate1Auto { get; private set; }
        public uint MaskGate2Auto { get; private set; }

        public uint MaskZasuwaOk { get; private set; }

        public uint MaskPressureOk { get; private set; }
        #endregion

        #region Kiosk_To_PLC_1
        public uint MaskProbeSched { get; private set; }
        public uint MaskProbePermit { get; private set; }
        public uint MaskH2OMeter { get; private set; }
        public uint MaskpHOn { get; private set; }
        public uint MaskTempOn { get; private set; }

        public uint MaskPaperEnd { get; private set; }
        public uint MaskPrinterOn { get; private set; }

        public uint MaskKioskOn { get; private set; }
        public uint MaskKioskSeqNoFlowAck { get; private set; }
        public uint MaskKioskSeqAck { get; private set; }

        public uint MaskRfidGateInErr { get; private set; }
        public uint MaskRfidGateOutErr { get; private set; }
        public uint MaskRfidScreenRoomErr { get; private set; }
        public uint MaskRfidDistributionErr { get; private set; }
        public uint MaskRfidRegNoRecErr { get; private set; }
        public uint MaskRfidKioskErr { get; private set; }

        public uint MaskRfidGateInUnknown { get; private set; }
        public uint MaskRfidGateOutUnknown { get; private set; }
        public uint MaskRfidScreenRoomUnknown { get; private set; }
        public uint MaskRfidDistributionUnknown { get; private set; }
        public uint MaskRfidRegNoRecUnknown { get; private set; }

        public uint MaskChztOn { get; private set; }

        public uint MaskBottlesBlockage { get; private set; }
        public uint MaskProbeErrorBlockage { get; private set; }

        public uint MaskPressureOn { get; private set; }
        public uint MaskPressureBlockageOn { get; private set; }
        #endregion

        #region Kiosk_To_PLC_2
        public uint MaskSeqStart { get; private set; }
        public uint MaskSeqStop { get; private set; }
        public uint MaskSeqHold { get; private set; }

        public uint MaskLightOn { get; private set; }
        public uint MaskLightOff { get; private set; }

        public uint MaskResetSampler { get; private set; }

        public uint MaskDistributionOpen { get; private set; }
        public uint MaskScreenRoomOpen { get; private set; }
        public uint MaskGateOpenCommand { get; private set; }
        public uint MaskGateOutOpenCommand { get; private set; }

        public uint MaskToiletCommand { get; private set; }
        #endregion

        public OpcConfiguration()
        {
            PopulateAddresses();
            PopulateMasksPlcToKiosk1();
            PopulateMasksPlcToKiosk2();
            PopulateMasksKioskToPlc1();
            PopulateMasksKioskToPlc2();
        }

        private void PopulateAddresses()
        {
            ServerAddress = ConfigurationManager.AppSettings["opcServerAddress"];
            NamespaceUri = ConfigurationManager.AppSettings["opsNamespace"];

            VarAddressLifeBit = ConfigurationManager.AppSettings["varAddressLifeBit"];
            VarAddressKioskLifeBit = ConfigurationManager.AppSettings["varAddressKioskLifeBit"];

            VarAddressTransactionLifeBit = ConfigurationManager.AppSettings["VarAddressTransactionLifeBit"];
            VarAddressTransactionKioskLifeBit = ConfigurationManager.AppSettings["VarAddressTransactionKioskLifeBit"];

            VarAddressFlowCnt = ConfigurationManager.AppSettings["VarAddressFlowCnt"];

            VarAddressProbeStart = ConfigurationManager.AppSettings["varAddressProbeStart"];
            VarAddressProbeMin = ConfigurationManager.AppSettings["varAddressProbeMin"];
            VarAddressDeclaredSewageAmount = ConfigurationManager.AppSettings["varAddressDeclaredSewageAmount"];
            VarAddressProbe1 = ConfigurationManager.AppSettings["varAddressProbe1"];
            VarAddressProbe2 = ConfigurationManager.AppSettings["varAddressProbe2"];
            VarAddressProbe3 = ConfigurationManager.AppSettings["varAddressProbe3"];
            VarAddressSeqHoldTime = ConfigurationManager.AppSettings["varAddressSeqHoldTime"];
            VarAddressSeqNoFlowTime = ConfigurationManager.AppSettings["varAddressSeqNoFlowTime"];
            VarAddressSeqNormalFlowEnd = ConfigurationManager.AppSettings["varAddressSeqNormalFlowEnd"];
            VarAddressWaterForMeasuringDevicesTime = ConfigurationManager.AppSettings["varAddressWaterForMeasuringDevicesTime"];

            VarAddressPressureFactor = ConfigurationManager.AppSettings["VarAddressPressureFactor"];
            VarAddressPressureMaxTime = ConfigurationManager.AppSettings["VarAddressPressureMaxTime"];

            VarAddressProbeScheduledBottleNo = ConfigurationManager.AppSettings["VarAddressProbeScheduledBottleNo"];
            VarAddressProbeAlarmBottleNo = ConfigurationManager.AppSettings["VarAddressProbeAlarmBottleNo"];

            VarAddressDumpPath = ConfigurationManager.AppSettings["VarAddressDumpPath"];
            VarAddressCameraRegistration = ConfigurationManager.AppSettings["VarAddressCameraRegistration"];
            VarAddressParameterOverrun = ConfigurationManager.AppSettings["VarAddressParameterOverrun"];

            VarAddressPlcToKiosk1 = ConfigurationManager.AppSettings["VarAddressPlCToKiosk1"];
            VarAddressPlcToKiosk2 = ConfigurationManager.AppSettings["VarAddressPlCToKiosk2"];
            VarAddressPlcToKiosk3 = ConfigurationManager.AppSettings["VarAddressPlCToKiosk3"];

            VarAddressKioskToPlc1 = ConfigurationManager.AppSettings["VarAddressKioskToPlc1"];
            VarAddressKioskToPlc2 = ConfigurationManager.AppSettings["VarAddressKioskToPlc2"];
        }
        private void PopulateMasksPlcToKiosk1()
        {
            Mask230VAC = uint.Parse(ConfigurationManager.AppSettings["Mask230VAC"]);
            Mask24VDC = uint.Parse(ConfigurationManager.AppSettings["Mask24VDC"]);

            MaskKioskDoorOpen = uint.Parse(ConfigurationManager.AppSettings["MaskKioskDoorOpen"]);
            MaskProbeDoorOpen = uint.Parse(ConfigurationManager.AppSettings["MaskProbeDoorOpen"]);

            MaskAirOk = uint.Parse(ConfigurationManager.AppSettings["MaskAirOk"]);
            MaskFlowOk = uint.Parse(ConfigurationManager.AppSettings["MaskFlowOk"]);
            MaskPhMeterOk = uint.Parse(ConfigurationManager.AppSettings["MaskPhMeterOk"]);
            MaskTempMeterOk = uint.Parse(ConfigurationManager.AppSettings["MaskTempMeterOk"]);
            MaskProbeOk = uint.Parse(ConfigurationManager.AppSettings["MaskProbeOk"]);
            MaskProbeReady = uint.Parse(ConfigurationManager.AppSettings["MaskProbeReady"]);

            MaskDistribution230VACOk = uint.Parse(ConfigurationManager.AppSettings["MaskDistribution230VACOk"]);
            MaskDistribution230VACUpsOk = uint.Parse(ConfigurationManager.AppSettings["MaskDistribution230VACUpsOk"]);
            MaskRack230VACOk = uint.Parse(ConfigurationManager.AppSettings["MaskRack230VACOk"]);
            MaskRack24VDCOk = uint.Parse(ConfigurationManager.AppSettings["MaskRack24VDCOk"]);
            MaskRack24VDCUPSOk = uint.Parse(ConfigurationManager.AppSettings["MaskRack24VDCUPSOk"]);
            MaskRack12VDCOk = uint.Parse(ConfigurationManager.AppSettings["MaskRack12VDCOk"]);

            MaskPumpRoomOk = uint.Parse(ConfigurationManager.AppSettings["MaskPumpRoomOk"]);
            MaskPumpRoomWorking = uint.Parse(ConfigurationManager.AppSettings["MaskPumpRoomWorking"]);
            MaskScreenRoomLeak = uint.Parse(ConfigurationManager.AppSettings["MaskScreenRoomLeak"]);
            MaskScreenOk = uint.Parse(ConfigurationManager.AppSettings["MaskScreenOk"]);
            MaskScreenAuto = uint.Parse(ConfigurationManager.AppSettings["MaskScreenAuto"]);
            MaskScreenFull = uint.Parse(ConfigurationManager.AppSettings["MaskScreenFull"]);

            MaskAugerOk = uint.Parse(ConfigurationManager.AppSettings["MaskAugerOk"]);
            MaskAugerAuto = uint.Parse(ConfigurationManager.AppSettings["MaskAugerAuto"]);

            MaskPreScreenOk = uint.Parse(ConfigurationManager.AppSettings["MaskPreScreenOk"]);
            MaskPreScreenAuto = uint.Parse(ConfigurationManager.AppSettings["MaskPreScreenAuto"]);

            MaskDumpPathOk = uint.Parse(ConfigurationManager.AppSettings["MaskDumpPathOk"]);
            MaskDumpPathAuto = uint.Parse(ConfigurationManager.AppSettings["MaskDumpPathAuto"]);

            MaskGateOk = uint.Parse(ConfigurationManager.AppSettings["MaskGateOk"]);
            MaskGateOpen = uint.Parse(ConfigurationManager.AppSettings["MaskGateOpen"]);

            MaskValveMeterOk = uint.Parse(ConfigurationManager.AppSettings["MaskValveMeterOk"]);
            MaskProbeResetting = uint.Parse(ConfigurationManager.AppSettings["MaskProbeResetting"]);
        }
        private void PopulateMasksPlcToKiosk2()
        {
            MaskSeqRun = uint.Parse(ConfigurationManager.AppSettings["MaskSeqRun"]);
            MaskSeqIdle = uint.Parse(ConfigurationManager.AppSettings["MaskSeqIdle"]);
            MaskSeqHeld = uint.Parse(ConfigurationManager.AppSettings["MaskSeqHeld"]);
            MaskSeqFinish = uint.Parse(ConfigurationManager.AppSettings["MaskSeqFinish"]);

            MaskSeqErrNoFlow = uint.Parse(ConfigurationManager.AppSettings["MaskSeqErrNoFlow"]);
            MaskSeqErrDev = uint.Parse(ConfigurationManager.AppSettings["MaskSeqErrDev"]);
            MaskSeqErrAlarm = uint.Parse(ConfigurationManager.AppSettings["MaskSeqErrAlarm"]);
            MaskSeqErrAlarmProbe = uint.Parse(ConfigurationManager.AppSettings["MaskSeqErrAlarmProbe"]);
            MaskSeqErrScreenFull = uint.Parse(ConfigurationManager.AppSettings["MaskSeqErrScreenFull"]);
            MaskSeqErrKiosk = uint.Parse(ConfigurationManager.AppSettings["MaskSeqErrKiosk"]);
            MaskSeqNoFlowAck = uint.Parse(ConfigurationManager.AppSettings["MaskSeqNoFlowAck"]);
            MaskSeqAck = uint.Parse(ConfigurationManager.AppSettings["MaskSeqAck"]);

            MaskProbeScheduledTaken = uint.Parse(ConfigurationManager.AppSettings["MaskProbeScheduledTaken"]);
            MaskProbeAlarmTaken = uint.Parse(ConfigurationManager.AppSettings["MaskProbeAlarmTaken"]);

            MaskKioskLightOn = uint.Parse(ConfigurationManager.AppSettings["MaskKioskLightOn"]);

            MaskWaterForMeasuringDevicesOn = uint.Parse(ConfigurationManager.AppSettings["MaskWaterForMeasuringDevicesOn"]);

            MaskSeqReady = uint.Parse(ConfigurationManager.AppSettings["MaskSeqReady"]);
            MaskSeqPermit = uint.Parse(ConfigurationManager.AppSettings["MaskSeqPermit"]);

            MaskGate2Ok = uint.Parse(ConfigurationManager.AppSettings["MaskGate2Ok"]);
            MaskGate2Open = uint.Parse(ConfigurationManager.AppSettings["MaskGate2Open"]);

            MaskPressureDumpError = uint.Parse(ConfigurationManager.AppSettings["MaskPressureDumpError"]);
            MaskPressurePumpOn = uint.Parse(ConfigurationManager.AppSettings["MaskPressurePumpOn"]);

            MaskChztOk = uint.Parse(ConfigurationManager.AppSettings["MaskChztOk"]);

            MaskGate1Auto = uint.Parse(ConfigurationManager.AppSettings["MaskGate1Auto"]);
            MaskGate2Auto = uint.Parse(ConfigurationManager.AppSettings["MaskGate2Auto"]);

            MaskZasuwaOk = uint.Parse(ConfigurationManager.AppSettings["MaskZasuwaOk"]);

            MaskPressureOk = uint.Parse(ConfigurationManager.AppSettings["MaskPressureOk"]);
        }

        private void PopulateMasksKioskToPlc1()
        {
            MaskProbeSched = uint.Parse(ConfigurationManager.AppSettings["MaskProbeSched"]);
            MaskProbePermit = uint.Parse(ConfigurationManager.AppSettings["MaskProbePermit"]);
            MaskH2OMeter = uint.Parse(ConfigurationManager.AppSettings["MaskH2OMeter"]);
            MaskpHOn = uint.Parse(ConfigurationManager.AppSettings["MaskpHOn"]);
            MaskTempOn = uint.Parse(ConfigurationManager.AppSettings["MaskTempOn"]);

            MaskPaperEnd = uint.Parse(ConfigurationManager.AppSettings["MaskPaperEnd"]);
            MaskPrinterOn = uint.Parse(ConfigurationManager.AppSettings["MaskPrinterOn"]);

            MaskKioskOn = uint.Parse(ConfigurationManager.AppSettings["MaskKioskOn"]);
            MaskKioskSeqNoFlowAck = uint.Parse(ConfigurationManager.AppSettings["MaskKioskSeqNoFlowAck"]);
            MaskKioskSeqAck = uint.Parse(ConfigurationManager.AppSettings["MaskKioskSeqAck"]);

            MaskRfidGateInErr = uint.Parse(ConfigurationManager.AppSettings["MaskRfidGateInErr"]);
            MaskRfidGateOutErr = uint.Parse(ConfigurationManager.AppSettings["MaskRfidGateOutErr"]);
            MaskRfidScreenRoomErr = uint.Parse(ConfigurationManager.AppSettings["MaskRfidScreenRoomErr"]);
            MaskRfidDistributionErr = uint.Parse(ConfigurationManager.AppSettings["MaskRfidDistributionErr"]);
            MaskRfidRegNoRecErr = uint.Parse(ConfigurationManager.AppSettings["MaskRfidRegNoRecErr"]);
            MaskRfidKioskErr = uint.Parse(ConfigurationManager.AppSettings["MaskRfidKioskErr"]);

            MaskRfidGateInUnknown = uint.Parse(ConfigurationManager.AppSettings["MaskRfidGateInUnknown"]);
            MaskRfidGateOutUnknown = uint.Parse(ConfigurationManager.AppSettings["MaskRfidGateOutUnknown"]);
            MaskRfidScreenRoomUnknown = uint.Parse(ConfigurationManager.AppSettings["MaskRfidScreenRoomUnknown"]);
            MaskRfidDistributionUnknown = uint.Parse(ConfigurationManager.AppSettings["MaskRfidDistributionUnknown"]);
            MaskRfidRegNoRecUnknown = uint.Parse(ConfigurationManager.AppSettings["MaskRfidRegNoRecUnknown"]);

            if (_chztOn)
            {
                MaskChztOn = uint.Parse(ConfigurationManager.AppSettings["MaskChztOn"]);
            }

            MaskBottlesBlockage = uint.Parse(ConfigurationManager.AppSettings["MaskBottlesBlockage"]);
            MaskProbeErrorBlockage = uint.Parse(ConfigurationManager.AppSettings["MaskProbeErrorBlockage"]);

            MaskPressureOn = uint.Parse(ConfigurationManager.AppSettings["MaskPressureOn"]);
            MaskPressureBlockageOn = uint.Parse(ConfigurationManager.AppSettings["MaskPressureBlockageOn"]);
        }
        private void PopulateMasksKioskToPlc2()
        {
            MaskSeqStart = uint.Parse(ConfigurationManager.AppSettings["MaskSeqStart"]);
            MaskSeqStop = uint.Parse(ConfigurationManager.AppSettings["MaskSeqStop"]);
            MaskSeqHold = uint.Parse(ConfigurationManager.AppSettings["MaskSeqHold"]);

            MaskLightOn = uint.Parse(ConfigurationManager.AppSettings["MaskLightOn"]);
            MaskLightOff = uint.Parse(ConfigurationManager.AppSettings["MaskLightOff"]);

            MaskResetSampler = uint.Parse(ConfigurationManager.AppSettings["MaskResetSampler"]);

            MaskDistributionOpen = uint.Parse(ConfigurationManager.AppSettings["MaskDistributionOpen"]);
            MaskScreenRoomOpen = uint.Parse(ConfigurationManager.AppSettings["MaskScreenRoomOpen"]);
            MaskGateOpenCommand = uint.Parse(ConfigurationManager.AppSettings["MaskGateOpenCommand"]);
            MaskGateOutOpenCommand = uint.Parse(ConfigurationManager.AppSettings["MaskGateOutOpenCommand"]);

            MaskToiletCommand = uint.Parse(ConfigurationManager.AppSettings["MaskToiletCommand"]);
        }
    }
}

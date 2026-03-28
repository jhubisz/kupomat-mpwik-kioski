using System.Collections.Generic;
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
        public string VarAddressCameraRegistration2 { get; private set; }
        public string VarAddressParameterOverrun { get; private set; }

        public string VarAddressPlcToKiosk1 { get; private set; }
        public string VarAddressPlcToKiosk2 { get; private set; }
        public string VarAddressPlcToKiosk3 { get; private set; }
        
        public string VarAddressPlcToKioskPermitCode { get; private set; }
        public string VarAddressPlcToKioskReadyCode { get; private set; }
        public string VarAddressPlcToKioskStopCode { get; private set; }
        public string VarAddressPlcToKioskMeasCode { get; private set; }

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

        public uint MaskPressureDumpError { get; private set; }
        public uint MaskPressurePumpOn { get; private set; }

        public uint MaskChztOk { get; private set; }

        public uint MaskGate1Auto { get; private set; }
        public uint MaskGate2Auto { get; private set; }

        public uint MaskZasuwaOk { get; private set; }

        public uint MaskPressureOk { get; private set; }
        #endregion
        
        public List<OpcConfigurationEntry<Enums.PermitCode>> PlcToKioskPermitCode { get; private set; }
        public List<OpcConfigurationEntry<Enums.ReadyCode>> PlcToKioskReadyCode { get; private set; }
        public List<OpcConfigurationEntry<Enums.StopCode>> PlcToKioskStopCode { get; private set; }
        public List<OpcConfigurationEntry<Enums.MeasCode>> PlcToKioskMeasCode { get; private set; }

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

        public uint MaskKioskOpenState { get; private set; }
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

            PopulateMasksPlcToKioskPermitCode();
            PopulateMasksPlcToKioskReadyCode();
            PopulateMasksPlcToKioskStopCode();
            PopulateMasksPlcToKioskMeasCode();

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

            VarAddressProbeStart = ConfigurationManager.AppSettings["VarAddressProbeStart"];
            VarAddressProbeMin = ConfigurationManager.AppSettings["VarAddressProbeMin"];
            VarAddressDeclaredSewageAmount = ConfigurationManager.AppSettings["VarAddressDeclaredSewageAmount"];
            VarAddressProbe1 = ConfigurationManager.AppSettings["VarAddressProbe1"];
            VarAddressProbe2 = ConfigurationManager.AppSettings["VarAddressProbe2"];
            VarAddressProbe3 = ConfigurationManager.AppSettings["VarAddressProbe3"];
            VarAddressSeqHoldTime = ConfigurationManager.AppSettings["VarAddressSeqHoldTime"];
            VarAddressSeqNoFlowTime = ConfigurationManager.AppSettings["VarAddressSeqNoFlowTime"];
            VarAddressSeqNormalFlowEnd = ConfigurationManager.AppSettings["VarAddressSeqNormalFlowEnd"];
            VarAddressWaterForMeasuringDevicesTime = ConfigurationManager.AppSettings["VarAddressWaterForMeasuringDevicesTime"];

            VarAddressPressureFactor = ConfigurationManager.AppSettings["VarAddressPressureFactor"];
            VarAddressPressureMaxTime = ConfigurationManager.AppSettings["VarAddressPressureMaxTime"];

            VarAddressProbeScheduledBottleNo = ConfigurationManager.AppSettings["VarAddressProbeScheduledBottleNo"];
            VarAddressProbeAlarmBottleNo = ConfigurationManager.AppSettings["VarAddressProbeAlarmBottleNo"];

            VarAddressDumpPath = ConfigurationManager.AppSettings["VarAddressDumpPath"];
            VarAddressCameraRegistration = ConfigurationManager.AppSettings["VarAddressCameraRegistration"];
            VarAddressCameraRegistration2 = ConfigurationManager.AppSettings["VarAddressCameraRegistration2"];
            VarAddressParameterOverrun = ConfigurationManager.AppSettings["VarAddressParameterOverrun"];

            VarAddressPlcToKiosk1 = ConfigurationManager.AppSettings["VarAddressPlCToKiosk1"];
            VarAddressPlcToKiosk2 = ConfigurationManager.AppSettings["VarAddressPlCToKiosk2"];
            VarAddressPlcToKiosk3 = ConfigurationManager.AppSettings["VarAddressPlCToKiosk3"];

            VarAddressPlcToKioskPermitCode = ConfigurationManager.AppSettings["VarAddressPlCToKioskPermitCode"];
            VarAddressPlcToKioskReadyCode = ConfigurationManager.AppSettings["VarAddressPlCToKioskReadyCode"];
            VarAddressPlcToKioskStopCode = ConfigurationManager.AppSettings["VarAddressPlCToKioskStopCode"];
            VarAddressPlcToKioskMeasCode = ConfigurationManager.AppSettings["VarAddressPlCToKioskMeasCode"];

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

            MaskPressureDumpError = uint.Parse(ConfigurationManager.AppSettings["MaskPressureDumpError"]);
            MaskPressurePumpOn = uint.Parse(ConfigurationManager.AppSettings["MaskPressurePumpOn"]);

            MaskChztOk = uint.Parse(ConfigurationManager.AppSettings["MaskChztOk"]);

            MaskGate1Auto = uint.Parse(ConfigurationManager.AppSettings["MaskGate1Auto"]);
            MaskGate2Auto = uint.Parse(ConfigurationManager.AppSettings["MaskGate2Auto"]);

            MaskZasuwaOk = uint.Parse(ConfigurationManager.AppSettings["MaskZasuwaOk"]);

            MaskPressureOk = uint.Parse(ConfigurationManager.AppSettings["MaskPressureOk"]);
        }

        private void PopulateMasksPlcToKioskPermitCode()
        {
            PlcToKioskPermitCode = new List<OpcConfigurationEntry<Enums.PermitCode>>();
            PlcToKioskPermitCode.Add(new OpcConfigurationEntry<Enums.PermitCode>
            {
                Type = Enums.PermitCode.PhMeasureErr,
                Mask = GetAppSettingAsUInt("MaskPermitCodePhMeasureErr"),
                Msg = ConfigurationManager.AppSettings["MsgPermitCodePhMeasureErr"],
            });
            PlcToKioskPermitCode.Add(new OpcConfigurationEntry<Enums.PermitCode>
            {
                Type = Enums.PermitCode.FlowErr,
                Mask = GetAppSettingAsUInt("MaskPermitCodeFlowErr"),
                Msg = ConfigurationManager.AppSettings["MsgPermitCodeFlowErr"],
            });
            PlcToKioskPermitCode.Add(new OpcConfigurationEntry<Enums.PermitCode>
            {
                Type = Enums.PermitCode.TempMeasureErr,
                Mask = GetAppSettingAsUInt("MaskPermitCodeTempMeasureErr"),
                Msg = ConfigurationManager.AppSettings["MsgPermitCodeTempMeasureErr"],
            });
            PlcToKioskPermitCode.Add(new OpcConfigurationEntry<Enums.PermitCode>
            {
                Type = Enums.PermitCode.HuberZSPErr,
                Mask = GetAppSettingAsUInt("MaskPermitCodeHuberZSPErr"),
                Msg = ConfigurationManager.AppSettings["MsgPermitCodeHuberZSPErr"],
            });

            PlcToKioskPermitCode.Add(new OpcConfigurationEntry<Enums.PermitCode>
            {
                Type = Enums.PermitCode.SepErr,
                Mask = GetAppSettingAsUInt("MaskPermitCodeSepErr"),
                Msg = ConfigurationManager.AppSettings["MsgPermitCodeSepErr"],
            });
            PlcToKioskPermitCode.Add(new OpcConfigurationEntry<Enums.PermitCode>
            {
                Type = Enums.PermitCode.PathErr,
                Mask = GetAppSettingAsUInt("MaskPermitCodePathErr"),
                Msg = ConfigurationManager.AppSettings["MsgPermitCodePathErr"],
            });
            PlcToKioskPermitCode.Add(new OpcConfigurationEntry<Enums.PermitCode>
            {
                Type = Enums.PermitCode.ChztMeasureErr,
                Mask = GetAppSettingAsUInt("MaskPermitCodeChztMeasureErr"),
                Msg = ConfigurationManager.AppSettings["MsgPermitCodeChztMeasureErr"],
            });
            PlcToKioskPermitCode.Add(new OpcConfigurationEntry<Enums.PermitCode>
            {
                Type = Enums.PermitCode.PressureMeasureErr,
                Mask = GetAppSettingAsUInt("MaskPermitCodePressureMeasureErr"),
                Msg = ConfigurationManager.AppSettings["MsgPermitCodePressureMeasureErr"],
            });

            PlcToKioskPermitCode.Add(new OpcConfigurationEntry<Enums.PermitCode>
            {
                Type = Enums.PermitCode.VDC24Err,
                Mask = GetAppSettingAsUInt("MaskPermitCode24VDCErr"),
                Msg = ConfigurationManager.AppSettings["MsgPermitCode24VDCErr"],
            });
            PlcToKioskPermitCode.Add(new OpcConfigurationEntry<Enums.PermitCode>
            {
                Type = Enums.PermitCode.VAC230Err,
                Mask = GetAppSettingAsUInt("MaskPermitCode230VACErr"),
                Msg = ConfigurationManager.AppSettings["MsgPermitCode230VACErr"],
            });

            PlcToKioskPermitCode.Add(new OpcConfigurationEntry<Enums.PermitCode>
            {
                Type = Enums.PermitCode.KioskDoorOpen,
                Mask = GetAppSettingAsUInt("MaskPermitCodeKioskDoorOpen"),
                Msg = ConfigurationManager.AppSettings["MsgPermitCodeKioskDoorOpen"],
            });
            PlcToKioskPermitCode.Add(new OpcConfigurationEntry<Enums.PermitCode>
            {
                Type = Enums.PermitCode.AirErr,
                Mask = GetAppSettingAsUInt("MaskPermitCodeAirErr"),
                Msg = ConfigurationManager.AppSettings["MsgPermitCodeAirErr"],
            });
            PlcToKioskPermitCode.Add(new OpcConfigurationEntry<Enums.PermitCode>
            {
                Type = Enums.PermitCode.DumpLockNoAuto,
                Mask = GetAppSettingAsUInt("MaskPermitCodeDumpLockNoAuto"),
                Msg = ConfigurationManager.AppSettings["MsgPermitCodeDumpLockNoAuto"],
            });
            PlcToKioskPermitCode.Add(new OpcConfigurationEntry<Enums.PermitCode>
            {
                Type = Enums.PermitCode.DumpLockErr,
                Mask = GetAppSettingAsUInt("MaskPermitCodeDumpLockErr"),
                Msg = ConfigurationManager.AppSettings["MsgPermitCodeDumpLockErr"],
            });
            PlcToKioskPermitCode.Add(new OpcConfigurationEntry<Enums.PermitCode>
            {
                Type = Enums.PermitCode.AutoFlushErr,
                Mask = GetAppSettingAsUInt("MaskPermitCodeAutoFlushErr"),
                Msg = ConfigurationManager.AppSettings["MsgPermitCodeAutoFlushErr"],
            });
            PlcToKioskPermitCode.Add(new OpcConfigurationEntry<Enums.PermitCode>
            {
                Type = Enums.PermitCode.FlushLockErr,
                Mask = GetAppSettingAsUInt("MaskPermitCodeFlushLockErr"),
                Msg = ConfigurationManager.AppSettings["MsgPermitCodeFlushLockErr"],
            });

            PlcToKioskPermitCode.Add(new OpcConfigurationEntry<Enums.PermitCode>
            {
                Type = Enums.PermitCode.S7Kiosk,
                Mask = GetAppSettingAsUInt("MaskPermitCodeS7Kiosk"),
                Msg = ConfigurationManager.AppSettings["MsgPermitCodeS7Kiosk"],
            });
            PlcToKioskPermitCode.Add(new OpcConfigurationEntry<Enums.PermitCode>
            {
                Type = Enums.PermitCode.S7PCS,
                Mask = GetAppSettingAsUInt("MaskPermitCodeS7PCS"),
                Msg = ConfigurationManager.AppSettings["MsgPermitCodeS7PCS"],
            });

            PlcToKioskPermitCode.Add(new OpcConfigurationEntry<Enums.PermitCode>
            {
                Type = Enums.PermitCode.ProbeErr,
                Mask = GetAppSettingAsUInt("MaskPermitCodeProbeErr"),
                Msg = ConfigurationManager.AppSettings["MsgPermitCodeProbeErr"],
            });
            PlcToKioskPermitCode.Add(new OpcConfigurationEntry<Enums.PermitCode>
            {
                Type = Enums.PermitCode.ProbeNoBottle,
                Mask = GetAppSettingAsUInt("MaskPermitCodeProbeNoBottle"),
                Msg = ConfigurationManager.AppSettings["MsgPermitCodeProbeNoBottle"],
            });
            PlcToKioskPermitCode.Add(new OpcConfigurationEntry<Enums.PermitCode>
            {
                Type = Enums.PermitCode.ProbeNoAuto,
                Mask = GetAppSettingAsUInt("MaskPermitCodeProbeNoAuto"),
                Msg = ConfigurationManager.AppSettings["MsgPermitCodeProbeNoAuto"],
            });

            PlcToKioskPermitCode.Add(new OpcConfigurationEntry<Enums.PermitCode>
            {
                Type = Enums.PermitCode.Canal1Floode,
                Mask = GetAppSettingAsUInt("MaskPermitCodeCanal1Flooded"),
                Msg = ConfigurationManager.AppSettings["MsgPermitCodeCanal1Flooded"],
            });
            PlcToKioskPermitCode.Add(new OpcConfigurationEntry<Enums.PermitCode>
            {
                Type = Enums.PermitCode.Canal2Flooded,
                Mask = GetAppSettingAsUInt("MaskPermitCodeCanal2Flooded"),
                Msg = ConfigurationManager.AppSettings["MsgPermitCodeCanal2Flooded"],
            });

            PlcToKioskPermitCode.Add(new OpcConfigurationEntry<Enums.PermitCode>
            {
                Type = Enums.PermitCode.PCSVoltageProtErr,
                Mask = GetAppSettingAsUInt("MaskPermitCodePCSVoltageProtErr"),
                Msg = ConfigurationManager.AppSettings["MsgPermitCodePCSVoltageProtErr"],
            });
            PlcToKioskPermitCode.Add(new OpcConfigurationEntry<Enums.PermitCode>
            {
                Type = Enums.PermitCode.PCS230VACErr,
                Mask = GetAppSettingAsUInt("MaskPermitCodePCS230VACErr"),
                Msg = ConfigurationManager.AppSettings["MsgPermitCodePCS230VACErr"],
            });
            PlcToKioskPermitCode.Add(new OpcConfigurationEntry<Enums.PermitCode>
            {
                Type = Enums.PermitCode.PCS24VDCErr,
                Mask = GetAppSettingAsUInt("MaskPermitCodePCS24VDCErr"),
                Msg = ConfigurationManager.AppSettings["MsgPermitCodePCS24VDCErr"],
            });
            PlcToKioskPermitCode.Add(new OpcConfigurationEntry<Enums.PermitCode>
            {
                Type = Enums.PermitCode.PCS12VDCErr,
                Mask = GetAppSettingAsUInt("MaskPermitCodePCS12VDCErr"),
                Msg = ConfigurationManager.AppSettings["MsgPermitCodePCS12VDCErr"],
            });
            PlcToKioskPermitCode.Add(new OpcConfigurationEntry<Enums.PermitCode>
            {
                Type = Enums.PermitCode.PCS400VACErr,
                Mask = GetAppSettingAsUInt("MaskPermitCodePCS400VACErr"),
                Msg = ConfigurationManager.AppSettings["MsgPermitCodePCS400VACErr"],
            });
        }
        private void PopulateMasksPlcToKioskReadyCode()
        {
            PlcToKioskReadyCode = new List<OpcConfigurationEntry<Enums.ReadyCode>>();
            PlcToKioskReadyCode.Add(new OpcConfigurationEntry<Enums.ReadyCode>
            {
                Type = Enums.ReadyCode.ChztClean,
                Mask = GetAppSettingAsUInt("MaskReadyCodeChztClean"),
                Msg = ConfigurationManager.AppSettings["MsgReadyCodeChztClean"],
            });
            PlcToKioskReadyCode.Add(new OpcConfigurationEntry<Enums.ReadyCode>
            {
                Type = Enums.ReadyCode.ZSPMax,
                Mask = GetAppSettingAsUInt("MaskReadyCodeZSPMax"),
                Msg = ConfigurationManager.AppSettings["MsgReadyCodeZSPMax"],
            });
            PlcToKioskReadyCode.Add(new OpcConfigurationEntry<Enums.ReadyCode>
            {
                Type = Enums.ReadyCode.ProbeRunning,
                Mask = GetAppSettingAsUInt("MaskReadyCodeProbeRunning"),
                Msg = ConfigurationManager.AppSettings["MsgReadyCodeProbeRunning"],
            });
            PlcToKioskReadyCode.Add(new OpcConfigurationEntry<Enums.ReadyCode>
            {
                Type = Enums.ReadyCode.ProbeNoBottles,
                Mask = GetAppSettingAsUInt("MaskReadyCodeProbeNoBottles"),
                Msg = ConfigurationManager.AppSettings["MsgReadyCodeProbeNoBottles"],
            });
            PlcToKioskReadyCode.Add(new OpcConfigurationEntry<Enums.ReadyCode>
            {
                Type = Enums.ReadyCode.ProbeErr,
                Mask = GetAppSettingAsUInt("MaskReadyCodeProbeErr"),
                Msg = ConfigurationManager.AppSettings["MsgReadyCodeProbeErr"],
            });
            PlcToKioskReadyCode.Add(new OpcConfigurationEntry<Enums.ReadyCode>
            {
                Type = Enums.ReadyCode.ProbeNoProg,
                Mask = GetAppSettingAsUInt("MaskReadyCodeProbeNoProg"),
                Msg = ConfigurationManager.AppSettings["MsgReadyCodeProbeNoProg"],
            });
        }
        private void PopulateMasksPlcToKioskStopCode()
        {
            PlcToKioskStopCode = new List<OpcConfigurationEntry<Enums.StopCode>>();
            PlcToKioskStopCode.Add(new OpcConfigurationEntry<Enums.StopCode>
            {
                Type = Enums.StopCode.SeparatorErr,
                Mask = GetAppSettingAsUInt("MaskStopCodeSeparatorErr"),
                Msg = ConfigurationManager.AppSettings["MsgStopCodeSeparatorErr"],
            });
            PlcToKioskStopCode.Add(new OpcConfigurationEntry<Enums.StopCode>
            {
                Type = Enums.StopCode.CanalFloodedErr,
                Mask = GetAppSettingAsUInt("MaskStopCodeCanalFloodedErr"),
                Msg = ConfigurationManager.AppSettings["MsgStopCodeCanalFloodedErr"],
            });

            PlcToKioskStopCode.Add(new OpcConfigurationEntry<Enums.StopCode>
            {
                Type = Enums.StopCode.PHMeasureErr,
                Mask = GetAppSettingAsUInt("MaskStopCodePHMeasureErr"),
                Msg = ConfigurationManager.AppSettings["MsgStopCodePHMeasureErr"],
            });
            PlcToKioskStopCode.Add(new OpcConfigurationEntry<Enums.StopCode>
            {
                Type = Enums.StopCode.TempMeasureErr,
                Mask = GetAppSettingAsUInt("MaskStopCodeTempMeasureErr"),
                Msg = ConfigurationManager.AppSettings["MsgStopCodeTempMeasureErr"],
            });
            PlcToKioskStopCode.Add(new OpcConfigurationEntry<Enums.StopCode>
            {
                Type = Enums.StopCode.ChztErr,
                Mask = GetAppSettingAsUInt("MaskStopCodeChztErr"),
                Msg = ConfigurationManager.AppSettings["MsgStopCodeChztErr"],
            });
            PlcToKioskStopCode.Add(new OpcConfigurationEntry<Enums.StopCode>
            {
                Type = Enums.StopCode.PressureErr,
                Mask = GetAppSettingAsUInt("MaskStopCodePressureErr"),
                Msg = ConfigurationManager.AppSettings["MsgStopCodePressureErr"],
            });
            PlcToKioskStopCode.Add(new OpcConfigurationEntry<Enums.StopCode>
            {
                Type = Enums.StopCode.FlowErr,
                Mask = GetAppSettingAsUInt("MaskStopCodeFlowErr"),
                Msg = ConfigurationManager.AppSettings["MsgStopCodeFlowErr"],
            });

            PlcToKioskStopCode.Add(new OpcConfigurationEntry<Enums.StopCode>
            {
                Type = Enums.StopCode.ProbeWashErr,
                Mask = GetAppSettingAsUInt("MaskStopCodeProbeWashErr"),
                Msg = ConfigurationManager.AppSettings["MsgStopCodeProbeWashErr"],
            });
            PlcToKioskStopCode.Add(new OpcConfigurationEntry<Enums.StopCode>
            {
                Type = Enums.StopCode.ProbeWashNoAuto,
                Mask = GetAppSettingAsUInt("MaskStopCodeProbeWashNoAuto"),
                Msg = ConfigurationManager.AppSettings["MsgStopCodeProbeWashNoAuto"],
            });

            PlcToKioskStopCode.Add(new OpcConfigurationEntry<Enums.StopCode>
            {
                Type = Enums.StopCode.LockErr,
                Mask = GetAppSettingAsUInt("MaskStopCodeLockErr"),
                Msg = ConfigurationManager.AppSettings["MsgStopCodeLockErr"],
            });
            PlcToKioskStopCode.Add(new OpcConfigurationEntry<Enums.StopCode>
            {
                Type = Enums.StopCode.LockNoAuto,
                Mask = GetAppSettingAsUInt("MaskStopCodeLockNoAuto"),
                Msg = ConfigurationManager.AppSettings["MsgStopCodeLockNoAuto"],
            });

            PlcToKioskStopCode.Add(new OpcConfigurationEntry<Enums.StopCode>
            {
                Type = Enums.StopCode.VDC24Err,
                Mask = GetAppSettingAsUInt("MaskStop24VDCErr"),
                Msg = ConfigurationManager.AppSettings["MsgStop24VDCErr"],
            });
            PlcToKioskStopCode.Add(new OpcConfigurationEntry<Enums.StopCode>
            {
                Type = Enums.StopCode.VAC230Err,
                Mask = GetAppSettingAsUInt("MaskStop230VACErr"),
                Msg = ConfigurationManager.AppSettings["MsgStop230VACErr"],
            });
            PlcToKioskStopCode.Add(new OpcConfigurationEntry<Enums.StopCode>
            {
                Type = Enums.StopCode.AirErr,
                Mask = GetAppSettingAsUInt("MaskStopAirErr"),
                Msg = ConfigurationManager.AppSettings["MsgStopAirErr"],
            });
            PlcToKioskStopCode.Add(new OpcConfigurationEntry<Enums.StopCode>
            {
                Type = Enums.StopCode.KioskOpen,
                Mask = GetAppSettingAsUInt("MaskStopKioskOpen"),
                Msg = ConfigurationManager.AppSettings["MsgStopKioskOpen"],
            });

            PlcToKioskStopCode.Add(new OpcConfigurationEntry<Enums.StopCode>
            {
                Type = Enums.StopCode.PathNotOpen,
                Mask = GetAppSettingAsUInt("MaskStopPathNotOpen"),
                Msg = ConfigurationManager.AppSettings["MsgStopPathNotOpen"],
            });
            PlcToKioskStopCode.Add(new OpcConfigurationEntry<Enums.StopCode>
            {
                Type = Enums.StopCode.S7PCSCommErr,
                Mask = GetAppSettingAsUInt("MaskStopCodeS7PCSCommErr"),
                Msg = ConfigurationManager.AppSettings["MsgStopCodeS7PCSCommErr"],
            });
            PlcToKioskStopCode.Add(new OpcConfigurationEntry<Enums.StopCode>
            {
                Type = Enums.StopCode.PathNotOpen2,
                Mask = GetAppSettingAsUInt("MaskStopPathNotOpen2"),
                Msg = ConfigurationManager.AppSettings["MsgStopPathNotOpen2"],
            });

            PlcToKioskStopCode.Add(new OpcConfigurationEntry<Enums.StopCode>
            {
                Type = Enums.StopCode.ProbeErr,
                Mask = GetAppSettingAsUInt("MaskStopCodeProbeErr"),
                Msg = ConfigurationManager.AppSettings["MsgStopCodeProbeErr"],
            });
            PlcToKioskStopCode.Add(new OpcConfigurationEntry<Enums.StopCode>
            {
                Type = Enums.StopCode.ProbeNotReady,
                Mask = GetAppSettingAsUInt("MaskStopCodeProbeNotReady"),
                Msg = ConfigurationManager.AppSettings["MsgStopCodeProbeNotReady"],
            });
            PlcToKioskStopCode.Add(new OpcConfigurationEntry<Enums.StopCode>
            {
                Type = Enums.StopCode.PressureOverage,
                Mask = GetAppSettingAsUInt("MaskStopCodePressureOverage"),
                Msg = ConfigurationManager.AppSettings["MsgStopCodePressureOverage"],
            });

            PlcToKioskStopCode.Add(new OpcConfigurationEntry<Enums.StopCode>
            {
                Type = Enums.StopCode.ScreenFul,
                Mask = GetAppSettingAsUInt("MaskStopCodeScreenFull"),
                Msg = ConfigurationManager.AppSettings["MsgStopCodeScreenFull"],
            });
            PlcToKioskStopCode.Add(new OpcConfigurationEntry<Enums.StopCode>
            {
                Type = Enums.StopCode.PathAutoErr,
                Mask = GetAppSettingAsUInt("MaskStopCodePathAutoErr"),
                Msg = ConfigurationManager.AppSettings["MsgStopCodePathAutoErr"],
            });
            PlcToKioskStopCode.Add(new OpcConfigurationEntry<Enums.StopCode>
            {
                Type = Enums.StopCode.PathErr,
                Mask = GetAppSettingAsUInt("MaskStopCodePathErr"),
                Msg = ConfigurationManager.AppSettings["MsgStopCodePathErr"],
            });

            PlcToKioskStopCode.Add(new OpcConfigurationEntry<Enums.StopCode>
            {
                Type = Enums.StopCode.PCSUPSActive,
                Mask = GetAppSettingAsUInt("MaskStopCodeUPSActive"),
                Msg = ConfigurationManager.AppSettings["MsgStopCodeUPSActive"],
            });
            PlcToKioskStopCode.Add(new OpcConfigurationEntry<Enums.StopCode>
            {
                Type = Enums.StopCode.PCS230VACErr,
                Mask = GetAppSettingAsUInt("MaskStopCode230VACErr"),
                Msg = ConfigurationManager.AppSettings["MsgStopCode230VACErr"],
            });
            PlcToKioskStopCode.Add(new OpcConfigurationEntry<Enums.StopCode>
            {
                Type = Enums.StopCode.PCS24VDCErr,
                Mask = GetAppSettingAsUInt("MaskStopCode24VDCErr"),
                Msg = ConfigurationManager.AppSettings["MsgStopCode24VDCErr"],
            });
            PlcToKioskStopCode.Add(new OpcConfigurationEntry<Enums.StopCode>
            {
                Type = Enums.StopCode.PCS12VDCErr,
                Mask = GetAppSettingAsUInt("MaskStopCode12VDCErr"),
                Msg = ConfigurationManager.AppSettings["MsgStopCode12VDCErr"],
            });
            PlcToKioskStopCode.Add(new OpcConfigurationEntry<Enums.StopCode>
            {
                Type = Enums.StopCode.PCSUPSErr,
                Mask = GetAppSettingAsUInt("MaskStopCodeUPSErr"),
                Msg = ConfigurationManager.AppSettings["MsgStopCodeUPSErr"],
            });
        }
        private void PopulateMasksPlcToKioskMeasCode()
        {
            PlcToKioskMeasCode = new List<OpcConfigurationEntry<Enums.MeasCode>>();
            PlcToKioskMeasCode.Add(new OpcConfigurationEntry<Enums.MeasCode>
            {
                Type = Enums.MeasCode.PHMin,
                Mask = GetAppSettingAsUInt("MaskMeasCodePHMin"),
                Msg = ConfigurationManager.AppSettings["MsgMeasCodePHMin"],
            });
            PlcToKioskMeasCode.Add(new OpcConfigurationEntry<Enums.MeasCode>
            {
                Type = Enums.MeasCode.PHMax,
                Mask = GetAppSettingAsUInt("MaskMeasCodePHMax"),
                Msg = ConfigurationManager.AppSettings["MsgMeasCodePHMax"],
            });

            PlcToKioskMeasCode.Add(new OpcConfigurationEntry<Enums.MeasCode>
            {
                Type = Enums.MeasCode.TempMin,
                Mask = GetAppSettingAsUInt("MaskMeasCodeTempMin"),
                Msg = ConfigurationManager.AppSettings["MsgMeasCodeTempMin"],
            });
            PlcToKioskMeasCode.Add(new OpcConfigurationEntry<Enums.MeasCode>
            {
                Type = Enums.MeasCode.TempMax,
                Mask = GetAppSettingAsUInt("MaskMeasCodeTempMax"),
                Msg = ConfigurationManager.AppSettings["MsgMeasCodeTempMax"],
            });

            PlcToKioskMeasCode.Add(new OpcConfigurationEntry<Enums.MeasCode>
            {
                Type = Enums.MeasCode.ChztMin,
                Mask = GetAppSettingAsUInt("MaskMeasCodeChztMin"),
                Msg = ConfigurationManager.AppSettings["MsgMeasCodeChztMin"],
            });
            PlcToKioskMeasCode.Add(new OpcConfigurationEntry<Enums.MeasCode>
            {
                Type = Enums.MeasCode.ChztMax,
                Mask = GetAppSettingAsUInt("MaskMeasCodeChztMax"),
                Msg = ConfigurationManager.AppSettings["MsgMeasCodeChztMax"],
            });
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

            MaskKioskOpenState = uint.Parse(ConfigurationManager.AppSettings["MaskKioskOpenState"]);
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

        private uint GetAppSettingAsUInt(string key)
        {
            return uint.Parse(ConfigurationManager.AppSettings[key]);
        }
    }
}

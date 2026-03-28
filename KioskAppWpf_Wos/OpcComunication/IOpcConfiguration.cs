using System.Collections.Generic;

namespace OpcCommunication
{
    public interface IOpcConfiguration
    {
        string ServerAddress { get; }
        string NamespaceUri { get; }

        string VarAddressLifeBit { get; }
        string VarAddressKioskLifeBit { get; }

        string VarAddressTransactionLifeBit { get; }
        string VarAddressTransactionKioskLifeBit { get; }

        string VarAddressFlowCnt { get; }
        string VarAddressTotalFlow { get; }

        string VarAddressProbeStart { get; }
        string VarAddressProbeMin { get; }
        string VarAddressDeclaredSewageAmount { get; }
        string VarAddressProbe1 { get; }
        string VarAddressProbe2 { get; }
        string VarAddressProbe3 { get; }
        string VarAddressSeqHoldTime { get; }
        string VarAddressSeqNoFlowTime { get; }
        string VarAddressSeqNormalFlowEnd { get; }
        string VarAddressWaterForMeasuringDevicesTime { get; }

        string VarAddressProbeScheduledBottleNo { get; }
        string VarAddressProbeAlarmBottleNo { get; }

        string VarAddressDumpPath { get; }
        string VarAddressCameraRegistration { get; }
        string VarAddressParameterOverrun { get; }

        string VarAddressPlcToKiosk1 { get; }
        string VarAddressPlcToKiosk2 { get; }
        string VarAddressPlcToKiosk3 { get; }

        string VarAddressPlcToKioskPermitCode { get; }
        string VarAddressPlcToKioskReadyCode { get; }
        string VarAddressPlcToKioskStopCode { get; }
        string VarAddressPlcToKioskMeasCode { get; }

        string VarAddressKioskToPlc1 { get; }
        string VarAddressKioskToPlc2 { get; }

        #region PLC_To_Kiosk_1
        uint Mask230VAC { get; }
        uint Mask24VDC { get; }

        uint MaskKioskDoorOpen { get; }
        uint MaskProbeDoorOpen { get; }

        uint MaskAirOk { get; }
        uint MaskFlowOk { get; }
        uint MaskPhMeterOk { get; }
        uint MaskTempMeterOk { get; }
        uint MaskProbeOk { get; }
        uint MaskProbeReady { get; }

        uint MaskDistribution230VACOk { get; }
        uint MaskDistribution230VACUpsOk { get; }
        uint MaskRack230VACOk { get; }
        uint MaskRack24VDCOk { get; }
        uint MaskRack24VDCUPSOk { get; }
        uint MaskRack12VDCOk { get; }

        uint MaskPumpRoomOk { get; }
        uint MaskPumpRoomWorking { get; }
        uint MaskScreenRoomLeak { get; }
        uint MaskScreenOk { get; }
        uint MaskScreenAuto { get; }
        uint MaskScreenFull { get; }

        uint MaskAugerOk { get; }
        uint MaskAugerAuto { get; }

        uint MaskPreScreenOk { get; }
        uint MaskPreScreenAuto { get; }

        uint MaskDumpPathOk { get; }
        uint MaskDumpPathAuto { get; }

        uint MaskGateOk { get; }
        uint MaskGateOpen { get; }

        uint MaskValveMeterOk { get; }
        uint MaskProbeResetting { get; }
        #endregion

        #region PLC_To_Kiosk_2
        uint MaskSeqRun { get; }
        uint MaskSeqIdle { get; }
        uint MaskSeqHeld { get; }
        uint MaskSeqFinish { get; }

        uint MaskSeqErrNoFlow { get; }
        uint MaskSeqErrDev { get; }
        uint MaskSeqErrAlarm { get; }
        uint MaskSeqErrAlarmProbe { get; }
        uint MaskSeqErrScreenFull { get; }
        uint MaskSeqErrKiosk { get; }
        uint MaskSeqNoFlowAck { get; }
        uint MaskSeqAck { get; }

        uint MaskProbeScheduledTaken { get; }
        uint MaskProbeAlarmTaken { get; }

        uint MaskKioskLightOn { get; }

        uint MaskWaterForMeasuringDevicesOn { get; }

        uint MaskSeqReady { get; }
        uint MaskSeqPermit { get; }

        uint MaskGate2Ok { get; }
        uint MaskGate2Open { get; }

        uint MaskChztOk { get; }

        uint MaskGate1Auto { get; }
        uint MaskGate2Auto { get; }

        uint MaskZasuwaOk { get; }
        #endregion

        List<OpcConfigurationEntry<Enums.PermitCode>> PlcToKioskPermitCode { get; }
        List<OpcConfigurationEntry<Enums.ReadyCode>> PlcToKioskReadyCode { get; }
        List<OpcConfigurationEntry<Enums.StopCode>> PlcToKioskStopCode { get; }
        List<OpcConfigurationEntry<Enums.MeasCode>> PlcToKioskMeasCode { get; }

        #region Kiosk_To_PLC_1
        uint MaskProbeSched { get; }
        uint MaskProbePermit { get; }
        uint MaskH2OMeter { get; }
        uint MaskpHOn { get; }
        uint MaskTempOn { get; }

        uint MaskPaperEnd { get; }
        uint MaskPrinterOn { get; }

        uint MaskKioskOn { get; }
        uint MaskKioskSeqNoFlowAck { get; }
        uint MaskKioskSeqAck { get; }

        uint MaskRfidGateInErr { get; }
        uint MaskRfidGateOutErr { get; }
        uint MaskRfidScreenRoomErr { get; }
        uint MaskRfidDistributionErr { get; }
        uint MaskRfidRegNoRecErr { get; }
        uint MaskRfidKioskErr { get; }

        uint MaskRfidGateInUnknown { get; }
        uint MaskRfidGateOutUnknown { get; }
        uint MaskRfidScreenRoomUnknown { get; }
        uint MaskRfidDistributionUnknown { get; }
        uint MaskRfidRegNoRecUnknown { get; }

        uint MaskChztOn { get; }

        uint MaskKioskOpenState { get; }
        #endregion

        #region Kiosk_To_PLC_2
        uint MaskSeqStart { get; }
        uint MaskSeqStop { get; }
        uint MaskSeqHold { get; }

        uint MaskLightOn { get; }
        uint MaskLightOff { get; }

        uint MaskResetSampler { get; }

        uint MaskDistributionOpen { get; }
        uint MaskScreenRoomOpen { get; }
        uint MaskGateOpenCommand { get; }
        uint MaskGateOutOpenCommand { get; }

        uint MaskToiletCommand { get; }
        #endregion
    }
}

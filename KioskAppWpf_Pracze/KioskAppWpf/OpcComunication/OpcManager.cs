using DbCommunication.Entities;
using DbCommunication.Enums;
using OpcCommunication.Devices;
using OpcCommunication.Enums;
using OpcComunication;
using System;

namespace OpcCommunication
{
    public class OpcManager
    {
        public bool OpcOn { get; set; }
        public bool PlcLifeBit { get; set; }
        public OpcClient Client { get; set; }
        public DevicesState DevicesState { get; set; }
        public DevicesSteering DevicesSteering { get; set; }
        public OpcParameters OpcParameters { get; set; }

        public CodeState<PermitCode> PermitCodeState { get; set; }
        public CodeState<ReadyCode> ReadyCodeState { get; set; }
        public CodeState<Enums.StopCode> StopCodeState { get; set; }
        public CodeState<MeasCode> MeasCodeState { get; set; }

        public OpcManager(bool opcOn)
        {
            OpcOn = opcOn;
            Client = OpcClient.GetInstance(new OpcConfiguration());

            DevicesState = new DevicesState()
            {
                Flow = new Flow(Client, OpcOn)
            };
            DevicesSteering = new DevicesSteering(Client, OpcOn);
            OpcParameters = new OpcParameters(Client, OpcOn);

            PermitCodeState = new CodeState<PermitCode>(Client.Configuration.PlcToKioskPermitCode);
            ReadyCodeState = new CodeState<ReadyCode>(Client.Configuration.PlcToKioskReadyCode);
            StopCodeState = new CodeState<Enums.StopCode>(Client.Configuration.PlcToKioskStopCode);
            MeasCodeState = new CodeState<MeasCode>(Client.Configuration.PlcToKioskMeasCode);
        }

        public DevicesState CheckDeviceState(KioskConfiguration kioskConf)
        {
            if (!OpcOn)
            {
                return CheckDeviceStateNoOpc(kioskConf);
            }

            DevicesState.KioskConf = kioskConf;

            int.TryParse(Client.ReadValue(Client.Configuration.VarAddressDumpPath), out int dumpPath);
            DevicesState.DumpPath = dumpPath;

            DevicesState.RegistrationNumberFromCamera = Client.ReadValue(Client.Configuration.VarAddressCameraRegistration);
            DevicesState.RegistrationNumberFromCamera2 = Client.ReadValue(Client.Configuration.VarAddressCameraRegistration2);

            DevicesState.StopCode = CheckStopCode();

            var opcRead = Client.ReadValue(Client.Configuration.VarAddressPlcToKiosk1);
            var opcRead2 = Client.ReadValue(Client.Configuration.VarAddressPlcToKiosk2);

            if (!uint.TryParse(opcRead, out uint plcToKiosk))
            {
                return DevicesState;
            }
            if (!uint.TryParse(opcRead2, out uint plcToKiosk2))
            {
                return DevicesState;
            }

            DevicesState.Power230Ok = (plcToKiosk & Client.Configuration.Mask230VAC) != 0;
            DevicesState.Power24Ok = (plcToKiosk & Client.Configuration.Mask24VDC) != 0;

            DevicesState.KioskDoorOpen = (plcToKiosk & Client.Configuration.MaskKioskDoorOpen) != 0;
            DevicesState.ProbeDoorOpen = (plcToKiosk & Client.Configuration.MaskProbeDoorOpen) != 0;

            DevicesState.AirOk = (plcToKiosk & Client.Configuration.MaskAirOk) != 0;
            DevicesState.FlowOk = (plcToKiosk & Client.Configuration.MaskFlowOk) != 0;
            DevicesState.PhMeterOk = (plcToKiosk & Client.Configuration.MaskPhMeterOk) != 0;
            DevicesState.TempMeterOk = (plcToKiosk & Client.Configuration.MaskTempMeterOk) != 0;
            DevicesState.ProbeOk = (plcToKiosk & Client.Configuration.MaskProbeOk) != 0;
            DevicesState.ProbeReadyOk = (plcToKiosk & Client.Configuration.MaskProbeReady) != 0;

            DevicesState.Distribution230VACOk = (plcToKiosk & Client.Configuration.MaskDistribution230VACOk) != 0;
            DevicesState.Distribution230VACUpsOk = (plcToKiosk & Client.Configuration.MaskDistribution230VACUpsOk) != 0;
            DevicesState.Rack230VACOk = (plcToKiosk & Client.Configuration.MaskRack230VACOk) != 0;
            DevicesState.Rack24VDCOk = (plcToKiosk & Client.Configuration.MaskRack24VDCOk) != 0;
            DevicesState.Rack24VDCUPSOk = (plcToKiosk & Client.Configuration.MaskRack24VDCUPSOk) != 0;
            DevicesState.Rack12VDCOk = (plcToKiosk & Client.Configuration.MaskRack12VDCOk) != 0;

            DevicesState.PumpRoomOk = (plcToKiosk & Client.Configuration.MaskPumpRoomOk) != 0;
            DevicesState.PumpRoomWorking = (plcToKiosk & Client.Configuration.MaskPumpRoomWorking) != 0;
            DevicesState.ScreenRoomLeak = (plcToKiosk & Client.Configuration.MaskScreenRoomLeak) != 0;
            DevicesState.ScreenRoomLeak = (plcToKiosk & Client.Configuration.MaskScreenRoomLeak) != 0;
            DevicesState.ScreenOk = (plcToKiosk & Client.Configuration.MaskScreenOk) != 0;
            DevicesState.ScreenAuto = (plcToKiosk & Client.Configuration.MaskScreenAuto) != 0;
            DevicesState.ScreenFull = (plcToKiosk & Client.Configuration.MaskScreenFull) != 0;

            DevicesState.AugerOk = (plcToKiosk & Client.Configuration.MaskAugerOk) != 0;
            DevicesState.AugerAuto = (plcToKiosk & Client.Configuration.MaskAugerAuto) != 0;

            DevicesState.PreScreenOk = (plcToKiosk & Client.Configuration.MaskPreScreenOk) != 0;
            DevicesState.PreScreenAuto = (plcToKiosk & Client.Configuration.MaskPreScreenAuto) != 0;

            DevicesState.DumpPathOk = (plcToKiosk & Client.Configuration.MaskDumpPathOk) != 0;
            DevicesState.DumpPathAuto = (plcToKiosk & Client.Configuration.MaskDumpPathAuto) != 0;

            DevicesState.GateOk = (plcToKiosk & Client.Configuration.MaskGateOk) != 0;
            DevicesState.GateOpen = (plcToKiosk & Client.Configuration.MaskGateOpen) != 0;

            DevicesState.ValveMeterOk = (plcToKiosk & Client.Configuration.MaskValveMeterOk) != 0;
            DevicesState.ProbeResetting = (plcToKiosk & Client.Configuration.MaskProbeResetting) != 0;

            DevicesState.Flow.CheckStatus(plcToKiosk, plcToKiosk2);

            DevicesState.PressureDumpError = (plcToKiosk2 & Client.Configuration.MaskPressureDumpError) != 0;
            DevicesState.PressurePumpOn = (plcToKiosk2 & Client.Configuration.MaskPressurePumpOn) != 0;

            DevicesState.PressureMeterOk = (plcToKiosk2 & Client.Configuration.MaskPressureOk) != 0;

            return DevicesState;
        }

        private DevicesState CheckDeviceStateNoOpc(KioskConfiguration kioskConf)
        {
            DevicesState.StopCode = CheckStopCode();

            var opcRead = "0";
            var opcRead2 = "0";
            if (!uint.TryParse(opcRead, out uint plcToKiosk))
            {
                return DevicesState;
            }
            if (!uint.TryParse(opcRead2, out uint plcToKiosk2))
            {
                return DevicesState;
            }

            DevicesState.Power230Ok = true;
            DevicesState.Power24Ok = true;

            DevicesState.KioskDoorOpen = false;
            DevicesState.ProbeDoorOpen = false;

            DevicesState.AirOk = true;
            DevicesState.FlowOk = true;
            DevicesState.PhMeterOk = true;
            DevicesState.TempMeterOk = true;
            DevicesState.PressureMeterOk = true;
            DevicesState.ProbeOk = true;
            DevicesState.ProbeReadyOk = true;

            DevicesState.Distribution230VACOk = true;
            DevicesState.Distribution230VACUpsOk = true;
            DevicesState.Rack230VACOk = true;
            DevicesState.Rack24VDCOk = true;
            DevicesState.Rack24VDCUPSOk = true;
            DevicesState.Rack12VDCOk = true;

            DevicesState.PumpRoomOk = true;
            DevicesState.PumpRoomWorking = true;
            DevicesState.ScreenRoomLeak = false;
            DevicesState.ScreenOk = true;
            DevicesState.ScreenAuto = true;
            DevicesState.ScreenFull = false;

            DevicesState.AugerOk = true;
            DevicesState.AugerAuto = true;

            DevicesState.PreScreenOk = true;
            DevicesState.PreScreenAuto = true;

            DevicesState.DumpPathOk = true;
            DevicesState.DumpPathAuto = true;

            DevicesState.PressureDumpError = true;
            DevicesState.PressurePumpOn = true;

            DevicesState.GateOk = true;
            DevicesState.GateOpen = true;

            DevicesState.ValveMeterOk = true;

            DevicesState.Flow.CheckStatus(plcToKiosk, plcToKiosk2);

            return DevicesState;
        }

        public short CheckPlcLiveBit()
        {
            if (OpcOn)
            {
                var v = Client.ReadValue(Client.Configuration.VarAddressLifeBit);
                short lifeBitCounter = 0;
                if (Int16.TryParse(v, out lifeBitCounter))
                    return lifeBitCounter;
                else
                    return 0;
            }
            else
            {
                return 0;
            }
        }
        public DbCommunication.Enums.StopCode CheckStopCode()
        {
            if (!OpcOn)
            {
                return DbCommunication.Enums.StopCode.None;
            }

            var orgStopCode = Client.ReadValue(Client.Configuration.VarAddressDumpPath);
            Enum.TryParse<DbCommunication.Enums.StopCode>(Client.ReadValue(Client.Configuration.VarAddressDumpPath), out DbCommunication.Enums.StopCode stopCode);
            return stopCode;
        }
        public void SendKioskLiveBit(short value)
        {
            if (OpcOn)
            {
                Client.WriteNewBlockValue(Client.Configuration.VarAddressKioskLifeBit, value);
            }
        }
        public short CheckTransactionLiveBit()
        {
            if (OpcOn)
            {
                var v = Client.ReadValue(Client.Configuration.VarAddressTransactionLifeBit);
                short transactionLifeBitCounter = 0;
                if (Int16.TryParse(v, out transactionLifeBitCounter))
                    return transactionLifeBitCounter;
                else
                    return 0;
            }
            else
            {
                return 0;
            }
        }
        public void SendTransactionKioskLiveBit(short value)
        {
            if (OpcOn)
            {
                Client.WriteNewBlockValue(Client.Configuration.VarAddressTransactionKioskLifeBit, value);
            }
        }

        public void BuildOpcParameters(KioskConfiguration config, bool printerOutOfPaperErrorState)
        {
            var opcParameters = new OpcParameters(Client, OpcOn);
            opcParameters.ProbeScheduled = false;
            opcParameters.ProbePermit = !config.KioskBlockages[KioskBlockageType.TakingSamples];
            opcParameters.WaterForMeasuringDevices = !config.KioskBlockages[KioskBlockageType.WaterForMeasuringDevices];

            opcParameters.MonitoringPhOn = !config.KioskBlockages[KioskBlockageType.MeasuringPh];
            opcParameters.MonitoringTempOn = !config.KioskBlockages[KioskBlockageType.MeasugingTemperature];
            opcParameters.MonitoringPressureOn = !config.KioskBlockages[KioskBlockageType.MeasuringPressure];
            opcParameters.PressureBlockageOn = !config.KioskBlockages[KioskBlockageType.PressureBlockageOn];

            opcParameters.PaperEnd = printerOutOfPaperErrorState;
            opcParameters.PrinterOn = !config.KioskBlockages[KioskBlockageType.Printer];

            opcParameters.ProbeStart = config.ConfigurationSettings[KioskConfigurationType.ProbeStartAfterPercent];
            opcParameters.ProbeMin = config.ConfigurationSettings[KioskConfigurationType.ProbeMinimalAfterStartL];
            opcParameters.Probe1 = config.ConfigurationSettings[KioskConfigurationType.Probe1AfterStartPercent];
            opcParameters.Probe2 = config.ConfigurationSettings[KioskConfigurationType.Probe2AfterStartPercent];
            opcParameters.Probe3 = config.ConfigurationSettings[KioskConfigurationType.Probe3AfterStartPercent];
            opcParameters.DeclaredSewageAmount = 0;
            opcParameters.SeqHoldTime = config.ConfigurationSettings[KioskConfigurationType.WaitTimeForEmptyScreen];
            opcParameters.SeqNoFlowTime = config.ConfigurationSettings[KioskConfigurationType.WaitTimeForInitialFlow];
            opcParameters.SeqNormalFlowEnd = config.ConfigurationSettings[KioskConfigurationType.WaitTimeForEndOfFlow];
            opcParameters.WaterForDriversTime = config.ConfigurationSettings[KioskConfigurationType.TimeForWaterForDrivers];
            opcParameters.WaterForMeasuringDevicesTime = config.ConfigurationSettings[KioskConfigurationType.TimeForWaterForMeasuringDevices];

            OpcParameters = opcParameters;
        }

        public void SetKioskSeqAckToTrue()
        {
            OpcParameters.SeqAck = true;
            OpcParameters.UpdateOpcParameters();
            DevicesState.Flow.DumpStartTime = null;
            DevicesState.Flow.SeqAckTime = DateTime.Now;
        }
        public void SetKioskSeqAckToFalse()
        {
            OpcParameters.SeqAck = false;
            OpcParameters.UpdateOpcParameters();
            DevicesState.Flow.SeqAckTime = null;
        }
        public void SetKioskSeqNoFlowAckToTrue()
        {
            OpcParameters.SeqNoFlowAck = true;
            OpcParameters.UpdateOpcParameters();
            DevicesState.Flow.DumpStartTime = null;
            DevicesState.Flow.SeqAckTime = DateTime.Now;
        }
        public void SetKioskSeqNoFlowAckToFalse()
        {
            OpcParameters.SeqNoFlowAck = false;
            OpcParameters.UpdateOpcParameters();
            DevicesState.Flow.SeqAckTime = null;
        }
        public void SetLicensePlateUnknownVariable()
        {
            OpcParameters.CarRegistrationNumberUnknown = true;
            OpcParameters.UpdateOpcParameters();
        }
        public void UnSetLicensePlateUnknownVariable()
        {
            OpcParameters.CarRegistrationNumberUnknown = false;
            OpcParameters.UpdateOpcParameters();
        }

        public void StartSequence()
        {
            DevicesSteering.SeqStart = true;
            DevicesSteering.SendSteering();
            DevicesState.Flow.FlowFlowCnt = 0;
            DevicesState.Flow.DumpStartTime = DateTime.Now;

            if (!OpcOn)
            {
                DevicesState.Flow.SeqAck = true;
            }
        }
        public void SendSequenceStop()
        {
            DevicesSteering.SeqStop = true;
            DevicesSteering.SendSteering();
        }
        public void SendSequenceHold()
        {
            DevicesSteering.SeqHold = true;
            DevicesSteering.SendSteering();
        }

        public void SendLightOn()
        {
            DevicesSteering.LightOn = true;
            DevicesSteering.SendSteering();
        }
        public void SendLightOff()
        {
            DevicesSteering.LightOff = true;
            DevicesSteering.SendSteering();
        }

        public void SendResetSampler()
        {
            DevicesSteering.SamplerReset = true;
            DevicesSteering.SendSteering();
        }

        public void SetDistributionOpen(bool open)
        {
            DevicesSteering.DistributionOpen = open;
            DevicesSteering.SendSteering();
        }
        public void SetScreenRoomOpen(bool open)
        {
            DevicesSteering.ScreenRoomOpen = open;
            DevicesSteering.SendSteering();
        }
        public void SetToiletOpen(bool open)
        {
            DevicesSteering.ToiletOpen = open;
            DevicesSteering.SendSteering();
        }
        public void SetGateOpen(bool open)
        {
            DevicesSteering.GateOpen = open;
            DevicesSteering.SendSteering();
        }

        private void SendCommands(uint parameters)
        {
            Client.WriteNewBlockValue(Client.Configuration.VarAddressKioskToPlc2, parameters);
        }

        public void CheckCodeState()
        {
            if (!OpcOn)
            {
                CheckCodeStateNoOpc();
            }

            CheckCodeState<PermitCode>(PermitCodeState, Client.Configuration.VarAddressPlcToKioskPermitCode);
            CheckCodeState<ReadyCode>(ReadyCodeState, Client.Configuration.VarAddressPlcToKioskReadyCode);
            CheckCodeStateInt<Enums.StopCode>(StopCodeState, Client.Configuration.VarAddressPlcToKioskStopCode);
            CheckCodeStateInt<MeasCode>(MeasCodeState, Client.Configuration.VarAddressPlcToKioskMeasCode);
        }
        private void CheckCodeStateNoOpc()
        {
            PermitCodeState.PopulateState(1217);
            ReadyCodeState.PopulateState(65);
            StopCodeState.PopulateState(3);
            MeasCodeState.PopulateState(3);
        }
        private void CheckCodeState<T>(CodeState<T> state, string varAddress)
        {
            var opcRead = Client.ReadValue(varAddress);
            if (uint.TryParse(opcRead, out uint opcState))
            {
                state.PopulateState(opcState);
            }
        }
        private void CheckCodeStateInt<T>(CodeState<T> state, string varAddress)
        {
            var opcRead = Client.ReadValue(varAddress);
            if (uint.TryParse(opcRead, out uint opcState))
            {
                state.PopulateStateInt(opcState);
            }
        }
    }
}

using DbCommunication.Entities;
using DbCommunication.Enums;

namespace OpcCommunication.Devices
{
    public class DevicesState
    {
        public Flow Flow { get; set; }
        public KioskConfiguration KioskConf { get; set; }

        public bool Power230Ok { get; set; }
        public bool Power24Ok { get; set; }

        public bool KioskDoorOpen { get; set; }
        public bool ProbeDoorOpen { get; set; }

        public bool AirOk { get; set; }
        public bool FlowOk { get; set; }
        public bool PhMeterOk { get; set; }
        public bool TempMeterOk { get; set; }
        public bool PressureMeterOk { get; set; }
        public bool ProbeOk { get; set; }
        public bool ProbeReadyOk { get; set; }

        public bool Distribution230VACOk { get; set; }
        public bool Distribution230VACUpsOk { get; set; }
        public bool Rack230VACOk { get; set; }
        public bool Rack24VDCOk { get; set; }
        public bool Rack24VDCUPSOk { get; set; }
        public bool Rack12VDCOk { get; set; }

        public bool PumpRoomOk { get; set; }
        public bool PumpRoomWorking { get; set; }
        public bool ScreenRoomLeak { get; set; }
        public bool ScreenOk { get; set; }
        public bool ScreenAuto { get; set; }
        public bool ScreenFull { get; set; }

        public bool PressureDumpError { get; set; }
        public bool PressurePumpOn { get; set; }

        public bool AugerOk { get; set; }
        public bool AugerAuto { get; set; }

        public bool PreScreenOk { get; set; }
        public bool PreScreenAuto { get; set; }

        public bool DumpPathOk { get; set; }
        public bool DumpPathAuto { get; set; }

        public bool GateOk { get; set; }
        public bool GateOpen { get; set; }

        public bool ValveMeterOk { get; set; }

        public bool ProbeResetting { get; set; }

        public int DumpPath { get; set; }

        public string RegistrationNumberFromCamera { get; set; }
        public string RegistrationNumberFromCamera2 { get; set; }

        public bool DeviceError
        {
            get
            {
                if (KioskConf == null) return true;

                return !Power230Ok || !Power24Ok
                    || KioskDoorOpen || ProbeDoorOpen
                    || !AirOk || !FlowOk
                    || (!PhMeterOk && !KioskConf.KioskBlockages[KioskBlockageType.MeasuringPh])
                    || (!TempMeterOk && !KioskConf.KioskBlockages[KioskBlockageType.MeasugingTemperature])
                    || (!ProbeOk && !KioskConf.KioskBlockages[KioskBlockageType.TakingSamples])
                    || (!ProbeReadyOk && !KioskConf.KioskBlockages[KioskBlockageType.TakingSamples])
                    || !Distribution230VACOk || !Distribution230VACUpsOk 
                    || !Rack230VACOk || !Rack24VDCOk || !Rack24VDCUPSOk || !Rack12VDCOk
                    || !PumpRoomOk || !PumpRoomWorking
                    || ScreenRoomLeak || !ScreenOk || ScreenFull
                    || !AugerOk || !PreScreenOk || !DumpPathOk 
                    || (!ValveMeterOk && !KioskConf.KioskBlockages[KioskBlockageType.WaterForMeasuringDevices])
                    ;
            }
        }

        public StopCode StopCode { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is DevicesState)
            {
                return Power230Ok == ((DevicesState)obj).Power230Ok
                    && Power24Ok == ((DevicesState)obj).Power24Ok

                    && KioskDoorOpen == ((DevicesState)obj).KioskDoorOpen
                    && ProbeDoorOpen == ((DevicesState)obj).ProbeDoorOpen

                    && AirOk == ((DevicesState)obj).AirOk
                    && FlowOk == ((DevicesState)obj).FlowOk
                    && PhMeterOk == ((DevicesState)obj).PhMeterOk
                    && TempMeterOk == ((DevicesState)obj).TempMeterOk
                    && PressureMeterOk == ((DevicesState)obj).PressureMeterOk
                    && ProbeOk == ((DevicesState)obj).ProbeOk
                    && ProbeReadyOk == ((DevicesState)obj).ProbeReadyOk

                    && Distribution230VACOk == ((DevicesState)obj).Distribution230VACOk
                    && Distribution230VACUpsOk == ((DevicesState)obj).Distribution230VACUpsOk
                    && Rack230VACOk == ((DevicesState)obj).Rack230VACOk
                    && Rack24VDCOk == ((DevicesState)obj).Rack24VDCOk
                    && Rack24VDCUPSOk == ((DevicesState)obj).Rack24VDCUPSOk
                    && Rack12VDCOk == ((DevicesState)obj).Rack12VDCOk

                    && PumpRoomOk == ((DevicesState)obj).PumpRoomOk
                    && PumpRoomWorking == ((DevicesState)obj).PumpRoomWorking
                    && ScreenRoomLeak == ((DevicesState)obj).ScreenRoomLeak
                    && ScreenOk == ((DevicesState)obj).ScreenOk
                    && ScreenAuto == ((DevicesState)obj).ScreenAuto
                    && ScreenFull == ((DevicesState)obj).ScreenFull

                    && AugerOk == ((DevicesState)obj).AugerOk
                    && AugerAuto == ((DevicesState)obj).AugerAuto

                    && PreScreenOk == ((DevicesState)obj).PreScreenOk
                    && PreScreenAuto == ((DevicesState)obj).PreScreenAuto

                    && DumpPathOk == ((DevicesState)obj).DumpPathOk
                    && DumpPathAuto == ((DevicesState)obj).DumpPathAuto

                    && PressureDumpError == ((DevicesState)obj).PressureDumpError
                    && PressurePumpOn == ((DevicesState)obj).PressurePumpOn

                    && GateOk == ((DevicesState)obj).GateOk
                    && GateOpen == ((DevicesState)obj).GateOpen

                    && ValveMeterOk == ((DevicesState)obj).ValveMeterOk
                    && ProbeResetting == ((DevicesState)obj).ProbeResetting

                    && DumpPath == ((DevicesState)obj).DumpPath
                    
                    && StopCode == ((DevicesState)obj).StopCode;
            }
            return false;
        }
    }
}

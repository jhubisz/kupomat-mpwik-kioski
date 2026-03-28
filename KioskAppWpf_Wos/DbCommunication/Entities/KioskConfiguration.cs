using DbCommunication.Enums;
using System;
using System.Collections.Generic;

namespace DbCommunication.Entities
{
    public class KioskConfiguration
    {
        public TimeSpan OpenHoursFrom
        {
            get
            {
                return TimeSpan.FromMinutes(OpenHoursFromInMinutesFromMidninght);
            }
        }
        public string OpenFromString
        {
            get
            {
                return OpenHoursFrom.ToString(@"hh\:mm");
            }
        }
        public int OpenHoursFromInMinutesFromMidninght { get; set; }
        public TimeSpan OpenHoursTo
        {
            get
            {
                return TimeSpan.FromMinutes(OpenHoursToInMinutesFromMidninght);
            }
        }
        public string OpenToString
        {
            get
            {
                return OpenHoursTo.ToString(@"hh\:mm");
            }
        }
        public int OpenHoursToInMinutesFromMidninght { get; set; }
        public KioskOpenState Open
        {
            get
            {
                if (DateTime.Now >= DateTime.Now.Date.AddMinutes(OpenHoursFromInMinutesFromMidninght)
                    && DateTime.Now <= DateTime.Now.Date.AddMinutes(OpenHoursToInMinutesFromMidninght))
                {
                    return KioskOpenState.Open;
                }
                else
                {
                    return KioskOpenState.Closed;
                }

            }
        }
        
        private Dictionary<KioskBlockageType, bool> kioskBlockages;
        public Dictionary<KioskBlockageType, bool> KioskBlockages
        {
            get
            {
                if (kioskBlockages == null)
                {
                    kioskBlockages = new Dictionary<KioskBlockageType, bool>();
                    kioskBlockages.Add(KioskBlockageType.TakingSamples, false);
                    kioskBlockages.Add(KioskBlockageType.MeasuringPh, false);
                    kioskBlockages.Add(KioskBlockageType.MeasuringConduction, false);
                    kioskBlockages.Add(KioskBlockageType.MeasuringTemperature, false);
                    kioskBlockages.Add(KioskBlockageType.Printer, false);
                    kioskBlockages.Add(KioskBlockageType.WaterForDrivers, false);
                    kioskBlockages.Add(KioskBlockageType.WaterForMeasuringDevices, false);
                    kioskBlockages.Add(KioskBlockageType.StationGate, false);
                    kioskBlockages.Add(KioskBlockageType.MeasuringChzt, false);
                    kioskBlockages.Add(KioskBlockageType.ToiletSteering, false);
                    kioskBlockages.Add(KioskBlockageType.CameraOpcRegistration, false);
                    kioskBlockages.Add(KioskBlockageType.Keyboard, false);
                    kioskBlockages.Add(KioskBlockageType.GatesSteering, false);
                }
                return kioskBlockages;
            }
            set
            {
                kioskBlockages = value;
            }
        }

        private Dictionary<KioskConfigurationType, short> configurationSettings;
        public Dictionary<KioskConfigurationType, short> ConfigurationSettings
        {
            get
            {
                if (configurationSettings == null)
                {
                    configurationSettings = new Dictionary<KioskConfigurationType, short>();
                    configurationSettings.Add(KioskConfigurationType.WaitTimeForEmptyScreen, 0);
                    configurationSettings.Add(KioskConfigurationType.WaitTimeForInitialFlow, 0);
                    configurationSettings.Add(KioskConfigurationType.WaitTimeForEndOfFlow, 0);
                    configurationSettings.Add(KioskConfigurationType.TimeForWaterForDrivers, 0);
                    configurationSettings.Add(KioskConfigurationType.TimeForWaterForMeasuringDevices, 0);
                    configurationSettings.Add(KioskConfigurationType.ProbeStartAfterPercent, 0);
                    configurationSettings.Add(KioskConfigurationType.ProbeMinimalAfterStartL, 0);
                    configurationSettings.Add(KioskConfigurationType.Probe1AfterStartPercent, 0);
                    configurationSettings.Add(KioskConfigurationType.Probe2AfterStartPercent, 0);
                    configurationSettings.Add(KioskConfigurationType.Probe3AfterStartPercent, 0);
                }
                return configurationSettings;
            }
            set
            {
                configurationSettings = value;
            }
        }

        public string KioskWiadomosc { get; set; }

        public string ServiceScreenPassword
        {
            get
            {
                return "Aqua.11!!";
            }
        }
    }
}

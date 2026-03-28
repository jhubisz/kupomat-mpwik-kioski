using RfidCommunication.Enums;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO.Ports;

namespace RfidCommunication
{
    public class RfidConfig
    {
        public Dictionary<ReaderLocation, RfidReaderConfig> RfidCardReader { get; set; }

        public RfidConfig()
        {
            RfidCardReader = new Dictionary<ReaderLocation, RfidReaderConfig>()
            {
                //{ ReaderLocation.GateIn,  GetReaderConfig(ReaderLocation.GateIn, "RfidGateIn") },
                //{ ReaderLocation.GateOut, GetReaderConfig(ReaderLocation.GateOut, "RfidGateOut") },

                //{ ReaderLocation.Kiosk, GetReaderConfig(ReaderLocation.Kiosk, "RfidKiosk") },

                ////{ ReaderLocation.Huber, GetReaderConfig(ReaderLocation.Huber, "RfidHuber") },
                //{ ReaderLocation.Rozdzielnia, GetReaderConfig(ReaderLocation.Rozdzielnia, "RfidRozdzielnia") },

                ////{ ReaderLocation.Camera, GetReaderConfig(ReaderLocation.Camera, "RfidKamera") },
            };
        }

        private RfidReaderConfig GetReaderConfig(ReaderLocation location, string configKey)
        {
            var readerConfig = new RfidReaderConfig()
            {
                ReaderLocation = location,
                ReaderType = (ReaderType)Enum.Parse(typeof(ReaderType), GetStringFromConfig(configKey + "Type"))
            };

            if (readerConfig.ReaderType == ReaderType.Ethernet)
            {
                readerConfig.Uri = GetStringFromConfig(configKey + "Uri");
                readerConfig.Login = GetStringFromConfig(configKey + "Login");
                readerConfig.Password = GetStringFromConfig(configKey + "Pass");
            }

            if (readerConfig.ReaderType == ReaderType.Com)
            {
                readerConfig.ComName = GetStringFromConfig(configKey + "ComName");
                readerConfig.ComDataBits = Int32.Parse(GetStringFromConfig(configKey + "ComDataBits"));
                readerConfig.ComBoundRate = Int32.Parse(GetStringFromConfig(configKey + "ComBoundRate"));
                readerConfig.ComParity = (Parity)Enum.Parse(typeof(Parity), GetStringFromConfig(configKey + "ComParity"));
                readerConfig.ComStopBits = (StopBits)Enum.Parse(typeof(StopBits), GetStringFromConfig(configKey + "ComStopBits"));
            }

            if (readerConfig.ReaderType == ReaderType.Camera)
            {
                readerConfig.ComName = GetStringFromConfig(configKey + "ComName");
                readerConfig.ComDataBits = Int32.Parse(GetStringFromConfig(configKey + "ComDataBits"));
                readerConfig.ComBoundRate = Int32.Parse(GetStringFromConfig(configKey + "ComBoundRate"));
                readerConfig.ComParity = (Parity)Enum.Parse(typeof(Parity), GetStringFromConfig(configKey + "ComParity"));
                readerConfig.ComStopBits = (StopBits)Enum.Parse(typeof(StopBits), GetStringFromConfig(configKey + "ComStopBits"));
            }

            return readerConfig;
        }
        private string GetStringFromConfig(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }
    }
}

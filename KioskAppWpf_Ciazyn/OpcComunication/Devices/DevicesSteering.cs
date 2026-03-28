using DbCommunication.Entities;
using OpcCommunication.Enums;
using System;
using System.Collections.Generic;

namespace OpcCommunication.Devices
{
    public class DevicesSteering
    {
        private bool OpcOn { get; set; }
        private OpcClient OpcClient { get; set; }
        
        public bool SeqStart { get; set; }
        public bool SeqStop { get; set; }
        public bool SeqHold { get; set; }
        public bool LightOn { get; set; }
        public bool LightOff { get; set; }
        public bool SamplerReset { get; set; }
        public bool DistributionOpen { get; set; }
        public bool ScreenRoomOpen { get; set; }
        public bool GateOpen { get; set; }
        public bool GateOutOpen { get; set; }

        public bool ToiletOpen { get; set; }

        private DevicesSteering() { }
        public DevicesSteering(OpcClient client, bool opcOn)
        {
            OpcClient = client;
            OpcOn = opcOn;
        }

        public void SendSteering()
        {
            uint parameters = 0;
            if (SeqStart)
            {
                parameters = parameters | OpcClient.Configuration.MaskSeqStart;
                SeqStart = false;
            }
            if (SeqStop)
            {
                parameters = parameters | OpcClient.Configuration.MaskSeqStop;
                SeqStop = false;
            }
            if (SeqHold)
            {
                parameters = parameters | OpcClient.Configuration.MaskSeqHold;
                SeqHold = false;
            }
            if (LightOn)
            {
                parameters = parameters | OpcClient.Configuration.MaskLightOn;
                LightOn = false;
            }
            if (LightOff)
            {
                parameters = parameters | OpcClient.Configuration.MaskLightOff;
                LightOff = false;
            }
            if (SamplerReset)
            {
                parameters = parameters | OpcClient.Configuration.MaskResetSampler;
                SamplerReset = false;
            }
            if (DistributionOpen)
            {
                parameters = parameters | OpcClient.Configuration.MaskDistributionOpen;
            }
            if (ScreenRoomOpen)
            {
                parameters = parameters | OpcClient.Configuration.MaskScreenRoomOpen;
            }
            if (ToiletOpen)
            {
                parameters = parameters | OpcClient.Configuration.MaskToiletCommand;
            }
            if (GateOpen)
            {
                parameters = parameters | OpcClient.Configuration.MaskGateOpenCommand;
            }
            if (GateOutOpen)
            {
                parameters = parameters | OpcClient.Configuration.MaskGateOutOpenCommand;
            }

            if (OpcOn)
                OpcClient.WriteNewBlockValue(OpcClient.Configuration.VarAddressKioskToPlc2, parameters);
        }
    }
}

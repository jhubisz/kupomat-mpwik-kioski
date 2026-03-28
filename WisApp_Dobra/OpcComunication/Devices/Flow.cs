using DbCommunication.Entities;
using DbCommunication.Enums;
using System;
using System.Configuration;

namespace OpcCommunication.Devices
{
    public class Flow
    {
        private bool _chztOn
        {
            get
            {
                return ConfigurationManager.AppSettings["_chztOn"] == "1";
            }
        }

        private bool OpcOn { get; set; }
        private OpcClient Client { get; set; }

        public int SamplerBottleNo { get; set; }

        public decimal FlowpHCurr { get; set; }
        public decimal FlowphMin { get; set; }
        public decimal FlowphMed { get; set; }
        public decimal FlowphMax { get; set; }

        public decimal FlowTempCurr { get; set; }
        public decimal FlowTempMin { get; set; }
        public decimal FlowTempMed { get; set; }
        public decimal FlowTempMax { get; set; }

        public decimal FlowChztCurr { get; set; }
        public decimal FlowChztMin { get; set; }
        public decimal FlowChztMed { get; set; }
        public decimal FlowChztMax { get; set; }

        public decimal FlowFlowCurr { get; set; }
        public decimal FlowFlowCnt { get; set; }

        public bool ScheduledSampleTaken { get; set; }
        public bool AlarmSampleTaken { get; set; }

        public bool SeqRun { get; set; }
        public bool SeqIdle { get; set; }
        public bool SeqHeld { get; set; }
        public bool SeqFinish { get; set; }

        public bool SeqErrNoFlow { get; set; }
        public bool SeqErrDev { get; set; }
        public bool SeqErrAlarm { get; set; }
        public bool SeqErrAlarmProbe { get; set; }
        public bool SeqErrScreenFull { get; set; }
        public bool SeqErrKiosk { get; set; }

        public bool SeqNoFlowAck { get; set; }
        public bool SeqAck { get; set; }
        public DateTime? SeqAckTime { get; set; }

        public bool SeqReady { get; set; }
        public bool SeqPermit { get; set; }

        public bool WaterForMeasuringDevicesRunning { get; set; }
        public short WaterForMeasuringDevicesTimeRemaining { get; set; }

        public bool DumpInProgress { get; set; }
        public DateTime? DumpStartTime { get; set; }

        private Flow() { }
        public Flow(OpcClient client, bool opcOn)
        {
            Client = client;
            OpcOn = opcOn;
        }

        public void CheckStatus(uint plcToKiosk, uint plcToKiosk2)
        {
            if (OpcOn)
            {
                try
                {
                    CheckStatus();
                    
                    // PLC_TO_KIOSK_2
                    SeqRun = (plcToKiosk2 & Client.Configuration.MaskSeqRun) != 0;
                    SeqIdle = (plcToKiosk2 & Client.Configuration.MaskSeqRun) != 0;
                    SeqHeld = (plcToKiosk2 & Client.Configuration.MaskSeqHeld) != 0;

                    SeqAck = (plcToKiosk2 & Client.Configuration.MaskSeqAck) != 0;
                    SeqNoFlowAck = (plcToKiosk2 & Client.Configuration.MaskSeqNoFlowAck) != 0;

                    SeqErrNoFlow = (plcToKiosk2 & Client.Configuration.MaskSeqErrNoFlow) != 0;
                    SeqErrDev = (plcToKiosk2 & Client.Configuration.MaskSeqErrDev) != 0;
                    SeqErrAlarm = (plcToKiosk2 & Client.Configuration.MaskSeqErrAlarm) != 0;
                    SeqErrScreenFull = (plcToKiosk2 & Client.Configuration.MaskSeqErrScreenFull) != 0;

                    ScheduledSampleTaken = (plcToKiosk2 & Client.Configuration.MaskProbeScheduledTaken) != 0;
                    AlarmSampleTaken = (plcToKiosk2 & Client.Configuration.MaskProbeAlarmTaken) != 0;

                    WaterForMeasuringDevicesRunning = (plcToKiosk2 & Client.Configuration.MaskWaterForMeasuringDevicesOn) != 0;

                    SeqReady = (plcToKiosk2 & Client.Configuration.MaskSeqReady) != 0;
                    SeqPermit = (plcToKiosk2 & Client.Configuration.MaskSeqPermit) != 0;
                }
                catch
                {
                }
            }
            else
            {
                // PLC_TO_KIOSK_2
                SeqRun = false;
                SeqIdle = false;
                SeqHeld = false;

                SeqAck = false;
                SeqNoFlowAck = false;

                SeqErrNoFlow = false;
                SeqErrDev = false;
                SeqErrAlarm = false;
                SeqErrScreenFull = false;

                ScheduledSampleTaken = false;
                AlarmSampleTaken = false;

                WaterForMeasuringDevicesRunning = false;

                SeqReady = true; 
                SeqPermit = true;
            }
        }
        public void CheckStatus()
        {
            if (OpcOn)
            {
                try
                {
                    var flowString = Client.ReadValue(Client.Configuration.VarAddressPlcToKiosk3);
                    var stringArray = flowString.Split(';');

                    SamplerBottleNo = int.Parse(stringArray[0]);
                    WaterForMeasuringDevicesTimeRemaining = short.Parse(stringArray[2]);

                    FlowpHCurr = Decimal.Divide(int.Parse(stringArray[3]), 100);
                    FlowphMin =  Decimal.Divide(int.Parse(stringArray[4]), 100);
                    FlowphMed =  Decimal.Divide(int.Parse(stringArray[5]), 100);
                    FlowphMax =  Decimal.Divide(int.Parse(stringArray[6]), 100);
                    FlowTempCurr =  Decimal.Divide(int.Parse(stringArray[7]), 100);
                    FlowTempMin =  Decimal.Divide(int.Parse(stringArray[8]), 100);
                    FlowTempMed =  Decimal.Divide(int.Parse(stringArray[9]), 100);
                    FlowTempMax =  Decimal.Divide(int.Parse(stringArray[10]), 100);
                    FlowFlowCurr =  Decimal.Divide(int.Parse(stringArray[11]), 100); // TODO: zmiana pozycji i wprowadzenie nowych wartości dla CHZT
                    if (_chztOn)
                    {
                        FlowChztCurr = Decimal.Divide(int.Parse(stringArray[12]), 100);
                        FlowChztMin = Decimal.Divide(int.Parse(stringArray[13]), 100);
                        FlowChztMed = Decimal.Divide(int.Parse(stringArray[14]), 100);
                        FlowChztMax = Decimal.Divide(int.Parse(stringArray[15]), 100);
                    }

                    FlowFlowCnt = Decimal.Parse(Client.ReadValue(Client.Configuration.VarAddressFlowCnt));
                }
                catch
                {
                }
            }
            else
            {
                SamplerBottleNo = 0;
                WaterForMeasuringDevicesTimeRemaining = 0;

                FlowpHCurr = 10;
                FlowphMin = 5;
                FlowphMed = 10;
                FlowphMax = 15;
                FlowTempCurr = 10;
                FlowTempMin = 5;
                FlowTempMed = 10;
                FlowTempMax = 15;
                FlowFlowCurr = 100;

                if (_chztOn)
                {
                    FlowChztCurr = 555;
                    FlowChztMin = 555;
                    FlowChztMed = 555;
                    FlowChztMax = 555;
                }

                FlowFlowCnt = 1;
            }
        }
        
        public TransactionParameters CheckTransactionParameters()
        {
            var parameters = new TransactionParameters();
            parameters.FlowphMax = FlowphMax;
            //parameters.FlowphMed = FlowphMed;
            parameters.FlowphMed = FlowpHCurr; // <= w przypadku powrotu do Med tą linię usunąć i odkomentować tą powyżej
            parameters.FlowphMin = FlowphMin;
            parameters.FlowTempMax = FlowTempMax;
            //parameters.FlowTempMed = FlowTempMed;
            parameters.FlowTempMed = FlowTempCurr; // <= w przypadku powrotu do Med tą linię usunąć i odkomentować tą powyżej
            parameters.FlowTempMin = FlowTempMin;

            if (_chztOn)
            {
                parameters.FlowChztMax = FlowChztMax;
                parameters.FlowChztMed = FlowChztCurr; 
                parameters.FlowChztMin = FlowChztMin;
            }

            return parameters;
        }

        public Sample CheckScheduledSampleTaken()
        {
            if (ScheduledSampleTaken)
            {
                var sampleBottleNo = short.Parse(Client.ReadValue(Client.Configuration.VarAddressProbeScheduledBottleNo));
                return new Sample() { Type = SampleType.Scheduled, ProbeTime = DateTime.Now, BottleNo = sampleBottleNo };
            }
            return null;
        }
        public Sample CheckAlarmSampleTaken()
        {
            if (AlarmSampleTaken)
            {
                var sampleBottleNo = short.Parse(Client.ReadValue(Client.Configuration.VarAddressProbeAlarmBottleNo));
                return new Sample() { Type = SampleType.Alarm, ProbeTime = DateTime.Now, BottleNo = sampleBottleNo };
            }
            return null;
        }

        public ParameterOverrun CheckAlarmParameterOverrun()
        {
            var parOverrunStr = Client.ReadValue(Client.Configuration.VarAddressParameterOverrun);
            var parOverrun = uint.Parse(parOverrunStr);

            return (ParameterOverrun)Enum.Parse(typeof(ParameterOverrun), parOverrun.ToString());
        }

        public override bool Equals(object obj)
        {
            if (obj is Flow)
            {
                return FlowpHCurr == ((Flow)obj).FlowpHCurr 
                    && FlowphMin == ((Flow)obj).FlowphMin
                    && FlowphMed == ((Flow)obj).FlowphMed
                    && FlowphMax == ((Flow)obj).FlowphMax
                    && FlowTempCurr == ((Flow)obj).FlowTempCurr
                    && FlowTempMin == ((Flow)obj).FlowTempMin
                    && FlowTempMed == ((Flow)obj).FlowTempMed
                    && FlowTempMax == ((Flow)obj).FlowTempMax
                    && FlowChztCurr == ((Flow)obj).FlowChztCurr
                    && FlowChztMin == ((Flow)obj).FlowChztMin
                    && FlowChztMed == ((Flow)obj).FlowChztMed
                    && FlowChztMax == ((Flow)obj).FlowChztMax
                    && FlowFlowCurr == ((Flow)obj).FlowFlowCurr
                    && FlowFlowCnt == ((Flow)obj).FlowFlowCnt;
            }
            return false;
        }
    }
}

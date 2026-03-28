using System;

namespace DbCommunication.Entities
{
    [Serializable]
    public class Sample
    {
        public SampleType Type { get; set; }
        public DateTime ProbeTime { get; set; }
        public short BottleNo { get; set; }
    }
}

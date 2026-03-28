using DbCommunication.Enums;
using System;

namespace DbCommunication.Entities
{
    [Serializable]
    public class HarmonogramedSample
    {
        public int Id { get; set; }
        public bool TakeSample { get; set; }
        public HarmonogramType Type { get; set; }
    }
}

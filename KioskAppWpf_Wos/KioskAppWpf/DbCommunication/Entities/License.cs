using System;

namespace DbCommunication.Entities
{
    [Serializable]
    public class StationLicense
    {
        public int Id { get; set; }
        public string Number { get; set; }
        public DateTime DataStart { get; set; }
        public DateTime DataKoniec { get; set; }
        public int DaysTillEnd { get; set; }
    }
}

using System;

namespace DbCommunication.Entities.DbRows
{
    public class KioskPrzeplywomierzDbRow
    {
        public int Id { get; set; }
        public int KioskId { get; set; }
        public DateTime Data { get; set; }
        public int Odczyt { get; set; }
        public DateTime DataWpisu { get; set; }
    }
}


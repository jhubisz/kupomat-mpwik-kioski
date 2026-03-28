using System;

namespace DbCommunication.Entities.DbRows
{
    public class ProbaTackaParagonDbRow
    {
        public int Id { get; set; }
        public int KioskId { get; set; }
        public int TackaId { get; set; }

        public int LiczbaProbHarm { get; set; }
        public int LiczbaProbAlrm { get; set; }

        public bool Wydrukowano { get; set; }
        public DateTime? DataWydruku{ get; set; }
    }
}

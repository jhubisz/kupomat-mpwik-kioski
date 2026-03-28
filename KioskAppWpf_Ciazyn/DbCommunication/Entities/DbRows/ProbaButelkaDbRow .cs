using System;

namespace DbCommunication.Entities.DbRows
{
    public class ProbaButelkaDbRow
    {
        public int Id { get; set; }
        public int KioskId { get; set; }
        public int TackaId { get; set; }
        public int NumerButelki { get; set; }
        public bool ProbaPobrana { get; set; }
        public DateTime ProbaData { get; set; }
        public int ProbaTypId { get; set; }
        public int ProbaTransakcjaId { get; set; }
        public DateTime DataStart { get; set; }
    }
}

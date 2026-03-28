using System;

namespace DbCommunication.Entities.DbRows
{
    public class ProbaTackaDbRow
    {
        public int Id { get; set; }
        public int KioskId { get; set; }
        public int TackaNr { get; set; }
        public DateTime DataStart{ get; set; }
    }
}

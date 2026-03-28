using KioskAppWpf.Enums;
using System;
using System.Collections.Generic;

namespace DbCommunication.Entities.DbRows
{
    public class TransactionDbRow
    {
        public int Id { get; set; }

        public int KioskId { get; set; }

        public int RodzajSciekowId { get; set; }
        public int KlientId { get; set; }
        public int SamochodId { get; set; }
        public int KartaId { get; set; }
        public string NumerUmowy { get; set; }

        public int ZadeklarowanaIlosc { get; set; }
        public decimal ZlanaIlosc { get; set; }

        public DateTime TransakcjaStart { get; set; }
        public DateTime ZrzutStart { get; set; }
        public DateTime ZrzutKoniec { get; set; }
        public DateTime TransakcjaKoniec { get; set; }

        public bool PobranoProbeHarm { get; set; }
        public bool PobranoProbeAlrm { get; set; }

        public bool ZakonczonaPoprawnie { get; set; }
        public string PowodZakonczenia { get; set; }
    }
}

using DbCommunication.Enums;
using System;

namespace DbCommunication.Entities.DbRows
{
    public class KlientDbRow
    {
        public int ExportRowId { get; set; }
        public ExportRowType ExportRowType {get; set;}
        public ExportAction ExportAction { get; set; }

        public int Id { get; set; }
        public string Nazwa { get; set; }
        public string Nip { get; set; }
        public string Regon { get; set; }
        public string KodPocztowy { get; set; }
        public string Miejscowosc { get; set; }
        public string AdresLinia1 { get; set; }
        public string AdresLinia2 { get; set; }
        public string Tel1 { get; set; }
        public string Tel2 { get; set; }
        public string NumerUmowy { get; set; }
        public bool PobieranieProbek { get; set; }
        public bool Blokada { get; set; }

        public DateTime DataStart { get; set; }
        public DateTime? DataKoniec { get; set; }
        public bool Deleted { get; set; }
    }
}

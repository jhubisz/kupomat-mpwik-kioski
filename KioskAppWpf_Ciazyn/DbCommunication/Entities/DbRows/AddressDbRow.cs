using DbCommunication.Enums;
using System;

namespace DbCommunication.Entities.DbRows
{
    public class AdresDbRow
    {
        public int ExportRowId { get; set; }
        public ExportRowType ExportRowType { get; set; }
        public ExportAction ExportAction { get; set; }

        public int Id { get; set; }
        public int KioskId { get; set; }

        public string Nazwa { get; set; }
        public int? UlicaId { get; set; }
        public int MiejscowoscId { get; set; }
        public int GminaId { get; set; }
        public string Numer { get; set; }
        public string NumerUmowy { get; set; }
        public int RodzajId { get; set; }

        public DateTime DataStart { get; set; }
        public DateTime? DataKoniec { get; set; }
        public bool Deleted { get; set; }
    }
}

using DbCommunication.Enums;
using System;

namespace DbCommunication.Entities.DbRows
{
    public class FirmaDbRow
    {
        public int ExportRowId { get; set; }
        public ExportRowType ExportRowType { get; set; }
        public ExportAction ExportAction { get; set; }

        public int Id { get; set; }
        public int KioskId { get; set; }
        public string Nazwa { get; set; }

        public DateTime DataStart { get; set; }
        public DateTime? DataKoniec { get; set; }
        public bool Deleted { get; set; }
    }
}

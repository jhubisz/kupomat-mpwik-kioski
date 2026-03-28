using DbCommunication.Enums;
using System;

namespace DbCommunication.Entities.DbRows
{
    public class SamochodDbRow
    {
        public int ExportRowId { get; set; }
        public ExportRowType ExportRowType { get; set; }
        public ExportAction ExportAction { get; set; }

        public int Id { get; set; }
        public int KlientId{ get; set; }
        public string Rejestracja { get; set; }

        public DateTime DataStart { get; set; }
        public DateTime? DataKoniec { get; set; }
        public bool Deleted { get; set; }
    }
}

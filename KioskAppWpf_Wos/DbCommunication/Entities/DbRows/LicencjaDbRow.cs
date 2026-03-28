using DbCommunication.Enums;
using System;

namespace DbCommunication.Entities.DbRows
{
    public class LicencjaDbRow
    {
        public int ExportRowId { get; set; }
        public ExportRowType ExportRowType { get; set; }
        public ExportAction ExportAction { get; set; }

        public int Id { get; set; }
        public int KlientId { get; set; }
        public int LokalizacjaId { get; set; }
        public string LicencjaNumer { get; set; }
        
        public DateTime DataStart { get; set; }
        public DateTime? DataKoniec { get; set; }
        public bool Deleted { get; set; }
    }
}

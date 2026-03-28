using DbCommunication.Enums;
using System;

namespace DbCommunication.Entities.DbRows
{
    public class KartaDbRow
    {
        public int ExportRowId { get; set; }
        public ExportRowType ExportRowType { get; set; }
        public ExportAction ExportAction { get; set; }

        public int Id { get; set; }
        public string KartaId { get; set; }
        public bool KartaWymianyTacki { get; set; }
        public bool KartaToiToi { get; set; }
        public decimal PojemnoscToiToi { get; set; }
        public int? SamochodId { get; set; }
        public bool? Blokada { get; set; }

        public DateTime DataStart { get; set; }
        public DateTime? DataKoniec { get; set; }
        public bool Deleted { get; set; }
    }
}

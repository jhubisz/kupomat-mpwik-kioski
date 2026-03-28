using DbCommunication.Enums;
using System;

namespace DbCommunication.Entities.DbRows
{
    public class HarmonogramDostawcaDbRow
    {
        public int ExportRowId { get; set; }
        public ExportRowType ExportRowType { get; set; }
        public ExportAction ExportAction { get; set; }

        public int Id { get; set; }
        public int HarmonogramId { get; set; }
        public int DostawcaId { get; set; }

        public bool Deleted { get; set; }
    }
}

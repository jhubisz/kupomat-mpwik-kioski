using DbCommunication.Enums;
using System;

namespace DbCommunication.Entities.DbRows
{
    public class CargoTypeDbRow
    {
        public int ExportRowId { get; set; }
        public ExportRowType ExportRowType { get; set; }
        public ExportAction ExportAction { get; set; }

        public int Id { get; set; }
        public string Name { get; set; }
        public string CargoCode { get; set; }

        public DateTime DateStart { get; set; }
        public DateTime? DateEnd { get; set; }
        public bool Deleted { get; set; }
    }
}

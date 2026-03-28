using DbCommunication.Enums;
using System;

namespace DbCommunication.Entities.DbRows
{
    public class HarmonogramDbRow
    {
        public int ExportRowId { get; set; }
        public ExportRowType ExportRowType { get; set; }
        public ExportAction ExportAction { get; set; }

        public int Id { get; set; }
        public int KioskId { get; set; }
        public DateTime Data { get; set; }
        public int? TypPoboruId { get; set; }
        public int? TypSciekowId { get; set; }
        public int? AdresWytworcyId { get; set; }
        public int? AdresGminaId { get; set; }
        public int? AdresMiejscowoscId { get; set; }
        public int? AdresUlicaId { get; set; }
        public string AdresNumer { get; set; }
        public int? LiczbaPoborow{ get; set; }

        public DateTime? DataStart { get; set; }
        public DateTime? DataKoniec { get; set; }
        public DateTime DataDodania { get; set; }
    }
}

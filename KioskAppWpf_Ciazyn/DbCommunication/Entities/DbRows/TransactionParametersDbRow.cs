using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbCommunication.Entities.DbRows
{
    public class TransactionParametersDbRow
    {
        public int Id { get; set; }

        public int KioskId { get; set; }

        public int TransakcjaId { get; set; }

        public decimal? PhMin { get; set; }
        public decimal? PhMax { get; set; }
        public decimal? PhAvg { get; set; }
        public decimal? PrzewodnoscMin { get; set; }
        public decimal? PrzewodnoscMax { get; set; }
        public decimal? PrzewodnoscAvg { get; set; }
        public decimal? TempMin { get; set; }
        public decimal? TempMax { get; set; }
        public decimal? TempAvg { get; set; }
        public decimal? ChztMin { get; set; }
        public decimal? ChztMax { get; set; }
        public decimal? ChztAvg { get; set; }
        public int? Cisnienie { get; set; }
    }
}

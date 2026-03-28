using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbCommunication.Entities.DbRows
{
    public class TransactionAddressDbRow
    {
        public int Id { get; set; }

        public int KioskId { get; set; }

        public int TransakcjaId { get; set; }
        public decimal ZadeklarowanaIlosc { get; set; }
        public string NumerUmowy { get; set; }

        public int? AdresId { get; set; }
        public int? FirmaId { get; set; }
        public int? RodId { get; set; }
        public string RodNrDzialki { get; set; }
    }
}

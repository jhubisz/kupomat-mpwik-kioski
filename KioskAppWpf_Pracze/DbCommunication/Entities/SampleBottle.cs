using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbCommunication.Entities
{
    public class SampleBottle
    {
        public int BottleId { get; set; }
        public int BottleNo { get; set; }
        public DateTime ProbeTakenTime { get; set; }
        public string ProbeType { get; set; }
        public int TransactionId { get; set; }
        public string SampleType { get; set; }
        public string VechicleRegistration { get; set; }
        public string RfidCardId { get; set; }

        public string CustomerName { get; set; }
        public string Addr1 { get; set; }
        public string Addr2 { get; set; }
        public string PostCode { get; set; }
        public string City { get; set; }

        public List<CustomerAddress> Addresses { get; set; }
    }
}

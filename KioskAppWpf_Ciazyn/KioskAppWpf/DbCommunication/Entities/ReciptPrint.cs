using System;

namespace DbCommunication.Entities
{
    public class ReciptPrint
    {
        public string ReciptNo { get; set; }
        public string CompanyAddressLine1 { get; set; }
        public string CompanyAddressLine2 { get; set; }
        public string CompanyAddressLine3 { get; set; }

        public string StationAddressLine1 { get; set; }
        public string StationAddressLine2 { get; set; }
        public string StationAddressLine3 { get; set; }

        public string CustomerAddressName { get; set; }
        public string CustomerAddressAddrLine1 { get; set; }
        public string CustomerAddressAddrLine2 { get; set; }
        public string CustomerAddressAddrLine3 { get; set; }
        public string CustomerAddressPostCode { get; set; }
        public string CustomerAddressCity { get; set; }
        public string CustomerAddressContractNo { get; set; }

        public string TranctorLicensePlate { get; set; }
        public string TrailerLicensePlate { get; set; }

        public string CargoType { get; set; }
        public string CargoCode { get; set; }

        public DateTime EntryTime { get; set; }
        public decimal EntryWeight { get; set; }
        public DateTime ExitTime { get; set; }
        public decimal ExitWeight { get; set; }

        public decimal NetWeight
        {
            get
            {
                if (EntryWeight > ExitWeight)
                    return EntryWeight - ExitWeight;
                else
                    return ExitWeight - EntryWeight;
            }
        }

        public string RfidCard { get; set; }
    }
}

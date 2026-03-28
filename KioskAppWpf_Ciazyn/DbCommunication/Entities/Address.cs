using System;

namespace DbCommunication.Entities
{
    [Serializable]
    public class Address
    {
        public int Id { get; set; }
        public Customer Customer { get; set; }

        public string Name { get; set; }
        public string Regon { get; set; }
        public string Nip { get; set; }
        public string PostCode { get; set; }
        public string City { get; set; }

        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }

        public string Tel1 { get; set; }
        public string Tel2 { get; set; }

        public string ContractNo { get; set; }
    }
}

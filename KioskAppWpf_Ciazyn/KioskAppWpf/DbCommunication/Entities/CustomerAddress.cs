using DbCommunication.Enums;
using System;

namespace DbCommunication.Entities
{
    [Serializable]
    public class CustomerAddress
    {
        public int? Id { get; set; }

        public Customer RegularAddressFor { get; set; }

        public Company Company { get; set; }
        public string Name { get; set; }

        public int GminaId { get; set; }
        public string GminaName { get; set; }

        public int MiejscowoscId { get; set; }
        public string MiejscowoscName { get; set; }

        public int UlicaId { get; set; }
        public string UlicaName { get; set; }

        public int RodId { get; set; }
        public string RodName { get; set; }

        public string AddressNumber { get; set; }

        public string ContractNo { get; set; }

        public decimal DeclaredAmount { get; set; }
        public decimal ActualAmount { get; set; }

        public AddressDbType DbType { get; set; }
    }
}

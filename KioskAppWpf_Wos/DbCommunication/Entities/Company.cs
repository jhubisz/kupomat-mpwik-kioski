using System;

namespace DbCommunication.Entities
{
    [Serializable]
    public class Company
    {
        public int? Id { get; set; }
        public string Name { get; set; }

        [NonSerialized]
        private CustomerAddress address;
        public CustomerAddress Address
        {
            get { return address; }
            set { address = value; }
        }

        public override string ToString()
        {
            var address = "";
            if (Address.UlicaName != "")
            {
                address = Address.UlicaName + " " + Address.AddressNumber + ", " + Address.MiejscowoscName;
            }
            else
            {
                address = Address.MiejscowoscName + " " + Address.AddressNumber;
            }
            return Name + " (" + address + ")";
        }
    }
}

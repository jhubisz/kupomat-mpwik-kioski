using System;

namespace DbCommunication.Entities
{
    [Serializable]
    public class Vechicle
    {
        public int Id { get; set; }
        public Customer Customer { get; set; }
        public string LicensePlate { get; set; }
        public string TrailerLicensePlate { get; set; }
    }
}

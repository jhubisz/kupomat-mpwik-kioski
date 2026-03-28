using System;

namespace DbCommunication.Entities
{
    [Serializable]
    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool TakingSamples { get; set; }
        public bool Blocked { get; set; }
        public Address Address { get; set; }
    }
}

using System;

namespace DbCommunication.Entities
{
    [Serializable]
    public class RfidCard
    {
        public int Id { get; set; }
        public string CardId { get; set; }
        public RfidCardType Type { get; set; }

        public bool Blocked
        {
            get
            {
                if (Type == RfidCardType.Superuser)
                    return false;
                return BlockedCard || Customer.Blocked;
            }
        }
        public bool BlockedCard { get; set; }
        
        public Vechicle Vechicle { get; set; }
        public Customer Customer { get; set; }
        public Address Address { get; set; }
    }
}

using System;

namespace DbCommunication.Entities
{
    [Serializable]
    public enum RfidCardType
    {
        Unknown = 0,
        Customer = 1,
        Superuser
    }
}

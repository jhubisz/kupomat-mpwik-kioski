using System;

namespace DbCommunication.Enums
{
    [Serializable]
    public enum HarmonogramType
    {
        DumpOrder = 1,
        SewageType,
        Company,
        CompanyAndSewageType,
        CustomerAddress,
        Parameters,
        TimeStamp
    }
}

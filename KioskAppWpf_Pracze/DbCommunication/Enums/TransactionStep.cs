using System;

namespace KioskAppWpf.Enums
{
    [Serializable]
    public enum TransactionStep
    {
        NoTransaction,
        TransactionCreated,
        
        ReciptPrint
    }
}

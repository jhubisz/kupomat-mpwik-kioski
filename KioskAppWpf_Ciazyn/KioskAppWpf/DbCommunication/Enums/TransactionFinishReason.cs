using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbCommunication.Enums
{
    [Serializable]
    public enum TransactionFinishReason
    {
        Unexpected,
        TransactionFinishedCorrectly,
        ParametersExceeded, 
        ScreenCleaningTimeout,
        DeviceError,
        NoPlcCommunication,
        PathChangingTimeout,
        ParameterExceededPh,
        ParameterExceededTemp,
        ParameterExceededChzt,
        ParameterExceededPressure
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbCommunication.Enums
{
    public enum KioskConfigurationType
    {
        WaitTimeForEmptyScreen = 1,
        WaitTimeForInitialFlow,
        WaitTimeForEndOfFlow,
        TimeForWaterForDrivers,
        TimeForWaterForMeasuringDevices,
        ProbeStartAfterPercent,
        ProbeMinimalAfterStartL,
        Probe1AfterStartPercent,
        Probe2AfterStartPercent,
        Probe3AfterStartPercent,
        PressureFactor,
        PressureMaxTime,
    }
}

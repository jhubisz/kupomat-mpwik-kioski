namespace DbCommunication.Enums
{
    public enum KioskBlockageType
    {
        TakingSamples = 1,
        MeasuringPh,
        MeasuringConduction,
        MeasuringTemperature,
        Printer,
        WaterForDrivers,
        WaterForMeasuringDevices,
        StationGate,
        MeasuringChzt,
        ToiletSteering,
        CameraOpcRegistration,
        Keyboard,
        Zasuwa,
        GatesSteering,
        ProbeBottles, // Blokada zgłaszania awarii końca butelek w pobieraku
        ProbeError, // Blokada zgłaszania awarii pobieraka
        MeasuringPressure,
        PressureBlockageOn,
    }
}

namespace OpcCommunication.Enums
{
    public enum StopCode
    {
        SeparatorErr,
        CanalFloodedErr,

        PHMeasureErr,
        TempMeasureErr,
        ChztErr,
        PressureErr,
        FlowErr,

        ProbeWashErr,
        ProbeWashNoAuto,

        LockErr,
        LockNoAuto,

        VDC24Err,
        VAC230Err,
        AirErr,
        KioskOpen,

        PathNotOpen,
        S7PCSCommErr,
        PathNotOpen2,

        ProbeErr,
        ProbeNotReady,
        PressureOverage,

        ScreenFul,
        PathAutoErr,
        PathErr,

        PCSUPSActive,
        PCS230VACErr,
        PCS24VDCErr,
        PCS12VDCErr,
        PCSUPSErr,
    }
}

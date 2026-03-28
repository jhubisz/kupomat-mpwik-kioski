namespace OpcCommunication.Enums
{
    public enum PermitCode
    {
        PhMeasureErr,
        FlowErr,
        TempMeasureErr,
        HuberZSPErr,

        SepErr,
        PathErr,
        ChztMeasureErr,
        PressureMeasureErr,

        VDC24Err,
        VAC230Err,

        KioskDoorOpen,
        AirErr,
        DumpLockNoAuto,
        DumpLockErr,
        AutoFlushErr,
        FlushLockErr,

        S7Kiosk,
        S7PCS,

        ProbeErr,
        ProbeNoBottle,
        ProbeNoAuto,

        Canal1Floode,
        Canal2Flooded,

        PCSVoltageProtErr,
        PCS230VACErr,
        PCS24VDCErr,
        PCS12VDCErr,
        PCS400VACErr,
    }
}

namespace KioskAppWpf.Enums
{
    public enum FlowState
    {
        SequenceFinished,
        SequenceFinishedTimeout,
        SequenceResetConfirmation,
        SequenceNoFlow,
        SequenceNoFlowTimeout,
        SequenceNoFlowResetConfirmation,

        SequenceThankYouPage,

        SequenceScreenCleaningStart,
        SequenceScreenCleaningEnd,
        SequenceScreenCleaningTimeout,

        SequenceErrorAlarm,
        SequenceErrorDevice,
        SequenceErrorNoFlow,
        SequenceErrorScreenFull,

        GateOpened,

        DumpPathChanged,
    }
}

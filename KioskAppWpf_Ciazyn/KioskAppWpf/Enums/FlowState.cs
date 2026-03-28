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
        
        SequencePathChangingStart,
        SequencePathChangingEnd,
        SequencePathChangingTimeout,

        SequenceErrorAlarm,
        SequenceErrorDevice,
        SequenceErrorNoFlow,
        SequenceErrorScreenFull,

        GateOpened,

        DumpPathChanged,
    }
}

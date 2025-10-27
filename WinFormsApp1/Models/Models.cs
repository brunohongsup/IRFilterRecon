
// File: Models/Enums.cs
namespace WinFormsApp1.Models
{
    public enum WorkerState { Initializing, Running, Stopped, StopFailed }
    public enum OutState { Ready, Busy }
    public enum InState { Ready, Run, Busy }

    public enum EPosState { Unknown, IsSafety, Critical }
    public enum EForeWorkerKey 
    { 
        LoadStage,
        LoadFeeder,
        LoadMagazine,

        SortStage,
        SortFeeder,
        SortMagazine,

        EjectUnit,
        PickerUnit,

        LoadTopCam,
        SortTopCam,

        PNP_Handler,

        Num
        
    }
    public enum EFrameAlignSel { LeftTop, RightBtm, RightTop, LeftBtm, Center, Num }
    public enum EFrameFilterType { Outer, Inner }
}

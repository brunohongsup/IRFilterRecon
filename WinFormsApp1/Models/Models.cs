
// File: Models/Enums.cs
namespace WinFormsApp1.Models
{
    public enum WorkerState { Initializing, Running }
    public enum OutState { Ready, Busy }
    public enum InState { Ready, Run, Busy }

    public enum EPosState { Unknown, IsSafety, Critical }
    public enum EForeWorkerKey { LoadStage, SortStage, EjectUnit }
    public enum EFrameAlignSel { LeftTop, RightBtm, RightTop, LeftBtm, Center, Num }
    public enum EFrameFilterType { Outer, Inner }
}

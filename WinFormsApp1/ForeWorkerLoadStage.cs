using System;
using System.Threading;
using System.Threading.Tasks;
using WinFormsApp1;
using WinFormsApp1.ForegroundWorker;
using WinFormsApp1.Models;

public class CFW_LoadStage : ForeWorkerBase
{
    private EForeWorkerKey _keyType;
    private bool _lockControl;
    private AlignResult[] _alignResults;
    private EPosState _posState;
    private bool _dryRunFrameComplete;
    private EFrameFilterType _sortingFilterType;
    private bool _silentStopFlag;

    public CFW_LoadStage(EForeWorkerKey key)
        : base(key.ToString())
    {
        _keyType = key;
        _lockControl = false;
        _posState = EPosState.Unknown;
        _silentStopFlag = false;
        _alignResults = new AlignResult[(int)EFrameAlignSel.Num];
    }

    public bool LockControl { get => _lockControl; set => _lockControl = value; }
    public bool SilentStopFlag => _silentStopFlag;
    public EPosState PosState => _posState;
    public void SetDryRunFrameComplete(bool complete) => _dryRunFrameComplete = complete;

    public override async Task Work(CancellationToken token)
    {
        OnStatus($"[{_keyType}] Worker started.");
        int lastStep = -1;

        try
        {
            while (!token.IsCancellationRequested)
            {
                // track changes in step
                if (lastStep != Step)
                {
                    lastStep = Step;
                    _silentStopFlag = false;
                }
                else
                {
                    _silentStopFlag = true;
                }

                switch (Step)
                {
                    case 10:
                        _input = InState.Busy;
                        _output = OutState.Busy;
                        ClearFrameAlignResult();
                        _posState = EPosState.Unknown;
                        OnStatus("Initializing alignment...");
                        Next(20);
                        break;

                    case 20:
                        OnStatus("Aligning frame...");
                        await DelayAsync(200, token);
                        Next(30);
                        break;

                    case 30:
                        OnStatus("Alignment complete.");
                        _posState = EPosState.IsSafety;
                        _output = OutState.Ready;
                        _input = InState.Ready;
                        Next(100);
                        break;

                    case 100:
                        await DelayAsync(100, token);
                        break;
                }

                await Task.Delay(10, token);
            }
        }
        catch (OperationCanceledException)
        {
            OnStatus($"[{_keyType}] Worker cancellation detected.");
        }
        finally
        {
            _output = OutState.Busy;
            _input = InState.Busy;
            _posState = EPosState.Unknown;
            OnStatus($"[{_keyType}] Worker stopped cleanly.");
        }
    }

    // Helper method to support token cancellation
    private async Task DelayAsync(int milliseconds, CancellationToken token)
    {
        try
        {
            await Task.Delay(milliseconds, token);
        }
        catch (TaskCanceledException)
        {
            // Expected cancellation path — no error
        }
    }

    private void ClearFrameAlignResult()
    {
        for (int i = 0; i < _alignResults.Length; i++)
            _alignResults[i] = new AlignResult();
    }
}

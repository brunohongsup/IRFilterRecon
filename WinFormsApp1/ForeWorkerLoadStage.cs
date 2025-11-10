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
    private readonly Form1 _mainForm;

    public CFW_LoadStage(EForeWorkerKey key, Form1 form)
        : base(key.ToString())
    {
        _keyType = key;
        _lockControl = false;
        _posState = EPosState.Unknown;
        _silentStopFlag = false;
        _alignResults = new AlignResult[(int)EFrameAlignSel.Num];
        _mainForm = form;
    }

    public bool LockControl { get => _lockControl; set => _lockControl = value; }
    public bool SilentStopFlag => _silentStopFlag;
    public EPosState PosState => _posState;
    public void SetDryRunFrameComplete(bool complete) => _dryRunFrameComplete = complete;

    private List<InspectionTask> _inspectionTasks = new List<InspectionTask>();

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
                        var productManager = ProductManager.Instance;
                        productManager.CreateNewProduct();
                        OnStatus("Initializing alignment...");
                        Next(20);
                        break;

                    case 20:
                        OnStatus("Aligning frame...");
                        //ToDo Get Align Data
                        await Task.Delay(200, token);
                        Next(30);
                        break;

                    case 30:
                        OnStatus("Alignment complete.");
                        var threadpool = Threadpool.Instance;
                        for(int i = 0; i < 200; i++)
                        {
                            var inspection = new InspectionTask();
                            threadpool.AddWork(inspection);
                            _inspectionTasks.Add(inspection);
                        }
                        
                        _posState = EPosState.IsSafety;
                        _output = OutState.Ready;
                        _input = InState.Ready;
                        Next(100);
                        break;

                    case 100:
                        bool allCompleted = true;
                        foreach (var insp in _inspectionTasks)
                        {
                            if (!insp.IsCompleted)
                            {
                                allCompleted = false;
                                break;
                            }
                        }

                        if (!allCompleted)
                        {
                            await Task.Delay(10, token);
                        }

                        else
                        {
                            OnStatus("All Task Done");
                            _inspectionTasks.Clear();
                            Next(10);
                        }

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
    private void ClearFrameAlignResult()
    {
        for (int i = 0; i < _alignResults.Length; i++)
            _alignResults[i] = new AlignResult();
    }
}

namespace WinFormsApp1.ForegroundWorker;
using System;
using System.Threading;
using System.Threading.Tasks;
using WinFormsApp1.Models;

public class ForeWorkerStatusEventArgs : EventArgs
{
    public string Message { get; }
    public DateTime Timestamp { get; }

    public ForeWorkerStatusEventArgs(string message)
    {
        Message = message;
        Timestamp = DateTime.Now;
    }
}

public abstract class ForeWorkerBase
{
    public event EventHandler<ForeWorkerStatusEventArgs>? StatusUpdated;

    protected void OnStatus(string msg)
    {
        StatusUpdated?.Invoke(this, new ForeWorkerStatusEventArgs(msg));
    }

    private bool _running;
    
    private bool _lockStop;
    
    private CancellationTokenSource? _cts;
    
    private Task? _workerTask;
    
    private readonly string _key;

    protected WorkerState _state;
    protected OutState _output;
    protected InState _input;
    protected int _curStep;
    protected int _lastStep;

    protected ForeWorkerBase(string key)
    {
        _key = key;
        _running = false;
        _lockStop = false;
        _state = WorkerState.Initializing;
        _output = OutState.Busy;
        _input = InState.Busy;
        _curStep = 10;
        _lastStep = 10;
    }

    public bool Running => _running;
    public WorkerState State => _state;
    public OutState Output => _output;
    public InState Input => _input;
    public int Step => _curStep;
    public bool IsLockStop => _lockStop;

    protected void LockStop() => _lockStop = true;
    protected void UnlockStop() => _lockStop = false;

    protected void Next(int step)
    {
        _lastStep = _curStep;
        _curStep = step;
    }

    protected async Task Delay(int milliseconds, CancellationToken token)
    {
        try
        {
            await Task.Delay(milliseconds, token);
        }
        catch (TaskCanceledException)
        {
            // expected when stopped normally
        }
    }

    public virtual void ResetStep()
    {
        _lastStep = 10;
        _curStep = 10;
        _lockStop = false;
        _state = WorkerState.Initializing;
    }

    public virtual void StartWorker()
    {
        StopWorker(); // Ensure no previous worker running
        ResetStep();

        _cts = new CancellationTokenSource();
        var token = _cts.Token;

        _running = true;
        _state = WorkerState.Running;
        OnStatus($"Worker [{_key}] started.");

        _workerTask = Task.Run(async () =>
        {
            try
            {
                await Work(token);
            }
            catch (OperationCanceledException)
            {
                OnStatus($"Worker [{_key}] cancelled.");
            }
            catch (Exception ex)
            {
                OnStatus($"Worker [{_key}] exception: {ex.Message}");
            }
            
            finally
            {
                _running = false;
                _state = WorkerState.Initializing;
                OnStatus($"Worker [{_key}] stopped.");
            }
        }, token);
    }

    public virtual async void StopWorker()
    {
        if (_workerTask == null || _cts == null)
            return;

        while (_lockStop)
            await Task.Delay(10);

        _cts.Cancel();

        try
        {
            await Task.WhenAny(_workerTask, Task.Delay(2000));
        }
        catch (Exception ex)
        {
            OnStatus($"Error stopping worker [{_key}]: {ex.Message}");
        }

        _workerTask = null;
        _cts.Dispose();
        _cts = null;
        _running = false;
        _state = WorkerState.Initializing;
    }

    public virtual void EMGStopWorker()
    {
        _cts?.Cancel();
        _workerTask = null;
        _cts = null;
        _running = false;
        _state = WorkerState.Initializing;
        OnStatus($"Emergency stop for worker [{_key}].");
    }

    // Derived classes must implement token-aware logic
    public abstract Task Work(CancellationToken token);
}

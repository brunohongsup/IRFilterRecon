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
    
    private readonly SemaphoreSlim _startStopLock = new SemaphoreSlim(1, 1);

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

    public virtual async Task StartWorker()
    {
        // 1. [수정] 동시 접근을 막기 위해 락 획득
        await _startStopLock.WaitAsync();

        try
        {
            // 2. [수정] 이전 워커가 완전히 멈출 때까지 "대기"
            // (StopWorker가 타임아웃되어도, 여기서 새 작업을 시작하는 것이 현재 로직)
            await StopWorker();

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
                    // 실제 작업 수행
                    await Work(token);
                }
                catch (OperationCanceledException)
                {
                    // 취소는 정상 종료로 간주
                    OnStatus($"Worker [{_key}] cancelled.");
                }
                catch (Exception ex)
                {
                    // 예외 발생
                    OnStatus($"Worker [{_key}] exception: {ex.Message}");
                }
                finally
                {
                    // 3. [수정] 'Initializing'이 아닌 'Stopped'가 논리적으로 맞음
                    _running = false;
                    _state = WorkerState.Stopped;
                    OnStatus($"Worker [{_key}] stopped.");
                }
            }, token); // 토큰을 Task.Run에도 전달
        }
        finally
        {
            // 4. [수정] 모든 로직(심지어 예외 발생 시)이 끝나면 락 해제
            _startStopLock.Release();
        }
    }

   // [권장] _lockStop 대신 클래스 멤버로 SemaphoreSlim 사용

public virtual async Task StopWorker()
{
    try
    {
        if (_workerTask == null || _cts == null)
            return;

        // 2. 취소 신호 전송
        OnStatus($"Stopping worker: {_key}...");
        _cts.Cancel();

        // 3. 워커가 종료될 때까지 2초간 대기
        var completedTask = await Task.WhenAny(_workerTask, Task.Delay(2000));

        if (completedTask == _workerTask)
        {
            // --- 4A. 성공: 워커가 2초 내에 정상 종료 ---
            try
            {
                // Task의 최종 상태(및 예외)를 관찰
                await _workerTask; 
            }
            catch (OperationCanceledException)
            {
                // 이것이 예상된 정상적인 종료 경로입니다.
                OnStatus($"Worker [{_key}] stopped gracefully.");
            }
            catch (Exception ex)
            {
                // 워커가 취소가 아닌 다른 예외로 인해 중단되었습니다.
                OnStatus($"Error stopping worker [{_key}]: {ex.Message}");
            }
            finally
            {
                // [안전] 태스크가 완료되었으므로 리소스 정리
                _workerTask.Dispose();
                _workerTask = null;
                _cts.Dispose();
                _cts = null;
                _running = false;
                _state = WorkerState.Stopped; // 'Stopped'가 더 명확
            }
        }
        else
        {
            // --- 4B. 실패: 2초 타임아웃 발생 ---
            // _workerTask는 여전히 실행 중입니다.
            OnStatus($"Warning: Worker [{_key}] did not stop within 2 seconds. Task is being orphaned.");
            
            // [중요] 리소스를 절대 정리(Dispose)하면 안 됩니다.
            // 대신, 태스크가 '멈추는 중'이거나 '실패'했음을 알리는 상태가 필요할 수 있습니다.
            _state = WorkerState.StopFailed; // 예시 상태
        }
    }
    catch (Exception ex)
    {
        // StopWorker 메서드 자체의 로직에서 발생한 예외
        OnStatus($"Critical error during StopWorker execution: {ex.Message}");
    }
    finally
    {
        // 5. 비동기 락 해제
        _startStopLock.Release();
    }
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

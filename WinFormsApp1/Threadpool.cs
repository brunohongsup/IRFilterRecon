namespace WinFormsApp1;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public class Threadpool : IDisposable
{
    private static readonly Lazy<Threadpool> _instance = new Lazy<Threadpool>(() =>
    {
        var tp = new Threadpool();
        tp.Init();
        return tp;
    });
    
    public static Threadpool Instance => _instance.Value;

    private readonly int _threadCount;
    private readonly List<Task> _workers = new List<Task>();
    private readonly BlockingCollection<ITask> _tasks = new BlockingCollection<ITask>();
    private CancellationTokenSource _cts = new CancellationTokenSource();

    private Threadpool()
    {
        _threadCount = Math.Max(Environment.ProcessorCount - 4, 4);
    }

    private void Init()
    {
        for (int i = 0; i < _threadCount; ++i)
        {
            var task = Task.Factory.StartNew(
                WorkerLoop,
                _cts.Token, 
                TaskCreationOptions.LongRunning, 
                TaskScheduler.Default
            );
            
            _workers.Add(task);
        }
    }

    public bool AddWork(ITask task)
    {
        if (_cts.IsCancellationRequested)
            return false;
        
        _tasks.Add(task);
        return true;
    }

    private void WorkerLoop()
    {
        try
        {
            foreach (var task in _tasks.GetConsumingEnumerable(_cts.Token))
            {
                task.Run();
            }
        }
        
        catch (OperationCanceledException)
        {
            // Exit loop when cancelled
        }
    }

    public void CleanUp()
    {
        _cts.Cancel();
        _tasks.CompleteAdding();
        Task.WaitAll(_workers.ToArray());
        _workers.Clear();
        _cts.Dispose();
        _cts = new CancellationTokenSource();
    }

    public void Dispose()
    {
        CleanUp();
        _tasks.Dispose();
    }
}

using WinFormsApp1.ForegroundWorker;
using WinFormsApp1.Models;

namespace WinFormsApp1;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public sealed class WorkersManager
{
    private static readonly Lazy<WorkersManager> _instance = new(() => new WorkersManager());
    public static WorkersManager Instance => _instance.Value;

    private readonly Dictionary<EForeWorkerKey, ForeWorkerBase> _workers;
    private readonly object _lock = new();

    public event EventHandler<ForeWorkerStatusEventArgs>? StatusBroadcast;

    private WorkersManager()
    {
        _workers = new Dictionary<EForeWorkerKey, ForeWorkerBase>();
    }

    // Register new worker
    public void RegisterWorker(EForeWorkerKey key, ForeWorkerBase worker)
    {
        lock (_lock)
        {
            if (!_workers.ContainsKey(key))
            {
                _workers[key] = worker;
                worker.StatusUpdated += Worker_StatusUpdated;
                OnStatus($"Worker [{key}] registered.");
            }
            else
            {
                OnStatus($"Worker [{key}] already registered.");
            }
        }
    }

    // Start single worker
    // 1. [수정] 메서드를 비동기(async)로 변경하고 Task를 반환하도록 합니다.
    public async Task StartWorker(EForeWorkerKey key)
    {
        // 2. [전제] _workers는 ConcurrentDictionary<EForeWorkerKey, ForeWorker>라고 가정합니다.
        if (_workers.TryGetValue(key, out var worker))
        {
            OnStatus($"Starting worker: {key}");
        
            // 3. [수정] 워커의 StartWorker가 완료될 때까지 "대기"합니다.
            // 이는 내부의 StopWorker 로직이 완료되기를 보장합니다.
            await worker.StartWorker(); 
        }
    }

    // Stop single worker
    public async Task StopWorker(EForeWorkerKey key)
    {
        if (_workers.TryGetValue(key, out var worker))
        {
            OnStatus($"Stopping worker: {key}");
            await worker.StopWorker();
        }
    }

    // Emergency stop all workers
    public void EmergencyStopAll()
    {
        lock (_lock)
        {
            foreach (var worker in _workers.Values)
                worker.EMGStopWorker();

            OnStatus("All workers emergency stopped.");
        }
    }

    // Start all workers
    public async Task StartAll()
    {
        foreach (var kvp in _workers)
        {
            OnStatus($"Starting [{kvp.Key}]...");
            await kvp.Value.StartWorker();
        }
    }

    // Stop all workers
    public async Task StopAllAsync()
    {
        List<Task> tasks = new();
        lock (_lock)
        {
            foreach (var kvp in _workers)
            {
                var k = kvp.Key;
                var w = kvp.Value;
                tasks.Add(Task.Run(() =>
                {
                    OnStatus($"Stopping [{k}]...");
                    w.StopWorker();
                }));
            }
        }
        
        await Task.WhenAll(tasks);
        OnStatus("All workers stopped.");
    }

    private void Worker_StatusUpdated(object? sender, ForeWorkerStatusEventArgs e)
    {
        StatusBroadcast?.Invoke(this, e);
    }

    private void OnStatus(string msg)
    {
        StatusBroadcast?.Invoke(this, new ForeWorkerStatusEventArgs(msg));
    }
}

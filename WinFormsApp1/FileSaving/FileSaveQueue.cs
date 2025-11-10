using System.Collections.Concurrent;

namespace WinFormsApp1;

public class FileSaver
{
    private static readonly Lazy<FileSaver> _instance =
        new Lazy<FileSaver>(() => new FileSaver(10000));
    
    private Queue<FileSaveJob> _queue;
    
    private readonly CancellationTokenSource _cts;

    private bool _running = false;


    public static FileSaver Instance
    {
        get
        {
            return _instance.Value;
        }
    }

    private FileSaver(int maxQueueSize = 1000)
    {
        _queue = new Queue<FileSaveJob>(maxQueueSize);
        _cts = new CancellationTokenSource();
    }

    public bool AddFile(FileSaveJob fileData)
    {
        lock(_queue)
        {
            _queue.Enqueue(fileData);
        }

        if (!_running)
        {
            _running = true;
            var processFiles = new RandomTask(ProcessFiles);
            Threadpool.Instance.AddWork(processFiles);
        }


        return true;
    }

    private void ProcessFiles()
    {
        while (_running)
        {
            Thread.Sleep(100);
            FileSaveJob? fileJob = null;
            lock(_queue)
            {
                if(_queue.Count == 0)
                    continue;

                fileJob = _queue.Dequeue();
            }

            fileJob.Save(); 
        }
    }

    public void Dispose()
    {
        _queue.Clear();
        _cts.Dispose();
    }
    
}
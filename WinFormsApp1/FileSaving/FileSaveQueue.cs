using System.Collections.Concurrent;

namespace WinFormsApp1;

public class FileSaver
{
    private static readonly Lazy<FileSaver> _instance =
        new Lazy<FileSaver>(() => new FileSaver(10000));
    
    private readonly BlockingCollection<FileSaveJob> _queue;
    
    private readonly Task _worker;
    
    private readonly CancellationTokenSource _cts;

    public static FileSaver Instance
    {
        get
        {
            return _instance.Value;
        }
    }

    private FileSaver(int maxQueueSize = 1000)
    {
        _queue = new BlockingCollection<FileSaveJob>(maxQueueSize);
        _cts = new CancellationTokenSource();
        _worker = Task.Run(() => ProcessFiles());
    }

    public void AddFile(FileSaveJob fileData)
    {
        _queue.Add(fileData);
    }

    private void ProcessFiles()
    {
        foreach (var file in _queue.GetConsumingEnumerable(_cts.Token))
        {
            try
            {
                file.Save();
            }
            
            catch (Exception ex)
            {
                // Log the error - don't let it kill the worker
                Console.WriteLine($"Failed to save {file.FilePath}: {ex.Message}");
            }
        }
    }

    public void Dispose()
    {
        _queue.CompleteAdding();  // Stop accepting new items
        _worker.Wait();            // Wait for queue to drain
        _queue.Dispose();
        _cts.Dispose();
    }
    
}
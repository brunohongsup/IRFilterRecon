
using WinFormsApp1;

public class RandomTask(Action work) : ITask
{
    private readonly Action _work = work ?? throw new ArgumentNullException(nameof(work));

    public void Run()
    {
        _work();
    }
}
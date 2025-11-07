namespace WinFormsApp1;

public class InspectionTask : ITask
{
    public bool IsCompleted => _isCompleted;

    private bool _isCompleted;

    public InspectionTask()
    {
        _isCompleted = false;
    }
    public override string ToString()
    {
        return "Inspection Task";
    }

    public void Run()
    {
        Thread.Sleep(1);
        _isCompleted = true;
    }
}
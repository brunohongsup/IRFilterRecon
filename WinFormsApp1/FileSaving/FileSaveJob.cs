namespace WinFormsApp1;

public abstract class FileSaveJob
{
    public required string FilePath { get; set; }
    
    public abstract void Save();
}

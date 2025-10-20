namespace WinFormsApp1;

public class CsvSaveJob : FileSaveJob
{
    public string? Data { get; set; }
    
    public string? Header { get; set; }
    
    public CsvSaveJob()
    {
    }
    
    public override void Save()
    {
        string? directory = Path.GetDirectoryName(FilePath);
        if (directory == null)
            return;
        
        Directory.CreateDirectory(directory);
        if (!File.Exists(FilePath))
        {
            using (StreamWriter writer = new StreamWriter(FilePath))
            {
                writer.WriteLine(Header);
            }
        }
        
        using (StreamWriter writer = new StreamWriter(FilePath, append:true))
        {
            writer.WriteLine(Data);
        }

    }
}
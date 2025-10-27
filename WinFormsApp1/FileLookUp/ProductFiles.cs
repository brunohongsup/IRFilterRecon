namespace WinFormsApp1;

public class ProductFiles
{
    public required string ProductId { get; set; }
    public DateTime Date { get; set; }
    public string? Directory { get; set; }
    public string? CsvFile { get; set; }
    public List<string> ImageFiles { get; set; } = new List<string>();
    
    public bool HasData => CsvFile != null || ImageFiles.Any();
}
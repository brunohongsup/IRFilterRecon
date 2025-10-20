using System.Drawing.Imaging;

namespace WinFormsApp1;

public class ImageSaveJob : FileSaveJob
{
    public Image? Image { get; set; }
    
    public ImageSaveJob(string filePath)
    {
        FilePath = filePath;   
    }
    
    public override void Save()
    {
        string? directory = Path.GetDirectoryName(FilePath);
        if (directory == null)
            return;
        
        Directory.CreateDirectory(directory);
        using (var ms = new MemoryStream())
        {
            if (Image != null)
            {
                Image.Save(ms, ImageFormat.Jpeg);
                File.WriteAllBytes(FilePath, ms.ToArray());
            }
        }
    }
}
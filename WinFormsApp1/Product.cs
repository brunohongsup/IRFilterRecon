namespace WinFormsApp1;

public class Product
{
    public Product(string id)
    {
        _id = id;
        Dx = -999.0f;
        Dy = -999.0f;
        Theta = -999.0f;
        _dateTime = DateTime.Now;
        CurlHeight = new float[4];
        foreach (var curlHeight in CurlHeight)
        {
        }
    }

    private string _id;
    
    public string Id
    {
        get => _id;
    }

    private DateTime _dateTime;

    public DateTime DateTime
    {
        get => _dateTime;
    }
    
    public float Dx { get; set; }
    
    public float Dy { get; set; }
    
    public float Theta { get; set; }

    public float[] CurlHeight { get; set; }
}
namespace WinFormsApp1;

public sealed class ProductManager
{
    private static readonly Lazy<ProductManager> _instance =
        new Lazy<ProductManager>(() => new ProductManager());

    private List<Product> _products = new List<Product>();
    
    private readonly object _lock = new object();

    public List<Product> Products
    {
        get
        {
            lock (_lock)
            {
                return new List<Product>(_products);
            }
        }
    }
    public static ProductManager Instance
    {
        get
        {
            return _instance.Value;
        }
    }

    public bool AddProduct(string id)
    {
        if (_products.Count > 500)
        {
            _products.RemoveRange(0, 450);
        }

        if(_products.Any(p => p.Id == id))
            return false;
        
        _products.Add(new Product(id));
        var fileSaver = FileSaver.Instance;
        fileSaver.AddFile(new CsvSaveJob()
        {
            FilePath = "D:\\Dat\\Cognex\\products.csv",
            Data = $"{id}, Good, Product, Good",
            Header = "Id, Product, Good,May, Good, Fuck,Great"
        });
        
        return true;
    }
    
    private ProductManager()
    {
        
    }
    
}
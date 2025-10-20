namespace WinFormsApp1;

public sealed class ProductManager
{
    private static readonly object padlock = new object();
    
    private static readonly Lazy<ProductManager> _instance =
        new Lazy<ProductManager>(() => new ProductManager());

    private List<Product> _products = new List<Product>();

    public List<Product> Products => _products;
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
        return true;
    }
    
    private ProductManager()
    {
        
    }
    
}
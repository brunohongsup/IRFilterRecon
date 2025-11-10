using System.ComponentModel;

namespace WinFormsApp1;

public sealed class ProductManager
{
    private static readonly Lazy<ProductManager> _instance =
        new Lazy<ProductManager>(() => new ProductManager());

    private readonly BindingList<Product> _products = new BindingList<Product>();
    private readonly object _lock = new object();

    public event Action<string>? OnProductAdded;

    public BindingList<Product> Products
    {
        get
        {
            lock (_lock)
            {
                return _products;
            }
        }
    }
    public static ProductManager Instance => _instance.Value;

    public bool AddProduct(string id)
    {
        lock (_lock)
        {
            if (_products.Count > 500)
            {
                for (int i = 0; i < 450; i++)
                    _products.RemoveAt(0);
            }

            if (_products.Any(p => p.Id == id))
                return false;

            _products.Add(new Product(id));
        }

        OnProductAdded?.Invoke(id);
        return true;
    }

    public void CreateNewProduct()
    {
        var id = RandomStringGenerator.GenerateRandomString(10);
        AddProduct(id);
    }

    private ProductManager() { }
}
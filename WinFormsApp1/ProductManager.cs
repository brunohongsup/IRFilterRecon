using System;
using System.Collections.Generic;
using System.Linq;

namespace WinFormsApp1;

public sealed class ProductManager
{
    private static readonly Lazy<ProductManager> _instance =
        new Lazy<ProductManager>(() => new ProductManager());

    // 1. Use a standard List<T> for the master data store. It's faster
    //    and we are managing thread safety manually.
    private readonly List<Product> _products = new List<Product>();
    private readonly object _lock = new object();

    // 2. Create new events to pass the *actual* product objects.
    //    This is what the UI will use to update itself.
    public event Action<Product>? ProductAdded;

    public event Action<List<Product>>? ProductsRemoved; // For the cleanup logic

    // (This event is no longer used)
    // public event Action<string>? OnProductAdded; 

    // 3. This property is dangerous as it "leaks" the collection.
    //    We'll replace it with a method that gets a safe snapshot.
    public List<Product> GetProductSnapshot()
    {
        lock (_lock)
        {
            return _products.ToList(); // Return a *copy*
        }
    }

    // Helper for the label
    public Product? GetLastProduct()
    {
        lock (_lock)
        {
            return _products.LastOrDefault();
        }
    }

    public static ProductManager Instance => _instance.Value;

    public bool AddProduct(string id)
    {
        Product newProduct;
        List<Product> removedProductList = null; // To hold removed items

        lock (_lock)
        {
            if (_products.Any(p => p.Id == id))
                return false;

            // 4. Handle the cleanup logic inside the lock
            if (_products.Count > 500)
            {
                removedProductList = new List<Product>();
                for (int i = 0; i < 450; i++)
                {
                    // Keep track of what we remove
                    removedProductList.Add(_products[0]);
                    _products.RemoveAt(0);
                }
            }

            newProduct = new Product(id);
            _products.Add(newProduct);
        }

        // 5. Fire events *outside* the lock to avoid deadlocks.
        //    The worker thread fires these.
        if (removedProductList != null)
        {
            ProductsRemoved?.Invoke(removedProductList);
        }

        ProductAdded?.Invoke(newProduct);

        return true;
    }

    public void CreateNewProduct()
    {
        var id = RandomStringGenerator.GenerateRandomString(10);
        AddProduct(id);
    }

    private ProductManager() { }
}
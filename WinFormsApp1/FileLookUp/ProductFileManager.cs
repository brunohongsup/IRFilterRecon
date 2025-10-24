using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace WinFormsApp1;

public class ProductFileManager
{
    private readonly string _baseDirectory = @"D:\DAT\IMAGE";
    
    // Get all files for a specific product and date
    
    private static readonly Lazy<ProductFileManager> _instance = new Lazy<ProductFileManager>(() => new ProductFileManager());
    
    public static ProductFileManager Instance => _instance.Value;
    public ProductFiles GetProductFiles(DateTime date, string productId)
    {
        string directory = BuildPath(date, productId);
        
        if (!Directory.Exists(directory))
            return null;

        var result = new ProductFiles
        {
            ProductId = productId,
            Date = date,
            Directory = directory
        };

        // Get CSV file
        string csvPath = Path.Combine(directory, "*.csv");
        if (File.Exists(csvPath))
            result.CsvFile = csvPath;

        // Get all image files
        result.ImageFiles = Directory.GetFiles(directory, "*.bmp")
            .OrderBy(f => f)
            .ToList();

        return result;
    }

    // Get all products for a specific date
    public List<string> GetProductsForDate(DateTime date)
    {
        string datePath = BuildDatePath(date);
        
        if (!Directory.Exists(datePath))
            return new List<string>();

        return Directory.GetDirectories(datePath)
            .Select(dir => Path.GetFileName(dir))
            .ToList();
    }

    // Get all dates that have data
    public List<DateTime> GetAvailableDates()
    {
        var dates = new List<DateTime>();

        if (!Directory.Exists(_baseDirectory))
            return dates;

        // Iterate through YYYY folders
        foreach (var yearDir in Directory.GetDirectories(_baseDirectory))
        {
            string yearStr = Path.GetFileName(yearDir);
            if (!int.TryParse(yearStr, out int year))
                continue;

            // Iterate through MM folders
            foreach (var monthDir in Directory.GetDirectories(yearDir))
            {
                string monthStr = Path.GetFileName(monthDir);
                if (!int.TryParse(monthStr, out int month))
                    continue;

                // Iterate through DD folders
                foreach (var dayDir in Directory.GetDirectories(monthDir))
                {
                    string dayStr = Path.GetFileName(dayDir);
                    if (int.TryParse(dayStr, out int day))
                    {
                        try
                        {
                            dates.Add(new DateTime(year, month, day));
                        }
                        catch
                        {
                            // Invalid date, skip
                        }
                    }
                }
            }
        }

        return dates.OrderByDescending(d => d).ToList();
    }

    // Search for product across date range
    public List<ProductFiles> FindProductInDateRange(string productId, DateTime startDate, DateTime endDate)
    {
        var results = new List<ProductFiles>();

        for (var date = startDate.Date; date <= endDate.Date; date = date.AddDays(1))
        {
            var files = GetProductFiles(date, productId);
            if (files != null && (files.ImageFiles.Any() || files.CsvFile != null))
            {
                results.Add(files);
            }
        }

        return results;
    }

    // Get all products across all dates (use carefully - can be slow)
    public Dictionary<DateTime, List<string>> GetAllProductsByDate()
    {
        var result = new Dictionary<DateTime, List<string>>();

        foreach (var date in GetAvailableDates())
        {
            var products = GetProductsForDate(date);
            if (products.Any())
            {
                result[date] = products;
            }
        }

        return result;
    }

    // Check if product exists for a date
    public bool ProductExistsForDate(DateTime date, string productId)
    {
        string directory = BuildPath(date, productId);
        return Directory.Exists(directory);
    }

    // Get specific image by index
    public string GetProductImage(DateTime date, string productId, int imageIndex)
    {
        string directory = BuildPath(date, productId);
        string imagePath = Path.Combine(directory, $"ProductImages-{imageIndex}.jpg");
        
        return File.Exists(imagePath) ? imagePath : null;
    }

    // Helper: Build full path
    private string BuildPath(DateTime date, string productId)
    {
        return Path.Combine(
            _baseDirectory,
            date.Year.ToString("D4"),
            date.Month.ToString("D2"),
            date.Day.ToString("D2"),
            productId
        );
    }

    // Helper: Build date path (without product ID)
    private string BuildDatePath(DateTime date)
    {
        return Path.Combine(
            _baseDirectory,
            date.Year.ToString("D4"),
            date.Month.ToString("D2"),
            date.Day.ToString("D2")
        );
    }

    // Create directory structure for new product
    public string EnsureDirectoryExists(DateTime date, string productId)
    {
        string directory = BuildPath(date, productId);
        Directory.CreateDirectory(directory);
        return directory;
    }
}

// Data class to hold results


// Usage Examples:
/*

var manager = new ProductFileManager();

// 1. Get files for specific product and date
var files = manager.GetProductFiles(DateTime.Now, "PROD-12345");
if (files != null)
{
    Console.WriteLine($"CSV: {files.CsvFile}");
    foreach (var img in files.ImageFiles)
        Console.WriteLine($"Image: {img}");
}

// 2. Get all products for today
var todaysProducts = manager.GetProductsForDate(DateTime.Today);

// 3. Find product across date range
var results = manager.FindProductInDateRange(
    "PROD-12345", 
    DateTime.Today.AddDays(-7), 
    DateTime.Today
);

// 4. Get all available dates
var dates = manager.GetAvailableDates();

// 5. Check if product exists
bool exists = manager.ProductExistsForDate(DateTime.Today, "PROD-12345");

// 6. Get specific image
string imagePath = manager.GetProductImage(DateTime.Today, "PROD-12345", 0);

// 7. Create directory for saving
string dir = manager.EnsureDirectoryExists(DateTime.Now, "PROD-12345");
File.WriteAllText(Path.Combine(dir, "ProductData.csv"), csvData);

*/
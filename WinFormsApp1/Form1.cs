using System.ComponentModel;
using System.Windows.Forms;
using WinFormsApp1.ForegroundWorker;

namespace WinFormsApp1;

public partial class Form1 : Form
{
    // 1. The Form has its *OWN* BindingList, just for the UI.
    //    This list will *only* be touched by the UI thread.
    private readonly BindingList<Product> _gridProducts = new BindingList<Product>();

    public Form1()
    {
        InitializeComponent();
        var productManager = ProductManager.Instance;

        // 2. Subscribe to the new thread-safe events
        productManager.ProductAdded += OnManagerProductAdded;
        productManager.ProductsRemoved += OnManagerProductsRemoved;

        // 3. Bind the DataGridView to the FORM'S list, not the manager's.
        dataGridView1.DataSource = _gridProducts;
        dataGridView1.AutoGenerateColumns = false;

        // Add columns (no change)
        dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = "Id",
            HeaderText = "Id"
        });
        dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = "DateTime",
            HeaderText = "Time"
        });

        var workersManager = WorkersManager.Instance;
        workersManager.StatusBroadcast += AddLog;
    }

    // 4. This event handler receives the "ProductAdded" event
    private void OnManagerProductAdded(Product newProduct)
    {
        // This is called from a worker thread, so we must marshal to the UI thread.
        if (InvokeRequired)
        {
            // Use BeginInvoke for "fire and forget"
            BeginInvoke(new Action<Product>(OnManagerProductAdded), newProduct);
            return;
        }

        // --- We are now safely on the UI thread ---
        _gridProducts.Add(newProduct);
        label_Id.Text = newProduct.Id; // Update the label here
    }

    // 5. This event handler receives the "ProductsRemoved" event
    private void OnManagerProductsRemoved(List<Product> removedProducts)
    {
        if (InvokeRequired)
        {
            BeginInvoke(new Action<List<Product>>(OnManagerProductsRemoved), removedProducts);
            return;
        }

        // --- We are now safely on the UI thread ---
        // We must remove the same items from our local list
        foreach (var productToRemove in removedProducts)
        {
            // Find the product in our grid list (a hashset/dictionary would be faster)
            var gridProduct = _gridProducts.FirstOrDefault(p => p.Id == productToRemove.Id);
            if (gridProduct != null)
            {
                _gridProducts.Remove(gridProduct);
            }
        }
    }

    private void AddLog(object? sender, ForeWorkerStatusEventArgs foreWorkerStatusEventArgs)
    {
        // This method is already correctly thread-safe, no changes needed.
        if (InvokeRequired)
        {
            BeginInvoke(new Action<object, ForeWorkerStatusEventArgs>(AddLog), sender, foreWorkerStatusEventArgs);
            return;
        }

        if (listBoxLog.Items.Count > 1000)
            listBoxLog.Items.RemoveAt(0);

        string line = $"[{foreWorkerStatusEventArgs.Timestamp}] {foreWorkerStatusEventArgs.Message}";
        listBoxLog.Items.Add(line);
        listBoxLog.TopIndex = listBoxLog.Items.Count - 1; // auto-scroll
    }

    private void sequenceStartButton_Click(object sender, EventArgs e)
    {
        var workerManager = WorkersManager.Instance;
        Action startWorking = async () => await workerManager.StartAllAsync();
        var threadpool = Threadpool.Instance;
        var startWorkingTask = new RandomTask(startWorking);
        threadpool.AddWork(startWorkingTask);
    }

    private void sequenceStopButton_Click(object sender, EventArgs e)
    {
        var workerManager = WorkersManager.Instance;
        Action stopWorking = async () => await workerManager.StopAllAsync();
        var threadpool = Threadpool.Instance;
        var stopWorkingTask = new RandomTask(stopWorking);
        threadpool.AddWork(stopWorkingTask);
    }
}
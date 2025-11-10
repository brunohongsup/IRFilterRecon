using System.ComponentModel;
using System.Windows.Forms;
using WinFormsApp1.ForegroundWorker;

namespace WinFormsApp1;

public partial class Form1 : Form
{
    public Form1()
    {
        InitializeComponent();
        var productManager = ProductManager.Instance;
        productManager.OnProductAdded += OnProductAdded;
        dataGridView1.DataSource = productManager.Products;
        dataGridView1.AutoGenerateColumns = false;

        // Add columns for only the properties you want
        dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = "Id",   // Property you want to show
            HeaderText = "Id"
        });

        dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = "DateTime",    // Another property you want to show
            HeaderText = "Time"
        });

        var workersManager = WorkersManager.Instance;
        workersManager.StatusBroadcast += AddLog;
    }

    private void AddLog(object? sender, ForeWorkerStatusEventArgs foreWorkerStatusEventArgs)
    {
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

    private async void sequenceStartButton_Click(object sender, EventArgs e)
    {
        var workerManager = WorkersManager.Instance;
        Action startWorking = async () => await workerManager.StartAllAsync();
        var threadpool = Threadpool.Instance;
        var startWorkingTask = new RandomTask(startWorking);
        threadpool.AddWork(startWorkingTask);
        await workerManager.StartAllAsync();
    }

    private async void sequenceStopButton_Click(object sender, EventArgs e)
    {
        var workerManager = WorkersManager.Instance;
        await workerManager.StopAllAsync();
    }

    private void OnProductAdded(string id)
    {
        if (InvokeRequired)
        {
            BeginInvoke(new Action(UpdateProducts));
        }

        else
        {
            UpdateProducts();
        }
    }

    private void UpdateProducts()
    {
        label_Id.Text = ProductManager.Instance.Products.Last().Id;
    }

}
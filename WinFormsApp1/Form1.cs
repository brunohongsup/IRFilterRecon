using WinFormsApp1.ForegroundWorker;

namespace WinFormsApp1;

public partial class Form1 : Form
{
    public Form1()
    {
        InitializeComponent();
        var productManager = ProductManager.Instance;
        dataGridView1.AutoGenerateColumns = false;
        dataGridView1.DataSource = productManager.Products;

        // Add columns for only the properties you want
        dataGridView1.Columns.Add(new DataGridViewTextBoxColumn {
            DataPropertyName = "Id",   // Property you want to show
            HeaderText = "Id"
        });
        
        dataGridView1.Columns.Add(new DataGridViewTextBoxColumn {
            DataPropertyName = "DateTime",    // Another property you want to show
            HeaderText = "Time"
        });
        
        var workermManager = WorkersManager.Instance;
        workermManager.StatusBroadcast += AddLog;
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

    private async void button1_Click(object sender, EventArgs e)
    {
        var workerManager = WorkersManager.Instance;
        await workerManager.StartAll();
    }

    private async void button2_Click(object sender, EventArgs e)
    {
        var workerManager = WorkersManager.Instance;
        await workerManager.StopAllAsync();
    }
}
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

    }
}
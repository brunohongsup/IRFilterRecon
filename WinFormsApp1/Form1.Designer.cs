namespace WinFormsApp1;

partial class Form1
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }

        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        dataGridView1 = new System.Windows.Forms.DataGridView();
        listBoxLog = new System.Windows.Forms.ListBox();
        ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
        SuspendLayout();
        // 
        // dataGridView1
        // 
        dataGridView1.Location = new System.Drawing.Point(0, 0);
        dataGridView1.Name = "dataGridView1";
        dataGridView1.Size = new System.Drawing.Size(668, 335);
        dataGridView1.TabIndex = 0;
        // 
        // listBoxLog
        // 
        listBoxLog.FormattingEnabled = true;
        listBoxLog.HorizontalScrollbar = true;
        listBoxLog.IntegralHeight = false;
        listBoxLog.ItemHeight = 15;
        listBoxLog.Location = new System.Drawing.Point(0, 385);
        listBoxLog.Name = "listBoxLog";
        listBoxLog.Size = new System.Drawing.Size(493, 170);
        listBoxLog.TabIndex = 1;
        // 
        // Form1
        // 
        AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
        AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        ClientSize = new System.Drawing.Size(857, 554);
        Controls.Add(listBoxLog);
        Controls.Add(dataGridView1);
        Text = "Form1";
        ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
        ResumeLayout(false);
    }

    private System.Windows.Forms.ListBox listBoxLog;

    private System.Windows.Forms.DataGridView dataGridView1;

    #endregion
}
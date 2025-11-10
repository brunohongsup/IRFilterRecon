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
        dataGridView1 = new DataGridView();
        listBoxLog = new ListBox();
        button1 = new Button();
        button2 = new Button();
        label_Id = new Label();
        ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
        SuspendLayout();
        // 
        // dataGridView1
        // 
        dataGridView1.Location = new Point(0, 0);
        dataGridView1.Name = "dataGridView1";
        dataGridView1.Size = new Size(493, 335);
        dataGridView1.TabIndex = 0;
        // 
        // listBoxLog
        // 
        listBoxLog.FormattingEnabled = true;
        listBoxLog.HorizontalScrollbar = true;
        listBoxLog.IntegralHeight = false;
        listBoxLog.ItemHeight = 15;
        listBoxLog.Location = new Point(0, 385);
        listBoxLog.Name = "listBoxLog";
        listBoxLog.Size = new Size(493, 170);
        listBoxLog.TabIndex = 1;
        // 
        // button1
        // 
        button1.Location = new Point(691, 81);
        button1.Name = "button1";
        button1.Size = new Size(78, 28);
        button1.TabIndex = 2;
        button1.Text = "Start";
        button1.UseVisualStyleBackColor = true;
        button1.Click += sequenceStartButton_Click;
        // 
        // button2
        // 
        button2.Location = new Point(691, 191);
        button2.Name = "button2";
        button2.Size = new Size(78, 28);
        button2.TabIndex = 3;
        button2.Text = "Stop";
        button2.UseVisualStyleBackColor = true;
        button2.Click += sequenceStopButton_Click;
        // 
        // label_Id
        // 
        label_Id.BorderStyle = BorderStyle.FixedSingle;
        label_Id.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
        label_Id.Location = new Point(499, 0);
        label_Id.Name = "label_Id";
        label_Id.Size = new Size(97, 29);
        label_Id.TabIndex = 4;
        label_Id.Text = "Id";
        label_Id.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // Form1
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(857, 554);
        Controls.Add(label_Id);
        Controls.Add(button2);
        Controls.Add(button1);
        Controls.Add(listBoxLog);
        Controls.Add(dataGridView1);
        Name = "Form1";
        Text = "Form1";
        ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
        ResumeLayout(false);
    }

    private System.Windows.Forms.Button button2;

    private System.Windows.Forms.Button button1;

    private System.Windows.Forms.ListBox listBoxLog;

    private System.Windows.Forms.DataGridView dataGridView1;

    #endregion

    private Label label_Id;
}
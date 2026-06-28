
namespace _5GAutoTool
{
    partial class ISFreqRange
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.chckboxOption1 = new System.Windows.Forms.CheckBox();
            this.chckboxOption2 = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtFreqTo = new System.Windows.Forms.TextBox();
            this.txtFreqFrom = new System.Windows.Forms.TextBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.lvwFreqRange = new System.Windows.Forms.ListView();
            this.cFreqFrom = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clFreqTo = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.chckboxOption1);
            this.groupBox1.Controls.Add(this.chckboxOption2);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.txtFreqTo);
            this.groupBox1.Controls.Add(this.txtFreqFrom);
            this.groupBox1.Controls.Add(this.btnOK);
            this.groupBox1.Controls.Add(this.btnCancel);
            this.groupBox1.Controls.Add(this.btnAdd);
            this.groupBox1.Controls.Add(this.lvwFreqRange);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(15, 16);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(647, 544);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Out-of-band (Interfering Sweep Frequency Range Setup)";
            // 
            // chckboxOption1
            // 
            this.chckboxOption1.AutoSize = true;
            this.chckboxOption1.Location = new System.Drawing.Point(7, 44);
            this.chckboxOption1.Name = "chckboxOption1";
            this.chckboxOption1.Size = new System.Drawing.Size(193, 24);
            this.chckboxOption1.TabIndex = 32;
            this.chckboxOption1.Text = "Set power lelvel from ";
            this.chckboxOption1.UseVisualStyleBackColor = true;
            // 
            // chckboxOption2
            // 
            this.chckboxOption2.AutoSize = true;
            this.chckboxOption2.Location = new System.Drawing.Point(7, 83);
            this.chckboxOption2.Name = "chckboxOption2";
            this.chckboxOption2.Size = new System.Drawing.Size(253, 24);
            this.chckboxOption2.TabIndex = 31;
            this.chckboxOption2.Text = "except ranges specified in list";
            this.chckboxOption2.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(23, 188);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(28, 20);
            this.label4.TabIndex = 30;
            this.label4.Text = "To";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(150, 191);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(45, 20);
            this.label3.TabIndex = 29;
            this.label3.Text = "MHz";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(150, 132);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(45, 20);
            this.label2.TabIndex = 28;
            this.label2.Text = "MHz";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 132);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(48, 20);
            this.label1.TabIndex = 27;
            this.label1.Text = "From";
            // 
            // txtFreqTo
            // 
            this.txtFreqTo.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtFreqTo.Location = new System.Drawing.Point(71, 181);
            this.txtFreqTo.Margin = new System.Windows.Forms.Padding(4);
            this.txtFreqTo.Name = "txtFreqTo";
            this.txtFreqTo.Size = new System.Drawing.Size(72, 30);
            this.txtFreqTo.TabIndex = 26;
            this.txtFreqTo.Text = "12750";
            // 
            // txtFreqFrom
            // 
            this.txtFreqFrom.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtFreqFrom.Location = new System.Drawing.Point(71, 125);
            this.txtFreqFrom.Margin = new System.Windows.Forms.Padding(4);
            this.txtFreqFrom.Name = "txtFreqFrom";
            this.txtFreqFrom.Size = new System.Drawing.Size(68, 30);
            this.txtFreqFrom.TabIndex = 25;
            this.txtFreqFrom.Text = "1";
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(550, 500);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(82, 34);
            this.btnOK.TabIndex = 5;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(444, 500);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(82, 34);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(71, 236);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(72, 34);
            this.btnAdd.TabIndex = 3;
            this.btnAdd.Text = "Add";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // lvwFreqRange
            // 
            this.lvwFreqRange.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvwFreqRange.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.cFreqFrom,
            this.clFreqTo});
            this.lvwFreqRange.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lvwFreqRange.FullRowSelect = true;
            this.lvwFreqRange.GridLines = true;
            this.lvwFreqRange.HideSelection = false;
            this.lvwFreqRange.Location = new System.Drawing.Point(282, 44);
            this.lvwFreqRange.Margin = new System.Windows.Forms.Padding(5);
            this.lvwFreqRange.Name = "lvwFreqRange";
            this.lvwFreqRange.Size = new System.Drawing.Size(350, 448);
            this.lvwFreqRange.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.lvwFreqRange.TabIndex = 2;
            this.lvwFreqRange.UseCompatibleStateImageBehavior = false;
            this.lvwFreqRange.View = System.Windows.Forms.View.Details;
            this.lvwFreqRange.MouseClick += new System.Windows.Forms.MouseEventHandler(this.lvwFreqRange_MouseClick);
            // 
            // cFreqFrom
            // 
            this.cFreqFrom.Text = "FROM (MHz)";
            this.cFreqFrom.Width = 120;
            // 
            // clFreqTo
            // 
            this.clFreqTo.Text = "TO (MHz)";
            this.clFreqTo.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.clFreqTo.Width = 120;
            // 
            // ISFreqRange
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(674, 572);
            this.Controls.Add(this.groupBox1);
            this.Name = "ISFreqRange";
            this.Text = "ISFreqRange";
            this.Load += new System.EventHandler(this.ISFreqRange_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ListView lvwFreqRange;
        private System.Windows.Forms.ColumnHeader cFreqFrom;
        private System.Windows.Forms.ColumnHeader clFreqTo;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtFreqTo;
        private System.Windows.Forms.TextBox txtFreqFrom;
        private System.Windows.Forms.CheckBox chckboxOption1;
        private System.Windows.Forms.CheckBox chckboxOption2;
    }
}
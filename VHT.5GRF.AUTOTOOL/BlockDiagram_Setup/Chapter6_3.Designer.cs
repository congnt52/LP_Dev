namespace _5GAutoTool
{
    partial class Chapter6_3
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Chapter6_3));
            this.btnSetupOK = new System.Windows.Forms.Button();
            this.picWiringDiagram = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.picWiringDiagram)).BeginInit();
            this.SuspendLayout();
            // 
            // btnSetupOK
            // 
            this.btnSetupOK.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSetupOK.Location = new System.Drawing.Point(962, 725);
            this.btnSetupOK.Name = "btnSetupOK";
            this.btnSetupOK.Size = new System.Drawing.Size(115, 44);
            this.btnSetupOK.TabIndex = 5;
            this.btnSetupOK.Text = "SETUP OK";
            this.btnSetupOK.UseVisualStyleBackColor = true;
            this.btnSetupOK.Click += new System.EventHandler(this.btnSetupOK_Click);
            // 
            // picWiringDiagram
            // 
            this.picWiringDiagram.Image = ((System.Drawing.Image)(resources.GetObject("picWiringDiagram.Image")));
            this.picWiringDiagram.Location = new System.Drawing.Point(28, 13);
            this.picWiringDiagram.Name = "picWiringDiagram";
            this.picWiringDiagram.Size = new System.Drawing.Size(1049, 706);
            this.picWiringDiagram.TabIndex = 4;
            this.picWiringDiagram.TabStop = false;
            this.picWiringDiagram.Click += new System.EventHandler(this.picWiringDiagram_Click);
            // 
            // Chapter6_3
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1105, 783);
            this.Controls.Add(this.btnSetupOK);
            this.Controls.Add(this.picWiringDiagram);
            this.Name = "Chapter6_3";
            this.Text = "Chapter6_3";
            ((System.ComponentModel.ISupportInitialize)(this.picWiringDiagram)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnSetupOK;
        private System.Windows.Forms.PictureBox picWiringDiagram;
    }
}
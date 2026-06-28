namespace _5GAutoTool
{
    partial class Chapter6_1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Chapter6_1));
            this.picWiringDiagram = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.picWiringDiagram)).BeginInit();
            this.SuspendLayout();
            // 
            // picWiringDiagram
            // 
            this.picWiringDiagram.Image = ((System.Drawing.Image)(resources.GetObject("picWiringDiagram.Image")));
            this.picWiringDiagram.Location = new System.Drawing.Point(12, 12);
            this.picWiringDiagram.Name = "picWiringDiagram";
            this.picWiringDiagram.Size = new System.Drawing.Size(1043, 702);
            this.picWiringDiagram.TabIndex = 2;
            this.picWiringDiagram.TabStop = false;
            // 
            // Chapter6_1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1067, 727);
            this.Controls.Add(this.picWiringDiagram);
            this.Name = "Chapter6_1";
            this.Text = "Chapter6_1";
            ((System.ComponentModel.ISupportInitialize)(this.picWiringDiagram)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.PictureBox picWiringDiagram;
    }
}
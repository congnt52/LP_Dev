using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace _5GAutoTool
{
    public partial class DuplicateFile : Form
    {
        public DuplicateFile()
        {
            InitializeComponent();
        }
        private Form5GAT mainForm = null;
        public DuplicateFile(Form callingForm)
        {
            mainForm = callingForm as Form5GAT;
            InitializeComponent();
        }
        private void btnSelectFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "All Files (*.*)|*.*";
            openFileDialog.Multiselect = false;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                textBoxFilePath.Text = openFileDialog.FileName;
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            string filePath = textBoxFilePath.Text;
            if (checkBoxDuplicate.Checked)
            {                
                string directoryPath = Path.GetDirectoryName(filePath);
                duplicateFilesToFolder(filePath, directoryPath);
            }
            else
            {
                FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();

                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    string directoryPath = folderBrowserDialog.SelectedPath;
                    duplicateFilesToFolder(filePath, directoryPath);
                }
            }
            this.Close();
        }

        private void duplicateFilesToFolder(string filePath, string folder)
        {
            try
            {
                string directoryPath = Path.GetDirectoryName(filePath);
                string fileName = Path.GetFileNameWithoutExtension(filePath);
                string fileExtension = Path.GetExtension(filePath);

                int start = int.Parse(textBoxStartIndex.Text);
                int end = int.Parse(textBoxEndIndex.Text);

                string prefix = textBoxPrefix.Text;

                for (int i = start; i <= end; i++)
                {
                    string newFileName = $"{prefix}{i}{fileExtension}";
                    string newPath = Path.Combine(folder, newFileName);
                    File.Copy(filePath, newPath);
                }

                MessageBox.Show("Files duplicated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch
            {
                MessageBox.Show("Files duplicated got error!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            
        }
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}

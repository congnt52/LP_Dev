using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _5GAutoTool
{
    public partial class CreateATTFormatFile : Form
    {
        public CreateATTFormatFile()
        {
            InitializeComponent();
        }

        private Form5GAT mainForm = null;
        public CreateATTFormatFile(Form callingForm)
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
            //string filePath = textBoxFilePath.Text;
            //if (checkBoxDuplicate.Checked)
            //{
            //    string directoryPath = Path.GetDirectoryName(filePath);
            //    duplicateFilesToFolder(filePath, directoryPath);
            //}
            //else
            //{
            //    FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();

            //    if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            //    {
            //        string directoryPath = folderBrowserDialog.SelectedPath;
            //        duplicateFilesToFolder(filePath, directoryPath);
            //    }
            //}
            this.Close();
        }
    }
}

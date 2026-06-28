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
using CsvHelper.Configuration;
using CsvHelper;
using System.Globalization;

namespace _5GAutoTool
{
    public partial class CreateATTFiles : Form
    {
        public CreateATTFiles()
        {
            InitializeComponent();
        }
        private Form5GAT mainForm = null;
        string ucorSource = "";
        public CreateATTFiles(Form callingForm)
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
            bool isAbs = chckAbs.Checked;
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            string directoryPath;            
            if (chckSameFolder.Checked)
            {
                directoryPath = Path.GetDirectoryName(filePath);
                createUCORFile(filePath, directoryPath, 0, isAbs);
                //import to SGS100A
                if(chckImport.Checked)
                {
                    
                }

                this.Close();
            }
            else
            {
                FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();

                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    directoryPath = folderBrowserDialog.SelectedPath;
                    createUCORFile(filePath, directoryPath, 0, isAbs);
                    this.Close();
                }
            }
            
            //this.Close();
        }
        private void createUCORFile(string source, string destination, int commentLineRows, bool absConvert)
        {
            //string directoryPath = Path.GetDirectoryName(source);
            string fileName = Path.GetFileNameWithoutExtension(source);
            string output = Path.Combine(destination, fileName + "_UCOR.csv");
            createCSVFileFromSource(source, output,0,true);

        }

        private void createCSVFileFromSource(string input, string output, int dataFromLine, bool absConvert = false)
        {
            try
            {
                var csvConfig = new CsvConfiguration(CultureInfo.CurrentCulture)
                {
                    HasHeaderRecord = false,
                    AllowComments = true
                };

                // Define multiple comment characters
                csvConfig.Comment = '#';
                csvConfig.Comment = '!'; // Overwrite the comment character to '!' (or add additional comments)

                // Read all lines from the input CSV file using CsvHelper
                using (var reader = new StreamReader(input))
                using (var csv = new CsvReader(reader, csvConfig))
                {
                    // Skip comment lines
                    while (dataFromLine > 0)
                    {
                        reader.ReadLine();
                        dataFromLine--;
                    }

                    // Read the records
                    var records = new List<string>();
                    while (csv.Read())
                    {
                        // check if Line data is not a numeric Value
                        if (isNumericValue(csv))
                        {
                            // Extract the data from columns 1 and 2
                            var value1 = csv.GetField<double>(0).ToString();
                            var tmp = csv.GetField<double>(1);
                            var value2 = absConvert ? Math.Abs(tmp).ToString() : tmp.ToString(); // Take absolute value of column 2
                            var record = $"{value1};{value2}";
                            // Add the record to the list
                            records.Add(record);
                        }
                    }
                    //check lai recode
                    //foreach(var item in records)
                    //{
                    //    Console.WriteLine(item);
                    //}

                    //Write the extracted data to a new CSV file
                    using (var writer = new StreamWriter(output))
                    using (var csvWriter = new CsvWriter(writer, csvConfig))
                    {
                        // Write the records to the output CSV file
                        foreach (var record in records)
                        {
                            csvWriter.WriteField(record);
                            csvWriter.NextRecord();
                        }
                    }
                }
                MessageBox.Show("Files created successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception e)
            {
                MessageBox.Show("Files created Fail! " + e, "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private bool importUCORtoSGS(VSG vsg,string source, string destination)
        {
            bool tmp = false;
            //check if vsg is connected
            //if(vsg.IsConnected)

            return tmp;
        }
        private bool isNumericValue(CsvReader csv)
        {
            return csv.TryGetField<double>(0, out _) && csv.TryGetField<double>(1, out _); //1: numerric , 0: not numeric
        }
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void CreateATTFiles_Load(object sender, EventArgs e)
        {
            //Lay thong tin SGS100A
            if(mainForm.VSG2.Model=="SGS100A")  //or VSG3 = SGS100A
            {
                chckImport.Text = $"Import to SGS100A: IP = {mainForm.VSG2.IpAddress} ({mainForm.VSG2.Status})";
            }
        }
    }
}

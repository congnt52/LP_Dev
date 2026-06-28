using System;
using System.Collections.Generic;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Reflection.Emit;
using System.ComponentModel;
using Microsoft.Office.Interop.Excel;

namespace _5GAutoTool
{
    public enum TestcaseStatus
    {
        Running,
        Pending, 
        Passed,
        Failed,
        Na,
    }
    public class TestcaseNode: TreeNode, INotifyPropertyChanged
    {
        private TestcaseStatus _status;
        public TestcaseStatus Status
        {
            get { return _status; }
            set
            {
                if (_status != value)
                {
                    _status = value;
                    Console.WriteLine($"{Text}:Status changed to {value}"); // Debug output
                    OnPropertyChanged(nameof(Status));
                    SetNodeProperties();
                }
            }
        }
        public System.Data.DataTable Parameters { get; set; }

        public TestcaseNode(string name ="")
        {            
            Tag = this;
            Text = name;
            Status = TestcaseStatus.Pending;
            Parameters = new System.Data.DataTable();
            SetNodeProperties();
        }

        public void Reset()
        {
            this.Parameters = null;
            this.Status = TestcaseStatus.Pending;
        }
        public void ResetStatus()
        {
            this.Status = TestcaseStatus.Pending;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void SetNodeProperties()
        {
            switch (Status)
            {
                case TestcaseStatus.Running:
                    ForeColor = Color.Blue;
                    ImageKey = "Running";
                    break;
                case TestcaseStatus.Passed:
                    ForeColor = Color.Green;
                    ImageKey = "Passed";
                    break;
                case TestcaseStatus.Failed:
                    ForeColor = Color.Red;
                    ImageKey = "Failed";
                    break;
                case TestcaseStatus.Pending:
                    ForeColor = Color.Black;
                    ImageKey = "Pending";
                    break;
                case TestcaseStatus.Na:
                    ForeColor = Color.Gray;
                    ImageKey = "Na";
                    break;                  
            }
            SelectedImageKey = ImageKey;
        }
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

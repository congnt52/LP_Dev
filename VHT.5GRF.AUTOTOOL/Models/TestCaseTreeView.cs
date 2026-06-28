using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using static System.Runtime.CompilerServices.RuntimeHelpers;
using Excel = Microsoft.Office.Interop.Excel;

namespace _5GAutoTool
{
    public class TestCaseTreeView: TreeView
    {
        // Singleton instance
        private static TestCaseTreeView _instance;
        // Lock object for thread safety
        private static readonly object _lock = new object();

        // List of TestcaseNode objects
        public List<TestcaseNode> TestcaseNodes { get; private set; }
        Dictionary<int, TreeNode> nodeDictionary = new Dictionary<int, TreeNode>(); // Level to node mapping
        private string name = "Root";

        public Form5GAT mainForm;

        public TestCaseTreeView()
        {
            TestcaseNodes = new List<TestcaseNode>();
            nodeDictionary = new Dictionary<int, TreeNode>();
        }

        public static TestCaseTreeView Instance
        {
            get
            {
                // Double-check locking for thread safety
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new TestCaseTreeView();
                        }
                    }
                }
                return _instance;
            }
        }


        public void UpdateNodeStatuses()
        {
            foreach (TreeNode node in Nodes)
            {
                UpdateNodeStatus(node);
            }
        }

        private void UpdateNodeStatus(TreeNode node)
        {
            if (node.Tag is TestcaseNode testcaseNode)
            {
                if (node.Checked)
                {
                    if(testcaseNode.Status!=TestcaseStatus.Passed && testcaseNode.Status!=TestcaseStatus.Failed)
                    {
                        testcaseNode.Status = TestcaseStatus.Pending;
                        //testcaseNode.UpdateProperties();
                    }                    
                }
                else
                {
                    testcaseNode.Status = TestcaseStatus.Na;
                    testcaseNode.ForeColor = Color.Gray;
                    testcaseNode.ImageKey = string.Empty;
                    testcaseNode.SelectedImageKey = ImageKey;
                }
                

            }

            foreach (TreeNode childNode in node.Nodes)
            {
                UpdateNodeStatus(childNode);
            }
        }
        public void CreateTreeViewFromExcel(string filePath,  TreeView trv)
        {
            PopulateTestcaseNodesFromExcel(filePath);
            UpdateToTreeView(trv, true);
        }

        private void PopulateTestcaseNodesFromExcel(string filePath)
        {
            var testcaseNodes = new List<TestcaseNode>();
            Excel.Application excelApp = null;
            Excel.Workbook workbook = null;
            Excel.Worksheet worksheet = null;

            try
            {
                excelApp = new Excel.Application();
                workbook = excelApp.Workbooks.Open(filePath);

                if (workbook.Sheets.Count == 0)
                {
                    MessageBox.Show("No sheets in the workbook.");
                    Log($"No sheets in the workbook {filePath}", LogLevel.ERROR);
                    return;
                }

                worksheet = (Excel.Worksheet)workbook.Sheets[1];
                Excel.Range usedRange = worksheet.UsedRange;
                name = usedRange.Cells[2, 1].Value?.ToString() ?? "Root";
                int headerCount = usedRange.Columns.Count;
                string[] headers = new string[headerCount];
                for (int i = 1; i <= headerCount; i++)
                {
                    headers[i - 1] = usedRange.Cells[1, i].Value?.ToString() ?? string.Empty;
                }

                var nodeDictionary = new Dictionary<int, TestcaseNode>(); // Dictionary to keep nodes by their level

                for (int row = 3; row <= usedRange.Rows.Count; row++)
                {
                    string nodeName = worksheet.Cells[row, 1].Value?.ToString();
                    if (string.IsNullOrEmpty(nodeName)) continue;

                    int level = nodeName.TakeWhile(c => c == '+').Count();
                    string trimmedNodeName = nodeName.TrimStart('+').Trim();

                    TestcaseNode testcaseNode = new TestcaseNode(trimmedNodeName);

                    for (int col = 2; col <= headerCount; col++)
                    {
                        testcaseNode.Parameters.Columns.Add(headers[col - 1]);
                    }

                    DataRow rowTable = testcaseNode.Parameters.NewRow();
                    for (int col = 2; col <= headerCount; col++)
                    {
                        rowTable[col - 2] = usedRange.Cells[row, col].Value?.ToString() ?? string.Empty;
                    }
                    testcaseNode.Parameters.Rows.Add(rowTable);
                    testcaseNode.Checked = true; //checkbox status

                    // Add the node to the list and dictionary
                    if (level == 1)
                    {
                        testcaseNodes.Add(testcaseNode);
                    }
                    else if (nodeDictionary.TryGetValue(level - 1, out TestcaseNode parentNode))
                    {
                        parentNode.Nodes.Add(testcaseNode);
                    }

                    nodeDictionary[level] = testcaseNode;
                }
                //update to TestcaseNodes
                TestcaseNodes = testcaseNodes;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading Excel data: " + ex.Message);
                Log($"Loading Excel file: {filePath} FAIL! {ex.Message}", LogLevel.ERROR);
            }
            finally
            {
                if (worksheet != null) System.Runtime.InteropServices.Marshal.ReleaseComObject(worksheet);
                if (workbook != null)
                {
                    workbook.Close(false);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(workbook);
                }
                if (excelApp != null) System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApp);
            }
            
        }
        public void UpdateFromTreeView(TreeView trvw)
        {
            foreach (TreeNode trvwNode in trvw.Nodes)
            {
                UpdateFromTreeView(trvwNode);
            }

        }
        
        private void UpdateFromTreeView(TreeNode trvwNode, bool isStartup = false)
        {
            if (trvwNode.Tag is TestcaseNode tcsNode)
            {
                TestcaseNode nodeInList = TestcaseNodes.FirstOrDefault(n => n.Name == tcsNode.Name);
                if (nodeInList != null)
                {
                    nodeInList.Checked = trvwNode.Checked;
                    //Log($"{nodeInList.Name}: check = {nodeInList.Checked}");
                    if (nodeInList.Checked && nodeInList.Status == TestcaseStatus.Na)
                    {
                        nodeInList.Status = TestcaseStatus.Pending;
                        //nodeInList.UpdateProperties();
                    }
                    else if (!nodeInList.Checked && nodeInList.Status == TestcaseStatus.Pending)
                    {
                        nodeInList.Status = TestcaseStatus.Na;
                        //nodeInList.UpdateProperties();
                    }
                    
                }
            }
            //update chidren nodes
            foreach (TreeNode child in trvwNode.Nodes)
            {
                UpdateFromTreeView(child);
            }

        }

        public void ShowTestcaseNodesStatus()
        {
            DashedLineLog("-----------------------");
            foreach (TestcaseNode node in TestcaseNodes)
            {                
                Log($"{node.Name}: checked = {node.Checked} Status = {node.Status} Color = {node.ForeColor} Image = {node.ImageKey}");                
            }
            DashedLineLog("-----------------------");
        }


        public void UpdateToTreeView(TreeView trvTestcase, bool isStartup = false)
        {
            trvTestcase.Nodes.Clear();
            //add icon
            trvTestcase.ImageList = new ImageList();

            trvTestcase.ImageList.Images.Add("pending", Image.FromFile(Path.Combine(System.Windows.Forms.Application.StartupPath, "Images", "Pending.png")));
            trvTestcase.ImageList.Images.Add("passed", Image.FromFile(Path.Combine(System.Windows.Forms.Application.StartupPath, "Images", "Passed.png")));
            trvTestcase.ImageList.Images.Add("failed", Image.FromFile(Path.Combine(System.Windows.Forms.Application.StartupPath, "Images", "Failed.png")));
            trvTestcase.ImageList.Images.Add("na", Image.FromFile(Path.Combine(System.Windows.Forms.Application.StartupPath, "Images", "Na.png")));
            trvTestcase.ImageList.Images.Add("running", Image.FromFile(Path.Combine(System.Windows.Forms.Application.StartupPath, "Images", "Running.png")));


            var rootNode = new TreeNode(name);
            trvTestcase.Nodes.Add(rootNode);

            // Add nodes to the TreeView
            foreach (TestcaseNode node in TestcaseNodes)
            {
                AddNodeToTreeView(node, rootNode, isStartup);
            }

            if (trvTestcase.Nodes.Count > 0)
            {
                trvTestcase.Nodes[0].Checked = true;
                trvTestcase.Nodes[0].EnsureVisible();
                trvTestcase.ExpandAll();                
            }
            
        }        

        private void AddNodeToTreeView(TestcaseNode testcaseNode, TreeNode parentNode, bool isStartup)
        {
            var treeNode = new TreeNode(testcaseNode.Name)
            {
                Tag = testcaseNode,
                Checked = testcaseNode.Checked,
                ForeColor = testcaseNode.ForeColor,
                ImageKey = testcaseNode.ImageKey,
                SelectedImageKey = ImageKey,
            };
            if(isStartup)
            {
                treeNode.ImageKey = string.Empty;
                treeNode.SelectedImageKey = string.Empty;
            }
            parentNode.Nodes.Add(treeNode);
            foreach (TestcaseNode node in testcaseNode.Nodes)
            {
                AddNodeToTreeView(node, treeNode, isStartup);
            }
        }
        public void GetNodesProperties(TreeNode tc)
        {
            if (tc.Tag is TestcaseNode tcsNode)
            {
                TestcaseNode nodeInList = TestcaseNodes.FirstOrDefault(n => n.Name == tcsNode.Name);
                if (nodeInList != null)
                {
                    //nodeInList.UpdateProperties();
                }
            }
        }


        /// <summary>
        /// Log Adapter
        /// </summary>
        LogAdapter log = LogAdapter.Instance;
        public void Log(string text, LogLevel logLevel = LogLevel.INFO)
        {
            log.Log(text, logLevel);
        }
        public void DashedLineLog(string title = "")
        {
            log.Log(title, LogLevel.DASH);
        }
    }
}

using CsvHelper.Configuration;
using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;
using System.Data;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;
using System.Drawing;
using _5GAutoTool.Measurements;
using System.Xml.Linq;
using System.Runtime.InteropServices.ComTypes;
using Keysight.SignalStudio.N7631;

namespace _5GAutoTool
{
    public partial class Form5GAT
    {

        //================= TREEVIEW FUNCTION ================================
        public void OpenFileData(string filename)
        {
            CreateTreeViewFromExcel(filename);
        }
        public void CreateTreeViewFromExcel(string filePath)
        {
            PopulateTestcaseNodesFromExcel(filePath);
            UpdateToTreeView(true);
        }

        private void PopulateTestcaseNodesFromExcel(string filePath)
        {
            var testcaseNodes = new List<TestcaseNode>();
            Excel.Application excelApp = null;
            Excel.Workbook workbook = null;
            Excel.Worksheet worksheet = null;

            ProgressLabel($"Opening file: {filePath}");
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
                treeViewName = usedRange.Cells[2, 1].Value?.ToString() ?? "Root";
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
                ProgressLabel($"Create Testcases List from file, done!");
                //foreach (TestcaseNode tc in testcaseNodes)
                //{
                //    Log($"+ {tc.Text}");
                //    if (tc.Nodes.Count > 0)
                //    {
                //        foreach (TestcaseNode node in tc.Nodes)
                //        {
                //            Log($"+ {node.Text}");
                //        }
                //    }
                //}
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
            ProgressLabel($"Updating status from Treeview...");
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

        public void ShowNodeProperties(TestcaseNode tcs)
        {
            Log($"{tcs.Text}: checked = {tcs.Checked} Status = {tcs.Status} Color = {tcs.ForeColor} Image = {tcs.ImageKey}");
            if (tcs.Nodes.Count > 0)
            {
                foreach (TestcaseNode node in tcs.Nodes)
                {
                    ShowNodeProperties(node);
                }
            }

        }

        private void UpdateToTreeView(bool isStartup = false)
        {
            ProgressLabel($"Updating to Treeview...");
            if (trvTestCases.InvokeRequired)
            {
                // We're on a different thread, invoke the update on the UI thread
                trvTestCases.Invoke(new Action(() => UpdateToTreeView(isStartup)));
            }
            else
            {
                trvTestCases.BeginUpdate();

                trvTestCases.Nodes.Clear();
                trvTestCases.ImageList = imageList;
                var rootNode = new TreeNode(treeViewName)
                {
                    Checked = true,
                    ImageKey = "TestPlan",
                    SelectedImageKey = "TestPlan",
                };
                trvTestCases.Nodes.Add(rootNode);

                // Add nodes to the TreeView
                foreach (TestcaseNode node in TestcaseNodes)
                {
                    AddNodeToTreeView(node, rootNode, isStartup);
                }

                if (trvTestCases.Nodes.Count > 0)
                {
                    //trvTestCases.Nodes[0].Checked = true;
                    trvTestCases.Nodes[0].EnsureVisible();
                    trvTestCases.ExpandAll();
                }

                trvTestCases.EndUpdate();
                ProgressLabel($"Updating to Treeview... => DONE!");
            }

        }

        public void UpdateNodeStatusToTreeView(TreeNode trvwNode)
        {

            TestcaseNode nodeInList = TestcaseNodes.FirstOrDefault(n => n.Text == trvwNode.Text);
            if (nodeInList != null)
            {
                trvwNode.Checked = nodeInList.Checked;
                trvwNode.ForeColor = nodeInList.ForeColor;
                trvwNode.ImageKey = nodeInList.ImageKey;
                trvwNode.SelectedImageKey = nodeInList.SelectedImageKey;
            }
            if (trvwNode.Nodes.Count > 0)
            {
                foreach (TreeNode node in trvwNode.Nodes)
                {
                    UpdateNodeStatusToTreeView(node);
                }
            }
        }

        public void UpdateNodeStatusToTreeView(TestcaseNode tcs, TreeNode tree)
        {
            foreach (TreeNode node in tree.Nodes)
            {
                if (node.Tag==tcs)
                {
                    node.Checked = tcs.Checked;
                    node.ForeColor = tcs.ForeColor;
                    node.ImageKey = tcs.ImageKey;
                    node.SelectedImageKey=tcs.SelectedImageKey;
                }
                if (node.Nodes.Count > 0)
                {
                    foreach (TreeNode n in node.Nodes)
                    {
                        UpdateNodeStatusToTreeView(tcs, n);
                    }
                }
            }
        }

        private void AddNodeToTreeView(TestcaseNode testcaseNode, TreeNode parentNode, bool isStartup)
        {
            ProgressLabel($"Adding Testcases Node to Treeview...");
            var treeNode = new TreeNode(testcaseNode.Text)
            {
                Tag = testcaseNode,
                Checked = testcaseNode.Checked,
                ForeColor = testcaseNode.ForeColor,
                ImageKey = testcaseNode.ImageKey,
                SelectedImageKey = testcaseNode.SelectedImageKey,
            };
            if (isStartup)
            {
                treeNode.ImageKey = "Pending";
                treeNode.SelectedImageKey = treeNode.ImageKey;
            }
            parentNode.Nodes.Add(treeNode);
            foreach (TestcaseNode node in testcaseNode.Nodes)
            {
                AddNodeToTreeView(node, treeNode, isStartup);
            }
            ProgressLabel($"Adding Testcases Node to Treeview..., DONE!");
        }
        public void GetNodesProperties(TreeNode tc)
        {
            if (tc.Tag is TestcaseNode tcsNode)
            {
                TestcaseNode nodeInList = TestcaseNodes.FirstOrDefault(n => n.Name == tcsNode.Name);
                if (nodeInList != null)
                {
                    // nodeInList.UpdateProperties();
                }
            }
        }
        private List<TreeNode> GetCheckedNodes(TreeNodeCollection nodes)
        {
            ProgressLabel($"Collecting Testcases Checked Node...");
            List<TreeNode> checkedNodes = new List<TreeNode>();

            foreach (TreeNode node in nodes)
            {
                if (node.Checked)
                {
                    checkedNodes.Add(node);
                }

                // Recursive call for child nodes
                if (node.Nodes.Count > 0)
                {
                    checkedNodes.AddRange(GetCheckedNodes(node.Nodes));
                }
            }
            return checkedNodes;
        }


        //Print Parameters
        private void PrintParamsToConsole(TestcaseNode node)
        {
            //data table 
            PrintDataTable(node.Parameters);
            if (node.Nodes.Count > 0)
            {
                foreach (TestcaseNode node2 in node.Nodes) { PrintParamsToConsole(node2); }
            }
        }
        public static void PrintDataTable(System.Data.DataTable table)
        {
            if (table == null || table.Columns.Count == 0)
            {
                Console.WriteLine("The DataTable is empty.");
                return;
            }

            // Print column headers
            foreach (DataColumn column in table.Columns)
            {
                Console.Write($"{column.ColumnName,-10}");
            }
            Console.WriteLine();

            // Print rows
            foreach (DataRow row in table.Rows)
            {
                foreach (var item in row.ItemArray)
                {
                    Console.Write($"{item,-20}");
                }
                Console.WriteLine();
            }
        }

        private void PrintTestcaseNodeParamsToLog(TestcaseNode tcs)
        {
            if (tcs.Parameters == null || tcs.Parameters.Columns.Count == 0)
            {
                Log($"[{tcs.Text}]The Parameters DataTable is empty.");
                return;
            }
            Log($"[{tcs.Text}] DataTable: ");
            var headers = new List<string>(tcs.Parameters.Columns.Cast<DataColumn>().Select(x => x.ColumnName).ToList());
            //var rowData = tcs.Parameters.Rows.Cast<string[]>().ToList();
            var rowData = new List<string[]>();

            foreach (DataRow row in tcs.Parameters.Rows)
            {
                // Convert each DataRow to a string array
                var rowValues = row.ItemArray.Select(item => item?.ToString()).ToArray();
                rowData.Add(rowValues);
            }
            PrintTable(headers, rowData);
        }
        
        private string ReadValueByHeader(DataTable dt, string headerName, int rowIndex = 0)
        {
            string strVal = "";

            // Check if DataTable is not null and contains the specified column
            if (dt != null && dt.Columns.Contains(headerName))
            {
                try
                {
                    // Check if the row index is within bounds
                    if (rowIndex >= 0 && rowIndex < dt.Rows.Count)
                    {
                        //var value = dt.Rows[rowIndex][headerName];
                        //strVal = value != DBNull.Value ? value.ToString() : string.Empty; // Handle DBNull
                        strVal = dt.Rows[rowIndex][headerName].ToString();
                    }
                    else
                    {
                        Log($"Cannot get value in DataTable! Row index {rowIndex} is out of range.", LogLevel.ERROR);
                    }
                }
                catch (Exception e)
                {
                    Log($"Error accessing DataTable: {e.Message}", LogLevel.ERROR);
                }
            }
            else
            {
                Log($"Cannot get value in DataTable! Column {headerName} not found!", LogLevel.ERROR);
            }

            return strVal;
        }

        private List<TestcaseNode> GetTestcasesByStatus(List<TestcaseNode> tcs, TestcaseStatus stt)
        {
            List<TestcaseNode> lst = new List<TestcaseNode>();
            foreach (TestcaseNode node in tcs)
            {
                if (node.Status == stt)
                {
                    lst.Add(node);
                }
            }
            return lst;
        }
        private void ResetTestcaseNodesStatus()
        {
            ProgressLabel($"Reseting Testcases Status...");
            foreach (TestcaseNode node in TestcaseNodes)
            {
                if(node.Checked)
                {
                    node.Status = TestcaseStatus.Pending;
                }
                else
                {
                    node.Status = TestcaseStatus.Na;
                }
            }
        }
        //==========================================================================
        void trvTestCases_AfterCheck(object sender, TreeViewEventArgs e)
        {
            //interaction check/uncheck
            if (e.Action != TreeViewAction.Unknown)
            {
                NodeCheckChange(e.Node);
                if (e.Node.Nodes.Count > 0)
                {
                    this.CheckAllChildNodes(e.Node, e.Node.Checked);
                }
                SelectParents(e.Node, e.Node.Checked);

                //UpdateToTreeView();
                UpdateNodeStatusToTreeView(e.Node);
                //ShowTestcaseNodesProperties();
            }
        }

        //
        //CheckAllChildNodes
        //
        private void CheckAllChildNodes(TreeNode treeNode, bool nodeChecked)
        {
            foreach (TreeNode node in treeNode.Nodes)
            {
                node.Checked = nodeChecked;
                NodeCheckChange(node);
                if (node.Nodes.Count > 0)
                {
                    // If the current node has child nodes, call the CheckAllChildsNodes method recursively.
                    this.CheckAllChildNodes(node, nodeChecked);
                }
            }
        }

        private void NodeCheckChange(TreeNode trvNode)
        {
            TestcaseNode nodeInList = TestcaseNodes.FirstOrDefault(n => n.Text == trvNode.Text);
            if (nodeInList != null)
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new Action(() =>
                    {
                        nodeInList.Checked = trvNode.Checked;
                        if (nodeInList.Checked && nodeInList.Status == TestcaseStatus.Na)
                        {
                            nodeInList.Status = TestcaseStatus.Pending;
                        }
                        else if (!nodeInList.Checked && nodeInList.Status == TestcaseStatus.Pending)
                        {
                            nodeInList.Status = TestcaseStatus.Na;
                        }
                    }));
                }
                else
                {
                    nodeInList.Checked = trvNode.Checked;
                    if (nodeInList.Checked && nodeInList.Status == TestcaseStatus.Na)
                    {
                        nodeInList.Status = TestcaseStatus.Pending;
                    }
                    else if (!nodeInList.Checked && nodeInList.Status == TestcaseStatus.Pending)
                    {
                        nodeInList.Status = TestcaseStatus.Na;
                    }
                }
                //Log($"[TestcaseNodes]{nodeInList.Text}: Change status to {nodeInList.Status}");
            }
        }
        private void NodeCheckChange()
        {
            foreach (TreeNode node in trvTestCases.Nodes)
            {
                TestcaseNode tc = node as TestcaseNode;
                TestcaseNode nodeInList = TestcaseNodes.FirstOrDefault(n => n.Tag == tc);
                if (nodeInList != null)
                {
                    nodeInList.Checked = node.Checked;
                    if (nodeInList.Checked && nodeInList.Status == TestcaseStatus.Na)
                    {
                        nodeInList.Status = TestcaseStatus.Pending;
                    }
                    else if (!nodeInList.Checked && nodeInList.Status == TestcaseStatus.Pending)
                    {
                        nodeInList.Status = TestcaseStatus.Na;
                    }
                }
            }
            //// Update the node's status in the list based on the checked state of the TreeNode
            //TestcaseNode nodeInList = testCaseTreeView.TestcaseNodes.FirstOrDefault(n => n.Name == node.Name);
            //if (nodeInList != null)
            //{
            //    nodeInList.Checked = node.Checked;
            //    nodeInList.Status = node.Checked ? TestcaseStatus.Pending : TestcaseStatus.Na;
            //    //Log($"{nodeInList.Name}:Checked = {nodeInList.Checked}\tStatus = {nodeInList.Status}");
            //}
        }

        //
        //SelectParents
        //
        private void SelectParents(TreeNode node, Boolean isChecked)
        {
            var parent = node.Parent;

            if (parent == null)
                return;
            if (!isChecked && HasCheckedNode(parent))
                return;
            parent.Checked = isChecked;
            SelectParents(parent, isChecked);
        }
        //
        //HasCheckedNode
        private bool HasCheckedNode(TreeNode node)
        {
            return node.Nodes.Cast<TreeNode>().Any(n => n.Checked);
        }

        public void deleteListView()
        {
            ListView lvwresult = lvwResult;
            ListViewItem item = new ListViewItem();
            lvwresult.Invoke(new MethodInvoker(delegate ()
            {
                lvwresult.Items[lvwResult.Items.Count - 1].Remove();
            }
            ));
        }
        public static void ShowParameters(DataGridView dtGridView, TestcaseNode testcaseNode)
        {
            dtGridView.DataSource = testcaseNode.Parameters;
            dtGridView.AutoResizeColumns((DataGridViewAutoSizeColumnsMode)ColumnHeaderAutoResizeStyle.ColumnContent);

        }

        #region Testing Log functions
        //Export result to HTML file
        public void ExporttoHtml()
        {
            try
            {
                string directoryPath = @"C:\test3gpp\OQC\32T32R10W\results\htmlfile";
                if (testingMode == "TX")
                {
                    directoryPath = directionPathResults + "Downlink" + "\\" + DateTime.Now.ToString("ddMMyyyy");
                }
                else
                {
                    directoryPath = directionPathResults + "Uplink" + "\\" + DateTime.Now.ToString("ddMMyyyy");
                }
                string filename = RruSerial.Trim() + "_PERFORMANCE-TEST_" + DateTime.Now.ToString("HHmm") + "_" + DateTime.Now.ToString("ddMMyyyy");
                DirectoryInfo directory = new DirectoryInfo(directoryPath);
                directory.Create();
                string resultFile = System.IO.Path.Combine(directoryPath, filename);
                resultFile = resultFile + ".html";
                StringBuilder strHTMLBuilder = new StringBuilder();
                strHTMLBuilder.Append("<html >");
                strHTMLBuilder.Append("<head >");
                strHTMLBuilder.Append("<body >");
                strHTMLBuilder.Append("<table border='1px' cellpadding='1' cellspacing='1' bgcolor='lightyellow' style='font-family:Garamond; font-size:smaller'>");
                strHTMLBuilder.Append("<tr >");
                lvwResult.Invoke(new MethodInvoker(delegate ()
                {
                    foreach (ColumnHeader ch in lvwResult.Columns)
                    {
                        strHTMLBuilder.Append("<td >");
                        strHTMLBuilder.Append(ch.Text);
                        strHTMLBuilder.Append("</td>");
                    }
                    foreach (ListViewItem lvi in lvwResult.Items)
                    {
                        strHTMLBuilder.Append("<tr >");
                        foreach (ListViewItem.ListViewSubItem lvs in lvi.SubItems)
                        {
                            strHTMLBuilder.Append("<td >");
                            strHTMLBuilder.Append(lvs.Text);
                            strHTMLBuilder.Append("</td>");
                        }
                        strHTMLBuilder.Append("</tr>");
                    }
                }));
                strHTMLBuilder.Append("</tr>");
                strHTMLBuilder.Append("</table>");
                strHTMLBuilder.Append("</body>");
                strHTMLBuilder.Append("</head>");
                strHTMLBuilder.Append("</html>");
                string htmltext = strHTMLBuilder.ToString();
                System.IO.File.WriteAllText(resultFile, htmltext);
            }
            catch (Exception e)
            {
                Log("Export to HTML: FAIL! " + e.Message, LogLevel.ERROR);

            }
        }

        public void ExportReport()
        {
            try
            {
                string flagPassFail = pass;
                for (int i = 0; i < 32; i++)
                {
                    if (dataPout[i, 0] == fail) { flagPassFail = fail; };
                    if (dataOBW[i, 0] == fail) { flagPassFail = fail; };
                    if (dataACLR[i, 0, 0] == fail) { flagPassFail = fail; };
                    if (dataACLR[i, 1, 0] == fail) { flagPassFail = fail; };
                    if (dataTotalDR[i, 0] == fail) { flagPassFail = fail; };
                    if (dataSEM[i, 0, 0] == fail) { flagPassFail = fail; };
                    if (dataSEM[i, 1, 0] == fail) { flagPassFail = fail; };
                    if (dataTXSUPR[i, 0] == fail) { flagPassFail = fail; };
                    for (int j = 0; j < 6; j++)
                    {
                        if (dataEVM[i, j, 0] == fail) { flagPassFail = fail; };
                        if (dataFreqErr[i, j, 0] == fail) { flagPassFail = fail; };
                    }

                    if (dataPsen[i, 0] == fail) { flagPassFail = fail; };
                    if (dataDR[i, 0] == fail) { flagPassFail = fail; };
                    if (dataACS[i, 0, 0] == fail) { flagPassFail = fail; };
                    if (dataINB[i, 0, 0] == fail) { flagPassFail = fail; };
                    if (dataNBB[i, 0, 0] == fail) { flagPassFail = fail; };
                    if (dataOBB[i, 0, 0] == fail) { flagPassFail = fail; };
                    if (dataICS[i, 0] == fail) { flagPassFail = fail; };
                    if (dataRXIMD[0, i, 0, 0] == fail) { flagPassFail = fail; };
                    if (dataRXIMD[1, i, 0, 0] == fail) { flagPassFail = fail; };
                }
                RruSerial = txtRRUSerial.Text;
                string resultFile = "";
                string filename = "";
                string time = DateTime.Now.ToString("ddMMyyyy");
                string directoryPath = @"C:\test3gpp\OQC\32T32R10W\Report";
                DirectoryInfo directory = new DirectoryInfo(directoryPath);
                directory.Create();
                filename = RruSerial.Trim() + "_PERFORMANCE-TEST_" + DateTime.Now.ToString("HHmm") + "_" + time + "_" + flagPassFail;
                resultFile = System.IO.Path.Combine(directoryPath, filename);
                resultFile = resultFile + ".xlsx";
                if (!File.Exists(resultFile))
                {
                    System.IO.File.Copy(@"C:\test3gpp\OQC\32T32R10W\Templates\MRU32T32R_Report_Template_v1_update.xlsx", resultFile);
                }
                // Set curor as hoursglass
                //Cursor.Current = Cursors.WaitCursor;
                Application.UseWaitCursor = true; //09.1.2024
                Excel.Application xlApp = new Excel.Application();
                Excel.Workbook xlWorkbook = xlApp.Workbooks.Open(resultFile);
                Excel._Worksheet worksheet5 = xlWorkbook.Sheets[5]; //worksheet TXRESULT
                Excel._Worksheet worksheet6 = xlWorkbook.Sheets[6]; //worksheet RXRESULT
                Excel.Range xlRange = worksheet5.UsedRange;
                xlApp.Visible = false;
                xlApp.UserControl = false;
                if (testingMode == "TX")
                {
                    for (int i = 0; i < 32; i++)
                    {
                        // Mang data[i,j]
                        // switch(j)
                        // j = 0 PASS/FAIL
                        // j = 1 Time
                        //j = 2 Result
                        //dataPout
                        worksheet5.Cells[i + 1, 5] = dataPout[i, 2];
                        worksheet5.Cells[i + 1, 8] = dataPout[i, 1];
                        worksheet5.Cells[i + 1, 9] = dataPout[i, 0];

                        //dataOBW
                        worksheet5.Cells[i + 33, 5] = dataOBW[i, 2];
                        worksheet5.Cells[i + 33, 8] = dataOBW[i, 1];
                        worksheet5.Cells[i + 33, 9] = dataOBW[i, 0];

                        //dataACLR
                        worksheet5.Cells[i + 65, 5] = dataACLR[i, 0, 2];
                        worksheet5.Cells[i + 65, 8] = dataACLR[i, 0, 1];
                        worksheet5.Cells[i + 65, 9] = dataACLR[i, 0, 0];

                        worksheet5.Cells[i + 97, 5] = dataACLR[i, 1, 2];
                        worksheet5.Cells[i + 97, 8] = dataACLR[i, 1, 1];
                        worksheet5.Cells[i + 97, 9] = dataACLR[i, 1, 0];

                        //dataOBUE
                        worksheet5.Cells[i + 129, 5] = dataSEM[i, 0, 2];
                        worksheet5.Cells[i + 129, 8] = dataSEM[i, 0, 1];
                        worksheet5.Cells[i + 129, 9] = dataSEM[i, 0, 0];

                        worksheet5.Cells[i + 161, 5] = dataSEM[i, 1, 2];
                        worksheet5.Cells[i + 161, 8] = dataSEM[i, 1, 1];
                        worksheet5.Cells[i + 161, 9] = dataSEM[i, 1, 0];

                        //dataEVM
                        for (int j = 0; j < 6; j++)
                        {
                            worksheet5.Cells[i + 193 + 32 * j, 5] = dataEVM[i, j, 2];
                            worksheet5.Cells[i + 193 + 32 * j, 8] = dataEVM[i, j, 1];
                            worksheet5.Cells[i + 193 + 32 * j, 9] = dataEVM[i, j, 0];
                        }

                        //dataFreqError
                        for (int j = 0; j < 6; j++)
                        {
                            worksheet5.Cells[i + 385 + 32 * j, 5] = dataFreqErr[i, j, 2];
                            worksheet5.Cells[i + 385 + 32 * j, 8] = dataFreqErr[i, j, 1];
                            worksheet5.Cells[i + 385 + 32 * j, 9] = dataFreqErr[i, j, 0];
                        }

                        //DataDynamic
                        worksheet5.Cells[i + 577, 5] = dataTotalDR[i, 2];
                        worksheet5.Cells[i + 577, 8] = dataTotalDR[i, 1];
                        worksheet5.Cells[i + 577, 9] = dataTotalDR[i, 0];

                        //DataOnOffPower
                        worksheet5.Cells[i + 609, 5] = dataOnOffPower[i, 2];
                        worksheet5.Cells[i + 609, 8] = dataOnOffPower[i, 1];
                        worksheet5.Cells[i + 609, 9] = dataOnOffPower[i, 0];

                        //DataOnOffTransient
                        worksheet5.Cells[i + 641, 5] = dataTransient[i, 2];
                        worksheet5.Cells[i + 641, 8] = dataTransient[i, 1];
                        worksheet5.Cells[i + 641, 9] = dataTransient[i, 0];

                        //DataTAE
                        worksheet5.Cells[i + 673, 5] = dataTAE[i, 2];
                        worksheet5.Cells[i + 673, 8] = dataTAE[i, 1];
                        worksheet5.Cells[i + 673, 9] = dataTAE[i, 0];
                        //DataSpurious
                        //DataIMD
                    }
                }

                else if (testingMode == "RX")
                {
                    for (int i = 0; i < 32; i++)
                    {
                        // Data Psen
                        //worksheet6.Cells[i + 272, 11] = dataPsen[i, 3];
                        //worksheet6.Cells[i + 272, 14] = dataPsen[i, 2];
                        //// Data Dynamic range 
                        //worksheet6.Cells[i + 280, 11] = dataDR[i, 3];
                        //worksheet6.Cells[i + 280, 14] = dataDR[i, 2];
                        //// Data ACS 
                        //worksheet6.Cells[i + 288, 11] = dataACS[i, 0, 3];
                        //worksheet6.Cells[i + 288, 14] = dataACS[i, 0, 2];
                        //// Data In band blocking
                        //worksheet6.Cells[i + 296, 11] = dataINB[i, 0, 3];
                        //worksheet6.Cells[i + 296, 14] = dataINB[i, 0, 2];
                        //// Data Narrow band blocking 
                        //worksheet6.Cells[i + 304, 11] = dataNBB[i, 0, 3];
                        //worksheet6.Cells[i + 304, 14] = dataNBB[i, 0, 2];
                        //// Data out of band blocking
                        //worksheet6.Cells[i + 312, 11] = dataOBB[i, 0, 3];
                        //worksheet6.Cells[i + 312, 14] = dataOBB[i, 0, 2];
                        // Data Receiver S
                        //worksheetx.Cells[67 + i, 7 - j] = dataICS[i, j];
                        // Data Gen IMD colum 76
                        // worksheetx.Cells[76 + i, 7 - j] = dataRXIMD[0, i, 0, j];
                        // Data Narrowband IMD colum 85
                        //worksheetx.Cells[85 + i, 7 - j] = dataRXIMD[1, i, 0, j];


                    }
                }

                //
                xlWorkbook.Save();
                Console.WriteLine(resultFile);

                //cleanup
                GC.Collect();
                GC.WaitForPendingFinalizers();

                // release com object to fully kill excel process running in the backgound
                Marshal.ReleaseComObject(xlRange);
                Marshal.ReleaseComObject(worksheet5);

                //close and release
                xlWorkbook.Close();
                Marshal.ReleaseComObject(xlWorkbook);

                //quit and release
                xlApp.Quit();
                Marshal.ReleaseComObject(xlApp);

                //Set cursor as default arrow
                //Cursor.Current = Cursors.Default;
                Application.UseWaitCursor = false;  //09.1.2024                
                lblStatusProgress_TextChanged("Kết quả đo: " + flagPassFail);
                //uploadtoServer.Send(testingMode, resultFile);
            }
            catch (Exception e)
            {
                Log("Export report: FAIL! " + e.Message, LogLevel.ERROR);
                lblStatusProgress_TextChanged("Save Results: FAIL");
            }
        }

        public void ExportListView()
        {
            try
            {
                string resultFile = "", filename;
                string directoryPath = @"C:\test3gpp\OQC\32T32R10W\results";
                if (testingMode == "TX")
                {
                    directoryPath = directionPathResults + "Downlink" + "\\" + DateTime.Now.ToString("ddMMyyyy");
                }
                else
                {
                    directoryPath = directionPathResults + "Uplink" + "\\" + DateTime.Now.ToString("ddMMyyyy");
                }
                filename = RruSerial.Trim() + "_PERFORMANCE-TEST_" + DateTime.Now.ToString("HHmm") + "_" + DateTime.Now.ToString("ddMMyyyy");
                DirectoryInfo directory;
                directory = new DirectoryInfo(directoryPath);
                directory.Create();
                resultFile = System.IO.Path.Combine(directoryPath, filename);
                resultFile = resultFile + ".xlsx";
                //Cursor.Current = Cursors.WaitCursor;
                Application.UseWaitCursor = true; //09.01.2024
                Excel.Application xlApp = new Excel.Application();
                xlApp.Visible = false;
                xlApp.DisplayAlerts = false;
                xlApp.UserControl = false;
                Excel.Workbook xlWorkbook = xlApp.Workbooks.Add(Type.Missing);
                Excel._Worksheet workSheet1 = xlWorkbook.Sheets[1];
                workSheet1 = (Excel._Worksheet)xlWorkbook.ActiveSheet;
                Excel.Range xlRange = workSheet1.UsedRange;
                workSheet1.Name = "sheet1";

                int i = 1;
                int j = 2;
                foreach (ColumnHeader ch in lvwResult.Columns)
                {
                    workSheet1.Cells[1, i] = ch.Text;
                    i++;
                }
                lvwResult.Invoke(new MethodInvoker(delegate ()
                {
                    foreach (ListViewItem lvi in lvwResult.Items)
                    {
                        i = 1;
                        {
                            foreach (ListViewItem.ListViewSubItem lvs in lvi.SubItems)
                            {
                                workSheet1.Cells[j, i] = lvs.Text;
                                i++;
                            }
                            j++;
                        }
                    }
                }));
                workSheet1.Cells.Select();
                workSheet1.Cells.EntireColumn.AutoFit();
                xlWorkbook.SaveAs(resultFile);
                xlApp.Quit();
                Console.WriteLine(resultFile);
                GC.Collect();
                GC.WaitForPendingFinalizers();
                Marshal.ReleaseComObject(xlRange);
                Marshal.ReleaseComObject(workSheet1);
                Marshal.ReleaseComObject(xlWorkbook);
                Marshal.ReleaseComObject(xlApp);
                ExporttoHtml();
                //Set cursor as default arrow
                //Cursor.Current = Cursors.Default;
                Application.UseWaitCursor = false; //09.01.2024
            }
            catch
            {
                Log("Export file fail", LogLevel.ERROR);
            }
        }
        public void ExportLogToTxt()
        {
            try
            {
                string filename = "";
                string directoryPath = $@"C:\test3gpp\OQC\32T32R10W\results\Downlink\{testCase}\{DateTime.Now.ToString("ddMMyyyy")}";

                filename = $"{RruSerial.Trim()}_LOG_{DateTime.Now.ToString("HHmm")}_{DateTime.Now.ToString("ddMMyyyy")}.txt";
                string filePath = $@"{directoryPath}\{filename}";
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    //print headers
                    //for(int i = 0; i < lvwLog.Columns.Count; i++)
                    //{
                    //    writer.Write(lvwLog.Columns[i].Text);
                    //    if(i < lvwLog.Columns.Count - 1)
                    //    {
                    //        writer.Write("\t");
                    //    }
                    //}
                    //writer.WriteLine();

                    //print data
                    foreach(ListViewItem item in lvwLog.Items)
                    {
                        writer.Write(item.Text);
                        for(int i = 1;i < lvwLog.Columns.Count;i++)
                        {
                            writer.Write("\t" + item.SubItems[i].Text);
                        }
                        writer.WriteLine();
                    }
                }
                Log($"Logfile exported to: {filePath}", LogLevel.INFO);

            }
            catch(Exception ex)
            {
                Log("Export logfile: FAIL! " + ex.Message, LogLevel.ERROR);
            }
        }

        public void DLCsvRecord(string[] data, object[] value, string[] result, string mode, string TM, int nvalue, int nresult, int port)
        {
            // data[]: la mang HEADER cua log VSA
            // value[]: la mang ket qua cua VSA tra ve
            // result[]: mang ket qua so sanh gia tri xau nhat
            // nvalue: so luong ket qua may do tra ve tuy moi bai do
            // nresult: so luong ket qua tuy moi bai do

            string time = DateTime.Now.ToString("ddMMyyyy");
            string directoryPath = @"C:\test3gpp\OQC\32T32R10W\logs\csv\" + "\\" + time;
            DirectoryInfo directory = new DirectoryInfo(directoryPath);
            directory.Create();
            string filepath = directoryPath + "\\" + RruSerial + "_" + mode + ".csv";
            bool fileExists = File.Exists(filepath);
            using (StreamWriter sw = new StreamWriter(filepath, true))
            {
                if (!fileExists)
                {
                    // Ghi các tên cột vào tệp nếu tệp chưa tồn tại
                    string[] header = new string[8] { "DATE", "TIME", "TYPE OF RRU", "CONFIG POWER", "FREQUENCY(MHz)", "MODE", "PORT", "TEST MODEL" };

                    for (int i = 0; i < 8; i++)
                    {
                        sw.Write(header[i] + ",");

                    }
                    for (int i = 0; i < (nvalue + nresult); i++)
                    {
                        sw.Write(data[i] + ",");
                    }
                }
                // Ghi dòng dữ liệu mới vào tệp
                try
                {
                    sw.Write("\n" + DateTime.Now.ToString("ddMMyyyy") + ",");
                    sw.Write(DateTime.Now.ToString("HH:mm:ss") + ",");
                    sw.Write("Pluto" + ",");
                    sw.Write(txtPower.Text + ",");
                    sw.Write(txtFrequency.Text + ",");
                    sw.Write(mode + ",");
                    sw.Write(port.ToString() + ",");
                    sw.Write(TM + ",");
                    for (int i = 0; i < nvalue; i++)
                    {
                        sw.Write(value[i].ToString() + ",");

                    }
                    for (int i = 0; i < nresult; i++)
                    {
                        sw.Write(result[i].ToString() + ",");
                    }
                }
                catch (Exception e)
                {
                    Log("Save Log to CSV file: FAIL! " + e.Message, LogLevel.ERROR);
                }
            }
        }

        public void ULCsvRecord(string specName, List<string> header, List<string> data)
        {
            string time = DateTime.Now.ToString("ddMMyyyy");
            string directoryPath = $@"{directionPathLogs}\csv\{time}";
            DirectoryInfo directory = new DirectoryInfo(directoryPath);
            directory.Create();
            string filepath = $@"{directoryPath}\{RruSerial.Trim()}_{specName}.csv";
            //Console.WriteLine(File.Exists(filepath).ToString());
            //Console.WriteLine(string.Join(",", header));

            if (!File.Exists(filepath))
            {
                using (StreamWriter sWriter = new StreamWriter(filepath))
                {
                    //write header
                    sWriter.WriteLine(string.Join(",", header));
                }
            }

            using (StreamWriter sWriter = new StreamWriter(filepath, append: true))
            {
                //write data
                sWriter.WriteLine(string.Join(",", data));
            }
        }

        public void ListViewShow(string switchRF, string Clause, string Spec, string limitMin, string result, string limitMax, string NRTM, string PassFail, string note)
        {
            count++;
            System.Threading.Thread.Sleep(100);
            ListView lvwresult = lvwResult;
            ListViewItem item = new ListViewItem();
            lvwResult.Invoke(new MethodInvoker(delegate ()
            {
                item.Text = switchRF;
                item.SubItems.Add(Clause);
                item.SubItems.Add(Spec);
                item.SubItems.Add(limitMin);
                item.SubItems.Add(result);
                item.SubItems.Add(limitMax);
                item.SubItems.Add(NRTM);
                //item.SubItems.Add(note);
                lvwresult.Items.Add(item);
                item.SubItems.Add(DateTime.Now.ToString("hh:mm:ss"));
                if (PassFail == pass)
                {
                    item.SubItems.Add(pass);
                    item.BackColor = Color.Green;
                }
                else
                {
                    item.SubItems.Add(fail);
                    item.BackColor = Color.Red;
                }
                item.SubItems.Add(note);  //tactTime
                lvwResult.Items[lvwResult.Items.Count - 1].EnsureVisible();
                System.Windows.Forms.Application.DoEvents();
            }));
        }
        #endregion

        #region Data Functions
        //====== 26.5.2024 - Code cho he Redhat =============
        private void portMappingFromExcel(string filename)
        {
            //xoa list
            PortMappings.Clear();
            int xRan = -1;
            int stream = -1;

            Log($"Reading PortMapping from:{filename}");
            Excel.Application xlApp = new Excel.Application();
            Excel.Workbook xlWorkbook = xlApp.Workbooks.Open(filename);
            Excel._Worksheet xlWorksheet = xlWorkbook.Sheets[1];
            Excel.Range xlRange = xlWorksheet.UsedRange;
            int rowCount = xlRange.Rows.Count;
            int colCount = xlRange.Columns.Count;

            //List<string> mappingPortHeaders = new List<string>() { "Stream Index", "Group 1", "Group 2", "Group 3", "Group 4" };
            List<string> mappingPortHeaders = new List<string>();
            List<string[]> mappingData = new List<string[]>();
            try
            {
                for (int i = 1; i <= rowCount; i++)
                {
                    string[] tmpData = new string[colCount];
                    for (int j = 1; j <= colCount; j++)
                    {
                        //doc header
                        if (i == 1)
                        {
                            mappingPortHeaders.Add(xlRange.Cells[1, j].Value.ToString());
                        }
                        else
                        {
                            //add mapping data
                            tmpData[j - 1] = xlRange.Cells[i, j].Value.ToString();

                            if (j > 1)
                            {
                                int portNum = int.Parse(xlRange.Cells[i, j].Value.ToString());
                                if (portNum > 0)
                                {
                                    //get xRan value and Stream Value
                                    string header1 = xlRange.Cells[1, j].Value.ToString();
                                    string header2 = xlRange.Cells[i, 1].Value.ToString();
                                    if (header1.Contains("Stream") || header1.Contains("Index"))
                                    {
                                        xRan = int.Parse(string.Join("", header2.Where(Char.IsDigit)));
                                        stream = int.Parse(string.Join("", header1.Where(Char.IsDigit)));
                                    }
                                    else if (header2.Contains("Stream") || header2.Contains("Index"))
                                    {
                                        xRan = int.Parse(string.Join("", header1.Where(Char.IsDigit)));
                                        stream = int.Parse(string.Join("", header2.Where(Char.IsDigit)));
                                    }

                                    PortMapping port = new PortMapping(portNum, xRan, stream);
                                    PortMappings.Add(port);
                                }
                            }
                        }
                    }
                    if (i > 1)
                    {
                        //add data to list
                        mappingData.Add(tmpData);
                    }
                }


                //print data to console
                PrintTable(mappingPortHeaders, mappingData);
            }
            catch (Exception ex)
            {
                Log($"Reading data got an error! {filename}" + ex.Message, LogLevel.ERROR);
            }
            finally
            {
                // Close Excel workbook and quit Excel application
                xlWorkbook?.Close(false);
                xlApp?.Quit();
                // Release COM objects
                System.Runtime.InteropServices.Marshal.ReleaseComObject(xlWorkbook);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(xlApp);
                // Force garbage collection to release memory
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

        private void PortListClearData(List<PortMapping> portList)
        {
            foreach(PortMapping port in portList)
            {
                port.Clear();
            }
        } 

        //TODO:Read data from XML - 13.6.2024
        private List<SpuriousRange> ReadSpurRangeFromXML(string filePath)
        {
            List<SpuriousRange> ranges = new List<SpuriousRange>();
            try
            {
                //Đọc file XML
                //log.Info("Load XML file from: " + filePath);
                Log("Load XML file from: " + filePath);
                XDocument document = XDocument.Load(filePath);
                // Query CorrectionFile path
                string correctionFileValue = document
                    .Descendants("Specification")
                    .Where(spec => (string)spec.Attribute("Name") == "Transmitter Spurious Emissions")
                    .Elements("CorrectionFile")
                    .Select(cf => (string)cf)
                    .FirstOrDefault();

                // Query the data and populate the list
                ranges = document
                    .Descendants("Specification")
                    .Where(spec => ((string)spec.Attribute("Name")).Contains("Transmitter Spurious Emissions"))
                    .Elements("RangeList")
                    .Elements("Range")
                    .Select(r => new SpuriousRange
                    {
                        Id = (int)r.Attribute("Id"),
                        Name = (string)r.Attribute("Name"),
                        StartFreq = (string)r.Attribute("StartFreq"),
                        StopFreq = (string)r.Attribute("StopFreq"),
                        ResBW = (string)r.Attribute("ResBW"),
                        VidBW = (string)r.Attribute("VidBW"),
                        RFCoupling = (string)r.Attribute("RFCoupling"),
                        SweepTime = (string)r.Attribute("sweepTime"),
                        TraceType = (string)r.Attribute("TraceType"),
                        Detector = (string)r.Attribute("Detector"),
                        AveNum = (int)r.Attribute("AveNum"),
                        Category = (string)r.Attribute("Category"),
                        ABSStartLim = (string)r.Attribute("ABSStartLim"),
                        ABSStopLim = (string)r.Attribute("ABSStopLim"),
                        CorrectionFile = correctionFileValue
                        //RruFreq = (string)r.Attribute("RruFreq"),
                        //RruNRTM = (string)r.Attribute("RruNRTM"),
                    }).ToList();
            }
            catch (Exception ew)
            {
                //log.Error(ew.Message);
                Log($"[Read Xml]: {ew.Message}");
            }
            return ranges;
        }

        //TODO: Load testcases from xml
        private List<SpuriousRange> LoadTestPlanFromXML(string filePath)
        {
            List<SpuriousRange> ranges = new List<SpuriousRange>();
            try
            {
                //Đọc file XML
                //log.Info("Load XML file from: " + filePath);
                Log("Load Testplan from: " + filePath);
                XDocument document = XDocument.Load(filePath);
                // Query CorrectionFile path
                string correctionFileValue = document
                    .Descendants("Specification")
                    .Where(spec => (string)spec.Attribute("Name") == "Transmitter Spurious Emissions")
                    .Elements("CorrectionFile")
                    .Select(cf => (string)cf)
                    .FirstOrDefault();

                // Query the data and populate the list
                ranges = document
                    .Descendants("Specification")
                    .Where(spec => ((string)spec.Attribute("Name")).Contains("Transmitter Spurious Emissions"))
                    .Elements("RangeList")
                    .Elements("Range")
                    .Select(r => new SpuriousRange
                    {
                        Id = (int)r.Attribute("Id"),
                        Name = (string)r.Attribute("Name"),
                        StartFreq = (string)r.Attribute("StartFreq"),
                        StopFreq = (string)r.Attribute("StopFreq"),
                        ResBW = (string)r.Attribute("ResBW"),
                        VidBW = (string)r.Attribute("VidBW"),
                        RFCoupling = (string)r.Attribute("RFCoupling"),
                        SweepTime = (string)r.Attribute("sweepTime"),
                        TraceType = (string)r.Attribute("TraceType"),
                        Detector = (string)r.Attribute("Detector"),
                        AveNum = (int)r.Attribute("AveNum"),
                        Category = (string)r.Attribute("Category"),
                        ABSStartLim = (string)r.Attribute("ABSStartLim"),
                        ABSStopLim = (string)r.Attribute("ABSStopLim"),
                        CorrectionFile = correctionFileValue
                        //RruFreq = (string)r.Attribute("RruFreq"),
                        //RruNRTM = (string)r.Attribute("RruNRTM"),
                    }).ToList();
            }
            catch (Exception ew)
            {
                //log.Error(ew.Message);
                Log($"[Read Xml]: {ew.Message}");
            }
            return ranges;
        }
        private void NewSpuriousMeasurement()
        {
            List<SpuriousRange> resultList = new List<SpuriousRange>();
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "XML Files|*.xml";
            openFileDialog.Title = "Chọn tệp tin XML";
            //openFileDialog.InitialDirectory = "..\\..\\XmlTestPlans";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = openFileDialog.FileName;
                string fileName = System.IO.Path.GetFileName(filePath);
                spurRanges = ReadSpurRangeFromXML(filePath);
            }

            //test - chay ham setup Swept SA bai do Spurious emissions
            foreach (var rng in spurRanges)
            {
                //phat RRU
                //MessageBox.Show("Đổi sang Port" + (j + 1));
                if (measurementBackgroundWorker.CancellationPending) return;
                System.Threading.Thread.Sleep(1);
                BBU.CHG_RRU_POWER(int.Parse(txtPWR.Text));

                //if (submeasClause[i1, 0] == "++ General")
                //{
                //    limitMax = double.Parse(submeasClause[i1, 3]);
                //    BBU.CHG_RRU_POWER(int.Parse(txtPWR.Text));
                //    // Spurious General 
                //    Measurement.Spur5G(VSA, BBU, "General", NRTM, arrAttDL[j], j + 1, setFrequency, "", out tseRS);
                //    //==============19.2.2024 save screen shot =====================
                //    SaveCSVdata(VSA, "General", measClause[i, 0], "P" + (j + 1), setFrequency);
                //    TakeScreenshotVSA(VSA, "General", measClause[i, 0], "P" + (j + 1), setFrequency);
                //    BBU.CHG_RRU_POWER(1000);
                //    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                //    for (int i2 = 0; i2 < tseRS.Length; i2++)
                //    {
                //        if (tseRS[i2] != null)
                //        {
                //            TSE[0, i2] = tseRS[i2];
                //            if (TSE[0, i2] > 0)
                //            {
                //                reTest = true;
                //            }
                //            else { reTest = false; }
                //        }
                //    }
                //}
                //delay time
                System.Threading.Thread.Sleep(3000);

                //get result and save data to csv
                VSA.SetupIMDTSE(rng);
                resultList.Add(rng);
                // Luu ket qua vao file csv tren may tinh
                List<string> csvHeader = new List<string> { "RangeID", "RangeName", "StartFreq", "StopFreq", "LimStart", "LimStop", "ResultFreq", "ResultLevel", "Pass/Fail" };
                List<string> csvData = new List<string> { rng.Id.ToString(), rng.Name, rng.StartFreq, rng.StopFreq, rng.ABSStartLim, rng.ABSStopLim, rng.ResultFreq, rng.ResultLevel, rng.ResultPassFail };

                ULCsvRecord("Transmitter Spurious Emission - General", csvHeader, csvData);
            }
            //test doc ket qua - sau thay bang ham luu csv file
            Log("Spurious Measurement Results:");
            string line = string.Format("|{0,-10}|{1,-10}|{2,-10}|{3,-10}|{4,-10}|{5,-10}|{6,-15}|{7,-10}|{8,-10}|", "RangeID", "RangeName", "StartFreq", "StopFreq", "LimStart", "LimStop", "ResultFreq", "ResultLevel", "Pass/Fail");
            //Console.WriteLine($"RangeID\tRangeName\tStartFreq\tStopFreq\tLimStart\tLimStop\tResultFreq\tResultLevel\tPass/Fail");
            //Console.WriteLine("{0,10}|{1,10}|{2,10}|{3,10}|{4,10}|{5,10}|{6,15}|{7,10}|{8,10}", "RangeID", "RangeName", "StartFreq", "StopFreq", "LimStart", "LimStop", "ResultFreq", "ResultLevel", "Pass/Fail");
            for (int i = 0; i < line.Length; i++)
            {
                Console.Write("-");
            }
            Console.WriteLine();
            Console.WriteLine(line);
            for (int i = 0; i < line.Length; i++)
            {
                Console.Write("-");
            }
            Console.WriteLine();

            foreach (SpuriousRange r in resultList)
            {
                //Console.WriteLine($"{r.Id}\t{r.Name}\t{r.StartFreq}\t{r.StopFreq}\t{r.ABSStartLim}\t{r.ABSStopLim}\t{r.ResultFreq}\t{r.ResultLevel}\t{r.ResultPassFail}");
                //Console.WriteLine("{0,10}|{1,10}|{2,10}|{3,10}|{4,10}|{5,10}|{6,15}|{7,10}|{8,10}", r.Id, r.Name, r.StartFreq, r.StopFreq, r.ABSStartLim, r.ABSStopLim,r.ResultFreq,r.ResultLevel,r.ResultPassFail);
                Console.WriteLine("{0,-10}|{1,-10}|{2,-10}|{3,-10}|{4,-10}|{5,-10}|{6,-15}|{7,-10}|{8,-10}", r.Id, r.Name, r.StartFreq, r.StopFreq, r.ABSStartLim, r.ABSStopLim, r.ResultFreq, r.ResultLevel, r.ResultPassFail);
            }

        }

        //19.2.2024 - VSA Screen Shot
        private void TakeScreenshotVSA(VSA vsa, string testCase, string specName, string port, string freq, string passfail="")
        {
            string screenShotFilePath = $"C:\\Screenshots\\{DateTime.Now.ToString("ddMMyyyy")}\\{RruSerial}\\{testCase}\\{specName}\\{(double.Parse(freq)/1e6)}MHz\\{port}_{DateTime.Now.ToString("HHmmss")}_{passfail.ToUpper()}.png";
            vsa.TakeScreenshot(screenShotFilePath);
            System.Threading.Thread.Sleep(3000);
            vsa.SingleSweepOnOff(1); //change to Continous Sweep
        }
        // 22.3.2024 - VSA Store CSV trace
        public void SaveCSVdata(VSA vsa, string testCase, string specName, string port, string freq)
        {
            string filePath = $"C:\\CSVData\\{DateTime.Now.ToString("ddMMyyyy")}";
            vsa.MakeDir(filePath);
            vsa.SaveCSVFile($"{filePath}\\{testCase}_{freq}_{port}_{DateTime.Now.ToString("HHmmss")}.csv");
        }

        private void PrintTable(List<string> headers, List<string[]> data)
        {
            log.PrintTable(headers, data);
        }


        #endregion

        ////================= Read ATT from CSV Files ==================
        //public void ReadATTfromCSVdir(string dir, string format, double FindFreq)
        //{
        //    if (AttenuatorCSVpath != dir || SetFrequencyValue != FindFreq)
        //    {
        //        //LogDashedLine($"RRU ATTENUATORS");
        //        //Log($"Reading RRU attenuators: Set frequency = {FindFreq}\tSource = {dir}");
        //        var list = Directory.GetFiles(dir, "*.csv");
        //        for (int j = 1; j <= list.Count(); j++)
        //        {
        //            string path = $@"{dir}\{format}{j}.csv";
        //            Attenuators.Add(ReadATTfromCSVFile(path, FindFreq));
        //        }
        //        //LogDashedLine();

        //        //update properties
        //        AttenuatorCSVpath = dir;
        //        SetFrequencyValue = FindFreq;
        //    }
        //}
        ////=====================================
        //public double ReadATTfromCSVFile(string path, double freqHz)
        //{
        //    double tempVal = 0.0;
        //    double min = 1.0, max = 100.0;
        //    if (File.Exists(path))
        //    {
        //        try
        //        {
        //            double tmp = 10.0;
        //            var csvConfig = new CsvConfiguration(CultureInfo.CurrentCulture)
        //            {
        //                HasHeaderRecord = true,
        //                Comment = '#',
        //                AllowComments = true,
        //                //Delimiter = ",",
        //                DetectDelimiter = true,
        //                DetectDelimiterValues = new[] { ",", ";", "\t", "|" },
        //            };
        //            using (var streamReader = new StreamReader(path, Encoding.UTF8))
        //            using (var csv = new CsvReader(streamReader, csvConfig))
        //            {

        //                var records = csv.GetRecords<dynamic>();
        //                foreach (var record in records)
        //                {
        //                    if (double.TryParse(csv.GetField(0), out double freq))
        //                    {
        //                        tmp = Math.Abs((freqHz - freq) / 1e6);
        //                        if (tmp < min)
        //                        {
        //                            double.TryParse(csv.GetField(1), out tempVal);
        //                            tempVal = Math.Abs(Math.Round(tempVal, 3));  //lay ve gia tri duong
        //                            break;
        //                        }
        //                        else if (tmp <= max)
        //                        {
        //                            double.TryParse(csv.GetField(1), out tempVal);
        //                            tempVal = Math.Abs(Math.Round(tempVal, 3));  //lay ve gia tri duong
        //                            max = tmp;
        //                        }

        //                    }
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            Error(ex, $"[RRU ReadATTfromCSVFile] Read ATT from CSVFile:{path} got error!");
        //            Log($"[RRU ReadATTfromCSVFile] CSVFile:{path} not exist!");
        //        }
        //    }
        //    else Log($"[RRU ReadATTfromCSVFile] CSVFile:{path} not exist!");

        //    return tempVal;
        //}

        private void GeneralDataSetupTX(string losspath, string clause, int testingPort, string mode, out double attSetVal)
        {
            if (measurementBackgroundWorker.CancellationPending) { attSetVal = 0; return; }
            System.Threading.Thread.Sleep(1);
            ProgressLabel($"Setting up environment:{clause}: Port{testingPort}...");
            //tactTime
            timeStep = DateTime.Now;
            //update data RRU
            RRU.Testcase = TestCase.TX;
            testCase = "Downlink";
            RRU.CurrentTestingPort = testingPort;
            RRU.CurrentTestingSpec = clause;
            RRU.TestModel = mode;
            LogDashedLine($"++++++++ {RRU.CurrentTestingSpec}:Port {testingPort} ++++++++");
            lblStatusProgress_TextChanged(RRU.CurrentTestingSpec + " :Port " + (testingPort));
            //doc csv
            RRU.ReadATTfromCSVdir(rruCSVFilePathTX, "TX", double.Parse(setFrequency));  //23.7.2024 - dang co van de o ham nay test bo lenh nay di
            string path = $@"{rruCSVFilePathTX}\TX{testingPort}.csv";
            double.TryParse(setFrequency, out double freq);
            attSetVal = RRU.ReadATTfromCSVFile(path, freq);

            Log($"[{RRU.Serial}][PORT:{testingPort}]: ATT Level={RRU.Attenuators[testingPort - 1]} dB at f={RRU.SetFrequencyValue / 1000000} MHz");
            Log($"RRU {RRU.Serial}: Path = {RRU.AttenuatorCSVpath}");



            //Update vao AttDL[] - tam thoi, sau se bo
            for (int i = 0; i < RRU.Attenuators.Count; i++)
            {
                arrAttDL[i] = RRU.Attenuators[i].ToString();
            }
            //update vaof frame ATT
            frmAttenuator.UpdateAttDL(arrAttDL);

            RFSwitchRRUPort("TX", testingPort - 1);

        }

        public double GeneralDataSetupRX(string clause, int testingPort, out double attSetVal)
        {
            if (measurementBackgroundWorker.CancellationPending) { attSetVal = 0; return attSetVal; }
            attSetVal = 0;
            //tactTime
            timeStep = DateTime.Now;
            //update data RRU
            RRU.Testcase = TestCase.RX;
            testCase = "Uplink";
            RRU.CurrentTestingPort = testingPort;
            RRU.CurrentTestingSpec = clause;
            ProgressLabel($"Setting up environment:{clause}: Port{testingPort}...");
            LogDashedLine($"++++++++ {RRU.CurrentTestingSpec}:Port {testingPort} ++++++++");
            lblStatusProgress_TextChanged(RRU.CurrentTestingSpec + " :Port " + (testingPort));
            //doc csv
            if (VSG1 != null)
            {
                VSG1.AssignAlias("Wanted Signal");

                //VSG1.ReadATTfromCSVdir(wsCSVFilePathRX, "RX", VSG1.SetFrequencyValue);                
                string path = $@"{wsCSVFilePathRX}\RX{testingPort}.csv";
                double.TryParse(setFrequency, out double freq);
                attSetVal = RRU.ReadATTfromCSVFile(path, freq);
                Log($"{VSG1.Name}: Set ATT_Port{testingPort}={attSetVal} dB at f={freq / 1e6} MHz");
                Log($"{VSG1.Name}: Path = {path}");
            }
            //Log($"{VSG1.Name}: Set ATT_Port{testingPort}={VSG1.Attenuators[testingPort]} dB at f={VSG1.SetFrequencyValue / 1e6} MHz");
            //Log($"{VSG1.Name}: Path = {VSG1.AttenuatorCSVpath}");
            //if (VSG2!=null) VSG2.ReadATTfromCSVdir(wsCSVFilePathRX, "ISRX", double.Parse(setFrequency));
            //if (VSG3!=null) VSG3.ReadATTfromCSVdir(wsCSVFilePathRX, "CWRX", double.Parse(setFrequency));
            //Update vao AttUL[] - tam thoi, sau se bo
            //for (int i = 0; i < VSG1.Attenuators.Count; i++)
            //{
            //    arrAttUL[i] = VSG1.Attenuators[i].ToString();
            //}
            ////update vao frame ATT
            //frmAttenuator.UpdateAttUL(arrAttUL);

            if (measurementBackgroundWorker.CancellationPending) return attSetVal;
            System.Threading.Thread.Sleep(1);
            return attSetVal;
            RFSwitchRRUPort("RX", testingPort - 1);
        }

    }
}
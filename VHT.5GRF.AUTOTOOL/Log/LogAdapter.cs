using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _5GAutoTool
{
    public enum LogLevel : uint
    {
        ERROR,
        WARN,
        SUCCESS,
        INFO,
        FAILED,
        DASH,
    }
    public class LogAdapter
    {
        private ListView _listView;
        private TextBox _searchTextBox;
        private Label _searchCountLabel;
        private List<int> _searchResults;
        private int _currentResultIndex;

        public ListView LvwError;

        public Form5GAT mainForm;
        public List<ListView> ViewLogs { get; private set; } = new List<ListView>();



        private Color searchColorResult = Color.Purple;
        private Color selectResultColor = Color.Blue;

        private static LogAdapter instance;

        public LogAdapter(ListView listView, TextBox searchTextBox, Label searchCountLabel, Button preButton, Button nextButton)
        {
            _listView = listView;
            _searchTextBox = searchTextBox;
            _searchCountLabel = searchCountLabel;

            _searchTextBox.TextChanged += SearchTextBox_TextChanged;
            preButton.Click += btnPre_Click;
            nextButton.Click += btnNext_Click;
        }

        public LogAdapter(ListView listView, Form5GAT mainForm)
        {
            this.mainForm = mainForm;
            _listView = listView;
            //_listView.Columns.Clear();

            //LvwError = CloneListView(_listView);

            CreateLogViews(listView);
        }


        public void setInstance(LogAdapter lat)
        {
            instance = lat;
        }

        private LogAdapter() { }

        public static LogAdapter Instance
        {
            get
            {
                if (instance == null)
                {
                    return new LogAdapter();
                }
                return instance;
            }
        }

        private void SearchTextBox_TextChanged(object sender, EventArgs e)
        {
            ClearSearchHighlights();

            string searchText = _searchTextBox.Text;

            if (!string.IsNullOrEmpty(searchText))
            {
                int searchCount = SearchAndHighlight(searchText);
                _searchCountLabel.Text = $"{searchCount} Results";
            }
            else
            {
                _searchCountLabel.Text = "0 Results";
            }
        }

        private int SearchAndHighlight(string searchText)
        {
            _searchResults = new List<int>();
            _currentResultIndex = -1;

            int searchCount = 0;

            foreach (ListViewItem item in _listView.Items)
            {
                if (item.SubItems[2].Text.Contains(searchText))
                {
                    _searchResults.Add(item.Index);
                    searchCount++;
                    item.BackColor = searchColorResult;
                }
            }

            if (_searchResults.Count > 0)
            {
                _currentResultIndex = 0;
                SelectSearchResult(_currentResultIndex);
            }
            else
            {
                ClearSearchHighlights();
            }

            return searchCount;
        }

        private void btnPre_Click(object sender, EventArgs e)
        {
            if (_currentResultIndex > 0)
            {
                _currentResultIndex--;
                SelectSearchResult(_currentResultIndex);
            }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (_currentResultIndex < _searchResults.Count - 1)
            {
                _currentResultIndex++;
                SelectSearchResult(_currentResultIndex);
            }
        }

        private void SelectSearchResult(int index)
        {
            if (index >= 0 && index < _searchResults.Count)
            {
                _listView.Items[_searchResults[index]].EnsureVisible();
                _listView.Items[_searchResults[index]].Selected = true;
                _listView.Focus();

                _searchCountLabel.Text = $"{index + 1}/{_searchResults.Count} Results";
            }
        }

        private void ClearSearchHighlights()
        {
            foreach (ListViewItem item in _listView.Items)
            {
                item.BackColor = _listView.BackColor;
            }
        }
        public void PrintDictionary(List<string> headers, Dictionary<string, List<int>> dict)
        {
            List<string[]> data = ConvertDictToList(dict);
            foreach (string[] s in data)
            {
                Log(string.Join(" / ", s));
            }
            PrintTable(headers, data);
        }
        //print table to consle
        public void PrintTable(List<string> headers, List<string[]> data)
        {
            // Calculate column widths
            int[] columnWidths = GetColumnWidths(headers, data);

            // Print separator
            PrintSeparator(columnWidths);
            // Print headers
            PrintRow(headers.ToArray(), columnWidths);

            // Print separator
            PrintSeparator(columnWidths);

            // Print data rows
            foreach (var row in data)
            {
                PrintRow(row, columnWidths);
            }
            // Print separator
            PrintSeparator(columnWidths);
        }

        private int[] GetColumnWidths(List<string> headers, List<string[]> data)
        {
            int numberOfColumns = headers.Count;
            int[] columnWidths = new int[numberOfColumns];

            // Initialize column widths to header lengths
            for (int i = 0; i < numberOfColumns; i++)
            {
                columnWidths[i] = headers[i].Length;
            }

            // Adjust column widths based on data
            foreach (var row in data)
            {
                for (int i = 0; i < numberOfColumns; i++)
                {
                    columnWidths[i] = (row[i] != null) ? Math.Max(columnWidths[i], row[i].Length) : columnWidths[i];
                    columnWidths[i] += 1;
                }
            }
            return columnWidths;
        }
        private void PrintRow(string[] row, int[] columnWidths)
        {
            string rowData = "";
            for (int i = 0; i < row.Length; i++)
            {
                //Console.Write(row[i].PadRight(columnWidths[i] + 2));
                string tmpData = row[i] ?? "";
                rowData += tmpData.PadRight(columnWidths[i]);
            }
            Log(rowData, LogLevel.DASH);
        }

        private void PrintSeparator(int[] columnWidths)
        {
            string separator = "";
            foreach (var width in columnWidths)
            {
                separator += new string('-', width);
            }
            Log(separator, LogLevel.DASH);
        }

        public void SaveLogToFile(string fileName)
        {
            using (StreamWriter writer = new StreamWriter(fileName))
            {
                foreach (ListViewItem item in _listView.Items)
                {
                    // Write each item and its subitems to the file, separated by tabs
                    writer.WriteLine(string.Join("\t", item.SubItems.Cast<ListViewItem.ListViewSubItem>().Select(subItem => subItem.Text)));
                }
            }
        }
        public async void Log(string text, LogLevel logLevel = LogLevel.INFO, string device = "", string source = "")
        {
            string strTime = DateTime.Now.ToString("dd/MM/yyyy HH:mm.ss") + ":";
            switch (logLevel)
            {
                case LogLevel.ERROR:
                    await Append(strTime, LogLevel.ERROR, text, device, source);
                    break;
                case LogLevel.WARN:
                    await Append(strTime, LogLevel.WARN, text, device, source);
                    break;
                case LogLevel.SUCCESS:
                    await Append(strTime, LogLevel.SUCCESS, text, device, source);
                    break;
                case LogLevel.FAILED:
                    await Append(strTime, LogLevel.FAILED, text, device, source);
                    break;
                case LogLevel.INFO:
                    await Append(strTime, LogLevel.INFO, text, device, source);
                    break;
                case LogLevel.DASH:
                    await Append("", LogLevel.DASH, text);
                    break;
            }

        }
        private async Task Append(string timestamp, LogLevel logLevel, string logMessage, string device = "", string source = "")
        {
            await Task.Run(() =>
            {
                if (_listView.InvokeRequired)
                {
                    _listView.Invoke(new Action(() => AddLogToListView(timestamp, logLevel, logMessage, device, source)));
                }
                else
                {
                    AddLogToListView(timestamp, logLevel, logMessage, device, source);
                }
            });

            // Save to log file
            //await SaveLogToFile(timestamp, logLevel, logMessage, device, source);
        }

        private void AddLogToListView(string timestamp, LogLevel logLevel, string logMessage, string device = "", string source = "")
        {
            _listView.BeginUpdate();

            ListViewItem newItem = new ListViewItem(timestamp);
            string logLvl = (logLevel != LogLevel.DASH) ? $"[{logLevel}]" : "";
            string logDevice = (device != "") ? $"[{device}]" : "";
            newItem.SubItems.Add(logLvl);
            newItem.SubItems.Add(logDevice);
            newItem.SubItems.Add(logMessage);
            newItem.SubItems.Add(source);
            newItem.ForeColor = GetLogLevelColor(logLevel);
            _listView.Items.Add(newItem);
            _listView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            // Scroll to the last item
            _listView.EnsureVisible(_listView.Items.Count - 1);

            _listView.EndUpdate();

            //add log to listview filtered
            ListViewItem cloneItem = (ListViewItem)newItem.Clone();

            //ViewLogs[0].Items.Add(cloneItem);

            ListView filteredLog = ViewLogs.FirstOrDefault(n => n.Name == logLevel.ToString());
            filteredLog.Items.Add(cloneItem);
            filteredLog.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            filteredLog.EnsureVisible(filteredLog.Items.Count - 1);

            //foreach (ListView view in ViewLogs)
            //{
            //    if (view.Name == logLevel.ToString())
            //    {
            //        view.Items.Add(cloneItem);
            //        view.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            //        view.EnsureVisible(view.Items.Count - 1);
            //        break;
            //    }
            //}


        }
        public async Task SaveLogToFile(string timestamp, LogLevel logLvl, string logMessage, string device = "", string source = "")
        {
            string serial = mainForm.RruSerial?.Trim() ?? "";
            string strLogLevel = (logLvl != LogLevel.DASH) ? $"[{logLvl}]" : "";
            string strLogDevice = (device != "") ? $"[{device}]" : "";
            string time = DateTime.Now.ToString("ddMMyyyy");
            string directoryPath = Path.Combine(mainForm.directionPathTemplate, "logs", time);

            // Ensure the directory exists
            Directory.CreateDirectory(directoryPath);

            // Build the log file path
            string filepath = Path.Combine(directoryPath, $"{serial}_{time}.txt");

            // Use StreamWriter with 'using' to ensure proper disposal
            using (FileStream fs = new FileStream(filepath, FileMode.Append, FileAccess.Write, FileShare.Read))
            using (StreamWriter sWriter = new StreamWriter(fs, Encoding.UTF8))
            {
                string text = $"{timestamp}:{strLogLevel}\t{strLogDevice}:{logMessage}";
                await sWriter.WriteLineAsync(text);
            }
        }
        private Color GetLogLevelColor(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.ERROR:
                    return Color.Red;
                case LogLevel.WARN:
                    return Color.Magenta;
                case LogLevel.SUCCESS:
                    return Color.LimeGreen;
                case LogLevel.FAILED:
                    return Color.Red;
                case LogLevel.INFO:
                    return Color.Black; //Color.FromArgb(173, 216, 230); Màu mặc định
                case LogLevel.DASH:
                    return Color.Black;
                default:
                    return Color.Black;
            }
        }
        private List<string[]> ConvertDictToList(Dictionary<string, List<int>> dict)
        {
            List<string[]> result = new List<string[]>();

            foreach (var entry in dict)
            {
                string[] values = new string[] { entry.Key, string.Join(", ", entry.Value) };

                // Add the array to the result list
                result.Add(values);
            }

            return result;
        }
        public ListView CloneListView(ListView source)
        {
            ListView clonedListView = new ListView
            {
                Dock = DockStyle.Fill,
                View = source.View,
                FullRowSelect = source.FullRowSelect,
                GridLines = source.GridLines,
                MultiSelect = source.MultiSelect,
                HideSelection = source.HideSelection
            };

            // Clone columns
            foreach (ColumnHeader column in source.Columns)
            {
                clonedListView.Columns.Add((ColumnHeader)column.Clone());
            }

            // Clone items
            foreach (ListViewItem item in source.Items)
            {
                clonedListView.Items.Add((ListViewItem)item.Clone());
            }

            return clonedListView;
        }
        private ListView CreateNewListViewFromSource(ListView source)
        {
            ListView clonedListView = new ListView
            {
                Dock = DockStyle.Fill,
                View = source.View,
                FullRowSelect = source.FullRowSelect,
                GridLines = source.GridLines,
                MultiSelect = source.MultiSelect,
                HideSelection = source.HideSelection
            };

            // Clone columns
            foreach (ColumnHeader column in source.Columns)
            {
                clonedListView.Columns.Add((ColumnHeader)column.Clone());
            }

            return clonedListView;
        }
        public void CreateLogViews(ListView source)
        {
            ViewLogs.Clear();
            foreach (var level in Enum.GetValues(typeof(LogLevel)))
            {
                ListView logView = new ListView
                {
                    Name = level.ToString(),
                    Dock = source.Dock,
                    View = source.View,
                    FullRowSelect = source.FullRowSelect,
                    GridLines = source.GridLines,
                    MultiSelect = source.MultiSelect,
                    HideSelection = source.HideSelection,
                    Width = source.Width,
                    Height = source.Height,
                    Anchor = source.Anchor,
                    Location = source.Location,
                    HeaderStyle = source.HeaderStyle,
                    Scrollable = true,
                };
                foreach (ColumnHeader header in source.Columns)
                {
                    logView.Columns.Add((ColumnHeader)header.Clone());
                }

                ViewLogs.Add(logView);
            }
        }

    }
}

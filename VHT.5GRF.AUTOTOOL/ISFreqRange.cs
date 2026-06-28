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
    public partial class ISFreqRange : Form
    {
        public ISFreqRange()
        {
            InitializeComponent();
        }
        private Form5GAT mainForm = null;
        private List<double[]> ranges = new List<double[]>();
        private ListView currentListView;

        public object ContextMenuStrip1 { get; private set; }

        public ISFreqRange(Form callingForm)
        {
            mainForm = callingForm as Form5GAT;
            ranges = mainForm.ListISFreqRange;
            InitializeComponent();
            updateListView();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            //cap nhat ranges tu listview
            ranges.Clear();
            foreach (ListViewItem item in lvwFreqRange.Items)
            {
                double.TryParse(item.Text, out double from);
                double.TryParse(item.SubItems[1].Text, out double to);
                //Console.WriteLine(from + " === " + to);
                ranges.Add(new double[] { from*1e6, to*1e6 });
            }
            //debug
            foreach(var range in ranges)
            {
                Console.WriteLine("List:" + range[0] + " , " + range[1]);
            }
            //cap nhat listview o mainForm
            mainForm.ListISFreqRange = ranges;
            currentListView = lvwFreqRange;
            //dong cua so
            this.Close();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            //add data to listview
            //ListViewItem addItem = new ListViewItem(txtFreqFrom.Text);
            //addItem.SubItems.Add(txtFreqTo.Text);
            //lvwFreqRange.Items.Add(addItem);
            try
            {
                double.TryParse(txtFreqFrom.Text,out double from);
                double.TryParse(txtFreqTo.Text, out double to);
                Console.WriteLine(from + ";" + to);
                //ranges.Add(new double[] { from, to });
                ListViewItem item = new ListViewItem(from.ToString());
                item.SubItems.Add(to.ToString());
                lvwFreqRange.Items.Add(item);
                //MessageBox.Show($"added range from {from} to {to}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Invalid Input! " + ex.Message);
            }
            //updateListView();
            
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            //lay lai listview cu
            ranges.Clear();
            ranges = mainForm.ListISFreqRange;
            lvwFreqRange = currentListView;
            //dong cua so
            this.Close();
        }
        private void updateListView()
        {
            //xoa listview cu
            lvwFreqRange.Items.Clear();

            //add gia tri moi
            foreach (var range in ranges)
            {
                ListViewItem item = new ListViewItem(range[0].ToString());
                item.SubItems.Add(range[1].ToString());
                lvwFreqRange.Items.Add(item);
            }
        }


        private void Remove_Click(object sender, EventArgs e)
        {
            if(lvwFreqRange.SelectedItems.Count > 0)
            {
                foreach(ListViewItem item in lvwFreqRange.SelectedItems)
                {
                    lvwFreqRange.Items.Remove(item);
                }
            }
        }


        //=================
        private void lvwFreqRange_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (lvwFreqRange.FocusedItem != null && lvwFreqRange.FocusedItem.Bounds.Contains(e.Location) == true)
                {
                    ContextMenu m = new ContextMenu();
                    MenuItem delMenuItem = new MenuItem("Delete");                    
                    m.MenuItems.Add(delMenuItem);
                    m.Show(lvwFreqRange, new Point(e.X, e.Y));
                    delMenuItem.Click += delegate (object sender2, EventArgs e2) {
                        DeletetAction(sender, e);
                    };// your action here

                }
            }
        }

        private void DeletetAction(object sender, MouseEventArgs e)
        {
            ListView ListViewControl = sender as ListView;
            foreach (ListViewItem eachItem in ListViewControl.SelectedItems)
            {
                // you can use this idea to get the ListView header's name is 'Id' before delete
                ListViewControl.Items.Remove(eachItem);
                removeItemInList(eachItem);
            }
        }

        private void ActionClick(object sender, MouseEventArgs e, string id)
        {
            //id is extra value when you need or delete it
            ListView ListViewControl = sender as ListView;
            foreach (ListViewItem tmpLstView in ListViewControl.SelectedItems)
            {
                Console.WriteLine(tmpLstView.Text);
            }

        }
        private void removeItemInList(ListViewItem item)
        {
            foreach (var range in ranges)
            {
                //if(range[0]==double.Parse(item.ToString()) && range[1] == double.Parse(item.SubItems.ToString())) 
                //{
                //    ranges.Remove(range);
                //}
                //MessageBox.Show(range[0] + " , " + double.Parse(item.ToString()) + " , " + range[1] + " , " + double.Parse(item.SubItems.ToString()));

            }
        }

        private void ISFreqRange_Load(object sender, EventArgs e)
        {
            lvwFreqRange.MouseUp += new MouseEventHandler(lvwFreqRange_MouseClick);
            currentListView = lvwFreqRange;
        }
    }
}

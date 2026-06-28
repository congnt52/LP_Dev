using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _5GAutoTool
{
    public class DoubleBufferedListView : ListView
    {
        public DoubleBufferedListView()
        {
            // Enable double buffering to reduce flickering
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer |
                          ControlStyles.AllPaintingInWmPaint, true);
            this.UpdateStyles();
        }
    }

    public class DoubleBufferedTreeView : TreeView
    {
        public DoubleBufferedTreeView()
        {
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer |
                           ControlStyles.ResizeRedraw |
                           ControlStyles.AllPaintingInWmPaint, true);
            this.UpdateStyles();
        }
    }
}

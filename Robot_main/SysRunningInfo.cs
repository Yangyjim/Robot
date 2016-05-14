using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Robot_main
{
    public partial class SysRunningInfo : Form
    {
        public SysRunningInfo()
        {
            InitializeComponent();
        }
        public void AddInfo(ref System.Windows.Forms.ListViewItem vItem)
        {
            listView_runninginfo.Items.Add(vItem);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Visible = false;
        }

        private void button_clear_Click(object sender, EventArgs e)
        {
            listView_runninginfo.Clear();
        }

        private void button_save_Click(object sender, EventArgs e)
        {
            //Hold on
        }
    }
}

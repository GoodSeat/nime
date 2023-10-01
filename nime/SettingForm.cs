using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GoodSeat.Nime
{
    public partial class SettingForm : Form
    {
        public SettingForm()
        {
            InitializeComponent();

            _treeViewContents.ExpandAll();
        }

        private void _treeViewContents_BeforeCollapse(object sender, TreeViewCancelEventArgs e)
        {
            e.Cancel = true;
        }

        private void _treeViewContents_AfterSelect(object sender, TreeViewEventArgs e)
        {

        }
    }
}

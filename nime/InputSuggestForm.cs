using GoodSeat.Nime.Conversion;
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
    public partial class InputSuggestForm : Form
    {
        public InputSuggestForm()
        {
            InitializeComponent();
            _lastUpdate = DateTime.Now;
            Opacity = 0.0;
        }

        DateTime _lastUpdate;

        internal void Clear()
        {
            _lastUpdate = DateTime.Now;
            treeView1.Nodes.Clear();
            Opacity = 0.0;
        }

        internal void UpdateSuggestion(HiraganaSequenceTree? tree, DateTime dateTime, Point location)
        {
            if (dateTime < _lastUpdate) return;
            _lastUpdate = dateTime;

            treeView1.Nodes.Clear();

            if (tree != null)
            {
                foreach (var (hiraganaSet, t) in tree.Tree)
                {
                    treeView1.Nodes.Add(MakeTreeNode(hiraganaSet, t));
                }
            }

            Location = location;
            if (treeView1.Nodes.Count != 0) Opacity = 0.8;
            else Opacity = 0.0;
        }

        TreeNode MakeTreeNode(HiraganaSet h, HiraganaSequenceTree tree)
        {
            TreeNode node = new TreeNode(h.Phrase);
            foreach (var (hiraganaSet, t) in tree.Tree)
            {
                node.Nodes.Add(MakeTreeNode(hiraganaSet, t));
            }
            node.ExpandAll();
            return node;
        }

        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                cp.ExStyle |= 0x80; return cp;
            }
        }

        private void InputSuggestForm_Shown(object sender, EventArgs e)
        {
            TopMost = true;
        }
    }
}

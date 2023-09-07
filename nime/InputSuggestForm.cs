using GoodSeat.Nime.Conversion;
using GoodSeat.Nime.Device;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GoodSeat.Nime
{
    public partial class InputSuggestForm : Form
    {
        internal InputSuggestForm(InputSuggestion inputSuggestion)
        {
            InitializeComponent();

            InputSuggestion = inputSuggestion;
            _lastUpdate = DateTime.Now;
            Opacity = 0.0;

            _keyboardWatcher = new KeyboardWatcher();
            _keyboardWatcher.KeyDown += _keyboardWatcher_KeyDown;
            _keyboardWatcher.KeyUp += _keyboardWatcher_KeyUp;
        }

        private async void _keyboardWatcher_KeyUp(object? sender, KeyboardWatcher.KeybordWatcherEventArgs e)
        {
            //e.Cancel = true;

            if (e.Key == VirtualKeys.Enter)
            {
                Exit(DialogResult.OK);
                return;
            }
            else if (e.Key == VirtualKeys.Esc)
            {
                Exit(DialogResult.Cancel);
                return;
            }
            else if (e.Key == VirtualKeys.BackSpace && !ConfirmedInput.Any())
            {
                Exit(DialogResult.Cancel);
                return;
            }
            else if (e.Key == VirtualKeys.BackSpace && ConfirmedInput.Any())
            {
                ConfirmedInput.RemoveAt(ConfirmedInput.Count - 1);
                if (ConfirmedInput.Any())
                {
                    TargetTree = await InputSuggestion.SearchPostOfAsync(ConfirmedInput.Last(), 2);
                }
                else
                {
                    TargetTree = _initialTargetTree;
                }
            }
            else
            {
                int i = -1;
                if (e.Key == VirtualKeys.A) i = 0;
                else if (e.Key == VirtualKeys.B && TargetTree.Tree.Count > 1) i = 1;
                else if (e.Key == VirtualKeys.C && TargetTree.Tree.Count > 2) i = 2;
                else if (e.Key == VirtualKeys.D && TargetTree.Tree.Count > 3) i = 3;
                else if (e.Key == VirtualKeys.E && TargetTree.Tree.Count > 4) i = 4;
                else if (e.Key == VirtualKeys.F && TargetTree.Tree.Count > 5) i = 5;
                else if (e.Key == VirtualKeys.G && TargetTree.Tree.Count > 6) i = 6;

                if (i >= 0) SelectIndexOf(i);
            }

            Refresh();
        }

        void SelectIndexOf(int i)
        {
            var h = TargetTree.Tree[i].Item1;
            ConfirmedInput.Add(h);
            TargetTree = InputSuggestion.SearchPostOfAsync(h, 2).Result;

            if (TargetTree.Tree.Count == 0)
            {
                Exit(DialogResult.OK);
                return;
            }
        }

        private void Exit(DialogResult result)
        {
            _keyboardWatcher.Enable = false;
            if (result == DialogResult.OK)
            {
                Opacity = 0;
            }
            else
            {
                TargetTree = _initialTargetTree;
            }

            Refresh();

            SuggestExit(this, result);
        }

        private void _keyboardWatcher_KeyDown(object? sender, KeyboardWatcher.KeybordWatcherEventArgs e)
        {
            e.Cancel = true;
        }

        KeyboardWatcher _keyboardWatcher;
        DateTime _lastUpdate;
        InputSuggestion InputSuggestion { get; set; }
        HiraganaSequenceTree? TargetTree { get; set; }
        HiraganaSequenceTree? _initialTargetTree;

        /// <summary>
        /// 入力補完ウインドウが閉じるときに呼び出されます。
        /// </summary>
        public event EventHandler<DialogResult> SuggestExit;

        internal List<HiraganaSet> ConfirmedInput { get; private set; } = new List<HiraganaSet>();


        internal bool StartSuggestion()
        {
            if (Opacity == 0.0 || TargetTree == null || TargetTree.Tree.Count == 0) return false;

            _initialTargetTree = TargetTree;

            ConfirmedInput = new List<HiraganaSet>();
            _keyboardWatcher.Enable = true;
            Opacity = 0.95;
            Refresh();

            if (TargetTree.Tree.Count == 1) SelectIndexOf(0); // MEMO:ここですぐに終了する可能性がある

            return _keyboardWatcher.Enable;
        }

        internal void Clear()
        {
            _lastUpdate = DateTime.Now;
            treeView1.Nodes.Clear();
            Opacity = 0.0;
        }

        internal void UpdateSuggestion(HiraganaSequenceTree? tree, DateTime dateTime, Point? location)
        {
            if (dateTime < _lastUpdate) return;
            TargetTree = tree;
            _lastUpdate = dateTime;

            treeView1.Nodes.Clear();

            if (tree != null)
            {
                foreach (var (hiraganaSet, t) in tree.Tree)
                {
                    treeView1.Nodes.Add(MakeTreeNode(hiraganaSet, t));
                }
            }

            if (location.HasValue) Location = location.Value;

            if (treeView1.Nodes.Count != 0) Opacity = _keyboardWatcher.Enable ? 0.97 : 0.9;
            else Opacity = 0.0;

            Refresh();
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

        private void InputSuggestForm_Shown(object sender, EventArgs e)
        {
            TopMost = true;
        }

        private void InputSuggestForm_Paint(object sender, PaintEventArgs e)
        {
            if (Opacity == 0.0) return;
            if (TargetTree == null) return;

            Opacity = _keyboardWatcher.Enable ? 0.97 : 0.9;

            var g = e.Graphics;

            g.SmoothingMode = SmoothingMode.AntiAlias;

            int nMax = 7;
            FontFamily f = SystemFonts.DefaultFont.FontFamily;

            var keys = ConvertCandidate.GetKeys(nMax).ToList();

            var brushD = new SolidBrush(Color.Black);
            var brushK = new SolidBrush(Color.DarkRed);
            var brushP = new SolidBrush(_keyboardWatcher.Enable ? Color.Gray : Color.DarkGray);

            int i = 0;
            float x = 2f, y = 2f;
            float mx = 0f;

            GraphicsPath pathHelp = new GraphicsPath();
            var msg = _keyboardWatcher.Enable ? "【BS:戻る  Esc:キャンセル  Enter:確定】" : "【Shift+Ctrlで補完を開始】";
            pathHelp.AddString(msg, f, 0, 9f, new Point(3, (int)y + 1), null);
            g.FillPath(new SolidBrush(Color.DimGray), pathHelp);
            y += 13;
            mx = Math.Max(mx, pathHelp.GetBounds().Right);

            if (_keyboardWatcher.Enable)
            {
                GraphicsPath pathConfirmed = new GraphicsPath();
                foreach (var h in ConfirmedInput)
                {
                    pathConfirmed.AddString(h.Phrase, f, 0, 15f, new PointF(x + 2, y + 2), null);
                    x = pathConfirmed.GetBounds().Right + 3f;
                }
                g.FillPath(brushD, pathConfirmed);
            }

            GraphicsPath pathKey = new GraphicsPath();
            GraphicsPath path = new GraphicsPath();

            foreach (var (h, children) in TargetTree.Tree)
            {
                var k = keys[i++];
                pathKey.AddString(k, f, 0, 12f, new PointF(x, y - 2), null);

                var txt = h.Phrase;
                if (children.Tree.Any()) txt += " ...";

                path.AddString(txt, f, 0, 15f, new PointF(x + 2f, y + 2), null);

                y += 20f;
                mx = Math.Max(mx, path.GetBounds().Right);
            }

            g.FillPath(brushP, path);
            if (_keyboardWatcher.Enable) g.FillPath(brushK, pathKey);

            Size = new Size((int)mx + 5, (int)y + 5);
        }

        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                cp.ExStyle |= 0x80; return cp;
            }
        }
    }
}

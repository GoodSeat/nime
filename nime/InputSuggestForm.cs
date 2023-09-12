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

                _stackTargetTree.RemoveAt(_stackTargetTree.Count - 1);
                TargetTree = _stackTargetTree.Last();
            }
            else
            {
                int i = -1;
                if (e.Key == VirtualKeys.A) i = 0;
                else if (e.Key == VirtualKeys.B && TargetTree.Children.Count > 1) i = 1;
                else if (e.Key == VirtualKeys.C && TargetTree.Children.Count > 2) i = 2;
                else if (e.Key == VirtualKeys.D && TargetTree.Children.Count > 3) i = 3;
                else if (e.Key == VirtualKeys.E && TargetTree.Children.Count > 4) i = 4;
                else if (e.Key == VirtualKeys.F && TargetTree.Children.Count > 5) i = 5;
                else if (e.Key == VirtualKeys.G && TargetTree.Children.Count > 6) i = 6;
                else if (e.Key == VirtualKeys.H && TargetTree.Children.Count > 7) i = 7;
                else if (e.Key == VirtualKeys.I && TargetTree.Children.Count > 8) i = 8;
                else if (e.Key == VirtualKeys.J && TargetTree.Children.Count > 9) i = 9;
                else if (e.Key == VirtualKeys.K && TargetTree.Children.Count > 10) i = 10;

                if (i >= 0) SelectIndexOf(i);
            }

            Refresh();
        }

        void SelectIndexOf(int i)
        {
            //var (h, f, hs) = TargetTree.Children[i];
            var tree = TargetTree.Children[i];

            ConfirmedInput.Add(tree.Word);

            var h_ = tree.Word;
            if (tree.ConsistPhrases != null && tree.ConsistPhrases.Any()) h_ = tree.ConsistPhrases.Last();

            TargetTree = InputSuggestion.SearchPostOfAsync(h_, 2).Result;
            _stackTargetTree.Add(TargetTree);

            if (ConfirmedInput.Count == 1)
            {
                if (tree.ConsistPhrases != null) HeadHiraganaSetForRegister = tree.ConsistPhrases;
                else
                {
                    HeadHiraganaSetForRegister.Clear();
                    HeadHiraganaSetForRegister.Add(h_);
                }
            }

            if (TargetTree == null || TargetTree.Children.Count == 0)
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

                ConfirmedPhraseList.Clear();
                HeadHiraganaSetForRegister.ForEach(ConfirmedPhraseList.Add);
                ConfirmedInput.RemoveAt(0);
                ConfirmedInput.ForEach(ConfirmedPhraseList.Add);
            }
            else
            {
                TargetTree = _stackTargetTree[0];
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

        List<HiraganaSequenceTree?> _stackTargetTree = new List<HiraganaSequenceTree?>();

        /// <summary>
        /// 入力補完ウインドウが閉じるときに呼び出されます。
        /// </summary>
        public event EventHandler<DialogResult> SuggestExit;

        List<HiraganaSet> ConfirmedInput { get; set; } = new List<HiraganaSet>();
        List<HiraganaSet> HeadHiraganaSetForRegister { get; set; } = new List<HiraganaSet>();

        internal List<HiraganaSet> ConfirmedPhraseList { get; private set; } = new List<HiraganaSet>();


        internal bool StartSuggestion()
        {
            if (Opacity == 0.0 || TargetTree == null || TargetTree.Children.Count == 0) return false;

            _stackTargetTree.Clear();
            _stackTargetTree.Add(TargetTree);

            ConfirmedInput.Clear();
            HeadHiraganaSetForRegister.Clear();
            _keyboardWatcher.Enable = true;
            Opacity = 0.95;

            if (TargetTree.Children.Count == 1) SelectIndexOf(0); // MEMO:ここですぐに終了する可能性がある
            Refresh();

            return _keyboardWatcher.Enable;
        }

        internal void Clear()
        {
            _lastUpdate = DateTime.Now;
            Opacity = 0.0;
        }

        internal void UpdateSuggestion(HiraganaSequenceTree? tree, DateTime dateTime, Point? location)
        {
            if (dateTime < _lastUpdate) return;
            TargetTree = tree;
            _lastUpdate = dateTime;

            if (location.HasValue) Location = location.Value;

            if (TargetTree != null && TargetTree.Children.Any()) Opacity = _keyboardWatcher.Enable ? 0.97 : 0.9;
            else Opacity = 0.0;

            Refresh();
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

            int nMax = 10;
            FontFamily f = SystemFonts.DefaultFont.FontFamily;

            var keys = ConvertCandidate.GetKeys(nMax).ToList();

            var brushD = new SolidBrush(Color.Black);
            var brushK = new SolidBrush(Color.DarkRed);
            var brushP = new SolidBrush(_keyboardWatcher.Enable ? Color.Gray : Color.DarkGray);

            float x = 2f, y = 2f;
            float mx = 0f;
            float my = 0f;

            GraphicsPath pathHelp = new GraphicsPath();
            var msg = _keyboardWatcher.Enable ? "【BS:戻る  Esc:キャンセル  Enter:確定】" : "【Tabで補完を開始】";
            pathHelp.AddString(msg, f, 0, 9f, new Point(3, (int)y + 1), null);
            g.FillPath(new SolidBrush(Color.DimGray), pathHelp);
            y += 13;

            GraphicsPath pathKey = new GraphicsPath();
            GraphicsPath path = new GraphicsPath();
            GraphicsPath pathConfirmed = new GraphicsPath();

            Action<HiraganaSequenceTree, HiraganaSet?> drawTree = (tree, hConfirmed) =>
            {
                int i = 0;
                float ly = y;
                foreach (var child in tree.Children)
                {
                    var k = keys[i++];
                    if (hConfirmed == null)
                    {
                        pathKey.AddString(k, f, 0, 12f, new PointF(x - 2, ly - 2), null);
                    }

                    var txt = child.Word.Phrase;
                    if (child.Word != hConfirmed && child.Children.Any()) txt += " ...";

                    path.AddString(txt, f, 0, 15f, new PointF(x + 2f, ly + 2), null);

                    if (child.Word == hConfirmed)
                    {
                        pathConfirmed.AddString(child.Word.Phrase, f, 0, 15f, new PointF(x + 2, ly + 2), null);
                    }

                    ly += 20f;
                    mx = Math.Max(mx, path.GetBounds().Right);

                    if (i >= nMax) break;
                }
                x = mx + 3f;
                my = Math.Max(my, ly);
            };

            if (_keyboardWatcher.Enable)
            {
                int j = 0;
                foreach (var h in ConfirmedInput)
                {
                    drawTree(_stackTargetTree[j++], h);
                }
                g.FillPath(brushD, pathConfirmed);
            }

            drawTree(TargetTree, null);

            g.FillPath(brushP, path);
            if (_keyboardWatcher.Enable) g.FillPath(brushK, pathKey);

            mx = Math.Max(mx, pathHelp.GetBounds().Right);
            Size = new Size((int)mx + 5, (int)my + 5);
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

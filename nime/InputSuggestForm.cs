using GoodSeat.Nime.Conversion;
using GoodSeat.Nime.Core;
using GoodSeat.Nime.Device;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GoodSeat.Nime
{
    /// <summary>
    /// 入力補完ウインドウを表します。
    /// </summary>
    public partial class InputSuggestForm : Form
    {
        /// <summary>
        /// 入力補完ウインドウを初期化します。
        /// </summary>
        /// <param name="inputSuggestion">入力補完情報。</param>
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

        KeyboardWatcher.KeybordWatcherEventArgs? _endKey;
        bool _ctrlPressed = false;

        KeyboardWatcher _keyboardWatcher;
        DateTime _lastUpdate;
        InputSuggestion InputSuggestion { get; set; }
        HiraganaSequenceTree? TargetTree { get; set; }

        List<HiraganaSequenceTree?> _stackTargetTree = new List<HiraganaSequenceTree?>();

        List<HiraganaSet> ConfirmedInput { get; set; } = new List<HiraganaSet>();
        List<HiraganaSet> HeadHiraganaSetForRegister { get; set; } = new List<HiraganaSet>();

        /// <summary>
        /// 最新の入力補完画面で確定されたフレーズリストを取得します。
        /// </summary>
        internal List<HiraganaSet> ConfirmedPhraseList { get; private set; } = new List<HiraganaSet>();


        /// <summary>
        /// 入力補完ウインドウが閉じるときに呼び出されます。
        /// </summary>
        public event EventHandler<DialogResult> SuggestExit;


        /// <summary>
        /// キーダウン時の動作を表します。
        /// </summary>
        /// <param name="sender">イベント発火元。</param>
        /// <param name="e">イベント情報。</param>
        private void _keyboardWatcher_KeyDown(object? sender, KeyboardWatcher.KeybordWatcherEventArgs e)
        {
            e.Cancel = true;

            if (e.Key == VirtualKeys.Ctrl || e.Key == VirtualKeys.ControlLeft || e.Key == VirtualKeys.ControlRight)
            {
                _ctrlPressed = true;
                return;
            }

            //if ((e.Key >= VirtualKeys.A && e.Key <= VirtualKeys.Z && !Utility.IsLockedCtrlKey()) || e.Key == VirtualKeys.ShiftLeft || e.Key == VirtualKeys.ShiftRight) // MEMO:IsLockedCtrlKeyがうまく考慮されない
            if ((e.Key >= VirtualKeys.A && e.Key <= VirtualKeys.Z && !_ctrlPressed) || e.Key == VirtualKeys.ShiftLeft || e.Key == VirtualKeys.ShiftRight)
            {
                Debug.WriteLine("_keyboardWatcher_KeyDown");
                _endKey = e;
                Exit(DialogResult.OK);
            }
        }

        /// <summary>
        /// キーアップ時の動作を表します。
        /// </summary>
        /// <param name="sender">イベント発火元。</param>
        /// <param name="e">イベント情報。</param>
        private void _keyboardWatcher_KeyUp(object? sender, KeyboardWatcher.KeybordWatcherEventArgs e)
        {
            //e.Cancel = true;
            // TODO:Ctrl + Xで選択中の履歴を削除

            if (e.Key == VirtualKeys.Ctrl || e.Key == VirtualKeys.ControlLeft || e.Key == VirtualKeys.ControlRight)
            {
                _ctrlPressed = false;
                return;
            }

            if (e.Key == VirtualKeys.Enter)
            {
                Exit(DialogResult.OK);
                return;
            }
            else if (e.Key == VirtualKeys.Esc || e.Key == VirtualKeys.ControlLeft || e.Key == VirtualKeys.ControlRight)
            {
                Exit(DialogResult.Cancel);
                return;
            }
            else if ((e.Key == VirtualKeys.BackSpace || e.Key == VirtualKeys.Left) && !ConfirmedInput.Any())
            {
                Exit(DialogResult.Cancel);
                return;
            }
            else if ((e.Key == VirtualKeys.BackSpace || e.Key == VirtualKeys.Left) && ConfirmedInput.Any())
            {
                ConfirmedInput.RemoveAt(ConfirmedInput.Count - 1);
                if (ConfirmedInput.Count == 0)
                {
                    Exit(DialogResult.Cancel);
                    return;
                }

                _stackTargetTree.RemoveAt(_stackTargetTree.Count - 1);
                TargetTree = _stackTargetTree.Last();
            }
            else if (e.Key == VirtualKeys.Down)
            {
                SelectNext();
            }
            else if (e.Key == VirtualKeys.Up)
            {
                SelectNext(-1);
            }
            else if (e.Key == VirtualKeys.Right)
            {
                SelectIndexOf(0);
            }
            else
            {
                if (!_ctrlPressed)
                {
                    _endKey = e;
                    Exit(DialogResult.OK);
                    return;
                }
                else if (TargetTree != null)
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
            }

            Refresh();
        }

        /// <summary>
        /// 現在の選択列において、選択されている単語を変更します。
        /// </summary>
        /// <param name="delta">現在の選択単語からの変化インデックス。</param>
        void SelectNext(int delta = 1)
        {
            int i = 0;
            int inext = 0;

            var treeCurrent = _stackTargetTree[_stackTargetTree.Count - 2];
            foreach (var tc in treeCurrent.Children)
            {
                if (tc.Word == ConfirmedInput.Last())
                {
                    inext = i + delta;
                    if (inext >= treeCurrent.Children.Count) inext = 0;
                    else if (inext < 0) inext = treeCurrent.Children.Count - 1;
                    break;
                }
                ++i;
            }
            if (inext > 9) inext = delta > 0 ? 0 : 9;

            var tree = treeCurrent.Children[inext];

            ConfirmedInput.RemoveAt(ConfirmedInput.Count - 1);
            ConfirmedInput.Add(tree.Word);

            var h_ = tree.Word;
            if (tree.ConsistPhrases != null && tree.ConsistPhrases.Any()) h_ = tree.ConsistPhrases.Last();

            TargetTree = InputSuggestion.SearchPostOfAsync(h_, 2).Result;
            _stackTargetTree.RemoveAt(_stackTargetTree.Count - 1);
            _stackTargetTree.Add(TargetTree);

            if (ConfirmedInput.Count == 1)
            {
                if (tree.ConsistPhrases != null) HeadHiraganaSetForRegister = new List<HiraganaSet>(tree.ConsistPhrases);
                else
                {
                    HeadHiraganaSetForRegister.Clear();
                    HeadHiraganaSetForRegister.Add(h_);
                }
            }
        }

        /// <summary>
        /// 次の列の指定インデックスの単語を選択します。
        /// </summary>
        /// <param name="i">選択インデックス。</param>
        void SelectIndexOf(int i)
        {
            //var (h, f, hs) = TargetTree.Children[i];
            if (TargetTree == null || !TargetTree.Children.Any()) return;
            var tree = TargetTree.Children[i];

            ConfirmedInput.Add(tree.Word);

            var h_ = tree.Word;
            if (tree.ConsistPhrases != null && tree.ConsistPhrases.Any()) h_ = tree.ConsistPhrases.Last();

            TargetTree = InputSuggestion.SearchPostOfAsync(h_, 2).Result;
            _stackTargetTree.Add(TargetTree);

            if (ConfirmedInput.Count == 1)
            {
                if (tree.ConsistPhrases != null) HeadHiraganaSetForRegister = new List<HiraganaSet>(tree.ConsistPhrases);
                else
                {
                    HeadHiraganaSetForRegister.Clear();
                    HeadHiraganaSetForRegister.Add(h_);
                }
            }
        }

        /// <summary>
        /// 入力補完の選択モードを終了します。
        /// </summary>
        /// <param name="result">設定する<see cref="DialogResult"/>。</param>
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

            using (DelayKeyInput delayKeyInput = new DelayKeyInput())
            {
                if (_endKey != null)
                {
                    DeviceOperator deviceOperator = new DeviceOperator();
                    deviceOperator.EnableWatchKeyboardOrMouse = true;
                    deviceOperator.KeyDown(_endKey.Key);
                }
                SuggestExit(this, result);
            }
            _endKey = null;
        }

        /// <summary>
        /// 入力補完の選択モードを開始します。
        /// </summary>
        /// <returns>補完ウインドウが起動されたか。</returns>
        internal bool StartSuggestion()
        {
            if (Opacity == 0.0 || TargetTree == null || TargetTree.Children.Count == 0) return false;

            _stackTargetTree.Clear();
            _stackTargetTree.Add(TargetTree);

            ConfirmedInput.Clear();
            HeadHiraganaSetForRegister.Clear();
            _keyboardWatcher.Enable = true;
            _ctrlPressed = false;
            Opacity = 0.95;

            //if (TargetTree.Children.Count == 1) SelectIndexOf(0); // MEMO:ここですぐに終了する可能性がある
            SelectIndexOf(0);
            Refresh();
            TopMost = true;

            return _keyboardWatcher.Enable;
        }

        /// <summary>
        /// 入力補完ウインドウを非表示にします。
        /// </summary>
        internal void Clear()
        {
            _lastUpdate = DateTime.Now;
            Opacity = 0.0;
        }

        /// <summary>
        /// 入力補完ウインドウの表示状態を更新します。
        /// </summary>
        /// <param name="tree">表示対象とする入力補完情報。</param>
        /// <param name="dateTime">この補完ウインドウ表示の起因となるイベントの発生時刻。</param>
        /// <param name="location">補完ウインドウの表示位置。</param>
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

        /// <summary>
        /// 入力補完ウインドウが表示された時の処理を表します。
        /// </summary>
        /// <param name="sender">イベントの発火元。</param>
        /// <param name="e">イベント情報。</param>
        private void InputSuggestForm_Shown(object sender, EventArgs e)
        {
            TopMost = true;
        }

        /// <summary>
        /// 入力補完ウインドウの描画イベントを処理します。
        /// </summary>
        /// <param name="sender">イベントの発火元。</param>
        /// <param name="e">イベント情報。</param>
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

            var brushD = new SolidBrush(Color.White);
            var brushD2= new SolidBrush(Color.DarkBlue);
            var brushK = new SolidBrush(Color.DarkRed);
            var brushP = new SolidBrush(_keyboardWatcher.Enable ? Color.Gray : Color.DarkGray);

            float x = 2f, y = 2f;
            float mx = 0f;
            float my = 0f;

            GraphicsPath pathHelp = new GraphicsPath();
            var msg = _keyboardWatcher.Enable ? "【BS:戻る  Ctrl+A~:選択  Esc:キャンセル  Enter:確定】" : "【↓で補完を開始】";
            pathHelp.AddString(msg, f, 0, 9f, new Point(3, (int)y + 1), null);
            g.FillPath(new SolidBrush(Color.DimGray), pathHelp);
            y += 13;

            GraphicsPath pathKey = new GraphicsPath();
            GraphicsPath path = new GraphicsPath();
            GraphicsPath pathConfirmed = new GraphicsPath();
            GraphicsPath pathConfirmed2 = new GraphicsPath();

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

                        GraphicsPath pathDummy = new GraphicsPath();
                        pathDummy.AddString(child.Word.Phrase, f, 0, 15f, new PointF(x + 2, ly + 2), null);
                        var rect = pathDummy.GetBounds();
                        rect.Offset(-2f, -2f);
                        rect.Width  = rect.Width + 4f;
                        rect.Height = rect.Height + 4f;
                        pathConfirmed2.AddRectangle(rect);
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
            }

            drawTree(TargetTree, null);

            if (!_keyboardWatcher.Enable)
            {
                if (TargetTree.Children.Count > 0) {
                    drawTree(TargetTree.Children[0], null);
                }
            }

            g.FillPath(brushP, path);
            if (_keyboardWatcher.Enable)
            {
                g.FillPath(brushK, pathKey);
                g.FillPath(brushD2, pathConfirmed2);
                g.FillPath(brushD, pathConfirmed);
            }

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

using GoodSeat.Nime.Conversion;
using GoodSeat.Nime.Device;
using nime;
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
    /// 変換結果の編集ウインドウを表します。
    /// </summary>
    public partial class ConvertDetailForm : Form
    {
        /// <summary>
        /// 変換ウインドウ上の動作区分を表します。
        /// </summary>
        enum Mode
        {
            SelectKey,
            EditSplit,
            EditPhrase,
            DeleteInputHisotry,
        }

        string HelpText { get; set; } = "s/S:区切編集  x:候補削除  y:コピー  A～:IME編集  Enter:OK  Esc:キャンセル";
        string HelpTextDeleteInputHistory { get; set; } = "【文節候補削除】 Esc:キャンセル (※辞書に登録がある場合、辞書からも削除されます)";
        string HelpTextEditSplit { get; set; } = "【文節区切りの編集】 Enter:OK  Esc:キャンセル";
        string HelpTextEditPhrase { get; set; } = "【IMEによる直接編集】 Enter:OK  Esc:キャンセル";

        Mode CurrentMode { get; set; } = Mode.SelectKey;

        /// <summary>
        /// 変換結果の編集ウインドウを表します。
        /// </summary>
        /// <param name="inputHistory">入力履歴情報。</param>
        /// <param name="convertToSentence">日本語文章への変換方法。</param>
        internal ConvertDetailForm(InputHistory inputHistory, ConvertToSentence convertToSentence)
        {
            InitializeComponent();

            InputHistory = inputHistory;
            ConvertToSentence = convertToSentence;

            _keyboardWatcher = new KeyboardWatcher();
            _keyboardWatcher.KeyDown += KeyboardWatcher_KeyDown;
            _keyboardWatcher.KeyUp += KeyboardWatcher_KeyUp;

            Opacity = 0.0;

            DirectInputWithIMEForm.EditEnded += DirectInputWithIMEForm_EditEnded;

            // TODO!:マウス操作でも直ちにOKで閉じるべき
        }

        /// <summary>
        /// 変換ウインドウが閉じるときに呼び出されます。
        /// </summary>
        public event EventHandler<DialogResult> ConvertExit;

        InputHistory InputHistory { get; set; }

        ConvertToSentence ConvertToSentence { get; set; }

        KeyboardWatcher _keyboardWatcher;


        bool _rapidOnSingle = true;

        string _hitKey = "";


        private void SelectPhraseWithKey()
        {
            if (string.IsNullOrEmpty(_hitKey))
            {
                Refresh();
                return;
            }

            foreach (var phrase in TargetSentence.PhraseList)
            {
                foreach (var c in phrase.Candidates)
                {
                    if (c.Key.Length == 1 && _hitKey.Length > 1) _hitKey = _hitKey.Substring(1);
                    else if (c.Key.Length == 2 && _hitKey.Length > 2) _hitKey = _hitKey.Substring(1);

                    if (c.Key == _hitKey)
                    {
                        _hitKey = "";
                        phrase.Selected = c.Phrase;
                        if (TargetSentence.PhraseList.Count == 1 && _rapidOnSingle)
                        {
                            Exit(DialogResult.OK);
                        }
                        else
                        {
                            Refresh();
                        }
                        return;
                    }
                }
            }
        }
        private void DeletePhraseWithKey()
        {
            if (string.IsNullOrEmpty(_hitKey))
            {
                Refresh();
                return;
            }

            ConvertCandidatePhrase targetPhrase = null;
            CandidatePhrase? deleteCandidate = null;
            foreach (var phrase in TargetSentence.PhraseList)
            {
                foreach (var c in phrase.Candidates)
                {
                    if      (c.Key.Length == 1 && _hitKey.Length > 1) _hitKey = _hitKey.Substring(1);
                    else if (c.Key.Length == 2 && _hitKey.Length > 2) _hitKey = _hitKey.Substring(1);

                    if (c.Key == _hitKey)
                    {
                        _hitKey = "";
                        deleteCandidate = c;
                        targetPhrase = phrase;
                        break;
                    }
                }
                if (deleteCandidate != null) break;
            }

            if (deleteCandidate != null)
            {
                targetPhrase.Candidates.Remove(deleteCandidate);
                InputHistory.Unregister(targetPhrase.OriginalHiragana, deleteCandidate.Phrase);

                CurrentMode = Mode.SelectKey;
                Refresh();
            }
        }

        private void EditSplit()
        {
            if (string.IsNullOrEmpty(_hitKey))
            {
                Refresh();
                return;
            }

            var n = SplitEditSentence.Where(c => c != ',').Count();
            var keys = ConvertCandidate.GetKeys(n).Take(n).ToList();
            for (int i = 0; i < keys.Count(); ++i)
            {
                var key = keys[i];
                if (key.Length == 1 && _hitKey.Length > 1) _hitKey = _hitKey.Substring(1);
                else if (key.Length == 2 && _hitKey.Length > 2) _hitKey = _hitKey.Substring(1);

                if (key == _hitKey)
                {
                    string result = "";

                    int index = 0;
                    for (int j = 0; j < n; ++j)
                    {
                        var c = SplitEditSentence[index++];
                        if (c == ',')
                        {
                            if (i + 1 != j) result += ",";

                            c = SplitEditSentence[index++];
                        }
                        else if (i + 1 == j) result += ",";

                        result += c;
                    }

                    SplitEditSentence = result;
                    _hitKey = "";

                    Refresh();
                    return;
                }
            }
        }

        private void KeyboardWatcher_KeyDown(object? sender, KeyboardWatcher.KeybordWatcherEventArgs e)
        {
            if (e.Key == VirtualKeys.ShiftLeft || e.Key == VirtualKeys.ShiftRight) return;

            e.Cancel = true;

            _startEditIMEWhenNextUp = false;

            // アルファベット(キーの選択)
            if (e.Key >= VirtualKeys.A && e.Key <= VirtualKeys.Z)
            {
                if (_keyboardWatcher.IsKeyLocked(Keys.LShiftKey) || _keyboardWatcher.IsKeyLocked(Keys.RShiftKey))
                {
                    // IME直接編集
                    _startEditIMEWhenNextUp = true;
                }
            }
        }

        bool _startEditIMEWhenNextUp = false;

        private void KeyboardWatcher_KeyUp(object? sender, KeyboardWatcher.KeybordWatcherEventArgs e)
        {
            if (e.Key == VirtualKeys.ShiftLeft || e.Key == VirtualKeys.ShiftRight) return;

            e.Cancel = true;
            if (CurrentMode == Mode.SelectKey)
            {
                if (e.Key == VirtualKeys.Esc)
                {
                    Exit(DialogResult.Cancel);
                }
                else if (e.Key == VirtualKeys.Enter)
                {
                    Exit(DialogResult.OK);
                }
                else if (e.Key == VirtualKeys.BackSpace && !string.IsNullOrEmpty(_hitKey))
                {
                    _hitKey = _hitKey.Substring(0, _hitKey.Length - 1);
                    SelectPhraseWithKey();
                }
                else if (e.Key == VirtualKeys.Y)
                {
                    Clipboard.SetText(TargetSentence.GetSelectedSentence());
                }
                else if (e.Key == VirtualKeys.X)
                {
                    CurrentMode = Mode.DeleteInputHisotry;
                    _hitKey = "";
                    Refresh();
                }
                else if (e.Key == VirtualKeys.S)
                {
                    CurrentMode = Mode.EditSplit;
                    SplitEditSentence = TargetSentence.MakeSentenceForHttpRequest();
                    if (_keyboardWatcher.IsKeyLocked(Keys.LShiftKey) || _keyboardWatcher.IsKeyLocked(Keys.RShiftKey))
                    {
                        SplitEditSentence = SplitEditSentence.Replace(",", "");
                    }
                    _hitKey = "";
                    Refresh();
                }
                // アルファベット(キーの選択)
                else if (e.Key >= VirtualKeys.A && e.Key <= VirtualKeys.Z)
                {
                    if (_startEditIMEWhenNextUp)
                    {
                        // IME直接編集
                        int n = (int)e.Key.ToString()[0] - (int)'A';
                        _hitKey = "";
                        if (_phrasePositions.Count > n)
                        {
                            CurrentMode = Mode.EditPhrase;
                            Refresh();
                            _keyboardWatcher.Enable = false;
                            var (rect, color) = _phrasePositions[n];
                            rect.Offset(Location);
                            DirectInputWithIMEForm.StartEdit(TargetSentence.PhraseList[n], rect, color);
                        }
                    }
                    else
                    {
                        _hitKey += e.Key.ToString().ToLower();
                        SelectPhraseWithKey();
                    }
                }
                // TODO:数字でIMEを利用した文節の直接編集
                else if ((e.Key >= VirtualKeys.D0 && e.Key <= VirtualKeys.D9) ||
                         (e.Key >= VirtualKeys.N0 && e.Key <= VirtualKeys.N9))
                {

                }
                else
                {
                    Exit(DialogResult.OK);
                }
            }
            else if (CurrentMode == Mode.EditSplit)
            {
                if (e.Key == VirtualKeys.Esc)
                {
                    CurrentMode = Mode.SelectKey;
                    _hitKey = "";
                    Refresh();
                }
                else if (e.Key == VirtualKeys.Enter)
                {
                    CurrentMode = Mode.SelectKey;
                    _hitKey = "";

                    try
                    {
                        var txt = SplitEditSentence;
                        if (!txt.Contains(',')) txt += ",";

                        var ans = ConvertToSentence.ConvertFromHiragana(txt, InputHistory, null, 200);
                        if (ans != null)
                        {
                            TargetSentence.ModifyConsideration(ans);
                        }
                    }
                    catch
                    { }

                    Refresh();
                }
                // アルファベット(文節区切りの編集)
                else if (e.Key >= VirtualKeys.A && e.Key <= VirtualKeys.Z)
                {
                    _hitKey += e.Key.ToString().ToLower();
                    EditSplit();
                }
            }
            else if (CurrentMode == Mode.DeleteInputHisotry)
            {
                if (e.Key == VirtualKeys.Esc || e.Key == VirtualKeys.Enter)
                {
                    CurrentMode = Mode.SelectKey;
                    _hitKey = "";
                    Refresh();
                }
                // アルファベット(キーの選択)
                else if (e.Key >= VirtualKeys.A && e.Key <= VirtualKeys.Z)
                {
                    _hitKey += e.Key.ToString().ToLower();
                    DeletePhraseWithKey();
                }
            }
        }

        private void DirectInputWithIMEForm_EditEnded(object? sender, DialogResult e)
        {
            CurrentMode = Mode.SelectKey;
            _keyboardWatcher.Enable = true;

            if (e == DialogResult.OK)
            {
                Refresh();
            }
            else
            {
                Exit(DialogResult.OK);
            }
        }

        public void Start(ConvertCandidate sentence, Point position, ConvertCandidate selected = null)
        {
            TargetSentence = ConvertCandidate.CopyFrom(selected ?? sentence);
            Location = position;
            SentenceWhenStart = sentence.GetSelectedSentence();

            _keyboardWatcher.Enable = true;
            Opacity = 0.9;
            Refresh();
            TopMost = true;
        }
        private void Exit(DialogResult result)
        {
            if (SentenceWhenStart == TargetSentence.GetSelectedSentence()) result = DialogResult.Cancel;

            _keyboardWatcher.Enable = false;
            Opacity = 0.0;

            ConvertExit?.Invoke(this, result);
        }


        public ConvertCandidate TargetSentence { get; private set; }

        public string SentenceWhenStart { get; set; }

        string SplitEditSentence { get; set; }


        List<(Rectangle, Color)> _phrasePositions = new List<(Rectangle, Color)>();

        private void ConvertDetailForm_Paint(object sender, PaintEventArgs e)
        {
            if (Opacity == 0.0) return;

            var g = e.Graphics;

            g.SmoothingMode = SmoothingMode.AntiAlias;

            var colors = new Color[]{
                Color.Black
                //Color.DarkRed, Color.DarkGreen, Color.DarkBlue, Color.DarkOliveGreen, Color.DarkOrange,
                //Color.DarkSalmon, Color.DarkSlateBlue, Color.DarkTurquoise, Color.DarkViolet, Color.DarkMagenta,
            };

            FontFamily f = SystemFonts.DefaultFont.FontFamily;

            float x = 5f;
            float mx = 0f;
            float my = 0f;

            var helpMsg = HelpText;
            if (CurrentMode == Mode.SelectKey || CurrentMode == Mode.DeleteInputHisotry || CurrentMode == Mode.EditPhrase)
            {
                if (CurrentMode == Mode.DeleteInputHisotry) helpMsg = HelpTextDeleteInputHistory;
                else if (CurrentMode == Mode.EditPhrase) helpMsg = HelpTextEditPhrase;

                _phrasePositions.Clear();

                var brushD = new SolidBrush(CurrentMode == Mode.SelectKey ? Color.Red : Color.Blue);
                var brushC = new SolidBrush(Color.DarkRed);
                var brushS = new SolidBrush(Color.DarkGray);

                int n = 0;
                foreach (var phrase in TargetSentence.PhraseList)
                {
                    const float sy = 5f;
                    float y = sy;

                    var color = colors[n++];
                    if (n >= colors.Length) n = 0;

                    var brush = new SolidBrush(color);
                    var pen = new Pen(brush);

                    if (CurrentMode == Mode.SelectKey)
                    {
                        GraphicsPath pathD = new GraphicsPath();
                        var kd = phrase.Candidates[0].Key[0].ToString().ToUpper();
                        pathD.AddString(kd, f, 0, 14f, new PointF(x - 4f, y - 4f), null);
                        g.FillPath(brushD, pathD);
                    }

                    GraphicsPath path = new GraphicsPath();
                    path.AddString(phrase.Selected, f, 0, 18f, new PointF(x, y), null);
                    g.FillPath(brush, path);

                    y += 21f;

                    GraphicsPath pathK = new GraphicsPath();
                    GraphicsPath pathS = new GraphicsPath();
                    GraphicsPath pathC = new GraphicsPath();
                    foreach (var candidate in phrase.Candidates)
                    {
                        pathK.AddString(candidate.Key, f, 0, 10f, new PointF(x, y), null);
                        pathC.AddString(candidate.Phrase, f, 0, 14f, new PointF(x + 17f, y), null);
                        if (candidate.Phrase == phrase.Selected)
                        {
                            RectangleF rect = new RectangleF(x, y, pathC.GetBounds().Right - x, 15f);
                            pathS.AddRectangle(rect);
                        }
                        y += 15f;
                    }
                    g.FillPath(brushS, pathS);
                    if (CurrentMode != Mode.EditPhrase) g.FillPath(brushD, pathK);
                    g.FillPath(brushC, pathC);

                    var w = Math.Max(path.GetBounds().Width, pathC.GetBounds().Width + 12f) + 20f;
                    _phrasePositions.Add((new Rectangle(new Point((int)x, (int)sy), new Size((int)w, (int)y)), color));
                    x += w;

                    mx = Math.Max(pathC.GetBounds().Right, path.GetBounds().Right);
                    my = Math.Max(my, pathC.GetBounds().Bottom);
                }
            }
            else if (CurrentMode == Mode.EditSplit)
            {
                helpMsg = HelpTextEditSplit;

                float y = 5f;

                var keys = ConvertCandidate.GetKeys(SplitEditSentence.Where(c => c != ',').Count()).ToList();

                GraphicsPath path = new GraphicsPath();
                GraphicsPath pathKey = new GraphicsPath();
                GraphicsPath pathSplit = new GraphicsPath();
                int n = 0;
                int nk = 0;
                Color color = colors[n];
                foreach (var c in SplitEditSentence)
                {
                    if (c == ',')
                    {
                        g.FillPath(new SolidBrush(color), path);
                        path = new GraphicsPath();

                        ++n;
                        if (n >= colors.Length) n = 0;
                        color = colors[n];

                        pathSplit.AddLine(new PointF(x + 1f, y), new PointF(x + 1f, y + 18f));
                        pathSplit.CloseFigure();
                        //x += 6f;
                    }
                    else
                    {
                        var k = keys[nk++];

                        path.AddString(c.ToString(), f, 0, 18f, new PointF(x, y), null);
                        x += 18f;
                        pathKey.AddString(k, f, 0, 11f, new PointF(x - 2f, y + 18f), null);
                    }
                }
                if (path.PointCount > 0)
                {
                    g.FillPath(new SolidBrush(color), path);
                }
                g.DrawPath(new Pen(Color.Red, 2), pathSplit);
                g.FillPath(new SolidBrush(Color.Red), pathKey);

                mx = x;
                my = 33f;
            }

            bool showHelp = true;
            if (showHelp)
            {
                GraphicsPath pathHelp = new GraphicsPath();
                pathHelp.AddString(helpMsg, f, 0, 9f, new Point(5, (int)my + 5), null);
                g.FillPath(new SolidBrush(Color.DimGray), pathHelp);
                my += 10;
            }

            Size = new Size((int)(mx + 10f), (int)(my + 8f));

            foreach (var s in Screen.AllScreens)
            {
                if (!s.WorkingArea.Contains(Location)) continue;

                var br = Location;
                br.Offset(Size.Width, Size.Height);

                var nl = Location;
                if (s.WorkingArea.Right < br.X) nl.Offset(s.WorkingArea.Right - br.X, 0);
                if (s.WorkingArea.Bottom < br.Y) nl.Offset(0, s.WorkingArea.Bottom - br.Y);
                Location = nl;
            }
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

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
    public partial class ConvertDetailForm : Form
    {
        enum Mode
        {
            SelectKey,
            EditSplit,
            EditPhrase,
        }

        Mode CurrentMode { get; set; } = Mode.SelectKey;

        public ConvertDetailForm()
        {
            InitializeComponent();

            KeyboardWatcher.KeyDown += KeyboardWatcher_KeyDown;

            // TODO!:マウス操作でも直ちにOKで閉じるべき
        }

        bool _rapidOnSingle = true;

        string _hitKey = "";

        private void Filtering()
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
                            DialogResult = DialogResult.OK;
                            Close();
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
            if (CurrentMode == Mode.SelectKey)
            {
                if (e.Key == VirtualKeys.Esc)
                {
                    DialogResult = DialogResult.Cancel;
                    Close();
                }
                else if (e.Key == VirtualKeys.Enter)
                {
                    DialogResult = DialogResult.OK;
                    Close();
                }
                else if (e.Key == VirtualKeys.BackSpace && !string.IsNullOrEmpty(_hitKey))
                {
                    _hitKey = _hitKey.Substring(0, _hitKey.Length - 1);
                    Filtering();
                }
                else if (e.Key == VirtualKeys.S)
                {
                    CurrentMode = Mode.EditSplit;
                    SplitEditSentence = TargetSentence.MakeSentenceForHttpRequest();
                    if (KeyboardWatcher.IsKeyLocked(Keys.LShiftKey) || KeyboardWatcher.IsKeyLocked(Keys.RShiftKey))
                    {
                        SplitEditSentence = SplitEditSentence.Replace(",","");
                    }
                    _hitKey = "";
                    Refresh();
                }
                // アルファベット(キーの選択)
                else if (e.Key >= VirtualKeys.A && e.Key <= VirtualKeys.Z)
                {
                    _hitKey += e.Key.ToString().ToLower();
                    Filtering();
                }
                // TODO:数字でIMEを利用した文節の直接編集
                else if ((e.Key >= VirtualKeys.D0 && e.Key <= VirtualKeys.D9) ||
                         (e.Key >= VirtualKeys.N0 && e.Key <= VirtualKeys.N9))
                {

                }
                else if (e.Key == VirtualKeys.Shift || e.Key == VirtualKeys.ShiftLeft || e.Key == VirtualKeys.ShiftRight)
                {
                    // Shiftは何もしない
                }
                else
                {
                    DialogResult = DialogResult.OK;
                    Close();
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
                        var ans = ConvertHiraganaToSentence.Request(SplitEditSentence);
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
        }

        public void SetTarget(ConvertCandidate sentence, Point position)
        {
            TargetSentence = sentence;
            Position = position;
        }

        public ConvertCandidate TargetSentence { get; private set; }

        string SplitEditSentence { get; set; }

        Point Position { get; set; }


        private void ConvertDetailForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            KeyboardWatcher.KeyDown -= KeyboardWatcher_KeyDown;
        }

        private void ConvertDetailForm_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;

            g.SmoothingMode = SmoothingMode.AntiAlias;

            var colors = new Color[]{
                Color.DarkRed, Color.DarkGreen, Color.DarkBlue, Color.DarkOliveGreen, Color.DarkOrange,
                Color.DarkSalmon, Color.DarkSlateBlue, Color.DarkTurquoise, Color.DarkViolet, Color.DarkMagenta,
            };

            FontFamily f = SystemFonts.DefaultFont.FontFamily;

            float x = 5f;
            float mx = 0f;
            float my = 0f;

            if (CurrentMode == Mode.SelectKey)
            {
                int n = 0;
                foreach (var phrase in TargetSentence.PhraseList)
                {
                    float y = 5f;

                    var brush = new SolidBrush(colors[n++]);
                    var pen = new Pen(brush);

                    GraphicsPath path = new GraphicsPath();
                    path.AddString(phrase.Selected, f, 0, 20f, new PointF(x, y), null);
                    g.FillPath(brush, path);

                    y += 23f;

                    GraphicsPath pathK = new GraphicsPath();
                    GraphicsPath pathS = new GraphicsPath();
                    GraphicsPath pathC = new GraphicsPath();
                    foreach (var candidate in phrase.Candidates)
                    {
                        pathK.AddString(candidate.Key, f, 0, 12f, new PointF(x, y), null);
                        pathC.AddString(candidate.Phrase, f, 0, 16f, new PointF(x + 20f, y), null);
                        if (candidate.Phrase == phrase.Selected)
                        {
                            RectangleF rect = new RectangleF(x, y, pathC.GetBounds().Right - x, 16f);
                            pathS.AddRectangle(rect);
                        }
                        y += 17f;
                    }
                    g.DrawPath(pen, pathS);
                    g.FillPath(brush, pathK);
                    g.FillPath(brush, pathC);

                    x += Math.Max(path.GetBounds().Width, pathC.GetBounds().Width) + 30f;

                    if (n >= colors.Length) n = 0;

                    mx = Math.Max(pathC.GetBounds().Right, path.GetBounds().Right);
                    my = Math.Max(my, pathC.GetBounds().Bottom);
                }
            }
            else if (CurrentMode == Mode.EditSplit)
            {
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

                        pathSplit.AddLine(new PointF(x + 6f, y), new PointF(x + 6f, y + 20f));
                        pathSplit.CloseFigure();
                        x += 6f;
                    }
                    else
                    {
                        var k = keys[nk++];
                         
                        path.AddString(c.ToString(), f, 0, 20f, new PointF(x, y), null);
                        x += 16f;
                        pathKey.AddString(k, f, 0, 12f, new PointF(x - 1f, y + 20f), null);
                    }
                }
                if (path.PointCount > 0)
                {
                    g.FillPath(new SolidBrush(color), path);
                }
                g.DrawPath(new Pen(Color.Red, 2), pathSplit);
                g.FillPath(new SolidBrush(Color.Red), pathKey);

                mx = x;
                my = 35f;
            }

            Size = new Size((int)(mx + 5f), (int)(my + 5f));

            foreach (var s in Screen.AllScreens)
            {
                if (!s.WorkingArea.Contains(Location)) continue;

                var br = Location;
                br.Offset(Size.Width, Size.Height);

                var nl = Location;
                if (s.WorkingArea.Right  < br.X) nl.Offset(s.WorkingArea.Right - br.X, 0);
                if (s.WorkingArea.Bottom < br.Y) nl.Offset(0, s.WorkingArea.Bottom - br.Y);
                Location = nl;
            }
        }

        private void ConvertDetailForm_Shown(object sender, EventArgs e)
        {
            Location = Position;
        }
    }
}

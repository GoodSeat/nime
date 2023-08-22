using Nime.Device;
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

namespace nime
{
    public partial class ConvertDetailForm : Form
    {
        public ConvertDetailForm()
        {
            InitializeComponent();

            KeyboardWatcher.KeyDown += KeyboardWatcher_KeyDown;
        }

        string _hitKey = "";

        private void KeyboardWatcher_KeyDown(object? sender, KeyboardWatcher.KeybordWatcherEventArgs e)
        {
            if (e.Key == Nime.Device.VirtualKeys.Esc)
            {
                DialogResult = DialogResult.Cancel;
                Close();
            }
            else if (e.Key == Nime.Device.VirtualKeys.Enter)
            {
                DialogResult = DialogResult.OK;
                Close();
            }
            // アルファベット
            else if (e.Key >= Nime.Device.VirtualKeys.A && e.Key <= Nime.Device.VirtualKeys.Z)
            {
                _hitKey += e.Key.ToString().ToLower();

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
                            Refresh();
                            return;
                        }
                    }
                }
            }
            else
            {
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        public void SetTarget(ConvertCandidate sentence, Point position)
        {
            TargetSentence = sentence;
            Position = position;
        }

        public ConvertCandidate TargetSentence { get; private set; }

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
                GraphicsPath pathC = new GraphicsPath();
                foreach (var candidate in phrase.Candidates)
                {
                    pathK.AddString(candidate.Key, f, 0, 12f, new PointF(x, y), null);
                    pathC.AddString(candidate.Phrase, f, 0, 16f, new PointF(x + 20f, y), null);
                    if (candidate.Phrase == phrase.Selected)
                    {
                        RectangleF rect = new RectangleF(x, y, pathC.GetBounds().Right - x, 16f);
                        pathK.AddRectangle(rect);
                    }
                    y += 17f;
                }
                g.DrawPath(pen, pathK);
                g.FillPath(brush, pathC);

                x += Math.Max(path.GetBounds().Width, pathC.GetBounds().Width) + 30f;

                if (n >= colors.Length) n = 0;

                mx = Math.Max(pathC.GetBounds().Right, path.GetBounds().Right);
                my = pathC.GetBounds().Bottom;
            }

            Size = new Size((int)(mx + 5f), (int)(my + 5f));
        }

        private void ConvertDetailForm_Shown(object sender, EventArgs e)
        {
            Location = Position;
        }
    }
}

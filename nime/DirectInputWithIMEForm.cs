using GoodSeat.Nime;
using GoodSeat.Nime.Device;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace nime
{
    public partial class DirectInputWithIMEForm : Form
    {
        private DirectInputWithIMEForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// IMEによる直接編集が終了した時に呼び出されます。
        /// </summary>
        public static event EventHandler<DialogResult> EditEnded;

        public static string LastEditText { get; private set; }

        public static ConvertCandidatePhrase LastTargetCandidatePhrase { get; private set; }

        public static void StartEdit(ConvertCandidatePhrase phrase, Rectangle rectangle, Color foreColor)
        {
            var form = new DirectInputWithIMEForm();
            form._textBoxDirectInput.Text = phrase.Selected;
            form._textBoxDirectInput.ForeColor = foreColor;

            LastTargetCandidatePhrase = phrase;
            LastEditText = null;

            form.Show();
            form.Width = rectangle.Width;
            form.Location = rectangle.Location;
            form.Top = form.Top - 2;
            form.InitialWidth = form.Width;

            form.Activate();

            Thread.Sleep(100);
            Application.DoEvents();

            // IME直接編集を開くときにShiftを押下しているため、変換キーの検知を邪魔しないように離す
            DeviceOperator.KeyUp(VirtualKeys.ShiftLeft);
            DeviceOperator.KeyUp(VirtualKeys.ShiftRight);

            Thread.Sleep(10);
            Application.DoEvents();

            // 変換の実行
            DeviceOperator.KeyStroke(VirtualKeys.Convert);
            //SendKeys.SendWait("{F13}");
        }


        int InitialWidth;

        private void DirectInputWithIMEForm_Shown(object sender, EventArgs e)
        {
            TopMost = true;
            _textBoxDirectInput.Focus();
            _textBoxDirectInput.SelectAll();
            //_textBoxDirectInput.SelectionStart = _textBoxDirectInput.Text.Length;
        }

        private void _textBoxDirectInput_KeyUp(object sender, KeyEventArgs e)
        {
            Debug.WriteLine("textBoxDirectInput_KeyUp");
            if (LastEditText != null)
            {
                Debug.WriteLine("  => Close");
                EditEnded?.Invoke(this, DialogResult.OK);
                Close();
            }
        }

        private void _textBoxDirectInput_KeyDown(object sender, KeyEventArgs e)
        {
            Debug.WriteLine("textBoxDirectInput_KeyDown");
            if (e.KeyCode == Keys.Enter) // => Up時に閉じる
            {
                LastEditText = _textBoxDirectInput.Text.Replace("\r", "").Replace("\n", "");
                Debug.WriteLine(" => Enter");

                if (string.IsNullOrEmpty(LastEditText))
                {
                    LastTargetCandidatePhrase.Selected = "";
                }
                else
                {
                    LastTargetCandidatePhrase.Selected = LastEditText;
                }
            }
            else if (e.KeyCode == Keys.Escape) // => Up時に閉じる
            {
                LastEditText = LastTargetCandidatePhrase.Selected;
                Debug.WriteLine(" => Esc");
            }

        }

        private void DirectInputWithIMEForm_Deactivate(object sender, EventArgs e)
        {
            Debug.WriteLine("DirectInputWithIMEForm_Leave");
            if (LastEditText == null)
            {
                Debug.WriteLine(" => CloseWithCancel");
                Close();
                EditEnded?.Invoke(this, DialogResult.Cancel);
            }
        }

        private void _textBoxDirectInput_TextChanged(object sender, EventArgs e)
        {
            if (_textBoxDirectInput.Text.Contains("\r") || _textBoxDirectInput.Text.Contains("\n"))
            {
                _textBoxDirectInput.Text = _textBoxDirectInput.Text.Replace("\r", "").Replace("\n", "");
            }

            Width = Math.Max(InitialWidth, GetTextSize(_textBoxDirectInput).Width + 20);
        }



        public static Size GetTextSize(TextBox textBox)
        {
            return GetTextSize(textBox, SystemInformation.MaxWindowTrackSize);
        }

        public static Size GetTextSize(TextBox textBox, Size proposedSize)
        {
            using (var g = textBox.CreateGraphics())
            {
                return TextRenderer.MeasureText(g, textBox.Text, textBox.Font, proposedSize, CreateTextFormatFlags(textBox));
            }
        }

        public static TextFormatFlags CreateTextFormatFlags(TextBox textBox)
        {
            TextFormatFlags format = TextFormatFlags.NoPrefix | TextFormatFlags.ExpandTabs
                                   | TextFormatFlags.NoClipping | TextFormatFlags.NoPadding;
            if (!textBox.Multiline)
            {
                format |= TextFormatFlags.SingleLine;
            }
            else if (textBox.WordWrap)
            {
                format |= TextFormatFlags.WordBreak;
            }

            if (textBox.RightToLeft == RightToLeft.Yes)
            {
                format |= TextFormatFlags.RightToLeft;
                switch (textBox.TextAlign)
                {
                    case HorizontalAlignment.Center:
                        format |= TextFormatFlags.HorizontalCenter;
                        break;
                    case HorizontalAlignment.Right:
                        format |= TextFormatFlags.Left;
                        break;
                    default:
                        format |= TextFormatFlags.Right;
                        break;
                }
            }
            else
            {
                switch (textBox.TextAlign)
                {
                    case HorizontalAlignment.Center:
                        format |= TextFormatFlags.HorizontalCenter;
                        break;
                    case HorizontalAlignment.Right:
                        format |= TextFormatFlags.Right;
                        break;
                    default:
                        format |= TextFormatFlags.Left;
                        break;
                }
            }
            return format;
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

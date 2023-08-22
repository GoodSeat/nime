using Nime.Device;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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

            KeyboardWatcher.KeyUp += KeyboardWatcher_KeyUp;
        }

        private void KeyboardWatcher_KeyUp(object? sender, KeyboardWatcher.KeybordWatcherEventArgs e)
        {
            if (e.Key == Nime.Device.VirtualKeys.Esc)
            {
                Close();
            }
        }

        public void SetText(string text)
        {
            richTextBox1.Text = text;
        }

        private void ConvertDetailForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            KeyboardWatcher.KeyUp -= KeyboardWatcher_KeyUp;
        }
    }
}

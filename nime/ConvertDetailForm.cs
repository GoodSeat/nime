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
        }

        public void SetText(string text)
        {
            richTextBox1.Text = text;
        }

    }
}

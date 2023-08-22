namespace nime
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            _labelInput = new Label();
            _labelJapaneseHiragana = new Label();
            notifyIcon1 = new NotifyIcon(components);
            contextMenuStrip1 = new ContextMenuStrip(components);
            _toolStripMenuItemExist = new ToolStripMenuItem();
            contextMenuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // _labelInput
            // 
            _labelInput.AutoSize = true;
            _labelInput.ForeColor = SystemColors.ControlDarkDark;
            _labelInput.Location = new Point(3, 18);
            _labelInput.Name = "_labelInput";
            _labelInput.Size = new Size(35, 15);
            _labelInput.TabIndex = 0;
            _labelInput.Text = "input";
            // 
            // _labelJapaneseHiragana
            // 
            _labelJapaneseHiragana.AutoSize = true;
            _labelJapaneseHiragana.Location = new Point(3, 3);
            _labelJapaneseHiragana.Name = "_labelJapaneseHiragana";
            _labelJapaneseHiragana.Size = new Size(53, 15);
            _labelJapaneseHiragana.TabIndex = 0;
            _labelJapaneseHiragana.Text = "hiragana";
            // 
            // notifyIcon1
            // 
            notifyIcon1.ContextMenuStrip = contextMenuStrip1;
            notifyIcon1.Icon = (Icon)resources.GetObject("notifyIcon1.Icon");
            notifyIcon1.Text = "nime";
            notifyIcon1.Visible = true;
            // 
            // contextMenuStrip1
            // 
            contextMenuStrip1.Items.AddRange(new ToolStripItem[] { _toolStripMenuItemExist });
            contextMenuStrip1.Name = "contextMenuStrip1";
            contextMenuStrip1.Size = new Size(113, 26);
            // 
            // _toolStripMenuItemExist
            // 
            _toolStripMenuItemExist.Name = "_toolStripMenuItemExist";
            _toolStripMenuItemExist.Size = new Size(112, 22);
            _toolStripMenuItemExist.Text = "終了(&E)";
            _toolStripMenuItemExist.Click += _toolStripMenuItemExist_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(320, 36);
            Controls.Add(_labelJapaneseHiragana);
            Controls.Add(_labelInput);
            FormBorderStyle = FormBorderStyle.None;
            Name = "Form1";
            ShowInTaskbar = false;
            Text = "Form1";
            TopMost = true;
            Shown += Form1_Shown;
            contextMenuStrip1.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label _labelInput;
        private Label _labelJapaneseHiragana;
        private NotifyIcon notifyIcon1;
        private ContextMenuStrip contextMenuStrip1;
        private ToolStripMenuItem _toolStripMenuItemExist;
    }
}
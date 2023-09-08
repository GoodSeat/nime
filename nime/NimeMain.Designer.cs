namespace GoodSeat.Nime
{
    partial class NimeMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NimeMain));
            _labelInput = new Label();
            _labelJapaneseHiragana = new Label();
            notifyIcon1 = new NotifyIcon(components);
            contextMenuStrip1 = new ContextMenuStrip(components);
            _toolStripMenuItemRunning = new ToolStripMenuItem();
            _toolStripMenuItemNaviView = new ToolStripMenuItem();
            _toolStripMenuItemSetting = new ToolStripMenuItem();
            toolStripSeparator1 = new ToolStripSeparator();
            _toolStripMenuItemRestart = new ToolStripMenuItem();
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
            _labelInput.Visible = false;
            // 
            // _labelJapaneseHiragana
            // 
            _labelJapaneseHiragana.AutoSize = true;
            _labelJapaneseHiragana.Location = new Point(3, 3);
            _labelJapaneseHiragana.Name = "_labelJapaneseHiragana";
            _labelJapaneseHiragana.Size = new Size(53, 15);
            _labelJapaneseHiragana.TabIndex = 0;
            _labelJapaneseHiragana.Text = "hiragana";
            _labelJapaneseHiragana.Visible = false;
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
            contextMenuStrip1.Items.AddRange(new ToolStripItem[] { _toolStripMenuItemRunning, _toolStripMenuItemNaviView, _toolStripMenuItemSetting, toolStripSeparator1, _toolStripMenuItemRestart, _toolStripMenuItemExist });
            contextMenuStrip1.Name = "contextMenuStrip1";
            contextMenuStrip1.Size = new Size(138, 120);
            // 
            // _toolStripMenuItemRunning
            // 
            _toolStripMenuItemRunning.Checked = true;
            _toolStripMenuItemRunning.CheckState = CheckState.Checked;
            _toolStripMenuItemRunning.Name = "_toolStripMenuItemRunning";
            _toolStripMenuItemRunning.Size = new Size(137, 22);
            _toolStripMenuItemRunning.Text = "有効(&E)";
            _toolStripMenuItemRunning.Click += _toolStripMenuItemRunning_Click;
            // 
            // _toolStripMenuItemNaviView
            // 
            _toolStripMenuItemNaviView.Checked = true;
            _toolStripMenuItemNaviView.CheckState = CheckState.Checked;
            _toolStripMenuItemNaviView.Name = "_toolStripMenuItemNaviView";
            _toolStripMenuItemNaviView.Size = new Size(137, 22);
            _toolStripMenuItemNaviView.Text = "入力表示(&V)";
            _toolStripMenuItemNaviView.Click += _toolStripMenuItemNaviView_Click;
            // 
            // _toolStripMenuItemSetting
            // 
            _toolStripMenuItemSetting.Name = "_toolStripMenuItemSetting";
            _toolStripMenuItemSetting.Size = new Size(137, 22);
            _toolStripMenuItemSetting.Text = "設定...(&S)";
            _toolStripMenuItemSetting.Click += _toolStripMenuItemSetting_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(134, 6);
            // 
            // _toolStripMenuItemRestart
            // 
            _toolStripMenuItemRestart.Name = "_toolStripMenuItemRestart";
            _toolStripMenuItemRestart.Size = new Size(137, 22);
            _toolStripMenuItemRestart.Text = "再起動(&R)";
            _toolStripMenuItemRestart.Click += _toolStripMenuItemRestart_Click;
            // 
            // _toolStripMenuItemExist
            // 
            _toolStripMenuItemExist.Name = "_toolStripMenuItemExist";
            _toolStripMenuItemExist.Size = new Size(137, 22);
            _toolStripMenuItemExist.Text = "終了(&Q)";
            _toolStripMenuItemExist.Click += _toolStripMenuItemExist_Click;
            // 
            // NimeMain
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(70, 36);
            Controls.Add(_labelJapaneseHiragana);
            Controls.Add(_labelInput);
            FormBorderStyle = FormBorderStyle.None;
            Name = "NimeMain";
            ShowInTaskbar = false;
            Text = "[nime]";
            TopMost = true;
            Shown += Form1_Shown;
            Paint += Form1_FormClosing;
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
        private ToolStripMenuItem _toolStripMenuItemRunning;
        private ToolStripMenuItem _toolStripMenuItemNaviView;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem _toolStripMenuItemSetting;
        private ToolStripMenuItem _toolStripMenuItemRestart;
    }
}
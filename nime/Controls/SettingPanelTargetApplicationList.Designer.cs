namespace GoodSeat.Nime.Controls
{
    partial class SettingPanelTargetApplicationList
    {
        /// <summary> 
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージド リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region コンポーネント デザイナーで生成されたコード

        /// <summary> 
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を 
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            splitContainer1 = new SplitContainer();
            _treeViewTarget = new TreeView();
            groupBox1 = new GroupBox();
            _textBoxFileNameMRU = new TextBox();
            label8 = new Label();
            label9 = new Label();
            _textBoxProductNameMRU = new TextBox();
            label10 = new Label();
            _textBoxTitleMRU = new TextBox();
            label11 = new Label();
            _textBoxClassNameMRU = new TextBox();
            _checkBoxRegexClassName = new CheckBox();
            _checkBoxRegexTitle = new CheckBox();
            _checkBoxRegexProductName = new CheckBox();
            _checkBoxRegexFileName = new CheckBox();
            _comboBoxBase = new ComboBox();
            _comboBoxClassName = new ComboBox();
            _comboBoxTitle = new ComboBox();
            _comboBoxProductName = new ComboBox();
            _comboBoxFileName = new ComboBox();
            _textBoxClassName = new TextBox();
            label7 = new Label();
            label5 = new Label();
            _textBoxTitle = new TextBox();
            label4 = new Label();
            _textBoxProductName = new TextBox();
            label3 = new Label();
            _textBoxFileName = new TextBox();
            label6 = new Label();
            label2 = new Label();
            _textBoxName = new TextBox();
            label1 = new Label();
            _buttonAddTarget = new Button();
            _buttonDeleteTarget = new Button();
            _buttonMoveUp = new Button();
            _buttonMoveDown = new Button();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // splitContainer1
            // 
            splitContainer1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            splitContainer1.Location = new Point(3, 32);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(_treeViewTarget);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(groupBox1);
            splitContainer1.Panel2.Controls.Add(_checkBoxRegexClassName);
            splitContainer1.Panel2.Controls.Add(_checkBoxRegexTitle);
            splitContainer1.Panel2.Controls.Add(_checkBoxRegexProductName);
            splitContainer1.Panel2.Controls.Add(_checkBoxRegexFileName);
            splitContainer1.Panel2.Controls.Add(_comboBoxBase);
            splitContainer1.Panel2.Controls.Add(_comboBoxClassName);
            splitContainer1.Panel2.Controls.Add(_comboBoxTitle);
            splitContainer1.Panel2.Controls.Add(_comboBoxProductName);
            splitContainer1.Panel2.Controls.Add(_comboBoxFileName);
            splitContainer1.Panel2.Controls.Add(_textBoxClassName);
            splitContainer1.Panel2.Controls.Add(label7);
            splitContainer1.Panel2.Controls.Add(label5);
            splitContainer1.Panel2.Controls.Add(_textBoxTitle);
            splitContainer1.Panel2.Controls.Add(label4);
            splitContainer1.Panel2.Controls.Add(_textBoxProductName);
            splitContainer1.Panel2.Controls.Add(label3);
            splitContainer1.Panel2.Controls.Add(_textBoxFileName);
            splitContainer1.Panel2.Controls.Add(label6);
            splitContainer1.Panel2.Controls.Add(label2);
            splitContainer1.Panel2.Controls.Add(_textBoxName);
            splitContainer1.Panel2.Controls.Add(label1);
            splitContainer1.Size = new Size(505, 450);
            splitContainer1.SplitterDistance = 167;
            splitContainer1.TabIndex = 0;
            // 
            // _treeViewTarget
            // 
            _treeViewTarget.Dock = DockStyle.Fill;
            _treeViewTarget.Location = new Point(0, 0);
            _treeViewTarget.Name = "_treeViewTarget";
            _treeViewTarget.Size = new Size(167, 450);
            _treeViewTarget.TabIndex = 0;
            _treeViewTarget.AfterSelect += _treeViewTarget_AfterSelect;
            // 
            // groupBox1
            // 
            groupBox1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            groupBox1.Controls.Add(_textBoxFileNameMRU);
            groupBox1.Controls.Add(label8);
            groupBox1.Controls.Add(label9);
            groupBox1.Controls.Add(_textBoxProductNameMRU);
            groupBox1.Controls.Add(label10);
            groupBox1.Controls.Add(_textBoxTitleMRU);
            groupBox1.Controls.Add(label11);
            groupBox1.Controls.Add(_textBoxClassNameMRU);
            groupBox1.Location = new Point(13, 238);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(309, 155);
            groupBox1.TabIndex = 4;
            groupBox1.TabStop = false;
            groupBox1.Text = "直近の入力対象";
            // 
            // _textBoxFileNameMRU
            // 
            _textBoxFileNameMRU.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            _textBoxFileNameMRU.Location = new Point(78, 27);
            _textBoxFileNameMRU.Name = "_textBoxFileNameMRU";
            _textBoxFileNameMRU.ReadOnly = true;
            _textBoxFileNameMRU.Size = new Size(215, 23);
            _textBoxFileNameMRU.TabIndex = 1;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(14, 30);
            label8.Name = "label8";
            label8.Size = new Size(56, 15);
            label8.TabIndex = 0;
            label8.Text = "ファイル名:";
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new Point(14, 59);
            label9.Name = "label9";
            label9.Size = new Size(46, 15);
            label9.TabIndex = 0;
            label9.Text = "製品名:";
            // 
            // _textBoxProductNameMRU
            // 
            _textBoxProductNameMRU.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            _textBoxProductNameMRU.Location = new Point(78, 56);
            _textBoxProductNameMRU.Name = "_textBoxProductNameMRU";
            _textBoxProductNameMRU.ReadOnly = true;
            _textBoxProductNameMRU.Size = new Size(215, 23);
            _textBoxProductNameMRU.TabIndex = 1;
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Location = new Point(14, 88);
            label10.Name = "label10";
            label10.Size = new Size(46, 15);
            label10.TabIndex = 0;
            label10.Text = "タイトル:";
            // 
            // _textBoxTitleMRU
            // 
            _textBoxTitleMRU.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            _textBoxTitleMRU.Location = new Point(78, 85);
            _textBoxTitleMRU.Name = "_textBoxTitleMRU";
            _textBoxTitleMRU.ReadOnly = true;
            _textBoxTitleMRU.Size = new Size(215, 23);
            _textBoxTitleMRU.TabIndex = 1;
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Location = new Point(14, 117);
            label11.Name = "label11";
            label11.Size = new Size(48, 15);
            label11.TabIndex = 0;
            label11.Text = "クラス名:";
            // 
            // _textBoxClassNameMRU
            // 
            _textBoxClassNameMRU.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            _textBoxClassNameMRU.Location = new Point(78, 114);
            _textBoxClassNameMRU.Name = "_textBoxClassNameMRU";
            _textBoxClassNameMRU.ReadOnly = true;
            _textBoxClassNameMRU.Size = new Size(215, 23);
            _textBoxClassNameMRU.TabIndex = 1;
            // 
            // _checkBoxRegexClassName
            // 
            _checkBoxRegexClassName.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            _checkBoxRegexClassName.AutoSize = true;
            _checkBoxRegexClassName.Location = new Point(302, 202);
            _checkBoxRegexClassName.Name = "_checkBoxRegexClassName";
            _checkBoxRegexClassName.Size = new Size(15, 14);
            _checkBoxRegexClassName.TabIndex = 3;
            _checkBoxRegexClassName.UseVisualStyleBackColor = true;
            _checkBoxRegexClassName.CheckedChanged += _checkBoxRegexClassName_CheckedChanged;
            // 
            // _checkBoxRegexTitle
            // 
            _checkBoxRegexTitle.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            _checkBoxRegexTitle.AutoSize = true;
            _checkBoxRegexTitle.Location = new Point(302, 173);
            _checkBoxRegexTitle.Name = "_checkBoxRegexTitle";
            _checkBoxRegexTitle.Size = new Size(15, 14);
            _checkBoxRegexTitle.TabIndex = 3;
            _checkBoxRegexTitle.UseVisualStyleBackColor = true;
            _checkBoxRegexTitle.CheckedChanged += _checkBoxRegexTitle_CheckedChanged;
            // 
            // _checkBoxRegexProductName
            // 
            _checkBoxRegexProductName.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            _checkBoxRegexProductName.AutoSize = true;
            _checkBoxRegexProductName.Location = new Point(302, 144);
            _checkBoxRegexProductName.Name = "_checkBoxRegexProductName";
            _checkBoxRegexProductName.Size = new Size(15, 14);
            _checkBoxRegexProductName.TabIndex = 3;
            _checkBoxRegexProductName.UseVisualStyleBackColor = true;
            _checkBoxRegexProductName.CheckedChanged += _checkBoxRegexProductName_CheckedChanged;
            // 
            // _checkBoxRegexFileName
            // 
            _checkBoxRegexFileName.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            _checkBoxRegexFileName.AutoSize = true;
            _checkBoxRegexFileName.Location = new Point(302, 115);
            _checkBoxRegexFileName.Name = "_checkBoxRegexFileName";
            _checkBoxRegexFileName.Size = new Size(15, 14);
            _checkBoxRegexFileName.TabIndex = 3;
            _checkBoxRegexFileName.UseVisualStyleBackColor = true;
            _checkBoxRegexFileName.CheckedChanged += _checkBoxRegexFileName_CheckedChanged;
            // 
            // _comboBoxBase
            // 
            _comboBoxBase.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            _comboBoxBase.DropDownStyle = ComboBoxStyle.DropDownList;
            _comboBoxBase.FormattingEnabled = true;
            _comboBoxBase.Items.AddRange(new object[] { "を含む", "と一致" });
            _comboBoxBase.Location = new Point(77, 41);
            _comboBoxBase.Name = "_comboBoxBase";
            _comboBoxBase.Size = new Size(240, 23);
            _comboBoxBase.TabIndex = 2;
            // 
            // _comboBoxClassName
            // 
            _comboBoxClassName.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            _comboBoxClassName.DropDownStyle = ComboBoxStyle.DropDownList;
            _comboBoxClassName.FormattingEnabled = true;
            _comboBoxClassName.Items.AddRange(new object[] { "を含む", "と一致" });
            _comboBoxClassName.Location = new Point(233, 198);
            _comboBoxClassName.Name = "_comboBoxClassName";
            _comboBoxClassName.Size = new Size(61, 23);
            _comboBoxClassName.TabIndex = 2;
            _comboBoxClassName.SelectedIndexChanged += _comboBoxClassName_SelectedIndexChanged;
            // 
            // _comboBoxTitle
            // 
            _comboBoxTitle.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            _comboBoxTitle.DropDownStyle = ComboBoxStyle.DropDownList;
            _comboBoxTitle.FormattingEnabled = true;
            _comboBoxTitle.Items.AddRange(new object[] { "を含む", "と一致" });
            _comboBoxTitle.Location = new Point(233, 169);
            _comboBoxTitle.Name = "_comboBoxTitle";
            _comboBoxTitle.Size = new Size(61, 23);
            _comboBoxTitle.TabIndex = 2;
            _comboBoxTitle.SelectedIndexChanged += _comboBoxTitle_SelectedIndexChanged;
            // 
            // _comboBoxProductName
            // 
            _comboBoxProductName.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            _comboBoxProductName.DropDownStyle = ComboBoxStyle.DropDownList;
            _comboBoxProductName.FormattingEnabled = true;
            _comboBoxProductName.Items.AddRange(new object[] { "を含む", "と一致" });
            _comboBoxProductName.Location = new Point(233, 140);
            _comboBoxProductName.Name = "_comboBoxProductName";
            _comboBoxProductName.Size = new Size(61, 23);
            _comboBoxProductName.TabIndex = 2;
            _comboBoxProductName.SelectedIndexChanged += _comboBoxProductName_SelectedIndexChanged;
            // 
            // _comboBoxFileName
            // 
            _comboBoxFileName.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            _comboBoxFileName.DropDownStyle = ComboBoxStyle.DropDownList;
            _comboBoxFileName.FormattingEnabled = true;
            _comboBoxFileName.Items.AddRange(new object[] { "を含む", "と一致" });
            _comboBoxFileName.Location = new Point(233, 111);
            _comboBoxFileName.Name = "_comboBoxFileName";
            _comboBoxFileName.Size = new Size(61, 23);
            _comboBoxFileName.TabIndex = 2;
            _comboBoxFileName.SelectedIndexChanged += _comboBoxFileName_SelectedIndexChanged;
            // 
            // _textBoxClassName
            // 
            _textBoxClassName.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            _textBoxClassName.Location = new Point(75, 198);
            _textBoxClassName.Name = "_textBoxClassName";
            _textBoxClassName.Size = new Size(152, 23);
            _textBoxClassName.TabIndex = 1;
            _textBoxClassName.TextChanged += _textBoxClassName_TextChanged;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(13, 44);
            label7.Name = "label7";
            label7.Size = new Size(46, 15);
            label7.TabIndex = 0;
            label7.Text = "継承元:";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(11, 201);
            label5.Name = "label5";
            label5.Size = new Size(48, 15);
            label5.TabIndex = 0;
            label5.Text = "クラス名:";
            // 
            // _textBoxTitle
            // 
            _textBoxTitle.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            _textBoxTitle.Location = new Point(75, 169);
            _textBoxTitle.Name = "_textBoxTitle";
            _textBoxTitle.Size = new Size(152, 23);
            _textBoxTitle.TabIndex = 1;
            _textBoxTitle.TextChanged += _textBoxTitle_TextChanged;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(11, 172);
            label4.Name = "label4";
            label4.Size = new Size(46, 15);
            label4.TabIndex = 0;
            label4.Text = "タイトル:";
            // 
            // _textBoxProductName
            // 
            _textBoxProductName.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            _textBoxProductName.Location = new Point(75, 140);
            _textBoxProductName.Name = "_textBoxProductName";
            _textBoxProductName.Size = new Size(152, 23);
            _textBoxProductName.TabIndex = 1;
            _textBoxProductName.TextChanged += _textBoxProductName_TextChanged;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(11, 143);
            label3.Name = "label3";
            label3.Size = new Size(46, 15);
            label3.TabIndex = 0;
            label3.Text = "製品名:";
            // 
            // _textBoxFileName
            // 
            _textBoxFileName.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            _textBoxFileName.Location = new Point(75, 111);
            _textBoxFileName.Name = "_textBoxFileName";
            _textBoxFileName.Size = new Size(152, 23);
            _textBoxFileName.TabIndex = 1;
            _textBoxFileName.TextChanged += _textBoxFileName_TextChanged;
            // 
            // label6
            // 
            label6.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label6.AutoSize = true;
            label6.Location = new Point(291, 78);
            label6.Name = "label6";
            label6.Size = new Size(31, 30);
            label6.TabIndex = 0;
            label6.Text = "正規\r\n表現";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(11, 114);
            label2.Name = "label2";
            label2.Size = new Size(56, 15);
            label2.TabIndex = 0;
            label2.Text = "ファイル名:";
            // 
            // _textBoxName
            // 
            _textBoxName.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            _textBoxName.Location = new Point(77, 12);
            _textBoxName.Name = "_textBoxName";
            _textBoxName.Size = new Size(240, 23);
            _textBoxName.TabIndex = 1;
            _textBoxName.TextChanged += _textBoxName_TextChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(13, 15);
            label1.Name = "label1";
            label1.Size = new Size(34, 15);
            label1.TabIndex = 0;
            label1.Text = "名前:";
            // 
            // _buttonAddTarget
            // 
            _buttonAddTarget.Location = new Point(3, 3);
            _buttonAddTarget.Name = "_buttonAddTarget";
            _buttonAddTarget.Size = new Size(47, 23);
            _buttonAddTarget.TabIndex = 1;
            _buttonAddTarget.Text = "追加";
            _buttonAddTarget.UseVisualStyleBackColor = true;
            _buttonAddTarget.Click += _buttonAddTarget_Click;
            // 
            // _buttonDeleteTarget
            // 
            _buttonDeleteTarget.Location = new Point(56, 3);
            _buttonDeleteTarget.Name = "_buttonDeleteTarget";
            _buttonDeleteTarget.Size = new Size(47, 23);
            _buttonDeleteTarget.TabIndex = 1;
            _buttonDeleteTarget.Text = "削除";
            _buttonDeleteTarget.UseVisualStyleBackColor = true;
            _buttonDeleteTarget.Click += _buttonDeleteTarget_Click;
            // 
            // _buttonMoveUp
            // 
            _buttonMoveUp.Location = new Point(109, 3);
            _buttonMoveUp.Name = "_buttonMoveUp";
            _buttonMoveUp.Size = new Size(25, 23);
            _buttonMoveUp.TabIndex = 1;
            _buttonMoveUp.Text = "↑";
            _buttonMoveUp.UseVisualStyleBackColor = true;
            // 
            // _buttonMoveDown
            // 
            _buttonMoveDown.Location = new Point(140, 3);
            _buttonMoveDown.Name = "_buttonMoveDown";
            _buttonMoveDown.Size = new Size(25, 23);
            _buttonMoveDown.TabIndex = 1;
            _buttonMoveDown.Text = "↓";
            _buttonMoveDown.UseVisualStyleBackColor = true;
            // 
            // SettingPanelTargetApplicationList
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(_buttonMoveDown);
            Controls.Add(_buttonMoveUp);
            Controls.Add(_buttonDeleteTarget);
            Controls.Add(_buttonAddTarget);
            Controls.Add(splitContainer1);
            Name = "SettingPanelTargetApplicationList";
            Size = new Size(511, 482);
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private SplitContainer splitContainer1;
        private TreeView _treeViewTarget;
        private TextBox _textBoxName;
        private Label label1;
        private TextBox _textBoxClassName;
        private Label label5;
        private TextBox _textBoxTitle;
        private Label label4;
        private TextBox _textBoxProductName;
        private Label label3;
        private TextBox _textBoxFileName;
        private Label label2;
        private ComboBox _comboBoxClassName;
        private ComboBox _comboBoxTitle;
        private ComboBox _comboBoxProductName;
        private ComboBox _comboBoxFileName;
        private Button _buttonAddTarget;
        private Button _buttonDeleteTarget;
        private CheckBox _checkBoxRegexClassName;
        private CheckBox _checkBoxRegexTitle;
        private CheckBox _checkBoxRegexProductName;
        private CheckBox _checkBoxRegexFileName;
        private Label label6;
        private ComboBox _comboBoxBase;
        private Label label7;
        private Button _buttonMoveUp;
        private Button _buttonMoveDown;
        private GroupBox groupBox1;
        private TextBox _textBoxFileNameMRU;
        private Label label8;
        private Label label9;
        private TextBox _textBoxProductNameMRU;
        private Label label10;
        private TextBox _textBoxTitleMRU;
        private Label label11;
        private TextBox _textBoxClassNameMRU;
    }
}

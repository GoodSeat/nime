namespace GoodSeat.Nime.Controls
{
    partial class SettingPanelTargetApplication
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
            checkBox1 = new CheckBox();
            checkBox2 = new CheckBox();
            label1 = new Label();
            checkBox3 = new CheckBox();
            label2 = new Label();
            checkBox4 = new CheckBox();
            label3 = new Label();
            comboBox1 = new ComboBox();
            comboBox2 = new ComboBox();
            checkBox5 = new CheckBox();
            checkBox6 = new CheckBox();
            _comboBoxBase = new ComboBox();
            label7 = new Label();
            SuspendLayout();
            // 
            // checkBox1
            // 
            checkBox1.AutoSize = true;
            checkBox1.Location = new Point(16, 77);
            checkBox1.Name = "checkBox1";
            checkBox1.Size = new Size(113, 19);
            checkBox1.TabIndex = 0;
            checkBox1.Text = "nimeを無効にする";
            checkBox1.UseVisualStyleBackColor = true;
            // 
            // checkBox2
            // 
            checkBox2.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            checkBox2.AutoSize = true;
            checkBox2.Location = new Point(328, 77);
            checkBox2.Name = "checkBox2";
            checkBox2.Size = new Size(15, 14);
            checkBox2.TabIndex = 0;
            checkBox2.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(16, 115);
            label1.Name = "label1";
            label1.Size = new Size(103, 15);
            label1.TabIndex = 1;
            label1.Text = "テキストの入力方法:";
            // 
            // checkBox3
            // 
            checkBox3.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            checkBox3.AutoSize = true;
            checkBox3.Location = new Point(328, 113);
            checkBox3.Name = "checkBox3";
            checkBox3.Size = new Size(15, 14);
            checkBox3.TabIndex = 0;
            checkBox3.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(16, 145);
            label2.Name = "label2";
            label2.Size = new Size(103, 15);
            label2.TabIndex = 1;
            label2.Text = "テキストの削除方法:";
            // 
            // checkBox4
            // 
            checkBox4.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            checkBox4.AutoSize = true;
            checkBox4.Location = new Point(328, 144);
            checkBox4.Name = "checkBox4";
            checkBox4.Size = new Size(15, 14);
            checkBox4.TabIndex = 0;
            checkBox4.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            label3.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label3.AutoSize = true;
            label3.Location = new Point(319, 54);
            label3.Name = "label3";
            label3.Size = new Size(31, 15);
            label3.TabIndex = 1;
            label3.Text = "継承";
            // 
            // comboBox1
            // 
            comboBox1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox1.FormattingEnabled = true;
            comboBox1.Items.AddRange(new object[] { "を含む", "と一致" });
            comboBox1.Location = new Point(125, 112);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new Size(184, 23);
            comboBox1.TabIndex = 3;
            // 
            // comboBox2
            // 
            comboBox2.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            comboBox2.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox2.FormattingEnabled = true;
            comboBox2.Items.AddRange(new object[] { "を含む", "と一致" });
            comboBox2.Location = new Point(125, 141);
            comboBox2.Name = "comboBox2";
            comboBox2.Size = new Size(184, 23);
            comboBox2.TabIndex = 3;
            // 
            // checkBox5
            // 
            checkBox5.AutoSize = true;
            checkBox5.Location = new Point(16, 190);
            checkBox5.Name = "checkBox5";
            checkBox5.Size = new Size(227, 19);
            checkBox5.TabIndex = 0;
            checkBox5.Text = "キャレット位置変化検知時にも変換を実行";
            checkBox5.UseVisualStyleBackColor = true;
            // 
            // checkBox6
            // 
            checkBox6.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            checkBox6.AutoSize = true;
            checkBox6.Location = new Point(328, 190);
            checkBox6.Name = "checkBox6";
            checkBox6.Size = new Size(15, 14);
            checkBox6.TabIndex = 0;
            checkBox6.UseVisualStyleBackColor = true;
            // 
            // _comboBoxBase
            // 
            _comboBoxBase.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            _comboBoxBase.DropDownStyle = ComboBoxStyle.DropDownList;
            _comboBoxBase.FormattingEnabled = true;
            _comboBoxBase.Items.AddRange(new object[] { "を含む", "と一致" });
            _comboBoxBase.Location = new Point(75, 13);
            _comboBoxBase.Name = "_comboBoxBase";
            _comboBoxBase.Size = new Size(150, 23);
            _comboBoxBase.TabIndex = 5;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(11, 16);
            label7.Name = "label7";
            label7.Size = new Size(46, 15);
            label7.TabIndex = 4;
            label7.Text = "継承元:";
            // 
            // SettingPanelTargetApplication
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(_comboBoxBase);
            Controls.Add(label7);
            Controls.Add(comboBox2);
            Controls.Add(comboBox1);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(checkBox4);
            Controls.Add(checkBox3);
            Controls.Add(checkBox6);
            Controls.Add(checkBox5);
            Controls.Add(checkBox2);
            Controls.Add(checkBox1);
            Name = "SettingPanelTargetApplication";
            Size = new Size(353, 336);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private CheckBox checkBox1;
        private CheckBox checkBox2;
        private Label label1;
        private CheckBox checkBox3;
        private Label label2;
        private CheckBox checkBox4;
        private Label label3;
        private ComboBox comboBox1;
        private ComboBox comboBox2;
        private CheckBox checkBox5;
        private CheckBox checkBox6;
        private ComboBox _comboBoxBase;
        private Label label7;
    }
}

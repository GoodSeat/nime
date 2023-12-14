namespace GoodSeat.Nime
{
    partial class SettingForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            TreeNode treeNode1 = new TreeNode("一般設定");
            TreeNode treeNode2 = new TreeNode("変換設定");
            TreeNode treeNode3 = new TreeNode("入力表示");
            TreeNode treeNode4 = new TreeNode("入力補完");
            TreeNode treeNode5 = new TreeNode("変換画面");
            TreeNode treeNode6 = new TreeNode("アプリ毎の設定");
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingForm));
            _treeViewContents = new TreeView();
            _splitContainer = new SplitContainer();
            _buttonCancel = new Button();
            _buttonOK = new Button();
            _buttonApply = new Button();
            ((System.ComponentModel.ISupportInitialize)_splitContainer).BeginInit();
            _splitContainer.Panel1.SuspendLayout();
            _splitContainer.SuspendLayout();
            SuspendLayout();
            // 
            // _treeViewContents
            // 
            _treeViewContents.Dock = DockStyle.Fill;
            _treeViewContents.FullRowSelect = true;
            _treeViewContents.HideSelection = false;
            _treeViewContents.Location = new Point(0, 0);
            _treeViewContents.Name = "_treeViewContents";
            treeNode1.Name = "_nodeGenealSetting";
            treeNode1.Text = "一般設定";
            treeNode2.Name = "_nodeConvertSetting";
            treeNode2.Text = "変換設定";
            treeNode3.Name = "_nodeInputView";
            treeNode3.Text = "入力表示";
            treeNode4.Name = "_nodeInputSuggestion";
            treeNode4.Text = "入力補完";
            treeNode5.Name = "_nodeConvertView";
            treeNode5.Text = "変換画面";
            treeNode6.Name = "_nodeEachApplicatoinSetting";
            treeNode6.Text = "アプリ毎の設定";
            _treeViewContents.Nodes.AddRange(new TreeNode[] { treeNode1, treeNode2, treeNode3, treeNode4, treeNode5, treeNode6 });
            _treeViewContents.ShowLines = false;
            _treeViewContents.ShowPlusMinus = false;
            _treeViewContents.ShowRootLines = false;
            _treeViewContents.Size = new Size(131, 603);
            _treeViewContents.TabIndex = 0;
            _treeViewContents.BeforeCollapse += _treeViewContents_BeforeCollapse;
            _treeViewContents.AfterSelect += _treeViewContents_AfterSelect;
            // 
            // _splitContainer
            // 
            _splitContainer.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            _splitContainer.Location = new Point(12, 12);
            _splitContainer.Name = "_splitContainer";
            // 
            // _splitContainer.Panel1
            // 
            _splitContainer.Panel1.Controls.Add(_treeViewContents);
            _splitContainer.Size = new Size(705, 603);
            _splitContainer.SplitterDistance = 131;
            _splitContainer.TabIndex = 1;
            // 
            // _buttonCancel
            // 
            _buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            _buttonCancel.Location = new Point(561, 621);
            _buttonCancel.Name = "_buttonCancel";
            _buttonCancel.Size = new Size(75, 23);
            _buttonCancel.TabIndex = 2;
            _buttonCancel.Text = "キャンセル";
            _buttonCancel.UseVisualStyleBackColor = true;
            // 
            // _buttonOK
            // 
            _buttonOK.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            _buttonOK.Location = new Point(480, 621);
            _buttonOK.Name = "_buttonOK";
            _buttonOK.Size = new Size(75, 23);
            _buttonOK.TabIndex = 2;
            _buttonOK.Text = "OK";
            _buttonOK.UseVisualStyleBackColor = true;
            // 
            // _buttonApply
            // 
            _buttonApply.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            _buttonApply.Location = new Point(642, 621);
            _buttonApply.Name = "_buttonApply";
            _buttonApply.Size = new Size(75, 23);
            _buttonApply.TabIndex = 2;
            _buttonApply.Text = "適用";
            _buttonApply.UseVisualStyleBackColor = true;
            // 
            // SettingForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(729, 656);
            Controls.Add(_buttonOK);
            Controls.Add(_buttonApply);
            Controls.Add(_buttonCancel);
            Controls.Add(_splitContainer);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "SettingForm";
            Text = "nimeの設定";
            _splitContainer.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)_splitContainer).EndInit();
            _splitContainer.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private TreeView _treeViewContents;
        private SplitContainer _splitContainer;
        private Button _buttonCancel;
        private Button _buttonOK;
        private Button _buttonApply;
    }
}
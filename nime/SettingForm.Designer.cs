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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingForm));
            _treeViewContents = new TreeView();
            _splitContainer = new SplitContainer();
            ((System.ComponentModel.ISupportInitialize)_splitContainer).BeginInit();
            _splitContainer.Panel1.SuspendLayout();
            _splitContainer.SuspendLayout();
            SuspendLayout();
            // 
            // _treeViewContents
            // 
            _treeViewContents.Dock = DockStyle.Fill;
            _treeViewContents.Location = new Point(0, 0);
            _treeViewContents.Name = "_treeViewContents";
            _treeViewContents.Size = new Size(219, 645);
            _treeViewContents.TabIndex = 0;
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
            _splitContainer.Size = new Size(838, 645);
            _splitContainer.SplitterDistance = 219;
            _splitContainer.TabIndex = 1;
            // 
            // SettingForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(862, 669);
            Controls.Add(_splitContainer);
            Icon = (Icon)resources.GetObject("$this.Icon");
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
    }
}
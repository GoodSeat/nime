namespace nime
{
    partial class DirectInputWithIMEForm
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
            _textBoxDirectInput = new TextBox();
            SuspendLayout();
            // 
            // _textBoxDirectInput
            // 
            _textBoxDirectInput.Dock = DockStyle.Fill;
            _textBoxDirectInput.Font = new Font("Yu Gothic UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            _textBoxDirectInput.ImeMode = ImeMode.On;
            _textBoxDirectInput.Location = new Point(0, 0);
            _textBoxDirectInput.Multiline = true;
            _textBoxDirectInput.Name = "_textBoxDirectInput";
            _textBoxDirectInput.Size = new Size(264, 26);
            _textBoxDirectInput.TabIndex = 0;
            _textBoxDirectInput.WordWrap = false;
            _textBoxDirectInput.TextChanged += _textBoxDirectInput_TextChanged;
            _textBoxDirectInput.KeyDown += _textBoxDirectInput_KeyDown;
            _textBoxDirectInput.KeyUp += _textBoxDirectInput_KeyUp;
            // 
            // DirectInputWithIMEForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(264, 26);
            Controls.Add(_textBoxDirectInput);
            FormBorderStyle = FormBorderStyle.None;
            Name = "DirectInputWithIMEForm";
            ShowInTaskbar = false;
            Text = "[nime]IMEでの編集";
            TopMost = true;
            Deactivate += DirectInputWithIMEForm_Deactivate;
            Shown += DirectInputWithIMEForm_Shown;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox _textBoxDirectInput;
    }
}
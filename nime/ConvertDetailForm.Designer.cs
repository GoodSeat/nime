﻿namespace GoodSeat.Nime
{
    partial class ConvertDetailForm
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
            SuspendLayout();
            // 
            // ConvertDetailForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(361, 151);
            FormBorderStyle = FormBorderStyle.None;
            Name = "ConvertDetailForm";
            ShowInTaskbar = false;
            Text = "[nime]変換ウインドウ";
            TopMost = true;
            Paint += ConvertDetailForm_Paint;
            ResumeLayout(false);
        }

        #endregion
    }
}
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
            _checkBoxEnabled = new CheckBox();
            _checkBoxEnabledDerived = new CheckBox();
            label1 = new Label();
            _checkBoxInputMethodDerived = new CheckBox();
            label2 = new Label();
            _checkBoxDeleteMethodDerived = new CheckBox();
            label3 = new Label();
            _comboBoxInputMethod = new ComboBox();
            _comboBoxDeleteMethod = new ComboBox();
            _checkBoxIgnoreCaretPositionChange = new CheckBox();
            _checkBoxIgnoreCaretPositionChangeDerived = new CheckBox();
            _comboBoxBase = new ComboBox();
            label7 = new Label();
            _checkBoxVisibleInputNavi = new CheckBox();
            _checkBoxVisibleInputNaviDerived = new CheckBox();
            _checkBoxEnabledInputSuggest = new CheckBox();
            _checkBoxEnabledInputSuggestDerived = new CheckBox();
            _checkBoxAutoConvertOnInputCommma = new CheckBox();
            _checkBoxAutoConvertOnInputCommmaDerived = new CheckBox();
            _checkBoxAutoConvertOnInputPeriod = new CheckBox();
            _checkBoxAutoConvertOnInputPeriodDerived = new CheckBox();
            _checkBoxForceConvertCtrlU = new CheckBox();
            _checkBoxForceConvertCtrlUDerived = new CheckBox();
            _checkBoxForceConvertCtrlI = new CheckBox();
            _checkBoxForceConvertCtrlIDerived = new CheckBox();
            _checkBoxForceConvertCtrlO = new CheckBox();
            _checkBoxForceConvertCtrlODerived = new CheckBox();
            _checkBoxForceConvertCtrlP = new CheckBox();
            _checkBoxForceConvertCtrlPDerived = new CheckBox();
            _checkBoxForceConvertF6 = new CheckBox();
            _checkBoxForceConvertF7 = new CheckBox();
            _checkBoxForceConvertF8 = new CheckBox();
            _checkBoxForceConvertF9 = new CheckBox();
            _checkBoxForceConvertF6Derived = new CheckBox();
            _checkBoxForceConvertF7Derived = new CheckBox();
            _checkBoxForceConvertF8Derived = new CheckBox();
            _checkBoxForceConvertF9Derived = new CheckBox();
            SuspendLayout();
            // 
            // _checkBoxEnabled
            // 
            _checkBoxEnabled.AutoSize = true;
            _checkBoxEnabled.Location = new Point(16, 77);
            _checkBoxEnabled.Name = "_checkBoxEnabled";
            _checkBoxEnabled.Size = new Size(113, 19);
            _checkBoxEnabled.TabIndex = 0;
            _checkBoxEnabled.Text = "nimeを有効にする";
            _checkBoxEnabled.UseVisualStyleBackColor = true;
            // 
            // _checkBoxEnabledDerived
            // 
            _checkBoxEnabledDerived.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            _checkBoxEnabledDerived.AutoSize = true;
            _checkBoxEnabledDerived.Location = new Point(328, 77);
            _checkBoxEnabledDerived.Name = "_checkBoxEnabledDerived";
            _checkBoxEnabledDerived.Size = new Size(15, 14);
            _checkBoxEnabledDerived.TabIndex = 0;
            _checkBoxEnabledDerived.UseVisualStyleBackColor = true;
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
            // _checkBoxInputMethodDerived
            // 
            _checkBoxInputMethodDerived.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            _checkBoxInputMethodDerived.AutoSize = true;
            _checkBoxInputMethodDerived.Location = new Point(328, 113);
            _checkBoxInputMethodDerived.Name = "_checkBoxInputMethodDerived";
            _checkBoxInputMethodDerived.Size = new Size(15, 14);
            _checkBoxInputMethodDerived.TabIndex = 0;
            _checkBoxInputMethodDerived.UseVisualStyleBackColor = true;
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
            // _checkBoxDeleteMethodDerived
            // 
            _checkBoxDeleteMethodDerived.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            _checkBoxDeleteMethodDerived.AutoSize = true;
            _checkBoxDeleteMethodDerived.Location = new Point(328, 144);
            _checkBoxDeleteMethodDerived.Name = "_checkBoxDeleteMethodDerived";
            _checkBoxDeleteMethodDerived.Size = new Size(15, 14);
            _checkBoxDeleteMethodDerived.TabIndex = 0;
            _checkBoxDeleteMethodDerived.UseVisualStyleBackColor = true;
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
            // _comboBoxInputMethod
            // 
            _comboBoxInputMethod.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            _comboBoxInputMethod.DropDownStyle = ComboBoxStyle.DropDownList;
            _comboBoxInputMethod.FormattingEnabled = true;
            _comboBoxInputMethod.Location = new Point(125, 112);
            _comboBoxInputMethod.Name = "_comboBoxInputMethod";
            _comboBoxInputMethod.Size = new Size(184, 23);
            _comboBoxInputMethod.TabIndex = 3;
            _comboBoxInputMethod.SelectedIndexChanged += _comboBoxInputMethod_SelectedIndexChanged;
            // 
            // _comboBoxDeleteMethod
            // 
            _comboBoxDeleteMethod.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            _comboBoxDeleteMethod.DropDownStyle = ComboBoxStyle.DropDownList;
            _comboBoxDeleteMethod.FormattingEnabled = true;
            _comboBoxDeleteMethod.Location = new Point(125, 141);
            _comboBoxDeleteMethod.Name = "_comboBoxDeleteMethod";
            _comboBoxDeleteMethod.Size = new Size(184, 23);
            _comboBoxDeleteMethod.TabIndex = 3;
            _comboBoxDeleteMethod.SelectedIndexChanged += _comboBoxDeleteMethod_SelectedIndexChanged;
            // 
            // _checkBoxIgnoreCaretPositionChange
            // 
            _checkBoxIgnoreCaretPositionChange.AutoSize = true;
            _checkBoxIgnoreCaretPositionChange.Location = new Point(16, 190);
            _checkBoxIgnoreCaretPositionChange.Name = "_checkBoxIgnoreCaretPositionChange";
            _checkBoxIgnoreCaretPositionChange.Size = new Size(227, 19);
            _checkBoxIgnoreCaretPositionChange.TabIndex = 0;
            _checkBoxIgnoreCaretPositionChange.Text = "キャレット位置変化検知時にも変換を実行";
            _checkBoxIgnoreCaretPositionChange.UseVisualStyleBackColor = true;
            // 
            // _checkBoxIgnoreCaretPositionChangeDerived
            // 
            _checkBoxIgnoreCaretPositionChangeDerived.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            _checkBoxIgnoreCaretPositionChangeDerived.AutoSize = true;
            _checkBoxIgnoreCaretPositionChangeDerived.Location = new Point(328, 190);
            _checkBoxIgnoreCaretPositionChangeDerived.Name = "_checkBoxIgnoreCaretPositionChangeDerived";
            _checkBoxIgnoreCaretPositionChangeDerived.Size = new Size(15, 14);
            _checkBoxIgnoreCaretPositionChangeDerived.TabIndex = 0;
            _checkBoxIgnoreCaretPositionChangeDerived.UseVisualStyleBackColor = true;
            // 
            // _comboBoxBase
            // 
            _comboBoxBase.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            _comboBoxBase.DropDownStyle = ComboBoxStyle.DropDownList;
            _comboBoxBase.FormattingEnabled = true;
            _comboBoxBase.Location = new Point(75, 13);
            _comboBoxBase.Name = "_comboBoxBase";
            _comboBoxBase.Size = new Size(268, 23);
            _comboBoxBase.TabIndex = 5;
            _comboBoxBase.DropDown += _comboBoxBase_DropDown;
            _comboBoxBase.SelectedIndexChanged += _comboBoxBase_SelectedIndexChanged;
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
            // _checkBoxVisibleInputNavi
            // 
            _checkBoxVisibleInputNavi.AutoSize = true;
            _checkBoxVisibleInputNavi.Location = new Point(16, 215);
            _checkBoxVisibleInputNavi.Name = "_checkBoxVisibleInputNavi";
            _checkBoxVisibleInputNavi.Size = new Size(154, 19);
            _checkBoxVisibleInputNavi.TabIndex = 0;
            _checkBoxVisibleInputNavi.Text = "入力された文字を表示する";
            _checkBoxVisibleInputNavi.UseVisualStyleBackColor = true;
            // 
            // _checkBoxVisibleInputNaviDerived
            // 
            _checkBoxVisibleInputNaviDerived.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            _checkBoxVisibleInputNaviDerived.AutoSize = true;
            _checkBoxVisibleInputNaviDerived.Location = new Point(328, 215);
            _checkBoxVisibleInputNaviDerived.Name = "_checkBoxVisibleInputNaviDerived";
            _checkBoxVisibleInputNaviDerived.Size = new Size(15, 14);
            _checkBoxVisibleInputNaviDerived.TabIndex = 0;
            _checkBoxVisibleInputNaviDerived.UseVisualStyleBackColor = true;
            // 
            // _checkBoxEnabledInputSuggest
            // 
            _checkBoxEnabledInputSuggest.AutoSize = true;
            _checkBoxEnabledInputSuggest.Location = new Point(16, 240);
            _checkBoxEnabledInputSuggest.Name = "_checkBoxEnabledInputSuggest";
            _checkBoxEnabledInputSuggest.Size = new Size(135, 19);
            _checkBoxEnabledInputSuggest.TabIndex = 0;
            _checkBoxEnabledInputSuggest.Text = "入力補完を有効にする";
            _checkBoxEnabledInputSuggest.UseVisualStyleBackColor = true;
            // 
            // _checkBoxEnabledInputSuggestDerived
            // 
            _checkBoxEnabledInputSuggestDerived.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            _checkBoxEnabledInputSuggestDerived.AutoSize = true;
            _checkBoxEnabledInputSuggestDerived.Location = new Point(328, 240);
            _checkBoxEnabledInputSuggestDerived.Name = "_checkBoxEnabledInputSuggestDerived";
            _checkBoxEnabledInputSuggestDerived.Size = new Size(15, 14);
            _checkBoxEnabledInputSuggestDerived.TabIndex = 0;
            _checkBoxEnabledInputSuggestDerived.UseVisualStyleBackColor = true;
            // 
            // _checkBoxAutoConvertOnInputCommma
            // 
            _checkBoxAutoConvertOnInputCommma.AutoSize = true;
            _checkBoxAutoConvertOnInputCommma.Location = new Point(16, 265);
            _checkBoxAutoConvertOnInputCommma.Name = "_checkBoxAutoConvertOnInputCommma";
            _checkBoxAutoConvertOnInputCommma.Size = new Size(170, 19);
            _checkBoxAutoConvertOnInputCommma.TabIndex = 0;
            _checkBoxAutoConvertOnInputCommma.Text = "\",\"の入力時に変換を実施する";
            _checkBoxAutoConvertOnInputCommma.UseVisualStyleBackColor = true;
            // 
            // _checkBoxAutoConvertOnInputCommmaDerived
            // 
            _checkBoxAutoConvertOnInputCommmaDerived.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            _checkBoxAutoConvertOnInputCommmaDerived.AutoSize = true;
            _checkBoxAutoConvertOnInputCommmaDerived.Location = new Point(328, 265);
            _checkBoxAutoConvertOnInputCommmaDerived.Name = "_checkBoxAutoConvertOnInputCommmaDerived";
            _checkBoxAutoConvertOnInputCommmaDerived.Size = new Size(15, 14);
            _checkBoxAutoConvertOnInputCommmaDerived.TabIndex = 0;
            _checkBoxAutoConvertOnInputCommmaDerived.UseVisualStyleBackColor = true;
            // 
            // _checkBoxAutoConvertOnInputPeriod
            // 
            _checkBoxAutoConvertOnInputPeriod.AutoSize = true;
            _checkBoxAutoConvertOnInputPeriod.Location = new Point(16, 290);
            _checkBoxAutoConvertOnInputPeriod.Name = "_checkBoxAutoConvertOnInputPeriod";
            _checkBoxAutoConvertOnInputPeriod.Size = new Size(170, 19);
            _checkBoxAutoConvertOnInputPeriod.TabIndex = 0;
            _checkBoxAutoConvertOnInputPeriod.Text = "\".\"の入力時に変換を実施する";
            _checkBoxAutoConvertOnInputPeriod.UseVisualStyleBackColor = true;
            // 
            // _checkBoxAutoConvertOnInputPeriodDerived
            // 
            _checkBoxAutoConvertOnInputPeriodDerived.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            _checkBoxAutoConvertOnInputPeriodDerived.AutoSize = true;
            _checkBoxAutoConvertOnInputPeriodDerived.Location = new Point(328, 290);
            _checkBoxAutoConvertOnInputPeriodDerived.Name = "_checkBoxAutoConvertOnInputPeriodDerived";
            _checkBoxAutoConvertOnInputPeriodDerived.Size = new Size(15, 14);
            _checkBoxAutoConvertOnInputPeriodDerived.TabIndex = 0;
            _checkBoxAutoConvertOnInputPeriodDerived.UseVisualStyleBackColor = true;
            // 
            // _checkBoxForceConvertCtrlU
            // 
            _checkBoxForceConvertCtrlU.AutoSize = true;
            _checkBoxForceConvertCtrlU.Location = new Point(16, 332);
            _checkBoxForceConvertCtrlU.Name = "_checkBoxForceConvertCtrlU";
            _checkBoxForceConvertCtrlU.Size = new Size(161, 19);
            _checkBoxForceConvertCtrlU.TabIndex = 0;
            _checkBoxForceConvertCtrlU.Text = "Ctrl+Uでひらがなに変換する";
            _checkBoxForceConvertCtrlU.UseVisualStyleBackColor = true;
            // 
            // _checkBoxForceConvertCtrlUDerived
            // 
            _checkBoxForceConvertCtrlUDerived.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            _checkBoxForceConvertCtrlUDerived.AutoSize = true;
            _checkBoxForceConvertCtrlUDerived.Location = new Point(328, 332);
            _checkBoxForceConvertCtrlUDerived.Name = "_checkBoxForceConvertCtrlUDerived";
            _checkBoxForceConvertCtrlUDerived.Size = new Size(15, 14);
            _checkBoxForceConvertCtrlUDerived.TabIndex = 0;
            _checkBoxForceConvertCtrlUDerived.UseVisualStyleBackColor = true;
            // 
            // _checkBoxForceConvertCtrlI
            // 
            _checkBoxForceConvertCtrlI.AutoSize = true;
            _checkBoxForceConvertCtrlI.Location = new Point(16, 352);
            _checkBoxForceConvertCtrlI.Name = "_checkBoxForceConvertCtrlI";
            _checkBoxForceConvertCtrlI.Size = new Size(153, 19);
            _checkBoxForceConvertCtrlI.TabIndex = 0;
            _checkBoxForceConvertCtrlI.Text = "Ctrl+Iでカタカナに変換する";
            _checkBoxForceConvertCtrlI.UseVisualStyleBackColor = true;
            // 
            // _checkBoxForceConvertCtrlIDerived
            // 
            _checkBoxForceConvertCtrlIDerived.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            _checkBoxForceConvertCtrlIDerived.AutoSize = true;
            _checkBoxForceConvertCtrlIDerived.Location = new Point(328, 352);
            _checkBoxForceConvertCtrlIDerived.Name = "_checkBoxForceConvertCtrlIDerived";
            _checkBoxForceConvertCtrlIDerived.Size = new Size(15, 14);
            _checkBoxForceConvertCtrlIDerived.TabIndex = 0;
            _checkBoxForceConvertCtrlIDerived.UseVisualStyleBackColor = true;
            // 
            // _checkBoxForceConvertCtrlO
            // 
            _checkBoxForceConvertCtrlO.AutoSize = true;
            _checkBoxForceConvertCtrlO.Location = new Point(16, 372);
            _checkBoxForceConvertCtrlO.Name = "_checkBoxForceConvertCtrlO";
            _checkBoxForceConvertCtrlO.Size = new Size(171, 19);
            _checkBoxForceConvertCtrlO.TabIndex = 0;
            _checkBoxForceConvertCtrlO.Text = "Ctrl+Oで半角ｶﾀｶﾅに変換する";
            _checkBoxForceConvertCtrlO.UseVisualStyleBackColor = true;
            // 
            // _checkBoxForceConvertCtrlODerived
            // 
            _checkBoxForceConvertCtrlODerived.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            _checkBoxForceConvertCtrlODerived.AutoSize = true;
            _checkBoxForceConvertCtrlODerived.Location = new Point(328, 372);
            _checkBoxForceConvertCtrlODerived.Name = "_checkBoxForceConvertCtrlODerived";
            _checkBoxForceConvertCtrlODerived.Size = new Size(15, 14);
            _checkBoxForceConvertCtrlODerived.TabIndex = 0;
            _checkBoxForceConvertCtrlODerived.UseVisualStyleBackColor = true;
            // 
            // _checkBoxForceConvertCtrlP
            // 
            _checkBoxForceConvertCtrlP.AutoSize = true;
            _checkBoxForceConvertCtrlP.Location = new Point(16, 392);
            _checkBoxForceConvertCtrlP.Name = "_checkBoxForceConvertCtrlP";
            _checkBoxForceConvertCtrlP.Size = new Size(183, 19);
            _checkBoxForceConvertCtrlP.TabIndex = 0;
            _checkBoxForceConvertCtrlP.Text = "Ctrl+Pで全角ローマ字に変換する";
            _checkBoxForceConvertCtrlP.UseVisualStyleBackColor = true;
            // 
            // _checkBoxForceConvertCtrlPDerived
            // 
            _checkBoxForceConvertCtrlPDerived.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            _checkBoxForceConvertCtrlPDerived.AutoSize = true;
            _checkBoxForceConvertCtrlPDerived.Location = new Point(328, 392);
            _checkBoxForceConvertCtrlPDerived.Name = "_checkBoxForceConvertCtrlPDerived";
            _checkBoxForceConvertCtrlPDerived.Size = new Size(15, 14);
            _checkBoxForceConvertCtrlPDerived.TabIndex = 0;
            _checkBoxForceConvertCtrlPDerived.UseVisualStyleBackColor = true;
            // 
            // _checkBoxForceConvertF6
            // 
            _checkBoxForceConvertF6.AutoSize = true;
            _checkBoxForceConvertF6.Location = new Point(16, 422);
            _checkBoxForceConvertF6.Name = "_checkBoxForceConvertF6";
            _checkBoxForceConvertF6.Size = new Size(139, 19);
            _checkBoxForceConvertF6.TabIndex = 0;
            _checkBoxForceConvertF6.Text = "F6でひらがなに変換する";
            _checkBoxForceConvertF6.UseVisualStyleBackColor = true;
            // 
            // _checkBoxForceConvertF7
            // 
            _checkBoxForceConvertF7.AutoSize = true;
            _checkBoxForceConvertF7.Location = new Point(16, 442);
            _checkBoxForceConvertF7.Name = "_checkBoxForceConvertF7";
            _checkBoxForceConvertF7.Size = new Size(136, 19);
            _checkBoxForceConvertF7.TabIndex = 0;
            _checkBoxForceConvertF7.Text = "F7でカタカナに変換する";
            _checkBoxForceConvertF7.UseVisualStyleBackColor = true;
            // 
            // _checkBoxForceConvertF8
            // 
            _checkBoxForceConvertF8.AutoSize = true;
            _checkBoxForceConvertF8.Location = new Point(16, 462);
            _checkBoxForceConvertF8.Name = "_checkBoxForceConvertF8";
            _checkBoxForceConvertF8.Size = new Size(148, 19);
            _checkBoxForceConvertF8.TabIndex = 0;
            _checkBoxForceConvertF8.Text = "F8で半角ｶﾀｶﾅに変換する";
            _checkBoxForceConvertF8.UseVisualStyleBackColor = true;
            // 
            // _checkBoxForceConvertF9
            // 
            _checkBoxForceConvertF9.AutoSize = true;
            _checkBoxForceConvertF9.Location = new Point(16, 482);
            _checkBoxForceConvertF9.Name = "_checkBoxForceConvertF9";
            _checkBoxForceConvertF9.Size = new Size(162, 19);
            _checkBoxForceConvertF9.TabIndex = 0;
            _checkBoxForceConvertF9.Text = "F9で全角ローマ字に変換する";
            _checkBoxForceConvertF9.UseVisualStyleBackColor = true;
            // 
            // _checkBoxForceConvertF6Derived
            // 
            _checkBoxForceConvertF6Derived.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            _checkBoxForceConvertF6Derived.AutoSize = true;
            _checkBoxForceConvertF6Derived.Location = new Point(328, 422);
            _checkBoxForceConvertF6Derived.Name = "_checkBoxForceConvertF6Derived";
            _checkBoxForceConvertF6Derived.Size = new Size(15, 14);
            _checkBoxForceConvertF6Derived.TabIndex = 0;
            _checkBoxForceConvertF6Derived.UseVisualStyleBackColor = true;
            // 
            // _checkBoxForceConvertF7Derived
            // 
            _checkBoxForceConvertF7Derived.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            _checkBoxForceConvertF7Derived.AutoSize = true;
            _checkBoxForceConvertF7Derived.Location = new Point(328, 442);
            _checkBoxForceConvertF7Derived.Name = "_checkBoxForceConvertF7Derived";
            _checkBoxForceConvertF7Derived.Size = new Size(15, 14);
            _checkBoxForceConvertF7Derived.TabIndex = 0;
            _checkBoxForceConvertF7Derived.UseVisualStyleBackColor = true;
            // 
            // _checkBoxForceConvertF8Derived
            // 
            _checkBoxForceConvertF8Derived.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            _checkBoxForceConvertF8Derived.AutoSize = true;
            _checkBoxForceConvertF8Derived.Location = new Point(328, 462);
            _checkBoxForceConvertF8Derived.Name = "_checkBoxForceConvertF8Derived";
            _checkBoxForceConvertF8Derived.Size = new Size(15, 14);
            _checkBoxForceConvertF8Derived.TabIndex = 0;
            _checkBoxForceConvertF8Derived.UseVisualStyleBackColor = true;
            // 
            // _checkBoxForceConvertF9Derived
            // 
            _checkBoxForceConvertF9Derived.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            _checkBoxForceConvertF9Derived.AutoSize = true;
            _checkBoxForceConvertF9Derived.Location = new Point(328, 482);
            _checkBoxForceConvertF9Derived.Name = "_checkBoxForceConvertF9Derived";
            _checkBoxForceConvertF9Derived.Size = new Size(15, 14);
            _checkBoxForceConvertF9Derived.TabIndex = 0;
            _checkBoxForceConvertF9Derived.UseVisualStyleBackColor = true;
            // 
            // SettingPanelTargetApplication
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(_comboBoxBase);
            Controls.Add(label7);
            Controls.Add(_comboBoxDeleteMethod);
            Controls.Add(_comboBoxInputMethod);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(_checkBoxDeleteMethodDerived);
            Controls.Add(_checkBoxInputMethodDerived);
            Controls.Add(_checkBoxForceConvertF9Derived);
            Controls.Add(_checkBoxForceConvertF8Derived);
            Controls.Add(_checkBoxForceConvertCtrlPDerived);
            Controls.Add(_checkBoxForceConvertF7Derived);
            Controls.Add(_checkBoxForceConvertCtrlODerived);
            Controls.Add(_checkBoxForceConvertF6Derived);
            Controls.Add(_checkBoxForceConvertCtrlIDerived);
            Controls.Add(_checkBoxForceConvertCtrlUDerived);
            Controls.Add(_checkBoxForceConvertF9);
            Controls.Add(_checkBoxAutoConvertOnInputPeriodDerived);
            Controls.Add(_checkBoxForceConvertCtrlP);
            Controls.Add(_checkBoxForceConvertF8);
            Controls.Add(_checkBoxAutoConvertOnInputCommmaDerived);
            Controls.Add(_checkBoxForceConvertCtrlO);
            Controls.Add(_checkBoxForceConvertF7);
            Controls.Add(_checkBoxEnabledInputSuggestDerived);
            Controls.Add(_checkBoxForceConvertCtrlI);
            Controls.Add(_checkBoxForceConvertF6);
            Controls.Add(_checkBoxVisibleInputNaviDerived);
            Controls.Add(_checkBoxForceConvertCtrlU);
            Controls.Add(_checkBoxAutoConvertOnInputPeriod);
            Controls.Add(_checkBoxAutoConvertOnInputCommma);
            Controls.Add(_checkBoxEnabledInputSuggest);
            Controls.Add(_checkBoxIgnoreCaretPositionChangeDerived);
            Controls.Add(_checkBoxVisibleInputNavi);
            Controls.Add(_checkBoxIgnoreCaretPositionChange);
            Controls.Add(_checkBoxEnabledDerived);
            Controls.Add(_checkBoxEnabled);
            Name = "SettingPanelTargetApplication";
            Size = new Size(353, 537);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private CheckBox _checkBoxEnabled;
        private CheckBox _checkBoxEnabledDerived;
        private Label label1;
        private CheckBox _checkBoxInputMethodDerived;
        private Label label2;
        private CheckBox _checkBoxDeleteMethodDerived;
        private Label label3;
        private ComboBox _comboBoxInputMethod;
        private ComboBox _comboBoxDeleteMethod;
        private CheckBox _checkBoxIgnoreCaretPositionChange;
        private CheckBox _checkBoxIgnoreCaretPositionChangeDerived;
        private ComboBox _comboBoxBase;
        private Label label7;
        private CheckBox _checkBoxVisibleInputNavi;
        private CheckBox _checkBoxVisibleInputNaviDerived;
        private CheckBox _checkBoxEnabledInputSuggest;
        private CheckBox _checkBoxEnabledInputSuggestDerived;
        private CheckBox _checkBoxAutoConvertOnInputCommma;
        private CheckBox _checkBoxAutoConvertOnInputCommmaDerived;
        private CheckBox _checkBoxAutoConvertOnInputPeriod;
        private CheckBox _checkBoxAutoConvertOnInputPeriodDerived;
        private CheckBox _checkBoxForceConvertCtrlU;
        private CheckBox _checkBoxForceConvertCtrlUDerived;
        private CheckBox _checkBoxForceConvertCtrlI;
        private CheckBox _checkBoxForceConvertCtrlIDerived;
        private CheckBox _checkBoxForceConvertCtrlO;
        private CheckBox _checkBoxForceConvertCtrlODerived;
        private CheckBox _checkBoxForceConvertCtrlP;
        private CheckBox _checkBoxForceConvertCtrlPDerived;
        private CheckBox _checkBoxForceConvertF6;
        private CheckBox _checkBoxForceConvertF7;
        private CheckBox _checkBoxForceConvertF8;
        private CheckBox _checkBoxForceConvertF9;
        private CheckBox _checkBoxForceConvertF6Derived;
        private CheckBox _checkBoxForceConvertF7Derived;
        private CheckBox _checkBoxForceConvertF8Derived;
        private CheckBox _checkBoxForceConvertF9Derived;
    }
}

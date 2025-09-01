using GoodSeat.Nime.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GoodSeat.Nime.Controls
{
    public partial class SettingPanelTargetApplicationList : UserControl, ISettingPanel
    {
        public SettingPanelTargetApplicationList()
        {
            InitializeComponent();

            NimeMain_TargetWindowChanged(null, NimeMain.TargetWindowInfoMRU);
            NimeMain.TargetWindowChanged += NimeMain_TargetWindowChanged;
        }

        private void NimeMain_TargetWindowChanged(object? sender, Windows.WindowInfo e)
        {
            _textBoxFileNameMRU.Text = e?.FileName;
            _textBoxClassNameMRU.Text = e?.ClassName;
            _textBoxProductNameMRU.Text = e?.ProductName;
            _textBoxTitleMRU.Text = e?.TitleBarText;
        }

        public string TitleOfContents { get => "ターゲットの設定"; }

        Setting TargetSetting { get; set; }

        public void OnCancel(Setting setting)
        {
            throw new NotImplementedException();
        }

        void InitTreeView(Setting setting, ApplicationSetting selectTarget)
        {
            TargetSetting = setting;
            _treeViewTarget.Nodes.Clear();

            var nodeDefault = new TreeNode(ApplicationSetting.DefaultSetting.Name)
            {
                Tag = ApplicationSetting.DefaultSetting
            };

            TreeNode targetNode = null;

            var nodes = setting.AppSettings.Select(s => new TreeNode(s.Name) { Tag = s }).ToList();
            nodes.ForEach(n =>
            {
                var s = n.Tag as ApplicationSetting;
                var np = nodes.FirstOrDefault(np => np.Tag == s.Parent) ?? nodeDefault;
                np.Nodes.Add(n);

                if (s == selectTarget) targetNode = n;
            });

            _treeViewTarget.Nodes.Add(nodeDefault);
            _treeViewTarget.ExpandAll();

            _treeViewTarget.SelectedNode = targetNode ?? nodeDefault;
        }

        public void OnLoading(Setting setting)
        {
            InitTreeView(setting, ApplicationSetting.DefaultSetting);
        }

        public void OnComeback(Setting setting)
        {
            var preSetting = _treeViewTarget.SelectedNode?.Tag as ApplicationSetting;
            InitTreeView(setting, preSetting);
        }

        public void OnOK(Setting setting)
        {
            throw new NotImplementedException();
        }

        bool NowDownloading { get; set; } = false;

        private void _treeViewTarget_AfterSelect(object sender, TreeViewEventArgs e)
        {
            var s = _treeViewTarget.SelectedNode?.Tag as ApplicationSetting;
            if (s == null) return;

            try
            {
                NowDownloading = true;
                _textBoxName.Text = s.Name;
                _comboBoxBase.SelectedItem = s.Parent;
                _textBoxFileName.Text = s.TargetWindow.GetTextOf(WindowIdentifyInfo.PropertyType.FileName);
                _textBoxProductName.Text = s.TargetWindow.GetTextOf(WindowIdentifyInfo.PropertyType.ProductName);
                _textBoxTitle.Text = s.TargetWindow.GetTextOf(WindowIdentifyInfo.PropertyType.TitleBarText);
                _textBoxClassName.Text = s.TargetWindow.GetTextOf(WindowIdentifyInfo.PropertyType.ClassName);
                _comboBoxFileName.SelectedItem = s.TargetWindow.GetMatchTypeOf(WindowIdentifyInfo.PropertyType.FileName);
                _comboBoxProductName.SelectedItem = s.TargetWindow.GetMatchTypeOf(WindowIdentifyInfo.PropertyType.ProductName);
                _comboBoxTitle.SelectedItem = s.TargetWindow.GetMatchTypeOf(WindowIdentifyInfo.PropertyType.TitleBarText);
                _comboBoxClassName.SelectedItem = s.TargetWindow.GetMatchTypeOf(WindowIdentifyInfo.PropertyType.ClassName);
                _checkBoxRegexFileName.Checked = s.TargetWindow.GetUsingRegexIn(WindowIdentifyInfo.PropertyType.FileName);
                _checkBoxRegexProductName.Checked = s.TargetWindow.GetUsingRegexIn(WindowIdentifyInfo.PropertyType.ProductName);
                _checkBoxRegexTitle.Checked = s.TargetWindow.GetUsingRegexIn(WindowIdentifyInfo.PropertyType.TitleBarText);
                _checkBoxRegexClassName.Checked = s.TargetWindow.GetUsingRegexIn(WindowIdentifyInfo.PropertyType.ClassName);

                splitContainer1.Panel2.Enabled = s != ApplicationSetting.DefaultSetting;
            }
            finally
            {
                NowDownloading = false;
            }
        }

        private void _buttonAddTarget_Click(object sender, EventArgs e)
        {
            var s = new ApplicationSetting() { Name = "新しいターゲット" };
            TargetSetting.AppSettings.Add(s);

            var node = new TreeNode(s.Name) { Tag = s };
            _treeViewTarget.Nodes[0].Nodes.Add(node);

            _treeViewTarget.SelectedNode = node;
        }

        private void _buttonDeleteTarget_Click(object sender, EventArgs e)
        {
            if (_treeViewTarget.SelectedNode == null) return;
            if (_treeViewTarget.SelectedNode == _treeViewTarget.Nodes[0]) return; // デフォルトの削除は不可

            var node = _treeViewTarget.SelectedNode;
            var s = node.Tag as ApplicationSetting;
            var sp = node.Parent.Tag as ApplicationSetting;

            var i = node.Parent.Nodes.IndexOf(node);
            node.Parent.Nodes.RemoveAt(i);
            for (var j = 0; j < node.Nodes.Count; j++)
            {
                (node.Nodes[j].Tag as ApplicationSetting).ParentOrg = sp;
                node.Parent.Nodes.Insert(i + j, node.Nodes[j]);
            }
            node.Nodes.Clear();

            TargetSetting.AppSettings.Remove(s);
        }

        void SetTo(Action<ApplicationSetting> action)
        {
            if (NowDownloading) return;

            var s = _treeViewTarget.SelectedNode?.Tag as ApplicationSetting;
            if (s == null) return;

            action(s);
        }

        private void _textBoxName_TextChanged(object sender, EventArgs e)
        {
            SetTo(s =>
            {
                _treeViewTarget.SelectedNode.Text = _textBoxName.Text;
                s.Name = _textBoxName.Text;
            });
        }


        private void _textBoxFileName_TextChanged(object sender, EventArgs e)
        {
            SetTo(s =>
            {
                s.TargetWindow.SetTextOf(WindowIdentifyInfo.PropertyType.FileName, _textBoxFileName.Text);
                s.TargetWindow.SetValidOf(WindowIdentifyInfo.PropertyType.FileName, !string.IsNullOrWhiteSpace(_textBoxFileName.Text));
            });
        }

        private void _comboBoxFileName_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetTo(s => s.TargetWindow.SetMatchTypeOf(WindowIdentifyInfo.PropertyType.FileName, (WindowIdentifyInfo.MatchType)(_comboBoxFileName.SelectedIndex)));
        }

        private void _checkBoxRegexFileName_CheckedChanged(object sender, EventArgs e)
        {
            SetTo(s => s.TargetWindow.SetUsingRegexIn(WindowIdentifyInfo.PropertyType.FileName, _checkBoxRegexFileName.Checked));
        }


        private void _textBoxProductName_TextChanged(object sender, EventArgs e)
        {
            SetTo(s =>
            {
                s.TargetWindow.SetTextOf(WindowIdentifyInfo.PropertyType.ProductName, _textBoxProductName.Text);
                s.TargetWindow.SetValidOf(WindowIdentifyInfo.PropertyType.ProductName, !string.IsNullOrWhiteSpace(_textBoxProductName.Text));
            });
        }

        private void _comboBoxProductName_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetTo(s => s.TargetWindow.SetMatchTypeOf(WindowIdentifyInfo.PropertyType.ProductName, (WindowIdentifyInfo.MatchType)(_comboBoxProductName.SelectedIndex)));
        }

        private void _checkBoxRegexProductName_CheckedChanged(object sender, EventArgs e)
        {
            SetTo(s => s.TargetWindow.SetUsingRegexIn(WindowIdentifyInfo.PropertyType.ProductName, _checkBoxRegexProductName.Checked));
        }

        private void _textBoxTitle_TextChanged(object sender, EventArgs e)
        {
            SetTo(s =>
            {
                s.TargetWindow.SetTextOf(WindowIdentifyInfo.PropertyType.TitleBarText, _textBoxTitle.Text);
                s.TargetWindow.SetValidOf(WindowIdentifyInfo.PropertyType.TitleBarText, !string.IsNullOrWhiteSpace(_textBoxTitle.Text));
            });
        }

        private void _comboBoxTitle_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetTo(s => s.TargetWindow.SetMatchTypeOf(WindowIdentifyInfo.PropertyType.TitleBarText, (WindowIdentifyInfo.MatchType)(_comboBoxTitle.SelectedIndex)));
        }

        private void _checkBoxRegexTitle_CheckedChanged(object sender, EventArgs e)
        {
            SetTo(s => s.TargetWindow.SetUsingRegexIn(WindowIdentifyInfo.PropertyType.TitleBarText, _checkBoxRegexTitle.Checked));
        }

        private void _textBoxClassName_TextChanged(object sender, EventArgs e)
        {
            SetTo(s =>
            {
                s.TargetWindow.SetTextOf(WindowIdentifyInfo.PropertyType.ClassName, _textBoxClassName.Text);
                s.TargetWindow.SetValidOf(WindowIdentifyInfo.PropertyType.ClassName, !string.IsNullOrWhiteSpace(_textBoxClassName.Text));
            });
        }

        private void _comboBoxClassName_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetTo(s => s.TargetWindow.SetMatchTypeOf(WindowIdentifyInfo.PropertyType.ClassName, (WindowIdentifyInfo.MatchType)(_comboBoxClassName.SelectedIndex)));
        }

        private void _checkBoxRegexClassName_CheckedChanged(object sender, EventArgs e)
        {
            SetTo(s => s.TargetWindow.SetUsingRegexIn(WindowIdentifyInfo.PropertyType.ClassName, _checkBoxRegexClassName.Checked));
        }

        private void _btnImportFileName_Click(object sender, EventArgs e)
        {
            _textBoxFileName.Text = _textBoxFileNameMRU.Text;
        }

        private void _btnImportProductName_Click(object sender, EventArgs e)
        {
            _textBoxProductName.Text = _textBoxProductNameMRU.Text;
        }

        private void _btnImportTitle_Click(object sender, EventArgs e)
        {
            _textBoxTitle.Text = _textBoxTitleMRU.Text;
        }

        private void _btnImportClassName_Click(object sender, EventArgs e)
        {
            _textBoxClassName.Text = _textBoxClassNameMRU.Text;
        }
    }
}

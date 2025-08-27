using GoodSeat.Nime.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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

        public void OnLoading(Setting setting)
        {
            TargetSetting = setting;
            _treeViewTarget.Nodes.Clear();

            var nodeDefault = new TreeNode(ApplicationSetting.DefaultSetting.Name)
            {
                Tag = ApplicationSetting.DefaultSetting
            };

            var nodes = setting.AppSettings.Select(s => new TreeNode(s.Name) { Tag = s }).ToList();
            nodes.ForEach(n =>
            {
                var s = n.Tag as ApplicationSetting;
                var np = nodes.FirstOrDefault(np => np.Tag == s.Parent) ?? nodeDefault;
                np.Nodes.Add(n);
            });

            _treeViewTarget.Nodes.Add(nodeDefault);
            _treeViewTarget.ExpandAll();

            _treeViewTarget.SelectedNode = nodeDefault;
        }

        public void OnComeback(Setting setting)
        {
            OnLoading(setting);
        }

        public void OnOK(Setting setting)
        {
            throw new NotImplementedException();
        }


        private void _treeViewTarget_AfterSelect(object sender, TreeViewEventArgs e)
        {
            var s = _treeViewTarget.SelectedNode?.Tag as ApplicationSetting;
            if (s == null) return;

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
            var s  = node.Tag as ApplicationSetting;
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
    }
}

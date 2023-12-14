using GoodSeat.Nime.Controls;
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

namespace GoodSeat.Nime
{
    public partial class SettingForm : Form
    {
        public SettingForm(Setting setting)
        {
            InitializeComponent();

            TargetSetting = setting;
            var treeNodeApplicationSetting = _treeViewContents.Nodes.OfType<TreeNode>().FirstOrDefault(tn => tn.Text == "アプリ毎の設定");
            MakeTreeNodeOfAppSetting(treeNodeApplicationSetting);
            treeNodeApplicationSetting.Tag = new SettingPanelTargetApplicationList();

            _treeViewContents.ExpandAll();

        }

        void MakeTreeNodeOfAppSetting(TreeNode treeParent)
        {
            Func<ApplicationSetting, SettingPanelTargetApplication> makeTag = s => new SettingPanelTargetApplication(s);

            var treeDefault = new TreeNode(ApplicationSetting.DefaultSetting.Name) { Tag = makeTag(ApplicationSetting.DefaultSetting) };
            treeParent.Nodes.Add(treeDefault);

            var lstTreeNodes = TargetSetting.AppSettings.Select(s => new TreeNode(s.Name) { Tag = makeTag(s) });
            foreach (var treeNode in lstTreeNodes)
            {
                var appSetting = (treeNode.Tag as SettingPanelTargetApplication).Target;
                if (appSetting.ParentOrg == null)
                {
                    treeParent.Nodes.Add(treeNode);
                }
                else
                {
                    var tn = lstTreeNodes.FirstOrDefault(t => (t.Tag as SettingPanelTargetApplication).Target == appSetting.Parent);
                    tn.Nodes.Add(treeNode);
                }
            }
        }

        Setting TargetSetting { get; set; }

        List<Control> AlreadyShown { get; set; } = new List<Control>();

        private void _treeViewContents_BeforeCollapse(object sender, TreeViewCancelEventArgs e)
        {
            e.Cancel = true;
        }

        private void _treeViewContents_AfterSelect(object sender, TreeViewEventArgs e)
        {
            var panel = _treeViewContents.SelectedNode.Tag as Control;
            if (panel != null)
            {
                _splitContainer.Panel2.Controls.Clear();
                _splitContainer.Panel2.Controls.Add(panel);
                panel.Dock = DockStyle.Fill;

                if (!AlreadyShown.Contains(panel))
                {
                    AlreadyShown.Add(panel);

                    if (_treeViewContents.SelectedNode.Tag is ISettingPanel settingPanel)
                    {
                        settingPanel.OnLoading(TargetSetting);
                    }
                }
            }

        }
    }
}

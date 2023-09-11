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
        }

        public string TitleOfContents { get => "ターゲットの設定"; }

        public void OnCancel(Setting setting)
        {
            throw new NotImplementedException();
        }

        public void OnLoading(Setting setting)
        {
            throw new NotImplementedException();
        }

        public void OnOK(Setting setting)
        {
            throw new NotImplementedException();
        }
    }
}

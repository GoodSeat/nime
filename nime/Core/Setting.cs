using GoodSeat.Nime.Core.KeySequences;
using GoodSeat.Nime.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodSeat.Nime.Core
{
    public class Setting
    {

        public Setting()
        {
            var vim = new WindowIdentifyInfo();
            vim.SetValidOf(WindowIdentifyInfo.PropertyType.FileName, true);
            vim.SetTextOf(WindowIdentifyInfo.PropertyType.FileName, "vim");

            var forVim = new ApplicationSetting()
            {
                TargetWindow = vim,
                DeleteOrg = new DeleteCurrentByBackspace(),
            };

            AppSettings.Add(forVim);
        }


        internal ApplicationSetting SearchCurrentSetting()
        {
            var wi = WindowInfo.ActiveWindowInfo;
            var s = AppSettings.FirstOrDefault(s => s.TargetWindow.MatchWith(wi));
            return s ?? ApplicationSetting.DefaultSetting;
        }


        internal List<ApplicationSetting> AppSettings { get; set; } = new List<ApplicationSetting>();

    }
}

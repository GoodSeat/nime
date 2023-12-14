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
            vim.SetValidOf(WindowIdentifyInfo.PropertyType.ClassName, true);
            vim.SetTextOf(WindowIdentifyInfo.PropertyType.ClassName, "Vim");
            vim.SetMatchTypeOf(WindowIdentifyInfo.PropertyType.ClassName, WindowIdentifyInfo.MatchType.Contain);
            var forVim = new ApplicationSetting()
            {
                Name = "Vim",
                TargetWindow = vim,
                DeleteOrg = new DeleteCurrentByDeleteAndBackspace(),
                AutoConvertOnInputCommmaOrg = false,
                AutoConvertOnInputPeriodOrg = false,
                UseForceModeOnlyWideRomajiWithCtrlPOrg = false,
                UseForceModeOnlyHiraganaWithCtrlUOrg = false,
                UseForceModeOnlyKatakanaWithCtrlIOrg = false,
                UseForceModeOnlyHalfKatakanaWithCtrlOOrg = false,
            };
            AppSettings.Add(forVim);


            var word = new WindowIdentifyInfo();
            word.SetValidOf(WindowIdentifyInfo.PropertyType.FileName, true);
            word.SetTextOf(WindowIdentifyInfo.PropertyType.FileName, "WINWORD.EXE");
            word.SetMatchTypeOf(WindowIdentifyInfo.PropertyType.FileName, WindowIdentifyInfo.MatchType.Contain);
            var forWord = new ApplicationSetting()
            {
                Name = "Word",
                TargetWindow = word,
                DeleteOrg = new DeleteCurrentBySelectWithDeleteExpectLast(),
                InputOrg = new InputTextByUsingClipboard(),
                IgnoreCaretChangedOrg = true
            };
            AppSettings.Add(forWord);


            var excel = new WindowIdentifyInfo();
            excel.SetValidOf(WindowIdentifyInfo.PropertyType.FileName, true);
            excel.SetTextOf(WindowIdentifyInfo.PropertyType.FileName, "EXCEL.EXE");
            excel.SetMatchTypeOf(WindowIdentifyInfo.PropertyType.FileName, WindowIdentifyInfo.MatchType.Contain);
            var forExcel = new ApplicationSetting()
            {
                Name = "Excel",
                TargetWindow = excel,
                DeleteOrg = new DeleteCurrentByDeleteAndBackspace(),
                //ParentOrg = forVim,
            };
            AppSettings.Add(forExcel);
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

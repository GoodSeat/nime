using GoodSeat.Nime.Core.KeySequences;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodSeat.Nime.Core
{
    internal class ApplicationSetting
    {
        public static ApplicationSetting DefaultSetting { get; set; }

        static ApplicationSetting()
        {
            DefaultSetting = new ApplicationSetting()
            {
                EnabledOrg = true,
                InputOrg = new InputTextBySendInput(),
                DeleteOrg = new DeleteCurrentBySelectWithDelete(),
                IgnoreCaretChangedOrg = false,
                VisibleInputViewOrg = true,
                VisibleInputSuggstionOrg = true,
                AutoConvertOnInputCommmaOrg = true,
                AutoConvertOnInputPeriodOrg = true,
                UseForceModeOnlyHiraganaWithCtrlUOrg = true,
                UseForceModeOnlyKatakanaWithCtrlIOrg = true,
                UseForceModeOnlyHalfKatakanaWithCtrlOOrg = true,
                UseForceModeOnlyWideRomajiWithCtrlPOrg = true,
                UseForceModeOnlyHiraganaWithF6Org = true,
                UseForceModeOnlyKatakanaWithF7Org = true,
                UseForceModeOnlyHalfKatakanaWithF8Org = true,
                UseForceModeOnlyWideRomajiWithF9Org = true,
            };
        }


        /// <summary>
        /// 対象とするウインドウ情報を設定もしくは取得します。
        /// </summary>
        public WindowIdentifyInfo TargetWindow { get; set; } = new WindowIdentifyInfo(); 

        /// <summary>
        /// 設定を継承するアプリ設定を設定もしくは取得します。
        /// </summary>
        public ApplicationSetting Parent { get => ParentOrg ?? DefaultSetting; }
        public ApplicationSetting? ParentOrg { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool Enabled { get => EnabledOrg ?? Parent.Enabled; }

        public DeleteCurrent Delete { get => DeleteOrg ?? Parent.Delete; }

        public InputText Input { get => InputOrg ?? Parent.Input; }
        /// <summary>
        /// キャレット位置変化の検知を無視して変換を実行するか否かを設定もしくは取得します。
        /// </summary>
        public bool IgnoreCaretChanged { get => IgnoreCaretChangedOrg ?? Parent.IgnoreCaretChanged; }
        public bool VisibleInputView { get => VisibleInputViewOrg ?? Parent.VisibleInputView; }
        public bool VisibleInputSuggstion { get => VisibleInputSuggstionOrg ?? Parent.VisibleInputSuggstion; }
        public bool AutoConvertOnInputCommma { get => AutoConvertOnInputCommmaOrg ?? Parent.AutoConvertOnInputCommma; }
        public bool AutoConvertOnInputPeriod { get => AutoConvertOnInputPeriodOrg ?? Parent.AutoConvertOnInputPeriod; }

        public bool UseForceModeOnlyHiraganaWithCtrlU { get => UseForceModeOnlyHiraganaWithCtrlUOrg ?? Parent.UseForceModeOnlyHiraganaWithCtrlU; }
        public bool UseForceModeOnlyKatakanaWithCtrlI { get => UseForceModeOnlyKatakanaWithCtrlIOrg ?? Parent.UseForceModeOnlyKatakanaWithCtrlI; }
        public bool UseForceModeOnlyHalfKatakanaWithCtrlO { get => UseForceModeOnlyHalfKatakanaWithCtrlOOrg ?? Parent.UseForceModeOnlyHalfKatakanaWithCtrlO; }
        public bool UseForceModeOnlyWideRomajiWithCtrlP { get => UseForceModeOnlyWideRomajiWithCtrlPOrg ?? Parent.UseForceModeOnlyWideRomajiWithCtrlP; }
        public bool UseForceModeOnlyHiraganaWithF6 { get => UseForceModeOnlyHiraganaWithF6Org ?? Parent.UseForceModeOnlyHiraganaWithF6; }
        public bool UseForceModeOnlyKatakanaWithF7 { get => UseForceModeOnlyKatakanaWithF7Org ?? Parent.UseForceModeOnlyKatakanaWithF7; }
        public bool UseForceModeOnlyHalfKatakanaWithF8 { get => UseForceModeOnlyHalfKatakanaWithF8Org ?? Parent.UseForceModeOnlyHalfKatakanaWithF8; }
        public bool UseForceModeOnlyWideRomajiWithF9 { get => UseForceModeOnlyWideRomajiWithF9Org ?? Parent.UseForceModeOnlyWideRomajiWithF9; }




        public bool? EnabledOrg { get; set; }

        public DeleteCurrent? DeleteOrg { get; set; }

        public InputText? InputOrg { get; set; }

        /// <summary>
        /// キャレット位置変化の検知を無視して変換を実行するか否かを設定もしくは取得します。
        /// </summary>
        public bool? IgnoreCaretChangedOrg { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool? VisibleInputViewOrg { get; set; }
        public bool? VisibleInputSuggstionOrg { get; set; }
        public bool? AutoConvertOnInputCommmaOrg { get; set; }
        public bool? AutoConvertOnInputPeriodOrg { get; set; }

        public bool? UseForceModeOnlyHiraganaWithCtrlUOrg { get; set; }
        public bool? UseForceModeOnlyKatakanaWithCtrlIOrg { get; set; }
        public bool? UseForceModeOnlyHalfKatakanaWithCtrlOOrg { get; set; }
        public bool? UseForceModeOnlyWideRomajiWithCtrlPOrg { get; set; }
        public bool? UseForceModeOnlyHiraganaWithF6Org { get; set; }
        public bool? UseForceModeOnlyKatakanaWithF7Org { get; set; }
        public bool? UseForceModeOnlyHalfKatakanaWithF8Org { get; set; }
        public bool? UseForceModeOnlyWideRomajiWithF9Org { get; set; }

    }
}

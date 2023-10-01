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
                IgnoreCaretChanged = false,
                VisibleInputViewOrg = true,
                VisibleInputSuggstionOrg = true,
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




        public bool? EnabledOrg { get; set; }

        public DeleteCurrent? DeleteOrg { get; set; }

        public InputText? InputOrg { get; set; }

        /// <summary>
        /// キャレット位置変化の検知を無視して変換を実行するか否かを設定もしくは取得します。
        /// </summary>
        public bool? IgnoreCaretChanged { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool? VisibleInputViewOrg { get; set; }
        public bool? VisibleInputSuggstionOrg { get; set; }

    }
}

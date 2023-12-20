using GoodSeat.Nime.Core.KeySequences;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GoodSeat.Nime.Core
{
    /// <summary>
    /// ターゲットごとの設定情報を表します。
    /// </summary>
    internal class ApplicationSetting
    {
        /// <summary>
        /// 規定の設定情報を設定もしくは取得します。
        /// </summary>
        public static ApplicationSetting DefaultSetting { get; set; }

        static ApplicationSetting()
        {
            DefaultSetting = new ApplicationSetting()
            {
                Name = "デフォルト",
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
        /// この設定を識別する名称を設定もしくは取得します。
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 対象とするウインドウ情報を設定もしくは取得します。
        /// </summary>
        public WindowIdentifyInfo TargetWindow { get; set; } = new WindowIdentifyInfo(); 


        /// <summary>
        /// 設定を継承するアプリ設定を設定もしくは取得します。
        /// </summary>
        public ApplicationSetting Parent { get => ParentOrg ?? DefaultSetting; }


        /// <summary>
        /// nimeを有効にするか否かを取得します。
        /// </summary>
        public bool Enabled { get => EnabledOrg ?? Parent.Enabled; }

        /// <summary>
        /// テキストの削除方法を取得します。
        /// </summary>
        public DeleteCurrent Delete { get => DeleteOrg ?? Parent.Delete; }
        /// <summary>
        /// テキストの入力方法を取得します。
        /// </summary>
        public InputText Input { get => InputOrg ?? Parent.Input; }
        /// <summary>
        /// キャレット位置変化の検知を無視して変換を実行するか否かを取得します。
        /// </summary>
        public bool IgnoreCaretChanged { get => IgnoreCaretChangedOrg ?? Parent.IgnoreCaretChanged; }
        /// <summary>
        /// 入力表示の可視を取得します。
        /// </summary>
        public bool VisibleInputView { get => VisibleInputViewOrg ?? Parent.VisibleInputView; }
        /// <summary>
        /// 入力補完の有効可否を取得します。
        /// </summary>
        public bool VisibleInputSuggstion { get => VisibleInputSuggstionOrg ?? Parent.VisibleInputSuggstion; }
        /// <summary>
        /// ","の入力時に、自動変換を実施するか否かを取得します。
        /// </summary>
        public bool AutoConvertOnInputCommma { get => AutoConvertOnInputCommmaOrg ?? Parent.AutoConvertOnInputCommma; }
        /// <summary>
        /// "."の入力時に、自動変換を実施するか否かを取得します。
        /// </summary>
        public bool AutoConvertOnInputPeriod { get => AutoConvertOnInputPeriodOrg ?? Parent.AutoConvertOnInputPeriod; }

        /// <summary>
        /// Ctrl+Uの押下でひらがなへの変換を実施するか否かを取得します。
        /// </summary>
        public bool UseForceModeOnlyHiraganaWithCtrlU { get => UseForceModeOnlyHiraganaWithCtrlUOrg ?? Parent.UseForceModeOnlyHiraganaWithCtrlU; }
        /// <summary>
        /// Ctrl+Iの押下でカタカナへの変換を実施するか否かを取得します。
        /// </summary>
        public bool UseForceModeOnlyKatakanaWithCtrlI { get => UseForceModeOnlyKatakanaWithCtrlIOrg ?? Parent.UseForceModeOnlyKatakanaWithCtrlI; }
        /// <summary>
        /// Ctrl+Oの押下で半角カタカナへの変換を実施するか否かを取得します。
        /// </summary>
        public bool UseForceModeOnlyHalfKatakanaWithCtrlO { get => UseForceModeOnlyHalfKatakanaWithCtrlOOrg ?? Parent.UseForceModeOnlyHalfKatakanaWithCtrlO; }
        /// <summary>
        /// Ctrl+Pの押下で全角ローマ字への変換を実施するか否かを取得します。
        /// </summary>
        public bool UseForceModeOnlyWideRomajiWithCtrlP { get => UseForceModeOnlyWideRomajiWithCtrlPOrg ?? Parent.UseForceModeOnlyWideRomajiWithCtrlP; }
        /// <summary>
        /// F6の押下でひらがなへの変換を実施するか否かを取得します。
        /// </summary>
        public bool UseForceModeOnlyHiraganaWithF6 { get => UseForceModeOnlyHiraganaWithF6Org ?? Parent.UseForceModeOnlyHiraganaWithF6; }
        /// <summary>
        /// F7の押下でカタカナへの変換を実施するか否かを取得します。
        /// </summary>
        public bool UseForceModeOnlyKatakanaWithF7 { get => UseForceModeOnlyKatakanaWithF7Org ?? Parent.UseForceModeOnlyKatakanaWithF7; }
        /// <summary>
        /// F8の押下で半角カタカナへの変換を実施するか否かを取得します。
        /// </summary>
        public bool UseForceModeOnlyHalfKatakanaWithF8 { get => UseForceModeOnlyHalfKatakanaWithF8Org ?? Parent.UseForceModeOnlyHalfKatakanaWithF8; }
        /// <summary>
        /// F9の押下で全角ローマ字への変換を実施するか否かを取得します。
        /// </summary>
        public bool UseForceModeOnlyWideRomajiWithF9 { get => UseForceModeOnlyWideRomajiWithF9Org ?? Parent.UseForceModeOnlyWideRomajiWithF9; }



        public ApplicationSetting? ParentOrg { get; set; }

        public bool? EnabledOrg { get; set; }

        public DeleteCurrent? DeleteOrg { get; set; } = null;

        public InputText? InputOrg { get; set; } = null;

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


        /// <summary>
        /// 指定のJsonエレメントデータから設定を復元します。
        /// </summary>
        /// <param name="data">復元元とするJsonエレメント。</param>
        public void Deserialize(JsonElement data)
        {
            Name = data.GetProperty(nameof(Name)).GetString();

            var target = data.GetProperty(nameof(TargetWindow));
            foreach (var type in Enum.GetValues(typeof(WindowIdentifyInfo.PropertyType)).OfType<WindowIdentifyInfo.PropertyType>())
            {
                var targetType = target.GetProperty(type.ToString());
                TargetWindow.SetMatchTypeOf(type, (WindowIdentifyInfo.MatchType)targetType.GetProperty(nameof(TargetWindow.GetMatchTypeOf)).GetInt32());
                TargetWindow.SetTextOf(type, targetType.GetProperty(nameof(TargetWindow.GetTextOf)).GetString());
                TargetWindow.SetUsingRegexIn(type, (bool)targetType.GetProperty(nameof(TargetWindow.GetUsingRegexIn)).GetBoolean());
                TargetWindow.SetValidOf(type, (bool)targetType.GetProperty(nameof(TargetWindow.GetValidOf)).GetBoolean());
            }

            // MEMO:Parentの設定は呼び出し側で実施

            JsonElement json;
            if (data.TryGetProperty(nameof(EnabledOrg), out json)) EnabledOrg = json.GetBoolean();
            if (data.TryGetProperty(nameof(DeleteOrg), out json)) DeleteOrg = DeleteCurrent.CreateByName(json.GetString());
            if (data.TryGetProperty(nameof(InputOrg), out json)) InputOrg = InputText.CreateByName(json.GetString());

            if (data.TryGetProperty(nameof(IgnoreCaretChangedOrg), out json)) IgnoreCaretChangedOrg = json.GetBoolean();
            if (data.TryGetProperty(nameof(VisibleInputViewOrg), out json)) VisibleInputViewOrg = json.GetBoolean();
            if (data.TryGetProperty(nameof(VisibleInputSuggstionOrg), out json)) VisibleInputSuggstionOrg = json.GetBoolean();
            if (data.TryGetProperty(nameof(AutoConvertOnInputCommmaOrg), out json)) AutoConvertOnInputCommmaOrg = json.GetBoolean();
            if (data.TryGetProperty(nameof(AutoConvertOnInputPeriodOrg), out json)) AutoConvertOnInputPeriodOrg = json.GetBoolean();

            if (data.TryGetProperty(nameof(UseForceModeOnlyHiraganaWithCtrlUOrg), out json)) UseForceModeOnlyHiraganaWithCtrlUOrg = json.GetBoolean();
            if (data.TryGetProperty(nameof(UseForceModeOnlyKatakanaWithCtrlIOrg), out json)) UseForceModeOnlyKatakanaWithCtrlIOrg = json.GetBoolean();
            if (data.TryGetProperty(nameof(UseForceModeOnlyHalfKatakanaWithCtrlOOrg), out json)) UseForceModeOnlyHalfKatakanaWithCtrlOOrg = json.GetBoolean();
            if (data.TryGetProperty(nameof(UseForceModeOnlyWideRomajiWithCtrlPOrg), out json)) UseForceModeOnlyWideRomajiWithCtrlPOrg = json.GetBoolean();
            if (data.TryGetProperty(nameof(UseForceModeOnlyHiraganaWithF6Org), out json)) UseForceModeOnlyHiraganaWithF6Org = json.GetBoolean();
            if (data.TryGetProperty(nameof(UseForceModeOnlyKatakanaWithF7Org), out json)) UseForceModeOnlyKatakanaWithF7Org = json.GetBoolean();
            if (data.TryGetProperty(nameof(UseForceModeOnlyHalfKatakanaWithF8Org), out json)) UseForceModeOnlyHalfKatakanaWithF8Org = json.GetBoolean();
            if (data.TryGetProperty(nameof(UseForceModeOnlyWideRomajiWithF9Org), out json)) UseForceModeOnlyWideRomajiWithF9Org = json.GetBoolean();

        }

        /// <summary>
        /// 設定を保存したJsonオブジェクトデータを生成して取得します。
        /// </summary>
        public Dictionary<string, object> Serialize()
        {
            var data = new Dictionary<string, object>();

            data.Add(nameof(Name), Name);

            var target = new Dictionary<string, object>();
            data.Add(nameof(TargetWindow), target);
            foreach (var type in Enum.GetValues(typeof(WindowIdentifyInfo.PropertyType)).OfType<WindowIdentifyInfo.PropertyType>())
            {
                var targetType = new Dictionary<string, object>();
                target.Add(type.ToString(), targetType);
                targetType.Add(nameof(TargetWindow.GetMatchTypeOf), TargetWindow.GetMatchTypeOf(type));
                targetType.Add(nameof(TargetWindow.GetTextOf), TargetWindow.GetTextOf(type));
                targetType.Add(nameof(TargetWindow.GetUsingRegexIn), TargetWindow.GetUsingRegexIn(type));
                targetType.Add(nameof(TargetWindow.GetValidOf), TargetWindow.GetValidOf(type));
            }

            if (ParentOrg != null) data.Add(nameof(ParentOrg), ParentOrg.Name);
            if (EnabledOrg != null) data.Add(nameof(EnabledOrg), EnabledOrg);
            if (DeleteOrg != null) data.Add(nameof(DeleteOrg), DeleteOrg.GetType().Name);
            if (InputOrg != null) data.Add(nameof(InputOrg), InputOrg.GetType().Name);

            if (IgnoreCaretChangedOrg != null) data.Add(nameof(IgnoreCaretChangedOrg), IgnoreCaretChangedOrg);
            if (VisibleInputViewOrg != null) data.Add(nameof(VisibleInputViewOrg), VisibleInputViewOrg);
            if (VisibleInputSuggstionOrg != null) data.Add(nameof(VisibleInputSuggstionOrg), VisibleInputSuggstionOrg);
            if (AutoConvertOnInputCommmaOrg != null) data.Add(nameof(AutoConvertOnInputCommmaOrg), AutoConvertOnInputCommmaOrg);
            if (AutoConvertOnInputPeriodOrg != null) data.Add(nameof(AutoConvertOnInputPeriodOrg), AutoConvertOnInputPeriodOrg);

            if (UseForceModeOnlyHiraganaWithCtrlUOrg != null) data.Add(nameof(UseForceModeOnlyHiraganaWithCtrlUOrg), UseForceModeOnlyHiraganaWithCtrlUOrg);
            if (UseForceModeOnlyKatakanaWithCtrlIOrg != null) data.Add(nameof(UseForceModeOnlyKatakanaWithCtrlIOrg), UseForceModeOnlyKatakanaWithCtrlIOrg);
            if (UseForceModeOnlyHalfKatakanaWithCtrlOOrg != null) data.Add(nameof(UseForceModeOnlyHalfKatakanaWithCtrlOOrg), UseForceModeOnlyHalfKatakanaWithCtrlOOrg);
            if (UseForceModeOnlyWideRomajiWithCtrlPOrg != null) data.Add(nameof(UseForceModeOnlyWideRomajiWithCtrlPOrg), UseForceModeOnlyWideRomajiWithCtrlPOrg);
            if (UseForceModeOnlyHiraganaWithF6Org != null) data.Add(nameof(UseForceModeOnlyHiraganaWithF6Org), UseForceModeOnlyHiraganaWithF6Org);
            if (UseForceModeOnlyKatakanaWithF7Org != null) data.Add(nameof(UseForceModeOnlyKatakanaWithF7Org), UseForceModeOnlyKatakanaWithF7Org);
            if (UseForceModeOnlyHalfKatakanaWithF8Org != null) data.Add(nameof(UseForceModeOnlyHalfKatakanaWithF8Org), UseForceModeOnlyHalfKatakanaWithF8Org);
            if (UseForceModeOnlyWideRomajiWithF9Org != null) data.Add(nameof(UseForceModeOnlyWideRomajiWithF9Org), UseForceModeOnlyWideRomajiWithF9Org);

            return data;
        }

    }
}

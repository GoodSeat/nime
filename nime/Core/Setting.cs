using GoodSeat.Nime.Core.KeySequences;
using GoodSeat.Nime.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GoodSeat.Nime.Core
{
    /// <summary>
    /// nimeの動作設定を表します。
    /// </summary>
    public class Setting
    {
        /// <summary>
        /// nimeの動作設定を初期化します。
        /// </summary>
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
                //DeleteOrg = new DeleteCurrentByDeleteAndBackspace(),
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
                //DeleteOrg = new DeleteCurrentBySelectWithDeleteExpectLast(),
                //InputOrg = new InputTextByUsingClipboard(),
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
                //DeleteOrg = new DeleteCurrentByDeleteAndBackspace(),
                //ParentOrg = forVim,
            };
            AppSettings.Add(forExcel);
        }


        /// <summary>
        /// 現在アクティブなアプリケーション設定を探索して取得します。
        /// </summary>
        /// <returns>対象となるアプリケーション設定。</returns>
        internal ApplicationSetting SearchCurrentSetting()
        {
            var wi = WindowInfo.ActiveWindowInfo;
            var s = AppSettings.FirstOrDefault(s => s.TargetWindow.MatchWith(wi));
            return s ?? ApplicationSetting.DefaultSetting;
        }


        /// <summary>
        /// アプリケーション設定リスト(既定に対する設定を除く)を設定もしくは取得します。
        /// </summary>
        internal List<ApplicationSetting> AppSettings { get; set; } = new List<ApplicationSetting>();



        /// <summary>
        /// 指定文字列がいずれかの操作キーワードに合致するか否かを判定します。但し、<see cref="EnableOperateByKeyword"/>がfalseの場合は常にfalseを返します。
        /// </summary>
        /// <param name="text">判定対象文字列。</param>
        /// <returns>指定文字列がいずれかの操作キーワードに合致するか否か。</returns>
        public bool IsOperateKeyword(string text)
        {
            if (!EnableOperateByKeyword || string.IsNullOrEmpty(text)) return false;
            return text == KeywordExit || text == KeywordStop || text == KeywordStart || text == KeywordVisible || text == KeywordSupport || text == KeywordSetting;
        }

        /// <summary>
        /// 操作キーワードによる操作を有効にするか否かを設定もしくは取得します。
        /// </summary>
        public bool EnableOperateByKeyword { get; set; } = true;
        /// <summary>
        /// 操作キーワードによりnimeを終了する場合に、挨拶を行うか否かを設定もしくは取得します。
        /// </summary>
        public bool SayGoodByeWhenExitByKeyword { get; set; } = true;

        /// <summary>
        /// nimeを終了するための操作キーワードを設定もしくは取得します。
        /// </summary>
        public string KeywordExit { get; set; } = "nQ";
        /// <summary>
        /// nimeを一時停止するための操作キーワードを設定もしくは取得します。
        /// </summary>
        public string KeywordStop { get; set; } = "nS";
        /// <summary>
        /// nimeを再開するための操作キーワードを設定もしくは取得します。
        /// </summary>
        public string KeywordStart { get; set; } = "nS";
        /// <summary>
        /// nimeの入力表示トグルの操作キーワードを設定もしくは取得します。
        /// </summary>
        public string KeywordVisible { get; set; } = "nV";
        /// <summary>
        /// nimeの入力補完トグルの操作キーワードを設定もしくは取得します。
        /// </summary>
        public string KeywordSupport { get; set; } = "nI";
        /// <summary>
        /// nimeの設定画面表示の操作キーワードを設定もしくは取得します。
        /// </summary>
        public string KeywordSetting { get; set; } = "nO";



        /// <summary>
        /// 指定のJsonオブジェクトデータから設定を復元します。
        /// </summary>
        /// <param name="data">復元元とするJsonオブジェクト。</param>
        public void Deserialize(Dictionary<string, JsonElement> data)
        {
            var settingVersion = new JsonElement();
            if (data.TryGetValue("SettingVersion", out settingVersion))
            {
            }

            if (data.ContainsKey(nameof(KeywordExit))) KeywordExit = data[nameof(KeywordExit)].GetString();
            if (data.ContainsKey(nameof(KeywordStop))) KeywordStop = data[nameof(KeywordStop)].GetString();
            if (data.ContainsKey(nameof(KeywordStart))) KeywordStart = data[nameof(KeywordStart)].GetString();
            if (data.ContainsKey(nameof(KeywordVisible))) KeywordVisible = data[nameof(KeywordVisible)].GetString();
            if (data.ContainsKey(nameof(KeywordSupport))) KeywordSupport = data[nameof(KeywordSupport)].GetString();
            if (data.ContainsKey(nameof(KeywordSetting))) KeywordSetting = data[nameof(KeywordSetting)].GetString();

            ApplicationSetting.DefaultSetting.Deserialize(data[nameof(ApplicationSetting.DefaultSetting)]);

            AppSettings.Clear();
            if (data.ContainsKey(nameof(AppSettings)))
            {
                var appSettingDatas = data[nameof(AppSettings)].EnumerateArray();
                var parent = new Dictionary<ApplicationSetting, string>();
                var apps = new Dictionary<string, ApplicationSetting>();
                foreach (var appSettingData in appSettingDatas)
                {
                    var appSetting = new ApplicationSetting();
                    appSetting.Deserialize(appSettingData);
                    AppSettings.Add(appSetting);

                    if (appSettingData.TryGetProperty(nameof(ApplicationSetting.ParentOrg), out var value))
                    {
                        parent.Add(appSetting, value.GetString());
                    }
                    apps.Add(appSettingData.GetProperty(nameof(ApplicationSetting.GUID)).GetString(), appSetting);
                }

                // Parentの設定
                foreach (var appSetting in AppSettings)
                {
                    if (!parent.ContainsKey(appSetting)) continue;
                    var guid = parent[appSetting];
                    appSetting.ParentOrg = apps[guid];
                }
            }

        }

        /// <summary>
        /// 設定を保存したJsonオブジェクトデータを生成して取得します。
        /// </summary>
        public Dictionary<string, object> Serialize()
        {
            var data = new Dictionary<string, object>();
            data.Add("SettingVersion", "1.0");
            data.Add(nameof(KeywordExit), KeywordExit);
            data.Add(nameof(KeywordStop), KeywordStop);
            data.Add(nameof(KeywordStart), KeywordStart);
            data.Add(nameof(KeywordVisible), KeywordVisible);
            data.Add(nameof(KeywordSupport), KeywordSupport);
            data.Add(nameof(KeywordSetting), KeywordSetting);

            data.Add(nameof(ApplicationSetting.DefaultSetting), ApplicationSetting.DefaultSetting.Serialize());

            var appSettingDatas = new List<Dictionary<string, object>>();
            foreach (var appSetting in AppSettings)
            {
                appSettingDatas.Add(appSetting.Serialize());
            }
            data.Add(nameof(AppSettings), appSettingDatas);


            return data;
        }

    }
}

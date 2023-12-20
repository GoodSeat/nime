using GoodSeat.Nime.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GoodSeat.Nime.Core
{
    /// <summary>
    /// ウインドウを識別する情報を表します。
    /// </summary>
    public class WindowIdentifyInfo
    {
        /// <summary>
        /// ウインドウ情報の文字列属性を表します。
        /// </summary>
        public enum PropertyType
        {
            /// <summary>タイトル。</summary>
            TitleBarText,
            /// <summary>製品名。</summary>
            ProductName,
            /// <summary>ファイル名。</summary>
            FileName,
            /// <summary>クラス名。</summary>
            ClassName
        }

        /// <summary>
        /// 文字列の一致判定方法を現します。
        /// </summary>
        public enum MatchType
        {
            /// <summary>部分一致。</summary>
            Contain,
            /// <summary>完全一致。</summary>
            Match
        }

        private Dictionary<PropertyType, string?> TextMap { get; set; }
        private Dictionary<PropertyType, bool> UseRegexMap { get; set; }
        private Dictionary<PropertyType, bool> ValidMap { get; set; }
        private Dictionary<PropertyType, MatchType> MatchMap { get; set; }

        private Dictionary<PropertyType, Regex?> RegexMap { get; set; }

        /// <summary>
        /// ウインドウを識別する情報を初期化します。
        /// </summary>
        public WindowIdentifyInfo()
        {
            TextMap = new Dictionary<PropertyType, string?>();
            RegexMap = new Dictionary<PropertyType, Regex?>();
            UseRegexMap = new Dictionary<PropertyType, bool>();
            ValidMap = new Dictionary<PropertyType, bool>();
            MatchMap = new Dictionary<PropertyType, MatchType>();

            for (PropertyType type = PropertyType.TitleBarText; type <= PropertyType.ClassName; type++)
            {
                TextMap.Add(type, null);
                RegexMap.Add(type, null);
                UseRegexMap.Add(type, false);
                ValidMap.Add(type, false);
                MatchMap.Add(type, MatchType.Contain);
            }
        }

        /// <summary>
        /// ウインドウを識別する情報を初期化します。
        /// </summary>
        /// <param name="baseInfo">初期化基準とするウインドウ識別情報。</param>
        public WindowIdentifyInfo(WindowIdentifyInfo baseInfo)
            : this()
        {
            for (PropertyType type = PropertyType.TitleBarText; type <= PropertyType.ClassName; type++)
            {
                TextMap[type] = baseInfo.TextMap[type];
                RegexMap[type] = baseInfo.RegexMap[type];
                UseRegexMap[type] = baseInfo.UseRegexMap[type];
                ValidMap[type] = baseInfo.ValidMap[type];
                MatchMap[type] = baseInfo.MatchMap[type];
            }
        }


        /// <summary>
        /// 指定属性の文字列を指定します。
        /// </summary>
        /// <param name="type">指定対象とする属性タイプ。</param>
        /// <param name="text">指定する文字列。</param>
        public void SetTextOf(PropertyType type, string text)
        {
            if (text == TextMap[type]) return;
            TextMap[type] = text;
            RegexMap[type] = null;
        }

        /// <summary>
        /// 指定属性の文字列を取得します。
        /// </summary>
        /// <param name="type">指定対象とする属性タイプ。</param>
        public string? GetTextOf(PropertyType type) { return TextMap[type]; }

        /// <summary>
        /// 指定属性の文字列を正規表現として解釈するか否かを指定します。
        /// </summary>
        /// <param name="type">指定対象とする属性タイプ。</param>
        /// <param name="asRegex">正規表現として扱うか否か。</param>
        public void SetUsingRegexIn(PropertyType type, bool asRegex) { UseRegexMap[type] = asRegex; }

        /// <summary>
        /// 指定属性の文字列を正規表現として解釈するか否かを取得します。
        /// </summary>
        /// <param name="type">指定対象とする属性タイプ。</param>
        public bool GetUsingRegexIn(PropertyType type) { return UseRegexMap[type]; }

        /// <summary>
        /// 指定属性を判定対象とするか否かを指定します。
        /// </summary>
        /// <param name="type">指定対象とする属性タイプ。</param>
        /// <param name="use">使用有無。</param>
        public void SetValidOf(PropertyType type, bool use) { ValidMap[type] = use; }

        /// <summary>
        /// 指定属性を判定対象とするか否かを取得します。
        /// </summary>
        /// <param name="type">指定対象とする属性タイプ。</param>
        public bool GetValidOf(PropertyType type) { return ValidMap[type]; }


        /// <summary>
        /// 指定文字列を検査する正規表現を取得します。
        /// </summary>
        /// <param name="type">指定対象とする属性タイプ。</param>
        /// <returns>検査用の正規表現。</returns>
        Regex? GetRegexOf(PropertyType type)
        {
            if (RegexMap[type] != null) return RegexMap[type];

            RegexMap[type] = new Regex(GetTextOf(type));
            return RegexMap[type];
        }

        /// <summary>
        /// 指定属性の判定方法を取得します。
        /// </summary>
        /// <param name="type"></param>
        public void SetMatchTypeOf(PropertyType type, MatchType matchType) { MatchMap[type] = matchType; }

        /// <summary>
        /// 指定属性の判定方法を取得します。
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public MatchType GetMatchTypeOf(PropertyType type) { return MatchMap[type]; }

        /// <summary>
        /// ウインドウ情報から指定の属性タイプの文字列を取得します。
        /// </summary>
        /// <param name="windowInfo">取得対象とするウインドウ情報。</param>
        /// <param name="type">指定対象とする属性タイプ。</param>
        /// <returns>ウインドウ情報から取得された文字列。</returns>
        string GetTextFromWindowInfoOf(WindowInfo windowInfo, PropertyType type)
        {
            switch (type)
            {
                case PropertyType.TitleBarText: return windowInfo.TitleBarText;
                case PropertyType.ProductName: return windowInfo.ProductName;
                case PropertyType.FileName: return windowInfo.FileName;
                case PropertyType.ClassName: return windowInfo.ClassName;
            }
            throw new NotImplementedException();
        }

        /// <summary>
        /// 指定のウインドウ情報が、この識別情報に合致するか否かを取得します。
        /// </summary>
        /// <param name="windowInfo">判定対象のウインドウ情報。</param>
        /// <returns>合致するか否か。</returns>
        public bool MatchWith(WindowInfo windowInfo)
        {
            for (PropertyType type = PropertyType.TitleBarText; type <= PropertyType.ClassName; type++)
            {
                if (!GetValidOf(type)) continue;

                string? filterText = GetTextOf(type);
                if (string.IsNullOrEmpty(filterText)) continue;

                string testText = GetTextFromWindowInfoOf(windowInfo, type);
                if (GetUsingRegexIn(type))
                {
                    if (GetMatchTypeOf(type) == MatchType.Contain)
                    {
                        if (GetRegexOf(type).IsMatch(testText)) return true;
                    }
                    else
                    {
                        if (GetRegexOf(type).Replace(testText, "") == "") return true;
                    }
                }
                else
                {
                    if (GetMatchTypeOf(type) == MatchType.Contain)
                    {
                        if (testText.Contains(filterText)) return true;
                    }
                    else
                    {
                        if (testText == filterText) return true;
                    }
                }
            }

            return false;
        }

    }
}

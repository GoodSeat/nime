using GoodSeat.Nime.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodSeat.Nime.Core
{
    internal class Utility
    {
        /// <summary>
        /// 指定文字がひらがなであるか否かを判定します。
        /// </summary>
        /// <param name="c">判定対象文字。</param>
        /// <returns>ひらがなであるか否か。</returns>
        public static bool IsHiragana(char c)
        {
            return 'あ' <= c && c <= 'ん';
        }

        /// <summary>
        /// 指定文字がアルファベットであるか否かを判定します。
        /// </summary>
        /// <param name="c">判定対象文字。</param>
        /// <returns>アルファベットであるか否か。</returns>
        public static bool IsAlphabet(char c)
        {
            return ('A' <= c && c <= 'Z')
                || ('a' <= c && c <= 'z');
        }

        /// <summary>
        /// 指定文字が大文字のアルファベットであるか否かを判定します。
        /// </summary>
        /// <param name="c">判定対象文字。</param>
        /// <returns>大文字のアルファベットであるか否か。</returns>
        public static bool IsUpperAlphabet(char c)
        {
            return ('A' <= c && c <= 'Z');
        }
        /// <summary>
        /// 指定文字が小文字のアルファベットであるか否かを判定します。
        /// </summary>
        /// <param name="c">判定対象文字。</param>
        /// <returns>小文字のアルファベットであるか否か。</returns>
        public static bool IsLowerAlphabet(char c)
        {
            return ('a' <= c && c <= 'z');
        }

        /// <summary>
        /// アルファベットから成る文字列をひらがなに変換して取得します。
        /// </summary>
        /// <param name="txt">変換対象とするアルファベットから成る文字列。</param>
        /// <returns>変換されたひらがな文字列。</returns>
        public static string ConvertToHiragana(string txt)
        {
            txt = txt.Replace("nn", "ん");

            // 大文字で区切る
            var list = txt.Aggregate(new List<string>(), (acc, c) => {
                if (acc.Count == 0 || IsUpperAlphabet(c)) acc.Add(c.ToString());
                else acc[acc.Count - 1] = acc[acc.Count - 1] + c.ToString();

                return acc;
            });

            var txtHiragana = string.Join("", list.Select(Microsoft.International.Converters.KanaConverter.RomajiToHiragana));
            txtHiragana = txtHiragana.Replace(",", "、");
            txtHiragana = txtHiragana.Replace(".", "。");
            txtHiragana = txtHiragana.Replace("|", "｜");
            txtHiragana = txtHiragana.Replace("\\", "￥");
            return txtHiragana;
        }

        /// <summary>
        /// 現在シフトキーが押下笹ているか否かを判定します。
        /// </summary>
        /// <returns>シフトキーが押下されているか否か。</returns>
        public static bool IsLockedShiftKey()
        {
            return KeyboardWatcher.IsKeyLockedStatic(Keys.LShiftKey) || KeyboardWatcher.IsKeyLockedStatic(Keys.RShiftKey);
        }


    }
}

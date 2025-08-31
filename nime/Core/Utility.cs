using GoodSeat.Nime.Device;
using GoodSeat.Nime.Windows;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace GoodSeat.Nime.Core
{
    /// <summary>
    /// 汎用的な静的メソッドを提供します。
    /// </summary>
    internal static class Utility
    {
        /// <summary>
        /// 指定文字がひらがなであるか否かを判定します。伸ばし棒や句読点もひらがなとして判定されます。カッコなどの記号はひらがなとしては判定されません。
        /// </summary>
        /// <param name="c">判定対象文字。</param>
        /// <returns>ひらがなであるか否か。</returns>
        public static bool IsHiragana(char c)
        {
            return ('あ' <= c && c <= 'ん') || ('ぁ' <= c && c <= 'ぉ') || ('ゃ' <= c && c <= 'ょ')
               ||  c == 'ヴ' || c == '、' || c == '。' || c == 'ー' ;
        }

        /// <summary>
        /// 指定の文字列が入力途上の日本語である可能性がある文字列か否かを判定します。
        /// </summary>
        /// <param name="textRomaji">判定対象のローマ字文字列。</param>
        /// <returns>指定の文字列が入力途上の日本語である可能性がある文字列か否か。</returns>
        public static bool IsMaybeJapaneseOnInput(string textRomaji)
        {
            bool existAlphabet = false;
            int alphabetContinue = 0;
            foreach (var c in textRomaji)
            {
                bool isAlphabet = Utility.IsLowerAlphabet(c);
                if (isAlphabet)
                {
                    alphabetContinue++;
                }
                else
                {
                    alphabetContinue = 0;
                }
                if (alphabetContinue >= 4) return false; // アルファベットが4文字以上連続した -> 日本語ではないだろう

                if (!existAlphabet) existAlphabet = isAlphabet;
                if (existAlphabet && Utility.IsHiragana(c)) return false; // アルファベットの後にひらがな -> 日本語ではないだろう
            }
            return true; // 上記以外は日本語の可能性あり
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
        /// <param name="convertLastN">末尾の単独のnを"ん"に変換するか否か。</param>
        /// <returns>変換されたひらがな文字列。</returns>
        public static string ConvertToHiragana(string txt, bool convertLastN = true)
        {
            if (string.IsNullOrEmpty(txt)) return txt;

            txt = txt.Replace("nn", "ん");

            bool lastIsN = !convertLastN && txt[txt.Length - 1] == 'n';
            if (lastIsN) txt = txt.Substring(0, txt.Length - 1);

            // 大文字で区切る
            var list = txt.Aggregate(new List<string>(), (acc, c) => {
                if (acc.Count == 0 || IsUpperAlphabet(c)) acc.Add(c.ToString());
                else acc[acc.Count - 1] = acc[acc.Count - 1] + c.ToString();

                return acc;
            })
            // 前の句が大文字のみで構成されていて、かつ大文字一文字から成る句は前の句にくっつける
            .Aggregate(new List<string>(), (acc, k) =>
            {
                if (acc.Count == 0 || !k.All(IsUpperAlphabet)) acc.Add(k);
                else if (acc[acc.Count - 1].All(IsUpperAlphabet) && k.Length == 1 && IsUpperAlphabet(k[0])) acc[acc.Count - 1] = acc[acc.Count - 1] + k;
                else acc.Add(k);

                return acc;
            });

            var txtHiragana = string.Join("", list.Select(k => k.All(IsUpperAlphabet) ? k : Microsoft.International.Converters.KanaConverter.RomajiToHiragana(k)));
            txtHiragana = txtHiragana.Replace(",", "、");
            txtHiragana = txtHiragana.Replace(".", "。");
            txtHiragana = txtHiragana.Replace("|", "｜");
            txtHiragana = txtHiragana.Replace("\\", "￥");
            if (lastIsN) txtHiragana += "n";
            return txtHiragana;
        }

        /// <summary>
        /// アルファベットの半角文字を全角に変換して取得します。
        /// </summary>
        /// <param name="narrowAlphabet">変換対象のアルファベットから成る半角文字列。</param>
        /// <returns>変換された全角文字列。</returns>
        public static string ToWide(string narrowAlphabet)
        {
            return narrowAlphabet.Select(c =>
            {
                switch (c)
                {
                    case 'a': return 'ａ';
                    case 'b': return 'ｂ';
                    case 'c': return 'ｃ';
                    case 'd': return 'ｄ';
                    case 'e': return 'ｅ';
                    case 'f': return 'ｆ';
                    case 'g': return 'ｇ';
                    case 'h': return 'ｈ';
                    case 'i': return 'ｉ';
                    case 'j': return 'ｊ';
                    case 'k': return 'ｋ';
                    case 'l': return 'ｌ';
                    case 'm': return 'ｍ';
                    case 'n': return 'ｎ';
                    case 'o': return 'ｏ';
                    case 'p': return 'ｐ';
                    case 'q': return 'ｑ';
                    case 'r': return 'ｒ';
                    case 's': return 'ｓ';
                    case 't': return 'ｔ';
                    case 'u': return 'ｕ';
                    case 'v': return 'ｖ';
                    case 'w': return 'ｗ';
                    case 'x': return 'ｘ';
                    case 'y': return 'ｙ';
                    case 'z': return 'ｚ';

                    case 'A': return 'Ａ';
                    case 'B': return 'Ｂ';
                    case 'C': return 'Ｃ';
                    case 'D': return 'Ｄ';
                    case 'E': return 'Ｅ';
                    case 'F': return 'Ｆ';
                    case 'G': return 'Ｇ';
                    case 'H': return 'Ｇ';
                    case 'I': return 'Ｉ';
                    case 'J': return 'Ｊ';
                    case 'K': return 'Ｋ';
                    case 'L': return 'Ｌ';
                    case 'M': return 'Ｍ';
                    case 'N': return 'Ｎ';
                    case 'O': return 'Ｏ';
                    case 'P': return 'Ｐ';
                    case 'Q': return 'Ｑ';
                    case 'R': return 'Ｒ';
                    case 'S': return 'Ｓ';
                    case 'T': return 'Ｔ';
                    case 'U': return 'Ｕ';
                    case 'V': return 'Ｖ';
                    case 'W': return 'Ｗ';
                    case 'X': return 'Ｘ';
                    case 'Y': return 'Ｙ';
                    case 'Z': return 'Ｚ';
                }
                return c;
            }).Aggregate("", (acc, c) => acc + c.ToString());
        }

        /// <summary>
        /// 全角のひらがなを半角カタカナに変換して取得します。
        /// </summary>
        /// <param name="hiragana">変換対象のひらがな文字列。</param>
        /// <returns>変換された半角カタカナ文字列。</returns>
        public static string ToNarrowKatakana(string hiragana)
        {
            return hiragana.Select(c =>
            {
                switch (c)
                {
                    case 'あ': return "ｱ";
                    case 'い': return "ｲ";
                    case 'う': return "ｳ";
                    case 'え': return "ｴ";
                    case 'お': return "ｵ";

                    case 'か': return "ｶ";
                    case 'き': return "ｷ";
                    case 'く': return "ｸ";
                    case 'け': return "ｹ";
                    case 'こ': return "ｺ";
                    case 'が': return "ｶﾞ";
                    case 'ぎ': return "ｷﾞ";
                    case 'ぐ': return "ｸﾞ";
                    case 'げ': return "ｹﾞ";
                    case 'ご': return "ｺﾞ";
                    case 'さ': return "ｻ";
                    case 'し': return "ｼ";
                    case 'す': return "ｽ";
                    case 'せ': return "ｾ";
                    case 'そ': return "ｿ";
                    case 'ざ': return "ｻﾞ";
                    case 'じ': return "ｼﾞ";
                    case 'ず': return "ｽﾞ";
                    case 'ぜ': return "ｾﾞ";
                    case 'ぞ': return "ｿﾞ";
                    case 'た': return "ﾀ";
                    case 'ち': return "ﾁ";
                    case 'つ': return "ﾂ";
                    case 'て': return "ﾃ";
                    case 'と': return "ﾄ";
                    case 'だ': return "ﾀﾞ";
                    case 'ぢ': return "ﾁﾞ";
                    case 'づ': return "ﾂﾞ";
                    case 'で': return "ﾃﾞ";
                    case 'ど': return "ﾄﾞ";

                    case 'な': return "ﾅ";
                    case 'に': return "ﾆ";
                    case 'ぬ': return "ﾇ";
                    case 'ね': return "ﾈ";
                    case 'の': return "ﾉ";

                    case 'は': return "ﾊ";
                    case 'ひ': return "ﾋ";
                    case 'ふ': return "ﾌ";
                    case 'へ': return "ﾍ";
                    case 'ほ': return "ﾎ";
                    case 'ば': return "ﾊﾞ";
                    case 'び': return "ﾋﾞ";
                    case 'ぶ': return "ﾌﾞ";
                    case 'べ': return "ﾍﾞ";
                    case 'ぼ': return "ﾎﾞ";
                    case 'ぱ': return "ﾊﾟ";
                    case 'ぴ': return "ﾋﾟ";
                    case 'ぷ': return "ﾌﾟ";
                    case 'ぺ': return "ﾍﾟ";
                    case 'ぽ': return "ﾎﾟ";

                    case 'ま': return "ﾏ";
                    case 'み': return "ﾐ";
                    case 'む': return "ﾑ";
                    case 'め': return "ﾒ";
                    case 'も': return "ﾓ";
                    case 'や': return "ﾔ";
                    case 'ゆ': return "ﾕ";
                    case 'よ': return "ﾖ";
                    case 'ら': return "ﾗ";
                    case 'り': return "ﾘ";
                    case 'る': return "ﾙ";
                    case 'れ': return "ﾚ";
                    case 'ろ': return "ﾛ";
                    case 'わ': return "ﾜ";
                    case 'を': return "ｦ";
                    case 'ん': return "ﾝ";

                    case 'っ': return "ｯ";
                    case 'ゃ': return "ｬ";
                    case 'ゅ': return "ｭ";
                    case 'ょ': return "ｮ";
                    case 'ぁ': return "ｧ";
                    case 'ぃ': return "ｨ";
                    case 'ぅ': return "ｩ";
                    case 'ぇ': return "ｪ";
                    case 'ぉ': return "ｫ";
                    case 'ヴ': return "ｳ";
                }
                return c.ToString();
            }).Aggregate("", (acc, c) => acc + c);
        }

        /// <summary>
        /// 現在シフトキーが押下されているか否かを判定します。
        /// </summary>
        /// <returns>シフトキーが押下されているか否か。</returns>
        public static bool IsLockedShiftKey()
        {
            return KeyboardWatcher.IsKeyLockedStatic(Keys.LShiftKey) || KeyboardWatcher.IsKeyLockedStatic(Keys.RShiftKey);
        }

        /// <summary>
        /// 現在コントロールキーが押下されているか否かを判定します。
        /// </summary>
        /// <returns>コントロールキーが押下されているか否か。</returns>
        public static bool IsLockedCtrlKey()
        {
            return KeyboardWatcher.IsKeyLockedStatic(Keys.Control) || KeyboardWatcher.IsKeyLockedStatic(Keys.ControlKey) ||
                   KeyboardWatcher.IsKeyLockedStatic(Keys.LControlKey) || KeyboardWatcher.IsKeyLockedStatic(Keys.RControlKey);
        }


        /// <summary>
        /// 指定要素を指定個数含むリストを初期化して取得します。
        /// </summary>
        /// <typeparam name="T">対象要素の型。</typeparam>
        /// <param name="element">リストに追加する要素。</param>
        /// <param name="n">要素数。</param>
        /// <returns><paramref name="element"/>を<paramref name="n"/>個だけ含むリスト。</returns>
        public static List<T> Duplicates<T>(T element, int n)
        {
            var values = new List<T>();
            for (int i = 0; i < n; i++) values.Add(element);
            return values;
        }


        /// <summary>
        /// キャレット位置を示すスクリーン上の座標及びキャレットサイズを取得します。正しく取得できなかった場合、Y座標=0の位置を返します。
        /// </summary>
        /// <param name="wi">取得対象とするウインドウハンドル。アクティブウインドウを対象とするにはnullを指定します。</param>
        /// <returns>キャレット位置を示すスクリーン上の座標及びキャレットサイズ。</returns>
        public static (Point, Size) GetCaretCoordinateAndSize(WindowInfo? wi = null)
        {
            if (UIA.TryGetCaretPosition(out var pos, out var size))
            {
                return (new Point((int)pos.X, (int)pos.Y), new Size((int)size.Width, (int)size.Height));
            }

            var inf = MSAA.GetCaretPosition(wi);
            if (inf.Item1.Y != 0)
            {
                return (inf.Item1, inf.Item2);
            }
            else
            {
                return (Caret.GetCaretPosition(wi), new Size(1, 15));
            }
            //UIAutomation.GetCaretPosition(); // TOOD!:WPF対応
        }

        /// <summary>
        /// キャレット位置を示すスクリーン上の座標を取得します。正しく取得できなかった場合、Y座標=0の位置を返します。
        /// </summary>
        /// <param name="wi">取得対象とするウインドウハンドル。アクティブウインドウを対象とするにはnullを指定します。</param>
        /// <returns>キャレット位置を示すスクリーン上の座標。</returns>
        public static Point GetCaretCoordinate(WindowInfo? wi = null)
        {
            return GetCaretCoordinateAndSize(wi).Item1;
        }

    }
}

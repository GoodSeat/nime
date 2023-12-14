using GoodSeat.Nime.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading.Tasks;

namespace GoodSeat.Nime.Conversion
{
    /// <summary>
    /// ローマ字もしくはひらがなからなる文字列を漢字を含んだ日本語文章に変換するメソッドを提供します。
    /// </summary>
    public class ConvertToSentence
    {
        /// <summary>
        /// ひらがなで構成されるテキストを漢字を含む日本語文章に変換して取得します。
        /// </summary>
        /// <param name="txtHiragana">変換元とするひらがなの文字列。</param>
        /// <param name="inputHistory">入力履歴情報。</param>
        /// <param name="splitHistory">文節区切りの編集情報。</param>
        /// <param name="timeout">変換処理のタイムアウト時間(ms)。</param>
        /// <returns>変換処理により得られた日本語文章情報。失敗した場合にはnull。</returns>
        internal ConvertCandidate? ConvertFromHiragana(string txtHiragana, InputHistory inputHistory, SplitHistory? splitHistory, int timeout)
        {
            try
            {
                // TODO!:ひらがなで45文字？を超えたあたりから結果が返ってこなくなるので、適当に分解するしかない
                // ,があるなら適当な,の位置で分解、さもなくばもう適当に分解するしかないか。
                var c0 = ConvertHiraganaToSentenceByGoogleCGI.Request(txtHiragana, timeout, inputHistory);
                if (splitHistory != null)
                {
                    var s0 = c0.MakeSentenceForHttpRequest();
                    var s1 = splitHistory.SplitConsiderHisory(s0);
                    if (s0 != s1) c0 = ConvertHiraganaToSentenceByGoogleCGI.Request(s1, timeout, inputHistory);
                }
                return c0;
            }
            catch { return null; }

        }

        /// <summary>
        /// 文節に対する変換種別を表します。
        /// </summary>
        public enum ForceMode
        {
            /// <summary>
            /// ひらがなに変換します。
            /// </summary>
            OnlyHiragana,
            /// <summary>
            /// カタカナに変換します。
            /// </summary>
            OnlyKatakana,
            /// <summary>
            /// 半角カタカナに変換します。
            /// </summary>
            OnlyHalfKatakana,
            /// <summary>
            /// 全角ローマ字に変換します。
            /// </summary>
            OnlyWideRomaji
        }

        /// <summary>
        /// 指定の変換モードに従って、ローマ字文字列を単独の文節として日本語に変換します。
        /// </summary>
        /// <param name="txtRomaji">変換元とするローマ字文字列。</param>
        /// <param name="inputHistory">入力履歴情報。</param>
        /// <param name="timeout">変換処理のタイムアウト時間(ms)。</param>
        /// <param name="mode">変換モード。</param>
        /// <returns>変換により取得した文字列。</returns>
        internal Task<ConvertCandidate?> ConvertFromRomajiAsync(string txtRomaji, InputHistory inputHistory, int timeout, ForceMode mode)
        {
            return Task.Run(() =>
            {
                var txtHiragana =  Utility.ConvertToHiragana(txtRomaji);

                var txtDst = txtHiragana;
                switch (mode)
                {
                    case ForceMode.OnlyKatakana:
                        txtDst = Microsoft.International.Converters.KanaConverter.HiraganaToKatakana(txtHiragana);
                        break;
                    case ForceMode.OnlyHalfKatakana:
                        //txtDst = Microsoft.International.Converters.KanaConverter.HiraganaToHalfwidthKatakana(txtHiragana);
                        txtDst = Utility.ToNarrowKatakana(txtHiragana);
                        break;
                    case ForceMode.OnlyWideRomaji:
                        txtDst = Utility.ToWide(txtRomaji);
                        break;
                }

                var res = ConvertFromHiragana(txtHiragana + ",", inputHistory, null, timeout);
                if (res == null) return res;

                Debug.Assert(res.PhraseList.Count == 1);
                res.PhraseList[0].Selected = txtDst;
                return res;
            });
        }

        /// <summary>
        /// ローマ字文字列を日本語に変換します。
        /// </summary>
        /// <param name="txtRomaji">変換元とするローマ字文字列。</param>
        /// <param name="inputHistory">入力履歴情報。</param>
        /// <param name="splitHistory">文節区切りの編集情報。</param>
        /// <param name="timeout">変換処理のタイムアウト時間。</param>
        /// <returns>変換により取得した文字列。</returns>
        internal Task<ConvertCandidate?> ConvertFromRomajiAsync(string txtRomaji, InputHistory inputHistory, SplitHistory splitHistory, int timeout)
        {
            return Task.Run(() =>
            {
                if (txtRomaji.All(c => !Utility.IsUpperAlphabet(c)))
                {
                    return ConvertFromHiragana(Utility.ConvertToHiragana(txtRomaji), inputHistory, splitHistory, timeout);
                }
                else
                {
                    var ss = new List<string>();
                    while (!string.IsNullOrEmpty(txtRomaji))
                    {
                        string word = txtRomaji[0].ToString();
                        txtRomaji = txtRomaji.Substring(1);

                        // 次もその次も大文字ならば、もう一字取る
                        while (txtRomaji.Length > 1 && Utility.IsUpperAlphabet(txtRomaji[0]) && Utility.IsUpperAlphabet(txtRomaji[1]))
                        {
                            word += txtRomaji[0].ToString();
                            txtRomaji = txtRomaji.Substring(1);
                        }

                        var w = txtRomaji.TakeWhile(c => !Utility.IsUpperAlphabet(c));
                        word = w.Aggregate(word, (acc, c) => acc + c.ToString());
                        ss.Add(word);

                        txtRomaji = txtRomaji.Substring(w.Count());
                    }

                    var cs = ss.AsParallel().Select(t =>
                    {
                        if (t.All(Utility.IsUpperAlphabet)) return new ConvertCandidate(t);
                        return ConvertFromHiragana(Utility.ConvertToHiragana(t), inputHistory, splitHistory, timeout);
                    });

                    if (cs == null || cs.Any(c => c == null)) return null;
                    return ConvertCandidate.Concat(cs.ToArray());
                }
            });

        }

    }

    /// <summary>
    /// Google CGI API for Japanese Input [https://www.google.co.jp/ime/cgiapi.html] を利用したひらがなから日本語への変換処理を提供します。
    /// </summary>
    public static class ConvertHiraganaToSentenceByGoogleCGI
    {
        /// <summary>
        /// ひらがな文字列から日本語文章に変換して取得します。
        /// </summary>
        /// <param name="txtHiragana">変換元とするひらがなの文字列。</param>
        /// <param name="timeout">変換処理のタイムアウト時間(ms)。</param>
        /// <param name="inputHistory">入力履歴情報。</param>
        /// <returns>変換処理により得られた日本語文章情報。</returns>
        internal static ConvertCandidate? Request(string txtHiragana, int timeout, InputHistory inputHistory)
        {
            using (var client = new HttpClient())
            {
                var txtReq = $"http://www.google.com/transliterate?langpair=ja-Hira|ja&text=" + txtHiragana;
                Debug.WriteLine("get:" + txtReq);

                var httpsResponse = client.GetAsync(txtReq);
                Task<string>? responseContent = null;

                for (int i = 0; i < timeout; i++)
                {
                    if (httpsResponse.IsCompleted)
                    {
                        responseContent = httpsResponse.Result.Content.ReadAsStringAsync();
                        break;
                    }
                    Thread.Sleep(1);
                }
                if (responseContent == null)
                {
                    return null; // TODO:本来は、とりあえずひらがな、カタカナを返すか、InputHistoryに基づいて結果を返してほしい
                }

                Debug.WriteLine("return:" + responseContent?.Result.ToString());

                var options = new JsonSerializerOptions
                {
                    Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                    WriteIndented = true
                };

                var ans = JsonSerializer.Deserialize<JsonResponse>("{ \"Strings\":" + responseContent.Result + " }", options);
                if (ans == null) return null;

                return new ConvertCandidate(ans, inputHistory);
            }
        }
    }

    public class JsonResponse
    {
        public List<List<object>> Strings { get; set; }
    }
}

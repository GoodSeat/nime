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
        internal ConvertCandidate? ConvertFromHiragana(string txtHiragana, InputHistory inputHistory, SplitHistory? splitHistory, int timeout)
        {
            try
            {
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

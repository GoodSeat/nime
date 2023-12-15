using GoodSeat.Nime.Core;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GoodSeat.Nime.Conversion
{
    /// <summary>
    /// 文節の候補リストから成る文章の変換候補情報を表します。
    /// </summary>
    public class ConvertCandidate
    {
        // キー候補
        //   sは分割、Sは区切り全削除&分割
        //   x->キー で辞書登録解除のトグル
        // Shift+キー でIMEによる直接編集 -> 辞書登録 (常に自動登録/元文字にアルファベットが含まれていなければ自動登録/常に自動登録しない)

        // IDE直接編集開くとき、以下の方法が欲しい
        //  ・開いて自動で変換候補を開く（終了時辞書登録する）
        //  ・開いて自動で変換候補を開く（終了時辞書登録しない）
        //  ・開いて何もしない(誤字修正用)（選択もしない、）
        static char[] s_keys = new char[]
        {
            'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r',
        };

        /// <summary>
        /// 各文節に関連付けるキー文字列を列挙します。
        /// </summary>
        /// <param name="totalCount">必要な列挙数。</param>
        /// <returns>キー文字列の列挙子。</returns>
        public static IEnumerable<string> GetKeys(int totalCount)
        {
            if (totalCount <= s_keys.Length)
            {
                foreach (var c in s_keys) yield return c.ToString();
            }
            else
            {
                foreach (var k1 in GetKeys(s_keys.Length))
                {
                    foreach (var k2 in GetKeys(s_keys.Length))
                    {
                        yield return k1 + k2;
                    }
                }
            }
        }

        /// <summary>
        /// 文章の変換候補情報の複製を生成して取得します。
        /// </summary>
        /// <param name="org">複製元の文章の変換候補情報。</param>
        /// <returns>複製された文章の変換候補情報。</returns>
        public static ConvertCandidate CopyFrom(ConvertCandidate org)
        {
            var result = new ConvertCandidate();
            org.PhraseList.ForEach(c => result.PhraseList.Add(ConvertCandidatePhrase.CopyFrom(c)));
            return result;
        }

        /// <summary>
        /// 任意数の文章の変換候補を結合し、一つの文章変換候補を生成します。
        /// </summary>
        /// <param name="convertCandidates">可変数の文章変換候補。</param>
        /// <returns>結合された文章変換候補。</returns>
        public static ConvertCandidate Concat(params ConvertCandidate[] convertCandidates)
        {
            var ps = convertCandidates.SelectMany(c => c.PhraseList);

            var keys1 = GetKeys(ps.Count()).Take(ps.Count()).ToList();

            var res = new ConvertCandidate();
            foreach (var (p, k1) in ps.Zip(keys1))
            {
                var keys2 = GetKeys(p.Candidates.Count).Take(p.Candidates.Count);
                var keys = keys2.Select(k2 => k1 + k2).ToList();

                foreach (var (pc, k) in p.Candidates.Zip(keys))
                {
                    pc.Key = k;
                }
                res.PhraseList.Add(p);
            }
            return res;
        }



        private ConvertCandidate() { }

        /// <summary>
        /// "GUI"や"USB"等の大文字ローマ字による候補文節を初期化します。
        /// </summary>
        /// <param name="englishAbbr">"GUI"や"USB"等の大文字ローマ字からなる文字列。</param>
        public ConvertCandidate(string englishAbbr)
        {
            var list = new List<string>
            {
                englishAbbr,
                Utility.ToWide(englishAbbr),
                englishAbbr.ToLower(),
                Utility.ToWide(englishAbbr.ToLower())
            };
            if (englishAbbr.Length > 1)
            {
                list.Add(englishAbbr[0] + englishAbbr.Substring(1).ToLower());
                list.Add(Utility.ToWide(englishAbbr[0] + englishAbbr.Substring(1).ToLower()));
            }
            else if (englishAbbr == "A") list.Add("あ");
            else if (englishAbbr == "I") list.Add("い");
            else if (englishAbbr == "U") list.Add("う");
            else if (englishAbbr == "E") list.Add("え");
            else if (englishAbbr == "O") list.Add("お");

            var keys1 = GetKeys(list.Count).Take(list.Count).ToList();

            var  cs = new List<CandidatePhrase>();
            foreach (var (phrase, k1) in list.Zip(keys1))
            {
                cs.Add(new CandidatePhrase(phrase, k1));
            }
            PhraseList.Add(new ConvertCandidatePhrase(englishAbbr, englishAbbr, cs));
        }

        /// <summary>
        /// 日本語変換APIのレスポンス情報から、文章の変換候補情報を初期化します。
        /// </summary>
        /// <param name="response">日本語変換APIのレスポンス情報。</param>
        /// <param name="inputHistory">入力履歴情報(優先して選択する文節を考慮する)。</param>
        internal ConvertCandidate(JsonResponse response, InputHistory inputHistory)
        {
            var keys1 = GetKeys(response.Strings.Count).Take(response.Strings.Count).ToList();

            var cs = response.Strings.Zip(keys1).AsParallel().Select((t, indx) =>
            {
                var (lst, k1) = t;
                var orgHiragana = ((JsonElement)lst[0]).ToString();
                var candidates = (JsonElement)lst[1];

                var ps = candidates.EnumerateArray().Select(e => e.ToString()).ToList();
                if (!ps.Contains(orgHiragana)) ps.Add(orgHiragana);

                var orgKatakana = Microsoft.International.Converters.KanaConverter.HiraganaToKatakana(orgHiragana);
                if (!ps.Contains(orgKatakana)) ps.Add(orgKatakana);

                var selectedPhrase = inputHistory.GetRecentryPharaseFor(orgHiragana);
                if (selectedPhrase != null && !ps.Contains(selectedPhrase)) ps.Insert(0, selectedPhrase);

                foreach (var p in inputHistory.GetConfirmedPharaseFor(orgHiragana))
                {
                    if (ps.Count >= 10) break;
                    if (!ps.Contains(p)) ps.Insert(0, p);
                }

                var keys2 = GetKeys(ps.Count).Take(ps.Count);
                var keys = keys2.Select(k2 => k1 + k2).ToList();
                if (response.Strings.Count == 1) keys = keys2.ToList();

                var phrase = new ConvertCandidatePhrase(orgHiragana, orgHiragana, ps.Zip(keys, (e, k) => new CandidatePhrase(e.ToString(), k)).ToList());
                if (selectedPhrase != null) phrase.Selected = selectedPhrase;

                return phrase;
            });
            cs.ToList().ForEach(PhraseList.Add);
        }


        /// <summary>
        /// 文節の候補リストを設定もしくは取得します。
        /// </summary>
        public List<ConvertCandidatePhrase> PhraseList { get; set; } = new List<ConvertCandidatePhrase>();


        /// <summary>
        /// 現在の選択状態に基づく文章を生成して取得します。
        /// </summary>
        /// <returns>選択された文章。</returns>
        public string GetSelectedSentence()
        {
            return string.Join("", PhraseList.Select(p => p.Selected));
        }

        /// <summary>
        /// この文章の変換候補に対応する日本語変換API問合せ用のひらがな文字列を生成して取得します。
        /// </summary>
        /// <returns>文節を,で区切ったひらがな文字列。</returns>
        public string MakeSentenceForHttpRequest()
        {
            return string.Join(",", PhraseList.Select(p => p.OriginalHiragana).ToList());
        }


        /// <summary>
        /// この文章の変換候補の各文節の選択状態を、指定の文章変換候補情報に基づいて調整します。
        /// </summary>
        /// <param name="newSentence">調整の基準とする変換候補情報。</param>
        public void ModifyConsideration(ConvertCandidate newSentence)
        {
            var oldList = PhraseList;
            PhraseList = newSentence.PhraseList;

            foreach (var phrase in PhraseList)
            {
                var oldPhrase = oldList.FirstOrDefault(p => p.OriginalHiragana == phrase.OriginalHiragana);
                if (oldPhrase != null)
                {
                    phrase.Selected = oldPhrase.Selected;
                    while (oldList.Count > 0)
                    {
                        bool exit = false;
                        if (oldList[0] == oldPhrase) exit = true;
                        oldList.RemoveAt(0);
                        if (exit) break;
                    }
                }
            }
        }

        /// <summary>
        /// 現在の選択状態を、入力履歴情報に登録します。
        /// </summary>
        /// <param name="recentryConfirmedInput">登録先の入力履歴情報。</param>
        internal void RegisterConfirmedInput(InputHistory recentryConfirmedInput)
        {
            foreach (var phrase in PhraseList)
            {
                recentryConfirmedInput.Register(phrase.OriginalHiragana, phrase.Selected);
            }
        }

    }


    /// <summary>
    /// 文節の変換候補情報を表します。
    /// </summary>
    public class ConvertCandidatePhrase
    {
        /// <summary>
        /// 文節の変換候補情報を初期化します。
        /// </summary>
        /// <param name="originalAlphabet">文節の元となるローマ字文字列。</param>
        /// <param name="originalHiragana">文節の元となるひらがな字文字列。</param>
        /// <param name="candidates">選択候補となる文節情報リスト。</param>
        public ConvertCandidatePhrase(string originalAlphabet, string originalHiragana, List<CandidatePhrase> candidates)
        {
            OriginalAlphabet = originalAlphabet;
            OriginalHiragana = originalHiragana;
            Candidates = candidates;

            Selected = candidates.FirstOrDefault().Phrase;
        }

        /// <summary>
        /// 選択されている文節文字列を設定もしくは取得します。
        /// </summary>
        public string Selected { get; set; }

        /// <summary>
        /// 文節の元となるローマ字文字列を設定もしくは取得します。
        /// </summary>
        public string OriginalAlphabet { get; set; }

        /// <summary>
        /// 文節の元となるひらがな文字列を設定もしくは取得します。
        /// </summary>
        public string OriginalHiragana { get; set; }

        /// <summary>
        /// 選択候補の文節情報リストを設定もしくは取得します。
        /// </summary>
        public List<CandidatePhrase> Candidates { get; set; }


        /// <summary>
        /// 文節の変換情報を複製して取得します。
        /// </summary>
        /// <param name="org">複製元とする文節の変換情報。</param>
        /// <returns>複製された文節の変換情報。</returns>
        public static ConvertCandidatePhrase CopyFrom(ConvertCandidatePhrase org)
        {
            var candidates = new List<CandidatePhrase>();
            org.Candidates.ForEach(c => candidates.Add(CandidatePhrase.CopyFrom(c)));

            var result = new ConvertCandidatePhrase(org.OriginalAlphabet, org.OriginalHiragana, candidates);
            result.Selected = org.Selected;
            return result;
        }
    }


    /// <summary>
    /// 文節情報を表します。
    /// </summary>
    public class CandidatePhrase
    {
        /// <summary>
        /// 文節情報を初期化します。
        /// </summary>
        /// <param name="phrase">文節の文字列。</param>
        /// <param name="key">この文節に関連付けるキー文字列。</param>
        public CandidatePhrase(string phrase, string key)
        {
            Phrase = phrase;
            Key = key;
        }

        /// <summary>
        /// この文節情報が保持する文節文字列を設定もしくは取得します。
        /// </summary>
        public string Phrase { get; set; }

        /// <summary>
        /// 関連付けられたキー文字列を設定もしくは取得します。
        /// </summary>
        public string Key { set; get; }

        /// <summary>
        /// 文節情報の複製を生成して取得します。
        /// </summary>
        /// <param name="org">複製元とする文節情報。</param>
        /// <returns>複製された文節情報。</returns>
        public static CandidatePhrase CopyFrom(CandidatePhrase org)
        {
            return new CandidatePhrase(org.Phrase, org.Key);
        }
    }

}

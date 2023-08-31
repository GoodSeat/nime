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

        public string GetSelectedSentence()
        {
            return string.Join("", PhraseList.Select(p => p.Selected));
        }

        public string MakeSentenceForHttpRequest()
        {
            return string.Join(",", PhraseList.Select(p => p.OriginalHiragana).ToList());
        }


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

        internal void RegisterConfirmedInput(InputHistory recentryConfirmedInput)
        {
            foreach (var phrase in PhraseList)
            {
                recentryConfirmedInput.Register(phrase.OriginalHiragana, phrase.Selected);
            }
        }


        public List<ConvertCandidatePhrase> PhraseList { get; set; } = new List<ConvertCandidatePhrase>();


        public static ConvertCandidate CopyFrom(ConvertCandidate org)
        {
            var result = new ConvertCandidate();
            org.PhraseList.ForEach(c => result.PhraseList.Add(ConvertCandidatePhrase.CopyFrom(c)));
            return result;
        }

    }

    public class ConvertCandidatePhrase
    {
        public ConvertCandidatePhrase(string originalAlphabet, string originalHiragana, List<CandidatePhrase> candidates)
        {
            OriginalAlphabet = originalAlphabet;
            OriginalHiragana = originalHiragana;
            Candidates = candidates;

            Selected = candidates.FirstOrDefault().Phrase;
        }

        public string Selected { get; set; }

        public string Key { get; set; }

        public string OriginalAlphabet { get; set; }

        public string OriginalHiragana { get; set; }

        public List<CandidatePhrase> Candidates { get; set; }


        public static ConvertCandidatePhrase CopyFrom(ConvertCandidatePhrase org)
        {
            var candidates = new List<CandidatePhrase>();
            org.Candidates.ForEach(c => candidates.Add(CandidatePhrase.CopyFrom(c)));

            var result = new ConvertCandidatePhrase(org.OriginalAlphabet, org.OriginalHiragana, candidates);
            result.Selected = org.Selected;
            result.Key = org.Key;
            return result;
        }
    }

    public class CandidatePhrase
    {
        public CandidatePhrase(string phrase, string key)
        {
            Phrase = phrase;
            Key = key;
        }

        public string Phrase { get; set; }

        public string Key { set; get; }

        public static CandidatePhrase CopyFrom(CandidatePhrase org)
        {
            return new CandidatePhrase(org.Phrase, org.Key);
        }
    }

}

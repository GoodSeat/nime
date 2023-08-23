using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace nime
{
    public class ConvertCandidate
    {

        // キー候補
        //   sは分割、Sは区切り全削除&分割
        //   x->キー で辞書登録解除のトグル
        // Shift+キー でIMEによる直接編集 -> 辞書登録 (常に自動登録/元文字にアルファベットが含まれていなければ自動登録/常に自動登録しない)
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

        public ConvertCandidate(JsonResponse response)
        {
            var keys1 = GetKeys(response.Strings.Count).Take(response.Strings.Count).ToList();

            foreach (var (lst, k1) in response.Strings.Zip(keys1))
            {
                var orgHiragana = ((JsonElement)lst[0]).ToString();
                var candidates = (JsonElement)lst[1];

                var ps = candidates.EnumerateArray().Select(e => e.ToString()).ToList();
                if (!ps.Contains(orgHiragana)) ps.Add(orgHiragana);

                var orgKatakana = Microsoft.International.Converters.KanaConverter.HiraganaToKatakana(orgHiragana);
                if (!ps.Contains(orgKatakana)) ps.Add(orgKatakana);

                var keys2 = GetKeys(ps.Count).Take(ps.Count);
                var keys = keys2.Select(k2 => k1 + k2).ToList();
                if (response.Strings.Count == 1) keys = keys2.ToList();

                var phrase = new ConvertCandidatePhrase(orgHiragana, orgHiragana, ps.Zip(keys, (e, k) => new CandidatePhrase(e.ToString(), k)).ToList());
                PhraseList.Add(phrase);
            }
        }

        public string GetSelectedSentence()
        {
            return string.Join("", PhraseList.Select(p => p.Selected));
        }

        public List<ConvertCandidatePhrase> PhraseList { get; set; } = new List<ConvertCandidatePhrase>();



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

    }

}

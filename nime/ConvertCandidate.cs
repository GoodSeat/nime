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

        // キー候補(s,mは文節区切り編集用)
        char[] s_keys = new char[]
        {
            'a', /*'s',*/ 'd', 'f', 'g',
            'h', 'j', 'k', 'l',
            /*'m',*/ 'n', 'u', 'y',
            'v', 'b', 'r', 't',
            'w', 'o', 'x',
        };

        IEnumerable<string> GetKeys(int totalCount)
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
            int countCandidateTotal = 0;
            foreach (var lst in response.Strings)
            {
                var candidates = (JsonElement)lst[1];
                countCandidateTotal += candidates.EnumerateArray().Count();
            }

            var keys = GetKeys(countCandidateTotal).Take(countCandidateTotal).ToList();

            int i = 0;
            foreach (var lst in response.Strings)
            {
                var key = (JsonElement)lst[0];
                var candidates = (JsonElement)lst[1];

                var phrase = new ConvertCandidatePhrase(key.ToString(), key.ToString(), candidates.EnumerateArray().Select(e => new CandidatePhrase(e.ToString(), keys[i++])).ToList());
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


        public void ModifyConsideration(ConvertCandidate oldSentence)
        {
            // TODO!:元の選択状態との差分を考慮して選択状態を近づける

        }

    }

    public class ConvertCandidatePhrase
    {
        public ConvertCandidatePhrase(string originalAlphabet, string originalHiragana, List<CandidatePhrase> candidates)
        {
            OriginalAlphabet = originalAlphabet;
            OriginalAlphabet = originalHiragana;
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

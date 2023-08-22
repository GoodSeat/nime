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
        IEnumerable<string> GetKeys(int totalCount)
        {
            if (totalCount <= 20)
            {
                for (int i = 0; i < totalCount; i++)
                {
                    yield return ((char)((int)'a' + i)).ToString();
                }
            }
            else
            {
                foreach (var k1 in GetKeys(20))
                {
                    foreach (var k2 in GetKeys(20))
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

using GoodSeat.Nime.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodSeat.Nime.Conversion
{
    internal class InputSuggestion
    {
        public Task RegisterHiraganaSequenceAsync(ConvertCandidate convertCandidate)
        {
            return Task.Run(() => {
                RegisterHiraganaSequence(convertCandidate.PhraseList.Select(p => new HiraganaSet(p.OriginalHiragana, p.Selected)).ToList());
            });
        }

        public void RegisterHiraganaSequence(List<HiraganaSet> values)
        {
            if (values.Count == 0) return;

            var set = values.FirstOrDefault();
            var (hiragana, phrase) = set;
            if (hiragana.Any(c => !Utility.IsHiragana(c))) return;

            var key = hiragana.Substring(0, 2);
            values.RemoveAt(0);
            var nextSet = values.FirstOrDefault() ?? new HiraganaSet("", "");

            Dictionary<string, (HiraganaSet, List<HiraganaSet>)> dic;
            if (!MapHistoryHiraganaSequence.TryGetValue(key, out dic))
            {
                dic = new Dictionary<string, (HiraganaSet, List<HiraganaSet>)>();
                MapHistoryHiraganaSequence.Add(key, dic);
            }

            if (dic.TryGetValue(set.Hiragana, out var list))
            {
                if (!list.Item2.Contains(nextSet)) list.Item2.Add(nextSet);
            }
            else
            {
                dic.Add(set.Hiragana, (set, new List<HiraganaSet>() { nextSet }));
            }

            if (values.Count != 0) RegisterHiraganaSequence(values);
        }


        private IEnumerable<string> NextCandidateFrom(string romaji)
        {
            if (romaji.Length == 1)
            {
                foreach (var t in new List<string>{ "a", "i", "u", "e", "o" }) 
                {
                    var th = Microsoft.International.Converters.KanaConverter.RomajiToHiragana(romaji + t);
                    if (!string.IsNullOrEmpty(th) && th.All(Utility.IsHiragana)) yield return th;
                }
                yield return "っ";
            }
            else if (romaji.Length == 2 && romaji[0] == romaji[1])
            {
                foreach (var n in NextCandidateFrom(romaji[0].ToString()))
                {
                    if (n != "っ") yield return "っ" + n;
                }
            }
            else
            {
                foreach (var t in new List<string>{ "a", "i", "u", "e", "o" }) 
                {
                    var th = Microsoft.International.Converters.KanaConverter.RomajiToHiragana(romaji + t);
                    if (!string.IsNullOrEmpty(th) && th.All(Utility.IsHiragana)) yield return th;
                }
            }
        }

        public Task<HiraganaSequenceTree?> SearchStartWithAsync(string hiragana)
        {
            return Task.Run(() =>
            {
                int n = hiragana.TakeWhile(Utility.IsHiragana).Count();
                if (n == 0) return null;

                var hiraganaDet = hiragana.Substring(0, n);
                var hiraganaMid = hiragana.Substring(n);

                var candidates = new List<string>();
                if (string.IsNullOrEmpty(hiraganaMid))
                {
                    candidates.Add(hiraganaDet);
                }
                else
                {
                    candidates.AddRange(NextCandidateFrom(hiraganaMid).Select(t => hiraganaDet + t));
                }
                if (candidates.Count == 0) candidates.Add(hiraganaDet);

                var result = new HiraganaSequenceTree();
                foreach (var candidate in candidates)
                {
                    var key = candidate.Substring(0, 2);
                    if (!MapHistoryHiraganaSequence.TryGetValue(key, out var dic)) continue;

                    foreach (var pair in dic)
                    {
                        var hs = pair.Key;
                        if (!hs.StartsWith(candidate)) continue;

                        var val = pair.Value;
                        var tree = MakeHiraganaSequenceTreeStartWith(val);

                        result.Tree.Add((val.Item1, tree));
                    }
                }

                return result;
            });
        }

        private HiraganaSequenceTree MakeHiraganaSequenceTreeStartWith((HiraganaSet, List<HiraganaSet>) hiraganaSets)
        {
            var result = new HiraganaSequenceTree();
            foreach (var set in hiraganaSets.Item2)
            {
                if (string.IsNullOrEmpty(set.Hiragana))
                {
                    result.Tree.Add((set, new HiraganaSequenceTree()));
                }
                else
                {
                    var key = set.Hiragana.Substring(0, 2);
                    if (!MapHistoryHiraganaSequence.TryGetValue(key, out var dic))
                    {
                        result.Tree.Add((set, new HiraganaSequenceTree()));
                    }
                    else
                    {
                        if (dic.TryGetValue(set.Hiragana, out var lst))
                        {
                            result.Tree.Add((set, MakeHiraganaSequenceTreeStartWith(lst)));
                        }
                        else
                        {
                            result.Tree.Add((set, new HiraganaSequenceTree()));
                        }
                    }

                }
            }

            return result;
        }

        // ひらがな2文字をキーとした、日本語文節とそれに続く文節リストの対応マップ
        public Dictionary<string, Dictionary<string, (HiraganaSet, List<HiraganaSet>)>> MapHistoryHiraganaSequence { get; set; } = new Dictionary<string, Dictionary<string, (HiraganaSet, List<HiraganaSet>)>>();

    }

    internal record HiraganaSet(string Hiragana, string Phrase);

    internal class HiraganaSequenceTree
    {
        public List<(HiraganaSet, HiraganaSequenceTree)> Tree { get; set; } = new List<(HiraganaSet, HiraganaSequenceTree)>();

        public List<List<HiraganaSet>> Take(int n)
        {
            throw new NotImplementedException();
        }


    }

}

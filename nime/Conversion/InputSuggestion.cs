using GoodSeat.Nime.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodSeat.Nime.Conversion
{
    internal class InputSuggestion
    {
        // TODO!:提案文節選択終了直後の場合、最後に選択した提案文節に続く候補として記録。
        // TODO!:文字入力直後の場合、最後に入力された文節に続く候補として記録。
        public Task RegisterHiraganaSequenceAsync(ConvertCandidate convertCandidate)
        {
            return Task.Run(() => {
                Debug.WriteLine("start:RegisterHiraganaSequenceAsync:" + convertCandidate.GetSelectedSentence());
                RegisterHiraganaSequence(convertCandidate.PhraseList.Select(p => new HiraganaSet(p.OriginalHiragana, p.Selected)).ToList());
                Debug.WriteLine("end:RegisterHiraganaSequenceAsync:" + convertCandidate.GetSelectedSentence());
            });
        }

        public void RegisterHiraganaSequence(List<HiraganaSet> values)
        {
            if (values.Count == 0) return;

            var set = values.FirstOrDefault();
            if (set.Hiragana.Any(c => !Utility.IsHiragana(c))) return; // TODO:暫定対応

            values.RemoveAt(0);
            if (values.Count != 0 && (values[0].Hiragana == "。" || values[0].Hiragana == "、"))
            {
                set = new HiraganaSet(set.Hiragana + values[0].Hiragana, set.Phrase + values[0].Phrase);
                values.RemoveAt(0);
            }

            var (hiragana, phrase) = set;
            var key = hiragana.Substring(0, 2);
            var nextSet = values.FirstOrDefault() ?? new HiraganaSet("", "");

            Dictionary<string, List<Page>> dic;
            if (!MapHistoryHiraganaSequence.TryGetValue(key, out dic))
            {
                dic = new Dictionary<string, List<Page>>();
                MapHistoryHiraganaSequence.Add(key, dic);
            }

            if (dic.TryGetValue(set.Hiragana, out var list))
            {
                var page = list.FirstOrDefault(p => p.Word == set);
                if (page != null)
                {
                    if (!page.NextCandidate.Contains(nextSet)) page.NextCandidate.Add(nextSet);
                    list.Remove(page);
                    list.Add(new Page(set, page.NextCandidate, DateTime.Now));
                }
                else
                {
                    list.Add(new Page(set, new List<HiraganaSet> { nextSet }, DateTime.Now));
                }
            }
            else
            {
                dic.Add(set.Hiragana, new List<Page>() { new Page(set, new List<HiraganaSet>() { nextSet }, DateTime.Now) });
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

        public Task<HiraganaSequenceTree> SearchStartWithAsync(HiraganaSet hiraganaSet, int depth)
        {
            return Task.Run(() =>
            {
                var result = new HiraganaSequenceTree();

                var key = hiraganaSet.Hiragana.Substring(0, 2);
                if (!MapHistoryHiraganaSequence.TryGetValue(key, out var dic)) return result;

                if (dic.TryGetValue(hiraganaSet.Hiragana, out var pages))
                {
                    foreach (var page in pages)
                    {
                        if (page.Word != hiraganaSet) continue;

                        var tree = MakeHiraganaSequenceTreeStartWith(page, depth);
                        result.Tree.Add((page.Word, tree));
                    }
                }

                return result;
            });
        }

        public Task<HiraganaSequenceTree?> SearchStartWithAsync(string hiragana, int depth)
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

                        foreach (var val in pair.Value)
                        {
                            var tree = MakeHiraganaSequenceTreeStartWith(val, depth);
                            result.Tree.Add((val.Word, tree));
                        }
                    }
                }

                return result;
            });
        }

        private HiraganaSequenceTree MakeHiraganaSequenceTreeStartWith(Page page, int depth)
        {
            var result = new HiraganaSequenceTree();

            if (depth > 0)
            {
                foreach (var set in page.NextCandidate)
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
                                var nextPage = lst.FirstOrDefault(p => p.Word == set);
                                if (nextPage != null) result.Tree.Add((set, MakeHiraganaSequenceTreeStartWith(nextPage, depth - 1)));
                                else result.Tree.Add((set, new HiraganaSequenceTree()));
                            }
                            else
                            {
                                result.Tree.Add((set, new HiraganaSequenceTree()));
                            }
                        }

                    }
                }
            }

            return result;
        }

        // ひらがな2文字をキーとした、ひらがな(日本語文節)とそれに続く文節リストの対応マップ
        public Dictionary<string, Dictionary<string, List<Page>>> MapHistoryHiraganaSequence { get; set; } = new Dictionary<string, Dictionary<string, List<Page>>>();

    }

    internal record Page(HiraganaSet Word, List<HiraganaSet> NextCandidate, DateTime LastUsed);

    internal record HiraganaSet(string Hiragana, string Phrase);

    internal class HiraganaSequenceTree
    {
        // 最終利用日の新しい順にソートしてセット
        public List<(HiraganaSet, HiraganaSequenceTree)> Tree { get; set; } = new List<(HiraganaSet, HiraganaSequenceTree)>();

        public List<List<HiraganaSet>> Take(int n)
        {
            throw new NotImplementedException();
        }


    }

}

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
        public List<HiraganaSet> ToHiraganaSetList(ConvertCandidate convertCandidate)
        {
            var lst = convertCandidate.PhraseList.Select(p => new HiraganaSet(p.OriginalHiragana, p.Selected)).ToList();
            var lst_ = lst.Aggregate(new List<HiraganaSet>(), (lstAcc, h) =>
            {
                if (lstAcc.Count == 0)
                {
                    lstAcc.Add(h);
                }
                else
                {
                    var hl = lstAcc[lstAcc.Count - 1];
                    if ((hl.Hiragana.Length <= 3 && !hl.Hiragana.Contains("。") && !hl.Hiragana.Contains("、"))
                     || (h.Hiragana == h.Phrase && h.Hiragana.Length <= 2))
                    {
                        var hnew = new HiraganaSet(hl.Hiragana + h.Hiragana, hl.Phrase + h.Phrase);
                        lstAcc.RemoveAt(lstAcc.Count - 1);
                        lstAcc.Add(hnew);
                    }
                    else
                    {
                        lstAcc.Add(h);
                    }
                }
                return lstAcc;
            });
            return lst_;
        }

        public Task RegisterHiraganaSequenceAsync(ConvertCandidate convertCandidate, HiraganaSet? lastPhrase = null)
        {
            return Task.Run(() => {
                Debug.WriteLine("start:RegisterHiraganaSequenceAsync:" + convertCandidate.GetSelectedSentence());
                var lst = ToHiraganaSetList(convertCandidate);
                if (lastPhrase != null && !lastPhrase.Hiragana.EndsWith('。')) lst.Insert(0, lastPhrase);

                RegisterHiraganaSequence(lst);
                Debug.WriteLine("end:RegisterHiraganaSequenceAsync:" + convertCandidate.GetSelectedSentence());
            });
        }

        public Task RegisterHiraganaSequenceAsync(List<HiraganaSet> values)
        {
            return Task.Run(() => {
                var vals = values.ToList();
                Debug.WriteLine("start:RegisterHiraganaSequenceAsync:" + values.Select(h => h.Phrase).Aggregate((s1, s2) => s1 + s2));
                RegisterHiraganaSequence(vals);
                Debug.WriteLine("end:RegisterHiraganaSequenceAsync:" + values.Select(h => h.Phrase).Aggregate((s1, s2) => s1 + s2));
            });
        }

        private void RegisterHiraganaSequence(List<HiraganaSet> values)
        {
            if (values.Count == 0) return;

            var set = values.FirstOrDefault();
            //if (set.Hiragana.Any(c => !Utility.IsHiragana(c))) return; // TODO:暫定対応

            values.RemoveAt(0);
            var (hiragana, phrase) = set;
            var key = SubstringKey(hiragana);
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


        private IEnumerable<string> NextCandidateFrom(char romaji, bool alsoxtu)
        {
            foreach (var t in new List<string>{ "a", "i", "u", "e", "o" }) 
            {
                var th = Microsoft.International.Converters.KanaConverter.RomajiToHiragana(romaji + t);
                if (!string.IsNullOrEmpty(th) && th.All(Utility.IsHiragana))
                {
                    yield return th;

                    if (alsoxtu) yield return "っ" + th;
                }
            }

            if (romaji == 'n') yield return "ん";
            else if (romaji == 'x' || romaji == 'l')
            { 
                yield return "ゃ";
                yield return "ゅ";
                yield return "ょ";
                yield return "っ";
            }
        }
        private IEnumerable<string> NextCandidateFrom(string romaji)
        {
            if (romaji.Length == 1)
            {
                foreach (var t in NextCandidateFrom(romaji[0], true)) yield return t;
            }
            else if (romaji.Length == 2 && romaji[0] == romaji[1])
            {
                foreach (var n in NextCandidateFrom(romaji[0], false))
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

        public Task<HiraganaSequenceTree?> SearchPostOfAsync(HiraganaSet hiraganaSet, int depth)
        {
            return Task.Run(() =>
            {
                var key = SubstringKey(hiraganaSet.Hiragana);
                if (!MapHistoryHiraganaSequence.TryGetValue(key, out var dic)) return null;

                if (dic.TryGetValue(hiraganaSet.Hiragana, out var pages))
                {
                    foreach (var page in pages)
                    {
                        if (page.Word != hiraganaSet) continue;

                        return MakeHiraganaSequenceTreeStartWith(page, depth);
                    }
                }

                return null;
            });
        }

        public Task<HiraganaSequenceTree?> SearchStartWithAsync(string hiragana, int depth)
        {
            return Task.Run(() =>
            {
                if (string.IsNullOrEmpty(hiragana) || hiragana[0] == '。' || hiragana[0] == '、') return null;

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

                List<HiraganaSequenceTree> lst = new List<HiraganaSequenceTree>();
                foreach (var candidate in candidates)
                {
                    var key = SubstringKey(candidate);

                    if (!MapHistoryHiraganaSequence.TryGetValue(key, out var dic)) continue;

                    foreach (var pair in dic)
                    {
                        var hs = pair.Key;
                        if (hs.StartsWith(candidate))
                        {
                            foreach (var page in pair.Value)
                            {
                                lst.Add(MakeHiraganaSequenceTreeStartWith(page, depth));
                            }
                        }
                        else if (candidate.StartsWith(hs))
                        {
                            var hNext = candidate.Substring(hs.Length);
                            var tree = SearchStartWithAsync(hNext, depth).Result;
                            if (tree != null)
                            {
                                lst.AddRange(tree.Children.SelectMany((hset) => {
                                    return pair.Value.Select(page =>
                                    {
                                        var w = new HiraganaSet(page.Word.Hiragana + hset.Word.Hiragana, page.Word.Phrase + hset.Word.Phrase);
                                        List<HiraganaSet>? l = hset.ConsistPhrases;
                                        if (l == null) l = new List<HiraganaSet>();
                                        l.Insert(0, page.Word);
                                        return new HiraganaSequenceTree(w, page.LastUsed, hset.Children, l);
                                    });
                                }));
                            }
                        }
                    }
                }

                if (lst.Any()) return new HiraganaSequenceTree(new HiraganaSet("", ""), DateTime.Now, lst, null);
                return null;
            });
        }

        private HiraganaSequenceTree MakeHiraganaSequenceTreeStartWith(Page page, int depth)
        {
            var lstAdd = new List<HiraganaSequenceTree>();
            if (depth > 0)
            {
                foreach (var set in page.NextCandidate)
                {
                    if (string.IsNullOrEmpty(set.Hiragana)) continue;

                    var key = SubstringKey(set.Hiragana);

                    if (!MapHistoryHiraganaSequence.TryGetValue(key, out var dic)) continue;

                    if (!dic.TryGetValue(set.Hiragana, out var lstPage)) continue;

                    var nextPage = lstPage.FirstOrDefault(p => p.Word == set);
                    if (nextPage == null) continue;

                    lstAdd.Add(MakeHiraganaSequenceTreeStartWith(nextPage, depth - 1));
                }
            }

            return new HiraganaSequenceTree(page.Word, page.LastUsed, lstAdd, new List<HiraganaSet> { page.Word });
        }

        string SubstringKey(string hiragana)
        {
            if (hiragana.Length <= 2) return hiragana;
            return hiragana.Substring(0, 2);
        }

        // 先頭のひらがな2文字をキーとした、ひらがな(日本語文節)とそれに続く文節リストの対応マップ
        public Dictionary<string, Dictionary<string, List<Page>>> MapHistoryHiraganaSequence { get; set; } = new Dictionary<string, Dictionary<string, List<Page>>>();

    }

    internal record HiraganaSet(string Hiragana, string Phrase);

    internal record Page(HiraganaSet Word, List<HiraganaSet> NextCandidate, DateTime LastUsed);

    internal class HiraganaSequenceTree
    {
        // 最終利用日の新しい順にソートしてセット
        internal HiraganaSequenceTree(HiraganaSet word, DateTime lastUsed, List<HiraganaSequenceTree> children, List<HiraganaSet>? consist)
        {
            Word = word;
            LastUsed = lastUsed;
            ConsistPhrases = consist;
            Children = children;
            Children.Sort((c1, c2) => (c1.LastUsed > c2.LastUsed) ? -1 : 1);
        }

        internal HiraganaSet Word { get; set; }
        internal DateTime LastUsed { get; set; }

        internal List<HiraganaSet>? ConsistPhrases { get; set; }

        // 表示用文字、続くツリー、構成文節リスト(通常はnull = 構成文節 == 表示用文字の文節一つ)
        public List<HiraganaSequenceTree> Children { get; set; } = new List<HiraganaSequenceTree>();

        public List<List<HiraganaSet>> Take(int n)
        {
            throw new NotImplementedException();
        }


    }

}

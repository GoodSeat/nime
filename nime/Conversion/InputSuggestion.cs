using GoodSeat.Nime.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodSeat.Nime.Conversion
{
    /// <summary>
    /// 入力補完機能と補完候補の保管機能を提供します。
    /// </summary>
    internal class InputSuggestion
    {
        /// <summary>
        /// 先頭のひらがな2文字をキーとした、ひらがな(日本語文節)と対応する文節継続情報リストの対応マップを設定もしくは取得します。
        /// </summary>
        public Dictionary<string, Dictionary<string, List<Page>>> MapHistoryHiraganaSequence { get; set; } = new Dictionary<string, Dictionary<string, List<Page>>>();


        /// <summary>
        /// 文章の変換候補情報からフレーズリストを生成して取得します。
        /// </summary>
        /// <param name="convertCandidate">元とする文章の変換候補情報。</param>
        /// <returns>文章の変換候補情報から生成されたフレーズリスト。</returns>
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
                    if ((hl.Hiragana.Length <= 3 && !hl.Hiragana.Contains("。") && !hl.Hiragana.Contains("、")) // 前のフレーズが短く句読点を含まない場合には結合
                     || (h.Hiragana == h.Phrase && h.Hiragana.Length <= 2)) // にて、の、から、等の接続詞は前のフレーズに結合
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

        /// <summary>
        /// 文章の変換候補情報の選択状態に基づいて、入力補完用情報に入力履歴を登録します。
        /// </summary>
        /// <param name="convertCandidate">登録用の文章の変換候補情報。</param>
        /// <param name="lastPhrase">この文章の前の文章が存在する場合、その最後の文節を指定。</param>
        /// <returns>本処理を実行するタスク。</returns>
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

        /// <summary>
        /// フレーズリストに基づいて、入力補完用情報に入力履歴を登録します。
        /// </summary>
        /// <param name="values">登録用のフレーズリスト。</param>
        /// <returns>本処理を実行するタスク。</returns>
        public Task RegisterHiraganaSequenceAsync(List<HiraganaSet> values)
        {
            return Task.Run(() => {
                var vals = values.ToList();
                Debug.WriteLine("start:RegisterHiraganaSequenceAsync:" + values.Select(h => h.Phrase).Aggregate((s1, s2) => s1 + s2));
                RegisterHiraganaSequence(vals);
                Debug.WriteLine("end:RegisterHiraganaSequenceAsync:" + values.Select(h => h.Phrase).Aggregate((s1, s2) => s1 + s2));
            });
        }

        /// <summary>
        /// フレーズリストに基づいて、入力補完用情報に入力履歴を登録します。
        /// </summary>
        /// <param name="values">登録用のフレーズリスト。</param>
        private void RegisterHiraganaSequence(List<HiraganaSet> values)
        {
            if (values.Count == 0) return;

            var word = values.FirstOrDefault();
            //if (set.Hiragana.Any(c => !Utility.IsHiragana(c))) return; // TODO:暫定対応

            values.RemoveAt(0);
            var (hiragana, phrase) = word;
            var key = SubstringKey(hiragana);

            Dictionary<string, List<Page>> dic;
            if (!MapHistoryHiraganaSequence.TryGetValue(key, out dic))
            {
                dic = new Dictionary<string, List<Page>>();
                MapHistoryHiraganaSequence.Add(key, dic);
            }

            var nextWord = values.FirstOrDefault() ?? new HiraganaSet("", "");
            if (dic.TryGetValue(word.Hiragana, out var list))
            {
                var page = list.FirstOrDefault(p => p.Word == word);
                if (page != null)
                {
                    if (!page.NextCandidate.Contains(nextWord)) page.NextCandidate.Add(nextWord);
                    list.Remove(page);
                    list.Add(new Page(word, page.NextCandidate, DateTime.Now));
                }
                else
                {
                    list.Add(new Page(word, new List<HiraganaSet> { nextWord }, DateTime.Now));
                }
            }
            else
            {
                dic.Add(word.Hiragana, new List<Page>() { new Page(word, new List<HiraganaSet>() { nextWord }, DateTime.Now) });
            }

            if (values.Count != 0) RegisterHiraganaSequence(values);
        }



        /// <summary>
        /// 履歴の登録情報を元に、指定のフレーズに続くフレーズツリーを生成して取得します。
        /// </summary>
        /// <param name="preWord">前のフレーズ。</param>
        /// <param name="depth">フレーズツリーの構築深さ。</param>
        /// <returns>生成されたフレーズツリー。該当フレーズの記録がなく、フレーズツリーを構築できない場合にはnull。</returns>
        public Task<HiraganaSequenceTree?> SearchPostOfAsync(HiraganaSet preWord, int depth)
        {
            return Task.Run(() =>
            {
                var key = SubstringKey(preWord.Hiragana);
                if (!MapHistoryHiraganaSequence.TryGetValue(key, out var dic)) return null;

                if (dic.TryGetValue(preWord.Hiragana, out var pages))
                {
                    foreach (var page in pages.ToList()) // MEMO:そのままforeachすると稀にコレクション変化で異常終了してしまうため
                    {
                        if (page.Word != preWord) continue;

                        return MakeHiraganaSequenceTreeStartWith(page, depth);
                    }
                }

                return null;
            });
        }

        /// <summary>
        /// 履歴の登録情報を元に、指定のフレーズから始まるフレーズツリーを生成して取得します。
        /// </summary>
        /// <param name="word">開始フレーズ。</param>
        /// <param name="depth">フレーズツリーの構築深さ。</param>
        /// <returns>生成されたフレーズツリー。該当フレーズの記録がなく、フレーズツリーを構築できない場合にはnull。</returns>
        public Task<HiraganaSequenceTree?> SearchStartWithAsync(string word, int depth)
        {
            return Task.Run(() =>
            {
                if (string.IsNullOrEmpty(word) || word[0] == '。' || word[0] == '、') return null;

                int n = word.TakeWhile(Utility.IsHiragana).Count();
                if (n == 0) return null;

                var hiraganaDet = word.Substring(0, n);
                var hiraganaMid = word.Substring(n);

                var candidates = new List<string>();
                if (string.IsNullOrEmpty(hiraganaMid))
                {
                    candidates.Add(hiraganaDet);
                }
                else
                {
                    candidates.AddRange(NextCandidateFrom(hiraganaMid).Select(t => hiraganaDet + t));
                }
                //if (candidates.Count == 0) candidates.Add(hiraganaDet);

                List<HiraganaSequenceTree> lst = new List<HiraganaSequenceTree>();
                foreach (var candidate in candidates)
                {
                    var key = SubstringKey(candidate);
                    if (!MapHistoryHiraganaSequence.TryGetValue(key, out var dic)) continue;

                    foreach (var pair in dic.ToList()) // MEMO:そのままdicを対象にループすると、稀にコレクション変化で異常終了してしまう。原因は調べていないので対症療法
                    {
                        var hs = pair.Key;
                        if (hs.StartsWith(candidate))
                        {
                            foreach (var page in pair.Value.ToList())
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
                                        var l = new List<HiraganaSet>();
                                        if (hset.ConsistPhrases != null) l.AddRange(hset.ConsistPhrases);
                                        l.Insert(0, page.Word);
                                        return new HiraganaSequenceTree(w, page.LastUsed, hset.Children, l);
                                    });
                                }));
                            }
                        }
                    }
                }

                HiraganaSequenceTree.MergeList(lst);

                if (lst.Any()) return new HiraganaSequenceTree(new HiraganaSet("", ""), DateTime.Now, lst, null);
                return null;
            });
        }

        /// <summary>
        /// 指定の文節継続情報のフレーズから始まるフレーズツリーを生成して取得します。
        /// </summary>
        /// <param name="page">開始フレーズを示す文節継続情報。</param>
        /// <param name="depth">フレーズツリーの構築深さ。</param>
        /// <returns>生成されたフレーズツリー。</returns>
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

        /// <summary>
        /// 指定のひらがな文字列に対して、<see cref="MapHistoryHiraganaSequence"/>にてキーとして扱うキー文字列を取得します。
        /// </summary>
        /// <param name="hiragana">判定対象のひらがな文字列。</param>
        /// <returns>キー文字列。</returns>
        string SubstringKey(string hiragana)
        {
            if (hiragana.Length <= 2) return hiragana;
            return hiragana.Substring(0, 2);
        }

        /// <summary>
        /// 入力途中のローマ字の文字列に対して、ひらがな候補を列挙します。
        /// </summary>
        /// <param name="romaji">入力途上のローマ字。</param>
        /// <returns>候補となるひらがな文字列の列挙。</returns>
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
        /// <summary>
        /// 指定のローマ字に対して、ひらがな候補を列挙します。
        /// </summary>
        /// <param name="romaji">ローマ字。</param>
        /// <param name="alsoxtu">「っ」も候補に含めるか否か。</param>
        /// <returns>候補となるひらがな文字列の列挙。</returns>
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

    }


    /// <summary>
    /// フレーズ（ひらがな文字列と変換後文字列から成るレコード）を表します。
    /// </summary>
    /// <param name="Hiragana">ひらがな文字列。</param>
    /// <param name="Phrase">変換後文字列。</param>
    internal record HiraganaSet(string Hiragana, string Phrase);

    /// <summary>
    /// フレーズと、それに続く候補フレーズリスト及び当該フレーズの最終利用時間の情報を表します。
    /// </summary>
    /// <param name="Word">対象フレーズ。</param>
    /// <param name="NextCandidate"><paramref name="Word"/>に続く候補フレーズリスト。</param>
    /// <param name="LastUsed">対象フレーズの最終利用時間。</param>
    internal record Page(HiraganaSet Word, List<HiraganaSet> NextCandidate, DateTime LastUsed);


    /// <summary>
    /// 特定フレーズとそれに続く候補フレーズのフレーズツリーから成るフレーズツリーを表します。
    /// </summary>
    internal class HiraganaSequenceTree
    {
        /// <summary>
        /// フレーズツリーを初期化します。
        /// </summary>
        /// <param name="word">開始フレーズ。</param>
        /// <param name="lastUsed">開始フレーズの最終利用時間。</param>
        /// <param name="children">対象フレーズに続く候補となるフレーズツリーリスト。</param>
        /// <param name="consist">開始フレーズが複数フレーズの合成から成る場合、その構成フレーズリストを指定。</param>
        internal HiraganaSequenceTree(HiraganaSet word, DateTime lastUsed, List<HiraganaSequenceTree> children, List<HiraganaSet>? consist)
        {
            Word = word;
            LastUsed = lastUsed;
            ConsistPhrases = consist;
            Children = children;
            Children.Sort((c1, c2) => {
                if (c1.IsCompositePhrase && !c2.IsCompositePhrase) return 1;
                if (!c1.IsCompositePhrase && c2.IsCompositePhrase) return -1;

                if (c1.IsCompositePhrase)
                {
                    var compLength = c1.ConsistPhrases[0].Hiragana.Length - c2.ConsistPhrases[0].Hiragana.Length;
                    if (compLength != 0) return -compLength;

                    var compCount = c1.ConsistPhrases.Count - c2.ConsistPhrases.Count;
                    if (compCount != 0) return compCount;
                }
                return (c1.LastUsed > c2.LastUsed) ? -1 : 1; // 最終利用日の新しい順にソート
            });
        }

        /// <summary>
        /// 対象となる開始フレーズを設定もしくは取得します。
        /// </summary>
        internal HiraganaSet Word { get; set; }
        /// <summary>
        /// 開始フレーズの最終利用時間を設定もしくは取得します。
        /// </summary>
        internal DateTime LastUsed { get; set; }

        /// <summary>
        /// 開始フレーズが複数フレーズの合成から成る場合、その構成フレーズリストを設定もしくは取得します。単一フレーズの場合にはnullとなります。
        /// </summary>
        internal List<HiraganaSet>? ConsistPhrases { get; set; }

        /// <summary>
        /// 対象となる開始フレーズの日本語文字列を取得します。但し、当該フレーズが複数フレーズの合成から成る場合には、フレーズ間にスペースを入れた文字列を返します。
        /// </summary>
        internal string PhraseSplitEachConsist
        {
            get
            {
                if (!IsCompositePhrase) return Word.Phrase;
                return string.Join(" ", ConsistPhrases.Select(c => c.Phrase).ToList());
            }
        }

        /// <summary>
        /// 対象となる開始フレーズが複数フレーズの合成から成るか否かを判定します。
        /// </summary>
        internal bool IsCompositePhrase
        {
            get => ConsistPhrases != null && ConsistPhrases.Count > 1;
        }

        /// <summary>
        /// 対象フレーズに続く候補となるフレーズツリーリストを設定もしくは取得します。
        /// </summary>
        public List<HiraganaSequenceTree> Children { get; set; } = new List<HiraganaSequenceTree>();

        /// <summary>
        /// 指定のフレーズツリーが同一の開始フレーズを対象とする場合に、後続フレーズツリーの情報をマージします。
        /// </summary>
        /// <param name="other">マージ対象とするフレーズツリー。</param>
        /// <returns>マージ処理を行ったか否か（＝開始フレーズが同一であったか否か）。</returns>
        bool MergeWith(HiraganaSequenceTree other)
        {
            if (Word != other.Word) return false;

            if (LastUsed == other.LastUsed && Children.Count == other.Children.Count)
            {
                return true;
            }
            LastUsed = LastUsed > other.LastUsed ? LastUsed : other.LastUsed;

            if (ConsistPhrases == null) ConsistPhrases = other.ConsistPhrases;
            else if (other.ConsistPhrases != null && other.ConsistPhrases.Count > ConsistPhrases.Count)  ConsistPhrases = other.ConsistPhrases;

            Children.AddRange(other.Children);
            MergeList(Children);

            return true;
        }


        /// <summary>
        /// フレーズツリーのリスト内の重複情報同士をマージして削除します。
        /// </summary>
        /// <param name="lst">処理対象とするフレーズツリーのリスト。</param>
        public static void MergeList(List<HiraganaSequenceTree> lst)
        {
            for (int i = 0; i < lst.Count; i++)
            {
                var h1 = lst[i];
                for (int j = i + 1; j < lst.Count; j++)
                {
                    var h2 = lst[j];
                    if (h1.MergeWith(h2))
                    {
                        lst.RemoveAt(j);
                        j--;
                    }
                }
            }
        }


        public override string ToString()
        {
            return "HiraganaSequenceTree:" + PhraseSplitEachConsist + (Children.Any() ? " ~" : "");
        }


    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodSeat.Nime.Conversion
{
    [Serializable]
    internal class InputHistory
    {
        
        public void Register(string inputHiragana, string confirmedPhrase)
        {
            List<string> list;
            if (!InputHistoryMap.TryGetValue(inputHiragana, out list))
            {
                list = new List<string>();
                InputHistoryMap.Add(inputHiragana, list);
            }

            if (list.Contains(confirmedPhrase)) list.Remove(confirmedPhrase);
            list.Add(confirmedPhrase);
        }

        public void Unregister(string inputHiragana, string confirmeedPhrase)
        {
            if (InputHistoryMap.TryGetValue(inputHiragana, out List<string> list))
            {
                list.Remove(confirmeedPhrase);
            }
        }

        /// <summary>
        /// 指定のひらがなの文節に対して確定されたもっとも最近の文節を取得します。
        /// </summary>
        /// <param name="inputHiragana">元の入力ひらがな文字列。</param>
        /// <returns>確定された最も最近の文節。該当がない場合にはnull。</returns>
        public string? GetRecentryPharaseFor(string inputHiragana)
        {
            if (!InputHistoryMap.TryGetValue(inputHiragana, out List<string> list)) return null;
            if (list.Count == 0) return null;
            return list.Last();
        }

        /// <summary>
        /// 指定のひらがなの文節に対して確定された文節を時系列の新しい順に列挙します。
        /// </summary>
        /// <param name="inputHiragana">元の入力ひらがな文字列。</param>
        /// <returns>確定された文節の列挙(時系列の新しい順)。</returns>
        public IEnumerable<string> GetConfirmedPharaseFor(string inputHiragana)
        {
            if (!InputHistoryMap.TryGetValue(inputHiragana, out List<string> list)) yield break;
            if (list.Count == 0) yield break;

            foreach (var p in list.Reverse<string>()) yield return p;
        }

        /// <summary>
        /// 文節に対する元ひらがなと確定文字列のリスト（古い順に格納）の対応マップを設定もしくは取得します。
        /// </summary>
        public Dictionary<string, List<string>> InputHistoryMap { get; set; } = new Dictionary<string, List<string>>();

    }
}

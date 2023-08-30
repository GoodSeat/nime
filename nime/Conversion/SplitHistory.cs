using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodSeat.Nime.Conversion
{
    internal class SplitHistory
    {

        public void RegisterHistory(string preEdit, string postEdit)
        {
            var pres  = preEdit.Split(',').ToList();
            var posts = postEdit.Split(',').ToList();

            var needRegister = new List<SplitRecord>();
            do
            {
                string pre = pres[0];
                int npre = 1;
                string post = posts[0];
                int npost = 1;

                while (pre != post)
                {
                    if (pre.Length < post.Length)
                    {
                        pre += pres[npre++];
                    }
                    else
                    {
                        post += posts[npost++];
                    }
                }

                if (npre != 1 || npost != 1)
                {
                    var rec = new SplitRecord(string.Join("", pres.Take(npre).ToArray()), string.Join(',', posts.Take(npost).ToArray()));
                    needRegister.Add(rec);
                }
                for (int i = 0; i < npre; ++i) pres.RemoveAt(0);
                for (int i = 0; i < npost; ++i) posts.RemoveAt(0);
            }
            while (pres.Count > 0);

            foreach (var rec in needRegister)
            {
                var key = rec.OriginalSplitHiraganaText;
                var key0 = key.Substring(0, 2);

                RecordMap.TryGetValue(key0, out var dict);
                if (dict == null)
                {
                    dict = new Dictionary<string, SplitRecord>();
                    RecordMap.Add(key0, dict);
                }

                if (dict.ContainsKey(key)) dict.Remove(key);
                dict.Add(key, rec);
            }
        }

        public string SplitConsiderHisory(string splitHiragana)
        {
            string result = "";
            for (int i = 0; i < splitHiragana.Length; i++)
            {
                if (splitHiragana[i] == ',' || i == splitHiragana.Length - 1)
                {
                    result += splitHiragana[i];
                    continue;
                }

                string key0 = splitHiragana.Substring(i, 2);
                if (key0[1] == ',' && i < splitHiragana.Length - 2)
                {
                    key0 = key0[0].ToString() + splitHiragana[i + 2].ToString();
                }

                if (!RecordMap.TryGetValue(key0, out var dict))
                {
                    result += splitHiragana[i];
                    continue;
                }

                var key = splitHiragana.Substring(i, 2);
                for (int j = i + 2; j < splitHiragana.Length; ++j)
                {
                    var cj = splitHiragana[j];
                    if (cj == ',')
                    {
                        if (dict.TryGetValue(key, out var rec))
                        {
                            result += rec.ConfirmedSplitHiraganaText + ",";
                            i += rec.ConfirmedSplitHiraganaText.Length;
                            key = "";
                            break;
                        }
                    }
                    else
                    {
                        key += cj;
                    }
                }

                if (!string.IsNullOrEmpty(key))
                {
                    if (dict.TryGetValue(key, out var rec))
                    {
                        result += rec.ConfirmedSplitHiraganaText + ",";
                        i += rec.ConfirmedSplitHiraganaText.Length;
                        continue;
                    }
                }

                result += splitHiragana[i];
            }

            return result;
        }

        /// <summary>
        /// 区切り位置を示すカンマを除く先頭のひらがな2文字と、それに対応するレコードマップ（編集前ひらがな文字列(カンマを除く)－<see cref="SplitRecord"/>）。
        /// </summary>
        public Dictionary<string, Dictionary<string, SplitRecord>> RecordMap { get; set; } = new Dictionary<string, Dictionary<string, SplitRecord>>();
    }

    /// <summary>
    /// 分割位置編集内容のレコードを表します。
    /// </summary>
    /// <param name="OriginalSplitHiraganaText">分割位置編集前のひらがな文字列(カンマを除く)。</param>
    /// <param name="ConfirmedSplitHiraganaText">分割位置編集後のひらがな文字列。</param>
    internal record SplitRecord
        (
        string OriginalSplitHiraganaText,
        string ConfirmedSplitHiraganaText
        );

    }

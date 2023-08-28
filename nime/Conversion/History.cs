using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodSeat.Nime.Conversion
{
    internal class History
    {

        public void Register(string inputHiragana, string confirmedPhrase)
        {
        }

        /// <summary>
        /// 変換ウインドウ上で変換後に確定された文節を時系列の新しい順に列挙します。
        /// </summary>
        /// <param name="input">元の入力ひらがな文字列。</param>
        /// <returns>変換ウインドウ上で変換後に確定された文節の列挙(時系列の新しい順)。</returns>
        public IEnumerable<string> GetConfirmedPharaseFor(string inputHiragana)
        {
            throw new NotImplementedException();
        }

    }
}

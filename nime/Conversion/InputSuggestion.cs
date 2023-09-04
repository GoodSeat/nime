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
            throw new NotImplementedException();
        }

        public Task<HiraganaSequenceTree> SearchStartWithAsync(string hiragana)
        {
            throw new NotImplementedException();
        }

        // ひらがな2文字をキーとした、日本語文節とそれに続く文節リストの対応マップ
        Dictionary<string, Dictionary<HiraganaSet, List<HiraganaSet>>> _mapHistoryHiraganaSequence;

    }

    internal record HiraganaSet(string Hiragana, string Phrase);

    internal class HiraganaSequenceTree
    {

        public List<List<HiraganaSet>> Take(int n)
        {
            throw new NotImplementedException();
        }


    }

}

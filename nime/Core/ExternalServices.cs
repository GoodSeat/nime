using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodSeat.Nime.Core
{
    /// <summary>
    /// 外部サービスを利用する各種メソッドを提供します。
    /// </summary>
    internal class ExternalServices
    {
        /// <summary>
        /// 指定の英単語に対応する和訳データを問い合わせます。該当データがない場合にはnullを返します。
        /// </summary>
        /// <param name="english">問い合わせる英単語。</param>
        /// <param name="timeout">タイムアウト時間(ms)。</param>
        /// <returns>和訳データ。該当がない場合にはnull。</returns>
        public static string GetDictorynaryDataFromEnglishToJapanese(string english, int timeout)
        {
            using (var client = new HttpClient())
            {
                var txtReq = $"https://api.excelapi.org/dictionary/enja?word={english.TrimEnd(',', '.').ToLower()}";

                var httpsResponse = client.GetAsync(txtReq);

                for (int i = 0; i < timeout; i++)
                {
                    if (httpsResponse.IsCompleted)
                    {
                        return httpsResponse.Result.Content.ReadAsStringAsync().Result;
                    }
                    Thread.Sleep(1);
                }
                return null;
            }
        }
    }
}

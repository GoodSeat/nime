using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading.Tasks;

namespace GoodSeat.Nime.Conversion
{
    public static class ConvertHiraganaToSentence
    {
        internal static ConvertCandidate? Request(string txtHiragana, int timeout, InputHistory inputHistory)
        {
            using (var client = new HttpClient())
            {
                var txtReq = $"http://www.google.com/transliterate?langpair=ja-Hira|ja&text=" + txtHiragana;
                Debug.WriteLine("get:" + txtReq);

                var httpsResponse = client.GetAsync(txtReq);
                Task<string> responseContent = null;

                for (int i = 0; i < timeout; i++)
                {
                    if (httpsResponse.IsCompleted)
                    {
                        responseContent = httpsResponse.Result.Content.ReadAsStringAsync();
                        break;
                    }
                    Thread.Sleep(1);
                }
                if (responseContent == null)
                {
                    return null; // TODO:本来は、とりあえずひらがな、カタカナを返すか、InputHistoryに基づいて結果を返してほしい
                }

                Debug.WriteLine("return:" + responseContent?.ToString());
                //DeviceOperator.InputText(responseContent);

                var options = new JsonSerializerOptions
                {
                    Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                    WriteIndented = true
                };

                var ans = JsonSerializer.Deserialize<JsonResponse>("{ \"Strings\":" + responseContent.Result + " }", options);
                if (ans == null) return null;

                return new ConvertCandidate(ans, inputHistory);
            }
        }

    }
}

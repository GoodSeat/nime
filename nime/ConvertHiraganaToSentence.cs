using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading.Tasks;

namespace GoodSeat.Nime
{
    public static class ConvertHiraganaToSentence
    {
        public static ConvertCandidate Request(string txtHiragana)
        {
            using (var client = new HttpClient())
            {
                var txtReq = $"http://www.google.com/transliterate?langpair=ja-Hira|ja&text=" + txtHiragana;
                Debug.WriteLine("get:" + txtReq);

                //var httpsResponse = await client.GetAsync(txtReq);
                //var responseContent = await httpsResponse.Content.ReadAsStringAsync();
                var httpsResponse = client.GetAsync(txtReq);
                var responseContentTask = httpsResponse.Result.Content.ReadAsStringAsync();

                var responseContent = responseContentTask.Result;
                if (responseContent == null) return null;

                Debug.WriteLine("return:" + responseContent?.ToString());
                //DeviceOperator.InputText(responseContent);

                var options = new JsonSerializerOptions
                {
                    Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                    WriteIndented = true
                };


                var ans = JsonSerializer.Deserialize<JsonResponse>("{ \"Strings\":" + responseContent + " }", options);
                if (ans == null) return null;

                return new ConvertCandidate(ans);
            }
        }

    }
}

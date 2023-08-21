using Nime.Device;
using System.Diagnostics;
using System.Runtime.Serialization.Json;
using System.Text;

namespace nime
{
    public partial class Form1 : Form
    {

        /*
         * 
         * Ctrl同時押し(トリガー、「変換」キーなどに設定可能としたい！)で呼び出し
         * マウスクリックで問答無用リセット(ホイールも、もはやマウス動いただけでもリセットすべきかも)
         * カーソルキーを追う、範囲を外れた場合や上下押下時にはリセット
         * 
         * 変換直後に再度Ctrl同時押しで変換ウインドウ呼び出し
         * 変換ウインドウでは、各文節の候補を一度に表示して、各候補に「a」「b」...「1a」…などを振って表示、firefoxのvimプラグインを参考に
         * 文節を区切るには、カーソルで移動して,入力？
         * 
         * 再度Ctrl同時押しで閉じる、入力済みの文字を消して再度入力
         * 
         */

        public Form1()
        {
            InitializeComponent();
            label1.Text = "";

            KeyboardWatcher.KeyUp += KeyboardWatcher_KeyUp;

            KeyboardWatcher.Start();
        }



        string _currentString = "";


        private async void KeyboardWatcher_KeyUp(object? sender, KeyboardWatcher.KeybordWatcherEventArgs e)
        {
            if (IMEWatcher.IsOnIME())
            {
                label1.Text = "";
                return;
            }

            if (e.Key == Nime.Device.VirtualKeys.ControlLeft || e.Key == Nime.Device.VirtualKeys.ControlRight)
            {
                label1.Text = "";
            }
            // アルファベット
            else if (e.Key >= Nime.Device.VirtualKeys.A && e.Key <= Nime.Device.VirtualKeys.Z)
            {
                label1.Text += e.Key;
            }
            // 数字

            // 削除
            else if (e.Key == Nime.Device.VirtualKeys.BackSpace)
            {
            }
            else if (e.Key == Nime.Device.VirtualKeys.Del)
            {
            }

            // 移動
            else if (e.Key == Nime.Device.VirtualKeys.Right)
            {
            }
            else if (e.Key == Nime.Device.VirtualKeys.Left)
            {
            }

            // 変換の実行
            else if (e.Key == Nime.Device.VirtualKeys.F2)
            {
                var txt = label1.Text;
                if (string.IsNullOrEmpty(txt)) return;

                for (int i = 0; i < txt.Length; i++) {
                    DeviceOperator.KeyStroke(Nime.Device.VirtualKeys.BackSpace);
                }
                label1.Text = "";

                var txtHiragana = ConvertHiragana(txt);

                using (var client = new HttpClient())
                {
                    var txtReq = $"http://www.google.com/transliterate?langpair=ja-Hira|ja&text=" + txtHiragana;
                    Debug.WriteLine("post:" + txtReq);

                    var httpsResponse = await client.PostAsync(txtReq, null);
                    var responseContent = await httpsResponse.Content.ReadAsStringAsync();

                    if (responseContent != null)
                    {
                        Debug.WriteLine("return:" + responseContent?.ToString());
                        DeviceOperator.InputText(responseContent);
                    }

                    // Response Bodyに含まれるJSONを格納するインスタンスを生成します。
                    //var answer = new Answer();
                    using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(responseContent))) {
                        //var ser = new DataContractJsonSerializer(answer.GetType());
                        //answer = ser.ReadObject(ms) as Answer;
                    }    

                }

            }
        }

        string ConvertHiragana(string text)
        {
            List<Tuple<string, string>> pairs = new List<Tuple<string, string>>()
            {
                Tuple.Create("KA", "か"),
                Tuple.Create("KI", "き"),
                Tuple.Create("KU", "く"),
                Tuple.Create("KE", "け"),
                Tuple.Create("KO", "け"),
                Tuple.Create("A", "あ"),
                Tuple.Create("I", "い"),
                Tuple.Create("U", "う"),
                Tuple.Create("E", "え"),
                Tuple.Create("O", "お"),
            };

            foreach (var pair in pairs)
            {
                text = text.Replace(pair.Item1, pair.Item2);
            }
            return text;
        }


    }
}
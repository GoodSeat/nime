using Nime.Device;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace nime
{
    public partial class Form1 : Form
    {

        /*
         * 
         * Ctrl同時押し(トリガー、「変換」キーなどに設定可能としたい！)で呼び出し
         * マウスクリックで問答無用リセット(ホイールも、もはやマウス動いただけでもリセットすべきかも)
         * 
         * 変換直後に再度Ctrl同時押しで変換ウインドウ呼び出し
         * 変換ウインドウでは、各文節の候補を一度に表示して、各候補に「f」「j」...「ab」…などを振って表示、firefoxのvimプラグインを参考に
         * 各文字間に1~9、01~99を振る。文節を区切るには、数字を打つ(文節区切りのトグル)。
         * 
         * 変換履歴は記録していって、次回選択時は優先度上げる
         * 辞書機能。選択肢の最優先に追加。出来れば辞書考慮して自動で,を挿入したい。 
         * 
         * リセット操作で閉じる、入力済みの文字を消して再度入力、但しEscだけはキャンセル
         * 
         * 
         * 「」の扱いとか、!とか?とか：とか
         * 英字キーボード、日本語キーボード
         * 
         * 変換中に入力が入ると、よろしくないところに文字列が入力されてしまう。
         *   DeviceOperator.InputText(ans.GetFirstSentence()); の入力が終わるまでに入力されたものは、一旦キャンセルして終わった後に遅延してシミュレートする。
         * 
         */

        public Form1()
        {
            InitializeComponent();
            Reset();

            KeyboardWatcher.KeyUp   += KeyboardWatcher_KeyUp;
            KeyboardWatcher.KeyDown += KeyboardWatcher_KeyDown;
            KeyboardWatcher.Start();
        }



        string _currentString = "";
        int _currentPos = 0;
        DateTime _lastShiftUp = DateTime.MinValue;

        Answer _lastAnswer;

        bool _nowConvertDetail = false;

        private void Reset()
        {
            label1.Text = "";
            label2.Text = "";
            _currentPos = 0;
            _lastAnswer = null;

            Debug.WriteLine("Reset!");
        }


        private async void KeyboardWatcher_KeyUp(object? sender, KeyboardWatcher.KeybordWatcherEventArgs e)
        {
            if (_nowConvertDetail) return;
            if (e.Key != Nime.Device.VirtualKeys.Shift && e.Key != Nime.Device.VirtualKeys.ShiftLeft && e.Key != Nime.Device.VirtualKeys.ShiftRight) return;

            var now = DateTime.Now;
            Console.WriteLine(now);

            if (now - _lastShiftUp > TimeSpan.FromMilliseconds(500))
            {
                _lastShiftUp = now;
            }
            else
            {
                if (string.IsNullOrEmpty(label1.Text) && _lastAnswer != null)
                {
                    ConvertDetailForm convertDetailForm = new ConvertDetailForm();
                    convertDetailForm.SetText(_lastAnswer.GetFirstSentence());
                    _nowConvertDetail = true;
                    convertDetailForm.ShowDialog();
                    _nowConvertDetail = false;
                }
                else if (!string.IsNullOrEmpty(label1.Text))
                {
                    // 変換の実行
                    var txt = label1.Text;
                    for (int i = 0; i < txt.Length; i++)
                    {
                        DeviceOperator.KeyStroke(Nime.Device.VirtualKeys.BackSpace);
                    }
                    Reset();

                    var txtHiragana = ConvertToHiragana(txt);

                    using (var client = new HttpClient())
                    {
                        var txtReq = $"http://www.google.com/transliterate?langpair=ja-Hira|ja&text=" + txtHiragana;
                        Debug.WriteLine("post:" + txtReq);

                        var httpsResponse = await client.PostAsync(txtReq, null);
                        var responseContent = await httpsResponse.Content.ReadAsStringAsync();

                        if (responseContent != null)
                        {
                            Debug.WriteLine("return:" + responseContent?.ToString());
                            //DeviceOperator.InputText(responseContent);

                            var options = new JsonSerializerOptions
                            {
                                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                                WriteIndented = true
                            };

                            var ans = JsonSerializer.Deserialize<Answer>("{ \"strings\":" + responseContent + " }", options);
                            if (ans != null)
                            {
                                DeviceOperator.InputText(ans.GetFirstSentence());
                            }
                            _lastAnswer = ans;
                        }
                    }
                }
            }

        }
        private void KeyboardWatcher_KeyDown(object? sender, KeyboardWatcher.KeybordWatcherEventArgs e)
        {
            if (_nowConvertDetail) return;
            if (IMEWatcher.IsOnIME())
            {
                Reset();
                return;
            }

            Debug.WriteLine(e.Key);

            if (e.Key == Nime.Device.VirtualKeys.Space || e.Key == Nime.Device.VirtualKeys.Enter)
            {
                Reset();
            }
            else if (e.Key == Nime.Device.VirtualKeys.ControlLeft || e.Key == Nime.Device.VirtualKeys.ControlRight)
            {
                Reset();
            }

            // アルファベット
            else if (e.Key >= Nime.Device.VirtualKeys.A && e.Key <= Nime.Device.VirtualKeys.Z)
            {
                label1.Text += e.Key;
                _currentPos++;
            }
            else if (e.Key == Nime.Device.VirtualKeys.Subtract || e.Key == Nime.Device.VirtualKeys.OEMMinus)
            {
                label1.Text += "ー";
                _currentPos++;
            }
            // 数字
            else if ((e.Key >= Nime.Device.VirtualKeys.D0 && e.Key <= Nime.Device.VirtualKeys.D9) ||
                     (e.Key >= Nime.Device.VirtualKeys.N0 && e.Key <= Nime.Device.VirtualKeys.N9))
            {
                label1.Text += e.Key.ToString()[1];
                _currentPos++;
            }
            else if (e.Key == Nime.Device.VirtualKeys.OEMPeriod)
            {
                label1.Text += ".";
                _currentPos++;
            }
            else if (e.Key == Nime.Device.VirtualKeys.OEMCommma)
            {
                label1.Text += ",";
                _currentPos++;
            }

            // 削除
            else if (e.Key == Nime.Device.VirtualKeys.BackSpace)
            {
                if (_currentPos <= 0)
                {
                    Reset();
                    return;
                }
                var txt = label1.Text;
                label1.Text = txt.Substring(0, _currentPos - 1) + txt.Substring(_currentPos);
                _currentPos--;
            }
            else if (e.Key == Nime.Device.VirtualKeys.Del)
            {
                if (_currentPos >= label1.Text.Length)
                {
                    Reset();
                    return;
                }
                var txt = label1.Text;
                label1.Text = txt.Substring(0, _currentPos) + txt.Substring(_currentPos + 1);
            }

            // 移動
            else if (e.Key == Nime.Device.VirtualKeys.Up || e.Key == Nime.Device.VirtualKeys.Down)
            {
                Reset();
                return;
            }
            else if (e.Key == Nime.Device.VirtualKeys.Left)
            {
                _currentPos--;
                if (_currentPos < 0)
                {
                    Reset();
                    return;
                }
            }
            else if (e.Key == Nime.Device.VirtualKeys.Right)
            {
                _currentPos++;
                if (_currentPos > label1.Text.Length)
                {
                    Reset();
                    return;
                }
            }
            else if (e.Key == Nime.Device.VirtualKeys.Shift || e.Key == Nime.Device.VirtualKeys.ShiftLeft || e.Key == Nime.Device.VirtualKeys.ShiftRight)
            {

            }
            else // 原則としてはリセットだろう…
            {
                Reset();
            }

            label2.Text = ConvertToHiragana(label1.Text);
        }

        string ConvertToHiragana(string txt)
        {
            txt = txt.Replace("NN", "ん");
            var txtHiragana = Microsoft.International.Converters.KanaConverter.RomajiToHiragana(txt);
            txtHiragana = txtHiragana.Replace(",", "、");
            txtHiragana = txtHiragana.Replace(".", "。");
            return txtHiragana;
        }

        class Answer
        {
            public List<List<object>> strings { get; set; }

            public string GetFirstSentence()
            {
                string ans = "";

                foreach (var lst in strings)
                {
                    var key = (JsonElement)lst[0];
                    var candidates = (JsonElement)lst[1];
                    ans += candidates[0].ToString();
                }

                return ans;
            }
        }




    }
}
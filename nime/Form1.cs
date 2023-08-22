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
         * Shift2連続押下(トリガー、「変換」キーなどに設定可能としたい！)で呼び出し
         * マウスクリックで問答無用リセット(ホイールも、もはやマウス動いただけでもリセットすべきかも)
         * 
         * 変換直後に再度Shift押し(あるいは時間が空いた場合には再度Shift2連続押下)で変換ウインドウ呼び出し
         * 変換ウインドウでは、各文節の候補を一度に表示して、各候補に「f」「j」...「ab」…などを振って表示、firefoxのvimプラグインを参考に
         * 各文字間にa~b、数が多い場合はaa~zzを振る。文節を区切るには、s+記号(文節区切りのトグル)、あるいはm+記号+記号(文節区切りの移動)。
         * 
         * 変換履歴は記録していって、次回選択時は優先度上げる
         * 辞書機能。選択肢の最優先に追加。出来れば辞書考慮して自動で,を挿入したい。 
         *  IMEから出力したテキストファイルをインポート。
         * 
         * リセット操作で閉じる、入力済みの文字を消して再度入力、但しEscだけはキャンセル
         * 
         * 
         * 「」の扱いとか、!とか?とか：とか
         * 全角スペースもどうしようか
         * 英字キーボード、日本語キーボード
         * せっかくなら計算機能も追加しちゃうか
         * 
         * 変換中に入力が入ると、よろしくないところに文字列が入力されてしまう。
         *   DeviceOperator.InputText(ans.GetFirstSentence()); の入力が終わるまでに入力されたものは、一旦キャンセルして終わった後に遅延してシミュレートする。
         * 
         */

        public Form1()
        {
            InitializeComponent();

            //SetStyle(ControlStyles.Selectable, false);
            Reset();

            KeyboardWatcher.KeyUp += KeyboardWatcher_KeyUp;
            KeyboardWatcher.KeyDown += KeyboardWatcher_KeyDown;
            KeyboardWatcher.Start();
        }



        string _currentString = "";
        int _currentPos = 0;
        DateTime _lastShiftUp = DateTime.MinValue;

        ConvertCandidate _lastAnswer;

        bool _nowConvertDetail = false;

        private void Reset()
        {
            _lastAnswer = null;

            if (string.IsNullOrEmpty(_labelInput.Text) && string.IsNullOrEmpty(_labelJapaneseHiragana.Text) && Opacity == 0.00) return;

            _labelInput.Text = "";
            _labelJapaneseHiragana.Text = "";
            _currentPos = 0;
            Opacity = 0.00;
        }

        private void DeleteCurrentText()
        {
            var lengthAll = _labelInput.Text.Length;
            int pos = _currentPos;
            for (int i = pos; i < lengthAll; i++)
            {
                DeviceOperator.KeyStroke(Nime.Device.VirtualKeys.Del);
            }
            for (int i = 0; i < pos; i++)
            {
                DeviceOperator.KeyStroke(Nime.Device.VirtualKeys.BackSpace);
            }
            Reset();
        }


        private /*async*/ void KeyboardWatcher_KeyUp(object? sender, KeyboardWatcher.KeybordWatcherEventArgs e)
        {
            if (_nowConvertDetail) return;
            if (IMEWatcher.IsOnIME()) return;

            if (e.Key != Nime.Device.VirtualKeys.ShiftRight) return;

            var now = DateTime.Now;
            Console.WriteLine(now);

            if (now - _lastShiftUp > TimeSpan.FromMilliseconds(500))
            {
                _lastShiftUp = now;
            }
            else
            {
                if (string.IsNullOrEmpty(_labelInput.Text) && _lastAnswer != null)
                {
                    var preTxt = _lastAnswer.GetSelectedSentence();

                    ConvertDetailForm convertDetailForm = new ConvertDetailForm();
                    convertDetailForm.SetTarget(_lastAnswer, Location);
                    _nowConvertDetail = true;
                    var result = convertDetailForm.ShowDialog();
                    if (result == DialogResult.OK && preTxt != convertDetailForm.TargetSentence.GetSelectedSentence())
                    {
                        for (int i = 0; i < preTxt.Length; i++)
                        {
                            DeviceOperator.KeyStroke(Nime.Device.VirtualKeys.BackSpace);
                        }
                        DeviceOperator.InputText(convertDetailForm.TargetSentence.GetSelectedSentence());
                    }
                    _nowConvertDetail = false;
                }
                else if (!string.IsNullOrEmpty(_labelInput.Text))
                {
                    // 変換の実行
                    var txt = _labelInput.Text;
                    DeleteCurrentText();

                    var txtHiragana = ConvertToHiragana(txt);

                    try
                    {
                        using (var client = new HttpClient())
                        {
                            var txtReq = $"http://www.google.com/transliterate?langpair=ja-Hira|ja&text=" + txtHiragana;
                            Debug.WriteLine("post:" + txtReq);

                            //var httpsResponse = await client.PostAsync(txtReq, null);
                            //var responseContent = await httpsResponse.Content.ReadAsStringAsync();
                            var httpsResponse = client.PostAsync(txtReq, null);
                            var responseContentTask = httpsResponse.Result.Content.ReadAsStringAsync();

                            var responseContent = responseContentTask.Result;
                            if (responseContent != null)
                            {
                                Debug.WriteLine("return:" + responseContent?.ToString());
                                //DeviceOperator.InputText(responseContent);

                                var options = new JsonSerializerOptions
                                {
                                    Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                                    WriteIndented = true
                                };


                                var ans = JsonSerializer.Deserialize<JsonResponse>("{ \"Strings\":" + responseContent + " }", options);
                                if (ans != null)
                                {
                                    DeviceOperator.InputText(ans.GetFirstSentence());
                                    _lastAnswer = new ConvertCandidate(ans);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        notifyIcon1.ShowBalloonTip(5000, "[nime]エラー", ex.Message, ToolTipIcon.Error);
                        Reset();
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
            if (e.Key == Nime.Device.VirtualKeys.Packet) return;

            Debug.WriteLine(e.Key);

            if (KeyboardWatcher.IsKeyLocked(Keys.LControlKey) || KeyboardWatcher.IsKeyLocked(Keys.RControlKey)
             || KeyboardWatcher.IsKeyLocked(Keys.Alt) || KeyboardWatcher.IsKeyLocked(Keys.LWin) || KeyboardWatcher.IsKeyLocked(Keys.RWin))
            {
                Reset();
                return;
            }

            else if (e.Key == Nime.Device.VirtualKeys.Space || e.Key == Nime.Device.VirtualKeys.Enter)
            {
                Reset();
                return;
            }
            else if (e.Key == Nime.Device.VirtualKeys.ControlLeft || e.Key == Nime.Device.VirtualKeys.ControlRight)
            {
                Reset();
                return;
            }

            // アルファベット
            else if (e.Key >= Nime.Device.VirtualKeys.A && e.Key <= Nime.Device.VirtualKeys.Z)
            {
                _labelInput.Text += e.Key;
                _currentPos++;
            }
            else if (e.Key == Nime.Device.VirtualKeys.Subtract || e.Key == Nime.Device.VirtualKeys.OEMMinus)
            {
                _labelInput.Text += "ー";
                _currentPos++;
            }
            // 数字
            else if ((e.Key >= Nime.Device.VirtualKeys.D0 && e.Key <= Nime.Device.VirtualKeys.D9) ||
                     (e.Key >= Nime.Device.VirtualKeys.N0 && e.Key <= Nime.Device.VirtualKeys.N9))
            {
                _labelInput.Text += e.Key.ToString()[1];
                _currentPos++;
            }
            else if (e.Key == Nime.Device.VirtualKeys.OEMPeriod)
            {
                _labelInput.Text += ".";
                _currentPos++;
            }
            else if (e.Key == Nime.Device.VirtualKeys.OEMCommma)
            {
                _labelInput.Text += ",";
                _currentPos++;
            }
            // TODO:各記号については、キーボードに応じて判断し分ける必要がある。

            // 削除
            else if (e.Key == Nime.Device.VirtualKeys.BackSpace)
            {
                if (_currentPos <= 0)
                {
                    Reset();
                    return;
                }
                var txt = _labelInput.Text;
                try
                {
                    _labelInput.Text = txt.Substring(0, _currentPos - 1) + txt.Substring(_currentPos);
                }
                catch
                {
                    Reset();
                    return;
                }
                _currentPos--;
            }
            else if (e.Key == Nime.Device.VirtualKeys.Del)
            {
                if (_currentPos >= _labelInput.Text.Length)
                {
                    Reset();
                    return;
                }
                var txt = _labelInput.Text;
                try
                {
                    _labelInput.Text = txt.Substring(0, _currentPos) + txt.Substring(_currentPos + 1);
                }
                catch
                {
                    Reset();
                    return;
                }
            }

            // 移動
            else if (e.Key == Nime.Device.VirtualKeys.Up || e.Key == Nime.Device.VirtualKeys.Down)
            {
                Reset();
                return;
            }
            else if (e.Key == Nime.Device.VirtualKeys.Left)
            {
                if (_labelInput.Text.Length > 0) _currentPos--;
                if (_currentPos < 0)
                {
                    Reset();
                    return;
                }
            }
            else if (e.Key == Nime.Device.VirtualKeys.Right)
            {
                if (_labelInput.Text.Length > 0) _currentPos++;
                if (_currentPos > _labelInput.Text.Length)
                {
                    Reset();
                    return;
                }
            }
            else if (e.Key == Nime.Device.VirtualKeys.Home)
            {
                // TODO:この入力の開始キャレット位置を記録しておき、その位置と一致するならResetしない。
                Reset();
                return;
            }
            else if (e.Key == Nime.Device.VirtualKeys.End)
            {
                // TODO:この入力中の最も右側のキャレット位置を記録しておき、その位置と一致するならResetしない。
                Reset();
                return;
            }

            // Shift+Escで未確定文字の削除
            else if (e.Key == Nime.Device.VirtualKeys.Esc && (KeyboardWatcher.IsKeyLocked(Keys.RShiftKey) || KeyboardWatcher.IsKeyLocked(Keys.LShiftKey)))
            {
                DeleteCurrentText();
                return;
            }
            else if (e.Key == Nime.Device.VirtualKeys.Shift || e.Key == Nime.Device.VirtualKeys.ShiftLeft || e.Key == Nime.Device.VirtualKeys.ShiftRight)
            {
                return; // _lastAnswerを消さないためにResetせずにreturncI

            }
            else // 原則としてはリセットだろう…
            {
                Reset();
                return;
            }

            if (_labelInput.Text.Length == 0)
            {
                Reset();
                return;
            }

            _labelJapaneseHiragana.Text = ConvertToHiragana(_labelInput.Text);

            if (_labelInput.Text.Length == 1)
            {
                var p = Caret.GetCaretPosition();
                SetDesktopLocation(p.X, p.Y + 15); // TODO!:本当はキャレットサイズを取得したい。
                Opacity = 0.80;
            }
        }

        string ConvertToHiragana(string txt)
        {
            txt = txt.Replace("NN", "ん");
            var txtHiragana = Microsoft.International.Converters.KanaConverter.RomajiToHiragana(txt);
            txtHiragana = txtHiragana.Replace(",", "、");
            txtHiragana = txtHiragana.Replace(".", "。");
            return txtHiragana;
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            TopMost = true;
        }

        private void _toolStripMenuItemExist_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }

    public class JsonResponse
    {
        public List<List<object>> Strings { get; set; }

        public string GetFirstSentence()
        {
            string ans = "";

            foreach (var lst in Strings)
            {
                var key = (JsonElement)lst[0];
                var candidates = (JsonElement)lst[1];
                ans += candidates[0].ToString();
            }

            return ans;
        }
    }

}
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
         * 
         * 
         * 設定項目
         *   数字の扱い(半角を優先/全角を優先/１文字なら全角、それ以外は半角)
         *   区切り文字入力時の自動変換(on/off, 有効文字数)
         *   自動辞書登録モード(常に自動登録/元文字にアルファベットが含まれていなければ自動登録/常に自動登録しない)
         *   キーボードレイアウト(JIS/US ※システムから取れればよかったのだが、どうも取る方法がわからなかった。)
         *   入力中のナビ表示ON/OFF、アプリケーション毎の設定
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
        Point _lastSetDesktopLocation = Point.Empty;

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

        bool _currentDeleting = false;
        private void DeleteCurrentText()
        {
            _currentDeleting = true;
            try
            {
                var lengthAll = _labelInput.Text.Length;
                int pos = _currentPos;

                if (KeyboardWatcher.IsKeyLocked(Keys.LShiftKey)) DeviceOperator.KeyUp(Nime.Device.VirtualKeys.ShiftLeft);
                if (KeyboardWatcher.IsKeyLocked(Keys.RShiftKey)) DeviceOperator.KeyUp(Nime.Device.VirtualKeys.ShiftRight);

                // UNDOの履歴を出来るだけまとめたいので、選択してから消す
                //Debug.WriteLine($"{_labelInput.Text}, pos:{pos} Not Shift");

                for (int i = pos; i < lengthAll; i++)
                {
                    DeviceOperator.KeyStroke(Nime.Device.VirtualKeys.Right);
                }
                DeviceOperator.KeyDown(Nime.Device.VirtualKeys.Shift);
                for (int i = 0; i < lengthAll; i++)
                {
                    DeviceOperator.KeyStroke(Nime.Device.VirtualKeys.Left);
                }
                DeviceOperator.KeyUp(Nime.Device.VirtualKeys.Shift);
                DeviceOperator.KeyStroke(Nime.Device.VirtualKeys.Del);

                Reset();
            }
            finally
            {
                _currentDeleting = false;
            }
        }


        private /*async*/ void KeyboardWatcher_KeyUp(object? sender, KeyboardWatcher.KeybordWatcherEventArgs e)
        {
            if (_nowConvertDetail) return;
            if (IMEWatcher.IsOnIME(true)) return;
            if (_currentDeleting) return;

            Debug.WriteLine($"keyUp:{e.Key}");

            if (e.Key == Nime.Device.VirtualKeys.OEMCommma || e.Key == Nime.Device.VirtualKeys.OEMPeriod)
            {
                if (_labelInput.Text.Length > 4) // 自動変換の実行("desu."とか"masu."を自動で変換したいので4文字を制限とする)
                {
                    var txtHiragana = ConvertToHiragana(_labelInput.Text);
                    bool isNumber = txtHiragana.All(c => ('0' <= c && c <= '9') || c == '、' || c == '。');

                    if (!isNumber && txtHiragana.All(c => c < 'A' || 'Z' < c ))
                    {
                        if (_labelInput.Text.Length < 10) // sizeなど、ひらがなに変換できても英語の場合もある(さすがに10文字超えていたら大丈夫だろう…)
                        {
                            // もし該当する英単語があるなら自動変換は止めておく
                            try
                            {
                                using (var client = new HttpClient())
                                {
                                    var txtReq = $"https://api.excelapi.org/dictionary/enja?word={_labelInput.Text.TrimEnd(',', '.').ToLower()}";
                                    Debug.WriteLine("get:" + txtReq);

                                    //var httpsResponse = await client.GetAsync(txtReq, null);
                                    //var responseContent = await httpsResponse.Content.ReadAsStringAsync();
                                    var httpsResponse = client.GetAsync(txtReq);
                                    var responseContentTask = httpsResponse.Result.Content.ReadAsStringAsync();

                                    var responseContent = responseContentTask.Result;
                                    if (responseContent != null)
                                    {
                                        Debug.WriteLine("return:" + responseContent?.ToString());

                                        if (!string.IsNullOrWhiteSpace(responseContent)) return; // もし該当する英単語があるなら自動変換は止めておく
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                            }
                        }
                        ActionConvert();
                    }
                }
                return;
            }

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
                        var txtPost = convertDetailForm.TargetSentence.GetSelectedSentence();
                        int isame = 0;
                        for (isame = 0; isame < Math.Min(preTxt.Length, txtPost.Length); isame++)
                        {
                            if (preTxt[isame] != txtPost[isame]) break;
                        }
                        for (int i = isame; i < preTxt.Length; i++)
                        {
                            DeviceOperator.KeyStroke(Nime.Device.VirtualKeys.BackSpace);
                        }
                        DeviceOperator.InputText(txtPost.Substring(isame));
                    }
                    _nowConvertDetail = false;
                }
                else if (!string.IsNullOrEmpty(_labelInput.Text))
                {
                    ActionConvert();
                }
            }

        }

        private void ActionConvert()
        {
            // 変換の実行
            var txt = _labelInput.Text;
            DeleteCurrentText();

            var txtHiragana = ConvertToHiragana(txt);

            try
            {
                var ans = ConvertHiraganaToSentence.Request(txtHiragana);
                if (ans != null)
                {
                    DeviceOperator.InputText(ans.GetSelectedSentence());
                    _lastAnswer = ans;
                }
            }
            catch (Exception ex)
            {
                notifyIcon1.ShowBalloonTip(5000, "[nime]エラー", ex.Message, ToolTipIcon.Error);
                Reset();
            }
        }

        void addText(string s)
        {
            if (_currentPos == _labelInput.Text.Length)
            {
                _labelInput.Text += s;
            }
            else
            {
                _labelInput.Text = _labelInput.Text.Substring(0, _currentPos) + s + _labelInput.Text.Substring(_currentPos);
            }
            _currentPos++;
        }

        private void KeyboardWatcher_KeyDown(object? sender, KeyboardWatcher.KeybordWatcherEventArgs e)
        {
            if (_currentDeleting) return;
            if (_nowConvertDetail) return;
            if (IMEWatcher.IsOnIME(true)) {
                Reset();
                return;
            }
            if (e.Key == Nime.Device.VirtualKeys.Packet) return;

            Debug.WriteLine($"keyDown:{e.Key}");

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
                addText(e.Key.ToString());
            }
            else if (e.Key == Nime.Device.VirtualKeys.Subtract || e.Key == Nime.Device.VirtualKeys.OEMMinus)
            {
                addText("ー");
            }
            // 数字
            else if ((e.Key >= Nime.Device.VirtualKeys.D0 && e.Key <= Nime.Device.VirtualKeys.D9) ||
                     (e.Key >= Nime.Device.VirtualKeys.N0 && e.Key <= Nime.Device.VirtualKeys.N9))
            {
                addText(e.Key.ToString()[1].ToString());
            }
            else if (e.Key == Nime.Device.VirtualKeys.OEMPeriod)
            {
                addText(".");
            }
            else if (e.Key == Nime.Device.VirtualKeys.OEMCommma)
            {
                addText(",");
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
                return; // _lastAnswerを消さないためにResetせずにreturnする
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

            var p = MSAA.GetCaretPosition();
            //UIAutomation.GetCaretPosition(); // WPF対応
            if (p.Y == 0)
            {
                p = Caret.GetCaretPosition();
                p.Y = p.Y + 15; // TODO!:本当はキャレットサイズを取得したい。
            }

            if (_labelInput.Text.Length == 1)
            {
                SetDesktopLocation(p.X, p.Y);
                _lastSetDesktopLocation = new Point(p.X, p.Y);
                Opacity = 0.80;
            }
            else
            {
                if (Math.Abs(_lastSetDesktopLocation.Y - p.Y) > 5)
                {
                    SetDesktopLocation(p.X, p.Y);
                    _lastSetDesktopLocation = new Point(p.X, p.Y);
                }
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

            var klID = InputLanguage.CurrentInputLanguage.Culture.KeyboardLayoutId;
            notifyIcon1.ShowBalloonTip(2000, "認識されたキーボード", InputLanguage.CurrentInputLanguage?.LayoutName?.ToString() + "\r\nキーボードレイアウトID:" + klID.ToString(), ToolTipIcon.Info);
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
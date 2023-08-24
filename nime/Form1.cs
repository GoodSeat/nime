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
         * Shift2連続押下(トリガー、「変換」キーなどに設定可能)で呼び出し
         * マウスクリックで問答無用リセット(ホイールも、もはやマウス動いただけでもリセットすべきかも)
         * 
         * ## 対応予定の機能
         *   入力ナビ、もっとかっこよく、ひらがなの該当箇所にキャレットを描画する
         *   変換履歴は記録していって、次回選択時は優先度上げる
         *   辞書機能。選択肢の最優先に追加。出来れば辞書考慮して自動で,を挿入したい。 
         *    IMEから出力したテキストファイルをインポート。
         *   ローマ字を選択した状態でトリガー押下時変換実施
         *   マスクされたテキストボックスでは表示しないようにする
         *   自動キーボード操作時、Esc押下で緊急停止できるようにする
         *   アクティブなコントロールがボタンである場合等、明らかにテキスト編集中でないことが検知できるのなら、その時は変換の起点としない
         *   大文字(Shift+アルファベット)で入力された文字は区切り位置としよう。例えば、kokodeHakimonowoNugu -> kokode,hakimonowo,nugu (この場合、残りの部分は分割しない、の意味ではなく自動分割するという点に注意)
         *   大文字から始まった一連の入力は変換対象外とする(本機能はOn/Off可能)
         *   変換候補に元ローマ字を出す、全てひらがな確定、全てカタカナ確定(単なるqqの入力とかも考えたが、vimとめっちゃ競合するからやめたほうがいいわ)
         *   変換ウインドウ上での直接編集(IMEMode)
         *   アプリケーションごとのCtrl解除の無効設定(Ctrl+h,Ctrl+U等の対応のため)
         *   英字キーボード、日本語キーボードを考慮した記号
         *   Viの入力モードだと、Shift+<-で選択できないので、一文字ずつ消すしかない。アプリケーションごとに消し方を設定できるようにする。
         *   やはり、補完がないと今どき不便には感じるよなぁ。
         * 
         * ## 課題
         *   「」の扱いとか、!とか?とか：とか
         *   全角スペースもどうしようか(Shift+Space...?)
         *   せっかくなら計算機能も追加しちゃうか
         *   WPFコントロールのキャレット位置(UIAutomation)
         *   既定で、すべてのデスクトップで出すようにしたい
         * 
         * ## 既知の不具合
         *   変換中に入力が入ると、よろしくないところに文字列が入力されてしまう。
         *     DeviceOperator.InputText(ans.GetFirstSentence()); の入力が終わるまでに入力されたものは、一旦キャンセルして終わった後に遅延してシミュレートする。
         * 
         * ## 設定項目
         *  ### 入力
         *   トリガーの設定、変換ウインドウ呼び出しトリガーの設定
         *   区切り文字入力時の自動変換(on/off, 有効文字数)
         *   数字の扱い(半角を優先/全角を優先/１文字なら全角、それ以外は半角)
         *   マウス移動判定閾値距離
         *   キーボードレイアウト(JIS/US ※システムから取れればよかったのだが、どうも取る方法がわからなかった。)
         *   文字の消し方(Shift+矢印で選択後DEL/BS、すべてBS、すべてDEL)(アプリケーションごとの設定)
         *  ### 入力ナビ
         *   入力中のナビ表示ON/OFF、キャレットに対する相対位置、キャレット位置が判定できない時の表示位置(アプリケーション毎の設定)
         *   カラースキーマ
         *  ### 変換ウインドウ
         *   変換ウインドウ上で使用するキー文字リスト(5文字以上)、小文字の後に大文字をキーとして使用するか
         *   一語を対象とした時の変換ウインドウのラピッド変換ON/OFF
         *  ### 辞書
         *   自動辞書登録モード(常に自動登録/元文字にアルファベットが含まれていなければ自動登録/常に自動登録しない)
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

                bool bIsDeleteByAllBS = false; // TODO!:未実装、アプリケーションごとの設定による
                if (!bIsDeleteByAllBS)
                {
                    DeviceOperator.KeyDown(Nime.Device.VirtualKeys.Shift);
                    for (int i = 0; i < lengthAll; i++)
                    {
                        DeviceOperator.KeyStroke(Nime.Device.VirtualKeys.Left);
                    }
                    DeviceOperator.KeyUp(Nime.Device.VirtualKeys.Shift);
                    DeviceOperator.KeyStroke(Nime.Device.VirtualKeys.Del);
                }
                else
                {
                    for (int i = 0; i < lengthAll; i++)
                    {
                        DeviceOperator.KeyStroke(Nime.Device.VirtualKeys.BackSpace);
                    }
                }

                Reset();
            }
            finally
            {
                _currentDeleting = false;
            }
        }

        private bool IsIgnorePatternInput()
        {
            if (string.IsNullOrEmpty(_labelInput.Text)) return false;

            var c = _labelInput.Text[0];
            return ('A' <= c && c <= 'Z'); // 1文字目が大文字の場合は無視
        }

        private void ActionConvert()
        {
            if (IsIgnorePatternInput()) // 1文字目が大文字の場合は無視
            {
                Reset();
                return;
            }

            // 変換の実行
            var txt = _labelInput.Text;
            var ans = Task.Run(() =>
            {
                var ss = new List<string>();
                while (!string.IsNullOrEmpty(txt))
                {
                    string word = txt[0].ToString();
                    txt = txt.Substring(1);

                    var w = txt.TakeWhile(c => c < 'A' || 'Z' < c);
                    word = w.Aggregate(word, (acc, c) => acc + c.ToString());
                    ss.Add(word);

                    txt = txt.Substring(w.Count());
                }

                var cs = ss.AsParallel().Select(t =>
                {
                    var txtHiragana = ConvertToHiragana(t);
                    try
                    {
                        return ConvertHiraganaToSentence.Request(txtHiragana);
                    }
                    catch
                    {
                        return null;
                    }
                });

                if (cs.Any(c => c == null)) return null;
                return ConvertCandidate.Concat(cs.ToArray());
            });

            DeleteCurrentText();

            var result = ans.Result;
            if (result == null)
            {
                notifyIcon1.ShowBalloonTip(5000, "[nime]エラー", "変換に失敗しました。", ToolTipIcon.Error);
            }
            else
            {
                _lastAnswer = result;
                DeviceOperator.InputText(_lastAnswer.GetSelectedSentence());
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


        private /*async*/ void KeyboardWatcher_KeyUp(object? sender, KeyboardWatcher.KeybordWatcherEventArgs e)
        {
            if (_nowConvertDetail) return;
            if (IMEWatcher.IsOnIME(true)) return;
            if (_currentDeleting) return;
            if (!_toolStripMenuItemRunning.Checked) return;

            Debug.WriteLine($"keyUp:{e.Key}");

            if ((!KeyboardWatcher.IsKeyLocked(Keys.LShiftKey) && !KeyboardWatcher.IsKeyLocked(Keys.RShiftKey)) &&
                (e.Key == Nime.Device.VirtualKeys.OEMCommma || e.Key == Nime.Device.VirtualKeys.OEMPeriod))
            {
                if (_labelInput.Text.Length > 4) // 自動変換の実行("desu."とか"masu."を自動で変換したいので4文字を制限とする)
                {
                    var txtHiragana = ConvertToHiragana(_labelInput.Text);
                    bool isNumber = txtHiragana.All(c => ('0' <= c && c <= '9') || c == '、' || c == '。');

                    if (!isNumber && txtHiragana.All(c => c < 'A' || 'Z' < c))
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

                    // 起動時にアクティブだったハンドルを覚えておいて、閉じた時にアクティブになるハンドルが異なるようなら変更をキャンセルすること！
                    //　⇒さもないと、ファイル名変更の際などにとんでもないことになる。
                    //　⇒むしろどうにかしたいのだが、ちょっと策は難しい気がする。フォーカスが別のウインドウに写った時点で、閉じてしまうので…ファイル名変更時には使えないのか…
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

        private void KeyboardWatcher_KeyDown(object? sender, KeyboardWatcher.KeybordWatcherEventArgs e)
        {
            if (_currentDeleting) return;
            if (_nowConvertDetail) return;
            if (!_toolStripMenuItemRunning.Checked) return;
            if (IMEWatcher.IsOnIME(true))
            {
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
                if (KeyboardWatcher.IsKeyLocked(Keys.LShiftKey) || KeyboardWatcher.IsKeyLocked(Keys.RShiftKey))
                {
                    addText(e.Key.ToString().ToString().ToUpper());
                }
                else
                {
                    addText(e.Key.ToString().ToString().ToLower());
                }
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

                if (!IsIgnorePatternInput())
                {
                    if (_toolStripMenuItemNaviView.Checked) Opacity = 0.80;
                }
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
            txt = txt.Replace("nn", "ん");
            txt = txt.Replace("NN", "んN");
            txt = txt.Replace("nN", "んN");
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

        private void _toolStripMenuItemRunning_Click(object sender, EventArgs e)
        {
            _toolStripMenuItemRunning.Checked = !_toolStripMenuItemRunning.Checked;
            Reset();
        }

        private void _toolStripMenuItemNaviView_Click(object sender, EventArgs e)
        {
            _toolStripMenuItemNaviView.Checked = !_toolStripMenuItemNaviView.Checked;
            Reset();
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
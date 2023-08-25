using GoodSeat.Nime.Windows;
using GoodSeat.Nime.Device;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace GoodSeat.Nime
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
         *   パスワード入力時などの、マスクドテキストボックスであるか否かを外から判定するのは難しそうなので、せめて簡単にナビ表示を消せるようにしたい。
         * 
         * ## 課題
         *   「」の扱いとか、!とか?とか：とか
         *   全角スペースもどうしようか(Shift+Space...?)
         *   せっかくなら計算機能も追加しちゃうか
         *   WPFコントロールのキャレット位置(UIAutomation)
         *   既定で、すべてのデスクトップで出すようにしたい
         *   Shift+矢印でテキスト選択できない場合、BSを文字数分だけ押して消すしかない。
         *     -> VimやTerminal、コマンドプロンプトなど。
         *     -> Excelもひどいことになる(F2で編集を開始していたら大丈夫なのだが)。
         *     アプリごとの指定も可能とするが、可能であれば基本は自動判断したい、MSAAやUIAutomationでSingleSelection的な判定ができたような。
         *   あと、vifmの検索で使おうとすると、挿入される文字も変だった。エンコーディングか何かの問題か?
         * 
         * ## 既知の不具合
         *   変換中に入力が入ると、よろしくないところに文字列が入力されてしまう。
         *     -> DeviceOperator.InputText(ans.GetFirstSentence()); の入力が終わるまでに入力されたものは、一旦キャンセルして終わった後に遅延してシミュレートする。
         *   Explorer上の、名前の変更、検索ボックス、アドレスボックスは軒並み変換ウインドウが使えない…（変換ウインドウ出した時点でフォーカスを失ってキャレットが外れてしまうので）
         * 
         * ## 設定項目
         *  ### 入力
         *   トリガーの設定、変換ウインドウ呼び出しトリガーの設定
         *   区切り文字入力時の自動変換(on/off, 有効文字数)
         *   数字の扱い(半角を優先/全角を優先/１文字なら全角、それ以外は半角)
         *   マウス移動判定閾値距離
         *   キーボードレイアウト(JIS/US ※システムから取れればよかったのだが、どうも取る方法がわからなかった。)
         *   文字の消し方(Shift+矢印で選択後DEL/BS、すべてBS、すべてDEL)(アプリケーションごとの設定)
         *   さよならの挨拶ON/OFF
         *   操作用キーワード設定
         *  ### 入力ナビ
         *   入力中のナビ表示ON/OFF、キャレットに対する相対位置、キャレット位置が判定できない時の表示位置(アプリケーション毎の設定)
         *   カラースキーマ
         *  ### 変換ウインドウ
         *   変換ウインドウ上で使用するキー文字リスト(5文字以上)、小文字の後に大文字をキーとして使用するか
         *   一語を対象とした時の変換ウインドウのラピッド変換ON/OFF
         *  ### 辞書
         *   自動辞書登録モード(常に自動登録/元文字にアルファベットが含まれていなければ自動登録/常に自動登録しない)
         *  ### アプリケーションごとの設定項目 ※設定画面起動時、直前にアクティブだったアプリを簡単に追加できるようにしたい。Smalkerのようにリンク文字列をおいておくか。あれこれだめ
         * 
         */

        public Form1()
        {
            InitializeComponent();

            //SetStyle(ControlStyles.Selectable, false);
            Reset();

            _convertDetailForm = new ConvertDetailForm();
            _convertDetailForm.ConvertExit += _convertDetailForm_ConvertExit;
            _convertDetailForm.Show();
            _convertDetailForm.TopMost = true;

            _keyboardWatcher = new KeyboardWatcher();
            _keyboardWatcher.KeyUp += KeyboardWatcher_KeyUp;
            _keyboardWatcher.KeyDown += KeyboardWatcher_KeyDown;
            _keyboardWatcher.Enable = true;
        }

        KeyboardWatcher _keyboardWatcher;
        ConvertDetailForm _convertDetailForm;

        int _currentPos = 0;
        DateTime _lastShiftUp = DateTime.MinValue;

        Point _lastSetDesktopLocation = Point.Empty;
        int _caretSize = 0;

        ConvertCandidate _lastAnswer;

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
            _keyboardWatcher.Enable = false;
            try
            {
                var lengthAll = _labelInput.Text.Length;
                int pos = _currentPos;

                if (_keyboardWatcher.IsKeyLocked(Keys.LShiftKey)) DeviceOperator.KeyUp(VirtualKeys.ShiftLeft);
                if (_keyboardWatcher.IsKeyLocked(Keys.RShiftKey)) DeviceOperator.KeyUp(VirtualKeys.ShiftRight);

                // UNDOの履歴を出来るだけまとめたいので、選択してから消す
                //Debug.WriteLine($"{_labelInput.Text}, pos:{pos} Not Shift");

                for (int i = pos; i < lengthAll; i++)
                {
                    DeviceOperator.KeyStroke(VirtualKeys.Right);
                }

                bool bIsDeleteByAllBS = false; // TODO!:未実装、アプリケーションごとの設定による
                if (!bIsDeleteByAllBS)
                {
                    DeviceOperator.KeyDown(VirtualKeys.Shift);
                    for (int i = 0; i < lengthAll; i++)
                    {
                        DeviceOperator.KeyStroke(VirtualKeys.Left);
                    }
                    DeviceOperator.KeyUp(VirtualKeys.Shift);
                    DeviceOperator.KeyStroke(VirtualKeys.Del); // 誤作動のことを考えると、DelよりBSの方がまだ安全かもしれない。
                }
                else
                {
                    for (int i = 0; i < lengthAll; i++)
                    {
                        DeviceOperator.KeyStroke(VirtualKeys.BackSpace);
                    }
                }

                Reset();
            }
            finally
            {
                _keyboardWatcher.Enable = true;
            }
        }

        private bool IsIgnorePatternInput()
        {
            if (string.IsNullOrEmpty(_labelInput.Text)) return false;

            var c = _labelInput.Text[0];
            return ('A' <= c && c <= 'Z'); // 1文字目が大文字の場合は無視
        }


        string[] _goodBys = new string[]{
            """ Good Bye! """,
            """ Thank you for using! """,
            """ See you again! """,
        };


        private void ActionConvert()
        {
            if (IsIgnorePatternInput()) // 1文字目が大文字の場合は無視
            {
                Reset();
                return;
            }

            // 変換の実行
            var txt = _labelInput.Text;

            // キーワード操作受付
            if (!_toolStripMenuItemRunning.Checked && txt == "nimestart")
            {
                DeleteCurrentText();
                _toolStripMenuItemRunning.Checked = true;
                notifyIcon1.ShowBalloonTip(2000, "nime", "入力受付を再開しました。", ToolTipIcon.Info);
                return;
            }
            else if (txt == "nimeexit")
            {
                DeleteCurrentText();

                Random r = new Random();
                var msg = " ■" + _goodBys[r.Next(0, _goodBys.Length)] + "■ ";
                DeviceOperator.InputText(msg);
                Thread.Sleep(750);
                for (int i = 0; i < msg.Length; ++i) DeviceOperator.KeyStroke(VirtualKeys.BackSpace);

                _toolStripMenuItemExist_Click(null, EventArgs.Empty);
                return;
            }
            if (!_toolStripMenuItemRunning.Checked) return;

            if (txt == "nimestop")
            {
                DeleteCurrentText();
                _toolStripMenuItemRunning.Checked = false;
                notifyIcon1.ShowBalloonTip(2000, "nime", "入力受付を停止しました。", ToolTipIcon.Info);
                return;
            }
            else if (txt == "nimevisible")
            {
                DeleteCurrentText();
                _toolStripMenuItemNaviView_Click(null, EventArgs.Empty);
                if (_toolStripMenuItemNaviView.Checked) notifyIcon1.ShowBalloonTip(2000, "nime", "入力表示をONにしました。", ToolTipIcon.Info);
                else notifyIcon1.ShowBalloonTip(2000, "nime", "入力表示をOFFにしました。", ToolTipIcon.Info);
                return;
            }
            else if (txt == "nimesetting")
            {
                DeleteCurrentText();
                // TODO!:show setting.
                return;
            }

            Func<ConvertCandidate> f = () =>
            {
                if (txt.All(c => c < 'A' || 'Z' < c))
                {
                    var txtHiragana = ConvertToHiragana(txt);
                    try
                    {
                        return ConvertHiraganaToSentence.Request(txtHiragana);
                    }
                    catch
                    {
                        return null;
                    }
                }
                else
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
                }
            };

            ConvertCandidate result = null;
            var mt = true;
            if (mt)
            {
                var ans = Task.Run(f);
                DeleteCurrentText();
                result = ans.Result;
            }
            else
            {
                DeleteCurrentText();
                result = f();
            }

            if (result == null)
            {
                notifyIcon1.ShowBalloonTip(5000, "[nime]エラー", "変換に失敗しました。", ToolTipIcon.Error);
            }
            else
            {
                _lastAnswer = result;
                //DeviceOperator.InputText(_lastAnswer.GetSelectedSentence());
                SendKeys.Send(_lastAnswer.GetSelectedSentence());
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

        Point CaretPosition(WindowInfo wi = null)
        {
            var taskCaret1 = MSAA.GetCaretPositionAsync(wi);
            var taskCaret2 = Caret.GetCaretPositionAsync(wi);
            if (taskCaret1.Result.Item1.Y != 0) return taskCaret1.Result.Item1;
            return taskCaret2.Result;
        }


        private /*async*/ void KeyboardWatcher_KeyUp(object? sender, KeyboardWatcher.KeybordWatcherEventArgs e)
        {
            if (IMEWatcher.IsOnIME(true)) return;
            if (e.Key == VirtualKeys.Packet) return;

            Debug.WriteLine($"keyUp:{e.Key}");

            if ((!_keyboardWatcher.IsKeyLocked(Keys.LShiftKey) && !_keyboardWatcher.IsKeyLocked(Keys.RShiftKey)) &&
                (e.Key == VirtualKeys.OEMCommma || e.Key == VirtualKeys.OEMPeriod))
            {
                if (_labelInput.Text.Length > 4 && _toolStripMenuItemRunning.Checked) // 自動変換の実行("desu."とか"masu."を自動で変換したいので4文字を制限とする)
                {
                    var txtHiragana = ConvertToHiragana(_labelInput.Text);
                    bool isNumber = txtHiragana.All(c => ('0' <= c && c <= '9') || c == '、' || c == '。');

                    bool existAlphabet = txtHiragana.Any(c => ('A' <= c && c <= 'Z') || ('a' <= c && c <= 'z'));
                    if (!isNumber && !existAlphabet)
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

            if (e.Key != VirtualKeys.ShiftRight) return;

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

                    // 変換ウインドウ立ち上げ時と終了時でキャレット位置がずれているようなら、変更をキャンセルする
                    //　⇒さもないと、複数のBackSpaceキーが送信されてしまい、ファイル名変更の際などにとんでもないことになる。
                    _ptWhenStartConvert = CaretPosition();
                    Debug.WriteLine($" 変換開始前キャレット位置 x:{_ptWhenStartConvert.X}, y:{_ptWhenStartConvert.Y}");

                    var location = Location;
                    location.Y = location.Y + Height + _caretSize;

                    _keyboardWatcher.Enable = false;
                    _convertDetailForm.Start(_lastAnswer, location);
                }
                else if (!string.IsNullOrEmpty(_labelInput.Text))
                {
                    ActionConvert();
                }
            }

        }

        Point _ptWhenStartConvert;

        private void _convertDetailForm_ConvertExit(object? sender, DialogResult e)
        {
            if (e == DialogResult.OK)
            {

                // Excelで、最初はうまくいっているのに、突然変換をキャンセル、になって以降成功しなくなる現象があった。Thread.Sleep(20)とApplication.DoEventswを入れてみたがどうだ...?
                //  -> MEMO:変換ウインドウの実装改善により、この処理は不要になったのではないかと期待
                //Thread.Sleep(20);
                //Application.DoEvents();

                var pNew = CaretPosition();
                Debug.WriteLine($" -> 変換後キャレット位置 x:{pNew.X}, y:{pNew.Y}");
                if (Math.Abs(_ptWhenStartConvert.Y - pNew.Y) < 10 && Math.Abs(_ptWhenStartConvert.X - pNew.X) < 200)
                {
                    Debug.WriteLine($"   -> 変換実施");
                    var txtPost = _convertDetailForm.TargetSentence.GetSelectedSentence();
                    int isame = 0;
                    for (isame = 0; isame < Math.Min(_convertDetailForm.SentenceWhenStart.Length, txtPost.Length); isame++)
                    {
                        if (_convertDetailForm.SentenceWhenStart[isame] != txtPost[isame]) break;
                    }

                    if (true) // TODO:アプリケーションによってはBS×文字数で対処
                    {
                        DeviceOperator.KeyDown(VirtualKeys.ShiftLeft);
                        for (int i = isame; i < _convertDetailForm.SentenceWhenStart.Length; i++)
                        {
                            DeviceOperator.KeyStroke(VirtualKeys.Left);
                        }
                        DeviceOperator.KeyUp(VirtualKeys.ShiftLeft);
                        DeviceOperator.KeyStroke(VirtualKeys.Del);
                    }
                    else
                    {
                        for (int i = isame; i < _convertDetailForm.SentenceWhenStart.Length; i++)
                        {
                            DeviceOperator.KeyStroke(VirtualKeys.BackSpace);
                        }
                    }
                    //DeviceOperator.InputText(txtPost.Substring(isame));
                    SendKeys.Send(txtPost.Substring(isame));
                }
                else
                {
                    Debug.WriteLine($"   -> 変換をキャンセル");
                    notifyIcon1.ShowBalloonTip(2000, "変換エラー", $"キャレット位置の変化({_ptWhenStartConvert.X},{_ptWhenStartConvert.Y} -> {pNew.X},{pNew.Y})を検知したため、変換をキャンセルしました。", ToolTipIcon.Warning);
                    // TODO!:変換結果を失わないように、変換結果を記録・編集するためのウインドウをpOrgの近くにShowしましょう(勝手にクリップボードを汚すのもあれだろうし…)
                    //      「変換結果を元のキャレット位置に戻すことができませんでした」的なツールチップの注釈と共に…

                    //       IMEを直接使って編集した場合には、どうしたってフォーカスが外れる。まぁ、こればっかりはいよいよ仕方ない気がするな。
                    //       そうなると、変換結果を戻すことができませんでしたウインドウはやはり必要、ということになりますな。

                    //       どうも、上記を正確に判断し切るのは難しい気がするので、変換をキャンセルする条件だけど無視して変換処理を実行するホワイトリストを設定できるようにしたほうが良いだろう。
                    //         Wordの_WwGクラスとか。(Wordも上部のメニューの検索ボックスはまずい。)
                }
            }
            _keyboardWatcher.Enable = true;
        }

        private void KeyboardWatcher_KeyDown(object? sender, KeyboardWatcher.KeybordWatcherEventArgs e)
        {
            if (IMEWatcher.IsOnIME(true))
            {
                Reset();
                return;
            }
            if (e.Key == VirtualKeys.Packet) return;

            Debug.WriteLine($"keyDown:{e.Key}");

            Task<Tuple<Point, Size>>? taskCaret1 = null;
            Task<Point>? taskCaret2 = null;
            if (_toolStripMenuItemRunning.Checked)
            {
                taskCaret1 = MSAA.GetCaretPositionAsync();
                taskCaret2 = Caret.GetCaretPositionAsync();
                //UIAutomation.GetCaretPosition(); // TOOD!:WPF対応
            }

            if (_keyboardWatcher.IsKeyLocked(Keys.LControlKey) || _keyboardWatcher.IsKeyLocked(Keys.RControlKey)
             || _keyboardWatcher.IsKeyLocked(Keys.Alt) || _keyboardWatcher.IsKeyLocked(Keys.LWin) || _keyboardWatcher.IsKeyLocked(Keys.RWin))
            {
                Reset();
                return;
            }

            else if (e.Key == VirtualKeys.Space || e.Key == VirtualKeys.Enter)
            {
                Reset();
                return;
            }
            else if (e.Key == VirtualKeys.ControlLeft || e.Key == VirtualKeys.ControlRight)
            {
                Reset();
                return;
            }

            // アルファベット
            else if (e.Key >= VirtualKeys.A && e.Key <= VirtualKeys.Z)
            {
                if (_keyboardWatcher.IsKeyLocked(Keys.LShiftKey) || _keyboardWatcher.IsKeyLocked(Keys.RShiftKey))
                {
                    addText(e.Key.ToString().ToString().ToUpper());
                }
                else
                {
                    addText(e.Key.ToString().ToString().ToLower());
                }
            }
            else if (e.Key == VirtualKeys.Subtract || e.Key == VirtualKeys.OEMMinus)
            {
                addText("ー");
            }
            // 数字
            else if ((e.Key >= VirtualKeys.D0 && e.Key <= VirtualKeys.D9) ||
                     (e.Key >= VirtualKeys.N0 && e.Key <= VirtualKeys.N9))
            {
                addText(e.Key.ToString()[1].ToString());
            }
            else if (e.Key == VirtualKeys.OEMPeriod)
            {
                addText(".");
            }
            else if (e.Key == VirtualKeys.OEMCommma)
            {
                addText(",");
            }
            // TODO:各記号については、キーボードに応じて判断し分ける必要がある。

            // 削除
            else if (e.Key == VirtualKeys.BackSpace)
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
            else if (e.Key == VirtualKeys.Del)
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
            else if (e.Key == VirtualKeys.Up || e.Key == VirtualKeys.Down)
            {
                Reset();
                return;
            }
            else if (e.Key == VirtualKeys.Left)
            {
                if (_labelInput.Text.Length > 0) _currentPos--;
                if (_currentPos < 0)
                {
                    Reset();
                    return;
                }
            }
            else if (e.Key == VirtualKeys.Right)
            {
                if (_labelInput.Text.Length > 0) _currentPos++;
                if (_currentPos > _labelInput.Text.Length)
                {
                    Reset();
                    return;
                }
            }
            else if (e.Key == VirtualKeys.Home)
            {
                // TODO:この入力の開始キャレット位置を記録しておき、その位置と一致するならResetしない。
                Reset();
                return;
            }
            else if (e.Key == VirtualKeys.End)
            {
                // TODO:この入力中の最も右側のキャレット位置を記録しておき、その位置と一致するならResetしない。
                Reset();
                return;
            }

            // Shift+Escで未確定文字の削除
            else if (e.Key == VirtualKeys.Esc && (_keyboardWatcher.IsKeyLocked(Keys.RShiftKey) || _keyboardWatcher.IsKeyLocked(Keys.LShiftKey)))
            {
                DeleteCurrentText();
                return;
            }
            else if (e.Key == VirtualKeys.Shift || e.Key == VirtualKeys.ShiftLeft || e.Key == VirtualKeys.ShiftRight)
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

            if (taskCaret1 != null && taskCaret2 != null)
            {
                Point p = Point.Empty;
                Size s = Size.Empty;
                if (taskCaret1.Result.Item1.Y != 0)
                {
                    p = taskCaret1.Result.Item1;
                    s = taskCaret1.Result.Item2;
                }
                else
                {
                    p = taskCaret2.Result;
                    s = new Size(1, 15);
                }

                if (Opacity == 0.00 && _labelInput.Text.Length == 1)
                {
                    SetDesktopLocation(p.X, p.Y);
                    _lastSetDesktopLocation = new Point(p.X, p.Y);
                    _caretSize = s.Height;

                    if (!IsIgnorePatternInput())
                    {
                        if (_toolStripMenuItemNaviView.Checked) Opacity = 0.60;
                    }
                }
                else
                {
                    // むしろワードとかはしょっちゅう更新するとたまにうまく行くのさえうまくいかなくなるが…
                    if (Math.Abs(_lastSetDesktopLocation.Y - p.Y) > 5)
                    {
                        SetDesktopLocation(p.X, p.Y);
                        _lastSetDesktopLocation = new Point(p.X, p.Y);
                    }
                }
            }

            _labelJapaneseHiragana.Text = ConvertToHiragana(_labelInput.Text);
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

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Color color = Color.Red;
            var txtShow = _labelJapaneseHiragana.Text;
            var txtInput = _labelInput.Text;

            if (Opacity == 0.0 && txtInput.Length > 1) return;

            if (txtInput == "nimeexit")
            {
                txtShow = "[nime]終了";
            }
            else if (txtInput == "nimestop")
            {
                txtShow = "[nime]停止";
            }
            else if (txtInput == "nimestart") // 通ることないはずだけど念のため
            {
                txtShow = "[nime]再開";
            }
            else if (txtInput == "nimevisible") // 通ることないはずだけど念のため
            {
                txtShow = "[nime]入力表示の無効化";
            }
            else
            {
                color = Color.Black;
            }

            FontFamily f = SystemFonts.DefaultFont.FontFamily;

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            var path = new GraphicsPath();
            path.AddString(txtShow, f, 0, 12f, new Point(2, 2), null);
            e.Graphics.FillPath(new SolidBrush(color), path);

            var x = (int)path.GetBounds().Width + 10;
            var y = (int)path.GetBounds().Height + 10;
            Size = new Size(x, y);

            var y_ = Math.Max(_lastSetDesktopLocation.Y - y, 0);
            SetDesktopLocation(_lastSetDesktopLocation.X, y_);
        }

        private void _labelJapaneseHiragana_TextChanged(object sender, EventArgs e)
        {
            Refresh();
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
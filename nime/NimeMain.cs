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
using GoodSeat.Nime.Conversion;
using GoodSeat.Nime.Core;
using GoodSeat.Nime.Core.KeyboardLayouts;

namespace GoodSeat.Nime
{
    public partial class NimeMain : Form
    {
        /*
         * ## 既定の設定
         *   トリガー    :変換キー(Shift2連続押下)
         *        変換実施、変換ウインドウ上での変換確定(=Enterと同じ)、
         *     +Ctrl (Ctrlはリセットキーとしても有為なので、ちょっと扱いに迷う。)
         *        入力表示トグル
         *   サブトリガー:無変換キー
         *        ひらがなへの変換実施、変換直後の変換キャンセル(ローマ字に戻す)、変換ウインドウ上での変換キャンセル(=Escと同じ)、
         *      +Ctrl (Ctrlはリセットキーとしても有為なので、ちょっと扱いに迷う。)
         *        カタカナへの変換実施
         * 
         * ## 対応予定の機能
         *   マウスクリックで問答無用リセット(ホイールも、もはやマウス動いただけでもリセットすべきかも)
         *   変換ウインドウ上で、開始括弧を変換したときに対応する閉じ括弧も合わせて変換する
         *   やはり、補完がないと今どき不便には感じるよなぁ。
         *   辞書機能。選択肢の最優先に追加。出来れば辞書考慮して自動で,を挿入したい。 
         *   
         *   MS-IMEから出力したテキストファイルをインポート。
         *   ローマ字を選択した状態で何らかのキー押下時変換実施(Ctrl+Xを送信してクリップボードを利用する他ないだろう。選択状態はわからないため、通常のトリガーとはキーを分けないといけない)
         *   日本語を選択した状態でトリガー押下時変換実施(Ctrl+Xを送信してクリップボードを利用する他ないだろう。選択状態はわからないため、通常のトリガーとはキーを分けないといけない)
         *   アクティブなコントロールがボタンである場合等、明らかにテキスト編集中でないことが検知できるのなら、その時は変換の起点としない
         *   変換候補に元ローマ字を出す、全てひらがな確定、全てカタカナ確定(単なるqqの入力とかも考えたが、vimとめっちゃ競合するからやめたほうがいいわ)
         *   アプリケーションごとのCtrl解除の無効設定(Ctrl+h,Ctrl+U等の対応のため)
         *   英字キーボード、日本語キーボードを考慮した記号
         *   Viの入力モードだと、Shift+<-で選択できないので、一文字ずつ消すしかない。アプリケーションごとに消し方を設定できるようにする。
         *   マスクされたテキストボックスでは表示しないようにする
         *     -> パスワード入力時などの、マスクドテキストボックスであるか否かを外から判定するのは難しそうなので、せめて簡単にナビ表示を消せるようにしたい。
         *   変換が効く間に無変換キー押下することで、変換したものを下のローマ字に戻して、SentenceOnInputの状態ももとに戻す
         *   どうしたって動作は不安定になりがちなので、再起動機能は欲しいかも
         *   多重起動は許さないべき
         *   せっかくなら計算機能も追加しちゃうか
         *   変換ウインドウ上で、何らかのキーで変換結果をクリップボードにコピーする
         *   
         *   (ver2)
         *   IMM or TSF を用いたIMEによる変換候補取得をサポート
         * 
         * ## 課題
         *   全角スペースはどうしようか(Shift+Space...?)
         *   WPFコントロールのキャレット位置(UIAutomation)
         *   Shift+矢印でテキスト選択できない場合、BSを文字数分だけ押して消すしかない。
         *     -> VimやTerminal、コマンドプロンプトなど。
         *     -> Excelもひどいことになる(F2で編集を開始していたら大丈夫なのだが)。
         *     アプリごとの指定も可能とするが、可能であれば基本は自動判断したい、MSAAやUIAutomationでSingleSelection的な判定ができたような。
         *   あと、vifmの検索で使おうとすると、挿入される文字も変だった。エンコーディングか何かの問題か?
         *   アクティブなアプリケーションが変わったら、Resetすべきだろう。何も操作していなくともアクティブなアプリケーションが変わることあるもんな。
         * 
         * ## 既知の不具合
         *   変換中に入力が入ると、よろしくないところに文字列が入力されてしまう。
         *     -> DeviceOperator.InputText(ans.GetFirstSentence()); の入力が終わるまでに入力されたものは、一旦キャンセルして終わった後に遅延してシミュレートする。
         *   Explorer上の、名前の変更、検索ボックス、アドレスボックスは軒並みIME直接編集ウインドウが使えない…（直接編集ウインドウ出した時点でフォーカスを失ってキャレットが外れてしまうので）
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
         *   一文節を対象とした時の変換ウインドウのラピッド変換ON/OFF
         *  ### 辞書
         *   自動辞書登録モード(常に自動登録/元文字にアルファベットが含まれていなければ自動登録/常に自動登録しない)
         *  ### アプリケーションごとの設定項目 ※設定画面起動時、直前にアクティブだったアプリを簡単に追加できるようにしたい。Smalkerのようにリンク文字列をおいておくか。
         * 
         */

        public NimeMain()
        {
            InitializeComponent();

            //SetStyle(ControlStyles.Selectable, false);
            Reset();

            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true, Encoder = JavaScriptEncoder.Create(UnicodeRanges.All) };
                string jsonString = File.ReadAllText(_filepathInputHistroy);
                InputHistory = JsonSerializer.Deserialize<InputHistory>(jsonString, options);
            }
            catch
            {
            }
            if (InputHistory == null) InputHistory = new InputHistory();

            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true, Encoder = JavaScriptEncoder.Create(UnicodeRanges.All) };
                string jsonString = File.ReadAllText(_filepathSplitHistroy);
                SplitHistory = JsonSerializer.Deserialize<SplitHistory>(jsonString, options);
            }
            catch
            {
            }
            if (SplitHistory == null) SplitHistory = new SplitHistory();

            _convertDetailForm = new ConvertDetailForm(InputHistory);
            _convertDetailForm.ConvertExit += _convertDetailForm_ConvertExit;
            _convertDetailForm.Show();
            _convertDetailForm.TopMost = true;

            _keyboardWatcher = new KeyboardWatcher();
            _keyboardWatcher.KeyUp += KeyboardWatcher_KeyUp;
            _keyboardWatcher.KeyDown += KeyboardWatcher_KeyDown;
            _keyboardWatcher.Enable = true;

        }

        string _filepathInputHistroy = "input.json";
        string _filepathSplitHistroy = "split.json";


        KeyboardWatcher _keyboardWatcher;
        ConvertDetailForm _convertDetailForm;

        DateTime _lastShiftUp = DateTime.MinValue;

        Point _lastSetDesktopLocation = Point.Empty;
        int _caretSize = 0;

        ConvertCandidate _lastAnswer;
        ConvertCandidate _canceledConversion = null;


        SentenceOnInput SentenceOnInput { get; set; } = new SentenceOnInput();
        KeyboardLayout KeyboardLayout { get; set; } = new KeyboardLayoutUS();
        InputHistory InputHistory { get; set; }
        SplitHistory SplitHistory { get; set; }

        SentenceOnInput PreSentenceOnInput { get; set; }
        Point PreLastSetDesktopLocation { get; set; }

        private void Reset(bool softReset = false)
        {
            Debug.WriteLine("Reset");

            PreLastSetDesktopLocation = _lastSetDesktopLocation;

            if (softReset) // 直後のBSで元に戻せるように取っておく
            {
                PreSentenceOnInput = SentenceOnInput;
            }
            else
            {
                PreSentenceOnInput = null;
            }

            _lastAnswer = null;
            _canceledConversion = null;

            SentenceOnInput = new SentenceOnInput();

            _labelJapaneseHiragana.Text = "";
            Opacity = 0.00;
        }

        private bool RestoreSoftReset(bool restore)
        {
            if (PreSentenceOnInput == null) return false;

            if (restore) SentenceOnInput = PreSentenceOnInput;

            PreSentenceOnInput = null;
            return restore;
        }

        private void DeleteCurrentText()
        {
            bool preEnable =  _keyboardWatcher.Enable;
            _keyboardWatcher.Enable = false;
            try
            {
                var lengthAll = SentenceOnInput.Text.Length;
                int pos = SentenceOnInput.CaretPosition;

                if (KeyboardWatcher.IsKeyLockedStatic(Keys.LShiftKey)) DeviceOperator.KeyUp(VirtualKeys.ShiftLeft);
                if (KeyboardWatcher.IsKeyLockedStatic(Keys.RShiftKey)) DeviceOperator.KeyUp(VirtualKeys.ShiftRight);

                bool bIsDeleteByAllBS = false; // TODO!:未実装、アプリケーションごとの設定による
                var keys = new List<(VirtualKeys, KeyEventType)>();
                if (!bIsDeleteByAllBS)
                {
                    // UNDOの履歴を出来るだけまとめたいので、選択してから消す
                    keys.AddRange(Utility.Duplicates((VirtualKeys.Right, KeyEventType.Stroke), lengthAll - pos));
                    keys.Add((VirtualKeys.ShiftLeft, KeyEventType.Down));
                    keys.AddRange(Utility.Duplicates((VirtualKeys.Left, KeyEventType.Stroke), lengthAll));
                    keys.Add((VirtualKeys.ShiftLeft, KeyEventType.Up));
                    //keys.Add((VirtualKeys.BackSpace, KeyEventType.Stroke)); // TMEMO:VsVimでは巧く動作しない
                    keys.Add((VirtualKeys.Del, KeyEventType.Stroke));
                }
                else
                {
                    keys.AddRange(Utility.Duplicates((VirtualKeys.Del, KeyEventType.Stroke), lengthAll - pos));
                    keys.AddRange(Utility.Duplicates((VirtualKeys.BackSpace, KeyEventType.Stroke), pos));
                }
                DeviceOperator.SendKeyEvents(keys.ToArray());

                Reset();
            }
            finally
            {
                _keyboardWatcher.Enable = preEnable;
            }
        }

        private bool IsIgnorePatternInput()
        {
            if (string.IsNullOrEmpty(SentenceOnInput.Text)) return false;

            var c = SentenceOnInput.Text[0];
            return ('A' <= c && c <= 'Z'); // 1文字目が大文字の場合は無視
        }


        string[] _goodBys = new string[]{
            """ Good Bye! """,
            """ Thank you for using! """,
            """ See you again! """,
        };

        private bool OperateWithKeyword(string txt)
        {
            // キーワード操作受付
            if (!_toolStripMenuItemRunning.Checked && txt == "nimestart")
            {
                DeleteCurrentText();
                _toolStripMenuItemRunning.Checked = true;
                notifyIcon1.ShowBalloonTip(2000, "nime", "入力受付を再開しました。", ToolTipIcon.Info);
                return true;
            }
            else if (txt == "nimeexit")
            {
                DeleteCurrentText();

                Random r = new Random();
                var msg = " ■" + _goodBys[r.Next(0, _goodBys.Length)] + "■ ";
                DeviceOperator.InputText(msg);
                Thread.Sleep(500);
                DeviceOperator.SendKeyEvents(Utility.Duplicates((VirtualKeys.BackSpace, KeyEventType.Stroke), msg.Length).ToArray());
                for (int i = 0; i < msg.Length; ++i) DeviceOperator.KeyStroke(VirtualKeys.ShiftLeft);

                _toolStripMenuItemExist_Click(null, EventArgs.Empty);
                return true;
            }
            if (!_toolStripMenuItemRunning.Checked) return false;

            if (txt == "nimestop")
            {
                DeleteCurrentText();
                _toolStripMenuItemRunning.Checked = false;
                notifyIcon1.ShowBalloonTip(2000, "nime", "入力受付を停止しました。", ToolTipIcon.Info);
                return true;
            }
            else if (txt == "nimevisible")
            {
                DeleteCurrentText();
                _toolStripMenuItemNaviView_Click(null, EventArgs.Empty);
                if (_toolStripMenuItemNaviView.Checked) notifyIcon1.ShowBalloonTip(2000, "nime", "入力表示をONにしました。", ToolTipIcon.Info);
                else notifyIcon1.ShowBalloonTip(2000, "nime", "入力表示をOFFにしました。", ToolTipIcon.Info);
                return true;
            }
            else if (txt == "nimesetting")
            {
                DeleteCurrentText();
                // TODO!:show setting.
                return true;
            }
            return false;
        }

        private void ActionConvert()
        {
            var txt = SentenceOnInput.Text;
            if (string.IsNullOrEmpty(txt))
            {
                Reset();
                return;
            }

            Debug.WriteLine("■ 変換開始:" + DateTime.Now.ToString() + "\"" + DateTime.Now.Millisecond.ToString());

            // ここからキーイベントをキャンセル(記録しておく)
            using (var keyDelay = new DelayKeyInput(_keyboardWatcher))
            {
                // キーワード操作受付
                if (OperateWithKeyword(txt)) return;

                int timeout = 200;

                Func<ConvertCandidate?> f = () =>
                {
                    if (txt.All(c => !Utility.IsUpperAlphabet(c)))
                    {
                        var txtHiragana = Utility.ConvertToHiragana(txt);
                        try
                        {
                            var c0 = ConvertHiraganaToSentence.Request(txtHiragana, timeout, InputHistory);
                            var s0 = c0.MakeSentenceForHttpRequest();
                            var s1 = SplitHistory.SplitConsiderHisory(s0);
                            if (s0 != s1) c0 = ConvertHiraganaToSentence.Request(s1, timeout, InputHistory);
                            return c0;
                        }
                        catch { return null; }
                    }
                    else
                    {
                        var ss = new List<string>();
                        while (!string.IsNullOrEmpty(txt))
                        {
                            string word = txt[0].ToString();
                            txt = txt.Substring(1);

                            // 次もその次も大文字ならば、もう一字取る
                            while (txt.Length > 1 && Utility.IsUpperAlphabet(txt[0]) && Utility.IsUpperAlphabet(txt[1]))
                            {
                                word += txt[0].ToString();
                                txt = txt.Substring(1);
                            }

                            var w = txt.TakeWhile(c => !Utility.IsUpperAlphabet(c));
                            word = w.Aggregate(word, (acc, c) => acc + c.ToString());
                            ss.Add(word);

                            txt = txt.Substring(w.Count());
                        }

                        var cs = ss.AsParallel().Select(t =>
                        {
                            if (t.All(Utility.IsUpperAlphabet)) return new ConvertCandidate(t);

                            var txtHiragana = Utility.ConvertToHiragana(t);
                            try
                            {
                                var c0 = ConvertHiraganaToSentence.Request(txtHiragana, timeout, InputHistory);
                                var s0 = c0.MakeSentenceForHttpRequest();
                                var s1 = SplitHistory.SplitConsiderHisory(s0);
                                if (s0 != s1) c0 = ConvertHiraganaToSentence.Request(s1, timeout, InputHistory);
                                return c0;
                            }
                            catch { return null; }
                        });

                        if (cs.Any(c => c == null)) return null;
                        return ConvertCandidate.Concat(cs.ToArray());
                    }
                };

                ConvertCandidate? result = null;
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
                    DeviceOperator.InputText(_lastAnswer.GetSelectedSentence());
                    //SendKeys.SendWait(_lastAnswer.GetSelectedSentence());
                }
            }

            Debug.WriteLine("■ 変換終了:" + DateTime.Now.ToString() + "\"" + DateTime.Now.Millisecond.ToString());
        }

        Point GetCaretCoordinate(WindowInfo wi = null)
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

            if (e.Key == VirtualKeys.Home || e.Key == VirtualKeys.End)
            {
                if (Utility.IsLockedShiftKey())
                {
                    Reset();
                }
                else
                {
                    if (!SentenceOnInput.TryMoveCaretPositionAsPostHomeOrEndKey(GetCaretCoordinate())) Reset();
                    Refresh();
                }
                return;
            }

            if (!Utility.IsLockedShiftKey() && (e.Key == VirtualKeys.OEMCommma || e.Key == VirtualKeys.OEMPeriod))
            {
                if (!IsIgnorePatternInput() && SentenceOnInput.Text.Length > 4 && _toolStripMenuItemRunning.Checked) // 自動変換の実行("desu."とか"masu."を自動で変換したいので4文字を制限とする)
                {
                    var txtHiragana = Utility.ConvertToHiragana(SentenceOnInput.Text);
                    bool isNumber = txtHiragana.All(c => ('0' <= c && c <= '9') || c == '、' || c == '。');

                    bool existAlphabet = txtHiragana.Any(Utility.IsLowerAlphabet);
                    if (!isNumber && !existAlphabet)
                    {
                        if (SentenceOnInput.Text.Length < 10) // sizeなど、ひらがなに変換できても英語の場合もある(さすがに10文字超えていたら大丈夫だろう…)
                        {
                            try
                            {
                                // もし該当する英単語があるなら自動変換は止めておく
                                if (!string.IsNullOrEmpty(ExternalServices.GetDictorynaryDataFromEnglishToJapanese(SentenceOnInput.Text, 200))) return;
                            }
                            catch (Exception ex) { }
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
                if (string.IsNullOrEmpty(SentenceOnInput.Text) && _lastAnswer != null)
                {
                    if (SentenceOnInput.HasMoved()) Location = PreLastSetDesktopLocation;
                    StartConvertDetail();
                }
                else if (!string.IsNullOrEmpty(SentenceOnInput.Text))
                {
                    ActionConvert();
                }
            }

        }

        Point _ptWhenStartConvert;

        private void StartConvertDetail()
        {
            // 変換ウインドウ立ち上げ時と終了時でキャレット位置がずれているようなら、変更をキャンセルする
            //　⇒さもないと、複数のBackSpaceキーが送信されてしまい、ファイル名変更の際などにとんでもないことになる。
            _ptWhenStartConvert = GetCaretCoordinate();
            Debug.WriteLine($" 変換開始前キャレット位置 x:{_ptWhenStartConvert.X}, y:{_ptWhenStartConvert.Y}");

            var location = Location;
            location.Y = location.Y + Height + _caretSize;

            _keyboardWatcher.Enable = false;
            _convertDetailForm.Start(_lastAnswer, location, _canceledConversion); // 変換失敗の記録が残っているなら、その選択状態をデフォルトとする
        }

        private void _convertDetailForm_ConvertExit(object? sender, DialogResult e)
        {
            if (e == DialogResult.OK)
            {
                _convertDetailForm.TargetSentence.RegisterConfirmedInput(InputHistory); // 入力履歴記録
                SplitHistory.RegisterHistory(_lastAnswer.MakeSentenceForHttpRequest(), _convertDetailForm.TargetSentence.MakeSentenceForHttpRequest()); // 分割編集履歴記録

                var pNew = GetCaretCoordinate();
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

                    var keys = new List<(VirtualKeys, KeyEventType)>();
                    if (true) // TODO:アプリケーションによってはBS×文字数で対処
                    {
                        keys.Add((VirtualKeys.ShiftLeft, KeyEventType.Down));
                        keys.AddRange(Utility.Duplicates((VirtualKeys.Left, KeyEventType.Stroke), _convertDetailForm.SentenceWhenStart.Length - isame));
                        keys.Add((VirtualKeys.ShiftLeft, KeyEventType.Up));
                        //keys.Add((VirtualKeys.BackSpace, KeyEventType.Stroke)); // TMEMO:VsVimでは巧く動作しない
                        keys.Add((VirtualKeys.Del, KeyEventType.Stroke)); // TMEMO:VsVimでは巧く動作しない
                    }
                    else
                    {
                        keys.AddRange(Utility.Duplicates((VirtualKeys.BackSpace, KeyEventType.Stroke), _convertDetailForm.SentenceWhenStart.Length - isame));
                    }
                    DeviceOperator.SendKeyEvents(keys.ToArray());

                    DeviceOperator.InputText(txtPost.Substring(isame));
                    //SendKeys.Send(txtPost.Substring(isame));

                    _lastAnswer = _convertDetailForm.TargetSentence;
                    _canceledConversion = null;
                }
                else
                {
                    _canceledConversion = _convertDetailForm.TargetSentence; // 変換の失敗を記録

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

            // 変換目的のSpace誤操作直後のBackSpaceによる状況復帰
            bool ignoreBSOrLeft = RestoreSoftReset(e.Key == VirtualKeys.BackSpace || e.Key == VirtualKeys.Left);

            Debug.WriteLine($"keyDown:{e.Key}");

            Task<Tuple<Point, Size>>? taskCaret1 = null;
            Task<Point>? taskCaret2 = null;
            if (_toolStripMenuItemRunning.Checked)
            {
                taskCaret1 = MSAA.GetCaretPositionAsync();
                taskCaret2 = Caret.GetCaretPositionAsync();
                //UIAutomation.GetCaretPosition(); // TOOD!:WPF対応
            }

            if (Environment.OSVersion.Version.Major >= 10)
            {
                var hwnd1 = this.Handle;
                var hwnd2 = _convertDetailForm.Handle;
                Task.Run(() =>
                {
                    if (!VirtualDesktopManager.DesktopManager.IsWindowOnCurrentVirtualDesktop(hwnd1))
                    {
                        var guid = VirtualDesktopManager.DesktopManager.GetWindowDesktopId(WindowInfo.ActiveWindowInfo.Handle);
                        VirtualDesktopManager.DesktopManager.MoveWindowToDesktop(hwnd1, ref guid);
                        VirtualDesktopManager.DesktopManager.MoveWindowToDesktop(hwnd2, ref guid);
                    }
                });
            }

            if (taskCaret1 != null && taskCaret2 != null)
            {
                SentenceOnInput.NotifyCurrentCaretCoordinate((taskCaret1.Result.Item1.Y != 0) ? taskCaret1.Result.Item1 : taskCaret2.Result);
            }

            // ひとまず、ショートカットキーっぽいものは軒並みリセット対象としておく
            if (_keyboardWatcher.IsKeyLocked(Keys.LControlKey) || _keyboardWatcher.IsKeyLocked(Keys.RControlKey)
             || _keyboardWatcher.IsKeyLocked(Keys.Alt) || _keyboardWatcher.IsKeyLocked(Keys.LWin) || _keyboardWatcher.IsKeyLocked(Keys.RWin))
            {
                Reset();
                return;
            }

            var input = KeyboardLayout.JudgeInputText(e.Key);
            if (input != null)
            {
                SentenceOnInput.InputText(input);
            }

            // 削除
            else if (e.Key == VirtualKeys.BackSpace)
            {
                if (!ignoreBSOrLeft && !SentenceOnInput.TryBackspace()) { Reset(); return; }
            }
            else if (e.Key == VirtualKeys.Del)
            {
                if (!SentenceOnInput.TryDelete()) { Reset(); return; }
            }

            // 移動
            else if (e.Key == VirtualKeys.Up || e.Key == VirtualKeys.Down)
            {
                Reset(); return;
            }
            else if (e.Key == VirtualKeys.Left)
            {
                if (!ignoreBSOrLeft && !SentenceOnInput.TryMoveLeft()) { Reset(); return; }
            }
            else if (e.Key == VirtualKeys.Right)
            {
                if (!SentenceOnInput.TryMoveRight()) { Reset(); return; }
            }
            else if (e.Key == VirtualKeys.Home || e.Key == VirtualKeys.End)
            {
                return; // 移動後のキャレット位置で有効有無を判定するため、KeyUpで処理する
            }

            // Shift + Escで未確定文字の削除
            else if (e.Key == VirtualKeys.Esc && Utility.IsLockedShiftKey())
            {
                DeleteCurrentText();
                return;
            }
            else if (e.Key == VirtualKeys.Shift || e.Key == VirtualKeys.ShiftLeft || e.Key == VirtualKeys.ShiftRight)
            {
                return; // _lastAnswerを消さないためにResetせずにreturnする
            }
            else if (e.Key == VirtualKeys.Space)
            {
                Reset(true);
                return;
            }

            // 原則としてはリセット
            else
            {
                Reset();
                return;
            }

            _labelJapaneseHiragana.Text = Utility.ConvertToHiragana(SentenceOnInput.Text);

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

                if (Opacity == 0.00 && SentenceOnInput.Text.Length == 1)
                {
                    SetDesktopLocation(p.X, p.Y);
                    _lastSetDesktopLocation = new Point(p.X, p.Y);
                    _caretSize = s.Height;
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

                bool viewIfNotJapanese = false;
                if (SentenceOnInput.CaretPosition != SentenceOnInput.Text.Length) viewIfNotJapanese = true;

                if (Opacity > 0.00) // 日本語じゃなさそうなら表示OFF
                {
                    if (SentenceOnInput.Text.Length == 0) Opacity = 0.00;
                    if (!viewIfNotJapanese && !Utility.IsMaybeJapaneseOnInput(_labelJapaneseHiragana.Text)) Opacity = 0.00;
                }
                else if (Opacity == 0.00 && _toolStripMenuItemNaviView.Checked) // 日本語っぽかったら再度表示
                {
                    int needHiragana = 2; // -1にすれば最初のアルファベットから表示される
                    if (!IsIgnorePatternInput() &&
                        _labelJapaneseHiragana.Text.Count(Utility.IsHiragana) > needHiragana &&
                        (Utility.IsMaybeJapaneseOnInput(_labelJapaneseHiragana.Text) || viewIfNotJapanese))
                    {
                        Opacity = 0.80;
                    }
                }
            }

            Refresh();
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
            var txtInput = SentenceOnInput.Text;

            //if (Opacity == 0.0 && txtInput.Length > 1) return;

            bool isOperationInput = true;
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
                isOperationInput = false;
            }

            FontFamily f = SystemFonts.DefaultFont.FontFamily;
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            if (!isOperationInput && SentenceOnInput.Text.Length > SentenceOnInput.CaretPosition) // 入力位置表示
            {
                Color colorCaret = Color.MediumPurple;
                var pre = SentenceOnInput.Text.Substring(0, SentenceOnInput.CaretPosition);
                var preHiragana = Utility.ConvertToHiragana(pre);

                if (txtShow.Substring(0, preHiragana.Length) == preHiragana)
                {
                    var pathDummy = new GraphicsPath();
                    var pathCaret = new GraphicsPath();
                    if (preHiragana.Length > 0)
                    {
                        pathDummy.AddString(preHiragana, f, 0, 12f, new Point(2, 2), null);
                        var rect = pathDummy.GetBounds();
                        pathCaret.AddLine(new Point((int)rect.Right + 2, (int)rect.Top), new Point((int)rect.Right + 2, (int)rect.Bottom + 2));
                    }
                    else
                    {
                        pathDummy.AddString("あ", f, 0, 12f, new Point(2, 2), null);
                        var rect = pathDummy.GetBounds();
                        pathCaret.AddLine(new Point(2, (int)rect.Top), new Point(2, (int)rect.Bottom + 2));
                    }

                    e.Graphics.DrawPath(new Pen(colorCaret, 2), pathCaret);
                }
                else
                {
                    var pathDummy = new GraphicsPath();
                    var rect1 = new RectangleF(0, 2, 2, 12);
                    if (preHiragana.Length > 1)
                    {
                        pathDummy.AddString(txtShow.Substring(0, preHiragana.Length - 1), f, 0, 12f, new Point(2, 2), null);
                        rect1 = pathDummy.GetBounds();
                    }

                    pathDummy.AddString(txtShow.Substring(0, preHiragana.Length), f, 0, 12f, new Point(2, 2), null);
                    var rect2 = pathDummy.GetBounds();

                    var pathCaret = new GraphicsPath();
                    pathCaret.AddRectangle(new Rectangle((int)rect1.Right + 2, (int)rect1.Top, (int)rect2.Right - (int)rect1.Right - 1, (int)rect2.Height + 2));

                    e.Graphics.FillPath(new SolidBrush(colorCaret), pathCaret);
                }
            }

            // 入力文字表示
            var path = new GraphicsPath();
            {
                path.AddString(txtShow, f, 0, 12f, new Point(2, 2), null);
                e.Graphics.FillPath(new SolidBrush(color), path);
            }

            if (!isOperationInput) // アルファベットのみ赤色で表示
            {
                var txtDummy = "";
                foreach (var c in txtShow)
                {
                    txtDummy += c;
                    if (Utility.IsAlphabet(c))
                    {
                        var pathDummy = new GraphicsPath();
                        pathDummy.AddString(txtDummy, f, 0, 12f, new Point(2, 2), null);

                        var pathA = new GraphicsPath();
                        var r = pathDummy.GetBounds().Right;
                        pathA.AddString(c.ToString(), f, 0, 12f, new Point(2, 2), null);
                        var r_ = pathA.GetBounds().Right;

                        Matrix m = new Matrix(); m.Translate((float)(r - r_), 0f, MatrixOrder.Append);
                        pathA.Transform(m);
                        e.Graphics.FillPath(new SolidBrush(Color.Red), pathA);
                    }
                }
            }

            var x = (int)path.GetBounds().Width + 10;
            var y = (int)path.GetBounds().Height + 10;
            Size = new Size(x, y);

            var y_ = Math.Max(_lastSetDesktopLocation.Y - y, 0);
            SetDesktopLocation(_lastSetDesktopLocation.X, y_);
        }

        private void Form1_FormClosing(object sender, EventArgs e)
        {
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true, Encoder = JavaScriptEncoder.Create(UnicodeRanges.All) };
                using FileStream createStream = File.Create(_filepathInputHistroy);
                JsonSerializer.Serialize(createStream, InputHistory, options);
                createStream.Dispose();
            }
            catch
            {
            }

            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true, Encoder = JavaScriptEncoder.Create(UnicodeRanges.All) };
                using FileStream createStream = File.Create(_filepathSplitHistroy);
                JsonSerializer.Serialize(createStream, SplitHistory, options);
                createStream.Dispose();
            }
            catch
            {
            }
        }

        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                cp.ExStyle |= 0x80; return cp;
            }
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
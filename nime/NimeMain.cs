﻿using GoodSeat.Nime.Windows;
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
using GoodSeat.Nime.Core.KeySequences;
using System.IO;

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
         *   !や?も区切り文字として自動変換対象とするのもよいだろう
         *   マウスクリックで問答無用リセット(ホイールも、もはやマウス動いただけでもリセットすべきかも)
         *   変換ウインドウ上で、開始括弧を変換したときに対応する閉じ括弧も合わせて変換する
         *   変換ウインドウ上でも、矢印で選択可能にする
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
         *   
         *   入力補完時、分割されている候補を簡単に結合させて記録させ直したい。入力補完用データだけで良いので。
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
         *     → DeviceOperator.InputText(ans.GetFirstSentence()); の入力が終わるまでに入力されたものは、一旦キャンセルして終わった後に遅延してシミュレートする
         *     → 対応したつもりだが、どうもうまく動作しない。
         *   Explorer上の、名前の変更、検索ボックス、アドレスボックスは軒並みIME直接編集ウインドウが使えない…（直接編集ウインドウ出した時点でフォーカスを失ってキャレットが外れてしまうので）
         *   SendInputやSendKeysで半角カタカナが入力できない、クリップボードを利用する他ないかもしれない
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
         * ## MEMO
         *  キーレイアウトとスキャンコード
         *  https://bsakatu.net/doc/scancode/
         *  http://hp.vector.co.jp/authors/VA003720/lpproj/others/kbdjpn.htm
         */

        public NimeMain()
        {
            InitializeComponent();

            this.Paint += Form1_Paint;

            Reset();
            Load();

            _convertDetailForm = new ConvertDetailForm(InputHistory, _convertToSentence);
            _convertDetailForm.ConvertExit += _convertDetailForm_ConvertExit;
            _convertDetailForm.Show();
            _convertDetailForm.TopMost = true;

            _inputSuggestForm = new InputSuggestForm(InputSuggestion);
            _inputSuggestForm.Show();
            _inputSuggestForm.SuggestExit += _inputSuggestForm_SuggestExit;

            _keyboardWatcher = new KeyboardWatcher();
            _keyboardWatcher.KeyUp += KeyboardWatcher_KeyUp;
            _keyboardWatcher.KeyDown += KeyboardWatcher_KeyDown;
            _keyboardWatcher.Enable = true;

        }

        string _filepathInputHistroy = "input.json";
        string _filepathSplitHistroy = "split.json";
        string _filepathInputSuggestion = "suggest.json";
        string _filepathSetting = "setting.json";

        KeyboardWatcher _keyboardWatcher;
        ConvertDetailForm _convertDetailForm;
        InputSuggestForm _inputSuggestForm;

        string _currentHiragana = "";
        Point _lastSetDesktopLocation = Point.Empty;
        Point _preLastSetDesktopLocation;
        int _caretSize = 0;

        ConvertCandidate _lastAnswer;
        ConvertCandidate _canceledConversion = null;
        SentenceOnInput _sentenceOnInput = new SentenceOnInput();
        SentenceOnInput _preSentenceOnInput;
        HiraganaSet? _lastPhrase;
        HiraganaSet? _lastPhrase2;
        Point _ptWhenStartConvert;

        Setting _setting = new Setting();
        ApplicationSetting? _currentAppSetting = null;

        ConvertToSentence _convertToSentence = new ConvertToSentence(); // 共通設定

        KeyboardLayout KeyboardLayout { get; set; } = new KeyboardLayoutUS(); // 共通設定
        InputHistory InputHistory { get; set; }
        SplitHistory SplitHistory { get; set; }
        InputSuggestion InputSuggestion { get; set; }

        bool ConvertOnlyVisibleInputNavi { get; set; } = true;

        /// <summary>
        /// 対象とする<see cref="WindowInfo"/>の変更時に呼び出されます。
        /// </summary>
        public static event EventHandler<WindowInfo> TargetWindowChanged;

        /// <summary>
        /// 直近の入力で対象とした<see cref="WindowInfo"/>を取得します。
        /// </summary>
        public static WindowInfo TargetWindowInfoMRU { get; set; }

        /// <summary>
        /// 現在対象とすべきアプリケーション設定を取得します。
        /// </summary>
        ApplicationSetting CurrentApplicationSetting
        {
            get
            {
                if (_currentAppSetting == null)
                {
                    _currentAppSetting = _setting.SearchCurrentSetting();
                    TargetWindowInfoMRU = WindowInfo.ActiveWindowInfo;
                    TargetWindowChanged?.Invoke(this, TargetWindowInfoMRU);
                }
                return _currentAppSetting;
            }
        }


        private void Reset(bool softReset = false)
        {
            Debug.WriteLine("Reset");

            _preLastSetDesktopLocation = _lastSetDesktopLocation;

            _preSentenceOnInput = softReset ? _sentenceOnInput : null;

            _lastAnswer = null;
            _canceledConversion = null;
            _currentAppSetting = null;

            _sentenceOnInput = new SentenceOnInput();

            _currentHiragana = "";
            _lastPhrase2 = _lastPhrase;
            _lastPhrase = null;
            Opacity = 0.00;

            _inputSuggestForm?.Clear();
        }

        private bool RestoreSoftReset(bool restore)
        {
            if (_preSentenceOnInput == null) return false;

            if (restore) _sentenceOnInput = _preSentenceOnInput;

            _preSentenceOnInput = null;
            return restore;
        }

        private void DeleteCurrentText()
        {
            CurrentApplicationSetting.Delete.Operate(TargetWindowInfoMRU, _sentenceOnInput.Text.Length, _sentenceOnInput.CaretPosition);
            Reset();
        }

        private bool IsIgnorePatternInput()
        {
            if (string.IsNullOrEmpty(_sentenceOnInput.Text)) return false;

            var c = _sentenceOnInput.Text[0];
            return ('A' <= c && c <= 'Z'); // 1文字目が大文字の場合は無視
        }

        private void Load()
        {
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

            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true, Encoder = JavaScriptEncoder.Create(UnicodeRanges.All) };
                string jsonString = File.ReadAllText(_filepathInputSuggestion);

                InputSuggestion = JsonSerializer.Deserialize<InputSuggestion>(jsonString, options);
            }
            catch
            {
            }
            if (InputSuggestion == null) InputSuggestion = new InputSuggestion();

            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true, Encoder = JavaScriptEncoder.Create(UnicodeRanges.All) };
                string jsonString = File.ReadAllText(_filepathSetting);

                var data = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(jsonString, options);
                _setting.Deserialize(data);
            }
            catch { _setting = new Setting(); }
        }

        private void Save()
        {
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true, Encoder = JavaScriptEncoder.Create(UnicodeRanges.All) };
                using FileStream createStream = File.Create(_filepathInputHistroy);
                JsonSerializer.Serialize(createStream, InputHistory, options);
                createStream.Dispose();
            }
            catch { }

            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true, Encoder = JavaScriptEncoder.Create(UnicodeRanges.All) };
                using FileStream createStream = File.Create(_filepathSplitHistroy);
                JsonSerializer.Serialize(createStream, SplitHistory, options);
                createStream.Dispose();
            }
            catch { }

            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true, Encoder = JavaScriptEncoder.Create(UnicodeRanges.All) };
                using FileStream createStream = File.Create(_filepathInputSuggestion);
                JsonSerializer.Serialize(createStream, InputSuggestion, options);
                createStream.Dispose();
            }
            catch { }

            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true, Encoder = JavaScriptEncoder.Create(UnicodeRanges.All) };
                using FileStream createStream = File.Create(_filepathSetting);
                JsonSerializer.Serialize(createStream, _setting.Serialize(), options);
                createStream.Dispose();
            }
            catch { }
        }


        string[] _goodBys = new string[]{
            """ Good Bye! """,
            """ Thank you for using! """,
            """ See you again! """,
        };

        private bool OperateWithKeyword(string txt)
        {
            // キーワード操作受付
            if (!_toolStripMenuItemRunning.Checked && txt == _setting.KeywordStart)
            {
                _toolStripMenuItemRunning.Checked = true;
                notifyIcon1.ShowBalloonTip(2000, "nime", "入力受付を再開しました。", ToolTipIcon.Info);
                return true;
            }
            else if (txt == _setting.KeywordExit)
            {
                if (_setting.SayGoodByeWhenExitByKeyword)
                {
                    Random r = new Random();
                    var msg = " ■" + _goodBys[r.Next(0, _goodBys.Length)] + "■ ";
                    CurrentApplicationSetting.Input.Operate(TargetWindowInfoMRU, msg);
                    Thread.Sleep(500);
                    new DeviceOperator().SendKeyEvents(Utility.Duplicates((VirtualKeys.BackSpace, KeyEventType.Stroke), msg.Length).ToArray());
                }
                _toolStripMenuItemExist_Click(null, EventArgs.Empty);
                return true;
            }
            if (!_toolStripMenuItemRunning.Checked) return false;

            if (txt == _setting.KeywordStop)
            {
                _toolStripMenuItemRunning.Checked = false;
                notifyIcon1.ShowBalloonTip(2000, "nime", "入力受付を停止しました。", ToolTipIcon.Info);
                return true;
            }
            else if (txt == _setting.KeywordVisible)
            {
                _toolStripMenuItemNaviView_Click(null, EventArgs.Empty);
                if (_toolStripMenuItemNaviView.Checked) notifyIcon1.ShowBalloonTip(2000, "nime", "入力表示をONにしました。", ToolTipIcon.Info);
                else notifyIcon1.ShowBalloonTip(2000, "nime", "入力表示をOFFにしました。", ToolTipIcon.Info);
                return true;
            }
            else if (txt == _setting.KeywordSupport)
            {
                _toolStripMenuInputSupport_Click(null, EventArgs.Empty);
                if (_toolStripMenuInputSupport.Checked) notifyIcon1.ShowBalloonTip(2000, "nime", "入力補完をONにしました。", ToolTipIcon.Info);
                else notifyIcon1.ShowBalloonTip(2000, "nime", "入力補完をOFFにしました。", ToolTipIcon.Info);
                return true;
            }
            else if (txt == _setting.KeywordSetting)
            {
                _toolStripMenuItemSetting_Click(this, EventArgs.Empty);
                return true;
            }
            return false;
        }

        private async void ActionConvert(ConvertToSentence.ForceMode? mode = null)
        {
            var txt = _sentenceOnInput.Text;
            if (!_toolStripMenuItemRunning.Checked && (!_setting.EnableOperateByKeyword || (txt != _setting.KeywordStart && txt != _setting.KeywordExit))) return;

            var lastPhrase = _lastPhrase;
            ConvertCandidate? result = null;

            // ここからキーイベントをキャンセル(記録しておく)
            using (var keyDelay = new DelayKeyInput(_keyboardWatcher))
            {
                if (string.IsNullOrEmpty(txt)) { Reset(); return; }

                Debug.WriteLine("■ 変換開始:" + DateTime.Now.ToString() + "\"" + DateTime.Now.Millisecond.ToString());

                int timeout = 200;
                Task<ConvertCandidate?> ans;

                if (mode == null)
                {
                    ans = _convertToSentence.ConvertFromRomajiAsync(txt, InputHistory, SplitHistory, timeout);
                }
                else
                {
                    ans = _convertToSentence.ConvertFromRomajiAsync(txt, InputHistory, timeout, mode.Value);
                }

                DeleteCurrentText();

                if (_setting.EnableOperateByKeyword && OperateWithKeyword(txt)) return; // キーワード操作受付

                result = ans.Result;
                if (result == null)
                {
                    notifyIcon1.ShowBalloonTip(5000, "[nime]エラー", "変換に失敗しました。", ToolTipIcon.Error);
                }
                else
                {
                    _lastAnswer = result;
                    CurrentApplicationSetting.Input.Operate(TargetWindowInfoMRU, _lastAnswer.GetSelectedSentence());

                    //Application.DoEvents();
                }
            }

            Debug.WriteLine("■ 変換終了:" + DateTime.Now.ToString() + "\"" + DateTime.Now.Millisecond.ToString());

            if (result != null)
            {
                _ = InputSuggestion.RegisterHiraganaSequenceAsync(result, lastPhrase);

                if (result.PhraseList.Any() && _toolStripMenuInputSupport.Checked)
                {
                    DateTime time = DateTime.Now;

                    Thread.Sleep(25); // MEMO:なぜかこれを挟まないとキャレット位置が正しく更新されない…
                    var (caretPos, caretSize) = Utility.GetCaretCoordinateAndSize();
                    var p = caretPos;
                    var location = new Point(p.X, p.Y + _caretSize);

                    _lastPhrase = InputSuggestion.ToHiraganaSetList(result).Last();
                    var suggest = await InputSuggestion.SearchPostOfAsync(_lastPhrase, 3);
                    _inputSuggestForm.UpdateSuggestion(suggest, time, location);
                }
            }
        }

        private void StartConvertDetail()
        {
            // 変換ウインドウ立ち上げ時と終了時でキャレット位置がずれているようなら、変更をキャンセルする
            //　⇒さもないと、複数のBackSpaceキーが送信されてしまい、ファイル名変更の際などにとんでもないことになる。
            _ptWhenStartConvert = Utility.GetCaretCoordinate();
            Debug.WriteLine($" 変換開始前キャレット位置 x:{_ptWhenStartConvert.X}, y:{_ptWhenStartConvert.Y}");

            var location = Location;
            location.Y = location.Y + Height + _caretSize;

            _inputSuggestForm.Opacity = 0.0;
            _keyboardWatcher.Enable = false;
            _convertDetailForm.Start(_lastAnswer, location, _canceledConversion); // 変換失敗の記録が残っているなら、その選択状態をデフォルトとする
        }

        private async void _convertDetailForm_ConvertExit(object? sender, DialogResult e)
        {
            if (e == DialogResult.OK)
            {
                _convertDetailForm.TargetSentence.RegisterConfirmedInput(InputHistory); // 入力履歴記録
                SplitHistory.RegisterHistory(_lastAnswer.MakeSentenceForHttpRequest(), _convertDetailForm.TargetSentence.MakeSentenceForHttpRequest()); // 分割編集履歴記録

                var pNew = Utility.GetCaretCoordinate();
                Debug.WriteLine($" -> 変換後キャレット位置 x:{pNew.X}, y:{pNew.Y}");
                if (CurrentApplicationSetting.IgnoreCaretChanged || Math.Abs(_ptWhenStartConvert.Y - pNew.Y) < 10 && Math.Abs(_ptWhenStartConvert.X - pNew.X) < 200)
                {
                    Debug.WriteLine($"   -> 変換実施");
                    var txtPost = _convertDetailForm.TargetSentence.GetSelectedSentence();
                    int isame = 0;
                    for (isame = 0; isame < Math.Min(_convertDetailForm.SentenceWhenStart.Length, txtPost.Length); isame++)
                    {
                        if (_convertDetailForm.SentenceWhenStart[isame] != txtPost[isame]) break;
                    }

                    _lastAnswer = _convertDetailForm.TargetSentence;
                    _canceledConversion = null;

                    _ = InputSuggestion.RegisterHiraganaSequenceAsync(_lastAnswer, _lastPhrase2);

                    int length = _convertDetailForm.SentenceWhenStart.Length - isame;
                    CurrentApplicationSetting.Delete.Operate(TargetWindowInfoMRU, length, length);
                    CurrentApplicationSetting.Input.Operate(TargetWindowInfoMRU, txtPost.Substring(isame));

                    DateTime time = DateTime.Now;
                    var location = Location;
                    location.Y = location.Y + Height + _caretSize;

                    if (_lastAnswer.PhraseList.Any() && _toolStripMenuInputSupport.Checked)
                    {
                        _lastPhrase = InputSuggestion.ToHiraganaSetList(_lastAnswer).Last();
                        var suggest = await InputSuggestion.SearchPostOfAsync(_lastPhrase, 3);
                        _inputSuggestForm.UpdateSuggestion(suggest, time, location);
                    }
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

        private void _inputSuggestForm_SuggestExit(object? sender, DialogResult e)
        {
            _keyboardWatcher.Enable = true;
            if (e == DialogResult.OK && _inputSuggestForm.ConfirmedPhraseList.Any())
            {
                DeleteCurrentText();

                DateTime time = DateTime.Now;
                _lastPhrase = _inputSuggestForm.ConfirmedPhraseList.Last();
                var suggest = InputSuggestion.SearchPostOfAsync(_lastPhrase, 3);

                var lst = _inputSuggestForm.ConfirmedPhraseList.ToList();
                if (_lastPhrase2 != null) lst.Insert(0, _lastPhrase2);
                foreach (var h in lst) Debug.WriteLine($"  入力補完候補登録:{h.Phrase}({h.Hiragana})");
                _ = InputSuggestion.RegisterHiraganaSequenceAsync(lst);

                var text = _inputSuggestForm.ConfirmedPhraseList.Select(h => h.Phrase).Aggregate((s1, s2) => s1 + s2);
                CurrentApplicationSetting.Input.Operate(TargetWindowInfoMRU, text);

                Thread.Sleep(75); // MEMO:なぜかこれを挟まないとキャレット位置が正しく更新されない…
                var (caretPos, caretSize) = Utility.GetCaretCoordinateAndSize();
                var p = caretPos;
                var location = new Point(p.X, p.Y + _caretSize);
                _inputSuggestForm.UpdateSuggestion(suggest.Result, time, location);

                // この直後に変換操作を実行した際に、直前の入力補完による入力に対して詳細変換を実施できるようにする
                Task.Run(() =>
                {
                    var ans = _convertToSentence.ConvertFromHiragana(string.Join(",", _inputSuggestForm.ConfirmedPhraseList.Select(p => p.Hiragana)), InputHistory, SplitHistory, 200);
                    ans.PhraseList.Zip(_inputSuggestForm.ConfirmedPhraseList, (p1, p2) =>
                    {
                        p1.Selected = p2.Phrase;
                        return "";
                    });

                    if (_lastAnswer == null) _lastAnswer = ans;
                });
            }
        }


        private void KeyboardWatcher_KeyUp(object? sender, KeyboardWatcher.KeybordWatcherEventArgs e)
        {
            if (IMEWatcher.IsOnIME(true)) return;
            if (e.Key == VirtualKeys.Packet) return;

            Debug.WriteLine($"keyUp:{e.Key} ScanCode:{e.ScanCode.ToString("x")}");

            var txtHiragana = Utility.ConvertToHiragana(_sentenceOnInput.Text);
            bool isNumber = _sentenceOnInput.Text.All(c => ('0' <= c && c <= '9') || c == '-' || c == ',' || c == '.');

            bool existAlphabet = txtHiragana.Any(Utility.IsLowerAlphabet);
            bool existHiragana = txtHiragana.Replace("、", "").Replace("。", "").Replace("ー", "").Any(Utility.IsHiragana);
            bool doAutoConvert = existHiragana && !isNumber && !existAlphabet;

            if (e.Key == VirtualKeys.Home || e.Key == VirtualKeys.End)
            {
                if (Utility.IsLockedShiftKey())
                {
                    Reset();
                }
                else
                {
                    if (!_sentenceOnInput.TryMoveCaretPositionAsPostHomeOrEndKey(Utility.GetCaretCoordinate())) Reset();
                    Refresh();
                }
                return;
            }
            else if (e.Key == VirtualKeys.Down && _inputSuggestForm.Opacity != 0)
            {
                if (_inputSuggestForm.StartSuggestion())
                {
                    e.Cancel = true;
                    _keyboardWatcher.Enable = false;
                    return;
                }
            }
            else if (Utility.IsLockedCtrlKey() && doAutoConvert)
            {
                if (e.Key == VirtualKeys.U && Opacity > 0.0)
                {
                    if (!CurrentApplicationSetting.UseForceModeOnlyHiraganaWithCtrlU) return;
                    ActionConvert(ConvertToSentence.ForceMode.OnlyHiragana);
                    e.Cancel = true;
                    return;
                }
                else if (e.Key == VirtualKeys.I && Opacity > 0.0)
                {
                    if (!CurrentApplicationSetting.UseForceModeOnlyKatakanaWithCtrlI) return;
                    ActionConvert(ConvertToSentence.ForceMode.OnlyKatakana);
                    e.Cancel = true;
                    return;
                }
                else if (e.Key == VirtualKeys.O && Opacity > 0.0)
                {
                    if (!CurrentApplicationSetting.UseForceModeOnlyHalfKatakanaWithCtrlO) return;
                    ActionConvert(ConvertToSentence.ForceMode.OnlyHalfKatakana);
                    e.Cancel = true;
                    return;
                }
                else if (e.Key == VirtualKeys.P && Opacity > 0.0)
                {
                    if (!CurrentApplicationSetting.UseForceModeOnlyWideRomajiWithCtrlP) return;
                    ActionConvert(ConvertToSentence.ForceMode.OnlyWideRomaji);
                    e.Cancel = true;
                    return;
                }
            }
            else if (e.Key == VirtualKeys.F6 && Opacity > 0.0 && doAutoConvert)
            {
                if (!CurrentApplicationSetting.UseForceModeOnlyHiraganaWithF6) return;
                ActionConvert(ConvertToSentence.ForceMode.OnlyHiragana);
                e.Cancel = true;
                return;
            }
            else if (e.Key == VirtualKeys.F7 && Opacity > 0.0 && doAutoConvert)
            {
                if (!CurrentApplicationSetting.UseForceModeOnlyKatakanaWithF7) return;
                ActionConvert(ConvertToSentence.ForceMode.OnlyKatakana);
                e.Cancel = true;
                return;
            }
            else if (e.Key == VirtualKeys.F8 && Opacity > 0.0 && doAutoConvert)
            {
                if (!CurrentApplicationSetting.UseForceModeOnlyHalfKatakanaWithF8) return;
                ActionConvert(ConvertToSentence.ForceMode.OnlyHalfKatakana);
                e.Cancel = true;
                return;
            }
            else if (e.Key == VirtualKeys.F9 && Opacity > 0.0 && doAutoConvert)
            {
                if (!CurrentApplicationSetting.UseForceModeOnlyWideRomajiWithF9) return;
                ActionConvert(ConvertToSentence.ForceMode.OnlyWideRomaji);
                e.Cancel = true;
                return;
            }


            if (!Utility.IsLockedShiftKey() && (e.Key == VirtualKeys.OEMCommma || e.Key == VirtualKeys.OEMPeriod))
            {
                if (e.Key == VirtualKeys.OEMCommma && !CurrentApplicationSetting.AutoConvertOnInputCommma) return;
                if (e.Key == VirtualKeys.OEMPeriod && !CurrentApplicationSetting.AutoConvertOnInputPeriod) return;

                bool isMaybeMailAddress = _sentenceOnInput.Text.Contains("@") && e.Key == VirtualKeys.OEMPeriod;
                if (!isMaybeMailAddress && !IsIgnorePatternInput() && _sentenceOnInput.Text.Length > 4 && _toolStripMenuItemRunning.Checked) // 自動変換の実行("desu."とか"masu."を自動で変換したいので4文字を制限とする)
                {
                    if (doAutoConvert)
                    {
                        if (doAutoConvert && _sentenceOnInput.Text.Length < 10) // sizeなど、ひらがなに変換できても英語の場合もある(さすがに10文字超えていたら大丈夫だろう…)
                        {
                            try
                            {
                                // もし該当する英単語があるなら自動変換は止めておく
                                if (!string.IsNullOrEmpty(ExternalServices.GetDictorynaryDataFromEnglishToJapanese(_sentenceOnInput.Text, 200))) return;
                            }
                            catch (Exception ex) { }
                        }
                        ActionConvert();
                        //Task.Run(() => ActionConvert());
                    }
                }
                return;
            }

            //if (e.Key != VirtualKeys.ShiftRight) return;
            //if (e.Key != VirtualKeys.Convert) return;
            if (e.ScanCode != 0x79) return; // 変換break

            if (string.IsNullOrEmpty(_sentenceOnInput.Text) && _lastAnswer != null)
            {
                if (_sentenceOnInput.HasMoved()) Location = _preLastSetDesktopLocation;
                StartConvertDetail();
            }
            else if (!string.IsNullOrEmpty(_sentenceOnInput.Text) && (!ConvertOnlyVisibleInputNavi || _setting.IsOperateKeyword(_sentenceOnInput.Text) || Opacity > 0.0 || !Utility.ConvertToHiragana(_sentenceOnInput.Text).Any(Utility.IsAlphabet)))
            {
                ActionConvert();
                //Task.Run(() => ActionConvert());
            }

        }

        private async void KeyboardWatcher_KeyDown(object? sender, KeyboardWatcher.KeybordWatcherEventArgs e)
        {
            if (IMEWatcher.IsOnIME(true)) { Reset(); return; }
            if (e.Key == VirtualKeys.Packet) return;
            if (string.IsNullOrEmpty(_sentenceOnInput.Text)) _currentAppSetting = null;

            // 変換目的のSpace誤操作直後のBackSpaceによる状況復帰
            bool ignoreBSOrLeft = RestoreSoftReset(e.Key == VirtualKeys.BackSpace || e.Key == VirtualKeys.Left);

            Debug.WriteLine($"keyDown:{e.Key} ScanCode:{e.ScanCode.ToString("x")}");

            // アクティブな仮想デスクトップに移動
            if (Environment.OSVersion.Version.Major >= 10)
            {
                try
                {
                    var hwnd1 = this.Handle;
                    var hwnd2 = _convertDetailForm.Handle;
                    var hwnd3 = _inputSuggestForm.Handle;
                    _ = Task.Run(() =>
                    {
                        if (!VirtualDesktopManager.DesktopManager.IsWindowOnCurrentVirtualDesktop(hwnd1))
                        {
                            var guid = VirtualDesktopManager.DesktopManager.GetWindowDesktopId(WindowInfo.ActiveWindowInfo.Handle);
                            VirtualDesktopManager.DesktopManager.MoveWindowToDesktop(hwnd1, ref guid);
                            VirtualDesktopManager.DesktopManager.MoveWindowToDesktop(hwnd2, ref guid);
                            VirtualDesktopManager.DesktopManager.MoveWindowToDesktop(hwnd3, ref guid);
                        }
                    });
                }
                catch { }
            }

            // キャレット位置判定、通知
            Point? caretPos = null;
            Size? caretSize = null;
            if (_toolStripMenuItemRunning.Checked) (caretPos, caretSize) = Utility.GetCaretCoordinateAndSize();
            if (caretPos != null) _sentenceOnInput.NotifyCurrentCaretCoordinate(caretPos.Value);

            // 入力補完モード
            if (e.Key == VirtualKeys.Down && _inputSuggestForm.Opacity != 0)
            {
                // MEMO:入力補完以降:候補が一つしかないときに、なぜかTab文字が入力されてしまうため、KeyUpで実行する(-> KeyboardWatherの修正に伴って、もしかしたら直っているかも)
                e.Cancel = true;
                return;
            }

            if (Opacity > 0.0 && (e.Key == VirtualKeys.ControlLeft))
            {
                // CtrlはKeyUpでResetする
                return;
            }
            // ひとまず、ショートカットキーっぽいものは軒並みリセット対象としておく
            else if (Opacity > 0.0 && _keyboardWatcher.IsKeyLocked(Keys.LControlKey))
            {
                var txtHiragana = Utility.ConvertToHiragana(_sentenceOnInput.Text);
                bool isNumber = _sentenceOnInput.Text.All(c => ('0' <= c && c <= '9') || c == '-' || c == ',' || c == '.');
                bool existAlphabet = txtHiragana.Any(Utility.IsLowerAlphabet);
                bool existHiragana = txtHiragana.Replace("、", "").Replace("。", "").Replace("ー", "").Any(Utility.IsHiragana);
                bool doAutoConvert = existHiragana && !isNumber && !existAlphabet;

                if (doAutoConvert && 
                    (  (e.Key == VirtualKeys.U && CurrentApplicationSetting.UseForceModeOnlyHiraganaWithCtrlU)
                    || (e.Key == VirtualKeys.I && CurrentApplicationSetting.UseForceModeOnlyKatakanaWithCtrlI)
                    || (e.Key == VirtualKeys.O && CurrentApplicationSetting.UseForceModeOnlyHalfKatakanaWithCtrlO)
                    || (e.Key == VirtualKeys.P && CurrentApplicationSetting.UseForceModeOnlyWideRomajiWithCtrlP)
                    ))
                {
                    e.Cancel = true;
                }
                else
                {
                    Reset();
                }
                return;
            }
            else if (Opacity > 0.0 && (e.Key == VirtualKeys.F6 || e.Key == VirtualKeys.F7 || e.Key == VirtualKeys.F8 || e.Key == VirtualKeys.F9))
            {
                e.Cancel = true;
                return;
            }

            // ひとまず、ショートカットキーっぽいものは軒並みリセット対象としておく
            else if (_keyboardWatcher.IsKeyLocked(Keys.LControlKey) || _keyboardWatcher.IsKeyLocked(Keys.RControlKey) ||
                     _keyboardWatcher.IsKeyLocked(Keys.Alt) || _keyboardWatcher.IsKeyLocked(Keys.LWin) || _keyboardWatcher.IsKeyLocked(Keys.RWin))
            {
                Reset(); return;
            }

            var input = KeyboardLayout.JudgeInputText(e.Key);
            if (input != null)
            {
                if (_currentAppSetting == null)
                {
                    _currentAppSetting = CurrentApplicationSetting;
                }
                _sentenceOnInput.InputText(input);
            }
            // 削除
            else if (e.Key == VirtualKeys.BackSpace)
            {
                if (!ignoreBSOrLeft && !_sentenceOnInput.TryBackspace()) { Reset(); return; }
            }
            else if (e.Key == VirtualKeys.Del)
            {
                if (!_sentenceOnInput.TryDelete()) { Reset(); return; }
            }

            // 移動
            else if (e.Key == VirtualKeys.Up || e.Key == VirtualKeys.Down)
            {
                Reset(); return;
            }
            else if (e.Key == VirtualKeys.Left)
            {
                if (!ignoreBSOrLeft && !_sentenceOnInput.TryMoveLeft()) { Reset(); return; }
            }
            else if (e.Key == VirtualKeys.Right)
            {
                if (!_sentenceOnInput.TryMoveRight()) { Reset(); return; }
            }
            else if (e.Key == VirtualKeys.Home || e.Key == VirtualKeys.End)
            {
                if (Utility.IsLockedShiftKey()) Reset();
                return; // 移動後のキャレット位置で有効有無を判定するため、KeyUpで処理する
            }

            // Shift + Escで未確定文字の削除
            else if (e.Key == VirtualKeys.Esc && Utility.IsLockedShiftKey())
            {
                DeleteCurrentText(); return;
            }
            else if (e.Key == VirtualKeys.Shift || e.Key == VirtualKeys.ShiftLeft || e.Key == VirtualKeys.ShiftRight)
            {
                return; // _lastAnswerを消さないためにResetせずにreturnする
            }
            else if (e.Key == VirtualKeys.Convert || e.Key == VirtualKeys.NonConvert || (int)e.ScanCode == 0x79)
            {
                return;
            }
            else if (e.Key == VirtualKeys.Space)
            {
                Reset(true); return;
            }

            // 原則としてはリセット
            else
            {
                Reset(); return;
            }

            _currentHiragana = Utility.ConvertToHiragana(_sentenceOnInput.Text, false);

            // 入力表示更新
            if (caretPos != null)
            {
                var p = caretPos.Value;
                var s = caretSize.Value;
                if (_sentenceOnInput.Text.Length == 1)
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
                if (_sentenceOnInput.CaretPosition != _sentenceOnInput.Text.Length) viewIfNotJapanese = true;

                if (Opacity > 0.00) // 日本語じゃなさそうなら表示OFF
                {
                    if (_sentenceOnInput.Text.Length == 0) Opacity = 0.00;
                    if (!viewIfNotJapanese && !Utility.IsMaybeJapaneseOnInput(_currentHiragana)) Opacity = 0.00;
                }
                else if (Opacity == 0.00 && _toolStripMenuItemNaviView.Checked) // 日本語っぽかったら再度表示
                {
                    bool review = _setting.IsOperateKeyword(_sentenceOnInput.Text);

                    int needHiragana = 2; // -1にすれば最初のアルファベットから表示される
                    if (!review) review = !IsIgnorePatternInput() && _currentHiragana.Count(Utility.IsHiragana) > needHiragana && (Utility.IsMaybeJapaneseOnInput(_currentHiragana) || viewIfNotJapanese);

                    if (review) Opacity = 0.80;
                }
            }

            Refresh();

            DateTime time = DateTime.Now;
            var location = Location;
            location.Y = location.Y + Height + _caretSize;

            if (!IsIgnorePatternInput() && _toolStripMenuItemRunning.Checked && _toolStripMenuInputSupport.Checked)
            {
                if (_currentHiragana.Length >= 2 || _inputSuggestForm.Opacity != 0.0)
                {
                    var suggest = await InputSuggestion.SearchStartWithAsync(_currentHiragana, 3);
                    _inputSuggestForm.UpdateSuggestion(suggest, time, location);
                }
                else
                {
                    _inputSuggestForm.UpdateSuggestion(null, time, location);
                }
            }
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            TopMost = true;

            var layout = KeyboardLayoutDetection.GetKeyboardLayeout();
            notifyIcon1.ShowBalloonTip(2000, "認識されたキーボード", InputLanguage.CurrentInputLanguage?.LayoutName?.ToString() + "\r\nキーボードレイアウト:" + layout.ToString(), ToolTipIcon.Info);

            //var klID = InputLanguage.CurrentInputLanguage.Culture.KeyboardLayoutId;
            //notifyIcon1.ShowBalloonTip(2000, "認識されたキーボード", InputLanguage.CurrentInputLanguage?.LayoutName?.ToString() + "\r\nキーボードレイアウトID:" + klID.ToString(), ToolTipIcon.Info);

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

        private void _toolStripMenuInputSupport_Click(object sender, EventArgs e)
        {
            _toolStripMenuInputSupport.Checked = !_toolStripMenuInputSupport.Checked;
            Reset();
        }

        SettingForm? _settingForm = null;
        private void _toolStripMenuItemSetting_Click(object sender, EventArgs e)
        {
            if (_settingForm != null) _settingForm.Focus();
            else
            {
                _settingForm = new SettingForm(_setting);
                _settingForm.Show();
                _settingForm.FormClosing += (s, e) => _settingForm = null;
            }
        }

        private void _toolStripMenuItemRestart_Click(object sender, EventArgs e)
        {

        }

        private void _toolStripMenuItemExist_Click(object sender, EventArgs e)
        {
            Save();
            this.Close();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Color color = Color.Red;
            var txtShow = _currentHiragana;
            var txtInput = _sentenceOnInput.Text;

            //if (Opacity == 0.0 && txtInput.Length > 1) return;

            bool isOperationInput = true;
            if (txtInput == _setting.KeywordExit)
            {
                txtShow = "[nime]終了";
            }
            else if (txtInput == _setting.KeywordStop)
            {
                txtShow = "[nime]停止";
            }
            else if (txtInput == _setting.KeywordStart) // 通ることないはずだけど念のため
            {
                txtShow = "[nime]再開";
            }
            else if (txtInput == _setting.KeywordVisible)
            {
                txtShow = "[nime]入力表示の無効化";
            }
            else if (txtInput == _setting.KeywordSupport)
            {
                txtShow = _toolStripMenuInputSupport.Checked ? "[nime]入力補完の無効化" : "[nime]入力補完の有効化";
            }
            else if (txtInput == _setting.KeywordSetting)
            {
                txtShow = "[nime]設定画面の表示";
            }
            else
            {
                color = Color.Black;
                isOperationInput = false;
            }
            if (Opacity == 0.0 && txtInput.Length > 0) return;

            FontFamily f = SystemFonts.DefaultFont.FontFamily;
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            if (!isOperationInput && _sentenceOnInput.Text.Length > _sentenceOnInput.CaretPosition) // 入力位置表示
            {
                Color colorCaret = Color.MediumPurple;
                var pre = _sentenceOnInput.Text.Substring(0, _sentenceOnInput.CaretPosition);
                var preHiragana = Utility.ConvertToHiragana(pre, false);

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

            if (txtInput.Length < 2) TopMost = true;
        }


        private void Form1_FormClosing(object sender, EventArgs e)
        {
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


}
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
         * Shift2�A������(�g���K�[�A�u�ϊ��v�L�[�Ȃǂɐݒ�\)�ŌĂяo��
         * �}�E�X�N���b�N�Ŗⓚ���p���Z�b�g(�z�C�[�����A���͂�}�E�X�����������ł����Z�b�g���ׂ�����)
         * 
         * ## �Ή��\��̋@�\
         *   ���̓i�r�A�����Ƃ������悭�A�Ђ炪�Ȃ̊Y���ӏ��ɃL�����b�g��`�悷��
         *   �ϊ������͋L�^���Ă����āA����I�����͗D��x�グ��
         *   �����@�\�B�I�����̍ŗD��ɒǉ��B�o����Ύ����l�����Ď�����,��}���������B 
         *    IME����o�͂����e�L�X�g�t�@�C�����C���|�[�g�B
         *   ���[�}����I��������ԂŃg���K�[�������ϊ����{
         *   �}�X�N���ꂽ�e�L�X�g�{�b�N�X�ł͕\�����Ȃ��悤�ɂ���
         *   �����L�[�{�[�h���쎞�AEsc�����ŋً}��~�ł���悤�ɂ���
         *   �A�N�e�B�u�ȃR���g���[�����{�^���ł���ꍇ���A���炩�Ƀe�L�X�g�ҏW���łȂ����Ƃ����m�ł���̂Ȃ�A���̎��͕ϊ��̋N�_�Ƃ��Ȃ�
         *   �啶��(Shift+�A���t�@�x�b�g)�œ��͂��ꂽ�����͋�؂�ʒu�Ƃ��悤�B�Ⴆ�΁AkokodeHakimonowoNugu -> kokode,hakimonowo,nugu (���̏ꍇ�A�c��̕����͕������Ȃ��A�̈Ӗ��ł͂Ȃ�������������Ƃ����_�ɒ���)
         *   �啶������n�܂�����A�̓��͕͂ϊ��ΏۊO�Ƃ���(�{�@�\��On/Off�\)
         *   �ϊ����Ɍ����[�}�����o���A�S�ĂЂ炪�Ȋm��A�S�ăJ�^�J�i�m��(�P�Ȃ�qq�̓��͂Ƃ����l�������Avim�Ƃ߂����ዣ�����邩���߂��ق���������)
         *   �ϊ��E�C���h�E��ł̒��ڕҏW(IMEMode)
         *   �A�v���P�[�V�������Ƃ�Ctrl�����̖����ݒ�(Ctrl+h,Ctrl+U���̑Ή��̂���)
         *   �p���L�[�{�[�h�A���{��L�[�{�[�h���l�������L��
         *   Vi�̓��̓��[�h���ƁAShift+<-�őI���ł��Ȃ��̂ŁA�ꕶ�������������Ȃ��B�A�v���P�[�V�������Ƃɏ�������ݒ�ł���悤�ɂ���B
         *   ��͂�A�⊮���Ȃ��ƍ��ǂ��s�ւɂ͊������Ȃ��B
         * 
         * ## �ۑ�
         *   �u�v�̈����Ƃ��A!�Ƃ�?�Ƃ��F�Ƃ�
         *   �S�p�X�y�[�X���ǂ����悤��(Shift+Space...?)
         *   ���������Ȃ�v�Z�@�\���ǉ������Ⴄ��
         *   WPF�R���g���[���̃L�����b�g�ʒu(UIAutomation)
         *   ����ŁA���ׂẴf�X�N�g�b�v�ŏo���悤�ɂ�����
         * 
         * ## ���m�̕s�
         *   �ϊ����ɓ��͂�����ƁA��낵���Ȃ��Ƃ���ɕ����񂪓��͂���Ă��܂��B
         *     DeviceOperator.InputText(ans.GetFirstSentence()); �̓��͂��I���܂łɓ��͂��ꂽ���̂́A��U�L�����Z�����ďI�������ɒx�����ăV�~�����[�g����B
         * 
         * ## �ݒ荀��
         *  ### ����
         *   �g���K�[�̐ݒ�A�ϊ��E�C���h�E�Ăяo���g���K�[�̐ݒ�
         *   ��؂蕶�����͎��̎����ϊ�(on/off, �L��������)
         *   �����̈���(���p��D��/�S�p��D��/�P�����Ȃ�S�p�A����ȊO�͔��p)
         *   �}�E�X�ړ�����臒l����
         *   �L�[�{�[�h���C�A�E�g(JIS/US ���V�X�e���������΂悩�����̂����A�ǂ��������@���킩��Ȃ������B)
         *   �����̏�����(Shift+���őI����DEL/BS�A���ׂ�BS�A���ׂ�DEL)(�A�v���P�[�V�������Ƃ̐ݒ�)
         *  ### ���̓i�r
         *   ���͒��̃i�r�\��ON/OFF�A�L�����b�g�ɑ΂��鑊�Έʒu�A�L�����b�g�ʒu������ł��Ȃ����̕\���ʒu(�A�v���P�[�V�������̐ݒ�)
         *   �J���[�X�L�[�}
         *  ### �ϊ��E�C���h�E
         *   �ϊ��E�C���h�E��Ŏg�p����L�[�������X�g(5�����ȏ�)�A�������̌�ɑ啶�����L�[�Ƃ��Ďg�p���邩
         *   ����ΏۂƂ������̕ϊ��E�C���h�E�̃��s�b�h�ϊ�ON/OFF
         *  ### ����
         *   ���������o�^���[�h(��Ɏ����o�^/�������ɃA���t�@�x�b�g���܂܂�Ă��Ȃ���Ύ����o�^/��Ɏ����o�^���Ȃ�)
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

                // UNDO�̗������o���邾���܂Ƃ߂����̂ŁA�I�����Ă������
                //Debug.WriteLine($"{_labelInput.Text}, pos:{pos} Not Shift");

                for (int i = pos; i < lengthAll; i++)
                {
                    DeviceOperator.KeyStroke(Nime.Device.VirtualKeys.Right);
                }

                bool bIsDeleteByAllBS = false; // TODO!:�������A�A�v���P�[�V�������Ƃ̐ݒ�ɂ��
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
            return ('A' <= c && c <= 'Z'); // 1�����ڂ��啶���̏ꍇ�͖���
        }

        private void ActionConvert()
        {
            if (IsIgnorePatternInput()) // 1�����ڂ��啶���̏ꍇ�͖���
            {
                Reset();
                return;
            }

            // �ϊ��̎��s
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
                notifyIcon1.ShowBalloonTip(5000, "[nime]�G���[", "�ϊ��Ɏ��s���܂����B", ToolTipIcon.Error);
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
                if (_labelInput.Text.Length > 4) // �����ϊ��̎��s("desu."�Ƃ�"masu."�������ŕϊ��������̂�4�����𐧌��Ƃ���)
                {
                    var txtHiragana = ConvertToHiragana(_labelInput.Text);
                    bool isNumber = txtHiragana.All(c => ('0' <= c && c <= '9') || c == '�A' || c == '�B');

                    if (!isNumber && txtHiragana.All(c => c < 'A' || 'Z' < c))
                    {
                        if (_labelInput.Text.Length < 10) // size�ȂǁA�Ђ炪�Ȃɕϊ��ł��Ă��p��̏ꍇ������(��������10���������Ă�������v���낤�c)
                        {
                            // �����Y������p�P�ꂪ����Ȃ玩���ϊ��͎~�߂Ă���
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

                                        if (!string.IsNullOrWhiteSpace(responseContent)) return; // �����Y������p�P�ꂪ����Ȃ玩���ϊ��͎~�߂Ă���
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

                    // �N�����ɃA�N�e�B�u�������n���h�����o���Ă����āA�������ɃA�N�e�B�u�ɂȂ�n���h�����قȂ�悤�Ȃ�ύX���L�����Z�����邱�ƁI
                    //�@�˂����Ȃ��ƁA�t�@�C�����ύX�̍ۂȂǂɂƂ�ł��Ȃ����ƂɂȂ�B
                    //�@�˂ނ���ǂ��ɂ��������̂����A������ƍ�͓���C������B�t�H�[�J�X���ʂ̃E�C���h�E�Ɏʂ������_�ŁA���Ă��܂��̂Łc�t�@�C�����ύX���ɂ͎g���Ȃ��̂��c
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

            // �A���t�@�x�b�g
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
                addText("�[");
            }
            // ����
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
            // TODO:�e�L���ɂ��ẮA�L�[�{�[�h�ɉ����Ĕ��f��������K�v������B

            // �폜
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

            // �ړ�
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
                // TODO:���̓��͂̊J�n�L�����b�g�ʒu���L�^���Ă����A���̈ʒu�ƈ�v����Ȃ�Reset���Ȃ��B
                Reset();
                return;
            }
            else if (e.Key == Nime.Device.VirtualKeys.End)
            {
                // TODO:���̓��͒��̍ł��E���̃L�����b�g�ʒu���L�^���Ă����A���̈ʒu�ƈ�v����Ȃ�Reset���Ȃ��B
                Reset();
                return;
            }

            // Shift+Esc�Ŗ��m�蕶���̍폜
            else if (e.Key == Nime.Device.VirtualKeys.Esc && (KeyboardWatcher.IsKeyLocked(Keys.RShiftKey) || KeyboardWatcher.IsKeyLocked(Keys.LShiftKey)))
            {
                DeleteCurrentText();
                return;
            }
            else if (e.Key == Nime.Device.VirtualKeys.Shift || e.Key == Nime.Device.VirtualKeys.ShiftLeft || e.Key == Nime.Device.VirtualKeys.ShiftRight)
            {
                return; // _lastAnswer�������Ȃ����߂�Reset������return����
            }
            else // �����Ƃ��Ă̓��Z�b�g���낤�c
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
            //UIAutomation.GetCaretPosition(); // WPF�Ή�
            if (p.Y == 0)
            {
                p = Caret.GetCaretPosition();
                p.Y = p.Y + 15; // TODO!:�{���̓L�����b�g�T�C�Y���擾�������B
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
            txt = txt.Replace("nn", "��");
            txt = txt.Replace("NN", "��N");
            txt = txt.Replace("nN", "��N");
            var txtHiragana = Microsoft.International.Converters.KanaConverter.RomajiToHiragana(txt);
            txtHiragana = txtHiragana.Replace(",", "�A");
            txtHiragana = txtHiragana.Replace(".", "�B");
            return txtHiragana;
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            TopMost = true;

            var klID = InputLanguage.CurrentInputLanguage.Culture.KeyboardLayoutId;
            notifyIcon1.ShowBalloonTip(2000, "�F�����ꂽ�L�[�{�[�h", InputLanguage.CurrentInputLanguage?.LayoutName?.ToString() + "\r\n�L�[�{�[�h���C�A�E�gID:" + klID.ToString(), ToolTipIcon.Info);
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
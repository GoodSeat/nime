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
         * Shift2�A������(�g���K�[�A�u�ϊ��v�L�[�Ȃǂɐݒ�\�Ƃ������I)�ŌĂяo��
         * �}�E�X�N���b�N�Ŗⓚ���p���Z�b�g(�z�C�[�����A���͂�}�E�X�����������ł����Z�b�g���ׂ�����)
         * 
         * �ϊ�����ɍēxShift����(���邢�͎��Ԃ��󂢂��ꍇ�ɂ͍ēxShift2�A������)�ŕϊ��E�C���h�E�Ăяo��
         * �ϊ��E�C���h�E�ł́A�e���߂̌�����x�ɕ\�����āA�e���Ɂuf�v�uj�v...�uab�v�c�Ȃǂ�U���ĕ\���Afirefox��vim�v���O�C�����Q�l��
         * �e�����Ԃ�a~b�A���������ꍇ��aa~zz��U��B���߂���؂�ɂ́As+�L��(���ߋ�؂�̃g�O��)�A���邢��m+�L��+�L��(���ߋ�؂�̈ړ�)�B
         * 
         * �ϊ������͋L�^���Ă����āA����I�����͗D��x�グ��
         * �����@�\�B�I�����̍ŗD��ɒǉ��B�o����Ύ����l�����Ď�����,��}���������B 
         *  IME����o�͂����e�L�X�g�t�@�C�����C���|�[�g�B
         * 
         * ���Z�b�g����ŕ���A���͍ς݂̕����������čēx���́A�A��Esc�����̓L�����Z��
         * 
         * 
         * �u�v�̈����Ƃ��A!�Ƃ�?�Ƃ��F�Ƃ�
         * �S�p�X�y�[�X���ǂ����悤��
         * �p���L�[�{�[�h�A���{��L�[�{�[�h
         * ���������Ȃ�v�Z�@�\���ǉ������Ⴄ��
         * 
         * �ϊ����ɓ��͂�����ƁA��낵���Ȃ��Ƃ���ɕ����񂪓��͂���Ă��܂��B
         *   DeviceOperator.InputText(ans.GetFirstSentence()); �̓��͂��I���܂łɓ��͂��ꂽ���̂́A��U�L�����Z�����ďI�������ɒx�����ăV�~�����[�g����B
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
                    // �ϊ��̎��s
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
                        notifyIcon1.ShowBalloonTip(5000, "[nime]�G���[", ex.Message, ToolTipIcon.Error);
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

            // �A���t�@�x�b�g
            else if (e.Key >= Nime.Device.VirtualKeys.A && e.Key <= Nime.Device.VirtualKeys.Z)
            {
                _labelInput.Text += e.Key;
                _currentPos++;
            }
            else if (e.Key == Nime.Device.VirtualKeys.Subtract || e.Key == Nime.Device.VirtualKeys.OEMMinus)
            {
                _labelInput.Text += "�[";
                _currentPos++;
            }
            // ����
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
                return; // _lastAnswer�������Ȃ����߂�Reset������returncI

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

            if (_labelInput.Text.Length == 1)
            {
                var p = Caret.GetCaretPosition();
                SetDesktopLocation(p.X, p.Y + 15); // TODO!:�{���̓L�����b�g�T�C�Y���擾�������B
                Opacity = 0.80;
            }
        }

        string ConvertToHiragana(string txt)
        {
            txt = txt.Replace("NN", "��");
            var txtHiragana = Microsoft.International.Converters.KanaConverter.RomajiToHiragana(txt);
            txtHiragana = txtHiragana.Replace(",", "�A");
            txtHiragana = txtHiragana.Replace(".", "�B");
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
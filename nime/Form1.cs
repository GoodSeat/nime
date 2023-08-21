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
         * Ctrl��������(�g���K�[�A�u�ϊ��v�L�[�Ȃǂɐݒ�\�Ƃ������I)�ŌĂяo��
         * �}�E�X�N���b�N�Ŗⓚ���p���Z�b�g(�z�C�[�����A���͂�}�E�X�����������ł����Z�b�g���ׂ�����)
         * 
         * �ϊ�����ɍēxCtrl���������ŕϊ��E�C���h�E�Ăяo��
         * �ϊ��E�C���h�E�ł́A�e���߂̌�����x�ɕ\�����āA�e���Ɂuf�v�uj�v...�uab�v�c�Ȃǂ�U���ĕ\���Afirefox��vim�v���O�C�����Q�l��
         * �e�����Ԃ�1~9�A01~99��U��B���߂���؂�ɂ́A������ł�(���ߋ�؂�̃g�O��)�B
         * 
         * �ϊ������͋L�^���Ă����āA����I�����͗D��x�グ��
         * �����@�\�B�I�����̍ŗD��ɒǉ��B�o����Ύ����l�����Ď�����,��}���������B 
         * 
         * ���Z�b�g����ŕ���A���͍ς݂̕����������čēx���́A�A��Esc�����̓L�����Z��
         * 
         * 
         * �u�v�̈����Ƃ��A!�Ƃ�?�Ƃ��F�Ƃ�
         * �p���L�[�{�[�h�A���{��L�[�{�[�h
         * 
         * �ϊ����ɓ��͂�����ƁA��낵���Ȃ��Ƃ���ɕ����񂪓��͂���Ă��܂��B
         *   DeviceOperator.InputText(ans.GetFirstSentence()); �̓��͂��I���܂łɓ��͂��ꂽ���̂́A��U�L�����Z�����ďI�������ɒx�����ăV�~�����[�g����B
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
                    // �ϊ��̎��s
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

            // �A���t�@�x�b�g
            else if (e.Key >= Nime.Device.VirtualKeys.A && e.Key <= Nime.Device.VirtualKeys.Z)
            {
                label1.Text += e.Key;
                _currentPos++;
            }
            else if (e.Key == Nime.Device.VirtualKeys.Subtract || e.Key == Nime.Device.VirtualKeys.OEMMinus)
            {
                label1.Text += "�[";
                _currentPos++;
            }
            // ����
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

            // �폜
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

            // �ړ�
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
            else // �����Ƃ��Ă̓��Z�b�g���낤�c
            {
                Reset();
            }

            label2.Text = ConvertToHiragana(label1.Text);
        }

        string ConvertToHiragana(string txt)
        {
            txt = txt.Replace("NN", "��");
            var txtHiragana = Microsoft.International.Converters.KanaConverter.RomajiToHiragana(txt);
            txtHiragana = txtHiragana.Replace(",", "�A");
            txtHiragana = txtHiragana.Replace(".", "�B");
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
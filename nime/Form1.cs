using Nime.Device;
using System.Diagnostics;
using System.Runtime.Serialization.Json;
using System.Text;

namespace nime
{
    public partial class Form1 : Form
    {

        /*
         * 
         * Ctrl��������(�g���K�[�A�u�ϊ��v�L�[�Ȃǂɐݒ�\�Ƃ������I)�ŌĂяo��
         * �}�E�X�N���b�N�Ŗⓚ���p���Z�b�g(�z�C�[�����A���͂�}�E�X�����������ł����Z�b�g���ׂ�����)
         * �J�[�\���L�[��ǂ��A�͈͂��O�ꂽ�ꍇ��㉺�������ɂ̓��Z�b�g
         * 
         * �ϊ�����ɍēxCtrl���������ŕϊ��E�C���h�E�Ăяo��
         * �ϊ��E�C���h�E�ł́A�e���߂̌�����x�ɕ\�����āA�e���Ɂua�v�ub�v...�u1a�v�c�Ȃǂ�U���ĕ\���Afirefox��vim�v���O�C�����Q�l��
         * ���߂���؂�ɂ́A�J�[�\���ňړ�����,���́H
         * 
         * �ēxCtrl���������ŕ���A���͍ς݂̕����������čēx����
         * 
         */

        public Form1()
        {
            InitializeComponent();
            label1.Text = "";

            KeyboardWatcher.KeyUp += KeyboardWatcher_KeyUp;

            KeyboardWatcher.Start();
        }



        string _currentString = "";


        private async void KeyboardWatcher_KeyUp(object? sender, KeyboardWatcher.KeybordWatcherEventArgs e)
        {
            if (IMEWatcher.IsOnIME())
            {
                label1.Text = "";
                return;
            }

            if (e.Key == Nime.Device.VirtualKeys.ControlLeft || e.Key == Nime.Device.VirtualKeys.ControlRight)
            {
                label1.Text = "";
            }
            // �A���t�@�x�b�g
            else if (e.Key >= Nime.Device.VirtualKeys.A && e.Key <= Nime.Device.VirtualKeys.Z)
            {
                label1.Text += e.Key;
            }
            // ����

            // �폜
            else if (e.Key == Nime.Device.VirtualKeys.BackSpace)
            {
            }
            else if (e.Key == Nime.Device.VirtualKeys.Del)
            {
            }

            // �ړ�
            else if (e.Key == Nime.Device.VirtualKeys.Right)
            {
            }
            else if (e.Key == Nime.Device.VirtualKeys.Left)
            {
            }

            // �ϊ��̎��s
            else if (e.Key == Nime.Device.VirtualKeys.F2)
            {
                var txt = label1.Text;
                if (string.IsNullOrEmpty(txt)) return;

                for (int i = 0; i < txt.Length; i++) {
                    DeviceOperator.KeyStroke(Nime.Device.VirtualKeys.BackSpace);
                }
                label1.Text = "";

                var txtHiragana = ConvertHiragana(txt);

                using (var client = new HttpClient())
                {
                    var txtReq = $"http://www.google.com/transliterate?langpair=ja-Hira|ja&text=" + txtHiragana;
                    Debug.WriteLine("post:" + txtReq);

                    var httpsResponse = await client.PostAsync(txtReq, null);
                    var responseContent = await httpsResponse.Content.ReadAsStringAsync();

                    if (responseContent != null)
                    {
                        Debug.WriteLine("return:" + responseContent?.ToString());
                        DeviceOperator.InputText(responseContent);
                    }

                    // Response Body�Ɋ܂܂��JSON���i�[����C���X�^���X�𐶐����܂��B
                    //var answer = new Answer();
                    using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(responseContent))) {
                        //var ser = new DataContractJsonSerializer(answer.GetType());
                        //answer = ser.ReadObject(ms) as Answer;
                    }    

                }

            }
        }

        string ConvertHiragana(string text)
        {
            List<Tuple<string, string>> pairs = new List<Tuple<string, string>>()
            {
                Tuple.Create("KA", "��"),
                Tuple.Create("KI", "��"),
                Tuple.Create("KU", "��"),
                Tuple.Create("KE", "��"),
                Tuple.Create("KO", "��"),
                Tuple.Create("A", "��"),
                Tuple.Create("I", "��"),
                Tuple.Create("U", "��"),
                Tuple.Create("E", "��"),
                Tuple.Create("O", "��"),
            };

            foreach (var pair in pairs)
            {
                text = text.Replace(pair.Item1, pair.Item2);
            }
            return text;
        }


    }
}
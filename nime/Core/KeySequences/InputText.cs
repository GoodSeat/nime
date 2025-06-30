using GoodSeat.Nime.Device;
using GoodSeat.Nime.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodSeat.Nime.Core.KeySequences
{
    public abstract class InputText
    {

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        public static InputText CreateByName(string name)
        {
            switch (name)
            {
                case nameof(InputTextBySendInput):
                    return new InputTextBySendInput();
                case nameof(InputTextBySendWait):
                    return new InputTextBySendWait();
                case nameof(InputTextByUsingClipboard):
                    return new InputTextByUsingClipboard();
            }
            return null;
        }

        public virtual void Operate(WindowInfo target, string input)
        {
            SetForegroundWindow(target.Handle); // TODO!:これがあると入力が安定する?
            OnOperate(input);
        }

        protected abstract void OnOperate(string input);

        public abstract string Title { get; }
        public virtual string Information { get => ""; }
    }

    public class InputTextBySendInput : InputText
    {
        DeviceOperator _deviceOperator = new DeviceOperator();

        public InputTextBySendInput(int? wait = 1)
        {
            Wait = wait;
        }

        public int? Wait { get; set; }

        protected override void OnOperate(string input)
        {
            if (Wait.HasValue)
            {
                foreach (var c in input)
                {
                    _deviceOperator.InputText(c.ToString());
                    Thread.Sleep(Wait.Value);
                }
            }
            else
            {
                _deviceOperator.InputText(input);
            }
        }
        public override string Title
        {
            get
            {
                return Wait.HasValue ? $"SendInput({Wait}msec待機)"
                    : $"SendInput(待機なし)";
            }
        }
    }

    public class InputTextBySendWait : InputText
    {
        protected override void OnOperate(string input)
        {
            SendKeys.SendWait(input);
        }
        public override string Title
        {
            get => "SendKeys.SendWait";
        }
    }

    public class InputTextByUsingClipboard : InputText
    {
        DeviceOperator _deviceOperator = new DeviceOperator();

        protected override void OnOperate(string input)
        {
            Clipboard.SetText(input);

            bool isLockedCtrlL = KeyboardWatcher.IsKeyLockedStatic(Keys.LControlKey);

            if (!isLockedCtrlL) _deviceOperator.KeyDown(VirtualKeys.ControlLeft);
            _deviceOperator.KeyStroke(VirtualKeys.V);
            if (!isLockedCtrlL) _deviceOperator.KeyUp(VirtualKeys.ControlLeft);

            //SendKeys.SendWait("{^v}");
        }
        public override string Title
        {
            get => "クリップボード経由";
        }
        public override string Information
        { 
            get => "入力文字列をクリップボードに転送し、Ctrl+Vを押下することで文字列を入力します。\r\n" +
                   "クリップボードの内容を上書してしまうため、基本的には非推奨ですが、Word等で文字の入力漏れが発生するアプリケーションで使用してください。";
        }
    }
}

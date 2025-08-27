using GoodSeat.Nime.Device;
using GoodSeat.Nime.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsInput;

namespace GoodSeat.Nime.Core.KeySequences
{
    public class Entry<T>
    {
        public string Title { get; set; }
        public T Value { get; set; }

        public override string ToString()
        {
            return Title;
        }
    }
        
    public abstract class InputText
    {

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        public static InputText CreateByName(string name)
        {
            switch (name)
            {
                case nameof(InputTextBySendInput): return new InputTextBySendInput();
                case nameof(InputTextBySendWait): return new InputTextBySendWait();
                case nameof(InputTextByUsingClipboard): return new InputTextByUsingClipboard();
                case nameof(InputTextByInputSimulator): return new InputTextByInputSimulator();
            }
            return null;
        }

        public static IEnumerable<Entry<InputText>> AllCandidates()
        {
            Func<string, Entry<InputText>> toEntry = s => new Entry<InputText> { Title = CreateByName(s).Title, Value = CreateByName(s) };

            yield return toEntry(nameof(InputTextBySendInput));
            yield return toEntry(nameof(InputTextBySendWait));
            yield return toEntry(nameof(InputTextByUsingClipboard));
            yield return toEntry(nameof(InputTextByInputSimulator));
        }


        public void Operate(WindowInfo target, string input)
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
                return Wait.HasValue
                    ? $"SendInputによる入力(1文字ごとに{Wait}msec待機)"
                    : $"SendInputによる入力";
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
            get => "SendKeys.SendWaitによる入力";
        }
    }

    public class InputTextByUsingClipboard : InputText
    {
        DeviceOperator _deviceOperator = new DeviceOperator();

        protected override void OnOperate(string input)
        {
            IDataObject originalClipboardData = null;
            try
            {
                // STAスレッドで実行する必要があるため、Try-Catchで囲む
                originalClipboardData = Clipboard.GetDataObject();
            }
            catch (Exception) { /* Wordなど一部アプリでは取得に失敗することがある */ }

            try
            {
                Clipboard.SetText(input);

                bool isLockedCtrlL = KeyboardWatcher.IsKeyLockedStatic(Keys.LControlKey);

                if (!isLockedCtrlL) _deviceOperator.KeyDown(VirtualKeys.ControlLeft);
                _deviceOperator.KeyStroke(VirtualKeys.V);
                if (!isLockedCtrlL) _deviceOperator.KeyUp(VirtualKeys.ControlLeft);

                //SendKeys.SendWait("{^v}");
            }
            finally
            {
                if (originalClipboardData != null)
                {
                    try
                    {
                        Clipboard.SetDataObject(originalClipboardData, true);
                    }
                    catch (Exception) {}
                }
            }
        }
        public override string Title
        {
            get => "クリップボード経由入力";
        }
        public override string Information
        { 
            get => "入力文字列をクリップボードに転送し、Ctrl+Vを押下することで文字列を入力します。\r\n" +
                   "クリップボードの内容を上書してしまうため、基本的には非推奨ですが、Word等で文字の入力漏れが発生するアプリケーションで使用してください。";
        }
    }

    public class InputTextByInputSimulator : InputText
    {
        public override string Title => "InputSimulator.TextEntryによる入力";

        InputSimulator _inputSimlator = new InputSimulator();

        protected override void OnOperate(string input)
        {
            _inputSimlator.Keyboard.TextEntry(input);
        }
    }
}

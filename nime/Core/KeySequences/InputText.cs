using GoodSeat.Nime.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodSeat.Nime.Core.KeySequences
{
    public abstract class InputText
    {
        public abstract void Operate(string input);

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

        public override void Operate(string input)
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
        public override void Operate(string input)
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

        public override void Operate(string input)
        {
            Clipboard.SetText(input);
            SendKeys.SendWait("{^v}");
        }
        public override string Title
        {
            get => "クリップボード経由";
        }
    }
}

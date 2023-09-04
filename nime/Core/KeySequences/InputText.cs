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
    }

    public class InputTextBySendInput : InputText
    {
        DeviceOperator _deviceOperator = new DeviceOperator();

        public override void Operate(string input)
        {
            _deviceOperator.InputText(input);
        }
    }

    public class InputTextBySendWait : InputText
    {
        public override void Operate(string input)
        {
            SendKeys.SendWait(input);
        }
    }

}

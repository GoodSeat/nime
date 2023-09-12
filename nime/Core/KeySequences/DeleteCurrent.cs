using GoodSeat.Nime.Device;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodSeat.Nime.Core.KeySequences
{
    internal abstract class DeleteCurrent
    {
        DeviceOperator _deviceOperator = new DeviceOperator();

        public void Operate(int deleteLength, int caretPos)
        {
            if (deleteLength == 0) return;

            if (KeyboardWatcher.IsKeyLockedStatic(Keys.LShiftKey)) _deviceOperator.KeyUp(VirtualKeys.ShiftLeft);
            if (KeyboardWatcher.IsKeyLockedStatic(Keys.RShiftKey)) _deviceOperator.KeyUp(VirtualKeys.ShiftRight);
            if (KeyboardWatcher.IsKeyLockedStatic(Keys.LControlKey)) _deviceOperator.KeyUp(VirtualKeys.ControlLeft);
            if (KeyboardWatcher.IsKeyLockedStatic(Keys.RControlKey)) _deviceOperator.KeyUp(VirtualKeys.ControlRight);

            Debug.WriteLine($"deteleCurrent.Operate:{deleteLength},{caretPos}");
            var keys = GetKeySequence(deleteLength, caretPos);
             _deviceOperator.SendKeyEvents(keys.ToArray());
     }

        protected abstract List<(VirtualKeys, KeyEventType)> GetKeySequence(int deleteLength, int caretPos);

    }

    internal class DeleteCurrentBySelectWithDelete : DeleteCurrent
    {
        protected override List<(VirtualKeys, KeyEventType)> GetKeySequence(int deleteLength, int caretPos)
        {
            var keys = new List<(VirtualKeys, KeyEventType)>();
            // UNDOの履歴を出来るだけまとめたいので、選択してから消す
            keys.AddRange(Utility.Duplicates((VirtualKeys.Right, KeyEventType.Stroke), deleteLength - caretPos));
            keys.Add((VirtualKeys.ShiftLeft, KeyEventType.Down));
            keys.AddRange(Utility.Duplicates((VirtualKeys.Left, KeyEventType.Stroke), deleteLength));
            keys.Add((VirtualKeys.ShiftLeft, KeyEventType.Up));
            //keys.Add((VirtualKeys.BackSpace, KeyEventType.Stroke)); // TMEMO:VsVimでは巧く動作しない
            keys.Add((VirtualKeys.Del, KeyEventType.Stroke));
            return keys;
        }
    }
    internal class DeleteCurrentByBackspace : DeleteCurrent
    {
        protected override List<(VirtualKeys, KeyEventType)> GetKeySequence(int deleteLength, int caretPos)
        {
            var keys = new List<(VirtualKeys, KeyEventType)>();
            keys.AddRange(Utility.Duplicates((VirtualKeys.Del, KeyEventType.Stroke), deleteLength - caretPos));
            keys.AddRange(Utility.Duplicates((VirtualKeys.BackSpace, KeyEventType.Stroke), caretPos));
            return keys;
        }
    }


}

using GoodSeat.Nime.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodSeat.Nime.Core.KeyboardLayouts
{
    internal class KeyboardLayoutUS : KeyboardLayout
    {
        public override string? JudgeInputText(VirtualKeys key, KeyboardWatcher watcher)
        {
            // アルファベット
            if (key >= VirtualKeys.A && key <= VirtualKeys.Z)
            {
                if (watcher.IsKeyLocked(Keys.LShiftKey) || watcher.IsKeyLocked(Keys.RShiftKey))
                {
                    return key.ToString().ToUpper();
                }
                else
                {
                    return key.ToString().ToLower();
                }
            }
            else if (key == VirtualKeys.Subtract || key == VirtualKeys.OEMMinus)
            {
                return "ー";
            }
            // 数字
            else if ((key >= VirtualKeys.D0 && key <= VirtualKeys.D9) ||
                     (key >= VirtualKeys.N0 && key <= VirtualKeys.N9))
            {
                return key.ToString()[1].ToString();
            }
            else if (key == VirtualKeys.OEMPeriod)
            {
                return ".";
            }
            else if (key == VirtualKeys.OEMCommma)
            {
                return ",";
            }

            return null;
        }
    }
}

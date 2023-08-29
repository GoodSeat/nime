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
        public override string? JudgeInputText(VirtualKeys key)
        {
            // アルファベット
            if (key >= VirtualKeys.A && key <= VirtualKeys.Z)
            {
                return Utility.IsLockedShiftKey() ? key.ToString().ToUpper() : key.ToString().ToLower();
            }
            // 数字
            else if ((key >= VirtualKeys.D0 && key <= VirtualKeys.D9) ||
                     (key >= VirtualKeys.N0 && key <= VirtualKeys.N9))
            {
                if (Utility.IsLockedShiftKey())
                {
                    switch (key)
                    {
                        case VirtualKeys.D1: return "!";
                        case VirtualKeys.D2: return "@";
                        case VirtualKeys.D3: return "#";
                        case VirtualKeys.D4: return "$";
                        case VirtualKeys.D5: return "%";
                        case VirtualKeys.D6: return "^";
                        case VirtualKeys.D7: return "&";
                        case VirtualKeys.D8: return "*";
                        case VirtualKeys.D9: return "(";
                        case VirtualKeys.D0: return ")";
                    }
                }
                else
                {
                    return key.ToString()[1].ToString();
                }
            }
            // -
            else if (key == VirtualKeys.Subtract || key == VirtualKeys.OEMMinus)
            {
                return Utility.IsLockedShiftKey() ? "_" : "ー";
            }
            // +
            else if (key == VirtualKeys.Add || key == VirtualKeys.OEMPlus)
            {
                return Utility.IsLockedShiftKey() ? "+" : "=";
            }
            // ;:
            else if (key == VirtualKeys.OEM1)
            {
                return Utility.IsLockedShiftKey() ? ":" : ";";
            }
            // /?
            else if (key == VirtualKeys.OEM2)
            {
                return Utility.IsLockedShiftKey() ? "?" : "/";
            }
            // `~
            else if (key == VirtualKeys.OEM3)
            {
                return Utility.IsLockedShiftKey() ? "~" : "`";
            }
            // [{
            else if (key == VirtualKeys.OEM4)
            {
                return Utility.IsLockedShiftKey() ? "{" : "[";
            }
            // \|
            else if (key == VirtualKeys.OEM5)
            {
                return Utility.IsLockedShiftKey() ? "|" : "\\";
            }
            // ]}
            else if (key == VirtualKeys.OEM6)
            {
                return Utility.IsLockedShiftKey() ? "}" : "]";
            }
            // '"
            else if (key == VirtualKeys.OEM7)
            {
                return Utility.IsLockedShiftKey() ? "\"" : "'";
            }

            else if (key == VirtualKeys.OEMCommma)
            {
                return Utility.IsLockedShiftKey() ? "<" : ",";
            }
            else if (key == VirtualKeys.OEMPeriod)
            {
                return Utility.IsLockedShiftKey() ? ">" : ".";
            }

            return null;
        }
    }
}

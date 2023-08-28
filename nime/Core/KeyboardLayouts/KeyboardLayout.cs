using GoodSeat.Nime.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodSeat.Nime.Core.KeyboardLayouts
{
    public abstract class KeyboardLayout
    {


        public abstract string? JudgeInputText(VirtualKeys key, KeyboardWatcher watcher);


    }
}

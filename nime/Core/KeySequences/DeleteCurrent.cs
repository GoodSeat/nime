using GoodSeat.Nime.Device;
using GoodSeat.Nime.Windows;
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

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        public static DeleteCurrent CreateByName(string name)
        {
            switch (name)
            {
                case nameof(DeleteCurrentBySelectWithDelete):
                    return new DeleteCurrentBySelectWithDelete();
                case nameof(DeleteCurrentBySelectWithBackspace):
                    return new DeleteCurrentBySelectWithBackspace();
                case nameof(DeleteCurrentBySelectWithDeleteExpectLast):
                    return new DeleteCurrentBySelectWithDeleteExpectLast();
                case nameof(DeleteCurrentByDeleteAndBackspace):
                    return new DeleteCurrentByDeleteAndBackspace();
            }
            return null;
        }

        public DeleteCurrent(int? wait = 1)
        {
            Wait = wait;
        }

        DeviceOperator _deviceOperator = new DeviceOperator();

        public int? Wait { get; set; }

        public void Operate(WindowInfo target, int deleteLength, int caretPos)
        {
            if (deleteLength == 0) return;
            SetForegroundWindow(target.Handle); // TODO!:これがあると操作が安定する?

            if (KeyboardWatcher.IsKeyLockedStatic(Keys.LShiftKey)) _deviceOperator.KeyUp(VirtualKeys.ShiftLeft);
            if (KeyboardWatcher.IsKeyLockedStatic(Keys.RShiftKey)) _deviceOperator.KeyUp(VirtualKeys.ShiftRight);
            if (KeyboardWatcher.IsKeyLockedStatic(Keys.LControlKey)) _deviceOperator.KeyUp(VirtualKeys.ControlLeft);
            if (KeyboardWatcher.IsKeyLockedStatic(Keys.RControlKey)) _deviceOperator.KeyUp(VirtualKeys.ControlRight);

            Debug.WriteLine($"deteleCurrent.Operate:{deleteLength},{caretPos}");
            var keys = GetKeySequence(deleteLength, caretPos);

            if (Wait.HasValue)
            {
                foreach (var key in keys)
                {
                    _deviceOperator.SendKeyEvents(key);
                    Thread.Sleep(Wait.Value);
                }
            }
            else
            {
                _deviceOperator.SendKeyEvents(keys.ToArray());
            }
        }

        protected abstract List<(VirtualKeys, KeyEventType)> GetKeySequence(int deleteLength, int caretPos);

        public abstract string Title { get; }
        public virtual string Information { get => ""; }

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

            // 既存文字列を消すのに失敗して、"n日本語ihongo"みたいな結果になってしまうことがあるため、消えるのを少し待つ
            //Application.DoEvents();
            //Thread.Sleep(5);

            //keys.Add((VirtualKeys.BackSpace, KeyEventType.Stroke)); // TMEMO:VsVimでは巧く動作しない
            keys.Add((VirtualKeys.Del, KeyEventType.Stroke));

            //Thread.Sleep(5);

            return keys;
        }
        public override string Title { get => "Shift+←による選択後にDEL"; }
    }
    internal class DeleteCurrentBySelectWithBackspace : DeleteCurrent
    {
        protected override List<(VirtualKeys, KeyEventType)> GetKeySequence(int deleteLength, int caretPos)
        {
            var keys = new List<(VirtualKeys, KeyEventType)>();
            // UNDOの履歴を出来るだけまとめたいので、選択してから消す
            keys.AddRange(Utility.Duplicates((VirtualKeys.Right, KeyEventType.Stroke), deleteLength - caretPos));
            keys.Add((VirtualKeys.ShiftLeft, KeyEventType.Down));
            keys.AddRange(Utility.Duplicates((VirtualKeys.Left, KeyEventType.Stroke), deleteLength));
            keys.Add((VirtualKeys.ShiftLeft, KeyEventType.Up));

            keys.Add((VirtualKeys.BackSpace, KeyEventType.Stroke)); // TMEMO:VsVimでは巧く動作しない

            return keys;
        }
        public override string Title { get => "Shift+←による選択後にBS"; }
    }
    internal class DeleteCurrentBySelectWithDeleteExpectLast : DeleteCurrent
    {
        protected override List<(VirtualKeys, KeyEventType)> GetKeySequence(int deleteLength, int caretPos)
        {
            var keys = new List<(VirtualKeys, KeyEventType)>();

            if (deleteLength > 1)
            {
                // UNDOの履歴を出来るだけまとめたいので、選択してから消す
                keys.AddRange(Utility.Duplicates((VirtualKeys.Right, KeyEventType.Stroke), deleteLength - caretPos));
                keys.Add((VirtualKeys.ShiftLeft, KeyEventType.Down));
                keys.AddRange(Utility.Duplicates((VirtualKeys.Left, KeyEventType.Stroke), deleteLength - 1));
                keys.Add((VirtualKeys.ShiftLeft, KeyEventType.Up));

                keys.Add((VirtualKeys.Del, KeyEventType.Stroke));
            }
            keys.Add((VirtualKeys.BackSpace, KeyEventType.Stroke));

            return keys;
        }
        public override string Title { get => "Shift+←による選択後にDEL(最後の一文字のみBS)"; }
    }

    internal class DeleteCurrentByDeleteAndBackspace : DeleteCurrent
    {
        protected override List<(VirtualKeys, KeyEventType)> GetKeySequence(int deleteLength, int caretPos)
        {
            var keys = new List<(VirtualKeys, KeyEventType)>();
            keys.AddRange(Utility.Duplicates((VirtualKeys.Del, KeyEventType.Stroke), deleteLength - caretPos));
            keys.AddRange(Utility.Duplicates((VirtualKeys.BackSpace, KeyEventType.Stroke), caretPos));
            return keys;
        }
        public override string Title { get => "DEL及びBS"; }
    }


}

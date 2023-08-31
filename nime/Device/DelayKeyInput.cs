using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodSeat.Nime.Device
{
    internal class DelayKeyInput : IDisposable
    {

        public DelayKeyInput(params KeyboardWatcher[] keyboardWatchers) 
        {
            KeyboardWatchers = keyboardWatchers.ToList();

            Debug.WriteLine($"#### Delay start");
            KeyboardWatchers.ForEach(kw => kw.Enable = false);

            KeyboardWatcher = new KeyboardWatcher();
            KeyboardWatcher.KeyDown += WhenKeyDown;
            KeyboardWatcher.KeyUp += WhenKeyUp;
            KeyboardWatcher.Enable = true;
        }

        List<(VirtualKeys, KeyEventType)> DelayTargetKeys = new List<(VirtualKeys, KeyEventType)>();
        KeyboardWatcher KeyboardWatcher { get; set; }
        List<KeyboardWatcher> KeyboardWatchers { get; set; }

        private void WhenKeyDown(object? sender, KeyboardWatcher.KeybordWatcherEventArgs e)
        {
            if (e.Key == VirtualKeys.Packet) return;

            Debug.WriteLine($"## IGNORE Keydown {e.Key}");
            DelayTargetKeys.Add((e.Key, KeyEventType.Down));
            e.Cancel = true;
        }

        private void WhenKeyUp(object? sender, KeyboardWatcher.KeybordWatcherEventArgs e)
        {
            if (e.Key == VirtualKeys.Packet) return;

            Debug.WriteLine($"## IGNORE Keyup {e.Key}");
            DelayTargetKeys.Add((e.Key, KeyEventType.Up));
            e.Cancel = true;
        }


        public void Dispose()
        {
            KeyboardWatcher.KeyDown -= WhenKeyDown;
            KeyboardWatcher.KeyUp -= WhenKeyUp;
            KeyboardWatcher.Dispose();

            KeyboardWatchers.ForEach(kw => kw.Enable = true);

            // キャンセルしていたキーイベントを再現
            try
            {
                DeviceOperator.EnableWatchKeyboard = true;

                Debug.WriteLine($"## Restore ignored key events... ->");
                DeviceOperator.SendKeyEvents(DelayTargetKeys.ToArray());
                Debug.WriteLine($"## -> end.");
            }
            finally
            {
                DeviceOperator.EnableWatchKeyboard = false;
            }
        }
    }
}

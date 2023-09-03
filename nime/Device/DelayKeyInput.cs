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

            KeyboardWatcher = new KeyboardWatcher(true);
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

            if (NowRestoring)
            {
                if (DelayTargetKeys.Count == 0) return;
                if (DelayTargetKeys[0].Item1 == e.Key && DelayTargetKeys[0].Item2 == KeyEventType.Down)
                {
                    DelayTargetKeys.RemoveAt(0);
                    Debug.WriteLine($"## => Keydown {e.Key}");
                    return;
                }
            }

            Debug.WriteLine($"## IGNORE Keydown {e.Key}");
            DelayTargetKeys.Add((e.Key, KeyEventType.Down));
            e.Cancel = true;
        }

        private void WhenKeyUp(object? sender, KeyboardWatcher.KeybordWatcherEventArgs e)
        {
            if (e.Key == VirtualKeys.Packet) return;

            if (NowRestoring)
            {
                if (DelayTargetKeys.Count == 0) return;
                if (DelayTargetKeys[0].Item1 == e.Key && DelayTargetKeys[0].Item2 == KeyEventType.Up)
                {
                    DelayTargetKeys.RemoveAt(0);
                    Debug.WriteLine($"## => Keyup {e.Key}");
                    return;
                }
            }

            Debug.WriteLine($"## IGNORE Keyup {e.Key}");
            DelayTargetKeys.Add((e.Key, KeyEventType.Up));
            e.Cancel = true;
        }

        bool NowRestoring { get; set; } = false;

        public void Dispose()
        {
            KeyboardWatchers.ForEach(kw => kw.Enable = true);

            // キャンセルしていたキーイベントを再現
            NowRestoring = true;
            Debug.WriteLine($"## Restore ignored key events... ->");
            var deviceOperator = new DeviceOperator();
            deviceOperator.EnableWatchKeyboard = true;

            while (DelayTargetKeys.Count != 0)
            {
                deviceOperator.SendKeyEvents(DelayTargetKeys[0]);
            }
            Debug.WriteLine($"## -> end.");

            KeyboardWatcher.KeyDown -= WhenKeyDown;
            KeyboardWatcher.KeyUp -= WhenKeyUp;

            KeyboardWatcher.Dispose();
        }
    }
}

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

        List<Tuple<VirtualKeys, bool>> DelayTargetKeys = new List<Tuple<VirtualKeys, bool>>();
        KeyboardWatcher KeyboardWatcher { get; set; }
        List<KeyboardWatcher> KeyboardWatchers { get; set; }

        private void WhenKeyDown(object? sender, KeyboardWatcher.KeybordWatcherEventArgs e)
        {
            if (e.Key == VirtualKeys.Packet) return;

            Debug.WriteLine($"## IGNORE Keydown {e.Key}");
            DelayTargetKeys.Add(Tuple.Create(e.Key, false));
            e.Cancel = true;
        }

        private void WhenKeyUp(object? sender, KeyboardWatcher.KeybordWatcherEventArgs e)
        {
            if (e.Key == VirtualKeys.Packet) return;

            Debug.WriteLine($"## IGNORE Keyup {e.Key}");
            DelayTargetKeys.Add(Tuple.Create(e.Key, true));
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
                foreach (var (k, ud) in DelayTargetKeys)
                {
                    if (!ud) DeviceOperator.KeyDown(k);
                    else     DeviceOperator.KeyUp(k);

                    if (!ud) Debug.WriteLine($"## delay Keydown {k}");
                    else     Debug.WriteLine($"## delay Keyup {k}");
                }
            }
            finally
            {
                DeviceOperator.EnableWatchKeyboard = false;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Windows.Forms;

namespace GoodSeat.Nime.Device
{
	/// <summary>
	/// キーボードフック時のデータを表します。
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct KeyboardHookData
	{
		public int vkCode;
		public int scanCode;
		public int flags;
		public int time;
		public IntPtr dwExtraInfo;
	}

    /// <summary>
    /// キーボードフックを監視するクラスです。
    /// </summary>
    public class KeyboardWatcher : IDisposable
	{
		#region P/Invoke
		
		/// <summary> フックプロシージャのためのデリゲートです。 </summary>
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, ref KeyboardHookData lParam);

		/// <summary> 指定した名前のモジュールハンドルを取得します。 </summary>
		[DllImport("kernel32.dll")]
		public static extern IntPtr GetModuleHandle(string lpModuleName);

		/// <summary> フックプロシージャ"lpfn"をフックチェーン内にインストールします。 </summary>
		[DllImport("user32.dll")]
		public static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, int dwThreadId);

		/// <summary>次のフックプロシージャにフック情報を渡します。</summary>
		[DllImport("user32.dll")]
		public static extern IntPtr CallNextHookEx(IntPtr hHook, int nCode, IntPtr wParam, ref KeyboardHookData lParam);

		/// <summary>インポートされたフックプロシージャを削除します。 </summary>
		[DllImport("user32.dll")]
		public static extern bool UnhookWindowsHookEx(IntPtr hHook);

		[DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
		public static extern short GetKeyState(int nVirtKey);

		public const int WH_KEYBOARD_LL = 13;
		public const int HC_ACTION = 0;
		public const int WM_KEYDOWN = 0x0100;
		public const int WM_KEYUP = 0x0101;
		public const int WM_SYSKEYDOWN = 0x0104;
		public const int WM_SYSKEYUP = 0x0105;

		#endregion

		/// <summary>
		/// キーボードフック時のイベントデータを格納する引数を表します。
		/// </summary>
		public sealed class KeybordWatcherEventArgs : EventArgs
		{
			bool _cancel;
			int _nativeWParam;
			KeyboardHookData _nativeLParam;
			bool _isValueUpdate;

			int _keyCode;
			int _scanCode;
			int _flags;
			int _time;
			
			internal KeybordWatcherEventArgs() {	}

			/// <summary>
			/// イベントデータの初期化
			/// </summary>
			/// <param name="wParam"></param>
			/// <param name="lParam"></param>
			internal void Initialize(int wParam, KeyboardHookData lParam)
			{
				this._cancel = false;
				this._isValueUpdate = false;
				this._nativeWParam = wParam;
				this._nativeLParam = lParam;

				this._keyCode = lParam.vkCode;
				this._scanCode = lParam.scanCode;
				this._flags = lParam.flags;
				this._time = lParam.time;
			}

			public int KeyCode { get { return this._keyCode; } }
			public VirtualKeys Key { get { return (VirtualKeys)_keyCode; } internal set { _keyCode = (int)value; } }
			public int ScanCode { get { return this._scanCode; } }
			public int Flags { get { return this._flags; } }
			public int Time { get { return this._time; } }

			/// <summary>
			/// マウスイベントのキャンセル有無を設定もしくは取得します。キャンセルされる場合、次のフックプロシージャにフックイベントが渡されません。
			/// </summary>
			public bool Cancel
			{
				set { this._cancel = value; }
				get { return this._cancel; }
			}

			/// <summary>
			/// フックイベントのデータを表すwParam引数を取得もしくは設定します。
			/// </summary>
			public int NativeWParam
			{
				set
				{
					this._nativeWParam = value;
					this._isValueUpdate = true;
				}
				get { return this._nativeWParam; }
			}

			/// <summary>
			/// フックイベントのデータを表すlParam引数を取得もしくは設定します。
			/// </summary>
			public KeyboardHookData NativeLParam
			{
				set
				{
					this._nativeLParam = value;
					this._isValueUpdate = true;
				}
				get { return this._nativeLParam; }
			}

			/// <summary>
			/// イベントがユーザーによって変更されたか否かを取得します。
			/// </summary>
			internal bool IsValueUpdate
			{ get { return this._isValueUpdate; } }

			/// <summary>
			/// 指定されたイベントデータに情報をコピーします。
			/// </summary>
			/// <param name="e"></param>
			public void Copy(ref KeybordWatcherEventArgs e)
			{
				if (e == null) e = new KeybordWatcherEventArgs();

				e._cancel = _cancel;
				e._nativeWParam = _nativeWParam;
				e._nativeLParam = _nativeLParam;
				e._isValueUpdate = _isValueUpdate;

				e._keyCode = this._keyCode;
				e._scanCode = this._scanCode;
				e._flags = this._flags;
				e._time = this._time;
			}
		}


		/// <summary>
		/// キーボードのイベント監視クラスを初期化します。
		/// </summary>
		public KeyboardWatcher(bool insertFirst = false)
		{
			if (insertFirst) s_activeWatcher.Insert(0, this);
            else s_activeWatcher.Add(this);
		}

        public void Dispose()
        {
			Enable = false;

            s_activeWatcher.Remove(this);
			if (s_activeWatcher.Count == 0) Exit();
        }

		/// <summary>
		/// キーボードの監視を行うか否かを設定もしくは取得します。
		/// </summary>
		public bool Enable
        {
			get => _enable;
			set
			{
				_enable = value;
				if (value && !NowWatching) Start();
			}
        }
		bool _enable = false;

		/// <summary>
		/// 指定キーが押下状態にあるか否かを判定します。
		/// </summary>
		/// <param name="Key_Value">判定対象の<see cref="Keys"/>。</param>
		/// <returns>押下されているか否か。</returns>
		public bool IsKeyLocked(Keys Key_Value) { return IsKeyLockedStatic(Key_Value); }

        /// <summary>何れかのキーボードが押されたときに呼び出されます。 </summary>
        public event EventHandler<KeybordWatcherEventArgs> SysKeyDown;

		/// <summary>何れかのキーボードが押されたときに呼び出されます。 </summary>
		public event EventHandler<KeybordWatcherEventArgs> KeyDown;

		/// <summary>何れかのキーボードが放されたときに呼び出されます。 </summary>
		public event EventHandler<KeybordWatcherEventArgs> SysKeyUp;

		/// <summary>何れかのキーボードが放されたときに呼び出されます。 </summary>
		public event EventHandler<KeybordWatcherEventArgs> KeyUp;


        private void NotifyKeyboardWatcher_KeyDown(object? sender, KeybordWatcherEventArgs e)
        {
			if (!Enable) return;
			KeyDown?.Invoke(this, e);
        }

        private void NotifyKeyboardWatcher_SysKeyDown(object? sender, KeybordWatcherEventArgs e)
        {
			if (!Enable) return;
			SysKeyDown?.Invoke(this, e);
        }

        private void NotifyKeyboardWatcher_KeyUp(object? sender, KeybordWatcherEventArgs e)
        {
			if (!Enable) return;
			KeyUp?.Invoke(this, e);
        }

        private void NotifyKeyboardWatcher_SysKeyUp(object? sender, KeybordWatcherEventArgs e)
        {
			if (!Enable) return;
			SysKeyUp?.Invoke(this, e);
        }



		static List<KeyboardWatcher> s_activeWatcher = new List<KeyboardWatcher>();


		static IntPtr s_hook;
		static LowLevelKeyboardProc s_proc;
		static bool s_enable = true;

		static void NotifySysKeyDown(object? sender, KeybordWatcherEventArgs e)
		{
			s_activeWatcher.ToList().ForEach(w => { if (!e.Cancel) { w.NotifyKeyboardWatcher_SysKeyDown(sender, e); } });
		}

		static void NotifyKeyDown(object? sender, KeybordWatcherEventArgs e)
		{
			s_activeWatcher.ToList().ForEach(w => { if (!e.Cancel) { w.NotifyKeyboardWatcher_KeyDown(sender, e); } });
		}

		static void NotifySysKeyUp(object? sender, KeybordWatcherEventArgs e)
		{
			s_activeWatcher.ToList().ForEach(w => { if (!e.Cancel) { w.NotifyKeyboardWatcher_SysKeyUp(sender, e); } });
		}

		static void NotifyKeyUp(object? sender, KeybordWatcherEventArgs e)
		{
			s_activeWatcher.ToList().ForEach(w => { if (!e.Cancel) { w.NotifyKeyboardWatcher_KeyUp(sender, e); } });
		}
			

		/// <summary>
		/// 静的コンストラクタ
		/// </summary>
		static KeyboardWatcher()
		{
			AppDomain.CurrentDomain.DomainUnload += delegate
			{
				if (s_hook != IntPtr.Zero) UnhookWindowsHookEx(s_hook);
			};
		}

		/// <summary>
		/// 現在キーボードを監視しているか否かを取得します。
		/// </summary>
		public static bool NowWatching { get { return s_hook != IntPtr.Zero; } }

		/// <summary>
		/// キーボードの監視を開始します。
		/// </summary>
		static void Start()
		{
			if (NowWatching) return;

			using (Process process = Process.GetCurrentProcess())
			using (ProcessModule module = process.MainModule)
			{
				s_hook = SetWindowsHookEx(WH_KEYBOARD_LL, s_proc = new LowLevelKeyboardProc(HookProc), GetModuleHandle(module.ModuleName), 0);
			}

			if (s_hook == IntPtr.Zero) throw new Exception("SetWindowsHookEx に失敗しました。");
		}

		/// <summary>
		/// キーボードの監視を終了します。
		/// </summary> 
		static void Exit()
		{
			if (!NowWatching) return;

            if (UnhookWindowsHookEx(s_hook) == false) throw new Exception("UnhookWindowsHookEx に失敗しました。");
		}

		/// <summary>
		/// キーボード監視時、イベント通知を行うかを設定もしくは取得します。
		/// </summary>
		static bool EnableStatic
		{
			get { return s_enable; }
			set { s_enable = value; }
		}

		/// <summary>
		/// キーボードフックの通知
		/// </summary>
		/// <returns></returns>
		static IntPtr HookProc(int nCode, IntPtr wParam, ref KeyboardHookData lParam)
		{
			if (!EnableStatic) return CallNextHookEx(s_hook, nCode, wParam, ref lParam);

			bool cancel = false;
			if (nCode == HC_ACTION)
			{
				if (lParam.dwExtraInfo == DeviceOperator.IGNORE_WATCHER) return CallNextHookEx(s_hook, nCode, wParam, ref lParam);

				var eventArgs = new KeybordWatcherEventArgs();
				eventArgs.Initialize((int)wParam, lParam);

                switch (wParam.ToInt32())
                {
                    case WM_KEYDOWN:
                        NotifyKeyDown(null, eventArgs);
                        break;
                    case WM_KEYUP:
                        NotifyKeyUp(null, eventArgs);
                        break;
                    case WM_SYSKEYDOWN:
                        NotifySysKeyDown(null, eventArgs);
                        break;
                    case WM_SYSKEYUP:
                        NotifySysKeyUp(null, eventArgs);
                        break;
                }

				// イベントの通知先でメッセージが編集された
				if (eventArgs.IsValueUpdate || eventArgs.Cancel)
				{
					wParam = (IntPtr)eventArgs.NativeWParam;
					lParam = eventArgs.NativeLParam;
				}

				cancel = eventArgs.Cancel;
			}

			return cancel ? (IntPtr)1 : CallNextHookEx(s_hook, nCode, wParam, ref lParam);
		}

		/// <summary>
		/// 指定キーが押下状態にあるか否かを判定します。
		/// </summary>
		/// <param name="Key_Value">判定対象の<see cref="Keys"/>。</param>
		/// <returns>押下されているか否か。</returns>
		/// <remarks>参考:https://detail.chiebukuro.yahoo.co.jp/qa/question_detail/q12189853917</remarks>
        public static bool IsKeyLockedStatic(Keys Key_Value)
        {
            bool Key_State = (GetKeyState((int)Key_Value) & 0x80) != 0;
            return Key_State;
        }

    }
}

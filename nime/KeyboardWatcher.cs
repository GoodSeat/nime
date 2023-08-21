using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Windows.Forms;

namespace Nime.Device
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
	/// キーボードフックを監視する静的クラスです。
	/// </summary>
	public static class KeyboardWatcher
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

		static IntPtr s_hook;
		static KeybordWatcherEventArgs s_eventArgs;
		static LowLevelKeyboardProc s_proc;
		static bool s_enable = true;

		static List<VirtualKeys> s_ignoreUpList = new List<VirtualKeys>();
		static List<VirtualKeys> s_ignoreDownList = new List<VirtualKeys>();
		
		/// <summary>何れかのキーボードが押されたときに呼び出されます。 </summary>
		public static event EventHandler<KeybordWatcherEventArgs> SysKeyDown;

		/// <summary>何れかのキーボードが押されたときに呼び出されます。 </summary>
		public static event EventHandler<KeybordWatcherEventArgs> KeyDown;

		/// <summary>何れかのキーボードが放されたときに呼び出されます。 </summary>
		public static event EventHandler<KeybordWatcherEventArgs> SysKeyUp;

		/// <summary>何れかのキーボードが放されたときに呼び出されます。 </summary>
		public static event EventHandler<KeybordWatcherEventArgs> KeyUp;

		/// <summary>
		/// 静的コンストラクタ
		/// </summary>
		static KeyboardWatcher()
		{
			s_eventArgs = new KeybordWatcherEventArgs();
			AppDomain.CurrentDomain.DomainUnload += delegate
			{
				if (s_hook != IntPtr.Zero)
					UnhookWindowsHookEx(s_hook);
			};

			s_downedKeys = new List<VirtualKeys>();

			for (int k = 0xF0; k <= 0xF6; k++) PhysicalTargetKeys.Add((VirtualKeys)k);
			for (int k = 0x15; k <= 0x19; k++) { if (k != 22) PhysicalTargetKeys.Add((VirtualKeys)k); }
			PhysicalTargetKeys.Add(VirtualKeys.Cancel);
		}

		/// <summary>
		/// 現在キーボードを監視しているか否かを取得します。
		/// </summary>
		public static bool NowWatching { get { return s_hook != IntPtr.Zero; } }

		/// <summary>
		/// キーボードの監視を開始します。
		/// </summary>
		public static void Start()
		{
			using (Process process = Process.GetCurrentProcess())
			using (ProcessModule module = process.MainModule)
			{
				s_hook = SetWindowsHookEx(WH_KEYBOARD_LL,
														   s_proc = new LowLevelKeyboardProc(HookProc),
														   GetModuleHandle(module.ModuleName),
														   0);
			}

			if (s_hook == IntPtr.Zero)
				throw new Exception("SetWindowsHookEx に失敗しました。");
		}

		/// <summary>
		/// キーボードの監視を終了します。
		/// </summary> 
		public static void Exit()
		{
			if (s_hook != IntPtr.Zero)
			{
				if (UnhookWindowsHookEx(s_hook) == false)
					throw new Exception("UnhookWindowsHookEx に失敗しました。");
			}
		}

		/// <summary>
		/// キーボード監視時、イベント通知を行うかを設定もしくは取得します。
		/// </summary>
		public static bool Enable
		{
			get { return s_enable; }
			set { s_enable = value; }
		}

		/// <summary>
		/// キー押上の際にイベントを発行しないキーを追加します。登録回数のみイベントの発行がキャンセルされます。
		/// </summary>
		/// <param name="key"></param>
		public static void AddIgnoreUpKey(VirtualKeys key)
		{
			s_ignoreUpList.Add(key);
		}

		/// <summary>
		/// キー押下の際にイベントを発行しないキーを追加します。登録回数のみイベントの発行がキャンセルされます。
		/// </summary>
		/// <param name="key"></param>
		public static void AddIgnoreDownKey(VirtualKeys key)
		{
			s_ignoreDownList.Add(key);
		}

		/// <summary>
		/// キーボードフックの通知
		/// </summary>
		/// <returns></returns>
		static IntPtr HookProc(int nCode, IntPtr wParam, ref KeyboardHookData lParam)
		{
			if (!Enable) return CallNextHookEx(s_hook, nCode, wParam, ref lParam);

			bool cancel = false;
			if (nCode == HC_ACTION)
			{
				s_eventArgs.Initialize((int)wParam, lParam);

				// イベント発行キャンセルの確認
				List<VirtualKeys> targetCheckIgnore = null;
				if (wParam.ToInt32() == WM_KEYDOWN || wParam.ToInt32() == WM_SYSKEYDOWN) targetCheckIgnore = s_ignoreDownList;
				else if (wParam.ToInt32() == WM_KEYUP || wParam.ToInt32() == WM_SYSKEYUP) targetCheckIgnore = s_ignoreUpList;

				bool ignore = false;
				if (targetCheckIgnore != null)
				{
					if (targetCheckIgnore.Contains(s_eventArgs.Key))
					{
						targetCheckIgnore.Remove(s_eventArgs.Key);
						ignore = true;
					}
				}
				if (!ignore)
				{
					switch (wParam.ToInt32())
					{
						case WM_KEYDOWN:
							OnKeyDown(s_eventArgs);
							if (KeyDown != null) KeyDown(null, s_eventArgs);
							break;
						case WM_KEYUP:
							OnKeyUp(s_eventArgs);
							if (KeyUp != null) KeyUp(null, s_eventArgs);
							break;
						case WM_SYSKEYDOWN:
							OnKeyDown(s_eventArgs);
							if (SysKeyDown != null) SysKeyDown(null, s_eventArgs);
							break;
						case WM_SYSKEYUP:
							OnKeyUp(s_eventArgs);
							if (SysKeyUp != null) SysKeyUp(null, s_eventArgs);
							break;
					}
				}

				// イベントの通知先でメッセージが編集された
				if (s_eventArgs.IsValueUpdate)
				{
					wParam = (IntPtr)s_eventArgs.NativeWParam;
					lParam = s_eventArgs.NativeLParam;
				}

				cancel = s_eventArgs.Cancel;
			}

			return cancel ? (IntPtr)1 : CallNextHookEx(s_hook, nCode, wParam, ref lParam);
		}

		#region 物理的なキー操作監視

		/* 
		 * 半角/全角キーやCapsLockキー、カタカナひらがなキー等のキー（以下総称してOEMキーとする）に対してKeyUpとKeyDownが正常に発行されないので、それを模倣するためのメソッド群。
		 * 最後に押されたのがOEMキーならば、キーダウン中に一定間隔で同じイベントが発行される特性を利用して、一定期間イベントがなかった時、OEMキーのKeyUpを発行。
		 * 最後に押されたのがOEMキー以外ならば、OEMキーダウン中もキーダウンが発行されることはないので、何かしらのキーアップ時に、ついでにOEMキーのKeyUpも発行してしまう。
		 * そのため、OEMキーを押してから、Aキーを押して、Aキーを離すと、OEMキーも一緒に離されたことになってしまう。
		 * また、OEMキーを押してから、Aキーを押して、OEMキーを離しても、OEMキーは押されたままの判定となる。
		 */

		static List<VirtualKeys> s_downedKeys;
		static bool s_physicalSimulate = true;
		static List<VirtualKeys> s_targetKeys = new List<VirtualKeys>();

		/// <summary>
		/// 物理的なキー操作シミュレーション対象とするキーの一覧を設定もしくは取得します。
		/// </summary>
		public static List<VirtualKeys> PhysicalTargetKeys
		{
			get { return s_targetKeys; }
			set { s_targetKeys = value; }
		}

		/// <summary>
		/// 通常のキーイベントが発行されないキーに対して、物理的なキー操作をシミュレートしてイベント発行するか否かを設定もしくは取得します。
		/// </summary>
		public static bool PhysicalSimulate
		{ get { return s_physicalSimulate; } set { s_physicalSimulate = value; } }			 

		/// <summary>
		/// 現在物理的に押下されているキーのリストを取得します。
		/// </summary>
		public static List<VirtualKeys> DownedKeys
		{
			get { return new List<VirtualKeys>(s_downedKeys.ToArray()); }
		}

		// 物理的なキーダウン時
		static void OnKeyDown(KeybordWatcherEventArgs e)
		{
			if (!PhysicalSimulate) return;

			while (s_downedKeys.Contains(e.Key)) s_downedKeys.Remove(e.Key);
			s_downedKeys.Add(e.Key);
		}

		// 物理的なキーアップ時
		static void OnKeyUp(KeybordWatcherEventArgs e)
		{
			if (!PhysicalSimulate) return;

			if (s_downedKeys.Count == 0) return;

			if (!IsTargetOEMKey(s_downedKeys[s_downedKeys.Count - 1]) &&  // 最後がOEMキーなら、タイマーが監視しているのでOK
				!IsTargetOEMKey(e.Key))
			{				
				for (int i = s_downedKeys.Count - 1; i >= 0; i--)
				{
					if (IsTargetOEMKey(s_downedKeys[i])) SimulatePhysicalKeyUp(s_downedKeys[i]);
				}
			}

			s_downedKeys.Remove(e.Key);
		}

		// 指定キーが通常のキーイベントを発生しないOEMキーか否かを取得
		static bool IsTargetOEMKey(VirtualKeys key)
		{
			return PhysicalTargetKeys.Contains(key);
		}

		// インターバル間更新がないなら、離されたということ
		static void s_watchOEMTimer_Tick(object sender, EventArgs e)
		{
			for (int i = s_downedKeys.Count - 1; i >= 0; i--)
			{
				if (IsTargetOEMKey(s_downedKeys[i])) SimulatePhysicalKeyUp(s_downedKeys[i]);
			}
		}

		// キーアップ再現
		static void SimulatePhysicalKeyUp(VirtualKeys key)
		{
			KeybordWatcherEventArgs e = new KeybordWatcherEventArgs();
			e.Initialize(s_eventArgs.NativeWParam, s_eventArgs.NativeLParam);
			e.Key = key;

			if (KeyUp != null) KeyUp(null, e);
		}

		#endregion

	}
}

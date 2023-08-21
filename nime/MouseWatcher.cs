using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Nime.Device
{
	/// <summary>
	/// マウスイベントのマウス座標を表します。
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct MousePoint
	{
		/// <summary>
		/// マウスイベントのマウス座標を初期化します。
		/// </summary>
		/// <param name="pt"></param>
		public MousePoint(int x, int y)
		{
			this.x = x;
			this.y = y;
		}

		/// <summary>
		/// マウスイベントのマウス座標を初期化します。
		/// </summary>
		/// <param name="pt"></param>
		public MousePoint(System.Drawing.Point pt)
		{
			x = pt.X;
			y = pt.Y;
		}

		/// <summary> x座標 </summary>
		public int x;

		/// <summary> y座標 </summary>
		public int y;

		/// <summary>
		/// 指定した座標との距離を取得します。
		/// </summary>
		/// <param name="p">測定対象の点</param>
		/// <returns></returns>
		public int GetSpan(MousePoint p)
		{
			return (int)Math.Sqrt(Math.Pow(p.x - x, 2) + Math.Pow(p.y - y, 2));
		}
	}

	/// <summary>
	/// マウスフック時のデータを表します。
	/// </summary>
	[StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
	public struct MouseHookData
	{
		public MousePoint pt;
		public int mouseData;
		public int flags;
		public int time;
		public IntPtr dwExtraInfo;
	}

	/// <summary>
	/// マウスフックを監視する静的クラスです。
	/// http://azumaya.s101.xrea.com/wiki/index.php?%B3%D0%BD%F1%2FC%A2%F4%2F%A5%B0%A5%ED%A1%BC%A5%D0%A5%EB%A5%D5%A5%C3%A5%AF
	/// http://lassy-tech.blogspot.com/2008/01/c-2.html
	/// （英語）： http://blogs.msdn.com/b/toub/archive/2006/05/03/589468.aspx
	/// </summary>
	public static class MouseWatcher
	{
		#region P/Invoke

		/// <summary> フックプロシージャのためのデリゲートです。 </summary>
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, ref MouseHookData lParam);

		/// <summary> 指定した名前のモジュールハンドルを取得します。 </summary>
		[DllImport("kernel32.dll")]
		static extern IntPtr GetModuleHandle(string lpModuleName);

		/// <summary> フックプロシージャ"lpfn"をフックチェーン内にインストールします。戻り値はフックプロシージャのハンドルです。 </summary>
		[DllImport("user32.dll")]
		static extern IntPtr SetWindowsHookEx(int idHook, LowLevelMouseProc lpfn, IntPtr hMod, int dwThreadId);

		/// <summary> 次のフックプロシージャにフック情報を渡します。 </summary>
		[DllImport("user32.dll")]
		static extern IntPtr CallNextHookEx(IntPtr hHook, int nCode, IntPtr wParam, ref MouseHookData lParam);

		/// <summary> インポートされたフックプロシージャを削除します。 </summary>
		[DllImport("user32.dll")]
		static extern bool UnhookWindowsHookEx(IntPtr hHook);

		
		const int WH_MOUSE_LL = 14;
		const int HC_ACTION = 0;
		const int WM_LBUTTONDOWN = 0x0201;
		const int WM_LBUTTONUP = 0x0202;
		//const int WM_LBUTTONDBLCLK = 0x0203;
		const int WM_MBUTTONDOWN = 0x0207;
		const int WM_MBUTTONUP = 0x0208;
		//const int WM_MBUTTONDBLCLK = 0x0209;
		const int WM_RBUTTONDOWN = 0x0204;
		const int WM_RBUTTONUP = 0x0205;
		//const int WM_RBUTTONDBLCLK = 0x204;

		const int WM_MOUSEMOVE = 0x0200;
		const int WM_MOUSEWHEEL = 0x020A;

		// チルト
		const int WM_MOUSEHWHEEL = 0x020E;
		const int WM_HSCROLL = 0x114;
		const int WM_VSCROLL = 0x115;

		const int WM_XBUTTONDOWN = 0x020B;
		const int WM_XBUTTONUP = 0x020C;
		//const int WM_XBUTTONDBLCLK = 0x020D;
		const int WM_NCXBUTTONDOWN = 0x0AB;
		const int WM_NCXBUTTONUP = 0x00AC;
		//const int WM_NCXBUTTONDBLCLK = 0x00AD;

		const int WHEEL_DELTA = 120;

		#endregion

		/// <summary>
		/// マウスフック時のイベントデータを格納する引数を表します。
		/// </summary>
		public sealed class MouseWatcherEventArgs : System.EventArgs
		{
			private bool _cancel;
			private int _nativeWParam;
			private MouseHookData _nativeLParam;
			private bool _isValueUpdate;

			private VirtualMouseButtons _button;
			private int _x;
			private int _y;
			private int _delta;
			private int _time;
			private DateTime _created;

			internal MouseWatcherEventArgs() { }

			/// <summary>
			/// イベントデータの初期化
			/// </summary>
			/// <param name="wParam"></param>
			/// <param name="lParam"></param>
			internal void Initialize(int wParam, MouseHookData lParam)
			{
				this._cancel = false;
				this._isValueUpdate = false;
				this._nativeWParam = wParam;
				this._nativeLParam = lParam;
				this._created = DateTime.Now;

				switch (wParam)
				{
					case WM_LBUTTONDOWN:
					case WM_LBUTTONUP:
						this._button = VirtualMouseButtons.Left;
						break;
					case WM_MBUTTONDOWN:
					case WM_MBUTTONUP:
						this._button = VirtualMouseButtons.Middle;
						break;
					case WM_RBUTTONDOWN:
					case WM_RBUTTONUP:
						this._button = VirtualMouseButtons.Right;
						break;
					case WM_XBUTTONDOWN:
					case WM_XBUTTONUP:
						// 参考：http://msdn.microsoft.com/en-us/library/ms646245%28VS.85%29.aspx
						if (HiWord(lParam.mouseData) == 0x0001)
							this._button = VirtualMouseButtons.XButton1;
						else
							this._button = VirtualMouseButtons.XButton2;
						break;
					default:
						this._button = VirtualMouseButtons.None;
						break;
				}
				this._x = lParam.pt.x;
				this._y = lParam.pt.y;
				this._delta = (wParam == WM_MOUSEWHEEL || wParam == WM_MOUSEHWHEEL || wParam == WM_HSCROLL) ? 
								  (int)HiWord(lParam.mouseData) : 0;
				this._time = lParam.time;
			}

			/// <summary>
			/// イベントに関連するマウスボタンを取得します。
			/// </summary>
			public VirtualMouseButtons Button { get { return this._button; } }

			/// <summary>
			/// イベントに関連するマウス位置のX座標を取得します。
			/// </summary>
			public int X { get { return this._x; } }

			/// <summary>
			/// イベントに関連するマウス位置のY座標を取得します。
			/// </summary>
			public int Y { get { return this._y; } }

			/// <summary>
			/// マウスイベント時のマウスカーソル位置を取得します。
			/// </summary>
			public MousePoint Point
			{
				get
				{
					MousePoint pt; pt.x = X; pt.y = Y; return pt;
				}
			}

			/// <summary>
			/// マウスホイール値の変化量を取得します。
			/// </summary>
			public int Delta { get { return this._delta; } }

			/// <summary>
			/// イベントにのTimeプロパティを取得します。
			/// </summary>
			public int Time { get { return this._time; } }

			/// <summary>
			/// このイベントの発生時刻を取得します。
			/// </summary>
			public DateTime CreatedTime { get { return _created; } }

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
			public MouseHookData NativeLParam
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
			public void Copy(ref MouseWatcherEventArgs e)
			{
				if (e == null) e = new MouseWatcherEventArgs();

				// e._cancel = _cancel;
				e._nativeWParam = _nativeWParam;
				e._nativeLParam = _nativeLParam;
				e._isValueUpdate = _isValueUpdate;

				e._button = _button;
				e._x = _x;
				e._y = _y;
				e._delta = _delta;
				e._time = _time;
			}

			/// <summary>
			/// イベントデータのコピーを返します。
			/// </summary>
			/// <returns></returns>
			public MouseWatcherEventArgs Copy()
			{
				MouseWatcherEventArgs e = null;
				Copy(ref e);
				return e;
			}

		}

		static IntPtr s_hook;
//		static MouseWatcherEventArgs s_eventArgs;
		static LowLevelMouseProc s_proc;
		static bool s_enable;
		static bool s_autoCheckIntegrity = true;
		static Dictionary<VirtualMouseButtons, int> s_rightKillMouseUp = new Dictionary<VirtualMouseButtons, int>();
		
		/// <summary> 何れかのマウスボタンが押されたときに呼び出されます。 </summary>
		public static event EventHandler<MouseWatcherEventArgs> MouseDown;

		/// <summary> 何れかのマウスボタンが放されたときに呼び出されます。 </summary>
		public static event EventHandler<MouseWatcherEventArgs> MouseUp;

		/// <summary> マウスが動いたときに呼び出されます。 </summary>
		public static event EventHandler<MouseWatcherEventArgs> MouseMove;

		/// <summary> マウスホイールの値が変更されたときに呼び出されます。 </summary>
		public static event EventHandler<MouseWatcherEventArgs> MouseWheel;

		/// <summary> マウスチルトの値が変更されたときに呼び出されます。 </summary>
		public static event EventHandler<MouseWatcherEventArgs> MouseTilt;

		/// <summary>
		/// 静的コンストラクタ
		/// </summary>
		static MouseWatcher()
		{
//			s_eventArgs = new MouseWatcherEventArgs();
			AppDomain.CurrentDomain.DomainUnload += delegate
			{
				if (s_hook != IntPtr.Zero)
					UnhookWindowsHookEx(s_hook);
			};
		}

		/// <summary>
		/// 現在マウスを監視しているか否かを取得します。
		/// </summary>
		public static bool NowWatching { get { return s_hook != IntPtr.Zero; } }

		/// <summary>
		/// マウスの監視を開始します。
		/// </summary>
		public static void Start()
		{
			using (Process process = Process.GetCurrentProcess())
			using (ProcessModule module = process.MainModule)
			{
				s_hook = SetWindowsHookEx( WH_MOUSE_LL,
														   s_proc = new LowLevelMouseProc(HookProc),
														   GetModuleHandle(module.ModuleName),
														   0);
			}

			if (s_hook == IntPtr.Zero) 
				throw new Exception("SetWindowsHookEx に失敗しました。");
		}

		/// <summary>
		/// マウスの監視を終了します。
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
		/// マウス監視時、イベント通知を行うかを設定もしくは取得します。
		/// </summary>
		public static bool Enable
		{
			get { return s_enable; }
			set { s_enable = value; }
		}

		/// <summary>
		/// マウスボタンの整合性をチェックし、不正なメッセージの無効化をキャンセルするか否かを設定もしくは取得します。
		/// </summary>
		public static bool AutoCheckButtonIntegrity
		{
			get { return s_autoCheckIntegrity; }
			set { s_autoCheckIntegrity = value; }
		}

		/// <summary>
		/// 上位ワードを取得します。
		/// </summary>
		/// <param name="wParam"></param>
		/// <returns></returns>
		static short HiWord(int input)
		{
			return (short)(input >> 16);
		}

		/// <summary>
		/// 下位ワードを取得します。
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		static short LoWord(int input)
		{
			return (short)(input & 0xFFFF);
		}


		/// <summary>
		/// マウスフックの通知
		/// </summary>
		/// <returns></returns>
		static IntPtr HookProc(int nCode, IntPtr wParam, ref MouseHookData lParam)
		{
			if (!Enable) return CallNextHookEx(s_hook, nCode, wParam, ref lParam);

			bool cancel = false;
			int downCount = 0;
			var eventArgs = new MouseWatcherEventArgs();
			if (nCode == HC_ACTION)
			{
				eventArgs.Initialize((int)wParam, lParam);

				switch (wParam.ToInt32())
				{
					case WM_LBUTTONDOWN:
					case WM_MBUTTONDOWN:
					case WM_RBUTTONDOWN:
					case WM_XBUTTONDOWN:
						downCount = 1;
						CallEvent(MouseDown, eventArgs);
						break;

					case WM_LBUTTONUP:
					case WM_MBUTTONUP:
					case WM_RBUTTONUP:
					case WM_XBUTTONUP:
						downCount = -1;
						CallEvent(MouseUp, eventArgs);
						break;

					case WM_MOUSEMOVE:
						CallEvent(MouseMove, eventArgs);
						break;

					case WM_MOUSEWHEEL:
						CallEvent(MouseWheel, eventArgs);
						break;
											
					case WM_MOUSEHWHEEL: // チルト（Vista以降でのみ有効）
					case WM_HSCROLL:	// チルト（xp以前ではこちらでメッセージが来てくれるかも）
						CallEvent(MouseTilt, eventArgs);
						break;
				}

				// イベントの通知先でメッセージが編集された
				if (eventArgs.IsValueUpdate)
				{
					wParam = (IntPtr)eventArgs.NativeWParam;
					lParam = eventArgs.NativeLParam;
				}

				// イベントの通知先でキャンセルされた
				cancel = eventArgs.Cancel;

				if (cancel && downCount != 0)
				{
					cancel = CheckMouseIntegrity(eventArgs.Button, downCount == 1);
				}
			}

#if DEBUG
			OutputInfomation(eventArgs.Button, cancel, wParam);
#endif

			return cancel ? (IntPtr)1 : CallNextHookEx(s_hook, nCode, wParam, ref lParam);
		}

		/// <summary>
		/// 共通のイベントコールメソッド
		/// </summary>
		private static void CallEvent(EventHandler<MouseWatcherEventArgs> eh, MouseWatcherEventArgs ev)
		{
			if (eh != null)
				eh(null, ev);
		}

		/// <summary>
		/// マウスダウンを再現したことを通知します。
		/// </summary>
		/// <param name="button"></param>
		public static void TellMouseDown(VirtualMouseButtons button)
		{
			if (!AutoCheckButtonIntegrity) return;
			else if (s_rightKillMouseUp[button] <= 0)
			{
#if DEBUG
				Console.WriteLine("状態が不正です。");
#endif
				return;
			}
			else s_rightKillMouseUp[button] -= 1;
		}

		/// <summary>
		/// マウスボタンの整合性チェックを行います。
		/// </summary>
		/// <param name="button"></param>
		/// <returns>キャンセルが不当な場合、falseを返します。</returns>
		static bool CheckMouseIntegrity(VirtualMouseButtons button, bool ButtonDown)
		{
			if (!AutoCheckButtonIntegrity) return true;

			if (!s_rightKillMouseUp.ContainsKey(button))
				s_rightKillMouseUp.Add(button, 0);
#if DEBUG
			if (button == VirtualMouseButtons.Right) Console.WriteLine("Right Down Killed Count::" + s_rightKillMouseUp[button].ToString());
#endif

			// ダウンを無効化したなら、アップを無効化する権利が一つ増える
			if (ButtonDown) s_rightKillMouseUp[button] += 1;
			else
			{
				if (s_rightKillMouseUp[button] <= 0)
				{
#if DEBUG
					SmalkerMainForm.ShowBalloonTipMessage(10000, "マウスボタンの整合性チェックからの通知", "MouseUp無効化をキャンセルしました。", System.Windows.Forms.ToolTipIcon.Info);
#endif
					//return false; // TODO:Temp 非同期にすると、タイムアップ後のコンテキストメニューが出てこなくなるので、消している。
					// 万が一、Upが通らなくなると危ないので、このチェックは残しておきたいのだが…
				}
				else s_rightKillMouseUp[button] -= 1;
			}
			return true;
		}

#if DEBUG
		/// <summary>
		/// 通知状態をコンソールに表示します。
		/// </summary>
		/// <param name="cancel"></param>
		/// <param name="wParam"></param>
		static void OutputInfomation(VirtualMouseButtons button, bool cancel, IntPtr wParam)
		{
			if (wParam.ToInt32() != WM_MOUSEMOVE)
			{
				string hookKind = "Down:";
				switch (wParam.ToInt32())
				{
					case WM_LBUTTONDOWN:
					case WM_MBUTTONDOWN:
					case WM_RBUTTONDOWN:
					case WM_XBUTTONDOWN:
						hookKind = "Down";
						break;
					case WM_LBUTTONUP:
					case WM_MBUTTONUP:
					case WM_RBUTTONUP:
					case WM_XBUTTONUP:
						hookKind = "Up";
						break;
					case WM_MOUSEWHEEL:
						hookKind = "Wheel";
						break;
					case WM_MOUSEHWHEEL: // チルト（Vista以降でのみ有効）
					case WM_HSCROLL:	// チルト（xp以前ではこちらでメッセージが来てくれるかも）
						hookKind = "Tilt";
						break;
				}
#if DEBUG
				Console.WriteLine(button.ToString() + " " + hookKind + "：" + (cancel ? "無効化されました" : "実行されました"));
#endif
			}
		}
#endif

	}
}

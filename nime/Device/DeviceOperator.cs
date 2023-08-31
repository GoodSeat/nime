using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Windows.Forms;

namespace GoodSeat.Nime.Device
{
	/// <summary>
	/// キーボードイベントの仮想キーを表します。
	/// 参考：http://msdn.microsoft.com/en-us/library/windows/desktop/dd375731%28v=vs.85%29.aspx
	/// </summary>
	public enum VirtualKeys
	{
		/// <summary> マウスの左ボタン </summary>
		LeftMouseButton = 0x01,
		/// <summary> マウスの右ボタン </summary>
		RightMouseButton = 0x02,
		/// <summary> Ctrl-Breakプロセス </summary>
		Cancel = 0x03,
		/// <summary> マウスの中ボタン </summary>
		MiddleMouseButton = 0x04,
		/// <summary> マウスのX1ボタン </summary>
		X1MouseButton = 0x05,
		/// <summary> マウスのX2ボタン </summary>
		X2MouseButton = 0x06,
		// 0x07 定義なし
		/// <summary> BackSpace </summary>
		BackSpace = 0x08,
		/// <summary> Tab </summary>
		Tab = 0x09,
		// 0x0A-0B Reserved
		/// <summary> Clear </summary>
		Clear = 0X0C,
		/// <summary> Enter </summary>
		Enter = 0x0D,
		// 0x0E-0F 定義なし
		/// <summary> Shift </summary>
		Shift = 0x10,
		/// <summary> Ctrl </summary>
		Ctrl = 0x11,
		/// <summary> Alt </summary>
		Alt = 0x12,
		/// <summary> Pause </summary>
		Pause = 0x13,
		/// <summary> Caps Lock </summary>
		CapsLock = 0x14,
		/// <summary> IME かな　モード </summary>
		Kana = 0x15,
		// 0x16 定義なし
		/// <summary> IME Junja モード </summary>
		Junja = 0x17,
		/// <summary> IME final モード </summary>
		Final = 0x18,
		/// <summary> IME 漢字 モード </summary>
		Kanji = 0x19,
		// 0x1A 定義なし
		/// <summary> Esc </summary>
		Esc = 0x1B,
		/// <summary> IME convert </summary>
		Convert = 0x1C,
		/// <summary> IME nonconvert </summary>
		NonConvert = 0x1D,
		/// <summary> IME accept </summary>
		Accept = 0x1E,
		/// <summary> IME モードチェンジ </summary>
		ModeChange = 0x1F,
		/// <summary> Space </summary>
		Space = 0x20,
		/// <summary> Page Up </summary>
		PageUp = 0x21,
		/// <summary> Page Down </summary>
		PageDown = 0x22,
		/// <summary> End </summary>
		End = 0x23,
		/// <summary> Home </summary>
		Home = 0x24,
		/// <summary> ← </summary>
		Left = 0x25,
		/// <summary> ↑ </summary>
		Up = 0x26,
		/// <summary> → </summary>
		Right = 0x27,
		/// <summary> ↓ </summary>
		Down = 0x28,
		/// <summary> Select </summary>
		Select = 0x29,
		/// <summary> Print </summary>
		Print = 0x2A,
		/// <summary> Execute </summary>
		Execute = 0x2B,
		/// <summary> Print Screen </summary>
		PrintScreen = 0x2C,
		/// <summary> Insert </summary>
		Ins = 0x2D,
		/// <summary> Del </summary>
		Del = 0x2E,
		/// <summary> Help </summary>
		Help = 0x2F,
		/// <summary> 0 </summary>
		D0 = 0x30,
		/// <summary> 1 </summary>
		D1 = 0x31,
		/// <summary> 2 </summary>
		D2 = 0x32,
		/// <summary> 3 </summary>
		D3 = 0x33,
		/// <summary> 4 </summary>
		D4 = 0x34,
		/// <summary> 5 </summary>
		D5 = 0x35,
		/// <summary> 6 </summary>
		D6 = 0x36,
		/// <summary> 7 </summary>
		D7 = 0x37,
		/// <summary> 8 </summary>
		D8 = 0x38,
		/// <summary> 9 </summary>
		D9 = 0x39,
		// 0x3A-40
		/// <summary> A </summary>
		A = 0x41,
		/// <summary> B </summary>
		B = 0x42,
		/// <summary> C </summary>
		C = 0x43,
		/// <summary> D </summary>
		D = 0x44,
		/// <summary> E </summary>
		E = 0x45,
		/// <summary> F </summary>
		F = 0x46,
		/// <summary> G </summary>
		G = 0x47,
		/// <summary> H </summary>
		H = 0x48,
		/// <summary> I </summary>
		I = 0x49,
		/// <summary> J </summary>
		J = 0x4A,
		/// <summary> K </summary>
		K = 0x4B,
		/// <summary> L </summary>
		L = 0x4C,
		/// <summary> M </summary>
		M = 0x4D,
		/// <summary> N </summary>
		N = 0x4E,
		/// <summary> O </summary>
		O = 0x4F,
		/// <summary> P </summary>
		P = 0x50,
		/// <summary> Q </summary>
		Q = 0x51,
		/// <summary> R </summary>
		R = 0x52,
		/// <summary> S </summary>
		S = 0x53,
		/// <summary> T </summary>
		T = 0x54,
		/// <summary> U </summary>
		U = 0x55,
		/// <summary> V </summary>
		V = 0x56,
		/// <summary> W </summary>
		W = 0x57,
		/// <summary> X </summary>
		X = 0x58,
		/// <summary> Y </summary>
		Y = 0x59,
		/// <summary> Z </summary>
		Z = 0x5A,
		/// <summary> Windows(左) </summary>
		WinLeft = 0x5B,
		/// <summary> Windows(右) </summary>
		WinRight = 0x5C,
		/// <summary> Applications </summary>
		Applications = 0x5D,
		// 0x5E Reserved
		/// <summary> Sleep </summary>
		Sleep = 0x5F,
		/// <summary> 0（テンキー） </summary>
		N0 = 0x60,
		/// <summary> 1（テンキー） </summary>
		N1 = 0x61,
		/// <summary> 2（テンキー） </summary>
		N2 = 0x62,
		/// <summary> 3（テンキー） </summary>
		N3 = 0x63,
		/// <summary> 4（テンキー） </summary>
		N4 = 0x64,
		/// <summary> 5（テンキー） </summary>
		N5 = 0x65,
		/// <summary> 6（テンキー） </summary>
		N6 = 0x66,
		/// <summary> 7（テンキー） </summary>
		N7 = 0x67,
		/// <summary> 8（テンキー） </summary>
		N8 = 0x68,
		/// <summary> 9（テンキー） </summary>
		N9 = 0x69,
		/// <summary> * </summary>
		Multiply = 0x6A,
		/// <summary> + </summary>
		Add = 0x6B,
		/// <summary> Separator </summary>
		Separator = 0x6C,
		/// <summary> Subtract </summary>
		Subtract = 0x6D,
		/// <summary> Decimal </summary>
		Decimal = 0x6E,
		/// <summary> / </summary>
		Divide = 0x6F,
		/// <summary> F1 </summary>
		F1 = 0x70,
		/// <summary> F2 </summary>
		F2 = 0x71,
		/// <summary> F3 </summary>
		F3 = 0x72,
		/// <summary> F4 </summary>
		F4 = 0x73,
		/// <summary> F5 </summary>
		F5 = 0x74,
		/// <summary> F6 </summary>
		F6 = 0x75,
		/// <summary> F7 </summary>
		F7 = 0x76,
		/// <summary> F8 </summary>
		F8 = 0x77,
		/// <summary> F9 </summary>
		F9 = 0x78,
		/// <summary> F10 </summary>
		F10 = 0x79,
		/// <summary> F11 </summary>
		F11 = 0x7A,
		/// <summary> F12 </summary>
		F12 = 0x7B,
		/// <summary> F13 </summary>
		F13 = 0x7C,
		/// <summary> F14 </summary>
		F14 = 0x7D,
		/// <summary> F15 </summary>
		F15 = 0x7E,
		/// <summary> F16 </summary>
		F16 = 0x7F,
		/// <summary> F17 </summary>
		F17 = 0x80,
		/// <summary> F18 </summary>
		F18 = 0x81,
		/// <summary> F19 </summary>
		F19 = 0x82,
		/// <summary> F20 </summary>
		F20 = 0x83,
		/// <summary> F21 </summary>
		F21 = 0x84,
		/// <summary> F22 </summary>
		F22 = 0x85,
		/// <summary> F23 </summary>
		F23 = 0x86,
		/// <summary> F24 </summary>
		F24 = 0x87,
		// 0x88-8F Unassigned
		/// <summary> Num Lock </summary>
		NumLock = 0x90,
		/// <summary> Scroll Lock </summary>
		ScrollLock = 0x91,
		// 0x92-96 OEM specific
		// 0x97-9F Unasigned
		/// <summary> Left Shift </summary>
		ShiftLeft = 0xA0,
		/// <summary> Right Shift </summary>
		ShiftRight = 0xA1,
		/// <summary> Left Control </summary>
		ControlLeft = 0xA2,
		/// <summary> Right Control </summary>
		ControlRight = 0xA3,
		/// <summary> Left Menu </summary>
		MenuLeft = 0xA4,
		/// <summary> Right Menu </summary>
		MenuRight = 0xA5,
		/// <summary> Browser Back </summary>
		BrowserBack = 0xA6,
		/// <summary> Browser Forward </summary>
		BrowserForward = 0xA7,
		/// <summary> Browser Refresh </summary>
		BrowserRefresh = 0xA8,
		/// <summary> Browser Stop </summary>
		BrowserStop = 0xA9,
		/// <summary> Browser Search </summary>
		BrowserSearch = 0xAA,
		/// <summary> Browser Favorites </summary>
		BrowserFavorites = 0xAB,
		/// <summary> Browser Start and Home </summary>
		BrowserHome = 0xAC,
		/// <summary> Volume Mute </summary>
		VolumeMute = 0xAD,
		/// <summary> Volume Down </summary>
		VolumeDown = 0xAE,
		/// <summary> Volume Up </summary>
		VolumeUp = 0xAF,
		/// <summary> Next Track </summary>
		NextTrack = 0xB0,
		/// <summary> Previous Track </summary>
		PreviousTrack = 0xB1,
		/// <summary> Stop Media </summary>
		StopMedia = 0xB2,
		/// <summary> Play/Pause Media </summary>
		PlayPauseMedia = 0xB3,
		/// <summary> Start mail </summary>
		StartMail = 0xB4,
		/// <summary> Select Media </summary>
		SelectMedia = 0xB5,
		/// <summary> Start Application 1 </summary>
		StartApplication1 = 0xB6,
		/// <summary> Start Application 2 </summary>
		StartApplication2 = 0xB7,
		// 0xB8-B9 Reserved
		/// <summary> VK_OEM_1 (';:' in US) </summary>
		OEM1 = 0xBA,
		/// <summary> + </summary>
		OEMPlus = 0xBB,
		/// <summary> , </summary>
		OEMCommma = 0xBC,
		/// <summary> - </summary>
		OEMMinus = 0xBD,
		/// <summary> . </summary>
		OEMPeriod = 0xBE,
		/// <summary> VK_OEM_2 ('/?' in US) </summary>
		OEM2 = 0xBF,
		/// <summary> VK_OEM_3 (~' in US) </summary>
		OEM3 = 0xC0,
		// 0xC1-D7 Reserved
		// 0sD8-DA Unassinged
		/// <summary> VK_OEM_4 ('[{' in US) </summary>
		OEM4 = 0xDB,
		/// <summary> VK_OEM_5 ('\|' in US) </summary>
		OEM5 = 0xDC,
		/// <summary> VK_OEM_6 (']}' in US) </summary>
		OEM6 = 0xDD,
		/// <summary> VK_OEM_7 (' ' " ' in US) </summary>
		OEM7 = 0xDE,
		/// <summary> VK_OEM_8 </summary>
		OEM8 = 0xDF,
		// 0xE0 Reserved
		// 0xE1 OEM spedific
		OEM102 = 0xE2,
		// 0xE3-E4 OEM specific
		Process = 0xE5,
		// 0xE6 OEM specific
		Packet = 0xE7,
		// 0xE8 Unassigned
		// 0xE9-F5 OEM specific
		// ==> vkを参考に追記
		/// <summary> 英数 </summary>
		OEMAttn = 0xF0,
		/// <summary> Shift + カタカナひらがな </summary>
		OEMFinish = 0xF1,
		/// <summary> カタカナひらがな </summary>
		OEMCopy = 0xF2,
		/// <summary> 全角/半角 </summary>
		OEMAuto = 0xF3,
		/// <summary> 全角/半角 </summary>
		OEMEnlw = 0xF4,
		/// <summary> ローマ字 </summary>
		OEMBackTab = 0xF5,
		// <==
		Attn = 0xF6,
		CrSel = 0xF7,
		ExSel = 0xF8,
		EraseEOF = 0xF9,
		Play = 0xFA,
		Zoom = 0xFB,
		NoName = 0xFC,
		PA1 = 0xFD,
		OEMClear = 0xFE
	}

	/// <summary>
	/// キーイベントタイプを表します。
	/// </summary>
    public enum KeyEventType { Down, Up, Stroke }
	
	/// <summary>
	/// マウスの仮想ボタンを表します。
	/// </summary>
	[Flags]
	public enum VirtualMouseButtons
	{
		/// <summary> なし </summary>
		None = 0,
		/// <summary> 左 </summary>
		Left = 1,
		/// <summary> 右 </summary>
		Right = 2,
		/// <summary> 中（ホイール） </summary>
		Middle = 4,
		/// <summary> X1 </summary>
		XButton1 = 8,
		/// <summary> X2 </summary>
		XButton2 = 16,
		/// <summary> 全て </summary>
		All = (Left | Right | Middle | XButton1 | XButton2)
	}

	/// <summary>
	/// 入力デバイスの操作クラスです。
	/// </summary>
	public static class DeviceOperator
	{
		static bool s_watchableKeyboard = false;

		/// <summary>
		/// 本クラスによるキーボード操作を、キーボード監視クラスが検知できるか否かを設定もしくは取得します。
		/// </summary>
		public static bool EnableWatchKeyboard
		{
			get { return s_watchableKeyboard; }
			set { s_watchableKeyboard = value; }
		}

		/// <summary>
		/// 現在のプロセスが64bit環境上で動作しているか否かを取得します。
		/// </summary>
		public static bool Is64bitEnvironment
		{
			get
			{
				if (IntPtr.Size == 4)
				{
					if (IsWow64())
					{
						//OSは64ビットです
						return true;
					}
					else
					{
						//OSは32ビットです
						return false;
					}
				}
				else if (IntPtr.Size == 8)
				{
					//OSは64ビットです
					return true;
				}

				return false;
			}
		}

		//現在のプロセスがWOW32上で動作しているか調べる
		private static bool IsWow64()
		{
			//IsWow64Processが使えるか調べる
			IntPtr wow64Proc = GetProcAddress(GetModuleHandle("Kernel32.dll"), "IsWow64Process");
			if (wow64Proc != IntPtr.Zero)
			{
				//IsWow64Processを呼び出す
				bool ret;
				if (IsWow64Process(System.Diagnostics.Process.GetCurrentProcess().Handle, out ret))
				{
					return ret;
				}
			}
			return false;
		}

		#region Import dll
		
		[DllImport("kernel32.dll", CharSet = CharSet.Auto)]
		private static extern IntPtr GetModuleHandle(string lpModuleName);

		[DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true)]
		private static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

		[DllImport("kernel32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool IsWow64Process([In] IntPtr hProcess, [Out] out bool lpSystemInfo);


		/// <summary>
		/// イベントを送信します。
		/// </summary>
		/// <param name="nInputs">Input構造体の数（イベント数）</param>
		/// <param name="pInputs">Input構造体</param>
		/// <param name="cbSize">Input構造体のサイズ</param>
		/// <returns></returns>
		[DllImport("user32.dll")]
		extern static uint SendInput(uint nInputs, Input[] pInputs, int cbSize);
		[DllImport("user32.dll")]
		extern static uint SendInput(uint nInputs, Input64[] pInputs, int cbSize);

		// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/winui/winui/windowsuserinterface/userinput/keyboardinput/keyboardinputreference/keyboardinputfunctions/mapvirtualkey.asp
		[DllImport("user32.dll")]
		extern static uint MapVirtualKey(uint uCode, uint uMapType);

		// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/winui/winui/windowsuserinterface/windowing/messagesandmessagequeues/messagesandmessagequeuesreference/messagesandmessagequeuesfunctions/getmessageextrainfo.asp
		[DllImport("user32.dll")]
		extern static IntPtr GetMessageExtraInfo();

		#endregion

		#region Input構造体

		/// <summary>
		/// 入力イベントのインプット構造体を表します。
		/// </summary>
		[StructLayout(LayoutKind.Explicit)]  // アンマネージ DLL 対応用 struct 記述宣言
		private struct Input
		{
			[FieldOffset(0)]
			public int type;
			[FieldOffset(4)]
			public MouseInput mi;
			[FieldOffset(4)]
			public KeyboardInput ki;
			[FieldOffset(4)]
			public HardwareInput hi;
		}

		/// <summary>
		/// 入力イベントのインプット構造体を表します。
		/// </summary>
		[StructLayout(LayoutKind.Explicit)]  // アンマネージ DLL 対応用 struct 記述宣言
		private struct Input64
		{
			[FieldOffset(0)]
			public int type;
			[FieldOffset(8)]
			public MouseInput mi;
			[FieldOffset(8)]
			public KeyboardInput ki;
			[FieldOffset(8)]
			public HardwareInput hi;
		}

		/// <summary>
		/// マウスイベント情報を表します。
		/// </summary>
		[StructLayout(LayoutKind.Sequential)]  // アンマネージ DLL 対応用 struct 記述宣言
		private struct MouseInput
		{
			internal int dx;
			internal int dy;
			internal int mouseData;
			internal uint dwFlags;
			internal uint time;
			internal IntPtr dwExtraInfo;
		}

		/// <summary>
		/// キーボードイベントの情報を表します。
		/// </summary>
		[StructLayout(LayoutKind.Sequential)]  // アンマネージ DLL 対応用 struct 記述宣言
		private struct KeyboardInput
		{
			internal ushort wVk;
			internal ushort wScan;
			internal uint dwFlags;
			internal uint time;
			internal IntPtr dwExtraInfo;
		}

		/// <summary>
		/// ハードウェアイベントの情報を表します。
		/// </summary>
		internal struct HardwareInput
		{
			internal uint uMsg;
			internal ushort wParamL;
			internal ushort wParamH;
		}

		#endregion
		
		#region 各種定数

		// type
		static int INPUT_MOUSE = 0;
		static int INPUT_KEYBOARD = 1;
		static int INPUT_HARDWARE = 2;

		// dwFlags
		static uint KEYEVENTF_KEYDOWN = 0x0000;
		static uint KEYEVENTF_EXTENDEDKEY = 0x0001;
		static uint KEYEVENTF_KEYUP = 0x0002;
		static uint KEYEVENTF_SCANCODE = 0x0008;
		static uint KEYEVENTF_UNICODE = 0x0004;

		// dwFlags
		static uint MOUSEEVENTF_MOVED = 0x0001;
		static uint MOUSEEVENTF_LEFTDOWN = 0x0002;  // 左ボタン Down
		static uint MOUSEEVENTF_LEFTUP = 0x0004;  // 左ボタン Up
		static uint MOUSEEVENTF_RIGHTDOWN = 0x0008;  // 右ボタン Down
		static uint MOUSEEVENTF_RIGHTUP = 0x0010;  // 右ボタン Up
		static uint MOUSEEVENTF_MIDDLEDOWN = 0x0020;  // 中ボタン Down
		static uint MOUSEEVENTF_MIDDLEUP = 0x0040;  // 中ボタン Up
		static uint MOUSEEVENTF_WHEEL = 0x0800; // 0x0080;
		static uint MOUSEEVENTF_HWHEEL = 0x1000;
		static uint MOUSEEVENTF_XDOWN = 0x0080; // 0x0100;
		static uint MOUSEEVENTF_XUP = 0x0100; // 0x0200;
		static uint MOUSEEVENTF_ABSOLUTE = 0x8000;

		static  int screen_length = 0x10000;  // for MOUSEEVENTF_ABSOLUTE (この値は固定)

		#endregion

		#region キーボード

		/// <summary>
		/// 指定した文字列を入力します。
		/// </summary>
		/// <param name="text"></param>
		public static void InputText(string text)
		{
			int len = text.Length;
			if (Is64bitEnvironment)
			{
				Input64[] inputs = new Input64[len * 2];
				for (int i = 0; i < len; i++)
				{
					int j = i * 2;
					inputs[j].type = INPUT_KEYBOARD;
					inputs[j].ki.dwFlags = KEYEVENTF_UNICODE;
					inputs[j].ki.wScan = text[i];

					int k = j + 1;
					inputs[k] = inputs[j];
					inputs[k].ki.dwFlags |= KEYEVENTF_KEYUP;
				}

				// イベントの発行
				if (inputs.Length != 0) SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(inputs[0]));
			}
			else
			{
				Input[] inputs = new Input[len * 2];
				for (int i = 0; i < len; i++)
				{
					int j = i * 2;
					inputs[j].type = INPUT_KEYBOARD;
					inputs[j].ki.dwFlags = KEYEVENTF_UNICODE;
					inputs[j].ki.wScan = text[i];

					int k = j + 1;
					inputs[k] = inputs[j];
					inputs[k].ki.dwFlags |= KEYEVENTF_KEYUP;
				}

				// イベントの発行
				if (inputs.Length != 0) SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(inputs[0]));
			}

			// Windows11のメモ帳での動作安定のために
			Thread.Sleep(text.Length);
			Application.DoEvents();
		}

		/// <summary>
		/// 連続したキーのDownもしくはUpイベントを発行します。
		/// </summary>
		/// <param name="values">キーとDownもしくはUpフラグのセットリスト。</param>
		public static void SendKeyEvents(params (VirtualKeys, KeyEventType)[] values)
		{
			if (values.Length == 0) return;

			var values_ = values.SelectMany((data, i) =>
			{
				var lst = new List<(VirtualKeys, KeyEventType)>();
				if (data.Item2 == KeyEventType.Stroke)
				{
					lst.Add((data.Item1, KeyEventType.Down));
					lst.Add((data.Item1, KeyEventType.Up));
				}
				else
				{
					lst.Add(data);
				}
				return lst;
			}).ToList();

			if (Is64bitEnvironment)
			{
				Input64[] inputs = new Input64[values_.Count];

				for (int i = 0; i < values_.Count; i++)
				{
					inputs[i].type = INPUT_KEYBOARD;
					inputs[i].ki.wVk = (ushort)values_[i].Item1;
					inputs[i].ki.wScan = (ushort)MapVirtualKey((uint)values_[i].Item1, 0);
					if (values_[i].Item2 == KeyEventType.Down)
					{
						inputs[i].ki.dwFlags = KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYDOWN;
					}
					else
					{
						inputs[i].ki.dwFlags = KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP;
					}
					inputs[i].ki.dwExtraInfo = GetMessageExtraInfo();
				}

				if (!EnableWatchKeyboard)
					foreach (var (key, ud) in values_)
					{
						if (ud == KeyEventType.Down) KeyboardWatcher.AddIgnoreDownKey(key);
						else						 KeyboardWatcher.AddIgnoreUpKey(key);
					}

				// イベントの発行
				SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(inputs[0]));
			}
			else
			{
				Input[] inputs = new Input[values_.Count];

				for (int i = 0; i < values_.Count; i++)
				{
					inputs[i].type = INPUT_KEYBOARD;
					inputs[i].ki.wVk = (ushort)values_[i].Item1;
					inputs[i].ki.wScan = (ushort)MapVirtualKey((uint)values_[i].Item1, 0);
					if (values_[i].Item2 == KeyEventType.Down)
					{
						inputs[i].ki.dwFlags = KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYDOWN;
					}
					else
					{
						inputs[i].ki.dwFlags = KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP;
					}
					inputs[i].ki.dwExtraInfo = GetMessageExtraInfo();
				}

				if (!EnableWatchKeyboard)
					foreach (var (key, ud) in values_)
					{
						if (ud == KeyEventType.Down) KeyboardWatcher.AddIgnoreDownKey(key);
						else						 KeyboardWatcher.AddIgnoreUpKey(key);
					}

				// イベントの発行
				SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(inputs[0]));
			}

			// Windows11のメモ帳での動作安定のために
			Thread.Sleep(1);
			Application.DoEvents();
		}

		/// <summary>
		/// 指定したキーを押します。
		/// </summary>
		/// <param name="keys"></param>
		public static void KeyDown(params VirtualKeys[] keys)
		{
			SendKeyEvents(keys.Select(k => (k, KeyEventType.Down)).ToArray());
		}

		/// <summary>
		/// 指定したキーを離します。
		/// </summary>
		/// <param name="keys"></param>
		public static void KeyUp(params VirtualKeys[] keys)
		{
			SendKeyEvents(keys.Select(k => (k, KeyEventType.Up)).ToArray());
        }

		/// <summary>
		/// 指定したキーを入力します。
		/// </summary>
		/// <param name="keys"></param>
		public static void KeyStroke(params VirtualKeys[] keys)
		{
			SendKeyEvents(keys.Select(k => (k, KeyEventType.Stroke)).ToArray());
		}

		#endregion

		#region マウス

		/// <summary>
		/// 指定したマウスボタンを押します。
		/// </summary>
		/// <param name="button"></param>
		public static void MouseButtonDown(VirtualMouseButtons button)
		{
			if (Is64bitEnvironment)
			{
				Input64[] inputs = new Input64[1];

				switch (button)
				{
					case VirtualMouseButtons.Left:
						inputs[0].mi.dwFlags = MOUSEEVENTF_LEFTDOWN; break;
					case VirtualMouseButtons.Middle:
						inputs[0].mi.dwFlags = MOUSEEVENTF_MIDDLEDOWN; break;
					case VirtualMouseButtons.Right:
						inputs[0].mi.dwFlags = MOUSEEVENTF_RIGHTDOWN; break;
					case VirtualMouseButtons.XButton1:
						inputs[0].mi.dwFlags = MOUSEEVENTF_XDOWN;
						inputs[0].mi.mouseData = 0x0001;
						break;
					case VirtualMouseButtons.XButton2:
						inputs[0].mi.dwFlags = MOUSEEVENTF_XDOWN;
						inputs[0].mi.mouseData = 0x0002;
						break;
				}

				SendInput((uint)1, inputs, Marshal.SizeOf(inputs[0]));
			}
			else
			{
				Input[] inputs = new Input[1];

				switch (button)
				{
					case VirtualMouseButtons.Left:
						inputs[0].mi.dwFlags = MOUSEEVENTF_LEFTDOWN; break;
					case VirtualMouseButtons.Middle:
						inputs[0].mi.dwFlags = MOUSEEVENTF_MIDDLEDOWN; break;
					case VirtualMouseButtons.Right:
						inputs[0].mi.dwFlags = MOUSEEVENTF_RIGHTDOWN; break;
					case VirtualMouseButtons.XButton1:
						inputs[0].mi.dwFlags = MOUSEEVENTF_XDOWN;
						inputs[0].mi.mouseData = 0x0001;
						break;
					case VirtualMouseButtons.XButton2:
						inputs[0].mi.dwFlags = MOUSEEVENTF_XDOWN;
						inputs[0].mi.mouseData = 0x0002;
						break;
				}

				SendInput((uint)1, inputs, Marshal.SizeOf(inputs[0]));
			}
		}

		/// <summary>
		/// 指定したマウスボタンを放します。
		/// </summary>
		/// <param name="button"></param>
		public static void MouseButtonUp(VirtualMouseButtons button)
		{
			if (Is64bitEnvironment)
			{
				Input64[] inputs = new Input64[1];

				switch (button)
				{
					case VirtualMouseButtons.Left:
						inputs[0].mi.dwFlags = MOUSEEVENTF_LEFTUP; break;
					case VirtualMouseButtons.Middle:
						inputs[0].mi.dwFlags = MOUSEEVENTF_MIDDLEUP; break;
					case VirtualMouseButtons.Right:
						inputs[0].mi.dwFlags = MOUSEEVENTF_RIGHTUP; break;
					case VirtualMouseButtons.XButton1:
						inputs[0].mi.dwFlags = MOUSEEVENTF_XUP;
						inputs[0].mi.mouseData = 0x0001;
						break;
					case VirtualMouseButtons.XButton2:
						inputs[0].mi.dwFlags = MOUSEEVENTF_XUP;
						inputs[0].mi.mouseData = 0x0002;
						break;
				}

				SendInput((uint)1, inputs, Marshal.SizeOf(inputs[0]));
			}
			else
			{
				Input[] inputs = new Input[1];

				switch (button)
				{
					case VirtualMouseButtons.Left:
						inputs[0].mi.dwFlags = MOUSEEVENTF_LEFTUP; break;
					case VirtualMouseButtons.Middle:
						inputs[0].mi.dwFlags = MOUSEEVENTF_MIDDLEUP; break;
					case VirtualMouseButtons.Right:
						inputs[0].mi.dwFlags = MOUSEEVENTF_RIGHTUP; break;
					case VirtualMouseButtons.XButton1:
						inputs[0].mi.dwFlags = MOUSEEVENTF_XUP;
						inputs[0].mi.mouseData = 0x0001;
						break;
					case VirtualMouseButtons.XButton2:
						inputs[0].mi.dwFlags = MOUSEEVENTF_XUP;
						inputs[0].mi.mouseData = 0x0002;
						break;
				}

				SendInput((uint)1, inputs, Marshal.SizeOf(inputs[0]));
			}
		}

		/// <summary>
		/// マウスホイール値を指定した値だけ変化させます。
		/// </summary>
		/// <param name="delta"></param>
		public static void MouseWheelChange(int delta)
		{
			if (Is64bitEnvironment)
			{
				Input64[] inputs = new Input64[1];

				inputs[0].mi.dwFlags = MOUSEEVENTF_WHEEL;
				inputs[0].mi.mouseData = delta;

				SendInput((uint)1, inputs, Marshal.SizeOf(inputs[0]));
			}
			else
			{
				Input[] inputs = new Input[1];

				inputs[0].mi.dwFlags = MOUSEEVENTF_WHEEL;
				inputs[0].mi.mouseData = delta;

				SendInput((uint)1, inputs, Marshal.SizeOf(inputs[0]));
			}
		}

		/// <summary>
		/// [Windows Vista以降でのみ有効]マウス横ホイール値を指定した値だけ変化させます。
		/// </summary>
		/// <param name="delta"></param>
		public static void MouseHWheelChange(int delta)
		{
			if (Is64bitEnvironment)
			{
				Input64[] inputs = new Input64[1];

				inputs[0].mi.dwFlags = MOUSEEVENTF_HWHEEL;
				inputs[0].mi.mouseData = delta;

				SendInput((uint)1, inputs, Marshal.SizeOf(inputs[0]));
			}
			else
			{
				Input[] inputs = new Input[1];

				inputs[0].mi.dwFlags = MOUSEEVENTF_HWHEEL;
				inputs[0].mi.mouseData = delta;

				SendInput((uint)1, inputs, Marshal.SizeOf(inputs[0]));
			}
		}

		/// <summary>
		/// 指定した座標にマウスを動かします。（精度が悪いです。精度を求めるならSystem.Windows.Forms.Cursor.Positionプロパティを利用してください。）
		/// </summary>
		/// <param name="toMove"></param>
		public static void MouseMove(Point toMove)
		{
			if (Is64bitEnvironment)
			{
				Input64[] inputs = new Input64[1];

				inputs[0].mi.dwFlags = MOUSEEVENTF_MOVED | MOUSEEVENTF_ABSOLUTE;

				Screen targetScreen = Screen.FromPoint(toMove);
				inputs[0].mi.dx = screen_length * toMove.X / Screen.PrimaryScreen.Bounds.Width;
				inputs[0].mi.dy = screen_length * toMove.Y / Screen.PrimaryScreen.Bounds.Height;

				SendInput((uint)1, inputs, Marshal.SizeOf(inputs[0]));
			}
			else
			{
				Input[] inputs = new Input[1];

				inputs[0].mi.dwFlags = MOUSEEVENTF_MOVED | MOUSEEVENTF_ABSOLUTE;

				Screen targetScreen = Screen.FromPoint(toMove);
				inputs[0].mi.dx = screen_length * toMove.X / Screen.PrimaryScreen.Bounds.Width;
				inputs[0].mi.dy = screen_length * toMove.Y / Screen.PrimaryScreen.Bounds.Height;

				SendInput((uint)1, inputs, Marshal.SizeOf(inputs[0]));
			}
		}

		#endregion

	}
}

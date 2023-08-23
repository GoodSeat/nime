// -----------------------------------------------------------------------------
//  Copyright (C) 2023 GoodSeat
//  Distributed under the MIT License
//  See https://sites.google.com/site/eatbaconandham/clapte/license 
// -----------------------------------------------------------------------------
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace GoodSeat.Nime.Windows
{
    /// <summary>
    /// アプリケーションウインドウの情報をカプセル化します。
    /// </summary>
    public partial class WindowInfo
    {
        /// <summary>
        /// マウスの座標を取得
        /// </summary>
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern bool GetCursorPos(ref POINT lpPoint);

        #region ウインドウ関連

        /// <summary>
        /// アプリケーションのウインドウ表示状態を設定します。
        /// </summary>
        /// <param name="wHnd"></param>
        /// <param name="cmdShow"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr wHnd, int cmdShow);


        /// <summary>
        /// Window の位置等を取得
        /// </summary>
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern bool GetWindowRect(IntPtr hWnd, ref RECT lpRect);

        /// <summary>
        /// 最大化されているか取得します。
        /// </summary>
        /// <param name="hWnd"></param>
        /// <returns></returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        private static extern bool IsZoomed(IntPtr hWnd);

        /// <summary>
        /// 最小化されているか取得します。
        /// </summary>
        /// <param name="hWnd"></param>
        /// <returns></returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        private static extern bool IsIconic(IntPtr hWnd);

        /// <summary>
        /// 指定したウインドウをアクティブにします。
        /// </summary>
        /// <param name="hWnd"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);


        static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2); // 全ての最前面ウインドウの後ろ
        static readonly IntPtr HWND_TOPMOST = new IntPtr(-1); // 最前面
        static readonly IntPtr HWND_TOP = new IntPtr(0); // 前面
        static readonly IntPtr HWND_BOTTOM = new IntPtr(1); // Zオーダーの最後

        static readonly uint SWP_NOSIZE = 0x0001; // サイズ維持フラグ
        static readonly uint SWP_NOMOVE = 0x0002; // 位置維持フラグ
        static readonly uint TOPMOST_FLAGS = (SWP_NOSIZE | SWP_NOMOVE); // サイズおよび位置の維持フラグ

        /// <summary>
        /// 指定ウインドウの表示状態を設定します。
        /// </summary>
        /// <param name="hWnd">対象のウインドウハンドル</param>
        /// <param name="hWndInsertAfter">Zオーダーの指定</param>
        /// <param name="x">移動先X座標</param>
        /// <param name="y">移動先Y座標</param>
        /// <param name="cx">変更後幅</param>
        /// <param name="cy">変更後高さ</param>
        /// <param name="flags">変更フラグを表すuint</param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint flags);

        #endregion

        #region Post/SendMessage
        [DllImport("user32.dll")]
        private static extern bool SendMessageCallback(IntPtr hWnd, uint Msg, Int32 wParam, Int32 lParam, SendAsyncProc lpCallback, Int32 dwData);
        private delegate void SendAsyncProc(IntPtr hwnd, uint uMsg, Int32 dwData, Int32 lResult);

        [DllImport("user32.dll")]
        private static extern bool IsHungAppWindow(IntPtr theWndHandle);

        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        private static extern Int32 SendMessage(IntPtr hWnd, uint Msg, Int32 wParam, ref COPYDATASTRUCT lParam);

        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        private static extern Int32 SendMessage(IntPtr hWnd, uint Msg, Int32 wParam, Int32 lParam);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
                
        [DllImport("user32.dll", SetLastError = true)]
        public static extern uint RegisterWindowMessage(string lpString);
        #endregion

        #region ウインドウハンドル/クラスの取得

        /// <summary>
        /// ウィンドウを検索します。(指定された文字列と一致するクラス名とウィンドウ名を持つトップレベルウィンドウのハンドルを取得)
        /// </summary>
        /// <param name="lpClassName">クラス名（指定しない場合null）</param>
        /// <param name="lpWindowName">ウインドウ名</param>
        /// <returns></returns>
        [DllImport("User32.dll", EntryPoint = "FindWindow", CharSet = CharSet.Auto)]
        private static extern IntPtr FindWindow(String lpClassName, String lpWindowName);

        /// <summary>
        /// 指定された座標を含むウィンドウのハンドルを取得
        /// </summary>
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr WindowFromPoint(POINT lpPoint);

        /// <summary>
        /// アクティブなアプリケーションのウインドウハンドルを取得
        /// </summary>
        /// <returns></returns>
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        /// <summary>
        /// デスクトップのウインドウハンドルを取得
        /// </summary>
        /// <returns></returns>
        [DllImport("user32.dll")]
        private static extern IntPtr GetDesktopWindow();

        /// <summary>
        /// ウインドウのクラス名を取得します。
        /// </summary>
        /// <param name="hWnd">対象のウインドウハンドル</param>
        /// <param name="s">取得されたクラス名の格納先</param>
        /// <param name="nMaxCount">クラス名のバッファサイズ</param>
        /// <returns></returns>
        [DllImport("User32.Dll", CharSet = CharSet.Unicode)]
        private static extern int GetClassName(IntPtr hWnd, StringBuilder s, int nMaxCount);

        /// <summary>
        /// ウインドウのタイトルバーのテキストを返します。
        /// </summary>
        /// <param name="hWnd">対象のウインドウハンドル</param>
        /// <param name="s">取得されたタイトルテキスト</param>
        /// <param name="nMaxCount">コピーする最大文字数</param>
        /// <returns></returns>
        [DllImport("User32.Dll", CharSet = CharSet.Unicode)]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder s, int nMaxCount);

        /// <summary>
        /// 実行ファイル名を取得します。[Vista以降でのみ]
        /// </summary>
        /// <param name="hProcess">プロセスハンドル</param>
        /// <param name="dwFlags"></param>
        /// <param name="lpExeName"></param>
        /// <param name="lpdwSize"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        static extern bool QueryFullProcessImageName(IntPtr hProcess, uint dwFlags, string lpExeName, ref int lpdwSize);

        /// <summary>
        /// プロセスハンドルへのアクセスフラグを表します。
        /// </summary>
        [Flags]
        enum ProcessAccessFlags : uint
        {
            Terminate = 0x00000001,
            CreateThread = 0x00000002,
            VMOperation = 0x00000008,
            VMRead = 0x00000010,
            VMWrite = 0x00000020,
            DuplicateHandle = 0x00000040,
            CreateProcess = 0x00000080,
            SetQuota = 0x00000100,
            SetInformation = 0x00000200,
            QueryInformation = 0x00000400,
            SuspendResume = 0x00000800,
            QueryLimitedInformation = 0x00001000,
            Synchronize = 0x00100000
        }

        /// <summary>
        /// プロセスハンドルへのアクセスを提供します。
        /// </summary>
        /// <param name="dwDesiredAccess"></param>
        /// <param name="bInheritHandle"></param>
        /// <param name="dwProcessId"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll", CallingConvention = CallingConvention.StdCall)]
        static extern IntPtr OpenProcess(ProcessAccessFlags dwDesiredAccess, bool bInheritHandle, uint dwProcessId);

        static string QueryFullProcessImageName(IntPtr hProcess, bool native)
        {
            uint dwFlags = (native ? (uint)0x00000001 : (uint)0);

            string text = new string((char)0, 1024);
            int len = text.Length;
            if (QueryFullProcessImageName(hProcess, dwFlags, text, ref len))
                return text.Substring(0, len);
            else
                return null;
        }

        /// <summary>
        /// 指定プロセスハンドルからファイル名を取得します。
        /// </summary>
        /// <param name="hWnd"></param>
        /// <returns></returns>
        static string GetFileNameOf(IntPtr hWnd)
        {
            IntPtr hProcess = IntPtr.Zero;
            try
            {
                int processID = 0;
                GetWindowThreadProcessId(hWnd, out processID);
                hProcess = OpenProcess(ProcessAccessFlags.QueryInformation | ProcessAccessFlags.VMRead, false, (uint)processID);

                return QueryFullProcessImageName(hProcess, true);
            }
            catch (Exception exc)
            {
                Debug.WriteLine("[" + exc.Message + "] " + exc.StackTrace);
                return null;
            }
            finally
            {
                if (hProcess != IntPtr.Zero)
                {
                    CloseHandle(hProcess);
                }
            }
        }

        /// <summary>
        /// ウインドウハンドルからプロセスIDを取得
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="lpdwProcessId"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);

        [DllImport("kernel32.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern bool CloseHandle(IntPtr handle);

        const Int32 WM_COPYDATA = 0x4A;
        const Int32 WM_USER = 0x400;

        #endregion

        /// <summary>
        /// COPYDATASTRUCT構造体 
        /// </summary>
        private struct COPYDATASTRUCT
        {
            public Int32 dwData;    //送信する32ビット値
            public Int32 cbData;    //lpDataのバイト数
            public string lpData;        //送信するデータへのポインタ(0も可能)
        }

        /// <summary>
        /// POINT 構造体
        /// </summary>
        private struct POINT
        {
            public int X;
            public int Y;
        }

        /// <summary>
        /// RECT 構造体
        /// </summary>
        private struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        /// <summary>
        /// ウインドウの表示状態を表します。
        /// </summary>
        public enum WindowState
        {
            /// <summary>
            /// ウインドウをアクティブ化し、最小化します。
            /// </summary>
            Minimized = 2,
            /// <summary>
            /// ウインドウをアクティブ化し、最大表示します。
            /// </summary>
            Maximized = 3,
            /// <summary>
            /// ウインドウをアクティブ化し、現在の位置とサイズで表示します。
            /// </summary>
            Show = 5,
            /// <summary>
            /// ウインドウを最小化します。
            /// </summary>
            Minimize = 6,
            /// <summary>
            /// ウインドウをアクティブ化せずに、最小化します。
            /// </summary>
            MinimizedNoActive = 7,
            /// <summary>
            /// ウインドウをアクティブ化せずに表示します
            /// </summary>
            ShowNoActive = 8,
            /// <summary>
            /// ウインドウをアクティブ化し、表示します。ウインドウが最小化もしくは最大化されている場合には、元に戻します。
            /// </summary>
            Restore = 9
        }

        // 参考
        enum ShowWindowCommands : int
        {
            /// <summary>
            /// Hides the window and activates another window.
            /// </summary>
            Hide = 0,
            /// <summary>
            /// Activates and displays a window. If the window is minimized or
            /// maximized, the system restores it to its original size and position.
            /// An application should specify this flag when displaying the window
            /// for the first time.
            /// </summary>
            Normal = 1,
            /// <summary>
            /// Activates the window and displays it as a minimized window.
            /// </summary>
            ShowMinimized = 2,
            /// <summary>
            /// Maximizes the specified window.
            /// </summary>
            Maximize = 3, // is this the right value?
            /// <summary>
            /// Activates the window and displays it as a maximized window.
            /// </summary>      
            ShowMaximized = 3,
            /// <summary>
            /// Displays a window in its most recent size and position. This value
            /// is similar to <see cref="Win32.ShowWindowCommand.Normal"/>, except
            /// the window is not activated.
            /// </summary>
            ShowNoActivate = 4,
            /// <summary>
            /// Activates the window and displays it in its current size and position.
            /// </summary>
            Show = 5,
            /// <summary>
            /// Minimizes the specified window and activates the next top-level
            /// window in the Z order.
            /// </summary>
            Minimize = 6,
            /// <summary>
            /// Displays the window as a minimized window. This value is similar to
            /// <see cref="Win32.ShowWindowCommand.ShowMinimized"/>, except the
            /// window is not activated.
            /// </summary>
            ShowMinNoActive = 7,
            /// <summary>
            /// Displays the window in its current size and position. This value is
            /// similar to <see cref="Win32.ShowWindowCommand.Show"/>, except the
            /// window is not activated.
            /// </summary>
            ShowNA = 8,
            /// <summary>
            /// Activates and displays the window. If the window is minimized or
            /// maximized, the system restores it to its original size and position.
            /// An application should specify this flag when restoring a minimized window.
            /// </summary>
            Restore = 9,
            /// <summary>
            /// Sets the show state based on the SW_* value specified in the
            /// STARTUPINFO structure passed to the CreateProcess function by the
            /// program that started the application.
            /// </summary>
            ShowDefault = 10,
            /// <summary>
            ///  <b>Windows 2000/XP:</b> Minimizes a window, even if the thread
            /// that owns the window is not responding. This flag should only be
            /// used when minimizing windows from a different thread.
            /// </summary>
            ForceMinimize = 11
        }

        
    }
}

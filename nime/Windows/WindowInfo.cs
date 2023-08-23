// -----------------------------------------------------------------------------
//  Copyright (C) 2023 GoodSeat
//  Distributed under the MIT License
//  See https://sites.google.com/site/eatbaconandham/clapte/license 
// -----------------------------------------------------------------------------
using System.Text;
using System.Diagnostics;

namespace GoodSeat.Nime.Windows
{
    /// <summary>
    /// アプリケーションウインドウの情報をカプセル化します。
    /// </summary>
    public partial class WindowInfo
    {
        /// <summary>
        /// ウインドウのヒットテストの結果を表します。
        /// </summary>
        public enum WindowsHitPosition
        {
            /// <summary> エラー </summary>
            Error = -2,
            /// <summary> 透明箇所 </summary>
            Transparent = -1,
            /// <summary> 特定不可 </summary>
            Nowhere = 0,
            /// <summary> クライアント領域 </summary>
            Client = 1,
            /// <summary> キャプション領域 </summary>
            Caption = 2,
            /// <summary> システムメニュー </summary>
            SystemMenu = 3,
            /// <summary> サイズ変更領域 </summary>
            SizeBar = 4,
            /// <summary> メニュー </summary>
            Menu = 5,
            /// <summary> 水平スクロール </summary>
            HorizontalScroll = 6,
            /// <summary> 垂直スクロール </summary>
            VerticalScroll = 7,
            /// <summary> 最小化ボタン </summary>
            MinimizeButton = 8,
            /// <summary> 最大化ボタン </summary>
            MaximizeButton = 9,
            /// <summary> 領域枠左 </summary>
            Left = 10,
            /// <summary> 領域枠右 </summary>
            Right = 11,
            /// <summary> 領域枠上 </summary>
            Top = 12,
            /// <summary> 領域枠左上 </summary>
            TopLeft = 13,
            /// <summary> 領域枠右上 </summary>
            TopRight = 14,
            /// <summary> 領域枠下 </summary>
            Bottom = 15,
            /// <summary> 領域枠左下 </summary>
            BottomLeft = 16,
            /// <summary> 領域枠右下 </summary>
            BottomRight = 17,
            /// <summary> 領域枠 </summary>
            Border = 18,
            /// <summary> オブジェクト </summary>
            Object = 19,
            /// <summary> 終了ボタン </summary>
            Close = 20,
            /// <summary> ヘルプ </summary>
            Help = 21
        }

        /// <summary>
        /// 現在アクティブなウィンドウアプリケーションの情報を取得します。
        /// </summary>
        public static WindowInfo ActiveWindowInfo
        {
            get
            {
                IntPtr hWnd = GetForegroundWindow();
                return new WindowInfo(hWnd);
            }
        }

        /// <summary>
        /// 現在マウスカーソルの下にあるアプリケーションの情報を取得します。
        /// </summary>
        public static WindowInfo PointedWindow
        {
            get
            {
                POINT lpPoint = default(POINT);
                // マウス座標を取得
                GetCursorPos(ref lpPoint);

                //マウス座標よりハンドル取得
                IntPtr hWnd = default(IntPtr);
                hWnd = WindowFromPoint(lpPoint);

                return new WindowInfo(hWnd);
            }
        }

        /// <summary>
        /// 指定した名前のウインドウを有するウインドウ情報を取得します。
        /// </summary>
        /// <param name="windowName"></param>
        /// <returns></returns>
        public static WindowInfo GetWindowInfo(string windowName)
        {
            try
            {
                return new WindowInfo(FindWindow(null, windowName));
            }
            catch
            {
                return new WindowInfo(IntPtr.Zero);
            }
        }

        /// <summary>
        /// 指定した名前のクラスとウインドウを有するウインドウ情報を取得します。
        /// </summary>
        /// <param name="windowName"></param>
        /// <returns></returns>
        public static WindowInfo GetWindowInfo(string className, string windowName)
        {
            try
            {
                return new WindowInfo(FindWindow(className, windowName));
            }
            catch
            {
                return new WindowInfo(IntPtr.Zero);
            }
        }

        /// <summary>
        /// 指定したファイル名のウインドウ情報を取得します。
        /// </summary>
        /// <param name="fullPath">フルパスで指定するか否か</param>
        /// <param name="name">ファイル名</param>
        /// <returns></returns>
        public static WindowInfo GetWindowInfo(bool fullPath, string name)
        {
            foreach (Process prc in Process.GetProcesses())
            {
                if (prc.MainWindowHandle != IntPtr.Zero) // メインウインドウを持っている
                {
                    string filePath = "";
                    try
                    {
                        filePath = prc.MainModule.FileName;
                    }
                    catch { continue; }

                    if (filePath.ToLower() == name.ToLower() || 
                        (!fullPath && System.IO.Path.GetFileName(filePath).ToLower() == System.IO.Path.GetFileName(name).ToLower()))
                    {
                        return new WindowInfo(prc.MainWindowHandle);
                    }
                }
            }
            return new WindowInfo(IntPtr.Zero);
        }

        /// <summary>
        /// 現在開かれているメインウインドウを有するすべてのアプリケーションのウインドウ情報を取得します。
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<WindowInfo> GetAllWindows()
        {
            foreach (Process prc in Process.GetProcesses())
            {
                if (prc.MainWindowHandle != IntPtr.Zero) // メインウインドウを持っている
                {
                    yield return new WindowInfo(prc.MainWindowHandle);
                }
            }
        }
        

        /// <summary>
        /// 現在マウスカーソルがタスクバー、もしくはドッキングされたウインドウ・ツールバー上にあるか否かを取得します。
        /// </summary>
        public static bool PointedTaskBar
        {
            get
            {
                foreach (Screen scr in Screen.AllScreens)
                {
                    if (scr.WorkingArea.Contains(Cursor.Position))
                        return false;
                }
                return true;
            }
        }

        /// <summary>
        /// 現在マウスカーソルがデスクトップ上にあるか否かを取得します。
        /// </summary>
        public static bool PointedDeskTop
        {
            get
            {
                if (PointedTaskBar) return false;

                foreach (WindowInfo win in GetAllWindows())
                {
                    if (win.OnCursor) return false;
                }

                WindowInfo pWin = PointedWindow;

                // デスクトップは最小化とか最大化とかしないはず
                if (pWin.IsMaximized || pWin.IsMinimized) return false;
                foreach (Screen scr in Screen.AllScreens)
                {
                    if (pWin.Size.Width < scr.WorkingArea.Width || pWin.Size.Height < scr.WorkingArea.Height)
                        return false;
                    if (scr.Bounds.Contains(pWin.Location.X - 1, pWin.Location.Y - 1))
                        return false;
                }

                return true;
            }
        }

        /// <summary>
        /// 現在起動中のウインドウを有するアプリケーションのフルパスを全て返す反復子を取得します。
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<string> GetAllWindowsFilePath()
        {
            foreach (Process prc in Process.GetProcesses())
            {
                if (prc.MainWindowHandle != IntPtr.Zero) // メインウインドウを持っている
                {
                    string filePath = "";
                    try
                    {
                        filePath = prc.MainModule.FileName;
                    }
                    catch { }

                    if (filePath != "")    yield return filePath;
                }
            }
        }



        IntPtr _hWnd;
        Process _p;

        static WindowInfo()
        {
            s_del = new SendAsyncProc(SendMessage_Callback);
            s_sendMessageResult = new Int32[10000];
            s_sendMessageResultSet = new bool[10000];
        }

        /// <summary>
        /// ウインドウ情報を初期化します。
        /// </summary>
        /// <param name="hWnd"></param>
        public WindowInfo(IntPtr hWnd)
        {
            _hWnd = hWnd;

            if (_hWnd.ToInt32() <= 0) // 取得失敗
            {
            }
            else
            {
                int processid;
                GetWindowThreadProcessId(_hWnd, out processid);
                _p = Process.GetProcessById(processid);
            }
        }

        #region プロパティ

        /// <summary>
        /// プロセス情報を取得します。
        /// </summary>
        public Process Process
        {
            get { return _p; }
        }

        /// <summary>
        /// 製品名を取得します。
        /// </summary>
        public string ProductName
        {
            get
            {
                try
                {
                    if (_p == null || _p.HasExited) return "";

                    //if (_p.MainWindowHandle != IntPtr.Zero && _p.MainWindowTitle != null)
                        return _p.MainModule.FileVersionInfo.ProductName;
                }
                catch
                {
                    return "";
                }
            }
        }

        /// <summary>
        /// アプリケーションのフルパスを取得します。
        /// </summary>
        public string FullPath
        {
            get
            {
                try
                {
                    if (_p == null || _p.HasExited) return "ウインドウ情報の取得に失敗しました。";
                    
                    return _p.MainModule.FileName;
                }
                catch
                {
                    return "";
                }
            }
        }

        /// <summary>
        /// アプリケーションのファイル名を取得します。
        /// </summary>
        public string FileName
        {
            get { return System.IO.Path.GetFileName(FullPath); }
        }

        /// <summary>
        /// アプリケーションウインドウのクラス名を取得します。
        /// </summary>
        public string ClassName
        {
            get
            {
                try
                {
                    if (_p == null || _p.HasExited) return "ウインドウ情報の取得に失敗しました。";
                }
                catch (Exception e)
                {
                    return e.Message;
                }

                StringBuilder sbClassName = new StringBuilder(256);
                GetClassName(_hWnd, sbClassName, sbClassName.Capacity);
                return sbClassName.ToString();
            }
        }

        /// <summary>
        /// アプリケーションウインドウのタイトルバーテキストを取得します。
        /// </summary>
        public string TitleBarText
        {
            get
            {
                try
                {
                    if (_p == null || _p.HasExited) return "ウインドウ情報の取得に失敗しました。";
                }
                catch (Exception e)
                {
                    return e.Message;
                }

                StringBuilder sbTitleText = new StringBuilder(256);
                GetWindowText(_hWnd, sbTitleText, sbTitleText.Capacity);
                return sbTitleText.ToString();
            }
        }

        /// <summary>
        /// ウインドウサイズを設定もしくは取得します。
        /// </summary>
        /// <param name="size"></param>
        public Size Size
        {
            get
            {
                try
                {
                    if (_p == null || _p.HasExited) return Size.Empty;
                }
                catch { return Size.Empty; }

                RECT rect = new RECT();
                GetWindowRect(_hWnd, ref rect);

                return new Size(rect.Right - rect.Left, rect.Bottom - rect.Top);
            }
            set 
            {
                SetWindowPos(_hWnd, HWND_TOP, 0, 0, value.Width, value.Height, SWP_NOMOVE);
            }
        }

        /// <summary>
        /// ウインドウ位置を設定もしくは取得します。
        /// </summary>
        public Point Location
        {
            get
            {
                try
                {
                    if (_p == null || _p.HasExited) return Point.Empty;
                }
                catch { return Point.Empty; }

                RECT rect = new RECT();
                GetWindowRect(_hWnd, ref rect);

                return new Point(rect.Left, rect.Top);
            }
            set
            {
                SetWindowPos(_hWnd, HWND_TOP, value.X, value.Y, 0, 0, SWP_NOSIZE);
            }
        }

        /// <summary>
        /// ウインドウの左隅位置を取得します。
        /// </summary>
        public int Left { get { return Location.X; } }

        /// <summary>
        /// ウインドウの右隅位置を取得します。
        /// </summary>
        public int Right { get { return Location.X + Size.Width; } }

        /// <summary>
        /// ウインドウの最上部位置を取得します。
        /// </summary>
        public int Top { get { return Location.Y; } }

        /// <summary>
        /// ウインドウの最下部位置を取得します。
        /// </summary>
        public int Bottom { get { return Location.Y + Size.Height; } }

        /// <summary>
        /// ウインドウの表示状態を取得します。
        /// </summary>
        public System.Windows.Forms.FormWindowState FormWindowState
        {
            get
            {
                if (IsMaximized) return FormWindowState.Maximized;
                else if (IsMinimized) return FormWindowState.Minimized;
                else return FormWindowState.Normal;
            }
        }

        /// <summary>
        /// ウインドウが最小化されているか否かを取得します。
        /// </summary>
        public bool IsMinimized
        {
            get 
            {
                try
                {
                    if (_p == null || _p.HasExited)
                        return false;
                    else
                        return IsIconic(_hWnd);
                }
                catch { return false; }
            }
        }

        /// <summary>
        /// ウインドウが最大化されているか否かを取得します。
        /// </summary>
        public bool IsMaximized
        {
            get
            {
                try
                {
                    if (_p == null || _p.HasExited)
                        return false;
                    else
                        return IsZoomed(_hWnd);
                }
                catch { return false; }
            }
        }
        
        /// <summary>
        /// マウスカーソルがアプリケーションウインドウの上にあるか否かを取得します。アクティブ/非アクティブに依りません。
        /// </summary>
        public bool OnCursor
        {
            get
            {
                try
                {
                    if (_p == null || _p.HasExited) return false;
                }
                catch { return false; }

                Rectangle area = new Rectangle(Location, Size);
                return area.Contains(Cursor.Position);
            }
        }

        /// <summary>
        /// このウインドウのアプリケーションウインドウを取得します。ただし、MainWindowが存在しない場合には、このウインドウをそのまま返します。
        /// </summary>
        public WindowInfo MainWindow
        {
            get 
            {
                try
                {
                    if (_p == null || _p.HasExited || _p.MainWindowHandle == IntPtr.Zero || _p.MainWindowHandle == _hWnd)
                        return this;
                    else
                        return new WindowInfo(_p.MainWindowHandle).MainWindow;
                }
                catch
                {
                    return this;
                }
            }
        }

        /// <summary>
        /// ウインドウハンドルを取得します。
        /// </summary>
        public IntPtr Handle
        { get { return _hWnd; } }

        #endregion

        #region 操作

        /// <summary>
        /// ウインドウの表示状態を設定します。
        /// </summary>
        /// <param name="state"></param>
        public void SetWindowState(WindowState state)
        {
            try
            {
                if (_p == null || _p.HasExited) return;
            }
            catch { return; }
            ShowWindow(_hWnd, (int)state);
        }

        /// <summary>
        /// ウインドウをアクティブ化します。
        /// </summary>
        public void Activate()
        {
            try
            {
                if (_p == null || _p.HasExited) return;
            }
            catch { return; }
            SetForegroundWindow(_hWnd);
        }

        /// <summary>
        /// このウインドウのメインアプリケーションをアクティブ化します。
        /// </summary>
        public void ActivateMainWindow()
        {
            try
            {
                if (_p == null || _p.HasExited) return;
            }
            catch { return; }
            MainWindow.Activate();
        }

        /// <summary>
        /// このウインドウに対してメッセージを送信し、メッセージ処理が終了するまで待機します。
        /// </summary>
        /// <param name="Msg"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        public int SendMessage(uint Msg, int wParam, int lParam)
        {
            try
            {
                if (_p == null || _p.HasExited) return 0;
                if (IsHungAppWindow(_hWnd)) return 0;
            }
            catch { return 0; }
//            return SendMessage(_hWnd, Msg, wParam, lParam);

            int localIndex = s_localIndex;
            s_localIndex++;
            if (s_localIndex >= 9999) s_localIndex = 0;

            s_sendMessageResultSet[localIndex] = false;
            s_sendMessageResult[localIndex] = 0;
            SendMessageCallback(_hWnd, Msg, wParam, lParam, s_del, localIndex);

            int tryCount = 0;
            Application.DoEvents();
            while (!s_sendMessageResultSet[localIndex] && tryCount <= 50)
            {
                System.Threading.Thread.Sleep(1);
                Application.DoEvents();
                tryCount++;
            }
#if DEBUG
            //if (!s_sendMessageResultSet[localIndex]) Console.WriteLine("応答がありませんでした。" + _hWnd.ToString());
            //else Console.WriteLine("lResult = " + s_sendMessageResult[localIndex].ToString());
#endif

            return s_sendMessageResult[localIndex];
        }

        static SendAsyncProc s_del;
        static Int32[] s_sendMessageResult;
        static bool[] s_sendMessageResultSet;
        static int s_localIndex;

        private static void SendMessage_Callback(IntPtr hwnd, uint uMsg, Int32 dwData, Int32 lResult)
        {
            s_sendMessageResult[dwData] = lResult;
            s_sendMessageResultSet[dwData] = true;
#if DEBUG
//            Console.WriteLine(dwData.ToString() + "<callBack_lResult> = " + lResult.ToString());
#endif
        }

        /// <summary>
        /// このウインドウに対してメッセージを送信します。
        /// </summary>
        /// <param name="Msg"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        public bool PostMessage(uint Msg, int wParam, int lParam)
        {
            try
            {
                if (_p == null || _p.HasExited) return true;
                return PostMessage(_hWnd, Msg, (IntPtr)wParam, (IntPtr)lParam);
            }
            catch { return false; }
        }

        /// <summary>
        /// 現在のカーソルが指し示す場所を取得します。
        /// </summary>
        /// <returns></returns>
        public WindowsHitPosition HitTest()
        {
            return HitTest(Cursor.Position);
        }

        /// <summary>
        /// 指定した座標が指し示す場所を取得します。
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public WindowsHitPosition HitTest(Point position)
        {
            try
            {
                if (_p == null || _p.HasExited) return WindowsHitPosition.Nowhere;
            }
            catch { return WindowsHitPosition.Error; }

            uint WM_NCHITTEST     = 0x0084;
            return (WindowsHitPosition)SendMessage(WM_NCHITTEST, 0, MakeLParam(position.X, position.Y));
        }

        /// <summary>
        /// LParamを作成して返します。
        /// </summary>
        /// <param name="LoWord"></param>
        /// <param name="HiWord"></param>
        /// <returns></returns>
        public static int MakeLParam(int LoWord, int HiWord)
        {
            return ((int)(((ushort)LoWord) | ((uint)((ushort)HiWord)) << 16 )) ;
        }

        #endregion

        
    }
}

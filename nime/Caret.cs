using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace nime
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>参考:https://dobon.net/vb/bbs/log3-56/32471.html</remarks>
    public static class Caret
    {
        [DllImport("user32.dll", EntryPoint = "GetCaretPos")]
        static extern bool GetCaretPos(out Point lpPoint);

        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        static extern IntPtr AttachThreadInput(IntPtr idAttach, IntPtr idAttachTo, bool fAttach);

        [DllImport("user32.dll")]
        static extern IntPtr GetWindowThreadProcessId(IntPtr hWnd, IntPtr lpdwProcessId);

        [DllImport("kernel32.dll")]
        static extern IntPtr GetCurrentThreadId();

        [DllImport("user32.dll")]
        static extern IntPtr GetFocus();

        [DllImport("user32.dll")]
        static extern bool ClientToScreen(IntPtr hwnd, out Point lpPoint);


        // TODO!:アプリケーションによってはうまく位置を取得できない。accLocationを使用した方法を検討のこと。
        public static Point GetCaretPosition()
        {
            IntPtr hWnd = GetForegroundWindow();

            IntPtr current = GetCurrentThreadId();
            IntPtr target = GetWindowThreadProcessId(hWnd, IntPtr.Zero);

            Point p;
            AttachThreadInput(current, target, true);
            GetCaretPos(out p);

            IntPtr fWnd = GetFocus();
            ClientToScreen(fWnd, out p);

            AttachThreadInput(current, target, false);

            return p;
        }
    }
}

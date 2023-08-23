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
    /// <remarks>参考:https://qiita.com/kob58im/items/a1644b36366f4d094a2c</remarks>
    public class IMEWatcher
    {
        [DllImport("User32.dll")]
        static extern int SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
        [DllImport("imm32.dll")]
        static extern IntPtr ImmGetDefaultIMEWnd(IntPtr hWnd);
        [DllImport("user32.dll")]
        static extern bool GetGUIThreadInfo(uint dwthreadid, ref GUITHREADINFO lpguithreadinfo);

        [StructLayout(LayoutKind.Sequential)]
        public struct GUITHREADINFO
        {
            public int cbSize;
            public int flags;
            public IntPtr hwndActive;
            public IntPtr hwndFocus;
            public IntPtr hwndCapture;
            public IntPtr hwndMenuOwner;
            public IntPtr hwndMoveSize;
            public IntPtr hwndCaret;
            public System.Drawing.Rectangle rcCaret;
        }

        const int WM_IME_CONTROL = 0x283;
        const int IMC_GETCONVERSIONMODE = 1;
        const int IMC_SETCONVERSIONMODE = 2;
        const int IMC_GETOPENSTATUS = 5;
        const int IMC_SETOPENSTATUS = 6;

        const int IME_CMODE_NATIVE    =  1;
        const int IME_CMODE_KATAKANA  =  2;
        const int IME_CMODE_FULLSHAPE =  8;
        const int IME_CMODE_ROMAN     = 16;

        const int CMode_HankakuKana = IME_CMODE_ROMAN | IME_CMODE_KATAKANA | IME_CMODE_NATIVE;
        const int CMode_ZenkakuEisu = IME_CMODE_ROMAN | IME_CMODE_FULLSHAPE;
        const int CMode_Hiragana    = IME_CMODE_ROMAN | IME_CMODE_FULLSHAPE | IME_CMODE_NATIVE;
        const int CMode_ZenkakuKana = IME_CMODE_ROMAN | IME_CMODE_FULLSHAPE | IME_CMODE_KATAKANA | IME_CMODE_NATIVE;

        public static bool IsOnIME(bool valueAlt)
        {
            //IME状態の取得
            GUITHREADINFO gti = new GUITHREADINFO();
            gti.cbSize = Marshal.SizeOf(gti);

            if (!GetGUIThreadInfo(0, ref gti)) {
                Console.WriteLine("GetGUIThreadInfo failed");
                return valueAlt;
                //throw new System.ComponentModel.Win32Exception(); // 2019.8.21追記:まれにここに来てしまう場合あるようなので throw せずに return; させたほうがよいかも
            }
            IntPtr imwd = ImmGetDefaultIMEWnd(gti.hwndFocus);

            int  imeConvMode = SendMessage(imwd, WM_IME_CONTROL, (IntPtr)IMC_GETCONVERSIONMODE, IntPtr.Zero);
            bool imeEnabled = (SendMessage(imwd, WM_IME_CONTROL, (IntPtr)IMC_GETOPENSTATUS, IntPtr.Zero) != 0);

            Console.WriteLine(imeEnabled.ToString() + " status code:"+imeConvMode.ToString());

            if ( imeEnabled ) {
                switch ( imeConvMode ) {
                case CMode_Hiragana:
                    /* Nothing to do */
                    break;
                case CMode_HankakuKana: /* through */
                case CMode_ZenkakuEisu: /* through */
                case CMode_ZenkakuKana:
                    //SendMessage(imwd, WM_IME_CONTROL, (IntPtr)IMC_SETCONVERSIONMODE, (IntPtr)CMode_Hiragana); // ひらがなモードに設定
                    break;
                default:
                    /* Nothing to do */
                    /* 環境によっては上のcaseをやめてここに飛ばしたほうがよいかも */
                    break;
                }
                return true;
            }/* else 無変換(半角英数) */
            return false;
        }

    }
}

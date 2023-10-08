using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace GoodSeat.Nime.Windows
{
    /// <summary>
    /// キーボードレイアウトの判定メソッドを提供します。
    /// </summary>
    public class KeyboardLayoutDetection
    {
        [DllImport("user32.dll")]
        private static extern long GetKeyboardLayoutName(StringBuilder pwskKLID);

        [DllImport("user32.dll")]
        private static extern int GetKeyboardType(int nTypeFlag);


        /// <summary>
        /// キーボードレイアウトのタイプを表します。
        /// </summary>
        public enum KeyboardLayeout
        {
            /// <summary>
            /// US配列。
            /// </summary>
            US,
            /// <summary>
            /// JIS配列。
            /// </summary>
            JIS,
            /// <summary>
            /// その他の配列。
            /// </summary>
            Other,
        }

        /// <summary>
        /// キーボードレイアウトタイプを判定して取得します。
        /// </summary>
        /// <returns>判定されたキーボードレイアウトタイプ。</returns>
        public static KeyboardLayeout GetKeyboardLayeout()
        {
            //var sb = new StringBuilder(9);
            //GetKeyboardLayoutName(sb);
            //switch (sb.ToString())
            //{
            //    case "00000411": return KeyboardLayeout.JIS;
            //    case "00000409": return KeyboardLayeout.US;
            //    default: return KeyboardLayeout.Other;
            //}

            int subtype = GetKeyboardType(1);

            switch (subtype)
            {
                case 0: return KeyboardLayeout.US;
                case 2: return KeyboardLayeout.JIS;
                default: return KeyboardLayeout.Other;
            }
        }


    }
}

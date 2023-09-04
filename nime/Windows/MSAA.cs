using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using GoodSeat.Nime.Windows;
using UIAutomationClient;

namespace GoodSeat.Nime.Windows
{
    public static class MSAA
    {
        [DllImport("oleacc.dll")]
        internal static extern int AccessibleObjectFromWindow(IntPtr hwnd, uint id, ref Guid iid, [In, Out, MarshalAs(UnmanagedType.IUnknown)] ref object ppvObject);

        [DllImport("oleacc.dll")]
        public static extern uint AccessibleObjectFromEvent( IntPtr hwnd, uint dwObjectID, uint dwChildID, out IAccessible ppacc, [MarshalAs(UnmanagedType.Struct)] out object pvarChild);

        [DllImport("oleacc.dll")]
        public static extern uint AccessibleChildren(IAccessible paccContainer, uint iChildStart, uint cChildren, [Out] object[] rgvarChildren, out uint pcObtained);


        #region Enumerations

        //Obj ID
        internal enum OBJID : uint
        {
            WINDOW = 0x00000000,
            SYSMENU = 0xFFFFFFFF,
            TITLEBAR = 0xFFFFFFFE,
            MENU = 0xFFFFFFFD,
            CLIENT = 0xFFFFFFFC,
            VSCROLL = 0xFFFFFFFB,
            HSCROLL = 0xFFFFFFFA,
            SIZEGRIP = 0xFFFFFFF9,
            CARET = 0xFFFFFFF8,
            CURSOR = 0xFFFFFFF7,
            ALERT = 0xFFFFFFF6,
            SOUND = 0xFFFFFFF5,
            CHILDID_SELF = 0,
            SELFLAG_TAKEFOCUS = 0x01
        }

        #endregion Enumerations


        public static Tuple<Point, Size> GetCaretPosition(WindowInfo wi = null)
        {
            return GetCaretPositionAsync(wi).Result;
        }

        public static Task<Tuple<Point, Size>> GetCaretPositionAsync(WindowInfo wi = null)
        {
            return Task.Run(() =>
            {
                Guid guid = typeof(IAccessible).GUID;
                object obj = null;

                wi = wi ?? WindowInfo.ActiveWindowInfo;
                int retVal1 = AccessibleObjectFromWindow(wi.Handle, (uint)OBJID.CARET, ref guid, ref obj);
                IAccessible iAccessible = obj as IAccessible;
                try
                {
                    iAccessible.accLocation(out int xLeft, out int yTop, out int cxWidth, out int cyHeight, (int)OBJID.CHILDID_SELF);
                    Debug.WriteLine($"accLocation:{xLeft},{yTop},{cxWidth},{cyHeight}");
                    return Tuple.Create(new Point(xLeft, yTop), new Size(cxWidth, cyHeight));
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("accLocationError!:" + ex.Message);
                }
                return Tuple.Create(Point.Empty, Size.Empty);
            });
        }


    }

}

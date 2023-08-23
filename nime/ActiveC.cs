using GoodSeat.Nime.Windows;
using System.Diagnostics;
using System.Runtime.InteropServices;
using UIAutomationClient;

namespace nime
{
    internal class ActiveC
    {

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);


        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetForegroundWindow(IntPtr hWnd);


        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);


        [DllImport("kernel32.dll")]
        static extern IntPtr GetCurrentThreadId();

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool AttachThreadInput(IntPtr idAttach, IntPtr idAttachTo, bool fAttach);


        [DllImport("user32.dll")]
        static extern IntPtr GetFocus();



//        private string GetActiveControlName(string className, string windowName)
//        {
//            IntPtr hwnd = FindWindow(className, windowName);
//            if (hwnd != IntPtr.Zero)
//            {
//                SetForegroundWindow(hwnd);
//                System.Threading.Thread.Sleep(1);
//
//                uint pid;
//                IntPtr tid = GetWindowThreadProcessId(hwnd, out pid);
//                if (AttachThreadInput(GetCurrentThreadId(), tid, true))
//                {
//                    IntPtr activeControl = GetFocus();
//                    if (activeControl != IntPtr.Zero)
//                    {
//                        AutomationElement ae = AutomationElement.FromHandle(activeControl);
//
//                        if (ae != null)
//                        {
//                            return ae.Current.Name;
//                        }
//                    }
//                }
//            }
//
//            return "";
//
//        }

        public static string GetActiveControlName3()
        {
            var wi = WindowInfo.ActiveWindowInfo;
            //IntPtr hwnd = GetForegroundWindow();
            IntPtr hwnd = wi.Handle;

            bool attached = true;
            uint pid;
            IntPtr tid = GetWindowThreadProcessId(hwnd, out pid);
            attached = AttachThreadInput(GetCurrentThreadId(), tid, true);

            if (attached)
            {
                IntPtr activeControl = GetFocus();
                if (activeControl != IntPtr.Zero)
                {
                    var automation = new UIAutomationClient.CUIAutomation8();
                    var element = automation.ElementFromHandle(activeControl);
                    //element.Dump();

                    var guid2 = typeof(IUIAutomationTextPattern2).GUID;
                    var ptr = element.GetCurrentPatternAs(UIA_PatternIds.UIA_TextPattern2Id, ref guid2);
                    //var ptr = element.GetCurrentPatternAs(10024, ref guid2);
                    if (ptr != IntPtr.Zero)
                    {
                        var pattern = (IUIAutomationTextPattern2)Marshal.GetObjectForIUnknown(ptr);
                        if (pattern != null)
                        {
                            var array = pattern.GetCaretRange(out int isActive).GetBoundingRectangles();
                            Debug.WriteLine($"array:{array.GetValue(0)},{array.GetValue(1)},{array.GetValue(2)},{array.GetValue(3)}");

                            var documentRange = pattern.DocumentRange;
                            var caretRange = pattern.GetCaretRange(out _);
                            if (caretRange != null)
                            {
                                var caretPos = caretRange.CompareEndpoints(
                                    TextPatternRangeEndpoint.TextPatternRangeEndpoint_Start,
                                    documentRange,
                                    TextPatternRangeEndpoint.TextPatternRangeEndpoint_Start);
                                //Debug.WriteLine(" caret is at " + caretPos);
                            }
                        }
                    }
                }
            }

            return "";

        }

    }
}

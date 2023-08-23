using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
//using Accessibility;
using GoodSeat.Nime.Windows;
using UIAutomationClient;

namespace nime
{
    public class Msaa
    {
        [DllImport("oleacc.dll")]
        internal static extern int AccessibleObjectFromWindow(IntPtr hwnd, uint id, ref Guid iid, [In, Out, MarshalAs(UnmanagedType.IUnknown)] ref object ppvObject);

        [DllImport("oleacc.dll")]
        public static extern uint AccessibleObjectFromEvent( IntPtr hwnd, uint dwObjectID, uint dwChildID, out IAccessible ppacc, [MarshalAs(UnmanagedType.Struct)] out object pvarChild);

        [DllImport("oleacc.dll")]
        public static extern uint AccessibleChildren(IAccessible paccContainer, uint iChildStart, uint cChildren, [Out] object[] rgvarChildren, out uint pcObtained);


         [DllImport("user32.dll")]
         static extern IntPtr GetFocus();

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

        IntPtr handle;

        [DllImport("user32.dll")]
        static extern bool GetGUIThreadInfo(uint dwthreadid, ref GUITHREADINFO lpguithreadinfo);

        [DllImport("user32.dll")]
        static extern IntPtr AttachThreadInput(IntPtr idAttach, IntPtr idAttachTo, bool fAttach);

        [DllImport("user32.dll")]
        static extern IntPtr GetWindowThreadProcessId(IntPtr hWnd, IntPtr lpdwProcessId);

        [DllImport("kernel32.dll")]
        static extern IntPtr GetCurrentThreadId();

        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

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

        public bool checkHandle()
        {
            Guid guid = typeof(IAccessible).GUID;
            object obj = null;

            // Use any window handle
            var wi = WindowInfo.ActiveWindowInfo;
            handle = wi.Handle;

            var acParent = AccessibleObjectHelper.GetAccessibleObjectFromWindow(handle, (uint)OBJID.CARET);
            {
                try
                {
                    acParent.accLocation(out int xLeft, out int yTop, out int cxWidth, out int cyHeight, (uint)OBJID.CHILDID_SELF);
                    Debug.WriteLine($"accLocation:{xLeft},{yTop},{cxWidth},{cyHeight}");
                }
                catch( Exception ex )
                {
                    Debug.WriteLine("accLocationError!:" + ex.Message);
                }
            }

            foreach (var c in AccessibleObjectHelper.GetChildren(acParent))
            {
                try
                {
                    c.accLocation(out int xLeft, out int yTop, out int cxWidth, out int cyHeight, (uint)OBJID.CHILDID_SELF);
                    Debug.WriteLine($"accLocation:{xLeft},{yTop},{cxWidth},{cyHeight}");
                }
                catch( Exception ex )
                {
                    Debug.WriteLine("accLocationError!:" + ex.Message);
                }
            }

            GUITHREADINFO gti = new GUITHREADINFO();
            gti.cbSize = Marshal.SizeOf(gti);
            if (!GetGUIThreadInfo(0, ref gti)) {
                Debug.WriteLine("GetGUIThreadInfo_error!");
            }

            ActiveC.GetActiveControlName3();

            var automation = new UIAutomationClient.CUIAutomation8();
            var element = automation.ElementFromHandle(gti.hwndFocus);

            //var cursor = System.Windows.Forms.Cursor.Position;
            //var element = automation.ElementFromPoint(new tagPOINT { x = cursor.X, y = cursor.Y });
            if (element != null)
            {
                Debug.WriteLine("Watched element " + element.CurrentName);
                //element.Dump();
                //element.DumpPatterns();

                var guid2 = typeof(IUIAutomationTextPattern2).GUID;
                var ptr = element.GetCurrentPatternAs(UIA_PatternIds.UIA_TextPattern2Id, ref guid2);
                //var ptr = element.GetCurrentPatternAs(10024, ref guid2);
                if (ptr != IntPtr.Zero)
                {
                    var pattern = (IUIAutomationTextPattern2)Marshal.GetObjectForIUnknown(ptr);
                    if (pattern != null)
                    {
                        var documentRange = pattern.DocumentRange;
                        var caretRange = pattern.GetCaretRange(out _);
                        if (caretRange != null)
                        {
                            var caretPos = caretRange.CompareEndpoints(
                                TextPatternRangeEndpoint.TextPatternRangeEndpoint_Start,
                                documentRange,
                                TextPatternRangeEndpoint.TextPatternRangeEndpoint_Start);
                            Debug.WriteLine(" caret is at " + caretPos);
                        }
                    }
                }
            }

            IntPtr hWnd = GetForegroundWindow();


            IntPtr current = GetCurrentThreadId();
            IntPtr target = GetWindowThreadProcessId(hWnd, IntPtr.Zero);

            AttachThreadInput(current, target, true);

            int retVal1 = AccessibleObjectFromWindow(hWnd, (int)OBJID.CARET, ref guid, ref obj);
            IAccessible iAccessible = obj as IAccessible;

            //int retVal = AccessibleObjectFromWindow(handle, (uint)OBJID.CARET, ref guid, ref obj);
            //iAccessible = (IAccessible)obj;

            try
            {
                string accWindowName = iAccessible.get_accName(0);
                string accWindowVal = iAccessible.get_accValue(0);
                System.Diagnostics.Debug.WriteLine("IAccessible Name : " + accWindowName);
                System.Diagnostics.Debug.WriteLine("IAccessible value : " + accWindowVal);
                System.Diagnostics.Debug.WriteLine("IAccessible Role is : " + iAccessible.get_accRole(0));

                System.Diagnostics.Debug.WriteLine("IAccessible Type: " + iAccessible.GetType());
                System.Diagnostics.Debug.WriteLine("IAccessible Help: " + iAccessible.accHelp);
                System.Diagnostics.Debug.WriteLine("IAccessible Focus is: " + iAccessible.accFocus);
                System.Diagnostics.Debug.WriteLine("IAccessible Selection is " + iAccessible.get_accState());
            }
            catch { }

            try
            {
                iAccessible.accLocation(out int xLeft, out int yTop, out int cxWidth, out int cyHeight, (int)OBJID.CHILDID_SELF);
                Debug.WriteLine($"accLocation:{xLeft},{yTop},{cxWidth},{cyHeight}");
            }
            catch( Exception ex )
            {
                Debug.WriteLine("accLocationError!:" + ex.Message);
            }


            IAccessible accCaret = null;
            object objchild;
            AccessibleObjectFromEvent(hWnd, (uint)OBJID.CARET, 0xFFFFFE24, out accCaret, out objchild);

            try
            {
                string accWindowName = accCaret.get_accName(0);
                string accWindowVal = accCaret.get_accValue(0);
                System.Diagnostics.Debug.WriteLine("accCaret Name : " + accWindowName);
                System.Diagnostics.Debug.WriteLine("accCaret value : " + accWindowVal);
                System.Diagnostics.Debug.WriteLine("accCaret Role is : " + accCaret.get_accRole(0));

                System.Diagnostics.Debug.WriteLine("accCaret Type: " + accCaret.GetType());
                System.Diagnostics.Debug.WriteLine("accCaret Help: " + accCaret.accHelp);
                System.Diagnostics.Debug.WriteLine("accCaret Focus is: " + accCaret.accFocus);
                System.Diagnostics.Debug.WriteLine("accCaret Selection is " + accCaret.get_accState());
            }
            catch { }

            try
            {
                uint ccc = 0xFFFFFE24;
                accCaret.accLocation(out int xLeft, out int yTop, out int cxWidth, out int cyHeight, ccc);
                Debug.WriteLine($"accLocation2:{xLeft},{yTop},{cxWidth},{cyHeight}");
            }
            catch( Exception ex )
            {
                Debug.WriteLine("accLocationError2!:" + ex.Message);
            }
            //iAccessible.accSelect((int)OBJID.SELFLAG_TAKEFOCUS, 0);
            //if (!accWindowName.Contains("Mozilla Firefox"))
            //    return false;

            //ProcessChildren(iAccessible, false);

            iAccessible = null;
            AttachThreadInput(current, target, false);
            return false;
        }

        private void ProcessChildren(IAccessible iChild, bool v)
        {
            IAccessible[] childs = new IAccessible[iChild.accChildCount];

            uint obtained = 0;
            uint firstChild = 0;
            uint childcount = (uint)iChild.accChildCount - 1;

            uint ret = AccessibleChildren(iChild, firstChild, childcount, childs, out obtained);
            int i = 0;
            foreach (IAccessible child in childs)
            {
                if (child == null)
                    continue;

                if (!Marshal.IsComObject(child)) continue;

                string accWindowName = child.get_accName(0);
                string accWindowVal = child.get_accValue(0);

                try
                {
                    System.Diagnostics.Debug.WriteLine("IAccessible Name : " + accWindowName);
                    System.Diagnostics.Debug.WriteLine("IAccessible value : " + accWindowVal);
                    System.Diagnostics.Debug.WriteLine("IAccessible Role is : " + child.get_accRole(0));

                    System.Diagnostics.Debug.WriteLine("IAccessible Type: " + child.GetType());
                    System.Diagnostics.Debug.WriteLine("IAccessible Focus is: " + child.accFocus);
                    System.Diagnostics.Debug.WriteLine("IAccessible Selection is " + child.get_accState());
                }
                catch { }

                try
                {
                    child.accLocation(out int xLeft, out int yTop, out int cxWidth, out int cyHeight, OBJID.CHILDID_SELF);
                    Debug.WriteLine($"accLocation:{xLeft},{yTop},{cxWidth},{cyHeight}");
                }
                catch
                {
                    Debug.WriteLine("accLocationError!");
                }

                ProcessChildren(child, false);

                Marshal.ReleaseComObject(child);
            }
        }

    }

    static class UIA
    {
        public static readonly CUIAutomation8Class Instance = new CUIAutomation8Class();

        public static IEnumerable<IUIAutomationElement> GetChildren(this IUIAutomationElement parent)
        {
            var walker = Instance.RawViewWalker;
            for (var child = walker.GetFirstChildElement(parent); child != null; child = walker.GetNextSiblingElement(child))
            {
                yield return child;
            }
        }

        public static IUIAutomationElement? FindDescendant(this IUIAutomationElement parent, Func<IUIAutomationElement, bool> predicate)
        {
            foreach (var child in parent.GetChildren())
            {
                if (predicate(child)) return child;

                var hit = child.FindDescendant(predicate);
                if (hit != null) return hit;
            }

            return null;
        }


        private static Dictionary<Type, int>? patternIdDict;

        public static T GetPattern<T>(this IUIAutomationElement element) => element.GetPatternAs<T>() ?? throw new InvalidOperationException($"Pattern type '{typeof(T)}' is null.");

        public static T? GetPatternAs<T>(this IUIAutomationElement element)
        {
            if (patternIdDict == null)
            {
                patternIdDict = new Dictionary<Type, int>();
                var assembly = typeof(IUIAutomationElement).Assembly;
                foreach (var patternIdField in typeof(UIA_PatternIds).GetFields())
                {
                    var patternName = patternIdField.Name[4..^2];
                    var patternId = (int)patternIdField.GetValue(null)!;
                    var patternTypeName = $"Interop.UIAutomationClient.IUIAutomation{patternName}";
                    var patternType = assembly.GetType(patternTypeName) ?? throw new InvalidOperationException($"{patternTypeName} not found.");
                    patternIdDict.Add(patternType, patternId);
                }
            }

            if (!patternIdDict.TryGetValue(typeof(T), out var id))
            {
                throw new ArgumentException($"Type '{typeof(T)}' is not UIA pattern type.", nameof(T));
            }

            return (T?)element.GetCurrentPattern(id);
        }

        public static IUIAutomationElement Dump(this IUIAutomationElement element, int limitNest = -1)
        {
            element.Dump(limitNest, 0);
            return element;
        }

        private static void Dump(this IUIAutomationElement element, int limitNest, int nest)
        {
            Debug.Write(new string(' ', nest));
            Debug.WriteLine($"{GetControlTypeName(element.CurrentControlType)}[{element.CurrentControlType}], Id: {element.CurrentAutomationId}, Class: {element.CurrentClassName}, Name: {element.CurrentName}");

            if (limitNest >= 0 && nest >= limitNest) return;

            foreach (var child in element.GetChildren())
            {
                child.Dump(limitNest, nest + 1);
            }
        }

        private static Dictionary<int, string>? controlTypeNameDict;

        private static string GetControlTypeName(int id)
        {
            if (controlTypeNameDict == null)
            {
                controlTypeNameDict =
                    typeof(UIA_ControlTypeIds)
                    .GetFields()
                    .ToDictionary(f => (int)f.GetValue(null)!, f => f.Name[4..^13]);
            }

            return controlTypeNameDict.TryGetValue(id, out var name) ? name : string.Empty;
        }


        public static IUIAutomationElement DumpPatterns(this IUIAutomationElement element)
        {
            if (valueDumpers == null)
            {
                var dumpers = new List<Action<IUIAutomationElement>>();
                var assembly = typeof(IUIAutomationElement).Assembly;
                foreach (var patternIdField in typeof(UIA_PatternIds).GetFields())
                {
                    var patternName = patternIdField.Name[4..^2];
                    var patternId = (int)patternIdField.GetValue(null)!;
                    var patternTypeName = $"Interop.UIAutomationClient.IUIAutomation{patternName}";
                    var patternType = assembly.GetType(patternTypeName) ?? throw new InvalidOperationException($"{patternTypeName} not found.");
                    var patternProps = patternType.GetProperties().Where(p => !p.Name.StartsWith("Cached")).ToArray();
                    dumpers.Add(element =>
                    {
                        var pattern = element.GetCurrentPattern(patternId);
                        if (pattern == null) return;

                        Debug.WriteLine($"# Pattern: {patternName}[{patternId}]");
                        foreach (var prop in patternProps)
                        {
                            try
                            {
                                Debug.WriteLine($"{prop.Name}: {prop.GetValue(pattern)}");
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine($"{prop.Name}: ERROR! {ex.Message}");
                            }
                        }
                    });
                }

                valueDumpers = dumpers;
            }

            foreach (var dumper in valueDumpers)
            {
                dumper(element);
            }

            return element;
        }

        private static List<Action<IUIAutomationElement>>? valueDumpers;
    }
}

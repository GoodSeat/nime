using GoodSeat.Nime.Windows;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using UIAutomationClient;

namespace nime
{
    internal class UIAutomation
    {
        public static void GetCaretPosition()
        {
            IntPtr hwnd = WindowInfo.ActiveWindowInfo.Handle;

            var automation = UIA.Instance;
            var element = automation.ElementFromHandle(hwnd);

            var tElement = GetTextElement(element, automation);
            if (tElement == null) return;

            var guid2 = typeof(IUIAutomationTextPattern2).GUID;
            var ptr = tElement.GetCurrentPatternAs(UIA_PatternIds.UIA_TextPattern2Id, ref guid2);

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
                        Debug.WriteLine(" caret is at " + caretPos);
                    }
                }
            }

        }

        private static IUIAutomationElement GetTextElement(IUIAutomationElement targetApp, CUIAutomation8Class automation)
        {
            targetApp.Dump();
            //targetApp.DumpPatterns();

            var textPatternCondition = automation.CreatePropertyCondition(UIA_PropertyIds.UIA_IsTextPattern2AvailablePropertyId, true);
            //var textPatternCondition = automation.CreatePropertyCondition(UIA_PropertyIds.UIA_IsTextPatternAvailablePropertyId, true);

            var targetTextElement = targetApp.FindFirst(TreeScope.TreeScope_Subtree, textPatternCondition);
            if (targetTextElement == null)
            {
                textPatternCondition = automation.CreatePropertyCondition(UIA_PropertyIds.UIA_IsTextPatternAvailablePropertyId, true);
                var ts = targetApp.FindAll(TreeScope.TreeScope_Subtree, textPatternCondition);

                var guid2 = typeof(IUIAutomationTextPattern).GUID;
                var guidL = typeof(IUIAutomationLegacyIAccessiblePattern).GUID;
                for (int i = 0; i < ts.Length; i++)
                {
                    var t = ts.GetElement(i);
                    var ptr = t.GetCurrentPatternAs(UIA_PatternIds.UIA_TextPatternId, ref guid2);
                    var pattern = (IUIAutomationTextPattern)Marshal.GetObjectForIUnknown(ptr);
                    if (pattern.SupportedTextSelection == SupportedTextSelection.SupportedTextSelection_None) continue;

                    // キャレットではなくテキスト範囲のIAccessibleになっているだろうな
                    var ptr2 = t.GetCurrentPatternAs(UIA_PatternIds.UIA_LegacyIAccessiblePatternId, ref guidL);
                    var patternL = (IUIAutomationLegacyIAccessiblePattern)Marshal.GetObjectForIUnknown(ptr2);

                }
            }
            return targetTextElement;
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
                    var patternTypeName = $"UIAutomationClient.IUIAutomation{patternName}";
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
            element.DumpPatterns();

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
                    var patternTypeName = $"UIAutomationClient.IUIAutomation{patternName}";
                    var patternType = assembly.GetType(patternTypeName) ?? throw new InvalidOperationException($"{patternTypeName} not found.");

                    var patternProps = patternType.GetProperties().Where(p => !p.Name.StartsWith("Cached")).ToArray();
                    dumpers.Add(element =>
                    {
                        var pattern = element.GetCurrentPattern(patternId);
                        if (pattern == null) return;

                        Debug.WriteLine($"     # Pattern: {patternName}[{patternId}]");
                        foreach (var prop in patternProps)
                        {
                            try
                            {
                                Debug.WriteLine($"       {prop.Name}: {prop.GetValue(pattern)}");
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine($"       {prop.Name}: ERROR! {ex.Message}");
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

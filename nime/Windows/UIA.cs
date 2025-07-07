using System;
using System.Diagnostics;
using System.Threading;
using System.Windows; // PointとRectのために PresentationCore.dll と WindowsBase.dll の参照が必要
using System.Windows.Automation;
using System.Windows.Automation.Text;

namespace GoodSeat.Nime.Windows
{
    public static class UIA
    {
        /// <summary>
        /// 現在フォーカスのあるUI要素からキャレットのスクリーン座標を取得しようと試みます。
        /// </summary>
        /// <param name="caretPoint">取得したキャレットの座標（左上の点）を格納する Point 構造体</param>
        /// <returns>座標の取得に成功した場合は true、それ以外は false</returns>
        public static bool TryGetCaretPosition(out System.Windows.Point caretPoint, out System.Windows.Size caretSize)
        {
            caretPoint = default;
            caretSize = default;

            try
            {
                // 1. フォーカスのあるUI要素を取得
                AutomationElement focusedElement = AutomationElement.FocusedElement;
                if (focusedElement == null)
                {
                    Debug.WriteLine("UIA: フォーカスのある要素が見つかりません。");
                    return false;
                }

                // 2. TextPatternをサポートしているか確認
                if (focusedElement.TryGetCurrentPattern(TextPattern.Pattern, out object patternProvider))
                {
                    var textPattern = (TextPattern)patternProvider;

                    // 3. 現在の選択範囲（キャレット位置）を取得
                    TextPatternRange[] selectionRanges = textPattern.GetSelection();
                    if (selectionRanges.Length == 0)
                    {
                        Debug.WriteLine("UIA: 選択範囲（キャレット）が見つかりません。");
                        return false;
                    }

                    // 4. 選択範囲からスクリーン座標の矩形（Rect）を取得
                    // 通常、キャレットは最初の範囲
                    System.Windows.Rect[] boundingCaretRects = selectionRanges[0].GetBoundingRectangles();
                    if (boundingCaretRects.Length == 0)
                    {
                        Debug.WriteLine("UIA: 座標の矩形が見つかりません。");
                        return false;
                    }

                    // 5. 矩形の左上の座標を返す
                    Rect caretRect = boundingCaretRects[0];
                    caretPoint = new System.Windows.Point(caretRect.Left, caretRect.Top);
                    caretSize = new System.Windows.Size(caretRect.Width, caretRect.Height);

                    Debug.WriteLine($"UIA: キャレット座標を取得しました: X={caretPoint.X}, Y={caretPoint.Y}, Height={caretRect.Height}");
                    return true;
                }
                else
                {
                    Debug.WriteLine("UIA: 対象の要素は TextPattern をサポートしていません。");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"UIA Error: {ex.Message}");
            }

            return false;
        }

        /// <summary>
        /// 現在のキャレット位置を基準に、前方(n)と後方(m)のテキストを選択します。
        /// </summary>
        /// <param name="charsBefore">キャレットの前方から選択する文字数 (n)</param>
        /// <param name="charsAfter">キャレットの後方から選択する文字数 (m)</param>
        /// <returns>削除操作の開始に成功した場合は true、それ以外は false</returns>
        public static bool SelectTextAroundCaret(int charsBefore, int charsAfter)
        {
            if (charsBefore < 0 || charsAfter < 0)
            {
                throw new ArgumentOutOfRangeException("文字数に負の値は指定できません。");
            }
            if (charsBefore == 0 && charsAfter == 0)
            {
                return true; // 何もせず成功
            }

            try
            {
                // 1. フォーカスのあるUI要素からTextPatternを取得
                AutomationElement focusedElement = AutomationElement.FocusedElement;
                if (focusedElement == null ||
                    !focusedElement.TryGetCurrentPattern(TextPattern.Pattern, out object patternProvider))
                {
                    Debug.WriteLine("UIA: TextPatternをサポートする要素にフォーカスしていません。");
                    return false;
                }

                var textPattern = (TextPattern)patternProvider;

                // 2. 現在のキャレット位置を取得
                TextPatternRange[] selectionRanges = textPattern.GetSelection();
                if (selectionRanges.Length == 0)
                {
                    Debug.WriteLine("UIA: キャレット位置を取得できません。");
                    return false;
                }
                TextPatternRange targetRange = selectionRanges[0];

                // 3. 範囲の開始点を後方へn文字、終了点を前方へm文字移動する
                // UIAは賢く、文書の先頭や末尾を超えて移動しようとしないため、
                // 文字数が足りなくてもエラーにならず、可能な範囲で最大限移動します。
                if (charsBefore > 0)
                {
                    targetRange.MoveEndpointByUnit(TextPatternRangeEndpoint.Start, TextUnit.Character, -charsBefore);
                }
                if (charsAfter > 0)
                {
                    targetRange.MoveEndpointByUnit(TextPatternRangeEndpoint.End, TextUnit.Character, charsAfter);
                }

                // 4. 作成した範囲を選択状態にする
                targetRange.Select();

                // 選択がUIに反映されるまでごくわずかに待機
                Thread.Sleep(10);

                Debug.WriteLine($"UIA: キャレットの前方{charsBefore}文字と後方{charsAfter}文字を選択しました。");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"UIA Error: {ex.Message}");
                return false;
            }
        }
    }

}

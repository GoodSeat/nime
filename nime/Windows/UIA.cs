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
        /// 現在フォーカスのあるUI要素に、指定されたテキストを送信します。
        /// </summary>
        /// <param name="text">送信する文字列</param>
        /// <returns>テキストの送信に成功した場合は true、それ以外は false</returns>
        public static bool SendText(string text)
        {
            try
            {
                // 1. 現在キーボードフォーカスを持っているUI要素を取得
                AutomationElement focusedElement = AutomationElement.FocusedElement;
                if (focusedElement == null)
                {
                    Debug.WriteLine("UIA: フォーカスのある要素が見つかりません。");
                    return false;
                }

                // --- 方法A: TextPattern (高機能なエディタ向け) を試す ---
                // TextPatternはカーソル位置への挿入など、より高度な操作をサポートする
                if (focusedElement.TryGetCurrentPattern(TextPattern.Pattern, out object textPatternProvider))
                {
                    var textPattern = (TextPattern)textPatternProvider;

                    // GetSupportedTextSelectionは常にSupportedTextSelection.Singleを返す必要がある
                    if (textPattern.SupportedTextSelection == SupportedTextSelection.Single)
                    {
                        // 現在のカーソル位置（または選択範囲）を取得
                        var selection = textPattern.GetSelection();
                        if (selection.Length > 0)
                        {
                            // 選択範囲にテキストを挿入（元のテキストは上書きされる）
                            var range = selection[0];
                            // この方法はUIAの仕様上サポートされていないことが多い
                            // 代わりにValuePatternを使うのが一般的
                        }
                    }

                    // TextPatternは直接のテキスト挿入より、ValuePatternへのフォールバックが現実的
                    // ここではValuePatternを優先的に試すロジックに切り替える
                }


                // --- 方法B: ValuePattern (一般的なテキストボックス向け) を試す ---
                // こちらのほうがより多くのコントロールでサポートされている
                if (focusedElement.TryGetCurrentPattern(ValuePattern.Pattern, out object valuePatternProvider))
                {
                    var valuePattern = (ValuePattern)valuePatternProvider;

                    // 読み取り専用でないことを確認
                    if (!valuePattern.Current.IsReadOnly)
                    {
                        Debug.WriteLine($"UIA: ValuePattern を使用して '{text}' を送信します。");
                        valuePattern.SetValue(text);
                        return true;
                    }
                    else
                    {
                        Debug.WriteLine("UIA: ValuePattern は読み取り専用です。");
                    }
                }

                Debug.WriteLine("UIA: 対象の要素は ValuePattern も TextPattern もサポートしていません。");
                return false;
            }
            catch (ElementNotAvailableException)
            {
                // 対象のUI要素が既になくなっている場合など
                Debug.WriteLine("UIA Error: 対象の要素が利用できません。");
                return false;
            }
            catch (Exception ex)
            {
                // その他の予期せぬエラー
                Debug.WriteLine($"UIA Error: {ex.Message}");
                return false;
            }
        }
        
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
    }
}

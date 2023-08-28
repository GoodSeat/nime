using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodSeat.Nime.Core
{
    /// <summary>
    /// 入力中の文を表します。
    /// </summary>
    internal class SentenceOnInput
    {
        /// <summary>
        /// 現在のキャレット座標を通知します。
        /// </summary>
        /// <param name="ptCaret">キャレット座標。</param>
        public void NotifyCurrentCaretCoordinate(Point ptCaret)
        {
            _caretCoordLastNotified = ptCaret;
            Debug.WriteLine($"NotifyCurrentCaretCoordinate:{ptCaret.X}, {ptCaret.Y}");

            if (_caretCoordWhenInputStart.HasValue)
            {
                if (!_caretCoordWhenMostRight.HasValue || _caretCoordWhenMostRight.Value.X < ptCaret.X)
                {
                    _caretCoordWhenMostRight = ptCaret;
                }
            }
        }

        Point? _caretCoordLastNotified;
        Point? _caretCoordWhenInputStart;
        Point? _caretCoordWhenMostRight;


        /// <summary>
        /// キャレット位置を先頭に移動します。但し、通知されている現在のキャレット座標が、ホーム位置として適切でない場合にはfalseを返します。
        /// </summary>
        /// <param name="ptCaret">現在のキャレット位置。</param>
        /// <returns>有効なホーム位置として認識されたか。</returns>
        public bool TryMoveCaretPositionAsPostHomeKey(Point ptCaret)
        {
            if (!_caretCoordWhenInputStart.HasValue) return false;
            if (ptCaret.X != _caretCoordWhenInputStart.Value.X) return false;

            CaretPosition = 0;
            return true;
        }

        /// <summary>
        /// キャレット位置を末尾に移動します。但し、通知されている現在のキャレット座標が、末尾位置として適切でない場合にはfalseを返します。
        /// </summary>
        /// <param name="ptCaret">現在のキャレット位置。</param>
        /// <returns>有効な末尾位置として認識されたか。</returns>
        public bool TryMoveCaretPositionAsPostEndKey(Point ptCaret)
        {
            if (!_caretCoordWhenMostRight.HasValue) return false;
            if (ptCaret.X != _caretCoordWhenMostRight.Value.X) return false;

            CaretPosition = Text.Length;
            return true;
        }

        /// <summary>
        /// 指定テキストを文に追加します。
        /// </summary>
        /// <param name="text"></param>
        public void InputText(string text)
        {
            if (string.IsNullOrEmpty(Text)) _caretCoordWhenInputStart = _caretCoordLastNotified;

            if (CaretPosition == Text.Length)
            {
                Text += text;
            }
            else
            {
                Text = Text.Substring(0, CaretPosition) + text + Text.Substring(CaretPosition);
            }
            CaretPosition += text.Length;
        }

        /// <summary>
        /// 入力されているテキストを取得します。
        /// </summary>
        public string Text { get; private set; } = string.Empty;

        /// <summary>
        /// キャレット位置を取得します。
        /// </summary>
        public int CaretPosition { get; private set; }

        /// <summary>
        /// キャレット位置の次の文字を削除します。現在の状態でこの操作が有効でない場合にはfalseを返します。
        /// </summary>
        /// <returns>この操作が有効であるか否か。</returns>
        public bool TryDelete()
        {
            if (CaretPosition >= Text.Length) return false;

            var txt = Text;
            try
            {
                Text = txt.Substring(0, CaretPosition) + txt.Substring(CaretPosition + 1);
            }
            catch
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// キャレット位置の前の文字を削除します。現在の状態でこの操作が有効でない場合にはfalseを返します。
        /// </summary>
        /// <returns>この操作が有効であるか否か。</returns>
        public bool TryBackspace()
        {
            if (CaretPosition <= 0) return false;

            var txt = Text;
            try
            {
                Text = txt.Substring(0, CaretPosition - 1) + txt.Substring(CaretPosition);
            }
            catch
            {
                return false;
            }
            CaretPosition--;
            return true;
        }

        /// <summary>
        /// キャレット位置を左に移動します。現在の状態でこの操作が有効でない場合にはfalseを返します。
        /// </summary>
        /// <returns>この操作が有効であるか否か。</returns>
        public bool TryMoveLeft()
        {
            if (Text.Length > 0) CaretPosition--;
            return (CaretPosition >= 0);
        }

        /// <summary>
        /// キャレット位置を右に移動します。現在の状態でこの操作が有効でない場合にはfalseを返します。
        /// </summary>
        /// <returns>この操作が有効であるか否か。</returns>
        public bool TryMoveRight()
        {
            if (Text.Length > 0) CaretPosition++;
            return (CaretPosition <= Text.Length);
        }

        /// <summary>
        /// 入力状態を空の状態にリセットします。
        /// </summary>
        public void Reset()
        {
            Text = string.Empty;
            CaretPosition = 0;

            _caretCoordLastNotified = null;
            _caretCoordWhenInputStart = null;
            _caretCoordWhenMostRight = null;

        }
    }
}

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
            if (_caretCoordCache.ContainsKey(ptCaret)) _caretCoordCache.Remove(ptCaret);
            _caretCoordCache.Add(ptCaret, CaretPosition);
        }

        Dictionary<Point, int> _caretCoordCache = new Dictionary<Point, int>();

        /// <summary>
        /// キャレット位置を文頭もしくは文末に移動します。但し、通知されている現在のキャレット座標が、位置として適切でない場合にはfalseを返します。
        /// </summary>
        /// <param name="ptCaret">現在のキャレット位置。</param>
        /// <returns>有効な位置として認識されたか。</returns>
        public bool TryMoveCaretPositionAsPostHomeOrEndKey(Point ptCaret)
        {
            if (ptCaret.X == 0) return false;
            if (!_caretCoordCache.TryGetValue(ptCaret, out int pos)) return false;

            CaretPosition = pos;
            return true;
        }

        /// <summary>
        /// 指定テキストを文に追加します。
        /// </summary>
        /// <param name="text"></param>
        public void InputText(string text)
        {
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

            _caretCoordCache.Clear();
        }

    }
}

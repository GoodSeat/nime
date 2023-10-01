using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodSeat.Nime.Windows
{
    public static class WindowMessages
    {
        // 特に意味はありません。特定のウインドウにこのメッセージを投げてタイムアウトするかどうかで生存確認を行う事ができます。
        public const uint WM_NULL = 0x0000;
        // ウインドウが作成されていることを示します。
        public const uint WM_CREATE = 0x0001;
        // ウインドウが破棄されようとしていることを示します。
        public const uint WM_DESTROY = 0x0002;
        // ウインドウの位置が変更されたことを示します。
        public const uint WM_MOVE = 0x0003;
        // ウインドウのサイズが変更されていることを示します。
        public const uint WM_SIZE = 0x0005;
        // アクティブ状態が変更されていることを示します。
        public const uint WM_ACTIVATE = 0x0006;
        // ウインドウがキーボード・フォーカスを取得したことを示します。
        public const uint WM_SETFOCUS = 0x0007;
        // ウインドウがキーボード・フォーカスを失っていることを示します。
        public const uint WM_KILLFOCUS = 0x0008;
        // ウインドウの有効または無効の状態が変更されていることを示します。
        public const uint WM_ENABLE = 0x000A;
        // ウインドウ内の再描画を許可または禁止します。
        public const uint WM_SETREDRAW = 0x000B;
        // ウインドウのテキストを設定します。
        public const uint WM_SETTEXT = 0x000C;
        // ウインドウに対応するテキストを取得します。
        public const uint WM_GETTEXT = 0x000D;
        // ウインドウに関連付けられているテキストの長さを取得します。
        public const uint WM_GETTEXTLENGTH = 0x000E;
        // ウインドウのクライアント領域を描画する必要があることを示します。
        public const uint WM_PAINT = 0x000F;
        // コントロール・メニューの[クローズ]コマンドが選ばれました。
        public const uint WM_CLOSE = 0x0010;
        // Windowsセッションを終了するよう要求します。
        public const uint WM_QUERYENDSESSION = 0x0011;
        // アプリケーションを強制終了するよう要求します。
        public const uint WM_QUIT = 0x0012;
        // アイコン化ウインドウを復元するよう要求します。
        public const uint WM_QUERYOPEN = 0x0013;
        // ウインドウの背景を消去する必要があることを示します。
        public const uint WM_ERASEBKGND = 0x0014;
        // システム・カラーの値が変更されたことを示します。
        public const uint WM_SYSCOLORCHANGE = 0x0015;
        // Windowsセッションが終了することを示します。
        public const uint WM_ENDSESSION = 0x0016;
        // (Win32ではもはや用いられません)
        public const uint WM_SYSTEMERROR = 0x0017;
        // ウインドウの表示または非表示の状態が変更されようとしていることを示します。
        public const uint WM_SHOWWINDOW = 0x0018;
        // 子コントロールが描画される直前であることを示します。
        public const uint WM_CTLCOLOR = 0x0019;
        // WIN.INIが変更されたことをアプリケーションに通知します。Windowsの設定が変更されたことをアプリケーションに通知します。
        public const uint WM_WININICHANGE = 0x001A;
        // デバイス モードの設定が変更されたことを示します。
        public const uint WM_DEVMODECHANGE = 0x001B;
        // 新しいタスクがアクティブになるタイミングをアプリケーションに通知します。
        public const uint WM_ACTIVATEAPP = 0x001C;
        // フォント リソース プールが変更されていることを示します。
        public const uint WM_FONTCHANGE = 0x001D;
        // システム時刻が設定されたことを示します。
        public const uint WM_TIMECHANGE = 0x001E;
        // 内部モードをキャンセルするようウインドウに通知します。
        public const uint WM_CANCELMODE = 0x001F;
        // マウス カーソルの形状を設定するようウインドウに促します。
        public const uint WM_SETCURSOR = 0x0020;
        // 非アクティブ ウインドウ内でマウスがクリックされたことを示します。
        public const uint WM_MOUSEACTIVATE = 0x0021;
        // 子ウインドウにアクティブであることを通知します。
        public const uint WM_CHILDACTIVATE = 0x0022;
        // CBTメッセージを区切ります。
        public const uint WM_QUEUESYNC = 0x0023;
        // アイコン表示時および最大表示時のサイズ情報を取得します。
        public const uint WM_GETMINMAXINFO = 0x0024;
        // アイコンが描画されようとしています。
        public const uint WM_PAINTICON = 0x0026;
        // アイコンの背景を塗りつぶすようアイコン化ウインドウに通知します。
        public const uint WM_ICONERASEBKGND = 0x0027;
        // フォーカスを別のダイアログ ボックス コントロールに設定します。
        public const uint WM_NEXTDLGCTL = 0x0028;
        // 印刷ジョブが追加または削除されたことを示します。(XP 以降ではサポートされません)
        public const uint WM_SPOOLERSTATUS = 0x002A;
        // オーナー描画コントロールまたはオーナー描画メニューを再描画する必要があることを示します。
        public const uint WM_DRAWITEM = 0x002B;
        // オーナー描画のコントロールまたは項目の寸法を要求します。
        public const uint WM_MEASUREITEM = 0x002C;
        // ほかのオーナー描画項目またはオーナー描画コントロールに代わったことを示します。
        public const uint WM_DELETEITEM = 0x002D;
        // リスト ボックスのキーストロークをそのオーナー ウインドウに提供します。
        public const uint WM_VKEYTOITEM = 0x002E;
        // リスト ボックスのキーストロークをそのオーナー ウインドウに提供します。
        public const uint WM_CHARTOITEM = 0x002F;
        // コントロールで使われるフォントを設定します。
        public const uint WM_SETFONT = 0x0030;
        // コントロールで使われているフォントを取得します。
        public const uint WM_GETFONT = 0x0031;
        // ウインドウにホット キーを関連付けます。
        public const uint WM_SETHOTKEY = 0x0032;
        // ウインドウのホット キーの仮想キー コードを取得します。
        public const uint WM_GETHOTKEY = 0x0033;
        // アイコン化ウインドウに対してマウス カーソルのハンドルを要求します。
        public const uint WM_QUERYDRAGICON = 0x0037;
        // コンボ ボックスまたはリスト ボックスの項目位置を判断します。
        public const uint WM_COMPAREITEM = 0x0039;
        // 
        public const uint WM_GETOBJECT = 0x003D;
        // メモリ不足状態であることを示します。
        public const uint WM_COMPACTING = 0x0041;
        // (Win32 ではもはや用いられません)
        public const uint WM_COMMNOTIFY = 0x0044;
        // ウインドウに新しいサイズまたは位置を通知します。
        public const uint WM_WINDOWPOSCHANGING = 0x0046;
        // ウインドウにサイズまたは位置の変更を通知します。
        public const uint WM_WINDOWPOSCHANGED = 0x0047;
        // システムが中断モードに入っていることを示します。
        public const uint WM_POWER = 0x0048;
        // ほかのアプリケーションにデータを渡します。
        public const uint WM_COPYDATA = 0x004A;
        // ユーザーがジャーナル モードをキャンセルしました。
        public const uint WM_CANCELJOURNAL = 0x004B;
        // 
        public const uint WM_NOTIFY = 0x004E;
        // 
        public const uint WM_INPUTLANGCHANGEREQUEST = 0x0050;
        // 
        public const uint WM_INPUTLANGCHANGE = 0x0051;
        // Windows XP	
        public const uint WM_TCARD = 0x0052;
        // Windows XP	
        public const uint WM_HELP = 0x0053;
        // Windows XP	ユーザがログオン/ログオフしたことを示します。
        public const uint WM_USERCHANGED = 0x0054;
        // 
        public const uint WM_NOTIFYFORMAT = 0x0055;
        // 
        public const uint WM_CONTEXTMENU = 0x007B;
        // SetWindowLong() によってウインドウのスタイルが変更されようとしています。
        public const uint WM_STYLECHANGING = 0x007C;
        // SetWindowLong() によってウインドウのスタイルが変更されました。
        public const uint WM_STYLECHANGED = 0x007D;
        // ディスプレイの解像度が変更されたことを示します。
        public const uint WM_DISPLAYCHANGE = 0x007E;
        // 
        public const uint WM_GETICON = 0x007F;
        // 
        public const uint WM_SETICON = 0x0080;
        // ウインドウの非クライアント領域が作成されていることを示します。
        public const uint WM_NCCREATE = 0x0081;
        // ウインドウの非クライアント領域が破棄されていることを示します。
        public const uint WM_NCDESTROY = 0x0082;
        // ウインドウのクライアント領域のサイズを計算します。
        public const uint WM_NCCALCSIZE = 0x0083;
        // マウス カーソルが移動したことを示します。
        public const uint WM_NCHITTEST = 0x0084;
        // ウインドウの枠を描画する必要があることを示します。
        public const uint WM_NCPAINT = 0x0085;
        // 非クライアント領域のアクティブ状態を変更します。
        public const uint WM_NCACTIVATE = 0x0086;
        // ダイアログ プロシージャがコントロール入力を処理できるようにします。
        public const uint WM_GETDLGCODE = 0x0087;
        // 非クライアント領域でマウス カーソルが移動したことを示します。
        public const uint WM_NCMOUSEMOVE = 0x00A0;
        // 非クライアント領域でマウスの左ボタンが押されたことを示します。
        public const uint WM_NCLBUTTONDOWN = 0x00A1;
        // 非クライアント領域でマウスの左ボタンが離されたことを示します。
        public const uint WM_NCLBUTTONUP = 0x00A2;
        // 非クライアント領域でマウスの左ボタンをダブルクリックしたことを示します。
        public const uint WM_NCLBUTTONDBLCLK = 0x00A3;
        // 非クライアント領域でマウスの右ボタンが押されたことを示します。
        public const uint WM_NCRBUTTONDOWN = 0x00A4;
        // 非クライアント領域でマウスの右ボタンが離されたことを示します。
        public const uint WM_NCRBUTTONUP = 0x00A5;
        // 非クライアント領域でマウスの右ボタンをダブルクリックしたことを示します。
        public const uint WM_NCRBUTTONDBLCLK = 0x00A6;
        // 非クライアント領域でマウスの中央ボタンが押されたことを示します。
        public const uint WM_NCMBUTTONDOWN = 0x00A7;
        // 非クライアント領域でマウスの中央ボタンが離されたことを示します。
        public const uint WM_NCMBUTTONUP = 0x00A8;
        // 非クライアント領域でマウスの中央ボタンをダブルクリックしたことを示します。
        public const uint WM_NCMBUTTONDBLCLK = 0x00A9;
        // 非クライアント領域でマウスの 4 つ目以降のボタンが押されたことを示します。
        public const uint WM_NCXBUTTONDOWN = 0x00AB;
        // 非クライアント領域でマウスの 4 つ目以降のボタンが離されたことを示します。
        public const uint WM_NCXBUTTONUP = 0x00AC;
        // 非クライアント領域でマウスの 4 つ目以降のボタンをダブルクリックしたことを示します。
        public const uint WM_NCXBUTTONDBLCLK = 0x00AD;
        // Windows Vista	
        public const uint WM_INPUT_DEVICE_CHANGE = 0x00FE;
        // Windows XP	RAW Input Device (キーボード/マウス/リモコン等) からの入力があったことを示します。
        public const uint WM_INPUT = 0x00FF;

    }
}

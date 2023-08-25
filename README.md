# nime

nime is Not IME.


## 

nimeは日本語入力用ツールです。
IMEがオフの状態でro-majidekorekore




## 動作確認を行ったアプリケーション

### 正常に動作していそう
以下のアプリケーションにおいては、正常に利用できることを確認しています。

* Microsoft Edge
* mozzila firefox
* Google Chrome

* Microsoft word
* visual studio 2022
* メモ帳

* Adobe Acrobat

### 一部問題あり
以下のアプリケーションにおいては、nimeにて予め設定を行う必要があったり、一部機能が利用できない等の制限がありますが、概ね問題なく利用することが可能です。

#### 入力表示がキャレット位置にでない

* Terminal
* gVim
* Inkscape


#### 文字数×BSで消すように設定が必要
文字変換時の動作が少々もたつきます。

* Terminal
* gVim
* excel(F2での編集時は問題なし)


#### nime上での変換編集時にIMEを用いた直接編集をすることができない

* Explorer(名前変更時、検索ボックス、アドレスバー利用時、変換時にIME利用による直接指定ができない)


### 正しく動作しない
現在のバージョンでは、まともに日本語が入力されません。

* NVim(正しい文字列が入力されない)


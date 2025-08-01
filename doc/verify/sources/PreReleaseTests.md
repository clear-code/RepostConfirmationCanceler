# ThinBridge リリース前検証手順

## 検証環境の用意

* Windows 11
* Google Chrome、Microsoft Edgeをインストール済みである。
* Google Chrome、Microsoft EdgeのGPO用ポリシーテンプレートを導入済みである。
* `/webextensions/` の内容を配置済みである。

準備は以下の手順で行う。

1. RepostConfirmationCancelerの最新のインストーラ `RepostConfirmationCanceler.exe` をダウンロードし、実行、インストールする。
2. Edgeアドオンの開発版パッケージを用意し、インストールするための設定を行う。
   1. Edgeを起動する。
   2. アドオンの管理画面（`edge://extensions`）を開く。
   3. `開発者モード` を有効化する。
   4. `拡張機能のパック` で `webextensions\edge` をパックする。（1つ上のディレクトリーに `edge.crx` と `edge.pem` が作られる）
   5. `edge.crx` をEdgeのアドオン管理画面にドラッグ＆ドロップし、インストールして、IDを控える。
      例：`oapdkmbdgdcjpacbjpcdfhncifimimcj`
   6. `C:\Program Files\RepostConfirmationCanceler\RepostConfirmationCancelerHost\edge.json` のアクセス権を変更し、ユーザー権限での書き込みを許可した上で、`"allowed_origins"` に、先ほど控えたIDに基づくURLを追加する。
      例：`"chrome-extension://oapdkmbdgdcjpacbjpcdfhncifimimcj/"`

## 検証

### すべてのURLを対象にした時の動作確認（警告ダイアログ無）

#### 準備

以下の通り設定して検証を行う。

* [doc\verify\sources\TestTools/Scenarios/scenario1.ini](../TestTools/Scenarios/scenario1.ini) を `C:\Program Files\RepostConfirmationCanceler\RepostConfirmationCanceler.ini` に配置する。
* 念のためEdgeを再起動する

#### 検証

* https://www.google.com/ を開く
  * すべてのURLを対象としている場合も、http/httpsのサイトを開いていないとアドインが動作しないため
* `doc\verify\sources\TestTools\form.html` を開く
* フォームに「test」と入力して、「送信」ボタンを押す
* 「form.html」 をリロードする
  * [ ] 「フォームを再送信しますか？」ダイアログ一瞬表示され、キャンセルされること
* この状態で、2分程待機する（時間経過で拡張機能が停止しないことの確認）
* 「form.html」 をリロードする
  * [ ] 「フォームを再送信しますか？」ダイアログ一瞬表示され、キャンセルされること

### すべてのURLを対象にした時の動作確認（警告ダイアログ無）

#### 準備

以下の通り設定して検証を行う。

* [doc\verify\sources\TestTools/Scenarios/scenario2.ini](../TestTools/Scenarios/scenario2.ini) を `C:\Program Files\RepostConfirmationCanceler\RepostConfirmationCanceler.ini` に配置する。
  * これは、「https://example.com」を開いている場合は
* 念のためEdgeを再起動する

#### 検証

* https://www.google.com/ を開く
  * すべてのURLを対象としている場合も、http/httpsのサイトを開いていないとアドインが動作しないため
* `doc\verify\sources\TestTools\form.html` を開く
* フォームに「test」と入力して、「送信」ボタンを押す
* 「form.html」 をリロードする
  * [ ] 「フォームを再送信しますか？」ダイアログ一瞬表示され、キャンセルされること
  * [ ] 「フォームの再送信が発生するため、このサイトでのリロードは禁止されています。\n\nリロードはキャンセルされました。」という警告ダイアログが表示されること
  * [ ] 警告ダイアログが前面に表示されること（Edgeの後ろに隠れないこと）
* 警告ダイアログをOKで閉じる
* 「form.html」 をリロードする
* 「フォームの再送信が発生するため、このサイトでのリロードは禁止されています。\n\nリロードはキャンセルされました。」という警告ダイアログが表示された状態で、再度「form.html」 をリロードする
* 「form.html」 をリロードする
  * [ ] 「フォームを再送信しますか？」ダイアログ一瞬表示され、キャンセルされること
  * [ ] 「フォームの再送信が発生するため、このサイトでのリロードは禁止されています。\n\nリロードはキャンセルされました。」という警告ダイアログが表示されること
  * [ ] 警告ダイアログが前面に表示されること（Edgeの後ろに隠れないこと）
* すべての警告ダイアログをOKで閉じる

### 特定のURLを対象にした時の動作確認（警告ダイアログ無）

#### 補足

本アドオンは、現在のタブが指定したURLを開いていなくても、いずれかのタブで指定したURLが開いている場合に動作する。
これは、ネイティブアプリ側で現在開いているタブを判定するのが難しいためである。

#### 準備

以下の通り設定して検証を行う。

* [doc\verify\sources\TestTools/Scenarios/scenario3.ini](../TestTools/Scenarios/scenario3.ini) を `C:\Program Files\RepostConfirmationCanceler\RepostConfirmationCanceler.ini` に配置する。
  * 設定の内容は以下の通り
    * 以下のサイトを対象とする
      * `*://example.com/jp*`
      * `*://example.com/us/??/`
      * `https://www.clear-code.com/`
    * 以下のサイトを除外する
      * `*://example.com/jp/exclude*`
* 念のためEdgeを再起動する

#### 検証

* `https://example.com/jp/exclude` を開く
* `doc\verify\sources\TestTools\form.html` を開く
* フォームに「test」と入力して、「送信」ボタンを押す
* 「form.html」 をリロードする
  * [ ] 「フォームを再送信しますか？」ダイアログが表示され、キャンセル**されない**こと
* 「フォームを再送信しますか？」ダイアログをキャンセルで閉じる
* `https://example.com/jp/` を開く
* 「form.html」 をリロードする
  * [ ] 「フォームを再送信しますか？」ダイアログ一瞬表示され、キャンセルされること
* Edgeを終了する
* タスクマネージャーを起動する
* 詳細タブを開く
* RepostConfirmationCanceler.exeが起動していれば終了する
* Edgeを起動する
* `https://example.com/us/ab/` を開く
* `doc\verify\sources\TestTools\form.html` を開く
* フォームに「test」と入力して、「送信」ボタンを押す
* 「form.html」 をリロードする
  * [ ] 「フォームを再送信しますか？」ダイアログ一瞬表示され、キャンセルされること
* Edgeを終了する
* タスクマネージャーを起動する
* 詳細タブを開く
* RepostConfirmationCanceler.exeが起動していれば終了する
* Edgeを起動する
* `https://www.clear-code.com/` を開く
* `doc\verify\sources\TestTools\form.html` を開く
* フォームに「test」と入力して、「送信」ボタンを押す
* 「form.html」 をリロードする
  * [ ] 「フォームを再送信しますか？」ダイアログ一瞬表示され、キャンセルされること
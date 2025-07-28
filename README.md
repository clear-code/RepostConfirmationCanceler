# RepostConfirmationCanceler

## 概要

特定のサイトを開いている際に、「フォームを再送信しますか？」ダイアログが表示されたら、自動でキャンセルするEdge向け拡張機能

## ビルド方法

* Visual Studio 2022
  * .NET Framework 4.6.2 SDKをインストール
    * RepostConfirmationCancelerがC#の.NET Framework 4.6.2で実装されているため
    * 以下のファイルを参照しているので、特に以下のファイルが存在していることを確認する
      * `C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.6.2\UIAutomationClient.dll`
      * `C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.6.2\UIAutomationTypes.dll`
  * MSVC v143のインストール
    * RepostConfirmationCancelerTalkがC++で実装されているため
  * Windows SDK 10.0の最新版のインストール
* Inno Setup 6.3.3以上をインストールする

### ネイティブアプリのインストーラー作成方法

1. RepostConfirmationCanceler.sln をVisual Studio 2022で開く
2. 構成Release、プラットフォームx86でソリューションのビルドを実行する
3. RepostConfirmationCancelerX64.iss をInno Setupで開く
4. Build -> Compileからインストーラーをコンパイルする
5. SetupOutput配下にネイティブアプリのインストーラーが作成される

## 検証環境での動作確認

* 作成したネイティブアプリのインストーラをインストールする
* Edgeを起動する
* Edgeでedge://extensions/を開く
* 「開発者モード」をONにする
* 「展開して読み込み」を選択する
* webextensions\edgeを読み込んで、IDを控える。
   例：`gfmemaifchppdpchjijkahhngfnjdihm`
* `C:\Program Files\RepostConfirmationCanceler\RepostConfirmationCancelerHost\edge.json` のアクセス権を変更し、ユーザー権限での書き込みを許可した上で、`"allowed_origins"` に、先ほど控えたIDに基づくURLを追加する。
   例：`"chrome-extension://gfmemaifchppdpchjijkahhngfnjdihm/"`
* （必要に応じて: 「サービスワーカー」をクリックし、DevToolsを起動する。当画面で逐次状況を観察しながらテストする。）
* `C:\Program Files\RepostConfirmationCanceler\RepostConfirmationCanceler.ini` が存在しなければ作成する
  * 書き込み権限がない場合、コマンドプロンプトを管理者権限で起動して作成する
* `C:\Program Files\RepostConfirmationCanceler\RepostConfirmationCanceler.ini` のアクセス権を変更し、ユーザー権限での書き込みを許可した上で、以下のような内容を記載する
  * ```
    [GLOBAL]
    @DISABLED
    @TOP_PAGE_ONLY

    [Edge]
    @TOP_PAGE_ONLY
    *
    ```

## 動作解説

* ブラウザの拡張機能 RepostConfirmationCanceler が、`RepostConfirmationCanceler.ini`の`[Edge]`セクションに記載されているURLを開いているかを判定する
  * サイトを開いたタイミングと、30秒ごとのポーリングで判定を行う
* 指定のURLを開いていた場合、ブラウザの拡張機能が RepostConfirmationCancelerTalk.exe を呼び出す。
  * このとき、`Q Edge`というメッセージを送信する
* `Q Edge`メッセージを受け取ったRepostConfirmationCancelerTalk.exeは、RepostConfirmationCanceler.exeを起動する
* RepostConfirmationCanceler.exeは、Edgeのウィンドウを1秒ごとに監視し、「フォームを再送信しますか？」ダイアログが表示されたら、自動でキャンセルする
  * RepostConfirmationCanceler.exeは初回起動後1分間常駐する
* 拡張機能の次のポーリングのタイミングでまだサイトを開いていた場合、改めてブラウザの拡張機能が RepostConfirmationCancelerTalk.exe を呼び出す
  * このとき、最初と同様に`Q Edge`というメッセージを送信する
* `Q Edge`メッセージを受け取ったRepostConfirmationCancelerTalk.exeは、RepostConfirmationCanceler.exeを起動する
  * もし、既にRepostConfirmationCanceler.exeが起動していた場合、既存のRepostConfirmationCanceler.exeに`keep-alive`メッセージを送信し、今回のRepostConfirmationCanceler.exeは直ちに終了する
  * `keep-alive`メッセージを受信した既存のRepostConfirmationCanceler.exeは、メッセージ受信から1分後にまで常駐時間を延ばす
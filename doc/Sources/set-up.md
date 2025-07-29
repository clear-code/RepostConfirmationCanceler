---
CJKmainfont: Noto Sans CJK JP
CJKoptions:
  - BoldFont=Noto Sans CJK JP Bold
titlepage-logo: ./set-up/media/image1.png
title: |
  RepostConfirmationCanceler  
  セットアップ手順
subject: RepostConfirmationCancelerセットアップ手順
date: 2025/09/26
author: 株式会社クリアコード
keywords: [RepostConfirmationCanceler, Set up]
titlepage: true
toc-title: 目次
toc-own-page: true
---

更新履歴

| 日付       | Version | 備考                              |
|------------|---------|-----------------------------------|
| 2025/09/26 | 1.0.0    | 第1版                             |

**本書について**

本書は、株式会社クリアコードが、RepostConfirmationCancelerを御利用いただく管理者向けに作成した資料となります。2025年9月時点のデータにより作成されており、それ以降の状況の変動によっては、本書の内容と事実が異なる場合があります。また、本書の内容に基づく運用結果については責任を負いかねますので、予めご了承下さい。

本書で使用するシステム名、製品名は、それぞれの各社の商標、または登録商標です。なお、本文中ではTM、®、©マークは省略しています。

\newpage
# システム要件

## 概要

RepostConfirmationCancelerが対応しているWindowsシステムについて、記述します。

RepostConfirmationCancelerを利用するために、別途ランタイムライブラリーや追加インストールが必要なコンポーネントは、ありません。

## 動作サポートOS

**クライアント系OS**

- **Windows 11 (64bit)**
- **Windows 10 (64bit)**

## 動作サポート ブラウザー

- **Microsoft Edge (最新版) ■レガシーEdgeは非対応**

\newpage
# インストール手順

## RepostConfirmationCancelerインストールについて

RepostConfirmationCancelerを利用するために、別途ランタイムライブラリー(.NET Framework等)の追加インストールは必要ありません。

## インストール方法

**RepostConfirmationCancelerのセットアップ用のインストーラーは2種類あります。**  
**御利用されるWindows環境に合わせてセットアップファイルを選択してください。**

・Windows 64bit(x64)環境用  
**RepostConfirmationCancelerSetup_x64.exe**

・Windows 32bit(x86)環境用  
**RepostConfirmationCancelerSetup_x86.exe**

Windows環境に合っていないセットアップファイルを実行すると以下のメッセージが表示されます。
![](set-up/media/image3.png)

> このプログラムは  
> x86プロセッサー向けの Windows にしかインストールできません。

対処方法：RepostConfirmationCancelerSetup_x64.exeを利用してください。

![](set-up/media/image4.png)

> このプログラムは  
> x64プロセッサー向けの Windows にしかインストールできません。

対処方法：RepostConfirmationCancelerSetup_x86.exeを利用してください。

**■Windows 11(64bit)、Windows 10(64bit)環境のセットアップ例**

（1） RepostConfirmationCancelerSetup_x64.exeを実行します。

![](set-up/media/image5.png)  
■管理者権限で実行してください。

（2） 「次へ」ボタンをクリックします。

![](set-up/media/image6.png)  
■インストール先を変更する場合は、「参照」ボタンよりインストール先を変更します。

（3） 「次へ」ボタンをクリックします。

![](set-up/media/image7.png)

（4） 「インストール」ボタンをクリックします。

![](set-up/media/image8.png)

（5） 「完了」ボタンをクリックします。

![](set-up/media/image9.png)

以上で、インストール作業は完了です。

\newpage
# アンインストール手順

## アンインストール方法 

（1）コントロールパネルより「プログラムと機能」を表示します。  
**■管理者権限で実行してください。**

![](set-up/media/image10.png)

（2）一覧より「RepostConfirmationCanceler」を選択しダブルクリックします。

![](set-up/media/image10.png)

（3）「はい」ボタンをクリックします。

![](set-up/media/image11.png)


（4）アンインストールが完了するとメッセージが表示されます。  
[OK]をクリックします。

![](set-up/media/image12.png)

（5）`<インストール先>\RepostConfirmationCanceler`フォルダーを削除します。

![](set-up/media/image13.png)

以上でアンインストールは、完了です。

\newpage
# サイレント インストール手順（自動インストール）

## RepostConfirmationCancelerサイレント インストールについて

RepostConfirmationCancelerのインストールをサイレント実行するための手順について記載します。

以下の環境用のSetupではサイレント インストールが可能です。  
**■管理者権限での実行が必要です。**

1.  応答ファイルの作成  
**■デフォルト値でのサイレントインストールの場合は応答ファイルの作成は不要です。**

2.  サイレント インストールによるSetupの実行.

## 応答ファイルの作成

（1）メモ帳を起動し`RepostConfirmationCanceler.inf`ファイルを作成します。

![](set-up/media/image14.png)

（2）メモ帳に設定値を記述します。  
カスタマイズ可能な項目は、インストール先とスタートメニューフォルダー名になります。

**インストール先：**  
**Dir=**

**スタートメニューフォルダー名：**  
**Group=**

```
[Setup]
Lang=jp
Dir=C:\Program Files\RepostConfirmationCanceler
Group=RepostConfirmationCanceler
NoIcons=0
Tasks=
```

## サイレント インストール

**デフォルト値でのサイレント インストールの場合**  
**■デフォルト値でのサイレントインストールの場合は応答ファイルの作成は不要です。**

（1）セットアップ用のEXEファイルに「/SP- /VERYSILENT」オプションを付与し実行します。  
例) RepostConfirmationCancelerSetup_x64.exe /SP- /VERYSILENT</p>

![](set-up/media/image15.png)

**応答ファイルを利用したサイレント インストールの場合**

（1）セットアップ用のEXEファイルに「/SP- /VERYSILENT **/LOADINF="RepostConfirmationCanceler.inf"**」オプションを付与し実行します。  
例) RepostConfirmationCancelerSetup_x64.exe /SP- /VERYSILENT /LOADINF="RepostConfirmationCanceler.inf"

■応答ファイル RepostConfirmationCanceler.infはSetupファイルと同一フォルダーに設置するかフルパスを指定します。

![](set-up/media/image16.png)

\newpage
# サイレント アンインストール手順（自動アンインストール）

## RepostConfirmationCancelerサイレント アンインストールについて

RepostConfirmationCancelerのアンインストールをサイレント実行するための手順について記載します。

以下の環境用のSetupではサイレント アンインストールが可能です。  
**■管理者権限での実行が必要です。**  
**ログ関連のフォルダーや設定ファイル関連はアンインストール後に削除されません。**

## サイレント アンインストール

（1）RepostConfirmationCancelerセットアップ先にあるunins000.exeファイルに「/VERYSILENT」オプションを付与し実行します。  
例) `C:\Program Files\RepostConfirmationCanceler\unins000.exe /VERYSILENT`

![](set-up/media/image17.png)

\newpage
# バージョンアップ手順

## RepostConfirmationCancelerバージョンアップについて

RepostConfirmationCancelerのアンインストールの必要はありません。上書きインストールを行ってください。  
■マイナーバージョンアップの場合はアンインストールの必要はありません。

## バージョンアップ方法

**RepostConfirmationCancelerのセットアップ用のインストーラーは2種類あります。**  
**御利用されるWindows環境に合わせてセットアップファイルを選択してください。**

・Windows 64bit(x64)環境用  
**RepostConfirmationCancelerSetup_x64.exe**

・Windows 32bit(x86)環境用  
**RepostConfirmationCancelerSetup_x86.exe**

Windows環境に合っていないセットアップファイルを実行すると以下のメッセージが表示されます。

![](set-up/media/image3.png)

> このプログラムは  
> x86プロセッサー向けの Windows にしかインストールできません。

対処方法：RepostConfirmationCancelerSetup_x64.exeを利用してください。

![](set-up/media/image4.png)

> このプログラムは  
> x64プロセッサー向けの Windows にしかインストールできません。

対処方法：RepostConfirmationCancelerSetup_x86.exeを利用してください。

**■Windows 11(64bit)、Windows 10(64bit)環境のセットアップ例**

（1）RepostConfirmationCancelerSetup_x64.exeを実行します。  
**■管理者権限で実行してください。**

![](set-up/media/image5.png)

（2）「インストール」ボタンをクリックします。

![](set-up/media/image18.png)

（3）「完了」ボタンをクリックします。

![](set-up/media/image19.png)

以上で、バージョンアップ作業は完了です。

\newpage
# Microsoft Edge RepostConfirmationCanceler拡張導入手順

## Microsoft EdgeへのRepostConfirmationCanceler拡張導入手順について

Microsoft Edgeのアドオンストアから拡張機能をインストールします。

また、ADに所属している端末にMicrosoft Edgeへの拡張機能のインストールを強制する場合、グループポリシー(GPO)でインストールを強制します。

### Microsoft EdgeのアドオンストアからのRepostConfirmationCanceler拡張の導入手順

* <RepostConfirmationCancelerのアドオンページのURL>を開きます
* インストールボタンから拡張機能をインストールします

### グループポリシー(GPO)を利用したRepostConfirmationCanceler拡張の導入手順

**予めMicrosoft Edgeのグループポリシー設定が完了した環境での手順になります。**

詳しくは、「Windowsデバイスで Microsoft Edge ポリシー設定を構成する」を参照ください。  
https://learn.microsoft.com/ja-jp/deployedge/configure-microsoft-edge

（1）グループポリシーエディターを起動します。

![](set-up/media/image27.png)

（2）「管理用テンプレート」―「Microsoft Edge」―「拡張機能」を選択します。  
「サイレント インストールされる拡張機能を制御する」をダブルクリックします。

![](set-up/media/image27a.png)

（3）「有効」を選択します。

オプション：  
「表示…」をクリックします。

表示するコンテンツ画面でRepostConfirmationCanceler拡張アプリID「<ストア登録後の拡張機能のIDに変更する>」を入力します。

![](set-up/media/image28.png)

■RepostConfirmationCanceler拡張アプリID  
<ストア登録後の拡張機能のIDに変更する>

（4）グループポリシーが適用されると、レジストリの以下のキーに値が追加されます。

`\SOFTWARE\Policies\Microsoft\Edge\ExtensionInstallForcelist`

![](set-up/media/image29.png)

（5）グループポリシーを利用し自動的に拡張機能が有効化されます。  
ユーザーは、拡張機能の削除や無効化を行うことはできません。

グループポリシーが適用されると青のスライドバーの左隣に鍵マークが表示されます。

![](set-up/media/image30.png)

\newpage
# モジュール構成

## RepostConfirmationCancelerモジュール構成

RepostConfirmationCanceler インストーラーには以下のモジュールが含まれています。(合計 約15MB)

--------------------------------------------------------------------------------------------
ファイル名                       サイズ           概要
-------------------------------- ---------------- ------------------------------------------
RepostConfirmationCanceler.exe                   約16KB          ダイアログキャンセルモジュール

`RepostConfirmationCancelerHost\` \              約160KB          モダンブラウザー拡張連携
RepostConfirmationCancelerTalk.exe

`RepostConfirmationCancelerHost\` \              約1KB            Microsoft Edge拡張連携
edge.json
--------------------------------------------------------------------------------------------

# OQSDrug ： オンライン資格確認 xml薬剤情報取得表示ソフト
## 概要
電子カルテ「ダイナミクス」では、閲覧同意がある方の薬剤情報や健診情報をPDF形式で取得可能ですが、本ソフトウェア **OQSDrug** を使用することで、次の機能を提供します：

- ダイナミクスから同意情報を取得し、独自にオン資端末に閲覧要求を行い **XML形式の薬剤情報を取得**。
- PDF形式に比べ薬歴をまとめてわかりやすく表示。
- レセプト情報由来だけでなく、電子処方箋、調剤結果由来のデータも取得しますので、処方元が電子処方箋であれば**即時取得が可能**です
- PDF形式の情報も取得期間や間隔を調整可能。
- **RSBase** の検査項目として自動ファイリング可能。
  ![oqsdrug4](https://github.com/user-attachments/assets/fc92e45c-42fa-453f-94ff-ef45789aca44)



---

## 動作条件

- **ダイナミクス**  
  ダイナミクスが本ソフトの必須環境です。

- **RSBase**  
  RSBaseがあれば以下の追加機能が利用可能です。ない場合もxml形式の薬歴表示は可能です：
  - カルテ遷移を検知し、薬歴が存在すれば **自動で薬歴をポップアップ表示**。
  - **RSBase** の検査項目として自動ファイリング可能。

### 実行環境
- .NET 4.8Frameworkが必要です。最近のWindows10以上ではデフォルトでインストールされていますが、起動できない場合、以下のリンクから **.NET Framework 4.8 ランタイム** をダウンロードしてください：  
[公式ダウンロードページ](https://dotnet.microsoft.com/ja-jp/download/dotnet-framework/net48)
 
- **Accessデータベースエンジン**  
  実行PCにAccessデータベースエンジンが必要です。
  - **Access 32bit版** のインストール
  - または **Access ランタイム 32bit版** のインストール
   ( [公式ダウンロードページ](https://www.microsoft.com/ja-jp/download/details.aspx?id=50040) )
    で導入されます。

---

## 設置方法

1. **Zipファイルをダウンロード**。
2. <<**初回のみ**>>`OQSDrug_Data.mdb` を **datadyna.mdbがあるフォルダ** にコピー。
3. その他のファイル（`OQSDrug.exe`等）を任意の場所にコピー。
4. `OQSDrug.exe` を起動。
5. レジストリ等は使ってませんので、アンインストールしたいときは、コピーしたファイルを削除してください。設定値は、%LOCALAPPDATA%\YUKI_ENT_CLINIC フォルダ(C:\Users\アカウント名\Appdata\Local\YUKI_ENT_CLINIC)以下に残ってることがありますので、不要なら削除してください。
---

## 設定と動作の説明

起動後に以下の設定を行います：

1. **メインウィンドウ**
   
   - ②に現在の状態が表示されます。NGとなっている項目があると動作開始ができませんので、まず①の設定を行ってください。
   - ③は取得結果が表示されます
   - ④は動作ログが表示されます
   - 「薬歴自動起動」は、RSBaseの連携(ID.datまたはtemp_rs.txtまたはthept.txt)が設定されている場合、ここをチェックすると患者遷移に連動して薬歴がある場合自動で薬歴をポップアップ表示します。（thept.txtが最も安定しているように思います）
   ![oqsdrug1](https://github.com/user-attachments/assets/3e6aac17-33f5-4eb7-b288-b717fd02cd4d)


2. **設定**

   - メイン設定の各種フォルダ、ファイルパスを選択/入力します
   - RSBase reloadは、チェックしてRSBaseのトップ画面のUrlを入力すると、PDF取得後Urlをリロードしてファイリングします。RSAuto等で自動ファイリングする場合は指定不要です。
   - 医療機関コードは10桁のものです
   - 動作間隔は既定で30秒です。あまり短いと、datadyna破損のリスクが上がるかもしれません。
   - 「薬剤情報」「特定健診」のグループは **PDF** をRSBaseにファイリングするときに使用してください。RSBaseの検査として登録されます。ダイナミクス標準機能で自動取得し、**RSBaseのPDFから変換する設定を利用してる方は使用しないほうがいい**と思います。
   - xmlの薬歴は標準で必ず取得します
   - Viewer設定、RSBase連動は、ID.dat, temp_rs.txt、thept.txt方式に対応しますが、**thept.txtが一番安定している**ようです。RSBaseの`(52)  c:\common\thept.txt (&& c:\ID_temp.txt)にIDを出力 `をyesにしてください。
   - 「薬歴を最前面で表示」をチェックすると、薬歴ウインドウが最前面で表示されるようになります
     ![oqsdrugsetting](https://github.com/user-attachments/assets/6daf97be-b1de-48dc-964f-ca5d43a98d22)

 
   
3. **薬歴ビュワーウインドウ**
   
   - ウインドウサイズと位置を記憶しますので、初回に適宜見やすい場所に移動して閉じてみてください。次回以降起動時はその位置サイズで表示されます。ウインドウが行方不明になったときは、位置サイズをリセットのボタンを押すと、メインウィンドウ近くに表示されます。
   - 月ごと集計チェックがオンのときは、各医療機関ごとの処方を月ごとに合計して表示します。オフのときは日毎の表示になります。見やすい方で適宜切り替えてください。
   - メインウィンドウで、自動起動のオプションを設定すると、カルテ遷移しそのカルテ番号で過去の薬歴があれば薬歴ビュワーウインドウをポップアップで表示します。
![oqsdrug3](https://github.com/user-attachments/assets/e9631708-a4b7-49e4-85d1-8bc7446bb032)

4. **タスクトレイアイコン**

    ![tray](https://github.com/user-attachments/assets/d1664e40-6947-488b-9616-092e50e73b9e)

   - タスクトレイから開始・停止、ビュワーの表示などの操作が行えます
   - 薬歴取得動作中はタスクトレイアイコンがアニメーションで回転します
     
---

## 注意事項
- 薬歴は`OQSDrug_Data.mdb`に蓄積されていきます。バージョンアップ時に上書きしてしまうと過去に取得した薬歴が消えてしまいますので、初回以外は上書きしないようにおねがいします。
- **稀にダイナミクスの動作が不安定になることがあります。datadyna.mdbのバックアップはしっかり取っていただき、もしダイナミクスの動作に支障をきたす場合は直ちに使用を中止してください。**
- ダイナミクスの最適化を行うときや、他のクライアントから書き込みを行うときは、本ソフトの動作を止めるようにお願いします。
- 必ず32bit版のAccessまたはAccessランタイムを使用してください。
- 現状xmlは薬剤履歴のみ対応しています。健診情報や診療手術情報はPDFの方が見やすそうなので、当面対応予定はありません。
- 要望、改善案などは何なりとご連絡あるいはDisscussionに投稿等よろしくお願いいたします。できる範囲でなるべく対応したいと思います。

---

## 今後の予定
- バグ取り
- バックエンドデータベースにmdbを使っていますが、データが増えると安定動作が難しいですので、RSBaseで利用しているPostgreSQLをバックエンドにすることを検討してます。
  

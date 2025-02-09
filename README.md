# OQSDrug ： オンライン資格確認 xml薬剤情報取得表示ソフト

## 概要

紹介動画

[![紹介動画](https://img.youtube.com/vi/vewT-Vm346o/0.jpg)](https://youtu.be/vewT-Vm346o)


電子カルテ「ダイナミクス」では、閲覧同意がある方の薬剤情報や健診情報をPDF形式で取得可能ですがxml形式には対応してないため、耳鼻科小児科など処方回数が多くなると薬歴が見にくくなります。
本ソフトウェア **OQSDrug** を使用することで、次のような利点があります：

- ダイナミクスから同意情報を取得し、独自にオン資端末に閲覧要求を行い **XML形式の薬剤・特定健診・診療手術情報を取得**
- PDF形式に比べ薬歴・健診結果をまとめてわかりやすく表示。
- レセプト情報由来だけでなく、電子処方箋、調剤結果由来のデータも取得し、処方元が電子処方箋であれば**即時取得が可能**
- PDF形式の情報も取得期間や間隔を調整可能
- **RSBase** の検査項目としてPDFを自動ファイリング可能。
- 薬歴データはmdbファイルに保存され、Access等から自作クエリ等作ってデータの再利用が可能
  ![oqsdrug4](https://github.com/user-attachments/assets/fc92e45c-42fa-453f-94ff-ef45789aca44)

- 薬歴は薬効分類で色分け表示可能
- 薬剤や健診結果を選択してコピーし電カル等へのペースト可能
- ダイナミクスやRSBaseのカルテ遷移を検知して、連動表示/非表示可能
  ![oqsdrug53](https://github.com/user-attachments/assets/45798881-3699-4e76-ba95-55c0000c1b87)

 

---

## 動作条件

- **ダイナミクス**  
  ダイナミクスが本ソフトの必須環境です。

- **RSBase**  
  RSBaseがあれば以下の追加機能が利用可能です。ない場合もxml形式の薬歴表示は可能です：
  - カルテ遷移を検知し、薬歴が存在すれば **自動で薬歴をポップアップ表示**
  - **RSBase** の検査項目としてPDFを自動ファイリング
  - 薬歴ビュワーからRSBaseの薬剤情報を表示

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

1. [Release一覧](https://github.com/YUKI-ENT/OQSDrug/releases)から**Zipファイルをダウンロード**。
2. <<**初回のみ**>>`OQSDrug_Data.mdb` を **datadyna.mdbがあるフォルダ** にコピー。
3. その他のファイル（`OQSDrug.exe`等）を任意の場所にコピー。
4. `OQSDrug.exe` を起動。
5. レジストリ等は使ってませんので、アンインストールしたいときは、コピーしたファイルを削除してください。設定値は、%LOCALAPPDATA%\YUKI_ENT_CLINIC フォルダ(C:\Users\アカウント名\Appdata\Local\YUKI_ENT_CLINIC)以下に残ってることがありますので、不要なら削除してください。
---

## 運用方法
運用方法は、大きく次の2パターンあります。推奨は１のように、OQSDrug専用のPCで実行する運用です。２の方法では、**クライアントダイナミクスが時々数秒フリーズしたようになる** ことがあります。

### 1.  【**推奨**】OQSDrug専用PCを用意する
  - ダイナミクスネットワーク内で、ダイナミクスクライアントが動作していないPCで、OQSDrugを実行
  - OQSDrugの設定で、「ダイナミクスの場所」は **ダイナミクスサーバーのdatadyna.mdb** を指定
  - 「開始」で取込開始
  ![oqsdrug6](https://github.com/user-attachments/assets/6f9ce95a-3443-41a4-a061-073da87e3264)

    
### 2. ダイナミクスクライアントの動作しているPCで、OQSDrugの取込動作とViewerを同時に使う
  - 別PCを用意できない場合、ダイナクライアントで稼働させます
  - 設定では、「ダイナミクスの場所」は **ダイナクライアントの稼働しているDyna_cnt.mdb**を指定します。**Datadynaを指定するとdatadynaが破損する可能性があります**
  - ダイナクライアントをバージョンアップしたときは、新しいクライアントを指定する必要があります。
    ![oqsdrug7](https://github.com/user-attachments/assets/ad630590-c28f-4206-bddc-3b139cb91e45)

## 設定と動作の説明

起動後に以下の設定を行います：

- **メインウィンドウ**
   
   - ②に現在の状態が表示されます。NGとなっている項目があると動作開始ができませんので、まず①の設定を行ってください。
   - ③は取得結果が表示されます
   - ④は動作ログが表示されます
   - ⑤ `開始` を押すと取込作業を開始し、ボタン名が`停止`になります。もう一度押すと停止します。取込間隔は`設定`で設定します（デフォルト値30秒）
   - ⑥ RSBase連動`薬歴`、`健診`は、設定でRSBaseの連携(ID.datまたはtemp_rs.txtまたはthept.txt)が設定されている場合、ここをチェックすると患者遷移に連動して薬歴がある場合自動で薬歴をポップアップ表示します。（thept.txtが最も安定しているように思います）
   - `自動開始`をチェックすると、下の4つの設定値がすべてOKになると自動で取得開始し、1つでもNGになると停止します。

    ![oqsdrug61](https://github.com/user-attachments/assets/f55c9b7d-fb8b-4870-aa53-774c4ecc7aaf)

      
- **設定**

  ① OQS_data.mdbの場所を指定します。ダイナサーバーに置くのをおすすめします。<br>
  ② ダイナミクスの場所は、上記`運用方法`に記載したとおり、別PCで実行するときは**ダイナサーバーのdatadyna.mdb**、クライアントで実行するときは**クライアントダイナ**を指定してください。<br>
  ③ オン資端末のreq,resフォルダを含むフォルダを指定<br>
  ④ PDFを保存するときは、RSBaseかRSAutoの取込フォルダを指定<br>
  ⑤  `RSBase reload`をチェックしてRSBaseのトップ画面のUrlを入力すると、PDF取得後Urlをリロードしてファイリングします。RSAuto等で自動ファイリングする場合は指定不要です。<br>
  ⑥  `医療機関コード`は10桁のものです<br>
  ⑦  動作間隔は既定で30秒です。<br>
  ⑧⑨  `薬剤情報``特定健診`のグループは **PDF** をRSBaseにファイリングするときに使用してください。RSBaseの検査として登録されます。ダイナミクス標準機能で自動取得し、**RSBaseのPDFから変換する設定を利用してる方は使用しないほうがいい**と思います。<br>
       xmlの薬歴、健診は標準で必ず取得します。<br>
  ⑩ 指定するとタスクトレイに最小化した状態で起動します<br>
  ⑪ `RSBase連動`は、ID.dat, temp_rs.txt、thept.txt方式に対応しますが、**thept.txtが一番安定している**ようです。RSBaseの`(52)  c:\common\thept.txt (&& c:\ID_temp.txt)にIDを出力 `をyesにしてください。<br>
     また、ダイナミクスの他社連携を利用してのID連携も可能です。[参照](https://github.com/YUKI-ENT/OQSDrug/releases/tag/v1.25.2.8) <br>
  ⑫ `薬歴を最前面で表示`をチェックすると、薬歴ウインドウが最前面で表示されるようになります<br>
  ⑫ ビュワー表示時の初期期間を設定します<br>
  
     ![oqsdrug42](https://github.com/user-attachments/assets/15b24a0f-2136-4dcd-864e-53c065981877)


- **タスクトレイアイコン**

    ![oqsdrug45](https://github.com/user-attachments/assets/02a39560-6d01-4a0a-a55e-3370e171398c)


   - タスクトレイから開始・停止、ビュワーの表示などの操作が行えます
   - 薬歴取得動作中はタスクトレイアイコンがアニメーションで回転します
 
   
- **薬歴ビュワーウインドウ**
   
   - ウインドウサイズと位置を記憶しますので、初回に適宜見やすい場所に移動して閉じてみてください。次回以降起動時はその位置サイズで表示されます。ウインドウが行方不明になったときは、`設定`で`位置サイズをリセット`のボタンを押すと、メインウィンドウ近くに表示されます。
   - `月ごと集計`がオンのときは、各医療機関ごとの処方を月ごとに合計して表示します。オフのときは処方日毎の表示になります。見やすい方で適宜切り替えてください。
   - `自院除外`をオンにすると、自院での処方を表示せず、他院からの処方のみ表示します。
   - メインウィンドウで、`薬歴自動起動`のオプションを設定すると、カルテ遷移しそのカルテ番号で過去の薬歴があれば薬歴ビュワーウインドウをポップアップで表示し、薬歴がなければウィンドウを閉じます。

      ![oqsdrug53](https://github.com/user-attachments/assets/0354db98-72d6-4440-a6e2-7885f8b48a63)

        
   - セルを選択して右クリックすると、コンテキストメニューが利用できます。
      ![oqsdrug11](https://github.com/user-attachments/assets/37f0404a-90e9-477d-9fd4-1d90e36b9489)
     
   - 複数セルの全角/半角コピーが可能です
   - **実行しているPCにRSBaseがインストールされている場合**、`薬情検索`メニューが表示されます。これを選択すると、現在選択されているセルの薬情をRSBaseで表示するとともに、あいまい検索でRSBase薬情に登録されている薬剤名一覧が表示されます。
   - `RSBase薬情検索` フォームでは、リストをダブルクリックで該当薬情のRSBaseページを開きます。下のテキストボックスは、手入力で修正するときに使ってください。
   ![oqsdrug12](https://github.com/user-attachments/assets/a1a826fb-2ef5-4415-8eec-2a59c384fc1d)


- **特定健診ビュワーウインドウ**     

  - セルの選択、右クリックでデータのコピーに対応しています
    
     ![oqsdrug44](https://github.com/user-attachments/assets/cd051cb0-dbde-4935-8e11-9082643b7e2f)

---

## 注意事項
- 薬歴は`OQSDrug_Data.mdb`に蓄積されていきます。バージョンアップ時に上書きしてしまうと過去に取得した薬歴が消えてしまいますので、初回以外は上書きしないようにおねがいします。
  また`OQSDrug_Data.mdb`のバックアップも適宜取っておいてください。
- **稀にダイナミクスの動作が不安定になることがあります。datadyna.mdbのバックアップはしっかり取っていただき、もしダイナミクスの動作に支障をきたす場合は直ちに使用を中止してください。**
- ダイナミクスの最適化を行うときや、他のクライアントから書き込みを行うときは、本ソフトの動作を止めるようにお願いします。
- 必ず32bit版のAccessまたはAccessランタイムを使用してください。
- 現状xmlは薬剤履歴のみ対応しています。健診情報や診療手術情報はPDFの方が見やすそうなので、当面対応予定はありません。
- 要望、改善案などは何なりとご連絡あるいはDisscussionに投稿等よろしくお願いいたします。できる範囲でなるべく対応したいと思います。

---

## 今後の予定
- バグ取り
- バックエンドデータベースにmdbを使っていますが、データが増えると安定動作が難しいですので、RSBaseで利用しているPostgreSQLをバックエンドにすることを検討してます。
  

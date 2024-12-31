# OQSDrug
## 概要
電子カルテ「ダイナミクス」では、閲覧同意がある方の薬剤情報や健診情報をPDF形式で取得可能ですが、本ソフトウェア **OQSDrug** を使用することで、次の機能を提供します：

- ダイナミクスから同意情報を取得し、独自に **XML形式の薬剤情報を取得**。
- 薬歴をわかりやすく表示。
- PDF形式の情報も取得期間や間隔を調整可能。
- **RSBase** の検査項目として自動ファイリング可能。

---

## 動作条件

- **ダイナミクス**  
  ダイナミクスが本ソフトの必須環境です。

- **RSBase**  
  RSBaseがあれば以下の追加機能が利用可能です：
  - カルテ遷移を検知し、薬歴が存在すれば **自動で薬歴をポップアップ表示**。
  - **RSBase** の検査項目として自動ファイリング可能。

### 実行環境
- **Accessデータベースエンジン**  
  実行PCにAccessデータベースエンジンが必要です。
  - **Access 32bit版** のインストール。
  - または **Access ランタイム 32bit版** のインストール。

---

## 設置方法

1. **Zipファイルをダウンロード**。
2. <<**初回のみ**>>`OQSDrug_Data.mdb` を **datadyna.mdbがあるフォルダ** にコピー。
3. その他のファイル（例: `OQSDrug.exe`）を任意の場所にコピー。
4. `OQSDrug.exe` を起動。

---

## 設定と動作の説明

起動後に以下の設定を行います：

1. **メインウィンドウ**
   
   - ②に現在の状態が表示されます。NGとなっている項目があると動作開始ができませんので、まず①の設定を行ってください。
   - ④は取得結果が表示されます
   - ③は動作ログが表示されます
   ![oqsdrug1](https://github.com/user-attachments/assets/3e6aac17-33f5-4eb7-b288-b717fd02cd4d)


2. **設定**

   - ①の各種フォルダ、ファイルパスを選択/入力します
   - RSBase reloadは、チェックしてRSBaseのトップ画面のUrlを入力すると、PDF取得後Urlをリロードしてファイリングします。RSAuto等で自動ファイリングする場合は、指定不要です。
   - 医療機関コードは10桁のものです
   - 動作間隔は既定で30秒です
   - ②はPDFをRSBaseにファイリングするときに使用してください。ダイナミクス標準機能で自動取得し、RSBaseのPDFから変換する設定を利用してる方は使用しないほうがいいと思います。
   - ③は薬歴ビュワーウインドウの設定です。RSBase連動する場合は適切なファイルを指定してください。
  ![oqsdrug2](https://github.com/user-attachments/assets/36dee710-1595-4f1b-b89a-e691c92907cb)

   
3. **薬歴ビュワーウインドウ**
   
   - ウインドウサイズと位置を記憶しますので、初回に適宜見やすい場所に移動して閉じてみてください。
   - 月ごと集計チェックがオンのときは、各医療機関ごとの処方を月ごとに合計して表示します。オフのときは日毎の表示になります。
   - メインウィンドウで、自動起動のオプションを設定すると、カルテ遷移しそのカルテ番号で過去の薬歴があれば薬歴ビュワーウインドウをポップアップで表示します。
![oqsdrug3](https://github.com/user-attachments/assets/e9631708-a4b7-49e4-85d1-8bc7446bb032)

---

## 注意事項
- 薬歴は`OQSDrug_Data.mdb`に蓄積されていきます。バージョンアップ時に上書きしてしまうと過去に取得した薬歴が消えてしまいますので、初回以外は上書きしないようにおねがいします。
- 稀にダイナミクスの動作が不安定になることがあります。datadyna.mdbのバックアップはしっかり取っていただき、もしダイナミクスの動作に支障をきたす場合は直ちに使用を中止してください。
- ダイナミクスの最適化を行うときや、他のクライアントから書き込みを行うときは、本ソフトの動作を止めるようにお願いします。
- 必ず32bit版のAccessまたはAccessランタイムを使用してください。

---


## ライセンス
このプロジェクトは、[MITライセンス](LICENSE) のもとで公開されています。

using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.OleDb;
using System.IO;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;
using System.Xml;
using System.Text;
using System.Data.Common;
using System.Data.SqlClient;
using static System.Net.WebRequestMethods;
using File = System.IO.File;
using System.Collections;


namespace OQSDrug
{
    public partial class Form1 : Form
    {
        public string DBProvider = "";
        public long tempId = 0;
        public bool autoRSB = false;

        private Timer timer;
        private bool isTimerRunning = false; // タイマーの状態フラグ

        private FileSystemWatcher fileWatcher;

        // 動的に追加するラベル
        private Label[] statusLabels;
        private Label[] statusTexts;

        private Form3 form3Instance = null;

        public Form1()
        {
            InitializeComponent();
            SetupTableLayout();
            InitializeTimer();
        }

        // タイマーの初期化
        private void InitializeTimer()
        {
            // タイマーを作成
            timer = new Timer
            {
                Interval = Properties.Settings.Default.TimerInterval * 1000 // インターバルを設定
            };
            // タイマーのイベントハンドラを登録
            timer.Tick += Timer_Tick;
        }

        // タイマーのイベントハンドラ
        private async void Timer_Tick(object sender, EventArgs e)
        {
            if (!isTimerRunning)
            {
                isTimerRunning = true;
                DateTime startTime = DateTime.Now;
                AddLog($"タイマーイベント開始: {startTime}");

                //Datadynaのデータ取得
                DataTable dynaTable = await LoadDataFromDatabaseAsync(Properties.Settings.Default.Datadyna);

                //薬剤PDF
                if (Properties.Settings.Default.DrugFileCategory > 0)
                {
                    MakeReq(Properties.Settings.Default.DrugFileCategory + 10, dynaTable);
                }

                //薬剤xmlは常に実行
                MakeReq(12, dynaTable);

                //健診PDF
                if (Properties.Settings.Default.KensinFileCategory > 0)
                {
                    MakeReq(Properties.Settings.Default.KensinFileCategory + 100, dynaTable);
                }

                await reloadDataAsync();

                //Resフォルダの処理

                bool processCompleted = false;
                bool isRemainRes = true;

                // 5秒ごとにProcessResAsyncを呼び出し
                while (!processCompleted || isRemainRes)
                {
                    await Task.Delay(5000);

                    processCompleted = await ProcessResAsync();
                    isRemainRes = await RemainResTask();

                    if ((DateTime.Now - startTime).TotalSeconds > (Properties.Settings.Default.TimerInterval - 5))
                    {
                        processCompleted = true;
                        isRemainRes = false;
                        AddLog("時間内に処理が終了しませんでしたので、タイマー処理を中止します");
                    }

                    await reloadDataAsync();
                    if (!timer.Enabled)
                    {
                        break;
                    }
                }
                isTimerRunning = false;
                AddLog($"タイマーイベント終了");
            }
        }

        // フォームが閉じられるときにタイマーを停止
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            if (timer != null)
            {
                timer.Stop();
                timer.Dispose();
            }
        }

        // データベースの内容を読み込み、DataGridViewに表示
        private async Task reloadDataAsync()
        {
            AddLog("DataGridViewを更新します");
            string connectionString = $"Provider={DBProvider};Data Source={Properties.Settings.Default.OQSDrugData};";

            string sql = "SELECT category, PtID, PtName, result, reqDate, reqFile, resDate, resFile, ID FROM reqResults ORDER BY reqResults.ID DESC";

            try
            {
                using (OleDbConnection connection = new OleDbConnection(connectionString))
                {
                    // 接続を開く
                    await connection.OpenAsync();

                    // データを取得してDataTableに格納
                    using (var command = new OleDbCommand(sql, connection))
                    {
                        using (var reader = await command.ExecuteReaderAsync())
                        using (var dataTable = new DataTable())
                        {
                            dataTable.Load(reader);

                            // DataGridViewに表示
                            dataGridView1.Invoke(new Action(() =>
                            {
                                //UI スレッド
                                dataGridView1.DataSource = dataTable;

                                ConfigureDataGridView(dataGridView1);


                            }));
                            AddLog("reqResultsテーブルを読み込みました");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // エラー処理
                AddLog($"エラー: {ex.Message}");
            }
        }


        private async void toolStripButtonSettings_Click(object sender, EventArgs e)
        {
            //動作中の場合は停止する
            if (isTimerRunning)
            {
                MessageBox.Show("一旦タイマー動作を停止します");
                StartStop.Checked = false;
            }

            //Form2を開く
            Form2 form2 = new Form2();
            form2.ShowDialog(this);

            //Form2閉じたあと
            SetupYZKSindicator();
            SetupTableLayout();
            bool okSettings = await UpdateStatus();

            if (okSettings)
            {
                //テーブルフィールドのアップデート
                bool fieldCheck = await AddFieldIfNotExists(Properties.Settings.Default.OQSDrugData, "drug_history", "Source", "INTEGER")
                                && await AddFieldIfNotExists(Properties.Settings.Default.OQSDrugData, "drug_history", "Revised", "YESNO");

                if (!fieldCheck)
                {
                    MessageBox.Show("OQSDrugDataのアップデートでエラーが発生しました。OQSDrugData.mdbにアクセスできるかを調べて再起動してください");
                }

                this.StartStop.Enabled = true;
                await reloadDataAsync();
            }

            LoadViewerSettings();

        }

        private async void StartStop_CheckedChanged(object sender, EventArgs e)
        {
            if (StartStop.Checked)
            {
                //開始
                StartStop.Text = "停止";
                StartStop.Image = Properties.Resources.Stop;
                timer.Interval = Properties.Settings.Default.TimerInterval * 1000;
                timer.Start();
                AddLog($"タイマー処理を開始します。間隔は{timer.Interval / 1000}秒です");

                //初回実行
                await Task.Run(() => Timer_Tick(timer, EventArgs.Empty));

            }
            else
            {
                StartStop.Text = "開始";
                StartStop.Image = Properties.Resources.Go;
                timer.Stop();
                AddLog("タイマー処理を終了します");
            }
        }

        public async void MakeReq(int category, DataTable dynaTable) //Category: 11:薬剤pdf, 12:薬剤xml、13:薬剤診療pdf、14：薬剤診療xml、101：健診pdf、102：健診xml
        {
            int Span = Properties.Settings.Default.YZspan;
            int YZinterval = Properties.Settings.Default.YZinterval, KSinterval = Properties.Settings.Default.KSinterval, Interval;
            string connectionOQSData = $"Provider={DBProvider};Data Source={Properties.Settings.Default.OQSDrugData};";
            string dynaPath = Properties.Settings.Default.Datadyna;
            string douiFlag = "", douiDate = "";
            DateTime checkDate;
            bool doReq = true;
            int reqCategory = 0, fileCategory = 0;
            string OQSpath = Properties.Settings.Default.OQSFolder;

            string startDate = DateTime.Now.AddMonths(-Span).ToString("yyyyMM");
            string endDate = DateTime.Now.ToString("yyyyMM");

            string reqDateSQL = "SELECT TOP 1 reqDate, ID FROM reqResults " +
                       "WHERE PtID = ? AND category = ? ORDER BY ID DESC";

            // レコードの更新クエリ
            string updateRecordSQL = "UPDATE reqResults SET reqFile = ?, reqDate = ?, resFile = NULL, resDate= NULL, result = NULL  WHERE ID = ?";

            // レコードの新規追加クエリ
            string insertRecordSQL = "INSERT INTO reqResults (Category, PtID, PtName, reqFile, reqDate) VALUES (?, ?, ?, ?, ?)";

            switch (category)
            {
                case 11:
                    douiFlag = "薬剤情報閲覧同意フラグ";
                    douiDate = "薬剤情報閲覧有効期限";
                    Interval = YZinterval;  // Replace with appropriate interval
                    reqCategory = 1;
                    fileCategory = 1;
                    AddLog("PDF薬剤情報取得用reqファイルを作成します");
                    break;
                case 101:
                    douiFlag = "特定検診情報閲覧同意フラグ";
                    douiDate = "特定検診情報閲覧有効期限";
                    Interval = KSinterval;
                    reqCategory = 2;
                    fileCategory = 1;
                    AddLog("特定検診情報取得用reqファイルを作成します");
                    break;
                case 12:
                    douiFlag = "薬剤情報閲覧同意フラグ";
                    douiDate = "薬剤情報閲覧有効期限";
                    Interval = YZinterval;  // Replace with appropriate interval
                    reqCategory = 3;
                    fileCategory = 2;
                    AddLog("xml薬剤情報取得用reqファイルを作成します");
                    break;
                case 13:
                    douiFlag = "薬剤情報閲覧同意フラグ";
                    douiDate = "薬剤情報閲覧有効期限";
                    Interval = YZinterval;  // Replace with appropriate interval
                    reqCategory = 4;
                    fileCategory = 1;
                    AddLog("PDF薬剤診療情報取得用reqファイルを作成します");
                    break;
                default:
                    AddLog("Invalid category");
                    return;
            }

            checkDate = fileCategory == 1 ? DateTime.Now.AddMonths(-Interval) : DateTime.Now.AddDays(-1);

            try
            {
                //同意フラグ抽出
                DataRow[] DouiRows = dynaTable.Select($"{douiFlag} = '1'");
                foreach (DataRow DouiRow in DouiRows)
                {
                    long ptId = DouiRow["カルテ番号"] == DBNull.Value ? 0 : Convert.ToInt64(DouiRow["カルテ番号"]);
                    string ptName = DouiRow["氏名"].ToString();

                    if (ptId == 0)
                    {
                        AddLog($"{ptName}さんのカルテ番号が空のため取得を試みます");
                        ptId = Name2ID(ptName, DouiRow["生年月日西暦"].ToString(), dynaTable);
                        AddLog($"カルテ番号{ptId}を取得しました。処理を継続します");
                    }

                    if (!IsDateStringAfterNow(DouiRow[douiDate].ToString()))
                    {
                        AddLog($"{ptId}:{ptName}さんの同意有効期限が切れているのでスキップします");
                    }
                    else
                    {   // 同意有効
                        using (OleDbConnection connData = new OleDbConnection(connectionOQSData))
                        {
                            await connData.OpenAsync();

                            string reqXml = "";
                            object resultId = null;

                            using (OleDbCommand cmd = new OleDbCommand(reqDateSQL, connData))
                            {
                                cmd.Parameters.AddWithValue("?", ptId);
                                cmd.Parameters.AddWithValue("?", category);

                                using (OleDbDataReader reqReader = (OleDbDataReader)await cmd.ExecuteReaderAsync())
                                {
                                    doReq = false;

                                    if (reqReader.HasRows && reqReader.Read())
                                    {
                                        DateTime reqDate = reqReader.GetDateTime(reqReader.GetOrdinal("reqDate"));

                                        if (reqDate < checkDate)
                                        {
                                            doReq = true;
                                            resultId = reqReader["ID"];
                                            AddLog($"{ptId}:{ptName}さんのxmlを作成します（更新）");
                                        }
                                        else
                                        {
                                            AddLog($"{ptId}:{ptName}さん再取得期間外なのでスキップします");
                                        }
                                    }
                                    else
                                    {
                                        AddLog($"{ptId}:{ptName}さんのxmlを作成します（新規）");
                                        doReq = true;
                                    }

                                    if (doReq)
                                    {
                                        var ptData = new
                                        {
                                            Id = ptId,
                                            Name = ptName,
                                            InsurerNumber = DouiRow["保険者番号"].ToString(),
                                            InsuranceCardSymbol = DouiRow["被保険者証記号"].ToString(),
                                            InsuredPersonIdentificationNumber = DouiRow["被保険者証番号"].ToString(),
                                            BranchNumber = DouiRow["被保険者証枝番"].ToString()
                                        };

                                        reqXml = await Task.Run(() => GenerateXML(ptData, OQSpath, startDate, endDate, Properties.Settings.Default.MCode, category));


                                        if (!string.IsNullOrEmpty(reqXml))
                                        {
                                            // Save or update reqResults table
                                            AddLog($"{ptId}:{ptName}さんのXML生成成功");
                                        }
                                    }
                                } //Reader

                                if (reqXml.Length > 0) //xml作成済み
                                {
                                    if (resultId != null)
                                    {
                                        // レコードが存在する場合は更新
                                        using (OleDbCommand updateCmd = new OleDbCommand(updateRecordSQL, connData))
                                        {
                                            updateCmd.Parameters.AddWithValue("?", reqXml);
                                            updateCmd.Parameters.AddWithValue("?", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                                            updateCmd.Parameters.AddWithValue("?", resultId);
                                            await updateCmd.ExecuteNonQueryAsync();

                                            AddLog($"{ptId}:{ptName}さんのxml作成に成功しました (更新)");
                                            //reqReadyFlag = true;
                                        }
                                    }
                                    else
                                    {
                                        // レコードが存在しない場合は新規追加
                                        using (OleDbCommand insertCmd = new OleDbCommand(insertRecordSQL, connData))
                                        {
                                            insertCmd.Parameters.AddWithValue("?", category);
                                            insertCmd.Parameters.AddWithValue("?", ptId);
                                            insertCmd.Parameters.AddWithValue("?", ptName); // 患者名を渡す
                                            insertCmd.Parameters.AddWithValue("?", reqXml);
                                            insertCmd.Parameters.AddWithValue("?", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                                            await insertCmd.ExecuteNonQueryAsync();

                                            AddLog($"{ptId}:{ptName}さんのxml作成に成功しました (新規追加)");
                                            //reqReadyFlag = true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                AddLog("MakeReq処理が終了しました");
            }
            catch (Exception ex)
            {
                AddLog($"Error occurred in MakeReq: {ex.Message}");
            }
        }

        private async Task<DataTable> LoadDataFromDatabaseAsync(string dynaPath)
        {
            string connectionString = $"Provider={DBProvider};Data Source={dynaPath};Mode=Read;Persist Security Info=False;";
            string query = "SELECT * FROM T_資格確認結果表示";

            DataTable dataTable = new DataTable();

            try
            {
                using (OleDbConnection connection = new OleDbConnection(connectionString))
                {
                    await connection.OpenAsync(); // 非同期で接続を開く

                    using (OleDbCommand command = new OleDbCommand(query, connection))
                    {
                        using (OleDbDataReader reader = (OleDbDataReader)await command.ExecuteReaderAsync()) // 非同期でデータを読み込む
                        {
                            dataTable.Load(reader); // DataReader のデータを DataTable にロード
                        }
                    }
                }

                AddLog("DatadynaのT_資格確認結果表示の取り込みが完了しました");
            }
            catch (Exception ex)
            {
                AddLog($"Datadynaの読み込みエラー:{ex.Message}");
                return null;
            }

            return dataTable;
        }

        public async Task<string> CheckDatabaseAsync(string dbPath, string tableName)
        {
            return await Task.Run(() =>
            {
                // 引数の妥当性チェック
                if (string.IsNullOrEmpty(dbPath))
                {
                    return "エラー: データベースパスが指定されていません。";
                }
                if (string.IsNullOrEmpty(tableName))
                {
                    return "エラー: テーブル名が指定されていません。";
                }

                // 接続文字列を作成
                string connectionString = $"Provider={DBProvider};Data Source={dbPath};";

                try
                {
                    // データベース接続
                    using (var connection = new OleDbConnection(connectionString))
                    {
                        connection.Open();

                        // テーブルが存在するかを直接確認する
                        string query = $"SELECT TOP 1 * FROM [{tableName}]";
                        try
                        {
                            using (var command = new OleDbCommand(query, connection))
                            {
                                command.ExecuteScalar();
                                return "OK"; // テーブルが存在
                            }
                        }
                        catch (OleDbException)
                        {
                            return $"エラー: テーブル '{tableName}' が存在しません。";
                        }
                    }
                }
                catch (Exception ex)
                {
                    return $"エラー: {ex.Message}"; // 例外発生時のエラーメッセージ
                }
            });
        }

        // ディレクトリチェックの非同期メソッド
        private async Task<string> CheckDirectoryExistsAsync(string directoryPath)
        {
            return await Task.Run(() =>
            {
                return Directory.Exists(directoryPath) ? "OK" : "NG";
            });
        }

        private async Task<bool> UpdateStatus()
        {
            bool AllOK = true;

            //設定初期値の確認
            if (Properties.Settings.Default.TimerInterval <= 0)
            {
                Properties.Settings.Default.TimerInterval = 30;
            }

            Properties.Settings.Default.Save();

            // 非同期チェックタスクを作成
            var tasks = new[]
            {
                CheckDatabaseAsync(Properties.Settings.Default.OQSDrugData, "reqResults"),
                CheckDatabaseAsync(Properties.Settings.Default.Datadyna, "T_資格確認結果表示"),
                CheckDirectoryExistsAsync(Properties.Settings.Default.OQSFolder),
                CheckDirectoryExistsAsync(Properties.Settings.Default.RSBgazouFolder)
            };

            // 各タスクのインデックスと結果を保持するための辞書
            var taskIndexMap = tasks
                .Select((task, index) => new { task, index })
                .ToDictionary(x => x.task, x => x.index);

            while (taskIndexMap.Any())
            {
                // 完了したタスクを取得
                var completedTask = await Task.WhenAny(taskIndexMap.Keys);

                // タスクの結果を取得
                string result = await completedTask;

                // 対応するインデックスを取得
                int index = taskIndexMap[completedTask];
                taskIndexMap.Remove(completedTask);

                // UI を更新（インデックスに基づいて更新）
                Invoke(new Action(() =>
                {
                    if (result == "OK")
                    {
                        statusTexts[index].Text = "OK";
                        statusTexts[index].ForeColor = Color.Green;
                    }
                    else
                    {
                        AllOK = false;

                        statusTexts[index].Text = "NG";
                        statusTexts[index].ForeColor = Color.Red;

                        AddLog(statusLabels[index].Text + ":" + result);
                    }
                }));
            }

            return AllOK;
        }

        private string getOLEProviders()
        {
            string returnString = "";
            try
            {
                Console.WriteLine("登録されているOLE DBプロバイダの一覧:");

                var enumerator = new OleDbEnumerator();

                // OleDbEnumerator.GetElements() を呼び出し
                var dataTable = enumerator.GetElements();


                // DataTable の内容を列挙
                foreach (System.Data.DataRow row in dataTable.Rows)
                {
                    returnString += $"プロバイダ名: {row["SOURCES_NAME"]}\n";

                }
                return returnString;
            }
            catch (Exception ex)
            {
                returnString += $"エラー: {ex.Message}";
                return returnString;
            }
        }

        private void InitializeDBProvider()
        {
            try
            {
                AddLog("登録されているOLE DBプロバイダを確認します...");

                // プロバイダの優先順序
                string[] preferredProviders =
                {
                    "Microsoft.Jet.OLEDB.4.0",
                    "Microsoft.ACE.OLEDB.12.0",
                    "Microsoft.ACE.OLEDB.15.0",
                    "Microsoft.ACE.OLEDB.16.0"
                };
                toolStripComboBoxDBProviders.Items.Clear();

                // OleDbEnumeratorを使用して登録されているプロバイダを取得
                var enumerator = new OleDbEnumerator();
                var dataTable = enumerator.GetElements();

                // 登録されているプロバイダの一覧を取得
                var availableProviders = new List<string>();
                foreach (System.Data.DataRow row in dataTable.Rows)
                {
                    string providerName = row["SOURCES_NAME"].ToString();
                    availableProviders.Add(providerName);
                }

                // プロバイダの優先順序に従ってチェック
                foreach (string provider in preferredProviders)
                {
                    if (availableProviders.Contains(provider))
                    {
                        AddLog($"DBプロバイダ: {DBProvider}が見つかりました");
                        toolStripComboBoxDBProviders.Items.Add(provider);
                        if (DBProvider.Length == 0)
                        {
                            DBProvider = provider;
                            AddLog($"使用するプロバイダ: {DBProvider}");
                            toolStripComboBoxDBProviders.SelectedIndex = toolStripComboBoxDBProviders.Items.IndexOf(provider);
                        }
                    }
                }

                // 適切なプロバイダが見つからなかった場合
                if (DBProvider.Length == 0)
                {
                    AddLog("適切なOLE DBプロバイダが見つかりませんでした。");
                }
            }
            catch (Exception ex)
            {
                AddLog($"エラー: {ex.Message}");
            }
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            //MessageBox.Show( getOLEProviders());
            InitializeDBProvider();

            SetupYZKSindicator();

            listViewLog.Columns.Add("TimeStamp", 100); // 列1: タイムスタンプ
            listViewLog.Columns.Add("Log", 400);   // 列2: メッセージ

            bool okSettings = await UpdateStatus();

            if (okSettings)
            {
                //テーブルフィールドのアップデート
                //テーブルフィールドのアップデート
                bool fieldCheck = await AddFieldIfNotExists(Properties.Settings.Default.OQSDrugData, "drug_history", "Source", "INTEGER")
                                && await AddFieldIfNotExists(Properties.Settings.Default.OQSDrugData, "drug_history", "Revised", "YESNO");
                if (!fieldCheck)
                {
                    MessageBox.Show("OQSDrugDataのアップデートでエラーが発生しました。OQSDrugData.mdbにアクセスできるかを調べて再起動してください");
                }
                this.StartStop.Enabled = true;
                await reloadDataAsync();
            }

            autoRSB = Properties.Settings.Default.autoRSB;
            checkBoxAutoview.Checked = autoRSB;

            LoadViewerSettings();


        }

        private void AddLog(string message)
        {
            if (listViewLog.InvokeRequired)
            {
                // UIスレッドに処理を委譲
                listViewLog.Invoke(new Action(() => AddLog(message)));
            }
            else
            {
                // メインスレッド上でUI操作
                var item = new ListViewItem(DateTime.Now.ToString("yy-MM-dd HH:mm:ss"));
                item.SubItems.Add(message);
                listViewLog.Items.Add(item);

                // 最大行数を超えたら古い行を削除
                if (listViewLog.Items.Count > 1000)
                {
                    listViewLog.Items.RemoveAt(0); // 最初の行を削除
                }

                // 最新のログを表示
                listViewLog.EnsureVisible(listViewLog.Items.Count - 1);
            }
        }

        private bool IsDateStringAfterNow(string dateString)
        {
            try
            {
                // 日付文字列を変換
                var targetDate = new DateTime(
                    int.Parse(dateString.Substring(0, 4)),   // 年
                    int.Parse(dateString.Substring(4, 2)),   // 月
                    int.Parse(dateString.Substring(6, 2)),   // 日
                    int.Parse(dateString.Substring(8, 2)),   // 時
                    int.Parse(dateString.Substring(10, 2)),  // 分
                    int.Parse(dateString.Substring(12, 2))   // 秒
                );

                // 現在時刻と比較
                return targetDate > DateTime.Now;
            }
            catch (Exception)
            {
                // 不正な文字列が渡された場合の処理
                AddLog($"Invalid date string: {dateString}");
                return false;
            }
        }

        private void SetupTableLayout()
        {
            // 状態表示の項目
            string[] items = {
            "OQSDrug_data.mdb",
            "datadyna.mdb",
            "OQSフォルダ",
            "Gazouフォルダ"
        };

            statusLabels = new Label[items.Length];
            statusTexts = new Label[items.Length];

            tableLayoutPanel.Controls.Clear();
            tableLayoutPanel.RowCount = items.Length; // 行数を項目数に合わせる
            tableLayoutPanel.ColumnCount = 2;        // 2列に設定


            for (int i = 0; i < items.Length; i++)
            {
                // 項目名ラベル
                statusLabels[i] = new Label
                {
                    Text = items[i],
                    AutoSize = true,
                    Font = new Font("Segoe UI", 12, FontStyle.Regular),
                    TextAlign = ContentAlignment.MiddleLeft,
                    Padding = new Padding(5)
                };

                // 状態表示ラベル
                statusTexts[i] = new Label
                {
                    Text = "Checking...", // 初期値
                    AutoSize = true,
                    Font = new Font("Segoe UI", 12, FontStyle.Bold),
                    ForeColor = Color.Gray,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Padding = new Padding(5)
                };

                // コントロールをTableLayoutPanelに追加
                tableLayoutPanel.Controls.Add(statusLabels[i], 0, i);
                tableLayoutPanel.Controls.Add(statusTexts[i], 1, i);
            }
        }

        private void buttonExit_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("終了しますか？", "終了", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                if (timer.Enabled)
                {
                    timer.Stop();
                    timer.Dispose();
                }
                Application.Exit();
            }
        }

        private void toolStripComboBoxDBProviders_SelectedIndexChanged(object sender, EventArgs e)
        {
            DBProvider = toolStripComboBoxDBProviders.Text;

            AddLog($"データベースプロバイダーを{DBProvider}に変更しました");
        }

        private void SetupYZKSindicator() //xmlは常に取得
        {
            Color activeYZ = Color.LightGreen;
            Color activeKS = Color.LightCyan;
            Color acticePDF = Color.Violet;
            Color activeXML = Color.Orange;
            Color activeText = SystemColors.ControlText;

            buttonYZXML.BackColor = activeXML;
            buttonYZXML.ForeColor = activeText;

            buttonYZ.BackColor = activeYZ;
            buttonYZ.ForeColor = activeText;
            buttonSR.BackColor = SystemColors.Control;
            buttonSR.ForeColor = SystemColors.ControlText;


            switch (Properties.Settings.Default.DrugFileCategory) //
            {
                case 1:
                    buttonYZPDF.BackColor = acticePDF;
                    buttonYZPDF.ForeColor = activeText;

                    break;
                case 3:
                    buttonYZPDF.BackColor = acticePDF;
                    buttonYZPDF.ForeColor = activeText;
                    buttonSR.BackColor = activeYZ;
                    buttonSR.ForeColor = activeText;
                    break;
                default:
                    buttonYZPDF.BackColor = SystemColors.Control;
                    buttonYZPDF.ForeColor = SystemColors.ControlText;
                    break;
            }

            //健診
            buttonKS.BackColor = activeKS;
            switch (Properties.Settings.Default.KensinFileCategory) //
            {
                case 1:
                    buttonKSPDF.BackColor = acticePDF;
                    buttonKSXML.BackColor = SystemColors.Control;
                    break;
                default:
                    buttonKS.BackColor = SystemColors.Control;
                    buttonKSPDF.BackColor = SystemColors.Control;
                    buttonKSXML.BackColor = SystemColors.Control;
                    break;
            }
        }

        private string GenerateXML(dynamic ptData, string folderPath, string startDate, string endDate, string medicalInstitutionCode, int category)
        {
            int fileCategory;
            try
            {
                // ArbitraryFileIdentifier を生成
                string currentDate = DateTime.Now.ToString("yyyyMMdd");
                string currentTime = DateTime.Now.ToString("HHmmss");
                string arbitraryFileIdentifier = ptData.Id.ToString();

                // FileSymbol を決定
                string fileSymbol;
                if (category > 100) //健診
                {
                    fileSymbol = "TKK";
                }
                else
                {
                    fileSymbol = "YKZ";
                }
                fileCategory = 2 - (category % 2);

                // ファイル名を生成
                string fileName = $"{fileSymbol}siquc01req_99{fileCategory:00}{currentDate}{ptData.Id}.xml";

                // XMLファイルのフルパス
                string filePath = Path.Combine(folderPath, "req", fileName);

                // フォルダが存在しない場合は作成
                string directoryPath = Path.Combine(folderPath, "req");
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                // XmlWriterSettingsの設定
                var settings = new XmlWriterSettings
                {
                    Indent = true,
                    Encoding = Encoding.GetEncoding("Shift_JIS"),
                    OmitXmlDeclaration = true // ヘッダーをカスタムで出力するため省略
                };

                // XMLコンテンツを構築
                using (XmlWriter writer = XmlWriter.Create(filePath, settings))
                // using (XmlWriter writer = XmlWriter.Create(filePath, new XmlWriterSettings { Indent = true, Encoding = System.Text.Encoding.GetEncoding("Shift_JIS") }))
                {
                    writer.WriteStartDocument();
                    // 手動でカスタムヘッダーを記述
                    writer.WriteRaw("<?xml version=\"1.0\" encoding=\"Shift_JIS\" standalone=\"no\"?>\n");
                    writer.WriteStartElement("XmlMsg");

                    // MessageHeader セクション
                    writer.WriteStartElement("MessageHeader");
                    writer.WriteElementString("MedicalInstitutionCode", medicalInstitutionCode);
                    writer.WriteElementString("InsurerNumber", ptData.InsurerNumber);
                    writer.WriteElementString("InsuranceCardSymbol", ptData.InsuranceCardSymbol);
                    writer.WriteElementString("InsuredPersonIdentificationNumber", ptData.InsuredPersonIdentificationNumber);
                    writer.WriteElementString("BranchNumber", ptData.BranchNumber);
                    writer.WriteElementString("ArbitraryFileIdentifier", arbitraryFileIdentifier);
                    writer.WriteEndElement(); // MessageHeader

                    // MessageBody セクション
                    writer.WriteStartElement("MessageBody");
                    if (category < 100) //薬剤
                    {
                        writer.WriteElementString("StartDate", startDate);
                        writer.WriteElementString("EndDate", endDate);
                    }
                    writer.WriteElementString("FileCategory", fileCategory.ToString());
                    writer.WriteElementString("PrDiInfClassification", "1");
                    writer.WriteElementString("MedicalTreatmentFlag", "1");
                    writer.WriteEndElement(); // MessageBody

                    writer.WriteEndElement(); // XmlMsg
                    writer.WriteEndDocument();
                }

                // 成功時にファイルパスを返す
                return filePath;
            }
            catch (Exception ex)
            {
                // エラー時は空文字列を返す
                AddLog($"Error: {ex.Message}");
                return string.Empty;
            }
        }

        private async Task<bool> ProcessResAsync()
        {
            string connectionOQSData = $"Provider={DBProvider};Data Source={Properties.Settings.Default.OQSDrugData};";

            try
            {
                string resFolder = Path.Combine(Properties.Settings.Default.OQSFolder, "res");
                string gazouFolder = Properties.Settings.Default.RSBgazouFolder;
                string[] RSBname = { "", Properties.Settings.Default.YZname, Properties.Settings.Default.KSname };

                // resフォルダの存在確認
                if (!Directory.Exists(resFolder))
                {
                    AddLog($"エラー: resフォルダが見つかりません。{resFolder}");
                    return true;
                }

                AddLog("resフォルダの処理を開始します");

                var files = Directory.GetFiles(resFolder);
                bool RSBreloadFlag = false;
                bool AllDataProcessed = false;

                foreach (var file in files)
                {
                    try
                    {
                        string fileNameWithoutExt = Path.GetFileNameWithoutExtension(file).Replace("res", "req");
                        string extension = Path.GetExtension(file).ToLower();
                        string resFilename = null;
                        string messageContent = null;

                        using (OleDbConnection connection = new OleDbConnection(connectionOQSData))
                        {
                            await connection.OpenAsync();

                            // reqResults テーブル内の該当レコードを検索
                            string query = $"SELECT * FROM reqResults WHERE resFile IS NULL AND reqFile LIKE '%{fileNameWithoutExt}%'";

                            using (OleDbCommand command = new OleDbCommand(query, connection))
                            using (DbDataReader reader = await command.ExecuteReaderAsync())
                            {
                                if (await reader.ReadAsync())
                                {
                                    long PtID = Convert.ToInt64(reader["PtID"]);
                                    int Category = reader["Category"] != DBNull.Value ? Convert.ToInt32(reader["Category"]) : 0;
                                    string PtName = (string)reader["PtName"];
                                    object resultId = reader["ID"];

                                    switch (extension)
                                    {
                                        case ".xml":
                                            AddLog($"{PtID}:{PtName}resフォルダにxmlファイルが見つかりました: {file}");
                                            var xmlDoc = new XmlDocument();

                                            try
                                            {
                                                resFilename = file;

                                                xmlDoc.Load(file);
                                                var resultCodeNode = xmlDoc.SelectSingleNode("//ResultCode");

                                                if (resultCodeNode != null && resultCodeNode.InnerText == "1")
                                                {
                                                    // xml薬歴
                                                    messageContent = await ProcessDrugInfoAsync(PtID, xmlDoc);
                                                }
                                                else
                                                {
                                                    var xmlNode = xmlDoc.SelectSingleNode("//MessageContents");
                                                    messageContent = xmlNode != null ? xmlNode.InnerText : "<MessageContents> タグが見つかりません";
                                                }
                                            }
                                            catch
                                            {
                                                messageContent = "XMLファイルの読み込みに失敗しました";
                                            }

                                            // XMLファイルを削除
                                            System.IO.File.Delete(file);
                                            AddLog($"{messageContent} xmlファイルを削除しました");
                                            break;

                                        case ".pdf":
                                            int ReadCategory = (int)Math.Floor(Math.Log10(Math.Abs(Category)) + 1);
                                            string targetFileName = $"{PtID / 10}~01~{DateTime.Now:yyyy_MM_dd}~{RSBname[ReadCategory]}~RSB.pdf";
                                            resFilename = Path.Combine(gazouFolder, targetFileName);

                                            System.IO.File.Move(file, resFilename);
                                            messageContent = "成功";
                                            AddLog($"{PtID}:{PtName} PDFファイルが見つかりgazouフォルダに移動しました: {resFilename}");
                                            RSBreloadFlag = true;
                                            break;
                                    }

                                    // レコード更新
                                    string updateQuery = "UPDATE reqResults SET resFile = ?, resDate = ?, result = ? WHERE ID = ?";
                                    using (OleDbCommand updateCommand = new OleDbCommand(updateQuery, connection))
                                    {
                                        updateCommand.Parameters.AddWithValue("@resFile", resFilename ?? string.Empty);
                                        updateCommand.Parameters.AddWithValue("@resDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                                        updateCommand.Parameters.AddWithValue("@result", messageContent ?? string.Empty);
                                        updateCommand.Parameters.AddWithValue("@ID", resultId);
                                        await updateCommand.ExecuteNonQueryAsync();
                                    }
                                }
                                else
                                {
                                    AddLog("すべてのresファイルの処理が終了しました");
                                    AllDataProcessed = true;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        AddLog($"{file}ファイル処理中にエラーが発生しました: {ex.Message}");
                    }
                }

                if (Properties.Settings.Default.RSBReload && RSBreloadFlag)
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = Properties.Settings.Default.RSBUrl,
                        UseShellExecute = true
                    });
                }

                AddLog("ProcessResAsyncを終了します");
                return AllDataProcessed;
            }
            catch (Exception ex)
            {
                AddLog($"エラーが発生しました: {ex.Message}");
                return false;
            }
        }

        private async Task<string> ProcessDrugInfoAsync(long ptID, XmlDocument xmlDoc)
        {
            string connectionOQSData = $"Provider={DBProvider};Data Source={Properties.Settings.Default.OQSDrugData};";

            var elementMappings = new Dictionary<string, List<string>>
            {
                { "MonthInf", new List<string> { "MeTrMonthInf", "CzDiMonthInf", "ShPrMonthInf" } },
                { "Org", new List<string> { "DiOrg", "CzDiOrg", "ShPrOrg" } },
                { "DiHCd", new List<string> { "MeTrDiHCd", "CzMeTrDiHCd", "ShMeTrDiHCd" } },
                { "HCd", new List<string> { "PrlsHCd", "CzPrlsHCd", "ShPrHCd" } },
                { "DiHNm", new List<string> { "MeTrDiHNm", "CzMeTrDiHNm", "ShMeTrDiHNm" } },
                { "Month", new List<string> { "MeTrMonth", "CzDiMonth", "ShPrMonth" } },
                { "HNm", new List<string> { "PrlsHNm", "CzPrlsHNm", "ShPrHNm" } },
                { "IsOrg", new List<string> { "PrIsOrg", "CzPrIsOrg", "ShPrIsOrg" } },
                { "Cl", new List<string> { "InOut", "CzPrCl", "ShPrCl" } },
                { "DateInf", new List<string> { "DiDateInfs/DiDateInf", "CzDiDateInfs/CzDiDateInf", "ShPrDateInfs/ShPrDateInf" } },
                { "DiDate", new List<string> { "DiDate", "CzDiDate", "ShPrDate" } },
                { "PrDate", new List<string> { "PrDate", "CzPrDate", "ShPrDate" } },
                { "DrugInf", new List<string> { "DrugInfs/DrugInf", "CzDrugInfs/CzDrugInf", "ShDrugInfs/ShDrugInf" } },
                { "DrugC", new List<string> { "DrugC", "CzDrugC", "ShDrugC" } },
                { "Qua1", new List<string> { "Qua1", "CzQua1", "ShQua1" } },
                { "UsageN", new List<string> { "UsageN", "CzUsageN", "ShUsageN" } },
                { "Times", new List<string> { "Times", "CzTimes", "ShTimes" } },
                { "IngreN", new List<string> { "IngreN", "CzIngreN", "ShIngreN" } },
                { "UsageCl", new List<string> { "MeTrIdCl", "CzUsageCl", "ShUsageCl" } },
                { "Unit", new List<string> { "Unit", "CzUnit", "ShUnit" } },
                { "DrugN", new List<string> { "DrugN", "CzDrugN", "ShDrugN" } }
            };
            // 処理するルートノード
            var rootNodes = new[]
            {
                "/XmlMsg/MessageBody/MeTrMonthInf",
                "/XmlMsg/MessageBody/ShPrInf/ShPrMonthInf",
                "/XmlMsg/MessageBody/CzDiInf/CzDiMonthInf"
            };

            try
            {
                // データベース接続
                using (OleDbConnection dbConnection = new OleDbConnection(connectionOQSData))
                {
                    await dbConnection.OpenAsync();

                    // PtIDの枝番を除外
                    long ptIDMain = ptID / 10;
                    string receiveDate = DateTime.Now.ToString("yyyyMMdd");

                    // Xmlヘッダー情報の取得
                    XmlNode headerNode = xmlDoc.SelectSingleNode("/XmlMsg/MessageHeader/QuaInf");
                    if (headerNode == null)
                    {
                        return "エラー：xmlヘッダー情報の取得に失敗しました";
                    }

                    string ptName = GetNodeValue(headerNode, "Name");
                    string ptKana = GetNodeValue(headerNode, "KanaName");
                    string ptBirth = GetNodeValue(headerNode, "Birth");

                    int recordCount = 0;

                    string insertSql = @"INSERT INTO drug_history (
                                                            Source, PtID, PtIDmain, ReceiveDate, PtName, PtKana, Birth, diOrg, MeTrDiHCd,
                                                            prlsHCd, MeTrDiHNm, MeTrMonth, prlsHNm, prIsOrg, InOut, DiDate, PrDate,
                                                            DrugC, Qua1, UsageN, Times, IngreN, MeTrIdCl, Unit, DrugN
                                                        ) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)";

                    foreach (var rootPath in rootNodes)
                    {
                        XmlNodeList monthInfList = xmlDoc.SelectNodes(rootPath);

                        if (monthInfList == null || monthInfList.Count == 0)
                        {
                            continue; // 該当ノードがない場合は次のルートノードに進む
                        }

                        foreach (XmlNode monthInfNode in monthInfList)
                        {
                            int Source = 0;
                            switch (monthInfNode.Name)
                            {
                                case "MeTrMonthInf":
                                    Source = 1;
                                    break;
                                case "CzDiMonthInf":
                                    Source = 2;
                                    break;
                                case "ShPrMonthInf":
                                    Source = 3;
                                    break;
                                default:
                                    Source = 0;
                                    break;
                            }
                            int diOrg = (int)NzConvert(GetNodeValue(monthInfNode, GetMatchingNodeName(monthInfNode, elementMappings, "Org")));
                            string meTrDiHCd = GetNodeValue(monthInfNode, GetMatchingNodeName(monthInfNode, elementMappings, "DiHCd"));
                            string prlsHCd = GetNodeValue(monthInfNode, GetMatchingNodeName(monthInfNode, elementMappings, "HCd"));
                            string meTrDiHNm = GetNodeValue(monthInfNode, GetMatchingNodeName(monthInfNode, elementMappings, "DiHNm"));
                            string meTrMonth = GetNodeValue(monthInfNode, GetMatchingNodeName(monthInfNode, elementMappings, "Month"));
                            string prlsHNm = GetNodeValue(monthInfNode, GetMatchingNodeName(monthInfNode, elementMappings, "HNm"));
                            int prIsOrg = (int)NzConvert(GetNodeValue(monthInfNode, GetMatchingNodeName(monthInfNode, elementMappings, "IsOrg")));
                            int inOut = (int)NzConvert(GetNodeValue(monthInfNode, GetMatchingNodeName(monthInfNode, elementMappings, "Cl")));

                            XmlNodeList dateInfList = monthInfNode.SelectNodes(GetMatchingNodeName(monthInfNode, elementMappings, "DateInf"));
                            foreach (XmlNode dateInfNode in dateInfList)
                            {
                                string diDate = GetNodeValue(dateInfNode, GetMatchingNodeName(dateInfNode, elementMappings, "DiDate"));
                                string prDate = GetNodeValue(dateInfNode, GetMatchingNodeName(dateInfNode, elementMappings, "PrDate"));

                                //既登録か
                                string sql = "SELECT ID, Source, Revised FROM drug_history WHERE PtIDmain = ? AND MeTrDiHCd = ?  AND DiDate = ?";

                                using (OleDbCommand checkCommand = new OleDbCommand(sql, dbConnection))
                                {
                                    checkCommand.Parameters.AddWithValue("?", ptIDMain);
                                    checkCommand.Parameters.AddWithValue("?", meTrDiHCd);
                                    checkCommand.Parameters.AddWithValue("?", diDate);

                                    using (OleDbDataReader reader = checkCommand.ExecuteReader())
                                    {
                                        bool doReadDH = true;

                                        while (reader.Read())
                                        {
                                            //既存レコードあり
                                            int resultId = reader.GetInt32(reader.GetOrdinal("ID"));
                                            int resultSource = reader.GetInt32(reader.GetOrdinal("Source"));

                                            // 既存の電処or調剤由来のデータならRevised=1とする
                                            if (resultSource > Source)
                                            {
                                                string updateSql = "UPDATE drug_history SET Revised = 1 WHERE ID = ?";

                                                using (OleDbCommand updateCmd = new OleDbCommand(updateSql, dbConnection))
                                                {
                                                    updateCmd.Parameters.AddWithValue("?", resultId);
                                                    updateCmd.ExecuteNonQuery();
                                                }
                                                AddLog($"{ptName}:由来{resultSource}の既存の薬歴レコードが見つかったため、Revisedフラグをセットしました");
                                                doReadDH = true;
                                            }
                                            else //同一Sourceかレセプトから登録済み
                                            {
                                                AddLog($"{ptName}:{diDate}{meTrDiHNm}からの{resultSource}由来の既存の薬歴レコードが見つかったため、読み込みをスキップします");
                                                doReadDH = false;
                                            }
                                        }

                                        if (doReadDH)
                                        {
                                            //薬歴取込
                                            XmlNodeList drugInfList = dateInfNode.SelectNodes(GetMatchingNodeName(dateInfNode, elementMappings, "DrugInf"));
                                            foreach (XmlNode drugInfNode in drugInfList)
                                            {
                                                string drugCode = GetNodeValue(drugInfNode, GetMatchingNodeName(drugInfNode, elementMappings, "DrugC"));
                                                float quantity = NzConvert(GetNodeValue(drugInfNode, GetMatchingNodeName(drugInfNode, elementMappings, "Qua1")));
                                                string usage = GetNodeValue(drugInfNode, GetMatchingNodeName(drugInfNode, elementMappings, "UsageN"));
                                                int times = (int)NzConvert(GetNodeValue(drugInfNode, GetMatchingNodeName(drugInfNode, elementMappings, "Times")));
                                                string ingredient = GetNodeValue(drugInfNode, GetMatchingNodeName(drugInfNode, elementMappings, "IngreN"));
                                                int meTrIdCl = (int)NzConvert(GetNodeValue(drugInfNode, GetMatchingNodeName(drugInfNode, elementMappings, "UsageCl")));
                                                string unit = GetNodeValue(drugInfNode, GetMatchingNodeName(drugInfNode, elementMappings, "Unit"));
                                                string drugName = GetNodeValue(drugInfNode, GetMatchingNodeName(drugInfNode, elementMappings, "DrugN"));


                                                using (OleDbCommand insertCommand = new OleDbCommand(insertSql, dbConnection))
                                                {
                                                    insertCommand.Parameters.AddWithValue("@Source", Source);
                                                    insertCommand.Parameters.AddWithValue("@PtID", ptID);
                                                    insertCommand.Parameters.AddWithValue("@PtIDmain", ptIDMain);
                                                    insertCommand.Parameters.AddWithValue("@ReceiveDate", receiveDate);
                                                    insertCommand.Parameters.AddWithValue("@PtName", ptName);
                                                    insertCommand.Parameters.AddWithValue("@PtKana", ptKana);
                                                    insertCommand.Parameters.AddWithValue("@Birth", ptBirth);
                                                    insertCommand.Parameters.AddWithValue("@diOrg", diOrg);
                                                    insertCommand.Parameters.AddWithValue("@MeTrDiHCd", meTrDiHCd);
                                                    insertCommand.Parameters.AddWithValue("@prlsHCd", prlsHCd);
                                                    insertCommand.Parameters.AddWithValue("@MeTrDiHNm", meTrDiHNm);
                                                    insertCommand.Parameters.AddWithValue("@MeTrMonth", meTrMonth);
                                                    insertCommand.Parameters.AddWithValue("@prlsHNm", prlsHNm);
                                                    insertCommand.Parameters.AddWithValue("@prIsOrg", prIsOrg);
                                                    insertCommand.Parameters.AddWithValue("@InOut", inOut);
                                                    insertCommand.Parameters.AddWithValue("@DiDate", diDate);
                                                    insertCommand.Parameters.AddWithValue("@PrDate", prDate);
                                                    insertCommand.Parameters.AddWithValue("@DrugC", drugCode);
                                                    insertCommand.Parameters.AddWithValue("@Qua1", quantity);
                                                    insertCommand.Parameters.AddWithValue("@UsageN", usage);
                                                    insertCommand.Parameters.AddWithValue("@Times", times);
                                                    insertCommand.Parameters.AddWithValue("@IngreN", ingredient);
                                                    insertCommand.Parameters.AddWithValue("@MeTrIdCl", meTrIdCl);
                                                    insertCommand.Parameters.AddWithValue("@Unit", unit);
                                                    insertCommand.Parameters.AddWithValue("@DrugN", drugName);

                                                    await insertCommand.ExecuteNonQueryAsync();
                                                }
                                                recordCount++;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    return $"成功：xml薬歴から{recordCount}件のレコードを読み込みました";
                }
            }
            catch (Exception ex)
            {
                return "エラー：" + ex.Message;
            }
        }

        private string GetMatchingNodeName(XmlNode node, Dictionary<string, List<string>> elementMappings, string key)
        {
            if (!elementMappings.ContainsKey(key))
            {
                return key; // マッピングがなければキーをそのまま返す
            }

            foreach (var possibleName in elementMappings[key])
            {
                if (node.SelectSingleNode(possibleName) != null || node.SelectNodes(possibleName).Count > 0)
                {
                    return possibleName; // ノードが存在する最初のマッピング名を返す
                }
            }

            return key; // マッチしなかった場合、デフォルトのキーを返す
        }


        private string GetNodeValue(XmlNode node, string xpath)
        {
            XmlNode selectedNode = node.SelectSingleNode(xpath);
            return selectedNode?.InnerText ?? string.Empty;
        }

        private float NzConvert(string value, float defaultValue = 0f)
        {
            return float.TryParse(value, out float result) ? result : defaultValue;
        }

        public async Task<bool> AddFieldIfNotExists(string databasePath, string tableName, string fieldName, string fieldFormat)
        {
            // 接続文字列の設定
            string connectionString = $"Provider={DBProvider};Data Source={databasePath};";

            try
            {
                using (var connection = new OleDbConnection(connectionString))
                {
                    await connection.OpenAsync();

                    // テーブルのスキーマ情報を取得
                    DataTable schemaTable = connection.GetSchema("Columns", new string[] { null, null, tableName, null });

                    // フィールドの存在チェック
                    foreach (DataRow row in schemaTable.Rows)
                    {
                        if (row["COLUMN_NAME"].ToString().Equals(fieldName, StringComparison.OrdinalIgnoreCase))
                        {
                            AddLog($"Field '{fieldName}' already exists in table '{tableName}'.");
                            return true;
                        }
                    }

                    // フィールドが存在しない場合は追加
                    string alterTableQuery = $"ALTER TABLE [{tableName}] ADD COLUMN [{fieldName}] {fieldFormat};";

                    using (var command = new OleDbCommand(alterTableQuery, connection))
                    {
                        await command.ExecuteNonQueryAsync();
                        AddLog($"Field '{fieldName}' has been added to table '{tableName}'.");

                        if (fieldName == "Source")
                        {
                            // SQLクエリ：SourceがNULLの場合に一括で1に設定
                            string updateSql = $"UPDATE {tableName} SET Source = 1 WHERE Source IS NULL";

                            using (OleDbCommand updateCommand = new OleDbCommand(updateSql, connection))
                            {
                                // クエリを実行して、更新を反映
                                int rowsAffected = await updateCommand.ExecuteNonQueryAsync();
                                AddLog("Sourceフィールドの初期値を設定しました");
                            }
                        }
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                AddLog($"An error occurred: {ex.Message}");
                return false;
            }
        }

        private async Task<bool> RemainResTask() //resを受け取っていないレコードを検索
        {
            string connectionOQSData = $"Provider={DBProvider};Data Source={Properties.Settings.Default.OQSDrugData};";

            try
            {
                using (OleDbConnection connection = new OleDbConnection(connectionOQSData))
                {
                    await connection.OpenAsync();

                    // 1. reqDate が1日以上前で resFile と result が NULL のレコードを削除
                    string updateQuery = @" DELETE FROM  reqResults
                                            WHERE reqDate < ? AND resFile IS NULL AND result IS NULL";

                    using (OleDbCommand updateCommand = new OleDbCommand(updateQuery, connection))
                    {
                        DateTime oneDayAgo = DateTime.Now.AddDays(-1);

                        // パラメータを設定
                        updateCommand.Parameters.AddWithValue("?", oneDayAgo.ToString("yyyy-MM-dd HH:mm:ss")); // 1日以上前

                        int updatedRows = await updateCommand.ExecuteNonQueryAsync();
                        if (updatedRows > 0) {
                            AddLog($"タイムアウトデータ{updatedRows}件削除しました");
                        }
                    }

                    // 2. resFile と result が NULL のレコードを検索
                    string checkQuery = @"
                                        SELECT COUNT(*)
                                        FROM reqResults
                                        WHERE resFile IS NULL AND result IS NULL";

                    using (OleDbCommand checkCommand = new OleDbCommand(checkQuery, connection))
                    {
                        int recordCount = (int)await checkCommand.ExecuteScalarAsync();

                        // レコードが存在すれば true、なければ false を返す
                        return recordCount > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                // 必要に応じてログを出力するなどのエラーハンドリング
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }
        }

        private long Name2ID(string ptName, string strBirth, DataTable dataTable)
        {
            long maxPtID = 0;

            var rows = dataTable.Select($"氏名 = '{ptName}' AND 生年月日西暦 = '{strBirth}' AND カルテ番号 IS NOT NULL");
            if (rows.Length == 0)
            {
                return 0;
            }

            // 条件に合う行の中で最大のカルテ番号を取得
            maxPtID = rows
                .Where(row => row["カルテ番号"] != DBNull.Value)  // DBNull.Value を除外
                .Max(row => Convert.ToInt64(row["カルテ番号"]));

            return maxPtID;
        }

        private void buttonViewer_Click(object sender, EventArgs e)
        {
            // Form3がすでに開いているか確認
            if (form3Instance == null || form3Instance.IsDisposed)
            {
                Form3 form3 = new Form3(this);

                // 前回の位置とサイズを復元
                if (Properties.Settings.Default.ViewerBounds != Rectangle.Empty)
                {
                    form3.StartPosition = FormStartPosition.Manual;
                    form3.Bounds = Properties.Settings.Default.ViewerBounds;
                }

                // TopMost状態を設定
                form3.TopMost = Properties.Settings.Default.ViewerTopmost;

                // Form3が閉じるときに位置、サイズ、TopMost状態を保存
                form3.FormClosing += (s, args) =>
                {
                    SaveViewerSettings(form3);
                };

                form3.Show(this);
            }
            else
            {
                // Form3が開いている場合、LoadDataIntoComboBoxes()を実行
                Task.Run(async () =>
                    await form3Instance.LoadDataIntoComboBoxes()
                );
                // すでに開いている場合はアクティブにする
                form3Instance.Activate();

            }
        }

        private void checkBoxAutoview_CheckedChanged(object sender, EventArgs e)
        {

            autoRSB = checkBoxAutoview.Checked;
            Properties.Settings.Default.autoRSB = autoRSB;
            Properties.Settings.Default.Save();

            if (autoRSB)
            {

                //temp_rs.txtが有効か確認
                if (File.Exists(Properties.Settings.Default.temprs))
                {
                    InitializeFileWatcher();

                }
                else
                {
                    MessageBox.Show($"{Properties.Settings.Default.temprs}がみつかりません");
                    checkBoxAutoview.Checked = false;
                    autoRSB = false;
                    Properties.Settings.Default.autoRSB = autoRSB;
                    Properties.Settings.Default.Save();
                }
            } else
            {
                stopFileWatcher();

            }
        }

        private void stopFileWatcher()
        {
            if (fileWatcher != null)
            {
                fileWatcher.EnableRaisingEvents = false;
                fileWatcher.Dispose();
            }
            AddLog("RSB連携を終了しました");
        }

        private void InitializeFileWatcher()
        {
            // FileSystemWatcherを作成し、監視対象のディレクトリとファイルを指定
            fileWatcher = new FileSystemWatcher();
            fileWatcher.Path = Path.GetDirectoryName(Properties.Settings.Default.temprs);  // ファイルがあるディレクトリ
            fileWatcher.Filter = Path.GetFileName(Properties.Settings.Default.temprs);     // ファイル名でフィルタリング
            fileWatcher.NotifyFilter = NotifyFilters.LastWrite; // 最終書き込み変更を監視

            // 監視イベントハンドラを設定
            fileWatcher.Changed += FileWatcher_Changed;

            // 監視を開始
            fileWatcher.EnableRaisingEvents = true;
            AddLog("RSB連携を開始しました");
        }

        // ファイルが変更されたときに呼ばれるイベントハンドラ
        private async void FileWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            // ファイル内容の読み取り
            try
            {
                string fileContent = System.IO.File.ReadAllText(Properties.Settings.Default.temprs);
                AddLog($"RSB連携ファイルの変更を検知：{fileContent}");

                // 数値に変換を試みる
                if (long.TryParse(fileContent, out long idValue))
                {
                    // 数値に変換できた場合、特定の関数を実行
                    tempId = idValue;

                    if (await existDrugHistory(tempId))
                    {
                        AddLog($"{tempId}の薬歴を開きます");
                        buttonViewer_Click(toolStripButtonViewer, EventArgs.Empty);
                    }
                }
                else
                {
                    AddLog("RSB連携ファイルの内容が数値に変換できませんでした。");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"FileWatcherエラー: {ex.Message}");
            }
        }

        private async Task<bool> existDrugHistory(long PtIDmain) //resを受け取っていないレコードを検索
        {
            string connectionOQSData = $"Provider={DBProvider};Data Source={Properties.Settings.Default.OQSDrugData};";

            try
            {
                using (OleDbConnection connection = new OleDbConnection(connectionOQSData))
                {
                    await connection.OpenAsync();

                    // SQL文の構文を修正
                    string sql = "SELECT COUNT(*) FROM drug_history WHERE PtIDmain = ?;";

                    using (OleDbCommand command = new OleDbCommand(sql, connection))
                    {
                        // パラメータを追加
                        command.Parameters.AddWithValue("?", PtIDmain);

                        // レコードのカウントを取得
                        int count = (int)await command.ExecuteScalarAsync();

                        // レコードが1つ以上存在するか確認
                        return count > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                // エラーハンドリング（必要に応じて適宜実装）
                AddLog($"existDrugHistory Error: {ex.Message}");
                return false;
            }

        }

        private void toolStripButtonVersion_Click(object sender, EventArgs e)
        {
            FormVersion formVersion = new FormVersion();
            formVersion.ShowDialog(this);
        }

        private void LoadViewerSettings()
        {
            // デフォルトの ViewerBounds を Form1 の現在位置 + オフセットで設定
            if (Properties.Settings.Default.ViewerBounds == Rectangle.Empty)
            {
                int offsetX = 100; // X方向のオフセット
                int offsetY = 100; // Y方向のオフセット

                // Form1 の現在位置を基準に初期位置を設定
                Properties.Settings.Default.ViewerBounds = new Rectangle(
                    this.Location.X + offsetX,
                    this.Location.Y + offsetY,
                    1280,  // デフォルトの幅
                    680   // デフォルトの高さ
                );
            }

          
            // 設定を保存
            Properties.Settings.Default.Save();
        }

        private void SaveViewerSettings(Form3 form)
        {
            // 現在の位置とサイズを保存
            Properties.Settings.Default.ViewerBounds = form.Bounds;
            Properties.Settings.Default.Save();
        }

        private void ConfigureDataGridView(DataGridView dataGridView)
        {
            if (dataGridView.InvokeRequired)
            {
                dataGridView.Invoke((MethodInvoker)(() => ConfigureDataGridView(dataGridView)));
                return;
            }

            // レコードセレクタを非表示にする
            dataGridView.RowHeadersVisible = false;

            // カラム幅を自動調整する
            dataGridView.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);

            // 行の高さを変更できないようにする
            dataGridView.AllowUserToResizeRows = false;
            // レコードセレクタを非表示にする
            dataGridView.RowHeadersVisible = false;

            // カラム幅を自動調整する
            dataGridView.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);

            // ソート機能を無効にする
            dataGridView.AllowUserToOrderColumns = false;
            // 各列のソートモードを無効にする
            foreach (DataGridViewColumn column in dataGridView.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }

            
            // 縦方向の罫線を非表示にする
            dataGridView.CellBorderStyle = DataGridViewCellBorderStyle.Raised;



        }


        private void dataGridView1_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {
            // 右クリックかどうかを確認
            if (e.Button == MouseButtons.Right)
            {
                // クリックされた行を選択状態にする
                dataGridView1.Rows[e.RowIndex].Selected = true;

                // コンテキストメニューを表示（必要なら設定）
                ContextMenu contextMenu = new ContextMenu();
                contextMenu.MenuItems.Add(new MenuItem("行を削除", async (s, args) => await DeleteRow(e.RowIndex)));
                contextMenu.Show(dataGridView1, dataGridView1.PointToClient(Cursor.Position));
            }
        }

        private async Task DeleteRow(int rowIndex)
        {
            if (rowIndex >= 0 && rowIndex < dataGridView1.Rows.Count)
            {
                if(MessageBox.Show("この取得履歴を削除しますか？\n 削除すると再取得間隔がリセットされますが、取得済データは消えません","削除の確認",MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    // IDフィールドの値を取得 
                    object idValue = dataGridView1.Rows[rowIndex].Cells["ID"].Value;
                    if (idValue != null)
                    {
                        // 親データ削除
                        await DeleteReqResultsRecord(idValue);

                        await reloadDataAsync();
                    }
                }
            }
        }

        private async Task DeleteReqResultsRecord(object idValue)
        {
            try
            {
                string connectionOQSData = $"Provider={DBProvider};Data Source={Properties.Settings.Default.OQSDrugData};";
                using (var connection = new OleDbConnection(connectionOQSData))
                {
                    await connection.OpenAsync();
                    string query = "DELETE FROM reqResults WHERE ID = ?";
                    using (var command = new OleDbCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("?", idValue);
                        int deleteRows = await command.ExecuteNonQueryAsync();
                        AddLog($"reqResltsから{deleteRows}件のレコードを削除しました");
                    }
                    
                }
            }
            catch (Exception ex)
            {
                AddLog($"DeleteReqResultsRecordでエラー：{ex.Message}");                
            }
        }
    }
}



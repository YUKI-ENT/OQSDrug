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
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Configuration;
using Microsoft.VisualBasic;
using System.Data.Odbc;
using System.Threading;


namespace OQSDrug
{
    public partial class Form1 : Form
    {
        public long tempId = 0;
        public bool autoRSB = false, forceIdLink = false, autoTKK = false, autoSR = false;
        
        public string RSBdrive = string.Empty;

        string DynaTable = "T_資格確認結果表示";
        DataTable dynaTable = new DataTable();

        DataTable reqResultsTable = new DataTable();

        // 最新の特定健診結果を保存しておく
        private static Dictionary<long, string> TKKdate = new Dictionary<long, string>();

        byte okSettings = 0;

        //private Timer timer;
        private bool isTimerRunning = false; // タイマーの状態フラグ
        private bool isOQSRunnnig = false;   //取得開始しているか
        private bool isFormVisible = true;  //最小化

        private System.Threading.Timer backgroundTimer; //非同期タイマー

        private FileSystemWatcher fileWatcher;
        string idFile = ""; //RSB連携
        int idStyle = 0;
        bool idChageCalled = false;
        int fileReadDelayms = 500;

        // バックアップ、ログファイル
        private static readonly string PersonalFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        private static readonly string OQSFolder = Path.Combine(PersonalFolder, "OQSDrug");
        private static readonly string LogFile = Path.Combine(OQSFolder, "OQSDrug.log");


        // 動的に追加するラベル
        private Label[] statusLabels;
        private Label[] statusTexts;

        public FormTKK formTKKInstance = null;
        public FormDI formDIInstance = null;
        public FormSR formSRInstance = null;

        // アイコンの配列を用意 (例: 3つのアイコン)
        private System.Windows.Forms.Timer animationTimer;
        private int currentFrame;
        private Icon[] icons;

        public Form1()
        {
            InitializeComponent();
            SetupTableLayout();
            //InitializeTimer();
        }

        private async Task RunTimerLogicAsync()
        {
            DateTime startTime = DateTime.Now;
            AddLog("タイマーイベント開始");

            // Status check
            okSettings = await UpdateStatus();
            Invoke(new Action(() => this.StartStop.Enabled = (okSettings == 0b1111)));

            //AutoStartStop
            if (Properties.Settings.Default.AutoStart)
            {
                Invoke(new Action(() => StartStop.Checked = (okSettings == 0b1111)));
            }

            //取得作業
            if (isOQSRunnnig)
            {
                if (okSettings != 0b1111)
                {   // Running->NG->Stop
                    Invoke(new Action(() => StartStop.Checked = false));
                }
                else
                {
                    await UpdateClientAsync();

                    // Datadynaのデータ取得
                    dynaTable.Clear();
                    dynaTable = await LoadDataFromDatabaseAsync(Properties.Settings.Default.Datadyna);

                    if (dynaTable != null)
                    {
                        // 薬剤PDF
                        if (Properties.Settings.Default.DrugFileCategory % 2 == 0) //xml
                        {
                            MakeReq(Properties.Settings.Default.DrugFileCategory + 10, dynaTable);
                        }
                        else
                        {
                            MakeReq((Properties.Settings.Default.DrugFileCategory % 10) + 1 + 10, dynaTable); //xml
                            MakeReq((Properties.Settings.Default.DrugFileCategory % 10) + 10, dynaTable);  //pdf
                        }

                        // 健診PDF
                        
                        if (Properties.Settings.Default.KensinFileCategory == 1)
                        {
                            MakeReq(102, dynaTable);
                            MakeReq(101, dynaTable); //固定間隔
                        }
                        else // xmlのみ、Or 健診日によってPDF日付を変える場合
                        {
                            MakeReq(102, dynaTable); // xmlを先行、取り込み後健診実施日を確定、TKKdateに設定, ProcessResAsyncで再度MakeReq
                        }
                    }
                    await reloadDataAsync();

                    // Resフォルダの処理
                    bool processCompleted = false;
                    bool isRemainRes = true;

                    // 5秒ごとにProcessResAsyncを呼び出し
                    while ((!processCompleted || isRemainRes) && isOQSRunnnig)
                    {
                        await Task.Delay(5000);

                        if (!isTimerRunning || !isOQSRunnnig) break;

                        processCompleted = await ProcessResAsync();
                        if (processCompleted) AddLog("すべてのresファイルを処理しました");

                        isRemainRes = await RemainResTask();

                        if (isRemainRes && (DateTime.Now - startTime).TotalSeconds > (Properties.Settings.Default.TimerInterval - 5))
                        {
                            processCompleted = true;
                            isRemainRes = false;
                            AddLog("時間内に処理が終了しませんでしたので、タイマー処理を中止します");
                        }

                        await reloadDataAsync();
                    }
                }
            }
            else if ((okSettings & 0b0001) == 1)  //OQSDrugData OK
            {
                //取得停止中はreloadのみ
                await reloadDataAsync();
            }
            AddLog($"タイマーイベント終了");
        }

        public void StartTimer()
        {
            backgroundTimer = new System.Threading.Timer(async _ =>
            {
                if (!isTimerRunning)
                {
                    isTimerRunning = true;
                    await RunTimerLogicAsync();
                    isTimerRunning = false;
                }
            }, null, 0, Properties.Settings.Default.TimerInterval * 1000);
        }

        public void StopTimer()
        {
            backgroundTimer?.Dispose();
            backgroundTimer = null;
        }


        // フォームが閉じられるときにタイマーを停止
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            //if (timer != null)
            //{
            //    timer.Stop();
            //    timer.Dispose();
            //}
            if (backgroundTimer != null)
            {
                backgroundTimer.Dispose();
            }

        }

        // データベースの内容を読み込み、DataGridViewに表示
        private async Task reloadDataAsync(bool skipSql = false)
        {
            if (!await CommonFunctions.WaitForDbUnlock(1000))
            {
                AddLog("データベースがロックされていたためreloadDataAsyncをスキップししました");
            }
            else
            {
                string sql = "SELECT CategoryName,  PtID, PtName, result, reqDate, reqFile, resDate, resFile, category, ID FROM reqResults WHERE reqDate > ?  ORDER BY reqResults.ID DESC";

                try
                {
                    using (DataTable dt = new DataTable())
                    {

                        if (!skipSql) // 最小化からの復帰時のみskipする
                        {
                            using (OleDbConnection connection = new OleDbConnection(CommonFunctions.connectionOQSdata))
                            {
                                // 接続を開く
                                await connection.OpenAsync();

                                // データを取得してDataTableに格納
                                using (var command = new OleDbCommand(sql, connection))
                                {
                                    command.Parameters.AddWithValue("?", DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd HH:mm:ss"));

                                    CommonFunctions.DataDbLock = true;

                                    using (var reader = await command.ExecuteReaderAsync())
                                    {
                                        dt.Load(reader);
                                    }
                                    CommonFunctions.DataDbLock = false;
                                }
                            }
                            reqResultsTable = dt;
                            AddLog("reqResultsテーブルを読み込みました");
                        }
                        if (isFormVisible)
                        {
                            // DataGridViewに表示
                            dataGridView1.Invoke(new Action(() =>
                            {
                                //UI スレッド
                                dataGridView1.DataSource = reqResultsTable;
                                ConfigureDataGridView(dataGridView1);
                            }));
                            AddLog("DataGridViewを更新しました");
                        }
                    }
                }
                catch (Exception ex)
                {
                    // エラー処理
                    AddLog($"エラー: {ex.Message}");
                }
                finally { CommonFunctions.DataDbLock = false; }
            }

        }


        private void toolStripButtonSettings_Click(object sender, EventArgs e)
        {
            //動作中の場合は停止する
            if (isTimerRunning)
            {
                Invoke(new Action(() => MessageBox.Show("一旦タイマー動作を停止します")));
                
                StartStop.Checked = false;
                
                StopTimer();
            }

            //Form2を開く
            Form2 form2 = new Form2(this);
            form2.ShowDialog(this);

            //Form2閉じたあと

            initializeForm();

            StartTimer();

        }

        private async void StartStop_CheckedChanged(object sender, EventArgs e)
        {
            if (StartStop.Checked)
            {
                if (await IsAccessAllowedAsync())
                {

                    //開始
                    StartStop.Text = "停止";
                    StartStop.Image = Properties.Resources.Stop;
                    animationTimer?.Start();

                    //StartTimer();
                    isOQSRunnnig = true;

                    AddLog($"タイマー処理を開始します。間隔は{Properties.Settings.Default.TimerInterval}秒です");

                }
                else
                {
                    StartStop.Checked = false ;

                    MessageBox.Show("他のPCで取込操作を行っているようです。2箇所以上で取込を行うとデータ競合が起こりますので、取込は1箇所でお願いします。\n" +
                        "薬歴や健診のビュワーとして使うときは、開始ボタンは押さずに利用してください。");

                    animationTimer?.Stop();
                    notifyIcon1.Icon = Properties.Resources.drug1;
                }
                //初回実行
                //await Task.Run(() => Timer_Tick(timer, EventArgs.Empty));
            }
            else
            {
                StartStop.Text = "開始";
                StartStop.Image = Properties.Resources.Go;
                //timer.Stop();
                animationTimer?.Stop();
                notifyIcon1.Icon = Properties.Resources.drug1;

                //StopTimer();
                isOQSRunnnig = false;

                await DeleteClientAsync();

                AddLog("タイマー処理を終了します");
            }
        }

        public async void MakeReq(int category, DataTable dynaTable, long targetId = 0) //Category: 11:薬剤pdf, 12:薬剤xml、13:薬剤診療pdf、14：薬剤診療xml、101：健診pdf、102：健診xml
        {
            int Span = Properties.Settings.Default.YZspan;
            int YZinterval = Properties.Settings.Default.YZinterval, KSinterval = Properties.Settings.Default.KSinterval, Interval;
            string dynaPath = Properties.Settings.Default.Datadyna;
            string douiFlag = "", douiDate = "";
            DateTime checkDate;
            bool doReq = true;
            int fileCategory = 0; //1:PDF, 2:xml
            string OQSpath = Properties.Settings.Default.OQSFolder;
            string CategoryName = "";

            string startDate = DateTime.Now.AddMonths(-Span).ToString("yyyyMM");
            string endDate = DateTime.Now.ToString("yyyyMM");

            string reqDateSQL = "SELECT TOP 1 reqDate, ID FROM reqResults " +
                       "WHERE PtID = ? AND category = ? ORDER BY ID DESC";

            // レコードの更新クエリ
            string updateRecordSQL = "UPDATE reqResults SET reqFile = ?, reqDate = ?, resFile = NULL, resDate= NULL, result = NULL  WHERE ID = ?";

            // レコードの新規追加クエリ
            string insertRecordSQL = "INSERT INTO reqResults (Category, PtID, PtName, reqFile, reqDate, CategoryName) VALUES (?, ?, ?, ?, ?, ?)";

            switch (category)
            {
                case 11:
                    douiFlag = "薬剤情報閲覧同意フラグ";
                    douiDate = "薬剤情報閲覧有効期限";
                    Interval = YZinterval;  // Replace with appropriate interval
                    fileCategory = 1; //1:PDF, 2:xml
                    CategoryName = "薬剤PDF";
                    AddLog("PDF薬剤情報取得用reqファイルを作成します");
                    break;
                case 101:
                    douiFlag = "特定検診情報閲覧同意フラグ";
                    douiDate = "特定検診情報閲覧有効期限";
                    Interval = KSinterval;
                    fileCategory = 1;
                    CategoryName = "健診PDF";
                    AddLog("特定検診PDF情報取得用reqファイルを作成します");
                    break;
                case 102:
                    douiFlag = "特定検診情報閲覧同意フラグ";
                    douiDate = "特定検診情報閲覧有効期限";
                    Interval = KSinterval;
                    fileCategory = 2;
                    CategoryName = "健診xml";
                    AddLog("特定検診xml情報取得用reqファイルを作成します");
                    break;
                case 12:
                    douiFlag = "薬剤情報閲覧同意フラグ";
                    douiDate = "薬剤情報閲覧有効期限";
                    Interval = YZinterval;  // Replace with appropriate interval
                    fileCategory = 2;
                    CategoryName = "薬剤xml";
                    AddLog("xml薬剤情報取得用reqファイルを作成します");
                    break;
                case 13:
                    douiFlag = "薬剤情報閲覧同意フラグ";
                    douiDate = "薬剤情報閲覧有効期限";
                    Interval = YZinterval;  // Replace with appropriate interval
                    fileCategory = 1;
                    CategoryName = "薬剤診療PDF";
                    AddLog("PDF薬剤診療情報取得用reqファイルを作成します");
                    break;
                case 14:
                    douiFlag = "薬剤情報閲覧同意フラグ";
                    douiDate = "薬剤情報閲覧有効期限";
                    Interval = YZinterval;  // Replace with appropriate interval
                    fileCategory = 2;
                    CategoryName = "薬剤診療xml";
                    AddLog("xml薬剤診療情報取得用reqファイルを作成します");
                    break;
                default:
                    AddLog("Invalid category");
                    return;
            }

            checkDate = fileCategory == 1 ? DateTime.Now.AddMonths(-Interval) : DateTime.Now.AddHours(-6); // xmlの場合は取得間隔6時間
            if (category == 102) checkDate = DateTime.Now.AddDays(-1); //特定健診xmlは1日1回

            if (targetId > 0) checkDate = DateTime.Now; //強制受信

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

                        if (ptId == 0 && targetId > 0) ptId = targetId;

                        AddLog($"カルテ番号{ptId}を取得しました。処理を継続します");
                    }

                    if (targetId == 0 || ptId == targetId)
                    {
                        if (!IsDateStringAfterNow(DouiRow[douiDate].ToString()))
                        {
                            AddLog($"{ptId}:{ptName}さんの同意有効期限が切れているのでスキップします");
                        }
                        else if (ptId == 0)
                        { //再取得もできない場合はスキップ
                            AddLog($"{ptName}さんのカルテ番号の取得ができなかったため処理をスキップします");
                        }
                        else
                        {   // 同意有効
                            if (!await CommonFunctions.WaitForDbUnlock(1000))
                            {
                                AddLog("データベースがロックされています。Makereq処理をスキップします");
                            }
                            else
                            {
                                CommonFunctions.DataDbLock = true;
                                try
                                {
                                    using (OleDbConnection connData = new OleDbConnection(CommonFunctions.connectionOQSdata))
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
                                                        AddLog($"{ptId}:{ptName}さんの{CategoryName}reqを作成します（更新）");
                                                    }
                                                    else
                                                    {
                                                        AddLog($"{ptId}:{ptName}さんの{CategoryName}は再取得期間外なのでスキップします");
                                                    }
                                                }
                                                else
                                                {
                                                    AddLog($"{ptId}:{ptName}さんの{CategoryName}reqを作成します（新規）");
                                                    doReq = true;
                                                }

                                                CommonFunctions.DataDbLock = false;

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
                                                        AddLog($"{ptId}:{ptName}さんの{CategoryName}req生成成功");
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

                                                        AddLog($"{ptId}:{ptName}さんの取得結果更新しました");
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
                                                        insertCmd.Parameters.AddWithValue("?", CategoryName);
                                                        await insertCmd.ExecuteNonQueryAsync();

                                                        AddLog($"{ptId}:{ptName}さん所得結果作成しました (新規追加)");
                                                        //reqReadyFlag = true;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    AddLog($"Makereqでエラー：{ex.ToString()}");
                                }
                                finally { CommonFunctions.DataDbLock = false; }
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
            string connectionString = $"Provider={CommonFunctions.DBProvider};Data Source={dynaPath};Mode=Read;Persist Security Info=False;";
            string query = "SELECT * FROM " + Properties.Settings.Default.DynaTable + ";";

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

                AddLog("ダイナミクスの" + Properties.Settings.Default.DynaTable + "の取り込みが完了しました");
            }
            catch (Exception ex)
            {
                AddLog($"ダイナミクスの読み込みエラー:{ex.Message}");
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
                string connectionString = $"Provider={CommonFunctions.DBProvider};Data Source={dbPath};";

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

        //RSBaseの妥当性チェック：PDF保存先が確保されているか
        private async Task<string> CheckRSBaseSetting()
        {
            int Yz = Properties.Settings.Default.DrugFileCategory;
            int Ks = Properties.Settings.Default.KensinFileCategory;
            string returnString = "";

            //検査登録のみの場合、gazouフォルダの有無
            if(Yz % 2 != 0 || Ks > 0) //PDF
            {
                if(Yz > 10 || Ks == 4) //SideShow PDF
                {
                    returnString = (await CheckDirectoryExistsAsync(Properties.Settings.Default.RSBServerFolder)) == "OK" ? "" : "Server " ;
                }

                if (Yz < 10 || Ks < 4) //検査登録
                {
                    returnString += (await CheckDirectoryExistsAsync(Properties.Settings.Default.RSBgazouFolder)) == "OK" ? "" : "Gazou";
                }
            }
            
            return (returnString == "") ? "OK" : $"NG:{returnString}";
        }

        private async Task<byte> UpdateStatus() //GasouF|OQSF|dyna|Data 
        {
            byte resultCode = 0;

            //DynaTable
            DynaTable = (Properties.Settings.Default.Datadyna.IndexOf("datadyna.mdb", StringComparison.OrdinalIgnoreCase) >= 0) ? "T_資格確認結果表示" : "WKO資格確認結果表示";

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
                CheckDatabaseAsync(Properties.Settings.Default.Datadyna, DynaTable),
                CheckDirectoryExistsAsync(Properties.Settings.Default.OQSFolder),
                CheckRSBaseSetting()
            };

            // 各タスクのインデックスと結果を保持するための辞書
            var taskIndexMap = tasks
                .Select((task, index) => new { task, index })
                .ToDictionary(x => x.task, x => x.index);

            while (taskIndexMap.Any())
            {
                try
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
                            // OKの場合、対応するビットを1にする
                            resultCode |= (byte)(1 << index);
                        }
                        else
                        {
                            statusTexts[index].Text = result;
                            statusTexts[index].ForeColor = Color.Red;

                            AddLog(statusLabels[index].Text + ":" + result);
                        }
                    }));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                    resultCode = 0;
                }
            }

            return resultCode;
        }

        private void InitializeDBProvider()
        {
            try
            {
                AddLog("登録されているOLE DBプロバイダを確認します...");

                // プロバイダの優先順序
                string[] preferredProviders =
                {
                    "Microsoft.Jet.OLEDB.4.0"
                    //"Microsoft.ACE.OLEDB.12.0",
                    //"Microsoft.ACE.OLEDB.15.0",
                    //"Microsoft.ACE.OLEDB.16.0"
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
                        AddLog($"DBプロバイダ: {provider}が見つかりました");
                        toolStripComboBoxDBProviders.Items.Add(provider);
                        if (CommonFunctions.DBProvider.Length == 0)
                        {
                            CommonFunctions.DBProvider = provider;
                            AddLog($"使用するプロバイダ: {CommonFunctions.DBProvider}");
                            toolStripComboBoxDBProviders.SelectedIndex = toolStripComboBoxDBProviders.Items.IndexOf(provider);
                        }
                    }
                }

                // 適切なプロバイダが見つからなかった場合
                if (CommonFunctions.DBProvider.Length == 0)
                {
                    AddLog("適切なOLE DBプロバイダが見つかりませんでした。");
                }
            }
            catch (Exception ex)
            {
                AddLog($"エラー: {ex.Message}");
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // 前バージョンからのUpgradeを実行していないときは、Upgradeを実施する
            if (Properties.Settings.Default.IsUpgrade == false)
            {
                // Upgradeを実行する
                Properties.Settings.Default.Upgrade();

                // 「Upgradeを実行した」という情報を設定する
                Properties.Settings.Default.IsUpgrade = true;

                // 現行バージョンの設定を保存する
                Properties.Settings.Default.Save();
            }

            if (Properties.Settings.Default.MinimumStart)
            {
                toolStripButtonToTaskTray_Click(sender, EventArgs.Empty);
            }

            PrepareLogFiles();

            InitAnimationTimer();

            initializeForm();

            // Timer start
            StartTimer();
        }

        private async void initializeForm()
        {
            //MessageBox.Show( getOLEProviders());
            InitializeDBProvider();
            loadConnectionString();

            SetupYZKSindicator();

            listViewLog.Columns.Add("TimeStamp", 100); // 列1: タイムスタンプ
            listViewLog.Columns.Add("Log");   // 列2: メッセージ

            okSettings = await UpdateStatus();

            await setStatus();

            autoRSB = Properties.Settings.Default.autoRSB;
            autoTKK = Properties.Settings.Default.autoTKK;
            autoSR = Properties.Settings.Default.autoSR;

            checkBoxAutoview.CheckedChanged -= checkBoxAutoview_CheckedChanged;
            checkBoxAutoTKK.CheckedChanged -= checkBoxAutoview_CheckedChanged;
            checkBoxAutoSR.CheckedChanged -= checkBoxAutoview_CheckedChanged;

            checkBoxAutoview.Checked = autoRSB;
            checkBoxAutoTKK.Checked = autoTKK;
            checkBoxAutoSR.Checked = autoSR;

            checkBoxAutoview.CheckedChanged += checkBoxAutoview_CheckedChanged;
            checkBoxAutoTKK.CheckedChanged += checkBoxAutoview_CheckedChanged;
            checkBoxAutoSR.CheckedChanged += checkBoxAutoview_CheckedChanged;

            checkBoxAutoview_CheckedChanged(this, EventArgs.Empty); //初回実行してFileWatcherを起動させる

            checkBoxAutoStart.Checked = Properties.Settings.Default.AutoStart;

            LoadViewerSettings();

            InitNotifyIcon();

            if ((okSettings & (0b0001)) == 1) //OQSDrugData OK
            {
                AddLog("特定健診基準値データを読み込みます");
                CommonFunctions.TKKreferenceDict = await CommonFunctions.LoadTKKReference();
                AddLog($"{CommonFunctions.TKKreferenceDict.Count}件のデータを読み込みました");

                await LoadKoroDataAsync();
            }

            RSBdrive = await GetRSBdrive();
            if (!string.IsNullOrEmpty(RSBdrive))
            {
                await LoadRSBDIAsync(RSBdrive + @"\Users\rsn\public_html\drug_RSB.dat");
            }

        }

        private async Task setStatus()
        {
            if ((okSettings & (0b0001)) == 1) //OQSDrugData OK
            {
                //テーブルフィールドのアップデート
                bool fieldCheck = await AddFieldIfNotExists(Properties.Settings.Default.OQSDrugData, "drug_history", "Source", "INTEGER")
                                && await AddFieldIfNotExists(Properties.Settings.Default.OQSDrugData, "drug_history", "Revised", "YESNO")
                                && await AddFieldIfNotExists(Properties.Settings.Default.OQSDrugData, "reqResults", "CategoryName", "TEXT(12) NULL");
                if (!fieldCheck)
                {
                    Invoke(new Action(() => MessageBox.Show("OQSDrugDataのアップデートでエラーが発生しました。OQSDrug_data.mdbにアクセスできるかを調べて再起動してください")));
                }

                // TKK_history table:
                string TKKtable = "TKK_history";
                if (await CheckDatabaseAsync(Properties.Settings.Default.OQSDrugData, TKKtable) != "OK")
                {
                    AddLog($"{TKKtable}がないので、作成します");
                    string sql = $@"CREATE TABLE {TKKtable} (
                                        ID AUTOINCREMENT PRIMARY KEY,
                                        EffectiveTime TEXT(8),
                                        ItemCode TEXT(32),
                                        ItemName TEXT(128),
                                        DataType TEXT(4),
                                        DataValue TEXT(64),
                                        Unit TEXT(32),
                                        Oid TEXT(32) NULL,
                                        DataValueName TEXT(64) NULL,
                                        PtIDmain LONG,
                                        PtName TEXT(64) NULL,
                                        PtKana TEXT(64) NULL,
                                        Sex INTEGER
                                    )";
                    if (await CreateTableAsync(Properties.Settings.Default.OQSDrugData, sql))
                    {
                        AddLog($"{TKKtable}を作成しました");
                    }
                    else
                    {
                        AddLog($"{TKKtable}の作成に失敗しました");
                    }
                }

                // TKK_reference table:
                string TKKreference = "TKK_reference";

                if (await CheckDatabaseAsync(Properties.Settings.Default.OQSDrugData, TKKreference) != "OK")
                {
                    AddLog($"{TKKreference}がないので、作成します");
                    string sql = $@"CREATE TABLE {TKKreference} (
                                        ID AUTOINCREMENT PRIMARY KEY,
                                        ItemCode TEXT(32),
                                        ItemName TEXT(128),
                                        Sex INTEGER,
                                        CompairType TEXT(8) NULL,
                                        Limit1 TEXT(16) NULL,
                                        Limit2 TEXT(16) NULL,
                                        IncludeValue TEXT(16) NULL
                                    )";
                    if (await CreateTableAsync(Properties.Settings.Default.OQSDrugData, sql))
                    {
                        AddLog($"{TKKreference}を作成しました。続いて初期値を入力します");
                        if (await setInitialReferenceAsync(Properties.Settings.Default.OQSDrugData))
                        {
                            AddLog("特定健診基準初期値を設定しました");

                        }
                    }
                    else
                    {
                        AddLog($"{TKKreference}の作成に失敗しました");
                    }
                }

                // 排他処理用テーブル
                string exclusiveTable = "connectedClient";
                if (await CheckDatabaseAsync(Properties.Settings.Default.OQSDrugData, exclusiveTable) != "OK")
                {
                    AddLog($"{exclusiveTable}がないので、作成します");
                    string sql = $@"
                            CREATE TABLE {exclusiveTable} (
                            ID AUTOINCREMENT PRIMARY KEY,
                            clientName TEXT(32),
                            lastUpdated DATETIME
                                    )";
                    if (await CreateTableAsync(Properties.Settings.Default.OQSDrugData, sql))
                    {
                        AddLog($"{exclusiveTable}を作成しました");
                    }
                    else
                    {
                        AddLog($"{exclusiveTable}の作成に失敗しました");
                    }
                }

                // Sinryo table:
                string SinryoTable = "sinryo_history";

                if (await CheckDatabaseAsync(Properties.Settings.Default.OQSDrugData, SinryoTable) != "OK")
                {
                    AddLog($"{SinryoTable}がないので、作成します");
                    string sql = $@"CREATE TABLE {SinryoTable} (
                                        id AUTOINCREMENT PRIMARY KEY,
                                        PtID LONG,
                                        PtIDmain LONG,
                                        PtName TEXT(255),
                                        PtKana TEXT(255),
                                        Birth TEXT(10),
                                        Sex INT,
                                        MeTrDiHCd TEXT(12) NULL,
                                        MeTrDiHNm TEXT(255) NULL,
                                        MeTrMonth TEXT(10) NULL,
                                        DiDate TEXT(10) NULL,
                                        SinInfN TEXT(255) NULL,
                                        SinInfCd TEXT(12) NULL,
                                        MeTrIdCl TEXT(12) NULL,
                                        Qua1 SINGLE,
                                        Times LONG,
                                        Unit TEXT(50) NULL,
                                        ReceiveDate TEXT(10) NULL
                                    )";
                    if (await CreateTableAsync(Properties.Settings.Default.OQSDrugData, sql))
                    {
                        AddLog($"{SinryoTable}を作成しました");
                    }
                    else
                    {
                        AddLog($"{SinryoTable}の作成に失敗しました");
                    }
                }

                await reloadDataAsync();
            }

            this.StartStop.Enabled = (okSettings == 0b1111);

        }

        private async Task<bool> setInitialReferenceAsync(string databasePath)
        {
            string connectionString = $"Provider={CommonFunctions.DBProvider};Data Source={databasePath};";
            bool result = false;

            // 初期データ
            var initialData = new[]
            {
            new { ItemCode = "9A755000000000001", ItemName = "収縮期血圧(その他)", CompairType = "<", Limit1 = "130", Limit2 = "140", IncludeValue = "" , Sex= 0},
            new { ItemCode = "9A751000000000001", ItemName = "収縮期血圧(1回目)", CompairType = "<", Limit1 = "130", Limit2 = "140", IncludeValue = "" , Sex= 0},
            new { ItemCode = "9A752000000000001", ItemName = "収縮期血圧(2回目)", CompairType = "<" , Limit1 = "130", Limit2 = "140", IncludeValue = "" , Sex= 0},
            new { ItemCode = "9A765000000000001", ItemName = "拡張期血圧(その他)", CompairType = "<" , Limit1 = "85", Limit2 = "90", IncludeValue = "" , Sex= 0},
            new { ItemCode = "9A761000000000001", ItemName = "拡張期血圧(1回目)", CompairType = "<" , Limit1 = "85", Limit2 = "90", IncludeValue = "" , Sex= 0},
            new { ItemCode = "9A762000000000001", ItemName = "拡張期血圧(2回目)", CompairType = "<", Limit1 = "85", Limit2 = "90", IncludeValue = "" , Sex= 0},
            new { ItemCode = "3F015000002327101", ItemName = "中性脂肪（トリグリセリド）", CompairType = "<", Limit1 = "150", Limit2 = "300", IncludeValue = "" , Sex= 0},
            new { ItemCode = "3F070000002327101", ItemName = "HDLコレステロール", CompairType = ">", Limit1 = "39", Limit2 = "34", IncludeValue = "" , Sex= 0},
            new { ItemCode = "3F077000002327101", ItemName = "LDLコレステロール", CompairType = "<", Limit1 = "120", Limit2 = "140", IncludeValue = "" , Sex= 0},
            new { ItemCode = "3B035000002327201", ItemName = "GOT(AST)", CompairType = "<", Limit1 = "31", Limit2 = "51", IncludeValue = "" , Sex= 0},
            new { ItemCode = "3B045000002327201", ItemName = "GPT(ALT)", CompairType = "<", Limit1 = "31", Limit2 = "51", IncludeValue = "" , Sex= 0},
            new { ItemCode = "3B090000002327101", ItemName = "γ-GT(γ-GTP)", CompairType = "<", Limit1 = "51", Limit2 = "101", IncludeValue = "" , Sex= 0},
            //new { ItemCode = "3C015000002327101", ItemName = "血清クレアチニン", CompairType = "", Limit1 = "", Limit2 = "", IncludeValue = "" , Sex= 0},
            new { ItemCode = "8A065000002391901", ItemName = "eGFR", CompairType = ">", Limit1 = "60", Limit2 = "45", IncludeValue = "" , Sex= 0},
            new { ItemCode = "3D010000001927201", ItemName = "空腹時血糖", CompairType = "<", Limit1 = "100", Limit2 = "126", IncludeValue = "" , Sex= 0},
            new { ItemCode = "3D046000001920402", ItemName = "HbA1c（ＮＧＳＰ値）", CompairType = "<", Limit1 = "5.6", Limit2 = "6.5", IncludeValue = "" , Sex= 0},
            new { ItemCode = "1A020000000190111", ItemName = "尿糖", CompairType = "=", Limit1 = "+-", Limit2 = "+", IncludeValue = "(-)" , Sex= 0},
            new { ItemCode = "1A010000000190111", ItemName = "尿蛋白", CompairType = "=", Limit1 = "+-", Limit2 = "+", IncludeValue = "(-)" , Sex= 0},
            //new { ItemCode = "2A040000001930102", ItemName = "ヘマトクリット値", CompairType = "", Limit1 = "", Limit2 = "", IncludeValue = "" , Sex= 0},
            new { ItemCode = "2A030000001930101", ItemName = "血色素量(ヘモグロビン値)", CompairType = ">", Limit1 = "12.0", Limit2 = "13.0", IncludeValue = "" , Sex= 1},
            new { ItemCode = "2A030000001930101", ItemName = "血色素量(ヘモグロビン値)", CompairType = ">", Limit1 = "11.0", Limit2 = "12.0", IncludeValue = "" , Sex= 2}
            //new { ItemCode = "2A020000001930101", ItemName = "赤血球数", CompairType = "", Limit1 = "", Limit2 = "", IncludeValue = "" , Sex= 0},
            //new { ItemCode = "9A110160700000011", ItemName = "心電図(所見の有無)", CompairType = "", Limit1 = "", Limit2 = "", IncludeValue = "" , Sex= 0},
            //new { ItemCode = "9A110160800000049", ItemName = "心電図所見", CompairType = "", Limit1 = "", Limit2 = "", IncludeValue = "" , Sex= 0}
        };

            try
            {
                using (var connection = new OleDbConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string insertQuery = "INSERT INTO TKK_reference (ItemCode, ItemName, CompairType, Limit1, Limit2, IncludeValue, Sex) VALUES (?, ?, ?, ?, ?, ?, ?);";

                    foreach (var data in initialData)
                    {
                        using (var command = new OleDbCommand(insertQuery, connection))
                        {
                            command.Parameters.AddWithValue("@ItemCode", data.ItemCode);
                            command.Parameters.AddWithValue("@ItemName", data.ItemName);
                            command.Parameters.AddWithValue("@DataType", data.CompairType);
                            command.Parameters.AddWithValue("@Limit1", data.Limit1);
                            command.Parameters.AddWithValue("@Limit2", data.Limit2);
                            command.Parameters.AddWithValue("@IncludeValue", data.IncludeValue);
                            command.Parameters.AddWithValue("@Sex", data.Sex);

                            await command.ExecuteNonQueryAsync();
                        }
                    }
                    result = true;
                }
            }
            catch (Exception ex)
            {
                result = false;
                AddLog($"setInitialReferenceAsyncでエラー{ex.Message}");
            }
            return result;
        }

        private async Task<bool> CreateTableAsync(string databasePath, string createTableQuery)
        {
            string connectionString = $"Provider={CommonFunctions.DBProvider};Data Source={databasePath};";
            bool result = false;

            using (var connection = new OleDbConnection(connectionString))
            {
                try
                {
                    await connection.OpenAsync();

                    using (var command = new OleDbCommand(createTableQuery, connection))
                    {
                        await command.ExecuteNonQueryAsync();
                    }
                    result = true;
                }
                catch (Exception ex)
                {
                    result = false;
                    AddLog($"CreateTableAsyncでエラー{ex.Message}");
                }
            }
            return result;
        }

        private async void AddLog(string message)
        {
            if (listViewLog.InvokeRequired)
            {
                // UIスレッドに処理を委譲
                listViewLog.Invoke(new Action(() => AddLog(message)));
            }
            else
            {
                // メインスレッド上でUI操作
                string timestamp = DateTime.Now.ToString("yy-MM-dd HH:mm:ss");
                var item = new ListViewItem(timestamp);
                item.SubItems.Add(message);
                listViewLog.Items.Add(item);

                // 最大行数を超えたら古い行を削除
                if (listViewLog.Items.Count > 1000)
                {
                    listViewLog.Items.RemoveAt(0); // 最初の行を削除
                }

                // 最新のログを表示
                listViewLog.EnsureVisible(listViewLog.Items.Count - 1);

                // ファイルにログを保存
                await SaveLogToFileAsync(timestamp, message);
            }
        }

        /// <summary>
        /// ログファイルを準備（古いログをリネーム）
        /// </summary>
        private void PrepareLogFiles()
        {
            // ログフォルダが存在しない場合は作成
            if (!Directory.Exists(OQSFolder))
            {
                Directory.CreateDirectory(OQSFolder);
            }

            string Log1 = Path.Combine(OQSFolder, "OQSDrug1.log");
            string Log2 = Path.Combine(OQSFolder, "OQSDrug2.log");

            if (File.Exists(Log2)) File.Delete(Log2);
            if (File.Exists(Log1)) File.Move(Log1, Log2);
            if (File.Exists(LogFile)) File.Move(LogFile, Log1);

        }

        /// <summary>
        /// ログをファイルに非同期で追記する
        /// </summary>
        private async Task SaveLogToFileAsync(string timestamp, string message)
        {
            try
            {
                string logEntry = $"{timestamp} {message}";
                using (StreamWriter writer = new StreamWriter(LogFile, append: true))
                {
                    await writer.WriteLineAsync(logEntry);
                }
            }
            catch (Exception ex)
            {
                // ログ保存エラー時の処理
                Console.WriteLine($"ログ保存中にエラーが発生しました: {ex.Message}");
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
            "OQSDrug_data",
            "ダイナミクス",
            "OQSフォルダ",
            "RSBase"
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
                if (animationTimer.Enabled)
                {
                    animationTimer.Stop();
                    animationTimer.Dispose();
                }

                // NotifyIconの解放
                if (notifyIcon1 != null)
                {
                    notifyIcon1.BalloonTipClicked -= NotifyIcon_BalloonTipClicked; // イベント解除
                    notifyIcon1.Dispose();
                }

                Application.Exit();
            }
        }

        private void toolStripComboBoxDBProviders_SelectedIndexChanged(object sender, EventArgs e)
        {
            CommonFunctions.DBProvider = toolStripComboBoxDBProviders.Text;

            AddLog($"データベースプロバイダーを{CommonFunctions.DBProvider}に変更しました");
        }

        private void SetupYZKSindicator() //xmlは常に取得
        {
            Color activeYZ = Color.LightGreen;
            Color activeKS = Color.LightCoral;
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
                case 2:
                    buttonYZPDF.BackColor = SystemColors.Control;
                    buttonYZPDF.ForeColor = SystemColors.ControlText;
                    buttonSR.BackColor = SystemColors.Control;
                    buttonSR.ForeColor = SystemColors.ControlText;
                    break;
                case 3:
                    buttonYZPDF.BackColor = acticePDF;
                    buttonYZPDF.ForeColor = activeText;
                    buttonSR.BackColor = activeYZ;
                    buttonSR.ForeColor = activeText;
                    break;
                case 4:
                    buttonYZPDF.BackColor = SystemColors.Control;
                    buttonYZPDF.ForeColor = SystemColors.ControlText;
                    buttonSR.BackColor = activeYZ;
                    buttonSR.ForeColor = activeText;
                    break;
                case 11:
                    buttonYZPDF.BackColor = acticePDF;
                    buttonYZPDF.ForeColor = activeText;

                    break;
                case 13:
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
                case 0:
                    buttonKS.BackColor = activeKS;
                    buttonKSPDF.BackColor = SystemColors.Control;
                    buttonKSXML.BackColor = activeXML;
                    break;
                default:
                    buttonKS.BackColor = activeKS;
                    buttonKSPDF.BackColor = acticePDF;
                    buttonKSXML.BackColor = activeXML;
                    break;
            }
        }

        private string GenerateXML(dynamic ptData, string folderPath, string startDate, string endDate, string medicalInstitutionCode, int category)
        {
            int fileCategory;
            try
            {
                // ArbitraryFileIdentifier を生成
                string currentDate = DateTime.Now.ToString("yyMMdd");
                string currentTime = DateTime.Now.ToString("HHmm");
                string arbitraryFileIdentifier = ptData.Id.ToString();

                // FileSymbol を決定
                string fileSymbol;
                if (category > 100) //健診
                {
                    fileSymbol = "TKK";
                }
                else
                {
                    fileSymbol = "YZK";
                }
                fileCategory = category % 10;

                // ファイル名を生成
                string fileName = $"{fileSymbol}siquc01req_{fileCategory:00}{currentDate}{currentTime}{ptData.Id}.xml";

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
            bool AllDataProcessed = true;

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
                // ファイル一覧を取得
                string[] fileList = Directory.GetFiles(resFolder);

                if (!await CommonFunctions.WaitForDbUnlock(1000))
                {
                    AddLog("データベースがロックされています。ProcessResAsyncをスキップします");
                }
                else
                {
                    using (OleDbConnection connection = new OleDbConnection(CommonFunctions.connectionOQSdata))
                    {
                        bool RSBreloadFlag = false;
                        bool RSBXMLreloadFlag = false;

                        await connection.OpenAsync();

                        //未処理レコード一覧を取得
                        string query = $"SELECT * FROM reqResults WHERE resFile IS NULL";

                        var records = new List<Dictionary<string, object>>();

                        using (OleDbCommand command = new OleDbCommand(query, connection))
                        using (DbDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var record = new Dictionary<string, object>();
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    record[reader.GetName(i)] = reader.GetValue(i);
                                }
                                records.Add(record);
                            }
                        }

                        foreach (var record in records)
                        {
                            bool isProcessed = false;

                            long PtID = Convert.ToInt64(record["PtID"]);
                            int Category = record["category"] != DBNull.Value ? Convert.ToInt32(record["category"]) : 0;
                            string PtName = (string)record["PtName"];
                            object resultId = record["ID"];
                            string reqFilePath = (string)record["reqFile"];

                            // ファイル名のみを取得
                            string reqfileName = Path.GetFileName(reqFilePath);

                            // ファイル名内の "req" を "res" に置き換え
                            string resFileName = reqfileName.Replace("req", "res");

                            // resFolder と置き換えたファイル名を連結してフルパスを生成
                            string resFilePath = Path.Combine(resFolder, resFileName);
                            string resBaseFileName = Path.GetFileNameWithoutExtension(resFilePath);

                            // ファイルの存在をチェック
                            foreach (string file in fileList)
                            {
                                if (Path.GetFileNameWithoutExtension(file) == resBaseFileName) //resが帰ってきてる
                                {
                                    string extension = Path.GetExtension(file).ToLower();
                                    string messageContent = "";

                                    switch (extension)
                                    {
                                        case ".xml":
                                            AddLog($"{PtID}:{PtName}resフォルダにxmlファイルが見つかりました: {resFileName}");
                                            var xmlDoc = new XmlDocument();

                                            try
                                            {
                                                xmlDoc.Load(file);
                                                var resultCodeNode = xmlDoc.SelectSingleNode("//ResultCode");

                                                if (resultCodeNode != null && resultCodeNode.InnerText == "1")
                                                {
                                                    // "YZK"で始まるかを判定
                                                    if (resFileName.StartsWith("YZK", StringComparison.OrdinalIgnoreCase))  //xml薬剤
                                                    {
                                                        messageContent = await ProcessDrugInfoAsync(PtID, xmlDoc);
                                                    }
                                                    else if (resFileName.StartsWith("TKK", StringComparison.OrdinalIgnoreCase)) //特定健診
                                                    {
                                                        messageContent = await ProcessTKKAsync(PtID, xmlDoc, connection); //TKKdateがsetされているはず

                                                        if (Properties.Settings.Default.KensinFileCategory > 0)
                                                        {
                                                            if (TKKdate.TryGetValue(PtID, out string lastTKKdate))
                                                            {
                                                                string lastReceived = await getLastReceivedDate(connection, PtID, 101);
                                                                if (lastTKKdate.CompareTo(lastReceived) > 0)
                                                                {
                                                                    AddLog("新しい健診結果が見つかりましたのでPDFを要求します");
                                                                    MakeReq(101, dynaTable, PtID);
                                                                    AllDataProcessed = false;
                                                                }
                                                            }
                                                            else
                                                            {
                                                                AddLog("健診xmlは読み込みましたが、最新健診実施日を読み込めませんでした");
                                                            }
                                                        }
                                                    }

                                                    if(Properties.Settings.Default.KeepXml && Properties.Settings.Default.RSBXml)
                                                    {
                                                        RSBXMLreloadFlag = true;
                                                        AddLog("RSBaseのxmlreloadフラグをセットしました");
                                                    }
                                                }
                                                else
                                                { //エラーファイル
                                                    var xmlNode = xmlDoc.SelectSingleNode("//MessageContents");
                                                    messageContent = xmlNode != null ? xmlNode.InnerText : "<MessageContents> タグが見つかりません";
                                                }
                                            }
                                            catch
                                            {
                                                messageContent = "XMLファイルの読み込みに失敗しました";
                                            }

                                            // XMLファイルを削除
                                            if (!Properties.Settings.Default.KeepXml)
                                            {
                                                System.IO.File.Delete(file);
                                                AddLog($"{messageContent} xmlファイルを削除しました");
                                            }
                                            break;

                                        case ".pdf":
                                            int ReadCategory = (int)Math.Floor(Math.Log10(Math.Abs(Category))); //1: 薬剤、2:健診

                                            string rsbDate = DateTime.Now.ToString("yyyy_MM_dd");
                                            //健診 健診日で登録
                                            if (Properties.Settings.Default.KensinFileCategory == 3 && (TKKdate.TryGetValue(PtID, out string value)))
                                            {
                                                rsbDate = $"{value.Substring(0, 4)}_{value.Substring(4, 2)}_{value.Substring(6, 2)}";
                                            }


                                            if ((ReadCategory == 1 && Properties.Settings.Default.DrugFileCategory < 10) || (ReadCategory == 2 && Properties.Settings.Default.KensinFileCategory < 4))
                                            { //検査として登録
                                                string targetFileName = $"{PtID / 10}~01~{rsbDate}~{RSBname[ReadCategory]}~RSB.pdf";
                                                string rsbFilePath = Path.Combine(gazouFolder, targetFileName);

                                                System.IO.File.Move(file, rsbFilePath);
                                                messageContent = "成功";
                                                AddLog($"{PtID}:{PtName} PDFファイルが見つかりgazouフォルダに移動しました: {rsbFilePath}");
                                                resFilePath = rsbFilePath;
                                                RSBreloadFlag = true;
                                            }
                                            else //SideShow登録
                                            {
                                                string RSBcategory = (ReadCategory == 1) ? "薬歴data"  : "健診data";
                                                string mynumberFoler = Path.Combine(Properties.Settings.Default.RSBServerFolder, "myNumber");
                                                
                                                await MoveFileToPatientFolder(mynumberFoler, (int)(PtID / 10), file, rsbDate, RSBcategory);

                                                AddLog($"PDFファイルが {mynumberFoler} に移動されました。");
                                            }

                                            break;
                                    }

                                    // レコード更新
                                    string updateQuery = "UPDATE reqResults SET resFile = ?, resDate = ?, result = ? WHERE ID = ?";
                                    using (OleDbCommand updateCommand = new OleDbCommand(updateQuery, connection))
                                    {
                                        updateCommand.Parameters.AddWithValue("@resFile", resFilePath ?? string.Empty);
                                        updateCommand.Parameters.AddWithValue("@resDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                                        updateCommand.Parameters.AddWithValue("@result", messageContent ?? string.Empty);
                                        updateCommand.Parameters.AddWithValue("@ID", resultId);

                                        CommonFunctions.DataDbLock = true;
                                        await updateCommand.ExecuteNonQueryAsync();
                                        CommonFunctions.DataDbLock = false;

                                    }
                                    isProcessed = true;
                                }
                            }

                            if (!isProcessed)
                            {
                                AddLog($"{resBaseFileName}が未着です");
                                AllDataProcessed = false;
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

                        if (RSBXMLreloadFlag)
                        {
                            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                            {
                                FileName = Properties.Settings.Default.RSBXmlURL,
                                UseShellExecute = true
                            });
                            AddLog($"RSBase XML reload URL{Properties.Settings.Default.RSBXmlURL}をコールしました");
                        }
                    }
                }
                AddLog("ProcessResAsyncを終了します");
                return AllDataProcessed;
            }
            catch (Exception ex)
            {
                AddLog($"ProcessResAsync処理中にエラーが発生しました: {ex.Message}");
                CommonFunctions.DataDbLock = false;
                return false;
            }
        }

        private async Task MoveFileToPatientFolder(string baseDir, long ptIDmain, string sourceFilePath, string rsbDate, string rsbCategory)
        {
            try
            {
                // PtIDmain の 1の位を取得
                int lastDigit = (int)(ptIDmain % 10);

                // サブフォルダのパス
                string subFolder = Path.Combine(baseDir, lastDigit.ToString());
                string patientFolder = Path.Combine(subFolder, ptIDmain.ToString());

                // フォルダを非同期で作成（存在しない場合のみ）
                if (!Directory.Exists(baseDir))
                {
                    Directory.CreateDirectory(baseDir);
                }
                if (!Directory.Exists(subFolder))
                {
                    Directory.CreateDirectory(subFolder);
                }
                if (!Directory.Exists(patientFolder))
                {
                    Directory.CreateDirectory(patientFolder);
                }

                // ファイル名のベース部分
                string fileBaseName = $"{rsbDate}_{ptIDmain}_";
                string fileExtension = ".pdf";

                // XX の部分を決定（50 から開始し、存在しないファイル名を探す）
                int fileIndex = 50;
                string destinationFilePath;
                do
                {
                    string fileName = $"{fileBaseName}{fileIndex}_{rsbCategory}{fileExtension}";
                    destinationFilePath = Path.Combine(patientFolder, fileName);
                    fileIndex++;
                } while (File.Exists(destinationFilePath));

                // ファイルを非同期で移動
                await Task.Run(() => File.Move(sourceFilePath, destinationFilePath));

            }
            catch (Exception ex)
            {
                AddLog($"CopyFiletoPatientFolderでエラー: {ex.Message}");
            }
        }

        private async Task<string> getLastReceivedDate(OleDbConnection connection, long ptId, int category)
        {
            string sql = @"SELECT TOP 1 resDate FROM reqResults WHERE(PtID \ 10) = ? AND category = ?  AND result LIKE '成功%' ORDER BY ID DESC;";
            string returnDate = "00000000";

            try
            {
                using (OleDbCommand command = new OleDbCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("?", (long)(ptId / 10));
                    command.Parameters.AddWithValue("?", category);

                    var result = await command.ExecuteScalarAsync();

                    if (result != null && result is DateTime resDate)
                    {
                        returnDate = resDate.ToString("yyyyMMdd");
                    }
                }
                return returnDate;
            }
            catch (Exception ex)
            {
                AddLog($"getLastReceivedDateでエラーが発生しました{ex.Message}");
                return returnDate;
            }
        }


        private async Task<string> ProcessTKKAsync(long ptID, XmlDocument xmlDoc, OleDbConnection dbConnection)
        {
            AddLog($"{ptID}の特定健診xmlを処理します");

            string lastTKKdate = "00000000";

            try
            {
                // PtIDの枝番を除外
                long ptIDMain = ptID / 10;

                // Xmlヘッダー情報の取得
                XmlNode headerNode = xmlDoc.SelectSingleNode("/XmlMsg/MessageHeader/QualificationsInfo");
                if (headerNode == null)
                {
                    return "エラー：xmlヘッダー情報の取得に失敗しました";
                }

                string ptName = GetNodeValue(headerNode, "Name");
                string ptKana = GetNodeValue(headerNode, "KanasName");
                string ptBirth = GetNodeValue(headerNode, "Birthday");
                int sex = (int)NzConvert(GetNodeValue(headerNode, "AdministrativeGenderCode"));

                int recordCount = 0;

                string insertSql = @"INSERT INTO TKK_history (
                                                            PtIDmain, PtName, PtKana, Sex,  EffectiveTime, ItemCode, ItemName,
                                                            DataType, DataValue, Unit, Oid, DataValueName) 
                                         VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)";

                XmlNodeList TKKresults = xmlDoc.SelectNodes("//SpecificHealthCheckupInfo");

                foreach (XmlNode TKKresult in TKKresults)
                {
                    string effectiveTime = GetNodeValue(TKKresult, "EffectiveTime");
                    if (effectiveTime.CompareTo(lastTKKdate) > 0) lastTKKdate = effectiveTime;

                    //既登録か
                    string sql = "SELECT COUNT(*) FROM TKK_history WHERE PtIDmain = ? AND EffectiveTime = ?";
                    int TKKcount = 0;

                    using (OleDbCommand checkCommand = new OleDbCommand(sql, dbConnection))
                    {
                        checkCommand.Parameters.AddWithValue("?", ptIDMain);
                        checkCommand.Parameters.AddWithValue("?", effectiveTime);

                        TKKcount = (int)await checkCommand.ExecuteScalarAsync();
                    }

                    if (TKKcount == 0) //新規レコード
                    {
                        var checkupInfos = TKKresult.SelectNodes("HealthCheckupResultAndQuestionInfos/HealthCheckupResultAndQuestionInfo");

                        foreach (XmlNode info in checkupInfos)
                        {
                            string itemCode = GetNodeValue(info, "ItemCode");
                            string dataValue = GetNodeValue(info, "DataValue");
                            dataValue = dataValue.Length > 64 ? dataValue.Substring(0, 64) : dataValue;
                            string itemName = GetNodeValue(info, "ItemName");
                            string dataType = GetNodeValue(info, "DataType");
                            string Unit = GetNodeValue(info, "Unit");
                            string Oid = GetNodeValue(info, "Oid");
                            string dataValueName = GetNodeValue(info, "DataValueName");
                            dataValueName = dataValueName.Length > 64 ? dataValueName.Substring(0, 64) : dataValueName;

                            //TKK_history追加
                            using (OleDbCommand insertCommand = new OleDbCommand(insertSql, dbConnection))
                            {
                                insertCommand.Parameters.AddWithValue("?", ptIDMain);
                                insertCommand.Parameters.AddWithValue("?", ptName);
                                insertCommand.Parameters.AddWithValue("?", ptKana);
                                insertCommand.Parameters.AddWithValue("?", sex);
                                insertCommand.Parameters.AddWithValue("?", effectiveTime);
                                insertCommand.Parameters.AddWithValue("?", itemCode);
                                insertCommand.Parameters.AddWithValue("?", itemName);
                                insertCommand.Parameters.AddWithValue("?", dataType);
                                insertCommand.Parameters.AddWithValue("?", dataValue);
                                insertCommand.Parameters.AddWithValue("?", Unit);
                                insertCommand.Parameters.AddWithValue("?", Oid);
                                insertCommand.Parameters.AddWithValue("?", dataValueName);

                                CommonFunctions.DataDbLock = true;
                                await insertCommand.ExecuteNonQueryAsync();
                                CommonFunctions.DataDbLock = false;
                            }
                            recordCount++;
                        }
                    }
                }

                if (recordCount > 0)
                {
                    string message = $"{ptName}さんの特定健診{recordCount}件取得";
                    ShowNotification($"{ptIDMain}", message);
                    AddLog(message);
                }

                TKKdate[ptID] = lastTKKdate;

                return $"成功：#{lastTKKdate}# xml健診から{recordCount}件のレコードを読み込みました";

            }
            catch (Exception ex)
            {
                return "エラー：" + ex.Message;
            }
        }

        private async Task<string> ProcessDrugInfoAsync(long ptID, XmlDocument xmlDoc)
        {
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
                using (OleDbConnection dbConnection = new OleDbConnection(CommonFunctions.connectionOQSdata))
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
                    int ptSex = (int)NzConvert(GetNodeValue(headerNode, "AdmGendCode"));

                    
                    int recordCount = 0;
                    int sinryoCount = 0;

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

                            int diOrg = (int)NzConvert(GetNodeValue(monthInfNode, GetMatchingNodeName(monthInfNode, elementMappings, "Org")),-1);
                            string meTrDiHCd = GetNodeValue(monthInfNode, GetMatchingNodeName(monthInfNode, elementMappings, "DiHCd"));
                            string prlsHCd = GetNodeValue(monthInfNode, GetMatchingNodeName(monthInfNode, elementMappings, "HCd"));
                            string meTrDiHNm = GetNodeValue(monthInfNode, GetMatchingNodeName(monthInfNode, elementMappings, "DiHNm"));
                            string meTrMonth = GetNodeValue(monthInfNode, GetMatchingNodeName(monthInfNode, elementMappings, "Month"));
                            string prlsHNm = GetNodeValue(monthInfNode, GetMatchingNodeName(monthInfNode, elementMappings, "HNm"));
                            int prIsOrg = (int)NzConvert(GetNodeValue(monthInfNode, GetMatchingNodeName(monthInfNode, elementMappings, "IsOrg")),-1);
                            int inOut = (int)NzConvert(GetNodeValue(monthInfNode, GetMatchingNodeName(monthInfNode, elementMappings, "Cl")), -1);

                            string MIcode = GetMedicalInstitutionCode(meTrDiHCd, prlsHCd); //医科歯科のコード

                            //診療情報付加のxmlでは何故かDiOrgとprIsOrgが抜けているのでprIsOegだけでも作成 ひどい仕様だわ
                            if(prIsOrg == -1)
                            {
                                prIsOrg = (MIcode == Properties.Settings.Default.MCode) ? 1 : 2;
                            }


                            XmlNodeList dateInfList = monthInfNode.SelectNodes(GetMatchingNodeName(monthInfNode, elementMappings, "DateInf"));
                            foreach (XmlNode dateInfNode in dateInfList)
                            {
                                string diDate = GetNodeValue(dateInfNode, GetMatchingNodeName(dateInfNode, elementMappings, "DiDate"));
                                string prDate = GetNodeValue(dateInfNode, GetMatchingNodeName(dateInfNode, elementMappings, "PrDate"));

                                //薬剤
                                string sql = "SELECT ID, Source, Revised FROM drug_history WHERE PtIDmain = ? AND (MeTrDiHCd = ? OR  PrlsHCd = ?) AND DiDate = ?";
                                
                                using (OleDbCommand checkCommand = new OleDbCommand(sql, dbConnection))
                                {
                                    checkCommand.Parameters.AddWithValue("?", ptIDMain);
                                    checkCommand.Parameters.AddWithValue("?", MIcode);
                                    checkCommand.Parameters.AddWithValue("?", MIcode);
                                    checkCommand.Parameters.AddWithValue("?", diDate);

                                    using (OleDbDataReader reader = checkCommand.ExecuteReader())
                                    {
                                        bool doReadDH = true;

                                        while (reader.Read())
                                        {
                                            //既存レコードあり
                                            int resultId = reader.GetInt32(reader.GetOrdinal("ID"));
                                            int resultSource = reader.IsDBNull(reader.GetOrdinal("Source")) ? 9 : reader.GetInt32(reader.GetOrdinal("Source"));

                                            // 既存の電処or調剤由来のデータならRevised=1とする
                                            if (resultSource > Source)
                                            {
                                                string updateSql = "UPDATE drug_history SET Revised = True WHERE ID = ?";

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
                                            if (drugInfList.Count > 0)
                                            {
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

                                //診療情報
                                XmlNode meTrInfsNode = dateInfNode.SelectSingleNode("MeTrInfs");
                                if (meTrInfsNode != null)
                                {
                                    var ptData = new
                                    {
                                        Id = ptID,
                                        Idmain = ptIDMain,
                                        Name = ptName,
                                        Kana = ptKana,
                                        Birth = ptBirth,
                                        Sex = ptSex,
                                        MeTrDiHCd = meTrDiHCd,
                                        MeTrDiHNm = meTrDiHNm,
                                        MeTrMonth = meTrMonth,
                                        DiDate = diDate
                                    };

                                    sinryoCount +=  await ProcessSinryoInfoAsync(dbConnection, meTrInfsNode, ptData);
                                }
                            }
                        }
                    }
                    if (recordCount > 0)
                    {
                        ShowNotification($"{ptIDMain}", $"{ptName}さんの薬歴{recordCount}件取得");
                    }
                    return $"成功：xml薬歴から{recordCount}件,診療情報{sinryoCount}件のレコードを読み込みました";
                }
            }
            catch (Exception ex)
            {
                return "エラー：" + ex.Message;
            }
        }

        private async Task<int> ProcessSinryoInfoAsync(OleDbConnection conn, XmlNode meTrInfsNode, dynamic ptData)
        {
            int count = 0;

            string insertSql = @"INSERT INTO sinryo_history (
                                   PtID, PtIDmain, PtName, PtKana, Birth, Sex, MeTrDiHCd,
                                   MeTrDiHNm, MeTrMonth,DiDate, SinInfN, SinInfCd, MeTrIdCl, Qua1, Times, Unit, ReceiveDate
                                 ) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)";
            string checkSql = @"SELECT COUNT(*) FROM sinryo_history WHERE PtIDmain = ? AND DiDate = ?;"; // 同一患者同一日なら取り込み済みとする

            try
            {
                foreach (XmlNode meTrInf in meTrInfsNode.SelectNodes("MeTrInf"))
                {
                    // 既存データのチェック
                    using (OleDbCommand checkCommand = new OleDbCommand(checkSql, conn))
                    {
                        checkCommand.Parameters.AddWithValue("?", ptData.Idmain);
                        checkCommand.Parameters.AddWithValue("?", ptData.DiDate);

                        int recordCount = (int)await checkCommand.ExecuteScalarAsync();
                        if (recordCount > 0)
                        {
                            continue; // 既にデータが存在する場合はスキップ
                        }
                    }

                    string SinInfN = GetNodeValue(meTrInf, "SinInfN");
                    string SinInfCd = GetNodeValue(meTrInf, "SinInfCd");
                    int times = (int)NzConvert(GetNodeValue(meTrInf, "Times"));
                    string MeTrIdCl = GetNodeValue(meTrInf, "MeTrIdCl");
                    string Unit = GetNodeValue(meTrInf, "Unit");
                    float qua1 = NzConvert(GetNodeValue(meTrInf, "Qua1"));
                    string meTrIdCl = GetNodeValue(meTrInf, "MeTrIdCl");
                    string receiveDate = DateTime.Now.ToString("yyyyMMdd");

                    // 新規データの挿入
                    using (OleDbCommand insertCommand = new OleDbCommand(insertSql, conn))
                    {
                        insertCommand.Parameters.AddWithValue("@PtID", ptData.Id);
                        insertCommand.Parameters.AddWithValue("@PtIDmain", ptData.Idmain);
                        insertCommand.Parameters.AddWithValue("@PtName", ptData.Name);
                        insertCommand.Parameters.AddWithValue("@PtKana", ptData.Kana);
                        insertCommand.Parameters.AddWithValue("@Birth", ptData.Birth);
                        insertCommand.Parameters.AddWithValue("@Sex", ptData.Sex);
                        insertCommand.Parameters.AddWithValue("@MeTrDiHCd", ptData.MeTrDiHCd);
                        insertCommand.Parameters.AddWithValue("@MeTrDiHNm", ptData.MeTrDiHNm);
                        insertCommand.Parameters.AddWithValue("@MeTrMonth", ptData.MeTrMonth);
                        insertCommand.Parameters.AddWithValue("@DiDate", ptData.DiDate);
                        insertCommand.Parameters.AddWithValue("@SinInfN", SinInfN);
                        insertCommand.Parameters.AddWithValue("@SinInfCd", SinInfCd);
                        insertCommand.Parameters.AddWithValue("@MeTrIdCl", meTrIdCl);
                        insertCommand.Parameters.AddWithValue("@Qua1", qua1);
                        insertCommand.Parameters.AddWithValue("@Times", times);
                        insertCommand.Parameters.AddWithValue("@Unit", Unit);
                        insertCommand.Parameters.AddWithValue("@ReceiveDate", receiveDate);

                        await insertCommand.ExecuteNonQueryAsync();
                        count++;
                    }
                }
                return count;
            }
            catch (Exception ex)
            {
                AddLog($"ProcessSinryoAsyncでエラーが発生しました：{ex.Message}");
                return count;
            }
        }
    
        

        private string GetMedicalInstitutionCode(string meTrDiHCd, string prlsHCd)
        {
            // コードが10桁かつ左から3番目が1または3の場合に有効とする 医科歯科
            bool IsValidCode(string code) =>
                !string.IsNullOrEmpty(code) &&
                code.Length == 10 &&
                (code[2] == '1' || code[2] == '3');

            // prlsHCdが条件に該当する場合は優先的に返す
            if (IsValidCode(prlsHCd))
            {
                return prlsHCd;
            }

            // prlsHCdが該当しない場合、meTrDiHCdを確認
            if (IsValidCode(meTrDiHCd))
            {
                return meTrDiHCd;
            }

            // どちらも該当しない場合は空文字を返す
            return string.Empty;
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
            string connectionString = $"Provider={CommonFunctions.DBProvider};Data Source={databasePath};";

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
            try
            {
                using (OleDbConnection connection = new OleDbConnection(CommonFunctions.connectionOQSdata))
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

        private async void checkBoxAutoview_CheckedChanged(object sender, EventArgs e) //RSB連動遷移
        {
            autoRSB = checkBoxAutoview.Checked;
            autoTKK = checkBoxAutoTKK.Checked;
            autoSR = checkBoxAutoSR.Checked;

            Properties.Settings.Default.autoRSB = autoRSB;
            Properties.Settings.Default.autoTKK = autoTKK;
            Properties.Settings.Default.autoSR = autoSR;

            Properties.Settings.Default.Save();

            if (autoRSB || autoTKK || autoSR)
            {
                InitializeFileWatcher();

                //初回読み込み
                if (idStyle < 3 && File.Exists(idFile))
                {
                    await ReadIdAsync(idFile,idStyle);
                } 
                else if(idStyle == 3)
                {
                    string latestIdFile = KeepLatestFile(idFile);
                    if (latestIdFile.Length > 0)
                    {
                        await ReadIdAsync(latestIdFile, idStyle);
                    }
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
            switch (Properties.Settings.Default.RSBID)
            {
                case 0:
                    idFile = @"C:\RSB_TEMP\ID.dat";
                    idStyle = 1;
                    break;
                case 1:
                    idFile = @"C:\RSB_TEMP\temp_rs.txt";
                    idStyle = 1;
                    break;
                case 2:
                    idFile = @"C:\common\thept.txt";
                    idStyle = 2;
                    break;
                case 3:
                    idFile = @"C:\DynaID";
                    idStyle = 3;
                    break;
                case 4:
                    idFile = @"D:\DynaID";
                    idStyle = 3;
                    break;
            }

            // FileSystemWatcherを作成し、監視対象のディレクトリとファイルを指定
            //fileWatcher = new FileSystemWatcher();
            //fileWatcher.Path = Path.GetDirectoryName(idFile);  // ファイルがあるディレクトリ
            //fileWatcher.Filter = Path.GetFileName(idFile);     // ファイル名でフィルタリング
            //fileWatcher.NotifyFilter = NotifyFilters.LastWrite; // 最終書き込み変更を監視

            string idPath = (idStyle < 3) ? Path.GetDirectoryName(idFile) : idFile;
            if (!Directory.Exists(idPath))
            {
                try
                {
                    Directory.CreateDirectory(idPath);
                    AddLog($"{idPath}が見つからなかったので作成しました");
                }
                catch(Exception ex)
                {
                    MessageBox.Show($"RSBase ID連携フォルダ{idPath}が存在しません。ID連携の設定を確認してください。{ex.Message}", "ID連携エラー");
                    return;
                }
            }

            try
            {
                if (idStyle == 3) //ダイナ他社連携
                {
                    fileWatcher = new FileSystemWatcher
                    {
                        Path = idFile,
                        Filter = "dyna*.txt", // 拡張子がTXTのすべてのファイル
                        NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite, // ファイル作成・更新を検知
                        EnableRaisingEvents = true             // 監視を有効化
                    };
                }
                else
                {
                    fileWatcher = new FileSystemWatcher
                    {
                        Path = idPath,   // ディレクトリを監視
                        Filter = Path.GetFileName(idFile),     // ファイル名でフィルタリング
                        NotifyFilter = NotifyFilters.LastWrite, // | NotifyFilters.CreationTime, // 必要な通知フィルタを設定
                        EnableRaisingEvents = true             // 監視を有効化
                    };
                }

                // 監視イベントハンドラを設定
                fileWatcher.Changed += FileWatcher_Changed;
                //fileWatcher.Created += FileWatcher_Changed;

                AddLog("RSB連携を開始しました");
            }
            catch (Exception ex)
            {
                AddLog($"FileWatcherの初期化に失敗しました{ex.ToString()}");
            }
        }

        // ファイルが変更されたときに呼ばれるイベントハンドラ
        private async void FileWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            if (!idChageCalled)
            {
                idChageCalled = true; //二重起動を避ける

                await Task.Delay(fileReadDelayms); // 読み込み遅延

                if ((idStyle < 3 && e.FullPath == idFile) || (idStyle == 3 && e.FullPath.StartsWith(idFile, StringComparison.OrdinalIgnoreCase)))
                {
                    // ファイル内容の読み取り
                    await ReadIdAsync(e.FullPath, idStyle);

                    await reloadDataAsync();
                }
                idChageCalled = false;
            }
        }

        private async Task ReadIdAsync(string filePath, int style)
        {
            try
            {
                // ファイルの内容を非同期で読み取る
                string fileContent = await Task.Run(() =>
                {
                    // FileStreamを使用して共有アクセスを許可
                    using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    using (var reader = new StreamReader(stream))
                    {
                        return reader.ReadLine();
                    }
                });

                if (style == 3)
                {
                    //ダイナ
                    fileContent = fileContent.Split(',')[0];
                }
                else
                {
                    //thept.txtは内容が違う
                    if (style == 2)
                    {
                        fileContent = fileContent.Split(',')[1];
                    }
                }
                AddLog($"ダイナ/RSB連携ファイルの変更を検知。内容：{fileContent}");

                // 数値に変換を試みる
                if (long.TryParse(fileContent, out long idValue))
                {
                    // 数値に変換できた場合
                    tempId = idValue;

                    if(autoRSB)  await OpenDrugHistory(tempId,false);

                    if(autoTKK)  await OpenTKKHistory(tempId,false);

                    if(autoSR)   await OpenSinryoHistory(tempId,false);
                   
                }
                else
                {
                    AddLog("RSB連携ファイルの内容が数値に変換できませんでした。");
                }

                //ダイナの場合は削除する
                if (style == 3)
                {
                    File.Delete(filePath);
                    AddLog($"{filePath}を削除しました");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"FileWatcherエラー: {ex.Message}");
            }
        }

        private string KeepLatestFile(string folderName) //最新のファイルのみ残してあとはすべて削除する
        {
            try
            {
                // 指定フォルダの全ファイルを取得
                var files = new DirectoryInfo(folderName).GetFiles();

                if (files.Length == 0)
                {
                    Console.WriteLine("フォルダ内にファイルがありません。");
                    return string.Empty;
                }

                // 更新日時が最新のファイルを取得
                var newestFile = files.OrderByDescending(f => f.LastWriteTime).First();

                Console.WriteLine($"残すファイル: {newestFile.FullName}");

                // 最新のファイル以外を削除
                foreach (var file in files)
                {
                    if (file.FullName != newestFile.FullName)
                    {
                        try
                        {
                            file.Delete();
                            Console.WriteLine($"削除: {file.Name}");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"削除失敗: {file.Name}, エラー: {ex.Message}");
                        }
                    }
                }

                // 最新のファイルのフルパスを返す
                return newestFile.FullName;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"エラー: {ex.Message}");
                return string.Empty;
            }
        }

        private async Task OpenDrugHistory(long ptId, bool messagePopup = false)
        {
            if (await existHistory(ptId, "drug_history"))
            {
                tempId = ptId;
                // UIスレッドで操作
                Invoke((Action)(() =>
                {
                    //buttonViewer_Click(toolStripButtonViewer, EventArgs.Empty);
                    toolStripButtonDI_Click(toolStripButtonViewer, EventArgs.Empty);
                }));
                AddLog($"{ptId}の薬歴を開きます");
            }
            else
            {
                //薬歴なしの場合はViewerを閉じる
                if (formDIInstance != null && !formDIInstance.IsDisposed)
                {
                    // UI スレッドで操作する必要があるため Invoke を使用
                    formDIInstance.Invoke((Action)(() =>
                    {
                        formDIInstance.Close(); // Form3 を閉じる
                        formDIInstance = null;
                    }));
                    AddLog($"{ptId}は薬歴がないので薬歴ビュワーを閉じます");
                }
                if (messagePopup)
                {
                    MessageBox.Show($"{ptId}の薬歴はありません");
                }
            }
        }

        private async Task OpenTKKHistory(long ptId, bool messagePopup = false)
        {
            if (await existHistory(ptId, "TKK_history"))
            {
                tempId = ptId;
                // UIスレッドで操作
                Invoke((Action)(() =>
                {
                    toolStripButtonTKK_Click(toolStripButtonViewer, EventArgs.Empty);
                }));
                AddLog($"{ptId}の健診結果を開きます");
            }
            else
            {
                //健診歴なしの場合はViewerを閉じる
                if (formTKKInstance != null && !formTKKInstance.IsDisposed)
                {
                    // UI スレッドで操作する必要があるため Invoke を使用
                    formTKKInstance.Invoke((Action)(() =>
                    {
                        formTKKInstance.Close(); // Form3 を閉じる
                        formTKKInstance = null;
                    }));
                    AddLog($"{ptId}は健診歴がないので健診ビュワーを閉じます");
                }
                if (messagePopup)
                {
                    MessageBox.Show($"{ptId}の健診歴はありません");
                }
            }
        }

        private async Task OpenSinryoHistory(long ptId, bool messagePopup = false)
        {
            if (await existHistory(ptId, "sinryo_history"))
            {
                tempId = ptId;
                // UIスレッドで操作
                Invoke((Action)(() =>
                {
                    toolStripButtonSinryo_Click(toolStripButtonSinryo, EventArgs.Empty);
                }));
                AddLog($"{ptId}の診療情報を開きます");
            }
            else
            {
                //なしの場合はViewerを閉じる
                if (formSRInstance != null && !formSRInstance.IsDisposed)
                {
                    // UI スレッドで操作する必要があるため Invoke を使用
                    formSRInstance.Invoke((Action)(() =>
                    {
                        formSRInstance.Close(); // 閉じる
                        formSRInstance = null;
                    }));
                    AddLog($"{ptId}は診療情報履歴がないのでビュワーを閉じます");
                }
                if (messagePopup)
                {
                    MessageBox.Show($"{ptId}の診療情報履歴はありません");
                }
            }
        }

        private async Task<bool> existHistory(long PtIDmain, string tableName)
        {
            try
            {
                if (await CommonFunctions.WaitForDbUnlock(2000))
                {
                    using (OleDbConnection connection = new OleDbConnection(CommonFunctions.connectionOQSdata))
                    {
                        await connection.OpenAsync();

                        // SQL文の構文を修正
                        string sql = $"SELECT COUNT(*) FROM {tableName} WHERE PtIDmain = ?;";
                        using (OleDbCommand command = new OleDbCommand(sql, connection))
                        {
                            // パラメータを追加
                            command.Parameters.AddWithValue("?", PtIDmain);

                            CommonFunctions.DataDbLock = true;

                            // レコードのカウントを取得
                            int count = (int)await command.ExecuteScalarAsync();

                            // レコードが1つ以上存在するか確認
                            CommonFunctions.DataDbLock = false;
                            return count > 0;
                        }
                    }
                }
                else
                {
                    AddLog("existHistoryでデータベースロックタイムアウト");
                    return false;
                }
            }
            catch (Exception ex)
            {
                // エラーハンドリング（必要に応じて適宜実装）
                AddLog($"existDrugHistory Error: {ex.Message}");
                return false;
            }
            finally
            {
                CommonFunctions.DataDbLock = false;
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

        private void SaveViewerSettings(Form form, string key)
        {
            if (form.WindowState != FormWindowState.Normal || form.WindowState == FormWindowState.Minimized) form.WindowState = FormWindowState.Normal;

            // 現在の位置とサイズを保存
            if (Properties.Settings.Default.Properties[key] != null) // キーが存在するか確認
            {
                Properties.Settings.Default[key] = form.Bounds;
                Properties.Settings.Default.Save();
            }
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

            // 特定のカラムの幅を固定にする
            if (dataGridView.Columns.Count > 0)
            {
                dataGridView.Columns[0].Width = 80;
                dataGridView.Columns[1].Width = 50;
                dataGridView.Columns[2].Width = 100;
                dataGridView.Columns[3].Width = 250;
                dataGridView.Columns[4].Width = 100;
                dataGridView.Columns[5].Width = 200;
                dataGridView.Columns[6].Width = 100;
                dataGridView.Columns[7].Width = 200;
            }

            // ソート機能を無効にする
            //dataGridView.AllowUserToOrderColumns = false;
            //// 各列のソートモードを無効にする
            //foreach (DataGridViewColumn column in dataGridView.Columns)
            //{
            //    column.SortMode = DataGridViewColumnSortMode.NotSortable;
            //}

            dataGridView.CellBorderStyle = DataGridViewCellBorderStyle.Raised;

            dataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect; //行全体選択
            dataGridView.MultiSelect = false; // 複数行選択を無効にする

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
                contextMenu.MenuItems.Add(new MenuItem("再取得", async (s, args) => await DeleteRow(e.RowIndex)));

                contextMenu.Show(dataGridView1, dataGridView1.PointToClient(Cursor.Position));
            }
        }

        private async Task DeleteRow(int rowIndex)
        {
            if (rowIndex >= 0 && rowIndex < dataGridView1.Rows.Count)
            {
                if(MessageBox.Show("この取得履歴を削除し再取得しますか？\n 削除すると再取得間隔がリセットされますが、取得済データは消えません","再取得の確認",MessageBoxButtons.OKCancel) == DialogResult.OK)
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
                using (var connection = new OleDbConnection(CommonFunctions.connectionOQSdata))
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

        private void InitAnimationTimer()
        {
            // アイコンの配列を用意
            icons = new Icon[]
            {
                Properties.Resources.drug1,
                Properties.Resources.drug2,
                Properties.Resources.drug3,
                Properties.Resources.drug4
            };

            // タイマーを初期化
            animationTimer = new System.Windows.Forms.Timer
            {
                Interval = 200 // 200msごとに切り替え
            };
            animationTimer.Tick += AnimationTimer_Tick;
        }

        private void InitNotifyIcon()
        {
            // コンテキストメニューを設定
            var contextMenu = new ContextMenuStrip();
            var startStopMenuItem = new ToolStripMenuItem();

            // 状態に応じたメニュー項目を更新
            UpdateStartStopMenuItem(startStopMenuItem);
            startStopMenuItem.Enabled = (okSettings == 0b1111);

            // メニューに動的な項目を追加
            startStopMenuItem.Click += (s, e) =>
            {
                // チェックボックスの状態を切り替え
                StartStop.Checked = !StartStop.Checked;

                // メニューを更新
                UpdateStartStopMenuItem(startStopMenuItem);

                StartStop_CheckedChanged(s, EventArgs.Empty); // 開始処理
                
            };

            contextMenu.Items.Add("メイン表示", Properties.Resources.drugicon, ShowForm); 
            contextMenu.Items.Add(startStopMenuItem); // 動的な項目を追加
            contextMenu.Items.Add(new ToolStripSeparator());
            contextMenu.Items.Add("薬歴", Properties.Resources.Text_preview,toolStripButtonDI_Click);
            contextMenu.Items.Add("診療情報", Properties.Resources.Equipment, toolStripButtonSinryo_Click);
            contextMenu.Items.Add("健診", Properties.Resources.Heart, toolStripButtonTKK_Click);
            contextMenu.Items.Add(new ToolStripSeparator());
            contextMenu.Items.Add("終了", Properties.Resources.Exit, ExitApplication);

            notifyIcon1.ContextMenuStrip = contextMenu;

            // バルーン通知の表示イベント
            notifyIcon1.BalloonTipClicked += NotifyIcon_BalloonTipClicked;

            // チェックボックスの状態変更時にもメニューを更新
            StartStop.CheckedChanged += (s, e) => UpdateStartStopMenuItem(startStopMenuItem);

        }

        private void UpdateStartStopMenuItem(ToolStripMenuItem menuItem)
        {
            if (StartStop.Checked)
            {
                menuItem.Text = "停止";
                menuItem.Image = Properties.Resources.Stop;
                //animationTimer?.Start();

                menuItem.Enabled = (okSettings == 0b1111);
            }
            else
            {
                menuItem.Text = "開始";
                menuItem.Image = Properties.Resources.Go;
                //animationTimer?.Stop();
                //notifyIcon1.Icon = Properties.Resources.drug1;
                
                menuItem.Enabled = (okSettings == 0b1111);
            }
        }

        // イベントが発生した場合にバルーン通知を表示
        public void ShowNotification(string title, string message)
        {
            try
            {
                if (this.InvokeRequired) // this はフォーム
                {
                    this.Invoke(new Action(() => ShowNotification(title, message)));
                }
                else
                {
                    notifyIcon1.BalloonTipTitle = title;
                    notifyIcon1.BalloonTipText = message;
                    notifyIcon1.BalloonTipIcon = ToolTipIcon.Info;
                    notifyIcon1.ShowBalloonTip(5000); // 5秒間表示
                }
            }
            catch (Exception ex)
            {
                AddLog($"エラー：ShowNotification：{ex.Message}");
            }
        }

        // バルーン通知をクリックしたときの処理
        private void NotifyIcon_BalloonTipClicked(object sender, EventArgs e)
        {
            //buttonViewer_Click(sender, EventArgs.Empty);
            toolStripButtonDI_Click(sender, EventArgs.Empty);
        }

        // フォームを表示する
        private async void ShowForm(object sender, EventArgs e)
        {
            isFormVisible = true;

            this.WindowState = FormWindowState.Normal;
            this.ShowInTaskbar = true;
            this.Show();
            this.Activate();                          // フォームをアクティブ化

            await reloadDataAsync(true);
        }

        // アプリケーションを終了する
        private void ExitApplication(object sender, EventArgs e)
        {
            buttonExit_Click(toolStripButtonExit, EventArgs.Empty);
        }

        
        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized || !this.Visible)
            {
                ShowForm(notifyIcon1, EventArgs.Empty);
            }
            else
            {
                // 表示されている場合はタスクトレイに最小化
                toolStripButtonToTaskTray_Click(sender, EventArgs.Empty);
            }
        }

        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            // アイコンを切り替える
            notifyIcon1.Icon = icons[currentFrame];
            currentFrame = (currentFrame + 1) % icons.Length; // フレームを更新
        }

        private async void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            await DeleteClientAsync();

            // クリーンアップ
            //timer?.Stop();
            //timer?.Dispose();
            backgroundTimer?.Dispose();

            notifyIcon1?.Dispose();
            animationTimer?.Stop();
            animationTimer?.Dispose();

            BackupSettings();
            //base.OnFormClosing(e);
        }

        private void toolStripButtonToTaskTray_Click(object sender, EventArgs e)
        {
            // フォームを非表示にし、タスクバーから削除
            this.Hide();
            this.ShowInTaskbar = false;

            isFormVisible = false;
        }
               
        private async void buttonReload_Click(object sender, EventArgs e)
        {
            await Task.Run(async ()=>  await reloadDataAsync());
        }

        private void listViewLog_SizeChanged(object sender, EventArgs e)
        {
            // 残りの幅を "Log" 列に割り当て
            listViewLog.Columns[1].Width = -2;
        }

        private async void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                var cellValue = dataGridView1.Rows[e.RowIndex].Cells["PtID"].Value;

                if (cellValue != null && long.TryParse(cellValue.ToString(), out long ptId))
                {
                    tempId = ptId / 10;
                    AddLog($"{tempId}のダブルクリックを検知しました");

                    forceIdLink = true;

                    var categoryValue = dataGridView1.Rows[e.RowIndex].Cells["category"].Value;
                    if (categoryValue != null && int.TryParse(categoryValue.ToString(), out int category))
                    {
                        if(category >= 100)
                        {
                            await OpenTKKHistory(tempId, true);
                        } else if(category >= 10)
                        {
                            await OpenDrugHistory(tempId, true);
                        }
                    }

                    //forceIdLink = false; //Form3側でクリアする
                }
                else
                {
                    MessageBox.Show("患者IDデータがありません。");
                }
            }
        }

        private async Task<string> GetRSBdrive()
        {
            // C: から F: ドライブまで
            for (char driveLetter = 'C'; driveLetter <= 'F'; driveLetter++)
            {
                string drivePath = $"{driveLetter}:";
                string fullPath = drivePath + @"\Users\rsn\public_html\drug_RSB.dat";
               
                // 非同期タスクとしてファイルの存在を確認
                bool exists = await Task.Run(() => File.Exists(fullPath));

                if (exists)
                {
                    AddLog($"RSBaseが{drivePath}ドライブに見つかりました。薬歴情報をバッファに読み込んでいます...");
                    return drivePath; // 見つかったドライブを返す
                }
            }

            AddLog("RSBaseが見つかりませんでした。");
            return null; // 見つからなかった場合
        }

        private void checkBoxAutoStart_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.AutoStart = checkBoxAutoStart.Checked;
            Properties.Settings.Default.Save(); 

        }

        private async Task LoadRSBDIAsync(string filePath)
        {
            if (!File.Exists(filePath))
            {
                AddLog($"指定されたRSBase薬情ファイルが見つかりませんでした。{filePath}");
                return;
            }

            // ファイルを非同期で読み込み
            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var reader = new StreamReader(stream, Encoding.GetEncoding("EUC-JP")))
            {
                string line;
                int count = 0;
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    // カンマ区切りで分割し、指定カラム(0, 3, 8)のみ取得
                    var columns = line.Split(',');
                    if (columns.Length > 7) // 必要なカラム数が存在するか確認
                    {
                        CommonFunctions.RSBDI.Add(new string[] { columns[0], columns[3], columns[8], columns[5] }); // 0:商品名、1:一般名、2:コード、3：先発
                        count++;
                    }
                }
                AddLog($"RSBase薬情インデックス{count}件を読み込みました。");
                AddLog("薬歴の右クリックでRSBase薬情表示が可能になります。");
            }
        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            Color YZcolor = Color.FromArgb(230, 255, 230);
            Color TKcolor = Color.FromArgb(255, 230, 230);

            // "category"列のインデックスを取得
            int categoryIndex = dataGridView1.Columns["category"].Index;

            // "category"列かどうかを確認
            if (e.ColumnIndex == categoryIndex)
            {
                // 現在の行のcategory列の値を取得
                if (int.TryParse(dataGridView1.Rows[e.RowIndex].Cells[categoryIndex].Value?.ToString(), out int categoryValue))
                {
                    // 行全体の背景色を変更
                    if (categoryValue >= 10 && categoryValue <= 99) // 2桁の場合
                    {
                        dataGridView1.Rows[e.RowIndex].DefaultCellStyle.BackColor = YZcolor;
                    }
                    else if (categoryValue >= 100 && categoryValue <= 999) // 3桁の場合
                    {
                        dataGridView1.Rows[e.RowIndex].DefaultCellStyle.BackColor = TKcolor;
                    }
                    else
                    {
                        // デフォルトの色に戻す場合
                        dataGridView1.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.White;
                    }
                }
            }
        }

        private void toolStripButtonTKK_Click(object sender, EventArgs e)
        {
            // FormTKKがすでに開いているか確認
            if (formTKKInstance == null || formTKKInstance.IsDisposed)
            {
                formTKKInstance = new FormTKK(this);

                // 前回の位置とサイズを復元
                if (Properties.Settings.Default.TKKBounds != Rectangle.Empty)
                {
                    formTKKInstance.StartPosition = FormStartPosition.Manual;
                    formTKKInstance.Bounds = Properties.Settings.Default.TKKBounds;

                    // マージンと境界線を設定
                    formTKKInstance.Padding = new Padding(0);
                    formTKKInstance.Margin = new Padding(0);
                    //form3Instance.FormBorderStyle = FormBorderStyle.None;
                }

                // TopMost状態を設定
                formTKKInstance.TopMost = Properties.Settings.Default.ViewerTopmost;

                // Form3が閉じるときに位置、サイズ、TopMost状態を保存
                formTKKInstance.FormClosing += (s, args) =>
                {
                    SaveViewerSettings(formTKKInstance, "TKKBounds");
                };

                formTKKInstance.Show(this);
            }
            else
            {
                // FormTKKが開いている場合、LoadDataIntoComboBoxes()を実行
                Task.Run(async () =>
                    await formTKKInstance.LoadToolStripComboBox()
                );
                // すでに開いている場合はアクティブにする
                formTKKInstance.Activate();

            }
        }

        private void BackupSettings()
        {
            try
            {
                // 完全なファイルパスを生成
                string defaultPath = Path.Combine(OQSFolder, "OQSDrug.config");
                string backupPath1 = Path.Combine(OQSFolder, "OQSDrug1.config");
                string backupPath2 = Path.Combine(OQSFolder, "OQSDrug2.config");

                // フォルダが存在しない場合は作成
                if (!Directory.Exists(OQSFolder))
                {
                    Directory.CreateDirectory(OQSFolder);
                }

                //旧バージョンがあれば移動
                string[] targetFiles = { "OQSDrug1.config", "OQSDrug2.config" };
                foreach (string fileName in targetFiles)
                {
                    // personalFolder 内のファイルパスを生成
                    string sourceFilePath = Path.Combine(PersonalFolder, fileName);

                    // ファイルが存在する場合は移動
                    if (File.Exists(sourceFilePath))
                    {
                        string destinationFilePath = Path.Combine(OQSFolder, fileName);
                        if (File.Exists(destinationFilePath))
                        {
                            File.Delete(sourceFilePath);
                        }
                        else
                        {
                            File.Move(sourceFilePath, destinationFilePath);
                        }
                    }
                }

                // 世代バックアップのロジック
                if (File.Exists(backupPath1))
                {
                    // 既存の OQSDrug1.config を OQSDrug2.config にリネーム
                    if (File.Exists(backupPath2))
                    {
                        File.Delete(backupPath2); // OQSDrug2.config を削除
                    }
                    File.Move(backupPath1, backupPath2);
                }

                if (File.Exists(defaultPath))
                {
                    // 現在の OQSDrug.config を OQSDrug1.config にリネーム
                    File.Move(defaultPath, backupPath1);
                }

                // 設定を保存
                var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal);
                config.SaveAs(defaultPath, ConfigurationSaveMode.Full);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"設定値のエクスポート時にエラーが発生しました：{ex.Message}");
            }
        }

        // 排他 チェック関数
        public async Task<bool> IsAccessAllowedAsync()
        {
            string localMachineName = Environment.MachineName;
            try
            {
                using (var connection = new OleDbConnection(CommonFunctions.connectionOQSdata))
                {
                    await connection.OpenAsync();
                    string query = @"
                SELECT  clientName, lastUpdated 
                FROM connectedClient 
                WHERE clientName <> ? 
                  AND lastUpdated > ?";
                    using (var command = new OleDbCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("clientName", localMachineName);
                        command.Parameters.AddWithValue("lastUpdated", DateTime.Now.AddMinutes(-3).ToString("yyyy-MM-dd HH:mm:ss"));

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (reader.HasRows)
                            {
                                return false; // 他のクライアントが使用中
                            }
                        }
                    }
                }
                return true; // アクセス可能
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error:IsAccessAllowedAsync {ex.Message}");
                return false;
            }
        }

        // 自分のPC名とタイムスタンプを更新
        public async Task UpdateClientAsync()
        {
            string localMachineName = Environment.MachineName;
            try
            {
                using (var connection = new OleDbConnection(CommonFunctions.connectionOQSdata))
                {
                    await connection.OpenAsync();
                    string updateQuery = $@"
                UPDATE connectedClient 
                SET lastUpdated = ? 
                WHERE clientName = ?";
                    using (var command = new OleDbCommand(updateQuery, connection))
                    {
                        command.Parameters.AddWithValue("lastUpdated", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                        command.Parameters.AddWithValue("clientName", localMachineName);

                        int affectedRows = await command.ExecuteNonQueryAsync();
                        if (affectedRows == 0)
                        {
                            // レコードが存在しない場合は挿入
                            string insertQuery = $@"
                        INSERT INTO connectedClient (clientName, lastUpdated) 
                        VALUES (?, ?)";
                            using (var insertCommand = new OleDbCommand(insertQuery, connection))
                            {
                                insertCommand.Parameters.AddWithValue("clientName", localMachineName);
                                insertCommand.Parameters.AddWithValue("lastUpdated", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                                await insertCommand.ExecuteNonQueryAsync();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"UpdateClientAsyncでエラー{ex.Message}");
            }
        }

        private void toolStripButtonDI_Click(object sender, EventArgs e)
        {
            if (formDIInstance == null || formDIInstance.IsDisposed)
            {
                formDIInstance = new FormDI(this);

                // 前回の位置とサイズを復元
                if (Properties.Settings.Default.ViewerBounds != Rectangle.Empty)
                {
                    formDIInstance.StartPosition = FormStartPosition.Manual;
                    formDIInstance.Bounds = Properties.Settings.Default.ViewerBounds;

                    // マージンと境界線を設定
                    formDIInstance.Padding = new Padding(0);
                    formDIInstance.Margin = new Padding(0);
                    //form3Instance.FormBorderStyle = FormBorderStyle.None;
                }

                // TopMost状態を設定
                formDIInstance.TopMost = Properties.Settings.Default.ViewerTopmost;

                // Form3が閉じるときに位置、サイズ、TopMost状態を保存
                formDIInstance.FormClosing += (s, args) =>
                {
                    SaveViewerSettings(formDIInstance, "ViewerBounds");
                };

                formDIInstance.Show(this);
            }
            else
            {
                // Form3が開いている場合、LoadDataIntoComboBoxes()を実行
                Task.Run(async () =>
                    await formDIInstance.LoadDataIntoComboBoxes()
                );
                // すでに開いている場合はアクティブにする
                formDIInstance.Activate();

            }
        }

        // 自分のPC名とタイムスタンプを削除
        public async Task DeleteClientAsync()
        {
            string localMachineName = Environment.MachineName;
            using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(2))) // 2秒のタイムアウト
            {
                try
                {
                    using (var connection = new OleDbConnection(CommonFunctions.connectionOQSdata))
                    {
                        var openTask = connection.OpenAsync(cts.Token);
                        if (await Task.WhenAny(openTask, Task.Delay(TimeSpan.FromSeconds(2))) != openTask)
                        {
                            throw new TimeoutException("データベース接続がタイムアウトしました。");
                        }

                        string updateQuery = @"DELETE FROM connectedClient WHERE clientName = ?";
                        using (var command = new OleDbCommand(updateQuery, connection))
                        {
                            command.Parameters.AddWithValue("clientName", localMachineName);

                            var executeTask = command.ExecuteNonQueryAsync(cts.Token);
                            if (await Task.WhenAny(executeTask, Task.Delay(TimeSpan.FromSeconds(2))) != executeTask)
                            {
                                command.Cancel(); // タイムアウト時にキャンセル
                                throw new TimeoutException("DELETE クエリがタイムアウトしました。");
                            }
                        }
                    }
                }
                catch (TimeoutException ex)
                {
                    MessageBox.Show($"処理がタイムアウトしました: {ex.Message}");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"DeleteClientAsync でエラー: {ex.Message}");
                }
            }
        }

        private async void toolStripButtonDebug_Click(object sender, EventArgs e)
        {
            if (long.TryParse(toolStripTextBoxDebug.Text, out long ptId))
            {
                OpenFileDialog op = new OpenFileDialog();
                op.Title = "xmlファイルの読込";
                op.FileName = "*.xml";
                //op.InitialDirectory = System.Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                op.Filter = "xmlファイル(*.xml)|*.xml|すべてのファイル(*.*)|*.*";
                op.FilterIndex = 1;
                op.RestoreDirectory = true;
                op.CheckFileExists = false;
                op.CheckPathExists = true;

                if (op.ShowDialog(this) == DialogResult.OK)
                {
                    string settingsFilePath = op.FileName;

                    var xmlDoc = new XmlDocument();

                    xmlDoc.Load(settingsFilePath);

                    MessageBox.Show(await ProcessDrugInfoAsync(ptId, xmlDoc));
                }

            }
            else
            {
                MessageBox.Show("ID(枝番付)を入力してから実行してください");
            }

            
        }

        private void toolStripVersion_DoubleClick(object sender, EventArgs e)
        {
            toolStripComboBoxDBProviders.Visible = !toolStripComboBoxDBProviders.Visible;
            toolStripButtonDebug.Visible = !toolStripButtonDebug.Visible;
            toolStripSeparatorDebug1.Visible = !toolStripSeparatorDebug1.Visible;
            toolStripSeparatorDebug2.Visible = !toolStripSeparatorDebug2.Visible;
            toolStripTextBoxDebug.Visible = !toolStripTextBoxDebug.Visible;
        }

        private void dataGridView1_CellToolTipTextNeeded(object sender, DataGridViewCellToolTipTextNeededEventArgs e)
        {
            e.ToolTipText = "行選択⇢右クリックで再取得メニュー表示\r\nダブルクリックで薬歴/健診歴を表示します\r\n";
        }

        private async void buttonPtIDSearch_Click(object sender, EventArgs e)
        {
            string strPtIDmain = null;

            textBoxPtIDmain.Invoke(new Action(() =>
                strPtIDmain = textBoxPtIDmain.Text
            ));

            if (long.TryParse(strPtIDmain, out long idValue))
            {
                // 数値に変換できた場合
                tempId = idValue;

                await OpenDrugHistory(tempId, true);

            }
        }

        private async void buttonTKKSearch_Click(object sender, EventArgs e)
        {
            string strPtIDmain = null;

            textBoxPtIDmain.Invoke(new Action(() =>
                strPtIDmain = textBoxPtIDmain.Text
            ));

            if (long.TryParse(strPtIDmain, out long idValue))
            {
                // 数値に変換できた場合
                tempId = idValue;

                await OpenTKKHistory(tempId, true);

            }
        }

        private async void buttonSRSearch_Click(object sender, EventArgs e)
        {
            string strPtIDmain = null;

            textBoxPtIDmain.Invoke(new Action(() =>
                strPtIDmain = textBoxPtIDmain.Text
            ));

            if (long.TryParse(strPtIDmain, out long idValue))
            {
                // 数値に変換できた場合
                tempId = idValue;

                await OpenSinryoHistory(tempId, true);

            }
        }

        public void toolStripButtonSinryo_Click(object sender, EventArgs e)
        {
            // FormSRがすでに開いているか確認
            if (formSRInstance == null || formSRInstance.IsDisposed)
            {
                formSRInstance = new FormSR(this);

                // 前回の位置とサイズを復元
                if (Properties.Settings.Default.SRBounds != Rectangle.Empty)
                {
                    formSRInstance.StartPosition = FormStartPosition.Manual;
                    formSRInstance.Bounds = Properties.Settings.Default.SRBounds;

                    // マージンと境界線を設定
                    formSRInstance.Padding = new Padding(0);
                    formSRInstance.Margin = new Padding(0);
                    //form3Instance.FormBorderStyle = FormBorderStyle.None;
                }

                // TopMost状態を設定
                formSRInstance.TopMost = Properties.Settings.Default.ViewerTopmost;

                // Form3が閉じるときに位置、サイズ、TopMost状態を保存
                formSRInstance.FormClosing += (s, args) =>
                {
                    SaveViewerSettings(formSRInstance, "SRBounds");
                };

                formSRInstance.Show(this);
            }
            else
            {
                // FormTKKが開いている場合、LoadDataIntoComboBoxes()を実行
                Task.Run(async () =>
                    await formSRInstance.LoadDataIntoComboBoxes()
                );
                // すでに開いている場合はアクティブにする
                formSRInstance.Activate();

            }
        }

        public async Task LoadKoroDataAsync()
        {
            try
            {
                string KOROpath = Path.Combine(Path.GetDirectoryName(Properties.Settings.Default.OQSDrugData), "KOROdata.mdb");

                if (File.Exists(KOROpath))
                {
                    AddLog("KOROdataが見つかりましたので薬品名コードを読み込みます");

                    string connectionKOROdata = $"Provider={CommonFunctions.DBProvider};Data Source={KOROpath};";
                    string sql = "SELECT 医薬品コード AS ReceptCode,  薬価基準コード AS MedisCode " +
                                 " FROM TG医薬品マスター " +
                                 " WHERE (((薬価基準コード) Is Not Null));";
                    int count = 0;

                    using (OleDbConnection connection = new OleDbConnection(connectionKOROdata))
                    {
                        await connection.OpenAsync();

                        using (OleDbCommand command = new OleDbCommand(sql, connection))
                        using (OleDbDataReader reader = (OleDbDataReader)await command.ExecuteReaderAsync())
                        {
                            // 既存データをクリア
                            CommonFunctions.ReceptToMedisCodeMap.Clear();

                            // データを読み込んでDictionaryに設定
                            while (await reader.ReadAsync())
                            {
                                string receptCode = reader["ReceptCode"].ToString();
                                string medisCode = reader["MedisCode"].ToString();

                                if (!CommonFunctions.ReceptToMedisCodeMap.ContainsKey(receptCode))
                                {
                                    CommonFunctions.ReceptToMedisCodeMap.Add(receptCode, medisCode);
                                    count++;
                                }
                            }
                        }
                    }

                    AddLog($"KOROdataから{count}件のコードを読み込みました");
                }
            }
            catch (Exception ex)
            {
                AddLog($"Error loading data: {ex.Message}");
            }
        }

        private void loadConnectionString()
        {
            CommonFunctions.connectionOQSdata = $"Provider={CommonFunctions.DBProvider};Data Source={Properties.Settings.Default.OQSDrugData};";
        }
    }
}



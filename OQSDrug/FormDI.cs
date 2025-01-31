using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using static OQSDrug.FormTKK;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;

namespace OQSDrug
{
    public partial class FormDI : Form
    {
        private const int SnapDistance = 16; // 吸着の距離（ピクセル）
        private int SnapCompPixel = 8;  //余白補正

        private Form1 _parentForm;
        private string provider;

        private Color[] RowColors = { Color.WhiteSmoke, Color.White };

        public List<string[]> RSBDI = new List<string[]>();
        private List<(long PtID, string PtName)> ptData = new List<(long PtID, string PtName)>();

        private DataTable DrugHistoryData = new DataTable();
        private Dictionary<string, string> ReceptToMedisCodeMap = new Dictionary<string, string>();

        private int ShowSpan = Properties.Settings.Default.ViewerSpan;
        private FormSearch formSearch = null;

        private string connectionOQSData = string.Empty;


        public FormDI(Form1 parentForm)
        {
            InitializeComponent();

            _parentForm = parentForm;
            provider = _parentForm.DBProvider;

            RSBDI = _parentForm.RSBDI;
            ReceptToMedisCodeMap = _parentForm.ReceptToMedisCodeMap;

            connectionOQSData = $"Provider={provider};Data Source={Properties.Settings.Default.OQSDrugData};";

            toolStrip1.Renderer = new CustomToolStripRenderer(); // カスタム描画を適用
        }

        public async Task LoadDataIntoComboBoxes()
        {
            if (await _parentForm.WaitForDbUnlock(2000))
            {
                ptData = new List<(long PtID, string PtName)>();

                string query = @"SELECT PtIDmain, PtName, Max(id) AS Maxid
                            FROM drug_history
                            GROUP BY PtIDmain, PtName
                            ORDER BY Max(id) DESC;";

                using (OleDbConnection connection = new OleDbConnection(connectionOQSData))
                {
                    try
                    {
                        await connection.OpenAsync();

                        _parentForm.DataDbLock = true;

                        // コマンドを作成
                        using (OleDbCommand command = new OleDbCommand(query, connection))
                        {

                            // データを取得
                            using (OleDbDataReader reader = (OleDbDataReader)await command.ExecuteReaderAsync())
                            {
                                while (reader.Read())
                                {
                                    long ptID = reader["PtIDmain"] == DBNull.Value ? 0 : Convert.ToInt64((reader["PtIDmain"]));
                                    string ptName = reader["PtName"].ToString();

                                    // PtIDとPtNameを結合した表示名を作成
                                    string displayName = $"{ptID.ToString().PadLeft(6, ' ')} : {ptName}";

                                    // DataTableに行を追加
                                    ptData.Add((ptID, displayName));
                                }
                            }
                        }

                        _parentForm.DataDbLock = false;

                        toolStrip1.Invoke(new Action(() =>
                        {

                            toolStripComboBoxPt.SelectedIndexChanged -= toolStripComboBoxPt_SelectedIndexChanged;

                            toolStripComboBoxPt.Items.Clear();
                            toolStripComboBoxPt.SelectedIndex = -1;

                            foreach (var item in ptData)
                            {
                                toolStripComboBoxPt.Items.Add(new PtItem { PtID = item.PtID, DisplayText = $"{item.PtName}" });
                            }

                            //if (toolStripComboBoxPt.Items.Count > 0)
                            //{
                            //    toolStripComboBoxPt.SelectedIndex = 0; // 初期選択
                            //}

                            toolStripComboBoxPt.SelectedIndexChanged += toolStripComboBoxPt_SelectedIndexChanged;

                            // RSB 連動か ダブルクリック起動か
                            if (_parentForm.autoRSB || _parentForm.forceIdLink)
                            {
                                _parentForm.forceIdLink = false;

                                // PtID が _parentForm.tempId に一致する行を検索
                                int index = ptData.FindIndex(p => p.PtID == _parentForm.tempId);
                                if (index >= 0)
                                {
                                    toolStripComboBoxPt.SelectedIndex = index;
                                }
                                else
                                {
                                    toolStripComboBoxPt.SelectedIndex = -1;
                                }
                            }
                        }));

                    }
                    catch (Exception ex)
                    {
                        // エラー処理
                        MessageBox.Show($"データの取得中にエラーが発生しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        _parentForm.DataDbLock = false;
                    }
                }
            }
            else
            {
                MessageBox.Show("データベースがロックされており、LoadDataIntoComboBoxに失敗しました。もう一度やり直してみてください。");
            }

        }

        private async void FormDI_Load(object sender, EventArgs e)
        {
            SetSpanButtonState(ShowSpan);
            SettoolStripButtonSpanEvent();

            toolStripButtonClass.Checked = Properties.Settings.Default.DrugClass;
            toolStripButtonClass.Enabled = (ReceptToMedisCodeMap.Count > 0);

            InitializeContextMenu();

            await LoadDataIntoComboBoxes();

            toolStripButtonSum.Checked = Properties.Settings.Default.Sum;

            toolStripButtonOmitMyOrg.Checked = Properties.Settings.Default.OmitMyOrg;

            // マウスホイールスクロールを補完
            dataGridViewFixed.MouseWheel += DataGridViewFixed_MouseWheel;

            //// DataGridViewを初期化
            //InitializeDataGridView(dataGridViewFixed);
            //InitializeDataGridView(dataGridViewDH);

            // DataGridViewにデータをバインド
            //dataGridViewFixed.DataSource = DrugHistoryData;
            //dataGridViewDH.DataSource = DrugHistoryData;

        }

        private async void toolStripComboBoxPt_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (toolStripComboBoxPt.SelectedItem is PtItem selectedPt)
                {
                    // 選択された PtID を取得
                    long ptID = selectedPt.PtID;

                    // sender が toolstripButton かどうかを判定
                    if (!(sender is ToolStripButton stripButton))
                    {
                        //コンボボックスからの場合表示期間をリセットする
                        ShowSpan = Properties.Settings.Default.ViewerSpan;
                        RemovetoolStripButtonSpanEvent();
                        SetSpanButtonState(ShowSpan);
                        SettoolStripButtonSpanEvent();
                    }

                    // DataGridView に表示するデータを取得
                    await ShowDrugData(ptID);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"患者ID選択時にエラーが発生しました:{ex.Message}");
            }
        }

        private async Task ShowDrugData(long PtID)
        {
            if (await _parentForm.WaitForDbUnlock(2000))
            {   
                string query = @"
                            TRANSFORM Sum(SubQuery.Times) AS Times
                            SELECT 
                                SubQuery.Hospital, 
                                SubQuery.DrugN, 
                                Dose,
                                SubQuery.DrugC, 
                                SubQuery.IngreN
                            FROM 
                                (
                                    SELECT 
                                        IIf(Len([PrlsHNm])=0, [MeTrDiHNm], [PrlsHNm]) AS Hospital, 
                                        drug_history.DrugN, 
                                        drug_history.DrugC,
                                        drug_history.IngreN,
                                        CStr([Qua1]) &  [Unit] AS Dose,
                                        drug_history.MeTrMonth,
                                        drug_history.DiDate,
                                        drug_history.Times
                                    FROM 
                                        drug_history
                                    WHERE 
                                        Revised = false AND %SPANFILTER% %MYORGFILTER%
                                        drug_history.PtIDmain = ?
                                ) AS SubQuery
                            GROUP BY 
                                SubQuery.Hospital, 
                                SubQuery.DrugN, 
                                SubQuery.DrugC, 
                                SubQuery.IngreN,
                                Dose
                            ORDER BY 
                                %FIELD% DESC
                            PIVOT
                                %FIELD% ;
                            ";

                string pivot = (Properties.Settings.Default.Sum) ? "SubQuery.MeTrMonth" : "SubQuery.DiDate";

                //long viewSpan = GetComboBoxSelectedValue(comboBoxSpan);
                string startDate = DateTime.Now.AddMonths(0 - ShowSpan).ToString("yyyyMM");
                string spanfilter = (ShowSpan == 0) ? "" : $" MeTrMonth >= '{startDate}' AND ";
                string myorgfilter = (Properties.Settings.Default.OmitMyOrg) ? "drug_history.PrIsOrg <> 1 AND " : "";

                query = query.Replace("%FIELD%", pivot);
                query = query.Replace("%SPANFILTER%", spanfilter);
                query = query.Replace("%MYORGFILTER%", myorgfilter);

                // 固定列の定義
                var fixedColumns = new List<string> { "Hospital", "DrugN", "Qua1", "Unit" };

                try
                {
                    int colorIndex = 0, i = 0;

                    using (OleDbConnection connection = new OleDbConnection(connectionOQSData))
                    {
                        // 接続を開く
                        await connection.OpenAsync();

                        // データ取得
                        using (var command = new OleDbCommand(query, connection))
                        {

                            command.Parameters.AddWithValue("?", PtID);

                            _parentForm.DataDbLock = true;

                            using (var reader = await command.ExecuteReaderAsync())
                            using (var dataTable = new DataTable())
                            {
                                dataTable.Load(reader);

                                _parentForm.DataDbLock = false;

                                // KOROdataからmedisCode
                                if (!dataTable.Columns.Contains("medisCode"))
                                {
                                    dataTable.Columns.Add("MedisCode", typeof(string));
                                }

                                //Row Color
                                if (!dataTable.Columns.Contains("Color"))
                                {
                                    dataTable.Columns.Add("Color", typeof(Color));
                                }

                                // 手動で加工する

                                string previousHospital = null; // 前回のHospital値
                                foreach (DataRow row in dataTable.Rows)
                                {
                                    // Hospital列を加工
                                    if (previousHospital == null || (row["Hospital"] != DBNull.Value && previousHospital !=  row["Hospital"].ToString()))
                                    { //新しい病院
                                        previousHospital = row["Hospital"].ToString();
                                        colorIndex++;
                                        if (colorIndex > 1) colorIndex = 0;
                                    }
                                    else //同じ病院のレコード
                                    {
                                        row["Hospital"] = ""; // 同じHospitalが続く場合は空白
                                    }
                                    

                                    string receptCode = row["DrugC"] == DBNull.Value ? "" : row["DrugC"].ToString();
                                    if (ReceptToMedisCodeMap.TryGetValue(receptCode, out string medisCode))
                                    {
                                        row["medisCode"] = medisCode;
                                    }
                                    else
                                    {
                                        row["medisCode"] = DBNull.Value;
                                    }

                                    if (Properties.Settings.Default.DrugClass)
                                    {
                                        row["Color"] = row["medisCode"] != DBNull.Value ? getDrugClassColor(row["medisCode"].ToString()) : Color.Empty;
                                    }
                                    else
                                    {
                                        row["Color"] = RowColors[colorIndex];
                                    }


                                    i++;
                                }

                                DrugHistoryData = dataTable;
                            }
                        }
                    }

                    // DataGridViewにバインド
                    // 非同期関数内でのUIスレッド操作
                    Invoke(new Action(() =>
                    {
                        //// DataGridViewを初期化
                        InitializeDataGridView(dataGridViewFixed);
                        InitializeDataGridView(dataGridViewDH);

                        //// DataGridViewにデータをバインド
                        dataGridViewFixed.DataSource = DrugHistoryData;
                        dataGridViewDH.DataSource = DrugHistoryData;

                        // DataGridViewの外観や動作を設定
                        ConfigureDataGridView(dataGridViewFixed);
                        ConfigureDataGridView(dataGridViewDH);
                    }));



                }
                catch (Exception ex)
                {
                    MessageBox.Show($"エラーが発生しました: {ex.Message}");
                }
                finally
                {
                    _parentForm.DataDbLock = false;
                }
            }
            else
            {
                MessageBox.Show("データベースがロックされており、ShowDrugDataに失敗しました。もう一度やり直してみてください。");
            }
        }

        private Color getDrugClassColor(string yjCode)
        {
            string digit1 = yjCode.Substring(0, 1);
            Color color;

            switch (digit1)
            {
                case "1": color = Color.FromArgb(208, 232, 242); break; // 淡い青
                case "2": color = Color.FromArgb(214, 245, 214); break; // 淡い緑
                case "3": color = Color.FromArgb(255, 228, 181); break; // 淡いオレンジ
                case "4": color = Color.FromArgb(255, 218, 218); break; // 淡いピンク
                case "5": color = Color.FromArgb(245, 230, 196); break; // 淡い黄土色
                case "6": color = Color.FromArgb(227, 215, 255); break; // 淡い紫
                case "7": color = Color.FromArgb(234, 234, 234); break; // 淡いグレー
                case "8": color = Color.FromArgb(255, 202, 202); break; // 淡い赤
                default: color = Color.White; break; // その他は白
            }
            return color;
        }

        private void InitializeDataGridView(DataGridView dataGridView)
        {
            dataGridView.DataSource = null;
            dataGridView.Rows.Clear();
            dataGridView.Columns.Clear();
            dataGridView.Refresh();

            // レコードセレクタを非表示にする
            dataGridView.RowHeadersVisible = false;

            // カラム幅を自動調整する
            dataGridView.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);

            // 行の高さを変更できないようにする
            dataGridView.AllowUserToResizeRows = false;

            dataGridView.AllowUserToAddRows = false;

            
        }

        private void ConfigureDataGridView(DataGridView dataGridView)
        {
            //共通
            // レコードセレクタを非表示にする
            dataGridView.RowHeadersVisible = false;

            // ソート機能を無効にする
            //dataGridView.AllowUserToOrderColumns = false;
            //各列のソートモードを無効にする
            foreach (DataGridViewColumn column in dataGridView.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }

            dataGridView.EditMode = DataGridViewEditMode.EditProgrammatically;
            dataGridView.SelectionMode = DataGridViewSelectionMode.CellSelect;
            
            // 縦方向の罫線を非表示にする
            dataGridView.CellBorderStyle = DataGridViewCellBorderStyle.Raised;
            // 特定の行の背景色を変える（例: 2番目の行の背景色を変更）
            for (int i = 0; i < dataGridView.Rows.Count; i++)
            {
                dataGridView.Rows[i].DefaultCellStyle.BackColor = dataGridView.Rows[i].Cells["Color"].Value as Color? ?? Color.Empty;
            }
            
            //個別
            if (dataGridView.Name.Contains("Fixed"))
            {
                //dataGridView.ScrollBars = ScrollBars.Vertical; // スクロールバーを無効化
                                                               // Fixedはスクロールバーを非表示にする
                dataGridView.ScrollBars = ScrollBars.Horizontal;

                for (int i = 0; i < dataGridView.Columns.Count; i++)
                {
                    if (i == 0)
                    {
                        dataGridView.Columns[i].Width = 120;
                        dataGridView.Columns[i].DefaultCellStyle.Font = new Font("Meiryo UI", 8);
                    }
                    else if (i == 1) dataGridView.Columns[i].Width = dataGridView.Width - 180;
                    else if (i == 2)
                    {
                        dataGridView.Columns[i].Frozen = true;
                        dataGridView.Columns[i].Width = 60;
                    } 
                    else if (i > 2) dataGridView.Columns[i].Visible = false;
                }
            }
            else //dataGridViewDH
            {
                dataGridView.Left = dataGridViewFixed.Right;
                dataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

                for (int i = 0; i < dataGridView.Columns.Count; i++)
                {
                    if (i < 5)
                    {
                        dataGridView.Columns[i].Visible = false;
                    }
                    else
                    {
                        dataGridView.Columns[i].Width = 60;
                    }
                }
                //dataGridView.Columns["DrugC"].Visible = true;
                dataGridView.Columns["Color"].Visible = false;
                dataGridView.Columns["MedisCode"].Visible = false;

                AdjustFixedHeight();
            }
           
        }

        private void toolStripButtonClose_Click(object sender, EventArgs e)
        {
            if(this.WindowState == FormWindowState.Maximized || this.WindowState == FormWindowState.Minimized) this.WindowState = FormWindowState.Normal;

            this.Close();
        }

        // 変数値に基づいてラジオボタンの状態を設定
        private void SetSpanButtonState(int value)
        {
            foreach (ToolStripItem item in toolStrip1.Items)
            {
                // 名前が "toolStripButtonSpan" で始まり、ToolStripButtonである場合
                if (item is ToolStripButton button && item.Name.StartsWith("toolStripButtonSpan"))
                {
                    // Tag の値が指定された値と一致する場合のみ Checked を true にする
                    if (int.TryParse(button.Tag.ToString(), out int tagValue))
                    {
                        button.Checked = (tagValue == value);
                    }
                }
            }
        }

        private void toolStripButtonSpan_CheckedChanged(object sender, EventArgs e)
        {
            ToolStripButton button = sender as ToolStripButton;

            if (button != null && button.Checked)
            {
                if (int.TryParse(button.Tag.ToString(), out int tagValue))
                {
                    ShowSpan = tagValue; // Tag から値を取得

                    //他のボタンをOffにする
                    SetSpanButtonState(ShowSpan);
                }
            }

            toolStripComboBoxPt_SelectedIndexChanged(sender, e);
        }

        private void SettoolStripButtonSpanEvent()
        {
            //toolStripButtonSpan1M.CheckedChanged += toolStripButtonSpan_CheckedChanged;
            toolStripButtonSpan3.CheckedChanged += toolStripButtonSpan_CheckedChanged;
            toolStripButtonSpan6.CheckedChanged += toolStripButtonSpan_CheckedChanged;
            toolStripButtonSpan12.CheckedChanged += toolStripButtonSpan_CheckedChanged;
            toolStripButtonSpanAll.CheckedChanged += toolStripButtonSpan_CheckedChanged;
        }

        private void RemovetoolStripButtonSpanEvent()
        {
            //toolStripButtonSpan1M.CheckedChanged -= toolStripButtonSpan_CheckedChanged;
            toolStripButtonSpan3.CheckedChanged -= toolStripButtonSpan_CheckedChanged;
            toolStripButtonSpan6.CheckedChanged -= toolStripButtonSpan_CheckedChanged;
            toolStripButtonSpan12.CheckedChanged -= toolStripButtonSpan_CheckedChanged;
            toolStripButtonSpanAll.CheckedChanged -= toolStripButtonSpan_CheckedChanged;
        }

        private void InitializeContextMenu()
        {
            // ContextMenuStripの作成
            ContextMenuStrip contextMenuStrip = new ContextMenuStrip();

            // 「全角でコピー」メニューアイテムを作成
            ToolStripMenuItem copyFullMenuItem = new ToolStripMenuItem("表示のままコピー");
            copyFullMenuItem.Tag = "full"; // 引数として識別するためのタグを設定
            copyFullMenuItem.Image = Properties.Resources.Zen;
            copyFullMenuItem.Click += CopyMenuItem_Click;

            // 「半角でコピー」メニューアイテムを作成
            ToolStripMenuItem copyHalfMenuItem = new ToolStripMenuItem("半角でコピー");
            copyHalfMenuItem.Tag = "half"; // 引数として識別するためのタグを設定
            copyHalfMenuItem.Image = Properties.Resources.Han;
            copyHalfMenuItem.Click += CopyMenuItem_Click;

            // 「薬情検索」メニューアイテムを作成（初期では追加しない）
            ToolStripMenuItem searchMedicineMenuItem = new ToolStripMenuItem("薬情検索");
            searchMedicineMenuItem.Image = Properties.Resources.Find;
            searchMedicineMenuItem.Click += SearchMedicineMenuItem_Click;

            // メニューアイテムを追加
            contextMenuStrip.Items.Add(copyFullMenuItem);
            contextMenuStrip.Items.Add(copyHalfMenuItem);
            if (_parentForm.RSBdrive != null)
            {
                contextMenuStrip.Items.Add(searchMedicineMenuItem);
            }
            // DataGridViewに右クリックメニューを設定
            dataGridViewFixed.ContextMenuStrip = contextMenuStrip;
        }

        private async void CopyMenuItem_Click(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem menuItem)
            {
                // Tagから動作を判別
                string mode = menuItem.Tag as string;

                if (dataGridViewFixed.SelectedCells.Count > 0)
                {
                    // 選択されたセルを行・列のインデックスでソート
                    var sortedCells = dataGridViewFixed.SelectedCells
                        .Cast<DataGridViewCell>()
                        .OrderBy(cell => cell.RowIndex)
                        .ThenBy(cell => cell.ColumnIndex)
                        .ToList();

                    List<string> cellValues = new List<string>();
                    foreach (DataGridViewCell selectedCell in sortedCells)
                    {
                        cellValues.Add(selectedCell.Value?.ToString() ?? string.Empty);
                    }

                    string clipboardText = string.Join(",", cellValues);
                    clipboardText = RemoveCampany(clipboardText);

                    if (mode == "half")
                    {
                        // 半角変換
                        clipboardText = Strings.StrConv(clipboardText, VbStrConv.Narrow, 0x0411);
                    }

                    // リトライを使ってクリップボードにコピー
                    bool success = (clipboardText.Length > 0) ? await CommonFunctions.RetryClipboardSetTextAsync(clipboardText) : false;

                    if (!success)
                    {
                        MessageBox.Show("クリップボードへのコピーに失敗しました。もう一度トライしてみてください。");
                    }
                }
            }
        }

        // 「薬情検索」メニューのクリック時処理
        private async void SearchMedicineMenuItem_Click(object sender, EventArgs e)
        {
            if (dataGridViewFixed.SelectedCells.Count > 0)
            {
                try
                {
                    List<string[]> results = new List<string[]>();

                    if (RSBDI.Count == 0)
                    {
                        RSBDI = _parentForm.RSBDI;
                    }

                    // 選択されたセルの中で最後のセルを取得
                    DataGridViewCell lastSelectedCell = dataGridViewFixed.SelectedCells[dataGridViewFixed.SelectedCells.Count - 1];

                    string drugName = lastSelectedCell.Value?.ToString();
                    string IngreN = dataGridViewFixed.Rows[lastSelectedCell.RowIndex].Cells["IngreN"].Value?.ToString();

                    if (drugName.Length > 0)
                    {
                        List<Tuple<string[], double>> topResults = await FuzzySearchAsync(drugName, IngreN, RSBDI, 0.2);

                        if (topResults.Count > 0)
                        {
                            // formSearchを開く、すでに開いていれば表示変更する
                            if (formSearch == null || formSearch.IsDisposed)
                            {
                                formSearch = new FormSearch(this);
                                formSearch.Show(this);
                            }

                            formSearch.SetDrugLists(topResults);
                            //formSearch.SetDrugName(topResults[0].Item1[0]);
                        }
                        else
                        {
                            MessageBox.Show("RSB薬情に該当薬剤が見つかりませんでした。");
                        }

                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("薬情表示時にエラーが発生しました。" + ex.Message);
                }
            }
        }

        // リストRSBDIのカラム1,2を対象にあいまい検索を行い、上位10件を返すメソッド
        public async Task<List<Tuple<string[], double>>> FuzzySearchAsync(string drugName, string ingreN, List<string[]> DI, double cutoffThreshold = 0.4, double bonusForOriginator = 0.4, double penaltyForMissingIngreN = 0.5)
        {
            //double bonusForOriginator = 0.4;        // "先発" の場合のボーナス
            //double penaltyForMissingIngreN = 0.5;   // 一般名が含まれない場合のペナルティ
            double weightColumn1 = 0.3;             // 1列目のスコアに対する重み
            double weightColumn2 = 0.5;             // 2列目のスコアに対する重み

            // 正規表現で「」や【】に囲まれた部分を削除
            string processedDrugName = RemoveCampany(drugName);

            //数字アルファベットは除去しておく
            //string drugNameNoDigit = RemoveDigits(drugName);

            if (ingreN == null || ingreN == "") ingreN = processedDrugName;

            // 各レコードに対してN-gram類似度を計算（非同期処理）
            var tasks = DI.Select(record => Task.Run(() =>
            {
                string column1 = record[0]; // 1列目
                string column2 = record[1]; // 2列目
                string column4 = record[3]; // 4列目（"先発" の確認に使用）

                double similarityColumn1 = CalculateNGramSimilarity(processedDrugName, column1);
                double similarityColumn2 = CalculateNGramSimilarity(ingreN, column2);

                // 編集距離を考慮したスコア
                double editDistanceScore = 1.0 - (double)CalculateLevenshteinDistance(processedDrugName, column1)
                                           / Math.Max(processedDrugName.Length, column1.Length);

                // 最終的なスコア（加重平均）
                double similarity = weightColumn1 * Math.Max(similarityColumn1, editDistanceScore) +
                                    weightColumn2 * similarityColumn2;

                bool exact = false;
                // 完全一致の特別スコア
                if (drugName == column1)
                {
                    similarity = 1.0;
                    exact = true;
                }
                else if (ingreN == column1)
                {
                    similarity = 0.9;
                    exact = true;
                }

                // "先発" のボーナス
                if (similarity > cutoffThreshold && column4 == "先発")
                {
                    similarity += bonusForOriginator;
                }

                // 一般名が含まれない場合のペナルティ
                if (!exact && !column2.Contains(ingreN) && !ingreN.Contains(column2))
                {
                    similarity -= penaltyForMissingIngreN;
                }

                // 類似度の下限を 0 に制限
                similarity = Math.Max(0, similarity);

                return new Tuple<string[], double>(record, similarity);  // レコードと類似度のタプルを返す
            }));

            // 全タスクの完了を待つ
            var results = await Task.WhenAll(tasks);

            // 類似度の降順で並び替え、カットオフ値以上のものだけフィルタリング
            var filteredResults = results
                .Where(r => r.Item2 >= cutoffThreshold)  // 類似度がカットオフ値以上のものを選択
                .OrderByDescending(r => r.Item2)  // 類似度の降順で並べ替え
                .Take(20)  // 上位10件を取得
                .ToList();

            return filteredResults;
        }

        private string RemoveCampany(string drugName)
        {
            // 正規表現で「」や【】に囲まれた部分を削除
            string pattern = @"[「【（][^」】）]*[」】）]";
            string processedDrugName = Regex.Replace(drugName, pattern, "");

            return processedDrugName;
        }

        private HashSet<string> GenerateNGrams(string input, int n)
        {
            var ngrams = new HashSet<string>();
            if (input.Length < n) return ngrams;

            for (int i = 0; i <= input.Length - n; i++)
            {
                ngrams.Add(input.Substring(i, n));
            }

            return ngrams;
        }

        private double CalculateNGramSimilarity(string source, string target, int n = 2)
        {
            var sourceNGrams = GenerateNGrams(source, n);
            var targetNGrams = GenerateNGrams(target, n);

            if (sourceNGrams.Count == 0 || targetNGrams.Count == 0) return 0.0;

            int intersectionCount = sourceNGrams.Intersect(targetNGrams).Count();
            int unionCount = sourceNGrams.Union(targetNGrams).Count();

            return (double)intersectionCount / unionCount;
        }

        public int CalculateLevenshteinDistance(string source, string target)
        {
            int[,] dp = new int[source.Length + 1, target.Length + 1];

            for (int i = 0; i <= source.Length; i++) dp[i, 0] = i;
            for (int j = 0; j <= target.Length; j++) dp[0, j] = j;

            for (int i = 1; i <= source.Length; i++)
            {
                for (int j = 1; j <= target.Length; j++)
                {
                    int cost = source[i - 1] == target[j - 1] ? 0 : 1;
                    dp[i, j] = Math.Min(Math.Min(dp[i - 1, j] + 1, dp[i, j - 1] + 1), dp[i - 1, j - 1] + cost);
                }
            }

            return dp[source.Length, target.Length];
        }

        private void toolStripButtonSum_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.Sum = toolStripButtonSum.Checked;
            Properties.Settings.Default.Save();

            if (toolStripButtonSum.Checked)
            {

            }

            toolStripComboBoxPt_SelectedIndexChanged(sender, EventArgs.Empty);
        }

        private void toolStripButtonOmitMyOrg_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.OmitMyOrg = toolStripButtonOmitMyOrg.Checked;
            Properties.Settings.Default.Save();

            toolStripComboBoxPt_SelectedIndexChanged(sender, EventArgs.Empty);
        }

        private void dataGridViewDH_Scroll(object sender, ScrollEventArgs e)
        {
            if (e.ScrollOrientation == ScrollOrientation.VerticalScroll)
            {
                dataGridViewFixed.FirstDisplayedScrollingRowIndex = dataGridViewDH.FirstDisplayedScrollingRowIndex;
            }
        }

        private void dataGridViewFixed_Scroll(object sender, ScrollEventArgs e)
        {
            if (e.ScrollOrientation == ScrollOrientation.VerticalScroll)
            {
                dataGridViewDH.FirstDisplayedScrollingRowIndex = dataGridViewFixed.FirstDisplayedScrollingRowIndex;
            }
        }

        private void FormDI_LocationChanged(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal)
            {
                CommonFunctions.SnapToScreenEdges(this, SnapDistance, SnapCompPixel);
            }
        }

        private void FormDI_SizeChanged(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal)
            {
                CommonFunctions.SnapToScreenEdges(this, SnapDistance, SnapCompPixel);
            }
        }

        // Fixed上でマウスホイールを回したら、DHをスクロール
        private void DataGridViewFixed_MouseWheel(object sender, MouseEventArgs e)
        {
            // WheelDeltaが正なら上スクロール、負なら下スクロール
            int lines = e.Delta > 0 ? -1 : 1;
            int newIndex = Math.Max(0, Math.Min(dataGridViewDH.RowCount - 1, dataGridViewDH.FirstDisplayedScrollingRowIndex + lines));

            // DHのスクロールを動かす
            dataGridViewDH.FirstDisplayedScrollingRowIndex = newIndex;
        }

        private void toolStripButtonClass_CheckStateChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.DrugClass = toolStripButtonClass.Checked;
            Properties.Settings.Default.Save();

            toolStripComboBoxPt_SelectedIndexChanged(sender, EventArgs.Empty);
        }

        
        private void AdjustFixedHeight()
        {
            // **表示されているカラムのみ** の幅を合計
            int totalColumnWidth = dataGridViewDH.Columns
                .Cast<DataGridViewColumn>()
                .Where(c => c.Visible)  // **表示されているカラムのみ対象**
                .Sum(c => c.Width);

            // DataGridView のクライアント領域の幅
            int clientWidth = dataGridViewDH.ClientSize.Width;

            // **縦スクロールバーが表示されているか判定**
            bool isVScrollBarVisible = dataGridViewDH.Rows.Count * dataGridViewDH.RowTemplate.Height > dataGridViewDH.ClientSize.Height;

            // **横スクロールバーが表示されるかを判定**
            bool isHScrollBarVisible = totalColumnWidth > (clientWidth - (isVScrollBarVisible ? SystemInformation.VerticalScrollBarWidth : 0));

            // **Fixed の高さを調整**
            dataGridViewFixed.Height = isHScrollBarVisible ? dataGridViewDH.Height - dataGridViewFixed.RowTemplate.Height : dataGridViewDH.Height;
        }

        private void dataGridViewDH_Resize(object sender, EventArgs e)
        {
            AdjustFixedHeight();
        }
    }
}

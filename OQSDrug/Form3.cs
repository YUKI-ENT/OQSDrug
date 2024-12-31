using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OQSDrug
{
    public partial class Form3 : Form
    {
        private Form1 _parentForm;
        private string provider;

        private Color[] RowColors = { Color.White, Color.WhiteSmoke};

        public Form3(Form1 parentForm)
        {
            InitializeComponent();

            _parentForm = parentForm;
            provider = _parentForm.DBProvider;
        }

        public async Task LoadDataIntoComboBoxes()
        {
            // クエリ文字列
            string query = @"SELECT 
                                dh.PtIDmain, 
                                dh.PtName, 
                                dh.ID 
                            FROM 
                                drug_history AS dh
                            WHERE 
                                dh.ID = (
                                    SELECT MAX(sub_dh.ID) 
                                    FROM drug_history AS sub_dh 
                                    WHERE sub_dh.PtIDmain = dh.PtIDmain
                                )
                            ORDER BY dh.ID DESC;
                            ";
            string connectionOQSData = $"Provider={provider};Data Source={Properties.Settings.Default.OQSDrugData};";
            // データテーブルを作成
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("PtID", typeof(long));
            dataTable.Columns.Add("DisplayName", typeof(string));

            using (OleDbConnection connection = new OleDbConnection(connectionOQSData))
            {
                try
                {
                    // 接続を開く
                    await connection.OpenAsync();

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
                                dataTable.Rows.Add(ptID, displayName);
                            }
                        }
                    }

                    // ComboBoxのデータソースを一旦クリア
                    comboBoxPtID.DataSource = null;

                    // ComboBoxにデータをバインド
                    comboBoxPtID.DataSource = dataTable;
                    comboBoxPtID.ValueMember = "PtID";
                    comboBoxPtID.DisplayMember = "DisplayName";

                    // RSB 連動
                    if (_parentForm.autoRSB)
                    {
                        // PtID が _parentForm.tempId に一致する行を検索
                        DataRow[] rows = dataTable.Select($"PtID = {_parentForm.tempId}");
                        // 一致するデータが見つかれば、選択する
                        if (rows.Length > 0)
                        {
                            // 最初の一致したレコードの PtID を SelectedValue に設定
                            comboBoxPtID.SelectedValue = rows[0]["PtID"];
                        }
                        else
                        {
                            comboBoxPtID.SelectedIndex = -1;
                        }
                    }

                }
                catch (Exception ex)
                {
                    // エラー処理
                    MessageBox.Show($"データの取得中にエラーが発生しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private async void Form3_Load(object sender, EventArgs e)
        {
            InitializeContextMenu();

            await LoadDataIntoComboBoxes();

            checkBoxSum.Checked = Properties.Settings.Default.Sum;


        }

        private async Task ShowDrugData(long PtID)
        {
            // 動的クエリをC#コード内で作成
            string query = @"
                            TRANSFORM Sum(drug_history.Times) AS Times
                            SELECT 
                                IIf(Len([PrlsHNm])=0, [MeTrDiHNm], [PrlsHNm]) AS Hospital, 
                                drug_history.DrugN, 
                                drug_history.Qua1, 
                                drug_history.Unit
                            FROM 
                                drug_history
                            WHERE 
                                Revised = false AND 
                                drug_history.PtIDmain = ?
                            GROUP BY 
                                IIf(Len([PrlsHNm])=0, [MeTrDiHNm], [PrlsHNm]), 
                                drug_history.DrugN, 
                                drug_history.Qua1, 
                                drug_history.Unit
                            ORDER BY 
                                %FIELD% DESC
                            PIVOT
                                %FIELD% ;";

            string pivot = (Properties.Settings.Default.Sum) ? "drug_history.MeTrMonth" : "drug_history.DiDate";

            query = query.Replace("%FIELD%", pivot);

            // 固定列の定義
            var fixedColumns = new List<string> { "Hospital", "DrugN", "Qua1", "Unit" };

            string connectionOQSData = $"Provider={provider};Data Source={Properties.Settings.Default.OQSDrugData};";

            try
            {
                using (OleDbConnection connection = new OleDbConnection(connectionOQSData))
                {
                    // 接続を開く
                    await connection.OpenAsync();

                    // データ取得
                    using (var command = new OleDbCommand(query, connection))
                    {

                        command.Parameters.AddWithValue("?", PtID);
                     
                        using (OleDbDataAdapter adapter = new OleDbDataAdapter(command))
                        using (var dataTable = new DataTable())
                        {
                            await Task.Run(() => adapter.Fill(dataTable));

                            // 手動で加工する
                            var processedTable = new DataTable();
                            Color[] RowColorSetting = new Color[100];
                            int colorIndex = 0, i=0;

                            // 列構造をコピー
                            foreach (DataColumn col in dataTable.Columns)
                            {
                                processedTable.Columns.Add(col.ColumnName, col.DataType);
                            }

                            string previousHospital = null; // 前回のHospital値
                            foreach (DataRow row in dataTable.Rows)
                            {
                                var newRow = processedTable.NewRow();

                                // Hospital列を加工
                                if (previousHospital == null || previousHospital != row["Hospital"].ToString())
                                {
                                    newRow["Hospital"] = row["Hospital"];
                                    previousHospital = row["Hospital"].ToString();
                                    colorIndex++;
                                    if (colorIndex > 1) colorIndex = 0;
                                }
                                else
                                {
                                    newRow["Hospital"] = ""; // 同じHospitalが続く場合は空白
                                }
                                RowColorSetting[i] = RowColors[colorIndex];
                                i++;

                                // MeTrMonth列を加工して下り順に入力 (例: 202409などの列があればソート)
                                var pivotColumns = dataTable.Columns.Cast<DataColumn>()
                                    .Where(c => c.ColumnName != "PtIDmain" && c.ColumnName != "PtName" && c.ColumnName != "Hospital" &&
                                                c.ColumnName != "DrugN" && c.ColumnName != "Qua1" && c.ColumnName != "Unit")
                                    .OrderByDescending(c => c.ColumnName);

                                foreach (var col in pivotColumns)
                                {
                                    newRow[col.ColumnName] = row[col.ColumnName];
                                }

                                // その他の列をそのままコピー
                                foreach (DataColumn col in dataTable.Columns)
                                {
                                    if (!pivotColumns.Contains(col) && col.ColumnName != "Hospital")
                                    {
                                        newRow[col.ColumnName] = row[col.ColumnName];
                                    }
                                }

                                processedTable.Rows.Add(newRow);
                            }

                            // DataGridViewにバインド
                            InitializeDataGridView(dataGridViewDH);
                            dataGridViewDH.DataSource = processedTable;
                            ConfigureDataGridView(dataGridViewDH, RowColorSetting);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"エラーが発生しました: {ex.Message}");
            }
        }

        private async void comboBoxPtID_SelectedIndexChanged(object sender, EventArgs e)
        {

            // PtIDmainの値を取得
            string strPtID = comboBoxPtID.SelectedValue.ToString();
            //MessageBox.Show(strPtID);
            // 数字としてパース可能かチェック
            if (long.TryParse(strPtID, out long PtID))
            {

                // 必要に応じて処理を実行
                await ShowDrugData(PtID);
            }

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

        }

        private void ConfigureDataGridView(DataGridView dataGridView, Color[] rowColors)
        {
            // レコードセレクタを非表示にする
            dataGridView.RowHeadersVisible = false;

            // カラム幅を自動調整する
            dataGridView.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);

            // 特定のカラムの幅を固定にする（例: 1番目と3番目のカラム）
            if (dataGridView.Columns.Count > 0)
            {
                dataGridView.Columns[0].Width = 150;  // 1番目のカラム
                
            }

            // ソート機能を無効にする
            dataGridView.AllowUserToOrderColumns = false;
            // 各列のソートモードを無効にする
            foreach (DataGridViewColumn column in dataGridView.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }

            // 特定の行の背景色を変える（例: 2番目の行の背景色を変更）
            for(int i = 0; i < dataGridView.Rows.Count; i++)
            {
                dataGridView.Rows[i].DefaultCellStyle.BackColor = rowColors[i];
            }

            // 縦方向の罫線を非表示にする
            dataGridView.CellBorderStyle = DataGridViewCellBorderStyle.Raised;



        }

        private void InitializeContextMenu()
        {
            // ContextMenuStripの作成
            ContextMenuStrip contextMenuStrip = new ContextMenuStrip();

            // 「コピー」メニューアイテムを作成
            ToolStripMenuItem copyMenuItem = new ToolStripMenuItem("コピー");
            copyMenuItem.Click += CopyMenuItem_Click;

            // メニューアイテムを追加
            contextMenuStrip.Items.Add(copyMenuItem);

            // DataGridViewに右クリックメニューを設定
            dataGridViewDH.ContextMenuStrip = contextMenuStrip;
        }

        private void CopyMenuItem_Click(object sender, EventArgs e)
        {
            // DataGridViewの選択されているセルを確認
            if (dataGridViewDH.SelectedCells.Count > 0)
            {
                // セルの値を格納するためのリスト
                List<string> cellValues = new List<string>();

                // 選択されたすべてのセルをループ
                foreach (DataGridViewCell selectedCell in dataGridViewDH.SelectedCells)
                {
                    // セルの値をリストに追加
                    cellValues.Add(selectedCell.Value?.ToString() ?? string.Empty);
                }

                // セルの値を区切り文字で連結（カンマ区切りなど）
                string clipboardText = string.Join(",", cellValues);

                // クリップボードにコピー
                Clipboard.SetText(clipboardText);
            }
        }

        private void checkBoxSum_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.Sum = checkBoxSum.Checked;
            Properties.Settings.Default.Save();

            comboBoxPtID_SelectedIndexChanged(comboBoxPtID, EventArgs.Empty);
        }
    }
}

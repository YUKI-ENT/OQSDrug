using Microsoft.VisualBasic;
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
using static OQSDrug.FormTKK;

namespace OQSDrug
{
    public partial class FormSR : Form
    {
        private const int SnapDistance = 16; // 吸着の距離（ピクセル）
        private int SnapCompPixel = 8;  //余白補正

        private Form1 _parentForm;

        private Color[] RowColors = { Color.WhiteSmoke, Color.White };

        private List<(long PtID, string PtName)> ptData = new List<(long PtID, string PtName)>();

        private DataTable SinryoData = new DataTable();

        private int ShowSpan = Properties.Settings.Default.ViewerSpan;

        public FormSR(Form1 parentForm)
        {
            InitializeComponent();

            _parentForm = parentForm;
            toolStrip1.Renderer = new CustomToolStripRenderer(); // カスタム描画を適用
        }

        public async Task LoadDataIntoComboBoxes()
        {
            if (await CommonFunctions.WaitForDbUnlock(2000))
            {
                ptData = new List<(long PtID, string PtName)>();

                string query = @"SELECT PtIDmain, PtName, Max(id) AS Maxid
                            FROM sinryo_history
                            GROUP BY PtIDmain, PtName
                            ORDER BY Max(id) DESC;";

                using (OleDbConnection connection = new OleDbConnection(CommonFunctions.connectionReadOQSdata))
                {
                    try
                    {
                        await connection.OpenAsync();

                        CommonFunctions.DataDbLock = true;

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

                        CommonFunctions.DataDbLock = false;

                        toolStrip1.Invoke(new Action(() =>
                        {

                            toolStripComboBoxPtID.SelectedIndexChanged -= toolStripComboBoxPtID_SelectedIndexChanged;

                            toolStripComboBoxPtID.Items.Clear();
                            toolStripComboBoxPtID.SelectedIndex = -1;

                            foreach (var item in ptData)
                            {
                                toolStripComboBoxPtID.Items.Add(new PtItem { PtID = item.PtID, DisplayText = $"{item.PtName}" });
                            }

                            toolStripComboBoxPtID.SelectedIndexChanged += toolStripComboBoxPtID_SelectedIndexChanged;

                            // RSB 連動か ダブルクリック起動か
                            if (_parentForm.autoRSB || _parentForm.forceIdLink)
                            {
                                _parentForm.forceIdLink = false;

                                // PtID が _parentForm.tempId に一致する行を検索
                                int index = ptData.FindIndex(p => p.PtID == _parentForm.tempId);
                                if (index >= 0)
                                {
                                    toolStripComboBoxPtID.SelectedIndex = index;
                                }
                                else
                                {
                                    toolStripComboBoxPtID.SelectedIndex = -1;
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
                        CommonFunctions.DataDbLock = false;
                    }
                }
            }
            else
            {
                MessageBox.Show("データベースがロックされており、LoadDataIntoComboBoxに失敗しました。もう一度やり直してみてください。");
            }

        }

        private void toolStripButtonClose_Click(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Maximized || this.WindowState == FormWindowState.Minimized) this.WindowState = FormWindowState.Normal;

            this.Close();
        }

        private async void toolStripComboBoxPtID_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (toolStripComboBoxPtID.SelectedItem is PtItem selectedPt)
                {
                    // 選択された PtID を取得
                    long ptID = selectedPt.PtID;

                    // DataGridView に表示するデータを取得
                    await ShowSRData(ptID);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"患者ID選択時にエラーが発生しました:{ex.Message}");
            }
        }

        private async void FormSR_Load(object sender, EventArgs e)
        {
            toolStripButtonSum.Checked = Properties.Settings.Default.Sum;

            await LoadDataIntoComboBoxes();

            InitializeContextMenu();
        }

        private async Task ShowSRData(long PtID)
        {
            if (await CommonFunctions.WaitForDbUnlock(2000))
            {
                string pivot = (toolStripButtonSum.Checked) ? "sinryo_history.[MeTrMonth]" : "sinryo_history.[DiDate]";

                string query = $@"
                                TRANSFORM Sum(sinryo_history.[Times]) AS SRTimes
                                SELECT sinryo_history.[MeTrDiHNm] AS Hospital, sinryo_history.[SinInfN], sinryo_history.[Qua1]
                                FROM sinryo_history
                                WHERE PtIDmain = ? 
                                GROUP BY sinryo_history.[MeTrDiHNm], sinryo_history.[SinInfN], sinryo_history.[Qua1]
                                ORDER BY {pivot} DESC
                                PIVOT  {pivot} ;
                                ";
                try
                {
                    int colorIndex = 0, i = 0;

                    using (OleDbConnection connection = new OleDbConnection(CommonFunctions.connectionReadOQSdata))
                    {
                        // 接続を開く
                        await connection.OpenAsync();

                        // データ取得
                        using (var command = new OleDbCommand(query, connection))
                        {

                            command.Parameters.AddWithValue("?", PtID);

                            CommonFunctions.DataDbLock = true;

                            using (var reader = await command.ExecuteReaderAsync())
                            using (var dataTable = new DataTable())
                            {
                                dataTable.Load(reader);

                                CommonFunctions.DataDbLock = false;

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
                                    if (previousHospital == null || (row["Hospital"] != DBNull.Value && previousHospital != row["Hospital"].ToString()))
                                    { //新しい病院
                                        previousHospital = row["Hospital"].ToString();
                                        colorIndex++;
                                        if (colorIndex > 1) colorIndex = 0;
                                    }
                                    else //同じ病院のレコード
                                    {
                                        row["Hospital"] = ""; // 同じHospitalが続く場合は空白
                                    }
                                    row["Color"] = RowColors[colorIndex];

                                    i++;
                                }

                                SinryoData = dataTable;
                            }
                        }
                    }

                    // DataGridViewにバインド
                    // 非同期関数内でのUIスレッド操作
                    Invoke(new Action(() =>
                    {
                        //// DataGridViewを初期化
                        InitializeDataGridView(dataGridViewSinryo);

                        //// DataGridViewにデータをバインド
                        dataGridViewSinryo.DataSource = SinryoData;

                        // DataGridViewの外観や動作を設定
                        ConfigureDataGridView(dataGridViewSinryo);
                    }));

                }
                catch (Exception ex)
                {
                    MessageBox.Show($"エラーが発生しました: {ex.Message}");
                }
                finally
                {
                    CommonFunctions.DataDbLock = false;
                }
            }
            else
            {
                MessageBox.Show("データベースがロックされており、ShowSinryoDataに失敗しました。もう一度やり直してみてください。");
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

            for (int i = 0; i < dataGridView.Columns.Count; i++)
            {
                if (i > 2)
                {
                    dataGridView.Columns[i].Width = 60;
                }
                else
                {
                    dataGridView.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                }
            }
            //dataGridView.Columns["DrugC"].Visible = true;
            dataGridView.Columns["Color"].Visible = false;
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

            // メニューアイテムを追加
            contextMenuStrip.Items.Add(copyFullMenuItem);
            contextMenuStrip.Items.Add(copyHalfMenuItem);

            dataGridViewSinryo.ContextMenuStrip = contextMenuStrip;
        }

        private async void CopyMenuItem_Click(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem menuItem)
            {
                // Tagから動作を判別
                string mode = menuItem.Tag as string;

                if (dataGridViewSinryo.SelectedCells.Count > 0)
                {
                    // 選択されたセルを行・列のインデックスでソート
                    var sortedCells = dataGridViewSinryo.SelectedCells
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

        private void FormSR_LocationChanged(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal)
            {
                CommonFunctions.SnapToScreenEdges(this, SnapDistance, SnapCompPixel);
            }
        }

        private void toolStripButtonSum_CheckedChanged(object sender, EventArgs e)
        {
            toolStripComboBoxPtID_SelectedIndexChanged(sender, e);
        }
    }
}

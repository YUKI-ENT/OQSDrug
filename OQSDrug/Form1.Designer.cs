﻿namespace OQSDrug
{
    partial class Form1
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージド リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.toolStripVersion = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonViewer = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonExit = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonToTaskTray = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonVersion = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonSettings = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonTKK = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonSinryo = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparatorDebug1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripComboBoxDBProviders = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripSeparatorDebug2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripTextBoxDebug = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripButtonDebug = new System.Windows.Forms.ToolStripButton();
            this.listViewLog = new System.Windows.Forms.ListView();
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.checkBoxAutoview = new System.Windows.Forms.CheckBox();
            this.StartStop = new System.Windows.Forms.CheckBox();
            this.checkBoxAutoStart = new System.Windows.Forms.CheckBox();
            this.checkBoxAutoTKK = new System.Windows.Forms.CheckBox();
            this.checkBoxAutoSR = new System.Windows.Forms.CheckBox();
            this.buttonReload = new System.Windows.Forms.Button();
            this.buttonYZ = new System.Windows.Forms.Button();
            this.buttonSR = new System.Windows.Forms.Button();
            this.buttonYZPDF = new System.Windows.Forms.Button();
            this.buttonYZXML = new System.Windows.Forms.Button();
            this.buttonKS = new System.Windows.Forms.Button();
            this.buttonKSPDF = new System.Windows.Forms.Button();
            this.buttonKSXML = new System.Windows.Forms.Button();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textBoxPtIDmain = new System.Windows.Forms.TextBox();
            this.buttonPtIDSearch = new System.Windows.Forms.Button();
            this.buttonTKKSearch = new System.Windows.Forms.Button();
            this.buttonSRSearch = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.toolStripVersion.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(12, 275);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowTemplate.Height = 21;
            this.dataGridView1.Size = new System.Drawing.Size(950, 430);
            this.dataGridView1.TabIndex = 2;
            this.dataGridView1.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellDoubleClick);
            this.dataGridView1.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dataGridView1_CellFormatting);
            this.dataGridView1.CellMouseUp += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridView1_CellMouseUp);
            this.dataGridView1.CellToolTipTextNeeded += new System.Windows.Forms.DataGridViewCellToolTipTextNeededEventHandler(this.dataGridView1_CellToolTipTextNeeded);
            // 
            // toolStripVersion
            // 
            this.toolStripVersion.Font = new System.Drawing.Font("游ゴシック", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.toolStripVersion.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonViewer,
            this.toolStripButtonExit,
            this.toolStripButtonToTaskTray,
            this.toolStripSeparator1,
            this.toolStripButtonVersion,
            this.toolStripSeparator2,
            this.toolStripButtonSettings,
            this.toolStripButtonTKK,
            this.toolStripButtonSinryo,
            this.toolStripSeparatorDebug1,
            this.toolStripComboBoxDBProviders,
            this.toolStripSeparatorDebug2,
            this.toolStripTextBoxDebug,
            this.toolStripButtonDebug});
            this.toolStripVersion.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.toolStripVersion.Location = new System.Drawing.Point(0, 0);
            this.toolStripVersion.Name = "toolStripVersion";
            this.toolStripVersion.Size = new System.Drawing.Size(974, 27);
            this.toolStripVersion.Stretch = true;
            this.toolStripVersion.TabIndex = 3;
            this.toolStripVersion.Text = "toolStrip1";
            this.toolStripVersion.DoubleClick += new System.EventHandler(this.toolStripVersion_DoubleClick);
            // 
            // toolStripButtonViewer
            // 
            this.toolStripButtonViewer.Font = new System.Drawing.Font("游ゴシック", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.toolStripButtonViewer.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonViewer.Image")));
            this.toolStripButtonViewer.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonViewer.Name = "toolStripButtonViewer";
            this.toolStripButtonViewer.Size = new System.Drawing.Size(59, 24);
            this.toolStripButtonViewer.Text = "薬歴";
            this.toolStripButtonViewer.ToolTipText = "薬歴を表示します";
            this.toolStripButtonViewer.Click += new System.EventHandler(this.toolStripButtonDI_Click);
            // 
            // toolStripButtonExit
            // 
            this.toolStripButtonExit.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripButtonExit.Font = new System.Drawing.Font("游ゴシック", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.toolStripButtonExit.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonExit.Image")));
            this.toolStripButtonExit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonExit.Name = "toolStripButtonExit";
            this.toolStripButtonExit.Size = new System.Drawing.Size(59, 24);
            this.toolStripButtonExit.Text = "終了";
            this.toolStripButtonExit.Click += new System.EventHandler(this.buttonExit_Click);
            // 
            // toolStripButtonToTaskTray
            // 
            this.toolStripButtonToTaskTray.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripButtonToTaskTray.Image = global::OQSDrug.Properties.Resources.Down;
            this.toolStripButtonToTaskTray.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonToTaskTray.Name = "toolStripButtonToTaskTray";
            this.toolStripButtonToTaskTray.Size = new System.Drawing.Size(23, 24);
            this.toolStripButtonToTaskTray.ToolTipText = "タスクトレイに最小化します";
            this.toolStripButtonToTaskTray.Click += new System.EventHandler(this.toolStripButtonToTaskTray_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 27);
            // 
            // toolStripButtonVersion
            // 
            this.toolStripButtonVersion.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripButtonVersion.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonVersion.Image = global::OQSDrug.Properties.Resources.Info;
            this.toolStripButtonVersion.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonVersion.Name = "toolStripButtonVersion";
            this.toolStripButtonVersion.Size = new System.Drawing.Size(23, 24);
            this.toolStripButtonVersion.Text = "Version";
            this.toolStripButtonVersion.Click += new System.EventHandler(this.toolStripButtonVersion_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 27);
            // 
            // toolStripButtonSettings
            // 
            this.toolStripButtonSettings.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripButtonSettings.Font = new System.Drawing.Font("游ゴシック", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.toolStripButtonSettings.Image = global::OQSDrug.Properties.Resources.Application;
            this.toolStripButtonSettings.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonSettings.Name = "toolStripButtonSettings";
            this.toolStripButtonSettings.Size = new System.Drawing.Size(59, 24);
            this.toolStripButtonSettings.Text = "設定";
            this.toolStripButtonSettings.Click += new System.EventHandler(this.toolStripButtonSettings_Click);
            // 
            // toolStripButtonTKK
            // 
            this.toolStripButtonTKK.Image = global::OQSDrug.Properties.Resources.Heart;
            this.toolStripButtonTKK.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonTKK.Name = "toolStripButtonTKK";
            this.toolStripButtonTKK.Size = new System.Drawing.Size(59, 24);
            this.toolStripButtonTKK.Text = "健診";
            this.toolStripButtonTKK.ToolTipText = "健診結果を表示します";
            this.toolStripButtonTKK.Click += new System.EventHandler(this.toolStripButtonTKK_Click);
            // 
            // toolStripButtonSinryo
            // 
            this.toolStripButtonSinryo.Image = global::OQSDrug.Properties.Resources.Equipment;
            this.toolStripButtonSinryo.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonSinryo.Name = "toolStripButtonSinryo";
            this.toolStripButtonSinryo.Size = new System.Drawing.Size(89, 24);
            this.toolStripButtonSinryo.Text = "診療手術";
            this.toolStripButtonSinryo.ToolTipText = "診療手術情報を表示します";
            this.toolStripButtonSinryo.Click += new System.EventHandler(this.toolStripButtonSinryo_Click);
            // 
            // toolStripSeparatorDebug1
            // 
            this.toolStripSeparatorDebug1.Name = "toolStripSeparatorDebug1";
            this.toolStripSeparatorDebug1.Size = new System.Drawing.Size(6, 27);
            this.toolStripSeparatorDebug1.Visible = false;
            // 
            // toolStripComboBoxDBProviders
            // 
            this.toolStripComboBoxDBProviders.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.toolStripComboBoxDBProviders.Name = "toolStripComboBoxDBProviders";
            this.toolStripComboBoxDBProviders.Size = new System.Drawing.Size(180, 27);
            this.toolStripComboBoxDBProviders.ToolTipText = "OleDbプロバイダを設定します";
            this.toolStripComboBoxDBProviders.Visible = false;
            this.toolStripComboBoxDBProviders.SelectedIndexChanged += new System.EventHandler(this.toolStripComboBoxDBProviders_SelectedIndexChanged);
            // 
            // toolStripSeparatorDebug2
            // 
            this.toolStripSeparatorDebug2.Name = "toolStripSeparatorDebug2";
            this.toolStripSeparatorDebug2.Size = new System.Drawing.Size(6, 27);
            this.toolStripSeparatorDebug2.Visible = false;
            // 
            // toolStripTextBoxDebug
            // 
            this.toolStripTextBoxDebug.Font = new System.Drawing.Font("Yu Gothic UI", 9F);
            this.toolStripTextBoxDebug.Name = "toolStripTextBoxDebug";
            this.toolStripTextBoxDebug.Size = new System.Drawing.Size(100, 27);
            this.toolStripTextBoxDebug.ToolTipText = "デバッグ用xmlの患者ID(枝番付)";
            this.toolStripTextBoxDebug.Visible = false;
            // 
            // toolStripButtonDebug
            // 
            this.toolStripButtonDebug.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButtonDebug.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonDebug.Image")));
            this.toolStripButtonDebug.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonDebug.Name = "toolStripButtonDebug";
            this.toolStripButtonDebug.Size = new System.Drawing.Size(58, 24);
            this.toolStripButtonDebug.Text = "Debug";
            this.toolStripButtonDebug.ToolTipText = "xmlファイルを読み込みます";
            this.toolStripButtonDebug.Visible = false;
            this.toolStripButtonDebug.Click += new System.EventHandler(this.toolStripButtonDebug_Click);
            // 
            // listViewLog
            // 
            this.listViewLog.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewLog.FullRowSelect = true;
            this.listViewLog.HideSelection = false;
            this.listViewLog.Location = new System.Drawing.Point(422, 91);
            this.listViewLog.Name = "listViewLog";
            this.listViewLog.Size = new System.Drawing.Size(540, 176);
            this.listViewLog.TabIndex = 8;
            this.toolTip1.SetToolTip(this.listViewLog, "ログを表示します");
            this.listViewLog.UseCompatibleStateImageBehavior = false;
            this.listViewLog.View = System.Windows.Forms.View.Details;
            this.listViewLog.SizeChanged += new System.EventHandler(this.listViewLog_SizeChanged);
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.AutoSize = true;
            this.tableLayoutPanel.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Inset;
            this.tableLayoutPanel.ColumnCount = 2;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 56.28141F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 43.71859F));
            this.tableLayoutPanel.Font = new System.Drawing.Font("Meiryo UI", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tableLayoutPanel.Location = new System.Drawing.Point(12, 137);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 4;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel.Size = new System.Drawing.Size(400, 130);
            this.tableLayoutPanel.TabIndex = 9;
            // 
            // toolTip1
            // 
            this.toolTip1.AutoPopDelay = 10100;
            this.toolTip1.InitialDelay = 300;
            this.toolTip1.ReshowDelay = 82;
            // 
            // checkBoxAutoview
            // 
            this.checkBoxAutoview.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkBoxAutoview.Font = new System.Drawing.Font("Meiryo UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.checkBoxAutoview.Image = global::OQSDrug.Properties.Resources.Text_preview;
            this.checkBoxAutoview.Location = new System.Drawing.Point(6, 16);
            this.checkBoxAutoview.Name = "checkBoxAutoview";
            this.checkBoxAutoview.Size = new System.Drawing.Size(84, 33);
            this.checkBoxAutoview.TabIndex = 27;
            this.checkBoxAutoview.Text = "薬歴";
            this.checkBoxAutoview.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.toolTip1.SetToolTip(this.checkBoxAutoview, "RSBase/ダイナと連動して薬歴が存在すれば自動で表示します");
            this.checkBoxAutoview.UseVisualStyleBackColor = true;
            this.checkBoxAutoview.CheckedChanged += new System.EventHandler(this.checkBoxAutoview_CheckedChanged);
            // 
            // StartStop
            // 
            this.StartStop.Appearance = System.Windows.Forms.Appearance.Button;
            this.StartStop.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.StartStop.Enabled = false;
            this.StartStop.Font = new System.Drawing.Font("Meiryo UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.StartStop.Image = global::OQSDrug.Properties.Resources.Go;
            this.StartStop.Location = new System.Drawing.Point(13, 30);
            this.StartStop.Name = "StartStop";
            this.StartStop.Size = new System.Drawing.Size(280, 45);
            this.StartStop.TabIndex = 6;
            this.StartStop.Text = "開始";
            this.StartStop.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.StartStop.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.toolTip1.SetToolTip(this.StartStop, "情報取得処理を開始/終了します");
            this.StartStop.UseVisualStyleBackColor = true;
            this.StartStop.CheckedChanged += new System.EventHandler(this.StartStop_CheckedChanged);
            // 
            // checkBoxAutoStart
            // 
            this.checkBoxAutoStart.Font = new System.Drawing.Font("Meiryo UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.checkBoxAutoStart.Image = global::OQSDrug.Properties.Resources.clock;
            this.checkBoxAutoStart.Location = new System.Drawing.Point(299, 40);
            this.checkBoxAutoStart.Name = "checkBoxAutoStart";
            this.checkBoxAutoStart.Size = new System.Drawing.Size(113, 45);
            this.checkBoxAutoStart.TabIndex = 29;
            this.checkBoxAutoStart.Text = "自動開始";
            this.checkBoxAutoStart.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.checkBoxAutoStart.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.toolTip1.SetToolTip(this.checkBoxAutoStart, "下の接続がOKになると自動で動作開始/停止します");
            this.checkBoxAutoStart.UseVisualStyleBackColor = true;
            this.checkBoxAutoStart.CheckedChanged += new System.EventHandler(this.checkBoxAutoStart_CheckedChanged);
            // 
            // checkBoxAutoTKK
            // 
            this.checkBoxAutoTKK.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkBoxAutoTKK.Font = new System.Drawing.Font("Meiryo UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.checkBoxAutoTKK.Image = global::OQSDrug.Properties.Resources.Heart;
            this.checkBoxAutoTKK.Location = new System.Drawing.Point(96, 16);
            this.checkBoxAutoTKK.Name = "checkBoxAutoTKK";
            this.checkBoxAutoTKK.Size = new System.Drawing.Size(84, 33);
            this.checkBoxAutoTKK.TabIndex = 28;
            this.checkBoxAutoTKK.Text = "健診";
            this.checkBoxAutoTKK.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.toolTip1.SetToolTip(this.checkBoxAutoTKK, "特定健診履歴を連動表示します");
            this.checkBoxAutoTKK.UseVisualStyleBackColor = true;
            this.checkBoxAutoTKK.CheckedChanged += new System.EventHandler(this.checkBoxAutoview_CheckedChanged);
            // 
            // checkBoxAutoSR
            // 
            this.checkBoxAutoSR.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkBoxAutoSR.Font = new System.Drawing.Font("Meiryo UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.checkBoxAutoSR.Image = global::OQSDrug.Properties.Resources.Equipment;
            this.checkBoxAutoSR.Location = new System.Drawing.Point(186, 16);
            this.checkBoxAutoSR.Name = "checkBoxAutoSR";
            this.checkBoxAutoSR.Size = new System.Drawing.Size(84, 33);
            this.checkBoxAutoSR.TabIndex = 29;
            this.checkBoxAutoSR.Text = "診療";
            this.checkBoxAutoSR.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.toolTip1.SetToolTip(this.checkBoxAutoSR, "診療手術情報を連動表示します");
            this.checkBoxAutoSR.UseVisualStyleBackColor = true;
            this.checkBoxAutoSR.CheckedChanged += new System.EventHandler(this.checkBoxAutoview_CheckedChanged);
            // 
            // buttonReload
            // 
            this.buttonReload.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonReload.Font = new System.Drawing.Font("Meiryo UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.buttonReload.Image = global::OQSDrug.Properties.Resources.Refresh;
            this.buttonReload.Location = new System.Drawing.Point(922, 46);
            this.buttonReload.Name = "buttonReload";
            this.buttonReload.Size = new System.Drawing.Size(40, 33);
            this.buttonReload.TabIndex = 28;
            this.toolTip1.SetToolTip(this.buttonReload, "取得結果を更新します");
            this.buttonReload.UseVisualStyleBackColor = true;
            this.buttonReload.Click += new System.EventHandler(this.buttonReload_Click);
            // 
            // buttonYZ
            // 
            this.buttonYZ.Enabled = false;
            this.buttonYZ.Location = new System.Drawing.Point(13, 87);
            this.buttonYZ.Name = "buttonYZ";
            this.buttonYZ.Size = new System.Drawing.Size(140, 23);
            this.buttonYZ.TabIndex = 19;
            this.buttonYZ.Text = "薬剤情報";
            this.buttonYZ.UseVisualStyleBackColor = true;
            // 
            // buttonSR
            // 
            this.buttonSR.Enabled = false;
            this.buttonSR.Location = new System.Drawing.Point(153, 87);
            this.buttonSR.Name = "buttonSR";
            this.buttonSR.Size = new System.Drawing.Size(140, 23);
            this.buttonSR.TabIndex = 20;
            this.buttonSR.Text = "診療情報";
            this.buttonSR.UseVisualStyleBackColor = true;
            // 
            // buttonYZPDF
            // 
            this.buttonYZPDF.Enabled = false;
            this.buttonYZPDF.Location = new System.Drawing.Point(311, 87);
            this.buttonYZPDF.Name = "buttonYZPDF";
            this.buttonYZPDF.Size = new System.Drawing.Size(37, 23);
            this.buttonYZPDF.TabIndex = 21;
            this.buttonYZPDF.Text = "PDF";
            this.buttonYZPDF.UseVisualStyleBackColor = true;
            // 
            // buttonYZXML
            // 
            this.buttonYZXML.Enabled = false;
            this.buttonYZXML.Location = new System.Drawing.Point(354, 87);
            this.buttonYZXML.Name = "buttonYZXML";
            this.buttonYZXML.Size = new System.Drawing.Size(37, 23);
            this.buttonYZXML.TabIndex = 22;
            this.buttonYZXML.Text = "XML";
            this.buttonYZXML.UseVisualStyleBackColor = true;
            // 
            // buttonKS
            // 
            this.buttonKS.Enabled = false;
            this.buttonKS.Location = new System.Drawing.Point(13, 108);
            this.buttonKS.Name = "buttonKS";
            this.buttonKS.Size = new System.Drawing.Size(280, 23);
            this.buttonKS.TabIndex = 23;
            this.buttonKS.Text = "健診情報";
            this.buttonKS.UseVisualStyleBackColor = true;
            // 
            // buttonKSPDF
            // 
            this.buttonKSPDF.Enabled = false;
            this.buttonKSPDF.Location = new System.Drawing.Point(311, 108);
            this.buttonKSPDF.Name = "buttonKSPDF";
            this.buttonKSPDF.Size = new System.Drawing.Size(37, 23);
            this.buttonKSPDF.TabIndex = 24;
            this.buttonKSPDF.Text = "PDF";
            this.buttonKSPDF.UseVisualStyleBackColor = true;
            // 
            // buttonKSXML
            // 
            this.buttonKSXML.Enabled = false;
            this.buttonKSXML.Location = new System.Drawing.Point(354, 108);
            this.buttonKSXML.Name = "buttonKSXML";
            this.buttonKSXML.Size = new System.Drawing.Size(37, 23);
            this.buttonKSXML.TabIndex = 25;
            this.buttonKSXML.Text = "XML";
            this.buttonKSXML.UseVisualStyleBackColor = true;
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "OQSDrug\r\n右クリックでメニュー表示\r\nダブルクリックで表示非表示切替";
            this.notifyIcon1.Visible = true;
            this.notifyIcon1.DoubleClick += new System.EventHandler(this.notifyIcon1_DoubleClick);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.checkBoxAutoSR);
            this.groupBox1.Controls.Add(this.checkBoxAutoTKK);
            this.groupBox1.Controls.Add(this.checkBoxAutoview);
            this.groupBox1.Font = new System.Drawing.Font("Meiryo UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.groupBox1.Location = new System.Drawing.Point(422, 30);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(282, 55);
            this.groupBox1.TabIndex = 30;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "ID連携";
            // 
            // textBoxPtIDmain
            // 
            this.textBoxPtIDmain.Font = new System.Drawing.Font("Meiryo UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.textBoxPtIDmain.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.textBoxPtIDmain.Location = new System.Drawing.Point(6, 19);
            this.textBoxPtIDmain.Name = "textBoxPtIDmain";
            this.textBoxPtIDmain.Size = new System.Drawing.Size(101, 27);
            this.textBoxPtIDmain.TabIndex = 31;
            this.toolTip1.SetToolTip(this.textBoxPtIDmain, "患者ID（枝番なし）で検索");
            // 
            // buttonPtIDSearch
            // 
            this.buttonPtIDSearch.Image = global::OQSDrug.Properties.Resources.Text_preview;
            this.buttonPtIDSearch.Location = new System.Drawing.Point(110, 20);
            this.buttonPtIDSearch.Margin = new System.Windows.Forms.Padding(0);
            this.buttonPtIDSearch.Name = "buttonPtIDSearch";
            this.buttonPtIDSearch.Size = new System.Drawing.Size(27, 27);
            this.buttonPtIDSearch.TabIndex = 32;
            this.toolTip1.SetToolTip(this.buttonPtIDSearch, "薬歴検索");
            this.buttonPtIDSearch.UseVisualStyleBackColor = true;
            this.buttonPtIDSearch.Click += new System.EventHandler(this.buttonPtIDSearch_Click);
            // 
            // buttonTKKSearch
            // 
            this.buttonTKKSearch.Image = global::OQSDrug.Properties.Resources.Heart;
            this.buttonTKKSearch.Location = new System.Drawing.Point(137, 20);
            this.buttonTKKSearch.Margin = new System.Windows.Forms.Padding(0);
            this.buttonTKKSearch.Name = "buttonTKKSearch";
            this.buttonTKKSearch.Size = new System.Drawing.Size(27, 27);
            this.buttonTKKSearch.TabIndex = 33;
            this.toolTip1.SetToolTip(this.buttonTKKSearch, "健診検索");
            this.buttonTKKSearch.UseVisualStyleBackColor = true;
            this.buttonTKKSearch.Click += new System.EventHandler(this.buttonTKKSearch_Click);
            // 
            // buttonSRSearch
            // 
            this.buttonSRSearch.Image = global::OQSDrug.Properties.Resources.Equipment;
            this.buttonSRSearch.Location = new System.Drawing.Point(164, 20);
            this.buttonSRSearch.Margin = new System.Windows.Forms.Padding(0);
            this.buttonSRSearch.Name = "buttonSRSearch";
            this.buttonSRSearch.Size = new System.Drawing.Size(27, 27);
            this.buttonSRSearch.TabIndex = 34;
            this.toolTip1.SetToolTip(this.buttonSRSearch, "診療検索");
            this.buttonSRSearch.UseVisualStyleBackColor = true;
            this.buttonSRSearch.Click += new System.EventHandler(this.buttonSRSearch_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.textBoxPtIDmain);
            this.groupBox2.Controls.Add(this.buttonSRSearch);
            this.groupBox2.Controls.Add(this.buttonPtIDSearch);
            this.groupBox2.Controls.Add(this.buttonTKKSearch);
            this.groupBox2.Font = new System.Drawing.Font("Meiryo UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.groupBox2.Location = new System.Drawing.Point(710, 30);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(200, 55);
            this.groupBox2.TabIndex = 35;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "ID検索";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.ClientSize = new System.Drawing.Size(974, 717);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.checkBoxAutoStart);
            this.Controls.Add(this.buttonReload);
            this.Controls.Add(this.buttonKSXML);
            this.Controls.Add(this.buttonKSPDF);
            this.Controls.Add(this.buttonKS);
            this.Controls.Add(this.buttonYZXML);
            this.Controls.Add(this.buttonYZPDF);
            this.Controls.Add(this.buttonSR);
            this.Controls.Add(this.buttonYZ);
            this.Controls.Add(this.tableLayoutPanel);
            this.Controls.Add(this.listViewLog);
            this.Controls.Add(this.StartStop);
            this.Controls.Add(this.toolStripVersion);
            this.Controls.Add(this.dataGridView1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "OQSDrugメイン";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.toolStripVersion.ResumeLayout(false);
            this.toolStripVersion.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.ToolStrip toolStripVersion;
        private System.Windows.Forms.ToolStripButton toolStripButtonSettings;
        private System.Windows.Forms.CheckBox StartStop;
        private System.Windows.Forms.ListView listViewLog;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripComboBox toolStripComboBoxDBProviders;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button buttonYZ;
        private System.Windows.Forms.Button buttonSR;
        private System.Windows.Forms.Button buttonYZPDF;
        private System.Windows.Forms.Button buttonYZXML;
        private System.Windows.Forms.Button buttonKS;
        private System.Windows.Forms.Button buttonKSPDF;
        private System.Windows.Forms.Button buttonKSXML;
        private System.Windows.Forms.CheckBox checkBoxAutoview;
        private System.Windows.Forms.ToolStripButton toolStripButtonViewer;
        private System.Windows.Forms.ToolStripButton toolStripButtonExit;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton toolStripButtonVersion;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.ToolStripButton toolStripButtonToTaskTray;
        private System.Windows.Forms.Button buttonReload;
        private System.Windows.Forms.CheckBox checkBoxAutoStart;
        private System.Windows.Forms.ToolStripButton toolStripButtonTKK;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox checkBoxAutoTKK;
        private System.Windows.Forms.ToolStripButton toolStripButtonDebug;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparatorDebug1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparatorDebug2;
        private System.Windows.Forms.ToolStripTextBox toolStripTextBoxDebug;
        private System.Windows.Forms.ToolStripButton toolStripButtonSinryo;
        private System.Windows.Forms.CheckBox checkBoxAutoSR;
        private System.Windows.Forms.TextBox textBoxPtIDmain;
        private System.Windows.Forms.Button buttonPtIDSearch;
        private System.Windows.Forms.Button buttonTKKSearch;
        private System.Windows.Forms.Button buttonSRSearch;
        private System.Windows.Forms.GroupBox groupBox2;
    }
}


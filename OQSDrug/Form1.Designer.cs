namespace OQSDrug
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
            this.toolStripComboBoxDBProviders = new System.Windows.Forms.ToolStripComboBox();
            this.listViewLog = new System.Windows.Forms.ListView();
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.checkBoxAutoview = new System.Windows.Forms.CheckBox();
            this.StartStop = new System.Windows.Forms.CheckBox();
            this.buttonYZ = new System.Windows.Forms.Button();
            this.buttonSR = new System.Windows.Forms.Button();
            this.buttonYZPDF = new System.Windows.Forms.Button();
            this.buttonYZXML = new System.Windows.Forms.Button();
            this.buttonKS = new System.Windows.Forms.Button();
            this.buttonKSPDF = new System.Windows.Forms.Button();
            this.buttonKSXML = new System.Windows.Forms.Button();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.toolStripVersion.SuspendLayout();
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
            this.dataGridView1.CellMouseUp += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridView1_CellMouseUp);
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
            this.toolStripComboBoxDBProviders});
            this.toolStripVersion.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.toolStripVersion.Location = new System.Drawing.Point(0, 0);
            this.toolStripVersion.Name = "toolStripVersion";
            this.toolStripVersion.ShowItemToolTips = false;
            this.toolStripVersion.Size = new System.Drawing.Size(974, 27);
            this.toolStripVersion.Stretch = true;
            this.toolStripVersion.TabIndex = 3;
            this.toolStripVersion.Text = "toolStrip1";
            // 
            // toolStripButtonViewer
            // 
            this.toolStripButtonViewer.Font = new System.Drawing.Font("游ゴシック", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.toolStripButtonViewer.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonViewer.Image")));
            this.toolStripButtonViewer.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonViewer.Name = "toolStripButtonViewer";
            this.toolStripButtonViewer.Size = new System.Drawing.Size(89, 24);
            this.toolStripButtonViewer.Text = "薬歴表示";
            this.toolStripButtonViewer.ToolTipText = "xml薬歴を表示します";
            this.toolStripButtonViewer.Click += new System.EventHandler(this.buttonViewer_Click);
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
            this.toolStripButtonToTaskTray.Size = new System.Drawing.Size(179, 24);
            this.toolStripButtonToTaskTray.Text = "タスクトレイに最小化";
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
            // toolStripComboBoxDBProviders
            // 
            this.toolStripComboBoxDBProviders.Name = "toolStripComboBoxDBProviders";
            this.toolStripComboBoxDBProviders.Size = new System.Drawing.Size(180, 27);
            this.toolStripComboBoxDBProviders.Visible = false;
            this.toolStripComboBoxDBProviders.SelectedIndexChanged += new System.EventHandler(this.toolStripComboBoxDBProviders_SelectedIndexChanged);
            // 
            // listViewLog
            // 
            this.listViewLog.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewLog.FullRowSelect = true;
            this.listViewLog.HideSelection = false;
            this.listViewLog.Location = new System.Drawing.Point(422, 87);
            this.listViewLog.Name = "listViewLog";
            this.listViewLog.Size = new System.Drawing.Size(540, 180);
            this.listViewLog.TabIndex = 8;
            this.listViewLog.UseCompatibleStateImageBehavior = false;
            this.listViewLog.View = System.Windows.Forms.View.Details;
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.AutoSize = true;
            this.tableLayoutPanel.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Inset;
            this.tableLayoutPanel.ColumnCount = 2;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 70F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
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
            // checkBoxAutoview
            // 
            this.checkBoxAutoview.AutoSize = true;
            this.checkBoxAutoview.Font = new System.Drawing.Font("Meiryo UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.checkBoxAutoview.Location = new System.Drawing.Point(422, 42);
            this.checkBoxAutoview.Name = "checkBoxAutoview";
            this.checkBoxAutoview.Size = new System.Drawing.Size(124, 24);
            this.checkBoxAutoview.TabIndex = 27;
            this.checkBoxAutoview.Text = "薬歴自動起動";
            this.toolTip1.SetToolTip(this.checkBoxAutoview, "RSBaseと連動して薬歴が存在すれば自動で表示します");
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
            this.StartStop.Size = new System.Drawing.Size(399, 45);
            this.StartStop.TabIndex = 6;
            this.StartStop.Text = "開始";
            this.StartStop.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.StartStop.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.toolTip1.SetToolTip(this.StartStop, "情報取得処理を開始/終了します");
            this.StartStop.UseVisualStyleBackColor = true;
            this.StartStop.CheckedChanged += new System.EventHandler(this.StartStop_CheckedChanged);
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
            this.notifyIcon1.Text = "右クリックでメニュー、\r\nダブルクリックで表示非表示切替";
            this.notifyIcon1.Visible = true;
            this.notifyIcon1.DoubleClick += new System.EventHandler(this.notifyIcon1_DoubleClick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.ClientSize = new System.Drawing.Size(974, 717);
            this.Controls.Add(this.checkBoxAutoview);
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
            this.Text = "薬歴取得メイン";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.toolStripVersion.ResumeLayout(false);
            this.toolStripVersion.PerformLayout();
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
    }
}


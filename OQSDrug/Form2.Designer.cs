namespace OQSDrug
{
    partial class Form2
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form2));
            this.textBoxDatadyna = new System.Windows.Forms.TextBox();
            this.textBoxOQSFolder = new System.Windows.Forms.TextBox();
            this.textBoxRSBgazou = new System.Windows.Forms.TextBox();
            this.groupBoxDrug = new System.Windows.Forms.GroupBox();
            this.checkBoxKeepXml = new System.Windows.Forms.CheckBox();
            this.checkBoxSinryou = new System.Windows.Forms.CheckBox();
            this.label12 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.comboBoxYZspan = new System.Windows.Forms.ComboBox();
            this.comboBoxYZinterval = new System.Windows.Forms.ComboBox();
            this.radioButtonD0 = new System.Windows.Forms.RadioButton();
            this.label6 = new System.Windows.Forms.Label();
            this.textBoxDrugName = new System.Windows.Forms.TextBox();
            this.radioButtonD3 = new System.Windows.Forms.RadioButton();
            this.radioButtonD2 = new System.Windows.Forms.RadioButton();
            this.radioButtonD1 = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.comboBoxTimerSecond = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textBoxMCode = new System.Windows.Forms.TextBox();
            this.buttonDatadyna = new System.Windows.Forms.Button();
            this.buttonOQSFolder = new System.Windows.Forms.Button();
            this.buttonRSBgazou = new System.Windows.Forms.Button();
            this.buttonSaveExit = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOQSDrugData = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.textBoxOQSDrugData = new System.Windows.Forms.TextBox();
            this.textBoxRSBaseURL = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.checkBoxRSBReload = new System.Windows.Forms.CheckBox();
            this.radioButtonK1 = new System.Windows.Forms.RadioButton();
            this.textBoxKensinName = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.radioButtonK0 = new System.Windows.Forms.RadioButton();
            this.groupBoxKensin = new System.Windows.Forms.GroupBox();
            this.radioButtonK3 = new System.Windows.Forms.RadioButton();
            this.radioButtonK2 = new System.Windows.Forms.RadioButton();
            this.label13 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.textBoxTemprs = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.checkBoxTopmost = new System.Windows.Forms.CheckBox();
            this.buttonViewerPositionReset = new System.Windows.Forms.Button();
            this.buttonImport = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.checkBoxMinimumStart = new System.Windows.Forms.CheckBox();
            this.toolTipSetting = new System.Windows.Forms.ToolTip(this.components);
            this.checkBoxAutoStart = new System.Windows.Forms.CheckBox();
            this.comboBoxRSBID = new System.Windows.Forms.ComboBox();
            this.label17 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.comboBoxViewSpan = new System.Windows.Forms.ComboBox();
            this.label19 = new System.Windows.Forms.Label();
            this.checkBoxOmitMyOrg = new System.Windows.Forms.CheckBox();
            this.radioButtonK4 = new System.Windows.Forms.RadioButton();
            this.label11 = new System.Windows.Forms.Label();
            this.textBoxRSBServerFolder = new System.Windows.Forms.TextBox();
            this.buttonRSBServerFolder = new System.Windows.Forms.Button();
            this.checkBoxRSBreloadXml = new System.Windows.Forms.CheckBox();
            this.textBoxRSBxmlURL = new System.Windows.Forms.TextBox();
            this.groupBoxDrug.SuspendLayout();
            this.groupBoxKensin.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBoxDatadyna
            // 
            this.textBoxDatadyna.Location = new System.Drawing.Point(186, 57);
            this.textBoxDatadyna.Name = "textBoxDatadyna";
            this.textBoxDatadyna.Size = new System.Drawing.Size(365, 19);
            this.textBoxDatadyna.TabIndex = 2;
            this.toolTipSetting.SetToolTip(this.textBoxDatadyna, resources.GetString("textBoxDatadyna.ToolTip"));
            // 
            // textBoxOQSFolder
            // 
            this.textBoxOQSFolder.Location = new System.Drawing.Point(186, 130);
            this.textBoxOQSFolder.Name = "textBoxOQSFolder";
            this.textBoxOQSFolder.Size = new System.Drawing.Size(365, 19);
            this.textBoxOQSFolder.TabIndex = 4;
            this.toolTipSetting.SetToolTip(this.textBoxOQSFolder, "オン資PCのreq/res/face等のフォルダが有る場所を指定します");
            // 
            // textBoxRSBgazou
            // 
            this.textBoxRSBgazou.Location = new System.Drawing.Point(186, 158);
            this.textBoxRSBgazou.Name = "textBoxRSBgazou";
            this.textBoxRSBgazou.Size = new System.Drawing.Size(365, 19);
            this.textBoxRSBgazou.TabIndex = 6;
            this.toolTipSetting.SetToolTip(this.textBoxRSBgazou, "RSBaseのgazouフォルダ、RSAutoの自動取り込みフォルダを指定します");
            // 
            // groupBoxDrug
            // 
            this.groupBoxDrug.Controls.Add(this.checkBoxSinryou);
            this.groupBoxDrug.Controls.Add(this.label12);
            this.groupBoxDrug.Controls.Add(this.label10);
            this.groupBoxDrug.Controls.Add(this.comboBoxYZspan);
            this.groupBoxDrug.Controls.Add(this.comboBoxYZinterval);
            this.groupBoxDrug.Controls.Add(this.radioButtonD0);
            this.groupBoxDrug.Controls.Add(this.label6);
            this.groupBoxDrug.Controls.Add(this.textBoxDrugName);
            this.groupBoxDrug.Controls.Add(this.radioButtonD3);
            this.groupBoxDrug.Controls.Add(this.radioButtonD2);
            this.groupBoxDrug.Controls.Add(this.radioButtonD1);
            this.groupBoxDrug.Location = new System.Drawing.Point(35, 317);
            this.groupBoxDrug.Name = "groupBoxDrug";
            this.groupBoxDrug.Size = new System.Drawing.Size(259, 153);
            this.groupBoxDrug.TabIndex = 4;
            this.groupBoxDrug.TabStop = false;
            this.groupBoxDrug.Text = "⑧薬剤情報(xmlは常時取得)";
            // 
            // checkBoxKeepXml
            // 
            this.checkBoxKeepXml.AutoSize = true;
            this.checkBoxKeepXml.Location = new System.Drawing.Point(44, 476);
            this.checkBoxKeepXml.Name = "checkBoxKeepXml";
            this.checkBoxKeepXml.Size = new System.Drawing.Size(104, 16);
            this.checkBoxKeepXml.TabIndex = 29;
            this.checkBoxKeepXml.Text = "xmlを削除しない";
            this.toolTipSetting.SetToolTip(this.checkBoxKeepXml, "ON: RESフォルダにxml薬歴を残します。\r\nRSBaseでxml薬歴や健診歴を取得するときはこれをONにしてください。\r\nresフォルダに残ったファイルは適" +
        "宜手動で削除してください。");
            this.checkBoxKeepXml.UseVisualStyleBackColor = true;
            this.checkBoxKeepXml.CheckedChanged += new System.EventHandler(this.checkBoxKeepXml_CheckedChanged);
            // 
            // checkBoxSinryou
            // 
            this.checkBoxSinryou.AutoSize = true;
            this.checkBoxSinryou.Location = new System.Drawing.Point(9, 19);
            this.checkBoxSinryou.Name = "checkBoxSinryou";
            this.checkBoxSinryou.Size = new System.Drawing.Size(105, 16);
            this.checkBoxSinryou.TabIndex = 15;
            this.checkBoxSinryou.Text = "診療情報も取得";
            this.checkBoxSinryou.UseVisualStyleBackColor = true;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(199, 110);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(49, 12);
            this.label12.TabIndex = 9;
            this.label12.Text = "期間(月)";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(199, 65);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(49, 12);
            this.label10.TabIndex = 8;
            this.label10.Text = "間隔(月)";
            // 
            // comboBoxYZspan
            // 
            this.comboBoxYZspan.FormattingEnabled = true;
            this.comboBoxYZspan.Location = new System.Drawing.Point(201, 125);
            this.comboBoxYZspan.Name = "comboBoxYZspan";
            this.comboBoxYZspan.Size = new System.Drawing.Size(40, 20);
            this.comboBoxYZspan.TabIndex = 26;
            // 
            // comboBoxYZinterval
            // 
            this.comboBoxYZinterval.FormattingEnabled = true;
            this.comboBoxYZinterval.Location = new System.Drawing.Point(201, 83);
            this.comboBoxYZinterval.Name = "comboBoxYZinterval";
            this.comboBoxYZinterval.Size = new System.Drawing.Size(40, 20);
            this.comboBoxYZinterval.TabIndex = 24;
            // 
            // radioButtonD0
            // 
            this.radioButtonD0.AutoSize = true;
            this.radioButtonD0.Location = new System.Drawing.Point(9, 41);
            this.radioButtonD0.Name = "radioButtonD0";
            this.radioButtonD0.Size = new System.Drawing.Size(62, 16);
            this.radioButtonD0.TabIndex = 16;
            this.radioButtonD0.TabStop = true;
            this.radioButtonD0.Text = "xmlのみ";
            this.radioButtonD0.UseVisualStyleBackColor = true;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(25, 86);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(64, 12);
            this.label6.TabIndex = 4;
            this.label6.Text = "RSB検査名";
            // 
            // textBoxDrugName
            // 
            this.textBoxDrugName.Location = new System.Drawing.Point(92, 83);
            this.textBoxDrugName.Name = "textBoxDrugName";
            this.textBoxDrugName.Size = new System.Drawing.Size(100, 19);
            this.textBoxDrugName.TabIndex = 28;
            // 
            // radioButtonD3
            // 
            this.radioButtonD3.AutoSize = true;
            this.radioButtonD3.Location = new System.Drawing.Point(9, 130);
            this.radioButtonD3.Name = "radioButtonD3";
            this.radioButtonD3.Size = new System.Drawing.Size(14, 13);
            this.radioButtonD3.TabIndex = 22;
            this.radioButtonD3.TabStop = true;
            this.radioButtonD3.UseVisualStyleBackColor = true;
            this.radioButtonD3.Visible = false;
            // 
            // radioButtonD2
            // 
            this.radioButtonD2.AutoSize = true;
            this.radioButtonD2.Location = new System.Drawing.Point(9, 108);
            this.radioButtonD2.Name = "radioButtonD2";
            this.radioButtonD2.Size = new System.Drawing.Size(148, 16);
            this.radioButtonD2.TabIndex = 20;
            this.radioButtonD2.TabStop = true;
            this.radioButtonD2.Text = "PDFをRSBase SideShow";
            this.radioButtonD2.UseVisualStyleBackColor = true;
            // 
            // radioButtonD1
            // 
            this.radioButtonD1.AutoSize = true;
            this.radioButtonD1.Location = new System.Drawing.Point(9, 63);
            this.radioButtonD1.Name = "radioButtonD1";
            this.radioButtonD1.Size = new System.Drawing.Size(143, 16);
            this.radioButtonD1.TabIndex = 18;
            this.radioButtonD1.TabStop = true;
            this.radioButtonD1.Text = "PDFをRSBase検査登録";
            this.radioButtonD1.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(33, 60);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(103, 12);
            this.label1.TabIndex = 5;
            this.label1.Text = "②ダイナミクスの場所";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(33, 133);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(109, 12);
            this.label2.TabIndex = 6;
            this.label2.Text = "③OQSフォルダの場所";
            this.toolTipSetting.SetToolTip(this.label2, "オン資PCのreq/res/face等のフォルダが有る場所を指定します");
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(33, 161);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(154, 12);
            this.label3.TabIndex = 7;
            this.label3.Text = "④RSBase取込フォルダ(gazou)";
            this.toolTipSetting.SetToolTip(this.label3, "RSBaseのgazouフォルダ、RSAutoの自動取り込みフォルダを指定します");
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(33, 287);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(97, 12);
            this.label4.TabIndex = 8;
            this.label4.Text = "⑦タイマー間隔(秒)";
            // 
            // comboBoxTimerSecond
            // 
            this.comboBoxTimerSecond.FormattingEnabled = true;
            this.comboBoxTimerSecond.Location = new System.Drawing.Point(174, 284);
            this.comboBoxTimerSecond.Name = "comboBoxTimerSecond";
            this.comboBoxTimerSecond.Size = new System.Drawing.Size(53, 20);
            this.comboBoxTimerSecond.TabIndex = 14;
            this.toolTipSetting.SetToolTip(this.comboBoxTimerSecond, "情報閲覧を行う間隔を秒数で指定します。");
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(33, 259);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(92, 12);
            this.label5.TabIndex = 10;
            this.label5.Text = "⑥医療機関コード";
            // 
            // textBoxMCode
            // 
            this.textBoxMCode.Location = new System.Drawing.Point(174, 256);
            this.textBoxMCode.Name = "textBoxMCode";
            this.textBoxMCode.Size = new System.Drawing.Size(100, 19);
            this.textBoxMCode.TabIndex = 12;
            this.toolTipSetting.SetToolTip(this.textBoxMCode, "10桁のものです");
            // 
            // buttonDatadyna
            // 
            this.buttonDatadyna.Location = new System.Drawing.Point(557, 57);
            this.buttonDatadyna.Name = "buttonDatadyna";
            this.buttonDatadyna.Size = new System.Drawing.Size(21, 19);
            this.buttonDatadyna.TabIndex = 3;
            this.buttonDatadyna.Text = "...";
            this.buttonDatadyna.UseVisualStyleBackColor = true;
            this.buttonDatadyna.Click += new System.EventHandler(this.buttonDatadyna_Click);
            // 
            // buttonOQSFolder
            // 
            this.buttonOQSFolder.Location = new System.Drawing.Point(557, 130);
            this.buttonOQSFolder.Name = "buttonOQSFolder";
            this.buttonOQSFolder.Size = new System.Drawing.Size(21, 19);
            this.buttonOQSFolder.TabIndex = 5;
            this.buttonOQSFolder.Text = "...";
            this.buttonOQSFolder.UseVisualStyleBackColor = true;
            this.buttonOQSFolder.Click += new System.EventHandler(this.buttonOQSFolder_Click);
            // 
            // buttonRSBgazou
            // 
            this.buttonRSBgazou.Location = new System.Drawing.Point(557, 158);
            this.buttonRSBgazou.Name = "buttonRSBgazou";
            this.buttonRSBgazou.Size = new System.Drawing.Size(21, 19);
            this.buttonRSBgazou.TabIndex = 7;
            this.buttonRSBgazou.Text = "...";
            this.buttonRSBgazou.UseVisualStyleBackColor = true;
            this.buttonRSBgazou.Click += new System.EventHandler(this.buttonRSBgazou_Click);
            // 
            // buttonSaveExit
            // 
            this.buttonSaveExit.Location = new System.Drawing.Point(491, 643);
            this.buttonSaveExit.Name = "buttonSaveExit";
            this.buttonSaveExit.Size = new System.Drawing.Size(93, 23);
            this.buttonSaveExit.TabIndex = 50;
            this.buttonSaveExit.Text = "保存して閉じる";
            this.buttonSaveExit.UseVisualStyleBackColor = true;
            this.buttonSaveExit.Click += new System.EventHandler(this.buttonSaveExit_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(410, 643);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 44;
            this.buttonCancel.Text = "キャンセル";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonOQSDrugData
            // 
            this.buttonOQSDrugData.Location = new System.Drawing.Point(557, 29);
            this.buttonOQSDrugData.Name = "buttonOQSDrugData";
            this.buttonOQSDrugData.Size = new System.Drawing.Size(21, 19);
            this.buttonOQSDrugData.TabIndex = 1;
            this.buttonOQSDrugData.Text = "...";
            this.buttonOQSDrugData.UseVisualStyleBackColor = true;
            this.buttonOQSDrugData.Click += new System.EventHandler(this.buttonOQSDrugData_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(33, 32);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(147, 12);
            this.label8.TabIndex = 21;
            this.label8.Text = "①OQSDrug_data.mdbの場所";
            this.toolTipSetting.SetToolTip(this.label8, "通常はダイナミクスのdatadyna.mdbのあるフォルダに配置・設定してください");
            // 
            // textBoxOQSDrugData
            // 
            this.textBoxOQSDrugData.Location = new System.Drawing.Point(186, 29);
            this.textBoxOQSDrugData.Name = "textBoxOQSDrugData";
            this.textBoxOQSDrugData.Size = new System.Drawing.Size(365, 19);
            this.textBoxOQSDrugData.TabIndex = 0;
            this.toolTipSetting.SetToolTip(this.textBoxOQSDrugData, "通常はダイナミクスのdatadyna.mdbのあるフォルダに配置・設定してください");
            // 
            // textBoxRSBaseURL
            // 
            this.textBoxRSBaseURL.Location = new System.Drawing.Point(236, 181);
            this.textBoxRSBaseURL.Name = "textBoxRSBaseURL";
            this.textBoxRSBaseURL.Size = new System.Drawing.Size(315, 19);
            this.textBoxRSBaseURL.TabIndex = 10;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(203, 184);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(27, 12);
            this.label9.TabIndex = 24;
            this.label9.Text = "URL";
            // 
            // checkBoxRSBReload
            // 
            this.checkBoxRSBReload.AutoSize = true;
            this.checkBoxRSBReload.Location = new System.Drawing.Point(45, 183);
            this.checkBoxRSBReload.Name = "checkBoxRSBReload";
            this.checkBoxRSBReload.Size = new System.Drawing.Size(142, 16);
            this.checkBoxRSBReload.TabIndex = 8;
            this.checkBoxRSBReload.Text = "RSBase reloadして取込";
            this.toolTipSetting.SetToolTip(this.checkBoxRSBReload, "RSAutoを使用しないときは、Top画面のURLをしていするとリロード取り込みします。\r\nRSBaseが本PCにインストールされている必要があります。\r\n");
            this.checkBoxRSBReload.UseVisualStyleBackColor = true;
            // 
            // radioButtonK1
            // 
            this.radioButtonK1.AutoSize = true;
            this.radioButtonK1.Location = new System.Drawing.Point(117, 18);
            this.radioButtonK1.Name = "radioButtonK1";
            this.radioButtonK1.Size = new System.Drawing.Size(104, 16);
            this.radioButtonK1.TabIndex = 32;
            this.radioButtonK1.TabStop = true;
            this.radioButtonK1.Text = "PDF to RSBase";
            this.radioButtonK1.UseVisualStyleBackColor = true;
            this.radioButtonK1.Visible = false;
            // 
            // textBoxKensinName
            // 
            this.textBoxKensinName.Location = new System.Drawing.Point(96, 76);
            this.textBoxKensinName.Name = "textBoxKensinName";
            this.textBoxKensinName.Size = new System.Drawing.Size(100, 19);
            this.textBoxKensinName.TabIndex = 40;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(26, 79);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(64, 12);
            this.label7.TabIndex = 6;
            this.label7.Text = "RSB検査名";
            // 
            // radioButtonK0
            // 
            this.radioButtonK0.AutoSize = true;
            this.radioButtonK0.Location = new System.Drawing.Point(6, 16);
            this.radioButtonK0.Name = "radioButtonK0";
            this.radioButtonK0.Size = new System.Drawing.Size(62, 16);
            this.radioButtonK0.TabIndex = 30;
            this.radioButtonK0.TabStop = true;
            this.radioButtonK0.Text = "xmlのみ";
            this.radioButtonK0.UseVisualStyleBackColor = true;
            // 
            // groupBoxKensin
            // 
            this.groupBoxKensin.Controls.Add(this.radioButtonK4);
            this.groupBoxKensin.Controls.Add(this.radioButtonK0);
            this.groupBoxKensin.Controls.Add(this.label7);
            this.groupBoxKensin.Controls.Add(this.textBoxKensinName);
            this.groupBoxKensin.Controls.Add(this.radioButtonK3);
            this.groupBoxKensin.Controls.Add(this.radioButtonK2);
            this.groupBoxKensin.Controls.Add(this.radioButtonK1);
            this.groupBoxKensin.Location = new System.Drawing.Point(330, 317);
            this.groupBoxKensin.Name = "groupBoxKensin";
            this.groupBoxKensin.Size = new System.Drawing.Size(248, 153);
            this.groupBoxKensin.TabIndex = 5;
            this.groupBoxKensin.TabStop = false;
            this.groupBoxKensin.Text = "⑨特定健診(xmlは常時取得)";
            // 
            // radioButtonK3
            // 
            this.radioButtonK3.AutoSize = true;
            this.radioButtonK3.Location = new System.Drawing.Point(6, 60);
            this.radioButtonK3.Name = "radioButtonK3";
            this.radioButtonK3.Size = new System.Drawing.Size(228, 16);
            this.radioButtonK3.TabIndex = 36;
            this.radioButtonK3.TabStop = true;
            this.radioButtonK3.Text = "PDF自動(健診日を検査日として検査登録";
            this.toolTipSetting.SetToolTip(this.radioButtonK3, "新しい健診結果があれば自動で取込み。\r\n健診実施日をRSBaseの検査日に検査として登録します\r\n");
            this.radioButtonK3.UseVisualStyleBackColor = true;
            // 
            // radioButtonK2
            // 
            this.radioButtonK2.AutoSize = true;
            this.radioButtonK2.Location = new System.Drawing.Point(6, 38);
            this.radioButtonK2.Name = "radioButtonK2";
            this.radioButtonK2.Size = new System.Drawing.Size(232, 16);
            this.radioButtonK2.TabIndex = 34;
            this.radioButtonK2.TabStop = true;
            this.radioButtonK2.Text = "PDF自動(取込日を検査日として検査登録)";
            this.toolTipSetting.SetToolTip(this.radioButtonK2, "新しい健診結果があれば自動でPDFを取込み。\r\n取込日をRSBaseの検査日に検査として登録します");
            this.radioButtonK2.UseVisualStyleBackColor = true;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(12, 9);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(55, 12);
            this.label13.TabIndex = 51;
            this.label13.Text = "メイン設定";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(20, 528);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(64, 12);
            this.label15.TabIndex = 53;
            this.label15.Text = "Viewer設定";
            // 
            // textBoxTemprs
            // 
            this.textBoxTemprs.Location = new System.Drawing.Point(410, 543);
            this.textBoxTemprs.Name = "textBoxTemprs";
            this.textBoxTemprs.Size = new System.Drawing.Size(151, 19);
            this.textBoxTemprs.TabIndex = 54;
            this.textBoxTemprs.Visible = false;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(43, 546);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(148, 12);
            this.label14.TabIndex = 55;
            this.label14.Text = "⑪ ダイナ/RSBase 連携方式";
            this.toolTipSetting.SetToolTip(this.label14, "RSBaseのID連携を設定すると\r\nカルテ遷移に連動して自動で薬歴ビュワーを起動します");
            // 
            // checkBoxTopmost
            // 
            this.checkBoxTopmost.AutoSize = true;
            this.checkBoxTopmost.Location = new System.Drawing.Point(44, 568);
            this.checkBoxTopmost.Name = "checkBoxTopmost";
            this.checkBoxTopmost.Size = new System.Drawing.Size(163, 16);
            this.checkBoxTopmost.TabIndex = 56;
            this.checkBoxTopmost.Text = "⑫ 薬歴・健診を最前面表示";
            this.checkBoxTopmost.UseVisualStyleBackColor = true;
            // 
            // buttonViewerPositionReset
            // 
            this.buttonViewerPositionReset.Location = new System.Drawing.Point(213, 564);
            this.buttonViewerPositionReset.Name = "buttonViewerPositionReset";
            this.buttonViewerPositionReset.Size = new System.Drawing.Size(218, 23);
            this.buttonViewerPositionReset.TabIndex = 57;
            this.buttonViewerPositionReset.Text = "薬歴・健診Windowの位置サイズをリセット";
            this.buttonViewerPositionReset.UseVisualStyleBackColor = true;
            this.buttonViewerPositionReset.Click += new System.EventHandler(this.buttonViewerPositionReset_Click);
            // 
            // buttonImport
            // 
            this.buttonImport.Location = new System.Drawing.Point(248, 643);
            this.buttonImport.Name = "buttonImport";
            this.buttonImport.Size = new System.Drawing.Size(75, 23);
            this.buttonImport.TabIndex = 58;
            this.buttonImport.Text = "インポート";
            this.buttonImport.UseVisualStyleBackColor = true;
            this.buttonImport.Click += new System.EventHandler(this.buttonImport_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(329, 643);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 59;
            this.button2.Text = "エクスポート";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.buttonExport_Click);
            // 
            // checkBoxMinimumStart
            // 
            this.checkBoxMinimumStart.AutoSize = true;
            this.checkBoxMinimumStart.Location = new System.Drawing.Point(44, 498);
            this.checkBoxMinimumStart.Name = "checkBoxMinimumStart";
            this.checkBoxMinimumStart.Size = new System.Drawing.Size(108, 16);
            this.checkBoxMinimumStart.TabIndex = 60;
            this.checkBoxMinimumStart.Text = "⑩最小化して開く";
            this.toolTipSetting.SetToolTip(this.checkBoxMinimumStart, "タスクトレイに最小化して起動します");
            this.checkBoxMinimumStart.UseVisualStyleBackColor = true;
            // 
            // checkBoxAutoStart
            // 
            this.checkBoxAutoStart.AutoSize = true;
            this.checkBoxAutoStart.Location = new System.Drawing.Point(248, 286);
            this.checkBoxAutoStart.Name = "checkBoxAutoStart";
            this.checkBoxAutoStart.Size = new System.Drawing.Size(102, 16);
            this.checkBoxAutoStart.TabIndex = 70;
            this.checkBoxAutoStart.Text = "自動開始/停止";
            this.toolTipSetting.SetToolTip(this.checkBoxAutoStart, "起動時に設定値がすべてOKなら、自動で取込動作を開始します");
            this.checkBoxAutoStart.UseVisualStyleBackColor = true;
            // 
            // comboBoxRSBID
            // 
            this.comboBoxRSBID.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxRSBID.FormattingEnabled = true;
            this.comboBoxRSBID.Items.AddRange(new object[] {
            "ID.dat",
            "temp_rs.txt",
            "thept.txt",
            "ダイナC:\\DynaID",
            "ダイナD:\\DynaID"});
            this.comboBoxRSBID.Location = new System.Drawing.Point(213, 543);
            this.comboBoxRSBID.Name = "comboBoxRSBID";
            this.comboBoxRSBID.Size = new System.Drawing.Size(124, 20);
            this.comboBoxRSBID.TabIndex = 61;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Font = new System.Drawing.Font("MS UI Gothic", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label17.Location = new System.Drawing.Point(174, 79);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(364, 33);
            this.label17.TabIndex = 64;
            this.label17.Text = "本アプリを動かすPCでダイナミクスクライアントが稼働中の場合はクライアントダイナを、\r\nクライアントが稼働してない場合は、サーバーのdatadyna.mdbを指定" +
    "してください。\r\nクライアントを指定した場合は、クライアントアップデート後再指定が必要です";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(44, 594);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(109, 12);
            this.label18.TabIndex = 65;
            this.label18.Text = "⑬初期表示期間(月)";
            // 
            // comboBoxViewSpan
            // 
            this.comboBoxViewSpan.FormattingEnabled = true;
            this.comboBoxViewSpan.Location = new System.Drawing.Point(213, 591);
            this.comboBoxViewSpan.Name = "comboBoxViewSpan";
            this.comboBoxViewSpan.Size = new System.Drawing.Size(65, 20);
            this.comboBoxViewSpan.TabIndex = 66;
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(284, 594);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(65, 12);
            this.label19.TabIndex = 67;
            this.label19.Text = "(全期間は0)";
            // 
            // checkBoxOmitMyOrg
            // 
            this.checkBoxOmitMyOrg.AutoSize = true;
            this.checkBoxOmitMyOrg.Location = new System.Drawing.Point(44, 618);
            this.checkBoxOmitMyOrg.Name = "checkBoxOmitMyOrg";
            this.checkBoxOmitMyOrg.Size = new System.Drawing.Size(100, 16);
            this.checkBoxOmitMyOrg.TabIndex = 69;
            this.checkBoxOmitMyOrg.Text = "⑭ 自施設除外";
            this.checkBoxOmitMyOrg.UseVisualStyleBackColor = true;
            // 
            // radioButtonK4
            // 
            this.radioButtonK4.AutoSize = true;
            this.radioButtonK4.Location = new System.Drawing.Point(6, 101);
            this.radioButtonK4.Name = "radioButtonK4";
            this.radioButtonK4.Size = new System.Drawing.Size(150, 16);
            this.radioButtonK4.TabIndex = 41;
            this.radioButtonK4.TabStop = true;
            this.radioButtonK4.Text = "PDF自動(SideShow登録)";
            this.toolTipSetting.SetToolTip(this.radioButtonK4, "新しい健診結果があれば自動でSideShowに取込み\r\n取込日で登録されます");
            this.radioButtonK4.UseVisualStyleBackColor = true;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(33, 213);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(257, 12);
            this.label11.TabIndex = 71;
            this.label11.Text = "⑤ RSBaseのサーバーフォルダ(...Public_html フォルダ)";
            this.toolTipSetting.SetToolTip(this.label11, "SideShowにPDFを保存するときに設定してください");
            // 
            // textBoxRSBServerFolder
            // 
            this.textBoxRSBServerFolder.Location = new System.Drawing.Point(236, 228);
            this.textBoxRSBServerFolder.Name = "textBoxRSBServerFolder";
            this.textBoxRSBServerFolder.Size = new System.Drawing.Size(315, 19);
            this.textBoxRSBServerFolder.TabIndex = 72;
            this.toolTipSetting.SetToolTip(this.textBoxRSBServerFolder, "SideShowに保存するときに、RSBaseサーバーのpublic_htmlフォルダのパスを設定してください\r\n例： \\\\DYNASERVER\\D\\Users\\" +
        "rsn\\public_html");
            // 
            // buttonRSBServerFolder
            // 
            this.buttonRSBServerFolder.Location = new System.Drawing.Point(557, 228);
            this.buttonRSBServerFolder.Name = "buttonRSBServerFolder";
            this.buttonRSBServerFolder.Size = new System.Drawing.Size(21, 19);
            this.buttonRSBServerFolder.TabIndex = 73;
            this.buttonRSBServerFolder.Text = "...";
            this.buttonRSBServerFolder.UseVisualStyleBackColor = true;
            this.buttonRSBServerFolder.Click += new System.EventHandler(this.buttonRSBServerFolder_Click);
            // 
            // checkBoxRSBreloadXml
            // 
            this.checkBoxRSBreloadXml.AutoSize = true;
            this.checkBoxRSBreloadXml.Enabled = false;
            this.checkBoxRSBreloadXml.Location = new System.Drawing.Point(154, 476);
            this.checkBoxRSBreloadXml.Name = "checkBoxRSBreloadXml";
            this.checkBoxRSBreloadXml.Size = new System.Drawing.Size(164, 16);
            this.checkBoxRSBreloadXml.TabIndex = 74;
            this.checkBoxRSBreloadXml.Text = "RSBaseでxml reload   URL:";
            this.toolTipSetting.SetToolTip(this.checkBoxRSBreloadXml, "xml薬歴や健診歴を取得後、RSBaseのxmlreloadを実行します。\r\nこのPCにRSBaseがインストールされている必要があります。");
            this.checkBoxRSBreloadXml.UseVisualStyleBackColor = true;
            // 
            // textBoxRSBxmlURL
            // 
            this.textBoxRSBxmlURL.Enabled = false;
            this.textBoxRSBxmlURL.Location = new System.Drawing.Point(316, 474);
            this.textBoxRSBxmlURL.Name = "textBoxRSBxmlURL";
            this.textBoxRSBxmlURL.Size = new System.Drawing.Size(235, 19);
            this.textBoxRSBxmlURL.TabIndex = 75;
            this.toolTipSetting.SetToolTip(this.textBoxRSBxmlURL, "リロードするURLを指定します");
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(596, 678);
            this.Controls.Add(this.textBoxRSBxmlURL);
            this.Controls.Add(this.checkBoxRSBreloadXml);
            this.Controls.Add(this.buttonRSBServerFolder);
            this.Controls.Add(this.textBoxRSBServerFolder);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.checkBoxKeepXml);
            this.Controls.Add(this.checkBoxAutoStart);
            this.Controls.Add(this.checkBoxOmitMyOrg);
            this.Controls.Add(this.label19);
            this.Controls.Add(this.comboBoxViewSpan);
            this.Controls.Add(this.label18);
            this.Controls.Add(this.label17);
            this.Controls.Add(this.comboBoxRSBID);
            this.Controls.Add(this.checkBoxMinimumStart);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.buttonImport);
            this.Controls.Add(this.buttonViewerPositionReset);
            this.Controls.Add(this.checkBoxTopmost);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.textBoxTemprs);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.checkBoxRSBReload);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.textBoxRSBaseURL);
            this.Controls.Add(this.buttonOQSDrugData);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.textBoxOQSDrugData);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonSaveExit);
            this.Controls.Add(this.buttonRSBgazou);
            this.Controls.Add(this.buttonOQSFolder);
            this.Controls.Add(this.buttonDatadyna);
            this.Controls.Add(this.groupBoxKensin);
            this.Controls.Add(this.textBoxMCode);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.comboBoxTimerSecond);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.groupBoxDrug);
            this.Controls.Add(this.textBoxRSBgazou);
            this.Controls.Add(this.textBoxOQSFolder);
            this.Controls.Add(this.textBoxDatadyna);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form2";
            this.Text = "設定";
            this.Load += new System.EventHandler(this.Form2_Load);
            this.groupBoxDrug.ResumeLayout(false);
            this.groupBoxDrug.PerformLayout();
            this.groupBoxKensin.ResumeLayout(false);
            this.groupBoxKensin.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxDatadyna;
        private System.Windows.Forms.TextBox textBoxOQSFolder;
        private System.Windows.Forms.TextBox textBoxRSBgazou;
        private System.Windows.Forms.GroupBox groupBoxDrug;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox comboBoxTimerSecond;
        private System.Windows.Forms.RadioButton radioButtonD3;
        private System.Windows.Forms.RadioButton radioButtonD2;
        private System.Windows.Forms.RadioButton radioButtonD1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBoxMCode;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBoxDrugName;
        private System.Windows.Forms.Button buttonDatadyna;
        private System.Windows.Forms.Button buttonOQSFolder;
        private System.Windows.Forms.Button buttonRSBgazou;
        private System.Windows.Forms.Button buttonSaveExit;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonOQSDrugData;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox textBoxOQSDrugData;
        private System.Windows.Forms.TextBox textBoxRSBaseURL;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.CheckBox checkBoxRSBReload;
        private System.Windows.Forms.RadioButton radioButtonD0;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.ComboBox comboBoxYZspan;
        private System.Windows.Forms.ComboBox comboBoxYZinterval;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.CheckBox checkBoxSinryou;
        private System.Windows.Forms.RadioButton radioButtonK1;
        private System.Windows.Forms.TextBox textBoxKensinName;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.RadioButton radioButtonK0;
        private System.Windows.Forms.GroupBox groupBoxKensin;
        private System.Windows.Forms.RadioButton radioButtonK2;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TextBox textBoxTemprs;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.CheckBox checkBoxTopmost;
        private System.Windows.Forms.Button buttonViewerPositionReset;
        private System.Windows.Forms.Button buttonImport;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.CheckBox checkBoxMinimumStart;
        private System.Windows.Forms.ToolTip toolTipSetting;
        private System.Windows.Forms.ComboBox comboBoxRSBID;
        private System.Windows.Forms.CheckBox checkBoxKeepXml;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.ComboBox comboBoxViewSpan;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.CheckBox checkBoxOmitMyOrg;
        private System.Windows.Forms.CheckBox checkBoxAutoStart;
        private System.Windows.Forms.RadioButton radioButtonK3;
        private System.Windows.Forms.RadioButton radioButtonK4;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox textBoxRSBServerFolder;
        private System.Windows.Forms.Button buttonRSBServerFolder;
        private System.Windows.Forms.CheckBox checkBoxRSBreloadXml;
        private System.Windows.Forms.TextBox textBoxRSBxmlURL;
    }
}
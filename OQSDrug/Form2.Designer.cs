﻿namespace OQSDrug
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form2));
            this.textBoxDatadyna = new System.Windows.Forms.TextBox();
            this.textBoxOQSFolder = new System.Windows.Forms.TextBox();
            this.textBoxRSBgazou = new System.Windows.Forms.TextBox();
            this.groupBoxDrug = new System.Windows.Forms.GroupBox();
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
            this.radioButtonK3 = new System.Windows.Forms.RadioButton();
            this.textBoxKensinName = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.radioButtonK0 = new System.Windows.Forms.RadioButton();
            this.comboBoxKSinterval = new System.Windows.Forms.ComboBox();
            this.label11 = new System.Windows.Forms.Label();
            this.groupBoxKensin = new System.Windows.Forms.GroupBox();
            this.radioButtonK2 = new System.Windows.Forms.RadioButton();
            this.label13 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.textBoxTemprs = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.checkBoxTopmost = new System.Windows.Forms.CheckBox();
            this.buttonViewerPositionReset = new System.Windows.Forms.Button();
            this.buttonImport = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.groupBoxDrug.SuspendLayout();
            this.groupBoxKensin.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBoxDatadyna
            // 
            this.textBoxDatadyna.Location = new System.Drawing.Point(174, 57);
            this.textBoxDatadyna.Name = "textBoxDatadyna";
            this.textBoxDatadyna.Size = new System.Drawing.Size(377, 19);
            this.textBoxDatadyna.TabIndex = 2;
            // 
            // textBoxOQSFolder
            // 
            this.textBoxOQSFolder.Location = new System.Drawing.Point(174, 85);
            this.textBoxOQSFolder.Name = "textBoxOQSFolder";
            this.textBoxOQSFolder.Size = new System.Drawing.Size(377, 19);
            this.textBoxOQSFolder.TabIndex = 4;
            // 
            // textBoxRSBgazou
            // 
            this.textBoxRSBgazou.Location = new System.Drawing.Point(174, 113);
            this.textBoxRSBgazou.Name = "textBoxRSBgazou";
            this.textBoxRSBgazou.Size = new System.Drawing.Size(377, 19);
            this.textBoxRSBgazou.TabIndex = 6;
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
            this.groupBoxDrug.Location = new System.Drawing.Point(35, 230);
            this.groupBoxDrug.Name = "groupBoxDrug";
            this.groupBoxDrug.Size = new System.Drawing.Size(192, 171);
            this.groupBoxDrug.TabIndex = 4;
            this.groupBoxDrug.TabStop = false;
            this.groupBoxDrug.Text = "薬剤情報(xmlは常時取得)";
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
            this.label12.Location = new System.Drawing.Point(137, 93);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(49, 12);
            this.label12.TabIndex = 9;
            this.label12.Text = "期間(月)";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(139, 41);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(49, 12);
            this.label10.TabIndex = 8;
            this.label10.Text = "間隔(月)";
            // 
            // comboBoxYZspan
            // 
            this.comboBoxYZspan.FormattingEnabled = true;
            this.comboBoxYZspan.Location = new System.Drawing.Point(139, 108);
            this.comboBoxYZspan.Name = "comboBoxYZspan";
            this.comboBoxYZspan.Size = new System.Drawing.Size(38, 20);
            this.comboBoxYZspan.TabIndex = 26;
            // 
            // comboBoxYZinterval
            // 
            this.comboBoxYZinterval.FormattingEnabled = true;
            this.comboBoxYZinterval.Location = new System.Drawing.Point(139, 62);
            this.comboBoxYZinterval.Name = "comboBoxYZinterval";
            this.comboBoxYZinterval.Size = new System.Drawing.Size(40, 20);
            this.comboBoxYZinterval.TabIndex = 24;
            // 
            // radioButtonD0
            // 
            this.radioButtonD0.AutoSize = true;
            this.radioButtonD0.Location = new System.Drawing.Point(9, 41);
            this.radioButtonD0.Name = "radioButtonD0";
            this.radioButtonD0.Size = new System.Drawing.Size(84, 16);
            this.radioButtonD0.TabIndex = 16;
            this.radioButtonD0.TabStop = true;
            this.radioButtonD0.Text = "取り込まない";
            this.radioButtonD0.UseVisualStyleBackColor = true;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(9, 137);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(64, 12);
            this.label6.TabIndex = 4;
            this.label6.Text = "RSB検査名";
            // 
            // textBoxDrugName
            // 
            this.textBoxDrugName.Location = new System.Drawing.Point(79, 134);
            this.textBoxDrugName.Name = "textBoxDrugName";
            this.textBoxDrugName.Size = new System.Drawing.Size(100, 19);
            this.textBoxDrugName.TabIndex = 28;
            // 
            // radioButtonD3
            // 
            this.radioButtonD3.AutoSize = true;
            this.radioButtonD3.Location = new System.Drawing.Point(9, 109);
            this.radioButtonD3.Name = "radioButtonD3";
            this.radioButtonD3.Size = new System.Drawing.Size(73, 16);
            this.radioButtonD3.TabIndex = 22;
            this.radioButtonD3.TabStop = true;
            this.radioButtonD3.Text = "PDF+XML";
            this.radioButtonD3.UseVisualStyleBackColor = true;
            this.radioButtonD3.Visible = false;
            // 
            // radioButtonD2
            // 
            this.radioButtonD2.AutoSize = true;
            this.radioButtonD2.Location = new System.Drawing.Point(9, 86);
            this.radioButtonD2.Name = "radioButtonD2";
            this.radioButtonD2.Size = new System.Drawing.Size(45, 16);
            this.radioButtonD2.TabIndex = 20;
            this.radioButtonD2.TabStop = true;
            this.radioButtonD2.Text = "XML";
            this.radioButtonD2.UseVisualStyleBackColor = true;
            this.radioButtonD2.Visible = false;
            // 
            // radioButtonD1
            // 
            this.radioButtonD1.AutoSize = true;
            this.radioButtonD1.Location = new System.Drawing.Point(9, 63);
            this.radioButtonD1.Name = "radioButtonD1";
            this.radioButtonD1.Size = new System.Drawing.Size(104, 16);
            this.radioButtonD1.TabIndex = 18;
            this.radioButtonD1.TabStop = true;
            this.radioButtonD1.Text = "PDF to RSBase";
            this.radioButtonD1.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(33, 60);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(87, 12);
            this.label1.TabIndex = 5;
            this.label1.Text = "Datadynaの場所";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(33, 88);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(97, 12);
            this.label2.TabIndex = 6;
            this.label2.Text = "OQSフォルダの場所";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(33, 116);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(105, 12);
            this.label3.TabIndex = 7;
            this.label3.Text = "RSBase取込フォルダ";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(33, 200);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(65, 12);
            this.label4.TabIndex = 8;
            this.label4.Text = "タイマー間隔";
            // 
            // comboBoxTimerSecond
            // 
            this.comboBoxTimerSecond.FormattingEnabled = true;
            this.comboBoxTimerSecond.Location = new System.Drawing.Point(174, 197);
            this.comboBoxTimerSecond.Name = "comboBoxTimerSecond";
            this.comboBoxTimerSecond.Size = new System.Drawing.Size(53, 20);
            this.comboBoxTimerSecond.TabIndex = 14;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(33, 172);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(80, 12);
            this.label5.TabIndex = 10;
            this.label5.Text = "医療機関コード";
            // 
            // textBoxMCode
            // 
            this.textBoxMCode.Location = new System.Drawing.Point(174, 169);
            this.textBoxMCode.Name = "textBoxMCode";
            this.textBoxMCode.Size = new System.Drawing.Size(100, 19);
            this.textBoxMCode.TabIndex = 12;
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
            this.buttonOQSFolder.Location = new System.Drawing.Point(557, 85);
            this.buttonOQSFolder.Name = "buttonOQSFolder";
            this.buttonOQSFolder.Size = new System.Drawing.Size(21, 19);
            this.buttonOQSFolder.TabIndex = 5;
            this.buttonOQSFolder.Text = "...";
            this.buttonOQSFolder.UseVisualStyleBackColor = true;
            this.buttonOQSFolder.Click += new System.EventHandler(this.buttonOQSFolder_Click);
            // 
            // buttonRSBgazou
            // 
            this.buttonRSBgazou.Location = new System.Drawing.Point(557, 113);
            this.buttonRSBgazou.Name = "buttonRSBgazou";
            this.buttonRSBgazou.Size = new System.Drawing.Size(21, 19);
            this.buttonRSBgazou.TabIndex = 7;
            this.buttonRSBgazou.Text = "...";
            this.buttonRSBgazou.UseVisualStyleBackColor = true;
            this.buttonRSBgazou.Click += new System.EventHandler(this.buttonRSBgazou_Click);
            // 
            // buttonSaveExit
            // 
            this.buttonSaveExit.Location = new System.Drawing.Point(495, 509);
            this.buttonSaveExit.Name = "buttonSaveExit";
            this.buttonSaveExit.Size = new System.Drawing.Size(75, 23);
            this.buttonSaveExit.TabIndex = 50;
            this.buttonSaveExit.Text = "閉じる";
            this.buttonSaveExit.UseVisualStyleBackColor = true;
            this.buttonSaveExit.Click += new System.EventHandler(this.buttonSaveExit_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(414, 509);
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
            this.label8.Size = new System.Drawing.Size(135, 12);
            this.label8.TabIndex = 21;
            this.label8.Text = "OQSDrug_data.mdbの場所";
            // 
            // textBoxOQSDrugData
            // 
            this.textBoxOQSDrugData.Location = new System.Drawing.Point(174, 29);
            this.textBoxOQSDrugData.Name = "textBoxOQSDrugData";
            this.textBoxOQSDrugData.Size = new System.Drawing.Size(377, 19);
            this.textBoxOQSDrugData.TabIndex = 0;
            // 
            // textBoxRSBaseURL
            // 
            this.textBoxRSBaseURL.Location = new System.Drawing.Point(205, 142);
            this.textBoxRSBaseURL.Name = "textBoxRSBaseURL";
            this.textBoxRSBaseURL.Size = new System.Drawing.Size(346, 19);
            this.textBoxRSBaseURL.TabIndex = 10;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(172, 145);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(27, 12);
            this.label9.TabIndex = 24;
            this.label9.Text = "URL";
            // 
            // checkBoxRSBReload
            // 
            this.checkBoxRSBReload.AutoSize = true;
            this.checkBoxRSBReload.Location = new System.Drawing.Point(33, 144);
            this.checkBoxRSBReload.Name = "checkBoxRSBReload";
            this.checkBoxRSBReload.Size = new System.Drawing.Size(100, 16);
            this.checkBoxRSBReload.TabIndex = 8;
            this.checkBoxRSBReload.Text = "RSBase reload";
            this.checkBoxRSBReload.UseVisualStyleBackColor = true;
            // 
            // radioButtonK1
            // 
            this.radioButtonK1.AutoSize = true;
            this.radioButtonK1.Location = new System.Drawing.Point(7, 63);
            this.radioButtonK1.Name = "radioButtonK1";
            this.radioButtonK1.Size = new System.Drawing.Size(104, 16);
            this.radioButtonK1.TabIndex = 32;
            this.radioButtonK1.TabStop = true;
            this.radioButtonK1.Text = "PDF to RSBase";
            this.radioButtonK1.UseVisualStyleBackColor = true;
            // 
            // radioButtonK3
            // 
            this.radioButtonK3.AutoSize = true;
            this.radioButtonK3.Enabled = false;
            this.radioButtonK3.Location = new System.Drawing.Point(7, 109);
            this.radioButtonK3.Name = "radioButtonK3";
            this.radioButtonK3.Size = new System.Drawing.Size(73, 16);
            this.radioButtonK3.TabIndex = 36;
            this.radioButtonK3.TabStop = true;
            this.radioButtonK3.Text = "PDF+XML";
            this.radioButtonK3.UseVisualStyleBackColor = true;
            this.radioButtonK3.Visible = false;
            // 
            // textBoxKensinName
            // 
            this.textBoxKensinName.Location = new System.Drawing.Point(77, 134);
            this.textBoxKensinName.Name = "textBoxKensinName";
            this.textBoxKensinName.Size = new System.Drawing.Size(100, 19);
            this.textBoxKensinName.TabIndex = 40;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(7, 137);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(64, 12);
            this.label7.TabIndex = 6;
            this.label7.Text = "RSB検査名";
            // 
            // radioButtonK0
            // 
            this.radioButtonK0.AutoSize = true;
            this.radioButtonK0.Location = new System.Drawing.Point(7, 41);
            this.radioButtonK0.Name = "radioButtonK0";
            this.radioButtonK0.Size = new System.Drawing.Size(84, 16);
            this.radioButtonK0.TabIndex = 30;
            this.radioButtonK0.TabStop = true;
            this.radioButtonK0.Text = "取り込まない";
            this.radioButtonK0.UseVisualStyleBackColor = true;
            // 
            // comboBoxKSinterval
            // 
            this.comboBoxKSinterval.FormattingEnabled = true;
            this.comboBoxKSinterval.Location = new System.Drawing.Point(128, 62);
            this.comboBoxKSinterval.Name = "comboBoxKSinterval";
            this.comboBoxKSinterval.Size = new System.Drawing.Size(38, 20);
            this.comboBoxKSinterval.TabIndex = 38;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(128, 41);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(49, 12);
            this.label11.TabIndex = 9;
            this.label11.Text = "間隔(月)";
            // 
            // groupBoxKensin
            // 
            this.groupBoxKensin.Controls.Add(this.label11);
            this.groupBoxKensin.Controls.Add(this.comboBoxKSinterval);
            this.groupBoxKensin.Controls.Add(this.radioButtonK0);
            this.groupBoxKensin.Controls.Add(this.label7);
            this.groupBoxKensin.Controls.Add(this.textBoxKensinName);
            this.groupBoxKensin.Controls.Add(this.radioButtonK3);
            this.groupBoxKensin.Controls.Add(this.radioButtonK2);
            this.groupBoxKensin.Controls.Add(this.radioButtonK1);
            this.groupBoxKensin.Location = new System.Drawing.Point(264, 230);
            this.groupBoxKensin.Name = "groupBoxKensin";
            this.groupBoxKensin.Size = new System.Drawing.Size(192, 171);
            this.groupBoxKensin.TabIndex = 5;
            this.groupBoxKensin.TabStop = false;
            this.groupBoxKensin.Text = "特定健診";
            // 
            // radioButtonK2
            // 
            this.radioButtonK2.AutoSize = true;
            this.radioButtonK2.Enabled = false;
            this.radioButtonK2.Location = new System.Drawing.Point(7, 86);
            this.radioButtonK2.Name = "radioButtonK2";
            this.radioButtonK2.Size = new System.Drawing.Size(45, 16);
            this.radioButtonK2.TabIndex = 34;
            this.radioButtonK2.TabStop = true;
            this.radioButtonK2.Text = "XML";
            this.radioButtonK2.UseVisualStyleBackColor = true;
            this.radioButtonK2.Visible = false;
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
            this.label15.Location = new System.Drawing.Point(12, 419);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(64, 12);
            this.label15.TabIndex = 53;
            this.label15.Text = "Viewer設定";
            // 
            // textBoxTemprs
            // 
            this.textBoxTemprs.Location = new System.Drawing.Point(174, 442);
            this.textBoxTemprs.Name = "textBoxTemprs";
            this.textBoxTemprs.Size = new System.Drawing.Size(377, 19);
            this.textBoxTemprs.TabIndex = 54;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(35, 445);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(94, 12);
            this.label14.TabIndex = 55;
            this.label14.Text = "temp_rs.txtの場所";
            // 
            // checkBoxTopmost
            // 
            this.checkBoxTopmost.AutoSize = true;
            this.checkBoxTopmost.Location = new System.Drawing.Point(35, 474);
            this.checkBoxTopmost.Name = "checkBoxTopmost";
            this.checkBoxTopmost.Size = new System.Drawing.Size(127, 16);
            this.checkBoxTopmost.TabIndex = 56;
            this.checkBoxTopmost.Text = "薬歴を最前面で表示";
            this.checkBoxTopmost.UseVisualStyleBackColor = true;
            // 
            // buttonViewerPositionReset
            // 
            this.buttonViewerPositionReset.Location = new System.Drawing.Point(176, 470);
            this.buttonViewerPositionReset.Name = "buttonViewerPositionReset";
            this.buttonViewerPositionReset.Size = new System.Drawing.Size(179, 23);
            this.buttonViewerPositionReset.TabIndex = 57;
            this.buttonViewerPositionReset.Text = "薬歴Windowの位置サイズをリセット";
            this.buttonViewerPositionReset.UseVisualStyleBackColor = true;
            this.buttonViewerPositionReset.Click += new System.EventHandler(this.buttonViewerPositionReset_Click);
            // 
            // buttonImport
            // 
            this.buttonImport.Location = new System.Drawing.Point(252, 509);
            this.buttonImport.Name = "buttonImport";
            this.buttonImport.Size = new System.Drawing.Size(75, 23);
            this.buttonImport.TabIndex = 58;
            this.buttonImport.Text = "インポート";
            this.buttonImport.UseVisualStyleBackColor = true;
            this.buttonImport.Click += new System.EventHandler(this.buttonImport_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(333, 509);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 59;
            this.button2.Text = "エクスポート";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.buttonExport_Click);
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(594, 550);
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
        private System.Windows.Forms.RadioButton radioButtonK3;
        private System.Windows.Forms.TextBox textBoxKensinName;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.RadioButton radioButtonK0;
        private System.Windows.Forms.ComboBox comboBoxKSinterval;
        private System.Windows.Forms.Label label11;
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
    }
}
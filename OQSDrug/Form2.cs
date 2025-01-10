using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Xml.XPath;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;

namespace OQSDrug
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            initForm();
        }
            
        private void initForm()
        {
            // 設定値を読み込む
            textBoxOQSDrugData.Text = Properties.Settings.Default.OQSDrugData;
            textBoxDatadyna.Text = Properties.Settings.Default.Datadyna;
            textBoxOQSFolder.Text = Properties.Settings.Default.OQSFolder;
            textBoxRSBgazou.Text = Properties.Settings.Default.RSBgazouFolder;
            checkBoxRSBReload.Checked = Properties.Settings.Default.RSBReload;
            textBoxRSBaseURL.Text = Properties.Settings.Default.RSBUrl;
            textBoxMCode.Text = Properties.Settings.Default.MCode;

            comboBoxTimerSecond.Items.AddRange(new object[] { 10, 30, 60 });
            int TimerInterval = Properties.Settings.Default.TimerInterval;
            // 該当する値を選択
            if (comboBoxTimerSecond.Items.Contains(TimerInterval))
            {
                comboBoxTimerSecond.SelectedItem = TimerInterval;
            }
            else
            {
                comboBoxTimerSecond.SelectedItem = 30;
            }

            //薬剤グループボックス
            checkBoxSinryou.Checked = false;
            radioButtonD0.Checked = false;
            radioButtonD1.Checked = false;

            switch (Properties.Settings.Default.DrugFileCategory) //
            {
                case 1:
                    radioButtonD1.Checked = true;
                    break;
                case 3:
                    radioButtonD1.Checked = true;
                    checkBoxSinryou.Checked = true;
                    break;
                default:
                    // デフォルト動作（例: 何も選択しない）
                    radioButtonD0.Checked = true;
                    break;
            }

            comboBoxYZinterval.Items.AddRange(new object[] { 1, 2, 3 });
            int YZinterval = Properties.Settings.Default.YZinterval;
            if (comboBoxYZinterval.Items.Contains(YZinterval))
            {
                comboBoxYZinterval.SelectedItem = YZinterval;
            }
            else
            {
                comboBoxYZinterval.SelectedIndex = 0;
            }

            comboBoxYZspan.Items.AddRange(new object[] { 3, 6, 12, 24 });
            int YZspan = Properties.Settings.Default.YZspan;
            if (comboBoxYZspan.Items.Contains(YZspan))
            {
                comboBoxYZspan.SelectedItem = YZspan;
            }
            else
            {
                comboBoxYZspan.SelectedIndex = 1;
            }
            textBoxDrugName.Text = Properties.Settings.Default.YZname;

            //健診情報
            radioButtonK0.Checked = false;
            radioButtonK1.Checked = false;
            radioButtonK2.Checked = false;
            radioButtonK3.Checked = false;
            switch (Properties.Settings.Default.KensinFileCategory)
            {
                case 1:
                    radioButtonK1.Checked = true;
                    break;
                default:
                    // デフォルト動作（例: 何も選択しない）
                    radioButtonK0.Checked = true;
                    break;
            }

            comboBoxKSinterval.Items.AddRange(new object[] { 1, 3, 6, 12 });
            int KSinterval = Properties.Settings.Default.KSinterval;
            if (comboBoxKSinterval.Items.Contains(KSinterval))
            {
                comboBoxKSinterval.SelectedItem = KSinterval;
            }
            else
            {
                comboBoxKSinterval.SelectedIndex = 2;
            }
            textBoxKensinName.Text = Properties.Settings.Default.KSname;

            textBoxTemprs.Text = Properties.Settings.Default.temprs;

            checkBoxTopmost.Checked = Properties.Settings.Default.ViewerTopmost;

            checkBoxMinimumStart.Checked = Properties.Settings.Default.MinimumStart;

            int savedIndex = Properties.Settings.Default.RSBID;
            if (savedIndex >= 0 && savedIndex < comboBoxRSBID.Items.Count)
            {
                comboBoxRSBID.SelectedIndex = savedIndex;
            }
            else
            {
                comboBoxRSBID.SelectedIndex = 0;
            }

            checkBoxKeepXml.Checked = Properties.Settings.Default.KeepXml;

            //comboBoxDynaTable.SelectedItem = Properties.Settings.Default.DynaTable;

            comboBoxViewSpan.Items.AddRange(new object[] { 0, 1, 3, 6, 12 });
            if (comboBoxViewSpan.Items.Contains(Properties.Settings.Default.ViewerSpan))
            {
                comboBoxViewSpan.SelectedItem = Properties.Settings.Default.ViewerSpan;
            }
            else
            {
                comboBoxViewSpan.SelectedIndex = 3;
            }

        }

        private void SaveSettings()
        {
            int DrugFileCategory = 0, KensinFileCategory = 0;

            Properties.Settings.Default.OQSDrugData = textBoxOQSDrugData.Text;
            Properties.Settings.Default.Datadyna =textBoxDatadyna.Text;
            Properties.Settings.Default.OQSFolder = textBoxOQSFolder.Text;
            Properties.Settings.Default.RSBgazouFolder = textBoxRSBgazou.Text;
            Properties.Settings.Default.RSBReload = checkBoxRSBReload.Checked;
            Properties.Settings.Default.RSBUrl = textBoxRSBaseURL.Text;
            Properties.Settings.Default.MCode = textBoxMCode.Text;

            Properties.Settings.Default.TimerInterval = Convert.ToUInt16(comboBoxTimerSecond.SelectedItem.ToString());

            //薬剤グループボックス
            // ラジオボタンの選択状況を確認
            DrugFileCategory = (radioButtonD0.Checked) ? 0 : 1;
            if (checkBoxSinryou.Checked) DrugFileCategory += 2;
            
            Properties.Settings.Default.DrugFileCategory = DrugFileCategory;
            Properties.Settings.Default.YZinterval = Convert.ToUInt16(comboBoxYZinterval.SelectedItem.ToString());
            Properties.Settings.Default.YZspan = Convert.ToUInt16(comboBoxYZspan.SelectedItem.ToString());
            Properties.Settings.Default.YZname = textBoxDrugName.Text;

            //健診グループボックス
            if (radioButtonK0.Checked)
            {
                KensinFileCategory = 0;
            }
            else if (radioButtonK1.Checked)
            {
                KensinFileCategory = 1;
            }
            Properties.Settings.Default.KensinFileCategory = KensinFileCategory;
            Properties.Settings.Default.KSinterval = Convert.ToUInt16(comboBoxKSinterval.SelectedItem.ToString());
            Properties.Settings.Default.KSname = textBoxKensinName.Text;

            Properties.Settings.Default.temprs = textBoxTemprs.Text;

            Properties.Settings.Default.ViewerTopmost = checkBoxTopmost.Checked ;

            Properties.Settings.Default.MinimumStart = checkBoxMinimumStart.Checked;

            Properties.Settings.Default.RSBID = comboBoxRSBID.SelectedIndex;

            Properties.Settings.Default.KeepXml = checkBoxKeepXml.Checked;

            //Properties.Settings.Default.DynaTable = comboBoxDynaTable.SelectedItem.ToString();

            Properties.Settings.Default.ViewerSpan = Convert.ToInt16(comboBoxViewSpan.SelectedItem.ToString());

            Properties.Settings.Default.Save();

        }

        private void buttonOQSDrugData_Click(object sender, EventArgs e)
        {
            // ファイル選択ダイアログの設定
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "MDB files (*.mdb)|*.mdb|All files (*.*)|*.*";
            openFileDialog.Title = "OQSDrug_data.mdbを選択してください";

            // ダイアログを表示して結果を確認
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                // 選択したファイルパスをテキストボックスに設定
                textBoxOQSDrugData.Text = openFileDialog.FileName;
            }
            
        }

        private void buttonDatadyna_Click(object sender, EventArgs e)
        {
            // ファイル選択ダイアログの設定
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "MDB files (*.mdb)|*.mdb|All files (*.*)|*.*";
            openFileDialog.Title = "Datadyna.mdbを選択してください";

            // ダイアログを表示して結果を確認
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                // 選択したファイルパスをテキストボックスに設定
                textBoxDatadyna.Text = openFileDialog.FileName;
            }
        }

        private void buttonOQSFolder_Click(object sender, EventArgs e)
        {
            // フォルダ選択ダイアログを開く
            using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
            {
                folderDialog.Description = "OQSフォルダを選択してください";
                folderDialog.ShowNewFolderButton = true;

                // ダイアログを表示して結果を確認
                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    // 選択したフォルダパスをテキストボックスに設定
                    textBoxOQSFolder.Text = folderDialog.SelectedPath;
                }
            }
        }

        private void buttonRSBgazou_Click(object sender, EventArgs e)
        {
            // フォルダ選択ダイアログを開く
            using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
            {
                folderDialog.Description = "RSBase取込フォルダを選択してください";
                folderDialog.ShowNewFolderButton = true;

                // ダイアログを表示して結果を確認
                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    // 選択したフォルダパスをテキストボックスに設定
                    textBoxRSBgazou.Text = folderDialog.SelectedPath;
                }
            }
        }
                
        private void buttonCancel_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("設定を保存せずに閉じますがよろしいですか？", "確認", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                this.Close();
            }
        }

        private void buttonSaveExit_Click(object sender, EventArgs e)
        {
            SaveSettings();

            try
            {
                // デフォルトの保存場所に自動保存
                string defaultPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.Personal),
                    "OQSDrug.config"
                );
                var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal);
                config.SaveAs(defaultPath, ConfigurationSaveMode.Full);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"設定値のエクスポート時にエラーが発生しました：{ex.Message}");
            }
            this.Close();
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            SaveSettings();
            // 確認のためメッセージ表示
            MessageBox.Show("設定が保存されました");
        }

        private void buttonViewerPositionReset_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.ViewerBounds = System.Drawing.Rectangle.Empty;
        }

        /// <summary>
        /// 現在の設定をXMLファイルにエクスポート（保存）
        /// </summary>
        private void buttonExport_Click(object sender, EventArgs e)
        {
            string settingsFilePath;

            SaveFileDialog op = new SaveFileDialog();
            op.Title = "設定の保存先";
            op.FileName = "OQSDrug.config";
            op.InitialDirectory = System.Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            op.Filter = "configファイル(*.config)|*.config|すべてのファイル(*.*)|*.*";
            op.FilterIndex = 1;
            op.RestoreDirectory = true;
            op.CheckFileExists = false;
            op.CheckPathExists = true;

            if (op.ShowDialog(this) == DialogResult.OK)
            {
                settingsFilePath = op.FileName;
                Properties.Settings.Default.Save();
                var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal);
                config.SaveAs(settingsFilePath);
                MessageBox.Show("設定を保存しました");
            }
            op.Dispose();
        }
        private void buttonImport_Click(object sender, EventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog();
            op.Title = "設定ファイルの読込";
            op.FileName = "OQSDrug.config";
            op.InitialDirectory = System.Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            op.Filter = "configファイル(*.config)|*.config|すべてのファイル(*.*)|*.*";
            op.FilterIndex = 1;
            op.RestoreDirectory = true;
            op.CheckFileExists = false;
            op.CheckPathExists = true;

            if (op.ShowDialog(this) == DialogResult.OK)
            {
                string settingsFilePath = op.FileName;
                Properties.Settings appSettings = Properties.Settings.Default;

                try
                {
                    var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal);

                    string appSettingsXmlName = Properties.Settings.Default.Context["GroupName"].ToString();
                    // returns "MyApplication.Properties.Settings";

                    // Open settings file as XML
                    var import = XDocument.Load(settingsFilePath);
                    // Get the whole XML inside the settings node
                    var settings = import.XPathSelectElements("//" + appSettingsXmlName);

                    config.GetSectionGroup("userSettings")
                        .Sections[appSettingsXmlName]
                        .SectionInformation
                        .SetRawXml(settings.Single().ToString());
                    config.Save(ConfigurationSaveMode.Modified);
                    ConfigurationManager.RefreshSection("userSettings");

                    appSettings.Reload();
                    initForm();

                    MessageBox.Show("設定を読み込みました");
                }
                catch (Exception ex) // Should make this more specific
                {
                    // Could not import settings.
                    appSettings.Reload(); // from last set saved, not defaults
                    MessageBox.Show(ex.ToString());
                }
            }

            op.Dispose();

        }
    }
}

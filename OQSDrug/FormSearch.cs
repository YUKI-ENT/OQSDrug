using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace OQSDrug
{
    public partial class FormSearch : Form
    {
        public FormSearch()
        {
            InitializeComponent();
        }

        // Form3 から HTML を受け取るためのメソッド
        public void SetDrugName(string drugName)
        {
            textBoxDrugName.Text = drugName;

            buttonSearch_Click(null, EventArgs.Empty);
        }

        public void SetDrugLists(List<Tuple<string[], double>> results)
        {
            listBoxDrugs.Items.Clear();

            foreach (var result in results)
            {
                string[] drugLists = result.Item1;  // レコード（drugLists[0], drugLists[1]）
                //double similarity = result.Item2;   // 類似度
                string Senpatsu = (drugLists[3] == "先発") ? "【先発】" : ""; 

                // 表示名は drugLists[0] に類似度を加えて表示
                string displayText = $"{Senpatsu}{drugLists[0]} ({drugLists[1]})"; 

                // 実際に取得したい値は drugLists[1] にする
                listBoxDrugs.Items.Add(new KeyValuePair<string, string>(displayText, drugLists[0]));
            }
            listBoxDrugs.DisplayMember = "Key";  // 表示されるのは Key（最初の値）
            listBoxDrugs.ValueMember = "Value";  // 実際に取得されるのは Value（2番目の値）
        }


        static string GenerateHtmlForm(string url, string postData)
        {
            // POSTデータを解析してフォームに変換
            var formInputs = string.Empty;
            foreach (var param in postData.Split('&'))
            {
                var keyValue = param.Split('=');
                if (keyValue.Length == 2)
                {
                    formInputs += $"<input type=\"hidden\" name=\"{keyValue[0]}\" value=\"{keyValue[1]}\">\n";
                }
            }

            // HTMLフォームを生成
            return $@"
                    <!DOCTYPE html>
                    <html>
                    <head>
                    <head><META HTTP-EQUIV=""Content-Type"" Content=""text/html; charset=Shift_JIS"">
                        <title>POST Form</title>
                    </head>
                    <body onload=""document.forms[0].submit();"">
                        <form action=""{url}"" method=""post"">
                            {formInputs}
                        </form>
                    </body>
                    </html>";
        }

        private void buttonSearch_Click(object sender, EventArgs e)
        {
            if(textBoxDrugName.Text.Length > 0)
            {
                string sjisString = ConvertToShiftJisString(textBoxDrugName.Text);
                
                // POST先のURLとデータを定義
                string url = "http://localhost/~rsn/kinki.cgi";
                //string postData = "yakka=" + WebUtility.UrlEncode(textBoxDrugName.Text);
                string postData = "yakka=" + sjisString;

                // HTMLファイルを生成
                string htmlContent = GenerateHtmlForm(url, postData);
                string tempHtmlPath = Path.Combine(Path.GetTempPath(), "tempPostForm.html");
                File.WriteAllText(tempHtmlPath, htmlContent,  Encoding.GetEncoding("Shift_JIS"));

                // 標準ブラウザでHTMLを開く
                Process.Start(new ProcessStartInfo
                {
                    FileName = tempHtmlPath,
                    UseShellExecute = true
                });
            }
        }

        static string ConvertToShiftJisString(string utf8input)
        {
            try
            {
                // UTF-8からShift_JISに変換
                Encoding utf8 = Encoding.UTF8;
                Encoding shiftJis = Encoding.GetEncoding("Shift_JIS");

                // UTF-8文字列をバイト配列に変換
                byte[] utf8Bytes = utf8.GetBytes(utf8input);

                // バイト配列をShift_JISのバイト配列に変換
                byte[] shiftJisBytes = Encoding.Convert(utf8, shiftJis, utf8Bytes);

                // Shift_JISバイト配列を文字列に変換
                return shiftJis.GetString(shiftJisBytes);
            }
            catch (Exception ex)
            {
                MessageBox.Show("文字コード変換でエラー:" + ex.Message);
                return null;
            }
        }

        private void listBoxDrugs_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxDrugs.SelectedItem is KeyValuePair<string, string> selectedItem)
            {
                string key = selectedItem.Key;  // 表示された値（Key）
                string value = selectedItem.Value;  // 実際に取得したい値（Value）
                
                textBoxDrugName.Text = value;
            }
        }

        private void listBoxDrugs_DoubleClick(object sender, EventArgs e)
        {
            buttonSearch_Click(sender, e);
        }
    }
}

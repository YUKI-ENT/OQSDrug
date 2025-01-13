using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OQSDrug
{
    public partial class FormDI : Form
    {
        public FormDI()
        {
            InitializeComponent();
        }

        // Form3 から HTML を受け取るためのメソッド
        public void DisplayHTMLContent(string htmlContent)
        {
            // WebBrowser コントロールで HTML コンテンツを表示
            webBrowser1.DocumentText = htmlContent;
        }

        public void SetDrugNames(IEnumerable<string> drugNames)
        {
            toolStripComboBoxDrugNames.Items.Clear();
            toolStripComboBoxDrugNames.Items.AddRange(drugNames.ToArray());
        }
    }
}

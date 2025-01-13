namespace OQSDrug
{
    partial class FormSearch
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormSearch));
            this.buttonSearch = new System.Windows.Forms.Button();
            this.listBoxDrugs = new System.Windows.Forms.ListBox();
            this.textBoxDrugName = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // buttonSearch
            // 
            this.buttonSearch.Font = new System.Drawing.Font("Meiryo UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.buttonSearch.Image = global::OQSDrug.Properties.Resources.Find;
            this.buttonSearch.Location = new System.Drawing.Point(490, 122);
            this.buttonSearch.Name = "buttonSearch";
            this.buttonSearch.Size = new System.Drawing.Size(44, 28);
            this.buttonSearch.TabIndex = 1;
            this.buttonSearch.UseVisualStyleBackColor = true;
            this.buttonSearch.Click += new System.EventHandler(this.buttonSearch_Click);
            // 
            // listBoxDrugs
            // 
            this.listBoxDrugs.Font = new System.Drawing.Font("Meiryo UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.listBoxDrugs.FormattingEnabled = true;
            this.listBoxDrugs.ItemHeight = 17;
            this.listBoxDrugs.Location = new System.Drawing.Point(12, 12);
            this.listBoxDrugs.Name = "listBoxDrugs";
            this.listBoxDrugs.Size = new System.Drawing.Size(472, 106);
            this.listBoxDrugs.TabIndex = 2;
            this.listBoxDrugs.SelectedIndexChanged += new System.EventHandler(this.listBoxDrugs_SelectedIndexChanged);
            this.listBoxDrugs.DoubleClick += new System.EventHandler(this.listBoxDrugs_DoubleClick);
            // 
            // textBoxDrugName
            // 
            this.textBoxDrugName.Font = new System.Drawing.Font("Meiryo UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.textBoxDrugName.Location = new System.Drawing.Point(12, 122);
            this.textBoxDrugName.Name = "textBoxDrugName";
            this.textBoxDrugName.Size = new System.Drawing.Size(472, 28);
            this.textBoxDrugName.TabIndex = 0;
            // 
            // FormSearch
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(546, 164);
            this.Controls.Add(this.listBoxDrugs);
            this.Controls.Add(this.buttonSearch);
            this.Controls.Add(this.textBoxDrugName);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormSearch";
            this.Text = "RSBase薬情検索";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button buttonSearch;
        private System.Windows.Forms.ListBox listBoxDrugs;
        private System.Windows.Forms.TextBox textBoxDrugName;
    }
}
namespace WinSearchDBAnalyzer
{
    partial class DetailViewForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DetailViewForm));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.listViewColumn = new System.Windows.Forms.ListView();
            this.columnName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.textBoxDetail = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.listViewColumn);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.textBoxDetail);
            this.splitContainer1.Size = new System.Drawing.Size(584, 562);
            this.splitContainer1.SplitterDistance = 194;
            this.splitContainer1.TabIndex = 0;
            // 
            // listViewColumn
            // 
            this.listViewColumn.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnName});
            this.listViewColumn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewColumn.FullRowSelect = true;
            this.listViewColumn.HideSelection = false;
            this.listViewColumn.Location = new System.Drawing.Point(0, 0);
            this.listViewColumn.MultiSelect = false;
            this.listViewColumn.Name = "listViewColumn";
            this.listViewColumn.Size = new System.Drawing.Size(194, 562);
            this.listViewColumn.TabIndex = 0;
            this.listViewColumn.UseCompatibleStateImageBehavior = false;
            this.listViewColumn.View = System.Windows.Forms.View.Details;
            this.listViewColumn.SelectedIndexChanged += new System.EventHandler(this.listViewColumn_SelectedIndexChanged);
            // 
            // columnName
            // 
            this.columnName.Text = "Column Name";
            // 
            // textBoxDetail
            // 
            this.textBoxDetail.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.textBoxDetail.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxDetail.Location = new System.Drawing.Point(0, 0);
            this.textBoxDetail.Multiline = true;
            this.textBoxDetail.Name = "textBoxDetail";
            this.textBoxDetail.ReadOnly = true;
            this.textBoxDetail.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxDetail.Size = new System.Drawing.Size(386, 562);
            this.textBoxDetail.TabIndex = 0;
            // 
            // DetailViewForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 562);
            this.Controls.Add(this.splitContainer1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "DetailViewForm";
            this.Text = "Detail View";
            this.TopMost = true;
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ListView listViewColumn;
        private System.Windows.Forms.TextBox textBoxDetail;
        private System.Windows.Forms.ColumnHeader columnName;
    }
}
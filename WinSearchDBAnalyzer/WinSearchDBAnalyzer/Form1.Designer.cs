namespace WinSearchDBAnalyzer
{
    partial class SelectColumnForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SelectColumnForm));
            this.label1 = new System.Windows.Forms.Label();
            this.listViewColumn = new System.Windows.Forms.ListView();
            this.buttonOK = new System.Windows.Forms.Button();
            this.item = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(203, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "Check the items you want to parse";
            // 
            // listViewColumn
            // 
            this.listViewColumn.CheckBoxes = true;
            this.listViewColumn.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.item});
            this.listViewColumn.Location = new System.Drawing.Point(12, 38);
            this.listViewColumn.Name = "listViewColumn";
            this.listViewColumn.Size = new System.Drawing.Size(360, 479);
            this.listViewColumn.TabIndex = 1;
            this.listViewColumn.UseCompatibleStateImageBehavior = false;
            this.listViewColumn.View = System.Windows.Forms.View.Details;
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(297, 523);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 27);
            this.buttonOK.TabIndex = 2;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // item
            // 
            this.item.Text = "File Information";
            this.item.Width = 350;
            // 
            // SelectColumnForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(384, 562);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.listViewColumn);
            this.Controls.Add(this.label1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SelectColumnForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "File Information";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SelectColumnForm_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListView listViewColumn;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.ColumnHeader item;
    }
}
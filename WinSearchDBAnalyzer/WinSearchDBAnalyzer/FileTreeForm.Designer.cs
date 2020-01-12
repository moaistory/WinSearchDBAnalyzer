namespace WinSearchDBAnalyzer
{
    partial class FileTreeForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FileTreeForm));
            this.treeViewExplorer = new System.Windows.Forms.TreeView();
            this.imageListIcon = new System.Windows.Forms.ImageList(this.components);
            this.SuspendLayout();
            // 
            // treeViewExplorer
            // 
            this.treeViewExplorer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeViewExplorer.ImageIndex = 0;
            this.treeViewExplorer.ImageList = this.imageListIcon;
            this.treeViewExplorer.Location = new System.Drawing.Point(0, 0);
            this.treeViewExplorer.Name = "treeViewExplorer";
            this.treeViewExplorer.SelectedImageIndex = 0;
            this.treeViewExplorer.Size = new System.Drawing.Size(234, 562);
            this.treeViewExplorer.TabIndex = 0;
            this.treeViewExplorer.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeViewExplorer_NodeMouseClick);
            // 
            // imageListIcon
            // 
            this.imageListIcon.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListIcon.ImageStream")));
            this.imageListIcon.TransparentColor = System.Drawing.Color.Transparent;
            this.imageListIcon.Images.SetKeyName(0, "folder.png");
            // 
            // FileTreeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(234, 562);
            this.CloseButton = false;
            this.CloseButtonVisible = false;
            this.Controls.Add(this.treeViewExplorer);
            this.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FileTreeForm";
            this.Text = "Directory Tree";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView treeViewExplorer;
        private System.Windows.Forms.ImageList imageListIcon;
    }
}
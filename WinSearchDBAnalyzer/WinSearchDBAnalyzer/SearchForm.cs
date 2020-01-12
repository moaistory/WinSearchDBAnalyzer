using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace WinSearchDBAnalyzer
{
    public partial class SearchForm : DockContent
    {
        private MainForm mainForm;
        public SearchForm(MainForm formMain)
        {
            InitializeComponent();
            this.ClientSize = new System.Drawing.Size(50,50);
            this.ResumeLayout(true);
            this.mainForm = formMain;
        }

        private void DockSearch_DockStateChanged(object sender, EventArgs e)
        {
            this.AutoHidePortion = 45;
            
        }

        private void DockSearch_Resize(object sender, EventArgs e)
        {
            textBoxSearch.Width = panel1.Width;
        }

        private void buttonSearch_Click_1(object sender, EventArgs e)
        {
            mainForm.searchBackground(textBoxSearch.Text);
        }
    }
}

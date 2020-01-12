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
    
    public partial class PreviewForm : DockContent
    {
        private MainForm mainForm;
        public PreviewForm(MainForm mainForm)
        {
            InitializeComponent();
            this.mainForm = mainForm;
        }

        public void setText(string summary)
        {
            textBox.Text = summary;
        }
    }
}

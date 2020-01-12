using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WinSearchDBAnalyzer
{
    public partial class WaitForm : Form
    {
        int maximum = 100;
        public WaitForm()
        {
            InitializeComponent();
        }
        public void setStyle(ProgressBarStyle style)
        {
            progressBar.Style = style;
        }

        public void setPrgress(int value)
        {
            try
            {
                progressBar.Maximum = maximum + 1;
                progressBar.Value = value + 1;
                progressBar.Maximum = maximum;
                progressBar.Value = value;
                labelPecentage.Text = "(" + value.ToString() + "/" + progressBar.Maximum + ")";
            }
            catch { }
        }

        public void setPercent(int value)
        {
            labelPecentage.Text = "(" + value + "%)";
        }
        public void maxProgress(int value)
        {
            maximum = value;

        }
        public void end(String message)
        {
            labelPecentage.Text = message;
        }

        public void setLabel(String message)
        {
            label.Text = message;
        }
    }
}

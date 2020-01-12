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
    public partial class SelectColumnForm : Form
    {
        public SelectColumnForm()
        {
            InitializeComponent();
        }

        public void setColumnList(List<string> columnList, List<string> parsingColumnList)
        {
            for(int i=0; i<columnList.Count; i++)
            {
                this.listViewColumn.Items.Add(columnList[i]);
                foreach (string parsingColumnName in parsingColumnList)
                {
                    if(columnList[i].Contains(parsingColumnName))
                    {
                        listViewColumn.Items[i].Checked = true;
                    }
                }
            }
        }

        public List<string> getCheckedColumn()
        {
            listViewColumn.Items[0].Checked = true;
            List<string> columnList = new List<string>();
            foreach (ListViewItem item in listViewColumn.CheckedItems)
            {
                columnList.Add(item.SubItems[0].Text);
            }
            return columnList;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            Close();
        }

        private void SelectColumnForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }
    }
}

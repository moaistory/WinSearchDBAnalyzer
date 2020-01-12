using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WinSearchDBAnalyzer.EDBParser;

namespace WinSearchDBAnalyzer
{
    public partial class DetailViewForm : Form
    {
        private ListViewItem listViewItem;
        
        public DetailViewForm()
        {
            InitializeComponent();
        }
        
        public void setListViewItem(ListViewItem listViewItem)
        {
            this.listViewItem = listViewItem;
            listViewColumn.Items[0].Selected = true;
            listViewColumn.Select();
        }

        public void setAutoColumnSize()
        {
            listViewColumn.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            listViewColumn.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }


        public void setColumnList(List<string> columnList)
        {
            foreach (string column in columnList)
            {
                listViewColumn.Items.Add(column);
            }
            setAutoColumnSize();
        }

        private void listViewColumn_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewColumn.Items.Count == 0)
                return;
            if (listViewColumn.SelectedIndices.Count == 0)
                return;

            textBoxDetail.Text = this.listViewItem.SubItems[listViewColumn.SelectedIndices[0]+1].Text.ToString();
        }
    }
}

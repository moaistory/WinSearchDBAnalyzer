using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using WinSearchDBAnalyzer.EDBParser;

namespace WinSearchDBAnalyzer
{
    public partial class ExplorerForm : DockContent
    {
        private MainForm mainForm;
        private List<ListViewItem> listViewItemList;
        private List<String> columnNameList;
        public ExplorerForm(MainForm mainForm)
        {
            InitializeComponent();
            this.mainForm = mainForm;
            columnNameList = new List<string>();
        }

        public void clearColumn()
        {
            listViewData.Columns.Clear();
        }

        public void clearData()
        {
            listViewData.Items.Clear();
        }
        public void setTitle(String title)
        {
            this.Text = title;
        }
        public int getColumnCount()
        {
            return listViewData.Columns.Count;
        }

        public void setData(List<ListViewItem> listViewItemList)
        {
            listViewData.BeginUpdate();
            this.listViewItemList = listViewItemList;
            foreach (ListViewItem listViewItem in listViewItemList)
            {
                listViewData.Items.Add(listViewItem);
            }
            listViewData.EndUpdate();
        }

        public void setColumn(List<string> columnList)
        {
            columnNameList.Clear();
            listViewData.Columns.Add("", 0, HorizontalAlignment.Left);
            foreach (string columnName in columnList)
            {
                listViewData.Columns.Add(columnName, -2, HorizontalAlignment.Left);
                columnNameList.Add(columnName);
            }
            //this.setAutoColumnSize();
        }

        public void setColumn(List<Column> columnList)
        {
            columnNameList.Clear();
            listViewData.Columns.Add("", 0, HorizontalAlignment.Left);
            foreach (Column column in columnList)
            {
                listViewData.Columns.Add(column.getName(), -2, HorizontalAlignment.Left);
                columnNameList.Add(column.getName());
            }
            //this.setAutoColumnSize();
        }

        public void addColumn(List<string> columnList)
        {
            foreach (string columnName in columnList)
            {
                listViewData.Columns.Add(columnName, -2, HorizontalAlignment.Left);
                columnNameList.Add(columnName);
            }
            //this.setAutoColumnSize();
        }

        public void setAutoColumnSize()
        {
            listViewData.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            listViewData.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }

        private void listViewData_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            foreach (ListViewItem lvi in listViewData.Items)
            {
                for (int i = 0; i < getColumnCount()- lvi.SubItems.Count;i++ )
                {
                    lvi.SubItems.Add("");
                }
            }

            // 방향 초기화
            for (int i = 0; i < listViewData.Columns.Count; i++)
            {
                listViewData.Columns[i].Text = listViewData.Columns[i].Text.Replace(" △", "");
                listViewData.Columns[i].Text = listViewData.Columns[i].Text.Replace(" ▽", "");
            }

            // DESC
            if (this.listViewData.Sorting == SortOrder.Ascending || listViewData.Sorting == SortOrder.None)
            {
                this.listViewData.ListViewItemSorter = new ListViewItemComparer(e.Column, "desc");
                listViewData.Sorting = SortOrder.Descending;
                listViewData.Columns[e.Column].Text = listViewData.Columns[e.Column].Text + " ▽";
            }
            // ASC
            else
            {
                this.listViewData.ListViewItemSorter = new ListViewItemComparer(e.Column, "asc");
                listViewData.Sorting = SortOrder.Ascending;
                listViewData.Columns[e.Column].Text = listViewData.Columns[e.Column].Text + " △";
            }
            listViewData.Sort();
        }

        private void listViewData_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            DetailViewForm detailViewFrom = new DetailViewForm();
            detailViewFrom.setColumnList(columnNameList);
            ListViewItem listViewItem = listViewData.SelectedItems[0];
            detailViewFrom.setListViewItem(listViewItem);
            detailViewFrom.Show();
        }

        private void listViewData_MouseClick(object sender, MouseEventArgs e)
        {
            /*
            string summary = "";
            string result = "";
            for (int i = 1; i < listViewData.Columns.Count; i++)
            {
                string columnName = listViewData.Columns[i].Text;
                if (columnName.Contains("-"))
                {
                    columnName = columnName.Split('-')[1];
                }
                columnName = columnName.Replace("System_", "");
                if (listViewData.Columns[i].Text.Contains("System_Search_AutoSummary : "))
                {
                    summary = columnName + Environment.NewLine;
                    summary += listViewData.SelectedItems[0].SubItems[i].Text + Environment.NewLine;
                }
                else
                {
                    result += columnName + " : ";
                    result += listViewData.SelectedItems[0].SubItems[i].Text + Environment.NewLine + Environment.NewLine;
                }
                
            }
            
            mainForm.setSummary(result + summary);
           */   
        }

        private void listViewData_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            string summary = "";
            string result = "";
            if (listViewData.SelectedItems.Count > 0)
            {
                for (int i = 1; i < listViewData.SelectedItems[0].SubItems.Count; i++)
                {
                    string columnName = listViewData.Columns[i].Text;
                    if (columnName.Contains("-"))
                    {
                        columnName = columnName.Split('-')[1];
                    }
                    columnName = columnName.Replace("System_", "");
                    if (listViewData.Columns[i].Text.Contains("System_Search_AutoSummary"))
                    {
                        summary = columnName +  " : " + Environment.NewLine;
                        summary += listViewData.SelectedItems[0].SubItems[i].Text.TrimEnd().Replace("\0", string.Empty) + Environment.NewLine;
                    }
                    else
                    {
                        result += columnName + " : ";
                        result += listViewData.SelectedItems[0].SubItems[i].Text.TrimEnd().Replace("\0", string.Empty) + Environment.NewLine + Environment.NewLine;
                    }

                }

                mainForm.setSummary(result + summary);
            }
            
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Windows.Forms;

namespace WinSearchDBAnalyzer
{
    public class ListViewItemComparer : IComparer
    {
        private int column;
        public string sort = "asc";
        public ListViewItemComparer()
        {
            column = 0;
        }
        public ListViewItemComparer(int column, string sort)
        {
            this.column = column;
            this.sort = sort;
        }

        public int Compare(object x, object y)
        {
            if (!(x is ListViewItem))
                return (0);
            if (!(y is ListViewItem))
                return (0);

            ListViewItem l1 = (ListViewItem)x;
            ListViewItem l2 = (ListViewItem)y;

            if (l1.ListView.Columns[column].Tag == null)
            {
                l1.ListView.Columns[column].Tag = "Text";
            }
            if (l1.SubItems[column] == null && l2.SubItems[column] == null)
            {
                return (0);
            }
            else if (l1.SubItems[column] == null)
            {
                return (-1);
            }
            else if (l2.SubItems[column] == null)
            {
                return (1);
            }

            if (l1.ListView.Columns[column].Tag.ToString() == "Numeric")
            {
                float fl1 = float.Parse(l1.SubItems[column].Text);
                float fl2 = float.Parse(l2.SubItems[column].Text);

                if (sort == "asc")
                {
                    return fl1.CompareTo(fl2);
                }
                else
                {
                    return fl2.CompareTo(fl1);
                }
            }
            else
            {
                string str1 = l1.SubItems[column].Text;
                string str2 = l2.SubItems[column].Text;

                if (sort == "asc")
                {
                    return str1.CompareTo(str2);
                }
                else
                {
                    return str2.CompareTo(str1);
                }
            }

            /*

            if (sort == "asc")
                return String.Compare(((ListViewItem)x).SubItems[col].Text, ((ListViewItem)y).SubItems[col].Text);
            else
                return String.Compare(((ListViewItem)y).SubItems[col].Text, ((ListViewItem)x).SubItems[col].Text);
             * */
        }
    }
}

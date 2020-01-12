using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WinSearchDBAnalyzer
{
    public class MyFile
    {
        public int parentId;
        public int id;
        public string lastModified;
        public int priority;
        public string name;
        public List<string> fileInformationList;
        public string summary;
        public bool isFolder;

        public MyFile()
        {
            this.fileInformationList = new List<string>();
            this.isFolder = false;
            this.summary = "";
            this.name = "";
        }

        public ListViewItem getListViewItem()
        {
            ListViewItem item = new ListViewItem();
            if (isFolder)
            {
                item.ImageIndex = 0;
            }
            item.SubItems.Add(name);
            item.SubItems.Add(lastModified);
            foreach (string data in this.fileInformationList)
            {
                item.SubItems.Add(data);
            }
            return item;
        }
    }
}

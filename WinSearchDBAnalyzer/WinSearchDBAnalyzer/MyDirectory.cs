using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WinSearchDBAnalyzer
{
    public class MyDirectory
    {
        public int id;
        public int parentId;
        public string name;
        public Dictionary<int, MyFile> fileDictonary = new Dictionary<int, MyFile>();        

        public MyDirectory()
        {
        }

        
        public void addFile(int id, MyFile myFile)
        {
            if(!fileDictonary.Keys.Contains(id))
                fileDictonary.Add(id, myFile);
        }

        public List<ListViewItem> getFileList()
        {
            List<ListViewItem> fileList = new List<ListViewItem>();
            foreach(int key in fileDictonary.Keys)
            {
                fileList.Add(fileDictonary[key].getListViewItem());
            }
            return fileList;
        }

        public int getFileCount()
        {
            return fileDictonary.Count;
        }
    }
}

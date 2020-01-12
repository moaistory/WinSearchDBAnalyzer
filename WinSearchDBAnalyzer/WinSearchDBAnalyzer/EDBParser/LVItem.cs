using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WinSearchDBAnalyzer.EDBParser
{
    public class LVItem
    {
        private HexReader hexReader;
        private int lvPtrNumber;
        private int totalSize;
        private List<Item> dataList;
        private bool isWin10;
        public LVItem(){}
        public LVItem(HexReader hexReader, int lvPtrNumber, int totalSize){
	        this.hexReader = hexReader;
	        this.lvPtrNumber = lvPtrNumber;
	        this.totalSize = totalSize;
	        this.dataList = new List<Item>();
            this.isWin10 = true;
        }

        public void setWin7()
        {
            this.isWin10 = false;
        }

        public void addData(Item item){	        
	        this.dataList.Add(item);
        }

        public void setColumnNameAndType(string columnName, ColType.TYPE type)
        {
            foreach (Item item in this.dataList)
            {
                item.setColumnName(columnName);
                item.setColumnType(type);
            }
        }

        public string getData(int id, ColType.TYPE type){
	        string value = "";
            foreach (Item item in this.dataList){			
		        item.setIdType(id, type);
                if (item.getColumnName().Contains("System_Search_AutoSummary"))
                {
                    if (this.isWin10)
                    {
                        value += item.getDecompressText();
                    }
                    else
                    {
                        value += (item.getDeobfuscation());
                    }
                }
                else
                {
                    value += item.getValue();
                }
	        }
	        return value;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using System.Globalization;
using System.Web;
using System.Net;
using System.Drawing;
namespace WinSearchDBAnalyzer.EDBParser
{
    public class EDBParserManager
    {
        private string inputPath;

	    private int signature;
	    private int pagesize;
	    private int version;
	    private int revision;
        private String UTCAddHour = "0";
        private String UTCAddMinute = "0";
        private HexReader hexReader = null;
	    private MSysObjects mSysObjects;
	    private Dictionary<int, Table> tableDict;
        private Dictionary<int, Table> lvTableDict;
        private MainForm mainForm;
        private BackgroundWorker backgroudWoker;
        private WaitForm waitForm;
        private bool isWin10 = true;
        Table systemIndex_GthrPthTable;
        Table systemIndex_GthrTable;
        Table systemIndex_PropertyStoreTable;

        //private FormSave formSave;
        private Dictionary<String, String> tableNameDict;

        public EDBParserManager(MainForm mainForm)
        {
            this.mainForm = mainForm;
        }

        public void init(string inputPath){
	        this.inputPath = inputPath;
	        this.hexReader = new HexReader(inputPath);
	        this.parseDatabaseHeader();
	        this.mSysObjects = new MSysObjects(this.hexReader, this.pagesize);
            this.tableNameDict = new Dictionary<string, string>();
            this.isWin10 = true;
        }

        public void setWin7()
        {
            this.isWin10 = false;
        }

        public void readRecordsRunWorkerCompletedEventHandler(object sender, RunWorkerCompletedEventArgs e)
        {
            Tuple<Table, List<ListViewItem>> result = (Tuple<Table, List<ListViewItem>>)e.Result;
            List<ListViewItem> listViewItemList = (List<ListViewItem>)result.Item2;
            //mainForm.setRecordListMethod(listViewItemList);
            mainForm.Invoke(mainForm.setRecordListDelegate, new object[] { listViewItemList });
            waitForm.Close();
        }

        public void readRecordsDoWork(object sender, DoWorkEventArgs e)
        {
            
            Tuple<Table, bool> tableData = (Tuple<Table, bool>)e.Argument;
            Table table = tableData.Item1;
            bool status = tableData.Item2;
            List<ListViewItem> listVIewItemList = new List<ListViewItem>();
            List<Dictionary<int, Item>> recordList;

            if (status == true)//normal
            {
                this.waitForm.end("");
                this.waitForm.setLabel("Parse : " + table.getTableName());
                //table.parseRecord();
                recordList = table.getRecordList();
            }
            else
            {
                this.waitForm.end("");
                this.waitForm.setLabel("Carve : " + table.getTableName());
                
                //table.carveRecord();
                recordList = table.getDeletedRecordList();
            }
            int totalSize = recordList.Count;
            waitForm.maxProgress(totalSize);
            int longValueNumber = table.getLongValueNumber();
            if (longValueNumber != -1)
            {
                if (lvTableDict.ContainsKey(longValueNumber))
                {
                    Table lvTable = lvTableDict[longValueNumber];
                    Dictionary<int, LVItem> lvItems = lvTable.parseLVItems();
                    table.setLVItemDict(lvItems);
                }
            }
            if (recordList.Count == 0)
            {
                e.Result =listVIewItemList;
            }
            this.waitForm.end("");
            List<Column> columnList = table.getColumnList();
            
            int count = 0;
            foreach (Dictionary<int, Item> record in recordList)
            {
                if (backgroudWoker.CancellationPending == true)
                {
                    break;
                }
                ++count;
                if (count % 1000 == 0 || recordList.Count == count)
                    waitForm.setPrgress(count); 
                ListViewItem listVIewItem = new ListViewItem();
                foreach (Column column in columnList)
                {
                    if (column.getIgnore())
                        continue;
                    int columnID = column.getID();
                    if (!record.ContainsKey(columnID))
                    {
                        listVIewItem.SubItems.Add("");
                    }
                    else
                    {
                        Item item = record[columnID];
                        if (!isWin10)
                            item.setWin7();
                        int recordID = item.getID();
                        int recordSize = item.getSize();
                        if (recordID > 0 && recordID <= 0x80)
                        {
                            listVIewItem.SubItems.Add(item.getValue());
                        }
                        else if (recordID >= 0x80 && recordID < 0x100)
                        {
                            if (recordSize < 0 || recordSize >= 256)
                            {
                                listVIewItem.SubItems.Add("Parsig Error : fixed size item");
                            }
                            else
                            {
                                listVIewItem.SubItems.Add(item.getValue());
                            }
                        }
                        else if (recordID >= 0x100 && recordID < 0xFFFF)
                        {
                            if (recordSize < 0 || recordSize >= 65535)
                            {
                                listVIewItem.SubItems.Add("Parsig Error : variable size item");
                            }
                            else
                            {
                                byte tagFlag = item.getTaggedDataItemFlag();
                                if (tagFlag == 0 || tagFlag == 1)
                                { //common
                                    listVIewItem.SubItems.Add(item.getValue());
                                }
                                else if (tagFlag == 3)
                                { //compress text
                                    listVIewItem.SubItems.Add(item.getDecompressText());
                                }
                                else
                                { //Pointer
                                    int pointerNumber = item.getPointNumber();
                                    Dictionary<int, LVItem> lvItemDict = table.getLvItemDict();
                                    if (lvItemDict.ContainsKey(pointerNumber))
                                    {
                                        lvItemDict[pointerNumber].setColumnNameAndType(column.getName(), (ColType.TYPE)column.getType());
                                        if (!this.isWin10) lvItemDict[pointerNumber].setWin7();
                                        listVIewItem.SubItems.Add(lvItemDict[pointerNumber].getData(item.getID(), item.getType()));
                                    }
                                    else
                                    {
                                        listVIewItem.SubItems.Add("Parsig Error : lV pointer");
                                    }
                                }
                            }
                        }
                    }
                }
                listVIewItemList.Add(listVIewItem);
            }
            this.waitForm.end("");

            this.waitForm.setLabel("Complete : " + table.getTableName());
            
            var result = new Tuple<Table, List<ListViewItem>>(table, listVIewItemList);
            e.Result = result;

        }

        public void waitFormClosingEventHandler(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                backgroudWoker.CancelAsync();
            }
        }

        public void colse()
        {
            if (this.hexReader != null)
            {
                this.hexReader.close();
                this.hexReader = null;
            }
        }

        public byte[] hextoByte(string hexString)
        {
            int len = hexString.Length / 2;
            byte[] data = new Byte[len];
            for (int i = 0; i < len; ++i)
            {
                string toParse = hexString.Substring(i * 2, 2);
                data[i] = byte.Parse(toParse, NumberStyles.HexNumber);
            }
            return data;
        }

        public bool parseDatabaseHeader(){
	        DatabaseHeader databaseHeader = new DatabaseHeader(this.hexReader);
	        databaseHeader.parseHeaderArea();
	        this.signature = databaseHeader.getSignature();
	        if(this.signature != -1985229329){
                return false;
	        }
	        this.pagesize = databaseHeader.getPagesize();
	        this.version = databaseHeader.getVersion();
	        this.revision = databaseHeader.getRevision();
            return true;
        }

        public void makeTable()
        {
            this.mSysObjects.makeTables();
            this.lvTableDict = this.mSysObjects.getLvTableDict();

            this.tableDict = new Dictionary<int, Table>();
            this.systemIndex_GthrPthTable = mSysObjects.getSystemIndex_GthrPthTable();
            this.systemIndex_GthrTable = mSysObjects.getSystemIndex_GthrTable();
            this.systemIndex_PropertyStoreTable = mSysObjects.getSystemIndex_PropertyStoreTable();
            this.tableDict.Add(systemIndex_GthrPthTable.getTableNumber(), systemIndex_GthrPthTable);
            this.tableDict.Add(systemIndex_GthrTable.getTableNumber(), systemIndex_GthrTable);
            this.tableDict.Add(systemIndex_PropertyStoreTable.getTableNumber(), systemIndex_PropertyStoreTable);
        }

        public Dictionary<int,Table> getTableDict(){
            return this.tableDict;
        }

        public Table getTable(int tableNumber)
        {
            if(tableDict.ContainsKey(tableNumber))
                return this.tableDict[tableNumber];
            else
                return null;
        }

        public void parseRecord(Table table, bool status)
        {
            backgroudWoker = new BackgroundWorker();
            backgroudWoker.DoWork += new DoWorkEventHandler(readRecordsDoWork);
            backgroudWoker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(readRecordsRunWorkerCompletedEventHandler);
            backgroudWoker.WorkerSupportsCancellation = true;
            waitForm = new WaitForm();
            waitForm.setStyle(ProgressBarStyle.Blocks);
            waitForm.setLabel("Please wait while loading records");
            waitForm.FormClosing += new FormClosingEventHandler(waitFormClosingEventHandler);
            backgroudWoker.RunWorkerAsync(new Tuple<Table, bool>(table, status));
            waitForm.ShowDialog();
        }

        public void findTablePage()
        {
	        int totalPageCount = (int)this.hexReader.getFileSize() / this.pagesize;
	        int offset = 0;
	        int tableNumber = 0;
	        int pageFlags = 0;
	        for(int curPageNum = 1; curPageNum<totalPageCount-1; curPageNum++){
		        offset = (curPageNum + 1) * this.pagesize;
		        pageFlags = this.hexReader.readInt(offset + 36);
		        if((pageFlags & 0x20) == 0x20){ //space page flag
			        continue;
		        }else if((pageFlags & 0x40) == 0x40){ //index page flag
			        continue;
		        }else if((pageFlags & 0x80) == 0x80){ //int value page flag
			        continue;
		        }
		        if ((pageFlags & 0x00000004) != 0x00000004){ // branch page flag
			        if ((pageFlags & 0x000000EF) != 0x00000001){ // branch page flag
				        if((pageFlags & 0x08) != 0x08){ //empty page flag
					        continue;
				        }
			        }
		        }
		        tableNumber = this.hexReader.readInt(offset + 24);
		        if(this.tableDict.ContainsKey(tableNumber)){
                    Table table = this.tableDict[tableNumber];
			        table.addDeletedPageSet(curPageNum);
		        }
	        }
        }

        public void setUTCTime(String UTCAddHour, String UTCAddMinute)
        {
            this.UTCAddHour = UTCAddHour;
            this.UTCAddMinute = UTCAddMinute;
            this.mSysObjects.setUTCTime(UTCAddHour, UTCAddMinute);
        }

        public Table getSystemIndex_GthrPthTable()
        {
            return this.systemIndex_GthrPthTable;
        }

        public Table getSystemIndex_GthrTable()
        {
            return this.systemIndex_GthrTable;
        }

        public Table getSystemIndex_PropertyStoreTable()
        {
            return this.systemIndex_PropertyStoreTable;
        }
    }
}

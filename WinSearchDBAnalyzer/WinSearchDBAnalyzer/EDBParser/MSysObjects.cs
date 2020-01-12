using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WinSearchDBAnalyzer.EDBParser
{
    public class MSysObjects
    {
        private Table tableMsysObject;
        private HexReader hexReader;
        private int pagesize;
        private Dictionary<int, Table> tableDict;
        private Dictionary<int, Table> lvTableDict;
        private String UTCAddHour = "0";
        private String UTCAddMinute = "0";
        private Table systemIndex_GthrTable;
        private Table systemIndex_GthrPthTable;
        private Table systemIndex_PropertyStoreTable;
        public MSysObjects(HexReader hexReader, int pagesize){
	        this.pagesize = pagesize;
	        this.tableMsysObject = null;
	        this.hexReader = hexReader;
	        this.tableDict = new Dictionary<int, Table>();
	        this.lvTableDict = new Dictionary<int, Table>();
        }

        public void setMSysObjectColumns(){
	        this.tableMsysObject.addColumn(new Column(1, 4, 4, "ObjidTable"));
            this.tableMsysObject.addColumn(new Column(2, 3, 2, "Type"));
            this.tableMsysObject.addColumn(new Column(3, 4, 4, "Id"));
            this.tableMsysObject.addColumn(new Column(4, 4, 4, "ColtypOrPgnoFDP"));
            this.tableMsysObject.addColumn(new Column(5, 4, 4, "SpaceUsage"));
            this.tableMsysObject.addColumn(new Column(6, 4, 4, "Flags"));
            this.tableMsysObject.addColumn(new Column(7, 4, 4, "PagesOrLocale"));
            this.tableMsysObject.addColumn(new Column(8, 1, 1, "RootFlag"));
            this.tableMsysObject.addColumn(new Column(9, 3, 2, "RecordOffset"));
            this.tableMsysObject.addColumn(new Column(10, 4, 4, "LCMapFlags"));
            this.tableMsysObject.addColumn(new Column(11, 17, 2, "KeyMost"));
            this.tableMsysObject.addColumn(new Column(128, 10, 255, "Name"));
            this.tableMsysObject.addColumn(new Column(129, 9, 255, "Stats"));
            this.tableMsysObject.addColumn(new Column(130, 10, 255, "TemplateTable"));
            this.tableMsysObject.addColumn(new Column(131, 9, 255, "DefaultValue"));
            this.tableMsysObject.addColumn(new Column(132, 9, 255, "KeyFldIDs"));
        }

        public void makeTables()
        {
	        this.tableMsysObject = new Table(this.hexReader, this.pagesize, "MsysObject", 2, 4);
            this.tableMsysObject.setUTCTime(UTCAddHour, UTCAddMinute);
	        this.setMSysObjectColumns();
	        this.tableMsysObject.init();
	        List<Dictionary<int, Item>> recordList = this.tableMsysObject.parseRecord();
	
	        foreach(Dictionary<int, Item> record in recordList){
		        int objidTable = Int32.Parse(record[1].getValue());
		        short type = Int16.Parse(record[2].getValue());
		        int id = Int32.Parse(record[3].getValue());
		        int coltypOrPgnoFDP = Int32.Parse(record[4].getValue());
		        int spaceUsage = Int32.Parse(record[5].getValue());
                string name = record[128].getValue().Replace("\0", string.Empty); 
		        
		        if(type==1){
			        if(!this.tableDict.ContainsKey(objidTable)){ //table가 존재하지 않음
				        Table table = new Table(this.hexReader, this.pagesize, name, id, coltypOrPgnoFDP);
                        table.setUTCTime(UTCAddHour, UTCAddMinute);
				        table.init();
                        if (table.getTableName().Equals("SystemIndex_Gthr"))
                        {
                            this.systemIndex_GthrTable = table;
                        }
                        else if (table.getTableName().Equals("SystemIndex_GthrPth"))
                        {
                            this.systemIndex_GthrPthTable = table;
                        }
                        else if (table.getTableName().Equals("SystemIndex_PropertyStore") || table.getTableName().Equals("SystemIndex_0A"))
                        {
                            this.systemIndex_PropertyStoreTable = table;
                        }
				        this.tableDict.Add(id, table);
			        }
		        }else if(type==2){
			        Column column = new Column(id, coltypOrPgnoFDP, spaceUsage, name);
			        if(objidTable == 2){ //MSysObject Table
				        this.tableMsysObject.addColumn(column);
			        }
			        this.tableDict[objidTable].addColumn(column);	
		        }else if(type==4){
			        if(this.tableDict.ContainsKey(objidTable)){
				        this.tableDict[objidTable].setLongValueNumber(id);
			        }
			        Table lvTable = new Table(this.hexReader, this.pagesize, name, id, coltypOrPgnoFDP);
                    lvTable.setUTCTime(UTCAddHour, UTCAddMinute);
			        lvTable.init();
			        this.lvTableDict.Add(id, lvTable);
		        }
	        }
	        return;
        }

        public Dictionary<int, Table> getTableDict(){
	        return this.tableDict;
        }

        public Dictionary<int, Table> getLvTableDict(){
	        return this.lvTableDict;
        }

        public void setUTCTime(String UTCAddHour, String UTCAddMinute)
        {
            this.UTCAddHour = UTCAddHour;
            this.UTCAddMinute = UTCAddMinute;
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

        public void recoveryDeletedTable()
        {
            this.tableMsysObject.carveRecord();
        }


    }
}

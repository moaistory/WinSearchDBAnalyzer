using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WinSearchDBAnalyzer.EDBParser
{
    public class Table
    {
        public const int OFFSET_PREPAGENUMBER = 0x10;
        public const int  OFFSET_NEXTPAGENUMBER = 0x14;
        public const int  OFFSETAVAILABLEPAGETAG = 0x22;
        public const int  OFFSET_PAGEFLAG = 0x24;
        private String UTCAddHour = "0";
        private String UTCAddMinute = "0";
        private List<Column> columnList;
        private List<String> columnNameList;
        private List<int> pageList;
        private HashSet<int> deletedPageSet;
        private List<Dictionary<int, Item>> recordList;
        private List<Dictionary<int, Item>> deletedRecordList;
        private Dictionary<int, LVItem> lvItemDict;
        private HexReader hexReader;
        private int pagesize;
        private int pageHeaderSize;
        private bool isWin10 = true;
        private string tableName;
        private int tableNumber;
        private int tableFDP;
        private int longValueNumber;
        private int lastColumnFixedItemID;
        private int lastColumnVariableItemID;
        private int lastColumnTaggedDataItemID;
        private MainForm mainForm;
        public Table(HexReader hexReader, int pagesize, string tableName, int tableNumber, int tableFDP){
	        this.hexReader = hexReader;
	        this.pagesize = pagesize;
	        if(this.pagesize >= 0x4000){
		        this.pageHeaderSize = 0x50;
	        }else{
		        this.pageHeaderSize = 0x28;
	        }
	        this.tableName = tableName;
	        this.tableNumber = tableNumber;
	        this.tableFDP = tableFDP;
	        this.longValueNumber = -1; //없음
	        this.columnList = new List<Column>();
            this.columnNameList = new List<String>();
	        this.recordList = new List<Dictionary<int, Item>>();
	        this.deletedRecordList = new List<Dictionary<int, Item>>();
	        this.lvItemDict = new Dictionary<int, LVItem>();
	        this.pageList = new List<int>();
	        this.deletedPageSet = new HashSet<int>();
	        this.lastColumnFixedItemID = 0;
	        this.lastColumnVariableItemID = 0;
	        this.lastColumnTaggedDataItemID = 0;
            this.isWin10 = true;
        }

        public void setMainForm(MainForm mainForm)
        {
            this.mainForm = mainForm;
        }

        public int getTableNumber()
        {
            return this.tableNumber;
        }

        public void setWin7()
        {
            this.isWin10 = false;
        }

        public int getPageFlags(int pageNumber)
        {
            int offset = (pageNumber + 1) * this.pagesize + OFFSET_PAGEFLAG;
            return this.hexReader.readInt(offset);
        }

        public int getPrePageNumber(int pageNumber)
        {
            int offset = (pageNumber + 1) * this.pagesize + OFFSET_PREPAGENUMBER;
            return this.hexReader.readInt(offset);
        }

        public int getNextPageNumber(int pageNumber)
        {
            int offset = (pageNumber + 1) * this.pagesize + OFFSET_NEXTPAGENUMBER;
            return this.hexReader.readInt(offset);
        }

        public short getAvailablePageTag(int pageNumber)
        {
            int offset = (pageNumber + 1) * this.pagesize + OFFSETAVAILABLEPAGETAG;
            if (offset < 0) return -1;
            return this.hexReader.readShort(offset);
        }

        public bool isBranchPage(int pageFlags)
        {
            if ((pageFlags & 0x00000004) == 0x00000004) return true;
            if ((pageFlags & 0x000000EF) == 0x00000001) return true;
            return false;
        }

        public short getRecordSize(int pageNumber, int tagNumber)
        {
            int offset = (pageNumber + 2) * this.pagesize - (tagNumber + 1) * 4;
            return this.hexReader.readShort(offset);
        }

        public short getRecordOffset(int pageNumber, int tagNumber)
        {
            int offset = (pageNumber + 2) * this.pagesize - (tagNumber + 1) * 4 + 2;
            return this.hexReader.readShort(offset);
        }

        public byte getPageTagFlags(int pageNumber, int tagNumber)
        {
            int offset = (pageNumber + 2) * this.pagesize - (tagNumber + 1) * 4 + 3;
            return this.hexReader.readByteDump(offset,1)[0];
        }

        public int getPageNumber(int recordSize, int recordOffset, byte pageTagFlags)
        {
            short pageKeySize = 0;
            int parsingOffset = 0;
            int pageNumber = 0;
            if (this.pagesize >= 0x4000)
            {
                pageTagFlags = this.hexReader.readByteDump(recordOffset + 1,1)[0];
            }
            if (isCFlags(pageTagFlags))
            {
                pageKeySize = this.hexReader.readShort(recordOffset + 2);
                parsingOffset = 4 + pageKeySize;
            }
            else
            {
                pageKeySize = (short)((int)this.hexReader.readShort(recordOffset) & 0x1FFF);
                parsingOffset = 2 + pageKeySize;
            }
            pageNumber = this.hexReader.readInt(recordOffset + parsingOffset);
            return pageNumber;
        }

        public bool isCFlags(byte pageTagFlag)
        {
            if ((pageTagFlag & 0x80) == 0x80)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool isDFlags(byte pageTagFlag)
        {
            if ((pageTagFlag & 0x40) == 0x40)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void setintValueNumber(int longValueNumber)
        {
            this.longValueNumber = longValueNumber;
        }

        
        public void setLongValueNumber(int longValueNumber){
	        this.longValueNumber = longValueNumber;
        }

        public List<int> parseBranchPage(int pageNumber){
	        List<int> branchPageList = new List<int>();
	        List<int> recordList = this.parseBranchRecord(pageNumber);


	        foreach(int record in recordList){
		        int childPageNumber = record;
		        int prePageNumber = getPrePageNumber(childPageNumber);
		        int pageFlags = 0;

                
			    pageFlags = this.getPageFlags(childPageNumber);
			    if(this.isBranchPage(pageFlags)){
				    List<int> childBranchPageList = parseBranchPage(childPageNumber);
				    branchPageList.AddRange(childBranchPageList);
			    }else{
				    branchPageList.Add(childPageNumber);
			    }
		        
	        }
	        return branchPageList;
        }

        public List<int> parseBranchRecord(int parsingPageNumber){
	        short availablePageTag = this.getAvailablePageTag(parsingPageNumber);
	        List<int> recordList = new List<int>();
	        short recordSize = 0;
	        int recordOffset = 0;
	        int tempPageNumber = 0;
	        byte pageTagFlags = 0;
	        for(int tagNumber = 1; tagNumber<availablePageTag; tagNumber++){
		        recordSize = (short)((int)this.getRecordSize(parsingPageNumber, tagNumber) & (this.pagesize-1));
		        recordOffset = (parsingPageNumber+1) * this.pagesize + this.pageHeaderSize;
		        recordOffset += this.getRecordOffset(parsingPageNumber, tagNumber) & (this.pagesize-1);
		        pageTagFlags = 0;
		        if(this.pagesize < 0x4000){
			        pageTagFlags = this.getPageTagFlags(parsingPageNumber, tagNumber);
		        }		
		        tempPageNumber = getPageNumber(recordSize, recordOffset, pageTagFlags);
		        recordList.Add(tempPageNumber);
	        }
	        return recordList;
        }

        /***** public method *****/
        public void init(){
	        int pageFlags = this.getPageFlags(this.tableFDP);
	        if(this.isBranchPage(pageFlags)){ // 트리 구조
		        this.pageList = parseBranchPage(this.tableFDP);
	        }else{
		        this.pageList = new List<int>(); //단일 노드(트리레벨 : 1)
		        this.pageList.Add(this.tableFDP);
	        }
        }

        
        public void setParsingColumn(List<string> parsingColumnNameList)
        {
            for (int i = 0; i < this.columnNameList.Count; i++)
            {
                this.columnList[i].setIgnore(true);
                for (int j = 0; j < parsingColumnNameList.Count; j++)
                {
                    if (this.columnNameList[i].Contains(parsingColumnNameList[j]))
                    {
                        this.columnList[i].setIgnore(false);
                        break;
                    }
                }
            }
        }

        public List<string> getParsingColumn()
        {
            List<string> parsingColumnList = new List<string>();
            for (int i = 1; i < this.columnList.Count; i++)
            {
                if (columnList[i].getIgnore() == false)
                {
                    parsingColumnList.Add(columnNameList[i]);
                }
            }
            return parsingColumnList;
        }

        public void addColumn(Column column){
	        int columnID = column.getID();
	        if(columnID >= 0 && columnID < 0x80){
		        if(columnID > this.lastColumnFixedItemID){
			        this.lastColumnFixedItemID = columnID;
		        }
	        }else if(columnID >= 0x80 && columnID < 0x100){
		        if(columnID > this.lastColumnVariableItemID){
			        this.lastColumnVariableItemID = columnID;
		        }
	        }else{
		        if(columnID > this.lastColumnTaggedDataItemID){
			        this.lastColumnTaggedDataItemID = columnID;
		        }
	        }
            this.columnNameList.Add(column.getName());
	        this.columnList.Add(column);
        }

        public Dictionary<int, Item> getRecord(int recordOffset, int recordSize, int jumpSize){
	        //레코드 파싱 시작
	        Dictionary<int, Item> record = new Dictionary<int, Item>();
	        int parsingOffset = recordOffset + jumpSize;
	        byte lastFixedItemID = this.hexReader.readByteDump(parsingOffset,1)[0];
	        byte lastVariableItemID = this.hexReader.readByteDump(parsingOffset+1,1)[0];
	        short firstVariableIDOffset = this.hexReader.readShort(parsingOffset+2);
	        short firstVariableDataOffset = (short)((lastVariableItemID-0x80+1) * 2);
	        short firstTaggedIDOffset = firstVariableIDOffset; 
	        short firstTaggedDataOffset = 0;
				
	
	
	        int id = 0;
            string columnName = "";
	        ColType.TYPE type = ColType.TYPE.UNKNOWN_TYPE;
            int spaceUsage = 0;
	        int itemOffset = 0;
	        int itemSize = 0;
	        short itemID = 0;
	        int fixedItemPosition = 4;
	        int variableItemPosition = 0;
	        int taggedDataItemPosition = 0;

	        short variableItemOffset = 0;
	        short nextVariableItemOffset = 0;
	        short taggedDataItemOffset = 0;
	        short nextTaggedDataItemOffset = 0;
	        //하나의 row에 있는 여러 항목 중 각 칼럼에 해당하는 항목 파싱
	        foreach(Column column in columnList){
		        id = column.getID();
		        type = (ColType.TYPE)column.getType();
                spaceUsage = column.getSpaceUsage();
                columnName = column.getName();
		        //Item item;
					
		        if(id>=0 && id < 0x80 && id<=lastFixedItemID){//fixed items 파싱

                    itemOffset = parsingOffset + fixedItemPosition;
                    itemSize = column.getSpaceUsage();
                    fixedItemPosition += itemSize;
                    if (!column.getIgnore())
                    {
                        Item item = new Item(this.hexReader, id, columnName, type, spaceUsage, itemOffset, itemSize);
                        item.UTCTime(UTCAddHour, UTCAddMinute);
                        record.Add(id, item);
                    }
		        }else if(id>=0x80 && id<0x100 && id<=lastVariableItemID){//variable items 파싱
			        nextVariableItemOffset = this.hexReader.readShort(parsingOffset + firstVariableIDOffset + variableItemPosition);
			        if(nextVariableItemOffset>=0){
				        itemOffset = parsingOffset + firstVariableIDOffset + firstVariableDataOffset + variableItemOffset;
				        itemSize = nextVariableItemOffset - variableItemOffset;
                        variableItemOffset = nextVariableItemOffset;
                        if (id == lastVariableItemID)
                        {
                            firstTaggedIDOffset = (short)(firstVariableIDOffset + firstVariableDataOffset + nextVariableItemOffset);
                        }

                        if (!column.getIgnore())
                        {
                            Item item = new Item(this.hexReader, id, columnName, type, spaceUsage, itemOffset, itemSize);
                            item.UTCTime(UTCAddHour, UTCAddMinute);
                            record.Add(id, item);
                        }
			        }
			        variableItemPosition += 2;
		        }else if(id>=0x100 && id < 0xFFFF){//tagged data items 파싱
                    taggedDataItemOffset = this.hexReader.readShort(parsingOffset + firstTaggedIDOffset + taggedDataItemPosition + 2);
                    itemID = this.hexReader.readShort(parsingOffset + firstTaggedIDOffset + taggedDataItemPosition);
                    if (itemID == id)
                    {
                        if (firstTaggedDataOffset == 0)
                        { //First
                            firstTaggedDataOffset = this.hexReader.readShort(parsingOffset + firstTaggedIDOffset + 2);
                        }
                        taggedDataItemPosition += 4;
                        itemOffset = parsingOffset + firstTaggedIDOffset + taggedDataItemOffset;
                        if (firstTaggedDataOffset == taggedDataItemPosition)
                        { //Last	
                            itemSize = recordSize - (jumpSize + firstTaggedIDOffset + taggedDataItemOffset);
                        }
                        else
                        {
                            nextTaggedDataItemOffset = this.hexReader.readShort(parsingOffset + firstTaggedIDOffset + taggedDataItemPosition + 2);
                            itemSize = nextTaggedDataItemOffset - taggedDataItemOffset;
                        }
                        if (!column.getIgnore())
                        {
                            Item item = new Item(this.hexReader, id, columnName, type, spaceUsage, itemOffset, itemSize);
                            item.UTCTime(UTCAddHour, UTCAddMinute);
                            record.Add(id, item);
                        }
                    }
                    
		        }
		        if((itemOffset+itemSize) - recordOffset >= recordSize){
			        break;
		        }
	        }
	        return record;
        }


        public int getPageCount()
        {
            return this.pageList.Count;
        }
        public List<Dictionary<int, Item>> parseRecord(){
	        //테이블에 속한 모든 페이지를 파싱함
            bool[] pageArray = new bool[1000000];
            Array.Clear(pageArray, 0, pageArray.Length);
            int parsePageCnt = 0;
	        foreach(int firstParsingPageNumber in pageList){
                
		        //각 페이지에는 트리구조로 되어있음
		        //아버지노드가 같고 같은 높이의 페이지는 이전 페이지번호와 다음 페이지 번호가 담겨있음. 
		        //없을 경우 0으로 되어있음
                int parsingPageNumber = firstParsingPageNumber;
		        while(parsingPageNumber != 0){
                    if (pageArray[parsingPageNumber] == false)
                        pageArray[parsingPageNumber] = true;
                    else
                        break;

                    //availablePageTag는 페이지 내에 있는 레코드의 개수를 나타냄
                    short availablePageTag = this.getAvailablePageTag(parsingPageNumber);
                    if (availablePageTag < 0) break;
			        //첫번째 태그는 레코드의 오프셋과 크기를 나타내지 않음. 그래서 0이 아닌 1부터 시작
			        int pageDataOffset = (parsingPageNumber+1) * this.pagesize + this.pageHeaderSize;
                    for (int tagNumber = 1; tagNumber<availablePageTag; tagNumber++){
				        byte pageTagFlags = 0;
				        short pageKeySize = 0;
				        int recordOffset =  pageDataOffset + (this.getRecordOffset(parsingPageNumber, tagNumber) & (this.pagesize-1));
				        short recordSize = (short)(this.getRecordSize(parsingPageNumber, tagNumber) & (this.pagesize-1));
				        int parsingOffset = recordOffset;
				        int jumpSize = 0;

				        if(this.pagesize >= 0x4000){
					        pageTagFlags = this.hexReader.readByteDump(parsingOffset+1,1)[0];
				        }else{
					        pageTagFlags = this.getPageTagFlags(parsingPageNumber, tagNumber);
				        }
                        
				        if(isDFlags(pageTagFlags)){ 
					        //D플래그가 되어 있으면 쓰이지 않는 레코드
					        continue; 
				        }
				        else if(isCFlags(pageTagFlags)){
					        //키 플래그 2바이트(실제로는 상위 3비트만 쓰임) + 키 사이즈 2바이트 => 4바이트
					        //C플래그가 되어 있으면 pageKeySize만큼 건너 뛰어야됨
					        pageKeySize = this.hexReader.readShort(parsingOffset+2);
					        jumpSize = 4 + pageKeySize; 
				        }else{
					        //키 플래그 3비트 + 키 사이즈 13비트 => 2바이트
					        //C또는 D플래그가 설정되어 있지 않다면 pageKeySize만큼 건너 뜀. 
					        pageKeySize = (short)(this.hexReader.readShort(parsingOffset) & 0x1FFF);
					        jumpSize = 2 + pageKeySize;
				        }
				        try{
					        Dictionary<int, Item> record = getRecord(recordOffset, recordSize, jumpSize);
					        this.recordList.Add(record);
				        }catch(Exception ex){
                            MessageBox.Show("Error : " + ex.Message.ToString());
				        }
			        }
                    if (mainForm != null)
                    {
                        parsePageCnt++;
                        if (parsePageCnt <= pageList.Count)
                        {
                            mainForm.changeProgress(parsePageCnt);
                        }
                    }
			        parsingPageNumber = getNextPageNumber(parsingPageNumber);
		        }
	        }
	        return this.recordList;
        }

        public Dictionary<int, LVItem> parseLVItems(){
            bool[] pageArray = new bool[1000000];
            Array.Clear(pageArray, 0, pageArray.Length);
	        foreach(int firstParsingPageNumber in pageList){
                int parsingPageNumber = firstParsingPageNumber;
		        while(parsingPageNumber != 0){
                    if (pageArray[parsingPageNumber] == false)
                        pageArray[parsingPageNumber] = true;
                    else
                        break;
			        short availablePageTag = this.getAvailablePageTag(parsingPageNumber);
                    if (availablePageTag < 0) break;
			        //일반 레코드와 다르게 LV는 첫번째 태그가 쓰임 따라서 0번부터 시작
			        int pageDataOffset = (parsingPageNumber+1) * this.pagesize + this.pageHeaderSize;
			        int firstLvNumber = 0;
			        int lvNumber = 0;
			        int lvTotalSize;

			        for(int tagNumber = 0; tagNumber < availablePageTag; tagNumber++){
				        byte pageTagFlags = 0;
				        short pageKeySize = 0;
				        int recordOffset =  pageDataOffset + (this.getRecordOffset(parsingPageNumber, tagNumber) & (this.pagesize-1));
                        
				        short recordSize = (short)(this.getRecordSize(parsingPageNumber, tagNumber) & (this.pagesize-1));
				        int parsingOffset = recordOffset;
				        int jumpSize = 0;
				
				        if(tagNumber == 0){
					        firstLvNumber = this.hexReader.readIntBigEndian(parsingOffset);
					        continue;
				        }

				        if(this.pagesize >= 0x4000){
					        pageTagFlags = this.hexReader.readByteDump(parsingOffset+1,1)[0];
				        }else{
					        pageTagFlags = this.getPageTagFlags(parsingPageNumber, tagNumber);
				        }


                        if(isDFlags(pageTagFlags)){ 
					        //D플래그가 되어 있으면 쓰이지 않는 레코드
                            continue; 
				        }
				        else if(isCFlags(pageTagFlags)){
					        //키 플래그 2바이트(실제로는 상위 3비트만 쓰임) + 키 사이즈 2바이트 => 4바이트
					        //C플래그가 되어 있으면 pageKeySize만큼 건너 뛰어야됨
					        pageKeySize = this.hexReader.readShort(parsingOffset+2);
					        jumpSize = 4; 
				        }else{
					        //키 플래그 3비트 + 키 사이즈 13비트 => 2바이트
					        //C또는 D플래그가 설정되어 있지 않다면 pageKeySize만큼 건너 뜀. 
					        pageKeySize = (short)(this.hexReader.readShort(parsingOffset) & 0x1FFF);
					        jumpSize = 2;
				        }
				        parsingOffset = recordOffset + jumpSize;
                        
				        if(recordSize == jumpSize + pageKeySize +8){
					        if(pageKeySize == 0){
						        lvNumber = firstLvNumber;
					        }else{
						        if(isCFlags(pageTagFlags)){
							        lvNumber = (short)((firstLvNumber & 0xFFFFFF00) | this.hexReader.readByteDump(parsingOffset,1)[0]);
						        }else{
							        lvNumber = this.hexReader.readIntBigEndian(parsingOffset);
						        }
					        }

					        parsingOffset += 4 + pageKeySize;
					        lvTotalSize = this.hexReader.readInt(parsingOffset);
					        LVItem lvItem = new LVItem(hexReader, lvNumber, lvTotalSize);
                            if (!this.isWin10)
                            {
                                lvItem.setWin7();
                            }
                            if (!lvItemDict.ContainsKey(lvNumber))
                            {
                                lvItemDict.Add(lvNumber, lvItem);
                            }
					        
				        }else{
					        parsingOffset += pageKeySize;
                            
					        if(lvItemDict.ContainsKey(lvNumber)){
                                Item item = new Item(hexReader, 0, "", ColType.TYPE.JET_coltypNil, 0, parsingOffset, recordSize - (parsingOffset - recordOffset));
                                item.UTCTime(UTCAddHour, UTCAddMinute);
						        lvItemDict[lvNumber].addData(item);
					        }
				        }
				
			        }
			        parsingPageNumber = getNextPageNumber(parsingPageNumber);
		        }
	        }
	        return this.lvItemDict;
        }
        
        public List<Dictionary<int, Item>> carveRecord(){
            int parsePageCnt = 0;
            bool[] bitmap = new bool[this.pagesize];
            bool[] pageArray = new bool[1000000];
            Array.Clear(pageArray, 0, pageArray.Length);
            //normal page
            foreach(int startPageNumber in pageList){
                int pageNumber =startPageNumber;
                while(pageNumber != 0){
                    if (pageArray[pageNumber] == false)
                        pageArray[pageNumber] = true;
                    else
                        break;
                    this.deletedPageSet.Remove(pageNumber);
                    short availablePageTag = this.getAvailablePageTag(pageNumber);
                    if (availablePageTag < 0) break;
                    Array.Clear(bitmap,0, this.pagesize);
			        
                    memset(bitmap, true, 0, this.pageHeaderSize); //page header area
                    memset(bitmap, true, (this.pagesize - availablePageTag*4), availablePageTag*4); //tag area
		
                    int pageDataOffset = (pageNumber+1) * this.pagesize + this.pageHeaderSize;
                    for(int tagNumber = 0; tagNumber < availablePageTag; tagNumber++){
                        int recordOffset = this.pageHeaderSize + this.getRecordOffset(pageNumber, tagNumber) & (this.pagesize-1);
                        short recordSize = (short)(this.getRecordSize(pageNumber, tagNumber) & (this.pagesize-1));
                        memset(bitmap, true, recordOffset, recordSize); //record area
                    }
                    findDeleteRecord(pageNumber , bitmap);
                    pageNumber = getNextPageNumber(pageNumber);
                    if (mainForm != null)
                    {
                        parsePageCnt++;
                        if (parsePageCnt <= pageList.Count)
                        {
                            mainForm.changeProgress(parsePageCnt);
                        }
                    }
                }
            }
            //deleted page
            foreach (int startPageNumber in deletedPageSet)
            {
                int pageNumber =startPageNumber;
                memset(bitmap, false, 0, this.pagesize);
                memset(bitmap, true, 0, this.pageHeaderSize); //page header area
                findDeleteRecord(pageNumber, bitmap);
            }

            return this.deletedRecordList;
        }
     
        public void memset(bool[] array, bool value, int index, int length)
        {
            for (int i = index; i < length; i++)
            {
                array[i] = value;
            }
        }
           
        void findDeleteRecord(int pageNumber, bool[] bitmap){	
            int pageOffset = (pageNumber+1) * this.pagesize;
            int curRecordOffset = -1;
            int curJumpSize = 0;
            int curRecordSize = 0;
            int maxOffset = this.pageHeaderSize;
            int minOffset = 0;
            Tuple<int,int> recordInfo = new Tuple<int, int>(0,0);
            int nextRecordOffset = 0;
            int nextJumpSize = 0;
            int firstVariableIDOffset = 0;
            //minOffset = findNoUsageOffset(bitmap, maxOffset);

            while(true){
                if(curRecordOffset < 0){
                    minOffset = findNoUsageOffset(bitmap, maxOffset);
                    while(minOffset > 0){
                        maxOffset = findUsageOffset(bitmap, minOffset+1);
                        recordInfo = findRecordStartOffset(bitmap, pageNumber, minOffset, maxOffset);
                        curRecordOffset = recordInfo.Item1;
                        curJumpSize = recordInfo.Item2;
                        if(curRecordOffset < 0){
                            minOffset = findNoUsageOffset(bitmap, maxOffset);
                        }else{
                            break;
                        }
                    }
                }
                if(minOffset < 0){
                    break;
                }
                firstVariableIDOffset = this.hexReader.readShort(pageOffset + curRecordOffset + curJumpSize + 2);
                recordInfo =findRecordStartOffset(bitmap, pageNumber, curRecordOffset + curJumpSize + firstVariableIDOffset, maxOffset);
                nextRecordOffset = recordInfo.Item1;
                nextJumpSize = recordInfo.Item2;
                if(nextRecordOffset < 0){ //Last
                    curRecordSize = maxOffset - curRecordOffset;
                    if (curRecordSize > 1000)
                    {
                        Byte[] data = hexReader.readByteDump(pageOffset + curRecordOffset + curJumpSize + firstVariableIDOffset, curRecordSize);
                        int tempSize = curRecordSize;
                        for(int i=0; i < curRecordSize-6; i+=6){
                            if (data[i] == 0x00 && data[i + 1] == 0x00 && data[i + 2] == 0x00 && data[i + 3] == 0x00 && data[i+4] == 0x00 && data[i + 5] == 0x00)
                            {
                                tempSize -= 6;
                            }
                        }
                        for (int i = tempSize; i < curRecordSize -6; i += 6)
                        {
                            if (data[i] == 0x00 && data[i + 1] == 0x00 && data[i + 2] == 0x00 && data[i + 3] == 0x00 && data[i + 4] == 0x00 && data[i + 5] == 0x00)
                            {
                                curRecordSize = i + +curJumpSize + firstVariableIDOffset + 1;
                                break;
                            }
                        }
                    }
                    

                }else{
                    curRecordSize = nextRecordOffset - curRecordOffset;
                }
                try{
                    
                    Dictionary<int, EDBParser.Item> record = this.getRecord(pageOffset + curRecordOffset, curRecordSize, curJumpSize);
                    
                    this.deletedRecordList.Add(record);
                }catch(Exception ex){
                    MessageBox.Show("Error : " + ex.Message);
                }
                curRecordOffset = nextRecordOffset;
                curJumpSize = nextJumpSize;
            }
        }
        
        //삭제된 레코드를 찾는 함수
        //비트맵을 통해 쓰이지 않는 공간을 확인
        //그리고 레코드 헤더를 찾음
        //메모리 속도 개선을 위해 Dictionary<int, Item>Info를 반환하는 것이 아닌 그 주소를 입력받고 내용을 수정함.
        public Tuple<int, int> findRecordStartOffset(bool[] bitmap, int pageNumber, int nowOffset, int maxOffset)
        {
            int parsingOffset = 0;
            byte pageTagFlags = 0;
            short pageKeySize = 0;
            int jumpSize = 0;
            byte lastFixedItemID;
            byte lastVariableItemID;
            short firstVariableIDOffset;
            int maxSize = 0;
            int fixedItemMaxSize = 0;
            //페이지 크기가 0x4000보다 작을 경우 pageTagFlags가 tag 영역에 있으므로 알 수 없다. 따라서 2번 확인해야 한다.
            int loopCount = 1;
            if(this.pagesize < 0x4000){
                loopCount = 2;
            }
            int pageOffset = (pageNumber+1) * this.pagesize;
            for(int i = nowOffset; i < maxOffset; i++){
                if(bitmap[i] == false){
                    for(int j = 0; j < loopCount; j++){
                        //첫번째 바이트는 0이 되어서는 안된다.
                        parsingOffset = pageOffset + i;

                        if(this.hexReader.readByteDump(parsingOffset,1)[0] == 0)
                            continue;

                        if(this.pagesize >= 0x4000){
                            pageTagFlags = this.hexReader.readByteDump(parsingOffset+1,1)[0];
                        }else{
                            if(loopCount == 1){
                                pageTagFlags = 0x80; //C플래그
                            }else{
                                pageTagFlags = 0x00;
                            }
                        }

                        if(isCFlags(pageTagFlags)){
                            pageKeySize = this.hexReader.readShort(parsingOffset+2);
                            jumpSize = 4 + pageKeySize; 
                        }else{
                            pageKeySize = (short)(this.hexReader.readShort(parsingOffset) & 0x1FFF);
                            jumpSize = 2 + pageKeySize;
                        }

                        maxSize = maxOffset - i;

                        if(pageKeySize < 0 || pageKeySize >= maxSize)
                            continue;

                        //100보다 큰 pageKeySize를 본적이 없다.
                        if(pageKeySize > 100)
                            continue;

                        parsingOffset += jumpSize;

                        lastFixedItemID = this.hexReader.readByteDump(parsingOffset,1)[0];
                        if(lastFixedItemID <= 0 || lastFixedItemID > this.lastColumnFixedItemID)
                            continue;

                        lastVariableItemID = this.hexReader.readByteDump(parsingOffset+1,1)[0];
				
                        if(this.lastColumnVariableItemID ==0 && lastVariableItemID != 127){
                                continue;
                        }
				
                        if(this.lastColumnVariableItemID !=0 && lastVariableItemID > this.lastColumnVariableItemID){
                                continue;
                        }
				
                        firstVariableIDOffset = this.hexReader.readShort(parsingOffset+2);
                        if(firstVariableIDOffset < 0 || firstVariableIDOffset > maxSize )
                            continue;

                        fixedItemMaxSize = getFixedItemMaxSize(lastFixedItemID);
                        if((firstVariableIDOffset - fixedItemMaxSize) > 7){
                            continue;
                        }

                        if((firstVariableIDOffset - fixedItemMaxSize) < 4){
                            continue;
                        }
				
                        return new Tuple<int, int>(i, jumpSize);
                    }
                }
            }
            return new Tuple<int, int>(-1, -1);
        }

        //쓰이지 않는 공간을 찾음. 페이지 끝까지 없으면 -1을 리턴
        int findNoUsageOffset(bool[] bitmap, int nowOffset)
        {
	        for(int i = nowOffset; i < this.pagesize; i++){
		        if(bitmap[i] == false){
			        return i;
		        }
	        }
	        return -1;
        }

        //쓰이는 공간을 찾음. 페이지 끝까지 없으면 this.pagesize을 리턴
        int findUsageOffset(bool[] bitmap, int nowOffset)
        {
	        for(int i = nowOffset; i < this.pagesize; i++){
		        if(bitmap[i] == true){
			        return i;
		        }
	        }
	        return this.pagesize;
        }

        public int getFixedItemMaxSize(int id){
	        int maxSize = 0;
	        
	        //하나의 row에 있는 여러 항목 중 각 칼럼에 해당하는 항목 파싱
	        foreach(Column column in columnList){
		        maxSize += column.getSpaceUsage();
		        if(column.getID() == id){
			        break;
		        }
	        }
	        return maxSize;
        }

        public string getTableName(){
	        return this.tableName;
        }

        public void setTableName(String tableName)
        {
            this.tableName = tableName;
        }


        public  List<Column> getColumnList(){
	        return this.columnList;
        }

        public  List<Dictionary<int, Item>> getRecordList(){
	        return this.recordList;
        }

        public int getRecordCount()
        {
            return this.recordList.Count;
        }

        public int getDeletedRecordCount()
        {
            return this.deletedRecordList.Count;
        }

        public  List<Dictionary<int, Item>> getDeletedRecordList(){
	        return this.deletedRecordList;
        }

        public int getLongValueNumber(){
	        return this.longValueNumber;
        }

        public Dictionary<int, LVItem> getLvItemDict(){
	        return this.lvItemDict;
        }

        public void setLVItemDict(Dictionary<int, LVItem> lvItemDict)
        {
	        this.lvItemDict = lvItemDict;
        }

        public void addDeletedPageSet(int pageNumber){
	        this.deletedPageSet.Add(pageNumber);
        }

        public List<String> getColumnNameList()
        {
            return this.columnNameList;
        }

        public void setUTCTime(String UTCAddHour, String UTCAddMinute)
        {
            this.UTCAddHour = UTCAddHour;
            this.UTCAddMinute = UTCAddMinute;
        }
    }

   
}

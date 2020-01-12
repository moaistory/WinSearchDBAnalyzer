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
using System.IO;

namespace WinSearchDBAnalyzer
{
    public partial class MainForm : Form
    {
        private FileTreeForm fileTreeForm;
        private PreviewForm previewForm;
        private SearchForm searchForm;
        private bool isParsing = true;
        private bool isWin10 = true;
        public WaitForm waitForm;
        EDBParserManager edbParserManager;
        private BackgroundWorker backgroudWoker;
        private String UTCTime = "UTC+0";
        private String UTCAddHour = "0";
        private String UTCAddMinute = "0";
        private Table parsingTable;
        private ExplorerForm explorerFrom;
        private Dictionary<int, MyFile> fileDictionary;
        private Dictionary<string, MyDirectory> extDirDictionary;
        public MainForm()
        {
            InitializeComponent();
            edbParserManager = new EDBParserManager(this);
            DockPanel_Load();
            init();
        }



        public delegate void setRecordList(List<ListViewItem> listViewItemList);
        public setRecordList setRecordListDelegate;
        public void setRecordListMethod(List<ListViewItem> listViewItemList)
        {
            if (parsingTable.getTableName().Equals("SystemIndex_GthrPth"))
            {
                fileTreeForm.init();
                listViewItemList.Sort(compareRecord);
                fileTreeForm.startAddDirectory();
                foreach (ListViewItem listViewItem in listViewItemList)
                {
                    try
                    {
                        MyDirectory myDir = new MyDirectory();
                        myDir.id = Int32.Parse(listViewItem.SubItems[1].Text);
                        myDir.parentId = Int32.Parse(listViewItem.SubItems[2].Text);
                        myDir.name = listViewItem.SubItems[3].Text;

                        fileTreeForm.addDirectory(myDir);
                    }
                    catch(Exception e)
                    {
                    }
                }
                fileTreeForm.finishAddDirectory();
            }

            if (parsingTable.getTableName().Equals("SystemIndex_Gthr"))
            {
                fileDictionary = new Dictionary<int, MyFile>();
                foreach (ListViewItem listViewItem in listViewItemList)
                {
                    try
                    {
                        MyFile myFile = new MyFile();
                        myFile.parentId = Int32.Parse(listViewItem.SubItems[1].Text);
                        myFile.id = Int32.Parse(listViewItem.SubItems[2].Text);
                        myFile.lastModified = listViewItem.SubItems[3].Text;
                        myFile.priority = Int32.Parse(listViewItem.SubItems[4].Text);
                        myFile.name = listViewItem.SubItems[5].Text;
                        if (!fileDictionary.Keys.Contains(myFile.id))
                        {
                            fileDictionary.Add(myFile.id, myFile);
                            fileTreeForm.addChild(myFile);
                        }
                    }
                    catch (Exception e)
                    {
                    }
                }
                List<string> columnList = new List<string>() { "FileName", "LastModified" };
                explorerFrom.clearData();
                explorerFrom.clearColumn();
                explorerFrom.setColumn(columnList);
                explorerFrom.Show(dockPanel, DockState.Document);
            }
            if (parsingTable.getTableName().Equals("SystemIndex_PropertyStore") || parsingTable.getTableName().Equals("SystemIndex_0A"))
            {
                MyDirectory categorizeDir = new MyDirectory();
                categorizeDir.id = -2;
                categorizeDir.parentId = 1;
                categorizeDir.name = "Categorize";
                fileTreeForm.addDirectory(categorizeDir);

                MyDirectory allDir = new MyDirectory();
                allDir.id = -3;
                allDir.parentId = 1;
                allDir.name = "ALL";
                fileTreeForm.addDirectory(allDir);

                extDirDictionary = new Dictionary<string, MyDirectory>();
                int categorizeID = -3;
                List<string> parsingColumn = parsingTable.getParsingColumn();
                explorerFrom.addColumn(parsingColumn);

                foreach (ListViewItem listViewItem in listViewItemList)
                {
                    MyFile myFile;
                    int key = Int32.Parse(listViewItem.SubItems[1].Text);
                    if (fileDictionary.ContainsKey(key))
                    {
                        myFile = fileDictionary[key];
                    }
                    else
                    {
                        myFile = new MyFile();
                        myFile.parentId = 0;
                        myFile.id = key;
                        myFile.lastModified = "0"; ;
                        myFile.priority = 0;
                        fileTreeForm.addChild(myFile);
                    }
                    allDir.addFile(myFile.id, myFile);
                    for (int i = 2; i < listViewItem.SubItems.Count; i++)
                    {
                        if(parsingColumn[ i- 2].Contains("System_IsFolder"))
                        {
                            if (listViewItem.SubItems[i].Text == "TRUE")
                            {
                                myFile.isFolder = true;
                            }
                        }
                        else if (parsingColumn[i - 2].Contains("System_Search_AutoSummary"))
                        {
                            myFile.summary = listViewItem.SubItems[i].Text;
                        }
                        else if (parsingColumn[i - 2].Contains("System_FileName"))
                        {
                            if(myFile.name=="")
                                myFile.name = listViewItem.SubItems[i].Text;
                        }
                        else if (parsingColumn[i - 2].Contains("System_FileExtension"))
                        {
                            string ext = listViewItem.SubItems[i].Text.ToLower();
                            if (!extDirDictionary.Keys.Contains(ext))
                            {
                                MyDirectory newDir = new MyDirectory();
                                newDir.id = categorizeID--;
                                newDir.parentId = categorizeDir.id;
                                newDir.name = ext;
                                fileTreeForm.addDirectory(newDir);
                                extDirDictionary.Add(ext, newDir);
                            }
                            extDirDictionary[ext].addFile(myFile.id, myFile);
                        }
                        myFile.fileInformationList.Add(listViewItem.SubItems[i].Text);
                    }
                }
                fileTreeForm.sortTreeView(categorizeDir.id);
                explorerFrom.setAutoColumnSize();
                if (!searchForm.IsActivated)
                {
                    searchForm.Show(dockPanel, DockState.DockTop);
                    dockPanel.DockTopPortion = 45;
                }
            }
        }

        private void init()
        {
            setRecordListDelegate = new setRecordList(setRecordListMethod);
            fileTreeForm = new FileTreeForm(this);
            previewForm = new PreviewForm(this);
            explorerFrom = new ExplorerForm(this);
            searchForm = new SearchForm(this);
            explorerFrom.setTitle("File Explorer");
            fileTreeForm.Show(dockPanel, DockState.DockLeft);
            previewForm.Show(dockPanel, DockState.DockRight);
        }

        public void setFileList(MyDirectory myDict)
        {
            explorerFrom.clearData();
            List<ListViewItem> fileList = myDict.getFileList();
            explorerFrom.setData(fileList);
        }


        public void setSummary(string summary)
        {
            previewForm.setText(summary);
        }

        public void searchBackground(String searchData)
        {
            backgroudWoker = new BackgroundWorker();
            backgroudWoker.DoWork += new DoWorkEventHandler(searchDoWrok);
            backgroudWoker.WorkerSupportsCancellation = true;
            backgroudWoker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(searchRunWorkerCompletedEventHandler);
            waitForm = new WaitForm();
            waitForm.setStyle(ProgressBarStyle.Blocks);
            waitForm.setLabel("Please wait while loading records");
            waitForm.FormClosing += new FormClosingEventHandler(waitFormClosingEventHandler);
            backgroudWoker.RunWorkerAsync(searchData);
            waitForm.ShowDialog();
        }

        public void searchDoWrok(object sender, DoWorkEventArgs e)
        {
            String searchData = (String)e.Argument;
            bool result = false;
            int count = 0;
            List<ListViewItem> data = new List<ListViewItem>();
            waitForm.maxProgress(fileDictionary.Keys.Count);
            foreach (MyFile myFile in fileDictionary.Values)
            {
                ++count;
                if (count % 1000 == 0 || fileDictionary.Values.Count == count)
                    waitForm.setPrgress(count); 
                ListViewItem listViewData = myFile.getListViewItem();
                foreach (ListViewItem.ListViewSubItem subItem in listViewData.SubItems)
                {
                    if (subItem.Text.ToLower().IndexOf(searchData.ToLower()) >= 0)
                    {
                        data.Add(listViewData);
                        result = true;
                        break;
                    }
                }
            }
            e.Result = result;

            if(result)
            {
                explorerFrom.clearData();
                explorerFrom.setData(data);
            }
        }


        public void searchRunWorkerCompletedEventHandler(object sender, RunWorkerCompletedEventArgs e)
        {
            waitForm.Close();
            bool result = (bool) e.Result;
            if (!result)
            {
                MessageBox.Show("Not found");
            }
        }


        public int compareRecord(ListViewItem listViewItemX, ListViewItem listViewItemY)
        {
            int x = 0;
            int y = 0;
            try { x = Int32.Parse(listViewItemX.SubItems[1].Text); }
            catch { }
            try { y = Int32.Parse(listViewItemY.SubItems[1].Text); }
            catch { }
            if (x < y)
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }

        public void openParsing(String fileName)
        {
            isParsing = true;
            edbParserManager.colse();
            HexReader hexReader = new HexReader(fileName);
            long signature = hexReader.readLong(4);
            if (signature != 6736818458095L)
            {
                MessageBox.Show("Please open the correct file", "This file is not ESE database", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            hexReader.close();

            InputUTCTimeForm inputUTCTimeForm = new InputUTCTimeForm();
            DialogResult dialogResut = inputUTCTimeForm.ShowDialog();
            if (dialogResut == DialogResult.OK)
            {
                // Read the contents of testDialog's TextBox.
                UTCTime = inputUTCTimeForm.getUTCTime();
                String[] Time = UTCTime.Replace("UTC-", "").Replace("UTC+", "").Split(':');

                if (Time.Length == 1)
                {
                    UTCAddHour = Time[0];
                }
                else if (Time.Length == 2)
                {
                    UTCAddHour = Time[0];
                    UTCAddMinute = Time[1];
                }

            }
            edbParserManager.init(fileName);
            edbParserManager.setUTCTime(UTCAddHour, UTCAddMinute);
            edbParserManager.parseDatabaseHeader();
            edbParserManager.makeTable();
            openFileUsingParsing(); 
            
            
        }

        public void openRecovery(String fileName)
        {
            isParsing = false;
            edbParserManager.colse();
            HexReader hexReader = new HexReader(fileName);
            long signature = hexReader.readLong(4);
            if (signature != 6736818458095L)
            {
                MessageBox.Show("Please open the correct file", "This file is not ESE database", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            hexReader.close();


            InputUTCTimeForm inputUTCTimeForm = new InputUTCTimeForm();
            DialogResult dialogResut = inputUTCTimeForm.ShowDialog();
            if (dialogResut == DialogResult.OK)
            {
                // Read the contents of testDialog's TextBox.
                UTCTime = inputUTCTimeForm.getUTCTime();
                String[] Time = UTCTime.Replace("UTC-", "").Replace("UTC+", "").Split(':');

                if (Time.Length == 1)
                {
                    UTCAddHour = Time[0];
                }
                else if (Time.Length == 2)
                {
                    UTCAddHour = Time[0];
                    UTCAddMinute = Time[1];
                }

            }
            edbParserManager.init(fileName);
            edbParserManager.setUTCTime(UTCAddHour, UTCAddMinute);
            edbParserManager.parseDatabaseHeader();
            edbParserManager.makeTable();
            edbParserManager.findTablePage();
            openFileUsingParsing();
        }

        public void openFileUsingParsing()
        {
            backgroudWoker = new BackgroundWorker();
            backgroudWoker.WorkerReportsProgress = true;
            backgroudWoker.WorkerSupportsCancellation = true;
            backgroudWoker.DoWork += new DoWorkEventHandler(parseTableDoWork);
            backgroudWoker.ProgressChanged += new ProgressChangedEventHandler(parseTableProgressChanged);
            backgroudWoker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(parseTableRunWorkerCompletedEventHandler);
            backgroudWoker.RunWorkerAsync();
        }

        public void parseTableDoWork(object sender, DoWorkEventArgs e)
        {
            toolStripStatusProgress.Text = "Parse : Directory List";
            Table systemIndex_GthrPthTable = this.edbParserManager.getSystemIndex_GthrPthTable();
            parsingTable = systemIndex_GthrPthTable;
            if (!systemIndex_GthrPthTable.getColumnNameList().Contains("Parent"))
            {
                MessageBox.Show("This version of the file is not supported.", "Support for Windows 7 or later", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;    
            }
            
            systemIndex_GthrPthTable.setMainForm(this);
            toolStripProgressBar.Maximum = systemIndex_GthrPthTable.getPageCount();
            if(isParsing)
                systemIndex_GthrPthTable.parseRecord();
            else
                systemIndex_GthrPthTable.carveRecord();

            edbParserManager.parseRecord(systemIndex_GthrPthTable, isParsing);
            ////////////////////////////////////////////////////////////////////////////////////////////////
            toolStripStatusProgress.Text = "Parse : File List";
            Table systemIndex_GthrTable = this.edbParserManager.getSystemIndex_GthrTable();
            parsingTable = systemIndex_GthrTable;
            systemIndex_GthrTable.setParsingColumn(new List<string>() {"ScopeID", "DocumentID", "LastModified", "Priority", "FileName" });
            systemIndex_GthrTable.setMainForm(this);
            toolStripProgressBar.Maximum = systemIndex_GthrTable.getPageCount();
            if (isParsing)
                systemIndex_GthrTable.parseRecord();
            else
                systemIndex_GthrTable.carveRecord();
            edbParserManager.parseRecord(systemIndex_GthrTable, isParsing);
            ////////////////////////////////////////////////////////////////////////////////////////////////
            toolStripStatusProgress.Text = "Parse : File Information";
            Table systemIndex_PropertyStoreTable = this.edbParserManager.getSystemIndex_PropertyStoreTable();
            parsingTable = systemIndex_PropertyStoreTable;
            List<string> columndNameList = systemIndex_PropertyStoreTable.getColumnNameList();
            SelectColumnForm selectColumnForm = new SelectColumnForm();
            
            if (systemIndex_PropertyStoreTable.getTableName().Contains("SystemIndex_0A"))
            {
                isWin10 = false;
                systemIndex_PropertyStoreTable.setWin7();
                edbParserManager.setWin7();
                selectColumnForm.setColumnList(columndNameList, new List<string>() { "DocID", "System_Size", "System_DateModified", "System_DateCreated", "System_DateAccessed", "System_IsFolder", "System_MIMEType", "System_ItemPathDisplay", "System_Search_AutoSummary", "System_ItemTypeText", "System_FileExtension", "System_FileName", "ActivityHistory_StartTime", "ActivityHistory_EndTime", "Activity_AppDisplayName", "Activity_ContentUri", "Activity_Description", "Activity_DisplayText", "ActivityHistory_AppId", "ActivityHistory_DeviceName" });
            }
            else
            {
                selectColumnForm.setColumnList(columndNameList, new List<string>() { "WorkID", "System_Size", "System_DateModified", "System_DateCreated", "System_DateAccessed", "System_IsFolder", "System_MIMEType", "System_ItemPathDisplay", "System_Search_AutoSummary", "System_ItemTypeText", "System_FileExtension", "System_FileName", "ActivityHistory_StartTime", "ActivityHistory_EndTime", "Activity_AppDisplayName", "Activity_ContentUri", "Activity_Description", "Activity_DisplayText", "ActivityHistory_AppId", "ActivityHistory_DeviceName" });
            }
            DialogResult dialogResut = selectColumnForm.ShowDialog();
            if (dialogResut == DialogResult.OK)
            {
                systemIndex_PropertyStoreTable.setParsingColumn(selectColumnForm.getCheckedColumn());
                systemIndex_PropertyStoreTable.setMainForm(this);
                toolStripProgressBar.Maximum = systemIndex_PropertyStoreTable.getPageCount();
                if (isParsing)
                    systemIndex_PropertyStoreTable.parseRecord();
                else
                    systemIndex_PropertyStoreTable.carveRecord();
                edbParserManager.parseRecord(systemIndex_PropertyStoreTable, isParsing);
            }            
        }

        public void parseTableRunWorkerCompletedEventHandler(object sender, RunWorkerCompletedEventArgs e)
        {
            toolStripStatusProgress.Text = "";
        }

        public void parseTableProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            toolStripProgressBar.Maximum = toolStripProgressBar.Maximum ;
            toolStripProgressBar.Value = e.ProgressPercentage ;   
        }

        public void changeProgress(int value)
        {
            backgroudWoker.ReportProgress(value);
        }

        public void waitFormClosingEventHandler(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                backgroudWoker.CancelAsync();
            }
        }

        
        public void FastCopy(string source, string destination)
        {

            int array_length = (int)Math.Pow(2, 19);
            byte[] dataArray = new byte[array_length];
            using (FileStream fsread = new FileStream
            (source, FileMode.Open, FileAccess.Read, FileShare.None, array_length))
            {
                using (BinaryReader bwread = new BinaryReader(fsread))
                {
                    if (File.Exists(destination))
                    {
                        File.Delete(destination);
                    }
                    using (FileStream fswrite = new FileStream
                    (destination, FileMode.Create, FileAccess.Write, FileShare.None, array_length))
                    {
                        using (BinaryWriter bwwrite = new BinaryWriter(fswrite))
                        {
                            for (; ; )
                            {
                                int read = bwread.Read(dataArray, 0, array_length);
                                if (0 == read)
                                    break;
                                bwwrite.Write(dataArray, 0, read);
                            }
                        }
                    }
                }
            }
            //File.Delete(source);
        }


        #region DockPanel Load
        private void DockPanel_Load()
        {
            // DockPanel 초기화
            dockPanel.AllowEndUserDocking = true;
            dockPanel.DocumentStyle = DocumentStyle.DockingMdi;
            dockPanel.Parent = this;
            dockPanel.Dock = DockStyle.Fill;
            dockPanel.BorderStyle = BorderStyle.None;
            MainForm_DockPanelColorSetting();                   // DockPanel 색상 정의
            Controls.Add(dockPanel);
            dockPanel.BringToFront();
        }
        #endregion

        #region DockPanel UI 초기화 함수
        private void MainForm_DockPanelColorSetting()
        {
            DockPaneStripSkin dockPaneSkin = new DockPaneStripSkin();

            dockPaneSkin.DocumentGradient.DockStripGradient.StartColor = System.Drawing.Color.FromArgb(80, 80, 80);
            dockPaneSkin.DocumentGradient.DockStripGradient.EndColor = System.Drawing.Color.FromArgb(80, 80, 80);
            dockPaneSkin.DocumentGradient.ActiveTabGradient.StartColor = System.Drawing.Color.FromArgb(80, 80, 80);
            dockPaneSkin.DocumentGradient.ActiveTabGradient.EndColor = System.Drawing.Color.FromArgb(80, 80, 80);
            dockPaneSkin.DocumentGradient.ActiveTabGradient.TextColor = System.Drawing.Color.White;
            dockPaneSkin.DocumentGradient.InactiveTabGradient.StartColor = System.Drawing.Color.FromArgb(80, 80, 80);
            dockPaneSkin.DocumentGradient.InactiveTabGradient.EndColor = System.Drawing.Color.FromArgb(80, 80, 80);
            dockPaneSkin.DocumentGradient.InactiveTabGradient.TextColor = System.Drawing.Color.White;

            dockPaneSkin.ToolWindowGradient.DockStripGradient.StartColor = System.Drawing.Color.FromArgb(80, 80, 80);
            dockPaneSkin.ToolWindowGradient.DockStripGradient.EndColor = System.Drawing.Color.FromArgb(80, 80, 80);
            dockPaneSkin.ToolWindowGradient.ActiveTabGradient.StartColor = System.Drawing.Color.FromArgb(80, 80, 80);
            dockPaneSkin.ToolWindowGradient.ActiveTabGradient.EndColor = System.Drawing.Color.FromArgb(80, 80, 80);
            dockPaneSkin.ToolWindowGradient.ActiveTabGradient.TextColor = System.Drawing.Color.White;
            dockPaneSkin.ToolWindowGradient.InactiveTabGradient.StartColor = System.Drawing.Color.FromArgb(80, 80, 80);
            dockPaneSkin.ToolWindowGradient.InactiveTabGradient.EndColor = System.Drawing.Color.FromArgb(80, 80, 80);
            dockPaneSkin.ToolWindowGradient.InactiveTabGradient.TextColor = System.Drawing.Color.White;
            dockPaneSkin.ToolWindowGradient.ActiveCaptionGradient.StartColor = System.Drawing.Color.FromArgb(80, 80, 80);
            dockPaneSkin.ToolWindowGradient.ActiveCaptionGradient.EndColor = System.Drawing.Color.FromArgb(80, 80, 80);
            dockPaneSkin.ToolWindowGradient.ActiveCaptionGradient.TextColor = System.Drawing.Color.White;
            dockPaneSkin.ToolWindowGradient.InactiveCaptionGradient.StartColor = System.Drawing.Color.FromArgb(80, 80, 80);
            dockPaneSkin.ToolWindowGradient.InactiveCaptionGradient.EndColor = System.Drawing.Color.FromArgb(80, 80, 80);
            dockPaneSkin.ToolWindowGradient.InactiveCaptionGradient.TextColor = System.Drawing.Color.White;
            dockPanel.Skin.DockPaneStripSkin = dockPaneSkin;
        }
        #endregion

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormOpen formOpen = new FormOpen(this);
            DialogResult dialogResut = formOpen.ShowDialog();
        }

        private void toolStripStatusLabel1_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://moaistory.blogspot.com/2018/10/winsearchdbanalyzer.html");
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            About about = new About();
            about.ShowDialog();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}

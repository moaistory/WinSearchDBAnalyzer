using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Management;

namespace WinSearchDBAnalyzer
{
    public partial class FormOpen : Form
    {
        //[DllImport("ExtractFiles.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        //public static extern bool FileCopy([MarshalAs(UnmanagedType.LPWStr)]String lpSrcName, [MarshalAs(UnmanagedType.LPWStr)]String lpDstName);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate bool FileCopy([MarshalAs(UnmanagedType.LPWStr)]String lpSrcName, [MarshalAs(UnmanagedType.LPWStr)]String lpDstName);

        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Ansi)]
        static extern IntPtr LoadLibrary([MarshalAs(UnmanagedType.LPStr)]string lpFileName);

        [DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

        [DllImport("kernel32", SetLastError = true)]
        static extern bool FreeLibrary(IntPtr hModule);

        private String filePath = "";
        private String workingDirectory = "";
        private BackgroundWorker backgroudWoker;
        private WaitForm waitForm;
        private MainForm mainForm;
        public FormOpen(MainForm mainForm)
        {
            InitializeComponent();
            this.mainForm = mainForm;
        }

        private void buttonFilePath_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Title = "Select Windows.edb file";
            if (open.ShowDialog() == DialogResult.OK)
            {
                HexReader hexReader = new HexReader(open.FileName);
                long signature = hexReader.readLong(4);
                if (signature != 6736818458095L)
                {
                    MessageBox.Show("Please open the correct file", "This file is not ESE database", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.filePath = "";
                    this.textBoxFilePath.Text = this.filePath;
                    hexReader.close();
                    return;
                }
                this.filePath = open.FileName;
                this.textBoxFilePath.Text = this.filePath;
                int status = hexReader.readInt(0x34);
                if (status == 2)
                {
                    this.labelFileStatus.Text = "File status : Dirty";
                    radioButtonParsing.Checked = true;
                }
                else
                {
                    this.labelFileStatus.Text = "File status : Clean";
                    DateTime currTime = DateTime.Now;
                    String time = currTime.ToString("yyyy") + "_" + currTime.ToString("MM") + "_" + currTime.ToString("dd") + "__" + currTime.ToString("HH_mm_ss");
                    workingDirectory = Path.GetTempPath() + @"WinSearchDBAnalyzer" + time + @"\" + Path.GetFileName(textBoxFilePath.Text);
                    textBoxWorkingDirectory.Text = workingDirectory;
                }
                hexReader.close();
            }
        }

        private void radioButtonExtract_CheckedChanged(object sender, EventArgs e)
        {
            //수정
            buttonFilePath.Enabled = false;
            buttonWorkingDirectory.Enabled = true;
            this.filePath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\Microsoft\Search\Data\Applications\Windows\Windows.edb";
            if (radioButtonExtract.Checked==true &&  !File.Exists(this.filePath))
            {
                MessageBox.Show("Cannot find windwos.edb, Run this program as an administrator", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.radioButtonOpen.Checked = true;
            }
            else
            {
                this.textBoxFilePath.Text = this.filePath;
                this.labelFileStatus.Text = "File status : Dirty";
                radioButtonParsing.Checked = true;
                DateTime currTime = DateTime.Now;
                String time = currTime.ToString("yyyy") + "_" + currTime.ToString("MM") + "_" + currTime.ToString("dd") + "__" + currTime.ToString("HH_mm_ss");
                workingDirectory = Path.GetTempPath() + @"WinSearchDBAanalyzer_" + time + @"\" + Path.GetFileName(textBoxFilePath.Text);
                textBoxWorkingDirectory.Text = workingDirectory;
                groupBoxWorking.Text = "Working directory and extracted file name";
            }
            
        }

        private void radioButtonOpen_CheckedChanged(object sender, EventArgs e)
        {
            this.filePath = "";
            this.textBoxFilePath.Text = this.filePath;
            this.labelFileStatus.Text = "File status :";
            buttonFilePath.Enabled = true;
            groupBoxWorking.Text = "Working directory and copied file name";
            buttonWorkingDirectory.Enabled = false;
            textBoxWorkingDirectory.Text = "";
            this.workingDirectory = "";

        }

        private void radioButtonParsing_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonOpen.Checked)
            {
                buttonWorkingDirectory.Enabled = false;
                textBoxWorkingDirectory.Text = "";
                this.workingDirectory = "";
            }
            else
            {
                buttonWorkingDirectory.Enabled = true;
                DateTime currTime = DateTime.Now;
                String time = currTime.ToString("yyyy") + "_" + currTime.ToString("MM") + "_" + currTime.ToString("dd") + "__" + currTime.ToString("HH_mm_ss");
                workingDirectory = Path.GetTempPath() + @"WinSearchDBAanalyzer_" + time + @"\" + Path.GetFileName(textBoxFilePath.Text);
                textBoxWorkingDirectory.Text = workingDirectory;
            }
        }

        private void radioButtonRecovery_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonOpen.Checked)
            {
                buttonWorkingDirectory.Enabled = false;
                textBoxWorkingDirectory.Text = "";
                this.workingDirectory = "";
            }
            else
            {
                buttonWorkingDirectory.Enabled = true;
                DateTime currTime = DateTime.Now;
                String time = currTime.ToString("yyyy") + "_" + currTime.ToString("MM") + "_" + currTime.ToString("dd") + "__" + currTime.ToString("HH_mm_ss");
                workingDirectory = Path.GetTempPath() + @"WinSearchDBAanalyzer_" + time + @"\" + Path.GetFileName(textBoxFilePath.Text);
                textBoxWorkingDirectory.Text = workingDirectory;
            }
        }

        private void buttonWorkingDirectory_Click(object sender, EventArgs e)
        {
            SaveFileDialog sf = new SaveFileDialog();
            
            sf.FileName = Path.GetFileName(textBoxFilePath.Text);

            if (sf.ShowDialog() == DialogResult.OK)
            {
                this.workingDirectory = sf.FileName;
                textBoxWorkingDirectory.Text = this.workingDirectory;
                if (File.Exists(workingDirectory))
                {
                    File.Delete(workingDirectory);
                }
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void extractFile()
        {
            
            waitForm = new WaitForm();
            waitForm.setLabel("extracting file");
            waitForm.FormClosing += new FormClosingEventHandler(waitFormClosingEventHandler);
            waitForm.setStyle(ProgressBarStyle.Marquee);
            waitForm.end("");
            backgroudWoker = new BackgroundWorker();
            backgroudWoker.DoWork += new DoWorkEventHandler(extractFileDoWork);
            backgroudWoker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(extractFileRunWorkerCompletedEventHandler);
            backgroudWoker.WorkerSupportsCancellation = true;
            backgroudWoker.RunWorkerAsync();
            waitForm.ShowDialog();   
        }

        private void analyzeFile()
        {
            this.Visible = false;
            if (radioButtonParsing.Checked == true)
            {
                if (radioButtonExtract.Checked)
                {
                    mainForm.openParsing(this.workingDirectory);
                }
                else
                {
                    mainForm.openParsing(this.filePath);
                }
            }
            else if (radioButtonRecovery.Checked == true)
            {
                if (radioButtonExtract.Checked)
                {
                    mainForm.openRecovery(this.workingDirectory);
                }
                else
                {
                    mainForm.openRecovery(this.filePath);
                }
                
            }
            this.Close();
        }

        public void extractFileDoWork(object sender, DoWorkEventArgs e)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(workingDirectory));
            if (!File.Exists("ExtractFiles.dll"))
            {
                using (Stream input = Assembly.GetExecutingAssembly().GetManifestResourceStream("WinSearchDBAnalyzer.Resources.ExtractFiles.dll"))
                using (Stream output = File.Create("ExtractFiles.dll"))
                {
                    byte[] buffer = new byte[8192];

                    int bytesRead;
                    while ((bytesRead = input.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        output.Write(buffer, 0, bytesRead);
                    }
                }
            }

            IntPtr hLib = LoadLibrary("ExtractFiles.dll");
            IntPtr ctorPtr = GetProcAddress(hLib, "FileCopy");
            FileCopy constructorFn = (FileCopy)Marshal.GetDelegateForFunctionPointer(ctorPtr, typeof(FileCopy));
            bool result = constructorFn(filePath, workingDirectory);
            FreeLibrary(hLib);

            if (File.Exists("ExtractFiles.dll"))
            {
                File.Delete("ExtractFiles.dll");
            }
            e.Result = result;
        }

        public void waitFormClosingEventHandler(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                Process[] processList = Process.GetProcessesByName("ExtractFile");
                if (processList.Length > 0)
                {
                    foreach (Process process in processList)
                    {
                        process.Kill();
                    }
                    MessageBox.Show("Cancel extracting file", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

        }

        public void extractFileRunWorkerCompletedEventHandler(object sender, RunWorkerCompletedEventArgs e)
        {
            bool result = (bool)e.Result;
            waitForm.Visible = false;
            waitForm.Close();

            if (result == false)
            {
                MessageBox.Show("Cannot extract file, Run this program as an administrator", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                analyzeFile();
            }

        }

        private void buttonOpen_Click(object sender, EventArgs e)
        {
            if (textBoxFilePath.Text.Length == 0)
            {
                MessageBox.Show("File path is blank", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!File.Exists(textBoxFilePath.Text))
            {
                MessageBox.Show("File does not exist", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (radioButtonExtract.Checked == true)
            {
                extractFile();
            }
            else
            {
                analyzeFile();
            }

        }

        private void FormOpen_FormClosed(object sender, FormClosedEventArgs e)
        {
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Escape))
            {
                this.Close();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}

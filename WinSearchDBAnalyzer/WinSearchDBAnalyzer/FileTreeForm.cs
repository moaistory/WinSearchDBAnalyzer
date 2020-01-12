using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace WinSearchDBAnalyzer
{
    public partial class FileTreeForm : DockContent
    {
        private MainForm mainForm;
        private Dictionary<TreeNode, MyDirectory> treeNodeDirectoryDict = new Dictionary<TreeNode, MyDirectory>();
        private Dictionary<int, TreeNode> idDirectoryDict = new Dictionary<int, TreeNode>();
        private MyDirectory unknownPathDir = new MyDirectory();
        private TreeNode unknownPathTreeNode = new TreeNode("Unknown");
        private TreeNode selectNode;
        public FileTreeForm(MainForm mainForm)
        {
            InitializeComponent();
            this.mainForm = mainForm;

        }

        public void init()
        {
            treeNodeDirectoryDict = new Dictionary<TreeNode, MyDirectory>();
            idDirectoryDict = new Dictionary<int, TreeNode>();
            treeViewExplorer.BeginUpdate();
            treeViewExplorer.Nodes.Clear();

            //UnknownPath
            unknownPathDir.parentId = -1;
            unknownPathDir.id = 0;
            unknownPathDir.name = "Unknown";
            selectNode = unknownPathTreeNode;
            treeNodeDirectoryDict.Add(unknownPathTreeNode, unknownPathDir);
            idDirectoryDict.Add(unknownPathDir.id, unknownPathTreeNode);
            treeViewExplorer.Nodes.Add(unknownPathTreeNode);

            treeViewExplorer.EndUpdate();
        }



        public void startAddDirectory()
        {
            treeViewExplorer.BeginUpdate();
        }

        public void addChild(MyFile myFile)
        {
            int parentId = 0;
            if (idDirectoryDict.ContainsKey(myFile.parentId))
                parentId = myFile.parentId;
            
            treeNodeDirectoryDict[idDirectoryDict[parentId]].addFile(myFile.id, myFile);
            
        }

        public TreeNode addDirectory(MyDirectory directory)
        {
            if (idDirectoryDict.ContainsKey(directory.id))
            {
                return idDirectoryDict[directory.id];
            }
            TreeNode treeNode = new TreeNode(directory.name);
            treeNodeDirectoryDict.Add(treeNode, directory);
            
            idDirectoryDict.Add(directory.id, treeNode);
            List<TreeNode> lostChildNodeList = new List<TreeNode>();
            foreach (TreeNode unknownTreeNode in unknownPathTreeNode.Nodes)
            {
                if(treeNodeDirectoryDict[unknownTreeNode].parentId == directory.id)
                {
                    lostChildNodeList.Add(unknownTreeNode);
                }
            }

            foreach (TreeNode lostChildNode in lostChildNodeList)
            {
                unknownPathTreeNode.Nodes.Remove(lostChildNode);
                treeNode.Nodes.Add(lostChildNode);
            }
                
            if (directory.parentId == 1)
            {
                treeViewExplorer.Nodes.Add(treeNode);
            }
            else if (idDirectoryDict.ContainsKey(directory.parentId))
            {
                idDirectoryDict[directory.parentId].Nodes.Add(treeNode);
            }
            else
            {
                unknownPathTreeNode.Nodes.Add(treeNode);
            }
            return treeNode;
        }

        public void finishAddDirectory()
        {
            
            treeViewExplorer.EndUpdate();
            
        }

        private void treeViewExplorer_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if(selectNode != e.Node)
            {
                selectNode = e.Node;
                selectNode.Text = treeNodeDirectoryDict[selectNode].name +  " (" + treeNodeDirectoryDict[selectNode].getFileCount() + ")";
                this.mainForm.setFileList(treeNodeDirectoryDict[e.Node]);
            }
        }

        public void setFileNameToCount()
        {
            foreach (int id in this.idDirectoryDict.Keys)
            {
                idDirectoryDict[id].Text = idDirectoryDict[id].Name + " (" + treeNodeDirectoryDict[idDirectoryDict[id]].getFileCount()+ ")";
            }
        }

        public void sortTreeView(int id)
        {
            idDirectoryDict[id].TreeView.Sort();
        }
    }
}

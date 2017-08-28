using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Entity.data;
using System.IO;
using System.Security.Cryptography;
using System.Threading;


namespace entity.model
{
    public partial class Form1 : Form
    {
        ContactManagerDBEntities db;
       
        public Form1()
        {
            InitializeComponent();
            PopulateTreeView();

        }
        void treeView1_NodeMouseClick(object sender,TreeNodeMouseClickEventArgs e)
        {
            TreeNode newSelected = e.Node;
            listView1.Items.Clear();
            DirectoryInfo nodeDirInfo = (DirectoryInfo)newSelected.Tag;
            ListViewItem.ListViewSubItem[] subItems;
            ListViewItem item = null;

            foreach (DirectoryInfo dir in nodeDirInfo.GetDirectories())
            {
                item = new ListViewItem(dir.Name, 0);
                subItems = new ListViewItem.ListViewSubItem[]
                          {new ListViewItem.ListViewSubItem(item, "Directory"),
                   new ListViewItem.ListViewSubItem(item,
                dir.LastAccessTime.ToShortDateString())};
                item.SubItems.AddRange(subItems);
                listView1.Items.Add(item);
            }
            foreach (FileInfo file in nodeDirInfo.GetFiles())
            {
                item = new ListViewItem(file.Name, 1);
                subItems = new ListViewItem.ListViewSubItem[]
                          { new ListViewItem.ListViewSubItem(item, "File"),
                   new ListViewItem.ListViewSubItem(item,
                file.LastAccessTime.ToShortDateString())};

                item.SubItems.AddRange(subItems);
                listView1.Items.Add(item);
            }
            

            listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            db = new ContactManagerDBEntities();
            contactBindingSource.DataSource = db.Contacts.ToList();
            contactTypeBindingSource.DataSource = db.ContactTypes.ToList();
            string[] logical = Directory.GetLogicalDrives();


            //TEST
            //DriveInfo[] drives = DriveInfo.GetDrives();
            //for (int i = 0; i <= drives.Length - 1; i++)
            //{
            //    comboBox1.Items.Add(drives[i].Name);
            //    comboBox1.SelectedIndex = -1;
            //}
            //END

            //foreach (string drive in logical)
            //{
            //    DriveInfo di = new DriveInfo(drive);
            //    int driveImage;

            //    switch (di.DriveType) //set the drive's icon
            //    {
            //        case DriveType.CDRom:
            //            driveImage = 3;
            //            break;
            //        case DriveType.Network:
            //            driveImage = 6;
            //            break;
            //        case DriveType.NoRootDirectory:
            //            driveImage = 8;
            //            break;
            //        case DriveType.Unknown:
            //            driveImage = 8;
            //            break;
            //        default:
            //            driveImage = 2;
            //            break;
            //    }
            //    TreeNode node = new TreeNode(drive.Substring(0, 1), driveImage, driveImage);
            //    node.Tag = drive;

            //    if (di.IsReady == true)
            //        node.Nodes.Add("...");


            //}



            //foreach (var x in logical)
            //    {
            //        treeView1.Nodes.Add(x);
            //    }

            }
        private void PopulateTreeView()
        {
            TreeNode rootNode;

            DirectoryInfo info = new DirectoryInfo(@"C:\Xml Files");
            if (info.Exists)
            {
                rootNode = new TreeNode(info.Name);
                rootNode.Tag = info;
                GetDirectories(info.GetDirectories(), rootNode);
                treeView1.Nodes.Add(rootNode);
            }
        }

        private void GetDirectories(DirectoryInfo[] subDirs,TreeNode nodeToAddTo)
        {
            TreeNode aNode;
            DirectoryInfo[] subSubDirs;
            foreach (DirectoryInfo subDir in subDirs)
            {
                aNode = new TreeNode(subDir.Name, 0, 0);
                aNode.Tag = subDir;
                aNode.ImageKey = "folder";
                subSubDirs = subDir.GetDirectories();
                if (subSubDirs.Length != 0)
                {
                    GetDirectories(subSubDirs, aNode);
                }
                nodeToAddTo.Nodes.Add(aNode);
            }
        }
        //AUTORIZACIJA FOLDERA ??
        private void dirsTreeView_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {

            //try
            //{

            //    TreeNode courentNode = e.Node;
            //    DirectoryInfo folderPaht = new DirectoryInfo(comboBox1.SelectedItem + courentNode.FullPath);

            //    foreach (DirectoryInfo dir in folderPaht.GetDirectories())
            //    {
            //        string fileName = dir.Name;
            //        TreeNode node = courentNode.Nodes.Add(fileName);
            //        node.Nodes.Add(" ");
            //    }

            //    foreach (FileInfo file in folderPaht.GetFiles())
            //    {
            //        //string ext = file.Extension;
            //        //if (ext.ToLower() == ".txt")
            //        //{
            //            TreeNode newNode = courentNode.Nodes.Add(file.Name);
            //        //}
            //    }
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //}





            //if (e.Node.Nodes.Count > 0)
            //{
            //    if (e.Node.Nodes[0].Text == "..." && e.Node.Nodes[0].Tag == null)
            //    {
            //        e.Node.Nodes.Clear();

            //        //get the list of sub direcotires
            //        string[] dirs = Directory.GetDirectories(e.Node.Tag.ToString());

            //        DirectoryInfo rootDir = new DirectoryInfo(e.Node.Tag.ToString());
            //        foreach (var file in rootDir.GetFiles())
            //        {
            //            TreeNode n = new TreeNode(file.Name, 13, 13);
            //            e.Node.Nodes.Add(n);
            //        }
            //        foreach (string dir in dirs)
            //        {
            //            DirectoryInfo di = new DirectoryInfo(dir);
            //            TreeNode node = new TreeNode(di.Name, 0, 1);

            //            try
            //            {
            //                //keep the directory's full path in the tag for use later
            //                node.Tag = dir;

            //                //if the directory has sub directories add the place holder
            //                if (di.GetDirectories().Count() > 0)
            //                    node.Nodes.Add(null, "...", 0, 0);
            //                foreach (var file in di.GetFiles())
            //                {
            //                    TreeNode n = new TreeNode(file.Name, 13, 13);
            //                    node.Nodes.Add(n);
            //                }
            //            }
            //            catch (UnauthorizedAccessException)
            //            {
            //                //display a locked folder icon
            //                node.ImageIndex = 12;
            //                node.SelectedImageIndex = 12;
            //            }
            //            catch (Exception ex)
            //            {
            //                MessageBox.Show(ex.Message, "DirectoryLister",
            //                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            //            }
            //            finally
            //            {
            //                e.Node.Nodes.Add(node);
            //            }
            //        }
            //    }
            //}
        }
        //private void button1_Click(object sender, EventArgs e)
        //{

        //    string fulPath = comboBox1.SelectedItem + treeView1.SelectedNode.FullPath;
        //    if (fulPath.ToLower().EndsWith(".txt"))
        //    {
        //        StreamReader read = new StreamReader(fulPath);
        //        richTextBox1.Text = read.ReadToEnd();
        //        read.Close();
        //    }
        //    else
        //        MessageBox.Show("");
        //}
        private async void buttonAdd_Click(object sender, EventArgs e)
        {
            using (AddContact form = new AddContact(new Contact()))
            {
                if (form.ShowDialog()== DialogResult.OK)
                {
                    try
                    {
                        contactBindingSource.Add(form.ContactInfo);
                        db.Contacts.Add(form.ContactInfo);
                        await db.SaveChangesAsync();
                    }
                    catch (Exception ex)
                    {

                        MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            contactBindingSource.DataSource = db.Contacts.ToList();
            contactTypeBindingSource.DataSource = db.ContactTypes.ToList();
            Cursor.Current = Cursors.Default;

        }

        private async void buttonEdit_Click(object sender, EventArgs e)
        {
            Contact obj = contactBindingSource.Current as Contact;
            if (obj != null)
            {
                using (AddContact frm = new AddContact(obj))
                {
                    if (frm.ShowDialog() == DialogResult.OK)
                    {
                        try
                        {
                            contactBindingSource.EndEdit();
                            await db.SaveChangesAsync();
                        }
                        catch (Exception ex)
                        {

                            MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    
                }
            }
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Do you want to save the changes?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                int rows = dataGridView.RowCount;
                for (int i = rows - 1; i >= 0; i--)
                {
                    if (dataGridView.Rows[i].Selected)
                    {
                        db.Contacts.Remove(dataGridView.Rows[i].DataBoundItem as Contact);
                        contactBindingSource.RemoveAt(dataGridView.Rows[i].Index);
                    }
                }
            }
        }

        private async void buttonSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("Do you want to save the changes?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    contactBindingSource.EndEdit();
                    await db.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonSaveXml_Click(object sender, EventArgs e)
        {
            if (backgroundWorker.IsBusy)
                return;
            using (SaveFileDialog sfd = new SaveFileDialog() {Filter="Text Documents|*.xml",ValidateNames = true })
            {
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    _inputParameter.ContactList = contactBindingSource.DataSource as List<Contact>;
                    _inputParameter.FileName = sfd.FileName;
                    progressBar1.Minimum = 0;
                    progressBar1.Value = 0;
                    backgroundWorker.RunWorkerAsync(_inputParameter);

                }
            }
            
        }

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            List<Contact> list = ((Dataparameter)e.Argument).ContactList;
            string fileName = ((Dataparameter)e.Argument).FileName;
            using (TextWriter tw = new StreamWriter(new FileStream(fileName,FileMode.Create ),Encoding.UTF8))
            {
                int index = 1;
                int process = list.Count;
                StringBuilder sb = new StringBuilder();
                foreach (Contact p in list)
                {
                    if (!backgroundWorker.CancellationPending)
                    {
                        backgroundWorker.ReportProgress(index++ * 100 / process);
                        sb.AppendLine(string.Format("Ime - {0} {1} : Adresa - : {2} : Datum upisa - : {3}", p.FirstName, p.LastName, p.Address, p.InsertDate));
                    }
                }
                tw.Write(sb.ToString());
            }
        }

        struct Dataparameter
        {
            public List<Contact> ContactList;
            public string FileName { get; set; }


        }
        Dataparameter _inputParameter;

        private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
            label1.Text = string.Format("Processing....{0}", e.ProgressPercentage);
            progressBar1.Update();
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Thread.Sleep(100);
            if (e.Error == null)
            {
                label1.Text = "your data has been saved to text file ";
            }
        }

        private  void buttonOpen_Click(object sender, EventArgs e)
        {
            AddTextInput frm = new AddTextInput();
            frm.ShowDialog();
        }

        private void contactBindingSource_CurrentChanged(object sender, EventArgs e)
        {

        }

        //private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        //{
        //    try
        //    {

        //    listView1.Clear();
        //    string[] dir = Directory.GetDirectories(treeView1.SelectedNode.Text);
        //    string[] dirx = Directory.GetFiles(treeView1.SelectedNode.Text);
        //    foreach (var dirs in dir)
        //    {
        //        listView1.Items.Add(dirs, 0);
        //        treeView1.SelectedNode.Nodes.Add(dirs);
                
        //        string folderName = Path.GetDirectoryName(dirs);
        //        this.label4.Text = folderName;





        //            //Autorizacija mi ne da da prodjem kroz loop
        //            //var numberOfFiles = Directory.GetFiles(dirs).Length;
        //            //this.label5.Text = numberOfFiles.ToString();

        //        }
        //    foreach (var dirs in dirx)
        //    {
        //        listView1.Items.Add(dirs, 0);
        //        treeView1.SelectedNode.Nodes.Add(dirs);
        //        string fileName = Path.GetFileName(dirs);
        //        this.label6.Text = fileName;
        //            DateTime creation = File.GetCreationTime(dirs);
        //            this.label3.Text = creation.ToLongDateString();

        //            //DateTime creation = File.GetCreationTime(dirs);
        //            //this.label3.Text = creation.ToLongDateString();
        //        }

        //    }
        //    catch (Exception exception)
        //    {
        //        Console.WriteLine(exception);
        //        throw;
        //    }
        //}
       





        //CONTEXTMENU ITEMS 

        private void listView1_MouseClick(object sender, MouseEventArgs e)
        {
            

            if (e.Button == MouseButtons.Right)
            {
               ContextMenu menu = new ContextMenu();

            }

        }

   
        private void propertiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

        //private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    treeView1.Nodes.Clear();

        //    DirectoryInfo path = new DirectoryInfo(comboBox1.SelectedItem.ToString());


        //    try
        //    {
        //        foreach (DirectoryInfo dir in path.GetDirectories())
        //        {
        //            TreeNode node = treeView1.Nodes.Add(dir.Name);
        //            node.Nodes.Add(" ");
        //        }

        //        foreach (FileInfo file in path.GetFiles())
        //        {
        //            if (file.Extension.ToLower() == ".txt")
        //            {
        //                TreeNode node = treeView1.Nodes.Add(file.Name);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //    }
        //}
    }
}

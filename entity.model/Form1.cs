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
        private ContactManagerDBEntities db;
       
        public Form1()
        {
            InitializeComponent();
            PopulateTreeView();
        }

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            var newSelected = e.Node;
            listView1.Items.Clear();
            var nodeDirInfo = (DirectoryInfo) newSelected.Tag;
            ListViewItem.ListViewSubItem[] subItems;
            ListViewItem item = null;

            foreach (var dir in nodeDirInfo.GetDirectories())
            {
                item = new ListViewItem(dir.Name, 0);
                subItems = new ListViewItem.ListViewSubItem[]
                {
                    new ListViewItem.ListViewSubItem(item, "Directory"),
                    new ListViewItem.ListViewSubItem(item, dir.LastAccessTime.ToShortDateString()),
                    new ListViewItem.ListViewSubItem(item, $"{dir.GetFiles().Sum(f => f.Length) / 1024} KB"),
                    new ListViewItem.ListViewSubItem(item, dir.GetFiles().Count().ToString())
                };
                item.SubItems.AddRange(subItems);
                listView1.Items.Add(item);
            }
            foreach (var file in nodeDirInfo.GetFiles())
            {
                item = new ListViewItem(file.Name, 1);
                subItems = new ListViewItem.ListViewSubItem[]
                {
                    new ListViewItem.ListViewSubItem(item, "File"),
                    new ListViewItem.ListViewSubItem(item, file.LastWriteTime.ToShortDateString()),
                    new ListViewItem.ListViewSubItem(item, $"{file.Length / 1024} KB"),
                    new ListViewItem.ListViewSubItem(item, ""),
                    new ListViewItem.ListViewSubItem(item, file.CreationTime.ToShortDateString()),
                    new ListViewItem.ListViewSubItem(item, file.LastAccessTime.ToShortDateString())
                };

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
            var logical = Directory.GetLogicalDrives();
        }

        private void PopulateTreeView()
        {
            TreeNode rootNode;

            var info = new DirectoryInfo(@"C:\Xml Files");
            if (info.Exists)
            {
                rootNode = new TreeNode(info.Name);
                rootNode.Tag = info;
                GetDirectories(info.GetDirectories(), rootNode);
                treeView1.Nodes.Add(rootNode);
            }
        }

        private void GetDirectories(DirectoryInfo[] subDirs, TreeNode nodeToAddTo)
        {
            TreeNode aNode;
            DirectoryInfo[] subSubDirs;
            foreach (var subDir in subDirs)
            {
                aNode = new TreeNode(subDir.Name, 0, 0);
                aNode.Tag = subDir;
                aNode.ImageKey = "folder";
                subSubDirs = subDir.GetDirectories();
                if (subSubDirs.Length != 0)
                    GetDirectories(subSubDirs, aNode);
                nodeToAddTo.Nodes.Add(aNode);
            }
        }

        private void dirsTreeView_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
        }

        private async void buttonAdd_Click(object sender, EventArgs e)
        {
            using (var form = new AddContact(new Contact()))
            {
                if (form.ShowDialog() == DialogResult.OK)
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

        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            contactBindingSource.DataSource = db.Contacts.ToList();
            contactTypeBindingSource.DataSource = db.ContactTypes.ToList();
            Cursor.Current = Cursors.Default;
        }

        private async void buttonEdit_Click(object sender, EventArgs e)
        {
            var obj = contactBindingSource.Current as Contact;
            if (obj != null)
                using (var frm = new AddContact(obj))
                {
                    if (frm.ShowDialog() == DialogResult.OK)
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

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            if (
                MessageBox.Show("Do you want to save the changes?", "Message", MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question) == DialogResult.Yes)
            {
                var rows = dataGridView.RowCount;
                for (var i = rows - 1; i >= 0; i--)
                    if (dataGridView.Rows[i].Selected)
                    {
                        db.Contacts.Remove(dataGridView.Rows[i].DataBoundItem as Contact);
                        contactBindingSource.RemoveAt(dataGridView.Rows[i].Index);
                    }
            }
        }

        private async void buttonSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (
                    MessageBox.Show("Do you want to save the changes?", "Message", MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question) == DialogResult.Yes)
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
            using (var sfd = new SaveFileDialog() {Filter = "Text Documents|*.xml", ValidateNames = true})
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
            var list = ((Dataparameter) e.Argument).ContactList;
            var fileName = ((Dataparameter) e.Argument).FileName;
            using (TextWriter tw = new StreamWriter(new FileStream(fileName, FileMode.Create), Encoding.UTF8))
            {
                var index = 1;
                var process = list.Count;
                var sb = new StringBuilder();
                foreach (var p in list)
                    if (!backgroundWorker.CancellationPending)
                    {
                        backgroundWorker.ReportProgress(index++ * 100 / process);
                        sb.AppendLine(string.Format("Ime - {0} {1} : Adresa - : {2} : Datum upisa - : {3}", p.FirstName,
                            p.LastName, p.Address, p.InsertDate));
                    }
                tw.Write(sb.ToString());
            }
        }

        private struct Dataparameter
        {
            public List<Contact> ContactList;
            public string FileName { get; set; }
        }

        private Dataparameter _inputParameter;

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
                label1.Text = "your data has been saved to text file ";
        }

        private void buttonOpen_Click(object sender, EventArgs e)
        {
            var frm = new AddTextInput();
            frm.ShowDialog();
        }

        private void contactBindingSource_CurrentChanged(object sender, EventArgs e)
        {
        }
    }
}

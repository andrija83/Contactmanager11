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

namespace entity.model
{
    public partial class AddContact : Form
    {
        public AddContact(Contact obj)
        {
            InitializeComponent();
            bindingSource1.DataSource = obj;
        }
        public Contact ContactInfo { get
            {
                return bindingSource1.Current as Contact;
            }
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            bindingSource1.EndEdit();
            DialogResult = DialogResult.OK;
        }

        private void AddContact_Load(object sender, EventArgs e)
        {
            comboBoxContactId.DisplayMember = "Contact Type";
            comboBoxContactId.ValueMember = "ContactTypeID";
            using (ContactManagerDBEntities db = new ContactManagerDBEntities())
            {
                comboBoxContactId.DataSource = db.ContactTypes.ToList();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TerminalBaglan
{
    public partial class PersonelBilgi : Form
    {
        GenelAyarlar g = new GenelAyarlar();
        public PersonelBilgi()
        {
                       InitializeComponent();
        }

        private void PersonelBilgi_Load(object sender, EventArgs e)
        {
            Form1 f = new Form1();
            Cursor = Cursors.WaitCursor;
            g.SetMachineNumber(1);
            g.bIsConnected = true;
         

            Cursor = Cursors.Default;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form1 f = new Form1();
            Cursor = Cursors.WaitCursor;
            g.SetMachineNumber(1);
            g.bIsConnected = true;
            //var sonuc = g.sta_SetUserInfo(f.hatalar, this.txtUserID, this.txtName, this.txtCardnumber);
            var sonuc = g.axCZKEM1.SSR_SetUserInfo(1, txtUserID.Text, txtName.Text.Trim(), "", 0, true);
            MessageBox.Show("sonuc : " + sonuc);
            Cursor = Cursors.Default;
        }
    }
}

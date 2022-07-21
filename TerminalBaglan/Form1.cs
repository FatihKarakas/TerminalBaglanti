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
    public partial class Form1 : Form
    {
        GenelAyarlar g = new GenelAyarlar();
        public string sFirmver = "";
        public string sMac = "";
        public string sPlatform = "";
        public string sSN = "";
        public string sProductTime = "";
        public string sDeviceName = "";
        public int iFPAlg = 0;
        public int iFaceAlg = 0;
        public string sProducter = "";
        public int adminCnt =0;
        public int userCount=0;
        public int fpCount=0;
        public int recordCount=0;
        public int pwdCnt=0;
        public int oplogCount=0;
        public int faceCount=0;
        public Form1()
        {
            InitializeComponent();
            toolStripStatusLabel1.Text = "Bağlantı Bekleniyor...";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            var baglan = g.sta_ConnectTCP(hatalar, terminalIpAdres.Text, PortNumber.Text, "1");
            g.bIsConnected = baglan == 1 ? true : false;
            if (baglan==1)
            {
                MessageBox.Show("Bağlantı Tamam", "Durum", MessageBoxButtons.OK, MessageBoxIcon.Information);
                button1.Visible = false;
                button2.Visible = true;
                g.bIsConnected = true;
                button2.Refresh();
                terminalIpAdres.Enabled = false;
                PortNumber.Enabled = false;
                getDeviceInfo();
                getKapasite();
                txtDeviceName.Text = sDeviceName;
                txtFirmwareVer.Text = sFirmver;
                txtManufacturer.Text = sProducter;
                txtManufactureTime.Text = sProductTime;
                txtSerialNumber.Text = sSN;
                txtPlatForm.Text = sPlatform;
                txtMac.Text = sMac;
                txtFPAlg.Text = iFPAlg.ToString();
                txtFaceAlg.Text = iFaceAlg.ToString();
                txtUserCnt.Text = userCount.ToString();
                txtOpLogCnt.Text = oplogCount.ToString();
                txtFaceCnt.Text = faceCount.ToString();
                txtAdminCnt.Text = adminCnt.ToString();
                txtFPCnt.Text = fpCount.ToString();
                txtPWDCnt.Text = pwdCnt.ToString();
                txtAttLogCnt.Text = recordCount.ToString();
                statusStrip1.BackColor = Color.Green;
                toolStripStatusLabel1.Text = "Terminal Bağlantısı Kuruldu";
                Cursor = Cursors.Default;
                dataGridView1.DataSource = g.sta_getEmployees();
               // dataGridView1.Sort(dataGridView1.Columns[1], ListSortDirection.Ascending);


            }
            else
            {
                MessageBox.Show("Bağlantı Kurulamadı", "Durum", MessageBoxButtons.OK, MessageBoxIcon.Error);
                statusStrip1.BackColor = Color.DarkRed;
            }
        }

        private void btnGetSystemInfo_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            button1.Visible = true;
            button2.Visible = false;
            toolStripStatusLabel1.Text = "Bağlantı Kapatıldı";
            terminalIpAdres.Enabled = true;
            PortNumber.Enabled = true;
            g.bIsConnected = false;
            statusStrip1.BackColor = Color.DarkRed;
        }

        protected int getDeviceInfo()
        {

            return g.sta_GetDeviceInfo(hatalar, out sFirmver, out sMac, out sPlatform, out sSN, out sProductTime, out sDeviceName, out iFPAlg, out iFaceAlg, out sProducter);
        }

        protected int getKapasite()
        {
         
            return g.sta_GetCapacityInfo(hatalar, out adminCnt, out userCount, out fpCount, out recordCount, out pwdCnt, out oplogCount, out faceCount);
        }

        private void btnGetDataInfo_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView1_Click(object sender, EventArgs e)
        {
           // PersonelBilgi pers = new PersonelBilgi();
            txtUserID.Text = dataGridView1.CurrentRow.Cells[0].Value.ToString();
            txtName.Text= dataGridView1.CurrentRow.Cells[1].Value.ToString();
            txtCardnumber.Text = dataGridView1.CurrentRow.Cells[4].Value.ToString();
            //pers.ShowDialog();
            //MessageBox.Show("Adı: " + dataGridView1.CurrentRow.Cells[1].Value.ToString(), "Personel", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            var sonuc = g.axCZKEM1.SSR_SetUserInfo(1, txtUserID.Text, txtName.Text.Trim(), "", 0, true);
            if (sonuc)
            {
                dataGridView1.DataSource = g.sta_getEmployees();
                dataGridView1.Refresh();
                MessageBox.Show("Personel Bilgisi Terminalde Güncellendi", "Personel Güncelleme", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Personel Bilgisi Güncelenemedi", "HATA", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}

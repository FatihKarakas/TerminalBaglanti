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
    }
}

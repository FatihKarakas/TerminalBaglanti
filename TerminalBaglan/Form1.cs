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
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
           
            var baglan = g.sta_ConnectTCP(hatalar, terminalIpAdres.Text, "4370", "1");
            g.bIsConnected = baglan == 1 ? true : false;
            if (baglan==1)
            {
                MessageBox.Show("Bağlantı Tamam", "Durum", MessageBoxButtons.OK, MessageBoxIcon.Information);
                button1.Visible = false;
                button2.Visible = true;
                g.bIsConnected = true;
                getDeviceInfo();
                txtDeviceName.Text = sDeviceName;
                txtFirmwareVer.Text = sFirmver;
                txtManufacturer.Text = sProducter;
                txtManufactureTime.Text = sProductTime;
                txtSerialNumber.Text = sSN;
                txtPlatForm.Text = sPlatform;
                txtMac.Text = sMac;
                txtFPAlg.Text = iFPAlg.ToString();
                txtFaceAlg.Text = iFaceAlg.ToString();
            }
            else
            {
                MessageBox.Show("Bağlantı Kurulamadı", "Durum", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnGetSystemInfo_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            button1.Visible = true;
            button2.Visible = false;
        }

        protected int getDeviceInfo()
        {

            return g.sta_GetDeviceInfo(hatalar, out sFirmver, out sMac, out sPlatform, out sSN, out sProductTime, out sDeviceName, out iFPAlg, out iFaceAlg, out sProducter);
        }

        private void btnGetDataInfo_Click(object sender, EventArgs e)
        {

        }
    }
}

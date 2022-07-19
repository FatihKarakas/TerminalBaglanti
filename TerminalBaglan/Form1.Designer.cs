namespace TerminalBaglan
{
    partial class Form1
    {
        /// <summary>
        ///Gerekli tasarımcı değişkeni.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///Kullanılan tüm kaynakları temizleyin.
        /// </summary>
        ///<param name="disposing">yönetilen kaynaklar dispose edilmeliyse doğru; aksi halde yanlış.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer üretilen kod

        /// <summary>
        /// Tasarımcı desteği için gerekli metot - bu metodun 
        ///içeriğini kod düzenleyici ile değiştirmeyin.
        /// </summary>
        private void InitializeComponent()
        {
            this.terminalIpAdres = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.cihazTabKontrol = new System.Windows.Forms.TabControl();
            this.CihazInfo = new System.Windows.Forms.TabPage();
            this.txtFaceAlg = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtPlatForm = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtMac = new System.Windows.Forms.TextBox();
            this.txtDeviceName = new System.Windows.Forms.TextBox();
            this.label21 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtSerialNumber = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.txtFPAlg = new System.Windows.Forms.TextBox();
            this.txtManufacturer = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.txtFirmwareVer = new System.Windows.Forms.TextBox();
            this.txtManufactureTime = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.CihazSayilar = new System.Windows.Forms.TabPage();
            this.hatalar = new System.Windows.Forms.ListBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.terminalIpLabel = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.txtFaceCnt = new System.Windows.Forms.TextBox();
            this.txtAdminCnt = new System.Windows.Forms.TextBox();
            this.label24 = new System.Windows.Forms.Label();
            this.txtUserCnt = new System.Windows.Forms.TextBox();
            this.label25 = new System.Windows.Forms.Label();
            this.label26 = new System.Windows.Forms.Label();
            this.txtPWDCnt = new System.Windows.Forms.TextBox();
            this.txtOpLogCnt = new System.Windows.Forms.TextBox();
            this.label27 = new System.Windows.Forms.Label();
            this.label28 = new System.Windows.Forms.Label();
            this.txtAttLogCnt = new System.Windows.Forms.TextBox();
            this.txtFPCnt = new System.Windows.Forms.TextBox();
            this.label29 = new System.Windows.Forms.Label();
            this.cihazTabKontrol.SuspendLayout();
            this.CihazInfo.SuspendLayout();
            this.CihazSayilar.SuspendLayout();
            this.SuspendLayout();
            // 
            // terminalIpAdres
            // 
            this.terminalIpAdres.Location = new System.Drawing.Point(140, 35);
            this.terminalIpAdres.Name = "terminalIpAdres";
            this.terminalIpAdres.Size = new System.Drawing.Size(196, 20);
            this.terminalIpAdres.TabIndex = 0;
            this.terminalIpAdres.Text = "10.80.15.220";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(522, 35);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "Bağlantı Kur";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // cihazTabKontrol
            // 
            this.cihazTabKontrol.Controls.Add(this.CihazInfo);
            this.cihazTabKontrol.Controls.Add(this.CihazSayilar);
            this.cihazTabKontrol.Location = new System.Drawing.Point(33, 82);
            this.cihazTabKontrol.Name = "cihazTabKontrol";
            this.cihazTabKontrol.SelectedIndex = 0;
            this.cihazTabKontrol.Size = new System.Drawing.Size(1139, 222);
            this.cihazTabKontrol.TabIndex = 3;
            // 
            // CihazInfo
            // 
            this.CihazInfo.Controls.Add(this.txtFaceAlg);
            this.CihazInfo.Controls.Add(this.label3);
            this.CihazInfo.Controls.Add(this.txtPlatForm);
            this.CihazInfo.Controls.Add(this.label1);
            this.CihazInfo.Controls.Add(this.txtMac);
            this.CihazInfo.Controls.Add(this.txtDeviceName);
            this.CihazInfo.Controls.Add(this.label21);
            this.CihazInfo.Controls.Add(this.label2);
            this.CihazInfo.Controls.Add(this.txtSerialNumber);
            this.CihazInfo.Controls.Add(this.label4);
            this.CihazInfo.Controls.Add(this.label20);
            this.CihazInfo.Controls.Add(this.txtFPAlg);
            this.CihazInfo.Controls.Add(this.txtManufacturer);
            this.CihazInfo.Controls.Add(this.label13);
            this.CihazInfo.Controls.Add(this.label19);
            this.CihazInfo.Controls.Add(this.txtFirmwareVer);
            this.CihazInfo.Controls.Add(this.txtManufactureTime);
            this.CihazInfo.Controls.Add(this.label14);
            this.CihazInfo.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.CihazInfo.Location = new System.Drawing.Point(4, 22);
            this.CihazInfo.Name = "CihazInfo";
            this.CihazInfo.Padding = new System.Windows.Forms.Padding(3);
            this.CihazInfo.Size = new System.Drawing.Size(1131, 196);
            this.CihazInfo.TabIndex = 0;
            this.CihazInfo.Text = "Cihaz Bilgileri";
            this.CihazInfo.UseVisualStyleBackColor = true;
            // 
            // txtFaceAlg
            // 
            this.txtFaceAlg.Location = new System.Drawing.Point(729, 62);
            this.txtFaceAlg.Name = "txtFaceAlg";
            this.txtFaceAlg.Size = new System.Drawing.Size(151, 20);
            this.txtFaceAlg.TabIndex = 53;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(629, 68);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(77, 13);
            this.label3.TabIndex = 52;
            this.label3.Text = "Face Algorithm";
            // 
            // txtPlatForm
            // 
            this.txtPlatForm.Location = new System.Drawing.Point(409, 24);
            this.txtPlatForm.Name = "txtPlatForm";
            this.txtPlatForm.Size = new System.Drawing.Size(151, 20);
            this.txtPlatForm.TabIndex = 39;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(63, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(57, 13);
            this.label1.TabIndex = 36;
            this.label1.Text = "Cihaz Adı :";
            // 
            // txtMac
            // 
            this.txtMac.Location = new System.Drawing.Point(409, 59);
            this.txtMac.Name = "txtMac";
            this.txtMac.Size = new System.Drawing.Size(151, 20);
            this.txtMac.TabIndex = 51;
            // 
            // txtDeviceName
            // 
            this.txtDeviceName.Location = new System.Drawing.Point(123, 24);
            this.txtDeviceName.Name = "txtDeviceName";
            this.txtDeviceName.Size = new System.Drawing.Size(153, 20);
            this.txtDeviceName.TabIndex = 37;
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(360, 65);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(30, 13);
            this.label21.TabIndex = 50;
            this.label21.Text = "MAC";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(304, 27);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(90, 13);
            this.label2.TabIndex = 38;
            this.label2.Text = "Çalışma Altyapısı :";
            // 
            // txtSerialNumber
            // 
            this.txtSerialNumber.Location = new System.Drawing.Point(123, 100);
            this.txtSerialNumber.Name = "txtSerialNumber";
            this.txtSerialNumber.Size = new System.Drawing.Size(153, 20);
            this.txtSerialNumber.TabIndex = 49;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(629, 27);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(66, 13);
            this.label4.TabIndex = 40;
            this.label4.Text = "FP Algorithm";
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(36, 103);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(48, 13);
            this.label20.TabIndex = 48;
            this.label20.Text = "Seri No :";
            // 
            // txtFPAlg
            // 
            this.txtFPAlg.Location = new System.Drawing.Point(729, 24);
            this.txtFPAlg.Name = "txtFPAlg";
            this.txtFPAlg.Size = new System.Drawing.Size(151, 20);
            this.txtFPAlg.TabIndex = 41;
            // 
            // txtManufacturer
            // 
            this.txtManufacturer.Location = new System.Drawing.Point(409, 100);
            this.txtManufacturer.Name = "txtManufacturer";
            this.txtManufacturer.Size = new System.Drawing.Size(151, 20);
            this.txtManufacturer.TabIndex = 47;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(19, 68);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(98, 13);
            this.label13.TabIndex = 42;
            this.label13.Text = "Firmware Versiyon :";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(320, 103);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(74, 13);
            this.label19.TabIndex = 46;
            this.label19.Text = "Üretici Firma : ";
            // 
            // txtFirmwareVer
            // 
            this.txtFirmwareVer.Location = new System.Drawing.Point(123, 62);
            this.txtFirmwareVer.Name = "txtFirmwareVer";
            this.txtFirmwareVer.Size = new System.Drawing.Size(153, 20);
            this.txtFirmwareVer.TabIndex = 43;
            // 
            // txtManufactureTime
            // 
            this.txtManufactureTime.Location = new System.Drawing.Point(729, 100);
            this.txtManufactureTime.Name = "txtManufactureTime";
            this.txtManufactureTime.Size = new System.Drawing.Size(151, 20);
            this.txtManufactureTime.TabIndex = 45;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(613, 103);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(79, 13);
            this.label14.TabIndex = 44;
            this.label14.Text = "Üretim zamanı :";
            // 
            // CihazSayilar
            // 
            this.CihazSayilar.Controls.Add(this.label9);
            this.CihazSayilar.Controls.Add(this.txtFaceCnt);
            this.CihazSayilar.Controls.Add(this.txtAdminCnt);
            this.CihazSayilar.Controls.Add(this.label24);
            this.CihazSayilar.Controls.Add(this.txtUserCnt);
            this.CihazSayilar.Controls.Add(this.label25);
            this.CihazSayilar.Controls.Add(this.label26);
            this.CihazSayilar.Controls.Add(this.txtPWDCnt);
            this.CihazSayilar.Controls.Add(this.txtOpLogCnt);
            this.CihazSayilar.Controls.Add(this.label27);
            this.CihazSayilar.Controls.Add(this.label28);
            this.CihazSayilar.Controls.Add(this.txtAttLogCnt);
            this.CihazSayilar.Controls.Add(this.txtFPCnt);
            this.CihazSayilar.Controls.Add(this.label29);
            this.CihazSayilar.Location = new System.Drawing.Point(4, 22);
            this.CihazSayilar.Name = "CihazSayilar";
            this.CihazSayilar.Padding = new System.Windows.Forms.Padding(3);
            this.CihazSayilar.Size = new System.Drawing.Size(1131, 196);
            this.CihazSayilar.TabIndex = 1;
            this.CihazSayilar.Text = "Sayısal Veriler";
            this.CihazSayilar.UseVisualStyleBackColor = true;
            // 
            // hatalar
            // 
            this.hatalar.FormattingEnabled = true;
            this.hatalar.Location = new System.Drawing.Point(37, 310);
            this.hatalar.Name = "hatalar";
            this.hatalar.Size = new System.Drawing.Size(1135, 95);
            this.hatalar.TabIndex = 4;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(412, 38);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(84, 20);
            this.textBox1.TabIndex = 5;
            this.textBox1.Text = "4370";
            // 
            // terminalIpLabel
            // 
            this.terminalIpLabel.AutoSize = true;
            this.terminalIpLabel.Location = new System.Drawing.Point(34, 38);
            this.terminalIpLabel.Name = "terminalIpLabel";
            this.terminalIpLabel.Size = new System.Drawing.Size(100, 13);
            this.terminalIpLabel.TabIndex = 6;
            this.terminalIpLabel.Text = "Terminal İp Adresi  :";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(357, 41);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(49, 13);
            this.label5.TabIndex = 7;
            this.label5.Text = "Port No :";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(522, 36);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(98, 23);
            this.button2.TabIndex = 8;
            this.button2.Text = "Bağlantı Kapat";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Visible = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 113);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(102, 13);
            this.label9.TabIndex = 39;
            this.label9.Text = "Yüz Tanıma Sayısı : ";
            // 
            // txtFaceCnt
            // 
            this.txtFaceCnt.Location = new System.Drawing.Point(112, 106);
            this.txtFaceCnt.Name = "txtFaceCnt";
            this.txtFaceCnt.Size = new System.Drawing.Size(123, 20);
            this.txtFaceCnt.TabIndex = 38;
            // 
            // txtAdminCnt
            // 
            this.txtAdminCnt.Location = new System.Drawing.Point(380, 29);
            this.txtAdminCnt.Name = "txtAdminCnt";
            this.txtAdminCnt.Size = new System.Drawing.Size(121, 20);
            this.txtAdminCnt.TabIndex = 29;
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(23, 29);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(82, 13);
            this.label24.TabIndex = 26;
            this.label24.Text = "Kullanıcı Sayısı :";
            // 
            // txtUserCnt
            // 
            this.txtUserCnt.Location = new System.Drawing.Point(112, 29);
            this.txtUserCnt.Name = "txtUserCnt";
            this.txtUserCnt.Size = new System.Drawing.Size(123, 20);
            this.txtUserCnt.TabIndex = 27;
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Location = new System.Drawing.Point(298, 32);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(81, 13);
            this.label25.TabIndex = 28;
            this.label25.Text = "Yönetici Sayısı :";
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Location = new System.Drawing.Point(569, 29);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(73, 13);
            this.label26.TabIndex = 30;
            this.label26.Text = "Parola Sayısı :";
            // 
            // txtPWDCnt
            // 
            this.txtPWDCnt.Location = new System.Drawing.Point(653, 26);
            this.txtPWDCnt.Name = "txtPWDCnt";
            this.txtPWDCnt.Size = new System.Drawing.Size(121, 20);
            this.txtPWDCnt.TabIndex = 31;
            // 
            // txtOpLogCnt
            // 
            this.txtOpLogCnt.Location = new System.Drawing.Point(653, 66);
            this.txtOpLogCnt.Name = "txtOpLogCnt";
            this.txtOpLogCnt.Size = new System.Drawing.Size(121, 20);
            this.txtOpLogCnt.TabIndex = 37;
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.Location = new System.Drawing.Point(14, 71);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(91, 13);
            this.label27.TabIndex = 32;
            this.label27.Text = "Kayıtlı Log Sayısı :";
            // 
            // label28
            // 
            this.label28.AutoSize = true;
            this.label28.Location = new System.Drawing.Point(562, 70);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(78, 13);
            this.label28.TabIndex = 36;
            this.label28.Text = "Op Log Sayısı :";
            // 
            // txtAttLogCnt
            // 
            this.txtAttLogCnt.Location = new System.Drawing.Point(112, 68);
            this.txtAttLogCnt.Name = "txtAttLogCnt";
            this.txtAttLogCnt.Size = new System.Drawing.Size(123, 20);
            this.txtAttLogCnt.TabIndex = 33;
            // 
            // txtFPCnt
            // 
            this.txtFPCnt.Location = new System.Drawing.Point(380, 67);
            this.txtFPCnt.Name = "txtFPCnt";
            this.txtFPCnt.Size = new System.Drawing.Size(121, 20);
            this.txtFPCnt.TabIndex = 35;
            // 
            // label29
            // 
            this.label29.AutoSize = true;
            this.label29.Location = new System.Drawing.Point(314, 70);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(51, 13);
            this.label29.TabIndex = 34;
            this.label29.Text = "FP Count";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1200, 436);
            this.Controls.Add(this.hatalar);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.terminalIpLabel);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.cihazTabKontrol);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.terminalIpAdres);
            this.Name = "Form1";
            this.Text = "Terminal Bağlantı";
            this.cihazTabKontrol.ResumeLayout(false);
            this.CihazInfo.ResumeLayout(false);
            this.CihazInfo.PerformLayout();
            this.CihazSayilar.ResumeLayout(false);
            this.CihazSayilar.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox terminalIpAdres;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TabControl cihazTabKontrol;
        private System.Windows.Forms.TabPage CihazInfo;
        private System.Windows.Forms.TextBox txtFaceAlg;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtPlatForm;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtMac;
        private System.Windows.Forms.TextBox txtDeviceName;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtSerialNumber;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.TextBox txtFPAlg;
        private System.Windows.Forms.TextBox txtManufacturer;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.TextBox txtFirmwareVer;
        private System.Windows.Forms.TextBox txtManufactureTime;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TabPage CihazSayilar;
        private System.Windows.Forms.ListBox hatalar;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label terminalIpLabel;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtFaceCnt;
        private System.Windows.Forms.TextBox txtAdminCnt;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.TextBox txtUserCnt;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.TextBox txtPWDCnt;
        private System.Windows.Forms.TextBox txtOpLogCnt;
        private System.Windows.Forms.Label label27;
        private System.Windows.Forms.Label label28;
        private System.Windows.Forms.TextBox txtAttLogCnt;
        private System.Windows.Forms.TextBox txtFPCnt;
        private System.Windows.Forms.Label label29;
    }
}


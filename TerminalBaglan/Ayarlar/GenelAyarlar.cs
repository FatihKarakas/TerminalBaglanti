
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

/// <summary>
/// ZKTeco Cihaza Bağlantı ve Kontrol Bilgileri 
/// Fatih KARAKAŞ Osmaniye /2022
/// </summary>
public class GenelAyarlar
{
    
    public zkemkeeper.CZKEMClass axCZKEM1 = new zkemkeeper.CZKEMClass();
    public List<Personel> employeeList = new List<Personel>();
    public List<BioTemplate> bioTemplateList = new List<BioTemplate>();
    public List<string> biometricTypes = new List<string>();
    public bool bIsConnected = false;//Bağlantı Açık mı?
    private static int iMachineNumber = 1;
    private static int idwErrorCode = 0;
    private static int iDeviceTpye = 1;
    bool bAddControl = true;



  
    #region UserBioTypeClass
    private string _biometricType = string.Empty;
    private string _biometricVersion = string.Empty;
    private SupportBiometricType _supportBiometricType = new SupportBiometricType();
    public const string PersBioTableName = "Pers_Biotemplate";
    public const string PersBioTableFields = "*";
    public SupportBiometricType supportBiometricType
    {
        get { return _supportBiometricType; }
    }
    public string biometricType
    {
        get { return _biometricType; }
    }
    public class Personel
    {
        public string personel { get; set; }
        public string AdiSoyadi { get; set; }
        public string Parola { get; set; }
        public int privilege { get; set; }
        public string cardNumarsi { get; set; }
    }
    public class SupportBiometricType
    {
        public bool fp_available { get; set; }
        public bool face_available { get; set; }
        public bool fingerVein_available { get; set; }
        public bool palm_available { get; set; }
    }
    public class BioTemplate
    {
        /// <summary>
        /// is valid,0:invalid,1:valid,default=1
        /// </summary>
        private int validFlag = 1;
        public virtual int valid_flag
        {
            get { return validFlag; }
            set { validFlag = value; }
        }
        /// <summary>
        /// is duress,0:not duress,1:duress,default=0
        /// </summary>
        public virtual int is_duress { get; set; }
        /// <summary>
        /// Biometric Type
        /// 0： Genel
        /// 1： Parmak Okuyucu
        /// 2： Yüz Tanıma
        /// 3： Ses Tanıma
        /// 4： Iris 
        /// 5： Retina
        /// 6： Palm prints
        /// 7： FingerVein
        /// 8： Palm Vein
        /// </summary>
        public virtual int bio_type { get; set; }
        /// <summary>
        /// template version
        /// </summary>
        public virtual string version { get; set; }
        /// <summary>
        /// data format
        /// ZK\ISO\ANSI 
        /// 0： ZK
        /// 1： ISO
        /// 2： ANSI
        /// </summary>
        public virtual int data_format { get; set; }
        /// <summary>
        /// template no
        /// </summary>
        public virtual int template_no { get; set; }
        /// <summary>
        /// template index
        /// </summary>
        public virtual int template_no_index { get; set; }
        /// <summary>
        /// template data
        /// </summary>
        public virtual string template_data { get; set; }
        /// <summary>
        /// personel
        /// </summary>
        public virtual string pin { get; set; }
    }
    public class BioType
    {
        public string name { get; set; }
        public int value { get; set; }
        public override string ToString()
        {
            return name;
        }
    }
    #endregion
    #region CihazBağlantı
    public bool GetConnectState()
    {
        return bIsConnected;
    }
    public void SetConnectState(bool state)
    {
        bIsConnected = state;
        //connected = state;
    }
    public int GetMachineNumber()
    {
        return iMachineNumber;
    }
    public void SetMachineNumber(int Number)
    {
        iMachineNumber = Number;
    }
    public int sta_ConnectTCP(ListBox lblOutputInfo, string ip, string port, string commKey)
    {
        if (ip == "" || port == "" || commKey == "")
        {
            lblOutputInfo.Items.Add("*Name, IP, Port or Commkey cannot be null !");
            return -1;// ip or port is null
        }
        if (Convert.ToInt32(port) <= 0 || Convert.ToInt32(port) > 65535)
        {
            lblOutputInfo.Items.Add("*Port illegal!");
            return -1;
        }
        if (Convert.ToInt32(commKey) < 0 || Convert.ToInt32(commKey) > 999999)
        {
            lblOutputInfo.Items.Add("*CommKey illegal!");
            return -1;
        }
        int idwErrorCode = 0;
        axCZKEM1.SetCommPassword(Convert.ToInt32(commKey));
        if (bIsConnected == true)
        {
            axCZKEM1.Disconnect();
            sta_UnRegRealTime();
            SetConnectState(false);
            lblOutputInfo.Items.Add("Terminal Bağlantısı Kapalı !");
            //connected = false;
            return -2; //disconnect
        }
        if (axCZKEM1.Connect_Net(ip, Convert.ToInt32(port)) == true)
        {
            SetConnectState(true);
            sta_RegRealTime(lblOutputInfo);
            lblOutputInfo.Items.Add(ip + " adresli Terminal Bağlantısı Tamam !");
            //get Biotype
            sta_getBiometricType();
            return 1;
        }
        else
        {
            axCZKEM1.GetLastError(ref idwErrorCode);
            lblOutputInfo.Items.Add("*Cihaza Bağlantı Hatası ,Hata Kodu =" + idwErrorCode.ToString());
            return idwErrorCode;
        }
    }
    public int sta_GetDeviceInfo(ListBox lblOutputInfo, out string sFirmver, out string sMac, out string sPlatform, out string sSN, out string sProductTime, out string sDeviceName, out int iFPAlg, out int iFaceAlg, out string sProducter)
    {
        int iRet = 0;
        sFirmver = "";
        sMac = "";
        sPlatform = "";
        sSN = "";
        sProducter = "";
        sDeviceName = "";
        iFPAlg = 0;
        iFaceAlg = 0;
        sProductTime = "";
        string strTemp = "";
        if (GetConnectState() == false)
        {
            lblOutputInfo.Items.Add("*Önce Terminal bağlantısını kurunuz!");
            return -1024;
        }
        axCZKEM1.EnableDevice(GetMachineNumber(), false);//disable the device
        axCZKEM1.GetSysOption(GetMachineNumber(), "~ZKFPVersion", out strTemp);
        iFPAlg = Convert.ToInt32(strTemp);
        axCZKEM1.GetSysOption(GetMachineNumber(), "ZKFaceVersion", out strTemp);
        iFaceAlg = Convert.ToInt32(strTemp);
        /*
        axCZKEM1.GetDeviceInfo(GetMachineNumber(), 72, ref iFPAlg);
        axCZKEM1.GetDeviceInfo(GetMachineNumber(), 73, ref iFaceAlg);
        */
        axCZKEM1.GetVendor(ref sProducter);
        axCZKEM1.GetProductCode(GetMachineNumber(), out sDeviceName);
        axCZKEM1.GetDeviceMAC(GetMachineNumber(), ref sMac);
        axCZKEM1.GetFirmwareVersion(GetMachineNumber(), ref sFirmver);
        /*
        if (sta_GetDeviceType() == 1)
        {
            axCZKEM1.GetDeviceFirmwareVersion(GetMachineNumber(), ref sFirmver);
        }
         */
        //lblOutputInfo.Items.Add("[func GetDeviceFirmwareVersion]Temporarily unsupported");
        axCZKEM1.GetPlatform(GetMachineNumber(), ref sPlatform);
        axCZKEM1.GetSerialNumber(GetMachineNumber(), out sSN);
        axCZKEM1.GetDeviceStrInfo(GetMachineNumber(), 1, out sProductTime);
        axCZKEM1.EnableDevice(GetMachineNumber(), true);//enable the device
        lblOutputInfo.Items.Add("Cihaz Bilgileri Başarı ile alındı");
        iRet = 1;
        return iRet;
    }
    public int sta_GetCapacityInfo(ListBox lblOutputInfo, out int adminCnt, out int userCount, out int fpCnt, out int recordCnt, out int pwdCnt, out int oplogCnt, out int faceCnt)
    {
        int ret = 0;
        adminCnt = 0;
        userCount = 0;
        fpCnt = 0;
        recordCnt = 0;
        pwdCnt = 0;
        oplogCnt = 0;
        faceCnt = 0;
        if (GetConnectState() == false)
        {
            lblOutputInfo.Items.Add("*Önce Terminal bağlantısını kurunuz!");
            return -1024;
        }
        axCZKEM1.EnableDevice(GetMachineNumber(), false);//disable the device
        axCZKEM1.GetDeviceStatus(GetMachineNumber(), 2, ref userCount);
        axCZKEM1.GetDeviceStatus(GetMachineNumber(), 1, ref adminCnt);
        axCZKEM1.GetDeviceStatus(GetMachineNumber(), 3, ref fpCnt);
        axCZKEM1.GetDeviceStatus(GetMachineNumber(), 4, ref pwdCnt);
        axCZKEM1.GetDeviceStatus(GetMachineNumber(), 5, ref oplogCnt);
        axCZKEM1.GetDeviceStatus(GetMachineNumber(), 6, ref recordCnt);
        axCZKEM1.GetDeviceStatus(GetMachineNumber(), 21, ref faceCnt);
        axCZKEM1.EnableDevice(GetMachineNumber(), true);//enable the device
        lblOutputInfo.Items.Add("Terminal sayısal bilgileri eklendi");
        ret = 1;
        return ret;
    }
    public void sta_DisConnect()
    {
        if (GetConnectState() == true)
        {
            axCZKEM1.Disconnect();
            sta_UnRegRealTime();
        }
    }
    #endregion
    #region TerminalTipi
    public int sta_GetDeviceType()
    {
        string sPlatform = "";
        int iFaceDevice = 0;
        if (axCZKEM1.IsTFTMachine(GetMachineNumber()))
        {
            axCZKEM1.GetDeviceInfo(GetMachineNumber(), 75, ref iFaceDevice);
            axCZKEM1.GetPlatform(GetMachineNumber(), ref sPlatform);
            if (sPlatform.Contains("ZMM"))
            {
                return 1;//new firmware device
            }
            else if (iFaceDevice == 1)
            {
                return 2;//face serial
            }
            else
            {
                return 3;//color device
            }
        }
        else
        {
            return 4;//black&whith device
        }
    }
    #endregion
    #region GerçekZamanBilgileri
    public delegate ListBox GetRealEventListBoxHandler();
    private GetRealEventListBoxHandler gRealEventListBoxHandler;
    private ListBox gRealEventListBox;
    public void sta_SetRTLogListBox(GetRealEventListBoxHandler gvHandler)
    {
        gRealEventListBoxHandler = gvHandler;
        gRealEventListBox = gRealEventListBoxHandler();
    }
    public void sta_UnRegRealTime()
    {
        this.axCZKEM1.OnFinger -= new zkemkeeper._IZKEMEvents_OnFingerEventHandler(axCZKEM1_OnFinger);
        this.axCZKEM1.OnVerify -= new zkemkeeper._IZKEMEvents_OnVerifyEventHandler(axCZKEM1_OnVerify);
        this.axCZKEM1.OnAttTransactionEx -= new zkemkeeper._IZKEMEvents_OnAttTransactionExEventHandler(axCZKEM1_OnAttTransactionEx);
        this.axCZKEM1.OnFingerFeature -= new zkemkeeper._IZKEMEvents_OnFingerFeatureEventHandler(axCZKEM1_OnFingerFeature);
        this.axCZKEM1.OnDeleteTemplate -= new zkemkeeper._IZKEMEvents_OnDeleteTemplateEventHandler(axCZKEM1_OnDeleteTemplate);
        this.axCZKEM1.OnNewUser -= new zkemkeeper._IZKEMEvents_OnNewUserEventHandler(axCZKEM1_OnNewUser);
        this.axCZKEM1.OnHIDNum -= new zkemkeeper._IZKEMEvents_OnHIDNumEventHandler(axCZKEM1_OnHIDNum);
        this.axCZKEM1.OnAlarm -= new zkemkeeper._IZKEMEvents_OnAlarmEventHandler(axCZKEM1_OnAlarm);
        this.axCZKEM1.OnDoor -= new zkemkeeper._IZKEMEvents_OnDoorEventHandler(axCZKEM1_OnDoor);
        this.axCZKEM1.OnEnrollFingerEx -= new zkemkeeper._IZKEMEvents_OnEnrollFingerExEventHandler(axCZKEM1_OnEnrollFingerEx);
        this.axCZKEM1.OnWriteCard += new zkemkeeper._IZKEMEvents_OnWriteCardEventHandler(axCZKEM1_OnWriteCard);
        this.axCZKEM1.OnEmptyCard += new zkemkeeper._IZKEMEvents_OnEmptyCardEventHandler(axCZKEM1_OnEmptyCard);
        this.axCZKEM1.OnHIDNum += new zkemkeeper._IZKEMEvents_OnHIDNumEventHandler(axCZKEM1_OnHIDNum);
        this.axCZKEM1.OnAttTransaction -= new zkemkeeper._IZKEMEvents_OnAttTransactionEventHandler(axCZKEM1_OnAttTransaction);
        this.axCZKEM1.OnKeyPress += new zkemkeeper._IZKEMEvents_OnKeyPressEventHandler(axCZKEM1_OnKeyPress);
        this.axCZKEM1.OnEnrollFinger += new zkemkeeper._IZKEMEvents_OnEnrollFingerEventHandler(axCZKEM1_OnEnrollFinger);
    }

    public int sta_GetUserInfo(ListBox lblOutputInfo, TextBox txtUserID, TextBox txtName, ComboBox cbPrivilege, TextBox txtCardnumber, TextBox txtPassword)
    {
        if (GetConnectState() == false)
        {
            lblOutputInfo.Items.Add("*Önce bağlantı kurulması lazım");
            return -1024;
        }

       

        int iPIN2Width = 0;
        string strTemp = "";
        axCZKEM1.GetSysOption(GetMachineNumber(), "~PIN2Width", out strTemp);
        iPIN2Width = Convert.ToInt32(strTemp);

       

        int idwErrorCode = 0;
        int iPrivilege = 0;
        string strName = "";
        string strCardno = "";
        string strPassword = "";
        bool bEnabled = false;

        axCZKEM1.EnableDevice(iMachineNumber, false);
        if (axCZKEM1.SSR_GetUserInfo(iMachineNumber, txtUserID.Text.Trim(), out strName, out strPassword, out iPrivilege, out bEnabled))//upload the user's information(card number included)
        {
            axCZKEM1.GetStrCardNumber(out strCardno);
            if (strCardno.Equals("0"))
            {
                strCardno = "";
            }
            txtName.Text = strName;
            txtPassword.Text = strPassword;
            txtCardnumber.Text = strCardno;
            cbPrivilege.SelectedIndex = iPrivilege;
            lblOutputInfo.Items.Add("Bilgiler Getirildi");
        }
        else
        {
            axCZKEM1.GetLastError(ref idwErrorCode);
            //modify by Leonard 2017/12/18
            txtName.Text = " ";
            txtPassword.Text = " ";
            txtCardnumber.Text = " ";
            cbPrivilege.SelectedIndex = 5;
            lblOutputInfo.Items.Add("The User is not exist");
            //end by Leonard
            lblOutputInfo.Items.Add("*Operation failed,ErrorCode=" + idwErrorCode.ToString());
        }
        axCZKEM1.EnableDevice(iMachineNumber, true);

        return 1;
    }
    public int sta_RegRealTime(ListBox lblOutputInfo)
    {
        if (GetConnectState() == false)
        {
            lblOutputInfo.Items.Add("*Önce Terminal bağlantısını kurunuz!");
            return -1024;
        }
        int ret = 0;
        if (axCZKEM1.RegEvent(GetMachineNumber(), 65535))//Here you can register the realtime events that you want to be triggered(the parameters 65535 means registering all)
        {
            //common interface
            this.axCZKEM1.OnFinger += new zkemkeeper._IZKEMEvents_OnFingerEventHandler(axCZKEM1_OnFinger);
            this.axCZKEM1.OnVerify += new zkemkeeper._IZKEMEvents_OnVerifyEventHandler(axCZKEM1_OnVerify);
            this.axCZKEM1.OnFingerFeature += new zkemkeeper._IZKEMEvents_OnFingerFeatureEventHandler(axCZKEM1_OnFingerFeature);
            this.axCZKEM1.OnDeleteTemplate += new zkemkeeper._IZKEMEvents_OnDeleteTemplateEventHandler(axCZKEM1_OnDeleteTemplate);
            this.axCZKEM1.OnNewUser += new zkemkeeper._IZKEMEvents_OnNewUserEventHandler(axCZKEM1_OnNewUser);
            this.axCZKEM1.OnHIDNum += new zkemkeeper._IZKEMEvents_OnHIDNumEventHandler(axCZKEM1_OnHIDNum);
            this.axCZKEM1.OnAlarm += new zkemkeeper._IZKEMEvents_OnAlarmEventHandler(axCZKEM1_OnAlarm);
            this.axCZKEM1.OnDoor += new zkemkeeper._IZKEMEvents_OnDoorEventHandler(axCZKEM1_OnDoor);
            //only for color device
            this.axCZKEM1.OnAttTransactionEx += new zkemkeeper._IZKEMEvents_OnAttTransactionExEventHandler(axCZKEM1_OnAttTransactionEx);
            this.axCZKEM1.OnEnrollFingerEx += new zkemkeeper._IZKEMEvents_OnEnrollFingerExEventHandler(axCZKEM1_OnEnrollFingerEx);
            //only for black&white device
            this.axCZKEM1.OnAttTransaction -= new zkemkeeper._IZKEMEvents_OnAttTransactionEventHandler(axCZKEM1_OnAttTransaction);
            this.axCZKEM1.OnWriteCard += new zkemkeeper._IZKEMEvents_OnWriteCardEventHandler(axCZKEM1_OnWriteCard);
            this.axCZKEM1.OnEmptyCard += new zkemkeeper._IZKEMEvents_OnEmptyCardEventHandler(axCZKEM1_OnEmptyCard);
            this.axCZKEM1.OnKeyPress += new zkemkeeper._IZKEMEvents_OnKeyPressEventHandler(axCZKEM1_OnKeyPress);
            this.axCZKEM1.OnEnrollFinger += new zkemkeeper._IZKEMEvents_OnEnrollFingerEventHandler(axCZKEM1_OnEnrollFinger);
            ret = 1;
        }
        else
        {
            axCZKEM1.GetLastError(ref idwErrorCode);
            ret = idwErrorCode;
            if (idwErrorCode != 0)
            {
                lblOutputInfo.Items.Add("*RegEvent failed,ErrorCode: " + idwErrorCode.ToString());
            }
            else
            {
                lblOutputInfo.Items.Add("*Terminal verisi bulunamadı!");
            }
        }
        return ret;
    }
    //When you are enrolling your finger,this event will be triggered.
    void axCZKEM1_OnEnrollFingerEx(string EnrollNumber, int FingerIndex, int ActionResult, int TemplateLength)
    {
        if (ActionResult == 0)
        {
            gRealEventListBox.Items.Add("Enroll finger succeed. UserID=" + EnrollNumber.ToString() + "...FingerIndex=" + FingerIndex.ToString());
        }
        else
        {
            gRealEventListBox.Items.Add("Enroll finger failed. Result=" + ActionResult.ToString());
        }
        throw new NotImplementedException();
    }
    //Door sensor event
    void axCZKEM1_OnDoor(int EventType)
    {
        gRealEventListBox.Items.Add("Door opened" + "...EventType=" + EventType.ToString());
        throw new NotImplementedException();
    }
    //When the dismantling machine or duress alarm occurs, trigger this event.
    void axCZKEM1_OnAlarm(int AlarmType, int EnrollNumber, int Verified)
    {
        gRealEventListBox.Items.Add("Alarm triggered" + "...AlarmType=" + AlarmType.ToString() + "...EnrollNumber=" + EnrollNumber.ToString() + "...Verified=" + Verified.ToString());
        throw new NotImplementedException();
    }
    //When you swipe a card to the device, this event will be triggered to show you the card number.
    void axCZKEM1_OnHIDNum(int CardNumber)
    {
        gRealEventListBox.Items.Add("Card event" + "...Cardnumber=" + CardNumber.ToString());
        throw new NotImplementedException();
    }
    //When you have enrolled a new user,this event will be triggered.
    void axCZKEM1_OnNewUser(int EnrollNumber)
    {
        gRealEventListBox.Items.Add("Enroll a　new user" + "...UserID=" + EnrollNumber.ToString());
        throw new NotImplementedException();
    }
    //When you have deleted one one fingerprint template,this event will be triggered.
    void axCZKEM1_OnDeleteTemplate(int EnrollNumber, int FingerIndex)
    {
        gRealEventListBox.Items.Add("Delete a finger template" + "...UserID=" + EnrollNumber.ToString() + "..FingerIndex=" + FingerIndex.ToString());
        throw new NotImplementedException();
    }
    //When you have enrolled your finger,this event will be triggered and return the quality of the fingerprint you have enrolled
    void axCZKEM1_OnFingerFeature(int Score)
    {
        gRealEventListBox.Items.Add("Press finger score=" + Score.ToString());
        throw new NotImplementedException();
    }
    //If your fingerprint(or your card) passes the verification,this event will be triggered,only for color device
    void axCZKEM1_OnAttTransactionEx(string EnrollNumber, int IsInValid, int AttState, int VerifyMethod, int Year, int Month, int Day, int Hour, int Minute, int Second, int WorkCode)
    {
        string time = Year + "-" + Month + "-" + Day + " " + Hour + ":" + Minute + ":" + Second;
        gRealEventListBox.Items.Add("Verify OK.UserID=" + EnrollNumber + " isInvalid=" + IsInValid.ToString() + " state=" + AttState.ToString() + " verifystyle=" + VerifyMethod.ToString() + " time=" + time);
        throw new NotImplementedException();
    }
    //If your fingerprint(or your card) passes the verification,this event will be triggered,only for black%white device
    private void axCZKEM1_OnAttTransaction(int EnrollNumber, int IsInValid, int AttState, int VerifyMethod, int Year, int Month, int Day, int Hour, int Minute, int Second)
    {
        string time = Year + "-" + Month + "-" + Day + " " + Hour + ":" + Minute + ":" + Second;
        gRealEventListBox.Items.Add("Verify OK.UserID=" + EnrollNumber.ToString() + " isInvalid=" + IsInValid.ToString() + " state=" + AttState.ToString() + " verifystyle=" + VerifyMethod.ToString() + " time=" + time);
        throw new NotImplementedException();
    }
    //After you have placed your finger on the sensor(or swipe your card to the device),this event will be triggered.
    //If you passes the verification,the returned value userid will be the user enrollnumber,or else the value will be -1;
    void axCZKEM1_OnVerify(int UserID)
    {
        if (UserID != -1)
        {
            gRealEventListBox.Items.Add("User fingerprint verified... UserID=" + UserID.ToString());
        }
        else
        {
            gRealEventListBox.Items.Add("Failed to verify... ");
        }
        throw new NotImplementedException();
    }
    //When you place your finger on sensor of the device,this event will be triggered
    void axCZKEM1_OnFinger()
    {
        gRealEventListBox.Items.Add("OnFinger event ");
        throw new NotImplementedException();
    }
    //When you have written into the Mifare card ,this event will be triggered.
    void axCZKEM1_OnWriteCard(int iEnrollNumber, int iActionResult, int iLength)
    {
        if (iActionResult == 0)
        {
            gRealEventListBox.Items.Add("Write Mifare Card OK" + "...EnrollNumber=" + iEnrollNumber.ToString() + "...TmpLength=" + iLength.ToString());
        }
        else
        {
            gRealEventListBox.Items.Add("...Write Failed");
        }
    }
    //When you have emptyed the Mifare card,this event will be triggered.
    void axCZKEM1_OnEmptyCard(int iActionResult)
    {
        if (iActionResult == 0)
        {
            gRealEventListBox.Items.Add("Empty Mifare Card OK...");
        }
        else
        {
            gRealEventListBox.Items.Add("Empty Failed...");
        }
    }
    //When you press the keypad,this event will be triggered.
    void axCZKEM1_OnKeyPress(int iKey)
    {
        gRealEventListBox.Items.Add("RTEvent OnKeyPress Has been Triggered, Key: " + iKey.ToString());
    }
    //When you are enrolling your finger,this event will be triggered.
    void axCZKEM1_OnEnrollFinger(int EnrollNumber, int FingerIndex, int ActionResult, int TemplateLength)
    {
        if (ActionResult == 0)
        {
            gRealEventListBox.Items.Add("Enroll finger succeed. UserID=" + EnrollNumber + "...FingerIndex=" + FingerIndex.ToString());
        }
        else
        {
            gRealEventListBox.Items.Add("Enroll finger failed. Result=" + ActionResult.ToString());
        }
        throw new NotImplementedException();
    }
    #endregion
    #region KullaniciBilgileri
    public int sta_DeleteEnrollData(ListBox lblOutputInfo, string cbUseID, string cbBackupDE)
    {
        if (GetConnectState() == false)
        {
            lblOutputInfo.Items.Add("*Önce Terminal bağlantısını kurunuz!");
            return -1024;
        }
        if (cbUseID.Trim() == "" || cbBackupDE.Trim() == "")
        {
            lblOutputInfo.Items.Add("*Please input data first!");
            return -1023;
        }
        int idwErrorCode = 0;
        string sUserID = cbUseID.Trim();
        int iBackupNumber = Convert.ToInt32(cbBackupDE.Trim());
        if (axCZKEM1.SSR_DeleteEnrollData(iMachineNumber, sUserID, iBackupNumber))
        {
            axCZKEM1.RefreshData(iMachineNumber);//the data in the device should be refreshed
            lblOutputInfo.Items.Add("SSR_DeleteEnrollData,UserID=" + sUserID + " BackupNumber=" + iBackupNumber.ToString());
        }
        else
        {
            axCZKEM1.GetLastError(ref idwErrorCode);
            if (idwErrorCode == 0 && iBackupNumber == 11)
                lblOutputInfo.Items.Add("SSR_DeleteEnrollData,UserID=" + sUserID + " BackupNumber=" + iBackupNumber.ToString());
            else
                lblOutputInfo.Items.Add("*Operation failed,ErrorCode=" + idwErrorCode.ToString());
        }
        return 1;
    }
    public int sta_OnlineEnroll(ListBox lblOutputInfo, TextBox txtUserID, string cbFingerIndex, string cbFlag)
    {
        if (GetConnectState() == false)
        {
            lblOutputInfo.Items.Add("*Önce Terminal bağlantısını kurunuz!");
            return -1024;
        }
        if (txtUserID.Text.Trim() == "" || cbFingerIndex.Trim() == "" || cbFlag.Trim() == "")
        {
            lblOutputInfo.Items.Add("*Please input data first!");
            return -1023;
        }
        int iPIN2Width = 0;
        int iIsABCPinEnable = 0;
        int iT9FunOn = 0;
        string strTemp = "";
        axCZKEM1.GetSysOption(GetMachineNumber(), "~PIN2Width", out strTemp);
        iPIN2Width = Convert.ToInt32(strTemp);
        axCZKEM1.GetSysOption(GetMachineNumber(), "~IsABCPinEnable", out strTemp);
        iIsABCPinEnable = Convert.ToInt32(strTemp);
        axCZKEM1.GetSysOption(GetMachineNumber(), "~T9FunOn", out strTemp);
        iT9FunOn = Convert.ToInt32(strTemp);
        /*
        axCZKEM1.GetDeviceInfo(iMachineNumber, 76, ref iPIN2Width);
        axCZKEM1.GetDeviceInfo(iMachineNumber, 77, ref iIsABCPinEnable);
        axCZKEM1.GetDeviceInfo(iMachineNumber, 78, ref iT9FunOn);
         */
        if (txtUserID.Text.Length > iPIN2Width)
        {
            lblOutputInfo.Items.Add("*User ID error! The max length is " + iPIN2Width.ToString());
            return -1022;
        }
        if (iIsABCPinEnable == 0 || iT9FunOn == 0)
        {
            if (txtUserID.Text.Substring(0, 1) == "0")
            {
                lblOutputInfo.Items.Add("*User ID error! The first letter can not be as 0");
                return -1022;
            }
            foreach (char tempchar in txtUserID.Text.ToCharArray())
            {
                if (!(char.IsDigit(tempchar)))
                {
                    lblOutputInfo.Items.Add("*User ID error! User ID only support digital");
                    return -1022;
                }
            }
        }
        int idwErrorCode = 0;
        string sUserID = txtUserID.Text.Trim();
        int iFingerIndex = Convert.ToInt32(cbFingerIndex.Trim());
        int iFlag = Convert.ToInt32(cbFlag.Trim());
        axCZKEM1.CancelOperation();
        //If the specified index of user's templates has existed ,delete it first
        axCZKEM1.SSR_DelUserTmpExt(iMachineNumber, sUserID, iFingerIndex);
        if (axCZKEM1.StartEnrollEx(sUserID, iFingerIndex, iFlag))
        {
            lblOutputInfo.Items.Add("Start to Enroll a new User,UserID=" + sUserID + " FingerID=" + iFingerIndex.ToString() + " Flag=" + iFlag.ToString());
            if (axCZKEM1.StartIdentify())
            {
                lblOutputInfo.Items.Add("Enroll a new User,UserID" + sUserID);
            }
            ;//After enrolling templates,you should let the device into the 1:N verification condition
        }
        else
        {
            axCZKEM1.GetLastError(ref idwErrorCode);
            lblOutputInfo.Items.Add("*Operation failed,ErrorCode=" + idwErrorCode.ToString());
        }
        return 1;
    }
    public int sta_SetUserInfo(ListBox lblOutputInfo, TextBox txtUserID, TextBox txtName,  TextBox txtCardnumber)
    {
       
        int iPIN2Width = 0;
        int iIsABCPinEnable = 0;
        int iT9FunOn = 0;
        string strTemp = "";
        axCZKEM1.GetSysOption(GetMachineNumber(), "~PIN2Width", out strTemp);
        iPIN2Width = Convert.ToInt32(strTemp);
        axCZKEM1.GetSysOption(GetMachineNumber(), "~IsABCPinEnable", out strTemp);
        iIsABCPinEnable = Convert.ToInt32(strTemp);
        axCZKEM1.GetSysOption(GetMachineNumber(), "~T9FunOn", out strTemp);
        iT9FunOn = Convert.ToInt32(strTemp);
    
        if (txtUserID.Text.Length > iPIN2Width)
        {
            lblOutputInfo.Items.Add("*User ID hatası Maksimum değer üzerinde " + iPIN2Width.ToString());
            return -1022;
        }
        if (iIsABCPinEnable == 0 || iT9FunOn == 0)
        {
            if (txtUserID.Text.Substring(0, 1) == "0")
            {
                lblOutputInfo.Items.Add("*User ID hatas! 0 User Id olamaz");
                return -1022;
            }
            foreach (char tempchar in txtUserID.Text.ToCharArray())
            {
                if (!(char.IsDigit(tempchar)))
                {
                    lblOutputInfo.Items.Add("*User ID hatası! User ID sadece sayı olabilir.");
                    return -1022;
                }
            }
        }
        int idwErrorCode = 0;
        string sdwEnrollNumber = txtUserID.Text.Trim();
        string sName = txtName.Text.Trim();
        string sCardnumber = txtCardnumber.Text.Trim();
        bool bEnabled = true;
        /*if (iPrivilege == 4)
        {
            bEnabled = false;
            iPrivilege = 0;
        }
        else
        {
            bEnabled = true;
        }*/
        axCZKEM1.EnableDevice(iMachineNumber, false);
        axCZKEM1.SetStrCardNumber(sCardnumber);//Before you using function SetUserInfo,set the card number to make sure you can upload it to the device
        if (axCZKEM1.SSR_SetUserInfo(iMachineNumber, sdwEnrollNumber, sName, "0", 0, bEnabled))//upload the user's information(card number included)
        {
            lblOutputInfo.Items.Add("Kullanıcı Bilgisi Düzenlendi");
        }
        else
        {
            axCZKEM1.GetLastError(ref idwErrorCode);
            lblOutputInfo.Items.Add("*İşlem hatası,ErrorCode=" + idwErrorCode.ToString());
        }
        axCZKEM1.RefreshData(iMachineNumber);//the data in the device should be refreshed
        axCZKEM1.EnableDevice(iMachineNumber, true);
        return 1;
    }
    public int sta_GetUserInfo(ListBox lblOutputInfo, TextBox txtUserID, TextBox txtName, string cbPrivilege, TextBox txtCardnumber, TextBox txtPassword)
    {
        if (GetConnectState() == false)
        {
            lblOutputInfo.Items.Add("*Önce Terminal bağlantısını kurunuz!");
            return -1024;
        }
        if (txtUserID.Text.Trim() == "")
        {
            lblOutputInfo.Items.Add("*Please input user id first!");
            return -1023;
        }
        int iPIN2Width = 0;
        string strTemp = "";
        axCZKEM1.GetSysOption(GetMachineNumber(), "~PIN2Width", out strTemp);
        iPIN2Width = Convert.ToInt32(strTemp);
        if (txtUserID.Text.Length > iPIN2Width)
        {
            lblOutputInfo.Items.Add("*User ID error! The max length is " + iPIN2Width.ToString());
            return -1022;
        }
        int idwErrorCode = 0;
        int iPrivilege = 0;
        string strName = "";
        string strCardno = "";
        string strPassword = "";
        bool bEnabled = false;
        axCZKEM1.EnableDevice(iMachineNumber, false);
        if (axCZKEM1.SSR_GetUserInfo(iMachineNumber, txtUserID.Text.Trim(), out strName, out strPassword, out iPrivilege, out bEnabled))//upload the user's information(card number included)
        {
            axCZKEM1.GetStrCardNumber(out strCardno);
            if (strCardno.Equals("0"))
            {
                strCardno = "";
            }
            txtName.Text = strName;
            txtPassword.Text = strPassword;
            txtCardnumber.Text = strCardno;
            //cbPrivilege.SelectedIndex = iPrivilege;
            lblOutputInfo.Items.Add("Get user information successfully");
        }
        else
        {
            axCZKEM1.GetLastError(ref idwErrorCode);
            //modify by Leonard 2017/12/18
            txtName.Text = " ";
            txtPassword.Text = " ";
            txtCardnumber.Text = " ";
           // cbPrivilege.SelectedIndex = 5;
            lblOutputInfo.Items.Add("The User is not exist");
            //end by Leonard
            lblOutputInfo.Items.Add("*Operation failed,ErrorCode=" + idwErrorCode.ToString());
        }
        axCZKEM1.EnableDevice(iMachineNumber, true);
        return 1;
    }
    public int sta_GetHIDEventCardNum(ListBox lblOutputInfo)
    {
        if (GetConnectState() == false)
        {
            lblOutputInfo.Items.Add("*Önce Terminal bağlantısını kurunuz!");
            return -1024;
        }
        int idwErrorCode = 0;
        string strHIDEventCardNum = "";
        if (axCZKEM1.GetHIDEventCardNumAsStr(out strHIDEventCardNum))
        {
            lblOutputInfo.Items.Add("GetHIDEventCardNumAsStr! HIDCardNum=" + strHIDEventCardNum);
        }
        else
        {
            axCZKEM1.GetLastError(ref idwErrorCode);
            lblOutputInfo.Items.Add("*Operation failed,ErrorCode=" + idwErrorCode.ToString());
        }
        return 1;
    }


    #region UserFace
    //public int sta_GetAllUserFaceInfo(ListBox lblOutputInfo, ProgressBar prgSta, ListView lvUserInfo)
    //{
    //    if (GetConnectState() == false)
    //    {
    //        lblOutputInfo.Items.Add("*Önce Terminal bağlantısını kurunuz!");
    //        return -1024;
    //    }
    //    string sEnrollNumber = "";
    //    string sName = "";
    //    string sPassword = "";
    //    int iPrivilege = 0;
    //    bool bEnabled = false;
    //    int iFaceIndex = 50;//the only possible parameter value
    //    string sTmpData = "";
    //    int iLength = 0;
    //    int num = 0;
    //    int index = 0;
    //    lvUserInfo.Items.Clear();
    //    axCZKEM1.EnableDevice(iMachineNumber, false);
    //    axCZKEM1.ReadAllUserID(iMachineNumber);//read all the user information to the memory
    //    while (axCZKEM1.SSR_GetAllUserInfo(iMachineNumber, out sEnrollNumber, out sName, out sPassword, out iPrivilege, out bEnabled))//get all the users' information from the memory
    //    {
    //        if (axCZKEM1.GetUserFaceStr(iMachineNumber, sEnrollNumber, iFaceIndex, ref sTmpData, ref iLength))//get the face templates from the memory
    //        {
    //            lvUserInfo.Items.Add(sEnrollNumber);
    //            if (bEnabled == true)
    //            {
    //                lvUserInfo.Items[index].SubItems.Add("true");
    //            }
    //            else
    //            {
    //                lvUserInfo.Items[index].SubItems.Add("false");
    //            }
    //            lvUserInfo.Items[index].SubItems.Add(sName);
    //            lvUserInfo.Items[index].SubItems.Add(sPassword);
    //            lvUserInfo.Items[index].SubItems.Add(iPrivilege.ToString());
    //            lvUserInfo.Items[index].SubItems.Add(iLength.ToString());
    //            lvUserInfo.Items[index].SubItems.Add(sTmpData);
    //            index++;
    //            num++;
    //        }
    //        prgSta.Value = num % 100;
    //    }
    //    prgSta.Value = 100;
    //    lblOutputInfo.Items.Add("Download user  face count : " + num.ToString());
    //    axCZKEM1.EnableDevice(iMachineNumber, true);
    //    return 1;
    //}
    //Upload user's face templates to device
    //public int sta_SetAllUserFaceInfo(ListBox lblOutputInfo, ProgressBar prgSta, ListView lvUserInfo)
    //{
    //    if (GetConnectState() == false)
    //    {
    //        lblOutputInfo.Items.Add("*Önce Terminal bağlantısını kurunuz!");
    //        return -1024;
    //    }
    //    if (lvUserInfo.Items.Count == 0)
    //    {
    //        lblOutputInfo.Items.Add("*There is no data can upload!");
    //        return -1023;
    //    }
    //    string sEnrollNumber = "";
    //    string sEnabled = "";
    //    bool bEnabled = false;
    //    string sName = "";
    //    string sPassword = "";
    //    int iPrivilege = 0;
    //    string sTmpData = "";
    //    int iLength = 0;
    //    int iFaceIndex = 50;
    //    int num = 0;
    //    axCZKEM1.EnableDevice(iMachineNumber, false);
    //    for (int i = 0; i < lvUserInfo.Items.Count; i++)
    //    {
    //        sEnrollNumber = lvUserInfo.Items[i].SubItems[0].Text;
    //        sEnabled = lvUserInfo.Items[i].SubItems[1].Text;
    //        if (sEnabled == "true")
    //        {
    //            bEnabled = true;
    //        }
    //        else
    //        {
    //            bEnabled = false;
    //        }
    //        sName = lvUserInfo.Items[i].SubItems[2].Text;
    //        sPassword = lvUserInfo.Items[i].SubItems[3].Text;
    //        iPrivilege = Convert.ToInt32(lvUserInfo.Items[i].SubItems[4].Text);
    //        iLength = Convert.ToInt32(lvUserInfo.Items[i].SubItems[5].Text);
    //        sTmpData = lvUserInfo.Items[i].SubItems[6].Text;
    //        if (axCZKEM1.SSR_SetUserInfo(iMachineNumber, sEnrollNumber, sName, sPassword, iPrivilege, bEnabled))//upload user information to the device
    //        {
    //            axCZKEM1.SetUserFaceStr(iMachineNumber, sEnrollNumber, iFaceIndex, sTmpData, iLength);//upload face templates information to the device
    //            num++;
    //            prgSta.Value = num % 100;
    //        }
    //        else
    //        {
    //            axCZKEM1.GetLastError(ref idwErrorCode);
    //            lblOutputInfo.Items.Add("*Operation failed,ErrorCode=!" + idwErrorCode.ToString());
    //            axCZKEM1.EnableDevice(iMachineNumber, true);
    //            return -1022;
    //        }
    //    }
    //    prgSta.Value = 100;
    //    axCZKEM1.RefreshData(iMachineNumber);//the data in the device should be refreshed
    //    axCZKEM1.EnableDevice(iMachineNumber, true);
    //    lblOutputInfo.Items.Add("Upload face successfully");
    //    return 1;
    //}
    #endregion
    #region UserPhoto
    //public int sta_DownloadAllUserPhoto(ListBox lblOutputInfo, TextBox txtAllPhotoPath)
    //{
    //    if (GetConnectState() == false)
    //    {
    //        lblOutputInfo.Items.Add("*Önce Terminal bağlantısını kurunuz!");
    //        return -1024;
    //    }
    //    if (txtAllPhotoPath.Text.Trim() == "")
    //    {
    //        lblOutputInfo.Items.Add("*Select photo path first.");
    //        return -1023;
    //    }
    //    int ret = 0;
    //    string photoPath = txtAllPhotoPath.Text.Trim();
    //    axCZKEM1.EnableDevice(iMachineNumber, false);
    //    if (axCZKEM1.GetAllUserPhoto(1, photoPath))
    //    {
    //        ret = 1;
    //        lblOutputInfo.Items.Add("Get All User Photo From the Device!");
    //    }
    //    else
    //    {
    //        axCZKEM1.GetLastError(ref idwErrorCode);
    //        ret = idwErrorCode;
    //        if (idwErrorCode != 0)
    //        {
    //            lblOutputInfo.Items.Add("*Download all user photo failed,ErrorCode: " + idwErrorCode.ToString());
    //        }
    //        else
    //        {
    //            lblOutputInfo.Items.Add("Terminal verisi bulunamadı!");
    //        }
    //    }
    //    //lblOutputInfo.Items.Add("[func GetAllUserPhoto]Temporarily unsupported");
    //    axCZKEM1.EnableDevice(GetMachineNumber(), true);//enable the device
    //    return ret;
    //}

    #endregion
    #region SMS
    public int sta_GetSMS(ListBox lblOutputInfo, TextBox txtSMSID, string cbTag, TextBox txtValidMin, DateTime dtStartTime, TextBox txtContent)
    {
        if (GetConnectState() == false)
        {
            lblOutputInfo.Items.Add("*Önce Terminal bağlantısını kurunuz!");
            return -1024;
        }
        if (txtSMSID.Text.Trim() == "")
        {
            lblOutputInfo.Items.Add("*Please input data first!");
            return -1023;
        }
        int idwErrorCode = 0;
        int iSMSID = Convert.ToInt32(txtSMSID.Text.Trim());
        int iTag = 0;
        int iValidMins = 0;
        string sStartTime = "";
        string sContent = "";
        axCZKEM1.EnableDevice(iMachineNumber, false);
        if (axCZKEM1.GetSMS(iMachineNumber, iSMSID, ref iTag, ref iValidMins, ref sStartTime, ref sContent))
        {
            switch (iTag)
            {
                case 253: cbTag = "0"; break;
                case 254: cbTag = "1"; break;
                case 255: cbTag = "2"; break;
            }
            txtSMSID.Text = iSMSID.ToString();
            cbTag = iTag.ToString();
            txtValidMin.Text = iValidMins.ToString();
            dtStartTime = Convert.ToDateTime(sStartTime);
            txtContent.Text = sContent;
            lblOutputInfo.Items.Add("Get SMS successfully");
        }
        else
        {
            axCZKEM1.GetLastError(ref idwErrorCode);
            lblOutputInfo.Items.Add("*Operation failed,ErrorCode=" + idwErrorCode.ToString());
        }
        axCZKEM1.EnableDevice(iMachineNumber, true);
        return 1;
    }
    public int sta_SetSMS(ListBox lblOutputInfo, TextBox txtSMSID, string cbTag, TextBox txtValidMin, DateTime dtStartTime, TextBox txtContent)
    {
        if (GetConnectState() == false)
        {
            lblOutputInfo.Items.Add("*Önce Terminal bağlantısını kurunuz!");
            return -1024;
        }
        if (txtSMSID.Text.Trim() == "" || cbTag.Trim() == "" || txtValidMin.Text.Trim() == "" || dtStartTime.ToString() == "" || txtContent.Text.Trim() == "")
        {
            lblOutputInfo.Items.Add("*Please input data first!");
            return -1023;
        }
        if (Convert.ToInt32(txtSMSID.Text.Trim()) <= 0)
        {
            lblOutputInfo.Items.Add("*SMS ID error!");
            return -1023;
        }
        if (Convert.ToInt32(txtValidMin.Text.Trim()) < 0 || Convert.ToInt32(txtValidMin.Text.Trim()) > 65535)
        {
            lblOutputInfo.Items.Add("*Expired time error!");
            return -1023;
        }
        int idwErrorCode = 0;
        int iSMSID = Convert.ToInt32(txtSMSID.Text.Trim());
        int iTag = 0;
        int iValidMins = Convert.ToInt32(txtValidMin.Text.Trim());
        string sStartTime = dtStartTime.ToString();
        string sContent = txtContent.Text.Trim();
        string sTag = cbTag.Trim();
        for (iTag = 253; iTag <= 255; iTag++)
        {
            if (sTag.IndexOf(iTag.ToString()) > -1)
            {
                break;
            }
        }
        axCZKEM1.EnableDevice(iMachineNumber, false);
        if (axCZKEM1.SetSMS(iMachineNumber, iSMSID, iTag, iValidMins, sStartTime, sContent))
        {
            axCZKEM1.RefreshData(iMachineNumber);//After you have set the short message,you should refresh the data of the device
            lblOutputInfo.Items.Add("Successfully set SMS! SMSType=" + iTag.ToString());
        }
        else
        {
            axCZKEM1.GetLastError(ref idwErrorCode);
            lblOutputInfo.Items.Add("*Operation failed,ErrorCode=" + idwErrorCode.ToString());
        }
        axCZKEM1.EnableDevice(iMachineNumber, true);
        return 1;
    }
    public int sta_SetUserSMS(ListBox lblOutputInfo, TextBox txtSMSID, string cbUserID)
    {
        if (GetConnectState() == false)
        {
            lblOutputInfo.Items.Add("*Önce Terminal bağlantısını kurunuz!");
            return -1024;
        }
        if (txtSMSID.Text.Trim() == "" || cbUserID.Trim() == "")
        {
            lblOutputInfo.Items.Add("*Please input data first!");
            return -1023;
        }
        int idwErrorCode = 0;
        int iSMSID = Convert.ToInt32(txtSMSID.Text.Trim());
        int iTag = 0;
        int iValidMins = 0;
        string sStartTime = "";
        string sContent = "";
        string sEnrollNumber = cbUserID.Trim();
        axCZKEM1.EnableDevice(iMachineNumber, false);
        if (axCZKEM1.GetSMS(iMachineNumber, iSMSID, ref iTag, ref iValidMins, ref sStartTime, ref sContent) == false)
        {
            lblOutputInfo.Items.Add("*The SMSID doesn't exist!!");
            axCZKEM1.EnableDevice(iMachineNumber, true);
            return -1022;
        }
        if (iTag != 254)
        {
            lblOutputInfo.Items.Add("*The SMS does not Personal SMS,please set it as Personal SMS first!!");
            axCZKEM1.EnableDevice(iMachineNumber, true);
            return -1022;
        }
        if (axCZKEM1.SSR_SetUserSMS(iMachineNumber, sEnrollNumber, iSMSID))
        {
            axCZKEM1.RefreshData(iMachineNumber);//After you have set user short message,you should refresh the data of the device
            lblOutputInfo.Items.Add("Successfully set user SMS! ");
        }
        else
        {
            axCZKEM1.GetLastError(ref idwErrorCode);
            lblOutputInfo.Items.Add("*Operation failed,ErrorCode=" + idwErrorCode.ToString());
        }
        axCZKEM1.EnableDevice(iMachineNumber, true);
        return 1;
    }
    public int sta_DelSMS(ListBox lblOutputInfo, TextBox txtSMSID)
    {
        if (GetConnectState() == false)
        {
            lblOutputInfo.Items.Add("*Önce Terminal bağlantısını kurunuz!");
            return -1024;
        }
        if (txtSMSID.Text.Trim() == "")
        {
            lblOutputInfo.Items.Add("*Please input data first!");
            return -1023;
        }
        int idwErrorCode = 0;
        int iSMSID = Convert.ToInt32(txtSMSID.Text.Trim());
        axCZKEM1.EnableDevice(iMachineNumber, false);
        if (axCZKEM1.DeleteSMS(iMachineNumber, iSMSID))
        {
            axCZKEM1.RefreshData(iMachineNumber);//After you have set user short message,you should refresh the data of the device
            lblOutputInfo.Items.Add("Successfully delete SMS! ");
        }
        else
        {
            axCZKEM1.GetLastError(ref idwErrorCode);
            lblOutputInfo.Items.Add("*Operation failed,ErrorCode=" + idwErrorCode.ToString());
        }
        axCZKEM1.EnableDevice(iMachineNumber, true);
        return 1;
    }
    public int sta_ClearSMS(ListBox lblOutputInfo)
    {
        if (GetConnectState() == false)
        {
            lblOutputInfo.Items.Add("*Önce Terminal bağlantısını kurunuz!");
            return -1024;
        }
        int idwErrorCode = 0;
        axCZKEM1.EnableDevice(iMachineNumber, false);
        if (axCZKEM1.ClearSMS(iMachineNumber))
        {
            axCZKEM1.RefreshData(iMachineNumber);//After you have set user short message,you should refresh the data of the device
            lblOutputInfo.Items.Add("Successfully clear all the SMS! ");
        }
        else
        {
            axCZKEM1.GetLastError(ref idwErrorCode);
            lblOutputInfo.Items.Add("*Operation failed,ErrorCode=" + idwErrorCode.ToString());
        }
        axCZKEM1.EnableDevice(iMachineNumber, true);
        return 1;
    }
    public int sta_ClearUserSMS(ListBox lblOutputInfo)
    {
        if (GetConnectState() == false)
        {
            lblOutputInfo.Items.Add("*Önce Terminal bağlantısını kurunuz!");
            return -1024;
        }
        int idwErrorCode = 0;
        axCZKEM1.EnableDevice(iMachineNumber, false);
        if (axCZKEM1.ClearUserSMS(iMachineNumber))
        {
            axCZKEM1.RefreshData(iMachineNumber);//After you have set user short message,you should refresh the data of the device
            lblOutputInfo.Items.Add("Successfully clear all the user SMS! ");
        }
        else
        {
            axCZKEM1.GetLastError(ref idwErrorCode);
            lblOutputInfo.Items.Add("*Operation failed,ErrorCode=" + idwErrorCode.ToString());
        }
        axCZKEM1.EnableDevice(iMachineNumber, true);
        return 1;
    }
    #endregion
    #region Workcode
    public int sta_GetWorkCode(ListBox lblOutputInfo, TextBox txtWorkcodeID, TextBox txtWorkcodeName)
    {
        if (GetConnectState() == false)
        {
            lblOutputInfo.Items.Add("*Önce Terminal bağlantısını kurunuz!");
            return -1024;
        }
        if (txtWorkcodeID.Text.Trim() == "")
        {
            lblOutputInfo.Items.Add("*Please input data first!");
            return -1023;
        }
        int idwErrorCode = 0;
        int iWorkcodeID = Convert.ToInt32(txtWorkcodeID.Text.Trim());
        string sName = "";
        axCZKEM1.EnableDevice(iMachineNumber, false);
        if (axCZKEM1.SSR_GetWorkCode(iWorkcodeID, out sName))
        {
            txtWorkcodeName.Text = sName;
            lblOutputInfo.Items.Add("Get workcode successfully");
        }
        else
        {
            axCZKEM1.GetLastError(ref idwErrorCode);
            lblOutputInfo.Items.Add("*Operation failed,ErrorCode=" + idwErrorCode.ToString());
        }
        axCZKEM1.EnableDevice(iMachineNumber, true);
        return 1;
    }
    public int sta_SetWorkCode(ListBox lblOutputInfo, TextBox txtWorkcodeID, TextBox txtWorkcodeName)
    {
        if (GetConnectState() == false)
        {
            lblOutputInfo.Items.Add("*Önce Terminal bağlantısını kurunuz!");
            return -1024;
        }
        if (txtWorkcodeID.Text.Trim() == "" || txtWorkcodeName.Text.Trim() == "")
        {
            lblOutputInfo.Items.Add("*Please input data first!");
            return -1023;
        }
        int idwErrorCode = 0;
        int iTmpID = 0;
        int iWorkcodeID = Convert.ToInt32(txtWorkcodeID.Text.Trim());
        string sName = txtWorkcodeName.Text.Trim();
        /*
        axCZKEM1.SSR_GetWorkCodeIDByName(iMachineNumber, sName, out iTmpID);
        if (iTmpID > 0)
        {
            lblOutputInfo.Items.Add("*Workcode AdiSoyadi duplicated!");
            return -1022;
        }
        axCZKEM1.EnableDevice(iMachineNumber, false);
        if (axCZKEM1.SSR_SetWorkCode(iWorkcodeID, sName))
        {
            lblOutputInfo.Items.Add("Successfully set workcode");
        }
        else
        {
            axCZKEM1.GetLastError(ref idwErrorCode);
            lblOutputInfo.Items.Add("*Operation failed,ErrorCode=" + idwErrorCode.ToString());
        }
        */
        //lblOutputInfo.Items.Add("[func SSR_GetWorkCodeIDByName]Temporarily unsupported");
        axCZKEM1.EnableDevice(iMachineNumber, false);
        if (axCZKEM1.SSR_SetWorkCode(iWorkcodeID, sName))
        {
            lblOutputInfo.Items.Add("Successfully set workcode");
        }
        else
        {
            axCZKEM1.GetLastError(ref idwErrorCode);
            lblOutputInfo.Items.Add("*Operation failed,ErrorCode=" + idwErrorCode.ToString());
        }
        axCZKEM1.EnableDevice(iMachineNumber, true);
        return 1;
    }
    public int sta_DelWorkCode(ListBox lblOutputInfo, TextBox txtWorkcodeID)
    {
        if (GetConnectState() == false)
        {
            lblOutputInfo.Items.Add("*Önce Terminal bağlantısını kurunuz!");
            return -1024;
        }
        if (txtWorkcodeID.Text.Trim() == "")
        {
            lblOutputInfo.Items.Add("*Please input data first!");
            return -1023;
        }
        int idwErrorCode = 0;
        int iWorkcodeID = Convert.ToInt32(txtWorkcodeID.Text.Trim());
        axCZKEM1.EnableDevice(iMachineNumber, false);
        if (axCZKEM1.SSR_DeleteWorkCode(iWorkcodeID))
        {
            lblOutputInfo.Items.Add("Successfully delete workcode");
        }
        else
        {
            axCZKEM1.GetLastError(ref idwErrorCode);
            lblOutputInfo.Items.Add("*Operation failed,ErrorCode=" + idwErrorCode.ToString());
        }
        axCZKEM1.EnableDevice(iMachineNumber, true);
        return 1;
    }
    public int sta_ClearWorkCode(ListBox lblOutputInfo)
    {
        if (GetConnectState() == false)
        {
            lblOutputInfo.Items.Add("*Önce Terminal bağlantısını kurunuz!");
            return -1024;
        }
        int idwErrorCode = 0;
        axCZKEM1.EnableDevice(iMachineNumber, false);
        if (axCZKEM1.SSR_ClearWorkCode())
        {
            lblOutputInfo.Items.Add("Successfully clear all workcode");
        }
        else
        {
            axCZKEM1.GetLastError(ref idwErrorCode);
            lblOutputInfo.Items.Add("*Operation failed,ErrorCode=" + idwErrorCode.ToString());
        }
        axCZKEM1.EnableDevice(iMachineNumber, true);
        return 1;
    }
    #endregion
    #region user role
    public string[] sApp = new string[]
    {
            "usermng",
            "access",
            "iccardmng",
            "comset",
            "sysset",
            "myset",
            "datamng",
            "udiskmng",
            "logquery",
            "printset",
            "sms",
            "workcode",
            "autotest",
            "sysinfo"
    };
    public string[] sFunUserMng = new string[]
    {
            "adduser",
            "userlist",
            "userliststyle"
    };
    public string[] sFunAccess = new string[]
    {
            "timezone",
            "holiday",
            "group",
            "unlockcomb",
            "accparam",
            "duressalarm",
            "antipassbackset"
    };
    public string[] sFunICCard = new string[]
    {
            "enrollnumcard",
            "enrollfpcard",
            "clearcard",
            "copycard",
            "setcardparam"
    };
    public string[] sFunComm = new string[]
    {
            "netset",
            "serialset",
            "linkset",
            "mobilenet",
            "wifiset",
            "admsset",
            "wiegandset"
    };
    public string[] sFunSystem = new string[]
    {
            "timeset",
            "attparam",
            "fpparam",
            "restoreset",
            "udiskupgrade",
    };
    public string[] sFunPersonalize = new string[]
    {
            "displayset",
            "voiceset",
            "bellset",
            "shortcutsset",
            "statemodeset",
            "autopowerset"
    };
    public string[] sFunDataMng = new string[]
    {
            "cleardata",
            "backupdata",
            "restoredata"
    };
    public string[] sFunUSBMng = new string[]
    {
            "udiskupload",
            "udiskdownload",
            "udiskset"
    };
    public string[] sFunAttSearch = new string[]
    {
            "attlog",
            "attpic",
            "blacklistpic"
    };
    public string[] sFunPrint = new string[]
    {
            "printinfoset",
            "printfuncset"
    };
    public string[] sFunSMS = new string[]
    {
            "addsms",
            "smslist"
    };
    public string[] sFunWorkCode = new string[]
    {
            "addworkcode",
            "workcodelist",
            "workcodesetting"
    };
    public string[] sFunAutoTest = new string[]
    {
            "alltest",
            "screentest",
            "voicetest",
            "keytest",
            "fptest",
            "realtimetest",
            "cameratest"
    };
    public string[] sFunSysInfo = new string[]
    {
            "datacapacity",
            "devinfo",
            "firmwareinfo"
    };
    //public int sta_GetUserRole(ListBox lblOutputInfo, string cbUserRole, int[] iAppState, int[] iFunUserMng, int[] iFunAccess, int[] iFunICCard, int[] iFunComm, int[] iFunSystem, int[] iFunPersonalize, int[] iFunDataMng, int[] iFunUSBMng, int[] iFunAttSearch, int[] iFunPrint, int[] iFunSMS, int[] iFunWorkCode, int[] iFunAutoTest, int[] iFunSysInfo)
    //{
    //    if (GetConnectState() == false)
    //    {
    //        lblOutputInfo.Items.Add("*Önce Terminal bağlantısını kurunuz!");
    //        return -1024;
    //    }
    //    if (cbUserRole.Text == "")
    //    {
    //        lblOutputInfo.Items.Add("*Please input user role!");
    //        return -1023;
    //    }
    //    int iPrivilege = cbUserRole.SelectedIndex;
    //    /*
    //                bool bFlag = false;
    //                if (iPrivilege == 2)
    //                {
    //                    axCZKEM1.IsUserDefRoleEnable(iMachineNumber, 4, out bFlag);
    //                    if (bFlag == false)
    //                    {
    //                        lblOutputInfo.Items.Add("*User Defined Role is unable!");
    //                        return -1023;
    //                    }
    //                }
    //    */
    //    int idwErrorCode = 0;
    //    string sAppName = "";
    //    string sFunName = "";
    //    int i = 0, j = 1;
    //    int l = 0, k = 1;
    //    int iUserRole = 0;
    //    /*
    //                switch (cbUserRole.SelectedIndex)
    //                {
    //                    case 0: iUserRole = 1; break;
    //                    case 1: iUserRole = 2; break;
    //                    case 2: iUserRole = 4; break;
    //                }
    //                axCZKEM1.EnableDevice(iMachineNumber, false);
    //                if (axCZKEM1.GetAppOfRole(iMachineNumber, iUserRole, out sAppName))
    //                {
    //                    if (axCZKEM1.GetFunOfRole(iMachineNumber, iUserRole, out sFunName))
    //                    {
    //                        string[] sTmp = Regex.Split(sAppName, "\r\n", RegexOptions.None);
    //                        string[] sTmp1 = Regex.Split(sFunName, "\r\n", RegexOptions.None);
    //                        for (l = 1; l < sTmp.Length; l++)
    //                        {
    //                            for (k = 0; k < sApp.Length; k++)
    //                            {
    //                                if (string.Compare(sTmp[l].ToString(), sApp[k].ToString()) == 0)
    //                                {
    //                                    iAppState[k] = 1;
    //                                    switch (k)
    //                                    {
    //                                        case 0:
    //                                            {
    //                                                for (i = 0; i < sFunUserMng.Length; i++)
    //                                                {
    //                                                    for (j = 1; j < sTmp1.Length; j++)
    //                                                    {
    //                                                        if (string.Compare(sTmp1[j].ToString(), sFunUserMng[i].ToString()) == 0)
    //                                                        {
    //                                                            iFunUserMng[i] = 1;
    //                                                            break;
    //                                                        }
    //                                                    }
    //                                                }
    //                                                break;
    //                                            }
    //                                        case 1:
    //                                            {
    //                                                for (i = 0; i < sFunAccess.Length; i++)
    //                                                {
    //                                                    for (j = 1; j < sTmp1.Length; j++)
    //                                                    {
    //                                                        if (string.Compare(sTmp1[j].ToString(), sFunAccess[i].ToString()) == 0)
    //                                                        {
    //                                                            iFunAccess[i] = 1;
    //                                                            break;
    //                                                        }
    //                                                    }
    //                                                }
    //                                                break;
    //                                            }
    //                                        case 2:
    //                                            {
    //                                                for (i = 0; i < sFunICCard.Length; i++)
    //                                                {
    //                                                    for (j = 1; j < sTmp1.Length; j++)
    //                                                    {
    //                                                        if (string.Compare(sTmp1[j].ToString(), sFunICCard[i].ToString()) == 0)
    //                                                        {
    //                                                            iFunICCard[i] = 1;
    //                                                            break;
    //                                                        }
    //                                                    }
    //                                                }
    //                                                break;
    //                                            }
    //                                        case 3:
    //                                            {
    //                                                for (i = 0; i < sFunComm.Length; i++)
    //                                                {
    //                                                    for (j = 1; j < sTmp1.Length; j++)
    //                                                    {
    //                                                        if (string.Compare(sTmp1[j].ToString(), sFunComm[i].ToString()) == 0)
    //                                                        {
    //                                                            iFunComm[i] = 1;
    //                                                            break;
    //                                                        }
    //                                                    }
    //                                                }
    //                                                break;
    //                                            }
    //                                        case 4:
    //                                            {
    //                                                for (i = 0; i < sFunSystem.Length; i++)
    //                                                {
    //                                                    for (j = 1; j < sTmp1.Length; j++)
    //                                                    {
    //                                                        if (string.Compare(sTmp1[j].ToString(), sFunSystem[i].ToString()) == 0)
    //                                                        {
    //                                                            iFunSystem[i] = 1;
    //                                                            break;
    //                                                        }
    //                                                    }
    //                                                }
    //                                                break;
    //                                            }
    //                                        case 5:
    //                                            {
    //                                                for (i = 0; i < sFunPersonalize.Length; i++)
    //                                                {
    //                                                    for (j = 1; j < sTmp1.Length; j++)
    //                                                    {
    //                                                        if (string.Compare(sTmp1[j].ToString(), sFunPersonalize[i].ToString()) == 0)
    //                                                        {
    //                                                            iFunPersonalize[i] = 1;
    //                                                            break;
    //                                                        }
    //                                                    }
    //                                                }
    //                                                break;
    //                                            }
    //                                        case 6:
    //                                            {
    //                                                for (i = 0; i < sFunDataMng.Length; i++)
    //                                                {
    //                                                    for (j = 1; j < sTmp1.Length; j++)
    //                                                    {
    //                                                        if (string.Compare(sTmp1[j].ToString(), sFunDataMng[i].ToString()) == 0)
    //                                                        {
    //                                                            iFunDataMng[i] = 1;
    //                                                            break;
    //                                                        }
    //                                                    }
    //                                                }
    //                                                break;
    //                                            }
    //                                        case 7:
    //                                            {
    //                                                for (i = 0; i < sFunUSBMng.Length; i++)
    //                                                {
    //                                                    for (j = 1; j < sTmp1.Length; j++)
    //                                                    {
    //                                                        if (string.Compare(sTmp1[j].ToString(), sFunUSBMng[i].ToString()) == 0)
    //                                                        {
    //                                                            iFunUSBMng[i] = 1;
    //                                                            break;
    //                                                        }
    //                                                    }
    //                                                }
    //                                                break;
    //                                            }
    //                                        case 8:
    //                                            {
    //                                                for (i = 0; i < sFunAttSearch.Length; i++)
    //                                                {
    //                                                    for (j = 1; j < sTmp1.Length; j++)
    //                                                    {
    //                                                        if (string.Compare(sTmp1[j].ToString(), sFunAttSearch[i].ToString()) == 0)
    //                                                        {
    //                                                            iFunAttSearch[i] = 1;
    //                                                            break;
    //                                                        }
    //                                                    }
    //                                                }
    //                                                break;
    //                                            }
    //                                        case 9:
    //                                            {
    //                                                for (i = 0; i < sFunPrint.Length; i++)
    //                                                {
    //                                                    for (j = 1; j < sTmp1.Length; j++)
    //                                                    {
    //                                                        if (string.Compare(sTmp1[j].ToString(), sFunPrint[i].ToString()) == 0)
    //                                                        {
    //                                                            iFunPrint[i] = 1;
    //                                                            break;
    //                                                        }
    //                                                    }
    //                                                }
    //                                                break;
    //                                            }
    //                                        case 10:
    //                                            {
    //                                                for (i = 0; i < sFunSMS.Length; i++)
    //                                                {
    //                                                    for (j = 1; j < sTmp1.Length; j++)
    //                                                    {
    //                                                        if (string.Compare(sTmp1[j].ToString(), sFunSMS[i].ToString()) == 0)
    //                                                        {
    //                                                            iFunSMS[i] = 1;
    //                                                            break;
    //                                                        }
    //                                                    }
    //                                                }
    //                                                break;
    //                                            }
    //                                        case 11:
    //                                            {
    //                                                for (i = 0; i < sFunWorkCode.Length; i++)
    //                                                {
    //                                                    for (j = 1; j < sTmp1.Length; j++)
    //                                                    {
    //                                                        if (string.Compare(sTmp1[j].ToString(), sFunWorkCode[i].ToString()) == 0)
    //                                                        {
    //                                                            iFunWorkCode[i] = 1;
    //                                                            break;
    //                                                        }
    //                                                    }
    //                                                }
    //                                                break;
    //                                            }
    //                                        case 12:
    //                                            {
    //                                                for (i = 0; i < sFunAutoTest.Length; i++)
    //                                                {
    //                                                    for (j = 1; j < sTmp1.Length; j++)
    //                                                    {
    //                                                        if (string.Compare(sTmp1[j].ToString(), sFunAutoTest[i].ToString()) == 0)
    //                                                        {
    //                                                            iFunAutoTest[i] = 1;
    //                                                            break;
    //                                                        }
    //                                                    }
    //                                                }
    //                                                break;
    //                                            }
    //                                        case 13:
    //                                            {
    //                                                for (i = 0; i < sFunSysInfo.Length; i++)
    //                                                {
    //                                                    for (j = 1; j < sTmp1.Length; j++)
    //                                                    {
    //                                                        if (string.Compare(sTmp1[j].ToString(), sFunSysInfo[i].ToString()) == 0)
    //                                                        {
    //                                                            iFunSysInfo[i] = 1;
    //                                                            break;
    //                                                        }
    //                                                    }
    //                                                }
    //                                                break;
    //                                            }
    //                                        default: break;
    //                                    }
    //                                    break;
    //                                }
    //                            }
    //                        }
    //                        axCZKEM1.RefreshData(iMachineNumber);//After you have set user short message,you should refresh the data of the device
    //                        lblOutputInfo.Items.Add("Get user role successfully! ");
    //                    }
    //                    else
    //                    {
    //                        axCZKEM1.GetLastError(ref idwErrorCode);
    //                        lblOutputInfo.Items.Add("*Get sub menu failed,ErrorCode=" + idwErrorCode.ToString());
    //                        return 1;
    //                    }
    //                }
    //                else
    //                {
    //                    axCZKEM1.GetLastError(ref idwErrorCode);
    //                    lblOutputInfo.Items.Add("*Get top menu failed,ErrorCode=" + idwErrorCode.ToString());
    //                }
    //                */
    //    lblOutputInfo.Items.Add("[func GetAppOfRole]Temporarily unsupported");
    //    //axCZKEM1.EnableDevice(iMachineNumber, true);
    //    return 1;
    //}
    //public int sta_SetUserRole(ListBox lblOutputInfo, string cbUserRole, int[] iAppState, int[] iFunUserMng, int[] iFunAccess, int[] iFunICCard, int[] iFunComm, int[] iFunSystem, int[] iFunPersonalize, int[] iFunDataMng, int[] iFunUSBMng, int[] iFunAttSearch, int[] iFunPrint, int[] iFunSMS, int[] iFunWorkCode, int[] iFunAutoTest, int[] iFunSysInfo)
    //{
    //    if (GetConnectState() == false)
    //    {
    //        lblOutputInfo.Items.Add("*Önce Terminal bağlantısını kurunuz!");
    //        return -1024;
    //    }
    //    if (cbUserRole.Text == "")
    //    {
    //        lblOutputInfo.Items.Add("*Please input user role!");
    //        return -1023;
    //    }
    //    int iPrivilege = cbUserRole.SelectedIndex;
    //    /*
    //                bool bFlag = false;
    //                if (iPrivilege == 2)
    //                {
    //                    axCZKEM1.IsUserDefRoleEnable(iMachineNumber, 4, out bFlag);
    //                    if (bFlag == false)
    //                    {
    //                        lblOutputInfo.Items.Add("*User Defined Role is unable!");
    //                        return -1023;
    //                    }
    //                }
    //    */
    //    int idwErrorCode = 0;
    //    int iUserRole = 0;
    //    int dd = 0;
    //    /*
    //    //SDK支持
    //    switch (cbUserRole.SelectedIndex)
    //    {
    //        case 0: iUserRole = 1; break;
    //        case 1: iUserRole = 2; break;
    //        case 2: iUserRole = 4; break;
    //    }
    //    for (int i = 0; i < iFunUserMng.Length; i++)
    //    {
    //        if (iFunUserMng[i] == 1)
    //        {
    //            if (!axCZKEM1.SetPermOfAppFun(iMachineNumber, iUserRole, sApp[0], sFunUserMng[i]))
    //            {
    //                axCZKEM1.GetLastError(ref idwErrorCode);
    //                lblOutputInfo.Items.Add("*Set User Mgt menu failed,sub menu index=" + i.ToString() + ",ErrorCode=" + idwErrorCode.ToString());
    //                dd ++;
    //            }
    //        }
    //        else
    //        {
    //            if (!axCZKEM1.DeletePermOfAppFun(iMachineNumber, iUserRole, sApp[0], sFunUserMng[i]))
    //            {
    //                axCZKEM1.GetLastError(ref idwErrorCode);
    //                lblOutputInfo.Items.Add("*Set User Mgt menu failed,sub menu index=" + i.ToString() + ",ErrorCode=" + idwErrorCode.ToString());
    //                dd ++;
    //            }
    //        }
    //    }
    //    for (int i = 0; i < iFunAccess.Length; i++)
    //    {
    //        if (iFunAccess[i] == 1)
    //        {
    //            if(!axCZKEM1.SetPermOfAppFun(iMachineNumber, iUserRole, sApp[1], sFunAccess[i]))
    //            {
    //                axCZKEM1.GetLastError(ref idwErrorCode);
    //                lblOutputInfo.Items.Add("*Set Access Control menu failed,sub menu index=" + i.ToString() + ",ErrorCode=" + idwErrorCode.ToString());
    //                dd ++;
    //            }
    //        }
    //        else
    //        {
    //            if (!axCZKEM1.DeletePermOfAppFun(iMachineNumber, iUserRole, sApp[1], sFunAccess[i]))
    //            {
    //                axCZKEM1.GetLastError(ref idwErrorCode);
    //                lblOutputInfo.Items.Add("*Set Access Control menu failed,sub menu index=" + i.ToString() + ",ErrorCode=" + idwErrorCode.ToString());
    //                dd ++;
    //            }
    //        }
    //    }
    //    for (int i = 0; i < iFunICCard.Length; i++)
    //    {
    //        if (iFunICCard[i] == 1)
    //        {
    //            if (!axCZKEM1.SetPermOfAppFun(iMachineNumber, iUserRole, sApp[2], sFunICCard[i]))
    //            {
    //                axCZKEM1.GetLastError(ref idwErrorCode);
    //                lblOutputInfo.Items.Add("*Set IC Card menu failed,sub menu index=" + i.ToString() + ",ErrorCode=" + idwErrorCode.ToString());
    //                dd ++;
    //            }
    //        }
    //        else
    //        {
    //            if (!axCZKEM1.DeletePermOfAppFun(iMachineNumber, iUserRole, sApp[2], sFunICCard[i]))
    //            {
    //                axCZKEM1.GetLastError(ref idwErrorCode);
    //                lblOutputInfo.Items.Add("*Set IC Card menu failed,sub menu index=" + i.ToString() + ",ErrorCode=" + idwErrorCode.ToString());
    //                dd ++;
    //            }
    //        }
    //    }
    //    for (int i = 0; i < iFunComm.Length; i++)
    //    {
    //        if (iFunComm[i] == 1)
    //        {
    //            if(!axCZKEM1.SetPermOfAppFun(iMachineNumber, iUserRole, sApp[3], sFunComm[i]))
    //            {
    //                axCZKEM1.GetLastError(ref idwErrorCode);
    //                lblOutputInfo.Items.Add("*Set Comm menu failed,sub menu index=" + i.ToString() + ",ErrorCode=" + idwErrorCode.ToString());
    //                dd ++;
    //            }
    //        }
    //        else
    //        {
    //            if (!axCZKEM1.DeletePermOfAppFun(iMachineNumber, iUserRole, sApp[3], sFunComm[i]))
    //            {
    //                axCZKEM1.GetLastError(ref idwErrorCode);
    //                lblOutputInfo.Items.Add("*Set Comm menu failed,sub menu index=" + i.ToString() + ",ErrorCode=" + idwErrorCode.ToString());
    //                dd ++;
    //            }
    //        }
    //    }
    //    for (int i = 0; i < iFunSystem.Length; i++)
    //    {
    //        if (iFunSystem[i] == 1)
    //        {
    //            if(!axCZKEM1.SetPermOfAppFun(iMachineNumber, iUserRole, sApp[4], sFunSystem[i]))
    //            {
    //                axCZKEM1.GetLastError(ref idwErrorCode);
    //                lblOutputInfo.Items.Add("*Set System menu failed,sub menu index=" + i.ToString() + ",ErrorCode=" + idwErrorCode.ToString());
    //                dd ++;
    //            }
    //        }
    //        else
    //        {
    //            if (!axCZKEM1.DeletePermOfAppFun(iMachineNumber, iUserRole, sApp[4], sFunSystem[i]))
    //            {
    //                axCZKEM1.GetLastError(ref idwErrorCode);
    //                lblOutputInfo.Items.Add("*Set System menu failed,sub menu index=" + i.ToString() + ",ErrorCode=" + idwErrorCode.ToString());
    //                dd++;
    //            }
    //        }
    //    }
    //    for (int i = 0; i < iFunPersonalize.Length; i++)
    //    {
    //        if (iFunPersonalize[i] == 1)
    //        {
    //            if(!axCZKEM1.SetPermOfAppFun(iMachineNumber, iUserRole, sApp[5], sFunPersonalize[i]))
    //            {
    //                axCZKEM1.GetLastError(ref idwErrorCode);
    //                lblOutputInfo.Items.Add("*Set Personalize menu failed,sub menu index=" + i.ToString() + ",ErrorCode=" + idwErrorCode.ToString());
    //                dd++;
    //            }
    //        }
    //        else
    //        {
    //            if (!axCZKEM1.DeletePermOfAppFun(iMachineNumber, iUserRole, sApp[5], sFunPersonalize[i]))
    //            {
    //                axCZKEM1.GetLastError(ref idwErrorCode);
    //                lblOutputInfo.Items.Add("*Set Personalize menu failed,sub menu index=" + i.ToString() + ",ErrorCode=" + idwErrorCode.ToString());
    //                dd++;
    //            }
    //        }
    //    }
    //    for (int i = 0; i < iFunDataMng.Length; i++)
    //    {
    //        if (iFunDataMng[i] == 1)
    //        {
    //            if(!axCZKEM1.SetPermOfAppFun(iMachineNumber, iUserRole, sApp[6], sFunDataMng[i]))
    //            {
    //                axCZKEM1.GetLastError(ref idwErrorCode);
    //                lblOutputInfo.Items.Add("*Set Data Mgt menu failed,sub menu index=" + i.ToString() + ",ErrorCode=" + idwErrorCode.ToString());
    //                dd++;
    //            }
    //        }
    //        else
    //        {
    //            if (!axCZKEM1.DeletePermOfAppFun(iMachineNumber, iUserRole, sApp[6], sFunDataMng[i]))
    //            {
    //                axCZKEM1.GetLastError(ref idwErrorCode);
    //                lblOutputInfo.Items.Add("*Set Data Mgt menu failed,sub menu index=" + i.ToString() + ",ErrorCode=" + idwErrorCode.ToString());
    //                dd++;
    //            }
    //        }
    //    }
    //    for (int i = 0; i < iFunUSBMng.Length; i++)
    //    {
    //        if (iFunUSBMng[i] == 1)
    //        {
    //            if(!axCZKEM1.SetPermOfAppFun(iMachineNumber, iUserRole, sApp[7], sFunUSBMng[i]))
    //            {
    //                axCZKEM1.GetLastError(ref idwErrorCode);
    //                lblOutputInfo.Items.Add("*Set USB Manager menu failed,sub menu index=" + i.ToString() + ",ErrorCode=" + idwErrorCode.ToString());
    //                dd++;
    //            }
    //        }
    //        else
    //        {
    //            if (!axCZKEM1.DeletePermOfAppFun(iMachineNumber, iUserRole, sApp[7], sFunUSBMng[i]))
    //            {
    //                axCZKEM1.GetLastError(ref idwErrorCode);
    //                lblOutputInfo.Items.Add("*Set USB Manager menu failed,menu index=" + i.ToString() + ",ErrorCode=" + idwErrorCode.ToString());
    //                dd++;
    //            }
    //        }
    //    }
    //    for (int i = 0; i < iFunAttSearch.Length; i++)
    //    {
    //        if (iFunAttSearch[i] == 1)
    //        {
    //            if(!axCZKEM1.SetPermOfAppFun(iMachineNumber, iUserRole, sApp[8], sFunAttSearch[i]))
    //            {
    //                axCZKEM1.GetLastError(ref idwErrorCode);
    //                lblOutputInfo.Items.Add("*Set Attendance menu failed,sub menu index=" + i.ToString() + ",ErrorCode=" + idwErrorCode.ToString());
    //                dd++;
    //            }
    //        }
    //        else
    //        {
    //            if (!axCZKEM1.DeletePermOfAppFun(iMachineNumber, iUserRole, sApp[8], sFunAttSearch[i]))
    //            {
    //                axCZKEM1.GetLastError(ref idwErrorCode);
    //                lblOutputInfo.Items.Add("*Set Attendance menu failed,sub menu index=" + i.ToString() + ",ErrorCode=" + idwErrorCode.ToString());
    //                dd++;
    //            }
    //        }
    //    }
    //    for (int i = 0; i < iFunPrint.Length; i++)
    //    {
    //        if (iFunPrint[i] == 1)
    //        {
    //            if(!axCZKEM1.SetPermOfAppFun(iMachineNumber, iUserRole, sApp[9], sFunPrint[i]))
    //            {
    //                axCZKEM1.GetLastError(ref idwErrorCode);
    //                lblOutputInfo.Items.Add("*Set Print menu failed,sub menu index=" + i.ToString() + ",ErrorCode=" + idwErrorCode.ToString());
    //                dd++;
    //            }
    //        }
    //        else
    //        {
    //            if (!axCZKEM1.DeletePermOfAppFun(iMachineNumber, iUserRole, sApp[9], sFunPrint[i]))
    //            {
    //                axCZKEM1.GetLastError(ref idwErrorCode);
    //                lblOutputInfo.Items.Add("*Set Print menu failed,sub menu index=" + i.ToString() + ",ErrorCode=" + idwErrorCode.ToString());
    //                dd++;
    //            }
    //        }
    //    }
    //    for (int i = 0; i < iFunSMS.Length; i++)
    //    {
    //        if (iFunSMS[i] == 1)
    //        {
    //            if(!axCZKEM1.SetPermOfAppFun(iMachineNumber, iUserRole, sApp[10], sFunSMS[i]))
    //            {
    //                axCZKEM1.GetLastError(ref idwErrorCode);
    //                lblOutputInfo.Items.Add("*Set Short Message menu failed,sub menu index=" + i.ToString() + ",ErrorCode=" + idwErrorCode.ToString());
    //                dd ++;
    //            }
    //        }
    //        else
    //        {
    //            if (!axCZKEM1.DeletePermOfAppFun(iMachineNumber, iUserRole, sApp[10], sFunSMS[i]))
    //            {
    //                axCZKEM1.GetLastError(ref idwErrorCode);
    //                lblOutputInfo.Items.Add("*Set Short Message menu failed,sub menu index=" + i.ToString() + ",ErrorCode=" + idwErrorCode.ToString());
    //                dd ++;
    //            }
    //        }
    //    }
    //    for (int i = 0; i < iFunWorkCode.Length; i++)
    //    {
    //        if (iFunWorkCode[i] == 1)
    //        {
    //            if(!axCZKEM1.SetPermOfAppFun(iMachineNumber, iUserRole, sApp[11], sFunWorkCode[i]))
    //            {
    //                axCZKEM1.GetLastError(ref idwErrorCode);
    //                lblOutputInfo.Items.Add("*Set Work Code menu failed,sub menu index=" + i.ToString() + ",ErrorCode=" + idwErrorCode.ToString());
    //                dd++;
    //            }
    //        }
    //        else
    //        {
    //            if (!axCZKEM1.DeletePermOfAppFun(iMachineNumber, iUserRole, sApp[11], sFunWorkCode[i]))
    //            {
    //                axCZKEM1.GetLastError(ref idwErrorCode);
    //                lblOutputInfo.Items.Add("*Set Work Code menu failed,sub menu index=" + i.ToString() + ",ErrorCode=" + idwErrorCode.ToString());
    //                dd++;
    //            }
    //        }
    //    }
    //    for (int i = 0; i < iFunAutoTest.Length; i++)
    //    {
    //        if (iFunAutoTest[i] == 1)
    //        {
    //            if(!axCZKEM1.SetPermOfAppFun(iMachineNumber, iUserRole, sApp[12], sFunAutoTest[i]))
    //            {
    //                axCZKEM1.GetLastError(ref idwErrorCode);
    //                lblOutputInfo.Items.Add("*Set Auto Test menu failed,sub menu index=" + i.ToString() + ",ErrorCode=" + idwErrorCode.ToString());
    //                dd++;
    //            }
    //        }
    //        else
    //        {
    //            if (!axCZKEM1.DeletePermOfAppFun(iMachineNumber, iUserRole, sApp[12], sFunAutoTest[i]))
    //            {
    //                axCZKEM1.GetLastError(ref idwErrorCode);
    //                lblOutputInfo.Items.Add("*Set Auto Test menu failed,sub menu index=" + i.ToString() + ",ErrorCode=" + idwErrorCode.ToString());
    //                dd++;
    //            }
    //        }
    //    }
    //    for (int i = 0; i < iFunSysInfo.Length; i++)
    //    {
    //        if (iFunSysInfo[i] == 1)
    //        {
    //            if(!axCZKEM1.SetPermOfAppFun(iMachineNumber, iUserRole, sApp[13], sFunSysInfo[i]))
    //            {
    //                axCZKEM1.GetLastError(ref idwErrorCode);
    //                lblOutputInfo.Items.Add("*Set System Info menu failed,sub menu index=" + i.ToString() + ",ErrorCode=" + idwErrorCode.ToString());
    //                dd++;
    //            }
    //        }
    //        else
    //        {
    //            if (!axCZKEM1.DeletePermOfAppFun(iMachineNumber, iUserRole, sApp[13], sFunSysInfo[i]))
    //            {
    //                axCZKEM1.GetLastError(ref idwErrorCode);
    //                lblOutputInfo.Items.Add("*Set System Info menu failed,sub menu index=" + i.ToString() + ",ErrorCode=" + idwErrorCode.ToString());
    //                dd++;
    //            }
    //        }
    //    }
    //    if (dd == 0)
    //    {
    //        lblOutputInfo.Items.Add("Set User Role successfully~");
    //    }
    //    */
    //    lblOutputInfo.Items.Add("[func SetPermOfAppFun]Temporarily unsupported");
    //    return 1;
    //}
    //#endregion
    #region UserBio
    /*
    public void connectDevice(string ip, int port, int commKey)
    {
        axCZKEM1.SetCommPassword(commKey);
        connected = axCZKEM1.Connect_Net(ip, port);
        if (connected)
        {
            sta_getBiometricType();
        }
    }
    public void disconnectDevice()
    {
        if (connected) axCZKEM1.Disconnect();
    }
    */
    private string sta_getSysOptions(string option)
    {
        string value = string.Empty;
        axCZKEM1.GetSysOption(iMachineNumber, option, out value);
        return value;
    }
    /// <summary>
    /// get version
    /// </summary>
    /// <returns></returns>
    public void sta_getBiometricVersion()
    {
        string result = string.Empty;
        _biometricVersion = sta_getSysOptions("BiometricVersion");
    }
    /// <summary>
    /// get support type
    /// </summary>
    /// <returns></returns>
    public void sta_getBiometricType()
    {
        string result = string.Empty;
        result = sta_getSysOptions("BiometricType");
        if (!string.IsNullOrEmpty(result))
        {
            _supportBiometricType.fp_available = result[1] == '1';
            _supportBiometricType.face_available = result[2] == '1';
            if (result.Length >= 9)
            {
                _supportBiometricType.fingerVein_available = result[7] == '1';
                _supportBiometricType.palm_available = result[8] == '1';
            }
        }
        _biometricType = result;
    }
    public List<Personel> sta_getEmployees()
    {
        if (!GetConnectState())
        {
            return new List<Personel>();
        }
        List<Personel> employees = new List<Personel>();
        string empnoStr = string.Empty;
        string name = string.Empty;
        string pwd = string.Empty;
        int pri = 0;
        bool enable = true;
        string cardNum = string.Empty;
        axCZKEM1.EnableDevice(iMachineNumber, false);
        try
        {
            axCZKEM1.ReadAllUserID(iMachineNumber);
            while (axCZKEM1.SSR_GetAllUserInfo(iMachineNumber, out empnoStr, out name, out pwd, out pri, out enable))
            {
                cardNum = "";
                if (axCZKEM1.GetStrCardNumber(out cardNum))
                {
                    if (string.IsNullOrEmpty(cardNum))
                        cardNum = "";
                }
                if (!string.IsNullOrEmpty(name))
                {
                    int index = name.IndexOf("\0");
                    if (index > 0)
                    {
                        name = name.Substring(0, index);
                    }
                }
                Personel emp = new Personel();
                emp.personel = empnoStr;
                emp.AdiSoyadi = name;
                emp.privilege = pri;
                emp.Parola = pwd;
                emp.cardNumarsi = cardNum;
                employees.Add(emp);
            }
        }
        catch
        {
        }
        finally
        {
            axCZKEM1.EnableDevice(iMachineNumber, true);
        }
        List<Personel> SortedList = employees.OrderBy(o => o.AdiSoyadi).ToList();
        return SortedList;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param AdiSoyadi="buffer"></param>
    /// <param AdiSoyadi="bioTemplate"></param>
    private void sta_getBioTemplateFromBuffer(string buffer, ref BioTemplate bioTemplate)
    {
        string temp;
        for (int i = 1; i <= 10; i++)
        {
            if (buffer.IndexOf(',') > 0)
            {
                temp = buffer.Substring(0, buffer.IndexOf(','));
            }
            else
            {
                temp = buffer;
            }
            switch (i)
            {
                case 1:
                    bioTemplate.pin = temp;
                    break;
                case 2:
                    bioTemplate.valid_flag = int.Parse(temp);
                    break;
                case 3:
                    bioTemplate.is_duress = int.Parse(temp);
                    break;
                case 4:
                    bioTemplate.bio_type = int.Parse(temp);
                    break;
                case 5:
                    bioTemplate.version = temp;
                    break;
                case 6:
                    bioTemplate.version = bioTemplate.version + "." + temp;
                    break;
                case 7:
                    bioTemplate.data_format = int.Parse(temp);
                    break;
                case 8:
                    bioTemplate.template_no = int.Parse(temp);
                    break;
                case 9:
                    bioTemplate.template_no_index = int.Parse(temp);
                    break;
                case 10:
                    bioTemplate.template_data = temp;
                    break;
            }
            buffer = buffer.Substring(buffer.IndexOf(',') + 1);
        }
    }
    /// <summary>
    /// get template
    /// </summary>
    /// <param AdiSoyadi="aBioType">
    /// <returns></returns>
    //private List<string> sta_batchDownloadBioTemplates(System.Windows.Forms.ProgressBar procBar, int aBioType)
    //{
    //    int tempNum = 50;
    //    int tn = 2;
    //    List<string> bufferList = new List<string>();
    //    foreach (Personel e in employeeList)
    //    {
    //        string filter;
    //        if (aBioType == 1)
    //            filter = string.Format("Type={0}", aBioType);
    //        else
    //            filter = string.Format("Pin={0}\tType={1}", e.personel, aBioType);
    //        int dataCount = axCZKEM1.SSR_GetDeviceDataCount(PersBioTableName, filter, string.Empty);
    //        int nC = aBioType == 1 ? 1 : aBioType == 2 ? 12 : aBioType == 7 ? 3 : aBioType == 8 ? 6 : 0;
    //        string strOffBuffer = string.Empty;
    //        int eBufferSize = 0;
    //        bool result = true;
    //        int n = 0;
    //        while (true)
    //        {
    //            n = 0;
    //            strOffBuffer = string.Empty;
    //            eBufferSize = dataCount * 3500 * nC;
    //            string option = string.Empty;
    //            try
    //            {
    //                result = axCZKEM1.SSR_GetDeviceData(iMachineNumber, out strOffBuffer, eBufferSize,
    //                    PersBioTableName, PersBioTableFields, filter, option);
    //            }
    //            catch
    //            {
    //                result = false;
    //                int errorCode = 0;
    //                axCZKEM1.GetLastError(ref errorCode);
    //            }
    //            if (result) break;
    //            if (!result && n == 2) break;
    //            n++;
    //        }
    //        if (result)
    //        {
    //            bufferList.Add(strOffBuffer);
    //            if ((bufferList.Count / tempNum) >= 0)
    //            {
    //                procBar.Value = tn;
    //                tn += tn;
    //                if (tn >= 90)
    //                    tn = 90;
    //                tempNum = tempNum + 50;
    //            }
    //        }
    //        if (aBioType == 1)   //表示指纹模板获取
    //        {
    //            break;
    //        }
    //    }
    //    return bufferList;
    //}
    /// <summary>
    /// 
    /// </summary>
    /// <param AdiSoyadi="aBioType"></param>
    /// <returns></returns>
    //public List<BioTemplate> sta_BatchGetBioTemplates(System.Windows.Forms.ProgressBar procBar, int aBioType)
    //{
    //    List<BioTemplate> bioTemplateList = new List<BioTemplate>();
    //    List<string> bufferList = sta_batchDownloadBioTemplates(procBar, aBioType);
    //    for (int i = 0; i < bufferList.Count; i++)
    //    {
    //        string[] buffers = bufferList[i].Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
    //        for (int j = 1; j < buffers.Length; j++)
    //        {
    //            BioTemplate bioTemplate = new BioTemplate();
    //            sta_getBioTemplateFromBuffer(buffers[j], ref bioTemplate);
    //            bioTemplateList.Add(bioTemplate);
    //        }
    //    }
    //    return bioTemplateList;
    //}
    private string sta_AssemblesAllUserBioTemplateInfo(List<BioTemplate> bioTemplateList, int aBioType, string version)
    {
        List<BioTemplate> uploadBioTemplateList = bioTemplateList.FindAll(t => t.bio_type == aBioType && t.version.Equals(version));
        string bioTemplateVersion = string.Empty;
        string eMajorVer = string.Empty;
        string eMinorVer = string.Empty;
        StringBuilder result = new StringBuilder();
        foreach (BioTemplate template in uploadBioTemplateList)
        {
            bioTemplateVersion = template.version;
            if (bioTemplateVersion.IndexOf('.') < 0) bioTemplateVersion = bioTemplateVersion + ".0";
            eMajorVer = bioTemplateVersion.Substring(0, bioTemplateVersion.IndexOf('.'));
            eMinorVer = bioTemplateVersion.Substring(bioTemplateVersion.IndexOf('.') + 1);
            result.Append(string.Format("Pin={0}\tValid={1}\tDuress={2}\tType={3}\tMajorVer={4}\tMinorVer={5}\tFormat={6}\tNo={7}\tIndex={8}\tTmp={9}\r\n",
                template.pin, template.valid_flag, template.is_duress, template.bio_type, eMajorVer, eMinorVer, template.data_format, template.template_no,
                template.template_no_index, template.template_data));
        }
        return result.ToString();
    }
    //public void sta_setBioTemplates(List<BioTemplate> bioTemplateList, out string message)
    //{
    //    message = string.Empty;
    //    if (string.IsNullOrEmpty(_biometricVersion)) sta_getBiometricVersion();
    //    if (string.IsNullOrEmpty(_biometricType)) sta_getBiometricType();
    //    string[] versions = _biometricVersion.Split(':');
    //    StringBuilder errorMsg = new StringBuilder();
    //    for (int i = 0; i < _biometricType.Length; i++)
    //    {
    //        if (_biometricType[i] == '1')
    //        {
    //            string buffer = sta_AssemblesAllUserBioTemplateInfo(bioTemplateList, i, versions[i]);
    //            try
    //            {
    //                int errorCode = 0;
    //                bool result = true;
    //                if (!string.IsNullOrEmpty(buffer))
    //                {
    //                    result = axCZKEM1.SSR_SE(1, PersBioTableName, buffer, string.Empty);
    //                }
    //                if (!result)
    //                {
    //                    axCZKEM1.GetLastError(ref errorCode);
    //                    errorMsg.Append(string.Format(" errorcode={0} ", errorCode));
    //                }
    //            }
    //            catch (Exception e)
    //            {
    //                errorMsg.Append(e.Message);
    //            }
    //        }
    //    }
    //    axCZKEM1.RefreshData(iMachineNumber);
    //    axCZKEM1.EnableDevice(iMachineNumber, true);
    //}
    public void sta_setEmployees(List<Personel> employees)
    {
        axCZKEM1.EnableDevice(1, false);
        try
        {
            bool batchUpdate = axCZKEM1.BeginBatchUpdate(iMachineNumber, 1);
            foreach (Personel emp in employees)
            {
                axCZKEM1.SetStrCardNumber(emp.cardNumarsi);
                axCZKEM1.SSR_SetUserInfo(iMachineNumber, emp.personel, emp.AdiSoyadi, emp.Parola, emp.privilege, true);
            }
            if (batchUpdate)
            {
                axCZKEM1.BatchUpdate(iMachineNumber);
                batchUpdate = false;
            }
        }
        catch
        { }
        finally
        {
            axCZKEM1.EnableDevice(iMachineNumber, true);
        }
    }
    #endregion

    #endregion
    #region PersonalizeMng


    //public int sta_uploadAdvertisePicture(ListBox lblOutputInfo, string pictureFile, string pictureName)
    //{
    //    if (GetConnectState() == false)
    //    {
    //        lblOutputInfo.Items.Add("*Önce Terminal bağlantısını kurunuz!");
    //        return -1024;
    //    }
    //    int ret = 0;
    //    axCZKEM1.EnableDevice(GetMachineNumber(), false);//disable the device
    //    if (axCZKEM1.UploadPicture(GetMachineNumber(), pictureFile, pictureName))
    //    {
    //        ret = 1;
    //        lblOutputInfo.Items.Add("Update a advertise picture!");
    //    }
    //    else
    //    {
    //        axCZKEM1.GetLastError(ref idwErrorCode);
    //        ret = idwErrorCode;
    //        if (idwErrorCode != 0)
    //        {
    //            lblOutputInfo.Items.Add("*Upload advertise picture failed,ErrorCode: " + idwErrorCode.ToString());
    //        }
    //        else
    //        {
    //            lblOutputInfo.Items.Add("Terminal verisi bulunamadı!");
    //        }
    //    }
    //    axCZKEM1.EnableDevice(GetMachineNumber(), true);//enable the device
    //    return ret;
    //}
    //public int sta_uploadWallpaper(ListBox lblOutputInfo, string pictureFile, string pictureName)
    //{
    //    if (GetConnectState() == false)
    //    {
    //        lblOutputInfo.Items.Add("*Önce Terminal bağlantısını kurunuz!");
    //        return -1024;
    //    }
    //    int ret = 0;
    //    axCZKEM1.EnableDevice(GetMachineNumber(), false);//disable the device
    //    if (axCZKEM1.UploadTheme(GetMachineNumber(), pictureFile, pictureName))
    //    {
    //        ret = 1;
    //        lblOutputInfo.Items.Add("Update a wallpaper!");
    //    }
    //    else
    //    {
    //        axCZKEM1.GetLastError(ref idwErrorCode);
    //        ret = idwErrorCode;
    //        if (idwErrorCode != 0)
    //        {
    //            lblOutputInfo.Items.Add("*Upload wallpaper failed,ErrorCode: " + idwErrorCode.ToString());
    //        }
    //        else
    //        {
    //            lblOutputInfo.Items.Add("Terminal verisi bulunamadı!");
    //        }
    //    }
    //    axCZKEM1.EnableDevice(GetMachineNumber(), true);//enable the device
    //    return ret;
    //}
    #endregion
    #region DataMng
    #region  AttLogMng
    //Terminal Log Okuması
    public int sta_readAttLog(ListBox lblOutputInfo, DataTable dt_log, string Terminal)
    {
        if (GetConnectState() == false)
        {
            lblOutputInfo.Items.Add("*Önce Terminal bağlantısını kurunuz!");
            return -1024;
        }
        int ret = 0;
        axCZKEM1.EnableDevice(GetMachineNumber(), false);//disable the device
        string sdwEnrollNumber = "";
        int idwVerifyMode = 0;
        int idwInOutMode = 0;
        int idwYear = 0;
        int idwMonth = 0;
        int idwDay = 0;
        int idwHour = 0;
        int idwMinute = 0;
        int idwSecond = 0;
        int idwWorkcode = 0;
        int islem = 1;
        string Mesaj = "";

        if (axCZKEM1.ReadGeneralLogData(GetMachineNumber()))
        {
            while (axCZKEM1.SSR_GetGeneralLogData(GetMachineNumber(), out sdwEnrollNumber, out idwVerifyMode,
                        out idwInOutMode, out idwYear, out idwMonth, out idwDay, out idwHour, out idwMinute, out idwSecond, ref idwWorkcode))//get records from the memory
            {
                var culture = new System.Globalization.CultureInfo("tr-TR");
                DateTime Tarih = new DateTime(idwYear, idwMonth, idwDay);
                DataRow dr = dt_log.NewRow();
                dr["UserID"] = sdwEnrollNumber;
                dr["Tarih"] = Tarih;   // new DateTime(idwYear, idwMonth, idwDay, idwHour, idwMinute, idwSecond).ToString();
                dr["Tarih1"] = new DateTime(idwYear, idwMonth, idwDay, idwHour, idwMinute, idwSecond).ToString();
                dr["Gun"] = culture.DateTimeFormat.GetDayName(Tarih.DayOfWeek);
                dr["Type"] = idwVerifyMode;
                dr["State"] = idwInOutMode;
                dr["WorkCode"] = idwWorkcode;
                dr["GirisSaat"] = new TimeSpan(idwHour, idwMinute, idwSecond);
                Mesaj += islem + "\tTerminal :\t" + Terminal + " \t Kullanıcı :" + sdwEnrollNumber + " : \t İşlem Tipi " + idwInOutMode + "\t" + Tarih + "\t" + new TimeSpan(idwHour, idwMinute, idwSecond) + "\n";

                dt_log.Rows.Add(dr);
                islem++;
            }
          
            ret = 1;
            lblOutputInfo.Items.Add(" Toplam " + islem + " kayıt tespit edildi : ");
        }
        else
        {
            axCZKEM1.GetLastError(ref idwErrorCode);
            ret = idwErrorCode;
            if (idwErrorCode != 0)
            {
                lblOutputInfo.Items.Add("*Log Okuma Hatası Hatakod: : " + idwErrorCode.ToString());
            }
            else
            {
                lblOutputInfo.Items.Add("Terminal verisi bulunamadı!");
            }
        }
        axCZKEM1.EnableDevice(GetMachineNumber(), true);//enable the device
        return ret;
    }
    public int sta_readAttLog(ListBox lblOutputInfo, DataTable dt_log)
    {
        if (GetConnectState() == false)
        {
            lblOutputInfo.Items.Add("*Önce Terminal bağlantısını kurunuz!");
            return -1024;
        }
        int ret = 0;
        axCZKEM1.EnableDevice(GetMachineNumber(), false);//disable the device
        string sdwEnrollNumber = "";
        int idwVerifyMode = 0;
        int idwInOutMode = 0;
        int idwYear = 0;
        int idwMonth = 0;
        int idwDay = 0;
        int idwHour = 0;
        int idwMinute = 0;
        int idwSecond = 0;
        int idwWorkcode = 0;
        int islem = 1;
        string Mesaj = "";

        if (axCZKEM1.ReadGeneralLogData(GetMachineNumber()))
        {
            while (axCZKEM1.SSR_GetGeneralLogData(GetMachineNumber(), out sdwEnrollNumber, out idwVerifyMode,
                        out idwInOutMode, out idwYear, out idwMonth, out idwDay, out idwHour, out idwMinute, out idwSecond, ref idwWorkcode))//get records from the memory
            {
                var culture = new System.Globalization.CultureInfo("tr-TR");
                DateTime Tarih = new DateTime(idwYear, idwMonth, idwDay);
                DataRow dr = dt_log.NewRow();
                dr["UserID"] = sdwEnrollNumber;
                dr["Tarih"] = Tarih;   // new DateTime(idwYear, idwMonth, idwDay, idwHour, idwMinute, idwSecond).ToString();
                dr["Tarih1"] = new DateTime(idwYear, idwMonth, idwDay, idwHour, idwMinute, idwSecond).ToString();
                dr["Gun"] = culture.DateTimeFormat.GetDayName(Tarih.DayOfWeek);
                dr["Type"] = idwVerifyMode;
                dr["State"] = idwInOutMode;
                dr["WorkCode"] = idwWorkcode;
                dr["GirisSaat"] = new TimeSpan(idwHour, idwMinute, idwSecond);
                Mesaj += islem + "\t Kullanıcı :" + sdwEnrollNumber + " : \t İşlem Tipi " + idwInOutMode + "\t" + Tarih + "\t" + new TimeSpan(idwHour, idwMinute, idwSecond) + "\n";

                dt_log.Rows.Add(dr);
                islem++;
            }
          
            ret = 1;
            lblOutputInfo.Items.Add(" Toplam " + islem + " kayıt tespit edildi : ");
        }
        else
        {
            axCZKEM1.GetLastError(ref idwErrorCode);
            ret = idwErrorCode;
            if (idwErrorCode != 0)
            {
                lblOutputInfo.Items.Add("*Log Okuma Hatası Hatakod: : " + idwErrorCode.ToString());
            }
            else
            {
                lblOutputInfo.Items.Add("Terminal verisi bulunamadı!");
            }
        }
        axCZKEM1.EnableDevice(GetMachineNumber(), true);//enable the device
        return ret;
    }
    //public int sta_readLogByPeriod(ListBox lblOutputInfo, DataTable dt_logPeriod, string fromTime, string toTime)
    //{
    //    if (GetConnectState() == false)
    //    {
    //        lblOutputInfo.Items.Add("*Önce Terminal bağlantısını kurunuz!");
    //        return -1024;
    //    }
    //    int ret = 0;
    //    axCZKEM1.EnableDevice(GetMachineNumber(), false);//disable the device
    //    string sdwEnrollNumber = "";
    //    int idwVerifyMode = 0;
    //    int idwInOutMode = 0;
    //    int idwYear = 0;
    //    int idwMonth = 0;
    //    int idwDay = 0;
    //    int idwHour = 0;
    //    int idwMinute = 0;
    //    int idwSecond = 0;
    //    int idwWorkcode = 0;
    //    if (axCZKEM1.ReadGeneralLogData(GetMachineNumber(), fromTime, toTime))
    //    {
    //        while (axCZKEM1.SSR_GetGeneralLogData(GetMachineNumber(), out sdwEnrollNumber, out idwVerifyMode,
    //                    out idwInOutMode, out idwYear, out idwMonth, out idwDay, out idwHour, out idwMinute, out idwSecond, ref idwWorkcode))//get records from the memory
    //        {
    //            DataRow dr = dt_logPeriod.NewRow();
    //            dr["User ID"] = sdwEnrollNumber;
    //            dr["Verify Date"] = idwYear + "-" + idwMonth + "-" + idwDay + " " + idwHour + ":" + idwMinute + ":" + idwSecond;
    //            dr["Verify Type"] = idwVerifyMode;
    //            dr["Verify State"] = idwInOutMode;
    //            dr["WorkCode"] = idwWorkcode;
    //            dt_logPeriod.Rows.Add(dr);
    //        }
    //        ret = 1;
    //    }
    //    else
    //    {
    //        axCZKEM1.GetLastError(ref idwErrorCode);
    //        ret = idwErrorCode;
    //        if (idwErrorCode != 0)
    //        {
    //            lblOutputInfo.Items.Add("*Read attlog by period failed,ErrorCode: " + idwErrorCode.ToString());
    //        }
    //        else
    //        {
    //            lblOutputInfo.Items.Add("Terminal verisi bulunamadı!");
    //        }
    //    }
    //    //lblOutputInfo.Items.Add("[func ReadTimeGLogData]Temporarily unsupported");
    //    axCZKEM1.EnableDevice(GetMachineNumber(), true);//enable the device
    //    return ret;
    //}
    //terminal log kayıtlarını boşalt
    public int sta_DeleteAttLog(ListBox lblOutputInfo)
    {
        if (GetConnectState() == false)
        {
            lblOutputInfo.Items.Add("*Önce Terminal bağlantısını kurunuz!");
            return -1024;
        }
        int ret = 0;
        axCZKEM1.EnableDevice(GetMachineNumber(), false);//disable the device
        if (axCZKEM1.ClearGLog(GetMachineNumber()))
        {
            axCZKEM1.RefreshData(GetMachineNumber());
            ret = 1;
        }
        else
        {
            axCZKEM1.GetLastError(ref idwErrorCode);
            ret = idwErrorCode;
            if (idwErrorCode != 0)
            {
                lblOutputInfo.Items.Add("*Delete attlog, ErrorCode: " + idwErrorCode.ToString());
            }
            else
            {
                lblOutputInfo.Items.Add("Terminal verisi bulunamadı!");
            }
        }
        axCZKEM1.EnableDevice(GetMachineNumber(), true);//enable the device
        return ret;
    }

    public int sta_ReadNewAttLog(ListBox lblOutputInfo, DataTable dt_logNew)
    {
        if (GetConnectState() == false)
        {
            lblOutputInfo.Items.Add("Önce Terminal bağlantısını kurunuz!");
            return -1024;
        }
        int ret = 0;
        axCZKEM1.EnableDevice(GetMachineNumber(), false);//disable the device
        string sdwEnrollNumber = "";
        int idwVerifyMode = 0;
        int idwInOutMode = 0;
        int idwYear = 0;
        int idwMonth = 0;
        int idwDay = 0;
        int idwHour = 0;
        int idwMinute = 0;
        int idwSecond = 0;
        int idwWorkcode = 0;
        if (axCZKEM1.ReadGeneralLogData(GetMachineNumber()))
        {
            while (axCZKEM1.SSR_GetGeneralLogData(GetMachineNumber(), out sdwEnrollNumber, out idwVerifyMode,
                        out idwInOutMode, out idwYear, out idwMonth, out idwDay, out idwHour, out idwMinute, out idwSecond, ref idwWorkcode))//get records from the memory
            {
                DataRow dr = dt_logNew.NewRow();
                dr["User ID"] = sdwEnrollNumber;
                dr["Verify Date"] = idwYear + "-" + idwMonth + "-" + idwDay + " " + idwHour + ":" + idwMinute + ":" + idwSecond;
                dr["Verify Type"] = idwVerifyMode;
                dr["Verify State"] = idwInOutMode;
                dr["WorkCode"] = idwWorkcode;
                dt_logNew.Rows.Add(dr);
            }
            ret = 1;
        }
        else
        {
            axCZKEM1.GetLastError(ref idwErrorCode);
            ret = idwErrorCode;
            if (idwErrorCode != 0)
            {
                lblOutputInfo.Items.Add("*Read attlog by period failed,ErrorCode: " + idwErrorCode.ToString());
            }
            else
            {
                lblOutputInfo.Items.Add("Terminal verisi bulunamadı!");
            }
        }
        //lblOutputInfo.Items.Add("[func ReadNewGLogData]Temporarily unsupported");
        axCZKEM1.EnableDevice(GetMachineNumber(), true);//enable the device
        return ret;
    }
    #endregion
    #region  AttPhotoMng
    public int sta_GetAllAttPhoto(ListBox lblOutputInfo, string photoPath)
    {
        if (GetConnectState() == false)
        {
            lblOutputInfo.Items.Add("*Önce Terminal bağlantısını kurunuz!");
            return -1024;
        }
        int ret = 0;
        axCZKEM1.EnableDevice(iMachineNumber, false);
        string AllPhotoName = "";
        if (!axCZKEM1.GetPhotoNamesByTime(GetMachineNumber(), 0, "", "", out AllPhotoName))
        {
            axCZKEM1.GetLastError(ref idwErrorCode);
            ret = idwErrorCode;
            if (idwErrorCode != 0)
            {
                lblOutputInfo.Items.Add("*Get photo AdiSoyadi failed,ErrorCode: " + idwErrorCode.ToString());
            }
            else
            {
                lblOutputInfo.Items.Add("Terminal verisi bulunamadı!");
            }
        }
        else
        {
            int Photolenth = 0;
            byte[] PhotoData = new byte[20480];
            string photoname = "";
            int j = 0;
            int len = AllPhotoName.Length;
            char[] allphotoname = AllPhotoName.ToCharArray();
            string finalPath = photoPath + "ALL\\";
            System.IO.Directory.CreateDirectory(finalPath);
            for (j = 0; j < len; j++)
            {
                if (allphotoname[j].ToString() != "\t" && allphotoname[j].ToString() != "\n")
                {
                    photoname += allphotoname[j].ToString();
                }
                else if (photoname != "")
                {
                    photoname += ".jpg";
                    if (axCZKEM1.GetPhotoByName(GetMachineNumber(), photoname, out PhotoData[0], out Photolenth))
                    {
                        //convert byte to image and save
                        // Image img = Image.FromStream(new MemoryStream(PhotoData));
                        // img.Save(finalPath + photoname);
                        File.WriteAllBytes(finalPath + photoname, PhotoData);
                    }
                    else
                    {
                        axCZKEM1.GetLastError(ref idwErrorCode);
                        ret = idwErrorCode;
                        if (idwErrorCode != 0)
                        {
                            lblOutputInfo.Items.Add("*Get photo failed,ErrorCode: " + idwErrorCode.ToString());
                        }
                        else
                        {
                            lblOutputInfo.Items.Add("Terminal verisi bulunamadı!");
                        }
                        break;
                    }
                    photoname = "";
                }
            }
            lblOutputInfo.Items.Add("Get All ATT photo succeed.");
            ret = 1;
        }
        axCZKEM1.EnableDevice(GetMachineNumber(), true);//enable the device
        return ret;
    }
    public int sta_GetAllAttPhotoByTimePeriod(ListBox lblOutputInfo, string photoPath, string fromTime, string toTime)
    {
        if (GetConnectState() == false)
        {
            lblOutputInfo.Items.Add("*Önce Terminal bağlantısını kurunuz!");
            return -1024;
        }
        int ret = 0;
        axCZKEM1.EnableDevice(iMachineNumber, false);
        lblOutputInfo.Items.Add(fromTime + "-----" + toTime);
        string AllPhotoName = "";
        if (!axCZKEM1.GetPhotoNamesByTime(GetMachineNumber(), 1, fromTime, toTime, out AllPhotoName))
        {
            axCZKEM1.GetLastError(ref idwErrorCode);
            ret = idwErrorCode;
            if (idwErrorCode != 0)
            {
                lblOutputInfo.Items.Add("*Get photo AdiSoyadi failed,ErrorCode: " + idwErrorCode.ToString());
            }
            else
            {
                lblOutputInfo.Items.Add("Terminal verisi bulunamadı!");
            }
        }
        else
        {
            int Photolenth = 0;
            byte[] PhotoData = new byte[20480];
            string photoname = "";
            int j = 0;
            int len = AllPhotoName.Length;
            char[] allphotoname = AllPhotoName.ToCharArray();
            string finalPath = photoPath + "ALL" + "-From" + fromTime.Replace(":", ".") + "-To" + toTime.Replace(":", ".") + "\\";
            System.IO.Directory.CreateDirectory(finalPath);
            for (j = 0; j < len; j++)
            {
                if (allphotoname[j].ToString() != "\t" && allphotoname[j].ToString() != "\n")
                {
                    photoname += allphotoname[j].ToString();
                }
                else if (photoname != "")
                {
                    photoname += ".jpg";
                    if (axCZKEM1.GetPhotoByName(GetMachineNumber(), photoname, out PhotoData[0], out Photolenth))
                    {
                        //convert byte to image and save
                        //Image img = Image.FromStream(new MemoryStream(PhotoData));
                        //img.Save(finalPath + photoname);
                        File.WriteAllBytes(finalPath + photoname, PhotoData);
                    }
                    else
                    {
                        axCZKEM1.GetLastError(ref idwErrorCode);
                        ret = idwErrorCode;
                        if (idwErrorCode != 0)
                        {
                            lblOutputInfo.Items.Add("*Get photo failed,ErrorCode: " + idwErrorCode.ToString());
                        }
                        else
                        {
                            lblOutputInfo.Items.Add("Terminal verisi bulunamadı!");
                        }
                        break;
                    }
                    photoname = "";
                }
            }
            lblOutputInfo.Items.Add("GetAllAttPhotoByTimePeriod succeed.");
            ret = 1;
        }
        axCZKEM1.EnableDevice(GetMachineNumber(), true);//enable the device
        return ret;
    }
    public int sta_GetAllPassPhoto(ListBox lblOutputInfo, string photoPath)
    {
        if (GetConnectState() == false)
        {
            lblOutputInfo.Items.Add("*Önce Terminal bağlantısını kurunuz!");
            return -1024;
        }
        int ret = 0;
        axCZKEM1.EnableDevice(iMachineNumber, false);
        string AllPhotoName = "";
        if (!axCZKEM1.GetPhotoNamesByTime(GetMachineNumber(), 0, "", "", out AllPhotoName))
        {
            axCZKEM1.GetLastError(ref idwErrorCode);
            ret = idwErrorCode;
            if (idwErrorCode != 0)
            {
                lblOutputInfo.Items.Add("*Get photo AdiSoyadi failed,ErrorCode: " + idwErrorCode.ToString());
            }
            else
            {
                lblOutputInfo.Items.Add("Terminal verisi bulunamadı!");
            }
        }
        else
        {
            int Photolenth = 0;
            byte[] PhotoData = new byte[20480];
            string photoname = "";
            int j = 0;
            string[] Allphotoname = AllPhotoName.Split('\n');
            int len = Allphotoname[0].Length;
            char[] allphotoname = Allphotoname[0].ToCharArray();
            string finalPath = photoPath + "PASS\\";
            System.IO.Directory.CreateDirectory(finalPath);
            for (j = 0; j < len; j++)
            {
                if (allphotoname[j].ToString() != "\t")
                {
                    photoname += allphotoname[j].ToString();
                }
                else if (photoname != "")
                {
                    photoname += ".jpg";
                    if (axCZKEM1.GetPhotoByName(GetMachineNumber(), photoname, out PhotoData[0], out Photolenth))
                    {
                        //convert byte to image and save
                        // Image img = Image.FromStream(new MemoryStream(PhotoData));
                        // img.Save(finalPath + photoname);
                        File.WriteAllBytes(finalPath + photoname, PhotoData);
                    }
                    else
                    {
                        axCZKEM1.GetLastError(ref idwErrorCode);
                        ret = idwErrorCode;
                        if (idwErrorCode != 0)
                        {
                            lblOutputInfo.Items.Add("*Get photo failed,ErrorCode: " + idwErrorCode.ToString());
                        }
                        else
                        {
                            lblOutputInfo.Items.Add("Terminal verisi bulunamadı!");
                        }
                        break;
                    }
                    photoname = "";
                }
            }
            lblOutputInfo.Items.Add("Get All PASS photo succeed.");
            ret = 1;
        }
        axCZKEM1.EnableDevice(GetMachineNumber(), true);//enable the device
        return ret;
    }
    public int sta_GetPassPhotoByTimePeriod(ListBox lblOutputInfo, string photoPath, string fromTime, string toTime)
    {
        if (GetConnectState() == false)
        {
            lblOutputInfo.Items.Add("*Önce Terminal bağlantısını kurunuz!");
            return -1024;
        }
        int ret = 0;
        axCZKEM1.EnableDevice(iMachineNumber, false);
        string AllPhotoName = "";
        if (!axCZKEM1.GetPhotoNamesByTime(GetMachineNumber(), 1, fromTime, toTime, out AllPhotoName))
        {
            axCZKEM1.GetLastError(ref idwErrorCode);
            ret = idwErrorCode;
            if (idwErrorCode != 0)
            {
                lblOutputInfo.Items.Add("*Get photo AdiSoyadi failed,ErrorCode: " + idwErrorCode.ToString());
            }
            else
            {
                lblOutputInfo.Items.Add("Terminal verisi bulunamadı!");
            }
        }
        else
        {
            int Photolenth = 0;
            byte[] PhotoData = new byte[20480];
            string photoname = "";
            int j = 0;
            string[] Allphotoname = AllPhotoName.Split('\n');
            int len = Allphotoname[0].Length;
            char[] allphotoname = Allphotoname[0].ToCharArray();
            string finalPath = photoPath + "PASS" + "-From" + fromTime.Replace(":", ".") + "-To" + toTime.Replace(":", ".") + "\\";
            System.IO.Directory.CreateDirectory(finalPath);
            for (j = 0; j < len; j++)
            {
                if (allphotoname[j].ToString() != "\t")
                {
                    photoname += allphotoname[j].ToString();
                }
                else if (photoname != "")
                {
                    photoname += ".jpg";
                    if (axCZKEM1.GetPhotoByName(GetMachineNumber(), photoname, out PhotoData[0], out Photolenth))
                    {
                        //convert byte to image and save
                        //Image img = Image.FromStream(new MemoryStream(PhotoData));
                        //img.Save(finalPath + photoname);
                        File.WriteAllBytes(finalPath + photoname, PhotoData);
                    }
                    else
                    {
                        axCZKEM1.GetLastError(ref idwErrorCode);
                        ret = idwErrorCode;
                        if (idwErrorCode != 0)
                        {
                            lblOutputInfo.Items.Add("*Get photo failed,ErrorCode: " + idwErrorCode.ToString());
                        }
                        else
                        {
                            lblOutputInfo.Items.Add("Terminal verisi bulunamadı!");
                        }
                        break;
                    }
                    photoname = "";
                }
            }
            lblOutputInfo.Items.Add("GetPassPhotoByTimePeriod succeed.");
            ret = 1;
        }
        axCZKEM1.EnableDevice(GetMachineNumber(), true);//enable the device
        return ret;
    }
    public int sta_GetAllBadPhoto(ListBox lblOutputInfo, string photoPath)
    {
        if (GetConnectState() == false)
        {
            lblOutputInfo.Items.Add("*Önce Terminal bağlantısını kurunuz!");
            return -1024;
        }
        int ret = 0;
        axCZKEM1.EnableDevice(iMachineNumber, false);
        string AllPhotoName = "";
        if (!axCZKEM1.GetPhotoNamesByTime(GetMachineNumber(), 0, "", "", out AllPhotoName))
        {
            axCZKEM1.GetLastError(ref idwErrorCode);
            ret = idwErrorCode;
            if (idwErrorCode != 0)
            {
                lblOutputInfo.Items.Add("*Get photo AdiSoyadi failed,ErrorCode: " + idwErrorCode.ToString());
            }
            else
            {
                lblOutputInfo.Items.Add("Terminal verisi bulunamadı!");
            }
        }
        else
        {
            int Photolenth = 0;
            byte[] PhotoData = new byte[20480];
            string photoname = "";
            int j = 0;
            string[] Allphotoname = AllPhotoName.Split('\n');
            int len = Allphotoname[1].Length;
            char[] allphotoname = Allphotoname[1].ToCharArray();
            string finalPath = photoPath + "BAD\\";
            System.IO.Directory.CreateDirectory(finalPath);
            for (j = 0; j < len; j++)
            {
                if (allphotoname[j].ToString() != "\t")
                {
                    photoname += allphotoname[j].ToString();
                }
                else if (photoname != "")
                {
                    photoname += ".jpg";
                    if (axCZKEM1.GetPhotoByName(GetMachineNumber(), photoname, out PhotoData[0], out Photolenth))
                    {
                        //convert byte to image and save
                        //Image img = Image.FromStream(new MemoryStream(PhotoData));
                        //img.Save(finalPath + photoname);
                        File.WriteAllBytes(finalPath + photoname, PhotoData);
                    }
                    else
                    {
                        axCZKEM1.GetLastError(ref idwErrorCode);
                        ret = idwErrorCode;
                        if (idwErrorCode != 0)
                        {
                            lblOutputInfo.Items.Add("*Get photo failed,ErrorCode: " + idwErrorCode.ToString());
                        }
                        else
                        {
                            lblOutputInfo.Items.Add("Terminal verisi bulunamadı!");
                        }
                        break;
                    }
                    photoname = "";
                }
            }
            lblOutputInfo.Items.Add("Get All BAD photo succeed.");
            ret = 1;
        }
        axCZKEM1.EnableDevice(GetMachineNumber(), true);//enable the device
        return ret;
    }
    public int sta_GetBadPhotoByTimePeriod(ListBox lblOutputInfo, string photoPath, string fromTime, string toTime)
    {
        if (GetConnectState() == false)
        {
            lblOutputInfo.Items.Add("*Önce Terminal bağlantısını kurunuz!");
            return -1024;
        }
        int ret = 0;
        axCZKEM1.EnableDevice(iMachineNumber, false);
        string AllPhotoName = "";
        if (!axCZKEM1.GetPhotoNamesByTime(GetMachineNumber(), 1, fromTime, toTime, out AllPhotoName))
        {
            axCZKEM1.GetLastError(ref idwErrorCode);
            ret = idwErrorCode;
            if (idwErrorCode != 0)
            {
                lblOutputInfo.Items.Add("*Get photo AdiSoyadi failed,ErrorCode: " + idwErrorCode.ToString());
            }
            else
            {
                lblOutputInfo.Items.Add("Terminal verisi bulunamadı!");
            }
        }
        else
        {
            int Photolenth = 0;
            byte[] PhotoData = new byte[20480];
            string photoname = "";
            int j = 0;
            string[] Allphotoname = AllPhotoName.Split('\n');
            int len = Allphotoname[1].Length;
            char[] allphotoname = Allphotoname[1].ToCharArray();
            string finalPath = photoPath + "BAD" + "-From" + fromTime.Replace(":", ".") + "-To" + toTime.Replace(":", ".") + "\\";
            System.IO.Directory.CreateDirectory(finalPath);
            for (j = 0; j < len; j++)
            {
                if (allphotoname[j].ToString() != "\t")
                {
                    photoname += allphotoname[j].ToString();
                }
                else if (photoname != "")
                {
                    photoname += ".jpg";
                    if (axCZKEM1.GetPhotoByName(GetMachineNumber(), photoname, out PhotoData[0], out Photolenth))
                    {
                        //convert byte to image and save
                        //Image img = Convert.ToBase64String(PhotoData); //Image.FromStream(new MemoryStream(PhotoData));
                        //img.Save(finalPath + photoname);
                        // Web için uygun Karakaş
                        File.WriteAllBytes(finalPath + photoname, PhotoData);
                    }
                    else
                    {
                        axCZKEM1.GetLastError(ref idwErrorCode);
                        ret = idwErrorCode;
                        if (idwErrorCode != 0)
                        {
                            lblOutputInfo.Items.Add("*Get photo failed,ErrorCode: " + idwErrorCode.ToString());
                        }
                        else
                        {
                            lblOutputInfo.Items.Add("Terminal verisi bulunamadı!");
                        }
                        break;
                    }
                    photoname = "";
                }
            }
            lblOutputInfo.Items.Add("GetBadPhotoByTimePeriod succeed.");
            ret = 1;
        }
        axCZKEM1.EnableDevice(GetMachineNumber(), true);//enable the device
        return ret;
    }
    public int sta_ClearAllAttPhoto(ListBox lblOutputInfo, int iFlag, string fromTime, string toTime)
    {
        if (GetConnectState() == false)
        {
            lblOutputInfo.Items.Add("*Önce Terminal bağlantısını kurunuz!");
            return -1024;
        }
        int ret = 0;
        axCZKEM1.EnableDevice(iMachineNumber, false);
        if (axCZKEM1.ClearPhotoByTime(iMachineNumber, iFlag, fromTime, toTime))
        {
            lblOutputInfo.Items.Add("Clear capture picture OK");
        }
        else
        {
            int errorcode = -1;
            axCZKEM1.GetLastError(ref errorcode);
            lblOutputInfo.Items.Add("Clear capture picture Failed" + errorcode.ToString());
        }
        axCZKEM1.EnableDevice(GetMachineNumber(), true);//enable the device
        return ret;
    }
    #endregion
    #region OPLOG
    public int sta_GetOplog(ListBox lblOutputInfo, DataTable dt_Oplog)
    {
        if (GetConnectState() == false)
        {
            lblOutputInfo.Items.Add("*Önce Terminal bağlantısını kurunuz!");
            return -1024;
        }
        int ret = 0;
        int iSuperLogCount = 0;
        axCZKEM1.EnableDevice(GetMachineNumber(), false);
        //if (axCZKEM1.ReadSuperLogData(GetMachineNumber()))
        if (axCZKEM1.ReadAllSLogData(GetMachineNumber()))
        {
            int idwTMachineNumber = 0;
            int iParams1 = 0;
            int iParams2 = 0;
            int idwManipulation = 0;
            int iParams3 = 0;
            int iParams4 = 0;
            int iYear = 0;
            int iMonth = 0;
            int iDay = 0;
            int iHour = 0;
            int iMin = 0;
            int iSencond = 0;
            int iAdmin = 0;
            string sUser = null;
            string sAdmin = null;
            string sTime = null;
            //while (axCZKEM1.SSR_GetSuperLogData(GetMachineNumber(), out idwTMachineNumber, out sAdmin, out sUser,
            //    out idwManipulation, out sTime, out iParams1, out iParams2, out iParams3))
            while (axCZKEM1.GetSuperLogData2(GetMachineNumber(), ref idwTMachineNumber, ref iAdmin, ref iParams4, ref iParams1, ref iParams2, ref idwManipulation, ref iParams3, ref iYear, ref iMonth, ref iDay, ref iHour, ref iMin, ref iSencond))
            {
                iSuperLogCount++;
                DataRow dr = dt_Oplog.NewRow();
                dr["Count"] = iSuperLogCount;
                dr["MachineNumber"] = GetMachineNumber();
                dr["Admin"] = iAdmin;
                //dr["UserPIN2"] = sUser;
                dr["Operation"] = idwManipulation;
                sTime = iYear + "-" + iMonth + "-" + iDay + " " + iHour + ":" + iMin + ":" + iSencond;
                dr["DateTime"] = sTime;
                dr["Param1"] = iParams1;
                dr["Param2"] = iParams2;
                dr["Param3"] = iParams3;
                dr["Param4"] = iParams4;
                dt_Oplog.Rows.Add(dr);
            }
            lblOutputInfo.Items.Add("Down oplog success.");
            ret = 1;
        }
        else
        {
            axCZKEM1.GetLastError(ref idwErrorCode);
            ret = idwErrorCode;
            if (idwErrorCode != 0)
            {
                lblOutputInfo.Items.Add("*Get OPLOG failed,ErrorCode: " + idwErrorCode.ToString());
            }
            else
            {
                lblOutputInfo.Items.Add("Terminal verisi bulunamadı!");
            }
        }
        axCZKEM1.EnableDevice(GetMachineNumber(), true);
        return ret;
    }
    public int sta_ClearOplog(ListBox lblOutputInfo)
    {
        if (GetConnectState() == false)
        {
            lblOutputInfo.Items.Add("*Önce Terminal bağlantısını kurunuz!");
            return -1024;
        }
        int ret = 0;
        axCZKEM1.EnableDevice(GetMachineNumber(), false);
        if (axCZKEM1.ClearSLog(GetMachineNumber()))
        {
            axCZKEM1.RefreshData(GetMachineNumber());//the data in the device should be refreshed
            lblOutputInfo.Items.Add("All operation logs have been cleared from teiminal!");
            ret = 1;
        }
        else
        {
            axCZKEM1.GetLastError(ref idwErrorCode);
            if (idwErrorCode != 0)
            {
                lblOutputInfo.Items.Add("ClearOplog failed,ErrorCode=" + idwErrorCode.ToString());
            }
            else
            {
                lblOutputInfo.Items.Add("Terminal verisi bulunamadı!");
            }
            ret = idwErrorCode;
        }
        axCZKEM1.EnableDevice(GetMachineNumber(), true);
        return ret;
    }
    #endregion
    #region ClearData
    public int sta_ClearAdmin(ListBox lblOutputInfo)
    {
        if (GetConnectState() == false)
        {
            lblOutputInfo.Items.Add("*Önce Terminal bağlantısını kurunuz!");
            return -1024;
        }
        int ret = 0;
        axCZKEM1.EnableDevice(GetMachineNumber(), false);
        if (axCZKEM1.ClearAdministrators(GetMachineNumber()))
        {
            axCZKEM1.RefreshData(GetMachineNumber());//the data in the device should be refreshed
            lblOutputInfo.Items.Add("All administrator have been cleared from teiminal!");
            ret = 1;
        }
        else
        {
            axCZKEM1.GetLastError(ref idwErrorCode);
            if (idwErrorCode != 0)
            {
                lblOutputInfo.Items.Add("*ClearAdmin failed,ErrorCode=" + idwErrorCode.ToString());
            }
            else
            {
                lblOutputInfo.Items.Add("Terminal verisi bulunamadı!");
            }
            ret = idwErrorCode;
        }
        axCZKEM1.EnableDevice(GetMachineNumber(), true);
        return ret;
    }
    public int sta_ClearAllLogs(ListBox lblOutputInfo)
    {
        if (GetConnectState() == false)
        {
            lblOutputInfo.Items.Add("*Önce Terminal bağlantısını kurunuz!");
            return -1024;
        }
        int ret = 0;
        axCZKEM1.EnableDevice(GetMachineNumber(), false);
        if (axCZKEM1.ClearData(GetMachineNumber(), 1))
        {
            axCZKEM1.RefreshData(GetMachineNumber());//the data in the device should be refreshed
            lblOutputInfo.Items.Add("All AttLogs have been cleared from teiminal!");
            ret = 1;
        }
        else
        {
            axCZKEM1.GetLastError(ref idwErrorCode);
            if (idwErrorCode != 0)
            {
                lblOutputInfo.Items.Add("*ClearAllLogs failed,ErrorCode=" + idwErrorCode.ToString());
            }
            else
            {
                lblOutputInfo.Items.Add("Terminal verisi bulunamadı!");
            }
            ret = idwErrorCode;
        }
        axCZKEM1.EnableDevice(GetMachineNumber(), true);
        return ret;
    }
    public int sta_ClearAllFps(ListBox lblOutputInfo)
    {
        if (GetConnectState() == false)
        {
            lblOutputInfo.Items.Add("*Önce Terminal bağlantısını kurunuz!");
            return -1024;
        }
        int ret = 0;
        axCZKEM1.EnableDevice(GetMachineNumber(), false);
        if (axCZKEM1.ClearData(GetMachineNumber(), 2))
        {
            axCZKEM1.RefreshData(GetMachineNumber());//the data in the device should be refreshed
            lblOutputInfo.Items.Add("All fp templates have been cleared from teiminal!");
            ret = 1;
        }
        else
        {
            axCZKEM1.GetLastError(ref idwErrorCode);
            if (idwErrorCode != 0)
            {
                lblOutputInfo.Items.Add("*ClearAllFps failed,ErrorCode=" + idwErrorCode.ToString());
            }
            else
            {
                lblOutputInfo.Items.Add("Terminal verisi bulunamadı!");
            }
            ret = idwErrorCode;
        }
        axCZKEM1.EnableDevice(GetMachineNumber(), true);
        return ret;
    }
    public int sta_ClearAllUsers(ListBox lblOutputInfo)
    {
        if (GetConnectState() == false)
        {
            lblOutputInfo.Items.Add("*Önce Terminal bağlantısını kurunuz!");
            return -1024;
        }
        int ret = 0;
        axCZKEM1.EnableDevice(GetMachineNumber(), false);
        if (axCZKEM1.ClearData(GetMachineNumber(), 5))
        {
            axCZKEM1.RefreshData(GetMachineNumber());//the data in the device should be refreshed
            lblOutputInfo.Items.Add("All users have been cleared from teiminal!");
            ret = 1;
        }
        else
        {
            axCZKEM1.GetLastError(ref idwErrorCode);
            if (idwErrorCode != 0)
            {
                lblOutputInfo.Items.Add("*ClearAllUsers failed,ErrorCode=" + idwErrorCode.ToString());
            }
            else
            {
                lblOutputInfo.Items.Add("Terminal verisi bulunamadı!");
            }
            ret = idwErrorCode;
        }
        axCZKEM1.EnableDevice(GetMachineNumber(), true);
        return ret;
    }
    public int sta_ClearAllData(ListBox lblOutputInfo)
    {
        if (GetConnectState() == false)
        {
            lblOutputInfo.Items.Add("*Önce Terminal bağlantısını kurunuz!");
            return -1024;
        }
        int ret = 0;
        axCZKEM1.EnableDevice(GetMachineNumber(), false);
        if (axCZKEM1.ClearKeeperData(GetMachineNumber()))
        {
            axCZKEM1.RefreshData(GetMachineNumber());//the data in the device should be refreshed
            lblOutputInfo.Items.Add("All Data have been cleared from teiminal!");
            ret = 1;
        }
        else
        {
            axCZKEM1.GetLastError(ref idwErrorCode);
            if (idwErrorCode != 0)
            {
                lblOutputInfo.Items.Add("*ClearAllData failed,ErrorCode=" + idwErrorCode.ToString());
            }
            else
            {
                lblOutputInfo.Items.Add("Terminal verisi bulunamadı!");
            }
            ret = idwErrorCode;
        }
        axCZKEM1.EnableDevice(GetMachineNumber(), true);
        return ret;
    }
    #endregion
    #endregion
    #region UserGroup
    public int sta_GetUserGroup(ListBox lblOutputInfo, string cboUAUserIDGroup, TextBox txtGroupNo1)
    {
        if (GetConnectState() == false)
        {
            lblOutputInfo.Items.Add("*Please connect the device first!");
            return -1024;
        }
        if (cboUAUserIDGroup.Trim() == "")
        {
            lblOutputInfo.Items.Add("*Please input data first!");
            return -1023;
        }
        int idwErrorCode = 0;
        int iUserID = Convert.ToInt32(cboUAUserIDGroup.Trim());
        int iUserGrp = 0;
        if (axCZKEM1.GetUserGroup(iMachineNumber, iUserID, ref iUserGrp))
        {
            txtGroupNo1.Text = iUserGrp.ToString();
            lblOutputInfo.Items.Add("Get user group successfully");
        }
        else
        {
            axCZKEM1.GetLastError(ref idwErrorCode);
            lblOutputInfo.Items.Add("*Operation failed,ErrorCode=" + idwErrorCode.ToString());
            return idwErrorCode;
        }
        return 1;
    }
    public int sta_SetUserGroup(ListBox lblOutputInfo, string cboUAUserIDGroup, TextBox txtGroupNo1)
    {
        if (GetConnectState() == false)
        {
            lblOutputInfo.Items.Add("*Please connect the device first!");
            return -1024;
        }
        if (cboUAUserIDGroup.Trim() == "" || txtGroupNo1.Text.Trim() == "")
        {
            lblOutputInfo.Items.Add("*Please input data first!");
            return -1023;
        }
        int idwErrorCode = 0;
        int iUserID = Convert.ToInt32(cboUAUserIDGroup.Trim());
        int iUserGrp = Convert.ToInt32(txtGroupNo1.Text.Trim());
        if (axCZKEM1.SetUserGroup(iMachineNumber, iUserID, iUserGrp))
        {
            axCZKEM1.RefreshData(iMachineNumber);//the data in the device should be refreshed
            lblOutputInfo.Items.Add("Set User Group, UserID:" + iUserID.ToString() + ", Group No:" + iUserGrp.ToString());
        }
        else
        {
            axCZKEM1.GetLastError(ref idwErrorCode);
            lblOutputInfo.Items.Add("*Operation failed,ErrorCode=" + idwErrorCode.ToString());
            return idwErrorCode;
        }
        return 1;
    }
    #endregion

    #region controldevice
    public int sta_ACUnlock(ListBox lblOutputInfo, TextBox txtDelay)
    {
        if (GetConnectState() == false)
        {
            lblOutputInfo.Items.Add("*Please connect the device first!");
            return -1024;
        }
        if (txtDelay.Text.Trim() == "")
        {
            lblOutputInfo.Items.Add("*Please input data first!");
            return -1023;
        }
        if (Convert.ToInt32(txtDelay.Text.Trim()) < 0 || Convert.ToInt32(txtDelay.Text.Trim()) > 10)
        {
            lblOutputInfo.Items.Add("*Delay error!");
            return -1022;
        }
        int idwErrorCode = 0;
        int iDelay = Convert.ToInt32(txtDelay.Text.Trim());//time to delay
        if (axCZKEM1.ACUnlock(iMachineNumber, iDelay))
        {
            lblOutputInfo.Items.Add("ACUnlock, Dalay Seconds:" + iDelay.ToString());
        }
        else
        {
            axCZKEM1.GetLastError(ref idwErrorCode);
            lblOutputInfo.Items.Add("*Operation failed,ErrorCode=" + idwErrorCode.ToString());
            return idwErrorCode;
        }
        return 1;
    }
    #endregion
    #region get and set wiegandfmt
    public int sta_GetWiegandFmt(ListBox lblOutputInfo, TextBox txtShowWiegandFmt)
    {
        if (GetConnectState() == false)
        {
            lblOutputInfo.Items.Add("*Please connect the device first!");
            return -1024;
        }
        string sWiegandFmt = "";
        int idwErrorCode = 0;
        if (axCZKEM1.GetWiegandFmt(iMachineNumber, ref sWiegandFmt))
        {
            txtShowWiegandFmt.Text = sWiegandFmt;
            lblOutputInfo.Items.Add("Operation Successed！");
        }
        else
        {
            axCZKEM1.GetLastError(ref idwErrorCode);
            lblOutputInfo.Items.Add("*Operation failed,ErrorCode=" + idwErrorCode.ToString());
            return idwErrorCode;
        }
        return 1;
    }
    public int sta_SetWiegandFmt(ListBox lblOutputInfo, TextBox txtSetWiegandFmt)
    {
        if (GetConnectState() == false)
        {
            lblOutputInfo.Items.Add("*Please connect the device first!");
            return -1024;
        }
        int idwErrorCode = 0;
        string sWiegandFmt = txtSetWiegandFmt.Text.Trim();
        if (axCZKEM1.SetWiegandFmt(iMachineNumber, sWiegandFmt))
        {
            axCZKEM1.RefreshData(iMachineNumber);//the data in the device should be refreshed
            lblOutputInfo.Items.Add("Operation Successed！");
        }
        else
        {
            axCZKEM1.GetLastError(ref idwErrorCode);
            lblOutputInfo.Items.Add("*Operation failed,ErrorCode=" + idwErrorCode.ToString());
            return idwErrorCode;
        }
        return 1;
    }
    #endregion
    #region Holiday
    //public int sta_SetHoliday(ListBox lblOutputInfo, TextBox txtHolidayId, DateTimePicker dtStartDate, DateTimePicker dtEndDate, TextBox txtTimeZoneId)
    //{
    //    if (GetConnectState() == false)
    //    {
    //        lblOutputInfo.Items.Add("*Please connect the device first!");
    //        return -1024;
    //    }
    //    if (txtHolidayId.Text.Trim() == "" || txtTimeZoneId.Text.Trim() == "")
    //    {
    //        lblOutputInfo.Items.Add("*Please input data first!");
    //        return -1023;
    //    }
    //    int iHolidayId = Convert.ToInt32(txtHolidayId.Text.Trim());
    //    if (iHolidayId < 1 || iHolidayId > 99)
    //    {
    //        lblOutputInfo.Items.Add("*Holiday ID error");
    //        return -1022;
    //    }
    //    int iTimezomeId = Convert.ToInt32(txtTimeZoneId.Text.Trim());
    //    if (iTimezomeId < 1 || iTimezomeId > 50)
    //    {
    //        lblOutputInfo.Items.Add("*Timezone index error!");
    //        return -1023;
    //    }
    //    int idwErrorCode = 0;
    //    DateTime dStartDate = DateTime.Parse(dtStartDate.Text);
    //    DateTime dEndDate = DateTime.Parse(dtEndDate.Text);
    //    int iSMonth = dStartDate.Month;
    //    int iSDay = dStartDate.Day;
    //    int iEMonth = dEndDate.Month;
    //    int iEDay = dEndDate.Day;
    //    if (axCZKEM1.SSR_SetHoliday(iMachineNumber, iHolidayId, iSMonth, iSDay, iEMonth, iEDay, iTimezomeId))
    //    {
    //        axCZKEM1.RefreshData(iMachineNumber);//the data in the device should be refreshed
    //        lblOutputInfo.Items.Add("Operation Successed！");
    //    }
    //    else
    //    {
    //        axCZKEM1.GetLastError(ref idwErrorCode);
    //        lblOutputInfo.Items.Add("*Operation failed,ErrorCode=" + idwErrorCode.ToString());
    //        return idwErrorCode;
    //    }
    //    return 1;
    //}
    //public int sta_GetHoliday(ListBox lblOutputInfo, TextBox txtHolidayId, DateTimePicker dtStartDate, DateTimePicker dtEndDate, TextBox txtTimeZoneId)
    //{
    //    if (GetConnectState() == false)
    //    {
    //        lblOutputInfo.Items.Add("*Please connect the device first!");
    //        return -1024;
    //    }
    //    if (txtHolidayId.Text.Trim() == "")
    //    {
    //        lblOutputInfo.Items.Add("*Please input data first!");
    //        return -1023;
    //    }
    //    int iHolidayId = Convert.ToInt32(txtHolidayId.Text.Trim());
    //    if (iHolidayId < 1 || iHolidayId > 99)
    //    {
    //        lblOutputInfo.Items.Add("*Holiday ID error");
    //        return -1022;
    //    }
    //    int idwErrorCode = 0;
    //    int iSMonth = 0;
    //    int iSDay = 0;
    //    int iEMonth = 0;
    //    int iEDay = 0;
    //    int iTimeZoneId = 0;
    //    if (axCZKEM1.SSR_GetHoliday(iMachineNumber, iHolidayId, ref iSMonth, ref iSDay, ref iEMonth, ref iEDay, ref iTimeZoneId))
    //    {
    //        dtStartDate.Text = iSMonth.ToString() + " " + iSDay.ToString();
    //        dtEndDate.Text = iEMonth.ToString() + " " + iEDay.ToString();
    //        txtTimeZoneId.Text = iTimeZoneId.ToString();
    //        lblOutputInfo.Items.Add("Get holiday successfully");
    //    }
    //    else
    //    {
    //        axCZKEM1.GetLastError(ref idwErrorCode);
    //        lblOutputInfo.Items.Add("*Operation failed,ErrorCode=" + idwErrorCode.ToString());
    //        return idwErrorCode;
    //    }
    //    return 1;
    //}
    #endregion
    #region Set&Get SystemOption
    public int sta_SetNONCTimeZone(ListBox lblOutputInfo, int parName, TextBox parm)
    {
        if (GetConnectState() == false)
        {
            lblOutputInfo.Items.Add("*Please connect the device first!");
            return -1024;
        }
        if (parm.Text.Trim() == "")
        {
            lblOutputInfo.Items.Add("*Please input data first!");
            return -1023;
        }
        int idwErrorCode = 0;
        string par = parm.Text.Trim();
        //int tmpPar = 0;
        string strTmpPar = "";
        string strParName = "";
        if (Convert.ToInt32(par) < 0 || Convert.ToInt32(par) > 50)
        {
            lblOutputInfo.Items.Add("*The timezone index error!");
            return -1022;
        }
        if (Convert.ToInt32(par) != 0)
        {
            if (parName == 81)
            {
                strParName = "~DCTZ";
            }
            else if (parName == 80)
            {
                strParName = "~DOTZ";
            }
            else
            {
                return -1020;
            }
            axCZKEM1.GetSysOption(iMachineNumber, strParName, out strTmpPar);
            if (strTmpPar.Equals(par))
            {
                lblOutputInfo.Items.Add("*The NO and NC can not be same!");
                return -1021;
            }
        }
        if (parName == 81)
        {
            strParName = "~DOTZ";
        }
        else if (parName == 80)
        {
            strParName = "~DCTZ";
        }
        if (axCZKEM1.SetSysOption(iMachineNumber, strParName, par))
        {
            lblOutputInfo.Items.Add("Operation Successed!");
        }
        else
        {
            axCZKEM1.GetLastError(ref idwErrorCode);
            lblOutputInfo.Items.Add("*Operation failed,ErrorCode=" + idwErrorCode.ToString());
            return idwErrorCode;
        }
        return 1;
    }
    public int sta_GetNONCTimeZone(ListBox lblOutputInfo, int parName, TextBox parm)
    {
        if (GetConnectState() == false)
        {
            lblOutputInfo.Items.Add("*Please connect the device first!");
            return -1024;
        }
        int idwErrorCode = 0;
        //int par = 0;
        /*
        if (axCZKEM1.GetDeviceInfo(iMachineNumber, parName, ref par))
        {
            parm.Text = par.ToString();
            lblOutputInfo.Items.Add("Get NO/NC TZ successfully");
        }
        else
        {
            axCZKEM1.GetLastError(ref idwErrorCode);
            lblOutputInfo.Items.Add("*Operation failed,ErrorCode=" + idwErrorCode.ToString());
            return idwErrorCode;
        }
         */
        string strParName = "";
        string par = "";
        if (parName == 81)
        {
            strParName = "~DOTZ";
        }
        else if (parName == 80)
        {
            strParName = "~DCTZ";
        }
        if (axCZKEM1.GetSysOption(iMachineNumber, strParName, out par))
        {
            parm.Text = par.ToString();
            lblOutputInfo.Items.Add("Get NO/NC TZ successfully");
        }
        else
        {
            axCZKEM1.GetLastError(ref idwErrorCode);
            lblOutputInfo.Items.Add("*Operation failed,ErrorCode=" + idwErrorCode.ToString());
            return idwErrorCode;
        }
        return 1;
    }
    #endregion
    #endregion

    //Synchronize the device time as the computer's.
    public int sta_SYNCTime(ListBox lblOutputInfo, Label lbDeviceTime)
    {
        if (GetConnectState() == false)
        {
            lblOutputInfo.Items.Add("*Önce Terminal bağlantısını kurunuz!");
            return -1024;
        }
        if (axCZKEM1.SetDeviceTime(iMachineNumber))
        {
            axCZKEM1.RefreshData(iMachineNumber);//the data in the device should be refreshed
            lblOutputInfo.Items.Add("Successfully SYNC the PC's time to device!");
        }
        else
        {
            axCZKEM1.GetLastError(ref idwErrorCode);
            lblOutputInfo.Items.Add("*Operation failed,ErrorCode=" + idwErrorCode.ToString());
        }
        return 1;
    }
    public int sta_GetDeviceTime(ListBox lblOutputInfo, Label lbDeviceTime)
    {
        if (GetConnectState() == false)
        {
            lblOutputInfo.Items.Add("*Önce Terminal bağlantısını kurunuz!");
            return -1024;
        }
        int idwYear = 0;
        int idwMonth = 0;
        int idwDay = 0;
        int idwHour = 0;
        int idwMinute = 0;
        int idwSecond = 0;
        if (axCZKEM1.GetDeviceTime(iMachineNumber, ref idwYear, ref idwMonth, ref idwDay, ref idwHour, ref idwMinute, ref idwSecond))//show the time
        {
            lbDeviceTime.Text = idwYear.ToString() + "-" + idwMonth.ToString() + "-" + idwDay.ToString() + " " + idwHour.ToString() + ":" + idwMinute.ToString() + ":" + idwSecond.ToString();
            lblOutputInfo.Items.Add("Get devie time successfully");
        }
        else
        {
            axCZKEM1.GetLastError(ref idwErrorCode);
            lblOutputInfo.Items.Add("*Operation failed,ErrorCode=" + idwErrorCode.ToString());
        }
        return 1;
    }


    #region Kontrol
    public int sta_btnRestartDevice(ListBox lblOutputInfo)
    {
        if (GetConnectState() == false)
        {
            lblOutputInfo.Items.Add("*Önce Terminal bağlantısını kurunuz!");
            return -1024;
        }
        if (axCZKEM1.RestartDevice(iMachineNumber))
        {
            sta_DisConnect();
            lblOutputInfo.Items.Add("The device will restart");
        }
        else
        {
            axCZKEM1.GetLastError(ref idwErrorCode);
            lblOutputInfo.Items.Add("*Operation failed,ErrorCode=" + idwErrorCode.ToString());
        }
        return 1;
    }
    public int sta_btnPowerOffDevice(ListBox lblOutputInfo)
    {
        if (GetConnectState() == false)
        {
            lblOutputInfo.Items.Add("*Önce Terminal bağlantısını kurunuz!");
            return -1024;
        }
        if (axCZKEM1.PowerOffDevice(iMachineNumber))
        {
            sta_DisConnect();
            lblOutputInfo.Items.Add("Power off device");
        }
        else
        {
            axCZKEM1.GetLastError(ref idwErrorCode);
            lblOutputInfo.Items.Add("*Operation failed,ErrorCode=" + idwErrorCode.ToString());
        }
        return 1;
    }
    #endregion
    #region Guncellemeler
    public int sta_btnUpdateFirmware(ListBox lblOutputInfo, TextBox txtFirmwareFile)
    {
        if (GetConnectState() == false)
        {
            lblOutputInfo.Items.Add("*Önce Terminal bağlantısını kurunuz!");
            return -1024;
        }
        int idwErrorCode = 0;
        string sFirmwareFile = txtFirmwareFile.Text.Trim();
        if (axCZKEM1.UpdateFirmware(sFirmwareFile))
        {
            lblOutputInfo.Items.Add("UpdateFirmware,Name=" + sFirmwareFile);
        }
        else
        {
            axCZKEM1.GetLastError(ref idwErrorCode);
            lblOutputInfo.Items.Add("*Operation failed,ErrorCode=" + idwErrorCode.ToString());
        }
        return 1;
    }
    #endregion
    #region R/W Dosya
    public int sta_btnSendFile(ListBox lblOutputInfo, TextBox txtSendFileName)
    {
        if (GetConnectState() == false)
        {
            lblOutputInfo.Items.Add("*Önce Terminal bağlantısını kurunuz!");
            return -1024;
        }
        if (txtSendFileName.Text.Trim() == "")
        {
            lblOutputInfo.Items.Add("*Please input the FileName  first!");
            return -1023;
        }
        int idwErrorCode = 0;
        string sFileName = txtSendFileName.Text.Trim();
        if (axCZKEM1.SendFile(iMachineNumber, sFileName))
        {
            axCZKEM1.RefreshData(iMachineNumber);//the data in the device should be refreshed
            lblOutputInfo.Items.Add("SendFile " + sFileName + " To the Device! ");
        }
        else
        {
            axCZKEM1.GetLastError(ref idwErrorCode);
            lblOutputInfo.Items.Add("*Operation failed,ErrorCode=" + idwErrorCode.ToString());
        }
        return 1;
    }
    public int sta_btnReadFile(ListBox lblOutputInfo, TextBox txtReadFileName, TextBox txtFilePath)
    {
        if (GetConnectState() == false)
        {
            lblOutputInfo.Items.Add("*Önce Terminal bağlantısını kurunuz!");
            return -1024;
        }
        if (txtFilePath.Text.Trim() == "" || txtReadFileName.Text.Trim() == "")
        {
            lblOutputInfo.Items.Add("*Please input the FileName and FilePath first!");
            return -1023;
        }
        int idwErrorCode = 0;
        string sFileName = txtReadFileName.Text.Trim();
        string sFilePath = txtFilePath.Text.Trim();
        if (axCZKEM1.ReadFile(iMachineNumber, sFileName, sFilePath))
        {
            lblOutputInfo.Items.Add("ReadFile " + sFileName + " To " + sFilePath);
        }
        else
        {
            axCZKEM1.GetLastError(ref idwErrorCode);
            lblOutputInfo.Items.Add("*Operation failed,ErrorCode=" + idwErrorCode.ToString());
        }
        return 1;
    }

    #endregion
}
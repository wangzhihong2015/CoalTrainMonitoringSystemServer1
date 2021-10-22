using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using SDK;
using System.Net;
using System.Runtime.InteropServices;
using System.IO;
using System.Xml.Serialization;
using System.Media;
using MvCamCtrl.NET;
using System.Drawing.Imaging;
using System.Net.Sockets;

namespace CoalTrainMonitoringSystemServer
{
    public partial class FormMain : Form
    {
        public delegate void delegateDestroy();//声明一个委托
        public static delegateDestroy OnDestroy = null; //实例化一个委托类型

        public static string systemXml = Application.StartupPath + "\\SystemSetting.xml";
        public static SystemParam sysParam = new SystemParam();

        DataControl _DataControl = null;
        FormDisplay[] _FormDisplayLst;
        FormDisplayBG[] _FormDisplayBG;
        MagDevice DdtMagDevice = new MagDevice(IntPtr.Zero);

        uint dev_num;
        bool checkFlag = false;

        public static DeviceSettings setting1 = new DeviceSettings();
        public static DeviceSettings setting2 = new DeviceSettings();
        public static DeviceSettings setting3 = new DeviceSettings();
        public static DeviceSettings setting4 = new DeviceSettings();

        const int MAX_ENUMDEVICE = 32;
        const uint MAX_DEVWINDOW_NUM = 4;   //显示窗口最大数量
        const int BORDER_WIDTH = 15;
        const int SPLITTER_DISTANCE = 25;
        GroupSDK.ENUM_INFO[] _LstEnumInfo = new GroupSDK.ENUM_INFO[MAX_ENUMDEVICE];
        List<String> folderName = new List<string>();

        //可见光
        MyCamera.MV_CC_DEVICE_INFO_LIST m_stDeviceList = new MyCamera.MV_CC_DEVICE_INFO_LIST();
        private MyCamera m_MyCamera = new MyCamera();
        bool m_bGrabbing = false;
        Thread m_hReceiveThread = null;
        MyCamera.MV_FRAME_OUT_INFO_EX m_stFrameInfo = new MyCamera.MV_FRAME_OUT_INFO_EX();

        // ch:用于从驱动获取图像的缓存 | en:Buffer for getting image from driver
        UInt32 m_nBufSizeForDriver = 0;
        IntPtr m_BufForDriver;
        private static Object BufForDriverLock = new Object();

        // ch:用于保存图像的缓存 | en:Buffer for saving image
        UInt32 m_nBufSizeForSaveImage = 0;
        IntPtr m_BufForSaveImage;

        List<String> imagePathList = new List<string>();
        List<String> thermalImagePathList = new List<string>();

        public static List<AlarmListInfo> alarmListInfoList_1 = new List<AlarmListInfo>();
        public static AlarmListInfo alarmListInfo_1 = new AlarmListInfo();
        public static AlarmInfo alarmInfo_1 = new AlarmInfo();

        public static AlarmListInfo alarmListInfo_2 = new AlarmListInfo();
        public static AlarmInfo alarmInfo_2 = new AlarmInfo();

        public static AlarmListInfo alarmListInfo_3 = new AlarmListInfo();
        public static AlarmInfo alarmInfo_3 = new AlarmInfo();

        public static AlarmListInfo alarmListInfo_4 = new AlarmListInfo();
        public static AlarmInfo alarmInfo_4 = new AlarmInfo();

        public static AlarmListInfo alarmListInfo_Read = new AlarmListInfo();
        //public static AlarmInfo alarmInfo = new AlarmInfo();

        /// <summary>
        /// 热成像图片文件数组
        /// </summary>
        FileInfo[] ThermalFiles;
        /// <summary>
        /// 可见光图片文件数组
        /// </summary>
        FileInfo[] CameraFiles;
        /// <summary>
        /// 热成像图形字体
        /// </summary>
        Font font1 = new Font("宋体", 30F);
        Pen penLightGreen = new Pen(Brushes.LightGreen);

        /// <summary>
        /// 1#设备标题文本框
        /// </summary>
        private TextBox txtInput_1;

        //服务端的IP地址
        //可通过指令ipconfig 在cmd上查询电脑IP
        string serverIP = "192.168.0.120";
        //服务端的端口
        int serverPort = 6000;

        public static Socket socketClient;

        //获取最大显示窗口数量
        public uint GetMaxDeviceWnd()
        {
            return MAX_DEVWINDOW_NUM;
        }
        public uint GetDeviceNum()
        {
            return dev_num;
        }

        public FormMain()
        {

            InitializeComponent();

            CheckForIllegalCrossThreadCalls = false;//允许跨线程调用
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true); // 禁止擦除背景.
            SetStyle(ControlStyles.DoubleBuffer, true); // 双缓冲

            //初始化所有窗口
            InitializeAllWindows();
            HideTab();


            //读取配置文件
            ReadInfoXml<SystemParam>(ref sysParam, systemXml);

            int heightDisplay = Screen.PrimaryScreen.Bounds.Height - toolStrip1.Height;

            panel1.Left = Screen.PrimaryScreen.Bounds.Width / 24;
            panel1.Top = heightDisplay / 40;
            panel1.Width = Screen.PrimaryScreen.Bounds.Width * 22 / 24;
            panel1.Height = Screen.PrimaryScreen.Bounds.Height * 13 / 16;

            splitContainer1.SplitterDistance = splitContainer1.Height / 7;
            //splitContainer1.Panel1Collapsed = true;
            splitContainerImage1.SplitterDistance = splitContainerImage1.Width * 8 / 15;
            splitContainerImage1.Panel1.Controls.Add(_FormDisplayBG[0]);

            splitContainer2.SplitterDistance = splitContainer2.Height / 7;
            splitContainer3.SplitterDistance = splitContainerImage1.Width * 3 / 5;
            splitContainer3.Panel1.Controls.Add(_FormDisplayBG[1]);

            splitContainer4.SplitterDistance = splitContainer4.Height / 7;
            splitContainer5.SplitterDistance = splitContainerImage1.Width * 3 / 5;
            splitContainer5.Panel1.Controls.Add(_FormDisplayBG[2]);

            splitContainer6.SplitterDistance = splitContainer6.Height / 7;
            splitContainer7.SplitterDistance = splitContainerImage1.Width * 3 / 5;
            splitContainer7.Panel1.Controls.Add(_FormDisplayBG[3]);

            splitContainer8.SplitterDistance = splitContainer8.Width * 6/ 7;
            splitContainer10.SplitterDistance = splitContainer10.Width * 6 / 7;
            splitContainer11.SplitterDistance = splitContainer11.Width * 6 / 7;
            splitContainer12.SplitterDistance = splitContainer12.Width * 6 / 7;

            if (sysParam.displayCount == 1)
            {

                splitContainer1.Left = BORDER_WIDTH;
                splitContainer1.Top = 0;
                splitContainer1.Width = panel1.Width - BORDER_WIDTH * 2;
                splitContainer1.Height = panel1.Height - BORDER_WIDTH * 2;

                splitContainer2.Visible = false;
                splitContainer4.Visible = false;
                splitContainer6.Visible = false;

            }
            else
            {
                splitContainer1.Left = BORDER_WIDTH;
                splitContainer1.Top = BORDER_WIDTH;
                splitContainer1.Width = panel1.Width / 2 - BORDER_WIDTH * 2;
                splitContainer1.Height = panel1.Height / 2 - BORDER_WIDTH * 2;

                splitContainer2.Visible = true;
                splitContainer4.Visible = true;
                splitContainer6.Visible = true;
            }

            //->四分布局

            splitContainer2.Left = BORDER_WIDTH * 2 + splitContainer1.Width;
            splitContainer2.Top = BORDER_WIDTH;
            splitContainer2.Width = panel1.Width / 2 - BORDER_WIDTH;
            splitContainer2.Height = panel1.Height / 2 - BORDER_WIDTH * 2;

            splitContainer4.Left = BORDER_WIDTH;
            splitContainer4.Top = BORDER_WIDTH * 3 + splitContainer1.Height;
            splitContainer4.Width = panel1.Width / 2 - BORDER_WIDTH * 2;
            splitContainer4.Height = panel1.Height / 2 - BORDER_WIDTH * 2;

            splitContainer6.Left = BORDER_WIDTH * 2 + splitContainer1.Width;
            splitContainer6.Top = BORDER_WIDTH * 3 + splitContainer1.Height;
            splitContainer6.Width = panel1.Width / 2 - BORDER_WIDTH;
            splitContainer6.Height = panel1.Height / 2 - BORDER_WIDTH * 2;

            splitContainer1.Panel2.Controls.Add(setting1);
            splitContainer2.Panel2.Controls.Add(setting2);
            splitContainer4.Panel2.Controls.Add(setting3);
            splitContainer6.Panel2.Controls.Add(setting4);

            setting1.Dock = DockStyle.Fill;
            setting1.BringToFront();
            setting1.Visible = false;

            setting2.Dock = DockStyle.Fill;
            setting2.BringToFront();
            setting2.Visible = false;

            setting3.Dock = DockStyle.Fill;
            setting3.BringToFront();
            setting3.Visible = false;

            setting4.Dock = DockStyle.Fill;
            setting4.BringToFront();
            setting4.Visible = false;

            int widthDisplay = Screen.PrimaryScreen.Bounds.Width;


            skinButton1.Left = widthDisplay * 12 / 20;
            skinButton1.Top = toolStrip1.Top + toolStrip1.Height / 2 - skinButton1.Height / 2;

            skinButton_MainView.Left = widthDisplay * 14 / 20;
            skinButton_MainView.Top = toolStrip1.Top + toolStrip1.Height / 2 - skinButton_MainView.Height / 2;
            skinButton_History.Left = widthDisplay * 16 / 20;
            skinButton_History.Top = toolStrip1.Top + toolStrip1.Height / 2 - skinButton_History.Height / 2;
            skinButton_Exit.Left = widthDisplay * 18 / 20;
            skinButton_Exit.Top = toolStrip1.Top + toolStrip1.Height / 2 - skinButton_Exit.Height / 2;

            //label_Company.Location = new Point(widthDisplay - label_Company.Width, heightDisplay - label_Company.Height/2);

            OnUpdateDisplayPostion();

            _DataControl = new DataControl();
            _DataControl.CreateService();//必须调用
            _DataControl.GetService().EnableAutoReConnect(true);//使能断线重连      
            FormMain.OnDestroy += new FormMain.delegateDestroy(OnMainDestroy);

            RefreshOnlineDevice();

            GetCameraDeviceList();

            Globals.SetMainFrm(this);
        }

        private void GetCameraDeviceList()
        {
            try
            {
                m_stDeviceList.nDeviceNum = 0;
                int nRet = MyCamera.MV_CC_EnumDevices_NET(MyCamera.MV_GIGE_DEVICE, ref m_stDeviceList);
                if (0 != nRet)
                {
                    Globals.Log("枚举可见光相机失败");
                    return;
                }
            }
            catch (Exception ex)
            {
                Globals.Log("枚举可见光相机" + ex.Message);
            }
        }


        private void HideTab()
        {
            tabControlMain.SizeMode = TabSizeMode.Fixed;
            tabControlMain.ItemSize = new Size(0, 1);
        }

        private string intToIp(uint ipAddress)
        {
            string[] ips = IPAddress.Parse(ipAddress.ToString()).ToString().Split('.');

            string temp = ips[0];
            ips[0] = ips[3];
            ips[3] = temp;

            temp = ips[1];
            ips[1] = ips[2];
            ips[2] = temp;

            return String.Join(".", ips);
        }

        private void RefreshOnlineDevice()
        {
            try
            {
                _DataControl.GetService().EnumCameras();
                Thread.Sleep(50);
                UpdateOnlineDevComboLst();
            }
            catch (Exception ex)
            {
                Globals.Log("枚举红外相机" + ex.Message);
            }
        }

        private void UpdateOnlineDevComboLst()
        {
            try
            {
                MagService service = _DataControl.GetService();
                dev_num = service.GetTerminalList(_LstEnumInfo, MAX_ENUMDEVICE);
            }
            catch (Exception ex)
            {
                Console.WriteLine("刷新在线设备列表" + ex.Message);
            }
        }

        void OnMainDestroy()
        {
            _DataControl.DestroyService();//必须调用
        }

        void InitializeAllWindows()
        {
            _FormDisplayLst = new FormDisplay[4];
            _FormDisplayBG = new FormDisplayBG[4];

            ////屏幕尺寸
            //Rectangle scr = System.Windows.Forms.Screen.GetBounds(this);
            //int w = scr.Width;
            //int h = scr.Height;

            //this.Width = w;
            //this.Height = h;

            //uint display_width = (uint)w / 2;
            //uint display_height = (uint)h;

            //if (display_width * 3 >= display_height * 4)//考虑比例 4:3
            //{
            //    uint ret = display_height % 3;
            //    if (ret != 0)
            //    {
            //        display_height -= ret;
            //    }
            //    display_width = display_height * 4 / 3;
            //}
            //else// 3:4
            //{
            //    uint ret = display_width % 4;
            //    if (ret != 0)
            //    {
            //        display_width -= ret;
            //    }
            //    display_height = display_width * 3 / 4;
            //}



            for (uint i = 0; i < 4; i++)
            {
                _FormDisplayBG[i] = new FormDisplayBG();
                _FormDisplayBG[i].TopLevel = false;
                _FormDisplayBG[i].Parent = this;
                _FormDisplayBG[i].FormBorderStyle = FormBorderStyle.None;
                _FormDisplayBG[i].TopLevel = false;
                _FormDisplayBG[i].Hide();
                //红外图像显示窗口
                _FormDisplayLst[i] = new FormDisplay();
                _FormDisplayLst[i].TopLevel = false;
                _FormDisplayLst[i].Parent = _FormDisplayBG[i];
                _FormDisplayLst[i].Dock = DockStyle.Fill;
                _FormDisplayLst[i].FormBorderStyle = FormBorderStyle.None;
                _FormDisplayLst[i].Hide();

                _FormDisplayLst[i].GetDateDisplay().WndIndex = i;

            }

            //1#图像保存线程
            try
            {
                Thread threadSaveThermography_1 = new Thread(new ThreadStart(ThreadSaveImage_1));
                threadSaveThermography_1.Name = "threadSaveThermography_1";
                threadSaveThermography_1.Start();
            }
            catch
            {
                Globals.Log("1#图像保存线程启动失败！");
                //MessageBox.Show("1#图像保存线程启动失败！");
            }

            //报警线程，只需一个线程
            try
            {
                Thread threadAlert = new Thread(new ThreadStart(ThreadAlert));
                threadAlert.Name = "threadAlert";
                threadAlert.Start();
            }
            catch
            {
                Globals.Log("1#报警线程启动失败！");
                //MessageBox.Show("1#报警线程启动失败！");
            }

            //关灯线程，只需一个线程
            try
            {
                Thread threadCloseLamp = new Thread(new ThreadStart(ThreadCloseLamp));
                threadCloseLamp.Name = "threadCloseLamp";
                threadCloseLamp.Start();
            }
            catch
            {
                Globals.Log("1#关灯线程启动失败！");
                //MessageBox.Show("1#报警线程启动失败！");
            }

            //照明灯监听线程，只需一个线程
            try
            {
                Thread threadListenLamp = new Thread(new ThreadStart(ThreadListenLamp));
                threadListenLamp.Name = "threadCloseLamp";
                threadListenLamp.Start();
            }
            catch
            {
                Globals.Log("1#照明灯监听线程启动失败！");
                //MessageBox.Show("1#报警线程启动失败！");
            }

        }


        /// <summary>
        /// 1#保存线程
        /// </summary>
        public void ThreadSaveImage_1()
        {
            DateTime time = DateTime.Now;

            while (true)
            {
                try
                {
                    if (Globals.bSaveAlert_1)
                    {
                        time = DateTime.Now;
                        //Bitmap CameraBitmap = new Bitmap(pictureBoxCamera1.Width, pictureBoxCamera1.Height);
                        //pictureBoxCamera1.DrawToBitmap(CameraBitmap, new Rectangle(0, 0, pictureBoxCamera1.Width, pictureBoxCamera1.Height));
                        Bitmap ThermalBitmap = new Bitmap(_FormDisplayLst[0].Width, _FormDisplayLst[0].Height);

                        _FormDisplayLst[0].DrawToBitmap(ThermalBitmap, new Rectangle(0, 0, _FormDisplayLst[0].Width, _FormDisplayLst[0].Height));

                        string folderNameThermal = Application.StartupPath + "\\SaveReport\\Thermal\\1\\"
                            + time.ToString("yyyy-MM-dd") + "\\" + time.ToString("HHmm");

                        //folderNameThermal.Substring

                        string folderNameCamera = Application.StartupPath + "\\SaveReport\\Camera\\1\\"
                               + time.ToString("yyyy-MM-dd") + "\\" + time.ToString("HHmm");

                        if (!Directory.Exists(folderNameThermal))
                        {
                            Directory.CreateDirectory(folderNameThermal);
                        }

                        if (!Directory.Exists(folderNameCamera))
                        {
                            Directory.CreateDirectory(folderNameCamera);
                        }

                        string fileNameThermal = time.ToString("HHmmssff") + ".bmp";
                        string fileNameCamera = time.ToString("HHmmssff") + ".bmp";
                        folderNameThermal = folderNameThermal + "\\" + fileNameThermal;
                        folderNameCamera = folderNameCamera + "\\" + fileNameCamera;

                        FormDisplay frmDisplay = _FormDisplayLst[DataDisplay.CurrSelectedWndIndex];
                        MagDevice device = frmDisplay.GetDateDisplay().GetDevice();

                        device.Lock();
                        //保存热成像图像
                        try
                        {
                            ThermalBitmap.Save(folderNameThermal);
                            //device.SaveDDT(folderNameThermal);
                        }
                        catch (Exception e1)
                        {
                            Globals.Log("ThreadSaveImage_1-1" + e1.Message);
                        }
                        device.Unlock();
                        //保存可见光图像
                        try
                        {
                            SaveCameraBmp(folderNameCamera);
                            //CameraBitmap.Save(folderNameCamera);
                        }
                        catch (Exception e2)
                        {
                            Globals.Log("ThreadSaveImage_1-2" + e2.Message);
                        }
                        //Globals.bSaveAlert_1 = false;

                        string xmlFile = Application.StartupPath + "\\SaveReport";
                        if (!Directory.Exists(xmlFile))
                        {
                            Directory.CreateDirectory(xmlFile);
                        }

                        xmlFile = xmlFile + "\\" + DateTime.Now.ToString("yyyy-MM-dd") + "_1.xml";

                        WriteInfoXml<AlarmListInfo>(alarmListInfo_1, xmlFile);
                        //Globals.bSaveXML_1 = false;
                        //alarmListInfoList_1.RemoveAt(0);
                        //Globals.bSaveAlert_1 = false;

                    }
                }
                catch (Exception ex)
                {
                    Globals.Log("ThreadSaveImage_1-3" + ex.Message);
                }
                //if (Globals.bSaveXML_1)
                //{
                //    string xmlFile = Application.StartupPath + "\\SaveReport";
                //    if (!Directory.Exists(xmlFile))
                //    {
                //        Directory.CreateDirectory(xmlFile);
                //    }

                //    xmlFile = xmlFile + "\\" + DateTime.Now.ToString("yyyy-MM-dd") + "_1.xml";

                //    WriteInfoXml<AlarmListInfo>(alarmListInfo_1, xmlFile);
                //    Globals.bSaveXML_1 = false;
                //    //alarmListInfoList_1.RemoveAt(0);
                //    Globals.bSaveAlert_1 = false;
                //}

                Thread.Sleep(1000);

            }
        }



        void OnUpdateDisplayPostion()
        {
            //int display_width = splitContainerImage1.Panel1.Width;
            //int display_height = splitContainerImage1.Panel1.Height;
            //if (display_width * 3 >= display_height * 4)//考虑比例 4:3
            //{
            //    int ret = display_height % 3;
            //    if (ret != 0)
            //    {
            //        display_height -= ret;
            //    }
            //    display_width = display_height * 4 / 3;
            //    splitContainerImage1.SplitterDistance = display_width;
            //}
            //else// 3:4
            //{
            //    int ret = display_width % 4;
            //    if (ret != 0)
            //    {
            //        display_width -= ret;
            //    }
            //    display_height = display_width * 3 / 4;
            //}

            _FormDisplayBG[0].Left = 0;
            _FormDisplayBG[0].Top = 0;
            _FormDisplayBG[0].Width = splitContainerImage1.Panel1.Width;
            _FormDisplayBG[0].Height = splitContainerImage1.Panel1.Height;
            _FormDisplayBG[0].Show();

            _FormDisplayBG[1].Left = 0;
            _FormDisplayBG[1].Top = 0;
            _FormDisplayBG[1].Width = splitContainer3.Panel1.Width;
            _FormDisplayBG[1].Height = splitContainer3.Panel1.Height;
            _FormDisplayBG[1].Show();

            _FormDisplayBG[2].Left = 0;
            _FormDisplayBG[2].Top = 0;
            _FormDisplayBG[2].Width = splitContainer5.Panel1.Width;
            _FormDisplayBG[2].Height = splitContainer5.Panel1.Height;
            _FormDisplayBG[2].Show();

            _FormDisplayBG[3].Left = 0;
            _FormDisplayBG[3].Top = 0;
            _FormDisplayBG[3].Width = splitContainer7.Panel1.Width;
            _FormDisplayBG[3].Height = splitContainer7.Panel1.Height;
            _FormDisplayBG[3].Show();

            //display_width = splitContainerImage1.Panel2.Width;
            //display_height = splitContainerImage1.Panel2.Height;
            //if (display_width * 3 >= display_height * 4)//考虑比例 4:3
            //{
            //    int ret = display_height % 3;
            //    if (ret != 0)
            //    {
            //        display_height -= ret;
            //    }
            //    display_width = display_height * 4 / 3;
            //}
            //else// 3:4
            //{
            //    int ret = display_width % 4;
            //    if (ret != 0)
            //    {
            //        display_width -= ret;
            //    }
            //    display_height = display_width * 3 / 4;
            //}
            pictureBoxCamera1.Top = 0;
            pictureBoxCamera1.Left = 0;
            pictureBoxCamera1.Width = splitContainerImage1.Panel2.Width;
            pictureBoxCamera1.Height = splitContainerImage1.Panel2.Height;

            pictureBoxCamera2.Top = 0;
            pictureBoxCamera2.Left = 0;
            pictureBoxCamera2.Width = splitContainer3.Panel2.Width;
            pictureBoxCamera2.Height = splitContainer3.Panel2.Height;

            pictureBoxCamera3.Top = 0;
            pictureBoxCamera3.Left = 0;
            pictureBoxCamera3.Width = splitContainer5.Panel2.Width;
            pictureBoxCamera3.Height = splitContainer5.Panel2.Height;

            pictureBoxCamera4.Top = 0;
            pictureBoxCamera4.Left = 0;
            pictureBoxCamera4.Width = splitContainer7.Panel2.Width;
            pictureBoxCamera4.Height = splitContainer7.Panel2.Height;

        }
        //获取某个显示窗口
        public FormDisplay GetFormDisplay(uint index)
        {
            return _FormDisplayLst[index];
        }

        private void FormMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                if (OnDestroy != null)
                {
                    //主线程运行onDestroy,包括FormDisplay的onDestroy
                    OnDestroy.Invoke();
                }
            }
            catch (Exception ex)
            {
                Globals.Log("FormMain_FormClosed" + ex.Message);
            }
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            try
            {
                //string backgroundImagePath = Application.StartupPath + "\\3363s.jpg";
                ////Console.WriteLine(backgroundImagePath);
                //Image TabBackground = Image.FromFile("3363s.jpg");//设置tab页面背景图片
                //if (TabBackground != null)
                //{
                //    //Console.WriteLine(tabControlMain.TabPages.Count);

                //    tabControlMain.TabPages[0].BackgroundImage = TabBackground;
                //    tabControlMain.TabPages[1].BackgroundImage = TabBackground;
                //    this.BackgroundImage = TabBackground;

                //}
                //else
                //{
                //    Globals.Log("为空");
                //}

                //读取当天报警索引信息
                string xmlFile = Application.StartupPath + "\\SaveReport\\" + DateTime.Now.ToString("yyyy-MM-dd") + "_1.xml";
                if (File.Exists(xmlFile))
                {
                    ReadInfoXml<AlarmListInfo>(ref alarmListInfo_1, xmlFile);
                }

                xmlFile = Application.StartupPath + "\\SaveReport\\" + DateTime.Now.ToString("yyyy-MM-dd") + "_2.xml"; ;

                if (File.Exists(xmlFile))
                {
                    ReadInfoXml<AlarmListInfo>(ref alarmListInfo_2, xmlFile);
                }

                xmlFile = Application.StartupPath + "\\SaveReport\\" + DateTime.Now.ToString("yyyy-MM-dd") + "_3.xml"; ;

                if (File.Exists(xmlFile))
                {
                    ReadInfoXml<AlarmListInfo>(ref alarmListInfo_3, xmlFile);
                }

                xmlFile = Application.StartupPath + "\\SaveReport\\" + DateTime.Now.ToString("yyyy-MM-dd") + "_4.xml"; ;

                if (File.Exists(xmlFile))
                {
                    ReadInfoXml<AlarmListInfo>(ref alarmListInfo_4, xmlFile);
                }

                comboBox1.Text = sysParam.deviceName_1;
                comboBox1.Items.Add(sysParam.deviceName_1);
                if (sysParam.displayCount == 4)
                {
                    comboBox1.Items.Add(sysParam.deviceName_2);
                    comboBox1.Items.Add(sysParam.deviceName_3);
                    comboBox1.Items.Add(sysParam.deviceName_4);
                }
            }
            catch (Exception ex)
            {
                Globals.Log("FormMain_Load" + ex.Message);
            }

        }

        public static bool ReadInfoXml<T>(ref T Info, string fileName)
        {
            try
            {
                FileStream fs = new FileStream(fileName, FileMode.Open);

                try
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(T));
                    Info = (T)serializer.Deserialize(fs);
                    fs.Close();
                    fs.Dispose();

                    return true;
                }
                catch (Exception e1)
                {
                    Globals.Log("ReadInfoXml-1" + e1.Message);
                    fs.Close();
                    fs.Dispose();

                    return false;
                }
            }
            catch (Exception e2)
            {
                Globals.Log("ReadInfoXml-2" + e2.Message);

                return false;
            }
        }

        public static void WriteInfoXml<T>(T Info, string fileName)
        {
            try
            {
                TextWriter myWriter = new StreamWriter(fileName);

                try
                {
                    XmlSerializer mySerializer = new XmlSerializer(typeof(T));
                    mySerializer.Serialize(myWriter, Info);
                    myWriter.Close();
                    myWriter.Dispose();
                }
                catch (Exception e1)
                {
                    Globals.Log("ReadInfoXml-2" + e1.Message);
                    myWriter.Close();
                    myWriter.Dispose();
                }
            }
            catch (Exception e2)
            {
                Globals.Log("ReadInfoXml-2" + e2.Message);
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            setting1.Visible = true;
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {

        }
        //private void NewFrameCome(uint hDevice, int intCamTemp, int intFFCCounter, int intCamState, int intStreamType, IntPtr pUserData)
        //{

        //}

        private void button9_Click(object sender, EventArgs e)
        {
            try
            {
                checkFlag = true;
                //读取当天报警索引信息
                string xmlFile = Application.StartupPath + "\\SaveReport\\" + Convert.ToDateTime(dateTimePicker1.Text).ToString("yyyy-MM-dd") + "_1.xml";
                if (File.Exists(xmlFile))
                {
                    ReadInfoXml<AlarmListInfo>(ref alarmListInfo_Read, xmlFile);
                }


                //DdtMagDevice.Initialize();
                //DdtMagDevice.StopProcessImage();

                //GroupSDK.OUTPUT_PARAM param = new GroupSDK.OUTPUT_PARAM();
                //param.intFPAWidth = 384;
                //param.intFPAHeight = 288;
                //param.intBMPWidth = 384;
                //param.intBMPHeight = 288;
                //param.intColorbarWidth = 20;
                //param.intColorbarHeight = 100;

                //DdtMagDevice.LoadDDT(param, @"F:\MyData\C#\新建文件夹 (2)\CoalTrainMonitoringSystemServer\CoalTrainMonitoringSystemServer\bin\Release\SaveReport\Thermal\1\2020-12-08\1\12100427.ddt", null, IntPtr.Zero);
                //pictureBox_Thermal.Invalidate(false);

                listView_AlertData.Items.Clear();
                pictureBox_Camera.Image = null;
                pictureBox_Thermal.Image = null;

                string folderNameThermal = Application.StartupPath + "\\SaveReport\\Thermal\\";
                string folderNameCamera = Application.StartupPath + "\\SaveReport\\Camera\\";

                int index = 0;
                if (comboBox1.Text == sysParam.deviceName_1)
                {
                    folderNameThermal += "1\\";
                    folderNameCamera += "1\\";
                    index = 1;
                }
                else if (comboBox1.Text == sysParam.deviceName_2)
                {
                    folderNameThermal += "2\\";
                    folderNameCamera += "2\\";
                    index = 2;
                }
                else if (comboBox1.Text == sysParam.deviceName_3)
                {
                    folderNameThermal += "3\\";
                    folderNameCamera += "3\\";
                    index = 3;
                }
                else if (comboBox1.Text == sysParam.deviceName_4)
                {
                    folderNameThermal += "4\\";
                    folderNameCamera += "4\\";
                    index = 4;
                }
                else
                {
                    label5.Text = "通道选择错误！";
                }

                folderNameThermal += Convert.ToDateTime(dateTimePicker1.Text).ToString("yyyy-MM-dd");
                folderNameCamera += Convert.ToDateTime(dateTimePicker1.Text).ToString("yyyy-MM-dd");

                DirectoryInfo drThermal = new DirectoryInfo(folderNameThermal);
                DirectoryInfo drCamera = new DirectoryInfo(folderNameCamera);

                //Console.WriteLine(folderNameCamera);

                if (drThermal.Exists && drCamera.Exists)
                {
                    label5.Text = "";
                    ThermalFiles = drThermal.GetFiles();
                    CameraFiles = drCamera.GetFiles();

                    this.listView_AlertData.View = View.Details;
                    if (listView_AlertData.Columns.Count == 0)
                    {
                        ColumnHeader columnHeader = new ColumnHeader();
                        columnHeader.Text = "序号";
                        columnHeader.Width = listView_AlertData.Width / 5;
                        columnHeader.TextAlign = HorizontalAlignment.Center;
                        listView_AlertData.Columns.Add(columnHeader);

                        ColumnHeader columnHeader1 = new ColumnHeader();
                        columnHeader1.Text = "探测时间";
                        columnHeader1.Width = listView_AlertData.Width * 4/ 5;
                        columnHeader1.TextAlign = HorizontalAlignment.Center;
                        listView_AlertData.Columns.Add(columnHeader1);

                        //ColumnHeader columnHeader2 = new ColumnHeader();
                        //columnHeader2.Text = "报警温度";
                        ////columnHeader2.Width = listView_AlertData.Width * 2 / 5 - 5;
                        //columnHeader2.Width = 0;
                        //columnHeader2.TextAlign = HorizontalAlignment.Center;
                        //listView_AlertData.Columns.Add(columnHeader2);
                    }


                    //AlarmListInfo alarmListInfo = new AlarmListInfo();
                    //switch (index)
                    //{
                    //    case 1:
                    //        alarmListInfo = alarmListInfo_1;
                    //        break;
                    //    case 2:
                    //        alarmListInfo = alarmListInfo_2;
                    //        break;
                    //    case 3:
                    //        alarmListInfo = alarmListInfo_3;
                    //        break;
                    //    case 4:
                    //        alarmListInfo = alarmListInfo_4;
                    //        break;
                    //    default:
                    //        break;
                    //}

                    //if (alarmListInfo.currentIndexID > 0)
                    //{
                    //    label5.Text = DateTime.Now.ToString("yyyy-MM-dd") + " " + comboBox1.Text + " 共计" + alarmListInfo.currentIndexID + "节车温度报警！";
                    //}
                    //else
                    //{
                    //    label5.Text = DateTime.Now.ToString("yyyy-MM-dd") + " " + comboBox1.Text + "无温度报警！";
                    //}
                    //DateTime time = DateTime.Now;

                    string folderFullName = Application.StartupPath + "\\SaveReport\\Thermal\\1\\"
                               + Convert.ToDateTime(dateTimePicker1.Text).ToString("yyyy-MM-dd");

                    DirectoryInfo TheFolder = new DirectoryInfo(folderFullName);

                    int i = 0;
                    folderName.Clear();
                    foreach (DirectoryInfo NextFolder in TheFolder.GetDirectories())
                    {
                        ListViewItem item = new ListViewItem();
                        item.SubItems[0].Text = (i + 1).ToString();
                        String fileName = NextFolder.Name;
                        folderName.Add(fileName);
                        fileName = fileName.Substring(0, 2) + ":" + fileName.Substring(2, 2);
                        item.SubItems.Add(fileName);
                        listView_AlertData.Items.Add(item);
                        i++;
                    }

                    //for (int i = 0; i < alarmListInfo_Read.currentIndexID; i++)
                    //{
                    //    ListViewItem item = new ListViewItem();
                    //    item.SubItems[0].Text = (i + 1).ToString();
                    //    item.SubItems.Add(alarmListInfo_Read.trainIndexList[i].detectTime);
                    //    item.SubItems.Add(alarmListInfo_Read.trainIndexList[i].alarmTemperatrue);
                    //    listView_AlertData.Items.Add(item);
                    //}

                }
                else
                {
                    label5.Text = "没有报警数据";
                }
            }
            catch (Exception ex)
            {
                Globals.Log("button9_Click" + ex.Message);
            }
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            try
            {
                DateTime time = DateTime.Now;
                time = DateTime.Now;

                string folderNameThermal = Application.StartupPath;

                if (!Directory.Exists(folderNameThermal))
                {
                    Directory.CreateDirectory(folderNameThermal);
                }

                string fileNameThermal = time.ToString("HHmmssff") + ".bmp";

                folderNameThermal = folderNameThermal + "\\" + fileNameThermal;

                FormDisplay frmDisplay = _FormDisplayLst[DataDisplay.CurrSelectedWndIndex];

                if (frmDisplay == null)
                {
                    Globals.Log("frmDisplay == null");
                    return;
                }

                MagDevice device = frmDisplay.GetDateDisplay().GetDevice();

                device.Lock();
                if (device.SaveBMP(0, folderNameThermal))
                {
                    Globals.Log("保存成功");
                }
                else
                {
                    Globals.Log("保存失败");
                }

                device.Unlock();

                frmDisplay.Invalidate(false);
            }
            catch (Exception ex)
            {
                Globals.Log("toolStripButton3_Click");
            }
        }

        public void ThreadListenLamp()
        {
            //使用指定的地址族，套接字类型和协议实例化新的套接字
            Socket socketServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //将字符串地址转化为网际协议地址
            IPAddress ipAddress = IPAddress.Parse(serverIP);//这里为服务器IP
            //使用指定地址和端口号实例化新的网络端点
            IPEndPoint endPoint = new IPEndPoint(ipAddress, serverPort);//ipAddress为服务器IP ， 6000为服务器端口
            try
            {
                //socket与本地终结点相关联，绑定IP与端口
                socketServer.Bind(endPoint);
                //设置最大侦听长度，并启动侦听
                socketServer.Listen(200);
            }
            catch
            {
                Globals.Log("请检查IP是否正确，或端口是否被占用？");
                socketServer.Close();
                return;
            }

            socketClient = socketServer.Accept();
            string[] strs = socketClient.RemoteEndPoint.ToString().Split(':');
            Globals.Log("客户端IP：" + strs[0] + " 端口：" + strs[1]);
        }

        public void ThreadCloseLamp()
        {
            Thread.Sleep(10);
            while (true)
            {
                try
                {
                    if (Globals.closeLampFlag)
                    {
                        Thread.Sleep(1000);
                        if (Globals.maxTemp < sysParam.alertTemp_1 * 0.9)
                        {
                            if (FormMain.socketClient != null)
                            {
                                //将字符串指令转换为byte数组
                                byte[] buf = System.Text.Encoding.Default.GetBytes("AT+STACH2=0\r\n");
                                //发送AT指令
                                FormMain.socketClient.Send(buf);
                            }
                            Globals.closeLampFlag = false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Globals.Log("ThreadCloseLamp" + ex.Message);
                }
            }
        }
        /// <summary>
        /// 声音报警线程，只需要一个线程
        /// </summary>
        public void ThreadAlert()
        {
            while (true)
            {
                Thread.Sleep(10);

                if (Globals.bSoundAlert)
                {
                    try
                    {
                        SoundPlayer player = new SoundPlayer();
                        player.SoundLocation = Application.StartupPath + "\\Alert.wav";
                        player.Load();
                        player.Play();
                        Thread.Sleep(5000);

                        Globals.bSoundAlert = false;
                    }
                    catch (Exception e)
                    {
                        Globals.Log("ThreadAlert" + e.Message);
                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ////发现红外相机
            if (dev_num > 0)
            {
                try
                {
                    DataDisplay.CurrSelectedWndIndex = 0;
                    _FormDisplayLst[DataDisplay.CurrSelectedWndIndex].Show();
                    FormDisplay display = _DataControl.GetCurrDisplayForm();
                    if (display != null)
                    {
                        MagDevice device = display.GetDateDisplay().GetDevice();
                        device.StopProcessImage();

                        if (device.LinkCamera(_LstEnumInfo[0].intCamIp, 2000))
                        //if (device.LinkCamera("192.168.1.101", 2000))
                        {
                            _FormDisplayLst[DataDisplay.CurrSelectedWndIndex].Show();
                            Globals.Log("相机连接成功");
                            DataDisplay.CurrSelectedWndIndex = display.GetDateDisplay().WndIndex;//更新选中框                  
                            display.GetDateDisplay().Play();
                        }
                        else
                        {
                            Globals.Log("相机连接失败");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Globals.Log("启动红外相机" + ex.Message);
                }
            }
            else
            {

                Globals.Log("没有发现相机");
            }

            if (m_stDeviceList.nDeviceNum > 0)//发现可见光相机
            {
                OpenCamera();
                StartGrab();
            }
        }

        public void OpenCamera()
        {
            try
            {
                MyCamera.MV_CC_DEVICE_INFO device;

                MyCamera.MV_CC_DEVICE_INFO device0 = (MyCamera.MV_CC_DEVICE_INFO)Marshal.PtrToStructure(m_stDeviceList.pDeviceInfo[0], typeof(MyCamera.MV_CC_DEVICE_INFO));
                MyCamera.MV_GIGE_DEVICE_INFO gigeInfo0 = (MyCamera.MV_GIGE_DEVICE_INFO)MyCamera.ByteToStruct(device0.SpecialInfo.stGigEInfo, typeof(MyCamera.MV_GIGE_DEVICE_INFO));

                if (gigeInfo0.chSerialNumber == "E83748532")
                {
                    device = device0;
                }
                else
                {
                    device = (MyCamera.MV_CC_DEVICE_INFO)Marshal.PtrToStructure(m_stDeviceList.pDeviceInfo[1], typeof(MyCamera.MV_CC_DEVICE_INFO));
                    MyCamera.MV_GIGE_DEVICE_INFO gigeInfo = (MyCamera.MV_GIGE_DEVICE_INFO)MyCamera.ByteToStruct(device.SpecialInfo.stGigEInfo, typeof(MyCamera.MV_GIGE_DEVICE_INFO));
                }

                // ch:打开设备 | en:Open device
                if (null == m_MyCamera)
                {
                    m_MyCamera = new MyCamera();
                    if (null == m_MyCamera)
                    {
                        Globals.Log("new MyCamera()失败");
                        return;
                    }
                }
                int nRet = m_MyCamera.MV_CC_CreateDevice_NET(ref device);
                if (MyCamera.MV_OK != nRet)
                {
                    Globals.Log("新建相机设备失败");
                    return;
                }
                nRet = m_MyCamera.MV_CC_OpenDevice_NET();
                if (MyCamera.MV_OK != nRet)
                {
                    m_MyCamera.MV_CC_DestroyDevice_NET();
                    Globals.Log("Device open fail!{0:8}" + String.Format("{0:X}", nRet));
                    return;
                }
                else
                {
                    Globals.Log("打开可见光相机成功");
                }

                // ch:探测网络最佳包大小(只对GigE相机有效) | en:Detection network optimal package size(It only works for the GigE camera)
                if (device.nTLayerType == MyCamera.MV_GIGE_DEVICE)
                {
                    int nPacketSize = m_MyCamera.MV_CC_GetOptimalPacketSize_NET();
                    if (nPacketSize > 0)
                    {
                        nRet = m_MyCamera.MV_CC_SetIntValue_NET("GevSCPSPacketSize", (uint)nPacketSize);
                        if (nRet != MyCamera.MV_OK)
                        {
                            Globals.Log("Set Packet Size failed!" + String.Format("{0:X}", nRet));
                        }
                    }
                    else
                    {
                        Globals.Log("Get Packet Size failed!" + String.Format("{0:X}", nRet));
                    }
                }

                // ch:设置采集连续模式 | en:Set Continues Aquisition Mode
                m_MyCamera.MV_CC_SetEnumValue_NET("AcquisitionMode", (uint)MyCamera.MV_CAM_ACQUISITION_MODE.MV_ACQ_MODE_CONTINUOUS);
                m_MyCamera.MV_CC_SetEnumValue_NET("TriggerMode", (uint)MyCamera.MV_CAM_TRIGGER_MODE.MV_TRIGGER_MODE_OFF);

                //MyCamera.MV_CC_DEVICE_INFO stDevInfo = new MyCamera.MV_CC_DEVICE_INFO();
                //stDevInfo.nTLayerType = MyCamera.MV_GIGE_DEVICE;
                //MyCamera.MV_GIGE_DEVICE_INFO stGigEDev = new MyCamera.MV_GIGE_DEVICE_INFO();
                //int nRet = MyCamera.MV_OK;
                //MyCamera device = new MyCamera();
                //string strCurrentIp = "192.168.1.111";
                //string strNetExport = "192.168.1.120";

                //var parts = strCurrentIp.Split('.');
                //try
                //{
                //    int nIp1 = Convert.ToInt32(parts[0]);
                //    int nIp2 = Convert.ToInt32(parts[1]);
                //    int nIp3 = Convert.ToInt32(parts[2]);
                //    int nIp4 = Convert.ToInt32(parts[3]);
                //    stGigEDev.nCurrentIp = (uint)((nIp1 << 24) | (nIp2 << 16) | (nIp3 << 8) | nIp4);

                //    parts = strNetExport.Split('.');
                //    nIp1 = Convert.ToInt32(parts[0]);
                //    nIp2 = Convert.ToInt32(parts[1]);
                //    nIp3 = Convert.ToInt32(parts[2]);
                //    nIp4 = Convert.ToInt32(parts[3]);
                //    stGigEDev.nNetExport = (uint)((nIp1 << 24) | (nIp2 << 16) | (nIp3 << 8) | nIp4);
                //}
                //catch
                //{
                //    Console.Write("Invalid Input!\n");
                //}

                //// stGigEDev结构体转为stDevInfo.SpecialInfo.stGigEInfo(Byte[])
                //IntPtr stGigeInfoPtr = Marshal.AllocHGlobal(Marshal.SizeOf(stGigEDev));
                //Marshal.StructureToPtr(stGigEDev, stGigeInfoPtr, false);
                //stDevInfo.SpecialInfo.stGigEInfo = new Byte[Marshal.SizeOf(stDevInfo.SpecialInfo)];
                //Marshal.Copy(stGigeInfoPtr, stDevInfo.SpecialInfo.stGigEInfo, 0, Marshal.SizeOf(stDevInfo.SpecialInfo));
                //Marshal.Release(stGigeInfoPtr);

                //// ch:创建设备 | en: Create device
                //nRet = device.MV_CC_CreateDevice_NET(ref stDevInfo);
                //if (MyCamera.MV_OK != nRet)
                //{
                //    Console.WriteLine("Create device failed:{0:x8}", nRet);
                //    Console.WriteLine("创建设备失败");
                //}
                //else
                //{
                //    Console.WriteLine("创建设备成功");
                //}

                //// ch:打开设备 | en:Open device
                //nRet = device.MV_CC_OpenDevice_NET();
                //if (MyCamera.MV_OK != nRet)
                //{
                //    Console.WriteLine("Open device failed:{0:x8}", nRet);

                //}
                //else
                //{
                //    Console.WriteLine("打开设备失败");
                //}
            }
            catch (Exception ex)
            {
                Globals.Log("打开可见光相机失败" + ex.Message);
            }
        }

        public void StartGrab()
        {
            try
            {
                // ch:标志位置位true | en:Set position bit true
                m_bGrabbing = true;

                m_hReceiveThread = new Thread(ReceiveThreadProcess);
                m_hReceiveThread.Start();

                m_stFrameInfo.nFrameLen = 0;//取流之前先清除帧长度
                m_stFrameInfo.enPixelType = MyCamera.MvGvspPixelType.PixelType_Gvsp_Undefined;
                // ch:开始采集 | en:Start Grabbing
                int nRet = m_MyCamera.MV_CC_StartGrabbing_NET();
                if (MyCamera.MV_OK != nRet)
                {
                    m_bGrabbing = false;
                    m_hReceiveThread.Join();
                    Globals.Log("Start Grabbing Fail!" + String.Format("{0:X}", nRet));
                }
                else
                {
                    Globals.Log("开始采集成功");
                }
            }
            catch (Exception ex)
            {
                Globals.Log("可见光开始采集失败" + ex.Message);
            }
        }

        public void ReceiveThreadProcess()
        {
            try
            {
                MyCamera.MVCC_INTVALUE stParam = new MyCamera.MVCC_INTVALUE();
                int nRet = m_MyCamera.MV_CC_GetIntValue_NET("PayloadSize", ref stParam);
                if (MyCamera.MV_OK != nRet)
                {
                    Globals.Log("Get PayloadSize failed" + String.Format("{0:X}", nRet));
                    return;
                }

                UInt32 nPayloadSize = stParam.nCurValue;
                if (nPayloadSize > m_nBufSizeForDriver)
                {
                    if (m_BufForDriver != IntPtr.Zero)
                    {
                        Marshal.Release(m_BufForDriver);
                    }
                    m_nBufSizeForDriver = nPayloadSize;
                    m_BufForDriver = Marshal.AllocHGlobal((Int32)m_nBufSizeForDriver);
                }

                if (m_BufForDriver == IntPtr.Zero)
                {
                    Globals.Log("m_BufForDriver == IntPtr.Zero");
                    return;
                }

                MyCamera.MV_FRAME_OUT_INFO_EX stFrameInfo = new MyCamera.MV_FRAME_OUT_INFO_EX();
                MyCamera.MV_DISPLAY_FRAME_INFO stDisplayInfo = new MyCamera.MV_DISPLAY_FRAME_INFO();

                while (m_bGrabbing)
                {
                    lock (BufForDriverLock)
                    {
                        nRet = m_MyCamera.MV_CC_GetOneFrameTimeout_NET(m_BufForDriver, nPayloadSize, ref stFrameInfo, 1000);
                        if (nRet == MyCamera.MV_OK)
                        {
                            m_stFrameInfo = stFrameInfo;
                        }
                    }

                    if (nRet == MyCamera.MV_OK)
                    {
                        if (RemoveCustomPixelFormats(stFrameInfo.enPixelType))
                        {
                            continue;
                        }
                        //Console.WriteLine(pictureBoxCamera1.Handle);
                        stDisplayInfo.hWnd = pictureBoxCamera1.Handle;
                        stDisplayInfo.pData = m_BufForDriver;
                        stDisplayInfo.nDataLen = stFrameInfo.nFrameLen;
                        stDisplayInfo.nWidth = stFrameInfo.nWidth;
                        stDisplayInfo.nHeight = stFrameInfo.nHeight;
                        stDisplayInfo.enPixelType = stFrameInfo.enPixelType;
                        m_MyCamera.MV_CC_DisplayOneFrame_NET(ref stDisplayInfo);
                    }
                    else
                    {
                        Globals.Log("nRet != MyCamera.MV_OK");
                        //if (bnTriggerMode.Checked)
                        //{
                        //    Thread.Sleep(5);
                        //}
                    }
                }
            }
            catch (Exception ex)
            {
                Globals.Log("ReceiveThreadProcess" + ex.Message);
            }
        }


        // ch:去除自定义的像素格式 | en:Remove custom pixel formats
        private bool RemoveCustomPixelFormats(MyCamera.MvGvspPixelType enPixelFormat)
        {
            Int32 nResult = ((int)enPixelFormat) & (unchecked((Int32)0x80000000));
            if (0x80000000 == nResult)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void SaveCameraBmp(string cameraBmpFileName)
        {
            try
            {
                if (false == m_bGrabbing)
                {
                    Globals.Log("没有开始采集可见光图像");
                    return;
                }

                if (RemoveCustomPixelFormats(m_stFrameInfo.enPixelType))
                {
                    Globals.Log("Not Support!");
                    return;
                }

                IntPtr pTemp = IntPtr.Zero;
                MyCamera.MvGvspPixelType enDstPixelType = MyCamera.MvGvspPixelType.PixelType_Gvsp_Undefined;
                if (m_stFrameInfo.enPixelType == MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono8 || m_stFrameInfo.enPixelType == MyCamera.MvGvspPixelType.PixelType_Gvsp_BGR8_Packed)
                {
                    pTemp = m_BufForDriver;
                    enDstPixelType = m_stFrameInfo.enPixelType;
                }
                else
                {
                    UInt32 nSaveImageNeedSize = 0;
                    MyCamera.MV_PIXEL_CONVERT_PARAM stConverPixelParam = new MyCamera.MV_PIXEL_CONVERT_PARAM();

                    lock (BufForDriverLock)
                    {
                        if (m_stFrameInfo.nFrameLen == 0)
                        {
                            Globals.Log("保存可见光图像失败");
                            return;
                        }

                        if (IsMonoData(m_stFrameInfo.enPixelType))
                        {
                            enDstPixelType = MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono8;
                            nSaveImageNeedSize = (uint)m_stFrameInfo.nWidth * m_stFrameInfo.nHeight;
                        }
                        else if (IsColorData(m_stFrameInfo.enPixelType))
                        {
                            enDstPixelType = MyCamera.MvGvspPixelType.PixelType_Gvsp_BGR8_Packed;
                            nSaveImageNeedSize = (uint)m_stFrameInfo.nWidth * m_stFrameInfo.nHeight * 3;
                        }
                        else
                        {
                            Globals.Log("No such pixel type!");
                            return;
                        }

                        if (m_nBufSizeForSaveImage < nSaveImageNeedSize)
                        {
                            if (m_BufForSaveImage != IntPtr.Zero)
                            {
                                Marshal.Release(m_BufForSaveImage);
                            }
                            m_nBufSizeForSaveImage = nSaveImageNeedSize;
                            m_BufForSaveImage = Marshal.AllocHGlobal((Int32)m_nBufSizeForSaveImage);
                        }

                        stConverPixelParam.nWidth = m_stFrameInfo.nWidth;
                        stConverPixelParam.nHeight = m_stFrameInfo.nHeight;
                        stConverPixelParam.pSrcData = m_BufForDriver;
                        stConverPixelParam.nSrcDataLen = m_stFrameInfo.nFrameLen;
                        stConverPixelParam.enSrcPixelType = m_stFrameInfo.enPixelType;
                        stConverPixelParam.enDstPixelType = enDstPixelType;
                        stConverPixelParam.pDstBuffer = m_BufForSaveImage;
                        stConverPixelParam.nDstBufferSize = m_nBufSizeForSaveImage;
                        int nRet = m_MyCamera.MV_CC_ConvertPixelType_NET(ref stConverPixelParam);
                        if (MyCamera.MV_OK != nRet)
                        {
                            Globals.Log("Convert Pixel Type Fail!");
                            return;
                        }
                        pTemp = m_BufForSaveImage;
                    }
                }

                lock (BufForDriverLock)
                {
                    if (enDstPixelType == MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono8)
                    {
                        //************************Mono8 转 Bitmap*******************************
                        Bitmap bmp = new Bitmap(m_stFrameInfo.nWidth, m_stFrameInfo.nHeight, m_stFrameInfo.nWidth * 1, PixelFormat.Format8bppIndexed, pTemp);

                        ColorPalette cp = bmp.Palette;
                        // init palette
                        for (int i = 0; i < 256; i++)
                        {
                            cp.Entries[i] = Color.FromArgb(i, i, i);
                        }
                        // set palette back
                        bmp.Palette = cp;
                        bmp.Save(cameraBmpFileName, ImageFormat.Bmp);
                    }
                    else
                    {
                        //*********************BGR8 转 Bitmap**************************
                        try
                        {
                            Bitmap bmp = new Bitmap(m_stFrameInfo.nWidth, m_stFrameInfo.nHeight, m_stFrameInfo.nWidth * 3, PixelFormat.Format24bppRgb, pTemp);
                            bmp.Save(cameraBmpFileName, ImageFormat.Bmp);
                        }
                        catch
                        {
                            Globals.Log("Write File Fail!");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Globals.Log("SaveCameraBmp" + ex.Message);
            }
        }


        private Boolean IsMonoData(MyCamera.MvGvspPixelType enGvspPixelType)
        {
            switch (enGvspPixelType)
            {
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono8:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono10:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono10_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono12:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono12_Packed:
                    return true;

                default:
                    return false;
            }
        }


        /************************************************************************
         *  @fn     IsColorData()
         *  @brief  判断是否是彩色数据
         *  @param  enGvspPixelType         [IN]           像素格式
         *  @return 成功，返回0；错误，返回-1 
         ************************************************************************/
        private Boolean IsColorData(MyCamera.MvGvspPixelType enGvspPixelType)
        {
            switch (enGvspPixelType)
            {
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGR8:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerRG8:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGB8:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerBG8:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGR10:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerRG10:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGB10:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerBG10:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGR12:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerRG12:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGB12:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerBG12:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGR10_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerRG10_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGB10_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerBG10_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGR12_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerRG12_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGB12_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerBG12_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_RGB8_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_YUV422_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_YUV422_YUYV_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_YCBCR411_8_CBYYCRYY:
                    return true;

                default:
                    return false;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (listView1.SelectedItems.Count > 0)
                {
                    //pictureBox_Thermal.ImageLocation = ThermalFiles[listView1.SelectedItems[0].Index].FullName;
                    pictureBox_Camera.ImageLocation = imagePathList[listView1.SelectedItems[0].Index];
                    pictureBox_Thermal.ImageLocation = thermalImagePathList[listView1.SelectedItems[0].Index];
                }
            }
            catch (Exception ex)
            {
                Globals.Log(ex.Message);
            }
        }

        private void pictureBox_Thermal_Paint(object sender, PaintEventArgs e)
        {
            //if (checkFlag)
            //{
            //    Graphics graphic = e.Graphics;
            //    DdtMagDevice.Lock();
            //    DrawDdtImages(graphic, this.Width, this.Height);
            //    GroupSDK.CAMERA_INFO cameraInfo = DdtMagDevice.GetCamInfo();
            //    DrawDdtMaxTemp(graphic, cameraInfo, this.Width, this.Height);
            //    DdtMagDevice.Unlock();
            //}

        }

        bool DrawDdtImages(Graphics graphic, int w, int h)
        {
            IntPtr pIrData = IntPtr.Zero;
            IntPtr pIrInfo = IntPtr.Zero;

            if (!DdtMagDevice.GetOutputBMPdata(ref pIrData, ref pIrInfo))
            {
                return false;
            }


            GroupSDK.CAMERA_INFO info = DdtMagDevice.GetCamInfo();

            IntPtr hDC = graphic.GetHdc();

            WINAPI.SetStretchBltMode(hDC, WINAPI.StretchMode.STRETCH_HALFTONE);
            WINAPI.StretchDIBits(hDC, 0, 0, w, h, 0, 0, info.intVideoWidth,
                    info.intVideoHeight, pIrData, pIrInfo, (uint)WINAPI.PaletteMode.DIB_RGB_COLORS,
                    (uint)WINAPI.ExecuteOption.SRCCOPY);

            graphic.ReleaseHdc();

            return true;
        }

        /// <summary>
        /// 调试计算温度
        /// </summary>
        /// <param name="temp"></param>
        /// <param name="correct"></param>
        /// <returns></returns>
        public static float correctTemp(float temp, float correct)
        {
            temp = temp + correct;

            return temp;
        }

        void DrawDdtMaxTemp(Graphics graphic, GroupSDK.CAMERA_INFO cameraInfo, int w, int h)
        {
            int[] info = new int[5];

            DdtMagDevice.GetRectTemperatureInfo(0, 0, 383, 287, info);
            //GroupSDK.CAMERA_INFO cameraInfo = DdtMagDevice.GetCamInfo();
            //Console.WriteLine(cameraInfo.intFPAWidth);
            //Console.WriteLine(cameraInfo.intFPAHeight);
            // 获取区域最高温度 0-最低温，1-最高温，2-平均温度，3-最低温度位置，4-最高温度位置
            int rectMaxTemp = info[1];

            //get the fpa coordinate
            int yFPA = info[4] / cameraInfo.intFPAWidth;//出现过试图除以零的情况
            int xFPA = info[4] - yFPA * cameraInfo.intFPAWidth;

            //convert to the screen coordinate
            int x = xFPA * pictureBox_Thermal.Width / cameraInfo.intFPAWidth;
            int y = pictureBox_Thermal.Height - yFPA * pictureBox_Thermal.Height / cameraInfo.intFPAHeight;


            float tmp;

            tmp = rectMaxTemp * 0.001f;
            tmp = correctTemp(tmp, FormMain.sysParam.adjustParam_1);
            Console.WriteLine(tmp);

            String s = tmp.ToString("F1");

            int pad = 1;
            int lineWidth = 8;
            int cx = (int)graphic.MeasureString(s, Font).Width;
            int cy = (int)graphic.MeasureString(s, Font).Height;

            /* draw cross for max temp point */
            Console.WriteLine(x - lineWidth);
            Console.WriteLine(y);

            Point pt1 = new Point(x - lineWidth, y);
            Point pt2 = new Point(x + lineWidth, y);
            graphic.DrawLine(penLightGreen, pt1, pt2);
            Point pt3 = new Point(x, y - lineWidth);
            Point pt4 = new Point(x, y + lineWidth);
            graphic.DrawLine(penLightGreen, pt3, pt4);

            /* draw text */
            x += pad;
            y += cy + pad;

            if (x > Width - cx)
            {
                x -= pad * 2 + cx;
            }
            if (y > Height)
            {
                y -= pad * 2 + cy * 2;
            }

            Point outPoint = new Point(x, y);
            graphic.DrawString(s, font1, Brushes.LightGreen, outPoint);
        }

        private void toolStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void listView_AlertData_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                listView1.Items.Clear();
                imagePathList.Clear();
                imageList1.Images.Clear();
                Thread.Sleep(100);

                ListViewItem item = listView_AlertData.GetItemAt(e.X, e.Y);

                string folderNameThermal = Application.StartupPath + "\\SaveReport\\Thermal\\";
                string folderNameCamera = Application.StartupPath + "\\SaveReport\\Camera\\";

                int index = 0;
                if (comboBox1.Text == sysParam.deviceName_1)
                {
                    folderNameThermal += "1\\";
                    folderNameCamera += "1\\";
                    index = 1;
                }
                //else if (comboBox1.Text == sysParam.deviceName_2)
                //{
                //    folderNameThermal += "2\\";
                //    folderNameCamera += "2\\";
                //    index = 2;
                //}
                //else if (comboBox1.Text == sysParam.deviceName_3)
                //{
                //    folderNameThermal += "3\\";
                //    folderNameCamera += "3\\";
                //    index = 3;
                //}
                //else if (comboBox1.Text == sysParam.deviceName_4)
                //{
                //    folderNameThermal += "4\\";
                //    folderNameCamera += "4\\";
                //    index = 4;
                //}
                else
                {
                    label5.Text = "通道选择错误！";
                }

                folderNameThermal += Convert.ToDateTime(dateTimePicker1.Text).ToString("yyyy-MM-dd") + "\\" + folderName[item.Index].ToString();
                folderNameCamera += Convert.ToDateTime(dateTimePicker1.Text).ToString("yyyy-MM-dd") + "\\" + folderName[item.Index].ToString();

                //folderNameCamera += "\\" + (item.Index + 1).ToString();
                //folderNameThermal += "\\" + (item.Index + 1).ToString();
                //Console.WriteLine("选中可见光文件夹" + folderNameCamera);
                //Console.WriteLine(folderNameCamera);
                DirectoryInfo dir = new DirectoryInfo(folderNameCamera);
                FileInfo[] fileInfo = dir.GetFiles("*.bmp");
                //this.imageList1.ColorDepth = ColorDepth.Depth32Bit;

                for (int i = 0; i < fileInfo.Length; i++)
                {
                    //获取文件完整目录
                    string picDirPath = fileInfo[i].FullName;

                    //记录图片源路径 双击显示图片时使用
                    imagePathList.Add(picDirPath);
                    //图片加载到ImageList控件和imageList图片列表
                    this.imageList1.Images.Add(Image.FromFile(picDirPath));
                }

                DirectoryInfo dirThermal = new DirectoryInfo(folderNameThermal);
                FileInfo[] fileInfoThermal = dirThermal.GetFiles("*.bmp");

                thermalImagePathList.Clear();
                for (int i = 0; i < fileInfoThermal.Length; i++)
                {
                    //获取文件完整目录
                    string picDirPath = fileInfoThermal[i].FullName;
                    //记录图片源路径 双击显示图片时使用
                    thermalImagePathList.Add(picDirPath);
                }


                //显示文件列表
                this.listView1.Items.Clear();
                this.listView1.LargeImageList = this.imageList1;
                this.listView1.View = View.LargeIcon;        //大图标显示
                //imageList1.ImageSize = new Size(40, 40);   //不能设置ImageList的图像大小 属性处更改

                //开始绑定
                this.listView1.BeginUpdate();
                //增加图片至ListView控件中
                for (int i = 0; i < imageList1.Images.Count; i++)
                {
                    ListViewItem lvi = new ListViewItem();
                    lvi.ImageIndex = i;
                    string fileNameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(imagePathList[i]);
                    lvi.Text = fileNameWithoutExtension;
                    this.listView1.Items.Add(lvi);
                }
                this.listView1.EndUpdate();

            }
            catch (Exception ex)
            {
                Globals.Log("listView_AlertData_MouseClick" + ex.Message);
            }

        }

        private void listView_AlertData_SelectedIndexChanged(object sender, EventArgs e)
        {
            //if (listView_AlertData.SelectedItems.Count > 0)
            //{
            //    Console.WriteLine("1111");

            //}
        }

        private void toolStripButton3_Click_1(object sender, EventArgs e)
        {
            string folderNameThermal = Application.StartupPath + "\\SaveReport\\" + "a.BMP";
            SaveCameraBmp(folderNameThermal);
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                //Globals.Log("正在关闭窗口");
                // ch:取流标志位清零 | en:Reset flow flag bit
                if (m_bGrabbing == true)
                {
                    m_bGrabbing = false;
                    m_hReceiveThread.Join();
                }

                if (m_BufForDriver != IntPtr.Zero)
                {
                    Marshal.Release(m_BufForDriver);
                }
                if (m_BufForSaveImage != IntPtr.Zero)
                {
                    Marshal.Release(m_BufForSaveImage);
                }

                // ch:关闭设备 | en:Close Device
                m_MyCamera.MV_CC_CloseDevice_NET();
                m_MyCamera.MV_CC_DestroyDevice_NET();
            }
            catch
            {
                Globals.Log("FormMain_FormClosing");
            }
        }


        private void skinButton_Exit_Click_1(object sender, EventArgs e)
        {
            try
            {
                if (FormMain.socketClient != null)
                {
                    //将字符串指令转换为byte数组
                    byte[] buf = System.Text.Encoding.Default.GetBytes("AT+STACH2=0\r\n");
                    //发送AT指令
                    FormMain.socketClient.Send(buf);
                }

                //Globals.Log("正在关闭窗口");
                // ch:取流标志位清零 | en:Reset flow flag bit
                if (m_bGrabbing == true)
                {
                    m_bGrabbing = false;
                    m_hReceiveThread.Join();
                }

                if (m_BufForDriver != IntPtr.Zero)
                {
                    Marshal.Release(m_BufForDriver);
                }
                if (m_BufForSaveImage != IntPtr.Zero)
                {
                    Marshal.Release(m_BufForSaveImage);
                }

                // ch:关闭设备 | en:Close Device
                m_MyCamera.MV_CC_CloseDevice_NET();
                m_MyCamera.MV_CC_DestroyDevice_NET();


                if (OnDestroy != null)
                {
                    //主线程运行onDestroy,包括FormDisplay的onDestroy
                    OnDestroy.Invoke();
                }

                try
                {
                    Environment.Exit(0);
                }
                catch { }
            }
            catch (Exception ex)
            {
                Globals.Log("skinButton_Exit_Click_1" + ex.Message);
            }

        }

        private void skinButton_MainView_Click(object sender, EventArgs e)
        {
            tabControlMain.SelectedIndex = 0;
        }

        private void skinButton_History_Click(object sender, EventArgs e)
        {
            tabControlMain.SelectedIndex = 1;
        }

        private void label1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            try
            {
                txtInput_1 = new TextBox();
                txtInput_1.AutoSize = false;
                txtInput_1.Parent = label1;
                txtInput_1.Multiline = true;
                txtInput_1.ImeMode = ImeMode.On;

                txtInput_1.Width = splitContainer8.Panel1.Width;
                txtInput_1.Height = splitContainer8.Panel1.Height;
                txtInput_1.Text = "\r\n" + label1.Text;
                txtInput_1.TextAlign = HorizontalAlignment.Center;
                txtInput_1.Tag = label1;
                txtInput_1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(txtInput_1_KeyPress);
                txtInput_1.Focus();
            }
            catch (Exception e1)
            {
                Globals.Log("label1_MouseDoubleClick" + e1.Message);
            }
        }

        private void txtInput_1_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if ((int)e.KeyChar == 13)
                {
                    if (txtInput_1 != null)
                    {
                        label1.Text = txtInput_1.Text.Replace("\r\n", "");
                        sysParam.deviceName_1 = label1.Text;
                        WriteInfoXml(sysParam, systemXml);
                        txtInput_1.Dispose();
                    }
                }
            }
            catch (Exception e1)
            {
                Globals.Log("txtInput_1_KeyPress" + e1.Message);
            }
        }

        private void skinButton1_Click(object sender, EventArgs e)
        {
            ////发现红外相机
            if (dev_num > 0)
            {
                DataDisplay.CurrSelectedWndIndex = 0;
                _FormDisplayLst[DataDisplay.CurrSelectedWndIndex].Show();
                FormDisplay display = _DataControl.GetCurrDisplayForm();
                if (display != null)
                {
                    MagDevice device = display.GetDateDisplay().GetDevice();
                    device.StopProcessImage();

                    if (device.LinkCamera(_LstEnumInfo[0].intCamIp, 2000))
                    //if (device.LinkCamera("192.168.1.101", 2000))
                    {
                        _FormDisplayLst[DataDisplay.CurrSelectedWndIndex].Show();
                        Globals.Log("相机连接成功");
                        DataDisplay.CurrSelectedWndIndex = display.GetDateDisplay().WndIndex;//更新选中框                  
                        display.GetDateDisplay().Play();
                    }
                    else
                    {
                        Globals.Log("相机连接失败");
                    }
                }
            }
            else
            {

                Globals.Log("没有发现相机");
            }

            if (m_stDeviceList.nDeviceNum > 0)//发现可见光相机
            {
                OpenCamera();
                StartGrab();
            }
        }

        private void skinButton2_Click(object sender, EventArgs e)
        {
            setting1.Visible = true;
        }

        private void button1_MouseHover(object sender, EventArgs e)
        {
            //button1.ForeColor = Color.Black;

        }

        private void button2_MouseHover(object sender, EventArgs e)
        {
            //button2.ForeColor = Color.Black;
        }

        private void skinButton1_Click_1(object sender, EventArgs e)
        {

        }

        private void skinButton2_Click_1(object sender, EventArgs e)
        {

        }

        private void skinButton1_Click_2(object sender, EventArgs e)
        {

        }

        private void skinButton2_Click_2(object sender, EventArgs e)
        {
            setting1.Visible = true;
        }

        private void button1_MouseLeave(object sender, EventArgs e)
        {
            //button1.ForeColor = Color.WhiteSmoke;
        }

        private void button2_MouseLeave(object sender, EventArgs e)
        {
            //button2.ForeColor = Color.WhiteSmoke;
        }

        private void skinButton1_Click_3(object sender, EventArgs e)
        {
            try
            {
                if (Globals.openLampFlag)
                {
                    Globals.openLampFlag = false;
                    if (FormMain.socketClient != null)
                    {
                        //将字符串指令转换为byte数组
                        byte[] buf = System.Text.Encoding.Default.GetBytes("AT+STACH2=0\r\n");
                        //发送AT指令
                        FormMain.socketClient.Send(buf);
                    }
                }
                else
                {
                    Globals.openLampFlag = true;
                    if (FormMain.socketClient != null)
                    {
                        //将字符串指令转换为byte数组
                        byte[] buf = System.Text.Encoding.Default.GetBytes("AT+STACH2=1\r\n");
                        //发送AT指令
                        FormMain.socketClient.Send(buf);
                    }
                }
            }
            catch (Exception ex)
            {
                Globals.Log("skinButton1_Click_3" + ex.Message);
            }     
        }

        private void button10_Click(object sender, EventArgs e)
        {
            ////发现红外相机
            if (dev_num > 0)
            {
                try
                {
                    DataDisplay.CurrSelectedWndIndex = 0;
                    _FormDisplayLst[DataDisplay.CurrSelectedWndIndex].Show();
                    FormDisplay display = _DataControl.GetCurrDisplayForm();
                    if (display != null)
                    {
                        MagDevice device = display.GetDateDisplay().GetDevice();
                        device.StopProcessImage();

                        if (device.LinkCamera(_LstEnumInfo[0].intCamIp, 2000))
                        //if (device.LinkCamera("192.168.1.101", 2000))
                        {
                            _FormDisplayLst[DataDisplay.CurrSelectedWndIndex].Show();
                            Globals.Log("相机连接成功");
                            DataDisplay.CurrSelectedWndIndex = display.GetDateDisplay().WndIndex;//更新选中框                  
                            display.GetDateDisplay().Play();
                        }
                        else
                        {
                            Globals.Log("相机连接失败");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Globals.Log("启动红外相机" + ex.Message);
                }
            }
            else
            {

                Globals.Log("没有发现相机");
            }

            if (m_stDeviceList.nDeviceNum > 0)//发现可见光相机
            {
                OpenCamera();
                StartGrab();
            }
        }

        private void button11_Click(object sender, EventArgs e)
        {
            setting1.Visible = true;
        }

        private void button10_MouseHover(object sender, EventArgs e)
        {
            button10.ForeColor = Color.Black;
        }

        private void button10_MouseLeave(object sender, EventArgs e)
        {
            button10.ForeColor = Color.WhiteSmoke;
        }

        private void button11_MouseHover(object sender, EventArgs e)
        {
            button11.ForeColor = Color.Black;
        }

        private void button11_MouseLeave(object sender, EventArgs e)
        {
            button11.ForeColor = Color.WhiteSmoke;
        }

        private void FormMain_Shown(object sender, EventArgs e)
        {

            string backgroundImagePath = Application.StartupPath + "\\3363s.jpg";
            //Console.WriteLine(backgroundImagePath);
            Image TabBackground = Image.FromFile("3363s.jpg");//设置tab页面背景图片
            if (TabBackground != null)
            {
                Console.WriteLine(tabControlMain.TabPages.Count);

                tabControlMain.TabPages[0].BackgroundImage = TabBackground;
                tabControlMain.TabPages[1].BackgroundImage = TabBackground;
                //this.BackgroundImage = TabBackground;

            }
            else
            {
                Globals.Log("为空");
            }
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;
                return cp;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            // 使用双缓冲
            this.DoubleBuffered = true;

             //背景重绘移动到此
            if (this.BackgroundImage != null)
            {
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                e.Graphics.DrawImage(
                    this.BackgroundImage,
                    new System.Drawing.Rectangle(0, 0, this.Width, this.Height),
                    0,
                    0,
                    this.BackgroundImage.Width,
                    this.BackgroundImage.Height,
                    System.Drawing.GraphicsUnit.Pixel);
            }
            base.OnPaint(e);
        }

        private void tabControlMain_DrawItem(object sender, DrawItemEventArgs e)
        {
            
        }

    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace CoalTrainMonitoringSystemServer
{
    public class Globals
    {
        public const string CONFIG_FILENAME = "config.xml";
        private static FormMain _FormMain = null;
        /// <summary>
        /// 是否播放报警声音标识
        /// </summary>
        public static bool bSoundAlert;
        /// <summary>
        /// 是否播放报警声音标识
        /// </summary>
        public static bool bSaveAlert_1;
        /// <summary>
        /// 是否保存XML标识
        /// </summary>
        public static bool bWrite_1 = false;
        /// <summary>
        /// 是否保存XML标识
        /// </summary>
        public static bool bSaveXML_1 = false;

        public static bool openLampFlag = false;

        public static bool closeLampFlag = false;

        public static float maxTemp;

        public Globals()
        {
            
        }
        public static FormMain GetMainFrm()
        {
            return _FormMain;
        }

        public static void SetMainFrm(FormMain frmMain)
        {
            _FormMain = frmMain;
        }

        //获得本机IP
        public static string GetLocalIP()
        {
            try
            {
                string HostName = Dns.GetHostName(); //得到主机名
                IPHostEntry IpEntry = Dns.GetHostEntry(HostName);
                for (int i = 0; i < IpEntry.AddressList.Length; i++)
                {
                    //从IP地址列表中筛选出IPv4类型的IP地址
                    //AddressFamily.InterNetwork表示此IP为IPv4,
                    //AddressFamily.InterNetworkV6表示此地址为IPv6类型
                    if (IpEntry.AddressList[i].AddressFamily == AddressFamily.InterNetwork)
                    {
                        return IpEntry.AddressList[i].ToString();
                    }
                }
                return "";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public static void Log(string str)
        {
            string folderName = Application.StartupPath + "\\Log\\";
            if (!Directory.Exists(folderName))
            {
                Directory.CreateDirectory(folderName);
            }
            DateTime dateTime = DateTime.Now;
            string fileName = dateTime.Year.ToString() + dateTime.Month.ToString("D2") + dateTime.Day.ToString("D2") + ".txt";

            str += " " + dateTime.Hour.ToString("D2") + ":" + dateTime.Minute.ToString("D2") + ":" + dateTime.Second.ToString("D2");
            str += "\r\n";
            try
            {
                FileStream fileStream = new FileStream(folderName + fileName, FileMode.Append);
                StreamWriter streamWriter = new StreamWriter(fileStream);
                streamWriter.WriteLine(str);
                streamWriter.Close();
                streamWriter.Dispose();
                fileStream.Close();
                fileStream.Dispose();
            }
            catch (Exception e)
            { }
        }
    }
}

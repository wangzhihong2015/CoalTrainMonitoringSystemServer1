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
        /// �Ƿ񲥷ű���������ʶ
        /// </summary>
        public static bool bSoundAlert;
        /// <summary>
        /// �Ƿ񲥷ű���������ʶ
        /// </summary>
        public static bool bSaveAlert_1;
        /// <summary>
        /// �Ƿ񱣴�XML��ʶ
        /// </summary>
        public static bool bWrite_1 = false;
        /// <summary>
        /// �Ƿ񱣴�XML��ʶ
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

        //��ñ���IP
        public static string GetLocalIP()
        {
            try
            {
                string HostName = Dns.GetHostName(); //�õ�������
                IPHostEntry IpEntry = Dns.GetHostEntry(HostName);
                for (int i = 0; i < IpEntry.AddressList.Length; i++)
                {
                    //��IP��ַ�б���ɸѡ��IPv4���͵�IP��ַ
                    //AddressFamily.InterNetwork��ʾ��IPΪIPv4,
                    //AddressFamily.InterNetworkV6��ʾ�˵�ַΪIPv6����
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

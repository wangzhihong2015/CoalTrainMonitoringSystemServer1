using System;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.IO;


namespace CoalTrainMonitoringSystemServer
{
    public class TimeManager
    {
        /// <summary>
        /// 获取当前系统时间
        /// </summary>
        public DateTime GetCurrentTime()
        {
            DateTime currentTime = new DateTime();
            currentTime = DateTime.Now;
            return currentTime;
        }

        public int GetCurrentYear()
        {
            int year = GetCurrentTime().Year;
            return year;
        }

        public int GetCurrentMonth()
        {
            int month = GetCurrentTime().Month;
            return month;
        }
    }
}

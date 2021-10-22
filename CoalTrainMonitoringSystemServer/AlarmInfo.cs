using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace CoalTrainMonitoringSystemServer
{
    public class AlarmInfo
    {
        /// <summary>
        /// 索引ID
        /// </summary>
        [XmlElement(Type = typeof(UInt32), ElementName = "ID")]
        public UInt32 IndexID = 0;
        /// <summary>
        /// 探测时间
        /// </summary>
        [XmlElement(ElementName = "探测时间")]
        public string detectTime;
        /// <summary>
        /// 报警温度
        /// </summary>
        [XmlElement(ElementName = "报警温度")]
        public string alarmTemperatrue = "";
    }

    public class AlarmListInfo
    {
        /// <summary>
        /// 当前索引ID
        /// </summary>
        [XmlElement(Type = typeof(UInt32), ElementName = "当前ID")]
        public UInt32 currentIndexID = 0;
        /// <summary>
        /// 过车信息
        /// </summary>
        [XmlElement(Type = typeof(AlarmInfo), ElementName = "报警信息")]
        public List<AlarmInfo> trainIndexList = new List<AlarmInfo>();
    }
}

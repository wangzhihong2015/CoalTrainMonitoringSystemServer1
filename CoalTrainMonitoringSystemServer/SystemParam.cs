using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace CoalTrainMonitoringSystemServer
{
    public class SystemParam
    {
        public SystemParam()
        { 
        
        }
        /// <summary>
        /// 1#设备名称
        /// </summary>
        [XmlElement(Type = typeof(string), ElementName = "设备名称_1")]
        public string deviceName_1 = "1#设备";
        /// <summary>
        /// 1#设备IP
        /// </summary>
        [XmlElement(Type = typeof(string), ElementName = "设备IP_1")]
        public string deviceIP_1 = "192.168.1.101";
        /// <summary>
        /// 1#可见光IP
        /// </summary>
        [XmlElement(Type = typeof(string), ElementName = "可见光IP_1")]
        public string cameraIP_1 = "192.168.1.105";
        /// <summary>
        /// 1#热成像端口
        /// </summary>
        [XmlElement(Type = typeof(int), ElementName = "热成像端口_1")]
        public int thermographyPort_1 = 54321;
        /// <summary>
        /// 1#可见光端口
        /// </summary>
        [XmlElement(Type = typeof(int), ElementName = "可见光端口_1")]
        public int cameraPort_1 = 54320;
        /// <summary>
        /// 1#报警温度
        /// </summary>
        [XmlElement(Type = typeof(float), ElementName = "报警温度_1")]
        public float alertTemp_1 = 37.0f;
        /// <summary>
        /// 1#修正参数
        /// </summary>
        [XmlElement(Type = typeof(float), ElementName = "修正参数_1")]
        public float adjustParam_1 = 0.0f;
        /// <summary>
        /// 1#模式：使用、调试
        /// </summary>
        [XmlElement(Type = typeof(string), ElementName = "使用模式_1")]
        public string mode_1 = "使用";
        
        ///////////////////////////////////////////////////////////////
        /// <summary>
        /// 2#设备名称
        /// </summary>
        [XmlElement(Type = typeof(string), ElementName = "设备名称_2")]
        public string deviceName_2 = "2#设备";
        /// <summary>
        /// 2#设备IP
        /// </summary>
        [XmlElement(Type = typeof(string), ElementName = "设备IP_2")]
        public string deviceIP_2 = "192.168.1.102";
        /// <summary>
        /// 2#可见光IP
        /// </summary>
        [XmlElement(Type = typeof(string), ElementName = "可见光IP_2")]
        public string cameraIP_2 = "192.168.1.106";
        /// <summary>
        /// 2#热成像端口
        /// </summary>
        [XmlElement(Type = typeof(int), ElementName = "热成像端口_2")]
        public int thermographyPort_2 = 54321;
        /// <summary>
        /// 2#可见光端口
        /// </summary>
        [XmlElement(Type = typeof(int), ElementName = "可见光端口_2")]
        public int cameraPort_2 = 54320;
        /// <summary>
        /// 2#报警温度
        /// </summary>
        [XmlElement(Type = typeof(float), ElementName = "报警温度_2")]
        public float alertTemp_2 = 37.0f;
        /// <summary>
        /// 2#修正参数
        /// </summary>
        [XmlElement(Type = typeof(float), ElementName = "修正参数_2")]
        public float adjustParam_2 = 0.0f;
        /// <summary>
        /// 2#模式：使用、调试
        /// </summary>
        [XmlElement(Type = typeof(string), ElementName = "使用模式_2")]
        public string mode_2 = "使用";
        ///////////////////////////////////////////////////////////////
        /// <summary>
        /// 3#设备名称
        /// </summary>
        [XmlElement(Type = typeof(string), ElementName = "设备名称_3")]
        public string deviceName_3 = "3#设备";
        /// <summary>
        /// 3#设备IP
        /// </summary>
        [XmlElement(Type = typeof(string), ElementName = "设备IP_3")]
        public string deviceIP_3 = "192.168.1.103";
        /// <summary>
        /// 3#可见光IP
        /// </summary>
        [XmlElement(Type = typeof(string), ElementName = "可见光IP_3")]
        public string cameraIP_3 = "192.168.1.107";
        /// <summary>
        /// 3#热成像端口
        /// </summary>
        [XmlElement(Type = typeof(int), ElementName = "热成像端口_3")]
        public int thermographyPort_3 = 54321;
        /// <summary>
        /// 3#可见光端口
        /// </summary>
        [XmlElement(Type = typeof(int), ElementName = "可见光端口_3")]
        public int cameraPort_3 = 54320;
        /// <summary>
        /// 3#报警温度
        /// </summary>
        [XmlElement(Type = typeof(float), ElementName = "报警温度_3")]
        public float alertTemp_3 = 37.0f;
        /// <summary>
        /// 3#修正参数
        /// </summary>
        [XmlElement(Type = typeof(float), ElementName = "修正参数_3")]
        public float adjustParam_3 = 0.0f;
        /// <summary>
        /// 3#模式：使用、调试
        /// </summary>
        [XmlElement(Type = typeof(string), ElementName = "使用模式_3")]
        public string mode_3 = "使用";
        ///////////////////////////////////////////////////////////////
        /// <summary>
        /// 4#设备名称
        /// </summary>
        [XmlElement(Type = typeof(string), ElementName = "设备名称_4")]
        public string deviceName_4 = "4#设备";
        /// <summary>
        /// 4#设备IP
        /// </summary>
        [XmlElement(Type = typeof(string), ElementName = "设备IP_4")]
        public string deviceIP_4 = "192.168.1.104";
        /// <summary>
        /// 4#可见光IP
        /// </summary>
        [XmlElement(Type = typeof(string), ElementName = "可见光IP_4")]
        public string cameraIP_4 = "192.168.1.108";
        /// <summary>
        /// 4#热成像端口
        /// </summary>
        [XmlElement(Type = typeof(int), ElementName = "热成像端口_4")]
        public int thermographyPort_4 = 54321;
        /// <summary>
        /// 4#可见光端口
        /// </summary>
        [XmlElement(Type = typeof(int), ElementName = "可见光端口_4")]
        public int cameraPort_4 = 54320;
        /// <summary>
        /// 4#报警温度
        /// </summary>
        [XmlElement(Type = typeof(float), ElementName = "报警温度_4")]
        public float alertTemp_4 = 37.0f;
        /// <summary>
        /// 4#修正参数
        /// </summary>
        [XmlElement(Type = typeof(float), ElementName = "修正参数_4")]
        public float adjustParam_4 = 0.0f;
        /// <summary>
        /// 4#模式：使用、调试
        /// </summary>
        [XmlElement(Type = typeof(string), ElementName = "使用模式_4")]
        public string mode_4 = "使用";
        /// <summary>
        /// 分屏显示：1、4
        /// </summary>
        [XmlElement(Type = typeof(int), ElementName = "分屏显示")]
        public int displayCount = 1;
        /// <summary>
        /// 大值显示参数: 默认值30
        /// </summary>
        [XmlElement(Type = typeof(int), ElementName = "显示参数")]
        public int displayParam = 30;

        /// <summary>
        /// 大值显示时长: 单位毫秒，默认值3000
        /// </summary>
        [XmlElement(Type = typeof(int), ElementName = "显示时长")]
        public int displayTime = 3000;

        /// <summary>
        /// 大值显示最小值 默认值28℃
        /// </summary>
        [XmlElement(Type = typeof(float), ElementName = "显示参数_Min")]
        public float minDisplay = 28.0f;
        /// <summary>
        /// 大值显示最大值 默认值45℃
        /// </summary>
        [XmlElement(Type = typeof(float), ElementName = "显示参数_Max")]
        public float maxDisplay = 45.0f;

        ///// <summary>
        ///// 照明灯IP
        ///// </summary>
        //[XmlElement(Type = typeof(float), ElementName = "照明灯_IP")]
        //public string lamp_IP = "192.168.0.199";

    }
}

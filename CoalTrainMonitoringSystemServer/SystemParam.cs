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
        /// 1#�豸����
        /// </summary>
        [XmlElement(Type = typeof(string), ElementName = "�豸����_1")]
        public string deviceName_1 = "1#�豸";
        /// <summary>
        /// 1#�豸IP
        /// </summary>
        [XmlElement(Type = typeof(string), ElementName = "�豸IP_1")]
        public string deviceIP_1 = "192.168.1.101";
        /// <summary>
        /// 1#�ɼ���IP
        /// </summary>
        [XmlElement(Type = typeof(string), ElementName = "�ɼ���IP_1")]
        public string cameraIP_1 = "192.168.1.105";
        /// <summary>
        /// 1#�ȳ���˿�
        /// </summary>
        [XmlElement(Type = typeof(int), ElementName = "�ȳ���˿�_1")]
        public int thermographyPort_1 = 54321;
        /// <summary>
        /// 1#�ɼ���˿�
        /// </summary>
        [XmlElement(Type = typeof(int), ElementName = "�ɼ���˿�_1")]
        public int cameraPort_1 = 54320;
        /// <summary>
        /// 1#�����¶�
        /// </summary>
        [XmlElement(Type = typeof(float), ElementName = "�����¶�_1")]
        public float alertTemp_1 = 37.0f;
        /// <summary>
        /// 1#��������
        /// </summary>
        [XmlElement(Type = typeof(float), ElementName = "��������_1")]
        public float adjustParam_1 = 0.0f;
        /// <summary>
        /// 1#ģʽ��ʹ�á�����
        /// </summary>
        [XmlElement(Type = typeof(string), ElementName = "ʹ��ģʽ_1")]
        public string mode_1 = "ʹ��";
        
        ///////////////////////////////////////////////////////////////
        /// <summary>
        /// 2#�豸����
        /// </summary>
        [XmlElement(Type = typeof(string), ElementName = "�豸����_2")]
        public string deviceName_2 = "2#�豸";
        /// <summary>
        /// 2#�豸IP
        /// </summary>
        [XmlElement(Type = typeof(string), ElementName = "�豸IP_2")]
        public string deviceIP_2 = "192.168.1.102";
        /// <summary>
        /// 2#�ɼ���IP
        /// </summary>
        [XmlElement(Type = typeof(string), ElementName = "�ɼ���IP_2")]
        public string cameraIP_2 = "192.168.1.106";
        /// <summary>
        /// 2#�ȳ���˿�
        /// </summary>
        [XmlElement(Type = typeof(int), ElementName = "�ȳ���˿�_2")]
        public int thermographyPort_2 = 54321;
        /// <summary>
        /// 2#�ɼ���˿�
        /// </summary>
        [XmlElement(Type = typeof(int), ElementName = "�ɼ���˿�_2")]
        public int cameraPort_2 = 54320;
        /// <summary>
        /// 2#�����¶�
        /// </summary>
        [XmlElement(Type = typeof(float), ElementName = "�����¶�_2")]
        public float alertTemp_2 = 37.0f;
        /// <summary>
        /// 2#��������
        /// </summary>
        [XmlElement(Type = typeof(float), ElementName = "��������_2")]
        public float adjustParam_2 = 0.0f;
        /// <summary>
        /// 2#ģʽ��ʹ�á�����
        /// </summary>
        [XmlElement(Type = typeof(string), ElementName = "ʹ��ģʽ_2")]
        public string mode_2 = "ʹ��";
        ///////////////////////////////////////////////////////////////
        /// <summary>
        /// 3#�豸����
        /// </summary>
        [XmlElement(Type = typeof(string), ElementName = "�豸����_3")]
        public string deviceName_3 = "3#�豸";
        /// <summary>
        /// 3#�豸IP
        /// </summary>
        [XmlElement(Type = typeof(string), ElementName = "�豸IP_3")]
        public string deviceIP_3 = "192.168.1.103";
        /// <summary>
        /// 3#�ɼ���IP
        /// </summary>
        [XmlElement(Type = typeof(string), ElementName = "�ɼ���IP_3")]
        public string cameraIP_3 = "192.168.1.107";
        /// <summary>
        /// 3#�ȳ���˿�
        /// </summary>
        [XmlElement(Type = typeof(int), ElementName = "�ȳ���˿�_3")]
        public int thermographyPort_3 = 54321;
        /// <summary>
        /// 3#�ɼ���˿�
        /// </summary>
        [XmlElement(Type = typeof(int), ElementName = "�ɼ���˿�_3")]
        public int cameraPort_3 = 54320;
        /// <summary>
        /// 3#�����¶�
        /// </summary>
        [XmlElement(Type = typeof(float), ElementName = "�����¶�_3")]
        public float alertTemp_3 = 37.0f;
        /// <summary>
        /// 3#��������
        /// </summary>
        [XmlElement(Type = typeof(float), ElementName = "��������_3")]
        public float adjustParam_3 = 0.0f;
        /// <summary>
        /// 3#ģʽ��ʹ�á�����
        /// </summary>
        [XmlElement(Type = typeof(string), ElementName = "ʹ��ģʽ_3")]
        public string mode_3 = "ʹ��";
        ///////////////////////////////////////////////////////////////
        /// <summary>
        /// 4#�豸����
        /// </summary>
        [XmlElement(Type = typeof(string), ElementName = "�豸����_4")]
        public string deviceName_4 = "4#�豸";
        /// <summary>
        /// 4#�豸IP
        /// </summary>
        [XmlElement(Type = typeof(string), ElementName = "�豸IP_4")]
        public string deviceIP_4 = "192.168.1.104";
        /// <summary>
        /// 4#�ɼ���IP
        /// </summary>
        [XmlElement(Type = typeof(string), ElementName = "�ɼ���IP_4")]
        public string cameraIP_4 = "192.168.1.108";
        /// <summary>
        /// 4#�ȳ���˿�
        /// </summary>
        [XmlElement(Type = typeof(int), ElementName = "�ȳ���˿�_4")]
        public int thermographyPort_4 = 54321;
        /// <summary>
        /// 4#�ɼ���˿�
        /// </summary>
        [XmlElement(Type = typeof(int), ElementName = "�ɼ���˿�_4")]
        public int cameraPort_4 = 54320;
        /// <summary>
        /// 4#�����¶�
        /// </summary>
        [XmlElement(Type = typeof(float), ElementName = "�����¶�_4")]
        public float alertTemp_4 = 37.0f;
        /// <summary>
        /// 4#��������
        /// </summary>
        [XmlElement(Type = typeof(float), ElementName = "��������_4")]
        public float adjustParam_4 = 0.0f;
        /// <summary>
        /// 4#ģʽ��ʹ�á�����
        /// </summary>
        [XmlElement(Type = typeof(string), ElementName = "ʹ��ģʽ_4")]
        public string mode_4 = "ʹ��";
        /// <summary>
        /// ������ʾ��1��4
        /// </summary>
        [XmlElement(Type = typeof(int), ElementName = "������ʾ")]
        public int displayCount = 1;
        /// <summary>
        /// ��ֵ��ʾ����: Ĭ��ֵ30
        /// </summary>
        [XmlElement(Type = typeof(int), ElementName = "��ʾ����")]
        public int displayParam = 30;

        /// <summary>
        /// ��ֵ��ʾʱ��: ��λ���룬Ĭ��ֵ3000
        /// </summary>
        [XmlElement(Type = typeof(int), ElementName = "��ʾʱ��")]
        public int displayTime = 3000;

        /// <summary>
        /// ��ֵ��ʾ��Сֵ Ĭ��ֵ28��
        /// </summary>
        [XmlElement(Type = typeof(float), ElementName = "��ʾ����_Min")]
        public float minDisplay = 28.0f;
        /// <summary>
        /// ��ֵ��ʾ���ֵ Ĭ��ֵ45��
        /// </summary>
        [XmlElement(Type = typeof(float), ElementName = "��ʾ����_Max")]
        public float maxDisplay = 45.0f;

        ///// <summary>
        ///// ������IP
        ///// </summary>
        //[XmlElement(Type = typeof(float), ElementName = "������_IP")]
        //public string lamp_IP = "192.168.0.199";

    }
}

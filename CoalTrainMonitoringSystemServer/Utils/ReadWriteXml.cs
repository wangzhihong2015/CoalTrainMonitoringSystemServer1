using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CoalTrainMonitoringSystemServer
{
    public  class ReadWriteXml
    {
        //函数名：CreatXmlText
        //功能：新建xml文件
        //参数：rootNodeName       根节点名称
        //      childNodeName      子节点名称
        //      childNodeContent   子节点内容
        //      xmlFileName        xml文件名称
        public void CreatXmlText(string rootNodeName,string childNodeName,string childNodeContent,string xmlFileName)
        {
            XDocument document = new XDocument();
            XElement root = new XElement(rootNodeName);
            XElement root1 = new XElement(childNodeName);
            //root1.SetAttributeValue("attribute", "ip");
            root1.SetElementValue("content", childNodeContent);
            root.Add(root1);
            root.Save(xmlFileName);//存储的文件路径
        }


        //函数名：ReadXml
        //功能：  读取xml文件子节点内容
        //参数：childNodeName      子节点名称
        //      xmlFileName        xml文件名称
        public string ReadXml(string xmlFilePath ,string childNodeName)
        {
            //存放xml文件的地址
            string path = xmlFilePath;
            //读取路径下的文件
            XDocument document =XDocument.Load(path);
            //得到根节点内的内容
            XElement root=document.Root;
            //查找子节点内容
            XElement childnode = root.Element(childNodeName);
            //XAttribute attribute = childnode.Attribute("attribute");
            //Console.WriteLine(attribute.Value);
            //查找具体内容
            XElement result = childnode.Element("content");       
            //XAttribute attribute = childnode.Attribute("attribute");
            //Console.WriteLine(attribute.Value);
            //查找具体内容
            return result.Value;
        
        }

        //函数名：WriteXml
        //功能：  xml添加或者修改子节点内容
        //参数：childNodeName      子节点名称
        //      childNodeContent   子节点内容
        public void WriteXml(string xmlFilePath, string childNodeName, string childNodeContent)
        {
            //存放xml文件的地址
            string path = xmlFilePath;
            //读取路径下的文件
            XDocument document = XDocument.Load(path);
            //得到根节点内的内容
            XElement root = document.Root;
            if (root.Element(childNodeName) == null)
            {
                XElement root1 = new XElement(childNodeName);
                //root1.SetAttributeValue("attribute", "ip");
                root1.SetElementValue("content", childNodeContent);
                root.Add(root1);               
            }
            else
            {
                XElement childnode = root.Element(childNodeName);
                childnode.SetElementValue("content", childNodeContent);               
            }
            root.Save(path);//存储的文件路径 
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoalTrainMonitoringSystemServer
{
    public class FileManager
    {
        public void  CreateFile(string fileName)
        {
            //判断是否已经有该文件
            if (!System.IO.File.Exists(fileName))
            {
                //如果没有创建文件
                FileStream fs1 = new FileStream(fileName, FileMode.Create, FileAccess.Write);//创建写入文件                //设置文件属性为隐藏
                //System.IO.File.SetAttributes(@"c:\\users\\administrator\\desktop\\webapplication1\\webapplication1\\testtxt.txt", FileAttributes.Hidden); //隐藏
                fs1.Close();
            }
        }
        //将收到的字符串保存为文本
        public void WriteStrToTxt(string str, string filePath)
        {
            StreamWriter FileWriter = new StreamWriter(filePath, true); //写文件
            FileWriter.Write(str + "\r\n");//将字符串写入
            FileWriter.Close(); //关闭StreamWriter对象
        }

    }
}

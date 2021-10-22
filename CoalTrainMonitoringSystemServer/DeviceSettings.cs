using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CoalTrainMonitoringSystemServer
{
    public partial class DeviceSettings : UserControl
    {
        /// <summary>
        /// 传入的设备编号
        /// </summary>
        public int deviceNo = 1;
        public DeviceSettings()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.AllPaintingInWmPaint, true);

            InitializeComponent();
            textBox2.Width = tableLayoutPanel1.Width / 4;
            textBox3.Width = tableLayoutPanel1.Width / 4;
            textBox4.Width = tableLayoutPanel1.Width / 4;
            textBox5.Width = tableLayoutPanel1.Width / 4;
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            switch (deviceNo)
            {
                case 1:
                    FormMain.sysParam.deviceName_1 = textBox1.Text;
                    FormMain.sysParam.cameraIP_1 = textBox5.Text;
                    FormMain.sysParam.deviceIP_1 = textBox4.Text;
                    FormMain.sysParam.alertTemp_1 = float.Parse(textBox2.Text);
                    FormMain.sysParam.adjustParam_1 = float.Parse(textBox3.Text);
                    break;
                case 2:
                    FormMain.sysParam.deviceName_2 = textBox1.Text;
                    FormMain.sysParam.cameraIP_2 = textBox5.Text;
                    FormMain.sysParam.deviceIP_2 = textBox4.Text;
                    FormMain.sysParam.alertTemp_2 = float.Parse(textBox2.Text);
                    FormMain.sysParam.adjustParam_2 = float.Parse(textBox3.Text);
                    break;
                case 3:
                    FormMain.sysParam.deviceName_3 = textBox1.Text;
                    FormMain.sysParam.cameraIP_3 = textBox5.Text;
                    FormMain.sysParam.deviceIP_3 = textBox4.Text;
                    FormMain.sysParam.alertTemp_3 = float.Parse(textBox2.Text);
                    FormMain.sysParam.adjustParam_3 = float.Parse(textBox3.Text);
                    break;
                case 4:
                    FormMain.sysParam.deviceName_4 = textBox1.Text;
                    FormMain.sysParam.cameraIP_4 = textBox5.Text;
                    FormMain.sysParam.deviceIP_4 = textBox4.Text;
                    FormMain.sysParam.alertTemp_4 = float.Parse(textBox2.Text);
                    FormMain.sysParam.adjustParam_4 = float.Parse(textBox3.Text);
                    break;
                default:
                    break;
            }
            try 
            {
                FormMain.WriteInfoXml(FormMain.sysParam, FormMain.systemXml);
            }
            catch (Exception ex)
            {
                Globals.Log("button1_Click" + ex.Message);
            }
         
        }

        private void DeviceSettings_Load(object sender, EventArgs e)
        {
            switch (deviceNo)
            {
                case 1:
                    textBox1.Text = FormMain.sysParam.deviceName_1;
                    textBox5.Text = FormMain.sysParam.cameraIP_1;
                    textBox4.Text = FormMain.sysParam.deviceIP_1;
                    textBox2.Text = FormMain.sysParam.alertTemp_1.ToString();
                    textBox3.Text = FormMain.sysParam.adjustParam_1.ToString();
                    break;
                case 2:
                    textBox1.Text = FormMain.sysParam.deviceName_2;
                    textBox5.Text = FormMain.sysParam.cameraIP_2;
                    textBox4.Text = FormMain.sysParam.deviceIP_2;
                    textBox2.Text = FormMain.sysParam.alertTemp_2.ToString();
                    textBox3.Text = FormMain.sysParam.adjustParam_2.ToString();
                    break;
                case 3:
                    textBox1.Text = FormMain.sysParam.deviceName_1;
                    textBox5.Text = FormMain.sysParam.cameraIP_1;
                    textBox4.Text = FormMain.sysParam.deviceIP_1;
                    textBox2.Text = FormMain.sysParam.alertTemp_1.ToString();
                    textBox3.Text = FormMain.sysParam.adjustParam_1.ToString();
                    break;
                case 4:

                    break;
                default:
                    break;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            switch (deviceNo)
            {
                case 1:
                    FormMain.setting1.Visible = false;
                    break;
                case 2:
                    FormMain.setting2.Visible = false;
                    break;
                case 3:
                    FormMain.setting3.Visible = false;
                    break;
                case 4:
                    FormMain.setting4.Visible = false;
                    break;
                default:
                    break;
            }
        }

        private void skinButton1_Click(object sender, EventArgs e)
        {
            switch (deviceNo)
            {
                case 1:
                    FormMain.sysParam.deviceName_1 = textBox1.Text;
                    FormMain.sysParam.cameraIP_1 = textBox5.Text;
                    FormMain.sysParam.deviceIP_1 = textBox4.Text;
                    FormMain.sysParam.alertTemp_1 = float.Parse(textBox2.Text);
                    FormMain.sysParam.adjustParam_1 = float.Parse(textBox3.Text);
                    break;
                case 2:
                    FormMain.sysParam.deviceName_2 = textBox1.Text;
                    FormMain.sysParam.cameraIP_2 = textBox5.Text;
                    FormMain.sysParam.deviceIP_2 = textBox4.Text;
                    FormMain.sysParam.alertTemp_2 = float.Parse(textBox2.Text);
                    FormMain.sysParam.adjustParam_2 = float.Parse(textBox3.Text);
                    break;
                case 3:
                    FormMain.sysParam.deviceName_3 = textBox1.Text;
                    FormMain.sysParam.cameraIP_3 = textBox5.Text;
                    FormMain.sysParam.deviceIP_3 = textBox4.Text;
                    FormMain.sysParam.alertTemp_3 = float.Parse(textBox2.Text);
                    FormMain.sysParam.adjustParam_3 = float.Parse(textBox3.Text);
                    break;
                case 4:
                    FormMain.sysParam.deviceName_4 = textBox1.Text;
                    FormMain.sysParam.cameraIP_4 = textBox5.Text;
                    FormMain.sysParam.deviceIP_4 = textBox4.Text;
                    FormMain.sysParam.alertTemp_4 = float.Parse(textBox2.Text);
                    FormMain.sysParam.adjustParam_4 = float.Parse(textBox3.Text);
                    break;
                default:
                    break;
            }
            try
            {
                FormMain.WriteInfoXml(FormMain.sysParam, FormMain.systemXml);
            }
            catch (Exception ex)
            {
                Globals.Log("button1_Click" + ex.Message);
            }
        }

        private void skinButton2_Click(object sender, EventArgs e)
        {
            switch (deviceNo)
            {
                case 1:
                    FormMain.setting1.Visible = false;
                    break;
                case 2:
                    FormMain.setting2.Visible = false;
                    break;
                case 3:
                    FormMain.setting3.Visible = false;
                    break;
                case 4:
                    FormMain.setting4.Visible = false;
                    break;
                default:
                    break;
            }
        }
    }
}

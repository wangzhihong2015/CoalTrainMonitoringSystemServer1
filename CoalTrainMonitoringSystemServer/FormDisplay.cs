using SDK;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CoalTrainMonitoringSystemServer
{
    public partial class FormDisplay : Form
    {
        DataDisplay _DataDisplay;
        Point _ptMouse = new Point();
        float holdMax_1 = 0.0F;

        /// <summary>
        /// 1# 持续下降间隔
        /// </summary>
        TimeSpan timeSpanDown_1 = new TimeSpan();
        /// <summary>
        /// 1# 最近一次上升的时间
        /// </summary>
        DateTime lastUpTime_1 = DateTime.Now;
        /// <summary>
        /// 热成像图形字体
        /// </summary>
        Font font1 = new Font("宋体", 30F);
        Pen penLightGreen = new Pen(Brushes.LightGreen);

        bool openLampFlag = false;


        public FormDisplay()
        {
            InitializeComponent();

            _DataDisplay = new DataDisplay();
            FormMain.OnDestroy += new FormMain.delegateDestroy(OnDestroy);
        }

        void OnDestroy()
        {
            MagDevice device = _DataDisplay.GetDevice();

            if (device.IsProcessingImage())
            {
                device.StopProcessImage();
            }

            if (device.IsLinked())
            {
                device.DisLinkCamera();
            }

            _DataDisplay.DestroyDevice();
        }

        public DataDisplay GetDateDisplay()
        {
            return _DataDisplay;
        }

        private void FormInfraredImage_Load(object sender, EventArgs e)
        {
            _DataDisplay.CreateDevice();
        }


        private void FormInfraredImage_Paint(object sender, PaintEventArgs e)
        {
            try
            {
                //Console.WriteLine(_DataDisplay.WndIndex);
                //Console.WriteLine("FormInfraredImage_Paint线程ID" + Thread.CurrentThread.ManagedThreadId.ToString());
                //if (Globals.GetMainFrm().GetDeviceNum() > 0 && DataDisplay.CurrSelectedWndIndex == _DataDisplay.WndIndex)
                //{
                Graphics graphic = e.Graphics;
                _DataDisplay.GetDevice().Lock();

                if (!DrawImages(graphic, this.Width, this.Height))//画红外图
                {
                    _DataDisplay.GetDevice().Unlock();

                    Rectangle rect = new Rectangle(0, 0, this.Width, this.Height);
                    graphic.FillRectangle(Brushes.White, rect);

                    return;
                }

                MagDevice device = _DataDisplay.GetDevice();
                GroupSDK.CAMERA_INFO info = device.GetCamInfo();

                int[] info1_1 = new int[5];

                if (_DataDisplay.GetDevice().GetRectTemperatureInfo(0, 0, 383, 287, info1_1))
                {
                    int temp = info1_1[1];
                    float maxTemp = temp * 0.001f;
                    Globals.maxTemp = maxTemp;
                    //温度高于报警值
                    if (maxTemp > FormMain.sysParam.alertTemp_1)
                    {
                        try
                        {
                            if (!Globals.openLampFlag)
                            {
                                openLampFlag = true;
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
                            Globals.Log("FormInfraredImage_Paint-1" + ex.Message);
                        }

                        if (maxTemp > holdMax_1)
                        {
                            holdMax_1 = maxTemp;
                        }
                        Globals.bSaveAlert_1 = true;
                        Globals.bSoundAlert = true;
                        Globals.bWrite_1 = true;
                        lastUpTime_1 = DateTime.Now;

                    }
                    else
                    {
                        if (Globals.bWrite_1)
                        {
                            //timeSpanDown_1 = DateTime.Now - lastUpTime_1;//下降持续时间

                            //if (timeSpanDown_1.Milliseconds > 800)
                            //{
                            try
                            {
                                Globals.closeLampFlag = true;
                                Globals.bSaveAlert_1 = false;
                                FormMain.alarmListInfo_1.currentIndexID++;
                                FormMain.alarmInfo_1 = new AlarmInfo();
                                FormMain.alarmInfo_1.IndexID = FormMain.alarmListInfo_1.currentIndexID;
                                FormMain.alarmInfo_1.detectTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                FormMain.alarmInfo_1.alarmTemperatrue = holdMax_1.ToString("F1") + "℃";
                                FormMain.alarmListInfo_1.trainIndexList.Add(FormMain.alarmInfo_1);
                                Globals.bSaveXML_1 = true;
                                Globals.bWrite_1 = false;
                                holdMax_1 = 0.0F;
                            }
                            catch (Exception ex)
                            {
                                Globals.Log("ThreadDisplay_1" + ex.Message);
                                Globals.bWrite_1 = false;
                            }
                            //}
                        }

                    }

                }
                else
                {
                    Globals.Log("获取温度信息失败");
                }

                DrawMaxTemp(graphic, info, info1_1);
                DrawMouseTemp(graphic, this.Width, this.Height);//鼠标测温

                _DataDisplay.GetDevice().Unlock();
            }
            catch (Exception ex)
            {
                Globals.Log("FormInfraredImage_Paint" + ex.Message);
            }
        }

        /// <summary>
        /// 绘制最高温度
        /// </summary>
        /// <param name="graphic"></param>
        /// <param name="w"></param>
        /// <param name="h"></param>
        void DrawMaxTemp(Graphics graphic, GroupSDK.CAMERA_INFO cameraInfo, int[] info)
        {
            try
            {
                // 获取区域最高温度 0-最低温，1-最高温，2-平均温度，3-最低温度位置，4-最高温度位置
                int rectMaxTemp = info[1];

                //get the fpa coordinate
                int yFPA = info[4] / cameraInfo.intFPAWidth;//出现过试图除以零的情况
                int xFPA = info[4] - yFPA * cameraInfo.intFPAWidth;

                //convert to the screen coordinate
                int x = xFPA * Width / cameraInfo.intFPAWidth + Left;
                int y = Height - yFPA * Height / cameraInfo.intFPAHeight + Top;

                float tmp;

                tmp = rectMaxTemp * 0.001f;
                tmp = correctTemp(tmp, FormMain.sysParam.adjustParam_1);

                String s = tmp.ToString("F1");

                int pad = 1;
                int lineWidth = 8;
                int cx = (int)graphic.MeasureString(s, Font).Width;
                int cy = (int)graphic.MeasureString(s, Font).Height;

                /* draw cross for max temp point */

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
            catch (Exception e1)
            {
                Globals.Log("DrawMaxTemp_1" + e1.Message);
            }
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


        bool DrawImages(Graphics graphic, int w, int h)
        {
            try
            {
                if (_DataDisplay.GetDevice().IsLinked() && !_DataDisplay.GetDevice().IsProcessingImage())
                {
                    return false;
                }

                IntPtr pIrData = IntPtr.Zero;
                IntPtr pIrInfo = IntPtr.Zero; ;

                if (_DataDisplay.GetDevice().isPlayingLocalMgs())
                {
                    if (!_DataDisplay.finishedPlaying())
                    {
                        _DataDisplay.GetDevice().LocalStorageMgsPopFrame();
                        _DataDisplay.frameWait();
                        Thread.Sleep(15);
                    }
                    else
                    {
                        _DataDisplay.GetDevice().LocalStorageMgsStop();
                    }
                }

                if (!_DataDisplay.GetDevice().GetOutputBMPdata(ref pIrData, ref pIrInfo))
                {
                    return false;
                }

                GroupSDK.CAMERA_INFO info = _DataDisplay.GetDevice().GetCamInfo();

                IntPtr hDC = graphic.GetHdc();

                WINAPI.SetStretchBltMode(hDC, WINAPI.StretchMode.STRETCH_HALFTONE);
                WINAPI.StretchDIBits(hDC, 0, 0, w, h, 0, 0, info.intVideoWidth,
                        info.intVideoHeight, pIrData, pIrInfo, (uint)WINAPI.PaletteMode.DIB_RGB_COLORS,
                        (uint)WINAPI.ExecuteOption.SRCCOPY);

                graphic.ReleaseHdc();
            }
            catch (Exception ex)
            {
                Globals.Log("DrawImages" + ex.Message);
            }
            return true;
        }

        void DrawMouseTemp(Graphics graphic, int w, int h)
        {
            try
            {
                Point pt = MousePosition;

                Point ptLeftUp = this.PointToScreen(new Point(0, 0));
                Point ptRightDown = this.PointToScreen(new Point(this.Width - 1, this.Height - 1));

                if (pt.X > ptRightDown.X || pt.X < ptLeftUp.X || pt.Y > ptRightDown.Y || pt.Y < ptLeftUp.Y)
                {
                    return;
                }

                MagDevice device = _DataDisplay.GetDevice();
                GroupSDK.CAMERA_INFO info = device.GetCamInfo();

                int intFPAx = _ptMouse.X * info.intFPAWidth / w;
                int intFPAy = info.intFPAHeight - _ptMouse.Y * info.intFPAHeight / h - 1;

                int intTemp = device.GetTemperatureProbe((uint)intFPAx, (uint)intFPAy, 1);

                GroupSDK.FIX_PARAM param = new GroupSDK.FIX_PARAM();
                device.GetFixPara(ref param);

                if (_DataDisplay.GetDisplayConfig().bEnableExtCorrect)
                {
                    intTemp = device.FixTemperature(intTemp, param.fEmissivity, (uint)intFPAx, (uint)intFPAy);
                }

                string sText = (intTemp * 0.001f).ToString("0.0");

                int cx = (int)graphic.MeasureString(sText, this.Font).Width;
                int cy = (int)graphic.MeasureString(sText, this.Font).Height;

                int x = _ptMouse.X;
                int y = _ptMouse.Y - cy;//默认右上

                if (_ptMouse.Y < cy)//处于上边沿
                {
                    y = _ptMouse.Y + 16;

                    if (_ptMouse.X < cx)//处于左边沿
                    {
                        x = _ptMouse.X + 16;
                    }
                    else
                    {
                        x = _ptMouse.X - cx;
                    }
                }
                else if (_ptMouse.X > w - cx)//右边沿
                {
                    x = _ptMouse.X - cx;
                }

                graphic.FillRectangle(Brushes.White, new Rectangle(x, y, cx, cy));
                graphic.DrawString(sText, this.Font, Brushes.Black, (float)x, (float)y);
            }
            catch (Exception ex)
            {
                Globals.Log("DrawMouseTemp" + ex.Message);
            }
        }

        private void FormInfraredImage_MouseMove(object sender, MouseEventArgs e)
        {
            _ptMouse = e.Location;
            Invalidate(false);
        }

    }
}

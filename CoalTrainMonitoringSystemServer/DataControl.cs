using System;
using System.Collections.Generic;
using System.Text;

namespace CoalTrainMonitoringSystemServer
{
    public class DataControl
    {
        //委托函数用来更新相机显示窗口的位置
        public delegate void delegateUpdateDisplayPostion();

        //新建delegateUpdateDisplayPostion委托的实例UpdateDisplayPostion
        public static delegateUpdateDisplayPostion UpdateDisplayPostion = null;

        MagService _MagService = null;

        //两行两列
        uint _DisplayRowNum = 2;
        uint _DisplayColNum = 2;

        //设置显示窗口数量
        public void SetDisplayWndNum(uint row, uint col)
        {
            _DisplayRowNum = row;
            _DisplayColNum = col;

            if (UpdateDisplayPostion != null)//通知UI更新
            {
                UpdateDisplayPostion();
            }
        }

        //获取显示窗口的数量，几行几列
        public void GetDisplayWndNum(ref uint row, ref uint col)
        {
            row = _DisplayRowNum;
            col = _DisplayColNum;
        }

        //创建服务 初始化MageService
        public bool CreateService()
        {
            if (_MagService == null)
            {
                _MagService = new MagService(IntPtr.Zero);
            }
            //初始化一个通道
            return _MagService.Initialize();
        }

        //销毁服务
        public void DestroyService()
        {
            if (_MagService != null)
            {
                _MagService.DeInitialize();
            }
        }

        //获取服务
        public MagService GetService()
        {
            return _MagService;
        }


        //使用相机IP判断是否是自己连接
        public bool IsLinkedByMyself(uint intCameraIP)
        {
            //获取最多显示相机图像的个数
            uint max_wnd = Globals.GetMainFrm().GetMaxDeviceWnd();

            for (uint i = 0; i < max_wnd; i++)
            {
                MagDevice device = Globals.GetMainFrm().GetFormDisplay(i).GetDateDisplay().GetDevice();

                if (device.IsLinked() && device.GetDevIPAddress() == intCameraIP)
                {
                    return true;
                }
            }

            return false;
        }


        //判断是否连接其他相机
        public bool IsLinkedByOthers(uint intUserIP)
        {
            if (intUserIP != 0 && intUserIP != _MagService.GetLocalIp())
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 是否被别人抢占了相机
        /// </summary>
        /// <returns></returns>
        public bool IsInvadedByOthers(uint intUserIP)
        {
            uint max_wnd = Globals.GetMainFrm().GetMaxDeviceWnd();

            for (uint i = 0; i < max_wnd; i++)
            {
                MagDevice device = Globals.GetMainFrm().GetFormDisplay(i).GetDateDisplay().GetDevice();

                if (device.GetDevIPAddress() != 0 && intUserIP != _MagService.GetLocalIp())
                {
                    return true;
                }
            }

            return false;
        }


        public FormDisplay GetFirstFreeDisplayForm()
        {

            uint max_wnd = Globals.GetMainFrm().GetMaxDeviceWnd();

            for (uint i = 0; i < max_wnd; i++)
            {
                FormDisplay frmDisplay = Globals.GetMainFrm().GetFormDisplay(i);
                MagDevice device = frmDisplay.GetDateDisplay().GetDevice();

                if (device.GetDevIPAddress() == 0)
                {
                    return frmDisplay;
                }
            }

            return null;
        }

        public FormDisplay GetCurrDisplayForm()
        {
            FormDisplay frmDisplay = Globals.GetMainFrm().GetFormDisplay(DataDisplay.CurrSelectedWndIndex);
            MagDevice device = frmDisplay.GetDateDisplay().GetDevice();

            if (device.GetDevIPAddress() == 0)
            {
                return frmDisplay;
            }
            else
            {
                return GetFirstFreeDisplayForm();
            }
        }

        public FormDisplay GetBindedDisplayForm(uint intCameraIP)
        {
            uint max_wnd = Globals.GetMainFrm().GetMaxDeviceWnd();

            for (uint i = 0; i < max_wnd; i++)
            {
                FormDisplay frmDisplay = Globals.GetMainFrm().GetFormDisplay(i);
                MagDevice device = frmDisplay.GetDateDisplay().GetDevice();

                if (device.GetDevIPAddress() == intCameraIP)
                {
                    return frmDisplay;
                }
            }

            return null;
        }
    }
}

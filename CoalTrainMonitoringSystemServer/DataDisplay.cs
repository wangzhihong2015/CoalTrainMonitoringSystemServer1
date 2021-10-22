using System;
using System.Collections.Generic;
using System.Text;
using SDK;
using System.Threading;

namespace CoalTrainMonitoringSystemServer
{
    public struct Display_Config
    {
        public bool bEnableExtCorrect;//是否启用温度修正
    }

    public class DataDisplay
    {
        Display_Config _DisplayConfig;

        private static uint _CurrSelectedWndIndex = 0;
        private uint _WndIndex = 0;
        private MagDevice _MagDevice = new MagDevice(IntPtr.Zero);
        GroupSDK.DelegateNewFrame NewFrame = null;

        private int _FrameCount = -1;

        public Display_Config GetDisplayConfig()
        {
            return _DisplayConfig;
        }

        public static uint CurrSelectedWndIndex
        {
            get { return _CurrSelectedWndIndex; }
            set { _CurrSelectedWndIndex = value; }
        }

        public uint WndIndex
        {
            get { return _WndIndex; }
            set { _WndIndex = value; }
        }

        public bool CreateDevice()
        {
            if (_MagDevice == null)
            {
                _MagDevice = new MagDevice(IntPtr.Zero);
            }

            if (NewFrame == null)
            {
                NewFrame = new GroupSDK.DelegateNewFrame(NewFrameCome);
            }

            return _MagDevice.Initialize();
        }

        public void DestroyDevice()
        {
            if (_MagDevice != null)
            {
                _MagDevice.DeInitialize();
            }
        }

        public MagDevice GetDevice()
        {
            return _MagDevice;
        }

        public bool Play()
        {
            GroupSDK.CAMERA_INFO cam_info = _MagDevice.GetCamInfo();

            GroupSDK.OUTPUT_PARAM param = new GroupSDK.OUTPUT_PARAM();
            param.intFPAWidth = (uint)cam_info.intFPAWidth;
            param.intFPAHeight = (uint)cam_info.intFPAHeight;
            param.intBMPWidth = (uint)cam_info.intVideoWidth;
            param.intBMPHeight = (uint)cam_info.intVideoHeight;
            param.intColorbarWidth = 20;
            param.intColorbarHeight = 100;

            if (_MagDevice.StartProcessImage(param, NewFrame, (uint)GroupSDK.STREAM_TYPE.STREAM_TEMPERATURE, IntPtr.Zero))
            {
                _MagDevice.SetColorPalette(GroupSDK.COLOR_PALETTE.IRONBOW);
                return true;
            }

            return false;
        }

        public bool LoadDdt(string sFileName)
        {
            _MagDevice.Initialize();
            _MagDevice.StopProcessImage();

            GroupSDK.OUTPUT_PARAM param = new GroupSDK.OUTPUT_PARAM();
            param.intFPAWidth = 384;
            param.intFPAHeight = 288;
            param.intBMPWidth = 384;
            param.intBMPHeight = 288;
            param.intColorbarWidth = 20;
            param.intColorbarHeight = 100;

            return _MagDevice.LoadDDT(param, sFileName, NewFrame, IntPtr.Zero);
        }

        public int LoadMgs(string sFileName)
        {
            _MagDevice.Initialize();
            _MagDevice.StopProcessImage();

            _FrameCount = _MagDevice.LocalStorageMgsPlay(sFileName, NewFrame, IntPtr.Zero);

            _MagDevice.SetAutoEnlargePara(5, 0, 0);
            _MagDevice.SetColorPalette(GroupSDK.COLOR_PALETTE.IRONBOW);

            GroupSDK.FIX_PARAM param = new GroupSDK.FIX_PARAM();
            _MagDevice.GetFixPara(ref param);

            if (param.fEmissivity > 0.0f)
            {
                _MagDevice.SetFixPara(ref param, true); //m_bEnableCorrect
            }

            return _FrameCount;
        }

        public void frameWait()
        {
            _FrameCount--;
        }

        public bool finishedPlaying()
        {
            return _FrameCount <= 0;
        }

        private void NewFrameCome(uint hDevice, int intCamTemp, int intFFCCounter, int intCamState, int intStreamType, IntPtr pUserData)
        {
            //Console.WriteLine("NewFrameCome线程ID" + Thread.CurrentThread.ManagedThreadId.ToString());
            Globals.GetMainFrm().GetFormDisplay(this._WndIndex).Invalidate(false);
        }
    }
}

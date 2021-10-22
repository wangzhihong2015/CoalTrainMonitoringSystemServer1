using System;
using System.Collections.Generic;
using System.Text;

namespace CoalTrainMonitoringSystemServer
{
    public class DataControl
    {
        //ί�к����������������ʾ���ڵ�λ��
        public delegate void delegateUpdateDisplayPostion();

        //�½�delegateUpdateDisplayPostionί�е�ʵ��UpdateDisplayPostion
        public static delegateUpdateDisplayPostion UpdateDisplayPostion = null;

        MagService _MagService = null;

        //��������
        uint _DisplayRowNum = 2;
        uint _DisplayColNum = 2;

        //������ʾ��������
        public void SetDisplayWndNum(uint row, uint col)
        {
            _DisplayRowNum = row;
            _DisplayColNum = col;

            if (UpdateDisplayPostion != null)//֪ͨUI����
            {
                UpdateDisplayPostion();
            }
        }

        //��ȡ��ʾ���ڵ����������м���
        public void GetDisplayWndNum(ref uint row, ref uint col)
        {
            row = _DisplayRowNum;
            col = _DisplayColNum;
        }

        //�������� ��ʼ��MageService
        public bool CreateService()
        {
            if (_MagService == null)
            {
                _MagService = new MagService(IntPtr.Zero);
            }
            //��ʼ��һ��ͨ��
            return _MagService.Initialize();
        }

        //���ٷ���
        public void DestroyService()
        {
            if (_MagService != null)
            {
                _MagService.DeInitialize();
            }
        }

        //��ȡ����
        public MagService GetService()
        {
            return _MagService;
        }


        //ʹ�����IP�ж��Ƿ����Լ�����
        public bool IsLinkedByMyself(uint intCameraIP)
        {
            //��ȡ�����ʾ���ͼ��ĸ���
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


        //�ж��Ƿ������������
        public bool IsLinkedByOthers(uint intUserIP)
        {
            if (intUserIP != 0 && intUserIP != _MagService.GetLocalIp())
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// �Ƿ񱻱�����ռ�����
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

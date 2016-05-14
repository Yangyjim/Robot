using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Advantech.Motion;
using Automation.BDaq;

namespace Robot_main
{
    public enum ActStatus
    {
        STATUS_STOPING = 0,
        STATUS_HOLDING,
        STATUS_RUNNING,
        STATUS_DISABLE
    }

    public struct axisParaminfo
    {        
        //是否生效
        public bool _bEnable { get; set; }
        //相对零点数值
        public double _dZeroPoint { get; set; }
        //轴最大速度
        public double _dMaxVelocity { get; set; }
        //轴最大加速度
        public double _dMaxAccVelocity { get; set; }
        //轴最大减速度
        public double _dMaxDecVelocity { get; set; }

        //PPU
        public UInt32 _nPPU { get; set; }
        //正向最大值
        public double _dbPlusMaxValue { get; set; }
        //负向最大值
        public double _dbMinusMaxValue { get; set; }

        //当前状态
        public UInt32 _nCurrentStatus { get;set; }

        //当前位置
        public double _dbCurrentPosition { get; set; }

        //当前速度
        public double _dbCurrentV { get; set; }

        //关节名称
        public string _strName { get; set; }

        //序列
        public int _nSequence { get; set; }

        public object node { get; set; }        
    }

    public struct armParamInfo
    {
        public int _nPortNum { get; set; }
        public bool _bTested { get; set; }
        //减速比
        public double _dbRedRatio { get; set; }
    }

    public class axisInfo: LOGINFO
    {
        InstantDoCtrl _hOut;
        public axisInfo()
        {
            _strType = "Unknown";
            _BoardID = 0;
            _hDevice = IntPtr.Zero;
            _nIndex = UInt32.MaxValue;
            setDefaultData();
            _tTime.Interval = 500;
            _tTime.Tick += new System.EventHandler(this._tTime_Tick);           
        }

        

        public axisInfo(string strType, uint BoardID, UInt32 nIndex, ref IntPtr hDevice)
        {
            _strType = strType;
            _BoardID = BoardID;
            _hDevice = hDevice;
            _hAxisHandle = hDevice;
            _nIndex = nIndex;
            _hOut = new InstantDoCtrl();
            //_hOut.SelectedDevice = new DeviceInformation(_stuArmParamInfo._nPortNum);
            setDefaultData();
        }

        public void setDefaultData()
        {
            //_hAxisHandle = IntPtr.Zero;
            _eStatus = AxisState.STA_AX_DISABLE;
            _bEnable = false;
            _bIsSvON = false;
            _dbCmdAng = new double();
            _dbActAng = new double();
            //_dbCmdPos = new double();
            //_dbActPos = new double();
            _nDirection = 0;
            _dbCurrentVel = new double();
            _dbCurrentAccV = new double();
            _dbCurrentDecV = new double();
            _eActStatus = ActStatus.STATUS_DISABLE;

            _struParamInfo._nPPU = 3600;
            _struParamInfo._dMaxVelocity = 500;
            _struParamInfo._dMaxAccVelocity = 1000;
            _struParamInfo._dMaxDecVelocity = 1000;
            _struParamInfo._dbMinusMaxValue = 0;
            _struParamInfo._dbPlusMaxValue = 60000;
            _struParamInfo._nCurrentStatus = 0;
            _struParamInfo._dbCurrentPosition = 0;
            _struParamInfo._dbCurrentV = 0;
            _struParamInfo._bEnable = false;

            _stuArmParamInfo._bTested = false;
            _stuArmParamInfo._nPortNum = 1;
            _stuArmParamInfo._dbRedRatio = 100;

        }



        IntPtr _hDevice;
        IntPtr _hAxisHandle;
        AxisState _eStatus;
        ActStatus _eActStatus;
        public UInt16 _nDirection{ get; set; }
        public bool _bIsSvON { get; set; }

        public axisParaminfo _struParamInfo;
        public armParamInfo _stuArmParamInfo;

        public bool _bVailed { get; set; }
        public string _strType { get; set; }
        public uint _BoardID { get; set; }
        public bool _bEnable { get; set; }

        //轴编号
        public UInt32 _nIndex { get; set; }
        //板卡编号
        public UInt32 _nBoardID { get; set; }

        

        double _dbCmdPos;
        double _dbCmdAng;
        public double getCmdAng() { return _dbCmdAng; }
        public double getCmdPos() { return _dbCmdPos; }

        double _dbActPos;
        double _dbActAng;
        public double getActPos() { return _dbActPos; }
        public double getActAng() { return _dbActAng; }

        public double _dbCurrentVel;
        public double _dbCurrentAccV;
        public double _dbCurrentDecV;
               
        public void enableTimer(bool bEnable)
        {
            _tTime.Enabled = bEnable;
            if(bEnable)
            _tTime.Start();
        }

        private void _tTime_Tick(object sender, EventArgs e)
        {
            updatePosition();
        }

        System.Windows.Forms.Timer _tTime = new System.Windows.Forms.Timer();        

        public void setParamInfo(ref axisParaminfo vInfo)
        {
            _struParamInfo = vInfo;
            _hOut.SelectedDevice = new DeviceInformation(_stuArmParamInfo._nPortNum);
        }

        public bool setActStatus(ActStatus eStatus)
       {
           _eActStatus = eStatus;
            //hold on
            /*switch(eStatus)
            {
                case ActStatus.STATUS_HOLDING:
                    _eActStatus = eStatus;
                    UInt32 nRet = Motion.mAcm_AxSimStartSuspendVel(_hAxisHandle, _nDirection);
                    if (nRet  == (UInt32)ErrorCode.SUCCESS)
                    {
                        
                        return true;
                    }
                    else
                    {

                    }
                    break;
                case ActStatus.STATUS_STOPING:
                    if (Motion.mAcm_AxSimStop(_hAxisHandle) == (UInt32)ErrorCode.SUCCESS)
                    {
                        _eActStatus = eStatus;
                        return true;
                    }
                    break;
                case ActStatus.STATUS_RUNNING:
                    if (Motion.mAcm_AxSimStart(_hAxisHandle) == (UInt32)ErrorCode.SUCCESS)
                    {
                        _eActStatus = eStatus;
                        return true;
                    }
                    break;
            }

            return false;*/
           return true;
       }

        //打开轴
        public bool OpenAxis()
        {
            Thread.Sleep(10);
            
            UInt32 Result = Motion.mAcm_AxOpen(_hDevice, (UInt16)(_nIndex%4), ref _hAxisHandle);

            if (!openAxisSv())
            {
                LOG_ERROR("伺服打开失败，请检查系统！");
                return false;
            }

            Thread.Sleep(10);
            enableTimer(true);            

            if (Result == 0)
            {
                _eStatus = AxisState.STA_AX_READY;
                return true;
            }
            else
            {
                _eStatus = AxisState.STA_AX_DISABLE;
            }
            return false;
        }

        public ActStatus getActStatus() 
        {
            return _eActStatus;
        }

        //获取轴当前状态
        public void updateStatus()
        {            
            UInt16 nState = 0;
            Motion.mAcm_AxGetState(_hAxisHandle, ref nState);
            _eStatus = (AxisState)nState;           
        }

        //正常减速停止
        public UInt32 normalStopAxis()
        {
            if (_hAxisHandle == IntPtr.Zero)
            {
                return 0;
            }
            UInt32 Result = Motion.mAcm_AxStopDec(_hAxisHandle);
            if (Result ==0)
                _eStatus = AxisState.STA_AX_STOPPING;
            
            return Result;
        }

        //紧急停止
        public UInt32 emgStopAxis()
        {
            if (_hAxisHandle == IntPtr.Zero)
            {
                return 0;
            }
            UInt32 Result = Motion.mAcm_AxStopEmg(_hAxisHandle);
            if (Result == 0)
                _eStatus = AxisState.STA_AX_STOPPING;
            
            return Result;
        }

        //关闭轴
        public bool closeAxis()
        {
            if (_hAxisHandle == IntPtr.Zero)
            {
                return true;
            }

           

            if (!closeAxisSv())
            {
                LOG_ERROR("关闭伺服失败");
                return false;
            }

            UInt32 Result = Motion.mAcm_AxClose(ref _hAxisHandle);
            if (Result ==0)
            {
                _eStatus = AxisState.STA_AX_DISABLE;
                enableTimer(false);
                return true;
            }
                        
            return true;
        }

        //打开伺服
        public bool openAxisSv()
        {
            if (_hAxisHandle == IntPtr.Zero || _bIsSvON)
            {
                _bIsSvON = true;
                return true;
            }
            UInt32 Result = Motion.mAcm_AxSetSvOn(_hAxisHandle, 1);
            if (Result ==0)
            {
                _bIsSvON = true;
                LOG_INFO("打开伺服成功， 10秒后打开继电器模块");
                return true;
               
            }
            return false;
        }

        //关闭伺服
        public bool closeAxisSv()
        {
            if (_hAxisHandle == IntPtr.Zero || !_bIsSvON)
            {
                _bIsSvON = false;
                return true;
            }
          
            UInt32 Result = Motion.mAcm_AxSetSvOn(_hAxisHandle, 0);
            if(Result == 0)
            {
                _bIsSvON = false;
                return true;
            }
            return false;
        }

        public bool openPort()
        {
            byte[] bufferForWriting = new byte[1];
            //_hOut.SelectedDevice = new DeviceInformation(_stuArmParamInfo._nPortNum);

            if (!_hOut.Initialized)
            {
                return false;
            }

            bufferForWriting[0] = 1;

            if (_hOut.Write(_stuArmParamInfo._nPortNum, 1, bufferForWriting) != Automation.BDaq.ErrorCode.Success)
            {
                LOG_FAT("打开端口" + _stuArmParamInfo._nPortNum + "失败");
                return false;
            }
            return true;
        }

        public bool isPortOpenClose()
        {
            byte[] bufferForReading = new byte[1];
            _hOut.Read((int)_stuArmParamInfo._nPortNum, (int)1, bufferForReading);
            if (bufferForReading[0] != 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool closePort()
        {
            byte[] bufferForWriting = new byte[1];
            bufferForWriting[0] = 0;
            if (_hOut.Write(_stuArmParamInfo._nPortNum, 1, bufferForWriting) != Automation.BDaq.ErrorCode.Success)
            {
                LOG_FAT("关闭端口" + _stuArmParamInfo._nPortNum + "失败");
                return false;
            }
            return true;
        }

        public byte getPortValue()
        {
            byte[] bufferForReading = new byte[1];
            _hOut.Read(_stuArmParamInfo._nPortNum, 1, bufferForReading);
            return bufferForReading[0];
        }

        //移动到绝对点位置
        /*public UInt32 moveToRelPos(double dbPosition)
        {
            UInt32 Result = Motion.mAcm_AxMoveRel(_hAxisHandle, dbPosition * _struParamInfo._nPPU);

            if(Result == (UInt32)ErrorCode.SUCCESS)
            {
                _dbAbsPos += (dbPosition * _struParamInfo._nPPU - _dbRelPos);
                _dbRelPos = dbPosition;
            }

            return Result;
        }

        //移动到相对点位置
        public UInt32 moveToAbsPos(double dbPosition)
        {
            UInt32 Result = Motion.mAcm_AxMoveAbs(_hAxisHandle, dbPosition * _struParamInfo._nPPU);
            if (Result == (UInt32)ErrorCode.SUCCESS)
            {
                _dbRelPos += (dbPosition * _struParamInfo._nPPU - _dbAbsPos);
                _dbAbsPos = dbPosition * _struParamInfo._nPPU;
            }

            return Result;
        }*/

        public double getPosByAng(double dbAng)
        {
            return (dbAng / 360) * 10000 * _stuArmParamInfo._dbRedRatio;
        }

        public bool MoveAheadToAng(double dbTargetAng)
        {
            return MoveAheadToPos(getPosByAng(dbTargetAng));                        
        }

        public bool MoveAheadToPos(double dbTargetPos)
        {
            /*if(dbTargetPos > _struParamInfo._dbPlusMaxValue || dbTargetPos < _struParamInfo._dbMinusMaxValue)
            {
                LOG_ERROR("目标位置:" + dbTargetPos + "超过限制");
                return false;
            }*/
            if (Motion.mAcm_AxMoveAbs(_hAxisHandle, dbTargetPos) ==0)
            {
                return true;
            }
            return false;
        }

        //获取理论位置
        public void updatePosition()
        {
            bool bResult = true;
            double dbPos = new double();
            UInt32 Result = Motion.mAcm_AxGetCmdPosition(_hAxisHandle, ref dbPos);
            if (Result ==0)
            {
                _dbCmdPos = dbPos;
                _dbCmdAng = _dbCmdPos *360/(10000 * _stuArmParamInfo._dbRedRatio);
            }
            else
            {
                bResult = false;
            }
            

            Result = Motion.mAcm_AxGetActualPosition(_hAxisHandle, ref dbPos);

            if (Result == 0)
            {
                _dbActPos = dbPos;
                _dbActAng = _dbActPos * ((double)360 /(double)(_struParamInfo._nPPU));
            }

            updateStatus();

            if (!bResult)
            {
                LOG_WARNING("获取位置错误");
            }
        }

        //设置理论位置
        public UInt32 dbCmdPosition(double dbPos)
        {            
            UInt32 Result = Motion.mAcm_AxSetCmdPosition(_hAxisHandle, dbPos);
            if (Result == 0)
            {
                _dbCmdPos = dbPos;
            }
            return Result;
        }

        
        public UInt32 resetAxisVel(double nVel, double nAcc, double nDec)
        {
            if (nVel < _struParamInfo._dMaxVelocity && nAcc < _struParamInfo._dMaxAccVelocity && nDec < _struParamInfo._dMaxDecVelocity)
            {               
                UInt32 Result = Motion.mAcm_AxChangeVelEx(_hAxisHandle, nVel, nAcc, nDec);

                if(Result == 0)
                {
                    _dbCurrentAccV = nAcc;
                    _dbCurrentDecV = nDec;
                    _dbCurrentVel = nVel;
                    return 0;
                }
            }
            return 255;
        }

    }
}

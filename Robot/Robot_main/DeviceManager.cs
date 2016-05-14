using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Advantech.Motion;
using Automation.BDaq;
using System.Threading;
using System.Xml;
using System.IO;
using System.Windows.Forms;


namespace Robot_main
{
    
    public class DeviceManager: LOGINFO
    {
        public struct armInfo
        {
            public double dbL3_;
            public double dbL4_;
            public double dbL6 ;
            public double dbL11;
            public double dbL22;
        }

        public const int DEVICE_STATUS = 0;

        DEV_LIST[] _CurAvailableDevs;
        IntPtr[] _DevHandle;

        List<axisInfo> _axisList = new List<axisInfo>();
        InstantDoCtrl instantDoCtrl = new InstantDoCtrl();
        
        InstantDiCtrl instantDiCtrl = new InstantDiCtrl();

 

        string FileName = "axis_config.xml";
        public string _StepFileName { get; set; }
        XmlDocument _myXmlDoc = null;
        
        public StepInfoManager _stepManager { get; set; }
        public uint _nDeviceCount = 0;
        public uint _nAxisCount = 0;
        public armInfo _armInfo = new armInfo();        


        public void closeDevice()
        {
            for (int i = 0; i < _axisList.Count; i++)
            {
                if (_axisList[i].normalStopAxis() != 0)
                {
                    LOG_ERROR("第" + i.ToString() + "轴停止失败");
                }
            }
        }        

        //初始化继电器card 
        bool initCard_1761()
        {
            instantDoCtrl.SelectedDevice = new DeviceInformation(0);
            instantDiCtrl.SelectedDevice = new DeviceInformation(0);
           if(instantDoCtrl.Initialized && 
                instantDiCtrl.Initialized)
            {
                LOG_INFO("1761 in/OUT init OK" + instantDiCtrl.PortCount.ToString() + ":" + instantDoCtrl.PortCount.ToString());
            }
           else
            {
                LOG_FAT("1761 init error!");
            }
            return true;
        }

        public bool initDevice()
        {
            _CurAvailableDevs = new DEV_LIST[Motion.MAX_DEVICES];
            _DevHandle = new IntPtr[Motion.MAX_DEVICES];
            _stepManager = new StepInfoManager();

            _armInfo.dbL3_ = 700;
            _armInfo.dbL4_ = 850;
            _armInfo.dbL6 = 300;
            _armInfo.dbL11 = 335;
            _armInfo.dbL22 = 47;

            if (File.Exists(FileName))
            {
                _myXmlDoc = new XmlDocument();
                _myXmlDoc.Load(FileName);
            }

            if (DEVICE_STATUS == 1)
            {
                int result = Motion.mAcm_GetAvailableDevs(_CurAvailableDevs, Motion.MAX_DEVICES, ref _nDeviceCount);
                if (result != (int)Advantech.Motion.ErrorCode.SUCCESS)
                {
                    LOG_FAT("设备检测错误", true);
                    return false;
                }
                initCard_1761();
            }
            else
                _nDeviceCount = 2;
            LOG_INFO("硬件初始化成功");

            uint i = 0;
            for (i = 0; i < _nDeviceCount; i++)
            {               
                InitMCard(_CurAvailableDevs[i].DeviceNum, i);
            }
        
            _nAxisCount = (uint)_axisList.Count;           
                       
            return LoadConfig();
            
        }

        void  getDeviceNumByID(UInt32 nDevID, ref uint nDevNum, ref string strType)
        {
            if(DEVICE_STATUS == 0)
            {
                strType = "PCI1245E";
                nDevNum = 4;
                return;
            }
            uint uDeviceTypet = nDevID >> 24 & 0xff;

            switch (uDeviceTypet)
            {
                case (ushort)DevTypeID.PCI1245:
                    strType = "PCI1245";
                    nDevNum = 4;
                    break;
                case (ushort)DevTypeID.PCI1265:
                    strType = "PCI1265";
                    nDevNum = 6;
                    break;
                case (ushort)DevTypeID.PCI1245E:
                    strType = "PCI1245E";
                    nDevNum = 4;
                    break;
                case (ushort)DevTypeID.PCI1245V:
                    strType = "PCI1245V";
                    nDevNum = 4;
                    break;
                case (ushort)DevTypeID.PCI1245L:
                    strType = "PCI1245L";
                    nDevNum = 4;
                    break;
                case (ushort)DevTypeID.PCI1285:
                    strType = "PCI1285";
                    nDevNum = 8;
                    break;
                case (ushort)DevTypeID.PCI1285E:
                    strType = "PCI1285E";
                    nDevNum = 8;
                    break;
                case (ushort)DevTypeID.PCI1245S:
                    strType = "PCI1245S";
                    nDevNum = 4;
                    break;
                default:
                    strType = "Unknown Device";
                    nDevNum = 0;
                    break;
            }

            if (nDevNum == 0)
            {
                LOG_FAT("设备没有可用轴", true);
            }
        }

        void CloseDevice(IntPtr hHandle)
        {
            if (hHandle != IntPtr.Zero)
            {
                Motion.mAcm_DevClose(ref hHandle);
            }
        }

        bool InitMCard(UInt32 nDevID, uint nBoard)
        {
            string strType = "";
            uint nDevNum = 0;
            getDeviceNumByID(nDevID, ref nDevNum, ref strType);

            IntPtr hDevHandle = IntPtr.Zero;
            if (DEVICE_STATUS == 1)
            {
                UInt32 ret = Motion.mAcm_DevOpen(nDevID, ref hDevHandle);
                if (ret != 0)
                {
                    LOG_FAT("打开设备失败：名称：" + strType + "ID=" + nDevID.ToString(), true);
                    return false;
                }


                uint axisCount = 0;
                uint BufferLength = 4;

                ret = Motion.mAcm_GetProperty(hDevHandle, (uint)PropertyID.FT_DevAxesCount, ref axisCount, ref BufferLength);
                if (ret != 0)
                {
                    LOG_FAT(strType + "ID=" + nDevID.ToString() + "获取设备参数失败", true);
                    CloseDevice(hDevHandle);
                    return false;
                }

                //检查从设备
                uint[] slaveDevs = new uint[16];
                uint asixTotal = axisCount;
                BufferLength = 64;
                ret = Motion.mAcm_GetProperty(hDevHandle, (uint)PropertyID.CFG_DevSlaveDevs, slaveDevs, ref BufferLength);
                if (ret == 0)
                {
                    int w = 0;
                    while (slaveDevs[w] != 0)
                    {
                        asixTotal += axisCount;
                        w++;
                    }
                }

                if (asixTotal != nDevNum)
                {
                    LOG_WARNING(strType + "拥有" + nDevNum.ToString() + "轴，实际有效轴数为：" + asixTotal.ToString(), true);
                    nDevNum = asixTotal;
                }
            }


             //依次打开各轴
             for (uint j = 0; j < nDevNum; j++)
            { 
                axisInfo vInfo = new axisInfo(strType, nBoard, j + nBoard * 4, ref hDevHandle);
                _axisList.Add(vInfo);
            }
            return true;
        }

        public int getAMIndexByAxisIndex(uint nIndex)
        {            
            uint nIndexSum = 0;

            for (int i = 0; i < _nDeviceCount; i++)
            {
                nIndexSum += _CurAvailableDevs[i].DeviceNum;
                if (nIndex < nIndexSum )
                {
                    return i;
                }                
            }

            LOG_ERROR("轴"+ nIndex.ToString() + "越界！");
            return -1;
        }
            

        public axisInfo getAxisInfo(uint nIndex)
        {
            //检查该index 属于哪个板卡
            if(nIndex < _axisList.Count)
            {
                return _axisList[(int)nIndex];
            }

            return new axisInfo();
        }
        
        public bool LoadConfig()
        {
            //加载轴配置信息
            string strConfigErr = "配置文件数据错误";

            if (_myXmlDoc != null)
            { 
                XmlNode rootNode = _myXmlDoc.SelectSingleNode("设备信息");
                if (rootNode == null || rootNode.ChildNodes.Count < 1)
                {
                    LOG_ERROR(strConfigErr,  true);
                    return false;
                }

                XmlNode armNode = rootNode.SelectSingleNode("臂信息");
                if(armNode != null)
                {
                    XmlAttributeCollection attributeCol = armNode.Attributes;

                    List<string> values = new List<string>();
                    foreach (XmlAttribute attri in attributeCol)
                    {
                        values.Add(attri.Value);
                    }

                    _armInfo.dbL3_ = double.Parse(values[0]);
                    _armInfo.dbL4_ = double.Parse(values[1]);
                    _armInfo.dbL6 = double.Parse(values[2]);
                    _armInfo.dbL11 = double.Parse(values[3]);
                    _armInfo.dbL22 = double.Parse(values[4]);

                }

                XmlNodeList deviceNodes = rootNode.SelectNodes("设备");               

                for(int i = 0; i< deviceNodes.Count; i++)
                {
                    XmlNode axisNodes = deviceNodes[i].SelectSingleNode("轴信息");
                    if (axisNodes == null || axisNodes.ChildNodes.Count < 1)
                    {
                        LOG_ERROR(strConfigErr, true);                       
                        return false;
                    }

                    if (!readAxisInfo(ref axisNodes, i))
                    {
                    }
                }                
            }

            LOG_INFO("配置文件加载成功");
           
            return true;
        }

        bool readAxisInfo(ref XmlNode rootNodes, int nCardIndex)
        {
            uint nIndex = 0;
            bool bRet = true;
            int ncount = 0;
            foreach (XmlNode node in rootNodes)
            {
                XmlAttributeCollection attributeCol = node.Attributes;

                List<string> values = new List<string>();
                foreach (XmlAttribute attri in attributeCol)
                {
                    values.Add(attri.Value);
                }

                if (values.Count != 14)
                {
                    LOG_ERROR("配置文件数据错误", true); 
                    return false;
                }

                
                nIndex = UInt32.Parse(values[0]);
                axisInfo vInfo = getAxisInfo((uint)(nCardIndex * 4 + ncount++));
                //vInfo._nIndex = (uint)(nCardIndex * 4 + ncount++);
                vInfo._struParamInfo.node = node;
                vInfo._bVailed = bool.Parse(values[1]);
                vInfo._struParamInfo._strName = values[2];
                vInfo._struParamInfo._nSequence = int.Parse(values[3]);
                vInfo._struParamInfo._dZeroPoint = double.Parse(values[4]);
                vInfo._struParamInfo._dMaxVelocity = double.Parse(values[5]);
                vInfo._struParamInfo._dMaxAccVelocity = double.Parse(values[6]);
                vInfo._struParamInfo._dMaxDecVelocity = double.Parse(values[7]);
                vInfo._struParamInfo._nPPU = UInt32.Parse(values[8]);
                vInfo._struParamInfo._dbPlusMaxValue = UInt32.Parse(values[9]);
                vInfo._struParamInfo._dbMinusMaxValue = Int32.Parse(values[10]);                         
                vInfo._stuArmParamInfo._nPortNum = int.Parse(values[11]);
               // vInfo._stuArmParamInfo._bTested = bool.Parse(values[11]);
                vInfo._stuArmParamInfo._dbRedRatio = double.Parse(values[13]);

                LOG_INFO(vInfo._struParamInfo._strName + "参数加载成功");
                 if(!vInfo._stuArmParamInfo._bTested)
                {
                    bRet = false;
                }
            }

            return bRet;
        }

        public void writeAxisInfo(ref axisInfo vinfo)
        {
            XmlElement axisNode = (XmlElement)(vinfo._struParamInfo.node);

            axisNode.SetAttribute("启用", vinfo._bVailed.ToString());
            axisNode.SetAttribute("名称", vinfo._struParamInfo._strName.ToString());
            axisNode.SetAttribute("序列", vinfo._struParamInfo._nSequence.ToString());
            axisNode.SetAttribute("零点数值", vinfo._struParamInfo._dZeroPoint.ToString());
            axisNode.SetAttribute("轴最大加速度", vinfo._struParamInfo._dMaxAccVelocity.ToString());
            axisNode.SetAttribute("轴最大减速度", vinfo._struParamInfo._dMaxDecVelocity.ToString());
            axisNode.SetAttribute("PPU", vinfo._struParamInfo._nPPU.ToString());
            axisNode.SetAttribute("正向最大值", vinfo._struParamInfo._dbPlusMaxValue.ToString());
            axisNode.SetAttribute("负向最大值", vinfo._struParamInfo._dbMinusMaxValue.ToString());
            axisNode.SetAttribute("端口号", vinfo._stuArmParamInfo._nPortNum.ToString());
            axisNode.SetAttribute("验证", vinfo._stuArmParamInfo._bTested.ToString());
            axisNode.SetAttribute("减速比", vinfo._stuArmParamInfo._dbRedRatio.ToString());
            _myXmlDoc.Save(FileName);
            LOG_INFO(vinfo._struParamInfo._strName + "参数已保存。");            
        }

        public void writeArmInfo()
        {
            if(!File.Exists(FileName))
            {
                return;
            }

            _myXmlDoc.Load(FileName);
            XmlNode rootNode = _myXmlDoc.SelectSingleNode("设备信息");
            if (rootNode == null)
                return;

            XmlElement armNode = (XmlElement)(rootNode.SelectSingleNode("臂信息"));
            armNode.SetAttribute("L3_", _armInfo.dbL3_.ToString());
            armNode.SetAttribute("L4_", _armInfo.dbL4_.ToString());
            armNode.SetAttribute("L6", _armInfo.dbL6.ToString());
            armNode.SetAttribute("L11", _armInfo.dbL11.ToString());
            armNode.SetAttribute("L22", _armInfo.dbL22.ToString());
            _myXmlDoc.Save(FileName);   

        }

        public void writeStepsInfo(string strName)
        {
            if (strName.Length == 0)
            {
                LOG_ERROR("路径文件名错误！");
                return;
            }

            _StepFileName = strName;
            XmlDocument myXmlDoc = new XmlDocument();
            if(File.Exists(_StepFileName))
            {
                string strTmpName = _StepFileName + ".bak";
                if(File.Exists(strTmpName))
                {
                    File.Delete(strTmpName);
                }
                File.Move(_StepFileName, strTmpName);
            }

            XmlDeclaration dec = myXmlDoc.CreateXmlDeclaration("1.0", "utf-8", "yes");
            myXmlDoc.AppendChild(dec);
            XmlElement root = myXmlDoc.CreateElement("运行轨迹");//加入根节点
            XmlElement steps = myXmlDoc.CreateElement("步骤");
            
            for(int i = 0; i< _stepManager.getStepCount(); i++)
            {
                StepInfo vInfo  = _stepManager.getStepByIndex(i);
                XmlElement step = myXmlDoc.CreateElement("step");
                
                step.SetAttribute("编号", vInfo.nIndex.ToString());
                step.SetAttribute("Px", vInfo.dbPx.ToString());
                step.SetAttribute("Py", vInfo.dbPy.ToString());
                step.SetAttribute("Pz", vInfo.dbPz.ToString());
                step.SetAttribute("方位角", vInfo.dnAngle.ToString());
                step.SetAttribute("仰角", vInfo.dbUpAngle.ToString());
                step.SetAttribute("起始速度", vInfo.dbBeginV.ToString());
                step.SetAttribute("运行速度", vInfo.dbRunningV.ToString());
                step.SetAttribute("加速度", vInfo.dbAccV.ToString());
                step.SetAttribute("减速度", vInfo.dbDecV.ToString());
                step.SetAttribute("等待", vInfo.waitTime.ToString());
                steps.AppendChild(step);
            }
            
            root.AppendChild(steps);
            myXmlDoc.AppendChild(root);

            myXmlDoc.Save(_StepFileName);
        }       

        public void AnalyzeStep()
        {            
            for(int i = 0; i< _stepManager.getStepCount(); i++)
            {
                if(!_stepManager.getStepByIndex(i).AnalyzeStep(this))
                {
                    LOG_ERROR("No:" + i.ToString() + "步骤分解失败");
                }
            }
        }

        public bool closeAxis(uint nIndex)
        {
            closePort((int)nIndex);
            Thread.Sleep(3000);
            return getAxisInfo(nIndex).closeAxis();

        }

        public bool openAxis(int nIndex)
        {
            axisInfo vinfo =  getAxisInfo((uint)nIndex);
            vinfo.OpenAxis();
            Thread.Sleep(3000);
            openPort(vinfo._stuArmParamInfo._nPortNum);
            if (isPortOpenClose(vinfo._stuArmParamInfo._nPortNum) != true)
            {
                LOG_ERROR("open");
            }

            return true;
        }

        public bool testAxisbyIndex(int nIndex)
        {
            axisInfo vInfo = getAxisInfo((uint)nIndex);

            bool bTestResult = false;
            
            if (vInfo.OpenAxis())
            {
                if (MessageBox.Show("轴 伺服 已经打开，结果确认？", "结果确认", MessageBoxButtons.YesNo) != DialogResult.Yes)
                {
                    bTestResult = true;
                }
            }

            if (!bTestResult)
            {
                MessageBox.Show("轴打开失败， 测试退出", "错误");
                return false; 
            }

            int nDistance = 500;
            if (vInfo.MoveAheadToPos((double)nDistance))
            {
                if (MessageBox.Show("轴前行" + nDistance.ToString() + "，结果确认？", "结果确认", MessageBoxButtons.YesNo) != DialogResult.Yes)
                {
                    bTestResult = true;
                }
            }

            if (!bTestResult)
            {
                MessageBox.Show("轴前行失败， 测试退出", "错误");
                return false;
            }

            if (vInfo.MoveAheadToPos(-nDistance))
            {
                if (MessageBox.Show("轴后退" + nDistance.ToString() + "，结果确认？", "结果确认", MessageBoxButtons.YesNo) != DialogResult.Yes)
                {
                    bTestResult = true;
                }
            }

            if (!bTestResult)
            {
                MessageBox.Show("轴后退失败， 测试退出", "错误");
                return false;
            }

            if (closeAxis(vInfo._nIndex))
            {
                if (MessageBox.Show("轴关闭成功，结果确认？", "结果确认", MessageBoxButtons.YesNo) != DialogResult.Yes)
                {
                    bTestResult = true;
                }
            }

            if (!bTestResult)
            {
                MessageBox.Show("轴关闭失败， 测试退出", "错误");
                return false;
            }

            vInfo._stuArmParamInfo._bTested = true;

            return true;
        }

        public bool testSRC(int nIndex)
        {
            return (_axisList[nIndex].openAxisSv() &&
                _axisList[nIndex].openAxisSv());
        }

        public bool testAxis(int nIndex)
        {
            return (_axisList[nIndex].OpenAxis() &&
                _axisList[nIndex].OpenAxis());
        }

        public bool testPort(int nIndex)
        {           
            if (!openPort(nIndex))
                return false;

            Thread.Sleep(1000);

            if (isPortOpenClose(nIndex) != true)
                return false;

            Thread.Sleep(1000);

            if (!closePort(nIndex))
                return false;

            Thread.Sleep(1000);

            if (isPortOpenClose(nIndex) != false)
                return false;

            Thread.Sleep(10);

            return true;
        }

        public bool openPort(int nIndex)
        {
            if (!instantDoCtrl.Initialized)
            {
                MessageBox.Show("No device be selected or device open failed!", "error");
            }

            byte portData = (byte)(Math.Pow(2,nIndex));

            byte inData = 0;

            if (instantDiCtrl.Read(0, out inData) != Automation.BDaq.ErrorCode.Success)
            {
                LOG_FAT("打开端口" + nIndex.ToString() + "失败");
                return false;
            }

            portData = (byte)(portData | inData);

            if (instantDoCtrl.Write(0,  portData) != Automation.BDaq.ErrorCode.Success)
            {
                LOG_FAT("打开端口" + nIndex.ToString() + "失败");
                return false;
            }

            LOG_INFO("打开端口" + nIndex.ToString() + "成功" );
            return true;

        }

        public bool closePort(int nIndex)
        {
            if(!instantDoCtrl.Initialized)
            {
                MessageBox.Show("No device be selected or device open failed!", "error");
            }

            byte portData = 0;

            if (instantDoCtrl.Write(0,  portData) != Automation.BDaq.ErrorCode.Success)
            {
                LOG_FAT("关闭端口" + nIndex.ToString() + "失败");
                return false;
            }

            LOG_INFO("关闭端口" + nIndex.ToString() + "成功");
            return true;
        }

        public bool isPortOpenClose(int nIndex)
        {
            if (!instantDiCtrl.Initialized)
            {
                MessageBox.Show("No device be selected or device open failed!", "error");
            }
            byte portData = 0;
            instantDiCtrl.Read((int)nIndex, out portData);
            LOG_INFO("Check the port status:" + portData.ToString());

            return true;
        }        

        public void LoadStepInfo()
        {
            if(_StepFileName.Length == 0)
            {
                LOG_ERROR("路径文件名错误！");
                return;
            }
            XmlDocument myXmlDoc = new XmlDocument();
            myXmlDoc.Load(_StepFileName);
            XmlNode rootNode = myXmlDoc.SelectSingleNode("运行轨迹");
            XmlNode stepNode = rootNode.SelectSingleNode("步骤");

            XmlNodeList steps = stepNode.SelectNodes("step");
            if(steps.Count < 1)
            {
                LOG_ERROR("路径信息加载错误！");
                return;
            }
            _stepManager.resetPath();
            uint nIndex = 0;
            foreach (XmlNode node in steps)
            {
                XmlAttributeCollection attributeCol = node.Attributes;

                List<string> values = new List<string>();
                foreach (XmlAttribute attri in attributeCol)
                {
                    values.Add(attri.Value);
                }

                StepInfo vInfo = new StepInfo();
                vInfo.nIndex = nIndex;
                vInfo.dbPx = double.Parse(values[1]);
                vInfo.dbPy = double.Parse(values[2]);
                vInfo.dbPz = double.Parse(values[3]);
                vInfo.dnAngle = double.Parse(values[4]);
                vInfo.dbUpAngle = double.Parse(values[5]);
                vInfo.dbBeginV = double.Parse(values[6]);
                vInfo.dbRunningV = double.Parse(values[7]);
                vInfo.dbAccV = double.Parse(values[8]);
                vInfo.dbDecV = double.Parse(values[9]);
               
                vInfo.waitTime = uint.Parse(values[10]);

                _stepManager.addStepInfo(nIndex, vInfo);
                nIndex++;

            }
        }
    }
}

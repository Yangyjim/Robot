using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Xml;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using Advantech.Motion;

namespace Robot_main
{
    

    public partial class Robot_main : Form
    {
        public static Robot_main pMainWin = null;
        DeviceManager _deviceManage;
        axisInfo _curruentAxisInfo;
        SysRunningInfo _RunningInfo;
        LOGINFO _logInfo = new LOGINFO();
        AxisStatusPic _hPicForm = new AxisStatusPic();
        List<Int32> _LineList = new List<Int32>();


        System.Windows.Forms.DataVisualization.Charting.Series line1;

        public Robot_main()
        {
            pMainWin = this;
            InitializeComponent();
            _RunningInfo = new SysRunningInfo();
            _RunningInfo.Visible = false;         
            _hPicForm.Visible = false;
            _LineList.Add(_hPicForm.addLine("理论位置"));
            _LineList.Add(_hPicForm.addLine("实际位置"));
        }        

        private void reLoadAxisInfo()
        {
            //tabControl_ArmInfo.TabPages.Clear();            

            for (uint i = 0; i < _deviceManage._nAxisCount; i++)
            {
                axisInfo vInfo  = _deviceManage.getAxisInfo(i);
                if (!vInfo._bVailed)
                    continue;               
                TabPage vPage = new TabPage();
                vPage.Tag = vInfo;
                vPage.Text = vInfo._struParamInfo._strName;
                tabControl_ArmInfo.TabPages.Add(vPage);                
            }

            uint nIndex = 0;
            while(_deviceManage.getAxisInfo(nIndex)._struParamInfo._nSequence == 99)
            { nIndex++; }            
            groupBox_Axis1.Text = _deviceManage.getAxisInfo(nIndex)._struParamInfo._strName;
            nIndex++;
            while (_deviceManage.getAxisInfo(nIndex)._struParamInfo._nSequence == 99)
            { nIndex++; }
            groupBox_Axis2.Text = _deviceManage.getAxisInfo(nIndex)._struParamInfo._strName;
            nIndex++;
            while (_deviceManage.getAxisInfo(nIndex)._struParamInfo._nSequence == 99)
            { nIndex++; }
            groupBox_Axis3.Text = _deviceManage.getAxisInfo(nIndex)._struParamInfo._strName;
            nIndex++;
            while (_deviceManage.getAxisInfo(nIndex)._struParamInfo._nSequence == 99)
            { nIndex++; }
            groupBox_Axis4.Text = _deviceManage.getAxisInfo(nIndex)._struParamInfo._strName;
            nIndex++;
            while (_deviceManage.getAxisInfo(nIndex)._struParamInfo._nSequence == 99)
            { nIndex++; }
            groupBox_Axis5.Text = _deviceManage.getAxisInfo(nIndex)._struParamInfo._strName;
            nIndex++;
            while (_deviceManage.getAxisInfo(nIndex)._struParamInfo._nSequence == 99)
            { nIndex++; }
            groupBox_Axis6.Text = _deviceManage.getAxisInfo(nIndex)._struParamInfo._strName;

            while (tabControl_ArmInfo.TabPages.Count > _deviceManage._nAxisCount + 1)
            {
                tabControl_ArmInfo.TabPages.RemoveAt(tabControl_ArmInfo.TabPages.Count);
            } 
           
            setAxisInfoToPage(0);
        }

        private void setAxisInfoToPage(int nIndex)
        {
            if(nIndex == 0)
            {
                groupBox_AxisInfo.Visible = false;
                //update all leg to page 0
                _curruentAxisInfo = null;
                return;
            }
            
            _curruentAxisInfo = (axisInfo)(tabControl_ArmInfo.SelectedTab.Tag);
            //_curruentAxisInfo = _deviceManage.getAxisInfo((uint)nIndex);
                        
            checkBox_AxisEnable.Checked = _curruentAxisInfo._struParamInfo._bEnable;
            _curruentAxisInfo._bEnable = _curruentAxisInfo._struParamInfo._bEnable;           
            //timer_Draw.Enabled = _curruentAxisInfo._struParamInfo._bEnable;
            resetAxisChart();

            textBox_AxisMaxPValue.Text = _curruentAxisInfo._struParamInfo._dbPlusMaxValue.ToString();
            textBox_AxisMaxDValue.Text = _curruentAxisInfo._struParamInfo._dbMinusMaxValue.ToString();
            textBox_AxisPPU.Text = _curruentAxisInfo._struParamInfo._nPPU.ToString();
            textBox_AxisZero.Text = _curruentAxisInfo._struParamInfo._dZeroPoint.ToString();
            textBox_AxisMaxV.Text = _curruentAxisInfo._struParamInfo._dMaxVelocity.ToString();
            textBox_AxisMaxAV.Text = _curruentAxisInfo._struParamInfo._dMaxAccVelocity.ToString();
            textBox_AxisMaxDV.Text = _curruentAxisInfo._struParamInfo._dMaxDecVelocity.ToString();
            textBox_BoardID.Text = _curruentAxisInfo._BoardID.ToString();
            textBox_BoardType.Text = _curruentAxisInfo._strType;
            pictureBox_PortStatus.Image = (System.Drawing.Image)Properties.Resources.ResourceManager.GetObject("stop");

            switch (_curruentAxisInfo._struParamInfo._nCurrentStatus)
            {
                case 0:
                    //pictureBox_Status.Image = (System.Drawing.Image)Properties.Resources.ResourceManager.GetObject("stop");
                    break;
                case 1:
                    //pictureBox_Status.Image = (System.Drawing.Image)Properties.Resources.ResourceManager.GetObject("hold");
                    break;
                default:
                    //pictureBox_Status.Image = (System.Drawing.Image)Properties.Resources.ResourceManager.GetObject("running");
                    break;
            }
            groupBox_AxisInfo.Visible = true;
            //comboBox_AxisCurrentStatus.SelectedIndex = (Int32)_curruentAxisInfo._struParamInfo._nCurrentStatus;
            //textBox_CurrentV.Text = _curruentAxisInfo._struParamInfo._dbCurrentV.ToString();
            //textBox_AxisCurrentCmdPos.Text = _curruentAxisInfo._struParamInfo._dbCurrentPosition.ToString();            
        }

        private void Robot_main_Load(object sender, EventArgs e)
        {
            _deviceManage = new DeviceManager();
            runninginfoManager.AddInfo("开始初始化系统...");          
            if(_deviceManage.initDevice())
            {
                //showAxisConfigForm();
            }
            reLoadAxisInfo();
        }

        public void logRunningInfo(runninginfo vInfo)
        {          
            ListViewItem vItem = new ListViewItem();
            vItem.Text = vInfo._time.ToString();
            switch(vInfo._eLevel)
            {
                case eInfoLevel.LEVEL_INFO:
                    vItem.ForeColor = System.Drawing.Color.Blue;
                    break;
                case eInfoLevel.LEVEL_ERROR:
                    vItem.ForeColor = System.Drawing.Color.Red;
                    break;
                case eInfoLevel.LEVEL_WARNING:
                    vItem.ForeColor = System.Drawing.Color.Orange;
                    break;
                case eInfoLevel.LEVEL_FAT:
                    vItem.ForeColor = System.Drawing.Color.Purple;
                    break;
                default:
                    vItem.ForeColor = System.Drawing.Color.Black;
                    break;
            }
            
            vItem.SubItems.Add(vInfo._strInfo);
            _RunningInfo.AddInfo(ref vItem);
        }        

        private void button_runstep_Click(object sender, EventArgs e)
        {
            for(int i = 0; i< _deviceManage._stepManager.getStepCount(); i++)
            {
                StepInfo vStep = _deviceManage._stepManager.getStepByIndex(i);
                vStep.ProcessingStep(ref _deviceManage);
                Thread.Sleep((int)(vStep.waitTime));                 
            }
        }

        

        private void button_normalstop_Click(object sender, EventArgs e)
        {
            _deviceManage.closeDevice();
        }     

        private void menu_stepPop_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        Int32 _mousePos_x;
        Int32 _mousePos_y;
        private void listView1_MouseClick(object sender, MouseEventArgs e)
        {
            _mousePos_x = e.X;
            _mousePos_y = e.Y;

            if(listView1.SelectedItems.Count == 1)
            {
                modifyStepInfo();
            }
        }

        private void contextMenuStrip_StepPop_Opening(object sender, CancelEventArgs e)
        {
            if(listView1.SelectedItems.Count != 1)
            {
                contextMenuStrip_StepPop.Items[1].Enabled = false;
                contextMenuStrip_StepPop.Items[2].Enabled = false;
            }
            else
            {
                contextMenuStrip_StepPop.Items[1].Enabled = true;
                contextMenuStrip_StepPop.Items[2].Enabled = true;
            }
        }

        private void toolStripMenuItem_add_Click(object sender, EventArgs e)
        {
            StepInfo vInfo = new StepInfo();
            vInfo.nIndex = (uint)(listView1.Items.Count);
            _deviceManage._stepManager.addStepInfo(vInfo.nIndex, vInfo); ;
            ListViewItem vItem = new ListViewItem(vInfo.nIndex.ToString());
            setStepInfoToItem(ref vItem, ref vInfo);
            listView1.Items.Add(vItem);
        }

        private void setStepInfoToItem(ref ListViewItem vItem, ref StepInfo vInfo)
        {
            vItem.Tag = vInfo;
            vItem.SubItems.Add(vInfo.dbPx.ToString());
            vItem.SubItems.Add(vInfo.dbPy.ToString());
            vItem.SubItems.Add(vInfo.dbPz.ToString());
            vItem.SubItems.Add(vInfo.dnAngle.ToString());
            vItem.SubItems.Add(vInfo.dbUpAngle.ToString());
            vItem.SubItems.Add(vInfo.dbBeginV.ToString());
            vItem.SubItems.Add(vInfo.dbRunningV.ToString());
            vItem.SubItems.Add(vInfo.dbAccV.ToString());
            vItem.SubItems.Add(vInfo.dbDecV.ToString());
            vItem.SubItems.Add(vInfo.waitTime.ToString());            
        }

        private void updateStepInfoFromItem()
        {
            ListViewItem vItem = listView1.SelectedItems[0];
            if (vItem.Tag == null) return;
            StepInfo vInfo = (StepInfo)(vItem.Tag);
            vInfo.dbPx = double.Parse(vItem.SubItems[1].Text);
            vInfo.dbPy = double.Parse(vItem.SubItems[2].Text);
            vInfo.dbPz = double.Parse(vItem.SubItems[3].Text);
            vInfo.dnAngle = double.Parse(vItem.SubItems[4].Text);
            vInfo.dbUpAngle = double.Parse(vItem.SubItems[5].Text);
            vInfo.dbBeginV = double.Parse(vItem.SubItems[6].Text);
            vInfo.dbRunningV = double.Parse(vItem.SubItems[7].Text);
            vInfo.dbAccV = double.Parse(vItem.SubItems[8].Text);
            vInfo.dbDecV = double.Parse(vItem.SubItems[9].Text);
            vInfo.waitTime = uint.Parse(vItem.SubItems[10].Text);
        }

        private void modifyStepInfo()
        {
            ListViewItem.ListViewSubItem SelectedLSI;
            ListViewHitTestInfo i = listView1.HitTest(_mousePos_x, _mousePos_y);
            SelectedLSI = i.SubItem;
            if (SelectedLSI == null)
            {
                TxtEdit.Visible = false;
                return;
            }

            int border = 0;
            switch (listView1.BorderStyle)
            {
                case BorderStyle.FixedSingle:
                    border = 1;
                    break;
                case BorderStyle.Fixed3D:
                    border = 2;
                    break;
            }
 
            int CellWidth = SelectedLSI.Bounds.Width;
            int CellHeight = SelectedLSI.Bounds.Height;
            int CellLeft = border + listView1.Left + i.SubItem.Bounds.Left;
            int CellTop = listView1.Top + i.SubItem.Bounds.Top;
            if (i.SubItem == i.Item.SubItems[0])
                return;

            TxtEdit.Location = new Point(CellLeft, CellTop);
            TxtEdit.Size = new Size(CellWidth, CellHeight);
            TxtEdit.Visible = true;
            TxtEdit.BringToFront();
            TxtEdit.Text = i.SubItem.Text;
            TxtEdit.Select();
            TxtEdit.SelectAll();
        }

        private void toolStripMenuItem_mod_Click(object sender, EventArgs e)
        {
            modifyStepInfo();
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            modifyStepInfo();
        }        

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0)
                TxtEdit.Visible = false;
                
            /*for (int i = 0; i < listView1.Items.Count; i++ )
            {
                ListViewItem vItem = listView1.Items[i];
                getStepInfoFromItem(vItem);
                //setStepInfoToItem(ref vItem, ref vInfo);                
            }*/
           
        }

        private void TxtEdit_Leave(object sender, EventArgs e)
        {
            ListViewItem.ListViewSubItem SelectedLSI;
            ListViewHitTestInfo i = listView1.HitTest(_mousePos_x, _mousePos_y);
            SelectedLSI = i.SubItem;
            if (SelectedLSI == null)
            {
                TxtEdit.Visible = false;
                return;
            }

            i.SubItem.Text = TxtEdit.Text;           
            updateStepInfoFromItem();
        }

        

        private void tabControl_Arm1_SelectedIndexChanged(object sender, EventArgs e)
        {
            setAxisInfoToPage(tabControl_ArmInfo.SelectedIndex);            
        }      

        void EnableAxisDyanmicControl(bool bIsEnable)
        {
            //timer_Draw.Enabled = bIsEnable;
            textBox_CurrentV.Enabled = bIsEnable;
            //textBox_currentAccV.Enabled = bIsEnable;
           // textBox_currentDecV.Enabled = bIsEnable;
           // button_waitingForStart.Enabled = bIsEnable;
           // radioButton_Plus.Enabled = bIsEnable;
          //  radioButton_Op.Enabled = bIsEnable;
            goAhead.Enabled = bIsEnable;
            goBack.Enabled = bIsEnable;
            button_AxisStop.Enabled = bIsEnable;
            button_Reset.Enabled = bIsEnable;
        }


        void resetAxisChart()
        {
            line1 = new System.Windows.Forms.DataVisualization.Charting.Series(_curruentAxisInfo._struParamInfo._strName + "位置");
            line1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            line1.Points.Clear();
            //chart1.Series.Clear();
            //chart1.Series.Add(line1);
        }       

        private void button_AxisStop_Click(object sender, EventArgs e)
        {            
            _curruentAxisInfo.normalStopAxis();
        }

        private void button_EmgStop_Click(object sender, EventArgs e)
        {
            _curruentAxisInfo.emgStopAxis();
        }

        private void button_Reset_Click(object sender, EventArgs e)
        {
            numericUpDown_EndPoint.Value = (decimal)_curruentAxisInfo._struParamInfo._dZeroPoint;
        }       

        private void updateDynamicInfo()
        {
            if (checkBox_AxisEnable.Enabled )
            {
                textBox_CmdPos.Text = _curruentAxisInfo.getCmdPos().ToString();
                textBox_ActPos.Text = _curruentAxisInfo.getActPos().ToString();
                textBox_CmdAng.Text = _curruentAxisInfo.getCmdAng().ToString();
                textBox_ActAng.Text = _curruentAxisInfo.getActAng().ToString();
                textBox_CurrentV.Text = _curruentAxisInfo._dbCurrentVel.ToString();
                comboBox_status.SelectedIndex = (Int32)_curruentAxisInfo.getActStatus();

                if (_curruentAxisInfo.isPortOpenClose())
                {
                    pictureBox_PortStatus.Image = (System.Drawing.Image)Properties.Resources.ResourceManager.GetObject("running"); ;
                }
                else
                {
                    pictureBox_PortStatus.Image = (System.Drawing.Image)Properties.Resources.ResourceManager.GetObject("stop");
                }

                _hPicForm.addPoint(_LineList[0], _curruentAxisInfo.getCmdPos());
                _hPicForm.addPoint(_LineList[1], _curruentAxisInfo.getActPos());
            }
        }

        private void goAhead_Click(object sender, EventArgs e)
        {
            if (radioButton_PTP.Checked)
            {
                numericUpDown_EndPoint.Value += 100;
                _curruentAxisInfo.MoveAheadToPos((double)(numericUpDown_EndPoint.Value));
            }
            else
            {
                numericUpDown_EndPoint.Value += 1;
                _curruentAxisInfo.MoveAheadToAng((double)(numericUpDown_EndPoint.Value));
            }
        }

        private void goBack_Click(object sender, EventArgs e)
        {
            numericUpDown_EndPoint.Value = (decimal)(_curruentAxisInfo._struParamInfo._dbMinusMaxValue);
        }

        private void axisSetup_Click(object sender, EventArgs e)
        {
            showAxisConfigForm();
        }

        void showAxisConfigForm()
        {
            Form_axisInfo vAxisInfoForm = new Form_axisInfo();
            vAxisInfoForm._hDevManage = _deviceManage;
            vAxisInfoForm._eInfoType = eInfoSetup.INFO_AXIS;
            vAxisInfoForm.ShowDialog();
            tabControl_ArmInfo.SelectedIndex = 0;
            //reLoadAxisInfo();
        }

        private void buttonLoadPath_Click(object sender, EventArgs e)
        {
            OpenFileDialog vDlg = new OpenFileDialog();
            vDlg.InitialDirectory = System.IO.Directory.GetCurrentDirectory();
            vDlg.Filter = "运动轨迹文件|*.dat";
            if (vDlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                _deviceManage._StepFileName = vDlg.FileName;
                listView1.Items.Clear();
                listView_AxisSteps.Items.Clear();
                comboBox_AxisSteps.Items.Clear();
                _deviceManage.LoadStepInfo();
            }
            else
                return;

            int nStepCount = _deviceManage._stepManager.getStepCount();
            for(int i = 0; i< nStepCount; i++)
            {
                ListViewItem vItem = new ListViewItem(i.ToString());
                StepInfo vInfo = _deviceManage._stepManager.getStepByIndex(i);
                setStepInfoToItem(ref vItem, ref vInfo);
                listView1.Items.Add(vItem);
            }
        }

        private void button_SavePath_Click(object sender, EventArgs e)
        {
            TxtEdit.Visible = false;
            SaveFileDialog vDlg = new SaveFileDialog();
            vDlg.InitialDirectory = System.IO.Directory.GetCurrentDirectory();
            vDlg.Filter = "运动轨迹文件|*.dat";
            
            if(_deviceManage._StepFileName == "")
            {
                _deviceManage._StepFileName = "PathInfo_" + System.DateTime.Today.ToString() + ".dat";
            }

            vDlg.FileName = _deviceManage._StepFileName;
            if (vDlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                _deviceManage.writeStepsInfo(vDlg.FileName);
            }

        }

        private void button_CheckPath_Click(object sender, EventArgs e)
        {
            listView_AxisSteps.Items.Clear();
            comboBox_AxisSteps.Items.Clear();
            _deviceManage.AnalyzeStep();
            for(int i = 1; i<= _deviceManage._stepManager.getStepCount(); i++)
            {
                string strName = "第" + i.ToString() + "步";
                comboBox_AxisSteps.Items.Add(strName);
            }
            MessageBox.Show("路径分解完毕");
            comboBox_AxisSteps.SelectedIndex = 0;

        }

        private void button_showPic_Click(object sender, EventArgs e)
        {
            changePicForm();
        }

        private void comboBox_AxisSteps_SelectedIndexChanged(object sender, EventArgs e)
        {
            StepInfo vInfo = _deviceManage._stepManager.getStepByIndex(comboBox_AxisSteps.SelectedIndex);

            listView_AxisSteps.Items.Clear();
            for (int i = 0; i < vInfo.getAxisStepCount(); i++ )
            {
                AxisStepInfo vStep = vInfo.getAxisStepInfoByIndex(i);
                string strName = _deviceManage.getAxisInfo(vStep.getIndex())._struParamInfo._strName;
                if (strName  != "")
                {
                    strName = "第" + i.ToString() + "轴";
                }
                ListViewItem vItem = new ListViewItem();
                vItem.SubItems.Add(vStep.getEndPoint().ToString());
                vItem.SubItems.Add(_deviceManage.getAxisInfo(vStep.getIndex()).getActPos() .ToString());
                  vItem.SubItems.Add(vStep.getShowAngle().ToString());
                vItem.SubItems.Add(_deviceManage.getAxisInfo(vStep.getIndex()).getActStatus().ToString());
                vItem.Tag = vStep;
                listView_AxisSteps.Items.Add(vItem);
            }
        }

        private void RunningInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _RunningInfo.Visible = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (radioButton_PTP.Checked)
            {
                double dbEndPoint = _curruentAxisInfo.getActPos() + (double)(numericUpDown_EndPoint.Value);
                _curruentAxisInfo.MoveAheadToPos(dbEndPoint);
            }else
            {
                double dbEndAng = _curruentAxisInfo.getActAng() + (double)(numericUpDown_EndPoint.Value);
                _curruentAxisInfo.MoveAheadToAng(dbEndAng);
            }
        }

        private void timer_currentUpdate_Tick(object sender, EventArgs e)
        {
            if (_curruentAxisInfo != null)
            {
                _curruentAxisInfo.updatePosition();
                _curruentAxisInfo.updateStatus();
                updateDynamicInfo();
            }
            else
            {
                if(_deviceManage.isPortOpenClose(0))
                {

                }
            }
            
        }
      
        private void checkBox_AxisEnable_CheckedChanged_1(object sender, EventArgs e)
        {
            if(_curruentAxisInfo == null)
            {
                _logInfo.LOG_FAT("没有找到当前轴信息！");
                return;
            }

            bool bEnable = checkBox_AxisEnable.Checked;
            if(bEnable)
            {
                _deviceManage.openAxis((int)(_curruentAxisInfo._nIndex));
                //_curruentAxisInfo.OpenAxis();
                timer_currentUpdate.Enabled = true;
                groupBox_AxisControl.Enabled = true;
                radioButton_PTP.Enabled = true;
                radioButton1.Enabled = true;


                if (radioButton_PTP.Checked)
                {
                    numericUpDown_EndPoint.Value = (decimal)(_curruentAxisInfo.getCmdPos());
                }
                else
                {
                    numericUpDown_EndPoint.Value = (decimal)(_curruentAxisInfo.getCmdAng());
                }
            }
            else
            {
                timer_currentUpdate.Enabled = false;

                groupBox_AxisControl.Enabled = false;

                _deviceManage.closeAxis(_curruentAxisInfo._nIndex);                
                pictureBox_PortStatus.Image = (System.Drawing.Image)Properties.Resources.ResourceManager.GetObject("stop");
                
            }
        }

        void changePicForm()
        {
            if (_hPicForm.Visible)
            {
                button_showPic.BackColor = System.Drawing.Color.White;
                button_showPic.Text = "显示状态图";
                _hPicForm.Visible = false;
            }
            else
            {
                button_showPic.BackColor = System.Drawing.Color.Gray;
                button_showPic.Text = "隐藏状态图";
                _hPicForm.Visible = true;

                //test code
                for(int i = 0; i<500; i++)
                {
                    _hPicForm.addPoint(_LineList[0], i/5);
                    _hPicForm.addPoint(_LineList[1], i % 5);
                }

            }
        }

        private void ShowOic_ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            changePicForm();
        }

        private void button_ShowHiadeRunningInfo_Click(object sender, EventArgs e)
        {
            if(_RunningInfo.Visible)
            {
                _RunningInfo.Visible = false;
                button_ShowHiadeRunningInfo.Text = "显示运行信息";
            }
            else
            {
                _RunningInfo.Visible = true;
                button_ShowHiadeRunningInfo.Text = "隐藏运行信息";
            }

        }

    }
}

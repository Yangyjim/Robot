using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Robot_main
{
    public enum eInfoSetup
    {
        INFO_AXIS = 0,
        INFO_ARM
    };
    public partial class Form_axisInfo : Form
    {      

        public DeviceManager _hDevManage { get; set; }        
        public eInfoSetup _eInfoType { get; set; }

        public Form_axisInfo()
        {
            InitializeComponent();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            setAxisParamToWindow();        
        }

        private void Form_axisInfo_Load(object sender, EventArgs e)
        {
            if (_hDevManage._nAxisCount < 1) return;
            comboBox1.Items.Clear();

            for (int i = 0; i < _hDevManage._nAxisCount; i++)
            {
                comboBox1.Items.Add("第" + i.ToString() + "轴");
            }
            comboBox1.SelectedIndex = 0;
            setAxisParamToWindow();            
        }

        void setAxisParamToWindow()
        {
            axisInfo vInfo = _hDevManage.getAxisInfo((uint)(comboBox1.SelectedIndex));

            textBox_MaxAccV.Text = vInfo._struParamInfo._dMaxAccVelocity.ToString();
            textBox_MaxDecV.Text = vInfo._struParamInfo._dMaxDecVelocity.ToString();
            textBox_MaxPlus.Text = vInfo._struParamInfo._dbPlusMaxValue.ToString();
            textBox_MaxV.Text = vInfo._struParamInfo._dMaxVelocity.ToString();
            textBox_PPU.Text = vInfo._struParamInfo._nPPU.ToString();
            textBox_zero.Text = vInfo._struParamInfo._dZeroPoint.ToString();
            comboBox_Name.SelectedIndex = vInfo._struParamInfo._nSequence;
            textBox_name.Text = vInfo._struParamInfo._strName;
            textBox_maxMinus.Text = vInfo._struParamInfo._dbMinusMaxValue.ToString();
            //textBox_ArmDia.Text = vInfo._stuArmParamInfo._dbDia.ToString();
            textBox_ArmLen.Text = vInfo._stuArmParamInfo._nPortNum.ToString();
            textBox_RedRatio.Text = vInfo._stuArmParamInfo._dbRedRatio.ToString();

            textBox_L3_.Text = _hDevManage._armInfo.dbL3_.ToString();
            textBox_L4_.Text = _hDevManage._armInfo.dbL4_.ToString();
            textBox_L6.Text = _hDevManage._armInfo.dbL6.ToString();
            textBox_L11.Text = _hDevManage._armInfo.dbL11.ToString();
            textBox_L22.Text = _hDevManage._armInfo.dbL22.ToString();
        }

        void getAxisParamFromWindow(ref axisInfo vInfo)
        {
            vInfo._struParamInfo._strName = textBox_name.Text;
            vInfo._struParamInfo._nSequence = comboBox_Name.SelectedIndex;
            vInfo._struParamInfo._nPPU = UInt32.Parse(textBox_PPU.Text);
            vInfo._struParamInfo._dbPlusMaxValue = UInt32.Parse(textBox_MaxPlus.Text);
            vInfo._struParamInfo._dbMinusMaxValue = Int32.Parse(textBox_maxMinus.Text);
            vInfo._struParamInfo._dZeroPoint = double.Parse(textBox_zero.Text);
            vInfo._struParamInfo._dMaxVelocity = double.Parse(textBox_MaxV.Text);
            vInfo._struParamInfo._dMaxDecVelocity = double.Parse(textBox_MaxDecV.Text);
            vInfo._struParamInfo._dMaxAccVelocity = double.Parse(textBox_MaxAccV.Text);
            //vInfo._stuArmParamInfo._dbDia = double.Parse(textBox_ArmDia.Text);
            vInfo._stuArmParamInfo._nPortNum = int.Parse(textBox_ArmLen.Text);
            vInfo._stuArmParamInfo._dbRedRatio = double.Parse(textBox_RedRatio.Text);
        }
       
        private void button1_Click(object sender, EventArgs e)
        {
            axisInfo vInfo = _hDevManage.getAxisInfo((uint)(comboBox1.SelectedIndex));
            if (tabControl_Setup.SelectedIndex == 0)
            {
                getAxisParamFromWindow(ref vInfo);
                _hDevManage.writeAxisInfo(ref vInfo);
            }
            else
            {
                _hDevManage._armInfo.dbL3_ = double.Parse(textBox_L3_.Text);
                _hDevManage._armInfo.dbL4_ = double.Parse(textBox_L4_.Text);
                _hDevManage._armInfo.dbL6 = double.Parse(textBox_L6.Text);
                _hDevManage._armInfo.dbL11 = double.Parse(textBox_L11.Text);
                _hDevManage._armInfo.dbL22 = double.Parse(textBox_L22.Text);
                _hDevManage.writeArmInfo();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //退出
            //_hAxisManage.save();
        }       

        private void button_TestAxis_Click_2(object sender, EventArgs e)
        {
            int nIndex = comboBox1.SelectedIndex;
            if (!_hDevManage.testAxisbyIndex(nIndex))
            {
                MessageBox.Show("第" + nIndex.ToString() + "自检失败！", "错误");
                return;
            }                        
        }

        private void button3_Click(object sender, EventArgs e)
        {

            if (_hDevManage.testPort(0))
            {
                checkBox_Port0.Checked = true;
                pictureBox_PortStatus0.Image = (System.Drawing.Image)Properties.Resources.ResourceManager.GetObject("running");
            }
            else
            {
                checkBox_Port0.Checked = false;
                pictureBox_PortStatus0.Image = (System.Drawing.Image)Properties.Resources.ResourceManager.GetObject("stop");
            }

            if (_hDevManage.testPort(1))
            {
                checkBox_Port1.Checked = true;
                pictureBox_PortStatus1.Image = (System.Drawing.Image)Properties.Resources.ResourceManager.GetObject("running");
            }
            else
            {
                checkBox_Port1.Checked = false;
                pictureBox_PortStatus1.Image = (System.Drawing.Image)Properties.Resources.ResourceManager.GetObject("stop");
            }

            if (_hDevManage.testPort(2))
            {
                checkBox_Port2.Checked = true;
                pictureBox_PortStatus2.Image = (System.Drawing.Image)Properties.Resources.ResourceManager.GetObject("running");
            }
            else
            {
                checkBox_Port2.Checked = false;
                pictureBox_PortStatus2.Image = (System.Drawing.Image)Properties.Resources.ResourceManager.GetObject("stop");
            }

            if (_hDevManage.testPort(3))
            {
                checkBox_Port3.Checked = true;
                pictureBox_PortStatus3.Image = (System.Drawing.Image)Properties.Resources.ResourceManager.GetObject("running");
            }
            else
            {
                checkBox_Port3.Checked = false;
                pictureBox_PortStatus3.Image = (System.Drawing.Image)Properties.Resources.ResourceManager.GetObject("stop");
            }

            if (_hDevManage.testPort(4))
            {
                checkBox_Port4.Checked = true;
                pictureBox_PortStatus4.Image = (System.Drawing.Image)Properties.Resources.ResourceManager.GetObject("running");
            }
            else
            {
                checkBox_Port4.Checked = false;
                pictureBox_PortStatus4.Image = (System.Drawing.Image)Properties.Resources.ResourceManager.GetObject("stop");
            }

            if (_hDevManage.testPort(5))
            {
                checkBox_Port5.Checked = true;
                pictureBox_PortStatus5.Image = (System.Drawing.Image)Properties.Resources.ResourceManager.GetObject("running");
            }
            else
            {
                checkBox_Port5.Checked = false;
                pictureBox_PortStatus5.Image = (System.Drawing.Image)Properties.Resources.ResourceManager.GetObject("stop");
            }

            if (_hDevManage.testPort(6))
            {
                checkBox_Port6.Checked = true;
                pictureBox_PortStatus6.Image = (System.Drawing.Image)Properties.Resources.ResourceManager.GetObject("running");
            }
            else
            {
                checkBox_Port6.Checked = false;
                pictureBox_PortStatus6.Image = (System.Drawing.Image)Properties.Resources.ResourceManager.GetObject("stop");
            }

            if (_hDevManage.testPort(7))
            {
                checkBox_Port7.Checked = true;
                pictureBox_PortStatus7.Image = (System.Drawing.Image)Properties.Resources.ResourceManager.GetObject("running");
            }
            else
            {
                checkBox_Port7.Checked = false;
                pictureBox_PortStatus7.Image = (System.Drawing.Image)Properties.Resources.ResourceManager.GetObject("stop");
            }

        }

        private void button_SRVTest_Click(object sender, EventArgs e)
        {
            if (_hDevManage.testSRC(0) && _hDevManage.testAxis(0))
            {
                checkBox_Port0.Checked = true;
                pictureBox_PortStatus0.Image = (System.Drawing.Image)Properties.Resources.ResourceManager.GetObject("running");
            }
            else
            {
                checkBox_Port0.Checked = false;
                pictureBox_PortStatus0.Image = (System.Drawing.Image)Properties.Resources.ResourceManager.GetObject("stop");
            }

            if (_hDevManage.testSRC(1) && _hDevManage.testAxis(1))
            {
                checkBox_Port1.Checked = true;
                pictureBox_PortStatus1.Image = (System.Drawing.Image)Properties.Resources.ResourceManager.GetObject("running");
            }
            else
            {
                checkBox_Port1.Checked = false;
                pictureBox_PortStatus1.Image = (System.Drawing.Image)Properties.Resources.ResourceManager.GetObject("stop");
            }

            if (_hDevManage.testSRC(2) && _hDevManage.testAxis(2))
            {
                checkBox_Port2.Checked = true;
                pictureBox_PortStatus2.Image = (System.Drawing.Image)Properties.Resources.ResourceManager.GetObject("running");
            }
            else
            {
                checkBox_Port2.Checked = false;
                pictureBox_PortStatus2.Image = (System.Drawing.Image)Properties.Resources.ResourceManager.GetObject("stop");
            }

            if (_hDevManage.testSRC(3) && _hDevManage.testAxis(3))
            {
                checkBox_Port3.Checked = true;
                pictureBox_PortStatus3.Image = (System.Drawing.Image)Properties.Resources.ResourceManager.GetObject("running");
            }
            else
            {
                checkBox_Port3.Checked = false;
                pictureBox_PortStatus3.Image = (System.Drawing.Image)Properties.Resources.ResourceManager.GetObject("stop");
            }

            if (_hDevManage.testSRC(4) && _hDevManage.testAxis(4))
            {
                checkBox_Port4.Checked = true;
                pictureBox_PortStatus4.Image = (System.Drawing.Image)Properties.Resources.ResourceManager.GetObject("running");
            }
            else
            {
                checkBox_Port4.Checked = false;
                pictureBox_PortStatus4.Image = (System.Drawing.Image)Properties.Resources.ResourceManager.GetObject("stop");
            }

            if (_hDevManage.testSRC(5) && _hDevManage.testAxis(5))
            {
                checkBox_Port5.Checked = true;
                pictureBox_PortStatus5.Image = (System.Drawing.Image)Properties.Resources.ResourceManager.GetObject("running");
            }
            else
            {
                checkBox_Port5.Checked = false;
                pictureBox_PortStatus5.Image = (System.Drawing.Image)Properties.Resources.ResourceManager.GetObject("stop");
            }

            if (_hDevManage.testSRC(6) && _hDevManage.testAxis(6))
            {
                checkBox_Port6.Checked = true;
                pictureBox_PortStatus6.Image = (System.Drawing.Image)Properties.Resources.ResourceManager.GetObject("running");
            }
            else
            {
                checkBox_Port6.Checked = false;
                pictureBox_PortStatus6.Image = (System.Drawing.Image)Properties.Resources.ResourceManager.GetObject("stop");
            }

            if (_hDevManage.testSRC(7) && _hDevManage.testAxis(7))
            {
                checkBox_Port7.Checked = true;
                pictureBox_PortStatus7.Image = (System.Drawing.Image)Properties.Resources.ResourceManager.GetObject("running");
            }
            else
            {
                checkBox_Port7.Checked = false;
                pictureBox_PortStatus7.Image = (System.Drawing.Image)Properties.Resources.ResourceManager.GetObject("stop");
            }

        }


    }
}

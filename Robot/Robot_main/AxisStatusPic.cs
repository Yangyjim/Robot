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
  
    public partial class AxisStatusPic : Form
    {
        List<LineInfo> _LineList = new List<LineInfo>();

        public AxisStatusPic()
        {
            InitializeComponent();
        }

        public int addLine(string strName)
        {
            LineInfo  line = new LineInfo(strName);
            _LineList.Add(line);
            chart_currentAxis.Series.Add(line._hLine);
            return _LineList.Count;
        }

        public void addPoint(Int32 nIndex, double dbValue)
        {
            _LineList[nIndex - 1].addPoint(nIndex);
        }
    }

    public class LineInfo
    {
        public LineInfo(string strName)
        {
            _strName = strName;
            _hLine = new System.Windows.Forms.DataVisualization.Charting.Series(_strName);
            _hLine.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.StepLine;
        }

        public void addPoint(double dbValue)
        {
            _hLine.Points.Add(dbValue);
        }
        public string _strName;
        public System.Windows.Forms.DataVisualization.Charting.Series _hLine;
    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Robot_main
{
    public class AxisStepInfo: LOGINFO
    {
        UInt32 _axisIndex = 0;
        double _dbEndPoint = 0;
        double _dbAngle = 0;
        double _dbShowAngle = 0;

        public AxisStepInfo(UInt32 nIndex)
        {
            _axisIndex = nIndex;
        }

        public UInt32 getIndex()
        {
            return _axisIndex;
        }

        public double getAngle()
        {
            return _dbAngle;
        }

        public double getEndPoint()
        {
            return _dbEndPoint;
        }

        public double getShowAngle()
        {
            return _dbShowAngle;
        }

        public bool setValue(ref axisInfo hAxis, double dbAngle)
        {
            double dbRangeLow = hAxis._struParamInfo._dbMinusMaxValue * Math.PI/180;
            double dbRangeHigh = hAxis._struParamInfo._dbPlusMaxValue * Math.PI/180;
            if (dbAngle > dbRangeHigh)
            {
                dbAngle -= 2 * Math.PI;

            }
            else if (dbAngle < dbRangeLow)
            {
                dbAngle += 2 * Math.PI;
            }

            _dbAngle = dbAngle;

            if (dbAngle > dbRangeHigh || dbAngle < dbRangeLow)
            {
                LOG_ERROR("轴目标点" + dbAngle.ToString() + "越界" + dbRangeLow.ToString() + ":" + dbRangeHigh.ToString());
                return false;
            }

            _dbShowAngle = dbAngle * 180 / Math.PI;
            _dbEndPoint = _dbAngle * 12800000 / (hAxis._stuArmParamInfo._dbRedRatio * 2 * Math.PI);

            LOG_INFO("计算目标点成功, Angle = " + _dbShowAngle.ToString() + ", 终点脉冲数:" + _dbEndPoint.ToString());
            return true;

        }        
    }



    public class StepInfo: LOGINFO
    {
        UInt32 _Index = 0;        
        List<AxisStepInfo> _AxisStepInfo;


        public StepInfo()
        {            
            _AxisStepInfo = new List<AxisStepInfo>();
        }

        public int getAxisStepCount() { return _AxisStepInfo.Count; }

        public AxisStepInfo getAxisStepInfoByIndex(int nIndex) 
        {
            return _AxisStepInfo[nIndex];
        }

        public UInt32 nIndex
        {
            get
            {
                return _Index;
            }
            set
            {
                _Index = value;
            }
        }
    
        double _dbPx = 1;
        public double dbPx
        {
            get
            {
                return _dbPx;
            }
            set
            {
                _dbPx = value;
            }
        }
        double _dbPy = 1;
        public double dbPy
        {
            get
            {
                return _dbPy;
            }
            set
            {
                _dbPy = value;
            }
        }
        double _dbPz = 1;
        public double dbPz
        {
            get
            {
                return _dbPz;
            }
            set
            {
                _dbPz = value;
            }
        }

        double _dbAngle = 0;
        public double dnAngle
        {
            get
            {
                return _dbAngle;
            }
            set
            {
                _dbAngle = value;
            }
        }

        double _dbUpAngle = 0;
        public double dbUpAngle
        {
            get
            {
                return _dbUpAngle;
            }
            set
            {
                _dbUpAngle = value;
            }
        }

        double _dbBeginV = 5;
        public double dbBeginV
        {
            get
            {
                return _dbBeginV;
            }
            set
            {
                _dbBeginV = value;
            }
        }
        double _dbRunningV = 5;
        public double dbRunningV
        {
            get
            {
                return _dbRunningV;
            }
            set
            {
                _dbRunningV = value;
            }
        }
        double _dbAccV = 1;
        public double dbAccV
        {
            get
            {
                return _dbAccV;
            }
            set
            {
                _dbAccV = value;
            }
        }
        double _dbDecV = 1;
        public double dbDecV
        {
            get
            {
                return _dbDecV;
            }
            set
            {
                _dbDecV = value;
            }
        }
        uint _waitTime = 5;
        public uint waitTime
        {
            get
            {
                return _waitTime;
            }
            set
            {
                _waitTime = value;
            }
        }        

        public static bool isEqual(StepInfo p, StepInfo s)
        {
            if (p.dbAccV.Equals(s.dbAccV) &&
                p.dbPx.Equals(s.dbPx) &&
                p.dbPy.Equals(s.dbPy) &&
                p.dbPz.Equals(s.dbPz) &&
                p.dbDecV.Equals(s.dbDecV) &&
                p.dbRunningV.Equals(s.dbRunningV) &&
                p.waitTime.Equals(s.waitTime))
                return true;

            return false;
        }

        public bool ProcessingStep(ref DeviceManager vDevM)
        {
            for (int i = 0; i < _AxisStepInfo.Count; i++)
            {
                axisInfo hAxis = vDevM.getAxisInfo(_AxisStepInfo[i].getIndex());
                hAxis.MoveAheadToPos(_AxisStepInfo[i].getEndPoint());
            }
            return true;
        }

        public bool AnalyzeStep(DeviceManager vDevM)
        {
            _AxisStepInfo.Clear();
            AxisStepInfo vInfo1 = new AxisStepInfo(0);
            AxisStepInfo vInfo2 = new AxisStepInfo(1);
            AxisStepInfo vInfo3 = new AxisStepInfo(2);
            AxisStepInfo vInfo4 = new AxisStepInfo(3);
            AxisStepInfo vInfo5 = new AxisStepInfo(4);
            AxisStepInfo vInfo6 = new AxisStepInfo(5);
            
            double dbTmpAngle = _dbAngle * Math.PI / 180;

            double dbL4 = Math.Pow((Math.Pow(vDevM._armInfo.dbL4_, 2) + Math.Pow(vDevM._armInfo.dbL22, 2)), 0.5);
            double db_d = vDevM._armInfo.dbL6 * Math.Cos(dbTmpAngle) +  _dbPz;
            double db_X_;
            if (_dbPx > 0)
                db_X_ = _dbPx - vDevM._armInfo.dbL11 + vDevM._armInfo.dbL6 * Math.Sin(dbTmpAngle);
            else
                db_X_ = _dbPx + vDevM._armInfo.dbL11 - vDevM._armInfo.dbL6 * Math.Sin(dbTmpAngle);

            double db_E = Math.Pow(db_X_, 2) + Math.Pow(db_d, 2);
            double db_D = (db_X_ * db_X_ + Math.Pow(vDevM._armInfo.dbL3_, 2) - Math.Pow(vDevM._armInfo.dbL4_, 2) - db_d * db_d)/2;
            double db_G = Math.Pow(db_D, 2) + Math.Pow(db_d, 2) * (Math.Pow(db_X_, 2) - Math.Pow(vDevM._armInfo.dbL4_, 2));
            double db_F = (Math.Pow(db_d, 2) + db_D) * db_X_;
            
            double db_a = (db_F - (db_X_/Math.Abs(db_X_)) * Math.Pow(((db_F * db_F) - (db_E * db_G)), 0.5)) / db_E;

            

            double db_c = db_X_ - db_a; 


            double db_b = Math.Pow((Math.Pow(vDevM._armInfo.dbL3_, 2) - Math.Pow(db_a, 2)), 0.5) - db_d;

            LOG_DEBUG("计算结果：方位角=" + dbTmpAngle.ToString() +  ", _d =" + db_d.ToString() + 
                ", L4 = "+dbL4.ToString() + ", _X = " + db_X_.ToString() + ", _E = " + db_E.ToString() + ", _D = " + db_D.ToString()
                + ", _G = " + db_G.ToString() + ", _F = "+ db_F.ToString() + ", a_ = " + db_a.ToString ()+ ", _c=" + db_c.ToString());

            axisInfo hAxis;
            double dbValue;
            //1
            hAxis = vDevM.getAxisInfo(vInfo1.getIndex());
            dbValue = Math.Acos(_dbPx / Math.Pow((Math.Pow(_dbPx, 2) + Math.Pow(_dbPy, 2)), 0.5)) + (Math.PI / 2) * (1 - _dbPx / Math.Abs(_dbPx));
            if (!vInfo1.setValue(ref hAxis, dbValue))
            {
                return false;
            }

            dbValue = Math.Atan((db_b + db_d) / (db_X_ * (1 - db_c / Math.Abs(db_X_))) + Math.PI * (1 - db_X_ / Math.Abs(db_X_)) / 2);
            hAxis = vDevM.getAxisInfo(vInfo2.getIndex());
            if (!vInfo2.setValue(ref hAxis, dbValue))
            {
                return false;
            }



            dbValue = -(vInfo2.getAngle() - Math.PI/2 + (db_X_*(Math.Atan(db_b/db_c) + Math.PI/2))/Math.Abs(db_X_));
            hAxis = vDevM.getAxisInfo(vInfo3.getIndex());
            if (!vInfo3.setValue(ref hAxis, dbValue))
            {
                return false;
            }

            dbValue = dbUpAngle  *Math.PI /180;
            hAxis = vDevM.getAxisInfo(vInfo4.getIndex());
            if (!vInfo4.setValue(ref hAxis, dbValue))
            {
                return false;
            }
            

           dbValue = -((db_X_ / Math.Abs(db_X_)) * (Math.Atan(db_c / db_d + dbTmpAngle)));
            hAxis = vDevM.getAxisInfo(vInfo5.getIndex());
            if (!vInfo5.setValue(ref hAxis, dbValue))
            {
                return false;
            }
            

            dbValue = _dbAngle *  Math.PI / 180;
            hAxis = vDevM.getAxisInfo(vInfo6.getIndex());
            if (!vInfo6.setValue(ref hAxis, dbValue))
            {
                return false;
            }

            _AxisStepInfo.Add(vInfo1);
            _AxisStepInfo.Add(vInfo2);
            _AxisStepInfo.Add(vInfo3);
            _AxisStepInfo.Add(vInfo4);
            _AxisStepInfo.Add(vInfo5);
            _AxisStepInfo.Add(vInfo6);
            LOG_INFO("分解步骤成功， 分别是：" + getAngByValue(vInfo1.getShowAngle()).ToString() + ":" + getAngByValue(vInfo2.getShowAngle()).ToString() +
               ":" + getAngByValue(vInfo3.getShowAngle()).ToString() + ":" + getAngByValue(vInfo4.getShowAngle()).ToString() + ":" +
               getAngByValue(vInfo5.getShowAngle()).ToString() + ":" + getAngByValue(vInfo6.getShowAngle()).ToString());
            return true;
        }

        double getAngByValue(double dbValue)
        {
            return dbValue * 180 / Math.PI;
        }     

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Windows.Forms;



namespace Robot_main
{
    public class LOGINFO
    {
        public void LOG_DEBUG(string strInfo, bool bPopWindow = false)
        {
            runninginfoManager.AddInfo(strInfo, eInfoLevel.LEVEL_DEBUG, bPopWindow);
        }

        public void LOG_INFO(string strInfo, bool bPopWindow = false)
        {
            runninginfoManager.AddInfo(strInfo, eInfoLevel.LEVEL_INFO, bPopWindow);
        }

        public void LOG_WARNING(string strInfo, bool bPopWindow = false)
        {
            runninginfoManager.AddInfo(strInfo, eInfoLevel.LEVEL_WARNING, bPopWindow);
        }

        public void LOG_ERROR(string strInfo, bool bPopWindow = true)
        {
            runninginfoManager.AddInfo(strInfo, eInfoLevel.LEVEL_ERROR, bPopWindow);
        }

        public void LOG_FAT(string strInfo, bool bPopWindow = true)
        {
            runninginfoManager.AddInfo(strInfo, eInfoLevel.LEVEL_FAT, bPopWindow);
        }
    }


    public class runninginfo
    {
        public DateTime _time;
        public string _strInfo;
        public eInfoLevel _eLevel;

        public runninginfo(DateTime vtime, string strInfo, eInfoLevel eLev = eInfoLevel.LEVEL_INFO)
        {
            _time = vtime;
            _strInfo = strInfo;
            _eLevel = eLev;
        }
    }

    public enum eInfoLevel
    {
        LEVEL_DEBUG = 0,
        LEVEL_INFO,
        LEVEL_WARNING,
        LEVEL_ERROR,
        LEVEL_FAT
    }

    static class runninginfoManager
    {
        static int nIndex = 0;
        static List<runninginfo> _info = new List<runninginfo>();



        static public void AddInfo(string strInfo, eInfoLevel eLev = eInfoLevel.LEVEL_INFO, bool bIsPopWindow = false)
        {
            DateTime vTime = DateTime.Now;
            runninginfo vInfo = new runninginfo(vTime, strInfo, eLev);
            _info.Add(vInfo);

            Robot_main.pMainWin.logRunningInfo(vInfo);

            if(bIsPopWindow && eLev != eInfoLevel.LEVEL_DEBUG)
            {
                string strInfoType;

                switch(eLev)
                {
                    case eInfoLevel.LEVEL_DEBUG:
                        strInfoType = "调试信息";
                        break;
                    case eInfoLevel.LEVEL_INFO:
                        strInfoType = "运行信息";
                        break;
                    case eInfoLevel.LEVEL_ERROR:
                        strInfoType = "错误信息";
                        break;
                    case eInfoLevel.LEVEL_FAT:
                        strInfoType = "致命错误";
                        break;
                    case eInfoLevel.LEVEL_WARNING:
                        strInfoType = "警告信息";
                        break;
                    default:
                        strInfoType = "运行信息";
                        break;
                }

                MessageBox.Show(strInfo, strInfoType, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }      
 
        static public bool GetNextInfo(ref runninginfo vinfo)
        {
            if(nIndex < _info.Count)
            {
                vinfo = _info[nIndex];
                nIndex++;
                return true;
            }
            return false;
        }        
    }
}

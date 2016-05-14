using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace Robot_main
{
    public class StepInfoManager
    {
        List<StepInfo> _stepList = new List<StepInfo>();        
        UInt32 _nStepArraySize = 0;

        public void addStepInfo(uint nIndex, StepInfo vInfo)
        {
            if (_nStepArraySize >= nIndex)
            {
                _stepList.Insert((int)nIndex, vInfo);
                _nStepArraySize++;
            }
        }
        public void modStepInfo(int nIndex, StepInfo vInfo)
        {
            if (_nStepArraySize > nIndex)
            {
                _stepList[nIndex] = vInfo;
            }
        }

        public void delStepInfo(int nIndex, StepInfo vInfo)
        {
            if (_nStepArraySize > nIndex)
            {
                _stepList.RemoveAt(nIndex);
                _nStepArraySize--;
            }
        }

        public void resetPath()
        {
            _stepList.Clear();
        }

        public int getStepCount()
        {
            return _stepList.Count;
        }

        public StepInfo getStepByIndex(int nIndex)
        {
            return _stepList[nIndex];
        }
    }
}

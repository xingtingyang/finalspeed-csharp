using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FinalSpeed.utils
{
    public class SpeedUnit
    {
        public int downSum;
        public int upSum;
        public SpeedUnit()
        {

        }

        public void addUp(int n)
        {
            upSum += n;
        }

        public void addDown(int n)
        {
            downSum += n;
        }
    }
}

using FinalSpeed.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FinalSpeed.cap
{
    public class CapServer
    {
        public CapServer()
        {
            CapEnv capEnv = null;
            try
            {
                capEnv = new CapEnv(false);
                capEnv.init();
            }
            catch (Exception e)
            {
                MLog.info(e.Message);
            }
        }
    }
}

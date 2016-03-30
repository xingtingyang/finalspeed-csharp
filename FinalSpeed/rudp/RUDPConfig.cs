using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FinalSpeed.rudp
{
    public class RUDPConfig
    {
        public static short protocal_ver = 0;

        public static int packageSize = 1000;

        public static bool twice_udp = false;

        public static bool twice_tcp = false;

        public static int maxWin = 5 * 1024;

        public static int ackListDelay = 5;
        public static int ackListSum = 300;

        public static bool double_send_start = true;

        public static int reSendDelay_min = 100;
        public static float reSendDelay = 0.37f;
        public static int reSendTryTimes = 10;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FinalSpeed.socket;

namespace FinalSpeed.utils
{
   public class MessageCheck
    {
        public static int checkVer(DatagramPacket dp)
        {
            int ver = ByteShortConvert.toShort(dp.getData(), 0);
            return ver;
        }
        public static int checkSType(DatagramPacket dp)
        {
            int sType = ByteShortConvert.toShort(dp.getData(), 2);
            return sType;
        }
    }
}

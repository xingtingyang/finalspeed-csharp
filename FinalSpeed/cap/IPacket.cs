using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PacketDotNet;


namespace FinalSpeed.cap
{
    public class IPacket
    {
        int index;

        int sequence;

        int legth;

        Packet packet;

        long firstSendTime;

        long sendTime;

        long reSendCount;
	
    }
}

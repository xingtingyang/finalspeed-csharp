using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace FinalSpeed.rudp
{
    public class UDPOutputStream
    {
        public ConnectionUDP conn;
        IPAddress dstIp;
        int dstPort;
        Sender sender;

        bool streamClosed = false;
        private ConnectionUDP conn1;

        public UDPOutputStream(ConnectionUDP conn)
        {
            this.conn = conn;
            this.dstIp = conn.dstIp;
            this.dstPort = conn.dstPort;
            this.sender = conn.sender;
        }


        public void write(byte[] data, int offset, int length)
        {
            sender.sendData(data, offset, length);
        }

        public void closeStream_Local()
        {
            sender.closeStream_Local();
        }
    }
}

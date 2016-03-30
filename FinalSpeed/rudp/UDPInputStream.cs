using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace FinalSpeed.rudp
{
    public class UDPInputStream
    {

        Socket ds;//UDP的socket,原文是Datagramsocket
        IPAddress dstIp;
        int dstPort;
        Receiver receiver;

        bool streamClosed = false;

        ConnectionUDP conn;

        public UDPInputStream(ConnectionUDP conn)
        {
            this.conn = conn;
            receiver = conn.receiver;
        }

        public int read(byte[] b, int off, int len)
        {
            byte[] b2 = null;
            b2 = read2();
            if (len < b2.Length)
            {
                throw new ConnectException("error5");
            }
            else
            {
                Array.Copy(b2, 0, b, off, b2.Length);
                //System.arraycopy(b2, 0, b, off, b2.length);
                return b2.Length;
            }
        }

        public byte[] read2()
        {
            return receiver.receive();
        }

        public void closeStream_Local()
        {
            if (!streamClosed)
            {
                receiver.closeStream_Local();
            }
        }
    }
}

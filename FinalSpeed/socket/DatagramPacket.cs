using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace FinalSpeed.socket
{
    /// <summary>
    /// <para>这是为了兼容原JAVA代码里的DatagramPacket才创建的类</para>
    /// <para>理论上可以不需要这个类的，后期我再改的更好一点吧</para>
    /// </summary>
    public class DatagramPacket
    {
        public byte[] buf { get; set; }
        public int offset { get; set; }
        public int length { get; set; }

        private IPAddress _ip;

        public IPAddress ip
        {
            get
            {
                return _ip;
            }
            set
            {
                _ip = value;
                address = new IPEndPoint(_ip, _port);
            }
        }

        private int _port;

        public int port
        {
            get
            {
                return _port;
            }
            set
            {
                _port = value;
                address = new IPEndPoint(_ip, _port);
            }
        }

        private IPEndPoint _address;
        private byte[] dpData;
        private int p;

        public IPEndPoint address
        {
            get
            {
                return _address;
            }
            set
            {
                _ip = _address.Address;
                _port = _address.Port;
            }
        }


        //public SocketAddress address { get; set; }

        public DatagramPacket(byte[] buf, int offset, int length, IPEndPoint address)
        {
            this.buf = buf;
            this.offset = offset;
            this.length = length;
            this.address = address;
        }

        public DatagramPacket(byte[] buf, int length)
            : this(buf, 0, length, null)
        {

        }




        public byte[] getData()
        {
            return buf.Skip(offset).Take(length).ToArray();
        }

        internal void setAddress(IPAddress dstIp)
        {
            ip = dstIp;
        }

        internal void setPort(int dstPort)
        {
            port = dstPort;
        }

        internal IPAddress getAddress()
        {
            return ip;
        }

        internal int getPort()
        {
            return port;
        }     
    }
}

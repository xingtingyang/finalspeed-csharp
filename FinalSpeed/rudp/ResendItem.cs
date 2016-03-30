using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FinalSpeed.rudp
{
    public class ResendItem
    {
        public int count;

        public ConnectionUDP conn;

        public int sequence;

        public long resendTime;

        public ResendItem(ConnectionUDP conn, int sequence)
        {
            this.conn = conn;
            this.sequence = sequence;
        }

        void addCount()
        {
            count++;
        }

        public int getCount()
        {
            return count;
        }

        public void setCount(int count)
        {
            this.count = count;
        }

        public ConnectionUDP getConn()
        {
            return conn;
        }

        public void setConn(ConnectionUDP conn)
        {
            this.conn = conn;
        }

        public int getSequence()
        {
            return sequence;
        }

        public void setSequence(int sequence)
        {
            this.sequence = sequence;
        }

        public long getResendTime()
        {
            return resendTime;
        }

        public void setResendTime(long resendTime)
        {
            this.resendTime = resendTime;
        }

    }
}

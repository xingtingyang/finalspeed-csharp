using FinalSpeed.rudp.message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FinalSpeed.rudp
{
    public class AckListTask
    {
        public ConnectionUDP conn;
        public AckListMessage alm;
        public int lastRead = 0;
        public List<int> ackList;

        public HashSet<int> set;
        public AckListTask(ConnectionUDP conn)
        {
            this.conn = conn;
            ackList = new List<int>();
            set = new HashSet<int>();
        }

        public void addAck(int sequence)
        {
            lock (this)
            {
                ////#MLog.println("sendACK "+sequence);
                if (!set.Contains(sequence))
                {
                    ackList.Add(sequence);
                    set.Add(sequence);
                }
            }
        }

        public void run()
        {
            lock (this)
            {
                int offset = 0;
                int packetLength = RUDPConfig.ackListSum;
                int length = ackList.Count;
                ////#MLog.println("ffffffffaaaaaaaaa "+length);
                int sum = (length / packetLength);
                if (length % packetLength != 0)
                {
                    sum += 1;
                }
                if (sum == 0)
                {
                    sum = 1;
                }
                int len = packetLength;
                if (length <= len)
                {
                    conn.sender.sendALMessage(ackList);
                    conn.sender.sendALMessage(ackList);
                }
                else
                {
                    for (int i = 0; i < sum; i++)
                    {
                        List<int> nl = copy(offset, len, ackList);
                        conn.sender.sendALMessage(nl);
                        conn.sender.sendALMessage(nl);
                        //				conn.sender.sendALMessage(nl);
                        //				conn.sender.sendALMessage(nl);
                        //				conn.sender.sendALMessage(nl);
                        offset += packetLength;
                        ////#MLog.println("fffffffffa "+nl.size());
                        if (offset + len > length)
                        {
                            len = length - (sum - 1) * packetLength;
                        }
                    }
                }
            }
        }

        public List<int> copy(int offset, int length, List<int> ackList)
        {
            List<int> nl = new List<int>();
            for (int i = 0; i < length; i++)
            {
                nl.Add(ackList[offset + i]);
            }
            return nl;
        }
    }

}

using FinalSpeed.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace FinalSpeed.rudp
{
    public class AckListManage
    {
        Thread mainThread;
        Dictionary<int, AckListTask> taskTable;
        public AckListManage()
        {
            taskTable = new Dictionary<int, AckListTask>();
            mainThread = new Thread(run);
            mainThread.Start();
        }

        void addAck(ConnectionUDP conn, int sequence)
        {
            lock (this)
            {
                if (!taskTable.ContainsKey(conn.connectId))
                {
                    AckListTask at = new AckListTask(conn);
                    taskTable.Add(conn.connectId, at);
                }
                AckListTask at1 = taskTable[conn.connectId];
                at1.addAck(sequence);
            }
        }

        void addLastRead(ConnectionUDP conn)
        {
            lock (this)
            {
                if (!taskTable.ContainsKey(conn.connectId))
                {
                    AckListTask at = new AckListTask(conn);
                    taskTable.Add(conn.connectId, at);
                }
            }
        }

        public void run()
        {
            while (true)
            {
                lock (this)
                {
                    //Iterator<Integer> it = taskTable.Keys.iterator();
                    //while (it.hasNext())
                    //{
                    //    int id = it.next();
                    //    AckListTask at = taskTable[id];
                    //    at.run();
                    //}
                    //用foreach来代替迭代器
                    foreach(AckListTask at in taskTable.Values)
                    {
                        at.run();
                    }

                    taskTable.Clear();
                    taskTable = null;
                    taskTable = new Dictionary<int, AckListTask>();
                }

                try
                {
                    Thread.Sleep(RUDPConfig.ackListDelay);
                }
                catch (Exception e)
                {
                    MLog.info(e.Message);
                }
            }
        }
    }
}

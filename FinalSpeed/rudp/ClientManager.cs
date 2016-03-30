using FinalSpeed.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;

namespace FinalSpeed.rudp
{
    public class ClientManager
    {
        Dictionary<int, ClientControl> clientTable = new Dictionary<int, ClientControl>();

        Thread mainThread;

        Route route;

        int receivePingTimeout = 8 * 1000;

        int sendPingInterval = 1 * 1000;

        Object syn_clientTable = new Object();

        public ClientManager(Route route)
        {
            this.route = route;
            mainThread = new Thread(() =>
                {
                    while (true)
                    {
                        try
                        {
                            Thread.Sleep(1000);
                        }
                        catch (Exception e)
                        {
                            MLog.info(e.Message);
                        }
                        scanClientControl();
                    }
                }
            );
            mainThread.Start();
        }

        void scanClientControl()
        {
            //Iterator<Integer> it=getClientTableIterator();//迭代器,取ClientTable的Keys
            long current = DateTime.Now.Millisecond;
            //MLog.println("ffffffffffff "+clientTable.size());
            foreach (ClientControl cc in clientTable.Values)
            {
                if (cc != null)
                {
                    if (current - cc.getLastReceivePingTime() < receivePingTimeout)
                    {
                        if (current - cc.getLastSendPingTime() > sendPingInterval)
                        {
                            cc.sendPingMessage();
                        }
                    }
                    else
                    {
                        //超时关闭client
                        MLog.println("超时关闭client " + cc.dstIp.ToString() + ":" + cc.dstPort + " " + DateTime.Now);
                        //					System.exit(0);
                        lock (syn_clientTable)
                        {
                            cc.close();
                        }
                    }
                }
            }
            //        while(it.hasNext())
            //        {
            //            ClientControl cc=clientTable.get(it.next());
            //            if(cc!=null){
            //                if(current-cc.getLastReceivePingTime()<receivePingTimeout){
            //                    if(current-cc.getLastSendPingTime()>sendPingInterval){
            //                        cc.sendPingMessage();
            //                    }
            //                }else {
            //                    //超时关闭client
            //                    MLog.println("超时关闭client "+cc.dstIp.getHostAddress()+":"+cc.dstPort+" "+new Date());
            ////					System.exit(0);
            //                    synchronized (syn_clientTable) {
            //                        cc.close();
            //                    }
            //                }
            //            }
            //        }
        }

        void removeClient(int clientId)
        {
            clientTable.Remove(clientId);
        }

        ///不用迭代器了
        //Iterator<Integer> getClientTableIterator()
        //{
        //    Iterator<Integer> it=null;
        //    synchronized (syn_clientTable) {
        //        it=new CopiedIterator(clientTable.keySet().iterator());
        //    }
        //    return it;
        //}

        ClientControl getClientControl(int clientId, IPAddress dstIp, int dstPort)
        {
            ClientControl c = clientTable[clientId];
            if (c == null)
            {
                c = new ClientControl(route, clientId, dstIp, dstPort);
                lock (syn_clientTable)
                {
                    clientTable.Add(clientId, c);
                }
            }
            return c;
        }
    }
}

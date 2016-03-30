using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using FinalSpeed.utils;
using FinalSpeed.socket;
using FinalSpeed.rudp.message;

namespace FinalSpeed.rudp
{
    class ClientControl
    {
        public int clientId;


        public Thread sendThread;

        public Object synlock = new Object();

        private Dictionary<int, SendRecord> sendRecordTable = new Dictionary<int, SendRecord>();


        public Dictionary<int, SendRecord> sendRecordTable_remote = new Dictionary<int, SendRecord>();


        public long startSendTime = 0;

        public const int maxSpeed = (int)(1024 * 1024);

        public const int initSpeed = (int)maxSpeed;

        public int currentSpeed = initSpeed;

        public int lastTime = -1;

        public Object syn_timeid = new Object();

        public long sended = 0;

        public long markTime = 0;

        public long lastSendPingTime, lastReceivePingTime = DateTime.Now.Millisecond;

        public Random ran = new Random();

        public Dictionary<int, long> pingTable = new Dictionary<int, long>();

        public int pingDelay = 250;

        public int clientId_real = -1;

        public long needSleep_All, trueSleep_All;

        public int maxAcked = 0;

        public long lastLockTime;

        public Route route;

        public IPAddress dstIp;

        public int dstPort;

        public Dictionary<int, ConnectionUDP> connTable = new Dictionary<int, ConnectionUDP>();

        Object syn_connTable = new Object();

        Object syn_tunTable = new Object();

        String password;

        public ResendManage resendMange;

        bool closed = false;



        public ClientControl(Route route, int clientId, IPAddress dstIp, int dstPort)
        {
            resendMange = new ResendManage();
            this.clientId = clientId;
            this.route = route;
            this.dstIp = dstIp;
            this.dstPort = dstPort;
        }

        public void sendPingMessage()
        {
            int pingid = Math.Abs(ran.Next(32));
            long pingTime = DateTime.Now.Millisecond;
            pingTable.Add(pingid, pingTime);
            lastSendPingTime = DateTime.Now.Millisecond;
            PingMessage lm = new PingMessage(0, route.localclientId, pingid, Route.localDownloadSpeed, Route.localUploadSpeed);
            lm.setDstAddress(dstIp);
            lm.setDstPort(dstPort);
            try
            {
                sendPacket(lm.getDatagramPacket());
            }
            catch (Exception e)
            {
                MLog.info(e.Message);
            }
        }

        public void sendPingMessage2(int pingId, IPAddress dstIp, int dstPort)
        {
            PingMessage2 lm = new PingMessage2(0, route.localclientId, pingId);
            lm.setDstAddress(dstIp);
            lm.setDstPort(dstPort);
            try
            {
                sendPacket(lm.getDatagramPacket());
            }
            catch (Exception e)
            {
                MLog.info(e.Message);
            }
        }



        public void onReceivePacket(DatagramPacket dp)
        {
            byte[] dpData = dp.getData();
            int sType = 0;
            sType = MessageCheck.checkSType(dp);
            int remote_clientId = ByteIntConvert.toInt(dpData, 8);
            if (sType == MessageType.sType_PingMessage)
            {
                PingMessage pm = new PingMessage(dp);
                sendPingMessage2(pm.getPingId(), dp.getAddress(), dp.getPort());
                currentSpeed = pm.getDownloadSpeed() * 1024;
            }
            else if (sType == MessageType.sType_PingMessage2)
            {
                PingMessage2 pm = new PingMessage2(dp);
                lastReceivePingTime = DateTime.Now.Millisecond;
                long t = pingTable[pm.getPingId()];
                if (t != null)
                {
                    pingDelay = (int)(DateTime.Now.Millisecond - t);
                    String protocal = "";
                    if (route.isUseTcpTun())
                    {
                        protocal = "tcp";
                    }
                    else
                    {
                        protocal = "udp";
                    }
                    //MLog.println("    receive_ping222: "+pm.getPingId()+" "+new Date());
                    //MLog.println("delay_"+protocal+" "+pingDelay+"ms "+dp.getAddress().getHostAddress()+":"+dp.getPort());
                }
            }
        }

        public void sendPacket(DatagramPacket dp)
        {

            //加密

            route.sendPacket(dp);
        }

        void addConnection(ConnectionUDP conn)
        {
            lock (syn_connTable)
            {
                connTable.Add(conn.connectId, conn);
            }
        }

        void removeConnection(ConnectionUDP conn)
        {
            lock (syn_connTable)
            {
                connTable.Remove(conn.connectId);
            }
        }

        public void close()
        {
            //closed=true;
            //route.clientManager.removeClient(clientId);
            //lock (syn_connTable) {
            //    Iterator<Integer> it=getConnTableIterator();
            //    while(it.hasNext()){
            //        final ConnectionUDP conn=connTable.get(it.next());
            //        if(conn!=null){
            //            Route.es.execute(()=>{

            //                    conn.stopnow=true;
            //                    conn.destroy(true);

            //            });

            //        }
            //    }
            //}
        }

        //Iterator<Integer> getConnTableIterator()
        //{
        //    Iterator<Integer> it = null;
        //    lock (syn_connTable)
        //    {
        //        it = new CopiedIterator(connTable.keySet().iterator());
        //    }
        //    return it;
        //}

        public void updateClientId(int newClientId)
        {
            clientId_real = newClientId;
            sendRecordTable.Clear();
            sendRecordTable_remote.Clear();
        }

        public void onSendDataPacket(ConnectionUDP conn)
        {

        }



        public void onReceivePing(PingMessage pm)
        {
            if (route.mode == 2)
            {
                currentSpeed = pm.getDownloadSpeed() * 1024;
                //#MLog.println("更新对方速度: "+currentSpeed);
            }
        }

        SendRecord getSendRecord(int timeId)
        {
            SendRecord record = null;
            lock (syn_timeid)
            {
                record = sendRecordTable[timeId];
                if (record == null)
                {
                    record = new SendRecord();
                    record.setTimeId(timeId);
                    sendRecordTable.Add(timeId, record);
                }
            }
            return record;
        }

        public int getCurrentTimeId()
        {
            long current = DateTime.Now.Millisecond;
            if (startSendTime == 0)
            {
                startSendTime = current;
            }
            int timeId = (int)((current - startSendTime) / 1000);
            return timeId;
        }

        public int getTimeId(long time)
        {
            int timeId = (int)((time - startSendTime) / 1000);
            return timeId;
        }

        //纳秒
        public void sendSleep(long startTime, int length)
        {
            lock (this)
            {
                if (route.mode == 1)
                {
                    currentSpeed = Route.localUploadSpeed;
                }
                if (sended == 0)
                {
                    markTime = startTime;
                }
                sended += length;
                //10K sleep
                if (sended > 10 * 1024)
                {
                    long needTime = (long)(1000 * 1000 * 1000f * sended / currentSpeed);
                    long usedTime = DateTime.Now.Ticks * 100 - markTime;// 原文是纳秒，DateTime.Now.Ticks只能获取到100纳秒为单位
                    if (usedTime < needTime)
                    {
                        long sleepTime = needTime - usedTime;
                        needSleep_All += sleepTime;

                        long moreTime = trueSleep_All - needSleep_All;
                        if (moreTime > 0)
                        {
                            if (sleepTime <= moreTime)
                            {
                                sleepTime = 0;
                                trueSleep_All -= sleepTime;
                            }
                        }

                        long s = needTime / (1000 * 1000);
                        int n = (int)(needTime % (1000 * 1000));
                        long t1 = DateTime.Now.Ticks * 100;
                        if (sleepTime > 0)
                        {
                            try
                            {
                                Thread.Sleep((int)s);//这里可能有问题,实际上Sleep做不到纳米级别的
                            }
                            catch (Exception e)
                            {
                                MLog.info(e.Message);
                            }
                            trueSleep_All += (DateTime.Now.Ticks * 100 - t1);
                            //#MLog.println("sssssssssss "+(trueSleep_All-needSleep_All)/(1000*1000));
                        }
                        ////#MLog.println("sleepb "+sleepTime+" l "+sended+" s "+s+" n "+n+" tt "+(moreTime));
                    }
                    sended = 0;
                }
            }

        }

        public Object getSynlock()
        {
            return synlock;
        }

        public void setSynlock(Object synlock)
        {
            this.synlock = synlock;
        }

        public void setClientId(int clientId)
        {
            this.clientId = clientId;
        }

        public int getClientId_real()
        {
            return clientId_real;
        }

        public void setClientId_real(int clientId_real)
        {
            this.clientId_real = clientId_real;
            lastReceivePingTime = DateTime.Now.Millisecond;
        }

        public long getLastSendPingTime()
        {
            return lastSendPingTime;
        }

        public void setLastSendPingTime(long lastSendPingTime)
        {
            this.lastSendPingTime = lastSendPingTime;
        }

        public long getLastReceivePingTime()
        {
            return lastReceivePingTime;
        }

        public void setLastReceivePingTime(long lastReceivePingTime)
        {
            this.lastReceivePingTime = lastReceivePingTime;
        }

        public String getPassword()
        {
            return password;
        }

        public void setPassword(String password)
        {
            this.password = password;
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using FinalSpeed.socket;
using System.Collections.Concurrent;

namespace FinalSpeed.rudp
{
    public class ConnectionUDP
    {
        public IPAddress dstIp;
        public int dstPort;
        public Sender sender;
        public Receiver receiver;
        public UDPOutputStream uos;
        public UDPInputStream uis;
        long connetionId;
        Route route;
        int mode;
        private boolean connected = true;
        long lastLiveTime = DateTime.Now.Millisecond;
        long lastSendLiveTime = 0;

        static Random ran = new Random();

        int connectId;

        ConnectionProcessor connectionProcessor;

        private ConcurrentQueue<DatagramPacket> dpBuffer = new ConcurrentQueue<DatagramPacket>();

        public ClientControl clientControl;

        public boolean localClosed = false, remoteClosed = false, destroied = false;

        public boolean stopnow = false;

        public ConnectionUDP(Route ro, InetAddress dstIp, int dstPort, int mode, int connectId, ClientControl clientControl)
        {
            this.clientControl = clientControl;
            this.route = ro;
            this.dstIp = dstIp;
            this.dstPort = dstPort;
            this.mode = mode;
            if (mode == 1)
            {
                //MLog.println("                 发起连接RUDP "+dstIp+":"+dstPort+" connectId "+connectId);
            }
            else if (mode == 2)
            {

                //MLog.println("                 接受连接RUDP "+dstIp+":"+dstPort+" connectId "+connectId);
            }
            this.connectId = connectId;
            try
            {
                sender = new Sender(this);
                receiver = new Receiver(this);
                uos = new UDPOutputStream(this);
                uis = new UDPInputStream(this);
                if (mode == 2)
                {
                    ro.createTunnelProcessor().process(this);
                }
            }
            catch (Exception e)
            {
                e.printStackTrace();
                connected = false;
                route.connTable.remove(connectId);
                e.printStackTrace();
                //#MLog.println("                 连接失败RUDP "+connectId);
                lock (this)
                {
                    notifyAll();
                }
                throw e;
            }
            //#MLog.println("                 连接成功RUDP "+connectId);
            lock (this)
            {
                notifyAll();
            }
        }

        public DatagramPacket getPacket(int connectId)
        {
            DatagramPacket dp = dpBuffer.Take(1) as DatagramPacket;
            return dp;
        }

        public string toString()
        {
            return new String(dstIp + ":" + dstPort);
        }

        public bool isConnected()
        {
            return connected;
        }

        public void close_local()
        {
            if (!localClosed)
            {
                localClosed = true;
                if (!stopnow)
                {
                    sender.sendCloseMessage_Conn();
                }
                destroy(false);
            }
        }

        public void close_remote()
        {
            if (!remoteClosed)
            {
                remoteClosed = true;
                destroy(false);
            }
        }

        //完全关闭
        public void destroy(boolean force)
        {
            if (!destroied)
            {
                if ((localClosed && remoteClosed) || force)
                {
                    destroied = true;
                    connected = false;
                    uis.closeStream_Local();
                    uos.closeStream_Local();
                    sender.destroy();
                    receiver.destroy();
                    route.removeConnection(this);
                    clientControl.removeConnection(this);
                }
            }
        }

        public void close_timeout()
        {
            ////#MLog.println("超时关闭RDP连接");
        }

        void live()
        {
            lastLiveTime = System.currentTimeMillis();
        }
    }
}

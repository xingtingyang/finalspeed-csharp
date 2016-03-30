using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using FinalSpeed.rudp.message;
using FinalSpeed.utils;
using System.Threading;
using FinalSpeed.socket;

namespace FinalSpeed.rudp
{
    public class Receiver
    {
        ConnectionUDP conn;
        Sender sender;
        public IPAddress dstIp;
        public int dstPort;
        Dictionary<int, DataMessage> receiveTable = new Dictionary<int, DataMessage>();
        int lastRead = -1;
        int lastReceive = -1;
        Object availOb = new Object();

        bool isReady = false;
        Object readyOb = new Object();
        byte[] b4 = new byte[4];
        int lastRead1 = 0;
        int maxWinR = 10;
        int lastRead2 = -1;
        UDPInputStream uis;

        float availWin = RUDPConfig.maxWin;

        int currentRemoteTimeId;

        int closeOffset;

        bool streamClose = false;

        bool reveivedClose = false;

        static int m = 0, x, x2, c;

        bool b = false, b2;

        public int nw;

        long received;

        Receiver(ConnectionUDP conn)
        {
            this.conn = conn;
            uis = new UDPInputStream(conn);
            this.sender = conn.sender;
            this.dstIp = conn.dstIp;
            this.dstPort = conn.dstPort;
        }

        //接收流数据
        public byte[] receive()
        {
            DataMessage me = null;
            if (conn.isConnected())
            {
                me = receiveTable[lastRead + 1];
                lock (availOb)
                {
                    if (me == null)
                    {
                        //MLog.println("等待中 "+conn.connectId+" "+(lastRead+1));

                        try
                        {
                            Monitor.Wait(availOb);	//availOb.Wait();//这里不知道c#如何改写
                        }
                        catch (Exception e)
                        {
                            MLog.println(e.Message);
                        }
                        me = receiveTable[lastRead + 1];
                        //MLog.println("等待完成aaa "+conn.connectId+" "+(lastRead+1));
                    }
                }

            }
            else
            {
                throw new ConnectException("连接未建立");
            }

            if (!streamClose)
            {
                checkCloseOffset_Remote();
                if (me == null)
                {
                    throw new ConnectException("连接已断开ccccccc");
                }
                else
                {
                }
                conn.sender.sendLastReadDelay();

                lastRead++;
                lock (availOb)
                {
                    receiveTable.Remove(me.getSequence());
                }

                received += me.getData().Length;
                //System.out.println("received "+received/1024/1024+"MB");
                return me.getData();
            }
            else
            {
                throw new ConnectException("连接已断开");
            }
        }

        public void onReceivePacket(DatagramPacket dp)
        {
            DataMessage me;
            if (dp != null)
            {
                if (conn.isConnected())
                {
                    int ver = MessageCheck.checkVer(dp);
                    int sType = MessageCheck.checkSType(dp);
                    if (ver == RUDPConfig.protocal_ver)
                    {
                        conn.live();
                        if (sType == MessageType.sType_DataMessage)
                        {
                            me = new DataMessage(dp);
                            int timeId = me.getTimeId();
                            SendRecord record = conn.clientControl.sendRecordTable_remote[timeId];
                            if (record == null)
                            {
                                record = new SendRecord();
                                record.setTimeId(timeId);
                                conn.clientControl.sendRecordTable_remote.Add(timeId, record);
                            }
                            record.addSended(me.getData().Length);

                            if (timeId > currentRemoteTimeId)
                            {
                                currentRemoteTimeId = timeId;
                            }

                            int sequence = me.getSequence();

                            conn.sender.sendAckDelay(me.getSequence());
                            if (sequence > lastRead)
                            {
                                lock (availOb)
                                {
                                    receiveTable.Add(sequence, me);
                                    if (receiveTable.ContainsKey(lastRead + 1))
                                    {
                                        Monitor.Pulse(availOb);   //availOb.notify();//不知对不对
                                    }
                                }
                            }
                        }
                        else if (sType == MessageType.sType_AckListMessage)
                        {
                            AckListMessage alm = new AckListMessage(dp);
                            int lastRead3 = alm.getLastRead();
                            if (lastRead3 > lastRead2)
                            {
                                lastRead2 = lastRead3;
                            }
                            List<int> ackList = alm.getAckList();

                            for (int i = 0; i < ackList.Count; i++)
                            {
                                int sequence = ackList[i];
                                conn.sender.removeSended_Ack(sequence);
                            }
                            SendRecord rc1 = conn.clientControl.getSendRecord(alm.getR1());
                            if (rc1 != null)
                            {
                                if (alm.getS1() > rc1.getAckedSize())
                                {
                                    rc1.setAckedSize(alm.getS1());
                                }
                            }

                            SendRecord rc2 = conn.clientControl.getSendRecord(alm.getR2());
                            if (rc2 != null)
                            {
                                if (alm.getS2() > rc2.getAckedSize())
                                {
                                    rc2.setAckedSize(alm.getS2());
                                }
                            }

                            SendRecord rc3 = conn.clientControl.getSendRecord(alm.getR3());
                            if (rc3 != null)
                            {
                                if (alm.getS3() > rc3.getAckedSize())
                                {
                                    rc3.setAckedSize(alm.getS3());
                                }
                            }

                            if (checkWin())
                            {
                                conn.sender.play();
                            }
                        }
                        else if (sType == MessageType.sType_CloseMessage_Stream)
                        {
                            CloseMessage_Stream cm = new CloseMessage_Stream(dp);
                            reveivedClose = true;
                            int n = cm.getCloseOffset();
                            closeStream_Remote(n);
                        }
                        else if (sType == MessageType.sType_CloseMessage_Conn)
                        {
                            CloseMessage_Conn cm2 = new CloseMessage_Conn(dp);
                            conn.close_remote();
                        }
                        else
                        {
                            ////#MLog.println("未处理数据包 "+sType);
                        }
                    }

                }
            }

        }

        public void destroy()
        {
            //#MLog.println("destroy destroy destroy");
            lock (availOb)
            {
                receiveTable.Clear();
            }
        }

        bool checkWin()
        {
            nw = conn.sender.sendOffset - lastRead2;
            bool b = false;
            if (nw < availWin)
            {
                b = true;
            }
            else
            {
            }
            return b;
        }

        void closeStream_Remote(int closeOffset)
        {
            this.closeOffset = closeOffset;
            if (!streamClose)
            {
                checkCloseOffset_Remote();
            }
        }

        void checkCloseOffset_Remote()
        {
            if (!streamClose)
            {
                if (reveivedClose)
                {
                    if (lastRead >= closeOffset - 1)
                    {
                        streamClose = true;
                        lock (availOb)
                        {
                            Monitor.PulseAll(availOb);    //availOb.notifyAll();
                        }
                        conn.sender.closeStream_Remote();
                    }
                }
            }
        }

        void closeStream_Local()
        {
            if (!streamClose)
            {
                c++;
                streamClose = true;
                lock (availOb)
                {
                    Monitor.PulseAll(availOb);    //availOb.notifyAll();
                }
                conn.sender.closeStream_Local();
            }
        }

        public int getCurrentTimeId()
        {
            return currentRemoteTimeId;
        }

        public void setCurrentTimeId(int currentTimeId)
        {
            this.currentRemoteTimeId = currentTimeId;
        }


        public int getCloseOffset()
        {
            return closeOffset;
        }


        public void setCloseOffset(int closeOffset)
        {
            this.closeOffset = closeOffset;
        }

    }
}

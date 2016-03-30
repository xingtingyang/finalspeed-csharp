using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FinalSpeed.rudp;
using FinalSpeed.socket;
using System.Net;


namespace FinalSpeed.rudp.message
{
    public abstract class Message
    {
        protected short ver = RUDPConfig.protocal_ver;
        protected short sType = 0;
        protected DatagramPacket dp;
        public int connectId;
        public int clientId;
        public int getSType()
        {
            return sType;
        }
        public int getVer()
        {
            return ver;
        }
        public DatagramPacket getDatagramPacket()
        {
            return dp;
        }
        public void setDstAddress(IPAddress dstIp)
        {
            dp.setAddress(dstIp);
        }
        public void setDstPort(int dstPort)
        {
            dp.setPort(dstPort);
        }
        public int getConnectId()
        {
            return connectId;
        }
        public void setConnectId(int connectId)
        {
            this.connectId = connectId;
        }
        public int getClientId()
        {
            return clientId;
        }
        public void setClientId(int clientId)
        {
            this.clientId = clientId;
        }

    }
}

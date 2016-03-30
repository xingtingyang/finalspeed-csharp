using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FinalSpeed.rudp
{
    public class SendRecord
    {
        int sendSize, sendSize_First;

        int sendCount;

        int ackedSize;

        int timeId;

        int speed;

        bool speedRecored = false;

        int resended;

        public SendRecord()
        {

        }

        public float getDropRate()
        {
            int droped = getSendSize() - getAckedSize();
            if (droped < 0)
            {
                droped = 0;
            }
            float dropRate = 0;
            if (getSendSize() > 0)
            {
                dropRate = (float)droped / getSendSize();
            }
            return dropRate;
        }

        public float getResendRate()
        {
            float resendRate = 0;
            if (getSendSize_First() > 0)
            {
                resendRate = (float)getResended() / getSendSize_First();
            }
            return resendRate;
        }

        public void addResended(int size)
        {
            resended += size;
        }

        public void addSended(int size)
        {
            sendCount++;
            sendSize += size;
        }

        public void addSended_First(int size)
        {
            sendSize_First += size;
        }

        public int getSendSize()
        {
            return sendSize;
        }

        public int getSendCount()
        {
            return sendCount;
        }

        public int getAckedSize()
        {
            return ackedSize;
        }

        //接收到的数据大小
        public void setAckedSize(int ackedSize)
        {
            if (ackedSize > this.ackedSize)
            {
                this.ackedSize = ackedSize;
            }
        }

        public int getTimeId()
        {
            return timeId;
        }

        public void setTimeId(int timeId)
        {
            this.timeId = timeId;
        }

        public int getSpeed()
        {
            return speed;
        }

        public void setSpeed(int speed)
        {
            this.speed = speed;
            speedRecored = true;
        }

        public bool isSpeedRecored()
        {
            return speedRecored;
        }

        public int getResended()
        {
            return resended;
        }

        public void setResended(int resended)
        {
            this.resended = resended;
        }

        public int getSendSize_First()
        {
            return sendSize_First;
        }

        public void setSendSize_First(int sendSize_First)
        {
            this.sendSize_First = sendSize_First;
        }
    }
}

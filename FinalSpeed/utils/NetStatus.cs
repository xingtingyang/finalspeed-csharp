using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace FinalSpeed.utils
{
    public class NetStatus
    {
        public long uploadSum;
        public long downloadSum;

        Thread mainThread;

        int averageTime;

        List<SpeedUnit> speedList;
        SpeedUnit currentUnit;

        public int upSpeed = 0;
        public int downSpeed = 0;

        public NetStatus()
            : this(2)
        {

        }

        public NetStatus(int averageTime)
        {
            this.averageTime = averageTime;
            speedList = new List<SpeedUnit>();
            for (int i = 0; i < averageTime; i++)
            {
                SpeedUnit unit = new SpeedUnit();
                if (i == 0)
                {
                    currentUnit = unit;
                }
                speedList.Add(unit);
            }
            mainThread = new Thread(() =>
            {

                long lastTime = DateTime.Now.Millisecond;
                while (true)
                {

                    if (Math.Abs(System.DateTime.Now.Millisecond - lastTime) > 1000)
                    {

                        lastTime = System.DateTime.Now.Millisecond;
                        calcuSpeed();
                    }
                    try
                    {
                        Thread.Sleep(100);
                    }
                    catch
                    {
                        break;
                    }
                }

            });
            mainThread.Start();
        }

        public void stop()
        {
            mainThread.Interrupt();
        }


        public int getUpSpeed()
        {
            return upSpeed;
        }

        public void setUpSpeed(int upSpeed)
        {
            this.upSpeed = upSpeed;
        }

        public int getDownSpeed()
        {
            return downSpeed;
        }

        public void setDownSpeed(int downSpeed)
        {
            this.downSpeed = downSpeed;
        }


        void calcuSpeed()
        {
            int ds = 0, us = 0;
            foreach (SpeedUnit unit in speedList)
            {
                ds += unit.downSum;
                us += unit.upSum;
            }
            upSpeed = (int)((float)us / speedList.Count);
            downSpeed = (int)(float)ds / speedList.Count;

            speedList.RemoveAt(0);
            SpeedUnit speedunit = new SpeedUnit();
            currentUnit = speedunit;
            speedList.Add(speedunit);
        }

        public void addDownload(int sum)
        {
            downloadSum += sum;
            currentUnit.addDown(sum);
        }

        public void addUpload(int sum)
        {
            uploadSum += sum;
            currentUnit.addUp(sum);
        }

        public void sendAvail()
        {

        }

        public void receiveAvail()
        {

        }

        public void setUpLimite(int speed)
        {

        }

        public void setDownLimite(int speed)
        {

        }
    }
}

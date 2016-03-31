using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FinalSpeed.utils;
using SharpPcap;
using PacketDotNet;
using System.Threading;

namespace FinalSpeed
{
    class Program
    {
        static bool b1 = false;
        static void Main(string[] args)
        {
            Thread thread = new Thread(() =>
            {
                try
                {
                    var devices = SharpPcap.CaptureDeviceList.Instance;
                    b1 = true;
                }
                catch (Exception e3)
                {
                    Console.WriteLine(e3.Message);
                }
            });
            thread.Start();
            try
            {
                thread.Join();
            }
            catch (Exception e1)
            {
                Console.WriteLine(e1.Message); ;
            }

            if(!b1)
            {
                Console.WriteLine("启动失败,请先安装libpcap");
            }
        }

        static void change(string s)
        {
            s += "abc";

        }
    }
}

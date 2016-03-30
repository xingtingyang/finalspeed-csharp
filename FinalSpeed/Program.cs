using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FinalSpeed.utils;

namespace FinalSpeed
{
    class Program
    {
        static void Main(string[] args)
        {
            string str = "one two three four";
            change(str);

            Console.WriteLine(str);

            Console.ReadLine();
        }

        static void change(string s)
        {
            s += "abc";
        }
    }
}

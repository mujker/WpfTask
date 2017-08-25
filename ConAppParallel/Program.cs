using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConAppParallel
{
    class Program
    {
        static XjjSocketWrapper socketWra = new XjjSocketWrapper() { IP = "192.168.2.223", Port = 9999 };
        private static bool _taskFlag = true;

        static void Main(string[] args)
        {
            Parallel.For(0, 100, (i) =>
            {
                while (_taskFlag)
                {
                    var s = socketWra.ReceiveDES($"ri,tyzx99#");
                    Console.WriteLine($"{i} : {s?.Substring(0,10)}");
                }
            });
        }
    }
}

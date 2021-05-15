using System;
using System.Threading;

namespace ConsoleUdbyderAdapter
{
    class Program
    {
        static UdbyderGui udbyderGui = new UdbyderGui();
        public static void ConsumeProc()
        {
            new Consumer().run(udbyderGui);
        }
        static void Main(string[] args)
        {
            Console.WriteLine("UdbyderAdapter");
            Console.WriteLine("-----------------------------");
            var t = new Thread(new ThreadStart(ConsumeProc));
            t.Name = "ConsumerThread";
            t.Priority = ThreadPriority.AboveNormal;
            t.Start();
            udbyderGui.Run();
            t.Join();
        }
    }
}

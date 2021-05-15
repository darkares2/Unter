using System;
using System.Threading;

namespace ConsoleKundeAdapter
{
    class Program
    {
        static KundeGui kundeGui = new KundeGui();
        public static void ConsumeProc()
        {
            new Consumer().run(kundeGui);
        }
        static void Main(string[] args)
        {
            Console.WriteLine("KundeAdapter");
            Console.WriteLine("-----------------------------");
            var t = new Thread(new ThreadStart(ConsumeProc));
            t.Name = "ConsumerThread";
            t.Priority = ThreadPriority.AboveNormal;
            t.Start();
            kundeGui.Run();
            t.Join();
        }
    }
}

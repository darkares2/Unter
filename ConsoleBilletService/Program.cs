using System;

namespace ConsoleBilletService
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("BilletService");
            Console.WriteLine("---------------------");
            new BilletService().run();
        }
    }
}

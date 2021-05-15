using System;

namespace ConsoleMaeglerService
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("MæglerService");
            Console.WriteLine("----------------------");
            new MaeglerService().run();
        }
    }
}

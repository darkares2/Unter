using System;

namespace ConsoleFakturaService
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Faktura Service");
            Console.WriteLine("--------------------------------------");
            new FakturaService().run();
        }
    }
}

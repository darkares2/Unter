using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using UnterLib;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace ConsoleKundeAdapter
{
    public class KundeGui
    {
        const int OFF = 0;
        const int WAITING = 1;
        const int TRIP = 2;
        int state = OFF;
        Guid clientId = Guid.Parse("a015149d-720a-452a-b007-648132812857");
        MessageSender messageSender = new MessageSender();
        
        public void Run()
        {
            while(true)
            {
                try
                {
                    int[] menuItems = menuAccordingToState();
                    var keyNumber = (int)Console.ReadKey(true).KeyChar;
                    if (menuItems.Contains(keyNumber))
                    {
                        handleChoice(keyNumber);
                    }
                } catch (OperationCanceledException) {
                    Console.WriteLine("Operation canceled - Status changed");
                }
                catch (InvalidOperationException)
                {
                    Console.WriteLine("Invalid operation - Status changed");
                }
            }
        }

        private void handleChoice(int keyNumber)
        {
            switch(state)
            {
                case OFF:
                    handleOffChoice(keyNumber); break;
            }
        }

        
        private void handleOffChoice(int keyNumber)
        {
            switch(keyNumber)
            {
                case '1': waiting(); break;
                default: throw new NotImplementedException();
            }
        }

        const int STD_INPUT_HANDLE = -10;

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern IntPtr GetStdHandle(int nStdHandle);


        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool CancelIoEx(IntPtr handle, IntPtr lpOverlapped);
        internal void waiting()
        {
            state = WAITING;
            messageSender.sendOrder(clientId, 55.6760968d, 12.5683371d);
            Task.Delay(60000).ContinueWith(_ =>
            {
                if (state == WAITING)
                {
                    state = OFF;
                    // Timeout => cancel the console read
                    var handle = GetStdHandle(STD_INPUT_HANDLE);
                    CancelIoEx(handle, IntPtr.Zero);
                }
            });
        }
        internal void orderAccept(OrderAcceptMessage orderAcceptMessage)
        {
            state = TRIP;
            var handle = GetStdHandle(STD_INPUT_HANDLE);
            CancelIoEx(handle, IntPtr.Zero);
        }
        internal void orderDone(OrderDoneMessage orderDoneMessage)
        {
            Console.WriteLine("Trip done...");
            state = OFF;
            var handle = GetStdHandle(STD_INPUT_HANDLE);
            CancelIoEx(handle, IntPtr.Zero);
        }


        private int[] menuAccordingToState()
        {
            switch(state)
            {
                case OFF: 
                    Console.WriteLine("Menu: 1) Unter!!!");
                    return new int[] { '1' };
                case WAITING:
                    Console.WriteLine("Waiting...");
                    break;
                case TRIP:
                    Console.WriteLine("Driving...");
                    break;
            }
            return new int[] { };
        }
    }
}

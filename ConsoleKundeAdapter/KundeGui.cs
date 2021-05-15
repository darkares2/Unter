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
        bool orderAcceptInput = false;
        OrderMessage orderMessage;

        public void Run()
        {
            while(true)
            {
                try
                {
                    if (orderAcceptInput)
                    {
                        handlerOrderAccept();
                        continue;
                    }
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

        private void handlerOrderAccept()
        {
            Console.WriteLine($"Tur med kunde: {orderMessage.clientId.ToString()}");
            Console.WriteLine("Acceptér tur [J/N]");
            var keyNumber = (int)Console.ReadKey(true).KeyChar;
            if (keyNumber == 'J' || keyNumber == 'n')
            {

                OrderAcceptMessage orderAcceptMessage = new OrderAcceptMessage()
                {
                    clientId = this.clientId,
                    location = new GeoData() { latitude = 55.6760968d, longitude = 12.5683371d },
                    timestamp = DateTime.UtcNow,
                    order = orderMessage
                };
                messageSender.sendOrderAccept(orderAcceptMessage);
            }
        }

        private void handleChoice(int keyNumber)
        {
            switch(state)
            {
                case OFF:
                    handleOffChoice(keyNumber); break;
                case LEDIG:
                    handleLedigChoice(keyNumber); break;
                case OPTAGET:
                    handleOptagetChoice(keyNumber); break;
            }
        }

        private void handleOptagetChoice(int keyNumber)
        {
            throw new NotImplementedException();
        }

        private void handleLedigChoice(int keyNumber)
        {
            switch (keyNumber)
            {
                case '1': messageSender.sendLocation(clientId, 55.6760968d, 12.5683371d); break;
                case '2': messageSender.sendStatus(clientId, OFF); break;
                default: throw new NotImplementedException();
            }
        }


        private void handleOffChoice(int keyNumber)
        {
            switch(keyNumber)
            {
                case '1': messageSender.sendStatus(clientId, LEDIG); break;
                default: throw new NotImplementedException();
            }
        }

        const int STD_INPUT_HANDLE = -10;

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool CancelIoEx(IntPtr handle, IntPtr lpOverlapped);
        internal void setStatus(StatusMessage statusMessage)
        {
            lock(this)
            {
                if (clientId.Equals(statusMessage.clientId))
                {
                    state = statusMessage.status;
                    var handle = GetStdHandle(STD_INPUT_HANDLE);
                    CancelIoEx(handle, IntPtr.Zero);
                }
            }

        }
        internal void order(Guid clientId, OrderMessage orderMessage)
        {
            lock (this)
            {
                if (this.clientId.Equals(clientId) && !orderAcceptInput)
                {
                    orderAcceptInput = true;
                    Task.Delay(60000).ContinueWith(_ =>
                    {
                        if (orderAcceptInput)
                        {
                            orderAcceptInput = false;
                            // Timeout => cancel the console read
                            var handle = GetStdHandle(STD_INPUT_HANDLE);
                            CancelIoEx(handle, IntPtr.Zero);
                        }
                    });
                }
            }
        }
        internal void orderAccept(Guid clientId, OrderAcceptMessage orderAcceptMessage)
        {
            throw new NotImplementedException();
        }


        private int[] menuAccordingToState()
        {
            switch(state)
            {
                case OFF: 
                    Console.WriteLine("Menu: 1) Ledig.");
                    return new int[] { '1' };
                case LEDIG:
                    Console.WriteLine("Menu: 1) Lokation. 2) Off.");
                    return new int[] { '1' , '2'};
            }
            return new int[] { };
        }
    }
}

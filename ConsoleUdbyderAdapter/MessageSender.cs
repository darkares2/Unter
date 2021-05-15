using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using UnterLib;

namespace ConsoleUdbyderAdapter
{
    public class MessageSender
    {
        static string bootstrapServers = "localhost:9092,localhost:9093, localhost:9094";
        ProducerConfig config = new ProducerConfig
        {
            BootstrapServers = bootstrapServers,
            ClientId = "UdbyderAdapter"
        };
        const string statusTopic = "UdbyderService.Status";
        const string geoTopic = "UdbyderService.Geo";
        const string orderAcceptTopic = "KundeAdapter.BestillingAccept";
        const string receiptTopic = "FakturaService.BestillingUdfort";

        public void sendStatus(Guid clientId, int status)
        {
            StatusMessage statusMessage = new StatusMessage();
            statusMessage.clientId = clientId;
            statusMessage.timestamp = DateTime.UtcNow;
            statusMessage.status = status;
            string json = JsonSerializer.Serialize(statusMessage);
            using (var producer = new ProducerBuilder<Null, string>(config).Build())
            {
                producer.Produce(statusTopic, new Message<Null, string> { Value = json });
                producer.Flush();
            }
            Console.WriteLine($"Status {status} sent");
        }

        public void sendLocation(Guid clientId, double latitude, double longitude)
        {
            LocationMessage locationMessage = new LocationMessage();
            locationMessage.clientId = clientId;
            locationMessage.timestamp = DateTime.UtcNow;
            locationMessage.location = new GeoData() { latitude = latitude, longitude = longitude };
            string json = JsonSerializer.Serialize(locationMessage);
            using (var producer = new ProducerBuilder<Null, string>(config).Build())
            {
                producer.Produce(geoTopic, new Message<Null, string> { Value = json });
                producer.Flush();
            }
            Console.WriteLine("Location sent");
        }

        public void sendOrderAccept(OrderAcceptMessage orderAcceptMessage)
        {
            string json = JsonSerializer.Serialize(orderAcceptMessage);
            using (var producer = new ProducerBuilder<Null, string>(config).Build())
            {
                producer.Produce(orderAcceptTopic, new Message<Null, string> { Value = json });
                producer.Flush();
            }
            Console.WriteLine("Order accept sent");
        }

        internal void sendOrderDone(Guid clientId, OrderMessage currentOrder, double latitude, double longitude)
        {
            OrderDoneMessage orderDoneMessage = new OrderDoneMessage()
            {
                clientId = clientId,
                timestamp = DateTime.UtcNow,
                location = new GeoData() { latitude = latitude, longitude = longitude },
                order = currentOrder
            };
            string json = JsonSerializer.Serialize(orderDoneMessage);
            using (var producer = new ProducerBuilder<Null, string>(config).Build())
            {
                producer.Produce(receiptTopic, new Message<Null, string> { Value = json });
                producer.Flush();
            }
            Console.WriteLine("Order done sent");

        }
    }
}

using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using UnterLib;

namespace ConsoleKundeAdapter
{
    public class MessageSender
    {
        static string bootstrapServers = "localhost:9092,localhost:9093, localhost:9094";
        ProducerConfig config = new ProducerConfig
        {
            BootstrapServers = bootstrapServers,
            ClientId = "KundeAdapter"
        };
        const string orderTopic = "MaeglerService.Bestilling";


        public void sendOrder(Guid clientId, double latitude, double longitude)
        {
            OrderMessage orderMessage = new OrderMessage();
            orderMessage.clientId = clientId;
            orderMessage.timestamp = DateTime.UtcNow;
            orderMessage.expiration = DateTime.UtcNow.AddMinutes(1);
            orderMessage.location = new GeoData() { latitude = latitude, longitude = longitude };
            string json = JsonSerializer.Serialize(orderMessage);
            using (var producer = new ProducerBuilder<Null, string>(config).Build())
            {
                producer.Produce(orderTopic, new Message<Null, string> { Value = json });
                producer.Flush();
            }
            Console.WriteLine("Order sent");
        }

    }
}

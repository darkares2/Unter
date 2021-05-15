using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using UnterLib;

namespace ConsoleMaeglerService
{
    public class MessageSender
    {
        static string bootstrapServers = "localhost:9092,localhost:9093, localhost:9094";
        ProducerConfig config = new ProducerConfig
        {
            BootstrapServers = bootstrapServers,
            ClientId = "MæglerService"
        };
        const string orderTopic = "UdbyderAdapter.Bestilling";

        public void sendBestilling(Guid clientId, OrderMessage order)
        {
            string json = JsonSerializer.Serialize(order);
            using (var producer = new ProducerBuilder<Null, string>(config).Build())
            {
                var headers = new Headers();
                headers.Add(new Header("clientId", clientId.ToByteArray()));
                producer.Produce(orderTopic, new Message<Null, string> { Value = json, Headers = headers });
                producer.Flush();
            }
        }
    }
}

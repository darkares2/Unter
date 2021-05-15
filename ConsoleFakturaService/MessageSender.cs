using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using UnterLib;

namespace ConsoleFakturaService
{
    public class MessageSender
    {
        static string bootstrapServers = "localhost:9092,localhost:9093, localhost:9094";
        ProducerConfig config = new ProducerConfig
        {
            BootstrapServers = bootstrapServers,
            ClientId = "FakturaService"
        };
        const string billTopic = "Faktura";

        public void sendBill(OrderDoneMessage orderDoneMessage)
        {
            string json = JsonSerializer.Serialize(orderDoneMessage);
            using (var producer = new ProducerBuilder<Null, string>(config).Build())
            {
                producer.Produce(billTopic, new Message<Null, string> { Value = json});
                producer.Flush();
            }
        }
    }
}

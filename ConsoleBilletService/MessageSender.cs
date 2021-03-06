using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using UnterLib;

namespace ConsoleBilletService
{
    public class MessageSender
    {
        static string bootstrapServers = "localhost:9092,localhost:9093, localhost:9094";
        ProducerConfig config = new ProducerConfig
        {
            BootstrapServers = bootstrapServers,
            ClientId = "BilletService"
        };
        const string ticketTopic = "UdbyderService.Billet";

        public void sendTicket(TicketMessage ticket)
        {
            string json = JsonSerializer.Serialize(ticket);
            using (var producer = new ProducerBuilder<Null, string>(config).Build())
            {
                producer.Produce(ticketTopic, new Message<Null, string> { Value = json });
                producer.Flush();
            }
        }
    }
}

using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using UnterLib;

namespace ConsoleUdbyderService
{
    public class MessageSender
    {
        static string bootstrapServers = "localhost:9092,localhost:9093, localhost:9094";
        ProducerConfig config = new ProducerConfig
        {
            BootstrapServers = bootstrapServers,
            ClientId = "UdbyderService"
        };
        const string statusTopic = "UdbyderAdapter.Status";
        const string ticketTopic = "BilletService.Billet";

        public void sendStatus(Guid clientId, int status)
        {
            StatusMessage statusMessage = new StatusMessage();
            statusMessage.clientId = clientId;
            statusMessage.status = status;
            string json = JsonSerializer.Serialize(statusMessage);
            using (var producer = new ProducerBuilder<Null, string>(config).Build())
            {
                producer.Produce(statusTopic, new Message<Null, string> { Value = json });
                producer.Flush();
            }
        }

        public void getTicket(Guid clientId)
        {
            NewTicketMessage newTicketMessage = new NewTicketMessage()
            {
                clientId = clientId
            };
            string json = JsonSerializer.Serialize(newTicketMessage);
            using (var producer = new ProducerBuilder<Null, string>(config).Build())
            {
                producer.Produce(ticketTopic, new Message<Null, string> { Value = json });
                producer.Flush();
            }
        }
    }
}

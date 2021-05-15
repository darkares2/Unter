using Confluent.Kafka;
using Confluent.Kafka.Admin;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using UnterLib;

namespace ConsoleBilletService
{
    public class BilletService
    {
        static string bootstrapServers = "localhost:9092,localhost:9093, localhost:9094";
        const string ticketTopic = "BilletService.Billet";
        ConsumerConfig config = new ConsumerConfig
        {
            BootstrapServers = "localhost:9092,localhost:9093, localhost:9094",
            GroupId = "BilletService",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };
        List<String> topics = new List<String>() { ticketTopic };
        DatabaseHandler databaseHandler = new DatabaseHandler();
        MessageSender messageSender = new MessageSender();
        public void run()
        {
            databaseHandler.init();
            CreateTopics().Wait();
            using (var consumer = new ConsumerBuilder<Ignore, string>(config).Build())
            {
                consumer.Subscribe(topics);
                while (true)
                {
                    var consumeResult = consumer.Consume();
                    if (consumeResult == null)
                        break;
                    if (consumeResult.Topic.Equals(ticketTopic))
                        handleTicketMessage(JsonSerializer.Deserialize<NewTicketMessage>(consumeResult.Message.Value));

                }
            }
        }

        private void handleTicketMessage(NewTicketMessage newTicketMessage)
        {
            TicketMessage ticket = new TicketMessage();
            ticket.clientId = newTicketMessage.clientId;
            ticket.sequence = databaseHandler.getNextSequence();
            messageSender.sendTicket(ticket);
        }

        private async Task CreateTopics()
        {
            using (var adminClient = new AdminClientBuilder(new AdminClientConfig { BootstrapServers = bootstrapServers }).Build())
            {
                try
                {
                    if (adminClient.GetMetadata(ticketTopic, TimeSpan.FromSeconds(10)) == null)
                        await adminClient.CreateTopicsAsync(new TopicSpecification[] {
                            new TopicSpecification { Name = ticketTopic, ReplicationFactor = 3, NumPartitions = 3 }
                        });
                }
                catch (CreateTopicsException e)
                {
                    Console.WriteLine($"An error occured creating topic {e.Results[0].Topic}: {e.Results[0].Error.Reason}");
                }
            }
        }

    }
}

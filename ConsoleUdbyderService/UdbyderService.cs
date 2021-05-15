using Confluent.Kafka;
using Confluent.Kafka.Admin;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using UnterLib;

namespace ConsoleUdbyderService
{
    public class UdbyderService
    {
        const int LEDIG = 1;

        static string bootstrapServers = "localhost:9092,localhost:9093, localhost:9094";
        const string geoTopic = "UdbyderService.Geo";
        const string statusTopic = "UdbyderService.Status";
        const string ticketTopic = "UdbyderService.Billet";
        ConsumerConfig config = new ConsumerConfig
        {
            BootstrapServers = "localhost:9092,localhost:9093, localhost:9094",
            GroupId = "UdbyderService",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };
        List<String> topics = new List<String>() { geoTopic, statusTopic, ticketTopic };
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
                    if (consumeResult.Topic.Equals(geoTopic))
                        handleGeoMessage(JsonSerializer.Deserialize < LocationMessage > (consumeResult.Message.Value));
                    else if (consumeResult.Topic.Equals(statusTopic))
                        handleStatusMessage(JsonSerializer.Deserialize<StatusMessage>(consumeResult.Message.Value));
                    else if (consumeResult.Topic.Equals(ticketTopic))
                        handleTicketMessage(JsonSerializer.Deserialize<TicketMessage>(consumeResult.Message.Value));
                }
            }
        }

        private void handleTicketMessage(TicketMessage ticketMessage)
        {
            databaseHandler.updateSequence(ticketMessage.clientId, ticketMessage.sequence);
        }

        private void handleStatusMessage(StatusMessage statusMessage)
        {
            if (databaseHandler.setNewStatus(statusMessage))
            {
                messageSender.sendStatus(statusMessage.clientId, statusMessage.status);
                if (statusMessage.status == LEDIG)
                    messageSender.getTicket(statusMessage.clientId);
            }
        }

        private void handleGeoMessage(LocationMessage locationMessage)
        {
            databaseHandler.insertLocation(locationMessage);
        }

        private async Task CreateTopics()
        {
            using (var adminClient = new AdminClientBuilder(new AdminClientConfig { BootstrapServers = bootstrapServers }).Build())
            {
                try
                {
                    if (adminClient.GetMetadata(statusTopic, TimeSpan.FromSeconds(10)) == null)
                        await adminClient.CreateTopicsAsync(new TopicSpecification[] {
                            new TopicSpecification { Name = statusTopic, ReplicationFactor = 3, NumPartitions = 3 }
                        });
                }
                catch (CreateTopicsException e)
                {
                    Console.WriteLine($"An error occured creating topic {e.Results[0].Topic}: {e.Results[0].Error.Reason}");
                }
                try
                {
                    if (adminClient.GetMetadata(geoTopic, TimeSpan.FromSeconds(10)) == null)
                        await adminClient.CreateTopicsAsync(new TopicSpecification[] {
                            new TopicSpecification { Name = geoTopic, ReplicationFactor = 3, NumPartitions = 3 }
                        });
                }
                catch (CreateTopicsException e)
                {
                    Console.WriteLine($"An error occured creating topic {e.Results[0].Topic}: {e.Results[0].Error.Reason}");
                }
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

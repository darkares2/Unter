using Confluent.Kafka;
using Confluent.Kafka.Admin;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using UnterLib;

namespace ConsoleMaeglerService
{
    public class MaeglerService
    {
        static string bootstrapServers = "localhost:9092,localhost:9093, localhost:9094";
        const string orderTopic = "MaeglerService.Bestilling";
        ConsumerConfig config = new ConsumerConfig
        {
            BootstrapServers = "localhost:9092,localhost:9093, localhost:9094",
            GroupId = "MæglerService",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };
        List<String> topics = new List<String>() { orderTopic };
        DatabaseHandler databaseHandler = new DatabaseHandler();
        MessageSender messageSender = new MessageSender();
        public void run()
        {
            CreateTopics().Wait();
            using (var consumer = new ConsumerBuilder<Ignore, string>(config).Build())
            {
                consumer.Subscribe(topics);
                while (true)
                {
                    var consumeResult = consumer.Consume();
                    if (consumeResult == null)
                        break;
                    if (consumeResult.Topic.Equals(orderTopic))
                        handleOrderMessage(JsonSerializer.Deserialize<OrderMessage>(consumeResult.Message.Value));

                }
            }
        }

        private void handleOrderMessage(OrderMessage orderMessage)
        {
            Guid? clientId = databaseHandler.getNearestSupplier(orderMessage.location.latitude, orderMessage.location.longitude);
            if (clientId.HasValue)
            {
                // TODO: Check if busy in redis.
                messageSender.sendBestilling(clientId.Value, orderMessage);
                Console.WriteLine("Would have marked supplier busy in redis");
                Console.WriteLine("Would have sent order to supplier");
            } else
            {
                Console.WriteLine("No suppliers available... Ignore...");
            }
        }

        private async Task CreateTopics()
        {
            using (var adminClient = new AdminClientBuilder(new AdminClientConfig { BootstrapServers = bootstrapServers }).Build())
            {
                try
                {
                    if (adminClient.GetMetadata(orderTopic, TimeSpan.FromSeconds(10)) == null)
                        await adminClient.CreateTopicsAsync(new TopicSpecification[] {
                            new TopicSpecification { Name = orderTopic, ReplicationFactor = 3, NumPartitions = 10 }
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

using Confluent.Kafka;
using Confluent.Kafka.Admin;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using UnterLib;

namespace ConsoleFakturaService
{
    public class FakturaService
    {
        static string bootstrapServers = "localhost:9092,localhost:9093, localhost:9094";
        const string orderTopic = "FakturaService.BestillingUdfort";
        const string billTopic = "Faktura";
        ConsumerConfig config = new ConsumerConfig
        {
            BootstrapServers = "localhost:9092,localhost:9093, localhost:9094",
            GroupId = "FakturaService",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };
        List<String> topics = new List<String>() { orderTopic };
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
                        handleOrderMessage(JsonSerializer.Deserialize<OrderDoneMessage>(consumeResult.Message.Value));

                }
            }
        }

        private void handleOrderMessage(OrderDoneMessage orderDoneMessage)
        {
            Console.WriteLine("Handle order done message");
            messageSender.sendBill(orderDoneMessage);
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
                try
                {
                    if (adminClient.GetMetadata(billTopic, TimeSpan.FromSeconds(10)) == null)
                        await adminClient.CreateTopicsAsync(new TopicSpecification[] {
                            new TopicSpecification { Name = billTopic, ReplicationFactor = 3, NumPartitions = 3 }
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

using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;

namespace PushVsPull
{
    internal class Program
    {
        static IModel channel;
        static IConnection conn;

        static void Main(string[] args)
        {
            var factory = new ConnectionFactory()
            {
                HostName = "localhost",
                VirtualHost = "/",
                Port = 5672,
                UserName = "guest",
                Password = "guest"
            };

            conn = factory.CreateConnection();
            channel = conn.CreateModel();

            //ReadMessageWithPushModel();

            channel.Close();
            conn.Close();
        }

        private static void ReadMessageWithPushModel()
        {
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (sender, e) =>
            {
                var message = Encoding.UTF8.GetString(e.Body.ToArray());
                Console.WriteLine($"Received message: {message}");
            };

            var consumerTag = channel.BasicConsume("my.queue1", true, consumer);

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();

            channel.BasicCancel(consumerTag);
        }

        private static void ReadMessageWithPullModel()
        {
            Console.WriteLine("Reading messages. Press 'e' to exit...");

            while (true)
            {
                Console.WriteLine("Trying to get message...");

                var response = channel.BasicGet("my.queue1", true);
                if (response != null)
                {
                    var message = Encoding.UTF8.GetString(response.Body.ToArray());
                    Console.WriteLine($"Received message: {message}");
                }

                if (Console.KeyAvailable)
                {
                    var keyValue = Console.ReadKey();
                    if (keyValue.KeyChar == 'e' || keyValue.KeyChar == 'E')
                    {
                        return;
                    }
                }

                Thread.Sleep(2000);
            }
        }
    }
}

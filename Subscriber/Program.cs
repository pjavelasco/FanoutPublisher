using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace Subscriber
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Enter the queue name: ");
            var queueName = Console.ReadLine();

            var factory = new ConnectionFactory()
            {
                HostName = "localhost",
                VirtualHost = "/",
                Port = 5672,
                UserName = "guest",
                Password = "guest"
            };

            var conn = factory.CreateConnection();
            var channel = conn.CreateModel();

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (sender, e) =>
            {
                var message = Encoding.UTF8.GetString(e.Body.ToArray());
                Console.WriteLine("Subscriber [{0}]. Message received: {1}", queueName, message);
            };

            var consumerTag = channel.BasicConsume(queueName, true, consumer);

            Console.WriteLine("Subscribed to the queue '{0}'. Press a key to exit...", queueName);
            Console.ReadKey();

            channel.BasicCancel(consumerTag);
            channel.Close();
            conn.Close();
        }
    }
}

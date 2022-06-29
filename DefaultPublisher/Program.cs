using RabbitMQ.Client;
using System;
using System.Text;

namespace DefaultPublisher
{
    internal class Program
    {
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

            var conn = factory.CreateConnection();
            var channel = conn.CreateModel();

            channel.QueueDeclare(
                "my.queue1",
                true,
                false,
                false,
                null);

            channel.QueueDeclare(
                "my.queue2",
                true,
                false,
                false,
                null);

            channel.BasicPublish(
                "",
                "my.queue1",
                null,
                Encoding.UTF8.GetBytes("Message queue1")
                );

            channel.BasicPublish(
                "",
                "my.queue2",
                null,
                Encoding.UTF8.GetBytes("Message queue2")
                );
        }
    }
}

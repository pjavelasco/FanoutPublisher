using RabbitMQ.Client;
using System;

namespace ExToEx
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

            channel.ExchangeDeclare(
                "exchange1",
                "direct",
                true,
                false,
                null);

            channel.ExchangeDeclare(
                "exchange2",
                "direct",
                true,
                false,
                null);

            channel.QueueDeclare(
                "queue1",
                true,
                false,
                false,
                null);

            channel.QueueDeclare(
                "queue2",
                true,
                false,
                false,
                null);

            channel.QueueBind("queue1", "exchange1", "key1");
            channel.QueueBind("queue2", "exchange2", "key2");

            channel.ExchangeBind("exchange2", "exchange1", "key2");

            channel.BasicPublish(
                "exchange1",
                "key1",
                null,
                System.Text.Encoding.UTF8.GetBytes("Message with routing key 1"));

            channel.BasicPublish(
                "exchange2",
                "key2",
                null,
                System.Text.Encoding.UTF8.GetBytes("Message with routing key 2"));
            
            

        }
    }
}

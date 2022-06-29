using RabbitMQ.Client;
using System;
using System.Collections.Generic;

namespace AlternateExchange
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
                "ex.fanout",
                "fanout",
                true,
                false,
                null);

            channel.ExchangeDeclare(
                "ex.direct",
                "direct",
                true,
                false,
                new Dictionary<string, object>()
                {
                    { "alternate-exchange", "ex.fanout" }
                });

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

            channel.QueueDeclare(
                "my.unrouted",
                true,
                false,
                false,
                null);

            channel.QueueBind("my.queue1", "ex.direct", "video");
            channel.QueueBind("my.queue2", "ex.direct", "audio");
            channel.QueueBind("my.unrouted", "ex.direct", "");

            channel.BasicPublish("ex.direct", "video", null, System.Text.Encoding.UTF8.GetBytes("Hello video"));

            // Goes to unrouted queue
            channel.BasicPublish("ex.direct", "text", null, System.Text.Encoding.UTF8.GetBytes("Hello text"));
        }
    }
}

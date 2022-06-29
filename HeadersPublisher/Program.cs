using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace HeadersPublisher
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
                "ex.headers",
                "headers",
                true,
                false,
                null);

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

            channel.QueueBind("my.errors", "ex.direct", "", new Dictionary<string, object> {
                { "x-match", "all" },
                { "job", "convert" },
                { "format", "jpg" }
            });
            channel.QueueBind("my.infos", "ex.direct", "", new Dictionary<string, object> {
                { "x-match", "any" },
                { "job", "convert" },
                { "format", "jpg" }
            });

            var props = channel.CreateBasicProperties();
            props.Headers = new Dictionary<string, object> {
                { "job", "convert" },
                { "format", "jpg" }
            };

            channel.BasicPublish(
                "ex.headers",
                "",
                props,
                Encoding.UTF8.GetBytes("Message 1")
                );

            props = channel.CreateBasicProperties();
            props.Headers = new Dictionary<string, object> {
                { "job", "convert" },
                { "format", "pdf" }
            };

            channel.BasicPublish(
                "ex.headers",
                "",
                props,
                Encoding.UTF8.GetBytes("Message 2")
                );

        }
    }
}

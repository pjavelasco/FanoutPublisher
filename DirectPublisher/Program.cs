using RabbitMQ.Client;
using System;
using System.Text;

namespace DirectPublisher
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
                "ex.direct",
                "direct",
                true,
                false,
                null);

            channel.QueueDeclare(
                "my.infos",
                true,
                false,
                false,
                null);

            channel.QueueDeclare(
                "my.errors",
                true,
                false,
                false,
                null);

            channel.QueueDeclare(
                "my.warnings",
                true,
                false,
                false,
                null);

            channel.QueueBind("my.errors", "ex.direct", "error");
            channel.QueueBind("my.infos", "ex.direct", "info");
            channel.QueueBind("my.warnings", "ex.direct", "warning");

            channel.BasicPublish(
                "ex.direct",
                "info",
                null,
                Encoding.UTF8.GetBytes("Message info")
                );

            channel.BasicPublish(
                "ex.direct",
                "error",
                null,
                Encoding.UTF8.GetBytes("Message error")
                );
            
            channel.BasicPublish(
                "ex.direct",
                "warning",
                null,
                Encoding.UTF8.GetBytes("Message warning")
                );


        }
    }
}

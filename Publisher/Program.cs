using RabbitMQ.Client;
using System;

namespace Publisher
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

            while (true)
            {
                Console.WriteLine("Enter a message to send:");
                var message = Console.ReadLine();

                if (message == "exit")
                {
                    break;
                }

                channel.BasicPublish(exchange: "ex.fanout", routingKey: "", body: System.Text.Encoding.UTF8.GetBytes(message));
            }

            channel.Close();
            conn.Close();
        }
    }
}

using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;

namespace WorkQueues
{
    /// <summary>
    /// Execute several instances of this program for have multiple workers.
    /// </summary>
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter the name for this worker:");
            var workerName = Console.ReadLine();

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
                var durationInSeconds = int.Parse(message);
                Console.Write($"[{workerName}] Task started. Duration: {durationInSeconds}");
                Thread.Sleep(durationInSeconds * 1000);
                Console.WriteLine($"[{workerName}] Task finished.");
            };

            var consumerTag = channel.BasicConsume("my.queue1", true, consumer);

            Console.WriteLine("Press any key to exit...");

            Console.ReadKey();

            channel.BasicCancel(consumerTag);
            channel.Close();
            conn.Close();
        }
    }
}

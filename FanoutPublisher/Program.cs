﻿using System;
using System.Text;
using RabbitMQ.Client;

namespace FanoutPublisher
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

            channel.QueueBind("my.queue1", "ex.fanout", "");
            channel.QueueBind("my.queue2", "ex.fanout", "");

            channel.BasicPublish(
                "ex.fanout",
                "",
                null,
                Encoding.UTF8.GetBytes("Message 1")
                );

            channel.BasicPublish(
                "ex.fanout",
                "",
                null,
                Encoding.UTF8.GetBytes("Message 2")
                );

            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();

            channel.QueueDelete("my.queue1");
            channel.QueueDelete("my.queue2");
            channel.ExchangeDelete("ex.fanout");

            channel.Close();
            conn.Close();
        }
    }
}
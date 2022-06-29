using Common;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;
using Constants = Common.Constants;

namespace Responser
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

            // Requests
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (sender, e) =>
            {
                var requestData = Encoding.UTF8.GetString(e.Body.ToArray());
                var request = JsonConvert.DeserializeObject<CalculationRequest>(requestData);

                Console.WriteLine("Request received: {0}", request);

                var response = new CalculationResponse();

                if (request.Operation == OperationType.Add)
                {
                    response.Result = request.Number1 + request.Number2;
                }
                else if (request.Operation == OperationType.Substract)
                {
                    response.Result = request.Number1 - request.Number2;
                }

                var responseData = JsonConvert.SerializeObject(response);
                var basicProperties = channel.CreateBasicProperties();
                basicProperties.Headers = new Dictionary<string, object>();
                basicProperties.Headers.Add(Constants.RequestIdHeaderKey, e.BasicProperties.Headers[Constants.RequestIdHeaderKey]);

                var responseQueueName = Encoding.UTF8.GetString((byte[])e.BasicProperties.Headers[Constants.ResponseQueueHeaderKey]);

                channel.BasicPublish("", responseQueueName, basicProperties, Encoding.UTF8.GetBytes(responseData));
            };
            channel.BasicConsume("requests", true, consumer);

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();

            channel.Close();
            conn.Close();
        }
    }
}

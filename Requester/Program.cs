using Common;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using Constants = Common.Constants;

namespace Requester
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var waitingRequests = new ConcurrentDictionary<string, CalculationRequest>();

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

            var responseQueueName = $"res.{Guid.NewGuid()}";
            channel.QueueDeclare(responseQueueName);

            // Responses
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (sender, e) =>
            {
                var requestId = Encoding.UTF8.GetString((byte[])e.BasicProperties.Headers[Constants.RequestIdHeaderKey]);

                CalculationRequest request;
                if (waitingRequests.TryGetValue(requestId, out request))
                {
                    var messageData = Encoding.UTF8.GetString(e.Body.ToArray());
                    var response = JsonConvert.DeserializeObject<CalculationResponse>(messageData);
                    Console.WriteLine("Calculation result: {0} = {1}", request, response);
                }
                
                var message = Encoding.UTF8.GetString(e.Body.ToArray());
                Console.WriteLine("Response received: {0}", message);
            };

            var consumeTag = channel.BasicConsume(responseQueueName, true, consumer);

            // Requests
            //while (true)
            //{
            //    Console.WriteLine("Enter a request: ");
            //    var request = Console.ReadLine();

            //    if (request == "exit")
            //    {
            //        break;
            //    }

            //    channel.BasicPublish("", "requests", null, Encoding.UTF8.GetBytes(request));
            //}

            Console.WriteLine("Press a key to send requests");
            Console.ReadKey();

            SendRequest(waitingRequests, channel, new CalculationRequest(2, 4, OperationType.Add), responseQueueName);
            SendRequest(waitingRequests, channel, new CalculationRequest(14, 6, OperationType.Substract), responseQueueName);
            SendRequest(waitingRequests, channel, new CalculationRequest(50, 2, OperationType.Add), responseQueueName);
            SendRequest(waitingRequests, channel, new CalculationRequest(30, 6, OperationType.Substract), responseQueueName);

            Console.ReadKey();

            channel.BasicCancel(consumeTag);
            channel.Close();
            conn.Close();
        }

        private static void SendRequest(
            ConcurrentDictionary<string, CalculationRequest> waitingRequest,
            IModel channel,
            CalculationRequest request,
            string responseQueueName)
        {
            var requestId = Guid.NewGuid().ToString();
            var requestData = JsonConvert.SerializeObject(request);            

            waitingRequest[requestId] = request;

            var basicProperties = channel.CreateBasicProperties();
            basicProperties.Headers = new Dictionary<string, object>();
            basicProperties.Headers.Add(Constants.RequestIdHeaderKey, Encoding.UTF8.GetBytes(requestId));
            basicProperties.Headers.Add(Constants.ResponseQueueHeaderKey, Encoding.UTF8.GetBytes(responseQueueName));

            channel.BasicPublish("", "requests", basicProperties, Encoding.UTF8.GetBytes(requestData));
        }
    }
}

using Azure.Storage.Queues;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CoffeeNChillExample.Services
{
    public class QueueService
    {
        private readonly QueueClient _queueClient;

        public QueueService(string connectionString, string queueName)
        {
            QueueClientOptions options = new QueueClientOptions
            {
                MessageEncoding = QueueMessageEncoding.Base64
            };

            _queueClient = new QueueClient(
                connectionString, queueName, options);

            _queueClient.CreateIfNotExists();
        }

        public async Task SendMessageAsync<T>(T message)
        {
            string jsonMessage = JsonSerializer.Serialize(message);

            await _queueClient.SendMessageAsync(jsonMessage);
        }
    }
}



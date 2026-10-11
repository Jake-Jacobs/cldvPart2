using Azure.Data.Tables;
using CoffeeNChillExample.Models;
using System;
using System.Collections.Generic;
using System.Text;
using CoffeeNChillExample.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text.Json;


namespace CoffeeNChillExample.Functions
{
    public class ProcessOrderQueue
    {
        private readonly ILogger<ProcessOrderQueue> _logger;
        private readonly OrderTableService _tableService;

        public ProcessOrderQueue(
            ILogger<ProcessOrderQueue> logger)
        {
            _logger = logger;

            string connectionString =
                Environment.GetEnvironmentVariable("AzureWebJobsStorage")
                ?? "UseDevelopmentStorage=true";

            _tableService = new OrderTableService(connectionString);
        }

        [Function("ProcessOrderQueue")]
        public async Task Run(
            [QueueTrigger("order-processing-queue",
                Connection = "AzureWebJobsStorage")] string message)
        {
            Order? order;

            try
            {
                order = JsonSerializer.Deserialize<Order>(
                    message,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex,
                    "Invalid JSON received in the order queue.");
                throw;
            }

            if (order == null ||
                string.IsNullOrWhiteSpace(order.OrderId) ||
                string.IsNullOrWhiteSpace(order.CustomerName) ||
                order.SelectedItemSKUs == null ||
                order.SelectedItemSKUs.Count == 0 ||
                order.TotalPrice <= 0)
            {
                _logger.LogError(
                    "Invalid order message received. Message will be retried.");
                throw new InvalidOperationException(
                    "Invalid order message.");
            }

            DateTime timestamp = order.OrderTimestamp == default
                ? DateTime.UtcNow
                : order.OrderTimestamp;

            string orderDate = timestamp.ToUniversalTime()
                .ToString("yyyy-MM-dd");

            OrderTableEntity tableOrder = new OrderTableEntity
            {
                PartitionKey = orderDate,
                RowKey = order.OrderId,
                CustomerName = order.CustomerName,
                SelectedItemSKUs = JsonSerializer.Serialize(
                    order.SelectedItemSKUs),
                TotalPrice = order.TotalPrice,
                OrderTimestamp = timestamp,
                Status = "Received"
            };

            try
            {
                await _tableService.AddOrderAsync(tableOrder);

                _logger.LogInformation(
                    "Order {OrderId} recorded with status Received.",
                    order.OrderId);

                string[] statuses =
                {
                    "Preparing",
                    "Ready",
                    "Collected"
                };

                foreach (string status in statuses)
                {
                    tableOrder.Status = status;

                    await _tableService.UpdateOrderAsync(tableOrder);

                    _logger.LogInformation(
                        "Order {OrderId} status changed to {Status}.",
                        order.OrderId, status);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Failed processing order {OrderId}.", order.OrderId);
                throw;
            }
        }
    }
}
        
    

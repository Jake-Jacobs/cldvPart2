using CoffeeNChillExample.Models;
using CoffeeNChillExample.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.Json;


namespace CoffeeNChillExample.Functions
{
    public class QueueOrder
    {
        private readonly ILogger<QueueOrder> _logger;
        private readonly QueueService _queueService;

        public QueueOrder(ILogger<QueueOrder> logger)
        {
            _logger = logger;

            string connectionString =
                Environment.GetEnvironmentVariable("AzureWebJobsStorage")
                ?? "UseDevelopmentStorage=true";

            _queueService = new QueueService(
                connectionString, "order-processing-queue");
        }

        [Function("QueueOrder")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post",
                Route = "orders/queue")] HttpRequestData req)
        {
            Order? order;

            try
            {
                order = await JsonSerializer.DeserializeAsync<Order>(
                    req.Body,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
            }
            catch (JsonException)
            {
                var badJson = req.CreateResponse(HttpStatusCode.BadRequest);
                await badJson.WriteStringAsync("Invalid JSON request.");
                return badJson;
            }

            if (order == null ||
                string.IsNullOrWhiteSpace(order.OrderId) ||
                string.IsNullOrWhiteSpace(order.CustomerName) ||
                order.SelectedItemSKUs == null ||
                order.SelectedItemSKUs.Count == 0 ||
                order.SelectedItemSKUs.Any(
                    sku => string.IsNullOrWhiteSpace(sku)) ||
                order.TotalPrice <= 0 ||
                double.IsNaN(order.TotalPrice) ||
                double.IsInfinity(order.TotalPrice))
            {
                var badRequest = req.CreateResponse(
                    HttpStatusCode.BadRequest);

                await badRequest.WriteStringAsync(
                    "Provide a valid OrderId, CustomerName, item SKU list and positive TotalPrice.");

                return badRequest;
            }

            if (order.OrderTimestamp == default)
            {
                order.OrderTimestamp = DateTime.UtcNow;
            }

            try
            {
                await _queueService.SendMessageAsync(order);

                _logger.LogInformation(
                    "Order {OrderId} added to order-processing-queue.",
                    order.OrderId);

                var response = req.CreateResponse(HttpStatusCode.Accepted);
                response.Headers.Add("Content-Type", "application/json");

                await response.WriteStringAsync(
                    JsonSerializer.Serialize(new
                    {
                        message = "Order accepted for processing.",
                        orderId = order.OrderId,
                        status = "Queued"
                    }));

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Failed to queue order {OrderId}.", order.OrderId);

                var error = req.CreateResponse(
                    HttpStatusCode.InternalServerError);

                await error.WriteStringAsync(
                    "The order could not be added to the queue.");

                return error;
            }
        }
    }
}




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
    public class GetOrders
    {
        private readonly ILogger<GetOrders> _logger;
        private readonly OrderTableService _tableService;

        public GetOrders(ILogger<GetOrders> logger)
        {
            _logger = logger;

            string connectionString =
                Environment.GetEnvironmentVariable("AzureWebJobsStorage")
                ?? "UseDevelopmentStorage=true";

            _tableService = new OrderTableService(connectionString);
        }

        [Function("GetOrders")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get",
                Route = "orders")] HttpRequestData req)
        {
            try
            {
                var orders = await _tableService.GetAllOrdersAsync();

                var response = req.CreateResponse(HttpStatusCode.OK);
                response.Headers.Add("Content-Type", "application/json");

                await response.WriteStringAsync(
                    JsonSerializer.Serialize(orders));

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve orders.");

                var response = req.CreateResponse(
                    HttpStatusCode.InternalServerError);

                await response.WriteStringAsync(
                    "Unable to retrieve orders.");

                return response;
            }
        }
    }
}



using Azure;
using Azure.Data.Tables;
using CoffeeNChillExample.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoffeeNChillExample.Services
{
    public class OrderTableService
    {
        private readonly TableClient _tableClient;

        public OrderTableService(string connectionString)
        {
            _tableClient = new TableClient(
                connectionString, "Orders");

            _tableClient.CreateIfNotExists();
        }

        public async Task AddOrderAsync(OrderTableEntity order)
        {
            await _tableClient.AddEntityAsync(order);
        }

        public async Task UpdateOrderAsync(OrderTableEntity order)
        {
            await _tableClient.UpsertEntityAsync(
                order, TableUpdateMode.Replace);
        }

        public async Task<OrderTableEntity?> GetOrderAsync(
            string orderDate, string orderId)
        {
            try
            {
                var result =
                    await _tableClient.GetEntityAsync<OrderTableEntity>(
                        orderDate, orderId);

                return result.Value;
            }
            catch (RequestFailedException ex) when (ex.Status == 404)
            {
                return null;
            }
        }

        public async Task<List<OrderTableEntity>> GetAllOrdersAsync()
        {
            List<OrderTableEntity> orders =
                new List<OrderTableEntity>();

            await foreach (OrderTableEntity order in
                _tableClient.QueryAsync<OrderTableEntity>())
            {
                orders.Add(order);
            }

            return orders;
        }
    }
}


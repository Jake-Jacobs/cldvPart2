using Azure;
using Azure.Data.Tables;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoffeeNChillExample.Models
{
    public class Order : ITableEntity
    {
        public string PartitionKey { get; set; } = "";
        public string RowKey { get; set; } = "";
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }

        public string OrderId { get; set; } = "";
        public string CustomerName { get; set; } = "";
        public List<string> SelectedItemSKUs { get; set; } = new();
        public double TotalPrice { get; set; }
        public DateTime OrderTimestamp { get; set; }
        public string Status { get; set; } = "Received";
    }
}



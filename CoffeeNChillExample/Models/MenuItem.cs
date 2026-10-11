using Azure;
using System;
using System.Collections.Generic;
using System.Text;
using Azure;
using Azure.Data.Tables;
using System;

namespace CoffeeNChillExample.Models
{
    public class MenuItem : ITableEntity //(IIE,2026)
    {
       
        public string PartitionKey { get; set; } = string.Empty;

      
        public string RowKey { get; set; } = string.Empty;

        public DateTimeOffset? Timestamp { get; set; }

        public ETag ETag { get; set; }

       
        public string Name { get; set; } = string.Empty; //(IIE,2026)

        public string Description { get; set; } = string.Empty;

        public double Price { get; set; }

        public bool IsAvailable { get; set; }

        public override string ToString()
        {
            return $"[{PartitionKey} | {RowKey}] : {Name} - R{Price}";
        }
    }
}
//FULL REFERENCE LIST
//IIE, 2026. Cloud Development B: Lecture notes distributed in CLDV6212 Learning Unit 1. https://mystudies.iie.edu.za/d2l/le/lessons/81852/units/5622998  
//IIE, 2026. Cloud Development B: Lecture notes distributed in CLDV6212 Learning Unit 2. https://mystudies.iie.edu.za/d2l/le/lessons/81852/units/5622999 
//IIE, 2026. Cloud Development B: Lecture notes distributed in CLDV6212 Learning Unit 3. https://mystudies.iie.edu.za/d2l/le/lessons/81852/units/5623000




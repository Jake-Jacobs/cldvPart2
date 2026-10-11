using CoffeeNChillExample.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Azure.Data.Tables;
using System;
using System.Collections.Generic;

namespace CoffeeNChillExample.Services
{
    public class TableService //(IIE,2026)
    {
       
        private readonly TableClient _tableClient;

       
        public TableService(string connectionString, string tableName)
        {
          
            _tableClient = new TableClient(connectionString, tableName);

            
            _tableClient.CreateIfNotExists();
        }

      
        public async Task AddEntityAsync(MenuItem newItem)
        {
            await _tableClient.AddEntityAsync(newItem);
        }

       
        public async Task<List<MenuItem>> GetAllEntitiesAsync() //(IIE,2026)
        {
            var allRecords = new List<MenuItem>();

            await foreach (MenuItem item in _tableClient.QueryAsync<MenuItem>())
            {
                allRecords.Add(item);
            }

            return allRecords;
        }

      
        public async Task<List<MenuItem>> GetEntitiesByCategoryAsync(string category)
        {
            var allRecords = new List<MenuItem>();

            await foreach (MenuItem item in _tableClient.QueryAsync<MenuItem>(
                item => item.PartitionKey == category))
            {
                allRecords.Add(item);
            }

            return allRecords;
        }

       
        public async Task<MenuItem?> GetEntityAsync(string category, string id)
        {
            try //(IIE,2026)
            {
                MenuItem item = await _tableClient.GetEntityAsync<MenuItem>(
                    category,
                    id);

                return item;
            }
            catch
            {
                return null;
            }
        }

        // Update an entity
        public async Task UpdateEntityAsync(MenuItem item)
        {
            await _tableClient.UpdateEntityAsync(
                item,
                item.ETag,
                TableUpdateMode.Replace);
        }

        // Delete an entity
        public async Task DeleteEntityAsync(string category, string id)
        {
            await _tableClient.DeleteEntityAsync(category, id);
        }
    }
}
//FULL REFERENCE LIST
//IIE, 2026. Cloud Development B: Lecture notes distributed in CLDV6212 Learning Unit 1. https://mystudies.iie.edu.za/d2l/le/lessons/81852/units/5622998  
//IIE, 2026. Cloud Development B: Lecture notes distributed in CLDV6212 Learning Unit 2. https://mystudies.iie.edu.za/d2l/le/lessons/81852/units/5622999 
//IIE, 2026. Cloud Development B: Lecture notes distributed in CLDV6212 Learning Unit 3. https://mystudies.iie.edu.za/d2l/le/lessons/81852/units/5623000




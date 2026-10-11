using CoffeeNChillExample.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;


namespace CoffeeNChillExample.Functions
{
    public class DeleteMenuItem //(IIE,2026)
    {
        private readonly ILogger<DeleteMenuItem> _logger;

        public DeleteMenuItem(ILogger<DeleteMenuItem> logger)
        {
            _logger = logger;
        }

        [Function("DeleteMenuItem")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(
                AuthorizationLevel.Anonymous,
                "delete",
                Route = "menu/{category}/{id}")]
            HttpRequestData req,
            string category,
            string id)
        {
            try
            {
                if (string.IsNullOrEmpty(category) ||
                    string.IsNullOrEmpty(id))
                {
                    var errorResponse =
                        req.CreateResponse(HttpStatusCode.BadRequest);

                    await errorResponse.WriteStringAsync(
                        "Category and ID are required.");

                    return errorResponse;
                }

                TableService tableService =
                    new TableService(
                        "UseDevelopmentStorage=true",
                        "Menuitems");

                var existingItem =
                    await tableService.GetEntityAsync(category, id);

                if (existingItem == null) //(IIE,2026)
                {
                    var errorResponse =
                        req.CreateResponse(HttpStatusCode.NotFound);

                    await errorResponse.WriteStringAsync(
                        "Menu item was not found.");

                    return errorResponse;
                }

                await tableService.DeleteEntityAsync(category, id);

                var response =
                    req.CreateResponse(HttpStatusCode.OK);

                await response.WriteStringAsync(
                    "Menu item deleted successfully.");

                return response;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error deleting menu item.");

                var response =
                    req.CreateResponse(
                        HttpStatusCode.InternalServerError);

                await response.WriteStringAsync(
                    "An error occurred while deleting the menu item.");

                return response;
            }
        }
    }
}
//FULL REFERENCE LIST
//IIE, 2026. Cloud Development B: Lecture notes distributed in CLDV6212 Learning Unit 1. https://mystudies.iie.edu.za/d2l/le/lessons/81852/units/5622998  
//IIE, 2026. Cloud Development B: Lecture notes distributed in CLDV6212 Learning Unit 2. https://mystudies.iie.edu.za/d2l/le/lessons/81852/units/5622999 
//IIE, 2026. Cloud Development B: Lecture notes distributed in CLDV6212 Learning Unit 3. https://mystudies.iie.edu.za/d2l/le/lessons/81852/units/5623000



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
    public class GetMenuItemsByCategory //(IIE,2026)
    {
        private readonly ILogger<GetMenuItemsByCategory> _logger;

        public GetMenuItemsByCategory(ILogger<GetMenuItemsByCategory> logger)
        {
            _logger = logger;
        }

        [Function("GetMenuItemsByCategory")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(
                AuthorizationLevel.Anonymous,
                "get",
                Route = "menu/category/{category}")]
            HttpRequestData req,
            string category)
        {
            _logger.LogInformation(
                "Getting menu items for category {category}.",
                category);

            if (string.IsNullOrEmpty(category))
            {
                var errorResponse =
                    req.CreateResponse(HttpStatusCode.BadRequest);

                await errorResponse.WriteStringAsync(
                    "Category is required.");

                return errorResponse;
            }

            try
            {
                TableService tableService =
                    new TableService(
                        "UseDevelopmentStorage=true",
                        "Menuitems");

                var menuItems =
                    await tableService.GetEntitiesByCategoryAsync(category); //(IIE,2026)

                var response = req.CreateResponse(HttpStatusCode.OK);

                await response.WriteAsJsonAsync(menuItems);

                return response;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error filtering menu items.");

                var response =
                    req.CreateResponse(
                        HttpStatusCode.InternalServerError);

                await response.WriteStringAsync(
                    "An error occurred while filtering menu items.");

                return response;
            }
        }
    }
}
//FULL REFERENCE LIST
//IIE, 2026. Cloud Development B: Lecture notes distributed in CLDV6212 Learning Unit 1. https://mystudies.iie.edu.za/d2l/le/lessons/81852/units/5622998  
//IIE, 2026. Cloud Development B: Lecture notes distributed in CLDV6212 Learning Unit 2. https://mystudies.iie.edu.za/d2l/le/lessons/81852/units/5622999 
//IIE, 2026. Cloud Development B: Lecture notes distributed in CLDV6212 Learning Unit 3. https://mystudies.iie.edu.za/d2l/le/lessons/81852/units/5623000




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
    public class GetAllMenuItems //(IIE,2026)
    {
        private readonly ILogger<GetAllMenuItems> _logger;

        public GetAllMenuItems(ILogger<GetAllMenuItems> logger)
        {
            _logger = logger;
        }

        [Function("GetAllMenuItems")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "menu")]
            HttpRequestData req)
        {
            _logger.LogInformation("Getting all menu items.");

            try
            {
                TableService tableService =
                    new TableService(
                        "UseDevelopmentStorage=true",
                        "Menuitems");

                var menuItems =
                    await tableService.GetAllEntitiesAsync();

                var response = req.CreateResponse(HttpStatusCode.OK);

                await response.WriteAsJsonAsync(menuItems);

                return response;
            }
            catch (Exception e) //(IIE,2026)
            {
                _logger.LogError(e, "Error getting menu items.");

                var response =
                    req.CreateResponse(
                        HttpStatusCode.InternalServerError);

                await response.WriteStringAsync(
                    "An error occurred while getting menu items.");

                return response;
            }
        }
    }
}
//FULL REFERENCE LIST
//IIE, 2026. Cloud Development B: Lecture notes distributed in CLDV6212 Learning Unit 1. https://mystudies.iie.edu.za/d2l/le/lessons/81852/units/5622998  
//IIE, 2026. Cloud Development B: Lecture notes distributed in CLDV6212 Learning Unit 2. https://mystudies.iie.edu.za/d2l/le/lessons/81852/units/5622999 
//IIE, 2026. Cloud Development B: Lecture notes distributed in CLDV6212 Learning Unit 3. https://mystudies.iie.edu.za/d2l/le/lessons/81852/units/5623000





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

    public class ListStaffDocuments //(IIE,2026)
    {
        private readonly ILogger<ListStaffDocuments> _logger;

        public ListStaffDocuments(ILogger<ListStaffDocuments> logger)
        {
            _logger = logger;
        }

        [Function("ListStaffDocuments")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(
                AuthorizationLevel.Anonymous,
                "get",
                Route = "documents")]
            HttpRequestData req)
        {
            try
            {
                FileService fileService =
                    new FileService(
                        "UseDevelopmentStorage=true",
                        "staff-docs");

                var documents =
                    await fileService.GetAllFilesAsync();

                var response =
                    req.CreateResponse(HttpStatusCode.OK);

                await response.WriteAsJsonAsync(documents);

                return response;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error listing staff documents."); //(IIE,2026)

                var response =
                    req.CreateResponse(
                        HttpStatusCode.InternalServerError);

                await response.WriteStringAsync(
                    "An error occurred while listing documents.");

                return response;
            }
        }
    }
}
//FULL REFERENCE LIST
//IIE, 2026. Cloud Development B: Lecture notes distributed in CLDV6212 Learning Unit 1. https://mystudies.iie.edu.za/d2l/le/lessons/81852/units/5622998  
//IIE, 2026. Cloud Development B: Lecture notes distributed in CLDV6212 Learning Unit 2. https://mystudies.iie.edu.za/d2l/le/lessons/81852/units/5622999 
//IIE, 2026. Cloud Development B: Lecture notes distributed in CLDV6212 Learning Unit 3. https://mystudies.iie.edu.za/d2l/le/lessons/81852/units/5623000


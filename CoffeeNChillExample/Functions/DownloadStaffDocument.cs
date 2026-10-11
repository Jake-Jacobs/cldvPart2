using CoffeeNChillExample.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;


namespace CoffeeNChillExample.Functions //(IIE,2026)
{
    public class DownloadStaffDocument
    {
        private readonly ILogger<DownloadStaffDocument> _logger;

        public DownloadStaffDocument(
            ILogger<DownloadStaffDocument> logger)
        {
            _logger = logger;
        }

        [Function("DownloadStaffDocument")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(
                AuthorizationLevel.Anonymous,
                "get",
                Route = "documents/download/{fileName}")]
            HttpRequestData req,
            string fileName)
        {
            try
            {
                if (string.IsNullOrEmpty(fileName))
                {
                    var errorResponse =
                        req.CreateResponse(HttpStatusCode.BadRequest);

                    await errorResponse.WriteStringAsync(
                        "File name is required.");

                    return errorResponse;
                }

                FileService fileService = //(IIE,2026)
                    new FileService(
                        "UseDevelopmentStorage=true",
                        "staff-docs");

                var fileStream =
                    await fileService.DownloadFileAsync(fileName);

                if (fileStream == null)
                {
                    var errorResponse =
                        req.CreateResponse(HttpStatusCode.NotFound);

                    await errorResponse.WriteStringAsync(
                        "File was not found.");

                    return errorResponse;
                }

                var response =
                    req.CreateResponse(HttpStatusCode.OK);

                response.Headers.Add(
                    "Content-Type",
                    "application/pdf");

                await fileStream.CopyToAsync(response.Body);

                return response;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error downloading document.");

                var response =
                    req.CreateResponse(
                        HttpStatusCode.InternalServerError);

                await response.WriteStringAsync(
                    "An error occurred while downloading the document.");

                return response;
            }
        }
    }
}
//FULL REFERENCE LIST
//IIE, 2026. Cloud Development B: Lecture notes distributed in CLDV6212 Learning Unit 1. https://mystudies.iie.edu.za/d2l/le/lessons/81852/units/5622998  
//IIE, 2026. Cloud Development B: Lecture notes distributed in CLDV6212 Learning Unit 2. https://mystudies.iie.edu.za/d2l/le/lessons/81852/units/5622999 
//IIE, 2026. Cloud Development B: Lecture notes distributed in CLDV6212 Learning Unit 3. https://mystudies.iie.edu.za/d2l/le/lessons/81852/units/5623000



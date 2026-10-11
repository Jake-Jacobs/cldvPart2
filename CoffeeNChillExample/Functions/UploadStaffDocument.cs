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
    public class UploadStaffDocument //(IIE,2026)
    {
        private readonly ILogger<UploadStaffDocument> _logger;

        public UploadStaffDocument(ILogger<UploadStaffDocument> logger)
        {
            _logger = logger;
        }

        [Function("UploadStaffDocument")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(
                AuthorizationLevel.Anonymous,
                "post",
                Route = "documents/upload")]
            HttpRequestData req)
        {
            try //(IIE,2026)
            {
                string fileName =
                    req.Headers.GetValues("fileName").FirstOrDefault();

                if (string.IsNullOrEmpty(fileName))
                {
                    var errorResponse =
                        req.CreateResponse(HttpStatusCode.BadRequest);

                    await errorResponse.WriteStringAsync(
                        "Please provide a fileName header.");

                    return errorResponse;
                }

                if (!fileName.EndsWith(".pdf"))
                {
                    var errorResponse =
                        req.CreateResponse(HttpStatusCode.BadRequest);

                    await errorResponse.WriteStringAsync(
                        "Only PDF files are allowed.");

                    return errorResponse;
                }

                FileService fileService =
                    new FileService(
                        "UseDevelopmentStorage=true",
                        "staff-docs");

                await fileService.UploadFileAsync(
                    req.Body,
                    fileName);

                var response =
                    req.CreateResponse(HttpStatusCode.OK);

                await response.WriteStringAsync(
                    "Staff document uploaded successfully.");

                return response;
            }
            catch (Exception e) //(IIE,2026)
            {
                _logger.LogError(e, "Error uploading staff document.");

                var response =
                    req.CreateResponse(
                        HttpStatusCode.InternalServerError);

                await response.WriteStringAsync(
                    "An error occurred while uploading the document.");

                return response;
            }
        }
    }
}
//FULL REFERENCE LIST
//IIE, 2026. Cloud Development B: Lecture notes distributed in CLDV6212 Learning Unit 1. https://mystudies.iie.edu.za/d2l/le/lessons/81852/units/5622998  
//IIE, 2026. Cloud Development B: Lecture notes distributed in CLDV6212 Learning Unit 2. https://mystudies.iie.edu.za/d2l/le/lessons/81852/units/5622999 
//IIE, 2026. Cloud Development B: Lecture notes distributed in CLDV6212 Learning Unit 3. https://mystudies.iie.edu.za/d2l/le/lessons/81852/units/5623000



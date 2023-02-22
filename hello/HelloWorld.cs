using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;
using Dapr.Client;

namespace dapr_func
{
    public static class HelloWorld
    {
        [FunctionName("HelloWorld")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            // Invoke the CurrentDate service to get the current date.
            HttpClient httpClient = DaprClient.CreateInvokeHttpClient();
            string currentDateString = await httpClient.GetStringAsync("http://dateapp/api/CurrentDate");
 
            string responseMessage = string.IsNullOrEmpty(name)
                ? $"This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response ({currentDateString})."
                : $"Hello, {name}. This HTTP triggered function executed successfully ({currentDateString}).";

            return new OkObjectResult(responseMessage);
        }
    }
}

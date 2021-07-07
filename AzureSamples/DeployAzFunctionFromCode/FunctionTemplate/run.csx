#r "Newtonsoft.Json"

using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

private static string _appName = "{{app-name}}";
private static string _endpoint = "{{endpoint}}";
private static string _method = "{{method}}";
private static string _apiKey = "{{api-key}}";

public async static Task Run(string messageJson, ILogger log)
{
    var message = JsonConvert.DeserializeObject<object>(messageJson);
    var response = await CallEndpoint(message, log);    
    response.EnsureSuccessStatusCode();
}

private static async Task<HttpResponseMessage> CallEndpoint(object message)
{
    var httpRequestModel = new {};
    var json = JsonConvert.SerializeObject(httpRequestModel);

    var url = _endpoint;

    using var client = new HttpClient();
    string contentType = "application/json";
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(contentType));

    HttpResponseMessage response;
    var httpMethod = new HttpMethod(_method);
    switch (httpMethod.ToString().ToUpper())
    {
        case "POST":
            {
                HttpContent body = new StringContent(json);
                body.Headers.ContentType = new MediaTypeHeaderValue(contentType);
                response = await client.PostAsync(url, body);
            }
            break;
        case "PUT":
            {
                HttpContent body = new StringContent(json);
                body.Headers.ContentType = new MediaTypeHeaderValue(contentType);
                response = await client.PutAsync(url, body);
            }
            break;
        default:
            throw new NotImplementedException();
    }
    
    return response;
}

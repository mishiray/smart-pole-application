using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Net;
using DigitalTwinFramework.DTOs;
using System.Net.Http.Json;
using DigitalTwinFramework.DTOs.Enums;
using RestSharp;

namespace DigitalTwinFramework.Services
{
    public interface IDigitalTwinMiddlewareService
    {
        Task<CustomResponse<object>> StoreData(CreateTelemetryDto telemetryData);
        Task<CustomResponse<string>> Connect();
    }

    public class DigitalTwinMiddlewareService : IDigitalTwinMiddlewareService
    {
        JsonSerializerOptions options = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNameCaseInsensitive = true
        };

        private readonly string bearerToken;
        private readonly string baseUrl;
        private HttpClient httpClient;

        public DigitalTwinMiddlewareService()
        {
            bearerToken = "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiI0NDQxYjc5OS1jMDg4LTQ3OTEtODZlZS0xZDVlMzk0MTkwNDUiLCJuYmYiOjE2ODI5Mzk5MTksImV4cCI6MTcxNDU2MjMxOSwiaWF0IjoxNjgyOTM5OTE5LCJpc3MiOiJodHRwOi8vbG9jYWxob3N0IiwiYXVkIjoiVXNlciJ9.qKrnGd75gSh8MEUmVUwkFRkR8Adl208RzhMDjuZkjQM";
            baseUrl = "https://digital-twin-middleware.azurewebsites.net/";
        }

        public async Task<CustomResponse<object>> StoreData(CreateTelemetryDto telemetryData)
        {
            try
            {
                using (httpClient = new HttpClient())
                {
                    httpClient.BaseAddress = new Uri($"{baseUrl}api/v1/Telemetries/log/");

                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    httpClient.DefaultRequestHeaders.Add("Authorization", bearerToken);

                    var response = httpClient.PostAsJsonAsync($"{httpClient.BaseAddress}", telemetryData);

                    if (response.Result.StatusCode == HttpStatusCode.OK)
                    {
                        return new CustomResponse<object>() { Message = "Data saved Successfully", Response = ServiceResponses.Success };
                    }
                    else
                    {
                        Console.WriteLine(response.Result.StatusCode);
                        Console.WriteLine(await response.Result.Content.ReadAsStringAsync());
                        return new CustomResponse<object>() { Message = "Data not saved", Response = ServiceResponses.BadRequest };
                    }

                    return new CustomResponse<object>() { Data = null, Response = ServiceResponses.Success };
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("error exception " + e.Message);
                return new CustomResponse<object>() { Message = e.Message, Response = ServiceResponses.Failed };
            }
        }

        public async Task<CustomResponse<string>> Connect()
        {
            try
            {
                using (httpClient = new HttpClient())
                {
                    httpClient.BaseAddress = new Uri($"{baseUrl}api/v1/Devices/connect/");

                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    httpClient.DefaultRequestHeaders.Add("Authorization", bearerToken);

                    var response = httpClient.GetAsync($"{httpClient.BaseAddress}");


                    if (response.Result.StatusCode == HttpStatusCode.OK)
                    {
                        return new CustomResponse<string>() { Message = "Connected Successfully", Response = ServiceResponses.Success };
                    }
                    else
                    {
                        return new CustomResponse<string>() { Message = "Connection not successful", Response = ServiceResponses.BadRequest };
                    }

                    return new CustomResponse<string>() { Data = null, Response = ServiceResponses.Success };
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error exception " + e.Message);
                return new CustomResponse<string>() { Message = e.Message, Response = ServiceResponses.Failed };
            }
        }
    }
}

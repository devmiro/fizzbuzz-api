using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using FizzBuzzJB.Helpers;
using FizzBuzzJB.Interfaces;
using FizzBuzzJB.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;

namespace FizzBuzzJB
{
    public class Program
    {
        private static bool keyFound;

        public static async Task Main(string[] args)
        {
            var host = new HostBuilder()
             .ConfigureServices(services =>
             {
                 services.AddHttpClient<FizzBuzzService>();
                 services.AddLogging();
             })
             .Build();


            GeneralError erro;
            IEnumerable<int> numbers;
            string treasure;

            var fbService = host.Services.GetRequiredService<FizzBuzzService>();            

            while (!keyFound)
            {
                (erro, numbers) = await fbService.RequestApiAsync();

                if (erro != null)
                    CheckError(erro);
                else if (numbers != null)
                {
                    List<string> results = Utils.GetFizzBuzz(numbers);

                    var jsonResult = JsonSerializer.Serialize(results);
                    var hash = Utils.GetHash(jsonResult);

                    (erro, treasure) = await fbService.PostHashAsync(hash, jsonResult);

                    if (!string.IsNullOrEmpty(treasure))
                    {
                        keyFound = true;
                        Console.WriteLine(treasure);
                    }
                    else
                        await fbService.DeleteAsync(hash);
                }
            }
        }        

        private static void CheckError(GeneralError erro)
        {
            Console.Error.WriteLine(erro.message);
        }
    }



    public class FizzBuzzService : IFizzBuzzService
    {
        private readonly HttpClient _httpClient;

        public FizzBuzzService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://codetest.jurosbaixos.com.br/v1/");
            _httpClient.DefaultRequestHeaders.Add(
            HeaderNames.Accept, "application/json");
            _httpClient.DefaultRequestHeaders.Add(
            "x-api-key", "SamirShowUsSomeNiceCode!");
        }

        public async Task<(GeneralError?, IEnumerable<int>?)> RequestApiAsync()
        {
            GeneralError? erro = null;
            IEnumerable<int>? numbers = null;
            try
            {
                var httpResponseMessage = await _httpClient.GetAsync("fizzbuzz");

                using var contentStream =
                    await httpResponseMessage.Content.ReadAsStreamAsync();

                if (httpResponseMessage.IsSuccessStatusCode)
                    numbers = await JsonSerializer.DeserializeAsync<IEnumerable<int>>(contentStream);
                else
                    erro = await JsonSerializer.DeserializeAsync<GeneralError>(contentStream);
            }
            catch (Exception ex)
            {

            }
            return (erro, numbers);
        }

        public async Task DeleteAsync(string hash)
        {
            await _httpClient.DeleteAsync($"fizzbuzz/{hash}");
        }

        public async Task<(GeneralError?, string?)> PostHashAsync(string hash, string jsonResult)
        {
            GeneralError? erro = null;
            string? treasure = string.Empty;
            var data = new StringContent(jsonResult, Encoding.UTF8, "application/json");
            try
            {
                await _httpClient.PostAsync($"fizzbuzz/{hash}", data);

                var response = await _httpClient.GetAsync($"fizzbuzz/{hash}/canihastreasure");

                using var contentStream =
                            await response.Content.ReadAsStreamAsync();

                if (response.IsSuccessStatusCode)
                {
                    treasure = await JsonSerializer.DeserializeAsync<string>(contentStream);
                }
                else
                {
                    erro = await JsonSerializer.DeserializeAsync<GeneralError>(contentStream);
                }
            }
            catch (HttpRequestException exception)
            {
                //_logger.LogError(exception, "HttpRequestException when calling the API");
                //return Result<GitHubRepositoryDto>.Retry("HttpRequestException when calling the API");
            }
            catch (Exception exception)
            {
                //_logger.LogError(exception, "Unhandled exception when calling the API");
                //// Here it's up to you if you want to throw or return Retry/Fail, im choosing to FAIL.
                //return Result<GitHubRepositoryDto>.Fail("Unhandled exception when calling the API");
            }
            return (erro, treasure);
        }
    }
}
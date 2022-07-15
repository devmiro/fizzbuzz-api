using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
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
     })
     .Build();

            GeneralError erro = null;
            IEnumerable<int> numbers;
            string treasure;

            var fbService = host.Services.GetRequiredService<FizzBuzzService>();
            //var numbers = new int[] { 326071, 71889, 823516, 188230, 437354, 549499, 350860, 428506, 439923, 31425, 414337, 427407, 136861, 193935, 333855, 950037, 163714, 442103, 201436, 328827, 224806, 68490, 243522, 940680, 654647, 368513, 262932, 324650, 567723, 742479, 22949, 933093, 744171, 857770, 550971, 78559, 126501, 193626, 902600, 893564, 924239, 206343, 854784, 123636, 913670, 798611, 862820, 228727, 628641, 587495, 604327, 817704, 740890, 874399, 649952, 192924, 109076, 282977, 912737, 409000, 270887, 80126, 442491, 395905, 31009, 612803, 792975, 895218, 406429, 443758, 802984, 901724, 802151, 234426, 106590, 123117, 471111, 269997, 152353, 45343, 412368, 836767, 796714, 767231, 71510, 655362, 642679, 477245, 462456, 835040, 791595, 280595, 177341, 298330, 743550, 799914, 663654, 15647, 141240, 703560 };

            while (!keyFound)
            {
                (erro, numbers) = await fbService.RequestApiAsync();

                if (erro != null)
                    CheckError(erro);
                else if (numbers != null)
                {
                    List<string> results = GetFizzBuzz(numbers);

                    var jsonResult = JsonSerializer.Serialize(results);
                    var hash = GetHash(jsonResult);

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

        private static string GetHash(string jsonResult)
        {
            using (SHA256 mySHA256 = SHA256.Create())
            {
                var sha = mySHA256.ComputeHash(Encoding.UTF8.GetBytes(jsonResult));
                var hexa = ComputeHexa(sha);
                Console.WriteLine(hexa);
                return hexa;
            }
        }

        private static void CheckError(GeneralError erro)
        {
            Console.Error.WriteLine(erro.message);
        }

        private static List<string> GetFizzBuzz(IEnumerable<int> numbers)
        {
            List<string> results = new List<string>();
            foreach (var x in numbers)
            {
                string result = string.Empty;
                if (x % 3 == 0)
                    result += "fizz";
                if (x % 5 == 0)
                    result += "buzz";

                if (!string.IsNullOrEmpty(result))
                    results.Add(result);
                else
                    results.Add(x.ToString());
            }

            return results;
        }

        public static string ComputeHexa(byte[] array)
        {
            string hexaResult = string.Empty;

            for (int i = 0; i < array.Length; i++)
            {
                hexaResult += $"{array[i]:x2}";
            }
            return hexaResult;
        }

    }



    public class FizzBuzzService
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

        public async Task<(GeneralError, IEnumerable<int>)> RequestApiAsync()
        {
            GeneralError erro = null;
            IEnumerable<int> numbers = null;
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

        internal async Task DeleteAsync(string hash)
        {
            await _httpClient.DeleteAsync($"fizzbuzz/{hash}");
        }

        internal async Task<(GeneralError, string)> PostHashAsync(string hash, string jsonResult)
        {
            GeneralError erro = null;
            string treasure = string.Empty;
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
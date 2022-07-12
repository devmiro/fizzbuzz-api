using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using FizzBuzzJB.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FizzBuzzJB
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = new HostBuilder()
     .ConfigureServices(services =>
     {
         services.AddHttpClient();
         services.AddTransient<FizzBuzzService>();
     })
     .Build();

            var fbService = host.Services.GetRequiredService<FizzBuzzService>();
            var numbers = new int[] { 326071, 71889, 823516, 188230, 437354, 549499, 350860, 428506, 439923, 31425, 414337, 427407, 136861, 193935, 333855, 950037, 163714, 442103, 201436, 328827, 224806, 68490, 243522, 940680, 654647, 368513, 262932, 324650, 567723, 742479, 22949, 933093, 744171, 857770, 550971, 78559, 126501, 193626, 902600, 893564, 924239, 206343, 854784, 123636, 913670, 798611, 862820, 228727, 628641, 587495, 604327, 817704, 740890, 874399, 649952, 192924, 109076, 282977, 912737, 409000, 270887, 80126, 442491, 395905, 31009, 612803, 792975, 895218, 406429, 443758, 802984, 901724, 802151, 234426, 106590, 123117, 471111, 269997, 152353, 45343, 412368, 836767, 796714, 767231, 71510, 655362, 642679, 477245, 462456, 835040, 791595, 280595, 177341, 298330, 743550, 799914, 663654, 15647, 141240, 703560 };
            //var numbers = await fbService.RequestApiAsync();
            List<string> results = new List<string>();
            foreach (var x in numbers)
            {
                string result = string.Empty;
                if (x % 3 == 0)
                    result += "fizz";
                if (x % 5 == 0)
                    result += "buzz";

                results.Add(result);
            }

            //var list = new List<string>() {"fizz","buzz","fizzbuzz"};
            var jsonResult = JsonSerializer.Serialize(results);
            using (SHA256 mySHA256 = SHA256.Create())
            {
                var sha = mySHA256.ComputeHash(Encoding.UTF8.GetBytes(jsonResult));
                var hexa = ComputeHexa(sha);
                Console.WriteLine(hexa);
            }
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
        private readonly IHttpClientFactory _httpClientFactory;

        public FizzBuzzService(IHttpClientFactory httpClientFactory) =>
        _httpClientFactory = httpClientFactory;

        public async Task<IEnumerable<int>> RequestApiAsync()
        {
            var httpRequestMessage = new HttpRequestMessage(
            HttpMethod.Get,
            "https://codetest.jurosbaixos.com.br/v1/fizzbuzz")
            {
                Headers =
            {
                { "Accept", "application/json" },
                { "x-api-key", "SamirShowUsSomeNiceCode!" }
            }
            };

            var httpClient = _httpClientFactory.CreateClient();
            var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);

            httpResponseMessage.EnsureSuccessStatusCode();

            using var contentStream =
                await httpResponseMessage.Content.ReadAsStreamAsync();

            return await JsonSerializer.DeserializeAsync<IEnumerable<int>>(contentStream);
        }
    }
}
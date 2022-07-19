using FizzBuzzJB;
using FizzBuzzJB.Helpers;
using FizzBuzzJB.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Text.Json;

namespace FizzBuzz.Test
{
    public class FizzBuzzTest
    {
        private FizzBuzzService _fizzBuzzService;
        public FizzBuzzTest()
        {
            var host = new HostBuilder()
             .ConfigureServices(services =>
             {
                 services.AddHttpClient<FizzBuzzService>();
             })
             .Build();

            _fizzBuzzService = host.Services.GetRequiredService<FizzBuzzService>();
        }

        [Fact]
        public async Task GetFizzBuzzTest()
        {
            GeneralError erro;
            IEnumerable<int> numbers;
            (erro, numbers) = await _fizzBuzzService.RequestApiAsync();
            Assert.NotEmpty(numbers);
            Assert.Null(erro);
        }

        [Fact]
        public void GetSha256Test()
        {
            var hashResult = "c66a63862cf416c2acfe81ae697c066cff80b430af31fc9cae70957f355ded7d";
            var results = new List<string>() { "fizz", "buzz", "fizzbuzz" };
            var json = JsonSerializer.Serialize(results);
            var hash = Utils.GetHash(json.ToString());
            Assert.Equal(hashResult, hash);
        }
    }
}
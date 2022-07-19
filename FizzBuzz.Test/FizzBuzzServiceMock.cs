using FizzBuzzJB.Interfaces;
using FizzBuzzJB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FizzBuzz.Test
{
    public class FizzBuzzServiceMock : IFizzBuzzService
    {
        private readonly FizzBuzzMock _mock;

        public FizzBuzzServiceMock()
        {
            _mock = new FizzBuzzMock();
        }
        public async Task DeleteAsync(string hash)
        {
            await Task.CompletedTask; 
        }

        public async Task<(GeneralError?, string?)> PostHashAsync(string hash, string jsonResult)
        {
            return await Task.FromResult((new GeneralError(), "sucess"));
        }

        public async Task<(GeneralError?, IEnumerable<int>?)> RequestApiAsync()
        {
            return await Task.FromResult((new GeneralError(), _mock.FizzBuzzNumbers));
        }
    }
}

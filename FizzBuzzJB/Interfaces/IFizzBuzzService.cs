using FizzBuzzJB.Models;

namespace FizzBuzzJB.Interfaces
{
    public interface IFizzBuzzService
    {
        Task<(GeneralError?, IEnumerable<int>?)> RequestApiAsync();
        Task DeleteAsync(string hash);
        Task<(GeneralError?, string?)> PostHashAsync(string hash, string jsonResult);
    }
}
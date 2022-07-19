using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace FizzBuzzJB.Helpers
{
    public static class Utils
    {
        public static string GetHash(string jsonResult)
        {
            using (SHA256 mySHA256 = SHA256.Create())
            {
                var sha = mySHA256.ComputeHash(Encoding.UTF8.GetBytes(jsonResult));
                var hexa = ComputeHexa(sha);
                Console.WriteLine(hexa);
                return hexa;
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

        public static List<string> GetFizzBuzz(IEnumerable<int> numbers)
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
    }
}

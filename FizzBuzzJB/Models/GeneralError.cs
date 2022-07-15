namespace FizzBuzzJB.Models
{
    public class GeneralError
    {
        public string message { get; set; }
        ICollection<Error> errors {get;set;}
    }

    public class Error {
        public string message { get; set; }
        public string path { get; set; }
    }
}
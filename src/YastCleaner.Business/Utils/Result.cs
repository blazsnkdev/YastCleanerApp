namespace YastCleaner.Business.Utils
{
    public class Result
    {
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }

        public static Result Ok() => new Result { Success = true };
        public static Result Fail(string message) => new Result { Success = false, ErrorMessage = message };
    }

    public class Result<T> : Result
    {
        public T? Value { get; set; }

        public static Result<T> Ok(T value) =>
            new Result<T> { Success = true, Value = value };

        public static Result<T> Fail(string message) =>
            new Result<T> { Success = false, ErrorMessage = message };
    }
}

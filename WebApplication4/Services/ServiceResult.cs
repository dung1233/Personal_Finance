namespace WebApplication4.Services
{
    public class ServiceResult<T>
    {
        public bool IsSuccess { get; set; }
        public T? Data { get; set; }
        public string? ErrorMessage { get; set; }

        public static ServiceResult<T> Success(T data) => new() { IsSuccess = true, Data = data };
        public static ServiceResult<T> Failure(string errorMessage, string v) => new() { IsSuccess = false, ErrorMessage = errorMessage };
    }
}

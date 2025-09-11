namespace ThermocronWeb.Models;

public class ApiResult<T>
{
    public T? Data { get; set; }
    public bool IsSuccess { get; set; }
    public string? ErrorMessage { get; set; }
    public int? StatusCode { get; set; }

    public static ApiResult<T> Success(T data)
    {
        return new ApiResult<T>
        {
            Data = data,
            IsSuccess = true,
            ErrorMessage = null,
            StatusCode = 200
        };
    }

    public static ApiResult<T> Error(string errorMessage, int statusCode)
    {
        return new ApiResult<T>
        {
            Data = default(T),
            IsSuccess = false,
            ErrorMessage = errorMessage,
            StatusCode = statusCode
        };
    }
}
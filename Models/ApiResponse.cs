namespace UserManagementAPI.Models;

public class ApiResponse<T>
{
    public int StatusCode { get; set; }
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }

    public static ApiResponse<T> SuccessResponse(T data, string message = "Operation completed successfully", int statusCode = 200)
    {
        return new ApiResponse<T>
        {
            StatusCode = statusCode,
            Success = true,
            Message = message,
            Data = data
        };
    }

    public static ApiResponse<T> ErrorResponse(string message, int statusCode = 500, T? data = default)
    {
        return new ApiResponse<T>
        {
            StatusCode = statusCode,
            Success = false,
            Message = message,
            Data = data
        };
    }
}

public class ApiResponse
{
    public int StatusCode { get; set; }
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public object? Data { get; set; }

    public static ApiResponse SuccessResponse(object? data = null, string message = "Operation completed successfully", int statusCode = 200)
    {
        return new ApiResponse
        {
            StatusCode = statusCode,
            Success = true,
            Message = message,
            Data = data
        };
    }

    public static ApiResponse ErrorResponse(string message, int statusCode = 500, object? data = null)
    {
        return new ApiResponse
        {
            StatusCode = statusCode,
            Success = false,
            Message = message,
            Data = data
        };
    }
}
namespace UserManagementAPI.Models
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }

        public ApiResponse(bool success, string message, T data = default)
        {
            Success = success;
            Message = message;
            Data = data;
        }

        public static ApiResponse<T> SuccessResponse(string message, T data = default)
        {
            return new ApiResponse<T>(true, message, data);
        }

        public static ApiResponse<T> FailResponse(string message)
        {
            return new ApiResponse<T>(false, message);
        }
    }

    public class ApiResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }

        public ApiResponse(bool success, string message)
        {
            Success = success;
            Message = message;
        }

        public static ApiResponse SuccessResponse(string message)
        {
            return new ApiResponse(true, message);
        }

        public static ApiResponse FailResponse(string message)
        {
            return new ApiResponse(false, message);
        }
    }
}
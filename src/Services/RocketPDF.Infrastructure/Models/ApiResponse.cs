using System.Net;

namespace RocketPDF.Infrastructure.Models
{
    public class ApiResponse<T>
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public int ErrorCode { get; set; }
        public object Errors { get; set; }
        public T Data { get; set; }

        public ApiResponse(int code, int errorCode, object errors, string message, T data)
        {
            Code = code;
            Message = message;
            ErrorCode = errorCode;
            Errors = errors;
            Data = data;
        }
    }

    public static class ApiResponseWrapper
    {
        public static object Ok(dynamic data = default, string message = "Success")
        {
            return new
            {
                Success = true,
                StatusCode = HttpStatusCode.OK,
                Message = message,
                Data = data
            };
        }

        public static object Fail(dynamic data = default, string message = "Failed", int errorCode = -1)
        {
            return new
            {
                Success = false,
                StatusCode = HttpStatusCode.OK,
                Message = message,
                ErrorCode = errorCode,
                Data = data
            };
        }

        public static object NoContent(dynamic data = default, string message = "No content", int errorCode = -1)
        {
            return new
            {
                Success = true,
                StatusCode = HttpStatusCode.NoContent,
                Message = message,
                ErrorCode = errorCode,
                Data = data
            };
        }

        public static object BadRequest(dynamic data = default, string message = "Bad request", int errorCode = -1, object errors = default)
        {
            return new
            {
                Success = false,
                StatusCode = HttpStatusCode.BadRequest,
                Message = message,
                ErrorCode = errorCode,
                Errors = errors,
                Data = data
            };
        }

        public static object NotFound(dynamic data = default, string message = "Not found", int errorCode = -1)
        {
            return new
            {
                Success = false,
                StatusCode = HttpStatusCode.NotFound,
                Message = message,
                ErrorCode = errorCode,
                Data = data
            };
        }

        public static object Conflict(dynamic data = default, string message = "Confict", int errorCode = -1)
        {
            return new
            {
                Success = false,
                StatusCode = HttpStatusCode.Conflict,
                Message = message,
                ErrorCode = errorCode,
                Data = data
            };
        }

        public static object InternalServerError(dynamic error = default, Guid? errorId = null, string message = "Internal server error", int errorCode = -1)
        {
            return new
            {
                Success = false,
                StatusCode = HttpStatusCode.InternalServerError,
                Message = message,
                ErrorCode = errorCode,
                ErrorId = errorId,
                Error = error
            };
        }
    }
}
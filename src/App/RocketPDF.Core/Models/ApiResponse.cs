namespace RocketPDF.Core.Models
{
    public class BaseResponse
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public int ErrorCode { get; set; }
        public object Errors { get; set; }

        public bool IsSuccessStatusCode => Code == 200 || Code == 201 || Code == 204;
    }

    public class BaseResponse<T> : BaseResponse
    {
        public T Data { get; set; }

        public BaseResponse(T data, int code, string message, int errorCode, object errors)
        {
            Data = data;
            Code = code;
            Message = message;
            ErrorCode = errorCode;
            Errors = errors;
        }
    }
}

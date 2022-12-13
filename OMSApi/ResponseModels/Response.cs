namespace OMSApi.ResponseModels
{
    public class Response<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T Data { get; set; }

        public Response(bool success, string message, T data)
        {
            Success = success;
            Message = message;
            Data = data;
        }

        public Response(bool success, string message)
        {
            Success = success;
            Message = message;
        }

        public Response(T data)
        {
            Success = true;
            Data = data;
        }
    }
}
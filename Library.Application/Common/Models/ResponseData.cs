namespace Library.Application.Common.Models
{
    public class ResponseData<T>
    {
        public T? Data { get; set; }
        public bool Success {  get; set; }
        public string ErrorMessage {  get; set; }
        public ResponseData(T data)
        {
            Data = data;
            Success = true;
            ErrorMessage = "It's all right";
        }
        public ResponseData(bool success, string errorMessage)
        {
            Success = success;
            ErrorMessage = errorMessage;
        }
    }
}

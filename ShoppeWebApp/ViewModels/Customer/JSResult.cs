namespace ShoppeWebApp.ViewModels.Customer
{
    public class JSResult
    {
        public bool IsSuccess { get; set; }
        public object? Data { get; set; } = null!;
        public string? Message { get; set; } = null!;
        public JSResult(bool isSuccess, object? data, string? message = null)
        {
            IsSuccess = isSuccess;
            Data = data;
            Message = message;
        }
    }
}

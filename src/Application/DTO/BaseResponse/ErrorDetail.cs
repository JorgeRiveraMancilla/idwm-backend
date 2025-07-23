namespace Tienda_UCN_api.src.Application.DTO.BaseResponse
{
    public class ErrorDetail(string message, string? details = null)
    {
        public string Message { get; set; } = message;
        public string? Details { get; set; } = details;
    }
}

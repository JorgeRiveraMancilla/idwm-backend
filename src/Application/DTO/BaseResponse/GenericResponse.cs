using Tienda_UCN_api.src.Application.DTO.BaseResponse;

namespace Tienda_UCN_api.src.Application.DTO
{
    public class GenericResponse<T>(string message, T? data = default, ErrorDetail? error = null)
    {
        public string Message { get; set; } = message;
        public T? Data { get; set; } = data;
        public ErrorDetail? Error { get; set; } = error;
    }
}

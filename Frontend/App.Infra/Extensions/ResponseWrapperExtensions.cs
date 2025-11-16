using ABCSharedLibrary.Wrappers;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace App.Infra.Extensions
{
    public static class ResponseWrapperExtensions
    {
        public static async Task<IResponseWrapper<T>> WrapToResponse<T>(this HttpResponseMessage responseMessage)
        {
            var responseAsString = await responseMessage.Content.ReadAsStringAsync();
            var responseObject = JsonSerializer.Deserialize<ResponseWrapper<T>>(responseAsString, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                ReferenceHandler = ReferenceHandler.Preserve
            });

            return responseObject;
        }

        public static async Task<IResponseWrapper> WrapToResponse(this HttpResponseMessage responseMessage)
        {
            var responseAsString = await responseMessage.Content.ReadAsStringAsync();
            var responseObject = JsonSerializer.Deserialize<ResponseWrapper>(responseAsString, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                ReferenceHandler = ReferenceHandler.Preserve
            });

            return responseObject;
        }
    }
}

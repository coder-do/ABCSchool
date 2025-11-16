using ABCSharedLibrary.Models.Requests.Token;
using ABCSharedLibrary.Wrappers;

namespace App.Infra.Services.Identity
{
    public interface ITokenService
    {
        Task<IResponseWrapper> LoginAsync(string tenant, TokenRequest request);
        Task<IResponseWrapper> LogoutAsync();
        Task<string> RefreshTokenAsync();
        Task<string> TryForceRefreshTokenAsync();
    }
}

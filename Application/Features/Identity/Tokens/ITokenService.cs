using ABCSharedLibrary.Models.Requests.Token;
using ABCSharedLibrary.Models.Responses.Token;

namespace Application.Features.Identity.Tokens
{
    public interface ITokenService
    {
        Task<TokenResponse> Login(TokenRequest request);
        Task<TokenResponse> RefreshTokenAsync(RefreshTokenRequest request);
    }
}

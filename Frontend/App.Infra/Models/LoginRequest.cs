using ABCSharedLibrary.Models.Requests.Token;

namespace App.Infra.Models
{
    public class LoginRequest : TokenRequest
    {
        public string Tenant { get; set; }
    }
}

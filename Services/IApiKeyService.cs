using DAHApp.Models;

namespace DAHApp.Services
{
    public interface IApiKeyService
    {
        Task<ApiKeyAuthenticationModel> CreateApiKey(TokenRequestModel model);
    }
}

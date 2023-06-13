using Azure.Core;
using DAHApp.Contexts;
using DAHApp.Models;
using Microsoft.AspNetCore.Identity;

namespace DAHApp.Services
{
    public class ApiKeyService : IApiKeyService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ApiKeyService(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<ApiKeyAuthenticationModel> CreateApiKey(TokenRequestModel request)
        {
            var apiKeyAuthenticationModel = new ApiKeyAuthenticationModel();
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user == null)
            {
                apiKeyAuthenticationModel.Message = $"Bad credentials";

                return apiKeyAuthenticationModel;
            }

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);

            if (!isPasswordValid)
            {
                apiKeyAuthenticationModel.Message = $"Bad credentials";

                return apiKeyAuthenticationModel;
            }

            var userApiKey = new UserApiKey
            {
                User = user,
                Value = GenerateApiKeyValue()
            };

            _context.UserApiKeys.Add(userApiKey);

            _context.SaveChanges();

            apiKeyAuthenticationModel.IsAuthenticated = true;
            apiKeyAuthenticationModel.Email = user.Email;
            apiKeyAuthenticationModel.UserName = user.UserName;

            var rolesList = await _userManager.GetRolesAsync(user).ConfigureAwait(false);
            apiKeyAuthenticationModel.Roles = rolesList.ToList();
            apiKeyAuthenticationModel.UserApiKey = userApiKey;

            return apiKeyAuthenticationModel;
        }

        private string GenerateApiKeyValue() =>
            $"{Guid.NewGuid().ToString()}-{Guid.NewGuid().ToString()}";
    }
}

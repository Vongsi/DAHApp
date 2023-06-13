using System.Text.Json.Serialization;

namespace DAHApp.Models
{
    public class ApiKeyAuthenticationModel
    {
        public string Message { get; set; }
        public bool IsAuthenticated { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public List<string> Roles { get; set; }
        public UserApiKey UserApiKey { get; set; }
    }
}

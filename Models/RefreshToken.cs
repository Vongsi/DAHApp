using Microsoft.EntityFrameworkCore;

namespace DAHApp.Models
{
    //https://codewithmukesh.com/blog/refresh-tokens-in-aspnet-core/#What_are_Refresh_Tokens_%E2%80%93_The_Solution
    [Owned]
    public class RefreshToken
    {
        public string Token { get; set; }
        public DateTime Expires { get; set; }
        public bool IsExpired => DateTime.UtcNow >= Expires;
        public DateTime Created { get; set; }
        public DateTime? Revoked { get; set; }
        public bool IsActive => Revoked == null && !IsExpired;
    }
}

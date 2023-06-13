using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DAHApp.Models
{
    [Index(nameof(Value), IsUnique = true)]
    public class UserApiKey
    {
        [JsonIgnore]
        public int ID { get; set; }

        [Required]
        public string Value { get; set; }

        [JsonIgnore]
        [Required]
        public string UserID { get; set; }

        [JsonIgnore]
        public ApplicationUser User { get; set; }
    }
}
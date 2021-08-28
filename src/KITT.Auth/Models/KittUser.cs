using Microsoft.AspNetCore.Identity;

namespace KITT.Auth.Models
{
    public class KittUser : IdentityUser
    {
        public string TwitchChannel { get; set; }
    }
}

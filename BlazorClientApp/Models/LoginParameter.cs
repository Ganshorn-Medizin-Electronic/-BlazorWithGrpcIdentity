using IdentityService;
using System.ComponentModel.DataAnnotations;

namespace BlazorClientApp.Models
{
    public class LoginParameter
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }

    public static class LoginParameterExtensionMethods
    {
        public static LoginRequest GetLoginRequest(this LoginParameter input)
        {
            return new LoginRequest()
            {
                UserName = input.UserName,
                Password = input.Password,
                RememberMe = input.RememberMe
            };
        }
    }
}

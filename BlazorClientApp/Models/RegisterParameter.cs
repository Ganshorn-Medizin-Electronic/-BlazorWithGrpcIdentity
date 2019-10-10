using IdentityService;
using System.ComponentModel.DataAnnotations;

namespace BlazorClientApp.Models
{
    public class RegisterParameter
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        [Compare(nameof(Password), ErrorMessage = "Passwords do not match")]
        public string PasswordConfirm { get; set; }
    }

    public static class RegisterParametersExtensionMethods
    {
        public static RegisterParameterRequest GetRegisterRequest(this RegisterParameter input)
        {
            return new RegisterParameterRequest()
            {
                UserName = input.UserName,
                Password = input.Password
            };
        }
    }
}

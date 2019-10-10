using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using IdentityService.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Logging;

namespace IdentityService
{
    public class AutorizationService : AuthorizationService.AuthorizationServiceBase
    {
        private readonly ILogger _logger;

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AutorizationService(
            UserManager<ApplicationUser> userManager, 
            SignInManager<ApplicationUser> signInManager,
            ILogger<AutorizationService> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        public override async Task<LoginResult> Login(LoginRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Login request startet for user " + request.UserName);

            var user = await _userManager.FindByNameAsync(request.UserName);

            if (user == null)
            {
                _logger.LogWarning("Login request aborted, due to user is null");
                throw new RpcException(new Status(StatusCode.NotFound, "User does not exist"));
            }

            var singInResult = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);

            if (!singInResult.Succeeded)
            {
                _logger.LogWarning("Login request aborted, due to invalid password");
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid password"));
            }

            await _signInManager.SignInAsync(user, request.RememberMe);

            _logger.LogInformation($"User {user.UserName} successfully logged in.");

            var result = new LoginResult()
            {
                LoginAllowed = true
            };

            return result;
        }

        public override async Task<Empty> Logout(Empty request, ServerCallContext context)
        {
            var user = context.GetHttpContext().User;

            await _signInManager.SignOutAsync();

            _logger.LogInformation($"User {user.Identity.Name} uccessfully logged out.");

            return new Empty();
        }

        //[Authorize("Administrators")]
        public override async Task<Empty> Register(RegisterParameterRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Register request startet for user " + request.UserName);

            var user = new ApplicationUser();
            user.UserName = request.UserName;
            var result = await _userManager.CreateAsync(user, request.Password);
            
            if (!result.Succeeded)
            {
                _logger.LogWarning("Register request aborted, due to invalid password");
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid password"));
            }

            _logger.LogInformation($"User {user.UserName} successfully registered. Logging in after registration ...");

            await InternalLoginAsync(new LoginRequest()
            {
                UserName = request.UserName,
                Password = request.Password
            });

            return new Empty();
        }

        public override Task<UserInfoResult> GetUserInfo(Empty request, ServerCallContext context)
        {
            var user = context.GetHttpContext().User;

            _logger.LogInformation($"Getting user information for user {user.Identity.Name}...");

            return Task.Run(() => BuildUserInfo(user));
        }

        private async Task InternalLoginAsync(LoginRequest request)
        {
            var user = await _userManager.FindByNameAsync(request.UserName);

            _logger.LogInformation($"Try internal login for user {user.UserName}.");

            await _signInManager.SignInAsync(user, request.RememberMe);
        }

        private UserInfoResult BuildUserInfo(ClaimsPrincipal user)
        {
            if (user.Identity.Name != null)
            {
                var result =  new UserInfoResult
                {
                    IsAuthenticated = user.Identity.IsAuthenticated,
                    UserName = user.Identity.Name
                };

                var claims = user.Claims
                    //Optionally: filter the claims you want to expose to the client
                    //.Where(c => c.Type == "test-claim")
                    .ToDictionary(c => c.Type, c => c.Value);

                foreach (var claim in claims)
                {
                    result.ExposedClaims.Add(claim.Key, claim.Value);
                }

                return result;
            }
            else
            {
                return new UserInfoResult
                {
                    IsAuthenticated = false,
                    UserName = string.Empty,
                };
            }
        }
    }
}

using IdentityService;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BlazorClientApp.Provider
{
    public class GrpcAuthenticationStateProvider : AuthenticationStateProvider
    {
        private UserInfoResult _userInfoCache;
        private readonly AuthorizationController _controller;
        public GrpcAuthenticationStateProvider(AuthorizationController controller)
        {
            _controller = controller;
        }

        public async Task Login(LoginRequest loginParameters)
        {
            await _controller.Login(loginParameters);

            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        public async Task Register(RegisterParameterRequest registerParameters)
        {
            await _controller.Register(registerParameters);

            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        public async Task Logout()
        {
            await _controller.Logout();
            _userInfoCache = null;

            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var identity = new ClaimsIdentity();
            
            try
            {
                var userInfo = await GetUserInfo();

                if (userInfo.IsAuthenticated)
                {
                    var claims = new[] { new Claim(ClaimTypes.Name, userInfo.UserName) }.Concat(userInfo.ExposedClaims.Select(c => new Claim(c.Key, c.Value)));
                    identity = new ClaimsIdentity(claims, "Server authentication");
   
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine("Request failed:" + ex.ToString());
            }

            return await Task.FromResult(new AuthenticationState(new ClaimsPrincipal(identity)));
        }

        private async Task<UserInfoResult> GetUserInfo()
        {
            if (_userInfoCache != null && _userInfoCache.IsAuthenticated)
            {
                return _userInfoCache;
            }

            _userInfoCache = await _controller.GetUserInfo();

            return _userInfoCache;
        }
    }
}

using Google.Protobuf.WellKnownTypes;
using Grpc.Net.Client;
using IdentityService;
using System;
using System.Threading.Tasks;
using static IdentityService.AuthorizationService;

namespace BlazorClientApp.Provider
{
    public class AuthorizationController
    {
        private AuthorizationServiceClient _client;

        public AuthorizationController()
        {
#if DEBUG
            AppContext.SetSwitch(
                "System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
#endif
            var channel = GrpcChannel.ForAddress("http://localhost:6000");

            _client = new AuthorizationServiceClient(channel);
        }

        public async Task Login(LoginRequest loginParameters)
        {
            LoginResult result = await _client.LoginAsync(loginParameters);

            Console.WriteLine(result.LoginAllowed);
        }

        public async Task Register(RegisterParameterRequest registerParameters)
        {
            await _client.RegisterAsync(registerParameters);
        }

        public async Task Logout()
        {
            await _client.LogoutAsync(new Empty());
        }

        public async Task<UserInfoResult> GetUserInfo()
        {
            var userInfo = await _client.GetUserInfoAsync(new Empty());

            return userInfo;
        }
    }
}

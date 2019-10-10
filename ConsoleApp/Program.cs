using Google.Protobuf.WellKnownTypes;
using Grpc.Net.Client;
using IdentityService;
using System;
using System.Threading.Tasks;
using static IdentityService.AuthorizationService;

namespace ConsoleApp
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            // This switch must be set before creating the GrpcChannel/ HttpClient.
#if DEBUG
            AppContext.SetSwitch(
                "System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
#endif

            await TryAuthentication();

            Console.WriteLine("\nDone");
        }

        private static async Task TryAuthentication()
        {
            var channel = GrpcChannel.ForAddress("https://localhost:6001");

            var authorizationClient = new AuthorizationServiceClient(channel);

            var registerIdentity = new RegisterParameterRequest()
            {
                UserName = "admin",
                Password = "123456"
            };

            await authorizationClient.RegisterAsync(registerIdentity);

            Console.WriteLine("\nRegistration done");

            await authorizationClient.LogoutAsync(new Empty());

            Console.WriteLine("\nLogout done");

            var loginIdentity = new LoginRequest()
            {
                UserName = "admin",
                Password = "123456"
            };

            LoginResult login = await authorizationClient.LoginAsync(loginIdentity);

            Console.WriteLine("\nLogin done");

        }
    }
}

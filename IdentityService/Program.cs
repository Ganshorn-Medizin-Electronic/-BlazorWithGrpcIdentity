using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace IdentityService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            var logger = host.Services.GetRequiredService<ILogger<Program>>();
            logger.LogInformation("IdentityService startup ...");

            host.Run();
        }

        // Additional configuration is required to successfully run gRPC on macOS.
        // For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureKestrel(serverOptions =>
                    {
                        serverOptions.Listen(IPAddress.Any, 6000);
                        serverOptions.Listen(IPAddress.Any, 6001,
                            listenOptions =>
                            {
                                listenOptions.UseHttps();
                            });
                    });

                    webBuilder.ConfigureLogging((hostingContext, logging) =>
                           {
                               logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                               logging.ClearProviders();
                               logging.AddConsole();
                               //logging.AddDebug();
                               //logging.AddEventSourceLogger();
                           });

                    webBuilder.UseStartup<Startup>();
                });
    }
}

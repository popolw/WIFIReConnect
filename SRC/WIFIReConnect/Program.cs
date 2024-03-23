using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace WIFIReConnect
{
    internal class Program
    {
        async static Task Main(string[] args)
        {
               await Host.CreateDefaultBuilder(args)

              .ConfigureHostConfiguration(builder =>
              {
                  builder.AddCommandLine(args);
              })
              .ConfigureLogging(builder =>
              {
                  builder.AddLog4Net();
              })
             .ConfigureServices((context, services) =>
             {
                 var wifi = context.Configuration["WIFI"];
                 if (string.IsNullOrWhiteSpace(wifi))
                 {
                     var message = "缺少启动参数WIFI";
                     throw new ArgumentNullException("WIFI", message);
                 }
                 services.AddHostedService<WIFIService>();
             })
             .UseWindowsService()
             .Build()
             .RunAsync();
        }
    }
}

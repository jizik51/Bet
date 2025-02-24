using System;
using System.Net;

namespace Bet;

class Program
{
    public static void Main()
    {
        try
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2Support", true);
            System.Net.ServicePointManager.SecurityProtocol =
                SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;

            Console.WriteLine(Guid.NewGuid());

            CreateHostBuilder().Build().Run();
        }
        catch (System.Exception ex)
        {
            Console.WriteLine($"{ex.Message}");
        }
    }

    public static IHostBuilder CreateHostBuilder() =>
        Host.CreateDefaultBuilder()
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
                // webBuilder.UseHttpSys();
                // webBuilder.UseUrls("https://localhost:5001");
            });
}
// netsh http add sslcert ipport=0.0.0.0:5001 certhash=fa3d80ef7de1424f5bb5ba0b9b1236d28f4dad85 appid="{05060bc9-e6b6-4abd-a2c4-26688626c447}"
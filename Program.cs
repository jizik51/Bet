using System;

namespace Bet;

// builder.Services.AddRazorPages();

// app.UseHttpsRedirection();

// app.MapRazorPages();
// app.MapControllers();
// app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");

class Program
{
    public static void Main()
    {
        CreateHostBuilder().Build().Run();
    }

    public static IHostBuilder CreateHostBuilder() =>
        Host.CreateDefaultBuilder()
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
}

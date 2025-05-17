using Serilog;
using Microsoft.Extensions.Hosting.WindowsServices;
public class Program
{
    public static async Task Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.Console()
            .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger();

        try
        {
            Log.Information("Starting service");

            await Host.CreateDefaultBuilder(args)
                .UseWindowsService()  
                .UseSerilog()
                .ConfigureServices(services =>
                {
                    services.AddHostedService<FileMoverService.Worker>();
                })
                .Build()
                .RunAsync();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Service terminated unexpectedly");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}

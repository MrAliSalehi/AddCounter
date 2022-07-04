using AddCounter.Application.Services;
using AddCounter.Common.Extensions;
using AddCounter.Common.Models;
using Serilog;

var host = Host.CreateDefaultBuilder(args);


host.ConfigureAppConfiguration((context, builder) =>
{

    builder
   .SetBasePath(context.HostingEnvironment.ContentRootPath)
   .AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", false, true)
   .Build();
});

host.ConfigureServices((context, services) =>
{
    var config = context.Configuration.GetSection("BotConfigs");
    services.Configure<BotConfigs>(config);
    context.Configuration.Bind(config.Key, Globals.BotConfigs);
    Globals.Configuration = context.Configuration;
    services.AddHostedService<UpdateService>();
    services.AddHostedService<RemoveMessageService>();
});
try
{
    host.InjectSerilog();

    await host.Build().RunAsync();
}
catch (Exception e)
{
    Console.WriteLine(e);
    Log.Error(e, nameof(Program));
    Console.ReadKey();
}

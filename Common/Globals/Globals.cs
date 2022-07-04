global using AddCounter.Common.Globals;
using AddCounter.Common.Models;


namespace AddCounter.Common.Globals
{
    public static class Globals
    {
        public static IConfiguration Configuration { get; set; } = default!;
        public static BotConfigs BotConfigs { get; set; } = new();
        public static string ApplicationEnv => Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")!;

    }

}
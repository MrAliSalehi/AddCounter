global using AddCounter.Common.Globals;
using AddCounter.Common.Models;
using AddCounter.DataLayer.Models;


namespace AddCounter.Common.Globals
{
    public static class Globals
    {
        public static List<Group> ReadyToSaveGroups { get; set; } = new();
        public static List<long> Admins { get; } = new() { 5346262876 };
        public static IConfiguration Configuration { get; set; } = default!;
        public static BotConfigs BotConfigs { get; set; } = new();
        public static string ApplicationEnv => Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")!;
    }
}
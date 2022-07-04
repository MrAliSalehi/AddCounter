using Serilog;

namespace AddCounter.Common.Extensions
{
    public static class Logger
    {
        public static IHostBuilder InjectSerilog(this IHostBuilder builder)
        {
            if (!File.Exists("logs.txt"))
                File.Create("logs.txt").Close();
            try
            {
                builder.UseSerilog((_, lc) =>
                {
                    lc.WriteTo.File(new FileInfo("logs.txt").FullName);
                });

            }
            catch (Exception e)
            {
                Log.Error(e, nameof(InjectSerilog));
            }
            return builder;
        }
    }
}
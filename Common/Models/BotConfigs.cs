namespace AddCounter.Common.Models
{
    public class BotConfigs
    {
        public string Token { get; set; } = default!;
        public List<long> Admins { get; set; } = default!;
    }
}
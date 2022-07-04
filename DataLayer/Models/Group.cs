namespace AddCounter.DataLayer.Models;

public class Group
{
    public int Id { get; set; }
    public long GroupId { get; set; }
    public string WelcomeMessage { get; set; } = "";
    public int RequiredAddCount { get; set; }
    public long AddPrice { get; set; }
    public ushort MessageDeleteTimeInMinute { get; set; } = 5;
    public bool BotStatus { get; set; } = true;
    public bool HideName { get; set; } = false;
    public bool NotifyForAdd { get; set; } = true;
    public bool FakeDetection { get; set; } = false;
    public bool SayWelcome { get; set; } = true;
    public string PayAdmin { get; set; } = "";
}
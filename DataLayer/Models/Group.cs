namespace AddCounter.DataLayer.Models;

public class Group
{
    public int Id { get; set; }
    public long GroupId { get; set; }
    public string WelcomeMessage { get; set; } = "";
    public int RequiredAddCount { get; set; }
    public long AddPrice { get; set; }

}
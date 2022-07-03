namespace AddCounter.DataLayer.Models;

public class Group
{
    public int Id { get; set; }
    public long GroupId { get; set; }
    public string WelcomeMessage { get; set; } = "";
    public virtual ICollection<Link> GroupLinks { get; set; } = new List<Link>();
    public uint RequiredAddCount { get; set; }

}
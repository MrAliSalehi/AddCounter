namespace AddCounter.DataLayer.Models;

public class User
{
    public int Id { get; set; }
    public long UserId { get; set; }
    public long ChatId { get; set; }
    public uint AddCount { get; set; }
}
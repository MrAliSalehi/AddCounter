namespace AddCounter.DataLayer.Models;

public class Link
{
    public int Id { get; set; }
    public string Url { get; set; } = default!;

    public virtual Group Group { get; set; } = default!;
}
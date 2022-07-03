namespace AddCounter.DataLayer.Models;

internal class RemoveMessageModel
{
    public RemoveMessageModel(long chatId, int messageId, ushort timeToRemove = 5)
    {
        ChatId = chatId;
        MessageId = messageId;
        TimeToRemove = DateTime.Now + TimeSpan.FromMinutes(timeToRemove);
    }

    public int MessageId { get; }
    public long ChatId { get; }
    public DateTime TimeToRemove { get; }
}
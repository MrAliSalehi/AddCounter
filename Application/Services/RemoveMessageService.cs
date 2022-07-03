using AddCounter.DataLayer.Models;
using Telegram.Bot;

namespace AddCounter.Application.Services;

public class RemoveMessageService : BackgroundService
{
    internal static List<RemoveMessageModel> MessagesToRemove = new();
    internal static TelegramBotClient? client;
    private async Task CheckForMessagesAsync(CancellationToken ct = default)
    {
        if (client is null)
            return;

        var messagesToRemove = MessagesToRemove.Where(p => p.TimeToRemove <= DateTime.Now).ToList();
        foreach (var message in messagesToRemove)
        {
            await client.DeleteMessageAsync(message.ChatId, message.MessageId, ct);
            MessagesToRemove.Remove(message);
        }
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000 * 1, stoppingToken);
            await CheckForMessagesAsync(stoppingToken);
        }
    }
}
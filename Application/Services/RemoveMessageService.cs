using AddCounter.DataLayer.Models;
using Telegram.Bot;

namespace AddCounter.Application.Services;

public class RemoveMessageService : BackgroundService
{
    internal static readonly List<RemoveMessageModel> MessagesToRemove = new();
    private readonly TelegramBotClient _client;

    public RemoveMessageService()
    {
        _client = new TelegramBotClient(Globals.BotConfigs.Token);
    }
    private async Task CheckForMessagesAsync(CancellationToken ct = default)
    {
        if (!MessagesToRemove.Any())
            return;



        var messagesToRemove = MessagesToRemove.Where(p => p.TimeToRemove <= DateTime.Now).ToList();
        foreach (var message in messagesToRemove)
        {
            await _client.DeleteMessageAsync(message.ChatId, message.MessageId, ct);
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
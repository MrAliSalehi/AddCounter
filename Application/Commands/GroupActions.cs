using AddCounter.Application.Services;
using AddCounter.DataLayer.Models;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using User = Telegram.Bot.Types.User;

namespace AddCounter.Application.Commands;

public static class GroupActions
{
    public static async Task ActionSayWelcomeAsync(this ITelegramBotClient client, User[] newUsers, Group group, CancellationToken ct = default)
    {
        foreach (var user in newUsers)
        {
            var name = user.Username is null or "" ? user.FirstName : $"@{user.Username}";
            var message = group.WelcomeMessage is null or "" ? "Welcome" : group.WelcomeMessage;
            var msg1 = await client.SendTextMessageAsync(group.GroupId, $"[{name}](tg://user?id={user.Id})\n{message}", ParseMode.MarkdownV2, cancellationToken: ct);
            RemoveMessageService.MessagesToRemove.Add(new RemoveMessageModel(group.GroupId, msg1.MessageId, group.MessageDeleteTimeInMinute));
        }
    }
}
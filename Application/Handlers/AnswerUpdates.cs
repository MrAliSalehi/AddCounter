using AddCounter.Application.Commands;
using AddCounter.DataLayer.Controllers;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using User = Telegram.Bot.Types.User;

namespace AddCounter.Application.Handlers;

internal static class AnswerUpdates
{
    internal static async Task HandleMessagesAsync(this ITelegramBotClient client, Message message, CancellationToken ct = default)
    {
        if (message.Text is null)
            return;

        if (message.From is null)
            return;

        if (!Globals.Admins.Contains(message.From.Id))
            return;

        var action = message.Text switch
        {
            "check" => Task.Run(() => AdminCommands.ValidateGroupAsync(client, message, ct), ct),
            "create" => client.CommandCreateGroupAsync(message, ct),
            "info" => client.CommandInfoOfAddAsync(message, ct),
            "res usr" => client.CommandResetUserAddsAsync(message, ct),
            "res gp" => client.CommandResetGroupSettingAsync(message, ct),
            "gp id" => client.CommandGroupIdAsync(message, ct),
            { } msg when (msg.StartsWith("wlc")) => client.CommandSetWelcomeMessageAsync(message, ct),
            { } msg when (msg.StartsWith("count")) => client.CommandSetAddCountAsync(message, ct),
            { } msg when (msg.StartsWith("price")) => client.CommandSetAddPriceAsync(message, ct),
            { } msg when (msg.StartsWith("del tm")) => client.CommandSetMessageDeleteTimeAsync(message, ct),

            _ => Task.CompletedTask
        };
        await action.ConfigureAwait(false);
    }

    internal static async Task HandleChatMemberAsync(this ITelegramBotClient client, User[] newUsers, User? from, long chatId, CancellationToken ct = default)
    {
        var group = await GroupController.GetGroupAsync(chatId, ct);
        if (group is null)
            return;

        foreach (var user in newUsers)
        {
            var name = user.Username is null or "" ? user.FirstName : $"@{user.Username}";
            var welcomeMessage = group.WelcomeMessage is null or "" ? "Welcome" : group.WelcomeMessage;
            var msg1 = await client.SendTextMessageAsync(chatId, $"{name}\n{welcomeMessage}", cancellationToken: ct);

        }
        if (from is not null && newUsers.All(p => p.Id != from.Id))
        {
            var fromName = from.Username is null or "" ? from.FirstName : $"@{from.Username}";
            var getUser = await UserController.GetUserAsync(from.Id, chatId, ct);
            var result = await UserController.UpdateUserAsync(from.Id, chatId, p =>
            {
                p.AddCount += Convert.ToUInt32(newUsers.Length);
            }, ct);
            var totalAdds = getUser.AddCount + newUsers.Length;
            var totalPrice = group.AddPrice * totalAdds;
            var response = result switch
            {
                0 => $"User {fromName} in Group[<i>{getUser.ChatId}</i>]\n" +
                    $"You Added <b>{getUser.AddCount}</b> Members Before.\n" +
                    $"Now You Added <b>{newUsers.Length}</b> More Users\n" +
                    $"You Need <b>{group.RequiredAddCount - (totalAdds)}</b> More To Get Paid.\n" +
                    $"Your Total Add Price until now Is [<b>${totalPrice}</b>]",
                _ => "Cant Store Your Data. Please Contact Admins!"
            };
            await client.SendTextMessageAsync(chatId, response, ParseMode.Html, cancellationToken: ct);
        }
    }
}
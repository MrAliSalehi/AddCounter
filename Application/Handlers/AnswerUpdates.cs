using AddCounter.Application.Commands;
using AddCounter.Application.Services;
using AddCounter.DataLayer.Controllers;
using AddCounter.DataLayer.Models;
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

        if (!Globals.BotConfigs.Admins.Contains(message.From.Id))
            return;

        var action = message.Text switch
        {
            "check" => Task.Run(() => AdminCommands.ValidateGroupAsync(client, message, ct), ct),
            "create" => client.CommandCreateGroupAsync(message, ct),
            "delete" => client.CommandDeleteGroupAsync(message, ct),
            "off" or "on" => client.CommandBotStateAsync(message, ct),
            "info" => client.CommandInfoOfAddAsync(message, ct),
            "res usr" => client.CommandResetUserAddsAsync(message, ct),
            "res gp" => client.CommandResetGroupSettingAsync(message, ct),
            "gp id" => client.CommandGroupIdAsync(message, ct),
            "hide name" or "show name" => client.CommandUserNameVisibilityAsync(message, ct),
            "dis n" or "en n" => client.CommandAddNotificationControlAsync(message, ct),
            "fake on" or "fake off" => client.CommandFakeDetectionAsync(message, ct),
            "wlc off" or "wlc on" => client.CommandWelcomeControlAsync(message, ct),
            { } msg when (msg.StartsWith("wlc")) => client.CommandSetWelcomeMessageAsync(message, ct),
            { } msg when (msg.StartsWith("count")) => client.CommandSetAddCountAsync(message, ct),
            { } msg when (msg.StartsWith("price")) => client.CommandSetAddPriceAsync(message, ct),
            { } msg when (msg.StartsWith("del tm")) => client.CommandSetMessageDeleteTimeAsync(message, ct),
            { } msg when (msg.StartsWith("payadmin")) => client.CommandSetPayAdminAsync(message, ct),

            _ => Task.CompletedTask
        };
        await action.ConfigureAwait(false);
    }

    internal static async Task HandleChatMemberAsync(this ITelegramBotClient client, User[] newUsers, User? from, long chatId, string chatName, CancellationToken ct = default)
    {
        var group = await GroupController.GetGroupAsync(chatId, ct);
        if (group is null)
            return;
        if (!group.BotStatus)
            return;

        if (group.SayWelcome)
        {
            await client.ActionSayWelcomeAsync(newUsers, group, ct);
        }

        if (from is not null && newUsers.All(p => p.Id != from.Id))
        {
            var fromName = from.Username is null or "" ? from.FirstName : $"@{from.Username}";
            if (group.HideName)
                fromName = "نام_مخفی_است";
            var getUser = await UserController.GetUserAsync(from.Id, chatId, ct);

            var totalNewAdds = group.FakeDetection ? newUsers.Count(user => !user.IsBot) : newUsers.Length;

            var result = await UserController.UpdateUserAsync(from.Id, chatId, p =>
            {
                p.AddCount += Convert.ToUInt32(totalNewAdds);
            }, ct);

            if (!group.NotifyForAdd)
                return;

            var totalAdds = getUser.AddCount + totalNewAdds;
            var totalPrice = group.AddPrice * totalAdds;
            var response = result switch
            {
                0 => $"کاربر {fromName} در گروه[<i>{chatName}</i>]\n" +
                    $"شما <b>{getUser.AddCount}</b> نفر را از قبل اد کرده بودید\n" +
                    $"الان <b>{totalNewAdds}</b> نفر اد کردید\n" +
                    $"شما به <b>{group.RequiredAddCount - (totalAdds)}</b> نفر دیگر نیاز دارید تا درخاست تسویه بدهید\n" +
                    $"ارزش کل اد های شما [<b>تومان {totalPrice}</b>] میباشد",
                _ => "Cant Store Your Data. Please Contact Admins!"
            };
            var msg2 = await client.SendTextMessageAsync(chatId, response, ParseMode.Html, cancellationToken: ct);
            RemoveMessageService.MessagesToRemove.Add(new RemoveMessageModel(chatId, msg2.MessageId, group.MessageDeleteTimeInMinute));

        }
    }
}
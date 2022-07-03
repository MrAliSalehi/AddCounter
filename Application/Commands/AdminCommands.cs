using AddCounter.DataLayer.Controllers;
using Serilog;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace AddCounter.Application.Commands;

internal static class AdminCommands
{
    public static async Task CommandCreateGroupAsync(this ITelegramBotClient client, Message message, CancellationToken ct = default)
    {
        var getGroup = await GroupController.GetGroupAsync(message.Chat.Id, ct);
        if (getGroup is not null)
        {
            _ = await client.SendTextMessageAsync(message.Chat.Id, "<i>Group Is Already Created</i>", ParseMode.Html, cancellationToken: ct).ConfigureAwait(false);
            return;
        }

        var result = await GroupController.AddGroupAsync(message.Chat.Id, ct);
        var response = result switch
        {
            0 => "Group Created SuccessFully",
            _ => "Can't Create Group Right Now!",
        };
        _ = await client.SendTextMessageAsync(message.Chat.Id, $"<i>{response}</i>", ParseMode.Html, cancellationToken: ct).ConfigureAwait(false);
    }
    public static async Task CommandSetWelcomeMessageAsync(this ITelegramBotClient client, Message message, CancellationToken ct = default)
    {
        var validate = await ValidateGroupAsync(client, message, ct);
        if (!validate)
            return;

        var welcomeMessage = message.Text?.Replace("wlc ", "");
        if (welcomeMessage is null or "")
        {
            _ = await client.SendTextMessageAsync(message.Chat.Id, "Welcome Message Cannot be Empty",
                cancellationToken: ct);
            return;
        }

        var updateResult =
        await GroupController.UpdateGroupAsync(p => p.WelcomeMessage = welcomeMessage, message.Chat.Id, ct);
        var response = updateResult switch
        {
            0 => "Group Updated SuccessFully.",
            _ => "Cannot Update The Group."
        };
        _ = await client.SendTextMessageAsync(message.Chat.Id, $"<i>{response}</i>", ParseMode.Html, cancellationToken: ct);
    }
    public static async Task CommandSetAddCountAsync(this ITelegramBotClient client, Message message, CancellationToken ct = default)
    {
        var validate = await ValidateGroupAsync(client, message, ct);
        if (!validate)
            return;
        var isNumber = int.TryParse(message.Text?.Replace("count ", ""), out var addCount);
        if (!isNumber)
        {
            _ = await client.SendTextMessageAsync(message.Chat.Id, "Add number Cannot Be Empty", cancellationToken: ct);
            return;
        }

        var updateResult = await GroupController.UpdateGroupAsync(p => p.RequiredAddCount = addCount, message.Chat.Id, ct);
        var response = updateResult switch
        {
            0 => "Group Updated SuccessFully",
            _ => "Cannot Update The Group!",
        };
        _ = await client.SendTextMessageAsync(message.Chat.Id, $"<i>{response}</i>", ParseMode.Html, cancellationToken: ct).ConfigureAwait(false);
    }
    public static async Task CommandInfoOfAddAsync(this ITelegramBotClient client, Message message, CancellationToken ct = default)
    {
        var validate = await ValidateGroupAsync(client, message, ct);
        if (!validate)
            return;
        var users = await UserController.GetUserByGroupIdAsync(message.Chat.Id, ct);
        uint totalAdds = 0;
        var builder = new StringBuilder();

        foreach (var user in users)
        {
            builder.Append($"User [{user.UserId}] Added [<b>{user.AddCount}</b>]\n");
            totalAdds += user.AddCount;
        }

        builder.Append($"\nTotal Adds:{totalAdds}");

        await client.SendTextMessageAsync(message.Chat.Id, "<i>Details Has Been Sent To Your Pv.</i>", ParseMode.Html,
            replyToMessageId: message.MessageId, cancellationToken: ct).ConfigureAwait(false);

        await client.SendTextMessageAsync(message.From!.Id, builder.ToString(), ParseMode.Html, cancellationToken: ct).ConfigureAwait(false);

        await client.DeleteMessageAsync(message.Chat.Id, message.MessageId, ct).ConfigureAwait(false);
    }
    public static async Task CommandResetUserAddsAsync(this ITelegramBotClient client, Message message, CancellationToken ct = default)
    {
        var validate = await ValidateGroupAsync(client, message, ct);
        if (!validate)
            return;
        var result = await UserController.ResetAllUserAddsAsync(message.Chat.Id, ct);
        var response = result switch
        {
            0 => "Users Has Been Reset SuccessFully",
            _ => "Cant Reset Users"
        };
        await client.SendTextMessageAsync(message.Chat.Id, response, cancellationToken: ct).ConfigureAwait(false);
    }
    public static async Task CommandResetGroupSettingAsync(this ITelegramBotClient client, Message message, CancellationToken ct = default)
    {
        var validate = await ValidateGroupAsync(client, message, ct);
        if (!validate)
            return;

        var result = await GroupController.ResetGroupSettingAsync(message.Chat.Id, ct);
        var response = result switch
        {
            0 => "Group Reset SuccessFully",
            1 => "Group Is Not Created",
            _ => "Cant Reset Group!"
        };
        await client.SendTextMessageAsync(message.Chat.Id, response, cancellationToken: ct).ConfigureAwait(false);
    }
    public static async Task CommandGroupIdAsync(this ITelegramBotClient client, Message message, CancellationToken ct = default)
    {
        try
        {
            await client.SendTextMessageAsync(message.Chat.Id, $"GroupID:[<i>{message.Chat.Id}</i>]", ParseMode.Html, cancellationToken: ct);
        }
        catch (Exception e)
        {
            Log.Error(e, nameof(CommandGroupIdAsync));
        }
    }
    public static async Task CommandSetAddPriceAsync(this ITelegramBotClient client, Message message, CancellationToken ct = default)
    {
        var validate = await ValidateGroupAsync(client, message, ct);
        if (!validate)
            return;

        var isNumber = long.TryParse(message.Text?.Replace("price ", ""), out var price);
        if (!isNumber)
        {
            await client.SendTextMessageAsync(message.Chat.Id, "Price Cannot Be Empty", cancellationToken: ct);
            return;
        }

        var result = await GroupController.UpdateGroupAsync(p => p.AddPrice = price, message.Chat.Id, ct);
        var response = result switch
        {
            0 => "Price Updated SuccessFully",
            _ => "Cannot Edit Price"
        };
        await client.SendTextMessageAsync(message.Chat.Id, response, cancellationToken: ct).ConfigureAwait(false);

    }
    public static async ValueTask<bool> ValidateGroupAsync(ITelegramBotClient client, Message message, CancellationToken ct = default)
    {
        var getGroup = await GroupController.GetGroupAsync(message.Chat.Id, ct);
        if (getGroup is not null)
            return true;
        _ = await client.SendTextMessageAsync(message.Chat.Id, "Group Is Not Created.\nPlease Create It First.",
            cancellationToken: ct);
        return false;

    }
}
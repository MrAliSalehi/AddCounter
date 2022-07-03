﻿using AddCounter.DataLayer.Controllers;
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
        var isNumber = uint.TryParse(message.Text?.Replace("count ", ""), out var addCount);
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
    public static async Task CommandAddLinkAsync(this ITelegramBotClient client, Message message, CancellationToken ct = default)
    {
        var validate = await ValidateGroupAsync(client, message, ct);
        if (!validate)
            return;
        var link = message.Text?.Replace("add link ", "");
        if (link is null or "")
        {
            _ = await client.SendTextMessageAsync(message.Chat.Id, "Link Cannot Be Empty", cancellationToken: ct);
            return;
        }

        var result = await LinkController.AddLinkAsync(message.Chat.Id, link, ct);
        var response = result switch
        {
            0 => "Link Added SuccessFully",
            1 => "Link Already Exists",
            _ => "Cannot Add Link!"
        };
        _ = await client.SendTextMessageAsync(message.Chat.Id, $"<i>{response}</i>", ParseMode.Html, cancellationToken: ct).ConfigureAwait(false);
    }
    public static async Task CommandRemoveLinkAsync(this ITelegramBotClient client, Message message, CancellationToken ct = default)
    {
        var validate = await ValidateGroupAsync(client, message, ct);
        if (!validate)
            return;

        var link = message.Text?.Replace("rem link ", "");
        if (link is null or "")
        {
            _ = await client.SendTextMessageAsync(message.Chat.Id, "Link Cannot Be Empty", cancellationToken: ct);
            return;
        }
        var result = await LinkController.RemoveLinkAsync(message.Chat.Id, link, ct);
        var response = result switch
        {
            0 => "Link Removed SuccessFully",
            1 => "Not Found",
            _ => "Cannot Remove The Link!"
        };
        await client.SendTextMessageAsync(message.Chat.Id, $"<i>{response}</i>", ParseMode.Html, cancellationToken: ct).ConfigureAwait(false);
    }

    public static async Task CommandCheckGroupsAsync(this ITelegramBotClient client, Message message, CancellationToken ct = default)
    {
        var getGroupsWithLink = await LinkController.GetAllLinksAsync(message.Chat.Id, ct);
        var builder = new StringBuilder();

        foreach (var link in getGroupsWithLink)
        {
            builder.Append($"{link.Id}.{link.Url}");
        }

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
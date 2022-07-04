using AddCounter.Application.Services;
using AddCounter.DataLayer.Controllers;
using AddCounter.DataLayer.Models;
using Serilog;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace AddCounter.Application.Commands;

internal static class AdminCommands
{
    public static async Task CommandSetMessageDeleteTimeAsync(this ITelegramBotClient client, Message message, CancellationToken ct = default)
    {
        try
        {
            var validate = await ValidateGroupAsync(client, message, ct);
            if (validate is null)
                return;
            var canParse = ushort.TryParse(message.Text?.Replace("del tm ", ""), out var time);
            if (!canParse)
            {
                var msg1 = await client.SendTextMessageAsync(message.Chat.Id, "Time Cannot be Empty", cancellationToken: ct);
                RemoveMessageService.MessagesToRemove.Add(new RemoveMessageModel(message.Chat.Id, msg1.MessageId));
                return;
            }

            var result = await GroupController.UpdateGroupAsync(p => p.MessageDeleteTimeInMinute = time, message.Chat.Id, ct);
            var response = result switch
            {
                0 => "Time Updated SuccessFully",
                _ => "Cannot Update Time."
            };
            var msg2 = await client.SendTextMessageAsync(message.Chat.Id, response, cancellationToken: ct).ConfigureAwait(false);
            RemoveMessageService.MessagesToRemove.Add(new RemoveMessageModel(message.Chat.Id, msg2.MessageId, time));

        }
        catch (Exception e)
        {
            Log.Error(e, nameof(CommandSetMessageDeleteTimeAsync));
        }

    }
    public static async Task CommandCreateGroupAsync(this ITelegramBotClient client, Message message, CancellationToken ct = default)
    {
        try
        {
            var getGroup = await GroupController.GetGroupAsync(message.Chat.Id, ct);
            if (getGroup is not null)
            {
                var msg1 = await client.SendTextMessageAsync(message.Chat.Id, "<i>Group Is Already Created</i>", ParseMode.Html, cancellationToken: ct).ConfigureAwait(false);
                RemoveMessageService.MessagesToRemove.Add(new RemoveMessageModel(message.Chat.Id, msg1.MessageId, getGroup.MessageDeleteTimeInMinute));
                return;
            }

            var result = await GroupController.AddGroupAsync(message.Chat.Id, ct);
            var response = result switch
            {
                0 => "Group Created SuccessFully",
                _ => "Can't Create Group Right Now!",
            };
            var msg2 = await client.SendTextMessageAsync(message.Chat.Id, $"<i>{response}</i>", ParseMode.Html, cancellationToken: ct).ConfigureAwait(false);
            RemoveMessageService.MessagesToRemove.Add(new RemoveMessageModel(message.Chat.Id, msg2.MessageId));
        }
        catch (Exception e)
        {
            Log.Error(e, nameof(CommandCreateGroupAsync));
        }


    }
    public static async Task CommandSetWelcomeMessageAsync(this ITelegramBotClient client, Message message, CancellationToken ct = default)
    {
        try
        {
            var validate = await ValidateGroupAsync(client, message, ct);
            if (validate is null)
                return;

            var welcomeMessage = message.Text?.Replace("wlc ", "");
            if (welcomeMessage is null or "")
            {
                var msg1 = await client.SendTextMessageAsync(message.Chat.Id, "Welcome Message Cannot be Empty", cancellationToken: ct);
                RemoveMessageService.MessagesToRemove.Add(new RemoveMessageModel(message.Chat.Id, msg1.MessageId, validate.MessageDeleteTimeInMinute));

                return;
            }

            var updateResult =
                await GroupController.UpdateGroupAsync(p => p.WelcomeMessage = welcomeMessage, message.Chat.Id, ct);
            var response = updateResult switch
            {
                0 => "Group Updated SuccessFully.",
                _ => "Cannot Update The Group."
            };
            var msg2 = await client.SendTextMessageAsync(message.Chat.Id, $"<i>{response}</i>", ParseMode.Html, cancellationToken: ct);
            RemoveMessageService.MessagesToRemove.Add(new RemoveMessageModel(message.Chat.Id, msg2.MessageId, validate.MessageDeleteTimeInMinute));

        }
        catch (Exception e)
        {
            Log.Error(e, nameof(CommandSetWelcomeMessageAsync));
        }

    }
    public static async Task CommandSetAddCountAsync(this ITelegramBotClient client, Message message, CancellationToken ct = default)
    {
        try
        {
            var validate = await ValidateGroupAsync(client, message, ct);
            if (validate is null)
                return;
            var isNumber = int.TryParse(message.Text?.Replace("count ", ""), out var addCount);
            if (!isNumber)
            {
                var msg1 = await client.SendTextMessageAsync(message.Chat.Id, "Add number Cannot Be Empty", cancellationToken: ct);
                RemoveMessageService.MessagesToRemove.Add(new RemoveMessageModel(message.Chat.Id, msg1.MessageId, validate.MessageDeleteTimeInMinute));

                return;
            }

            var updateResult = await GroupController.UpdateGroupAsync(p => p.RequiredAddCount = addCount, message.Chat.Id, ct);
            var response = updateResult switch
            {
                0 => "Group Updated SuccessFully",
                _ => "Cannot Update The Group!",
            };
            var msg2 = await client.SendTextMessageAsync(message.Chat.Id, $"<i>{response}</i>", ParseMode.Html, cancellationToken: ct).ConfigureAwait(false);
            RemoveMessageService.MessagesToRemove.Add(new RemoveMessageModel(message.Chat.Id, msg2.MessageId, validate.MessageDeleteTimeInMinute));

        }
        catch (Exception e)
        {
            Log.Error(e, nameof(CommandSetAddCountAsync));
        }

    }
    public static async Task CommandInfoOfAddAsync(this ITelegramBotClient client, Message message, CancellationToken ct = default)
    {
        try
        {
            var validate = await ValidateGroupAsync(client, message, ct);
            if (validate is null)
                return;
            var users = await UserController.GetUserByGroupIdAsync(message.Chat.Id, ct);
            uint totalAdds = 0;
            var builder = new StringBuilder();
            builder.Append($"Information For Group [{message.Chat.Title}]({message.Chat.Id})\n");
            foreach (var user in users)
            {
                builder.Append($"User [{user.UserId}] Added [<b>{user.AddCount}</b>]\n");
                totalAdds += user.AddCount;
            }

            builder.Append($"\nTotal Adds:{totalAdds}");

            var msg1 = await client.SendTextMessageAsync(message.Chat.Id, "<i>Details Has Been Sent To Your Pv.</i>", ParseMode.Html,
                replyToMessageId: message.MessageId, cancellationToken: ct).ConfigureAwait(false);

            await client.SendTextMessageAsync(message.From!.Id, builder.ToString(), ParseMode.Html, cancellationToken: ct).ConfigureAwait(false);

            await client.DeleteMessageAsync(message.Chat.Id, message.MessageId, ct).ConfigureAwait(false);

            RemoveMessageService.MessagesToRemove.Add(new RemoveMessageModel(message.Chat.Id, msg1.MessageId, validate.MessageDeleteTimeInMinute));
        }
        catch (Exception e)
        {
            Log.Error(e, nameof(CommandInfoOfAddAsync));
        }

    }
    public static async Task CommandResetUserAddsAsync(this ITelegramBotClient client, Message message, CancellationToken ct = default)
    {
        try
        {
            var validate = await ValidateGroupAsync(client, message, ct);
            if (validate is null)
                return;
            var result = await UserController.ResetAllUserAddsAsync(message.Chat.Id, ct);
            var response = result switch
            {
                0 => "Users Has Been Reset SuccessFully",
                _ => "Cant Reset Users"
            };
            var msg1 = await client.SendTextMessageAsync(message.Chat.Id, response, cancellationToken: ct).ConfigureAwait(false);
            RemoveMessageService.MessagesToRemove.Add(new RemoveMessageModel(message.Chat.Id, msg1.MessageId, validate.MessageDeleteTimeInMinute));

        }
        catch (Exception e)
        {
            Log.Error(e, nameof(CommandResetUserAddsAsync));
        }

    }
    public static async Task CommandResetGroupSettingAsync(this ITelegramBotClient client, Message message, CancellationToken ct = default)
    {
        try
        {
            var validate = await ValidateGroupAsync(client, message, ct);
            if (validate is null)
                return;

            var result = await GroupController.ResetGroupSettingAsync(message.Chat.Id, ct);
            var response = result switch
            {
                0 => "Group Reset SuccessFully",
                1 => "Group Is Not Created",
                _ => "Cant Reset Group!"
            };
            var msg1 = await client.SendTextMessageAsync(message.Chat.Id, response, cancellationToken: ct).ConfigureAwait(false);
            RemoveMessageService.MessagesToRemove.Add(new RemoveMessageModel(message.Chat.Id, msg1.MessageId, validate.MessageDeleteTimeInMinute));
        }
        catch (Exception e)
        {
            Log.Error(e, nameof(CommandResetGroupSettingAsync));
        }


    }
    public static async Task CommandGroupIdAsync(this ITelegramBotClient client, Message message, CancellationToken ct = default)
    {
        try
        {
            var msg1 = await client.SendTextMessageAsync(message.Chat.Id, $"GroupID:[<i>{message.Chat.Id}</i>]", ParseMode.Html, cancellationToken: ct);
            RemoveMessageService.MessagesToRemove.Add(new RemoveMessageModel(message.Chat.Id, msg1.MessageId));

        }
        catch (Exception e)
        {
            Log.Error(e, nameof(CommandGroupIdAsync));
        }
    }
    public static async Task CommandSetAddPriceAsync(this ITelegramBotClient client, Message message, CancellationToken ct = default)
    {
        try
        {
            var validate = await ValidateGroupAsync(client, message, ct);
            if (validate is null)
                return;

            var isNumber = long.TryParse(message.Text?.Replace("price ", ""), out var price);
            if (!isNumber)
            {
                var msg1 = await client.SendTextMessageAsync(message.Chat.Id, "Price Cannot Be Empty", cancellationToken: ct);
                RemoveMessageService.MessagesToRemove.Add(new RemoveMessageModel(message.Chat.Id, msg1.MessageId, validate.MessageDeleteTimeInMinute));

                return;
            }

            var result = await GroupController.UpdateGroupAsync(p => p.AddPrice = price, message.Chat.Id, ct);
            var response = result switch
            {
                0 => "Price Updated SuccessFully",
                _ => "Cannot Edit Price"
            };
            var msg2 = await client.SendTextMessageAsync(message.Chat.Id, response, cancellationToken: ct).ConfigureAwait(false);
            RemoveMessageService.MessagesToRemove.Add(new RemoveMessageModel(message.Chat.Id, msg2.MessageId, validate.MessageDeleteTimeInMinute));
        }
        catch (Exception e)
        {
            Log.Error(e, nameof(CommandSetAddPriceAsync));
        }


    }
    public static async Task CommandDeleteGroupAsync(this ITelegramBotClient client, Message message, CancellationToken ct = default)
    {
        try
        {
            var validate = await ValidateGroupAsync(client, message, ct);
            if (validate is null)
                return;

            var result = await GroupController.RemoveGroupAsync(message.Chat.Id, ct);
            var response = result switch
            {
                0 => "Group Removed SuccessFully",
                1 => "Group Is Not Created!",
                _ => "Cannot Remove Group!"
            };
            var msg1 = await client.SendTextMessageAsync(message.Chat.Id, response, cancellationToken: ct);
            RemoveMessageService.MessagesToRemove.Add(new RemoveMessageModel(message.Chat.Id, msg1.MessageId, validate.MessageDeleteTimeInMinute));
        }
        catch (Exception e)
        {
            Log.Error(e, nameof(CommandDeleteGroupAsync));
        }
    }
    public static async Task CommandBotStateAsync(this ITelegramBotClient client, Message message, CancellationToken ct = default)
    {
        try
        {
            var validate = await ValidateGroupAsync(client, message, ct);
            if (validate is null)
                return;
            var result = await GroupController.UpdateGroupAsync(p =>
            {
                p.BotStatus = message.Text is not "off" and "on";

            }, message.Chat.Id, ct);
            var response = result switch
            {
                0 => "Bot Updated",
                _ => "Cannot Update The Bot Status"
            };
            var msg1 = await client.SendTextMessageAsync(message.Chat.Id, response, cancellationToken: ct);
            RemoveMessageService.MessagesToRemove.Add(new RemoveMessageModel(message.Chat.Id, msg1.MessageId, validate.MessageDeleteTimeInMinute));

        }
        catch (Exception e)
        {
            Log.Error(e, nameof(CommandBotStateAsync));
        }
    }
    public static async Task CommandUserNameVisibilityAsync(this ITelegramBotClient client, Message message, CancellationToken ct = default)
    {
        try
        {
            var validate = await ValidateGroupAsync(client, message, ct);
            if (validate is null)
                return;
            var result = await GroupController.UpdateGroupAsync(p =>
            {
                p.HideName = message.Text!.Contains("hide") || !message.Text.Contains("show");

            }, message.Chat.Id, ct);
            var response = result switch
            {
                0 => "Bot Updated",
                _ => "Cannot Update The Bot."
            };
            var msg1 = await client.SendTextMessageAsync(message.Chat.Id, response, cancellationToken: ct);
            RemoveMessageService.MessagesToRemove.Add(new RemoveMessageModel(message.Chat.Id, msg1.MessageId, validate.MessageDeleteTimeInMinute));

        }
        catch (Exception e)
        {
            Log.Error(e, nameof(CommandBotStateAsync));
        }
    }
    public static async ValueTask<Group?> ValidateGroupAsync(ITelegramBotClient client, Message message, CancellationToken ct = default)
    {
        try
        {
            var getGroup = await GroupController.GetGroupAsync(message.Chat.Id, ct);
            if (getGroup is not null)
                return getGroup;
            _ = await client.SendTextMessageAsync(message.Chat.Id, "Group Is Not Created.\nPlease Create It First.",
                cancellationToken: ct);
            return null;
        }
        catch (Exception e)
        {
            Log.Error(e, nameof(ValidateGroupAsync));
            return null;
        }


    }
}
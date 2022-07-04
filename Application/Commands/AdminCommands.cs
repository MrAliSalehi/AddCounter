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
                var msg1 = await client.SendTextMessageAsync(message.Chat.Id, "زمان نباید خالی باشد", cancellationToken: ct);
                RemoveMessageService.MessagesToRemove.Add(new RemoveMessageModel(message.Chat.Id, msg1.MessageId));
                return;
            }

            var result = await GroupController.UpdateGroupAsync(p => p.MessageDeleteTimeInMinute = time, message.Chat.Id, ct);
            var response = result switch
            {
                0 => "زمان اپدیت شد.",
                _ => "نمیشه"
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
                var msg1 = await client.SendTextMessageAsync(message.Chat.Id, "<i>گروه از قبل ساخته شده</i>", ParseMode.Html, cancellationToken: ct).ConfigureAwait(false);
                RemoveMessageService.MessagesToRemove.Add(new RemoveMessageModel(message.Chat.Id, msg1.MessageId, getGroup.MessageDeleteTimeInMinute));
                return;
            }

            var result = await GroupController.AddGroupAsync(message.Chat.Id, ct);
            var response = result switch
            {
                0 => "گروه ساخته شد",
                _ => "نمیشه",
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
                var msg1 = await client.SendTextMessageAsync(message.Chat.Id, "پیام نمیتواند خالی باشد", cancellationToken: ct);
                RemoveMessageService.MessagesToRemove.Add(new RemoveMessageModel(message.Chat.Id, msg1.MessageId, validate.MessageDeleteTimeInMinute));

                return;
            }

            var updateResult =
                await GroupController.UpdateGroupAsync(p => p.WelcomeMessage = welcomeMessage, message.Chat.Id, ct);
            var response = updateResult switch
            {
                0 => "گروه اپدیت شد",
                _ => "نمیشه"
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
                var msg1 = await client.SendTextMessageAsync(message.Chat.Id, "تعداد نمیتواند خالی باشد", cancellationToken: ct);
                RemoveMessageService.MessagesToRemove.Add(new RemoveMessageModel(message.Chat.Id, msg1.MessageId, validate.MessageDeleteTimeInMinute));

                return;
            }

            var updateResult = await GroupController.UpdateGroupAsync(p => p.RequiredAddCount = addCount, message.Chat.Id, ct);
            var response = updateResult switch
            {
                0 => "گروه اپدیت شد",
                _ => "نمیشه",
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
            builder.Append($"اطلاعات برای گروه [{message.Chat.Title}]\n");
            foreach (var user in users)
            {
                builder.Append($"[کاربر](tg://user?id={user.UserId}) اضافه کرد [{user.AddCount}] نفر\n");
                totalAdds += user.AddCount;
            }

            builder.Append($"\nمجموع اد ها:{totalAdds}");

            var msg1 = await client.SendTextMessageAsync(message.Chat.Id, "<i>اطلاعات به پیوی ارسال شد.</i>", ParseMode.Html,
                replyToMessageId: message.MessageId, cancellationToken: ct).ConfigureAwait(false);

            await client.SendTextMessageAsync(message.From!.Id, builder.ToString(),
                ParseMode.MarkdownV2,
                cancellationToken: ct)
                .ConfigureAwait(false);

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
                0 => "کاربران ریست شدند",
                _ => "نمیشه"
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
                0 => "تنظیمات گروه ریست شد",
                1 => "هنوز گروه رو نساختی",
                _ => "نمیشه"
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
            var msg1 = await client.SendTextMessageAsync(message.Chat.Id, $"ایدی گروه:[<i>{message.Chat.Id}</i>]", ParseMode.Html, cancellationToken: ct);
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
                var msg1 = await client.SendTextMessageAsync(message.Chat.Id, "قیمت نمیتواند خالی باشد", cancellationToken: ct);
                RemoveMessageService.MessagesToRemove.Add(new RemoveMessageModel(message.Chat.Id, msg1.MessageId, validate.MessageDeleteTimeInMinute));

                return;
            }

            var result = await GroupController.UpdateGroupAsync(p => p.AddPrice = price, message.Chat.Id, ct);
            var response = result switch
            {
                0 => "قیمت اپدیت شد",
                _ => "نمیشه"
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
                0 => "گروه حذف شد",
                1 => "اصن گروه فعال نیست ",
                _ => "نمیشه"
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
                0 => "بروزرسانی شد",
                _ => "نمیشه"
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
                0 => "بروزرسانی شد",
                _ => "نمیشه"
            };
            var msg1 = await client.SendTextMessageAsync(message.Chat.Id, response, cancellationToken: ct);
            RemoveMessageService.MessagesToRemove.Add(new RemoveMessageModel(message.Chat.Id, msg1.MessageId, validate.MessageDeleteTimeInMinute));

        }
        catch (Exception e)
        {
            Log.Error(e, nameof(CommandBotStateAsync));
        }
    }
    public static async Task CommandAddNotificationControlAsync(this ITelegramBotClient client, Message message, CancellationToken ct = default)
    {
        try
        {
            var validate = await ValidateGroupAsync(client, message, ct);
            if (validate is null)
                return;
            var result = await GroupController.UpdateGroupAsync(p =>
            {
                p.NotifyForAdd = message.Text is not "dis n" and "en n";
            }, message.Chat.Id, ct);
            var response = result switch
            {
                0 => "وضعیت اعلان بروزرسانی شد",
                _ => "نمیشه"
            };
            var msg1 = await client.SendTextMessageAsync(message.Chat.Id, response, cancellationToken: ct);
            RemoveMessageService.MessagesToRemove.Add(new RemoveMessageModel(message.Chat.Id, msg1.MessageId, validate.MessageDeleteTimeInMinute));
        }
        catch (Exception e)
        {
            Log.Error(e, nameof(CommandAddNotificationControlAsync));
        }
    }
    public static async Task CommandFakeDetectionAsync(this ITelegramBotClient client, Message message, CancellationToken ct = default)
    {
        try
        {
            var validate = await ValidateGroupAsync(client, message, ct);
            if (validate is null)
                return;
            var result = await GroupController.UpdateGroupAsync(p =>
            {
                p.FakeDetection = message.Text is "fake on" or not "fake off";
            }, message.Chat.Id, ct);
            var response = result switch
            {
                0 => "تشخیص فیک اپدیت شد",
                _ => "نمیشه"
            };

            var msg1 = await client.SendTextMessageAsync(message.Chat.Id, response, cancellationToken: ct);
            RemoveMessageService.MessagesToRemove.Add(new RemoveMessageModel(message.Chat.Id, msg1.MessageId, validate.MessageDeleteTimeInMinute));
        }
        catch (Exception e)
        {
            Log.Error(e, nameof(CommandFakeDetectionAsync));
        }
    }
    public static async Task CommandWelcomeControlAsync(this ITelegramBotClient client, Message message, CancellationToken ct = default)
    {
        try
        {
            var validate = await ValidateGroupAsync(client, message, ct);
            if (validate is null)
                return;

            var result = await GroupController.UpdateGroupAsync(p =>
            {
                p.SayWelcome = message.Text is "wlc on" or not "wlc off";
            }, message.Chat.Id, ct);
            var response = result switch
            {
                0 => "حله",
                _ => "نمیشه"
            };

            var msg1 = await client.SendTextMessageAsync(message.Chat.Id, response, cancellationToken: ct);
            RemoveMessageService.MessagesToRemove.Add(new RemoveMessageModel(message.Chat.Id, msg1.MessageId, validate.MessageDeleteTimeInMinute));
        }
        catch (Exception e)
        {
            Log.Error(e, nameof(CommandWelcomeControlAsync));
        }
    }
    public static async Task CommandSetPayAdminAsync(this ITelegramBotClient client, Message message, CancellationToken ct = default)
    {
        try
        {
            var validate = await ValidateGroupAsync(client, message, ct);
            if (validate is null)
                return;

            var result = await GroupController.UpdateGroupAsync(p =>
            {
                p.PayAdmin = message.Text?.Replace("payadmin", "") ?? "@.";
            }, message.Chat.Id, ct);
            var response = result switch
            {
                0 => "حله",
                _ => "نمیشه"
            };

            var msg1 = await client.SendTextMessageAsync(message.Chat.Id, response, cancellationToken: ct);
            RemoveMessageService.MessagesToRemove.Add(new RemoveMessageModel(message.Chat.Id, msg1.MessageId, validate.MessageDeleteTimeInMinute));
        }
        catch (Exception e)
        {
            Log.Error(e, nameof(CommandWelcomeControlAsync));
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
using AddCounter.Application.Commands;
using Telegram.Bot;
using Telegram.Bot.Types;

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

        _ = message.Text switch
        {
            "check" => Task.Run(() => AdminCommands.ValidateGroupAsync(client, message, ct), ct),
            "create" => client.CommandCreateGroupAsync(message, ct),
            { } msg when (msg.StartsWith("wlc")) => client.CommandSetWelcomeMessageAsync(message, ct),
            { } msg when (msg.StartsWith("count")) => client.CommandSetAddCountAsync(message, ct),

            _ => Task.CompletedTask
        };

    }

    internal static async Task HandleChatMemberAsync(this ITelegramBotClient client, ChatMemberUpdated chatMember, CancellationToken ct = default)
    {

    }
}
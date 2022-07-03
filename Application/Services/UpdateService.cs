using AddCounter.Application.Handlers;
using Serilog;
using Telegram.Bot;

namespace AddCounter.Application.Services
{
    public class UpdateService : BackgroundService
    {
        private readonly TelegramBotClient _client;
        public UpdateService()
        {
            _client = new TelegramBotClient(Globals.BotConfigs.Token);
        }
        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            var me = await _client.GetMeAsync(cancellationToken).ConfigureAwait(false);
            Log.Information("Bot Started With : {0}", me.Username);
            await base.StartAsync(cancellationToken).ConfigureAwait(false);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await _client.ReceiveAsync((client, update, ct) =>
                {
                    var updateHandler = update switch
                    {
                        { Message.NewChatMembers: not null } => client.HandleChatMemberAsync(update.Message.NewChatMembers, update.Message.From, update.Message.Chat.Id, ct),

                        { Message: not null } => client.HandleMessagesAsync(update.Message, ct),


                        _ => Task.CompletedTask,

                    };

                    return updateHandler;

                }, (_, exception, _) =>
                {
                    Log.Error(exception, "OnError");
                    return Task.CompletedTask;

                }, cancellationToken: stoppingToken);
                await Task.Delay(300, stoppingToken);
            }
        }


    }
}
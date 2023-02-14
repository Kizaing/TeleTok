using System;
using System.Configuration;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TeleTok
{
    public class TelegramListener
    {
        public void RunListener()
        {
            var botClient = new TelegramBotClient(TeleTok.token);
            using var cts = new CancellationTokenSource();

            // StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
            ReceiverOptions receiverOptions = new ()
            {
                AllowedUpdates = Array.Empty<UpdateType>() // receive all update types
            };

            botClient.StartReceiving(
                updateHandler: HandleUpdateAsync,
                errorHandler: HandlePollingErrorAsync,
                receiverOptions: receiverOptions,
                cancellationToken: cts.Token
            );

            while (true)
            {
                // Do nothing until the stuff happens
            };

            cts.Cancel();
        }

        async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            // Only process Message updates: https://core.telegram.org/bots/api#message
            if (update.Message is not { } message)
                return;
            // Only process text messages
            if (message.Text is not { } messageText)
                return;

            var chatId = message.Chat.Id;

            string proxyUrl;

            // Checks if the text contains a valid URL
            bool isUri = Uri.IsWellFormedUriString(messageText, UriKind.RelativeOrAbsolute);

            // Passes the url along to the video downloader if it is valid AND a tiktok link
            if (isUri)
            {
                try
                {
                    if(messageText.Contains("tiktok.com"))
                    {
                        proxyUrl = VidDownload.TikTokURL(messageText);

                        Message ttVideo = await botClient.SendVideoAsync(
                            chatId: chatId,
                            video: proxyUrl,
                            cancellationToken: cancellationToken
                        );
                    }
                }
                catch 
                {
                    Console.WriteLine("Valid TikTok URI was sent, but was not a video!");
                }
            }
        }

        Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }
    }
}

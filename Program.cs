using Microsoft.Extensions.Configuration;
using System;
using System.Web;
using System.Text.RegularExpressions;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TeleTok
{
    public class TeleTok
    {
        public static IConfigurationRoot config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("config.json", true)
            .Build();

        public static string token = config.GetSection("TeleTokConf:token").Value;
        public static string ptInstance = config.GetSection("TeleTokConf:proxitokInstance").Value;

        static async Task Main(string[] args)
        {
            var botClient = new TelegramBotClient(token);
            using var cts = new CancellationTokenSource();

            // StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
            ReceiverOptions receiverOptions = new ()
            {
                AllowedUpdates = Array.Empty<UpdateType>() // receive all update types
            };

            // HandleUpdateAsync and HandlePollingErrorAsync no worky Tyler help
            botClient.StartReceiving(
                updateHandler: HandleUpdateAsync,
                pollingErrorHandler: HandlePollingErrorAsync,
                receiverOptions: receiverOptions,
                cancellationToken: cts.Token
            );

            
            var me = await botClient.GetMeAsync();

            Console.WriteLine($"Start listening for @{me.Username}");
            while (true)
            {
                // Do nothing until the stuff happens
            }

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

            Console.WriteLine($"Received a '{messageText}' message in chat {chatId}.");

            // Echo received message text
            Message sentMessage = await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: "You said:\n" + messageText,
                cancellationToken: cancellationToken);
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
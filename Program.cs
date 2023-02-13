using Microsoft.Extensions.Configuration;
using System;
using Telegram.Bot;

namespace TeleTok
{
    class TeleTok
    {
        static async Task Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("config.json", true)
                .Build();

            var token = config.GetSection("TeleTokConf:token").Value;

            var botClient = new TelegramBotClient(token);

            var me = await botClient.GetMeAsync();

            Console.WriteLine($"Hello world! I am user {me.Id} and my name is {me.FirstName}.");
        }
    }
}
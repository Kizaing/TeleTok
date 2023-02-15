using Microsoft.Extensions.Configuration;
using System;

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
            //Checks if the config json data is valid
            if(token == "" || token == null || token == "INSERT TOKEN HERE")
            {
                LogMessage("Telegram bot token is invalid! Exiting...");
            }
            else if(ptInstance == "" || ptInstance == null || ptInstance == "PROXITOK INSTANCE URL")
            {
                LogMessage("Proxitok instance is invalid! Exiting...");
            }
            else
            {
                TelegramListener listener = new TelegramListener();
                Console.WriteLine("Now listening...");

                listener.RunListener();
            }            
        }

        public static void LogMessage(string text)
        {   
            DateTime now =DateTime.Now;

            Console.WriteLine("[" + now.ToString() + "] " + text);
        }
    }
}
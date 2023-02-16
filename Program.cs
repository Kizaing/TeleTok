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

        // Value to see what bot mode to run in
        public static string botMode = config.GetSection("TeleTokConf:botMode").Value;

        // Telegram bot config
        public static string token = config.GetSection("TeleTokConf:token").Value;
        public static string ptInstance = config.GetSection("TeleTokConf:ptInstance").Value;

        // Matrix bot config
        public static string matrixAddress = config.GetSection("TeleTokConf:matrixAddress").Value;
        public static string mBotUser = config.GetSection("TeleTokConf:mBotUser").Value;
        public static string mBotPass = config.GetSection("TeleTokConf:mBotPass").Value;

        static async Task Main(string[] args)
        {
            //Checks to see what mode to run the bot in

            if(botMode == "telegram")
            {
                //Checks if the config json data is valid
                if(ConfigCheck(token) == false)
                {
                    LogMessage("Telegram bot token is invalid! Exiting...");
                }
                else if(ConfigCheck(ptInstance) == false)
                {
                    LogMessage("Proxitok instance is invalid! Exiting...");
                }
                else
                {
                    TelegramListener tListener = new TelegramListener();
                    LogMessage("Now listening...");

                    tListener.RunListener();
                }
            }
            else if(botMode == "matrix")
            {
                //Checks if the config json data is valid
                if(ConfigCheck(matrixAddress) == false)
                {
                    LogMessage("Synapse address is invalid! Exiting...");
                }
                else if(ConfigCheck(mBotUser) == false)
                {
                    LogMessage("Matrix bot username is invalid! Exiting...");
                }
                else if(ConfigCheck(mBotPass) == false)
                {
                    LogMessage("Matrix bot password is invalid! Exiting...");
                }
                else
                {
                    MatrixListener mListener = new MatrixListener();
                    LogMessage("Now listening...");

                    mListener.RunListener();
                }              
            }
            else
            {
                LogMessage("Bot mode is not configured! Enter either \'telegram\' or \'matrix\'");
            }
        }

        public static bool ConfigCheck(string confItem) 
        {
            if(confItem == "" || confItem == null
                || confItem == "INSERT TOKEN HERE"
                || confItem == "PROXITOK INSTANCE URL"
                || confItem == "SYNAPSE SERVER URL"
                || confItem == "MATRIX BOT USERNAMEL"
                || confItem == "MATRIX BOT PASSWORD")
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static void LogMessage(string text)
        {   
            DateTime now =DateTime.Now;

            Console.WriteLine("[" + now.ToString() + "] " + text);
        }
    }
}
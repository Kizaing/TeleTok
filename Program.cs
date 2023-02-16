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
        public static Uri mAddress;
        public static string mBotUser = config.GetSection("TeleTokConf:mBotUser").Value;
        public static string mBotPass = config.GetSection("TeleTokConf:mBotPass").Value;

        static async Task Main(string[] args)
        {
            //Checks to see what mode to run the bot in
            LogMessage($"The current running config is: \n Bot Mode: {botMode} \n Telegram Token: {token} \n ProxiTok Instance: {ptInstance} \n Matrix Homeserver: {matrixAddress} \n Matrix Bot User: {mBotUser}");

            if(botMode == "telegram")
            {
                //Checks if the config json data is valid
                if(!ConfigCheck(token))
                {
                    LogMessage("Telegram bot token is invalid! Exiting...");
                }
                else if(!ConfigCheck(ptInstance))
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
                if(Uri.IsWellFormedUriString(matrixAddress, UriKind.Absolute))
                {
                    mAddress = new Uri(matrixAddress);
                }
                else
                {
                    LogMessage("Matrix server address is not a valid URL! Exiting...");
                }

                if(!ConfigCheck(mBotUser))
                {
                    LogMessage("Matrix bot username is invalid! Exiting...");
                }
                else if(!ConfigCheck(mBotPass))
                {
                    LogMessage("Matrix bot password is invalid! Exiting...");
                }
                else
                {
                    MatrixListener mListener = new MatrixListener();
                    LogMessage("Now listening...");

                    await mListener.RunListener();
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
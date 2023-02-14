using System;
using System.Diagnostics;

namespace TeleTok
{
    public class VidDownload
    {
        static void TikTokURL(string videourl)
        {
            string url = videourl;

            if(url.Contains("vm.tiktok.com"))
            {
                url = UnshortenUrl(url);
            }

            var proc = new Process 
            {
                // Function downloads tiktok urls passed from the listener
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/bash",
                    Arguments = "-c \"tiktok-scraper --filepath /app/videos -d " + videourl + "\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };

            proc.Start();
            proc.WaitForExit();

        }

        static string UnshortenUrl(string videourl)
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(videourl);
            req.AllowAutoRedirect = false;
            var resp = req.GetResponse();
            string realUrl = resp.Headers["Location"];

            return realUrl;
        }
    }
}
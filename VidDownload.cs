using System;
using System.Diagnostics;
using System.Net;

namespace TeleTok
{
    public class VidDownload
    {
        // Takes the scraped TikTok URL and appends it to the proxy downloader link then returns it
        public string TikTokURL(string videourl)
        {
            string url = videourl;
            string proxyUrl;

            if(url.Contains("vm.tiktok.com"))
            {
                url = UnshortenUrl(url);
            }

            proxyUrl = TeleTok.ptInstance + "/download?url=" + url;

            return proxyUrl;
        }

        // Runs the URL through a web request then returns the full url
        // If the URL is already the full one, it won't really do anything but will catch shortened ones
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
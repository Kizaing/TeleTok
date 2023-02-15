using System;
using System.Diagnostics;
using System.Net;

namespace TeleTok
{
    public class VidDownload
    {
        // Takes the scraped TikTok URL and appends it to the proxy downloader link then returns it
        public static string TikTokURL(string videourl)
        {
            string url = videourl;
            string proxyUrl;

            TeleTok.LogMessage("Video for " + videourl + " processing..");

            if(url.Contains("vm.tiktok.com"))
            {
                url = UnshortenUrl(url);
            }

            proxyUrl = CreateDownloadLink(url);

            return proxyUrl;
        }

        // Runs the URL through a web request then returns the full url
        static string UnshortenUrl(string videourl)
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(videourl);
            req.AllowAutoRedirect = false;
            var resp = req.GetResponse();
            string realUrl = resp.Headers["Location"];

            return realUrl;
        }

        //Breaks apart the URL and extracts the User and Video ID to be processed into a working download link
        static string CreateDownloadLink(string videourl)
        {
            Uri segmentedUri = new Uri(videourl);
            segmentedUri = new Uri(segmentedUri.AbsoluteUri.Replace(segmentedUri.Query, string.Empty));

            string videoUser = segmentedUri.Segments[1];
            videoUser = videoUser.Replace(@"/", "");
            string videoID = segmentedUri.Segments[3];

            string fixedUrl = "https://www.tiktok.com/" + videoUser + "/video/" + videoID + @"&id=" + videoID + @"&user=" + videoUser.Remove(0);

            string proxyLink = TeleTok.ptInstance + "/download?url=" + fixedUrl;

            TeleTok.LogMessage("Input User ID is: " + videoUser);
            TeleTok.LogMessage("Input video ID is: " + videoID);

            return proxyLink;
        }
    }
}
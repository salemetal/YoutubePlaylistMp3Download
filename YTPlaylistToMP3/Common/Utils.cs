using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace YTPlaylistToMP3.Common
{
    public class Utils
    {
        public const string youtubeApiUrl = "https://www.googleapis.com/youtube/v3/playlistItems?key=AIzaSyAzSrDGhBplYOps2eScj-1wuK6tx6k2B-E&part=snippet,contentDetails&maxResults=50&playlistId=";
        public const string listUrlregex = "list=.+[a-zA-Z0-9]$";
        public const string ffmpegPath = @"C:\Program Files\FFMPEG\bin\ffmpeg.exe";
        public const int noOfThreads = 8;

        public static bool IsInternetConnection()
        {
            System.Net.WebRequest req = System.Net.WebRequest.Create("https://www.google.co.in/");
            try
            {
                var resp = req.GetResponse();
                resp.Close();
                req = null;
                return true;
            }
            catch (Exception)
            {
                req = null;
                return false;
            }
        } 
    }

    public enum MediaType
    {
        Undefined,
        Mp3,
        Mp4
    }
}

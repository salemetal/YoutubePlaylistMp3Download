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
        public const string listUrlregex = "list=.+[a-zA-Z0-9]$";
        public  const string ffmpegPath = @"C:\Program Files\FFMPEG\bin\ffmpeg.exe";

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
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Windows.Controls;
using Newtonsoft.Json;
using YTPlaylistToMP3.Common;
using YTPlaylistToMP3.Model;

namespace YTPlaylistToMP3.Donwload
{
    public class Download
    {
        private BackgroundWorker _backgroundWorker;
        private Playlist _playlist;
        private Console _console;
        private DirectoryInfo _dirInfo;
        private List<PlaylistItem> _playlistItems;
        private ProgressBar _pbMain;

        public Download(string playlistId, TextBlock mainWindowTb, DirectoryInfo dirInfo, ProgressBar pbMain)
        {
            _playlist = new Playlist(playlistId);
            _console = new Console(mainWindowTb);
            _dirInfo = dirInfo;
            _pbMain = pbMain;

            SetBackgroundWorker();
        }

        public void DownloadItems()
        {
            if (IsApiResponseOk())
            {
                _backgroundWorker.RunWorkerAsync();
            }
            else
            {
                WriteToConsole("API response NOT OK!");
            }
        }

        #region Background Worker Events
        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            int percentPerItem = Convert.ToInt32(Math.Floor(100m / _playlistItems.Count));
            int itemCounter = 0;

            foreach (PlaylistItem item in _playlistItems)
            {
                try
                {         
                    string link = "https://youtube.com/watch?v=" + item.ContentDetails.VideoId;

                    var videoInfos = YoutubeExtractor.DownloadUrlResolver.GetDownloadUrls(link);
                    var info = videoInfos.Where(inf => inf.VideoType == YoutubeExtractor.VideoType.Mp4).OrderByDescending(inf => inf.AudioBitrate).First();
                    if (info.RequiresDecryption)
                    {
                        YoutubeExtractor.DownloadUrlResolver.DecryptDownloadUrl(info);
                    }
                    string cleanedUpTitle = CleanUpTitle(info.Title); //scanning for forbidden chars

                    string filepath = Path.Combine(_dirInfo.FullName, $"{cleanedUpTitle}{info.VideoExtension}");

                    var youtubeExtractor = new YoutubeExtractor.VideoDownloader(info, filepath);

                    WriteToConsole($"Downloading: {cleanedUpTitle}");
                    try
                    {
                        youtubeExtractor.Execute();              
                        itemCounter++;

                        ExtractMp3FromMp4($"{_dirInfo.FullName}\\{cleanedUpTitle}");
                        File.Delete($"{_dirInfo.FullName}\\{cleanedUpTitle}.mp4");

                        WriteToConsole($" OK", false);
                        _backgroundWorker.ReportProgress (itemCounter * percentPerItem);
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    WriteToConsole($"Error: {ex.Message}");
                }
            }

            _backgroundWorker.ReportProgress(100);
        }

        private void ExtractMp3FromMp4(string mp4Path)
        {
            var inputFile = $"{mp4Path}.mp4";
            var outputFile = $"{mp4Path}.mp3";
            var mp3out = "";
            var ffmpegProcess = new Process();
            ffmpegProcess.StartInfo.UseShellExecute = false;
            ffmpegProcess.StartInfo.RedirectStandardInput = true;
            ffmpegProcess.StartInfo.RedirectStandardOutput = true;
            ffmpegProcess.StartInfo.RedirectStandardError = true;
            ffmpegProcess.StartInfo.CreateNoWindow = true;
            ffmpegProcess.StartInfo.FileName = Utils.ffmpegPath;
            ffmpegProcess.StartInfo.Arguments = " -i " + inputFile + " -vn -ar 44100 -ac 1 -b:a 32k -f mp3 " + outputFile;
            ffmpegProcess.Start();
            ffmpegProcess.StandardOutput.ReadToEnd();
            mp3out = ffmpegProcess.StandardError.ReadToEnd();
            ffmpegProcess.WaitForExit();
            if (!ffmpegProcess.HasExited)
            {
                ffmpegProcess.Kill();
            }
        }

        private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //message code -1
            if (e.ProgressPercentage == -1)
            {
                ConsoleMessage consoleMessage = (ConsoleMessage)e.UserState;
                _console.Write(consoleMessage.Message, consoleMessage.IsNewLine);
            }
            else if(e.ProgressPercentage > 0)
            {
                _pbMain.Value = e.ProgressPercentage;
            }
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                _console.Write($"Error in Process : {e.Error}");

            }
            else
            {
                _console.Write($"Operation Completed : {e.Result}");
            }
        }
        #endregion

        #region Private Methods
        private void SetBackgroundWorker()
        {
            _backgroundWorker = new BackgroundWorker()
            {
                WorkerReportsProgress = true
            };

            _backgroundWorker.DoWork += backgroundWorker_DoWork;

            _backgroundWorker.ProgressChanged += backgroundWorker_ProgressChanged;

            _backgroundWorker.RunWorkerCompleted += backgroundWorker_RunWorkerCompleted;
        }

        private string CleanUpTitle(string title)
        {
            ReplaceChar("<");
            ReplaceChar(">");
            ReplaceChar(":");
            ReplaceChar("\"");
            ReplaceChar("/");
            ReplaceChar(@"\");
            ReplaceChar("|");
            ReplaceChar("?");
            ReplaceChar("*");

            void ReplaceChar(string character)
            {
                if (title.Contains(character))
                   title = title.Replace(character, "");
            }
            title = title.Replace(" ", "_");
            title = title.Replace("__", "_");
            title = title.Replace("___", "_");

            return title;
        }

        private bool IsApiResponseOk()
        {
            WriteToConsole(Strings.CheckingApiResponse, false);

            APIResponse apiresponse = null;

            try
            {
                do
                {
                    _playlistItems = new List<PlaylistItem>();

                    var uri = new Uri("https://www.googleapis.com/youtube/v3/playlistItems?key=AIzaSyAo1OGaye6UmKHYLt0dh5aBfimcZPZ_Ow0&part=snippet,contentDetails&maxResults=50&playlistId=" + this._playlist.Id + (apiresponse == null ? "" : $"&pageToken={apiresponse.NextPageToken}"));
                    var client = new HttpClient();
                    var response = client.GetStringAsync(uri);
                    apiresponse = JsonConvert.DeserializeObject<APIResponse>(response.Result);
                    _playlistItems.AddRange(apiresponse.Items);
                }
                while (apiresponse.NextPageTokenExists);

                WriteToConsole($"{Strings.ResponseOK} { _playlistItems.Count} {Strings.ItemsFound}.");
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private void WriteToConsole(string message, bool isNewLine = true)
        {
            ConsoleMessage consoleMessage = new ConsoleMessage
            {
                IsNewLine = isNewLine,
                Message = message
            };

            _backgroundWorker.ReportProgress(-1, consoleMessage);
        }
        #endregion






    }
}

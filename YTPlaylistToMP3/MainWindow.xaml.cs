using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using Common;
using YTPlaylistToMP3.Common;
using YTPlaylistToMP3.Donwload;

namespace YTPlaylistToMP3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            RbMp3.IsChecked = true;
        }

        private void ButtonDownload_OnClick(object sender, RoutedEventArgs e)
        {
            ClearFeedback();

            if(Utils.IsInternetConnection())
            {
                if (IsCheckOk())
                {
                    string ytPlaylistUrl = TxtYTPlaylistUrl.Text.Trim();

                    string ytPlaylistId = ytPlaylistUrl.Substring(ytPlaylistUrl.IndexOf("list=") + "list=".Length);
                    string savePath = TxtSavePath.Text.Trim();

                    MediaType mediaType = (bool)RbMp3.IsChecked ? MediaType.Mp3 : MediaType.Mp4;

                    Download download = new Download(ytPlaylistId, TbDownloadFeedback, new DirectoryInfo(savePath), pbStatus, mediaType);
                    download.DownloadItems();
                    
                }
            }
            else
            {
                MessageBox.Show(Strings.NoNetConnection, Constants.Message.ErrorCaption, MessageBoxButton.OK);
            } 
        }

        private bool IsCheckOk()
        {
            if (IsPlaylistIdOK() &&
                IsSavePathOK())
            {
                return true;
            }

            return false;
        }

        private bool IsSavePathOK()
        {
            string savePath = TxtSavePath.Text.Trim();

            if (string.IsNullOrWhiteSpace(savePath))
            {
                MessageBox.Show(Strings.SavePathRequired, Constants.Message.ErrorCaption, MessageBoxButton.OK);
                return false;
            }

            if (!Directory.Exists(savePath))
            {
                MessageBox.Show($"{Strings.FolderNotFound}: {savePath}!",
                           Constants.Message.ErrorCaption, MessageBoxButton.OK);
                return false;
            }

            return true;
        }

        private bool IsPlaylistIdOK()
        {
            string playlistUrl = TxtYTPlaylistUrl.Text.Trim();
            Regex ytUrlRgx = new Regex(Utils.listUrlregex);

            if (string.IsNullOrWhiteSpace(playlistUrl))
            {
                MessageBox.Show(Strings.UrlRequired, Constants.Message.ErrorCaption, MessageBoxButton.OK);
                return false;
            }
            if (!ytUrlRgx.IsMatch(playlistUrl))
            {
                MessageBox.Show(Strings.UrlNotValid, Constants.Message.ErrorCaption, MessageBoxButton.OK);
                return false;
            }

            return true;
        }

        private void ClearFeedback()
        {
            TbDownloadFeedback.Text = string.Empty;
        }
    }
}

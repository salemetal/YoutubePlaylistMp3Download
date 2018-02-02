using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YTPlaylistToMP3.Model
{
    public class APIResponse
    {
        public string Kind { get; set; }
        public string Etag { get; set; }
        public string NextPageToken { get; set; }
        public string PrevPageToken { get; set; }

        public PageInfo PageInfo { get; set; }
        public IEnumerable<PlaylistItem> Items { get; set; }

        public bool NextPageTokenExists => !string.IsNullOrEmpty(NextPageToken);
    }
}

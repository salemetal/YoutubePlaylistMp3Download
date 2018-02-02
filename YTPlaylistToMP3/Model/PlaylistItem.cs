using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YTPlaylistToMP3.Model
{
    public class PlaylistItem
    {
        public string Id { get; set; }
        public string Kind { get; set; }
        public string Etag { get; set; }
        public PlaylistItemSnippet Snippet { get; set; }
        public ContentDetails ContentDetails { get; set; }
    }
}

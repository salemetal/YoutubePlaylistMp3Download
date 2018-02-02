using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YTPlaylistToMP3.Model
{
    public class Playlist
    {
        public Playlist(string playlistId)
        {
            this.Id = playlistId;
        }

        public string Id { get; set; }
    }
}

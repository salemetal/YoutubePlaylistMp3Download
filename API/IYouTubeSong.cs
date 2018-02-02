using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API
{
    public interface IYouTubeSong
    {
        string Artist { get; set; }
        string SongId { get; set; }
        string Title { get; set; }
        string PlayListItemId { get; set; }
        ulong? Duration { get; set; }
        string OriginalTitle { get; set; }
        Guid SongGuid { get; set; }
    }
}

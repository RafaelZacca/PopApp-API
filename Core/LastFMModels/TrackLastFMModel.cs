using System.Collections.Generic;

namespace Core.LastFMModels
{
    public class TrackLastFMModel
    {
        public string Mbid { get; set; }
        public string Name { get; set; }
        public ArtistLastFMModel Artist { get; set; }
        public AlbumLastFMModel Album { get; set; }
    }
}

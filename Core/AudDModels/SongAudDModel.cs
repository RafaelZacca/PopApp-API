using System;

namespace Core.AudDModels
{
    public class SongAudDModel
    {
        public string Artist { get; set; }
        public string Title { get; set; }
        public string Album { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string Label { get; set; }
        public string Timecode { get; set; }
        public string SongLink { get; set; }
        public SpotifyAudDModel Spotify { get; set; }
    }
}

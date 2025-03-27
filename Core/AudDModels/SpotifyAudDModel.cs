using System.Collections.Generic;

namespace Core.AudDModels
{
    public class SpotifyAudDModel
    {
        public string Href { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public int TrackNumber { get; set; }
        public string Uri { get; set; }
        public SpotifyAlbumAudDModel Album { get; set; }
        public List<SpotifyArtistAudDModel> Artists { get; set; }
    }
}

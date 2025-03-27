using Database.Supports.Bases;
using System.Collections.Generic;

namespace Database.Models
{
    public class SongModel
    {
        public string Name { get; set; }
        public string ArtistName { get; set; }
        public ImageModel Image { get; set; }
        public IEnumerable<SongModel> RecommendedSongs { get; set; }
    }
}

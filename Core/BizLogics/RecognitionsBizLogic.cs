using Core.AudDModels;
using Core.LastFMModels;
using Core.Supports.Bases;
using Core.Supports.Managers;
using Database.Models;
using Database.Repositories;
using Database.Supports.Contexts;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Core.BizLogics
{
    public class RecognitionsBizLogic : BaseBizLogic<RecognitionsRepository, RecognitionModel>
    {
        public RecognitionsBizLogic(IConfiguration configuration, PopAppContext context) : base(configuration)
        {
            Repository = new RecognitionsRepository(context);
        }

        public async Task<SongModel> InsertAndRecognize(RecognitionModel entity, IDbContextTransaction inheritedTransaction = null)
        {
            var dbContextTransaction = inheritedTransaction ?? Repository.Context.Database.BeginTransaction();

            try
            {
                //Save recognition in database
                var recognition = await base.Insert(entity, dbContextTransaction);

                if (inheritedTransaction == null)
                {
                    dbContextTransaction.Commit();
                }

                //Send song and try to recognize it
                var formData = new[]{
                    new KeyValuePair<string, string>("return","spotify"),
                    new KeyValuePair<string, string>("url",Configuration["Api:Url"]+"/recognitions/"+recognition.Id+"/mp3"),
                };

                if (!string.IsNullOrWhiteSpace(Configuration["Security:AudDKey"]))
                {
                    formData.Append(new KeyValuePair<string, string>("api_token", Configuration["Security:AudDKey"]));
                }

                //var audDResult = await HttpRequestsManager.ExecuteRequest<RecognitionAudDModel>(Configuration.GetSection("Api")["AudDUrl"], HttpMethod.Post, null, formData);

                var audDResult = JsonConvert.DeserializeObject<RecognitionAudDModel>("{'status':'success','result':{'artist':'Ricardo Arjona','title':'Historia de Taxi','album':'Historias','release_date':'1994-04-14','label':'Columbia','timecode':'03:42','song_link':'https://lis.tn/HistoriaDeTaxi','spotify':{'album':{'name':'Historias','artists':[{'name':'Ricardo Arjona','id':'0h1zs4CTlU9D2QtgPxptUD','uri':'spotify:artist:0h1zs4CTlU9D2QtgPxptUD','href':'https://api.spotify.com/v1/artists/0h1zs4CTlU9D2QtgPxptUD','external_urls':{'spotify':'https://open.spotify.com/artist/0h1zs4CTlU9D2QtgPxptUD'}}],'album_group':'','album_type':'album','id':'110rZdMmIjA335CehJJkuk','uri':'spotify:album:110rZdMmIjA335CehJJkuk','available_markets':null,'href':'https://api.spotify.com/v1/albums/110rZdMmIjA335CehJJkuk','images':[{'height':640,'width':640,'url':'https://i.scdn.co/image/ab67616d0000b27398bf0a6f6828a3ee131d5a02'},{'height':300,'width':300,'url':'https://i.scdn.co/image/ab67616d00001e0298bf0a6f6828a3ee131d5a02'},{'height':64,'width':64,'url':'https://i.scdn.co/image/ab67616d0000485198bf0a6f6828a3ee131d5a02'}],'external_urls':{'spotify':'https://open.spotify.com/album/110rZdMmIjA335CehJJkuk'},'release_date':'1994-04-14','release_date_precision':'day'},'external_ids':{'isrc':'MXF149400057'},'popularity':65,'is_playable':true,'linked_from':null,'artists':[{'name':'Ricardo Arjona','id':'0h1zs4CTlU9D2QtgPxptUD','uri':'spotify:artist:0h1zs4CTlU9D2QtgPxptUD','href':'https://api.spotify.com/v1/artists/0h1zs4CTlU9D2QtgPxptUD','external_urls':{'spotify':'https://open.spotify.com/artist/0h1zs4CTlU9D2QtgPxptUD'}}],'available_markets':null,'disc_number':1,'duration_ms':405120,'explicit':false,'external_urls':{'spotify':'https://open.spotify.com/track/005Dlt8Xaz3DkaXiRJgdiS'},'href':'https://api.spotify.com/v1/tracks/005Dlt8Xaz3DkaXiRJgdiS','id':'005Dlt8Xaz3DkaXiRJgdiS','name':'Historia de Taxi','preview_url':'https://p.scdn.co/mp3-preview/31d154ae3779e131729bee18ac590ae2df64dbfe?cid=e44e7b8278114c7db211c00ea273ac69','track_number':4,'uri':'spotify:track:005Dlt8Xaz3DkaXiRJgdiS'}}}");

                //Use song data to get recommendations
                var recommendationsUrl = string.Format(
                    Configuration["Api:LastFMUrl"] + "?method=track.getsimilar&artist={0}&track={1}&api_key={2}&format=json&limit=10",
                    audDResult.Result.Artist.Replace(" ", "+"),
                    audDResult.Result.Title.Replace(" ", "+"),
                    Configuration["Security:LastFMKey"]
                );

                var lastFMResult = await HttpRequestsManager.ExecuteRequest<SuggestionLastFMModel>(recommendationsUrl, HttpMethod.Get);

                var recommendedSongs = new List<SongModel>();
                foreach(var track in lastFMResult.Similartracks.Track)
                {
                    var trackUrl = string.Format(
                        Configuration["Api:LastFMUrl"] + "?format=json&method=track.getInfo&api_key={0}&mbid={1}",
                        Configuration["Security:LastFMKey"],
                        track.Mbid
                    );
                    var trackResult = await HttpRequestsManager.ExecuteRequest<TrackSearchLastFMModel>(trackUrl, HttpMethod.Get);
                    recommendedSongs.Add(new SongModel()
                    {
                        ArtistName = track.Artist.Name,
                        Name = track.Name,
                        Image = new ImageModel() { Url = trackResult?.Track?.Album?.Image != null ? trackResult?.Track?.Album?.Image[2].Text : null, Height = 200, Width = 200 }
                    });

                }

                //Build response
                var song = new SongModel()
                {
                    Name = audDResult.Result.Title,
                    ArtistName = audDResult.Result.Artist,
                    RecommendedSongs = recommendedSongs
                };

                return song;
            }
            catch (Exception ex)
            {
                if (inheritedTransaction == null)
                {
                    dbContextTransaction.Rollback();
                }
                throw;
            }
        }

        public async Task<byte[]> GetRecognitionBytes(int id, IDbContextTransaction inheritedTransaction = null)
        {
            var dbContextTransaction = inheritedTransaction ?? Repository.Context.Database.BeginTransaction();

            try
            {
                var recognition = await base.Get(id, dbContextTransaction);
                byte[] bytes = Convert.FromBase64String(recognition.Base64);

                if (inheritedTransaction == null)
                {
                    dbContextTransaction.Commit();
                }

                return bytes;
            }
            catch
            {
                if (inheritedTransaction == null)
                {
                    dbContextTransaction.Rollback();
                }
                throw;
            }
        }
    }
}

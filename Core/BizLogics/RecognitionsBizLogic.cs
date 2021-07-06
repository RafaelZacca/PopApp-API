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

                dynamic formData;

                //Send song and try to recognize it
                if (!string.IsNullOrWhiteSpace(Configuration["Security:AudDKey"]))
                {
                    formData = new[]{
                        new KeyValuePair<string, string>("return","spotify"),
                        new KeyValuePair<string, string>("url",Configuration["Api:Url"]+"/recognitions/"+recognition.Id+"/mp3"),
                        new KeyValuePair<string, string>("api_token", Configuration["Security:AudDKey"])
                    };
                }
                else
                {
                    formData = new[]{
                        new KeyValuePair<string, string>("return","spotify"),
                        new KeyValuePair<string, string>("url",Configuration["Api:Url"]+"/recognitions/"+recognition.Id+"/mp3"),
                    };
                }


                var audDResult = await HttpRequestsManager.ExecuteRequest<RecognitionAudDModel>(Configuration.GetSection("Api")["AudDUrl"], HttpMethod.Post, null, formData);

                //Use song data to get recommendations
                var recommendationsUrl = string.Format(
                    Configuration["Api:LastFMUrl"] + "?method=track.getsimilar&artist={0}&track={1}&api_key={2}&format=json&limit=10",
                    audDResult.Result.Artist.Replace(" ", "+"),
                    audDResult.Result.Title.Replace(" ", "+"),
                    Configuration["Security:LastFMKey"]
                );

                var lastFMResult = await HttpRequestsManager.ExecuteRequest<SuggestionLastFMModel>(recommendationsUrl, HttpMethod.Get);

                var recommendedSongs = new List<SongModel>();
                foreach (var track in lastFMResult.Similartracks.Track)
                {
                    TrackSearchLastFMModel trackResult = null;
                    try
                    {
                        var trackUrl = string.Format(
                            Configuration["Api:LastFMUrl"] + "?format=json&method=track.getInfo&api_key={0}&mbid={1}",
                            Configuration["Security:LastFMKey"],
                            track.Mbid
                        );
                        trackResult = await HttpRequestsManager.ExecuteRequest<TrackSearchLastFMModel>(trackUrl, HttpMethod.Get);
                    }
                    catch { }
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

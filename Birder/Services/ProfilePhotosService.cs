﻿using Birder.Data.Model;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;

namespace Birder.Services
{

    public interface IProfilePhotosService
    {
        IEnumerable<Observation> GetThumbnailsUrl(IEnumerable<Observation> observations);
        Observation GetThumbnailsUrl(Observation observation);
    }

    public class ProfilePhotosService : IProfilePhotosService
    {
        private IMemoryCache _cache;
        private readonly IFlickrService _flickrService;

        public ProfilePhotosService(IMemoryCache memoryCache
                                   , IFlickrService flickrService)
        {
            _cache = memoryCache;
            _flickrService = flickrService;
        }

        /// <summary>
        /// Sets the bird thumbnail url from the Flickr API or the cache (collection of Observation)
        /// </summary>
        /// <param name="observations"></param>
        /// <returns></returns>
        public IEnumerable<Observation> GetThumbnailsUrl(IEnumerable<Observation> observations)
        {
            foreach (var observation in observations)
            {
                if (_cache.TryGetValue(string.Concat("thumb-", observation.Bird.BirdId), out string cacheUrl))
                {
                    observation.Bird.ThumbnailUrl = cacheUrl;
                }
                else
                {
                    observation.Bird.ThumbnailUrl = _flickrService.GetThumbnailUrl(observation.Bird.Species);
                    //_cache.Set(string.Concat("thumb-", observation.Bird.BirdId), observation.Bird.ThumbnailUrl);
                    AddResponseToCache(string.Concat("thumb-", observation.Bird.BirdId), observation.Bird.ThumbnailUrl);
                }
            }

            return observations;
        }

        /// <summary>
        /// /// Sets the bird thumbnail url from the Flickr API or the cache (single Observation)
        /// </summary>
        /// <param name="observation"></param>
        /// <returns></returns>
        public Observation GetThumbnailsUrl(Observation observation)
        {
            if (_cache.TryGetValue(string.Concat("thumb-", observation.Bird.BirdId), out string cacheUrl))
            {
                observation.Bird.ThumbnailUrl = cacheUrl;
            }
            else
            {
                observation.Bird.ThumbnailUrl = _flickrService.GetThumbnailUrl(observation.Bird.Species);
                //_cache.Set(string.Concat("thumb-", observation.Bird.BirdId), observation.Bird.ThumbnailUrl);
                AddResponseToCache(string.Concat("thumb-", observation.Bird.BirdId), observation.Bird.ThumbnailUrl);
            }

            return observation;
        }

        public void AddResponseToCache(string id, string url)
        {
            _cache.Set(id, url, TimeSpan.FromDays(5));
        }
    }
}

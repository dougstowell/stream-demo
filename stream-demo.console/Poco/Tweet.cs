using System;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using Tweetinvi.Models;

namespace StreamDemo
{
    internal class StreamTweet
    {
        public StreamTweet(ITweet original)
        {
            Id = original.Id;
            FullText = Regex.Replace(original.FullText, @"(@[A-Za-z0-9]+)|([^0-9A-Za-z \t])|(\w+:\/\/\S+)", " ").ToString();
            CreatedAt = original.CreatedAt;
            User = original.CreatedBy.Name;
            UserDescription = original.CreatedBy.Description;
            FollowersCount = original.CreatedBy.FollowersCount;

            var coords = original.Place.BoundingBox.Coordinates.First();
            Latitude = coords.Latitude;
            Longitude = coords.Longitude;
            Country = original.Place.Country;
        }


        public long Id { get; set; }

        public string FullText { get; set; }

        public DateTime CreatedAt { get; set; }

        public string User { get; set; }

        public string UserDescription { get; set; }

        public int FollowersCount { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public string Country { get; set; }


        public override string ToString()
        {
            return JsonSerializer.Serialize<StreamTweet>(this);
        }
    }
}

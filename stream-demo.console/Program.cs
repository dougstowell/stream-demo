using System;
using Tweetinvi;
using dotenv.net;

namespace StreamDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            DotEnv.Config();

            // add your Twitter API credentials here
            Auth.SetUserCredentials(
                Environment.GetEnvironmentVariable("CONSUMER_KEY"),
                Environment.GetEnvironmentVariable("CONSUMER_SECRET"),
                Environment.GetEnvironmentVariable("USER_ACCESS_TOKEN"),
                Environment.GetEnvironmentVariable("USER_ACCESS_SECRET")
            );

            var stream = Stream.CreateFilteredStream();
            var term = "COVID";
            stream.AddTrack(term);
            stream.MatchingTweetReceived += (sender, tweet) =>
            {
                Console.WriteLine(
                    $"A tweet containing { term } has been found; the tweet is { tweet.Tweet }"
                );
            };
            stream.StartStreamMatchingAllConditions();
        }
    }
}

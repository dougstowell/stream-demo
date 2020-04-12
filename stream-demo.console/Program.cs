using System;
using Tweetinvi;
using dotenv.net;
using System.Threading.Tasks;
using Microsoft.Azure.EventHubs;
using System.Text;
using Tweetinvi.Models;
using System.Linq;

namespace StreamDemo
{
    class Program
    {
        private static EventHubClient _eventHubClient = null;


        /// <summary>
        /// Main program entry point - async.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>Result of the async call.</returns>
        private static async Task MainAsync(string[] args)
        {
            DotEnv.Config();

            try
            {
                // Set up twitter connection.
                Auth.SetUserCredentials(
                    Environment.GetEnvironmentVariable("TWITTER_CONSUMER_KEY"),
                    Environment.GetEnvironmentVariable("TWITTER_CONSUMER_SECRET"),
                    Environment.GetEnvironmentVariable("TWITTER_USER_ACCESS_TOKEN"),
                    Environment.GetEnvironmentVariable("TWITTER_USER_ACCESS_SECRET")
                );

                // Create hub client.
                var ehConnStr = new EventHubsConnectionStringBuilder(
                    Environment.GetEnvironmentVariable("EH_CONN_STR")
                ).ToString();
                _eventHubClient = EventHubClient.CreateFromConnectionString(ehConnStr);

                var stream = Stream.CreateFilteredStream();
                
                stream.AddTweetLanguageFilter(LanguageFilter.English);

                var track = "COVID";
                stream.AddTrack(track);

                stream.MatchingTweetReceived += async (sender, data) =>
                {
                    try
                    {
                        if (data.Tweet.Place != null)
                        {
                            var tweetPayload = new StreamTweet(data.Tweet);
                            await _eventHubClient.SendAsync(
                                new EventData(Encoding.UTF8.GetBytes(tweetPayload.ToString()))
                            );

                            Console.WriteLine(
                                $"A tweet has been SENT; the tweet is { data.Tweet }"
                            );
                        }

                    }
                    catch (Exception exSend)
                    {
                        Console.WriteLine($"Error in send - { exSend.Message }");
                    }
                };
                stream.StartStreamMatchingAllConditions();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                await _eventHubClient.CloseAsync();
            }
        }

        /// <summary>
        /// Main program entry point.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public static void Main(string[] args)
        {
            MainAsync(args).GetAwaiter().GetResult();
        }
    }
}

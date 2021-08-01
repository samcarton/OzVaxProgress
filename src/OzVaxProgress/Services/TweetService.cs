using System;
using System.Threading.Tasks;
using Tweetinvi;
using Tweetinvi.Models;

namespace OzVaxProgress.Services
{
    public class TweetService
    {
        private readonly TwitterCredentials _credentials;
        public TweetService()
        {
            _credentials = new TwitterCredentials(
                Environment.GetEnvironmentVariable("TWITTER_CONSUMER_KEY"),
                Environment.GetEnvironmentVariable("TWITTER_CONSUMER_SECRET"),
                Environment.GetEnvironmentVariable("TWITTER_OZVAX_USER_ACCESS_TOKEN"),
                Environment.GetEnvironmentVariable("TWITTER_OZVAX_USER_ACCESS_TOKEN_SECRET")
            );
        }
        
        public Task TweetAsync(string content)
        {
            var client = new TwitterClient(_credentials);
            return client.Tweets.PublishTweetAsync(content);
        }
    }
}
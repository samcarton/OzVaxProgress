using System;
using System.Text;
using System.Threading.Tasks;
using OzVaxProgress.Models;

namespace OzVaxProgress.Services
{
    public class OzVaxProgressService
    {
        private readonly StatsService _statsService;
        private readonly StorageService _storageService;
        private readonly ProgressBarService _progressBarService;
        private readonly TweetService _tweetService;

        private static Country _australia = new () { Code = "AUS", Name = "Australia"};

        public OzVaxProgressService()
        {
            _statsService = new StatsService();
            _storageService = new StorageService();
            _progressBarService = new ProgressBarService();
            _tweetService = new TweetService();
        }

        public Task ExecuteAsync()
        {
            return ExecuteCountryAsync(_australia);
        }

        async Task ExecuteCountryAsync(Country country)
        {
            var lastUpdated = await _storageService.GetLastUpdatedAsync(country.Code);
            var vaxResponse = await _statsService.GetLatestVaccinationsByCountryAsync(country.Name);

            if (vaxResponse.Success && (lastUpdated == null || DateTime.Parse(lastUpdated.Date) < DateTime.Parse(vaxResponse.Vaccinations.Date)))
            {
                var populationResponse = await _statsService.GetPopulationByCountryCodeAsync(country.Code);
                if (populationResponse.Success == false)
                {
                    Console.WriteLine("Unable to retrieve population data, aborting.");
                    return;
                }

                var summary = new SummaryBuilder().Build(vaxResponse, populationResponse);
                var tweet = new TweetBuilder().BuildTweet(summary);
                await TweetAsync(tweet);
                Console.WriteLine($"Tweeted update for country code {country.Code}: [{vaxResponse.Vaccinations.Date},{summary}]");

                await _storageService.SetLastUpdatedAsync(new LastUpdatedModel{CountryCode = country.Code, Date = vaxResponse.Vaccinations.Date});
                Console.WriteLine($"Updated 'last updated' for {country.Code}");

                return;
            }
            
            Console.WriteLine($"No update required - vaxResponse.Success: {vaxResponse.Success}");
        }


        private Task TweetAsync(string tweet)
        {
            return _tweetService.TweetAsync(tweet);
        }

        class Country
        {
            public string Code { get; set; }
            public string Name { get; set; }
        }
    }
}
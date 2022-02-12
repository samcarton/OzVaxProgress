using System.Text;
using OzVaxProgress.Models;

namespace OzVaxProgress.Services
{
    public class TweetBuilder
    {
        private readonly ProgressBarService _progressBarService;

        public TweetBuilder()
        {
            _progressBarService = new ProgressBarService();
        }

        
        public string BuildTweet(SummaryRecord summary)
        {
            var stringBuilder = new StringBuilder($"Vax as of {summary.Date}");
            stringBuilder.AppendLine("");
            stringBuilder.AppendLine($"1💉:{_progressBarService.CreateProgressBar(summary.FirstDoseFraction)} {summary.FirstDoseFraction:P2}");
            stringBuilder.AppendLine($"2💉:{_progressBarService.CreateProgressBar(summary.SecondDoseFraction)} {summary.SecondDoseFraction:P2}");
            stringBuilder.AppendLine($"3💉:{_progressBarService.CreateProgressBar(summary.BoosterFraction)} {summary.BoosterFraction:P2}");
            stringBuilder.AppendLine("");
            stringBuilder.AppendLine("#CovidVaccine #COVID19 #COVID19Aus");
            
            return stringBuilder.ToString();
        }
        
        public string BuildSyringeTweet(double progressFraction, string date)
        {
            var stringBuilder = new StringBuilder(_progressBarService.CreateSyringeProgressBar(progressFraction));
            
            stringBuilder.AppendLine("");
            stringBuilder.AppendLine("");
            stringBuilder.AppendLine($"Fully vaccinated as of {date}: {progressFraction:P2}");
            stringBuilder.AppendLine("");
            stringBuilder.AppendLine("#CovidVaccine #COVID19 #COVID19Aus");
            
            return stringBuilder.ToString();
        }
    }
}
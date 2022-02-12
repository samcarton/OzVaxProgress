using OzVaxProgress.Models;

namespace OzVaxProgress.Services
{
    public class SummaryBuilder
    {
        public SummaryRecord Build(GetVaccinationsResponse vaxResponse, GetPopulationResponse populationResponse)
        {
            var firstDoseFraction = (vaxResponse.Vaccinations.FirstDose ?? 0) / (double)populationResponse.Population.Population;
            var secondDoseFraction = (vaxResponse.Vaccinations.SecondDose ?? 0) / (double)populationResponse.Population.Population;
            var boosterFraction = (vaxResponse.Vaccinations.Boosted ?? 0) / (double)populationResponse.Population.Population;
            return new SummaryRecord(firstDoseFraction, secondDoseFraction, boosterFraction, vaxResponse.Vaccinations.Date);
        }
    }
}
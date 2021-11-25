using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration.Attributes;
using Flurl.Http;

namespace OzVaxProgress.Services
{
    public class StatsService
    {
        public async Task<GetPopulationResponse> GetPopulationByCountryCodeAsync(string isoCountryCode)
        {
            try
            {
                await using var populationStream =
                    await "https://github.com/owid/covid-19-data/raw/master/scripts/input/un/population_latest.csv"
                        .GetStreamAsync();

                using var reader = new StreamReader(populationStream);
                using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
                foreach (var row in csv.GetRecords<PopulationCsv>())
                {
                    if (string.Equals(row.IsoCode, isoCountryCode, StringComparison.InvariantCultureIgnoreCase))
                    {
                        return new GetPopulationResponse
                        {
                            Success = true,
                            Population = new PopulationRecord(row.Population, row.Year)
                        };
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return new GetPopulationResponse {Success = false};
        }
        
        public async Task<GetVaccinationsResponse> GetLatestVaccinationsByCountryAsync(string countryName)
        {
            try
            {
                await using var vaxStream =
                    await $"https://github.com/owid/covid-19-data/raw/master/public/data/vaccinations/country_data/{countryName}.csv"
                        .GetStreamAsync();

                using var reader = new StreamReader(vaxStream);
                using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
                var latest = csv.GetRecords<VaccinationCsv>().ToList().OrderByDescending(r => r.Date).FirstOrDefault();
                if (latest != null)
                {
                    return new GetVaccinationsResponse
                    {
                        Success = true,
                        Vaccinations = new VaccinationRecord(latest.Date, latest.TotalVaccinations,
                            latest.PeopleFullyVaccinated)
                    };
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return new GetVaccinationsResponse {Success = false};
        }
    }

    public class GetPopulationResponse
    {
        public bool Success { get; set; }
        public PopulationRecord Population { get; set; }
    }
    
    public class GetVaccinationsResponse
    {
        public bool Success { get; set; }
        public VaccinationRecord Vaccinations { get; set; }
    }

    public record PopulationRecord(long Population, long Year);
    public record VaccinationRecord(string Date, long? Total, long? FullyVaccinated);

    public class PopulationCsv
    {
        [Name("entity")] public string Country { get; set; }
        [Name("iso_code")] public string IsoCode { get; set; }
        [Name("year")] public long Year { get; set; }
        [Name("population")] public long Population { get; set; }
    }

    public class VaccinationCsv
    {
        [Name("location")]
        public string Location {get;set;}
        [Name("date")]
        public string Date {get;set;}
        [Name("vaccine")]
        public string Vaccine {get;set;}
        [Name("source_url")]
        public string SourceUrl {get;set;}
        [Name("total_vaccinations")]
        public long? TotalVaccinations {get;set;}
        [Name("people_vaccinated")]
        public long? PeopleVaccinated {get;set;}
        [Name("people_fully_vaccinated")]
        public long? PeopleFullyVaccinated {get;set;}
    }
}
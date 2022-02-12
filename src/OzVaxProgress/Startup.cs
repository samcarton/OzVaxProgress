using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OzVaxProgress.Services;

namespace OzVaxProgress
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/execute/{countryCode:alpha}", async context =>
                {
                    var countryCode = (string) context.Request.RouteValues["countryCode"];
                    if (string.Equals(countryCode, "aus", StringComparison.InvariantCultureIgnoreCase) == false)
                    {
                        throw new NotSupportedException($"Country code not yet supported: {countryCode}");
                    }

                    var service = new OzVaxProgressService();
                    await service.ExecuteAsync();
                    await context.Response.WriteAsync("Executed");
                });

                #region Test endpoints

#if DEBUG

                endpoints.MapGet("/population", async context =>
                {
                    var service = new StatsService();
                    var pop = await service.GetPopulationByCountryCodeAsync("AUS");
                    var resp = pop.Success ? $"Success! Population: {pop.Population}" : "Failed to fetch population";
                    await context.Response.WriteAsync(resp);
                });

                endpoints.MapGet("/vaccinations", async context =>
                {
                    var service = new StatsService();
                    var vax = await service.GetLatestVaccinationsByCountryAsync("Australia");
                    var resp = vax.Success
                        ? $"Success! Vaccinations: {vax.Vaccinations}"
                        : "Failed to fetch vaccinations";
                    await context.Response.WriteAsync(resp);
                });

                endpoints.MapGet("/progress", async context =>
                {
                    var service = new StatsService();
                    var pop = await service.GetPopulationByCountryCodeAsync("AUS");
                    var vax = await service.GetLatestVaccinationsByCountryAsync("Australia");

                    var resp = "Failed to fetch progress";
                    if (pop.Success && vax.Success)
                    {
                        Console.WriteLine(vax.Vaccinations);
                        resp = new TweetBuilder().BuildTweet(new SummaryBuilder().Build(vax,pop));
                    }

                    await context.Response.WriteAsync(resp);
                });

                endpoints.MapGet("/progressBar", async context =>
                {
                    var response = new ProgressBarService().CreateProgressBar(0.69);
                    await context.Response.WriteAsync(response);
                });
                
                endpoints.MapGet("/progressBar/syringe", async context =>
                {
                    var syringe = new ProgressBarService().CreateSyringeProgressBar(0.69);
                    await context.Response.WriteAsync(syringe);
                });

                endpoints.MapGet("/testStorage", async context =>
                {
                    var storageService = new StorageService();
                    var emptyGet = await storageService.GetLastUpdatedAsync("NOTHING");
                    Console.WriteLine($"emptyGet: {emptyGet?.Date ?? "null"}");

                    await storageService.SetLastUpdatedAsync(new LastUpdatedModel()
                        {CountryCode = "TEST", Date = "2021-02-02"});
                    Console.WriteLine("Saved test data...");

                    var testGet = await storageService.GetLastUpdatedAsync("TEST");
                    Console.WriteLine($"testGet: {testGet?.Date ?? "null"}");

                    await context.Response.WriteAsync("OK");
                });

#endif

                #endregion
            });
        }
    }
}
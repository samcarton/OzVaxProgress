using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Google.Cloud.Firestore;

namespace OzVaxProgress.Services
{
    public class StorageService
    {
        private readonly string _projectId;
        private const string CollectionId = "vaxservicedata";

        public StorageService()
        {
            _projectId = Environment.GetEnvironmentVariable("GCP_PROJECT_ID");
        }

        public async Task<LastUpdatedModel> GetLastUpdatedAsync(string countryCode)
        {
            var db = await FirestoreDb.CreateAsync(_projectId);

            var docRef = db.Collection(CollectionId).Document(countryCode);

            var snapshot = await docRef.GetSnapshotAsync();
            if (snapshot.Exists)
            {
                return LastUpdatedModel.FromSnapshot(countryCode, snapshot.ToDictionary());
            }

            Console.WriteLine($"No data found for countryCode '{countryCode}'");
            return null;
        }
        
        public async Task SetLastUpdatedAsync(LastUpdatedModel model)
        {
            var db = await FirestoreDb.CreateAsync(_projectId);

            await db.Collection(CollectionId).Document(model.CountryCode).SetAsync( model.ToSnapshot()
                );

            Console.WriteLine($"Saved last updated date for {model.CountryCode}: {model.Date}");
        }
    }

    public class LastUpdatedModel
    {
        public LastUpdatedModel()
        {
        }

        public string CountryCode { get; set; }
        public string Date { get; set; }

        public Dictionary<string, object> ToSnapshot() => new() {{nameof(Date), Date}};
        
        public static LastUpdatedModel FromSnapshot(string countryCode, IDictionary<string,object> snapshot)
        {
            var model = new LastUpdatedModel();
            model.CountryCode = countryCode;
            if (snapshot.TryGetValue(nameof(Date), out var x))
            {
                model.Date = x.ToString();
            }

            return model;
        }
    }
}
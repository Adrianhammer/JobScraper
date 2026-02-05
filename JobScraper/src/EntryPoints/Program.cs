using JobScraper;
using Microsoft.Extensions.Configuration;

namespace JobScraper
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                //Will crash if appsettings.json is not found and won't reload if the file is changed after startup
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
                .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: false)
                .AddUserSecrets<Program>()
                .AddEnvironmentVariables()
                .Build();

            // Suggested AWS Lambda env vars (set in Lambda configuration):
            // - ConnectionStrings__DefaultConnection
            // - SmsSettings__AccountSID
            // - SmsSettings__AuthToken
            // - SmsSettings__RecipientNumber
            // - SmsSettings__SenderNumber

            var requiredKeys = new[]
            {
                "ConnectionStrings:DefaultConnection",
                "SmsSettings:AccountSID",
                "SmsSettings:AuthToken",
                "SmsSettings:RecipientNumber",
                "SmsSettings:SenderNumber"
            };
            foreach (var key in requiredKeys)
            {
                if (string.IsNullOrWhiteSpace(configuration[key]))
                    throw new InvalidOperationException($"Missing config: {key}");
            }

            var alert = new JobAlert(configuration);

            using var jobrepo = new JobRepository(configuration);
            Console.WriteLine("Database and table setup complete. Starting job fetch...");

            List<JobResponseModels.Datum> scrapedJobs = await JobFetcher.RunJobFetch();

            var newlyInsertedJobs = jobrepo.UpsertJob(scrapedJobs);
            await alert.SendNewJobAlert(newlyInsertedJobs);
        }
    }
}

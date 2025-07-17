using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;
using JobScraper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.EnvironmentVariables;

namespace jobscraper
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            string dbPath = "jobscraper.db";
            
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                //Will crash if appsettings.json is not found and won´t reload if file is changed after startup
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: false)
                .AddEnvironmentVariables()
                .Build();

            var alert = new JobAlert(configuration);

            using (JobRepository jobrepo = new JobRepository(dbPath))
            {
                Console.WriteLine("Database and table setup complete. Starting job fetch...");

                List<JobResponseModels.Datum> scrapedJobs = await JobController.RunJobFetch();

                var newlyInsertedJobs = jobrepo.UpsertJob(scrapedJobs);
                await alert.SendNewJobAlert(newlyInsertedJobs);
            }
        }
    }
}

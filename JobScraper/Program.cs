using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;
using JobScraper;

namespace jobscraper
{

    public class Program
    {
        public static async Task Main(string[] args)
        {
            string dbPath = "jobscraper.db";

            using (JobRepository jobrepo = new JobRepository(dbPath))
            {
                Console.WriteLine("Database and table setup complete. Starting job fetch...");

                List<JobResponseModels.Datum> scrapedJobs = await JobController.RunJobFetch();

                var existingJobsInDb = jobrepo.GetJobs();
                
                jobrepo.UpsertJob(scrapedJobs, existingJobsInDb);
                
            }
        }
    }
}

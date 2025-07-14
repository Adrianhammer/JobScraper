using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;

namespace JobScraper;

public class JobController
{
    public static async Task RunJobFetch()
    {
        string docPath = "/Users/adrianhammer/AA/projects/desktop/JobScraper/JobScraper/json";
        var url = "https://candidate.webcruiter.com/api/odvert/companysearch/5864";
        string requestPayload = "{Take: 20, skip: 0, page: 1, pageSize: 20, sort: [{field: '1', dir: 'desc'}]}";

        List<JobResponseModels.Datum> newlyScrapedJobs = new List<JobResponseModels.Datum>();

        using var httpClient = new HttpClient();

        var content = new StringContent(
            requestPayload,
            Encoding.UTF8,
            "application/json"
        );

        var response = await httpClient.PostAsync(url, content);

        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        
        JobResponseModels.Root? root = JsonSerializer.Deserialize<JobResponseModels.Root>(json);

        if (root?.Data != null)
        {
            foreach (var item in root.Data)
            {
                newlyScrapedJobs.Add(item);
            }
        }

        foreach (var job in newlyScrapedJobs)
        {
            //Console.WriteLine($"ID: {job.Id}, Annonse: {job.Heading}, Stilling: {job.HeadingNotOverruled}, Publisert: {job.PublishedDate}, Søknadsfrist: {job.ApplyWithinDate}, Sted: {job.Workplace} ");
        }

        Console.WriteLine(newlyScrapedJobs.Count);

       
    }
}
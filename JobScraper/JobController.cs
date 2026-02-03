using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;

namespace JobScraper;

public class JobController
{
    private static readonly HttpClient HttpClient = new HttpClient();
    // Reuse HttpClient for Lambda warm starts to avoid socket exhaustion.

    public static async Task<List<JobResponseModels.Datum>> RunJobFetch()
    {
        var url = "https://candidate.webcruiter.com/api/odvert/companysearch/5864";
        // payload the site needs to process job postings
        string requestPayload = "{Take: 20, skip: 0, page: 1, pageSize: 20, sort: [{field: '1', dir: 'desc'}]}";
        List<JobResponseModels.Datum> newlyScrapedJobs = new List<JobResponseModels.Datum>();

        var content = new StringContent(
            requestPayload,
            Encoding.UTF8,
            "application/json"
        );

        var response = await HttpClient.PostAsync(url, content);

        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        
        JobResponseModels.Root? root = JsonSerializer.Deserialize<JobResponseModels.Root>(json);

        // Adds to list, so I can check newly fetched vs stored
        if (root?.Data != null)
        {
            foreach (var item in root.Data)
            {
                newlyScrapedJobs.Add(item);
            }
        }
        return newlyScrapedJobs;
    }
}

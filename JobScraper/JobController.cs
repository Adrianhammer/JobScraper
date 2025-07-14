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

        foreach (var job in root.Data)
        {
            Console.WriteLine(job.CompanyName);
            Console.WriteLine(job.Heading);
            Console.WriteLine(job.HeadingNotOverruled);
            Console.WriteLine(job.JobCategory);
            Console.WriteLine("\n");
        }

        /*
        using (StreamWriter outputFile = new StreamWriter(Path.Combine(docPath, "result.json")))
        {
            foreach (var line in json.Split("\n"))
            {
                outputFile.WriteLine(line);
            }
        }
        */
    }
}
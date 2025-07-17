using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace JobScraper;

public class JobAlert
{
    private readonly IConfiguration _configuration;
    private static readonly HttpClient _httpClient = new HttpClient();

    public JobAlert(IConfiguration configuration)
    {
        _configuration = configuration;

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Token",
            _configuration["SmsSettings:ApiToken"]
        );

    }

    public async Task SendNewJobAlert(List<JobResponseModels.Datum> jobsToNotifyUser)
    {
        if (jobsToNotifyUser.Count == 0)
        {
            Console.WriteLine("No new jobs to notify about!");
            return;
        }
            var phoneNumber = _configuration["SmsSettings:RecipientNumber"];
            StringBuilder sb = new StringBuilder("New job postings:\n", jobsToNotifyUser.Count);

            foreach (var job in jobsToNotifyUser)
            {
                sb.Append($"- {job.HeadingNotOverruled} \n");
            }

            var messages = new
            {
                sender = _configuration["SmsSettings:Sender"],
                message = sb.ToString(),
                recipients = new[] { new { msisdn = phoneNumber } },
            };

            using var resp = await _httpClient.PostAsync(
                "https://gatewayapi.com/rest/mtsms",
                JsonContent.Create(messages)
            );

            // On 2xx, print the SMS ID´s received back from the API
            // otherwise print the response content to see the error:
            if (resp.IsSuccessStatusCode && resp.Content != null)
            {
                Console.WriteLine("success!");
                var content = await resp.Content.ReadFromJsonAsync<Dictionary<string, dynamic>>();
                foreach (var smsId in content["ids"].EnumerateArray())
                {
                    Console.WriteLine("allocated SMS id: {0:G}", smsId);
                }
            }
            else if (resp.Content != null)
            {
                Console.WriteLine("failed :(\nresponse content:");
                var content = await resp.Content.ReadAsStringAsync();
                Console.WriteLine(content);
            }
    }
}
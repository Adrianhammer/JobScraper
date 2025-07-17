using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;

namespace JobScraper;

public class JobAlert
{
    //Class purpose:
    //Method named SendNewJobAlert() needs to be triggered every time UpsertJob() method calls InsertJob()
    //Call the gatewayapi to send the user a text
    //SendNewJobAlert() needs to have a param that is a list to send a body of text
    //Telephone number can be hardcoded for now, but later add user input
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
    //Will have this as method just having no param for test
    //public async Task SendNewJobAlert(List<JobResponseModels.Datum> jobsToNotifyUser)
    public async Task SendNewJobAlert()
    {
        var phoneNumber = _configuration["SmsSettings:RecipientNumber"];
        
        var messages = new
        {
            sender = _configuration["SmsSettings:Sender"],
            message = "Test",
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
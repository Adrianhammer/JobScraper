using System.Net.Http.Headers;
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

    public void SendNewJobAlert(List<JobResponseModels.Datum> jobsToNotifyUser)
    {

        var phoneNumber = _configuration["SmsSettings:RecipientNumber"];
        
        var messages = new
        {
            sender = _configuration["SmsSettings:Sender"],
            message = "Test",
            recipients = new[] { new { msisdn = phoneNumber } },
        };
    }
}
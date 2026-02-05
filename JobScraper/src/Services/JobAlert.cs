using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using Microsoft.Extensions.Configuration;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using System.Threading.Tasks;

namespace JobScraper;

//Later changes:
//Recipient number needs to be fetched from DB for currently logged in user
public class JobAlert
{
    private readonly IConfiguration _configuration;
    public JobAlert(IConfiguration configuration)
    {
        _configuration = configuration;

        var accountSid = _configuration["SmsSettings:AccountSID"];
        var authToken = _configuration["SmsSettings:AuthToken"];
        TwilioClient.Init(accountSid, authToken);
    }

    public async Task SendNewJobAlert(List<JobResponseModels.Datum> jobsToNotifyUser)
    {
        if (jobsToNotifyUser.Count == 0)
        {
            Console.WriteLine("No new jobs to notify about!");
            return;
        }
            var recipientNumber = _configuration["SmsSettings:RecipientNumber"];
            var senderNumber = _configuration["SmsSettings:SenderNumber"];
            StringBuilder sb = new StringBuilder("New job postings:\n", jobsToNotifyUser.Count);

            foreach (var job in jobsToNotifyUser)
            {
                sb.Append($"- {job.HeadingNotOverruled} \n");
            }

            var message = await MessageResource.CreateAsync(
                body:  sb.ToString(),
                from: new Twilio.Types.PhoneNumber(senderNumber),
                to: new Twilio.Types.PhoneNumber(recipientNumber)
            );
            
            if (message.Status == MessageResource.StatusEnum.Queued ||
                message.Status == MessageResource.StatusEnum.Sending || 
                message.Status == MessageResource.StatusEnum.Sent ||
                message.Status == MessageResource.StatusEnum.Delivered)
            {
                Console.WriteLine($"WhatsApp/SMS message sent successfully! SID: {message.Sid}, Status: {message.Status}");
            }
            else
            {
                Console.WriteLine($"Message sending failed or is in an unexpected status. SID: {message.Sid}, Status: {message.Status}, Error Code: {message.ErrorCode}, Error Message: {message.ErrorMessage}");
            }
    }
}
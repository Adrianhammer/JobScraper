using Amazon.Lambda.Core;
using Microsoft.Extensions.Configuration;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace JobScraper;


public class LambdaEntryPoint
{
    // Lambda handler must be: ASSEMBLY::TYPE::METHOD
    public async Task Handler()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
            .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: false)
            .AddEnvironmentVariables()
            .Build();
        
        // Same keys enforced in Program.cs
        var requiredKeys = new[]
        {
            "ConnectionStrings:DefaultConnection",
            "SmsSettings:AccountSID",
            "SmsSettings:AuthToken",
            "SmsSettings:RecipientNumber",
            "SmsSettings:SenderNumber"
        };
        foreach (var requiredKey in requiredKeys)
        {
            if (string.IsNullOrWhiteSpace(configuration[requiredKey]))
            {
                throw new InvalidOperationException($"Missing required key {requiredKey}");
            }
        }

        var alert = new JobAlert(configuration);
        
        using var jobrepo = new JobRepository(configuration);
        Console.WriteLine("Database and table setup complete. Starting job fetch...");

        List<JobResponseModels.Datum> scrapedJobs = await JobController.RunJobFetch();

        var newlyInsertedJobs = jobrepo.UpsertJob(scrapedJobs);
        await alert.SendNewJobAlert(newlyInsertedJobs);
    }
}

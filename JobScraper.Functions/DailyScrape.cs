using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using JobScraper;

namespace JobScraper.Functions;

public class DailyScrape
{
    private readonly ILogger _logger;

    public DailyScrape(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<DailyScrape>();
    }

    [Function("DailyScrape")]
    public async Task Run([TimerTrigger("0 0 11 * * *")] TimerInfo myTimer)
    {
        _logger.LogInformation("C# Timer trigger function executed at: {executionTime}", DateTime.Now);
        
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

        List<JobResponseModels.Datum> scrapedJobs = await JobFetcher.RunJobFetch();

        var newlyInsertedJobs = jobrepo.UpsertJob(scrapedJobs);
        await alert.SendNewJobAlert(newlyInsertedJobs);
        
        _logger.LogInformation("C# Timer trigger function finished at: {executionTime}", DateTime.Now);

        
        if (myTimer.ScheduleStatus is not null)
        {
            _logger.LogInformation("Next timer schedule at: {nextSchedule}", myTimer.ScheduleStatus.Next);
        }
    }
}

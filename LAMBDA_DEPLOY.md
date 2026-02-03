# AWS Lambda Deploy Checklist (JobScraper)

This file is a short, repeatable checklist for packaging and deploying the JobScraper
to AWS Lambda using the managed .NET 10 runtime.

## 1) Prereqs (one-time)

- Install .NET 10 SDK.
- Install the Lambda tooling:

```bash
dotnet tool install -g Amazon.Lambda.Tools
```

## 2) Package the Lambda zip (every deploy)

```bash
dotnet lambda package \
  --project-location JobScraper \
  --configuration Release \
  --framework net10.0 \
  --output-package bin/Release/net10.0/JobScraper.zip
```

## 3) Create the Lambda (first time only)

- Runtime: .NET 10
- Architecture: x86_64
- Handler: JobScraper::JobScraper.LambdaEntryPoint::Handler
- Timeout: 30-60 seconds
- Memory: 512 MB (adjust as needed)
- VPC: None (needs outbound internet for scraping + Twilio)

## 4) Update code (every deploy)

- Upload the zip:
  - bin/Release/net10.0/JobScraper.zip

## 5) Environment variables (required)

- ConnectionStrings__DefaultConnection
- SmsSettings__AccountSID
- SmsSettings__AuthToken
- SmsSettings__RecipientNumber
- SmsSettings__SenderNumber

## 6) Schedule with EventBridge (first time only)

- Create a rule to invoke the Lambda on a schedule.
- Note: EventBridge cron uses UTC.
- Example:
  - rate(1 day)
  - or cron(0 12 * * ? *) for 12:00 UTC
- Oslo time note:
  - In February (CET, UTC+1), 12:00 Oslo = 11:00 UTC -> cron(0 11 * * ? *)
  - In summer (CEST, UTC+2), 12:00 Oslo = 10:00 UTC -> cron(0 10 * * ? *)
  - Consider EventBridge Scheduler with time zone Europe/Oslo to avoid DST changes

## 7) Quick smoke test

- Run a manual test in Lambda and check CloudWatch logs.
- Verify:
  - Scrape succeeds
  - Supabase insert works
  - Twilio message is sent

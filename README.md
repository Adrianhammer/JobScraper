<div align="center">
  <img src="https://github.com/user-attachments/assets/4b13bc75-dec3-482b-ba04-2e7511eca1e8" alt="JobScraper Logo" height="150" width="150">
  <h1>JobScraper</h1>
  <p>A C# job scraper with cloud-friendly deployment paths</p>
	<img alt="Static Badge" src="https://img.shields.io/badge/Language-C%23-green?style=flat">
 	<img alt="Static Badge" src="https://img.shields.io/badge/Framework-.NET-violet?style=flat">
  	<img alt="Static Badge" src="https://img.shields.io/badge/Database-Supabase-white?style=flat">
</div>

# 🚀 Overview

JobScraper automates fetching job postings from specific online sources (currently scoped to Innovasjon Norge via Webcruiter), stores new postings in Supabase PostgreSQL, and notifies via Twilio. The core logic is designed to run locally, on AWS Lambda, or on Azure Functions.

**Please note: This project is currently under active development, and more exciting features are planned!**

## ✨ Features

- Targeted web scraping from supported endpoints
- Structured parsing of job data into strong types
- Change detection and notification delivery via Twilio
- Daily scheduling on AWS Lambda or Azure Functions

# 🛠️ Tech Stack

![.NET](https://img.shields.io/badge/.NET-10-512BD4?style=flat&logo=dotnet&logoColor=white)
![C#](https://img.shields.io/badge/C%23-239120?style=flat&logo=csharp&logoColor=white)
![Supabase](https://img.shields.io/badge/Supabase-3FCF8E?style=flat&logo=supabase&logoColor=white)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-4169E1?style=flat&logo=postgresql&logoColor=white)
![Twilio](https://img.shields.io/badge/Twilio-F22F46?style=flat&logo=twilio&logoColor=white)
![AWS Lambda](https://img.shields.io/badge/AWS%20Lambda-FF9900?style=flat&logo=awslambda&logoColor=white)
![EventBridge](https://img.shields.io/badge/Amazon%20EventBridge-FF9900?style=flat&logo=amazonaws&logoColor=white)
![Azure Functions](https://img.shields.io/badge/Azure%20Functions-0062AD?style=flat&logo=azurefunctions&logoColor=white)

# 📦 Getting Started (local)

### Prerequisites

- .NET SDK 10.0 or higher installed

### Installation

1. Clone the repository:

```bash
git clone https://github.com/YourGitHubUsername/JobScraper.git
cd JobScraper
```

2. Restore NuGet packages:
```bash
dotnet restore
```

This will install the required dependencies.

### Running the Application

To run the scraper locally:

```bash
dotnet run
```
Upon execution, the application will:

1. Connect to Supabase PostgreSQL
2. Fetch job data from the configured web endpoint
3. Insert any new jobs into the Jobs table

# 🧱 Repo Structure

Core projects:
- `/Users/adrianhammer/AA/projects/desktop/JobScraper/JobScraper` (core logic)
- `/Users/adrianhammer/AA/projects/desktop/JobScraper/JobScraper.Functions` (Azure Functions wrapper)

Deployment docs:
- `/Users/adrianhammer/AA/projects/desktop/JobScraper/LAMBDA_DEPLOY.md`
- `/Users/adrianhammer/AA/projects/desktop/JobScraper/LAMBDA_CICD.md`

# ☁️ AWS Lambda Deploy (OIDC)

GitHub Actions uses OIDC to deploy a zip package to AWS Lambda:

```mermaid
flowchart LR
  A["GitHub Actions (job)"] --> B["OIDC token from GitHub"]
  B --> C["AWS STS: AssumeRoleWithWebIdentity"]
  C --> D["IAM Role: GitHubActions-Lambda-Deploy"]
  D --> E["Temporary AWS credentials"]
  E --> F["Lambda UpdateFunctionCode"]
```

# ☁️ Azure Functions Deploy (ZIP)

Azure Functions uses a timer trigger and ZIP deployment:

```mermaid
flowchart LR
  A["Azure Functions project"] --> B["dotnet publish"]
  B --> C["ZIP package"]
  C --> D["az functionapp deployment source config-zip"]
  D --> E["Function App (TimerTrigger)"]
```

See `/Users/adrianhammer/AA/projects/desktop/JobScraper/LAMBDA_DEPLOY.md` for AWS deployment steps and `/Users/adrianhammer/AA/projects/desktop/JobScraper/LAMBDA_CICD.md` for CI/CD options.

# 📄 License

Distributed under the MIT License. See ```LICENSE``` for more information.

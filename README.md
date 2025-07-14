# 🚀 Overview


JobScraper is a powerful and efficient C# application designed to automate the process of fetching job postings from specific online sources *(currently scoped in on webcruiter)*. It deserializes JSON data, extracts key job details, and intelligently stores them in a local SQLite database.

This project is built with a focus on clean architecture, maintainability, and robust data handling, serving as a practical learning experience in web scraping, data persistence, and C# best practices. 

**Please note: This project is currently under active development, and more exciting features are planned!**

## ✨ Features

- Targeted Web Scraping: Fetches job posting data from specified API endpoints.
- JSON Deserialization: Converts raw JSON responses into structured C# objects (JobResponseModels.Datum).
- Local Data Persistence: Stores job details in a lightweight SQLite database.
- Foundational Data Handling: Establishes the core for future capabilities:
	- Detecting new job postings.
	- Identifying updates to existing job postings.
	- Tracking job application deadlines.


# 🛠️ Technologies Used
- Language: C# 
- Framework: .NET (9.0)
- Database: SQLite
  
<img style="margin: 10px" src="https://profilinator.rishav.dev/skills-assets/csharp-original.svg" alt="C#" height="50" /> <img style="margin: 10px" src="https://github.com/dotnet/brand/blob/main/logo/dotnet-logo.svg" alt=".NET" height="50" /> <img style="margin: 10px" src="https://github.com/Adrianhammer/HabitTracker/blob/master/HabitTracker/Assets/sqlite-ar21.svg" alt="SqLite" height="50" /> 

# 📦 Getting Started (experimental)

### Prerequisites

- .NET SDK 9.0 or higher installed.

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

This will install Microsoft.Data.Sqlite and other necessary dependencies.

### Running the Application


To run the scraper and populate your SQLite database:

```bash
dotnet run
```
Upon execution, the application will:

1. Initialize or open the jobscraper.db SQLite database file in the project's output directory (bin/Debug/net9.0/).
2. Create the Jobs table if it doesn't already exist.
3. Fetch job data from the configured web endpoint.
4. Insert the newly scraped jobs into the Jobs table.

# 📊 Database Structure

The Jobs table in jobscraper.db is designed to store key details for each job posting:

| Column Name         | Data Type | Constraints            | Description                                   |
| :------------------ | :-------- | :--------------------- | :-------------------------------------------- |
| `id`                | `TEXT`    | `PRIMARY KEY`, `NOT NULL` | Unique identifier for the job posting.        |
| `company_name`      | `TEXT`    | `NOT NULL`             | The name of the company posting the job.      |
| `heading`           | `TEXT`    | `NOT NULL`             | The primary title or heading of the job.      |
| `job_position`      | `TEXT`    | `NULL`                 | Overruled heading or specific job position.   |
| `published_date`    | `TEXT`    | `NOT NULL`             | Date the job was originally published (ISO8601). |
| `application_deadline`| `TEXT`    | `NOT NULL`             | The simplified "Apply Within" date string (e.g., "31.07.2025"). |
| `workplace`         | `TEXT`    | `NOT NULL`             | Location or primary workplace of the job.     |
| `open_advert_url`   | `TEXT`    | `NOT NULL`             | Direct URL to the job advertisement.          |

# 🗺️ Project Roadmap

- Implement logic for checking existing jobs in the database to prevent duplicates.
- Add logic to identify and update changed job postings (e.g., updated deadlines, new details).
- Develop a scheduling mechanism (e.g., using System.Timers.Timer or a background service) for automatic, periodic job fetching.
- Explore advanced comparison strategies for robust state matching.
- Implement notification system for new or updated jobs.

# 📄 License

Distributed under the MIT License. See ```LICENSE``` for more information.

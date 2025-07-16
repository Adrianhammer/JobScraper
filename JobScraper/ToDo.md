## JobScraper To-Do List

### 🚀 Immediate Focus

- [ ]  **Refactor `InsertJob` method:**
    - [ ]  Change `InsertJob` to accept and insert a *single* `JobResponseModels.Datum` object (not a list).
    - [ ]  Ensure all parameters in the `command.Parameters.AddWithValue` calls correctly reference the *properties* of that single `Datum` object (e.g., `job.Id`, `job.CompanyName`).

- [ ]  **Improve `UpsertJob` comparison logic:**
    - [ ]  Inside `UpsertJob`, iterate through `newJobs`.
    - [ ]  For *each* `newJob`, efficiently check if its `Id` already exists within the `storedJobs` list.
        - *Hint:* Consider converting `storedJobs` into a data structure optimized for fast lookups by ID (e.g., a hash-based collection).
    - [ ]  If a `newJob.Id` does NOT exist in `storedJobs`, add that `newJob` to a new `List<JobResponseModels.Datum>` (e.g., `jobsToInsert`).
    - [ ]  After the loop, iterate through `jobsToInsert` and call the (now correctly refactored) `InsertJob(singleJob)` for each item.

### 💡 Next Steps (Post-Upsert Logic)

- [ ]  Refine handling of existing jobs that might have *changed* (not just new ones).
    - *Consider:* Updating existing records if certain fields are different (e.g., `ApplicationDeadline`).
- [ ]  Implement cleanup for old/expired jobs in the database if desired (e.g., jobs no longer appearing in the scrape).
- [ ]  Add proper logging instead of `Console.WriteLine` for errors and status messages (e.g., using a library like Serilog).

### ✅ Done (as of today)

- [x]  Set up `JobController` to fetch and return `List<JobResponseModels.Datum>`.
- [x]  Created `JobRepository` for database interactions.
- [x]  Implemented `CreateTable` method.
- [x]  Implemented `GetJobs` method to retrieve all stored jobs as `List<JobResponseModels.Datum>`.
- [x]  Ensured `JobRepository` uses `IDisposable` for proper connection management.
- [x]  Improved efficiency by calling `GetJobs()` once in `Main`.
- [x]  Integrated database connection setup into `JobRepository` constructor.
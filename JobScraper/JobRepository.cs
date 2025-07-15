using System.Data;
using Microsoft.Data.Sqlite;

namespace JobScraper;

public class JobRepository : IDisposable
{
    private readonly SqliteConnection _sqliteConn;

    public JobRepository(string databasePath)
    {
        _sqliteConn = new SqliteConnection($"Data Source={databasePath};");
        try
        {
            _sqliteConn.Open();
            Console.WriteLine("SQLite connection established");
            CreateTable();
        }
        catch (SqliteException ex)
        {
            Console.WriteLine($"Error opening or creating SQLite database: {ex.Message}");
            throw;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected error: {ex.Message}");
            throw;
        }
    }

    private void CreateTable()
    {
        using (SqliteCommand command = _sqliteConn.CreateCommand())
        {
            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS Jobs (
                    id TEXT PRIMARY KEY NOT NULL,
                    company_name TEXT NOT NULL,
                    heading TEXT NOT NULL,
                    job_position TEXT NULL,
                    published_date TEXT NOT NULL,
                    application_deadline TEXT NOT NULL,
                    workplace TEXT NOT NULL,
                    open_advert_url TEXT NOT NULL
                );";
            try
            {
                command.ExecuteNonQuery();
                Console.WriteLine("Jobs table created successfully or already exists");
            }
            catch (SqliteException ex)
            {
                Console.WriteLine($"Error creating Jobs table: {ex.Message}");
                throw;
            }
        }
    }
    
    //To be used later for notifying etc.
    public List<JobResponseModels.Datum> GetJobs()
    {
        List<JobResponseModels.Datum> getStoredJobs = new List<JobResponseModels.Datum>();
        
        using (SqliteCommand command = _sqliteConn.CreateCommand())
        {
            command.CommandText = @"SELECT * FROM Jobs";
            
            using (SqliteDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    JobResponseModels.Datum job = new JobResponseModels.Datum();
                    job.Id = reader.GetString("ID");
                    job.CompanyName = reader.GetString("company_name");
                    job.Heading = reader.GetString("heading");
                    job.HeadingNotOverruled = reader.GetString("job_position");
                    job.PublishedDate = reader.GetString("published_date");
                    job.ApplyWithinDate = reader.GetString("application_deadline");
                    job.Workplace = reader.GetString("workplace");
                    job.OpenAdvertUrl = reader.GetString("open_advert_url");
                    getStoredJobs.Add(job);
                }
            }
        }
        return getStoredJobs;
    }
    
    public void InsertJob(JobResponseModels.Datum job, List<JobResponseModels.Datum> jobs)
    {
        try
        {
            using (SqliteCommand command = _sqliteConn.CreateCommand())
            {
                command.CommandText = @"
                INSERT INTO Jobs (id, company_name, heading, job_position,  published_date, application_deadline, workplace, open_advert_url)
                VALUES (@id, @company_name, @heading, @job_position, @published_date, @application_deadline, @workplace, @open_advert_url);";
                command.Parameters.AddWithValue("@id", job.Id);
                command.Parameters.AddWithValue("@company_name", job.CompanyName);
                command.Parameters.AddWithValue("@heading", job.Heading);
                command.Parameters.AddWithValue("@job_position", job.HeadingNotOverruled);
                command.Parameters.AddWithValue("@published_date", job.PublishedDate);
                command.Parameters.AddWithValue("@application_deadline", job.ApplyWithinDate);
                command.Parameters.AddWithValue("@workplace", job.Workplace);
                command.Parameters.AddWithValue("@open_advert_url", job.OpenAdvertUrl);
                command.ExecuteNonQuery();
            } 
        }
        catch (SqliteException ex)
        {
            Console.WriteLine($"Error inserting job: {ex.Message}");
            throw;
        }
    }
    
    //Create method that compares the newly retrieved job postings (JobController.newlyScrapedJobs list) to what is stored in the database (JobRepository.getStoredJobs list)
    //A method that compares list items
    //Method needs two list parameters for the two lists
    //Create a list that will hold the job postings that do not exist
    //If a list item has equal newjobs.ID -> do not add that item to the list -> go to next item -> check again -> if it has not equal newjobs.ID -> add item to the list
    //After a loop is run that checks for equality between the lists, and the job postings are added
    //Insert those job postings to the database by calling InsertJob() method sending the newly created list as a param (List<JobResponseModels.Datum> jobs)

    public void UpsertJob(List<JobResponseModels.Datum> newJobs, List<JobResponseModels.Datum> storedJobs)
    {
        int i = 0;
        List<JobResponseModels.Datum> checkedJobs = new List<JobResponseModels.Datum>();

        try
        {
            foreach (JobResponseModels.Datum newJob in newJobs)
            {
                if (newJob.Id.Contains(storedJobs[i].Id))
                {
                    Console.WriteLine($"New job {newJob.Id} already exists");
                    i++;
                }
                else
                {
                    Console.WriteLine($"New job {newJob.Id} added");
                    checkedJobs.Add(newJob);
                }
            }
            InsertJob();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        
        //testing to see what is in thew checkJobs list
        int j = 1;
        foreach (var checkedJob in checkedJobs)
        {
            Console.WriteLine($"checkedJobs list item {j}: {checkedJob.Id}");
            j++;
        }
    }

    public void Dispose()
    {
        if (_sqliteConn != null && _sqliteConn.State == ConnectionState.Open)
        {
            Console.WriteLine("Closing SQLite connection");
            _sqliteConn.Close();
            _sqliteConn.Dispose();
        }
    }
}
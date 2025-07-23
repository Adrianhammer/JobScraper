using System.Data;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace JobScraper;

public class JobRepository : IDisposable
{
    private readonly NpgsqlConnection _pgConn;

    public JobRepository(IConfiguration configuration)
    {
        var connString = configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrEmpty(connString))
            throw new InvalidOperationException("Connection string is missing");
        
        _pgConn = new NpgsqlConnection(connString);
        try
        {
            _pgConn.Open();
            Console.WriteLine("Supabase PostgreSQL connection established");
            CreateTable();
        }
        catch (NpgsqlException ex)
        {
            Console.WriteLine($"Error opening or creating Supabase PostgreSQL connection: {ex.Message}");
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
        using var command = _pgConn.CreateCommand();
        command.CommandText = @"
                CREATE TABLE IF NOT EXISTS Jobs (
                  id text PRIMARY KEY NOT NULL,
                  company_name text,
                  heading text,
                  job_position text,
                  published_date text,
                  application_deadline text,
                  workplace text,
                  open_advert_url text
                );";
            try
            {
                command.ExecuteNonQuery();
                Console.WriteLine("Jobs table created successfully or already exists");
            }
            catch (NpgsqlException ex)
            {
                Console.WriteLine($"Error creating Jobs table: {ex.Message}");
                throw;
            }
    }
    
    public List<JobResponseModels.Datum> GetJobs()
    {
        List<JobResponseModels.Datum> allStoredJobs = new List<JobResponseModels.Datum>();

        using var command = _pgConn.CreateCommand();
        command.CommandText = "SELECT * FROM Jobs";
        
        using var reader = command.ExecuteReader();
        
        while (reader.Read())
        {
            var job = new JobResponseModels.Datum
            {
                Id = reader.GetString(reader.GetOrdinal("id")),
                CompanyName = reader.GetString(reader.GetOrdinal("company_name")),
                Heading = reader.GetString(reader.GetOrdinal("heading")),
                HeadingNotOverruled = reader.IsDBNull(reader.GetOrdinal("job_position")) ? null : reader.GetString(reader.GetOrdinal("job_position")),
                PublishedDate = reader.GetString(reader.GetOrdinal("published_date")),
                ApplyWithinDate = reader.GetString(reader.GetOrdinal("application_deadline")),
                Workplace = reader.GetString(reader.GetOrdinal("workplace")),
                OpenAdvertUrl = reader.GetString(reader.GetOrdinal("open_advert_url"))
            };
            allStoredJobs.Add(job);
        }
        return allStoredJobs;
    }
    
    public void InsertJob(JobResponseModels.Datum job)
    {
        try
        {
            using var command = _pgConn.CreateCommand();
            
            command.CommandText = @"
            INSERT INTO Jobs (id, company_name, heading, job_position,  published_date, application_deadline, workplace, open_advert_url)
            VALUES (@id, @company_name, @heading, @job_position, @published_date, @application_deadline, @workplace, @open_advert_url);";
            
            command.Parameters.AddWithValue("@id", job.Id);
            command.Parameters.AddWithValue("@company_name", job.CompanyName);
            command.Parameters.AddWithValue("@heading", job.Heading);
            command.Parameters.AddWithValue("@job_position", (object?)job.HeadingNotOverruled ?? DBNull.Value);            command.Parameters.AddWithValue("@published_date", job.PublishedDate);
            command.Parameters.AddWithValue("@application_deadline", job.ApplyWithinDate);
            command.Parameters.AddWithValue("@workplace", job.Workplace);
            command.Parameters.AddWithValue("@open_advert_url", job.OpenAdvertUrl);
            
            command.ExecuteNonQuery();
        }
        catch (NpgsqlException ex)
        {
            Console.WriteLine($"Error inserting job: {ex.Message}");
            throw;
        }
    }
    
    public List<JobResponseModels.Datum> UpsertJob(List<JobResponseModels.Datum> scrapedJobs)
    {
        List<JobResponseModels.Datum> jobsToNotifyUser = new List<JobResponseModels.Datum>();
        
        try
        {
            foreach (JobResponseModels.Datum job in scrapedJobs)
            {
                if (JobExists(job.Id) == false)
                {
                    Console.WriteLine($"Added job: {job.HeadingNotOverruled}");
                    InsertJob(job);
                    jobsToNotifyUser.Add(job);
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        
        return jobsToNotifyUser;
    }

    public bool JobExists(string jobId)
    {
        try
        {
            using var command = _pgConn.CreateCommand();
            command.CommandText = @"SELECT COUNT(*) FROM Jobs WHERE id = @jobId";
            command.Parameters.AddWithValue("@jobId", jobId);
            
            long id = (long) command.ExecuteScalar();
            return id > 0;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public void Dispose()
    {
        if (_pgConn?.State == ConnectionState.Open)
        {
            Console.WriteLine("Closing Supabase connection");
            _pgConn.Close();
            _pgConn.Dispose();
        }
    }
}
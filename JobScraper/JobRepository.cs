using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

    public void InsertJob(JobResponseModels.Datum job)
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

    public void Dispose()
    {
        if (_sqliteConn != null && _sqliteConn.State == System.Data.ConnectionState.Open)
        {
            Console.WriteLine("Closing SQLite connection");
            _sqliteConn.Close();
            _sqliteConn.Dispose();
        }
    }
}
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
        //todo
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
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;
using JobScraper;

namespace jobscraper
{
    
    public class Program
    {
        public static async Task Main(string[] args)
        {
            await JobController.RunJobFetch();
        }
    }
}
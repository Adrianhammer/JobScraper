using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

public class Program
{
    public static async Task Main()
    {
        var url = "https://candidate.webcruiter.com/api/odvert/companysearch/5864";
        string requestPayload = "{Take: 20, skip: 0, page: 1, pageSize: 20, sort: [{field: '1', dir: 'desc'}]}";

        using var httpClient = new HttpClient();

        var content = new StringContent(requestPayload, Encoding.UTF8, "application/json");
        var response = await httpClient.PostAsync(url, content);

        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();

        Console.WriteLine(json);
    }
}
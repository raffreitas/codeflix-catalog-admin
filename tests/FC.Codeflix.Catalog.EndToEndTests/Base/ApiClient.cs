using System.Text;
using System.Text.Json;

namespace FC.Codeflix.Catalog.EndToEndTests.Base;
public class ApiClient(HttpClient httpClient)
{
    public async Task<(HttpResponseMessage?, TOutput?)> Post<TOutput>(string route, object payload)
    {
        var response = await httpClient.PostAsync(
            route,
            new StringContent(
                JsonSerializer.Serialize(payload),
                Encoding.UTF8,
                "application/json"
            )
        );
        var output = await GetOutput<TOutput>(response);
        return (response, output);
    }

    public async Task<(HttpResponseMessage?, TOutput?)> Put<TOutput>(string route, object payload)
    {
        var response = await httpClient.PutAsync(
            route,
            new StringContent(
                JsonSerializer.Serialize(payload),
                Encoding.UTF8,
                "application/json"
            )
        );
        var output = await GetOutput<TOutput>(response);
        return (response, output);
    }

    public async Task<(HttpResponseMessage? response, TOutput? output)> Get<TOutput>(string route)
    {
        var response = await httpClient.GetAsync(route);
        var output = await GetOutput<TOutput>(response);
        return (response, output);
    }

    public async Task<(HttpResponseMessage? response, TOutput? output)> Delete<TOutput>(string route)
    {
        var response = await httpClient.DeleteAsync(route);
        var output = await GetOutput<TOutput>(response);
        return (response, output);
    }

    private static async Task<TOutput?> GetOutput<TOutput>(HttpResponseMessage responseMessage)
    {
        var outputString = await responseMessage.Content.ReadAsStringAsync();

        TOutput? output = default;

        if (!string.IsNullOrWhiteSpace(outputString))
            output = JsonSerializer.Deserialize<TOutput>(
                outputString,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }
            );

        return output;
    }
}

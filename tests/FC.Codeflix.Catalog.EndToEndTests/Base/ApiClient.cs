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

        var outputString = await response.Content.ReadAsStringAsync();

        TOutput? output = default;

        if (!string.IsNullOrWhiteSpace(outputString))
            output = JsonSerializer.Deserialize<TOutput>(
                outputString,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }
            );

        return (response, output);
    }

    public async Task<(HttpResponseMessage? response, TOutput? output)> Get<TOutput>(string route)
    {
        var response = await httpClient.GetAsync(route);
        var outputString = await response.Content.ReadAsStringAsync();

        TOutput? output = default;
        if (!string.IsNullOrWhiteSpace(outputString))
            output = JsonSerializer.Deserialize<TOutput>(
                outputString,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }
            );

        return (response, output);
    }

    public async Task<(HttpResponseMessage? response, TOutput? output)> Delete<TOutput>(string route)
    {
        var response = await httpClient.DeleteAsync(route);
        var outputString = await response.Content.ReadAsStringAsync();

        TOutput? output = default;
        if (!string.IsNullOrWhiteSpace(outputString))
            output = JsonSerializer.Deserialize<TOutput>(
                outputString,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }
            );

        return (response, output);
    }
}

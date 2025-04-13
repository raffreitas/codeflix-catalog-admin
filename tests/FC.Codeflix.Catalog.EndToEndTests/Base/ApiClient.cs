using System.Text;
using System.Text.Json;

using Microsoft.AspNetCore.WebUtilities;

namespace FC.Codeflix.Catalog.EndToEndTests.Base;
public class ApiClient(HttpClient httpClient)
{
    private static readonly JsonSerializerOptions DefaultSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        PropertyNameCaseInsensitive = true
    };

    public async Task<(HttpResponseMessage?, TOutput?)> Post<TOutput>(string route, object payload)
    {
        var response = await httpClient.PostAsync(
            route,
            new StringContent(
                JsonSerializer.Serialize(payload, DefaultSerializerOptions),
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
                JsonSerializer.Serialize(payload, DefaultSerializerOptions),
                Encoding.UTF8,
                "application/json"
            )
        );
        var output = await GetOutput<TOutput>(response);
        return (response, output);
    }

    public async Task<(HttpResponseMessage? response, TOutput? output)> Get<TOutput>(
        string route,
        object? queryStringObject = null)
    {
        var newRoute = PrepareGetRoute(route, queryStringObject);
        var response = await httpClient.GetAsync(newRoute);
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
            output = JsonSerializer.Deserialize<TOutput>(outputString, DefaultSerializerOptions);

        return output;
    }

    private static string PrepareGetRoute(string route, object? queryStringObject)
    {
        if (queryStringObject is null)
            return route;

        var parametersJson = JsonSerializer.Serialize(queryStringObject, DefaultSerializerOptions);
        var parametersDict = Newtonsoft.Json.JsonConvert
            .DeserializeObject<Dictionary<string, string>>(parametersJson);

        return QueryHelpers.AddQueryString(route, parametersDict!);
    }
}

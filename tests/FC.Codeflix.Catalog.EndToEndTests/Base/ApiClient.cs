using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

using FC.Codeflix.Catalog.Application.UseCases.Videos.Common;

using Keycloak.AuthServices.Authentication;

using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;

using Newtonsoft.Json;

using JsonSerializer = System.Text.Json.JsonSerializer;

namespace FC.Codeflix.Catalog.EndToEndTests.Base;

public class ApiClient
{
    private static readonly JsonSerializerOptions DefaultSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower, PropertyNameCaseInsensitive = true
    };

    private readonly HttpClient _httpClient;
    private readonly KeycloakAuthenticationOptions _keycloakOptions;
    private readonly IConfiguration _configuration;
    private readonly string _tokenEndpoint;

    public ApiClient(HttpClient httpClient, KeycloakAuthenticationOptions keycloakOptions, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _keycloakOptions = keycloakOptions ?? throw new ArgumentNullException(nameof(keycloakOptions));
        _configuration = configuration;

        var realm = !string.IsNullOrWhiteSpace(_keycloakOptions?.Realm)
            ? _keycloakOptions.Realm
            : _configuration["Keycloak:realm"];
        var authServerUrl = !string.IsNullOrWhiteSpace(_keycloakOptions?.AuthServerUrl)
            ? _keycloakOptions.AuthServerUrl
            : _configuration["Keycloak:auth-server-url"];

        if (string.IsNullOrWhiteSpace(authServerUrl) || string.IsNullOrWhiteSpace(realm))
            throw new InvalidOperationException("Keycloak configuration is missing 'auth-server-url' or 'realm'.");

        _tokenEndpoint = $"{authServerUrl.TrimEnd('/')}/realms/{realm!.Trim('/')}/protocol/openid-connect/token";
        AddAuthorizationHeader();
    }

    private const string AdminUser = "admin";
    private const string AdminPassword = "123456";

    private void AddAuthorizationHeader()
    {
        var accessToken = GetAccessTokenAsync(AdminUser, AdminPassword).GetAwaiter().GetResult();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
    }

    public async Task<string> GetAccessTokenAsync(string user, string password)
    {
        var client = new HttpClient();
        var request = new HttpRequestMessage(HttpMethod.Post, _tokenEndpoint);
        var collection = new List<KeyValuePair<string, string>>
        {
            new("grant_type", "password"),
            new("client_id", !string.IsNullOrWhiteSpace(_keycloakOptions?.Resource)
                ? _keycloakOptions.Resource
                : _configuration["Keycloak:resource"] ?? string.Empty),
            new("client_secret", _keycloakOptions?.Credentials?.Secret
                                 ?? _configuration["Keycloak:credentials:secret"] ?? string.Empty),
            new("username", user),
            new("password", password)
        };
        var content = new FormUrlEncodedContent(collection);
        request.Content = content;
        var response = await client.SendAsync(request);
        var credentials = await GetOutput<Credentials>(response);
        return credentials!.AccessToken;
    }

    public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request) =>
        await _httpClient.SendAsync(request);

    public async Task<(HttpResponseMessage?, TOutput?)> Post<TOutput>(string route, object payload)
    {
        var response = await _httpClient.PostAsync(
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
        var response = await _httpClient.PutAsync(
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
        var response = await _httpClient.GetAsync(newRoute);
        var output = await GetOutput<TOutput>(response);
        return (response, output);
    }

    public async Task<(HttpResponseMessage? response, TOutput? output)> Delete<TOutput>(string route)
    {
        var response = await _httpClient.DeleteAsync(route);
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
        var parametersDict = JsonConvert
            .DeserializeObject<Dictionary<string, string>>(parametersJson);

        return QueryHelpers.AddQueryString(route, parametersDict!);
    }

    internal async Task<(HttpResponseMessage?, TOutput?)> PostFormData<TOutput>(string route, FileInput file)
        where TOutput : class
    {
        var fileContent = new StreamContent(file.FileStream);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);

        using var content = new MultipartFormDataContent();
        content.Add(fileContent, "media_file", $"media.{file.Extension}");
        var response = await _httpClient.PostAsync(route, content);
        var output = await GetOutput<TOutput>(response);
        return (response, output);
    }
}
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Gemini_Messenger;

internal sealed class GeminiClient : IDisposable
{
    private readonly HttpClient httpClient = new()
    {
        DefaultRequestHeaders =
        {
            CacheControl = new() { NoCache = true },
        },
        BaseAddress = new("https://api.gemini.com"),
    };

    private readonly ReadOnlyMemory<byte> apiSecret;

    public GeminiClient(string apiKey, string apiSecret)
    {
        httpClient.DefaultRequestHeaders.Add("X-GEMINI-APIKEY", apiKey);

        this.apiSecret = Encoding.ASCII.GetBytes(apiSecret);
    }

    public void Dispose() => httpClient.Dispose();

    public async Task<Result<string>> SendAsync(string message)
    {
        JsonNode? json;
        try
        {
            json = JsonNode.Parse(message, documentOptions: new() { AllowTrailingCommas = true });
        }
        catch (JsonException ex)
        {
            return Result.Error("The message is not valid JSON. " + ex.Message);
        }

        if ((string?)json?["request"] is not { Length: > 0 } endPoint)
            return Result.Error("Please specify the \"request\" property.");

        if (endPoint[0] != '/')
            return Result.Error("The \"request\" property value must begin with a forward slash (/).");

        json["nonce"] = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        var payload = Convert.ToBase64String(Encoding.UTF8.GetBytes(json.ToJsonString()));
        var signature = Convert.ToHexString(HMACSHA384.HashData(key: apiSecret.Span, source: Encoding.ASCII.GetBytes(payload)));

        using var request = new HttpRequestMessage(HttpMethod.Post, endPoint);
        request.Headers.Add("X-GEMINI-PAYLOAD", payload);
        request.Headers.Add("X-GEMINI-SIGNATURE", signature);

        using var response = await httpClient.SendAsync(request);
        return Result.Success(await response.Content.ReadAsStringAsync());
    }
}

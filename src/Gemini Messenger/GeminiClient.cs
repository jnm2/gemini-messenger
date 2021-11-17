using System.Security.Cryptography;
using System.Text;
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

    public async Task<string> SendAsync(string message)
    {
        var json = JsonNode.Parse(message);

        if ((string?)json?["request"] is not { } endPoint)
        {
            throw new NotImplementedException("Error handling");
        }

        json["nonce"] = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        var payload = Convert.ToBase64String(Encoding.UTF8.GetBytes(json.ToJsonString()));
        var signature = Convert.ToHexString(HMACSHA384.HashData(key: apiSecret.Span, source: Encoding.ASCII.GetBytes(payload)));

        using var request = new HttpRequestMessage(HttpMethod.Post, endPoint);
        request.Headers.Add("X-GEMINI-PAYLOAD", payload);
        request.Headers.Add("X-GEMINI-SIGNATURE", signature);

        using var response = await httpClient.SendAsync(request);
        return await response.Content.ReadAsStringAsync();
    }
}

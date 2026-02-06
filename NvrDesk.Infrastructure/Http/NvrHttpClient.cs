namespace NvrDesk.Infrastructure.Http;

public sealed class NvrHttpClient(HttpClient httpClient)
{
    public async Task<bool> ProbeAsync(Uri uri, CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, uri);
        using var response = await httpClient.SendAsync(request, cancellationToken);
        return response.IsSuccessStatusCode;
    }
}

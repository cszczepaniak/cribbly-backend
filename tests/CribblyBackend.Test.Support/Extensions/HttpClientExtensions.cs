using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace CribblyBackend.Test.Support.Extensions
{
    public static class HttpClientExtensions
    {
        public static async Task<HttpResponseMessage> PatchAsJsonAsync<T>(this HttpClient client, string uri, T content)
        {
            return await client.PatchAsync(uri, JsonContent.Create(content));
        }
    }
}
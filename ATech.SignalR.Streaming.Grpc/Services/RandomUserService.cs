using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ATech.SignalR.Streaming.Grpc.Models;
using Microsoft.Extensions.Logging;

namespace ATech.SignalR.Streaming.Grpc.Services
{
    public interface IRandomUserService
    {
        Task<RandomUser> GetUser(CancellationToken cancellationToken);
    }

    public class RandomUserService : IRandomUserService
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly ILogger<RandomUserService> logger;
        private readonly JsonSerializerOptions options;

        public RandomUserService(IHttpClientFactory httpClientFactory,
                                 ILogger<RandomUserService> logger)
        {
            this.httpClientFactory = httpClientFactory;
            this.logger = logger;

            options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        public async Task<RandomUser> GetUser(CancellationToken cancellationToken = default(CancellationToken))
        {
            using var client = httpClientFactory.CreateClient();
            using var response = await client.GetAsync("https://randomuser.me/api/", HttpCompletionOption.ResponseHeadersRead);

            response.EnsureSuccessStatusCode();

            using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);

            var randomUser = await JsonSerializer.DeserializeAsync<RandomUser>(stream, options);

            return randomUser;
        }
    }
}

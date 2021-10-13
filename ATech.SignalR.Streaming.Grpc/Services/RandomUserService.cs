using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ATech.SignalR.Streaming.Grpc.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace ATech.SignalR.Streaming.Grpc.Services
{
    public record User
    {
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public interface IRandomUserService
    {
        Task<User> GetUser(CancellationToken cancellationToken);
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

        public async Task<User> GetUser(CancellationToken cancellationToken = default(CancellationToken))
        {
            using var client = httpClientFactory.CreateClient();
            var response = await client.GetStringAsync("https://randomuser.me/api/");

            var parsed = JObject.Parse(response);
            var results = parsed["results"];
            var name = results[0]["name"];
            return new User
            {
                FirstName = name["first"].ToString(),
                LastName = name["last"].ToString(),
                Title = name["title"].ToString(),
            };
        }
    }
}
